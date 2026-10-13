# -*- coding: utf-8 -*-
"""RUIF GATE-LOC 审计：键对称 / 重复 / 空值 / 占位符 / 双向引用"""
import re, sys, io, os, json
import xml.etree.ElementTree as ET

ROOT = r"E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF"
EN = os.path.join(ROOT, "Languages", "English", "Keyed", "RUIF_Components.xml")
ZH = os.path.join(ROOT, "Languages", "ChineseSimplified", "Keyed", "RUIF_Components.xml")

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")

def parse(path):
    """按出现顺序返回 [(key, value, line)]，并检出重复键（ET 会静默去重，故用正则按行取）。"""
    with open(path, "r", encoding="utf-8") as f:
        lines = f.readlines()
    items = []
    dups = []
    seen = {}
    tag_re = re.compile(r"<([A-Za-z0-9_.]+)>(.*?)</\1>\s*$")
    for i, ln in enumerate(lines, 1):
        m = tag_re.search(ln.rstrip("\n"))
        if not m:
            continue
        k, v = m.group(1), m.group(2)
        if k in seen:
            dups.append((k, seen[k], i))
        else:
            seen[k] = i
        items.append((k, v, i))
    # ET 良构校验
    wellformed = True
    err = None
    try:
        ET.parse(path)
    except Exception as e:
        wellformed = False
        err = str(e)
    return items, dups, wellformed, err

def ph(v):
    return sorted(re.findall(r"\{(\d+)\}", v))

en_items, en_dups, en_wf, en_err = parse(EN)
zh_items, zh_dups, zh_wf, zh_err = parse(ZH)

en = {k: (v, l) for k, v, l in en_items}
zh = {k: (v, l) for k, v, l in zh_items}

print("=" * 70)
print("【A】语言文件基本形态")
print("=" * 70)
print(f"EN 键数 = {len(en_items)}  唯一键 = {len(en)}  重复 = {len(en_dups)}  良构 = {en_wf} {en_err or ''}")
print(f"ZH 键数 = {len(zh_items)}  唯一键 = {len(zh)}  重复 = {len(zh_dups)}  良构 = {zh_wf} {zh_err or ''}")
if en_dups:
    print("  EN 重复键:", en_dups)
if zh_dups:
    print("  ZH 重复键:", zh_dups)

print()
print("=" * 70)
print("【B】键集合对称性（字面比对）")
print("=" * 70)
only_en = sorted(set(en) - set(zh))
only_zh = sorted(set(zh) - set(en))
print(f"仅 EN 有（ZH 缺失）: {len(only_en)} {only_en}")
print(f"仅 ZH 有（EN 缺失）: {len(only_zh)} {only_zh}")
print(f"交集 = {len(set(en) & set(zh))}")
print("对称性判定:", "PASS" if not only_en and not only_zh and len(en) == len(zh) else "BLOCKED")

print()
print("=" * 70)
print("【C】空值 / 纯空白值")
print("=" * 70)
empty = []
for name, d in (("EN", en), ("ZH", zh)):
    for k, (v, l) in d.items():
        if v.strip() == "":
            empty.append(f"{name}:{k} (line {l}) value={v!r}")
print("空值命中:", len(empty))
for e in empty:
    print("  ", e)

print()
print("=" * 70)
print("【D】占位符一致性（{0}/{1} 数量与序号）")
print("=" * 70)
mismatch = []
ph_issues = []
for k in sorted(set(en) & set(zh)):
    pe, pz = ph(en[k][0]), ph(zh[k][0])
    if pe != pz:
        mismatch.append((k, pe, pz, en[k][1], zh[k][1]))
    # 序号连续性检查
    for tag, (v, l) in (("EN", en[k]), ("ZH", zh[k])):
        nums = [int(x) for x in ph(v)]
        if nums and nums != list(range(len(nums))):
            ph_issues.append(f"{tag}:{k} 序号不连续 {ph(v)} (line {l}) value={v!r}")
print("占位符错配数:", len(mismatch))
for k, pe, pz, le, lz in mismatch:
    print(f"  {k}: EN{pe}(L{le}) vs ZH{pz}(L{lz})")
print("序号异常:", len(ph_issues))
for p in ph_issues:
    print("  ", p)

print()
print("=" * 70)
print("【E】命名空间分布")
print("=" * 70)
from collections import Counter
def ns(k):
    parts = k.split(".")
    # RUIF.Comp.Xxx.Yyy -> RUIF.Comp.Xxx
    return ".".join(parts[:3]) if len(parts) >= 3 else ".".join(parts)
cnt = Counter(ns(k) for k in en)
for n, c in sorted(cnt.items()):
    print(f"  {n}: {c}")

print()
print("=" * 70)
print("【F】源码引用扫描（双向）")
print("=" * 70)
SRC_DIRS = [os.path.join(ROOT, "Source", "Core"),
            os.path.join(ROOT, "Source", "Layout"),
            os.path.join(ROOT, "Source", "Components")]
# 排除产物目录
def is_prod(p):
    return ("\\obj\\" in p) or ("\\bin\\" in p) or p.endswith(".xml")

lit_re = re.compile(r'"((?:RUIF|MyMod)\.[A-Za-z0-9_.]+)"')
plus_ident_re = re.compile(r'Tr\.Get\(([A-Za-z_][A-Za-z0-9_]*)\s*(?:,|\))')
refs = {}   # key -> [file:line]
dynamic = []  # 非字面量键（变量/常量）引用点
for d in SRC_DIRS:
    for dirpath, _, files in os.walk(d):
        if is_prod(dirpath):
            continue
        for fn in files:
            if not fn.endswith(".cs"):
                continue
            p = os.path.join(dirpath, fn)
            with open(p, "r", encoding="utf-8", errors="replace") as f:
                for i, ln in enumerate(f, 1):
                    for m in lit_re.finditer(ln):
                        refs.setdefault(m.group(1), []).append(f"{p}:{i}")

# 动态键：Tr.Get(SOMETHING) 形式
for d in SRC_DIRS:
    for dirpath, _, files in os.walk(d):
        if is_prod(dirpath):
            continue
        for fn in files:
            if not fn.endswith(".cs"):
                continue
            p = os.path.join(dirpath, fn)
            with open(p, "r", encoding="utf-8", errors="replace") as f:
                for i, ln in enumerate(f, 1):
                    if "Tr.Get(" in ln and '"' not in ln.split("Tr.Get(", 1)[1][:40]:
                        pass  # 交由下方统一判定
# 更稳的：抓取 Tr.Get( 后第一个实参
dyn_re = re.compile(r'Tr\.Get\(\s*([^")][^,)]*?)\s*[,)]')
for d in SRC_DIRS:
    for dirpath, _, files in os.walk(d):
        if is_prod(dirpath):
            continue
        for fn in files:
            if not fn.endswith(".cs"):
                continue
            p = os.path.join(dirpath, fn)
            with open(p, "r", encoding="utf-8", errors="replace") as f:
                for i, ln in enumerate(f, 1):
                    for m in dyn_re.finditer(ln):
                        arg = m.group(1).strip()
                        if arg.startswith('"'):
                            continue
                        dynamic.append(f"{p}:{i}  Tr.Get({arg}...)")

print(f"源码中不同键引用数 = {len(refs)}")
print(f"总引用点 = {sum(len(v) for v in refs.values())}")
print(f"动态（非常量字面量）键引用点 = {len(dynamic)}")
for x in dynamic[:40]:
    print("  DYN", x)

print()
print("--- F1. 语言文件有键、源码未引用（死键）---")
dead = sorted(set(en) - set(refs))
print(f"死键数 = {len(dead)}")
for k in dead:
    print(f"  {k}  ")
    print(f"      EN(L{en[k][1]}): {en[k][0]!r}")
    print(f"      ZH(L{zh[k][1]}): {zh[k][0]!r}")

print()
print("--- F2. 源码引用、语言文件缺键（悬空引用）---")
dangling = sorted(set(refs) - set(en))
print(f"悬空引用数 = {len(dangling)}")
for k in dangling:
    print(f"  {k}: {refs[k]}")

print()
print("--- F3. 全量键引用清单（键 -> 引用点）---")
for k in sorted(refs):
    print(f"  {k}  <=  {', '.join(refs[k])}")
