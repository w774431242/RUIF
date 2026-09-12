<#
.SYNOPSIS
  RUIF 静态体检（与 verify_release.ps1 分工不同：那道是"发布门禁"，这道是"代码健康度"）

.DESCRIPTION
  跑 7 组检查，全部只读、可重复执行：
   1 规模与文档覆盖 —— 公开类型/成员数；公开成员在 RUIF.xml 中的注释覆盖率
   2 死公共 API      —— 公开成员名在（源码/测试/文档/示例）里一次都没出现的候选（启发式，需人工确认）
   3 文档↔实现一致性 —— 从 docs/DevDocs 抽取 `RUIF.X.Y(...)` / `Type.Member(` 引用并反射校验是否存在
   4 中文硬编码      —— 会进入运行时的中文字符串字面量（用户可见文案必须走 Keyed 或由调用方传入）
   5 红线扫描        —— 裸 GUI.color / new Color(白名单外) / 组件读游戏状态(Find.) / 空 catch
   6 分配热点        —— Components 里的容器分配（需人工确认是否在点击分支而非每帧）
   7 测试覆盖        —— 每个公开类型是否被 Tests 引用

  用法：pwsh -File RUIF\_scripts\health_check.ps1      （先编译，脚本读 Assemblies\RUIF.dll）
#>
param(
    [string]$RimWorldPath = 'E:\SteamLibrary\steamapps\common\RimWorld'
)

$ErrorActionPreference = 'Stop'
$ruifRoot = Split-Path -Parent $PSScriptRoot
$testsDir = Join-Path $ruifRoot 'Tests'
$docsDir = Join-Path $ruifRoot 'docs'

$total = 0
function Head([string]$m) { Write-Host ''; Write-Host "== $m ==" -ForegroundColor Cyan }
# 剥离行尾注释（仅当 `//` 之前双引号数量为偶数，避免误伤字符串里的 //）；供字面量扫描使用
function Strip-Comment([string]$line) {
    $q = ([regex]::Matches($line, '"')).Count
    if ($q % 2 -ne 0) { return $line }
    $i = $line.IndexOf('//')
    if ($i -ge 0) { return $line.Substring(0, $i) }
    return $line
}
function Info([string]$m) { Write-Host "  $m" }
function Hit([string]$m) { $script:total++; Write-Host "  [HIT] $m" -ForegroundColor Yellow }
function Good([string]$m) { Write-Host "  [OK ] $m" -ForegroundColor Green }

# ---------- 反射准备（必须先预加载引擎程序集，否则派生自 Verse 的类型无法解析） ----------
$managed = Join-Path $RimWorldPath 'RimWorldWin64_Data\Managed'
foreach ($n in @('UnityEngine.dll', 'UnityEngine.CoreModule.dll', 'UnityEngine.IMGUIModule.dll',
                 'UnityEngine.TextRenderingModule.dll', 'Assembly-CSharp.dll')) {
    $p = Join-Path $managed $n
    if (Test-Path $p) { [void][Reflection.Assembly]::LoadFrom($p) }
}
$dll = Join-Path $ruifRoot 'Assemblies\RUIF.dll'
$asm = [Reflection.Assembly]::LoadFrom($dll)
$pubTypes = @($asm.GetTypes() | Where-Object { $_.IsPublic -or $_.IsNestedPublic } | Sort-Object FullName)

# ------------------------------------------------------------------ 1 规模与文档覆盖
Head '1. 规模与 XML 注释覆盖'
[xml]$doc = Get-Content ([IO.Path]::ChangeExtension($dll, '.xml')) -Raw
$docKeys = New-Object 'Collections.Generic.HashSet[string]'
foreach ($m in $doc.doc.members.member) { [void]$docKeys.Add($m.name) }

$flags = [Reflection.BindingFlags]::Public -bor [Reflection.BindingFlags]::Static -bor
         [Reflection.BindingFlags]::Instance -bor [Reflection.BindingFlags]::DeclaredOnly
function XmlKey([string]$kind, [string]$typeName, [string]$memberName) {
    return "$kind`:$($typeName -replace '\+', '.')$(if ($memberName) { '.' + $memberName })"
}

$memberTotal = 0; $memberDoc = 0; $undoc = @()
foreach ($t in $pubTypes) {
    $tn = $t.FullName
    if ($t.IsEnum) { continue }   # 枚举成员（public static literal）不属"需要文档注释的 API 面"，计入会虚增分母
    foreach ($f in $t.GetFields($flags)) {
        if ($f.IsSpecialName) { continue }
        $memberTotal++
        if ($docKeys.Contains((XmlKey 'F' $tn $f.Name))) { $memberDoc++ } else { $undoc += "$tn.$($f.Name)" }
    }
    foreach ($p in $t.GetProperties($flags)) {
        $memberTotal++
        if ($docKeys.Contains((XmlKey 'P' $tn $p.Name))) { $memberDoc++ } else { $undoc += "$tn.$($p.Name)" }
    }
    foreach ($m in $t.GetMethods($flags)) {
        if ($m.IsSpecialName -or $m.IsConstructor) { continue }
        $memberTotal++
        $key = 'M:' + ($tn -replace '\+', '.') + '.' + $m.Name
        # 无参方法的 XML 成员名**不带括号**（`M:Ns.Type.Method`），带参的才带 `(...)` —— 两种都要判
        $found = $docKeys.Contains($key)
        if (-not $found) { foreach ($k in $docKeys) { if ($k.StartsWith($key + '(')) { $found = $true; break } } }
        if ($found) { $memberDoc++ } else { $undoc += "$tn.$($m.Name)()" }
    }
}
$cov = if ($memberTotal -gt 0) { [math]::Round(100.0 * $memberDoc / $memberTotal, 1) } else { 0 }
Info "公开类型 $($pubTypes.Count) / 公开成员 $memberTotal / 有 XML 注释 $memberDoc（$cov%）"
if ($cov -ge 95) { Good "文档覆盖率 $cov% ≥ 95%" } else { Hit "文档覆盖率仅 $cov%（缺注释 $($undoc.Count) 个）" }
if ($undoc.Count -gt 0 -and $undoc.Count -le 20) { foreach ($u in $undoc) { Info "  缺注释: $u" } }

# ------------------------------------------------------------------ 2 死公共 API
Head '2. 死公共 API（源码/测试/文档/示例均未引用）'
$searchRoots = @((Join-Path $ruifRoot 'Source'), $testsDir, $docsDir, (Join-Path $ruifRoot 'DevDocs'))
$corpus = @()
foreach ($r in $searchRoots) {
    if (Test-Path $r) {
        $corpus += @(Get-ChildItem $r -Recurse -File -ErrorAction SilentlyContinue |
                     Where-Object { $_.Extension -in @('.cs', '.md', '.html', '.xml') } |
                     ForEach-Object { [IO.File]::ReadAllText($_.FullName) })
    }
}
$corpusText = $corpus -join "`n"
$dead = @()
foreach ($t in $pubTypes) {
    if ($t.Name.Length -lt 4) { continue }          # 过短名字误报率高，跳过
    foreach ($m in $t.GetMethods($flags)) {
        if ($m.IsSpecialName -or $m.IsConstructor) { continue }
        if ($m.Name.Length -lt 5) { continue }
        $hits = ([regex]::Matches($corpusText, [regex]::Escape($m.Name))).Count
        if ($hits -le 2) { $dead += "$($t.Name).$($m.Name)()  (引用 $hits 次)" }   # 定义+声明各 1 次
    }
}
if ($dead.Count -eq 0) { Good '未发现"零引用"的公开方法' }
else { Hit "疑似死 API（零/近零引用，需人工确认是否属对外预留）: $($dead.Count) 个"; foreach ($d in ($dead | Select-Object -First 25)) { Info "  $d" } }

# ------------------------------------------------------------------ 3 文档 ↔ 实现一致性
Head '3. 文档 ↔ 实现一致性（文档里提到的 API 是否真实存在）'
$docFiles = @()
# ⚠️ 本检查曾有**两个盲区**（2026-09-12 修，正因为它们手册里的 `Atoms.ButtonBar` 才长期潜伏）：
#   ① `Get-ChildItem -Include` 不带 `-Recurse` / 通配路径时**返回空**（实测 0 个）⇒ 旧写法实际只扫到 README.md；
#   ② 正则只认 `RUIF.` 前缀 ⇒ 漏掉 `using RUIF;` 后的无前缀写法。
# 只审「消费方真正会读的」文档：DevDocs 随包件（排除 体检报告_* / CHANGELOG 这类时点记录）+ README。
# docs/ 其余是设计 / 规划 / 历史文档，含大量"设计意图 / 旧版本"API 名（RUIF.Button、RUIF.Theme…），纳入会淹没真问题。
$devDocsDir = Join-Path $ruifRoot 'DevDocs'
if (Test-Path $devDocsDir) {
    $docFiles += @(Get-ChildItem $devDocsDir -File -ErrorAction SilentlyContinue |
        Where-Object { $_.Extension -in '.md', '.html' -and $_.Name -notlike '体检报告_*' -and $_.Name -ne 'CHANGELOG.md' })
}
$docFiles += @(Get-Item (Join-Path $ruifRoot 'README.md'))
# 占位符 / 文件名后缀 / 语言键命名空间（`RUIF.Comp.*` 等）：避免 `Api.Xxx`、`Tokens.cs`、`RUIF.Comp` 这类误报
$extLike = @('cs', 'md', 'html', 'xml', 'png', 'dll', 'pdb', 'txt', 'csproj', 'json', 'Xxx', 'X')
$keyNs = @('UI', 'Comp', 'Tests')
# ⭐ 类型名用**大小写敏感**集合：PS 哈希表默认大小写不敏感 ⇒ 会把 HTML 里的 JS `CHARTS.filter` 误配到 `Charts`
$typeNames = New-Object 'Collections.Generic.HashSet[string]'
foreach ($t in $pubTypes) { [void]$typeNames.Add($t.Name) }
function Get-PubType([string]$n) { return ($pubTypes | Where-Object { $_.Name -ceq $n } | Select-Object -First 1) }

$missing = @()
foreach ($f in $docFiles) {
    $txt = [IO.File]::ReadAllText($f.FullName)
    # ① 类型引用 `RUIF.X`（如 API参考.md 的 `## RUIF.Atoms`）⇒ 校验类型是否存在
    foreach ($m in [regex]::Matches($txt, '\bRUIF\.([A-Z][A-Za-z0-9_]*)')) {
        $tn = $m.Groups[1].Value
        if ($extLike -contains $tn -or $keyNs -contains $tn -or $tn -ceq 'RUIF') { continue }
        if (-not $typeNames.Contains($tn)) { $missing += "$($f.Name): RUIF.$tn （类型不存在）" }
    }
    # ② 成员引用 `[RUIF.]Type.Member`（含 `using RUIF;` 后的**无前缀**写法，如手册里并不存在的 `Atoms.ButtonBar`）
    foreach ($m in [regex]::Matches($txt, '\b(?:RUIF\.)?([A-Z][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*)')) {
        $tn = $m.Groups[1].Value; $mem = $m.Groups[2].Value
        if ($tn -ceq 'RUIF') { continue }                  # 交 ① 处理
        if ($extLike -contains $mem) { continue }
        if (-not $typeNames.Contains($tn)) { continue }    # 非 RUIF 类型（引擎 / JS）跳过
        $t = Get-PubType $tn
        $ok = @($t.GetMembers($flags) | Where-Object { $_.Name -eq $mem }).Count -gt 0
        if (-not $ok) { $missing += "$($f.Name): $tn.$mem （成员不存在）" }
    }
}
$missing = @($missing | Sort-Object -Unique)
if ($missing.Count -eq 0) { Good '文档引用的 RUIF.* API 全部存在' }
else { Hit "文档引用了不存在的 API: $($missing.Count) 处"; foreach ($x in ($missing | Select-Object -First 25)) { Info "  $x" } }

# 反向：文档是否还在提已移除的 DevTools / Gallery
$stale = @(); $staleEntry = @()
foreach ($f in $docFiles) {
    $txt = [IO.File]::ReadAllText($f.FullName)
    foreach ($m in [regex]::Matches($txt, 'Gallery|DevTools|F11')) {
        if ($f.Name -eq 'CHANGELOG.md' -or $f.Name -eq '健康评估与发布记录.md') { continue }  # 变更历史里提属正常
        $rec = "$($f.Name): $($m.Value)"
        $stale += $rec
        if ($f.Name -eq 'index.html' -or $f.Name -eq 'README.md') { $staleEntry += $rec }
    }
}
$stale = @($stale | Sort-Object -Unique); $staleEntry = @($staleEntry | Sort-Object -Unique)
# 口径（本检查一贯声明）：**入口 / README 必须为 0**；路线图 / 候选清单等历史件记录"已移出"属正常。
if ($staleEntry.Count -gt 0) { Hit "入口 / README 仍提及已移除的演示区: $($staleEntry.Count) 处"; foreach ($x in $staleEntry) { Info "  $x" } }
elseif ($stale.Count -gt 0) { Good "入口 / README 无已移除特性残留（Gallery/DevTools/F11）；其余 $($stale.Count) 处属路线图 / 候选清单的历史记录"; foreach ($x in ($stale | Select-Object -First 15)) { Info "  $x" } }
else { Good '不含已移除特性的残留引用（Gallery/DevTools/F11）' }

# ------------------------------------------------------------------ 4 中文硬编码
Head '4. 中文硬编码（会进入运行时的字符串字面量）'
$cjk = @()
foreach ($f in (Get-ChildItem (Join-Path $ruifRoot 'Source') -Recurse -Filter *.cs)) {
    $n = 0
    foreach ($line in (Get-Content $f.FullName)) {
        $n++
        $code = Strip-Comment $line
        if ([string]::IsNullOrWhiteSpace($code)) { continue }
        foreach ($x in [regex]::Matches($code, '"[^"]*\p{IsCJKUnifiedIdeographs}[^"]*"')) {
            $cjk += "$($f.Name):$n  $($x.Value)"
        }
    }
}
if ($cjk.Count -eq 0) { Good '框架源码无中文硬编码文案' }
else {
    $byFile = $cjk | Group-Object { ($_ -split ':')[0] } | Sort-Object Count -Descending
    Hit "组件/内核层中文硬编码 $($cjk.Count) 处，分布在 $($byFile.Count) 个文件（用户可见文案应走 Keyed 或由调用方传入）"
    foreach ($g in ($byFile | Select-Object -First 12)) { Info "  $($g.Name)  ×$($g.Count)" }
}

# ------------------------------------------------------------------ 5 红线扫描
Head '5. 红线扫描'
# 令牌真源文件：UI 配色只允许在这里产生（组件层任何 new Color 都是违规）。
# Tokens.cs 是唯一的设计令牌真源（16 色固定基线）；Draw.cs / InputExAtoms.cs 为内核与输入控件的历史白名单。
$colorWhitelist = @('Tokens.cs', 'Draw.cs', 'InputExAtoms.cs')
$red = @()
foreach ($f in (Get-ChildItem (Join-Path $ruifRoot 'Source') -Recurse -Filter *.cs)) {
    $n = 0
    foreach ($line in (Get-Content $f.FullName)) {
        $n++
        $code = Strip-Comment $line
        if ([string]::IsNullOrWhiteSpace($code)) { continue }
        if ($code -match 'new Color\(' -and $colorWhitelist -notcontains $f.Name) { $red += "$($f.Name):$n 越白名单 new Color" }
        if ($code -match 'GUI\.color\s*=' -and $f.Name -ne 'Draw.cs') { $red += "$($f.Name):$n 裸写 GUI.color（应走 Draw 作用域）" }
        if ($code -match '\bcatch\s*\{\s*\}') { $red += "$($f.Name):$n 空 catch" }
    }
}
if ($red.Count -eq 0) { Good '无越白名单 new Color / 无裸 GUI.color / 无空 catch' }
else { Hit "红线命中 $($red.Count) 处"; foreach ($x in $red) { Info "  $x" } }

$stateRead = @()
foreach ($f in (Get-ChildItem (Join-Path $ruifRoot 'Source\Components') -Filter *.cs)) {
    $n = 0
    foreach ($line in (Get-Content $f.FullName)) {
        $n++
        $code = Strip-Comment $line
        if ([string]::IsNullOrWhiteSpace($code)) { continue }
        # 白名单：`Find.WindowStack.Add(new FloatMenu(...))` 是**原版唯一的浮层/右键菜单通道**，无替代扩展点
        #   （GOV-007「先用原生扩展点」）；且都在点击分支。其余 Find./Current.Game/GetComponent 仍照常报。
        if ($code -match 'Find\.WindowStack\.Add\(\s*new FloatMenu') { continue }
        if ($code -match '\bFind\.' -or $code -match 'Current\.Game' -or $code -match 'GetComponent<') {
            $stateRead += "$($f.Name):$n  $($code.Trim())"
        }
    }
}
if ($stateRead.Count -eq 0) { Good '组件层未读游戏状态（Find./Current.Game/GetComponent）' }
else { Hit "组件层出现全局访问 $($stateRead.Count) 处（需确认是否属官方 FloatMenu 通道）"; foreach ($x in $stateRead) { Info "  $x" } }

# ------------------------------------------------------------------ 6 分配热点
Head '6. Components 容器分配（需确认是否在点击分支）'
$allocs = @()
foreach ($f in (Get-ChildItem (Join-Path $ruifRoot 'Source\Components') -Filter *.cs)) {
    $n = 0
    foreach ($line in (Get-Content $f.FullName)) {
        $n++
        if ($line -match '\bnew (List|Dictionary|HashSet)<|\.Select\(|\.Where\(|\.ToArray\(|\.ToList\(') {
            $allocs += "$($f.Name):$n  $($line.Trim())"
        }
    }
}
if ($allocs.Count -eq 0) { Good '组件层无容器分配 / 无 LINQ' }
else { Info "分配点 $($allocs.Count) 处（人工确认：点击分支=可接受，绘制路径=需改）"; foreach ($x in $allocs) { Info "  $x" } }

# ------------------------------------------------------------------ 7 测试覆盖
Head '7. 测试覆盖（公开类型是否被 Tests 引用）'
$testText = ''
foreach ($f in (Get-ChildItem $testsDir -Recurse -Filter *.cs -ErrorAction SilentlyContinue)) {
    $testText += [IO.File]::ReadAllText($f.FullName)
}
$untested = @()
foreach ($t in $pubTypes) {
    if ($t.Name.Length -lt 4) { continue }
    if ($t.IsEnum -or $t.IsValueType) { continue }
    if ($testText.IndexOf($t.Name, [StringComparison]::Ordinal) -lt 0) { $untested += $t.Name }
}
Info "公开类型 $($pubTypes.Count)，其中未被测试代码提及 $($untested.Count) 个"
if ($untested.Count -gt 0) { foreach ($u in $untested) { Info "  未覆盖: $u" } }

# ------------------------------------------------------------------ 汇总
Write-Host ''
Write-Host "[体检完成] 命中待办项合计 $script:total 组（HIT 表示需要人工判断，不等同于缺陷）" -ForegroundColor Cyan
