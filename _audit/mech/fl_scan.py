# 机制研发部：扫描「以 Tokens.FontLabel 为字号的标签落进高度 < 22 的矩形」的候选点
# 用途：量化 B3（Tiny 静默降级为 Small，行高 18->22）的真实影响面
import re, os, collections

ROOT = r'E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF\Source'
SMALL_H = 22.0

files = []
for dp, dn, fn in os.walk(ROOT):
    if os.sep + 'obj' in dp or os.sep + 'bin' in dp:
        continue
    for f in fn:
        if f.endswith('.cs'):
            files.append(os.path.join(dp, f))


def stmts(text):
    """粗糙切句：以 depth==0 的 ';' 为界，跳过注释与字符串。"""
    i = 0; line = 1; depth = 0; start = 0; startline = 1
    out = []
    n = len(text)
    while i < n:
        ch = text[i]
        if ch == '\n':
            line += 1
        elif ch == '/' and text[i:i + 2] == '//':
            j = text.find('\n', i)
            i = j if j >= 0 else n
            continue
        elif ch == '/' and text[i:i + 2] == '/*':
            j = text.find('*/', i + 2)
            line += text.count('\n', i, j if j > 0 else n)
            i = (j + 2) if j > 0 else n
            continue
        elif ch == '"':
            i += 1
            while i < n and text[i] != '"':
                if text[i] == '\\':
                    i += 1
                i += 1
        elif ch == '(':
            if depth == 0:
                start = i; startline = line
            depth += 1
        elif ch == ')':
            depth -= 1
            if depth == 0:
                out.append((startline, text[start:i + 1]))
        elif ch == ';' and depth == 0:
            start = i; startline = line
        i += 1
    return out


def split_args(s):
    parts = []; d = 0; cur = ''
    for c in s:
        if c in '([':
            d += 1
        if c in ')]':
            d -= 1
        if c == ',' and d == 0:
            parts.append(cur.strip()); cur = ''
        else:
            cur += c
    parts.append(cur.strip())
    return parts


hits = collections.defaultdict(list)
for fp in files:
    t = open(fp, encoding='utf-8-sig', errors='replace').read()
    for ln, s in stmts(t):
        if 'Tokens.FontLabel' not in s:
            continue
        flat = re.sub(r'\s+', ' ', s)
        for m in re.finditer(r'new Rect\(', s):
            # 取该 new Rect( 的配对实参串
            i = m.end(); d = 1; j = i
            while j < len(s) and d > 0:
                if s[j] == '(':
                    d += 1
                elif s[j] == ')':
                    d -= 1
                j += 1
            args = split_args(s[i:j - 1])
            if len(args) == 4:
                mm = re.fullmatch(r'(\d+(?:\.\d+)?)f', args[3].strip())
                if mm and float(mm.group(1)) < SMALL_H:
                    hits[fp].append((ln, 'Rect.h=' + args[3].strip(), flat[:170]))
        for m in re.finditer(r'\.Take\(\s*(\d+(?:\.\d+)?)f\s*\)', s):
            if float(m.group(1)) < SMALL_H:
                hits[fp].append((ln, 'Take(%s)' % m.group(1), flat[:170]))

total = 0
for k in sorted(hits):
    rel = os.path.relpath(k, os.path.dirname(ROOT))
    print('## %s  (%d)' % (rel, len(hits[k])))
    for ln, h, s in sorted(hits[k]):
        print('    L%-5d %-14s %s' % (ln, h, s))
    total += len(hits[k])
print('TOTAL 候选点 = %d （分布在 %d 个文件）' % (total, len(hits)))
