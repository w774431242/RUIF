<#
.SYNOPSIS
  RUIF 发布门禁：一致性 / 引用面 / 本地化 / 发布区卫生 全项校验（发布前必跑）

.DESCRIPTION
  九道检查（任一 FAIL 即退出码 1，禁止部署）：
   1 版本一致性      —— 构建产物 dll 内的 Api.VersionString == RUIF About modVersion
   2 程序集引用白名单 —— 只允许引擎允许集；**禁止 0Harmony**（框架红线）
   3 两区哈希一致    —— 开发区框架 dll 与发布区 dll 逐文件 Get-FileHash 比对
   4 本地化键对称    —— Languages/{ChineseSimplified,English} 键集合逐项比对（双向）
   5 代码翻译键覆盖  —— 源码里翻译键字面量的每个键都在两份语言文件中存在
   6 发布区卫生      —— 无 *.bak_* / nul 残留；镜像一致（开发区已删除的文件不得残留在发布区）
   7 开发者包        —— DevDocs 必备文件齐备 + BUILD-INFO 版本一致 + 发布区已同步
   8 XML 良构        —— About / Languages / Patches 全量 + About 必备字段
   9 占位符与重复键  —— 中英同键占位符一致（LOC-004）；XML 无重复键（LOC-005）

  用法：pwsh -File RUIF\_scripts\verify_release.ps1   （先编译 + sync_devdocs.ps1 + sync_ruif.ps1）
#>
param(
    [string]$RimWorldPath = 'E:\SteamLibrary\steamapps\common\RimWorld'
)

$ErrorActionPreference = 'Stop'

$ruifRoot = Split-Path -Parent $PSScriptRoot
$modsRoot = Join-Path $RimWorldPath 'Mods'
$relRuif = Join-Path $modsRoot 'RUIF'

$script:fail = 0
function Pass([string]$m) { Write-Host "  [PASS] $m" -ForegroundColor Green }
function Fail([string]$m) { Write-Host "  [FAIL] $m" -ForegroundColor Red; $script:fail++ }
function Head([string]$m) { Write-Host "== $m ==" -ForegroundColor Cyan }

# ---------------------------------------------------------------- 1) 版本一致性
Head '1. 版本一致性（构建产物为准）'
$engineManaged = Join-Path $RimWorldPath 'RimWorldWin64_Data\Managed'
foreach ($n in @('UnityEngine.dll', 'UnityEngine.CoreModule.dll', 'UnityEngine.IMGUIModule.dll',
                 'UnityEngine.TextRenderingModule.dll', 'Assembly-CSharp.dll')) {
    $p = Join-Path $engineManaged $n
    if (Test-Path $p) { [void][Reflection.Assembly]::LoadFrom($p) }
}
$ruifDll = Join-Path $ruifRoot 'Assemblies\RUIF.dll'
$asm = $null
if (Test-Path $ruifDll) { $asm = [Reflection.Assembly]::LoadFrom($ruifDll) } else { Fail "缺少构建产物 $ruifDll（先编译）" }

$dllVersion = $null
if ($asm) {
    $apiT = $asm.GetType('RUIF.Api')
    if ($apiT) { $dllVersion = $apiT.GetField('VersionString').GetRawConstantValue() }
}
if (-not $dllVersion) { Fail '无法从 dll 读出 RUIF.Api.VersionString' } else { Pass "dll 版本 = $dllVersion" }

# 程序集版本必须同样被声明且与 Api.VersionString 一致。
# 背景（2026-09-12）：GenerateAssemblyInfo=false 时若不手写 [AssemblyVersion]，
# Assembly.GetName().Version 恒为 0.0.0.0 —— 只有 Api.VersionString 有值，外部工具/反射看到的是 0。
$asmVer = $asm.GetName().Version
if (-not $asmVer -or $asmVer.Major -eq 0) {
    Fail "程序集版本未声明（= $asmVer）—— 请在 Source/Core/AssemblyInfo.cs 加 [assembly: AssemblyVersion(...)]"
} elseif ($asmVer.ToString(3) -ne $dllVersion) {
    Fail "程序集版本 $asmVer 与 Api.VersionString $dllVersion 不一致"
} else {
    Pass "程序集版本 = $asmVer（与 Api.VersionString 一致）"
}

function Get-AboutVersion([string]$path) {
    if (-not (Test-Path $path)) { return $null }
    [xml]$x = Get-Content $path -Raw -Encoding UTF8
    return ($x.ModMetaData.modVersion -replace '^v', '')
}
$aboutRuif = Get-AboutVersion (Join-Path $ruifRoot 'About\About.xml')
if ($aboutRuif -eq $dllVersion) { Pass "RUIF About.xml = $aboutRuif" } else { Fail "RUIF About.xml=$aboutRuif ≠ dll=$dllVersion" }

# ---------------------------------------------------------------- 2) 引用白名单
Head '2. 程序集引用白名单（禁止 0Harmony）'
$allowed = @('mscorlib', 'System', 'System.Core', 'System.Xml', 'System.Drawing',
             'Assembly-CSharp', 'UnityEngine',
             'UnityEngine.CoreModule', 'UnityEngine.IMGUIModule', 'UnityEngine.TextRenderingModule',
             'UnityEngine.SharedInternalsModule')
if ($asm) {
    $refs = $asm.GetReferencedAssemblies() | ForEach-Object { $_.Name }
    $bad = $refs | Where-Object { $allowed -notcontains $_ }
    if ($bad) { Fail ("越界引用: " + ($bad -join ', ')) } else { Pass ("引用面 = " + ($refs -join ', ')) }
    if ($refs -contains '0Harmony') { Fail '框架红线：RUIF.dll 不得引用 0Harmony' }
}

# ---------------------------------------------------------------- 3) 两区哈希
Head '3. 开发区 vs 发布区 dll 哈希'
function Check-Hash([string]$dev, [string]$rel, [string]$label) {
    if (-not (Test-Path $dev)) { Fail "$label 开发区缺 dll: $dev"; return }
    if (-not (Test-Path $rel)) { Fail "$label 发布区缺 dll: $rel"; return }
    $h1 = (Get-FileHash $dev -Algorithm MD5).Hash
    $h2 = (Get-FileHash $rel -Algorithm MD5).Hash
    if ($h1 -eq $h2) { Pass "$label 哈希一致 $h1" } else { Fail "$label 哈希不一致`n         dev=$h1`n         rel=$h2" }
}
Check-Hash $ruifDll (Join-Path $relRuif 'Assemblies\RUIF.dll') 'RUIF'

# ---------------------------------------------------------------- 4) 本地化键对称
Head '4. 本地化键集合对称（中 ↔ 英）'
function Get-KeySet([string]$dir) {
    $set = New-Object 'Collections.Generic.HashSet[string]'
    if (-not (Test-Path $dir)) { return $set }
    foreach ($f in Get-ChildItem $dir -Recurse -Filter *.xml -File) {
        [xml]$x = Get-Content $f.FullName -Raw -Encoding UTF8
        foreach ($n in $x.SelectNodes('//*[not(*)]')) {
            if ($n.Name -ne '#comment') { [void]$set.Add($n.Name) }
        }
    }
    return $set
}
function Compare-Keys([string]$langDirA, [string]$langDirB, [string]$label) {
    $a = Get-KeySet $langDirA
    $b = Get-KeySet $langDirB
    $missB = @($a | Where-Object { -not $b.Contains($_) })
    $missA = @($b | Where-Object { -not $a.Contains($_) })
    if ($missB.Count -eq 0 -and $missA.Count -eq 0) {
        Pass "$label 键集合一致（各 $($a.Count) 键）"
    } else {
        Fail "$label 键不一致"
        if ($missB.Count) { Write-Host ("         英文缺: " + (($missB | Select-Object -First 12) -join ', ')) -ForegroundColor Yellow }
        if ($missA.Count) { Write-Host ("         中文缺: " + (($missA | Select-Object -First 12) -join ', ')) -ForegroundColor Yellow }
    }
    return $a
}
$ruifZhKeys = Compare-Keys (Join-Path $ruifRoot 'Languages\ChineseSimplified') (Join-Path $ruifRoot 'Languages\English') 'RUIF'

# ---------------------------------------------------------------- 5) 代码翻译键覆盖
Head '5. 翻译键覆盖（键命名空间精确扫描 + 死键审计）'
function Check-TranslateKeys([string]$srcDir, $keySet, [string]$nsPattern, [string]$label) {
    if (-not (Test-Path $srcDir)) { return }
    $used = New-Object 'Collections.Generic.HashSet[string]'
    $prefixes = New-Object 'Collections.Generic.HashSet[string]'
    foreach ($f in Get-ChildItem $srcDir -Recurse -Filter *.cs -File) {
        $txt = [IO.File]::ReadAllText($f.FullName)
        # 先剥离注释：文档注释里的示例键会被当成真实键，产生"缺翻译"的假 FAIL。
        $txt = [regex]::Replace($txt, '/\*.*?\*/', '', [Text.RegularExpressions.RegexOptions]::Singleline)
        $txt = [regex]::Replace($txt, '(?m)^\s*//.*$', '')
        # ① 键命名空间字面量（框架的键多存放在数组/常量里再调 Translate，不能只认 "X".Translate()）
        foreach ($m in [regex]::Matches($txt, '"(' + $nsPattern + '[A-Za-z0-9_.]*)"')) {
            $v = $m.Groups[1].Value
            if ($v.EndsWith('.')) { [void]$prefixes.Add($v) } else { [void]$used.Add($v) }
        }
        # ② 直接调用式（覆盖任何非命名空间约定的键）
        foreach ($m in [regex]::Matches($txt, '"([A-Za-z0-9_][A-Za-z0-9_.]*)"\s*\.\s*Translate')) {
            [void]$used.Add($m.Groups[1].Value)
        }
    }

    if ($used.Count -eq 0) { Fail "$label 扫描到 0 个翻译键 —— 检测器失效（不是 PASS）"; return }

    $missing = @($used | Where-Object { -not $keySet.Contains($_) })
    if ($missing.Count -eq 0) { Pass "$label 源码用到 $($used.Count) 个键，全部有翻译" }
    else { Fail ("$label 缺翻译键: " + ($missing -join ', ')) }

    foreach ($p in $prefixes) {
        $hit = @($keySet | Where-Object { $_.StartsWith($p) })
        if ($hit.Count -gt 0) { Pass "$label 前缀 '$p' 匹配 $($hit.Count) 个键" }
        else { Fail "$label 前缀 '$p' 在语言文件中无任何键（前缀被改名？）" }
    }

    # 死键审计（信息级）：只审"键命名空间"内的键，避免把 DefInjected 键误报为死键
    $nsRegex = '^(?:' + $nsPattern + ')'
    $dead = @($keySet | Where-Object {
        $k = $_
        ($k -match $nsRegex) -and (-not $used.Contains($k)) -and
        (-not ($prefixes | Where-Object { $k.StartsWith($_) }))
    })
    if ($dead.Count -gt 0) {
        Write-Host "  [WARN] $label 未被源码引用的键（$($dead.Count)）: " -ForegroundColor Yellow -NoNewline
        Write-Host (($dead | Select-Object -First 10) -join ', ') -ForegroundColor Yellow
    }
}
Check-TranslateKeys (Join-Path $ruifRoot 'Source') $ruifZhKeys 'RUIF\.Comp\.' 'RUIF'

# ---------------------------------------------------------------- 6) 发布区卫生
Head '6. 发布区卫生'
if (-not (Test-Path $relRuif)) {
    Fail "RUIF 发布区不存在: $relRuif（先跑 sync_ruif.ps1）"
} else {
    $files = Get-ChildItem $relRuif -Recurse -File
    $junk = @($files | Where-Object { $_.Name -like '*.bak_*' -or $_.Name -like '*.bak' -or $_.Name -eq 'nul' })
    if ($junk.Count -eq 0) { Pass "RUIF 无备份/垃圾文件（$($files.Count) 文件）" }
    else { Fail "RUIF 残留垃圾: " + (($junk | ForEach-Object { $_.Name }) -join ', ') }
}

# 镜像一致性：发布区不得残留"开发区已删除"的文件（残留的旧文件仍会被游戏加载，表现为"删了却还在"）
$mirror = @{ n = 'RUIF'; Src = $ruifRoot; Dst = $relRuif;
             Dirs = @('About', 'Assemblies', 'Defs', 'Languages', 'Textures', 'Patches', 'DevDocs')
             Keep = @('About\PublishedFileId.txt') }   # 只由游戏写入发布区，不算"残留"（见下）
$stale = @()
foreach ($d in $mirror.Dirs) {
    $s = Join-Path $mirror.Src $d
    $dst = Join-Path $mirror.Dst $d
    if (-not (Test-Path $dst)) { continue }
    foreach ($f in Get-ChildItem $dst -Recurse -File) {
        $rel = $f.FullName.Substring($dst.Length).TrimStart('\')
        # Keep 存的是「相对 Mod 根」的路径，$rel 只相对当前子目录 ⇒ 比较要拼上 $d
        if ($mirror.Keep -contains ($d + '\' + $rel)) { continue }
        if (-not (Test-Path (Join-Path $s $rel))) { $stale += ($d + '\' + $rel) }
    }
}
if ($stale.Count -eq 0) { Pass "$($mirror.n) 发布区与开发区镜像一致（无残留文件）" }
else { Fail ("$($mirror.n) 发布区残留开发区已删除的文件（游戏仍会加载它们）: " + (($stale | Select-Object -First 8) -join ', ') + ' —— 跑 sync_ruif.ps1') }

# ⭐ 工坊条目 ID 保护（2026-09-12 事故断言）：About\PublishedFileId.txt 由游戏在上传成功后写入发布区。
# 它一旦丢失，下一次上传会被 Steam 当成"新作品" ⇒ 同一个 Mod 在创意工坊里出现第二个条目。
$pubIdRel = 'About\PublishedFileId.txt'
$pubIdRelFile = Join-Path $relRuif $pubIdRel
$pubIdDevFile = Join-Path $ruifRoot $pubIdRel
if (Test-Path $pubIdRelFile) {
    $pubId = (Get-Content $pubIdRelFile -Raw).Trim()
    if (-not (Test-Path $pubIdDevFile)) {
        Fail "工坊条目 ID 只在发布区、开发区缺失（$pubId）—— 下次上传会新建工坊条目！跑 sync_ruif.ps1 回写"
    }
    elseif ((Get-Content $pubIdDevFile -Raw).Trim() -ne $pubId) {
        Fail "工坊条目 ID 两区不一致（dev=$((Get-Content $pubIdDevFile -Raw).Trim()) / rel=$pubId）—— 手工确认要保留哪个条目"
    }
    else { Pass "工坊条目 ID 受保护（$pubId · 两区一致）" }
}
else { Pass '尚未上传创意工坊（无 About\PublishedFileId.txt）—— 首次上传后本项开始断言' }

# ---------------------------------------------------------------- 7) 开发者包
Head '7. 开发者包（DevDocs：离线可读的第三方接入入口）'
$devdocs = Join-Path $ruifRoot 'DevDocs'
$relDevdocs = Join-Path $relRuif 'DevDocs'
$required = @('index.html', '零基础上手教程.html', 'API参考.md', '终极宝典v1.0-fixed.html', '组件库全量目录.md', 'BUILD-INFO.txt')
$missing = @()
foreach ($f in $required) { if (-not (Test-Path (Join-Path $devdocs $f))) { $missing += $f } }
if ($missing.Count -eq 0) { Pass "开发者包结构完整（$($required.Count) 项必备文件）" }
else { Fail '开发者包缺件: ' + ($missing -join ', ') + ' —— 先跑 sync_devdocs.ps1' }

$infoFile = Join-Path $devdocs 'BUILD-INFO.txt'
if (Test-Path $infoFile) {
    $m = [regex]::Match((Get-Content $infoFile -Raw), 'version:\s*(\S+)')
    if ($m.Success -and $m.Groups[1].Value -eq $dllVersion) { Pass "BUILD-INFO 版本 = $dllVersion" }
    else { Fail "BUILD-INFO 版本 '$($m.Groups[1].Value)' ≠ dll '$dllVersion'（改版后忘了重生成开发者包）" }
}

if (Test-Path $relDevdocs) {
    $a = @(Get-ChildItem $devdocs -Recurse -File | Where-Object { $_.DirectoryName -notmatch '\\(obj|bin)($|\\)' }).Count
    $b = @(Get-ChildItem $relDevdocs -Recurse -File).Count
    if ($a -eq $b) { Pass "发布区 DevDocs 已同步（$a 文件，已排除 IDE 构建缓存）" }
    else { Fail "发布区 DevDocs 文件数 $b ≠ 开发区（去缓存后）$a（sync_ruif.ps1 未同步？）" }
} else { Fail '发布区缺少 DevDocs' }

# ---------------------------------------------------------------- 8) XML 良构
Head '8. XML 良构（About / Languages / Patches 全量）'
$xmlFiles = @()
foreach ($sub in @('About', 'Languages', 'Patches')) {
    $d = Join-Path $ruifRoot $sub
    if (Test-Path $d) { $xmlFiles += @(Get-ChildItem $d -Recurse -Filter *.xml -File) }
}
$badXml = @()
foreach ($f in $xmlFiles) {
    try { [void][xml](Get-Content $f.FullName -Raw -Encoding UTF8) }
    catch { $badXml += ($f.Name + ' :: ' + $_.Exception.Message) }
}
if ($badXml.Count -eq 0) { Pass "XML 全部良构（$($xmlFiles.Count) 个文件）" }
else { foreach ($b in $badXml) { Fail "XML 解析失败: $b" } }

# About.xml 必备字段（缺 packageId / modVersion 会让 Mod 列表出错或显示异常）
$aboutPath = Join-Path $ruifRoot 'About\About.xml'
if (-not (Test-Path $aboutPath)) { Fail 'RUIF 缺 About.xml' } else {
    [xml]$x = Get-Content $aboutPath -Raw -Encoding UTF8
    $pkgId = $x.ModMetaData.packageId      # 注意：不能用 $pid —— PowerShell 只读自动变量
    $ver = $x.ModMetaData.modVersion
    if ([string]::IsNullOrEmpty($pkgId)) { Fail 'RUIF About.xml 缺 packageId' } else { Pass "RUIF packageId = $pkgId" }
    if ([string]::IsNullOrEmpty($ver)) { Fail 'RUIF About.xml 缺 modVersion' } else { Pass "RUIF modVersion = $ver" }
}

# ---------------------------------------------------------------- 9) 占位符与重复键
Head '9. 本地化占位符一致性与重复键（LOC-004 / LOC-005）'
function Get-KeyedNodes([string]$file) {
    if (-not (Test-Path $file)) { return @() }
    [xml]$x = Get-Content $file -Raw -Encoding UTF8
    return @($x.SelectNodes('//LanguageData/*'))
}
function Get-PlaceholderKey($text) {
    return ((@([regex]::Matches([string]$text, '\{\d+\}') | ForEach-Object { $_.Value }) | Sort-Object -Unique) -join ',')
}
function Check-Placeholders([string]$zhFile, [string]$enFile, [string]$label) {
    if (-not (Test-Path $zhFile) -or -not (Test-Path $enFile)) { Fail "$label 语言文件缺失"; return }
    $zh = @{}
    foreach ($n in Get-KeyedNodes $zhFile) { $zh[$n.Name] = $n.InnerText }
    $en = @{}
    foreach ($n in Get-KeyedNodes $enFile) { $en[$n.Name] = $n.InnerText }

    $bad = @()
    foreach ($k in $zh.Keys) {
        if (-not $en.ContainsKey($k)) { continue }   # 键缺失已由检查 4 报出，这里只比占位符
        $pz = Get-PlaceholderKey $zh[$k]
        $pe = Get-PlaceholderKey $en[$k]
        if ($pz -ne $pe) { $bad += "$k zh=[$pz] en=[$pe]" }
    }
    if ($bad.Count -eq 0) { Pass "$label 占位符一致（比对 $($zh.Count) 键）" }
    else { foreach ($b in $bad) { Fail "$label 占位符不一致（LOC-004）: $b" } }

    foreach ($f in @($zhFile, $enFile)) {
        $txt = [IO.File]::ReadAllText($f)
        $dups = @([regex]::Matches($txt, '<([A-Za-z][A-Za-z0-9_.]+)>') |
                  ForEach-Object { $_.Groups[1].Value } |
                  Group-Object | Where-Object { $_.Count -gt 1 })
        if ($dups.Count -gt 0) {
            Fail ("$(Split-Path $f -Leaf) 重复键（LOC-005）: " + (($dups | ForEach-Object { "$($_.Name)×$($_.Count)" }) -join ', '))
        } else { Pass "$(Split-Path $f -Leaf) 无重复键" }
    }
}
Check-Placeholders (Join-Path $ruifRoot 'Languages\ChineseSimplified\Keyed\RUIF_Components.xml') (Join-Path $ruifRoot 'Languages\English\Keyed\RUIF_Components.xml') 'RUIF.Components'

# ---------------------------------------------------------------- 汇总
Write-Host ''
if ($script:fail -eq 0) {
    Write-Host '[GATE PASS] 发布门禁全项通过，可部署/发布。' -ForegroundColor Green
    exit 0
} else {
    Write-Host "[GATE FAIL] $($script:fail) 项未通过 —— 禁止发布。" -ForegroundColor Red
    exit 1
}
