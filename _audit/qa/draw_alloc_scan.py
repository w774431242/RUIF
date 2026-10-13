import re, glob, io
from collections import Counter

pats = [
    ('Array.ConvertAll', r'Array\.ConvertAll\s*\('),
    ('ToString', r'\.ToString\s*\('),
    ('string.Join', r'string\.Join\s*\('),
    ('SplitLines', r'\bSplitLines\s*\('),
    ('new-container', r'new\s+(?:List|Dictionary|HashSet|Queue|Stack)<'),
    ('LINQ', r'\.(?:Select|Where|ToArray|ToList|OrderBy)\s*\('),
    ('Enumerable', r'Enumerable\.'),
]

files = glob.glob('Source/**/*.cs', recursive=True)
files = [f for f in files if '/obj/' not in f and '/bin/' not in f]
hits = []
for f in files:
    rel = f.replace(chr(92), '/')
    for i, ln in enumerate(io.open(f, encoding='utf-8').read().splitlines(), 1):
        code = re.sub(r'//.*$', '', ln)
        st = code.strip()
        if st.startswith('*') or st.startswith('/*'):
            continue
        for name, p in pats:
            if re.search(p, code):
                hits.append((rel, i, name))

print("DRAW_ALLOC_SITES_TOTAL =", len(hits))
c = Counter(h[2] for h in hits)
for k, v in c.most_common():
    print("  %-18s %d" % (k, v))
print("-- files --")
cf = Counter(h[0] for h in hits)
for k, v in cf.most_common():
    print("  %-42s %d" % (k, v))
print("-- sites --")
for h in hits:
    print("  %s:%d  %s" % h)
