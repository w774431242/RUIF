# 一次性取证脚本：缓存全类型表并按正则检索（首次运行生成 types.txt）
param([Parameter(Mandatory = $true)][string]$Pattern, [int]$Max = 60)
$p = "$env:USERPROFILE\.dotnet\tools\ilspycmd.exe"
$dll = 'E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll'
$out = "$env:TEMP\rw_spy3"
New-Item -ItemType Directory -Force $out | Out-Null
$f = "$out\types.txt"
if (-not (Test-Path $f) -or (Get-Item $f).Length -lt 1000) { & $p $dll -l c > $f 2>$null }
$hits = Select-String -Path $f -Pattern $Pattern
$hits | Select-Object -First $Max | ForEach-Object { $_.Line.Trim() }
'--- total: ' + $hits.Count
