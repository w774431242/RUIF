# -*- coding: utf-8 -*-
"""RUIF 组件图鉴 · 终极宝典 v1.0 —— 真融合构建脚本

三份图鉴 HTML 打散重组为单个文档：
- 内容按「游戏程序集用途」9 大域重排（不按卷、不带 ABC 序号）
- CSS 逐规则 scope 前缀化（.v-main/.v-chart/.v-new），主题变量挂容器
- JS 各卷 IIFE 容器化（document -> ROOT），id 加卷前缀，统一主题广播 window.CROWN
- 侧栏 9 域跨卷索引 + ScrollSpy + 全局搜索（跨三卷过滤）

⚠️ 交付物是 终极宝典v1.0-fixed.html —— 在产物基础上人工修 bug 的版本（且设为只读）。
   本脚本的产物 终极宝典v1.0.html 只作中间态：重跑本脚本后必须把修复重新套用到交付版，
   否则会出现「交付版比产物新」的分叉。v2.2.0 起的「装饰层与热门风格主题」章节就是直接打在交付版上的。

⛔ 历史构建器（2026-09-12 起勿再执行）：
   源卷 组件设计图鉴.html 已删除（内容被交付版完整吸收，且不再随包）；
   另两卷 图表组件图鉴.html / 新增组件图鉴.html 亦不随包。重跑会因缺件失败。
"""
import os, re, datetime

os.chdir(os.path.dirname(os.path.abspath(__file__)))

def read(fn):
    return open(fn, encoding="utf-8").read()

def strip_assets(s):
    styles = [m.group(1) for m in re.finditer(r'<style[^>]*>(.*?)</style>', s, re.S)]
    s = re.sub(r'<style[^>]*>.*?</style>', '', s, flags=re.S)
    scripts = [m.group(1) for m in re.finditer(r'<script[^>]*>(.*?)</script>', s, re.S)]
    s = re.sub(r'<script[^>]*>.*?</script>', '', s, flags=re.S)
    return s, styles, scripts

def get_body(s):
    return re.search(r'<body[^>]*>(.*)</body>', s, re.S).group(1)

def cut_block(html, start_marker, tag):
    """从 start_marker 起取平衡的 tag 块，返回 (block, rest)"""
    i = html.index(start_marker)
    lt = i if start_marker[0] == '<' else html.rindex('<', 0, i)
    gt = html.index('>', lt) + 1
    depth, j = 1, gt
    pat = re.compile(r'</' + tag + r'>|<' + tag + r'[\s>]', re.I)
    while depth:
        m = pat.search(html, j)
        if not m:
            raise ValueError('unbalanced ' + tag)
        depth += -1 if m.group(0).startswith('</') else 1
        j = m.end()
    return html[lt:j], html[:lt] + html[j:]

def scope_css(css, scope, kfpre):
    res, i, n = [], 0, len(css)
    def prefix_selector(sel):
        parts = []
        for p in sel.split(','):
            p = p.strip()
            if not p:
                continue
            if p in (':root', 'html', 'body'):
                parts.append(scope)
            elif p.startswith(':root'):
                parts.append(scope + p[5:])
            elif p == '*':
                parts.append(scope + ' *')
            elif p.startswith('body'):
                rest = p[4:].strip()
                parts.append(scope if not rest else scope + ' ' + rest)
            else:
                parts.append(scope + ' ' + p)
        return ', '.join(parts)
    while i < n:
        if css.startswith('/*', i):
            j = css.find('*/', i)
            j = n if j == -1 else j + 2
            res.append(css[i:j]); i = j; continue
        b = css.find('{', i)
        if b == -1:
            res.append(css[i:]); break
        sel = css[i:b]
        depth, j = 1, b + 1
        while j < n and depth:
            if css[j] == '{': depth += 1
            elif css[j] == '}': depth -= 1
            j += 1
        blk = css[b+1:j-1]
        s = sel.strip()
        if s.startswith('@keyframes'):
            name = s.split()[1]
            res.append('@keyframes %s%s{%s}' % (kfpre, name, blk))
        elif s.startswith('@media') or s.startswith('@supports'):
            res.append(s + '{' + scope_css(blk, scope, kfpre) + '}')
        else:
            res.append(prefix_selector(sel) + '{' + blk + '}')
        i = j
    out = ''.join(res)
    out = re.sub(r'(animation(?:-name)?\s*:\s*)(?!none|inherit|initial)([a-zA-Z_][\w-]*)',
                 lambda m: m.group(1) + kfpre + m.group(2), out)
    return out

def prefix_ids(html, pre):
    ids = set(re.findall(r'id="([^"]+)"', html))
    for x in ids:
        html = html.replace('id="%s"' % x, 'id="%s-%s"' % (pre, x))
        html = html.replace('href="#%s"' % x, 'href="#%s-%s"' % (pre, x))
        html = html.replace('url(#%s)' % x, 'url(#%s-%s)' % (pre, x))
    return html

def div_balance(html):
    return len(re.findall(r'<div\b', html)) - len(re.findall(r'</div>', html))

# ================================================================
# 主卷：组件设计图鉴
# ================================================================
s1 = read("组件设计图鉴.html")
s1_noasset, s1_styles, _ = strip_assets(s1)
s1_body = get_body(s1_noasset)
s1_body = cut_block(s1_body, '<header class="hero">', 'header')[1]
if '<div class="switcher">' in s1_body:      # switcher 可能嵌在 hero 内
    s1_body = cut_block(s1_body, '<div class="switcher">', 'div')[1]
s1_body = s1_body.replace('<div id="bar"></div>', '')
s1_body = cut_block(s1_body, '<aside class="side">', 'aside')[1]
s1_body = s1_body.replace('<div class="app">', '', 1)

h2s = [(m.group(1), m.start()) for m in re.finditer(r'<h2 id="([^"]+)">', s1_body)]
prelude = s1_body[:h2s[0][1]]
frags_main = {}
run = div_balance(prelude)
for k, (aid, st) in enumerate(h2s):
    end = h2s[k+1][1] if k+1 < len(h2s) else len(s1_body)
    seg = s1_body[st:end]
    b = div_balance(seg)
    if run + b < 0:
        while div_balance(seg) + run < 0:
            seg = seg[:seg.rindex('</div>')]
        b = div_balance(seg)
    run += b
    frags_main[aid] = seg
# 主卷片段 id/href 加 m- 前缀（侧栏跳转依据）
prelude = prefix_ids(prelude, 'm')
frags_main = {k: prefix_ids(v, 'm') for k, v in frags_main.items()}
print("主卷 prelude balance:", div_balance(prelude), "| 段:", list(frags_main))

# ================================================================
# 图表卷
# ================================================================
s2 = read("图表组件图鉴.html")
s2_noasset, s2_styles, s2_scripts = strip_assets(s2)
s2_body = get_body(s2_noasset)
m_chips = re.search(r'<div class="chips" id="chips"></div>', s2_body)
chart_chips = m_chips.group(0).replace('id="chips"', 'id="c-chips"')
chart_toolbar = prefix_ids(cut_block(s2_body, '<div class="toolbar">', 'div')[0], 'c')
for text, aid in [("〇、过滤清单 — 官方引擎完全无法支撑 / 明确不引入", "g-filter"),
                  ("一、成本分档总览", "g-tiers"),
                  ("二、全量目录总表（按成本升序）", "g-toc")]:
    s2_body = s2_body.replace("<h2>" + text + "</h2>", '<h2 id="%s">%s</h2>' % (aid, text), 1)
h2s2 = [(m.group(1), m.start()) for m in re.finditer(r'<h2 id="([^"]+)">', s2_body)]
frags_chart = {}
for k, (aid, st) in enumerate(h2s2):
    end = h2s2[k+1][1] if k+1 < len(h2s2) else len(s2_body)
    seg = s2_body[st:end]
    b = div_balance(seg)
    if b > 0:
        seg += '</div>' * b
    elif b < 0:
        for _ in range(-b):
            seg = seg[:seg.rindex('</div>')]
    frags_chart[aid] = seg
frags_chart = {k: prefix_ids(v, 'c') for k, v in frags_chart.items()}
print("图表卷段:", list(frags_chart))

# ================================================================
# 新增卷
# ================================================================
s3 = read("新增组件图鉴.html")
s3_noasset, s3_styles, s3_scripts = strip_assets(s3)
s3_body = get_body(s3_noasset)
new_toolbar = prefix_ids(cut_block(s3_body, '<div class="toolbar">', 'div')[0], 'n')
sec3 = re.findall(r'<section data-batch="(\d)">(.*?)</section>', s3_body, re.S)
batch_anchor = {"0": "n-bridge", "1": "n-t1", "2": "n-domain", "3": "n-twin",
                "4": "n-panel", "5": "n-charts12", "6": "n-plan"}
frags_new = {}
for num, seg in sec3:
    aid = batch_anchor[num]
    seg = prefix_ids(seg, 'n')
    seg = seg.replace("<h2>", '<h2 id="%s">' % aid, 1)   # 前缀化后再注入段首锚点
    frags_new[aid] = seg
print("新增卷 sections:", sorted(frags_new))

# ================================================================
# JS 改写
# ================================================================
j2 = s2_scripts[-1]
# 图表碎片被拆分到多个域（g-toc 在 v-main、g-tiers/g-filter 各在不同域），
# 原单根 document.querySelector('.v-chart') 覆盖不到挂载点 -> 改为全文档查询。
# 同时把主题同步到 .v-chart 容器，使 scope 后的 CSS 主题变量生效。
j2 = j2.replace("function setTheme(k){curTheme=k;",
                "function setTheme(k){curTheme=k;document.querySelectorAll('.v-chart').forEach(function(e){e.dataset.theme=k;});")
j2 = re.sub(r"document\.getElementById\('", "ROOT.querySelector('#c-", j2)
j2 = j2.replace('document.querySelectorAll(', 'ROOT.querySelectorAll(')
j2 = j2.replace("sec.id='tier'+t;", "sec.id='c-tier'+t;")   # 动态 tier band id 同步前缀
js_chart = ("(function(){\nconst ROOT=document;\n"
            + j2 +
            "\nwindow.CROWN=window.CROWN||{};window.CROWN.chart=setTheme;\n})();")

j3 = s3_scripts[-1]
# 同图表卷：新增碎片跨多个 data-batch 段，ROOT 改全文档查询；主题同步到 .v-new 容器。
j3 = re.sub(r"document\.getElementById\('th-dark'\)", "ROOT.querySelector('#n-th-dark')", j3)
j3 = re.sub(r"document\.getElementById\('th-vanilla'\)", "ROOT.querySelector('#n-th-vanilla')", j3)
j3 = j3.replace("document.querySelectorAll('canvas[data-d]')", "ROOT.querySelectorAll('canvas[data-d]')")
j3 = j3.replace("document.querySelectorAll('[data-f]')", "ROOT.querySelectorAll('[data-f]')")
j3 = j3.replace("document.querySelectorAll('main > section')", "ROOT.querySelectorAll('section[data-batch]')")
j3 = j3.replace("function mark(){",
                "function setTheme(k){curTheme=k;document.querySelectorAll('.v-new').forEach(function(e){e.dataset.theme=k;});mark();renderAll();}\nfunction mark(){")
js_new = ("(function(){\nconst ROOT=document;\n"
          + j3 +
          "\nwindow.CROWN=window.CROWN||{};window.CROWN.fresh=setTheme;\n})();")

js_main = """(function(){
const ROOTS=[...document.querySelectorAll('.v-main,.v-chart,.v-new')];
window.CROWN=window.CROWN||{};
window.CROWN.main=function(t){ ROOTS.forEach(function(r){r.dataset.theme=t;}); };
window.CROWN.search=function(v){
  v=(v||'').toLowerCase().trim();
  ROOTS.forEach(function(r){
    r.querySelectorAll('.comp,.chart-card').forEach(function(c){
      const hit=!v || ((c.dataset.k||c.innerText).toLowerCase().includes(v));
      c.style.display=hit?'':'none';
    });
  });
};
})();"""

# ================================================================
# CSS 组装
# ================================================================
css_main = scope_css('\n'.join(s1_styles), '.v-main', 'km-')
css_chart = scope_css('\n'.join(s2_styles), '.v-chart', 'kc-')
css_new = scope_css('\n'.join(s3_styles), '.v-new', 'kn-')

# 主题清单（从主卷 switcher 提取）
raw = read("组件设计图鉴.html")
m_sw = re.search(r'<div class="switcher">(.*?)</div>\s*</header>', raw, re.S)
themes = re.findall(r'data-t="([^"]+)"[^>]*>([^<]*)<', m_sw.group(1)) if m_sw else [("dark", "Dark")]
print("主题:", themes)

# ================================================================
# 9 大程序集用途域
# ================================================================
DOMAINS = [
 ("d-base", "基础渲染层", "UnityEngine IMGUI · GUI.color / Rect",
  "框架地基：设计令牌与绘制原子。对应 RUIF 的 Theme / Draw / Layout 三层，全部走官方公开 IMGUI。",
  [("m-tokens", "设计令牌体系", "语义变量 · 多主题单一真源"),
   ("m-a1", "基础原子", "文本 · 几何 · 状态 · 视觉 · 工坊组件")],
  prelude + frags_main.get("tokens", "") + frags_main.get("a1", ""), "v-main"),
 ("d-input", "交互输入层", "Event / KeyboardShortcut / Mouse",
  "按钮、滑杆、下拉、取色器、矩阵选择——只在这一层读取输入事件。",
  [("m-b1", "输入控件", "可交互控件全集 · 含 HSV 取色器与优先级矩阵")],
  frags_main.get("b1", ""), "v-main"),
 ("d-data", "数据展示层", "Pawn / ResourceCounter / Room / WealthWatcher",
  "环世界内容向数据展示：官方公共模型直读原子 + 图表目录索引。",
  [("m-c1", "游戏数据直读组件", "资源 / 心情 / 技能 / 财富 · 零反射"),
   ("c-g-toc", "图表全量目录总表", "48 项按成本升序（T0–T3）")],
  frags_main.get("c1", "") + frags_chart.get("g-toc", ""), "v-main"),
 ("d-alert", "反馈警报层", "Alert / Message / Letter",
  "Toast、确认框、撤销提示、警报脉冲——与官方消息/警报语义对齐的反馈体系。",
  [("m-d1", "反馈与警报组件", "Toast / Confirm / Undo / AlertBlinker 等")],
  frags_main.get("d1", ""), "v-main"),
 ("d-nav", "导航壳层", "MainTabWindow / Dialog / ITab / FloatMenu",
  "页面导航与壳层组件 + 官方 Dialog/Alert/Page 形态的组件化候选。",
  [("m-e1", "导航组件", "TabBar / CollapsibleTree / CommandPalette / Dock"),
   ("n-panel", "面板级候选 8 类", "警报中心 / 浮动菜单 / 账单队列 / 过滤器树 / 信息卡 / 搜索 / 策略 / 向导")],
  frags_main.get("e1", "") + frags_new.get("n-panel", ""), "v-main"),
 ("d-layout", "版式页面层", "Page / TabWindow / 版式预设",
  "组合样式模式、版式深挖与整页级预设（总督仪表盘 / 殖民者档案 / 研究图谱…）。",
  [("m-pat", "组合样式模式", "六大版式模式"),
   ("m-fdeep", "版式深挖", "12 模式详析"),
   ("m-p1", "页面级精美预设", "P1–P12 整页复刻")],
  frags_main.get("pat", "") + frags_main.get("fdeep", "") + frags_main.get("p1", ""), "v-main"),
 ("d-industry", "工业孪生层", "PowerNet / CompPowerTrader / Map / WorkTable",
  "数字孪生方向：DataBridge 数据桥（取数-缓存-快照-域-渲染五段链路）+ 设备/能源/俯视复合件。T1 四原子已在 RUIF v1.2.0 落地。",
  [("n-bridge", "DataBridge 数据桥架构", "孪生与图表的分界线 · 五段链路"),
   ("n-t1", "工业状态原子", "Gauge 表盘 / LEDMatrix 灯阵 / 瓶颈带 / 状态时序"),
   ("n-domain", "DataBridge 域面板", "能源 Power + 库存 Stock 两域"),
   ("n-twin", "孪生复合件", "GridMap 俯视 / 电路拓扑 / 生产链")],
  new_toolbar + frags_new.get("n-bridge", "") + frags_new.get("n-t1", "")
      + frags_new.get("n-domain", "") + frags_new.get("n-twin", ""), "v-new"),
 ("d-chart", "图表统计层", "HistoryUtility / TaleRecorder / Charts",
  "48 项图表候选按成本四档 + Charts v1.2 算法件。所有预览为 Canvas 令牌实时绘制。",
  [("c-g-tiers", "图表卡库（按成本分档）", "48 项 · T0 复用原子 → T3 复杂布局"),
   ("n-charts12", "Charts v1.2 算法件", "SPC 控制图 / Pareto 排列图 / Sankey 能流")],
  chart_chips + chart_toolbar + frags_chart.get("g-tiers", "")
      + frags_new.get("n-charts12", ""), "v-chart"),
 ("d-eng", "工程边界", "落地管线 · 能力过滤",
  "落地管线（I0–I5）、极量扩展储备、官方引擎能力边界与五批排期总表。",
  [("m-plan", "落地管线", "重构版阶段计划 I0–I5"),
   ("m-x-a", "极量扩展储备", "工坊驱动头脑风暴"),
   ("c-g-filter", "过滤清单", "官方引擎完全无法支撑的 8 项"),
   ("n-plan", "新增落地总表", "20 项 / 五批排期底稿")],
  frags_main.get("plan", "") + frags_main.get("x-a", "")
      + frags_chart.get("g-filter", "") + frags_new.get("n-plan", ""), "v-main"),
]

body_secs, anchors = [], []
for did, name, api, intro, items, content, scopes in DOMAINS:
    items_html = ""
    for aid, t, d in items:
        anchors.append((aid, t, did))
        items_html += ('<button class="it" data-t="%s"><span class="t">%s</span>'
                       '<span class="s">%s</span></button>' % (aid, t, d))
    body_secs.append(
        '<section class="domain %s" id="%s" data-scopes="%s" data-theme="dark">'
        '<div class="dhead"><h2>%s</h2><div class="api">%s</div>'
        '<div class="intro">%s</div></div>'
        '<div class="dbody">%s</div></section>' % (scopes, did, scopes, name, api, intro, content))
body_html = '\n'.join(body_secs)

nav_html, cur_d = "", None
for aid, t, did in anchors:
    if did != cur_d:
        dom = next(x for x in DOMAINS if x[0] == did)
        nav_html += '<div class="grp" data-d="%s">%s</div><div class="grp-sub">%s</div>' % (did, dom[1], dom[2])
        cur_d = did
    nav_html += '<button class="it" data-t="%s"><span class="t">%s</span></button>' % (aid, t)

theme_btns = ''.join('<button class="tbtn%s" data-t="%s">%s</button>'
                     % (' on' if t == 'dark' else '', t, label) for t, label in themes)
today = datetime.date.today().isoformat()

CROWN_CSS = """
*{box-sizing:border-box;margin:0;padding:0}
html{scroll-behavior:smooth}
body{background:#101318;color:#d8dee6;font:14px/1.6 "Segoe UI","Microsoft YaHei",sans-serif}
#cbar{position:fixed;top:0;left:0;height:3px;width:0;background:#d9a441;z-index:99}
#top{position:fixed;top:0;left:0;right:0;height:52px;background:#161a20;border-bottom:1px solid #2a303a;
     display:flex;align-items:center;gap:14px;padding:0 16px;z-index:50}
#top .brand{font-size:14.5px;white-space:nowrap}
#top .brand b{color:#d9a441}
#top .ver{font-size:10.5px;color:#6b7482;margin-left:8px}
.themes{display:flex;gap:5px}
.tbtn{background:#1f242b;border:1px solid #2a303a;color:#aeb7c4;font:11.5px/1.6 inherit;
      padding:4px 10px;border-radius:6px;cursor:pointer}
.tbtn:hover{border-color:#d9a441;color:#d9a441}
.tbtn.on{background:#2a3040;border-color:#d9a441;color:#d9a441}
#q{flex:1;max-width:340px;margin-left:auto;background:#1b2027;border:1px solid #2a303a;color:#d8dee6;
   border-radius:7px;padding:6px 12px;font:12.5px inherit;outline:none}
#q:focus{border-color:#d9a441}
#wrap{display:flex;padding-top:52px}
#side{position:fixed;top:52px;bottom:0;left:0;width:292px;background:#141820;border-right:1px solid #222833;
      display:flex;flex-direction:column}
.sidecap{padding:12px 14px 6px;font-size:11px;color:#6b7482;letter-spacing:2px}
#nav{flex:1;overflow-y:auto;padding:0 8px 12px}
.grp{margin:12px 6px 0;font-size:12.5px;font-weight:600;color:#d8dee6}
.grp-sub{margin:0 6px 4px;font-size:10px;color:#5b6472}
.it{display:block;width:100%;text-align:left;background:none;border:0;color:#aeb7c4;
    padding:6px 10px;border-radius:6px;cursor:pointer;font:12.5px/1.5 inherit}
.it:hover{background:#1b212b;color:#d8dee6}
.it.on{background:#232a35;color:#d9a441;box-shadow:inset 2px 0 0 #d9a441}
.it .s{display:block;font-size:10.5px;color:#6b7482;line-height:1.4}
.sidefoot{padding:9px 14px;border-top:1px solid #222833;font-size:10px;color:#5b6472;line-height:1.7}
#content{flex:1;margin-left:292px;padding:0 0 60px}
.domain{border-bottom:1px solid #1c222c;padding:26px 34px 40px}
.dhead h2{font-size:21px;color:#d9a441;margin-bottom:2px}
.dhead .api{font-size:11.5px;color:#7d8796;font-family:Consolas,monospace;margin-bottom:6px}
.dhead .intro{font-size:12.5px;color:#8a94a3;max-width:880px;margin-bottom:14px}
.frag-head{font-size:11px;color:#6b7482;margin:10px 0 6px}
/* —— 三卷 scope 容器基线（承接各自原 body 规则的兜底）—— */
.v-main,.v-chart,.v-new{font:14px/1.65 "Segoe UI",system-ui,"Microsoft YaHei",sans-serif}
.v-main h2,.v-chart h2,.v-new h2{scroll-margin-top:64px}
#foot{text-align:center;color:#5b6472;font-size:11px;padding:30px 0 10px}
__V_MAIN_CSS__
__V_CHART_CSS__
__V_NEW_CSS__
""".replace("__V_MAIN_CSS__", css_main).replace("__V_CHART_CSS__", css_chart).replace("__V_NEW_CSS__", css_new)

SHELL = """<!DOCTYPE html>
<html lang="zh-CN" data-theme="dark">
<head>
<meta charset="utf-8">
<title>RUIF 组件图鉴 · 终极宝典 v1.0</title>
<style>__CROWN_CSS__</style>
</head>
<body>
<div id="cbar"></div>
<header id="top">
  <div class="brand">RUIF 组件图鉴 <b>终极宝典 v1.0</b><span class="ver">3 卷真融合 · 按游戏程序集用途 · __TODAY__</span></div>
  <div class="themes">__THEMES__</div>
  <input id="q" placeholder="搜索组件…（跨全部章节）">
</header>
<div id="wrap">
<aside id="side">
  <div class="sidecap">程序集用途分类</div>
  <nav id="nav">__NAV__</nav>
  <div class="sidefoot">RUIF v2.2.0 · RimWorld 1.6 实证<br>内容源：组件设计图鉴 / 图表组件图鉴 / 新增组件图鉴</div>
</aside>
<main id="content">
__BODY__
<footer id="foot">终极宝典 v1.0 · 由 _build_crown.py 融合构建 · RUIF v2.2.0</footer>
</main>
</div>
<script>
__JS_MAIN__
__JS_CHART__
__JS_NEW__
(function(){
const sideLinks=[...document.querySelectorAll('#nav .it')];
const anchorIds=sideLinks.map(function(b){return b.dataset.t;});
function mark(aid){
  sideLinks.forEach(function(b){b.classList.toggle('on',b.dataset.t===aid);});
}
sideLinks.forEach(function(b){b.addEventListener('click',function(){
  const el=document.getElementById(b.dataset.t);
  if(el)el.scrollIntoView({behavior:'smooth',block:'start'});
  mark(b.dataset.t);
});});
const spy=function(){
  let cur=anchorIds[0];
  for(const aid of anchorIds){
    const el=document.getElementById(aid);
    if(el && el.getBoundingClientRect().top<130) cur=aid;
  }
  mark(cur);
};
addEventListener('scroll',function(){
  spy();
  const h=document.documentElement;
  document.getElementById('cbar').style.width=(h.scrollTop/(h.scrollHeight-h.clientHeight)*100)+'%';
},{passive:true});
const tb=document.querySelectorAll('.tbtn');
tb.forEach(function(b){b.addEventListener('click',function(){
  tb.forEach(function(x){x.classList.remove('on');});
  b.classList.add('on');
  const t=b.dataset.t;
  window.CROWN.main(t);
  if(t==='dark'||t==='vanilla'){ window.CROWN.chart(t); window.CROWN.fresh(t); }
});});
const q=document.getElementById('q');
q.addEventListener('input',function(){ window.CROWN.search(q.value); });
})();
</script>
</body>
</html>"""

out = (SHELL
       .replace("__CROWN_CSS__", CROWN_CSS)
       .replace("__THEMES__", theme_btns)
       .replace("__NAV__", nav_html)
       .replace("__BODY__", body_html)
       .replace("__JS_MAIN__", js_main)
       .replace("__JS_CHART__", js_chart)
       .replace("__JS_NEW__", js_new)
       .replace("__TODAY__", today))

open("终极宝典v1.0.html", "w", encoding="utf-8", newline="").write(out)
print("written:", os.path.getsize("终极宝典v1.0.html"))
