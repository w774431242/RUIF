$ErrorActionPreference = 'Continue'
$managed = 'E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed'
$global:managedDir = $managed
$resolver = [System.ResolveEventHandler]{
    param($s, $e)
    $simple = ($e.Name -split ',')[0]
    $cand = Join-Path $global:managedDir ($simple + '.dll')
    if (Test-Path $cand) { return [Reflection.Assembly]::LoadFrom($cand) }
    return $null
}
[System.AppDomain]::CurrentDomain.add_AssemblyResolve($resolver)

$log = New-Object Collections.Generic.List[string]
foreach ($n in @('Assembly-CSharp.dll','UnityEngine.CoreModule.dll','UnityEngine.IMGUIModule.dll','UnityEngine.TextRenderingModule.dll')) {
    $p = Join-Path $managed $n
    $a = [Reflection.Assembly]::LoadFrom($p)
    try {
        $ts = $a.GetTypes()
        $log.Add("$n : GetTypes OK  count=$($ts.Count)")
    } catch {
        $ex = $_.Exception
        $log.Add("$n : GetTypes THREW $($ex.GetType().FullName)")
        if ($ex -is [Reflection.ReflectionTypeLoadException]) {
            $log.Add("     Types=$($ex.Types.Count)  nonNull=$(@($ex.Types | Where-Object { $_ -ne $null }).Count)  LoaderExceptions=$($ex.LoaderExceptions.Count)")
            foreach ($le in ($ex.LoaderExceptions | Select-Object -First 5)) { $log.Add("     LE: $($le.Message)") }
        } else { $log.Add("     msg: $($ex.Message)") }
    }
}
[IO.File]::WriteAllLines('E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF\_audit\mech\diag.txt', $log, (New-Object Text.UTF8Encoding($false)))
