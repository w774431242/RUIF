# 一次性取证脚本：反编译指定原版类型到 %TEMP%\rw_spy3（不随 Mod 分发）
$p = "$env:USERPROFILE\.dotnet\tools\ilspycmd.exe"
$dll = 'E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll'
$out = "$env:TEMP\rw_spy3"
New-Item -ItemType Directory -Force $out | Out-Null
$types = @(
  'Verse.GenUI', 'Verse.TexUI', 'Verse.SolidColorMaterials', 'Verse.Widgets', 'Verse.GenDraw',
  'RimWorld.PawnTable', 'RimWorld.PawnColumnWorker', 'RimWorld.MainTabWindow',
  'RimWorld.ColonistBar', 'Verse.Listing_Tree', 'RimWorld.Dialog_ConfigurePawnTable',
  'Verse.SimpleCurveDrawer', 'Verse.SimpleCurveDrawerStyle'
)
foreach ($t in $types) { & $p $dll -t $t -o $out 2>$null | Out-Null }
Get-ChildItem $out -File | Where-Object { $_.Length -gt 0 } | Select-Object -ExpandProperty Name
