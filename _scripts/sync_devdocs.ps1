<#
.SYNOPSIS
  生成对外「开发者包」RUIF\DevDocs\（随框架 Mod 一起分发，离线可读）

.DESCRIPTION
  解决的核心问题：第三方开发者手里必须有一份**不依赖游戏运行**的完整入口。
  因此按白名单把对外文档复制成独立目录：

    DevDocs/
    ├── index.html                      ← 唯一入口（开发者入口.html 的副本）
    ├── 零基础上手教程.html              ← 第三方开发者向：从 0 到画出第一个窗口（示例真编译）
    ├── API参考.md                      ← 全部公开 API 的真实签名（自动生成）
    ├── 接入指南与引擎可行性审计.html    ← 接入流程 + 依赖语义 + 引擎三级审计
    ├── 组件库全量目录.md                ← 按用途分类的组件索引
    ├── CHANGELOG.md
    └── BUILD-INFO.txt                  ← 版本/生成时间（供 verify_release 断言）

  白名单机制：internal 设计文档（方案草案、评估、落地管线等）**一律不进**开发者包。
  发布顺序：编译 → gen_api_reference.ps1 → sync_devdocs.ps1 → sync_ruif.ps1 → verify_release.ps1
#>
$ErrorActionPreference = 'Stop'

$ruifRoot = Split-Path -Parent $PSScriptRoot
$docs = Join-Path $ruifRoot 'docs'
$out = Join-Path $ruifRoot 'DevDocs'

# 版本从 About.xml 读（不手抄）
[xml]$about = Get-Content (Join-Path $ruifRoot 'About\About.xml') -Raw
$version = ($about.ModMetaData.modVersion -replace '^v', '')

# 用 .NET API 直接删除：不依赖回收站（回收站不可用/被安全策略拦截时 Remove-Item 会失败）
# 删除前先清只读：docs/终极宝典 带只读保护属性，复制出的副本会继承 → 不清会被拒（Access denied）
if (Test-Path $out) {
    Get-ChildItem $out -Recurse -File -Force -ErrorAction SilentlyContinue |
        ForEach-Object { if ($_.IsReadOnly) { $_.IsReadOnly = $false } }
    [IO.Directory]::Delete($out, $true)
}
New-Item -ItemType Directory -Force -Path $out | Out-Null

# ---- 1) 对外文档白名单 ----
# ⭐ docs/ 允许**分层**（2026-09-12 起按 01-设计草案 … 07-构建工具 分子目录）⇒
#    一律按**文件名递归定位**，脚本与目录结构解耦（以后再挪目录不会再断发布链）。
function Find-DocFile([string]$name) {
    $hit = @(Get-ChildItem $docs -Recurse -File -Filter $name -ErrorAction SilentlyContinue |
             Where-Object { $_.DirectoryName -notmatch '\\(obj|bin)($|\\)' } |
             Sort-Object FullName | Select-Object -First 1)
    if ($hit.Count -eq 0) { return $null }
    return $hit[0].FullName
}

$whitelist = @(
    '零基础上手教程.html',
    'API参考.md',
    '终极宝典v1.0-fixed.html',
    '接入指南与引擎可行性审计.html',
    '组件库全量目录.md',
    'CHANGELOG.md'
)
# 体检报告按**模式**纳入全部历史报告（v2.1.1 起）：
# 它们是"这个框架经历过哪些体检、结论与遗留项是什么"的可审计证据，外部开发者据此判断成熟度；
# 用通配而非写死文件名，避免每出一份新报告就得改本脚本（v2.1.0 时就漏掉了新报告）。
$whitelist += @(Get-ChildItem $docs -Recurse -File -Filter '体检报告_*.md' -ErrorAction SilentlyContinue |
                ForEach-Object { $_.Name })
foreach ($f in $whitelist) {
    $p = Find-DocFile $f
    if (-not $p) { throw "对外文档缺失，无法生成开发者包: docs\<任意层>\$f" }
    Copy-Item $p $out -Force
}
$entry = Find-DocFile '开发者入口.html'
if (-not $entry) { throw '对外文档缺失: docs\<任意层>\开发者入口.html' }
Copy-Item $entry (Join-Path $out 'index.html') -Force

# ---- 2) BUILD-INFO（供门禁断言版本一致） ----
$info = @(
    'RUIF DevDocs（对外开发者包）',
    "version: $version",
    "generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')",
    'source: RUIF/docs/**（白名单，按文件名递归定位）',
    '入口: index.html'
) -join "`r`n"
[IO.File]::WriteAllText((Join-Path $out 'BUILD-INFO.txt'), $info, (New-Object Text.UTF8Encoding($false)))

$files = Get-ChildItem $out -Recurse -File
# 发布包内不留只读属性：只读副本会让后续同步/清理失败（源 docs/终极宝典 本身仍是只读保护）
Get-ChildItem $out -Recurse -File -Force | ForEach-Object { if ($_.IsReadOnly) { $_.IsReadOnly = $false } }
$sizeKb = [math]::Round((($files | Measure-Object Length -Sum).Sum / 1KB), 1)
Write-Host "[OK] DevDocs v$version 生成完毕：$($files.Count) 文件 / $sizeKb KB -> $out" -ForegroundColor Green
