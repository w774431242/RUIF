import re, glob, io

cjk = re.compile('[\u4e00-\u9fff]')
strlit = re.compile(r'"([^"\\]*(?:\\.[^"\\]*)*)"')
hits = []
user_facing = 0
for f in glob.glob('Source/**/*.cs', recursive=True):
    if '/obj/' in f or '/bin/' in f:
        continue
    lines = io.open(f, encoding='utf-8').read().splitlines()
    for i, ln in enumerate(lines, 1):
        code = re.sub(r'//.*$', '', ln)
        st = code.strip()
        if st.startswith('*') or st.startswith('/*'):
            continue
        for m in strlit.finditer(code):
            if cjk.search(m.group(1)):
                hits.append((f, i, m.group(1)))
print("TOTAL_LITERALS=", len(hits))
for h in hits:
    print("%s:%d: %s" % h)
