# 机制研发部：从 api_count.txt（反射实测）机械复算「组件」各口径，为门禁断言提供可复现表述
import re, collections

PATH = r'E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF\_audit\mech\api_count.txt'
rows = []
started = False
for ln in open(PATH, encoding='utf-8-sig', errors='replace'):
    ln = ln.rstrip('\n')
    if ln.startswith('--- 类型清单'):
        started = True; continue
    if not started or not ln.strip():
        continue
    parts = ln.split('\t')
    if len(parts) != 3:
        continue
    rows.append((parts[0], parts[1], int(parts[2])))

top = [r for r in rows if '.' not in r[0][len('RUIF.'):]]
nested = [r for r in rows if r not in top]
print('公开类型（含嵌套）      :', len(rows))
print('  顶级 public           :', len(top))
print('  嵌套 public           :', len(nested))

enumvals = 0
for t, kind, mc in rows:
    if kind == 'enum':
        enumvals += mc
print('枚举值（= 各 enum 成员数）:', enumvals)
print('公共方法合计            :', sum(r[2] for r in rows))

# 口径：组件类 = 名字以 Atoms 结尾的静态类 + Charts/Shapes/Industrial（非 Fmt）
COMP = lambda n: n.endswith('Atoms') or n in ('RUIF.Charts', 'RUIF.Shapes', 'RUIF.Industrial')
comp = [r for r in rows if COMP(r[0])]
print()
print('--- 组件口径 ---')
print('组件族（类数，含嵌套结构体所在的外层）:',
      len({r[0] for r in comp}))
print('组件类公共方法合计（= 口径 A 宽）    :', sum(r[2] for r in comp))
# 去掉明显是「布局辅助」的方法：这里只做提示，不硬编码
big = sorted(comp, key=lambda r: -r[2])
print('  组件类方法榜前 12:', ', '.join('%s=%d' % (r[0].replace('RUIF.', ''), r[2]) for r in big[:12]))

# 组件族归并（去掉 B/C 后缀候选族）
fam = collections.Counter()
for t, kind, mc in comp:
    n = t.replace('RUIF.', '')
    n = re.sub(r'(Cand|Ex|Plus)?(Atoms)(B|C)?$', '', n) or n
    fam[n] += mc
print()
print('归并后的族（去 Cand/Ex/Plus 与 B/C 后缀）:', len(fam))
for k, v in sorted(fam.items(), key=lambda kv: -kv[1]):
    print('   %-18s %d' % (k, v))
