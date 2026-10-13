# 拆分《终极宝典》为按分类的图鉴 HTML（保留原 <style> 与整段脚本，用隐藏 stub 保证脚本不抛错）
$ErrorActionPreference = 'Stop'
$src = "e:\SteamLibrary\steamapps\common\RimWorld\11\RUIF\docs\02-组件图鉴\终极宝典v1.0-fixed.html"
$outDir = "e:\SteamLibrary\steamapps\common\RimWorld\11\RUIF\docs\02-组件图鉴\分类图鉴"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$html = [System.IO.File]::ReadAllText($src, [System.Text.Encoding]::UTF8)

$style = [regex]::Match($html, '<style>.*?</style>', [Text.RegularExpressions.RegexOptions]::Singleline).Value
$scriptBlocks = [regex]::Matches($html, '<script>.*?</script>', [Text.RegularExpressions.RegexOptions]::Singleline)
$allScripts = ($scriptBlocks | ForEach-Object { $_.Value }) -join "`n"
$chrome = [regex]::Match($html, '<div id="cbar"></div>\s*<header id="top">.*?</header>\s*<div id="wrap">', [Text.RegularExpressions.RegexOptions]::Singleline).Value

$secMatches = [regex]::Matches($html, '<section class="domain[^"]*" id="(d-[^"]*)"[^>]*>.*?</section>', [Text.RegularExpressions.RegexOptions]::Singleline)
$sections = @{}
foreach ($m in $secMatches) { $sections[$m.Groups[1].Value] = $m.Value }

$navBlock = [regex]::Match($html, '<nav id="nav">.*?</nav>', [Text.RegularExpressions.RegexOptions]::Singleline).Value
$navMatches = [regex]::Matches($navBlock, '<div class="g" data-d="([^"]+)">(.*?)(?=<div class="g" data-d=|</nav>)', [Text.RegularExpressions.RegexOptions]::Singleline)
$navGroups = @{}
foreach ($m in $navMatches) { $navGroups[$m.Groups[1].Value] = '<div class="g" data-d="' + $m.Groups[1].Value + '">' + $m.Groups[2].Value + '</div>' }

$cats = @(
  @{name='原子组件图鉴'; title='原子组件'; secs=@('d-base')},
  @{name='输入组件图鉴'; title='输入组件'; secs=@('d-input')},
  @{name='数据组件图鉴'; title='数据组件'; secs=@('d-data')},
  @{name='反馈组件图鉴'; title='反馈组件'; secs=@('d-alert')},
  @{name='导航组件图鉴'; title='导航组件'; secs=@('d-nav','d-nav-x')},
  @{name='工业组件图鉴'; title='工业组件'; secs=@('d-industry')},
  @{name='图表组件图鉴'; title='图表组件'; secs=@('d-chart')},
  @{name='候选组件图鉴'; title='候选与扩展组件'; secs=@('d-eng')}
)

$catLinks = ($cats | ForEach-Object { '      <a class="cat-link" href="' + $_.name + '.html">' + $_.title + '</a>' }) -join "`n"

foreach ($cat in $cats) {
  $contentSecs = ($cat.secs | ForEach-Object { $sections[$_] }) -join "`n"
  $navInner = ($cat.secs | ForEach-Object { $navGroups[$_] }) -join "`n"

  $stubs = ''
  if (-not ($cat.secs -contains 'd-chart')) {
    $stubs += '<div id="c-chips"></div><button id="c-th-dark"></button><button id="c-th-vanilla"></button><div id="c-tiers"></div><div id="c-summary"></div>'
  }
  if (-not ($cat.secs -contains 'd-industry')) {
    $stubs += '<button id="n-th-dark"></button><button id="n-th-vanilla"></button>'
  }
  if (-not ($cat.secs -contains 'd-base')) {
    $stubs += '<div id="auditCounts"></div>'
  }
  $stubWrap = '<div id="_stubs" style="display:none">' + $stubs + '</div>'

  $title = 'RUIF 组件图鉴 · ' + $cat.title
  $out = @"
<!DOCTYPE html>
<html lang="zh-CN" data-theme="dark">
<head>
<meta charset="utf-8">
<title>$title</title>
$style
</head>
<body>
$chrome
<aside id="side">
<div class="sidecap">分类图鉴</div>
<nav id="nav">
<div class="nbar"><span class="nstat">8 分类 · 拆分视图</span></div>
<div class="g"><div class="gs">跳转其他分类</div><div class="gi">
$catLinks
</div></div>
$navInner
</nav>
<div class="sidefoot">RUIF v1.1.0 · RimWorld 1.6 实证<br>分类拆分自《终极宝典》</div>
</aside>
<main id="content">
$contentSecs
</main>
</div>
$stubWrap
$allScripts
</body>
</html>
"@
  [System.IO.File]::WriteAllText((Join-Path $outDir ($cat.name + '.html')), $out, [System.Text.Encoding]::UTF8)
  Write-Output ("wrote " + $cat.name + ".html  (" + $out.Length + " chars)")
}
