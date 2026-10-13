"""
pe_cli_read.py —— 直读 PE/CLI 元数据，不加载程序集（无反射 LoadFrom）。
用途：为 RUIF 发布门禁提供「离线可执行」的 dll 事实来源：
  * TypeDef 行数 / public 顶级+nested 计数
  * Assembly 表版本号（替代 Assembly.GetName().Version）
  * AssemblyRef 表引用程序集名（替代 GetReferencedAssemblies()）
用法： python pe_cli_read.py <dll>
"""
import struct, sys

def read_pe_metadata(path):
    data = open(path, 'rb').read()
    assert data[:2] == b'MZ', 'not PE'
    pe_off = struct.unpack_from('<I', data, 0x3C)[0]
    assert data[pe_off:pe_off+4] == b'PE\0\0', 'no PE sig'
    coff = pe_off + 4
    num_sections = struct.unpack_from('<H', data, coff+2)[0]
    size_opt = struct.unpack_from('<H', data, coff+16)[0]
    opt = coff + 20
    magic = struct.unpack_from('<H', data, opt)[0]
    if magic == 0x10B:      # PE32
        dd_off, num_dd_off = opt + 96, opt + 92
    elif magic == 0x20B:    # PE32+
        dd_off, num_dd_off = opt + 112, opt + 108
    else:
        raise Exception('bad magic 0x%x' % magic)
    sections = []
    sec = opt + size_opt
    for i in range(num_sections):
        o = sec + i*40
        name = data[o:o+8].rstrip(b'\0').decode('latin1')
        vsize, vaddr, rsize, raddr = struct.unpack_from('<IIII', data, o+8)
        sections.append((vaddr, vsize, raddr, rsize))
    def rva_to_off(rva):
        for vaddr, vsize, raddr, rsize in sections:
            if vaddr <= rva < vaddr + max(vsize, rsize):
                return raddr + (rva - vaddr)
        raise Exception('rva 0x%x not mapped' % rva)
    # CLI header = data dir 14
    cli_rva, cli_size = struct.unpack_from('<II', data, dd_off + 14*8)
    cli = rva_to_off(cli_rva)
    md_rva, md_size = struct.unpack_from('<II', data, cli + 8)
    md = rva_to_off(md_rva)
    assert data[md:md+4] == b'BSJB', 'no metadata sig'
    ver_len = struct.unpack_from('<I', data, md+12)[0]
    p = md + 16 + ((ver_len + 3) & ~3)
    flags, nstreams = struct.unpack_from('<HH', data, p)
    p += 4
    streams = {}
    for i in range(nstreams):
        soff, ssize = struct.unpack_from('<II', data, p)
        p += 8
        end = data.index(b'\0', p)
        sname = data[p:end].decode('latin1')
        p = end + 1
        p = (p + 3) & ~3
        streams[sname] = (md + soff, ssize)
    t = streams['#~'][0]
    heap_sizes = data[t+6]
    valid = struct.unpack_from('<Q', data, t+8)[0]
    p = t + 24
    rowcount = {}
    for i in range(64):
        if valid & (1 << i):
            rowcount[i] = struct.unpack_from('<I', data, p)[0]
            p += 4
    size_S = 4 if heap_sizes & 0x01 else 2
    size_G = 4 if heap_sizes & 0x02 else 2
    size_B = 4 if heap_sizes & 0x04 else 2
    def idx(tbl):
        return 4 if rowcount.get(tbl, 0) >= 65536 else 2
    coded = {
        'TypeDefOrRef': ([2,1,27],2), 'HasConstant': ([4,8,23],2),
        'HasCustomAttribute': ([6,4,1,2,8,9,10,0,14,23,20,17,26,27,32,35,38,39,40,42,44],5),
        'HasFieldMarshal': ([4,8],1), 'HasDeclSecurity': ([2,6,32],2),
        'MemberRefParent': ([2,1,26,6,27],3), 'HasSemantics': ([20,23],1),
        'MethodDefOrRef': ([6,10],1), 'MemberForwarded': ([4,6],1),
        'Implementation': ([38,35,39],2), 'CustomAttributeType': ([6,10],3),
        'ResolutionScope': ([0,26,35,1],2), 'TypeOrMethodDef': ([2,6],1),
    }
    def csize(name):
        tbls, bits = coded[name]
        mx = max([rowcount.get(x, 0) for x in tbls] or [0])
        return 2 if mx < (1 << (16 - bits)) else 4
    schemas = {
        0:[('u2',),('S',),('G',),('G',),('G',)],
        1:[('C','ResolutionScope'),('S',),('S',)],
        2:[('u4',),('S',),('S',),('C','TypeDefOrRef'),('T',4),('T',6)],
        3:[('T',4)], 4:[('u2',),('S',),('B',)],
        5:[('T',6)], 6:[('u4',),('u2',),('u2',),('S',),('B',),('T',8)],
        7:[('T',8)], 8:[('u2',),('u2',),('S',)],
        9:[('T',2),('C','TypeDefOrRef')], 10:[('C','MemberRefParent'),('S',),('B',)],
        11:[('u2',),('C','HasConstant'),('B',)], 12:[('C','HasCustomAttribute'),('C','CustomAttributeType'),('B',)],
        13:[('C','HasFieldMarshal'),('B',)], 14:[('u2',),('C','HasDeclSecurity'),('B',)],
        15:[('u2',),('u4',),('T',2)], 16:[('u4',),('T',4)],
        17:[('B',)], 18:[('T',2),('T',20)], 19:[('T',20)], 20:[('u2',),('S',),('C','TypeDefOrRef')],
        21:[('T',2),('T',23)], 22:[('T',23)], 23:[('u2',),('S',),('B',)],
        24:[('u2',),('T',6),('C','HasSemantics')], 25:[('T',2),('C','MethodDefOrRef'),('C','MethodDefOrRef')],
        26:[('S',)], 27:[('B',)], 28:[('u2',),('C','MemberForwarded'),('S',),('T',26)],
        29:[('u4',),('T',4)], 30:[('u4',),('u4',)], 31:[('u4',)],
        32:[('u4',),('u2',),('u2',),('u2',),('u2',),('u4',),('B',),('S',),('S',)],
        33:[('u4',)], 34:[('u4',),('u4',),('u4',)],
        35:[('u2',),('u2',),('u2',),('u2',),('u4',),('B',),('S',),('S',),('B',)],
        36:[('u4',),('T',35)], 37:[('u4',),('u4',),('u4',),('T',35)],
        38:[('u4',),('S',),('B',)], 39:[('u4',),('u4',),('S',),('S',),('C','Implementation')],
        40:[('u4',),('u4',),('S',),('C','Implementation')], 41:[('T',2),('T',2)],
        42:[('u2',),('u2',),('C','TypeOrMethodDef'),('S',)],
        43:[('C','MethodDefOrRef'),('B',)],
        44:[('T',42),('C','TypeDefOrRef')],
    }
    def colsize(c):
        k = c[0]
        if k == 'u2': return 2
        if k == 'u4': return 4
        if k == 'S':  return size_S
        if k == 'G':  return size_G
        if k == 'B':  return size_B
        if k == 'T':  return idx(c[1])
        if k == 'C':  return csize(c[1])
        raise Exception(k)
    def rowsize(tbl):
        return sum(colsize(c) for c in schemas[tbl])
    strbase = streams['#Strings'][0]
    def sget(i):
        e = data.index(b'\0', strbase + i)
        return data[strbase + i:e].decode('utf-8', 'replace')
    # 定位 Assembly(32) 与 AssemblyRef(35) 与 TypeDef(2) 行偏移
    offsets = {}
    cur = p
    for i in range(64):
        if valid & (1 << i):
            offsets[i] = cur
            cur += rowcount.get(i, 0) * rowsize(i)
    result = {'typedefs': rowcount.get(2, 0)}
    if 32 in offsets and rowcount.get(32, 0) >= 1:
        o = offsets[32]
        r = rowsize(32)
        maj, mi, bld, rev = struct.unpack_from('<HHHH', data, o+4)
        result['assembly_version'] = '%d.%d.%d.%d' % (maj, mi, bld, rev)
        # name 是第 8 列
        cols = schemas[32]
        off = 0
        for c in cols[:7]:
            off += colsize(c)
        nameoff = struct.unpack_from('<I' if size_S == 4 else '<H', data, o+off)[0]
        result['assembly_name'] = sget(nameoff)
    refs = []
    if 35 in offsets:
        o = offsets[35]; r = rowsize(35)
        cols = schemas[35]; off = 0
        for c in cols[:6]:
            off += colsize(c)
        for k in range(rowcount.get(35, 0)):
            ro = o + k*r
            nameoff = struct.unpack_from('<I' if size_S == 4 else '<H', data, ro+off)[0]
            refs.append(sget(nameoff))
    result['assembly_refs'] = refs
    return result

if __name__ == '__main__':
    r = read_pe_metadata(sys.argv[1])
    print('TYPEDEFS =', r['typedefs'])
    print('ASM_VERSION =', r.get('assembly_version'))
    print('ASM_NAME =', r.get('assembly_name'))
    print('REFS =', ', '.join(r['assembly_refs']))
