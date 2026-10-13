# 机制研发部审计用：反射统计 RUIF.dll 公共类型 / 成员（独立复算，不使用 gen_api_reference.ps1 的口径）
$ErrorActionPreference = 'Stop'
$root = 'E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF'
$managed = 'E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed'
$dll = Join-Path $root 'Assemblies\RUIF.dll'
$out = Join-Path $root '_audit\mech\api_count.txt'

foreach ($n in @('UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.IMGUIModule.dll','UnityEngine.TextRenderingModule.dll','Assembly-CSharp.dll')) {
    $p = Join-Path $managed $n
    if (Test-Path $p) { [void][Reflection.Assembly]::LoadFrom($p) }
}
$asm = [Reflection.Assembly]::LoadFrom($dll)
$types = $asm.GetTypes()
$pub = @($types | Where-Object { $_.IsPublic -or $_.IsNestedPublic })

$flags = [Reflection.BindingFlags]::Public -bor [Reflection.BindingFlags]::Static -bor [Reflection.BindingFlags]::Instance -bor [Reflection.BindingFlags]::DeclaredOnly

$fields = 0; $props = 0; $methods = 0; $enumVals = 0
foreach ($t in $pub) {
    if ($t.IsEnum) { $enumVals += [Enum]::GetNames($t).Count; continue }
    $fields  += @($t.GetFields($flags)    | Where-Object { -not $_.IsSpecialName }).Count
    $props   += @($t.GetProperties($flags)).Count
    $methods += @($t.GetMethods($flags) | Where-Object {
        -not $_.IsSpecialName -and -not $_.IsConstructor -and
        $_.Name -notlike 'get_*' -and $_.Name -notlike 'set_*' -and
        $_.Name -notlike 'add_*' -and $_.Name -notlike 'remove_*' }).Count
}
$memberTotal = $fields + $props + $methods + $enumVals

$lines = New-Object Collections.Generic.List[string]
$lines.Add("Assembly            : $dll")
$lines.Add("AssemblyFileTimeUtc : $((Get-Item $dll).LastWriteTimeUtc.ToString('u'))")
$lines.Add("PublicTypes         : $($pub.Count)")
$lines.Add("ALLTypesInclNested  : $($types.Count)")
$lines.Add("PublicFields        : $fields")
$lines.Add("PublicProperties    : $props")
$lines.Add("PublicMethods       : $methods")
$lines.Add("EnumValues          : $enumVals")
$lines.Add("PublicMembers(gen口径=field+prop+method+enumval) : $memberTotal")
$lines.Add("PublicMembers(不含enum值)                       : $($fields+$props+$methods)")
$lines.Add("")
$lines.Add("--- 类型清单（FullName | Kind | 公开方法数）---")
foreach ($t in ($pub | Sort-Object FullName)) {
    $kind = if ($t.IsEnum) { 'enum' } elseif ($t.IsValueType) { 'struct' } elseif ($t.IsAbstract -and $t.IsSealed) { 'static class' } elseif ($t.IsInterface) { 'interface' } else { 'class' }
    $mc = if ($t.IsEnum) { [Enum]::GetNames($t).Count } else {
        @($t.GetMethods($flags) | Where-Object { -not $_.IsSpecialName -and -not $_.IsConstructor -and $_.Name -notlike 'get_*' -and $_.Name -notlike 'set_*' -and $_.Name -notlike 'add_*' -and $_.Name -notlike 'remove_*' }).Count
    }
    $lines.Add(("{0}`t{1}`t{2}" -f ($t.FullName -replace '\+','.'), $kind, $mc))
}
[IO.File]::WriteAllLines($out, $lines, (New-Object Text.UTF8Encoding($false)))
