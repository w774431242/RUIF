<#
.SYNOPSIS
  生成对外 API 参考：RUIF\docs\API参考.md

.DESCRIPTION
  以反射读取 RUIF.dll 的**全部公共 API**并导出 Markdown（类型分组 + 真实签名 + 类型级摘要）。
  必须先把引擎程序集预加载进当前进程，否则派生自 Verse.Window / Verse.Mod 的类型无法解析
  （表现为 ReflectionTypeLoadException，只能拿到部分类型 —— 会静默漏掉整类 API）。

  为什么不用手写清单：手写必然与实现漂移；本脚本产物是"发布的 API 面"的可复现快照。

  用法（发布前，先编译）：
    pwsh -File RUIF\_scripts\gen_api_reference.ps1
#>
param(
    [string]$RimWorldPath = 'E:\SteamLibrary\steamapps\common\RimWorld',
    [string]$OutFile
)

$ErrorActionPreference = 'Stop'

$ruifRoot = Split-Path -Parent $PSScriptRoot
$dll = Join-Path $ruifRoot 'Assemblies\RUIF.dll'
if (-not (Test-Path $dll)) { throw "RUIF.dll not found - build first: $dll" }
if (-not $OutFile) {
    # docs/ 允许分层（2026-09-12 起按 01…07 分子目录）⇒ 先按文件名递归找现有参考，
    # 找到就**原位重写**（不产生第二份），找不到才回落到扁平路径。
    $found = @(Get-ChildItem (Join-Path $ruifRoot 'docs') -Recurse -File -Filter 'API参考.md' -ErrorAction SilentlyContinue |
               Sort-Object FullName | Select-Object -First 1)
    $OutFile = if ($found.Count -gt 0) { $found[0].FullName } else { Join-Path $ruifRoot 'docs\API参考.md' }
}

# ---------- 1) 预加载引擎程序集（顺序敏感：先底层模块） ----------
$managed = Join-Path $RimWorldPath 'RimWorldWin64_Data\Managed'
foreach ($n in @('UnityEngine.dll', 'UnityEngine.CoreModule.dll', 'UnityEngine.IMGUIModule.dll',
                 'UnityEngine.TextRenderingModule.dll', 'Assembly-CSharp.dll')) {
    $p = Join-Path $managed $n
    if (Test-Path $p) { [void][Reflection.Assembly]::LoadFrom($p) }
}

$asm = [Reflection.Assembly]::LoadFrom($dll)
$allTypes = $asm.GetTypes()          # 预加载后应无异常；仍用 try 兜底以免脚本整体失败
$pub = $allTypes | Where-Object { $_.IsPublic -or $_.IsNestedPublic } | Sort-Object FullName

$version = 'unknown'
$modType = $asm.GetType('RUIF.Api')
if ($modType) {
    $f = $modType.GetField('VersionString')
    if ($f) { $version = $f.GetRawConstantValue() }
}

# ---------- 2) 类型级摘要来自编译器 XML 文档（T: 键无歧义） ----------
$docMap = @{}
$xmlPath = [IO.Path]::ChangeExtension($dll, '.xml')
if (Test-Path $xmlPath) {
    [xml]$doc = Get-Content $xmlPath -Raw
    foreach ($m in $doc.doc.members.member) {
        if ($m.name -like 'T:*') { $docMap[$m.name] = (($m.summary.InnerText -replace '\s+', ' ').Trim()) }
    }
}

# ---------- 3) 类型名格式化 ----------
$aliases = @{
    'System.Void' = 'void'; 'System.Boolean' = 'bool'; 'System.Int32' = 'int'; 'System.Single' = 'float'
    'System.Double' = 'double'; 'System.Int64' = 'long'; 'System.String' = 'string'; 'System.Object' = 'object'
    'System.Char' = 'char'; 'System.Byte' = 'byte'
}

function Format-TypeName([Type]$t, [bool]$shortOnly) {
    if ($null -eq $t) { return 'void' }
    if ($t.IsByRef) { return (Format-TypeName $t.GetElementType() $shortOnly) }
    # ⚠️ 多维数组必须按 rank 还原（`float[,]` 曾被打成 `float[]` ⇒ 消费方照抄会 CS1503）
    if ($t.IsArray) {
        $rank = $t.GetArrayRank()
        $suffix = '[]'
        if ($rank -gt 1) { $suffix = '[' + (',' * ($rank - 1)) + ']' }
        return (Format-TypeName $t.GetElementType() $shortOnly) + $suffix
    }
    if ($t.IsGenericParameter) { return $t.Name }
    if ($t.IsGenericType) {
        $name = $t.Name
        $i = $name.IndexOf('`'); if ($i -ge 0) { $name = $name.Substring(0, $i) }
        $inner = ($t.GetGenericArguments() | ForEach-Object { Format-TypeName $_ $true }) -join ', '
        return "$name<$inner>"
    }
    if ($aliases.ContainsKey($t.FullName)) { return $aliases[$t.FullName] }
    if ($shortOnly) { return $t.Name }
    if ($t.Namespace -eq 'RUIF' -or $t.DeclaringType) { return $t.Name }
    return $t.FullName
}

function Format-Param([Reflection.ParameterInfo]$p) {
    $s = ''
    if ($p.IsOut) { $s += 'out ' } elseif ($p.ParameterType.IsByRef) { $s += 'ref ' }
    $s += (Format-TypeName $p.ParameterType $false) + ' ' + $p.Name
    if ($p.HasDefaultValue) {
        $dv = $p.DefaultValue
        if ($null -eq $dv) { $s += ' = null' }
        elseif ($dv -is [string]) { $s += ' = "' + $dv + '"' }
        elseif ($dv -is [bool]) { $s += ' = ' + $dv.ToString().ToLowerInvariant() }
        else { $s += ' = ' + $dv }
    }
    return $s
}

function Format-Constant($v) {
    if ($null -eq $v) { return 'null' }
    if ($v -is [string]) { return '"' + $v + '"' }
    if ($v -is [bool]) { return $v.ToString().ToLowerInvariant() }
    if ($v -is [single] -or $v -is [double]) { return ([string]::Format([Globalization.CultureInfo]::InvariantCulture, '{0}f', $v)) }
    return $v.ToString()
}

# ---------- 4) 逐类型导出 ----------
$flags = [Reflection.BindingFlags]::Public -bor [Reflection.BindingFlags]::Static `
       -bor [Reflection.BindingFlags]::Instance -bor [Reflection.BindingFlags]::DeclaredOnly

$sb = [Text.StringBuilder]::new()
$memberTotal = 0
$typeCount = 0

[void]$sb.AppendLine('# RUIF 对外 API 参考')
[void]$sb.AppendLine()
[void]$sb.AppendLine("> 版本 **v$version** · 自动生成于 $(Get-Date -Format 'yyyy-MM-dd HH:mm') · 生成器 ``RUIF/_scripts/gen_api_reference.ps1``")
[void]$sb.AppendLine('> 断言口径：v1.0.0 起 API 冻结 —— 删改公共 API 须先 `[Obsolete]` 保留一个主版本。')
[void]$sb.AppendLine('>')
[void]$sb.AppendLine('> 类型名规则：`RUIF.*` 用短名；引擎类型用全限定名（`UnityEngine.Rect` / `Verse.GameFont` 等）。')
[void]$sb.AppendLine('> 消费方引用 `RUIF.dll` 后，同目录的 `RUIF.xml` 会提供 IDE 悬停文档（成员级摘要以源码注释为准）。')
[void]$sb.AppendLine()
[void]$sb.AppendLine('## 目录')
[void]$sb.AppendLine()

foreach ($t in $pub) {
    $display = $t.FullName -replace '\+', '.'
    [void]$sb.AppendLine("- ``$display`` - $(if($t.IsEnum){'enum'}elseif($t.IsValueType){'struct'}elseif($t.IsAbstract -and $t.IsSealed){'static class'}else{'class'})")
}
[void]$sb.AppendLine()

foreach ($t in $pub) {
    $typeCount++
    $display = $t.FullName -replace '\+', '.'
    $kind = if ($t.IsEnum) { 'enum' } elseif ($t.IsValueType) { 'struct' }
            elseif ($t.IsAbstract -and $t.IsSealed) { 'static class' }
            elseif ($t.IsInterface) { 'interface' } else { 'class' }
    $base = ''
    if ($t.BaseType -and $t.BaseType.FullName -ne 'System.Object' `
        -and $t.BaseType.FullName -ne 'System.ValueType' -and -not $t.IsEnum -and -not $t.IsInterface) {
        $base = ' : ' + (Format-TypeName $t.BaseType $false)
    }

    [void]$sb.AppendLine("## ``$display``")
    [void]$sb.AppendLine()
    [void]$sb.AppendLine("``public $kind $($t.Name)$base``")
    $key = 'T:' + $display
    if ($docMap.ContainsKey($key) -and $docMap[$key]) {
        [void]$sb.AppendLine()
        [void]$sb.AppendLine('> ' + $docMap[$key])
    }
    [void]$sb.AppendLine()

    if ($t.IsEnum) {
        [void]$sb.AppendLine('```csharp')
        foreach ($n in [Enum]::GetNames($t)) {
            $val = [Convert]::ToInt64([Enum]::Parse($t, $n))
            [void]$sb.AppendLine("$n = $val")
            $memberTotal++
        }
        [void]$sb.AppendLine('```')
        [void]$sb.AppendLine()
        continue
    }

    $lines = New-Object Collections.Generic.List[string]

    foreach ($fd in ($t.GetFields($flags) | Sort-Object Name)) {
        if ($fd.IsSpecialName) { continue }
        $mods = ''
        if ($fd.IsLiteral) { $mods = 'const ' } elseif ($fd.IsStatic) { $mods = 'static ' }
        if ($fd.IsInitOnly) { $mods += 'readonly ' }
        $line = "public $mods$(Format-TypeName $fd.FieldType $false) $($fd.Name)"
        if ($fd.IsLiteral) { $line += ' = ' + (Format-Constant $fd.GetRawConstantValue()) }
        $lines.Add($line)
        $memberTotal++
    }

    foreach ($pr in ($t.GetProperties($flags) | Sort-Object Name)) {
        $acc = if ($pr.CanRead -and $pr.CanWrite) { ' { get; set; }' } elseif ($pr.CanRead) { ' { get; }' } else { ' { set; }' }
        $st = if ($pr.GetGetMethod($true) -and $pr.GetGetMethod($true).IsStatic) { 'static ' }
              elseif ($pr.GetSetMethod($true) -and $pr.GetSetMethod($true).IsStatic) { 'static ' } else { '' }
        $lines.Add("public $st$(Format-TypeName $pr.PropertyType $false) $($pr.Name)$acc")
        $memberTotal++
    }

    $methods = $t.GetMethods($flags) | Where-Object {
        -not $_.IsSpecialName -and $_.Name -notlike 'get_*' -and $_.Name -notlike 'set_*' `
        -and $_.Name -notlike 'add_*' -and $_.Name -notlike 'remove_*' -and -not $_.IsConstructor
    }
    foreach ($me in ($methods | Sort-Object Name)) {
        $st = if ($me.IsStatic) { 'static ' } else { '' }
        $nm = $me.Name
        if ($me.IsGenericMethodDefinition) {
            $gp = ($me.GetGenericArguments() | ForEach-Object { $_.Name }) -join ', '
            $nm = "$nm<$gp>"
        }
        $ps = ($me.GetParameters() | ForEach-Object { Format-Param $_ }) -join ', '
        $lines.Add("public $st$(Format-TypeName $me.ReturnType $false) $nm($ps)")
        $memberTotal++
    }

    if ($lines.Count -eq 0) {
        [void]$sb.AppendLine('_(无公共成员)_')
        [void]$sb.AppendLine()
        continue
    }

    [void]$sb.AppendLine('```csharp')
    foreach ($l in $lines) { [void]$sb.AppendLine($l) }
    [void]$sb.AppendLine('```')
    [void]$sb.AppendLine()
}

# ---------- 5) 落盘（UTF-8 无 BOM，Markdown 惯例） ----------
$enc = New-Object Text.UTF8Encoding($false)
[IO.File]::WriteAllText($OutFile, $sb.ToString(), $enc)

Write-Host "[OK] $typeCount types / $memberTotal public members -> $OutFile" -ForegroundColor Green
