# 开发用：反编译 RUIF.dll 指定类型（取枚举/无注释成员的真值，RUIF.xml 只含有注释的成员）
param([Parameter(Mandatory = $true)][string[]]$Types)
$p = "$env:USERPROFILE\.dotnet\tools\ilspycmd.exe"
$dll = Join-Path $PSScriptRoot '..\Assemblies\RUIF.dll'
$out = "$env:TEMP\ruif_spy"
New-Item -ItemType Directory -Force $out | Out-Null
foreach ($t in $Types) { & $p $dll -t $t -o $out 2>$null | Out-Null }
foreach ($t in $Types) {
  $f = Join-Path $out "$t.decompiled.cs"
  if (Test-Path $f) {
    "===== $t ====="
    Get-Content $f | Where-Object { $_ -match '^\s*(public|internal)?\s*(static )?(readonly )?\w+.*(=|;|\{)' -and $_ -notmatch '^\s*//' } |
      Select-Object -First 30
  } else { "===== $t : 不存在（0 字节）=====" }
}
