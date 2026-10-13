<#
.SYNOPSIS
  RUIF 开发区 -> 发布区同步（单 Mod：纯组件库）

.DESCRIPTION
  ① 进程守卫：游戏运行中 dll 被占用，直接中止；
  ② 白名单目录同步（RUIF：About/Assemblies/Defs/Languages/Textures/Patches/DevDocs）；
  ③ 清理发布区备份垃圾与构建缓存（*.bak_* / *.bak / nul / obj / bin）—— 发布包必须干净；
  ④ 镜像清理：发布区白名单目录里"开发区已不存在"的文件必须删掉
     （历史遗留的主题资产 / XML 设置等随之被清出发布区）；
  ⑤ 框架 dll 做 MD5 比对（开发区 == 发布区）；
  ⑥ ⭐ 保护「只由游戏写入发布区」的文件（当前 = About\PublishedFileId.txt）：
     镜像清理放过它，并在结尾把它**回写开发区**。
     教训（2026-09-12）：丢了这个文件，下一次上传会被 Steam 当成"新作品"，
     同一个 Mod 在创意工坊里会凭空多出一个条目（表现为"怎么变成了两个 Mod"）。
#>
param(
    [switch]$SkipHashCheck
)

$ErrorActionPreference = 'Stop'

# 0) 守卫：游戏运行中禁止同步
if (Get-Process -Name RimWorldWin64 -ErrorAction SilentlyContinue) {
    Write-Host '[ABORT] RimWorldWin64 is running - dll would be locked.' -ForegroundColor Red
    exit 2
}

$ruifRoot = Split-Path -Parent $PSScriptRoot
$modsRoot = 'E:\SteamLibrary\steamapps\common\RimWorld\Mods'

$targets = @(
    @{ Name = 'RUIF'; Src = $ruifRoot; Dst = (Join-Path $modsRoot 'RUIF');
       Dirs = @('About', 'Assemblies', 'Defs', 'Languages', 'Textures', 'Patches', 'DevDocs');
       Cache = @('obj', 'bin')
       Keep  = @('About\PublishedFileId.txt') },
    # 开发用组件验证台（不随框架发布）：源码在 _demo\RUIF_Demo，只同步游戏需要的三个目录，
    # Source\ 留在开发区（发布区不带源码，也就不会被镜像清理误删）。
    @{ Name = 'RUIF_Demo'; Src = (Join-Path $ruifRoot '_demo\RUIF_Demo'); Dst = (Join-Path $modsRoot 'RUIF_Demo');
       Dirs = @('About', 'Assemblies', 'Defs');
       Cache = @('obj', 'bin')
       Keep  = @() }
)

$dllResults = @()

foreach ($t in $targets) {
    if (-not (Test-Path $t.Src)) { Write-Host "[ABORT] source not found: $($t.Src)" -ForegroundColor Red; exit 2 }
    if (-not (Test-Path $t.Dst)) { New-Item -ItemType Directory -Force -Path $t.Dst | Out-Null }

    $copied = 0
    foreach ($d in $t.Dirs) {
        $s = Join-Path $t.Src $d
        if (Test-Path $s) {
            $dst = Join-Path $t.Dst $d
            New-Item -ItemType Directory -Force -Path $dst | Out-Null
            Copy-Item (Join-Path $s '*') -Destination $dst -Recurse -Force
            $copied++
        }
    }
    Write-Host "[SYNC] $($t.Name): copied $copied dir(s) -> $($t.Dst)"

    # 清只读：源文件可能带只读属性（复制会继承），会让后续镜像清理/删除被拒（Access denied）
    Get-ChildItem $t.Dst -Recurse -File -Force -ErrorAction SilentlyContinue |
        ForEach-Object { if ($_.IsReadOnly) { $_.IsReadOnly = $false } }

    # 构建缓存（obj/bin）绝不进发布包：它们是本机产物，也只对编译者有意义
    if ($t.Cache) {
        $caches = @(Get-ChildItem $t.Dst -Recurse -Directory -ErrorAction SilentlyContinue |
                    Where-Object { $t.Cache -contains $_.Name })
        if ($caches.Count -gt 0) {
            foreach ($c in $caches) { [IO.Directory]::Delete($c.FullName, $true) }
            Write-Host "[CLEAN] $($t.Name): removed $($caches.Count) build cache dir(s)" -ForegroundColor Yellow
        }
    }

    # 清理发布区垃圾（历史遗留 *.bak_* 会让发布包不干净；RimWorld 只加载 .dll，但仍须清理）
    $junk = @(Get-ChildItem $t.Dst -Recurse -File -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -like '*.bak_*' -or $_.Name -like '*.bak' -or $_.Name -eq 'nul' })
    if ($junk.Count -gt 0) {
        foreach ($j in $junk) { Remove-Item -LiteralPath $j.FullName -Force }
        Write-Host "[CLEAN] $($t.Name): removed $($junk.Count) junk file(s)" -ForegroundColor Yellow
    }

    # 镜像清理：发布区白名单目录里"开发区已不存在"的文件必须删掉。
    # 否则删掉的 Def / 语言文件仍会被游戏从发布区旧文件里加载 —— 表现为"改动看起来没生效"。
    # ⚠️ 例外：$t.Keep 里列出的"只由游戏写入发布区"的文件必须放过（见 SYNOPSIS ⑥）。
    $stale = @()
    foreach ($d in $t.Dirs) {
        $s = Join-Path $t.Src $d
        $dst = Join-Path $t.Dst $d
        if (-not (Test-Path $dst)) { continue }
        foreach ($f in Get-ChildItem $dst -Recurse -File) {
            $rel = $f.FullName.Substring($dst.Length).TrimStart('\')
            # ⚠️ Keep 里存的是「相对 Mod 根」的路径（如 About\PublishedFileId.txt），
            # 而 $rel 只相对当前子目录 ⇒ 比较必须拼上 $d，否则例外永远不会命中。
            if ($t.Keep -and ($t.Keep -contains ($d + '\' + $rel))) { continue }
            if (-not (Test-Path (Join-Path $s $rel))) { $stale += $f.FullName }
        }
    }
    if ($stale.Count -gt 0) {
        foreach ($f in $stale) { Remove-Item -LiteralPath $f -Force }
        Write-Host "[PRUNE] $($t.Name): removed $($stale.Count) stale file(s)" -ForegroundColor Yellow
        foreach ($f in $stale) { Write-Host "        - $($f.Substring($t.Dst.Length).TrimStart('\'))" -ForegroundColor DarkYellow }
    }
    # 顺带清掉因此变空的目录（最深优先）
    foreach ($d in $t.Dirs) {
        $dst = Join-Path $t.Dst $d
        if (-not (Test-Path $dst)) { continue }
        Get-ChildItem $dst -Recurse -Directory |
            Sort-Object { $_.FullName.Length } -Descending |
            Where-Object { @(Get-ChildItem $_.FullName -Recurse -File).Count -eq 0 } |
            ForEach-Object { Remove-Item -LiteralPath $_.FullName -Force }
    }

    # ⭐ 保护性回写：发布区的「游戏写入文件」若开发区缺失，回写开发区。
    # 这样它既不会被下一次镜像清理抹掉（开发区已存在 ⇒ 视为正常文件），也能被备份/版本管理覆盖到。
    if ($t.Keep) {
        foreach ($rel in $t.Keep) {
            $relFile = Join-Path $t.Dst $rel
            if (-not (Test-Path $relFile)) { continue }
            $devFile = Join-Path $t.Src $rel
            $devDir = Split-Path $devFile -Parent
            if (-not (Test-Path $devDir)) { New-Item -ItemType Directory -Force -Path $devDir | Out-Null }
            if (-not (Test-Path $devFile)) {
                Copy-Item $relFile $devFile -Force
                Write-Host "[BACKFILL] $($t.Name): $rel <- release area（工坊条目 ID 已保护）" -ForegroundColor Cyan
            }
            elseif ((Get-FileHash $relFile).Hash -ne (Get-FileHash $devFile).Hash) {
                Write-Host "[WARN] $($t.Name): $rel 开发区与发布区不一致 —— 以开发区为准，必要时手工确认" -ForegroundColor Yellow
            }
        }
    }

    # 框架 dll 哈希比对
    if (-not $SkipHashCheck) {
        foreach ($dllName in @('RUIF.dll')) {
            $sDll = Join-Path (Join-Path $t.Src 'Assemblies') $dllName
            if (-not (Test-Path $sDll)) { continue }
            $dDll = Join-Path (Join-Path $t.Dst 'Assemblies') $dllName
            if (-not (Test-Path $dDll)) { Write-Host "[FAIL] $dllName missing in release area" -ForegroundColor Red; exit 3 }
            $h1 = (Get-FileHash $sDll -Algorithm MD5).Hash
            $h2 = (Get-FileHash $dDll -Algorithm MD5).Hash
            if ($h1 -ne $h2) {
                Write-Host "[FAIL] $dllName hash mismatch`n  dev: $h1`n  rel: $h2" -ForegroundColor Red
                exit 3
            }
            Write-Host "[OK] $dllName hash match: $h1" -ForegroundColor Green
            $dllResults += "$dllName=$h1"
        }
    }
}

Write-Host ''
Write-Host '[DONE] RUIF sync complete. 下一步：verify_release.ps1' -ForegroundColor Cyan
