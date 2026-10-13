#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Phase A：把标准规范组件预览升级为「落地规范」。
给每张标准卡注入 .land 落地契约块（实现锚点 / 交互类型 / 最小尺寸 / 降级档），
vanilla 原版卡注入 B 类·不落地定责。实现锚点取自卡片自身签名类名，且校验真实存在于
Source/Components/*.cs（绝不编造路径）。幂等：先 strip_land 再注入。"""
import re

HTML = r"E:/SteamLibrary/steamapps/common/RimWorld/11/RUIF/docs/02-组件图鉴/标准规范组件预览.html"

# 真实源文件基名（来自 Source/Components/*.cs，grep 真值，禁止编造）
VALID = {
    "ButtonAtoms","ChartCandAtoms","ChartCandFmt","Charts","CommandCandAtoms","CommandCandFmt",
    "DataAtoms","DataCandAtoms","DataCandFmt","DataExAtoms","DomainCandAtoms","DomainCandFmt",
    "FeedbackAtoms","FeedbackAtomsB","FeedbackCandAtoms","FeedbackCandFmt","HiAtoms","HiCandFmt",
    "Industrial","IndustrialCandAtoms","IndustrialCandFmt","InputAtoms","InputCandAtoms","InputCandFmt",
    "InputExAtoms","InputPlusAtoms","InteractAtoms","LayoutCandAtoms","MiscAtoms","NavAtoms","NavAtomsB",
    "NavCandAtoms","NavCandFmt","NavFmt","OfficialDataAtoms","OfficialDataAtomsB","OfficialDataAtomsC",
    "OfficialDataCandAtoms","OfficialDataCandFmt","OfficialFmt","PanelAtoms","PanelCandAtoms","PanelCandFmt",
    "PawnTableAtoms","ProgressAtoms","ProgressCandAtoms","ProgressCandFmt","ReorderAtoms","ReorderCandAtoms",
    "ReorderCandFmt","ReorderFmt","Shapes","StatusAtoms","StatusCandAtoms","StatusCandFmt","TableAtoms",
    "TableCandAtoms","TexAtoms","TextAtoms","TextCandAtoms","TextCandFmt","VisualAtoms","VisualCandAtoms",
    "VisualCandFmt","WindowFrame",
}

# 域 → (兜底文件, 交互类型, 交互备注, 降级档) —— 域级约定，族内具体以源文件该方法为准
DOMAIN_CONTRACT = {
    "panel":    ("PanelCandAtoms.cs","展示型","void·绘制；参数经 PanelCandFmt 传入，自身零状态","D1 省略 / D4 换档(L→M→S)"),
    "text":     ("TextCandAtoms.cs","展示型","void·绘制；富文本按 span 最大高度垂直居中","D1 省略 / D2 减少条目"),
    "status":   ("StatusCandAtoms.cs","展示型","void·绘制；Pulse/Spinner/Caret 为 float/枚举语义","D1 省略 / D3 收窄内边距"),
    "progress": ("ProgressCandAtoms.cs","展示型","void·绘制；FillWipe 自铺底+分段实心，勿叠底/勿压 label","D2 减少条目 / D3 收窄"),
    "visual":   ("VisualCandAtoms.cs","展示型","void·绘制；GradientBar/Swatch 按 Measure 取尺寸","D1 省略 / D4 换档"),
    "reorder":  ("ReorderCandAtoms.cs","交互型","拖拽返回 groupId 变化；状态归调用方 ref int","D2 减少条目 / D5 裁剪"),
    "official": ("OfficialDataCandAtoms.cs","交互型","InfoCardButton 返回 bool（命中）；状态归调用方","D1 省略 / D4 换档"),
    "nav":      ("NavCandAtoms.cs","交互型","Close/Back 返回 bool（命中）；键盘可达","D1 省略"),
    "data":     ("DataCandAtoms.cs","展示型","void·绘制；DataEx 为原生 Atoms 直传 Rect","D2 减少条目 / D5 裁剪"),
    "industrial":("IndustrialCandAtoms.cs","展示型","void·绘制；Industrial 业务组件","D2 减少条目"),
    "feedback": ("FeedbackCandAtoms.cs","展示型","void·绘制；FeedbackB 为原生 Atoms","D1 省略 / D3 收窄"),
    "input":    ("InputCandAtoms.cs","交互型","控件返回 bool/新值；状态归调用方 ref；含键盘焦点环","D1 省略 / D3 收窄内边距"),
    "table":    ("TableCandAtoms.cs","交互型","HeaderCell/ResizeHandle 返回 bool（命中/拖拽）","D5 裁剪 + 滚动"),
    "layout":   ("LayoutCandAtoms.cs","交互型","WindowFrame 含交互件；状态归调用方","D4 换档 / D6 省略"),
    "command":  ("CommandCandAtoms.cs","交互型","CommandPalette 键盘驱动；返回选中","D1 省略 / D5 裁剪"),
}
SKIP_DOMAINS = {"overview","tokens","scope","ext"}

def strip_land(src):
    out = []; i = 0; n = len(src); tag = '<div class="land'
    while i < n:
        idx = src.find(tag, i)
        if idx == -1:
            out.append(src[i:]); break
        out.append(src[i:idx])
        pos = idx + len(tag); depth = 1; j = pos
        while j < n:
            oi = src.find('<div', j); ci = src.find('</div>', j)
            if ci == -1: break
            if oi != -1 and oi < ci: depth += 1; j = oi + 4
            else: depth -= 1; j = ci + 6
            if depth == 0: break
        i = j
    return "".join(out)

def find_cards(src):
    cards = []; i = 0; n = len(src); open_tag = '<div class="comp'
    while i < n:
        idx = src.find(open_tag, i)
        if idx == -1: break
        pos = idx + len(open_tag); depth = 1; j = pos
        while j < n:
            oi = src.find('<div', j); ci = src.find('</div>', j)
            if ci == -1: break
            if oi != -1 and oi < ci: depth += 1; j = oi + 4
            else: depth -= 1; j = ci + 6
            if depth == 0: break
        cards.append((idx, j, src[idx:j]))
        i = j
    return cards

def domain_positions(src):
    return [(m.start(), m.group(1)) for m in re.finditer(r'<div class="domain" id="([^"]+)"', src)]

def domain_of(pos, domains):
    d = None
    for p, idd in domains:
        if p <= pos: d = idd
        else: break
    return d

def pick_class(html):
    found = [m for m in VALID if re.search(r'\b' + re.escape(m) + r'\.', html)]
    if not found: return None
    atoms = [f for f in found if 'Atoms' in f]
    return (atoms or found)[0]

def extract_size(html):
    m = re.search(r'<div class="size">.*?<code>(.*?)</code>', html, re.S)
    return m.group(1).strip() if m else None

def transform(card_html, domain_id):
    is_vanilla = 'class="comp vanilla"' in card_html
    if is_vanilla:
        land = ('<div class="land bclass"><span class="lk">定责</span> '
                'B 类（引擎降级）：需 RimWorld 程序集运行，浏览器无法渲染真实 IMGUI，'
                '仅静态示意，<b>不落地</b>。</div>')
    else:
        cls = pick_class(card_html)
        c = DOMAIN_CONTRACT.get(domain_id, ("", "展示型", "自身零状态", "D1 省略"))
        fb_file, interaction, note, degrade = c
        anchor = (cls + ".cs") if cls else fb_file
        minsize = extract_size(card_html) or "见推荐尺寸"
        land = ('<div class="land">'
                '<span class="lk">实现</span> <code>Source/Components/%s</code> '
                '<span class="lk">交互</span> %s · %s '
                '<span class="lk">最小</span> %s '
                '<span class="lk">降级</span> %s</div>' %
                (anchor, interaction, note, minsize, degrade))
    marker = '<p class="desc">'
    idx = card_html.find(marker)
    if idx == -1:
        ci = card_html.rfind('</div>')
        return card_html[:ci] + land + card_html[ci:]
    return card_html[:idx] + land + "\n      " + card_html[idx:]

def main():
    with open(HTML, encoding="utf-8") as f:
        src = f.read()
    before = src.count('<div class="land')
    src = strip_land(src)
    after_strip = src.count('<div class="land')
    print("剥离旧落地块: %d -> %d" % (before, after_strip))

    domains = domain_positions(src)
    cards = find_cards(src)
    print("解析到组件卡: %d" % len(cards))

    new_parts = []
    i = 0
    injected_std = 0; injected_van = 0; skipped = 0
    for (s, e, html) in cards:
        did = domain_of(s, domains)
        if did in SKIP_DOMAINS:
            new_parts.append(html); skipped += 1; continue
        transformed = transform(html, did)
        if 'class="comp vanilla"' in html[:60]:
            injected_van += 1
        else:
            injected_std += 1
        new_parts.append(transformed)

    # 按文档顺序重建：cards 已按出现顺序，逐个替换
    # 用反向位置替换更安全；这里直接拼接（cards 顺序即文档顺序）
    # 重建需按 (s,e) 顺序切片
    ordered = sorted(zip([c[0] for c in cards], [c[1] for c in cards], new_parts))
    out = []
    prev = 0
    for (s, e, nh) in ordered:
        out.append(src[prev:s]); out.append(nh); prev = e
    out.append(src[prev:])
    new_src = "".join(out)

    with open(HTML, "w", encoding="utf-8") as f:
        f.write(new_src)
    print("注入：标准卡 %d · vanilla 卡 %d · 跳过(meta/ext) %d" % (injected_std, injected_van, skipped))
    print("落地块总数: %d" % new_src.count('<div class="land'))

if __name__ == "__main__":
    main()
