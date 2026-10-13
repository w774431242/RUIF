import re, glob, io, sys
# wide 口径：CJK 表意 + 全角中文标点（： 「 」 （ ） 、 ， 。 ？ ！）
lo = chr(0x4e00); hi = chr(0x9fff)
punct = ''.join(chr(c) for c in (0x3001, 0x3002, 0x300c, 0x300d, 0xff08, 0xff09, 0xff0c, 0xff1a, 0xff1f, 0xff01))
pat = re.compile('[' + lo + '-' + hi + punct + ']')
strlit = re.compile(r'"([^"\\]*(?:\\.[^"\\]*)*)"')
hits = []
for f in glob.glob('Source/**/*.cs', recursive=True):
    if '/obj/' in f or '/bin/' in f:
        continue
    for i, ln in enumerate(io.open(f, encoding='utf-8').read().splitlines(), 1):
        code = re.sub(r'//.*$', '', ln)
        st = code.strip()
        if st.startswith('*') or st.startswith('/*'):
            continue
        for m in strlit.finditer(code):
            if pat.search(m.group(1)):
                hits.append((f, i, m.group(1)))
print("WIDE_TOTAL_LITERALS=", len(hits))
for h in hits:
    print("%s:%d: %s" % h)
