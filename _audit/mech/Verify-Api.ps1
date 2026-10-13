# 机制研发部：把 Source 里出现的「类型.成员」引用逐个对游戏程序集做存在性核验
$ErrorActionPreference = 'Stop'
$root    = 'E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF'
$managed = 'E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed'
$out     = Join-Path $root '_audit\mech\apiverify.txt'
$errLog  = New-Object Collections.Generic.List[string]

# ---- 0) 依赖解析钩子（缺任一依赖会让 GetTypes 抛 ReflectionTypeLoadException） ----
$resolver = [System.ResolveEventHandler]{
    param($s, $e)
    $simple = ($e.Name -split ',')[0]
    $cand = Join-Path $global:managedDir ($simple + '.dll')
    if (Test-Path $cand) { return [Reflection.Assembly]::LoadFrom($cand) }
    return $null
}
[System.AppDomain]::CurrentDomain.add_AssemblyResolve($resolver)

# ---- 1) 载入游戏程序集 ----
$global:managedDir = $managed
$asms = New-Object Collections.Generic.List[object]
foreach ($n in @('Assembly-CSharp.dll','UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.IMGUIModule.dll','UnityEngine.TextRenderingModule.dll')) {
    $p = Join-Path $managed $n
    if (Test-Path $p) {
        try { $asms.Add([Reflection.Assembly]::LoadFrom($p)) }
        catch { $errLog.Add("load fail: $n -> $($_.Exception.Message)") }
    }
}
Get-ChildItem $managed -Filter 'UnityEngine*.dll' | ForEach-Object {
    try { $asms.Add([Reflection.Assembly]::LoadFrom($_.FullName)) }
    catch { $errLog.Add("load fail: $($_.Name)") }
}

# ---- 2) 建「短名 -> 成员名集合」索引 ----
$shortMap  = @{}
$typeIndex = @{}
foreach ($a in $asms) {
    $ts = $null
    try { $ts = $a.GetTypes() }
    catch {
        $ts = @()
        foreach ($x in $_.Exception.Types) { if ($x -ne $null) { $ts += $x } }
    }
    foreach ($t in $ts) {
        if ($t -eq $null) { continue }
        $n = $t.Name
        if (-not $typeIndex.ContainsKey($n)) {
            $typeIndex[$n] = 0
            $shortMap[$n]  = New-Object 'System.Collections.Generic.HashSet[string]'
        }
        $typeIndex[$n] = $typeIndex[$n] + 1
        $set = $shortMap[$n]
        $flags = [Reflection.BindingFlags]::Public -bor [Reflection.BindingFlags]::Static -bor [Reflection.BindingFlags]::Instance -bor [Reflection.BindingFlags]::DeclaredOnly
        foreach ($m in $t.GetMembers($flags)) { [void]$set.Add($m.Name) }
        foreach ($nt in $t.GetNestedTypes()) { [void]$set.Add($nt.Name) }
    }
}

# ---- 3) RUIF 自有前缀（跳过） ----
$ruifPrefixes = New-Object 'System.Collections.Generic.HashSet[string]'
$countLines = Get-Content (Join-Path $root '_audit\mech\api_count.txt')
foreach ($ln in $countLines) {
    if ($ln -match "`t") {
        $full = ($ln -split "`t")[0]
        $segs = $full -split '\.'
        [void]$ruifPrefixes.Add($segs[$segs.Count - 1])
    }
}
foreach ($extra in @('Tr','TrCache','Measure','Api','Tokens','Px','Layout','Draw','Atoms','TexCache','TexFmt','TexAtoms','IntKey','VStack','HStack','Grid','Fmt')) {
    [void]$ruifPrefixes.Add($extra)
}

# ---- 4) 逐个核验 ----
$lines = New-Object Collections.Generic.List[string]
$pairs = Get-Content (Join-Path $root '_audit\mech\pairs.txt')
$checked = 0
foreach ($pair in $pairs) {
    $parts = $pair -split '\.'
    if ($parts.Count -ne 2) { continue }
    $ty = $parts[0]; $mem = $parts[1]
    if ($ruifPrefixes.Contains($ty)) { continue }
    if ($ty -eq 'System') { continue }
    $checked = $checked + 1
    if (-not $shortMap.ContainsKey($ty)) { $lines.Add("UNRESOLVED-TYPE   $pair"); continue }
    if (-not $shortMap[$ty].Contains($mem)) { $lines.Add("UNRESOLVED-MEMBER $pair   (同名类型数=$($typeIndex[$ty]))"); continue }
}

# ---- 5) 三层引用 ----
$triples = Get-Content (Join-Path $root '_audit\mech\triples.txt')
foreach ($tri in $triples) {
    $parts = $tri -split '\.'
    if ($parts.Count -ne 3) { continue }
    if ($ruifPrefixes.Contains($parts[0])) { continue }
    if ($parts[0] -eq 'RUIF') { continue }
    if ($parts[0] -eq 'Find') {
        if (-not $shortMap.ContainsKey($parts[1])) { $lines.Add("UNRESOLVED-NESTED $tri") }
    }
}

$header = @("pairs total   : $($pairs.Count)",
            "pairs checked : $checked  (已剔除 RUIF 自有类型)",
            "types indexed : $($shortMap.Count)",
            "errlog        : $($errLog.Count)",
            "--- 未解析条目如下（下方为空 = 全部解析成功）---")
[IO.File]::WriteAllLines($out, ($header + $lines), (New-Object Text.UTF8Encoding($false)))
