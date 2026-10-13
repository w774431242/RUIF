# -*- coding: utf-8 -*-
"""RUIF 硬编码中文扫描：只判「字符串字面量里的中文」= 违规；注释里的中文 = 正常。
输出逐处命中（文件:行号 + 字符串原文）。"""
import os, re, sys, io

ROOT = r"E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF"
SRC = os.path.join(ROOT, "Source")
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")

CJK = re.compile(r"[\u4e00-\u9fff\u3000-\u303f\uff00-\uffef]")

def scan_file(path):
    """状态机：逐字符走，剥离注释，抽取字符串字面量（含 @ 与 $ 前缀、逐字串）。"""
    with open(path, "r", encoding="utf-8", errors="replace") as f:
        s = f.read()
    n = len(s)
    i = 0
    line = 1
    lits = []      # (line, raw_literal, content)
    while i < n:
        c = s[i]
        if c == "\n":
            line += 1
            i += 1
            continue
        # 注释
        if c == "/" and i + 1 < n and s[i+1] == "/":
            while i < n and s[i] != "\n":
                i += 1
            continue
        if c == "/" and i + 1 < n and s[i+1] == "*":
            i += 2
            while i + 1 < n and not (s[i] == "*" and s[i+1] == "/"):
                if s[i] == "\n":
                    line += 1
                i += 1
            i += 2
            continue
        # 字符字面量
        if c == "'":
            i += 1
            while i < n and s[i] != "'":
                if s[i] == "\\":
                    i += 1
                i += 1
            i += 1
            continue
        # 字符串字面量
        if c == '"' or (c in "@$" and i + 1 < n and (s[i+1] == '"' or (s[i+1] in "@$" and i + 2 < n and s[i+2] == '"'))):
            start_line = line
            j = i
            verbatim = False
            while j < n and s[j] in "@$":
                if s[j] == "@":
                    verbatim = True
                j += 1
            if j >= n or s[j] != '"':
                i += 1
                continue
            j += 1
            buf = []
            while j < n:
                ch = s[j]
                if ch == "\n":
                    line += 1
                if verbatim:
                    if ch == '"' and j + 1 < n and s[j+1] == '"':
                        buf.append('"')
                        j += 2
                        continue
                    if ch == '"':
                        j += 1
                        break
                    buf.append(ch)
                    j += 1
                else:
                    if ch == "\\" and j + 1 < n:
                        buf.append(s[j:j+2])
                        j += 2
                        continue
                    if ch == '"':
                        j += 1
                        break
                    buf.append(ch)
                    j += 1
            raw = s[i:j]
            lits.append((start_line, raw, "".join(buf)))
            i = j
            continue
        i += 1
    return lits

hits = []
total_files = 0
for dirpath, dirnames, files in os.walk(SRC):
    dirnames[:] = [d for d in dirnames if d.lower() not in ("obj", "bin")]
    for fn in files:
        if not fn.endswith(".cs"):
            continue
        total_files += 1
        p = os.path.join(dirpath, fn)
        for ln, raw, content in scan_file(p):
            if CJK.search(content):
                hits.append((p, ln, raw.strip(), content.strip()))

print(f"扫描 .cs 文件数 = {total_files}")
print(f"字符串字面量含中文（硬编码候选）= {len(hits)}")
print("-" * 100)
for p, ln, raw, content in hits:
    rel = os.path.relpath(p, ROOT)
    print(f"{rel}:{ln}")
    print(f"    RAW : {raw}")
    print(f"    TEXT: {content}")
