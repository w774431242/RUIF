#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
RUIF 封面（About/Preview.png）—— 程序化 / 算法绘制生成器（框架能力信息图版）

设计原则与框架本身一致：**零贴图、零外部素材**。封面全部由几何基元
（切角多边形、矩形、线段、渐变、扫描线、点阵、高斯模糊发光）按参数化规则绘制。

数据同源（不手抄颜色与事实）：
  · 16 色设计令牌取自 `Source/Core/Tokens.cs`（唯一、固定、不可变的颜色基线，
    写成 CSS 式 0–255 通道便于绘图）；
  · 事实数据取自工程自身（可复核）：
    `About/About.xml`（交付清单：内核 + 原子 42 / 输入 28 / 数据 40 / 导航 12 / 反馈 17 / 图表 10 / 工业 4）
    `_scripts/gen_api_reference.ps1`（73 公开类型 / 435 公开成员——**对外统一口径**，含枚举成员；体检脚本的 381 为不含枚举成员口径，两者并存但对外一律用 435）
    `README.md` §3（分层 L0–L4）、§6（设计令牌 16 色）

用法：
    python RUIF/_scripts/gen_preview.py                    # → RUIF/About/Preview.png
    python RUIF/_scripts/gen_preview.py --out D:/x.png
    python RUIF/_scripts/gen_preview.py --ss 3             # 超采样倍率（默认 2）
"""

from __future__ import annotations

import argparse
import os
import random
import sys
import time

from PIL import Image, ImageDraw, ImageFilter, ImageFont

# --------------------------------------------------------------------------- 常量

RUIF_ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
REPO_ROOT = os.path.dirname(RUIF_ROOT)
FONT_DIR = os.path.join(REPO_ROOT, "Fonts")
WIN_FONTS = r"C:\Windows\Fonts"

VERSION = "v1.0.1"
BASE_W, BASE_H = 1376, 768

# 事实数据（来源见文件头注释）
STAT_TYPES, STAT_MEMBERS = 73, 435
COMPONENTS = [("原子", 42), ("数据展示", 40), ("输入控件", 28), ("反馈", 17), ("导航", 12), ("图表", 10), ("工业", 4)]
LAYERS = [
    ("L4", "Layout", "版式游标 · VStack / HStack / Grid / Scroll · struct 零分配"),
    ("L3", "Compound", "复合组件 · 面板 / 卡片 / 表格 / 图谱 / 驾驶舱"),
    ("L2", "Atoms", "原子组件 · 文本 / 视觉 / 状态 / 进度 / 面板 / 交互"),
    ("L1", "Tokens", "设计令牌 · 16 色只读基线 · 间距 / 字号 / 尺寸"),
    ("L0", "Draw", "零分配作用域 · 颜色 / 字体 / 旋转 / 裁剪 Scope"),
]
# 与 Source/Core/Tokens.cs 同源（唯一、固定、不可变的 16 色基线；顺序 = 源码声明顺序）
TOKENS = [
    ("Window",      (22, 24, 29)),   ("Panel",       (29, 32, 38)),
    ("PanelRaised", (36, 40, 48)),   ("Card",        (34, 38, 46)),
    ("CardHover",   (44, 49, 59)),   ("Border",      (69, 75, 86)),
    ("BorderSoft",  (49, 54, 63)),   ("BorderHair",  (42, 46, 54)),
    ("Text",        (217, 221, 227)), ("Muted",      (143, 150, 160)),
    ("Dim",         (110, 118, 130)), ("Accent",     (78, 154, 240)),
    ("Info",        (122, 184, 232)), ("Alive",      (88, 179, 104)),
    ("Dead",        (217, 83, 79)),   ("Warn",       (224, 160, 60)),
]
TOK = dict(TOKENS)
# 面板绘制函数沿用索引契约：2=window 3=panel 4=border 5=accent 6=text 7=muted
PALETTE = ("tokens", "设计基线", TOK["Window"], TOK["Panel"], TOK["Border"],
           TOK["Accent"], TOK["Text"], TOK["Muted"])
SEM_ALIVE, SEM_WARN, SEM_DEAD = TOK["Alive"], TOK["Warn"], TOK["Dead"]


# --------------------------------------------------------------------------- 几何 / 绘图工具

def rgba(c, a=255):
    return (c[0], c[1], c[2], a)


def mix(c1, c2, t):
    return tuple(int(round(c1[i] + (c2[i] - c1[i]) * t)) for i in range(3))


def vgrad(size, top, bottom):
    img = Image.new("RGBA", size)
    d = ImageDraw.Draw(img)
    w, h = size
    for y in range(h):
        d.line([(0, y), (w, y)], fill=rgba(mix(top, bottom, y / max(1, h - 1))))
    return img


def chamfer_poly(x, y, w, h, cut):
    """切角矩形（CornerStyle.Chamfer）。"""
    return [(x + cut, y), (x + w - cut, y), (x + w, y + cut), (x + w, y + h - cut),
            (x + w - cut, y + h), (x + cut, y + h), (x, y + h - cut), (x, y + cut)]


def rounded_poly(x, y, w, h, r, seg=6):
    """圆角矩形多边形（CornerStyle.Rounded，用圆弧采样近似）。"""
    import math
    pts = []
    for (cx, cy, a0) in ((x + w - r, y + r, -90), (x + w - r, y + h - r, 0),
                         (x + r, y + h - r, 90), (x + r, y + r, 180)):
        for i in range(seg + 1):
            a = math.radians(a0 + 90 * i / seg)
            pts.append((cx + r * math.cos(a), cy + r * math.sin(a)))
    return pts


def load_font(name, size, variation=None, index=0):
    path = name if os.path.isabs(name) else os.path.join(FONT_DIR, name)
    if not os.path.exists(path):
        path = os.path.join(WIN_FONTS, os.path.basename(name))
    font = ImageFont.truetype(path, size, index=index)
    if variation:
        try:
            font.set_variation_by_name(variation)
        except Exception:
            pass
    return font


def zh_font(size, bold=False):
    p = "msyhbd.ttc" if bold else "msyh.ttc"
    try:
        return load_font(p, size)
    except Exception:
        return load_font("msyh.ttc", size)


def text_tracked(draw, pos, text, font, fill, tracking=0.0):
    x, y = pos
    for ch in text:
        draw.text((x, y), ch, font=font, fill=fill)
        x += draw.textlength(ch, font=font) + tracking
    return x


def glowify(base, layer, radii, gain=1.0):
    """把 layer 上的亮色元素多级高斯模糊后叠加回 base（霓虹发光）。"""
    for r in radii:
        blurred = layer.filter(ImageFilter.GaussianBlur(r))
        if gain != 1.0:
            a = blurred.split()[3].point(lambda v: min(255, int(v * gain)))
            blurred.putalpha(a)
        base = Image.alpha_composite(base, blurred)
    return Image.alpha_composite(base, layer)


def new_layer(size):
    """半透明绘制必须走独立图层：RGBA 画布上的 ImageDraw 是替换像素（含 alpha），
    低 alpha 填充直接画会在 convert('RGB') 时退化成不透明实色块。"""
    return Image.new("RGBA", size, (0, 0, 0, 0))


# --------------------------------------------------------------------------- 板块绘制

def draw_background(img, scale, th):
    """底：window 色 + 渐变 + 网格 + 扫描线 + 青/品红双光晕。"""
    w, h = img.size
    window, border, accent, text = th[2], th[4], th[5], th[6]

    ov = new_layer((w, h))
    d = ImageDraw.Draw(ov)
    step = 64 * scale
    for x in range(0, w, step):
        d.line([(x, 0), (x, h)], fill=rgba(border, 24))
    for y in range(0, h, step):
        d.line([(0, y), (w, y)], fill=rgba(border, 24))
    spacing, alpha = 7 * scale, int(0.055 * 255)      # 扫描线：自绘装饰，与任何主题无关
    for y in range(0, h, spacing):
        d.line([(0, y), (w, y)], fill=rgba(text, alpha))
    img.alpha_composite(ov)

    halo = new_layer((w, h))
    hd = ImageDraw.Draw(halo)
    hd.ellipse([int(0.30 * w) - int(0.36 * w), int(0.16 * h) - int(0.42 * h),
                int(0.30 * w) + int(0.36 * w), int(0.16 * h) + int(0.42 * h)], fill=rgba(accent, 42))
    hd.ellipse([int(0.92 * w) - int(0.30 * w), int(0.96 * h) - int(0.34 * h),
                int(0.92 * w) + int(0.30 * w), int(0.96 * h) + int(0.34 * h)], fill=rgba(SEM_DEAD, 54))
    img.alpha_composite(halo.filter(ImageFilter.GaussianBlur(130 * scale)))


def panel(lay, x, y, w, h, scale, th, title, sub=None, icon=None):
    """统一样式的信息面板：切角底 + 边框 + 标题栏（左侧 accent 竖条）。"""
    cut = int(9 * scale)
    border, accent, muted, panel_c = th[4], th[5], th[7], th[3]
    d = ImageDraw.Draw(lay)
    # 底（多画一层略亮的标题栏分区）
    body = vgrad((w, h), mix(panel_c, (0, 0, 0), 0.26), mix(panel_c, (0, 0, 0), 0.02))
    tmp = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    ImageDraw.Draw(tmp).polygon(chamfer_poly(0, 0, w - 1, h - 1, cut), fill=255)
    # 用 alpha_composite 语义把底贴到 lay 上（lay 与 base 同尺寸，这里直接以图层为单位返回补丁）
    patch = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    patch.paste(body, (x, y), tmp)
    lay.alpha_composite(patch) if hasattr(lay, "alpha_composite") else None
    d.polygon(chamfer_poly(x, y, w, h, cut), outline=rgba(border, 205), width=max(1, int(1.2 * scale)))
    d.line([(x + cut, y), (x + w - cut, y)], fill=rgba(accent, 190), width=int(1.6 * scale))

    tb = y + int(30 * scale)
    d.line([(x + cut, tb), (x + w - cut, tb)], fill=rgba(border, 150), width=max(1, int(0.9 * scale)))
    d.rectangle([x + int(12 * scale), y + int(10 * scale), x + int(15 * scale), y + int(21 * scale)],
                fill=rgba(accent, 240))
    d.text((x + int(24 * scale), y + int(7 * scale)), title, font=zh_font(int(16 * scale), bold=True),
           fill=rgba(th[6], 245))
    if sub:
        d.text((x + w - int(14 * scale) - d.textlength(sub, font=load_font("consola.ttf", int(13 * scale))),
                y + int(10 * scale)), sub, font=load_font("consola.ttf", int(13 * scale)), fill=rgba(muted, 225))
    return tb


def draw_header(base, lay, scale, th):
    w = base.size[0]
    accent, text, muted = th[5], th[6], th[7]

    f_main = load_font("Cinzel-Variable.ttf", int(126 * scale), variation="Bold")
    f_sub = load_font("consola.ttf", int(23 * scale))
    f_zh = zh_font(int(19 * scale))
    f_num = load_font("consola.ttf", int(17 * scale))
    f_ver = load_font("consola.ttf", int(19 * scale))

    # 标题（发光层 → 合成 → 实心字；顺序反了实心字会被盖成 accent 色）
    glow = new_layer(base.size)
    ImageDraw.Draw(glow).text((int(62 * scale), int(34 * scale)), "RUIF", font=f_main, fill=rgba(accent, 255))
    base = glowify(base, glow, radii=(8 * scale, 26 * scale), gain=1.05)

    d = ImageDraw.Draw(lay)
    d.text((int(62 * scale), int(34 * scale)), "RUIF", font=f_main, fill=rgba(text, 255))
    text_tracked(d, (int(66 * scale), int(180 * scale)), "RIMWORLD UI FRAMEWORK", f_sub,
                 rgba(muted, 255), tracking=3.4 * scale)
    d.text((int(66 * scale), int(216 * scale)),
           "面向全体 Mod 开发者的通用 UI 框架 · 即时模式 + 声明式布局 · 零贴图 / 零 shader / 零资产依赖",
           font=f_zh, fill=rgba(muted, 240))

    # 右上：版本 + 关键数据
    vw = d.textlength(VERSION, font=f_ver)
    d.text((w - int(64 * scale) - vw, int(52 * scale)), VERSION, font=f_ver, fill=rgba(accent, 240))
    line = f"{STAT_TYPES} TYPES · {STAT_MEMBERS} MEMBERS · 153 COMPONENTS · 16 TOKENS"
    lw = d.textlength(line, font=f_num)
    d.text((w - int(64 * scale) - lw, int(196 * scale)), line, font=f_num, fill=rgba(text, 215))

    d.line([(int(62 * scale), int(258 * scale)), (w - int(62 * scale), int(258 * scale))],
           fill=rgba(accent, 120), width=max(1, int(1.1 * scale)))
    return base


def draw_layers_panel(lay, x, y, w, h, scale, th, tb):
    """分层架构 L0–L4：5 条色带，自下而上由深到亮。"""
    border, accent, muted, text = th[4], th[5], th[7], th[6]
    d = ImageDraw.Draw(lay)
    n = len(LAYERS)
    top = tb + int(12 * scale)
    gap = int(7 * scale)
    row_h = int((h - (top - y) - int(12 * scale) - (n - 1) * gap) / n)
    f_id = load_font("consola.ttf", int(15 * scale))
    f_nm = load_font("consola.ttf", int(13 * scale))
    f_ds = zh_font(int(12 * scale))

    for i, (lid, nm, ds) in enumerate(LAYERS):          # L4 在最上
        ry = top + i * (row_h + gap)
        t = i / max(1, n - 1)
        band = mix(mix(th[2], accent, 0.05 + 0.14 * (1 - t)), (0, 0, 0), 0.10 + 0.22 * t)
        d.rectangle([x + int(14 * scale), ry, x + w - int(14 * scale), ry + row_h], fill=rgba(band, 235))
        d.rectangle([x + int(14 * scale), ry, x + int(17 * scale), ry + row_h], fill=rgba(accent, 230 - i * 26))
        d.text((x + int(26 * scale), ry + int(4 * scale)), lid, font=f_id, fill=rgba(accent, 250))
        d.text((x + int(62 * scale), ry + int(5 * scale)), nm, font=f_nm, fill=rgba(text, 245))
        d.text((x + int(150 * scale), ry + int(7 * scale)), ds, font=f_ds, fill=rgba(muted, 235))


def draw_components_panel(lay, x, y, w, h, scale, th, tb):
    """组件库 153 件：水平条形图（数量真实取自 About 交付清单）。"""
    accent, muted, text, border = th[5], th[7], th[6], th[4]
    d = ImageDraw.Draw(lay)
    f_nm = zh_font(int(13 * scale))
    f_no = load_font("consola.ttf", int(13 * scale))
    top = tb + int(14 * scale)
    n = len(COMPONENTS)
    gap = int(9 * scale)
    row_h = int((h - (top - y) - int(14 * scale) - (n - 1) * gap) / n)
    label_w = int(76 * scale)
    num_w = int(34 * scale)
    bar_x = x + int(14 * scale) + label_w
    bar_max = w - int(28 * scale) - label_w - num_w
    mx = max(v for _, v in COMPONENTS)

    for i, (nm, val) in enumerate(COMPONENTS):
        ry = top + i * (row_h + gap)
        d.text((x + int(14 * scale), ry + int(1 * scale)), nm, font=f_nm, fill=rgba(text, 240))
        bw = int(bar_max * val / mx)
        col = accent if i < 2 else (SEM_ALIVE if i < 4 else SEM_WARN)
        d.rectangle([bar_x, ry + int(2 * scale), bar_x + bar_max, ry + row_h - int(2 * scale)],
                    fill=rgba(mix(th[3], (0, 0, 0), 0.35), 220))
        d.rectangle([bar_x, ry + int(2 * scale), bar_x + bw, ry + row_h - int(2 * scale)], fill=rgba(col, 230))
        d.text((bar_x + bar_max + int(8 * scale), ry + int(1 * scale)), str(val), font=f_no, fill=rgba(col, 245))


def ink_on(color):
    """在给定底色上选一个可读的前景色（自动对比，不依赖任何主题）。"""
    lum = 0.2126 * color[0] + 0.7152 * color[1] + 0.0722 * color[2]
    return (16, 18, 22) if lum > 140 else (233, 236, 241)


def draw_tokens_panel(lay, x, y, w, h, scale, th, tb):
    """设计令牌 16 色：4×4 色卡，每格填自己的颜色并把令牌名写在格内（自动选可读前景）。"""
    border = th[4]
    d = ImageDraw.Draw(lay)
    f_nm = load_font("consola.ttf", max(8, int(11 * scale)))
    top = tb + int(14 * scale)
    cols, rows = 4, 4
    gap = int(6 * scale)
    cw = int((w - int(28 * scale) - (cols - 1) * gap) / cols)
    chh = int((h - (top - y) - int(14 * scale) - (rows - 1) * gap) / rows)

    for i, (name, col) in enumerate(TOKENS):
        cx = x + int(14 * scale) + (i % cols) * (cw + gap)
        cy = top + (i // cols) * (chh + gap)
        # 深色令牌（Window/Panel 等）与暗底几乎同色，必须靠描边划出边界
        d.rectangle([cx, cy, cx + cw, cy + chh], fill=rgba(col, 255),
                    outline=rgba(mix(border, th[6], 0.42), 220), width=max(1, int(0.9 * scale)))
        fg = ink_on(col)
        tw = d.textlength(name, font=f_nm)
        d.text((cx + (cw - tw) / 2, cy + (chh - int(11 * scale)) / 2 - int(1 * scale)),
               name, font=f_nm, fill=rgba(fg, 245))


def draw_integration_panel(lay, x, y, w, h, scale, th, tb):
    """接入与交付：两种依赖模式 + 交付物标签。"""
    accent, muted, text, border = th[5], th[7], th[6], th[4]
    d = ImageDraw.Draw(lay)
    f_h = zh_font(int(13 * scale), bold=True)
    f_t = zh_font(int(12 * scale))
    f_k = load_font("consola.ttf", int(12 * scale))
    top = tb + int(14 * scale)

    rows = [
        ("硬依赖", accent, "About.xml 声明 modDependencies + Api.IsAvailable 版本门控",
         "modDependencies → RUIF.Api"),
        ("软依赖", SEM_ALIVE, "反射探测 + 隔离桥：框架缺失仍可加载并降级",
         "RuifProbe / RuifBridge"),
    ]
    row_h = int(34 * scale)
    for i, (nm, col, desc, code) in enumerate(rows):
        ry = top + i * (row_h + int(8 * scale))
        d.rectangle([x + int(14 * scale), ry, x + w - int(14 * scale), ry + row_h], fill=rgba(mix(th[3], (0, 0, 0), 0.42), 225))
        d.rectangle([x + int(14 * scale), ry, x + int(17 * scale), ry + row_h], fill=rgba(col, 235))
        d.text((x + int(26 * scale), ry + int(4 * scale)), nm, font=f_h, fill=rgba(col, 250))
        d.text((x + int(26 * scale), ry + int(18 * scale)), desc, font=f_t, fill=rgba(text, 225))
        d.text((x + w - int(20 * scale) - d.textlength(code, font=f_k), ry + int(9 * scale)),
               code, font=f_k, fill=rgba(muted, 225))

    # 交付标签行
    tags = ["DevDocs 离线包", "中英双语", "设计令牌固定", "零 Harmony 依赖", "API 全公开"]
    ty = top + 2 * (row_h + int(8 * scale)) + int(6 * scale)
    tx = x + int(14 * scale)
    for t in tags:
        tw = d.textlength(t, font=f_t) + int(16 * scale)
        if tx + tw > x + w - int(14 * scale):
            break
        d.rectangle([tx, ty, tx + tw, ty + int(21 * scale)], fill=rgba(accent, 32),
                    outline=rgba(accent, 120), width=max(1, int(0.9 * scale)))
        d.text((tx + int(8 * scale), ty + int(3 * scale)), t, font=f_t, fill=rgba(text, 235))
        tx += tw + int(7 * scale)

    # 质量指标行（工程成熟度的硬指标，与 health_check / verify_release 口径一致）
    qy = ty + int(27 * scale)
    if qy < y + h - int(9 * scale):
        d.text((x + int(14 * scale), qy),
               "绘制路径 0 分配 · 翻译缓存有界 512 · 静态体检七组 · 发布门禁九道 · 171 项单测",
               font=f_t, fill=rgba(muted, 240))


def draw_palette_footer(lay, scale, th, y):
    """底栏：16 条设计令牌色带（每条填自己的颜色）+ 令牌名。"""
    accent, muted = th[5], th[7]
    d = ImageDraw.Draw(lay)
    f_lab = load_font("consola.ttf", int(16 * scale))
    f_nm = load_font("consola.ttf", max(8, int(10 * scale)))

    d.text((int(62 * scale), y + int(2 * scale)), "16 TOKENS", font=f_lab, fill=rgba(accent, 245))
    x0 = int(62 * scale) + d.textlength("16 TOKENS", font=f_lab) + int(18 * scale)
    avail = 1376 * scale - int(62 * scale) - x0
    gap = int(6 * scale)
    cw = int((avail - (len(TOKENS) - 1) * gap) / len(TOKENS))
    chh = int(30 * scale)
    for i, (name, col) in enumerate(TOKENS):
        cx = x0 + i * (cw + gap)
        d.rectangle([cx, y, cx + cw, y + chh], fill=rgba(col, 255),
                    outline=rgba(mix(th[4], th[6], 0.58), 230), width=max(1, int(0.9 * scale)))
        d.text((cx, y + chh + int(5 * scale)), name, font=f_nm, fill=rgba(muted, 235))


# --------------------------------------------------------------------------- 主流程

def build(out_path: str, ss: int = 2):
    t0 = time.time()
    th = PALETTE
    w, h = BASE_W * ss, BASE_H * ss

    base = Image.new("RGBA", (w, h), rgba(th[2], 255))
    base.alpha_composite(vgrad((w, h), th[2], th[3]))
    draw_background(base, ss, th)

    # ---- 外框（Neon + CornerTicks）
    frame = new_layer((w, h))
    fd = ImageDraw.Draw(frame)
    m, cut = 20 * ss, 15 * ss
    fd.line(chamfer_poly(m, m, w - 2 * m, h - 2 * m, cut) + [chamfer_poly(m, m, w - 2 * m, h - 2 * m, cut)[0]],
            fill=rgba(th[5], 205), width=2 * ss)
    t = 26 * ss
    for (px, py, dx, dy) in ((m, m, 1, 1), (w - m, m, -1, 1), (m, h - m, 1, -1), (w - m, h - m, -1, -1)):
        fd.line([(px + dx * cut, py), (px + dx * (cut + t), py)], fill=rgba(th[5], 240), width=3 * ss)
        fd.line([(px, py + dy * cut), (px, py + dy * (cut + t))], fill=rgba(th[5], 240), width=3 * ss)
    base = glowify(base, frame, radii=(6 * ss, 16 * ss), gain=0.8)

    lay = new_layer((w, h))
    base = draw_header(base, lay, ss, th)

    # ---- 面板几何
    LX, RX = int(62 * ss), int(576 * ss)
    LW = int(490 * ss)
    RW = w - int(62 * ss) - RX
    BY = int(280 * ss)
    AH, BH = int(214 * ss), int(150 * ss)
    CH, DH = int(196 * ss), int(168 * ss)

    tb = panel(lay, LX, BY, LW, AH, ss, th, "分层架构", "L0 → L4")
    draw_layers_panel(lay, LX, BY, LW, AH, ss, th, tb)
    tb = panel(lay, LX, BY + AH + int(16 * ss), LW, BH, ss, th, "设计令牌 16 色", "fixed")
    draw_tokens_panel(lay, LX, BY + AH + int(16 * ss), LW, BH, ss, th, tb)
    tb = panel(lay, RX, BY, RW, CH, ss, th, "组件库 153 件", "7 类")
    draw_components_panel(lay, RX, BY, RW, CH, ss, th, tb)
    tb = panel(lay, RX, BY + CH + int(16 * ss), RW, DH, ss, th, "接入与交付", "2 modes")
    draw_integration_panel(lay, RX, BY + CH + int(16 * ss), RW, DH, ss, th, tb)

    draw_palette_footer(lay, ss, th, int(690 * ss))

    base = Image.alpha_composite(base, lay)

    # ---- 暗角 + 缩回目标尺寸
    vig = Image.new("L", (w, h), 0)
    ImageDraw.Draw(vig).ellipse([-int(0.16 * w), -int(0.28 * h), int(1.16 * w), int(1.28 * h)], fill=255)
    vig = vig.filter(ImageFilter.GaussianBlur(90 * ss))
    dark = Image.new("RGBA", (w, h), (0, 0, 0, 74))
    dark.putalpha(Image.eval(vig, lambda v: int((255 - v) * 74 / 255)))
    base = Image.alpha_composite(base, dark)

    img = base.convert("RGB").resize((BASE_W, BASE_H), Image.LANCZOS)
    os.makedirs(os.path.dirname(os.path.abspath(out_path)), exist_ok=True)
    img.save(out_path, "PNG", optimize=True)
    print(f"[OK] {out_path}  {img.size[0]}x{img.size[1]}  {os.path.getsize(out_path):,} bytes  "
          f"({time.time() - t0:.2f}s, ss={ss})")
    return out_path


def main(argv=None):
    ap = argparse.ArgumentParser(description="RUIF 封面程序化生成器（框架能力信息图）")
    ap.add_argument("--out", default=os.path.join(RUIF_ROOT, "About", "Preview.png"))
    ap.add_argument("--ss", type=int, default=2, help="超采样倍率（默认 2）")
    a = ap.parse_args(argv)
    build(a.out, max(1, a.ss))
    return 0


if __name__ == "__main__":
    sys.exit(main())
