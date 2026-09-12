<#
.SYNOPSIS
  文档示例真编译验证：把随包文档里的示例代码对**发布区** RUIF.dll 编译一次。

.DESCRIPTION
  依据 `docs/接入审计报告_2026-09-12.md` 的 S-1：文档示例必须真编译，
  禁止"自述编译通过"（曾因此漏出 `Atoms.ButtonBar` / `CommandPalette.Open` 两处不存在的 API）。

  维护方式：改完随包文档的代码示例后，把示例**逐字**同步到 ConsumerSamples.cs，再跑本脚本。
  用法：pwsh -File RUIF/_verify/consumer_docs/verify_samples.ps1
#>
param(
    [string]$Dotnet = 'C:\dotnet8\dotnet.exe'
)
$ErrorActionPreference = 'Stop'
$proj = Join-Path $PSScriptRoot 'consumer_docs.csproj'
& $Dotnet build $proj -c Release -v m
if ($LASTEXITCODE -ne 0) { Write-Host '[FAIL] 文档示例编译失败——禁止带病发布' -ForegroundColor Red; exit 1 }
Write-Host '[OK] 文档示例对发布区 RUIF.dll 编译通过' -ForegroundColor Green
