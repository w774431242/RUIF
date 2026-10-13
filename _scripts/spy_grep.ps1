# 一次性取证脚本：在反编译产物里按正则检索（默认 %TEMP%\rw_spy3）
param(
  [Parameter(Mandatory = $true)][string]$Pattern,
  [string]$Dir = "$env:TEMP\rw_spy3",
  [int]$Max = 40
)
$hits = Get-ChildItem "$Dir\*.cs" | ForEach-Object {
  $f = $_.Name -replace '\.decompiled\.cs$', ''
  Select-String -Path $_.FullName -Pattern $Pattern | ForEach-Object {
    "$f : " + ($_.Line.Trim() -replace '\s+', ' ')
  }
}
$hits | Select-Object -First $Max
"--- total: " + @($hits).Count
