# -*- coding: utf-8 -*-
import io, sys, os, re
from collections import Counter
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")
root = r"E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF\Source"
BS = chr(92)   # backslash
c = Counter(); sites = {}
for dp, dn, fs in os.walk(root):
    dn[:] = [d for d in dn if d.lower() not in ("obj", "bin")]
    for fn in fs:
        if not fn.endswith(".cs"):
            continue
        p = os.path.join(dp, fn)
        src = open(p, encoding="utf-8", errors="replace").read()
        for m in re.finditer(r"Tr\.Get\(", src):
            j = m.end(); depth = 1; args = 1
            while j < len(src) and depth > 0:
                ch = src[j]
                if ch in "([":
                    depth += 1
                elif ch in ")]":
                    depth -= 1
                elif ch == "," and depth == 1:
                    args += 1
                if ch == '"':
                    j += 1
                    while j < len(src) and src[j] != '"':
                        if src[j] == BS:
                            j += 1
                        j += 1
                j += 1
            line = src[:m.start()].count("\n") + 1
            c[args] += 1
            sites.setdefault(args, []).append(f"{fn}:{line}")
print("Tr.Get 实参个数分布:", dict(sorted(c.items())))
for a in sorted(sites):
    if a >= 3:
        print(f"  {a} 参 -> {sites[a]}")
