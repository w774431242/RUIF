# 一次性取证脚本 2（逐类型反编译）
$p = "$env:USERPROFILE\.dotnet\tools\ilspycmd.exe"
$dll = 'E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll'
$out = "$env:TEMP\rw_spy3"
$types = @('LudeonTK.UIScaling', 'Verse.UIHighlighter', 'Verse.GenText')
foreach ($t in $types) { & $p $dll -t $t -o $out 2>$null | Out-Null }
Get-ChildItem $out -File | Where-Object { $_.Name -match 'UIScaling|UIHighlighter|GenText' } | Select-Object Name,Length
