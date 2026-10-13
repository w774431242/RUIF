# 【表现层与本地化审计报告】GATE-UI / GATE-LOC
审计对象：RUIF v1.1.2（开发根 `…\RimWorld\11\RUIF`，发布区 `…\RimWorld\Mods\RUIF`）
审计人：界面与本地化部 · 颜一寸　　纪律：只读；全部结论附 `文件:行号` 或命令原文
证据脚本/落盘：`_audit/loc_audit.py` `_audit/hardcode_scan.py` `_audit/tr_arity.py`
`_audit/hc_now.txt`（health_check 实跑）`_audit/verify_release_now.txt`（发布门禁实跑）

---

## 1. 判定总表

| # | 检查项 | 四态 | 证据摘要 |
|---|---|---|---|
| 1 | 中英键集合字面对称 | **PASS** | 各 60 键、交集 60、仅 EN 0 / 仅 ZH 0（`_audit/loc_audit.py` §B） |
| 2 | 重复键 / 空值 / 良构 | **PASS** | 重复 0、空值 0、XML 良构；`verify_release.ps1` 第 9 项 PASS |
| 3 | 占位符一致性（`{0}/{1}`） | **PASS** | 错配 0、序号不连续 0；`verify_release` 第 9 项「比对 60 键」PASS |
| 4 | 死键 / 悬空引用（双向） | **PASS** | 死键 0；悬空 4 处经逐一核实**全为误报**（§2.3） |
| 5 | **源码中文硬编码** | **BLOCKED** | health_check 实跑 **24 处 / 8 文件**；独立宽口径 **26 处 / 9 文件**；README §2 / About.xml 宣称「零硬编码」 |
| 6 | 绘制作用域与全局状态泄漏 | **PASS WITH EXCEPTION** | 全局写点仅 `Core/Draw.cs`；29/29 作用域走 `using`；**3 处裸 `GUI.BeginGroup/EndGroup` 无 try/finally** |
| 7 | 组件零状态 | **PASS WITH EXCEPTION** | `NavAtoms.cs:123` `NavAtoms.cs:217` 两个 `static` 可变字段 |
| 8 | 组件禁读游戏状态 | **PASS WITH EXCEPTION** | `PawnTableAtoms.cs:110 Find.CurrentMap`（1 处） |
| 9 | 设计令牌集中度 | **PASS WITH EXCEPTION** | 8 处越白名单 `new Color`（其中 `TexFmt.Heat` 5 色为**真缺口**）；`Measure` 全库**仅 1 处**使用 |
| 10 | **随包文档健康（DevDocs）** | **BLOCKED** | `终极宝典v1.0-fixed.html` 93 处描述已移除的 Theme 层；5 份旧版体检报告随包 |
| 11 | 封面图与定位一致性 | **PASS WITH EXCEPTION** | 不描述主题层（定位 OK）；但图上 `v1.0.1 / 73 TYPES / 435 MEMBERS` 全陈旧 |
| 12 | 脚本漂移与门禁假信号 | **BLOCKED** | `gen_preview.py` **当前不可运行**且事实陈旧；`health_check.ps1` 白名单陈旧（8 误报）+ 口径低报；门禁**无硬编码阻断项** |

**Gate 判定**
| Gate | 四态 | 证据 |
|---|---|---|
| **GATE-UI** | **BLOCKED** | §5 裸 BeginGroup 栈风险（3 处）+ §6 `TexFmt.Heat` 色板出 `Tokens` + §6 组件读游戏状态 + §3 beginGroup 之外见 §6 |
| **GATE-LOC** | **BLOCKED** | §3 用户可见中文硬编码 21 处（GOV-010）+ 文档宣称「零硬编码」与实测矛盾 |
| GATE-LAND | **N/A** | 本轮未派发 1:1 落地比对任务，且无实机截图；不伪造判定 |

---

## 2. 本地化键对称性与死键（双向扫描）

### 2.1 命令（可复核）
```bash
python RUIF/_audit/loc_audit.py                                   # 键对称/重复/空值/占位符/双向引用
pwsh -File RUIF/_scripts/verify_release.ps1                       # 门禁第 4/5/9 道
pwsh -File RUIF/_scripts/health_check.ps1                         # 体检第 4 组
```
`loc_audit.py` 不与 `[xml]` 的静默去重同源：按行正则抓键，**重复键可检出**（`verify_release` 用 `[xml]` 会丢失重复信息，靠独立正则补）。

### 2.2 结果
| 项 | EN | ZH |
|---|---|---|
| 键条目 | 60 | 60 |
| 唯一键 | 60 | 60 |
| 重复键 | 0 | 0 |
| 空值/纯空白 | 0 | 0 |
| 良构 | ✅ | ✅ |
| 仅本方有 | 0 | 0 |

对称性判定：**PASS**（字面一致，非仅数量相等）。命名空间分布：`RUIF.Comp.Common 20 · Nav 6 · Relation 5 · Relative 4 · Bill 3 · Density 3 · …` 等 19 个子命名空间，**全部落在 `RUIF.Comp.*`**。

### 2.3 死键 / 悬空引用
- **死键 0**（60 键全部被源码引用）。
- 悬空引用 4 处，**逐处核实后全为误报**：
  | 命中 | 真实身份 | 证据 |
  |---|---|---|
  | `RUIF.CommandPalette.Query` @ `NavAtoms.cs:306` | **GUI 控件名**（`GUI.SetNextControlName`），非翻译键 | `private const string CtrlName = "RUIF.CommandPalette.Query";` |
  | `RUIF.FocusSearch.Field` @ `NavAtomsB.cs:261` | 同上（`GUI.GetNameOfFocusedControl`） | `private const string SearchCtrl = "RUIF.FocusSearch.Field";` |
  | `RUIF.Panel.Research` @ `HiAtoms.cs:24` | **XML 文档注释里的示例 tag**（`UIHighlighter`） | `<summary>…tag 用稳定字符串，如 <c>"RUIF.Panel.Research"</c>）</summary>` |
  | `RUIF.Tests` @ `AssemblyInfo.cs:17` | **程序集名**（`InternalsVisibleTo`），非键 | `[assembly: InternalsVisibleTo("RUIF.Tests")]` |

### 2.4 扫描口径核实（README 提到的「`RUIF.(UI\|Comp).` 精确扫描」）
- 现网口径已收窄为 **`RUIF\.Comp\.`**：`verify_release.ps1:172` `Check-TranslateKeys … 'RUIF\.Comp\.'`；体检第 4 组不依赖命名空间（全量字面量扫描）。
- **口径覆盖评估**：当前 60 键**全部**落在 `RUIF.Comp.*`，故现有口径对这个键空间是**完整**的（我独立全量扫描也是 60）。
- **风险（非当前缺陷）**：若将来新增非 `RUIF.Comp.` 前缀的键，门禁第 5 道会静默漏判（该道只认该命名空间 + `"X".Translate()` 直呼式）。建议把口径改为「白名单前缀 + `Tr.Get` 实参抽取」双轨。
- 门禁第 5 道的**死键审计仅为 WARN**（`verify_release.ps1:167-170`），不阻断。当前死键 0，暂无实害。

---

## 3. 硬编码残留逐处清单（EXC-002 独立复核）

### 3.1 结论：**不是 0**。EXC-002 已回归。
| 口径 | 处数 | 文件数 |
|---|---|---|
| `health_check.ps1` 实跑（第 4 组） | **24** | 8 |
| 我的独立宽口径（含全角标点/`「」`） | **26** | 9 |

命令：`pwsh -File RUIF\_scripts\health_check.ps1` → `_audit/hc_now.txt`
```
== 4. 中文硬编码（会进入运行时的字符串字面量） ==
  [HIT] 组件/内核层中文硬编码 24 处，分布在 8 个文件（用户可见文案应走 Keyed 或由调用方传入）
```

### 3.2 逐处清单（`文件:行号` + 字符串 + 定性）
| 文件:行 | 字符串 | 定性 |
|---|---|---|
| `DataCandAtoms.cs:45` | `"▲优先"` | **用户可见**（账单队列自绘标签） |
| `DataCandAtoms.cs:46` | `"⏸ 暂停"` | **用户可见** |
| `DataCandAtoms.cs:186` | `"我方库存"` | **用户可见**（贸易面板表头） |
| `DataCandAtoms.cs:187` | `"商队提供"` | **用户可见** |
| `DomainCandAtoms.cs:22` | `"DataBridge · 低频采样快照"` | **用户可见**（自绘标题） |
| `FeedbackCandAtoms.cs:63` | `"危险"` / `"警告"` / `"提示"`（×3） | **用户可见**（警报严重度标签） |
| `InputAtoms.cs:220` | `"："`（全角冒号） | **用户可见**（`Draw.Label(rect, label + "：" + state …)`；且 `state` 是 enum 名，未翻译） |
| `InputCandAtoms.cs:95` | `"按下按键…"` | **用户可见**（键位捕获占位符） |
| `LayoutCandAtoms.cs:54` | `"钻取抽屉 · "` | **用户可见** |
| `NavCandAtoms.cs:108` | `"搜索…"` | **用户可见**（搜索框占位） |
| `PawnTableAtoms.cs:61` | `"找不到 PawnTableDef「"` + `"」（Def 名拼写或所属 Mod 未启用）"` | **用户可见**（`LastError`，在 L137 被 `Draw.Label` 画出） |
| `PawnTableAtoms.cs:137` | `"（无可用表格）"` | **用户可见** |
| `PawnTableAtoms.cs:146` | `"PawnTableDef「"` + `"」 · 列 "` | **用户可见**（状态行） |
| `PawnTableAtoms.cs:147` | `" · 行 "` | **用户可见** |
| `PawnTableAtoms.cs:148` | `" · 排序 "` + `"无"` | **用户可见** |
| `PawnTableAtoms.cs:149` | `"　（点表头循环：降 → 升 → 清除；Shift 点击跳过）"` | **用户可见**；`showStatusLine` **默认 true** ⇒ 默认绘制 |
| `TexCache.cs:126` | `" 超出 [0,255]，已夹紧。"` | 开发者诊断（`Log`） |
| `TexCache.cs:143` | `"RUIF.TexCache.KeyOf: 参数 "` + `" 超出 [0,2047]，已夹紧 —— "` | 开发者诊断 |
| `TexCache.cs:144` | `"夹紧后可能与其它值共用键；请减小量化步长或扩展位段。"` | 开发者诊断 |
| `TexCache.cs:193` | `"RUIF.TexCache: 尝试在非主线程创建贴图（已忽略）。"` | 开发者诊断 |

**用户可见违规 = 21 处**（26 − 5 条 `TexCache` 日志）。`health_check` 口径下为 19 处。

### 3.3 两条独立佐证
1. **交付面确认**：`*Cand*` 组件确在交付目录内（`DevDocs/组件库全量目录.md:42-84,100,149` 均标 `✅ 已落地`），非死代码 ⇒ 其自绘中文即随包 UI 文案。
2. **「0」的来源是陈旧快照**：仓库根残留 `_hc_out.txt`（旧版 73 类型口径的运行输出）第 14 行 `[OK ] 框架源码无中文硬编码文案`；而当前实跑为 24。README §2 L26 / About.xml L18、L31 沿用「源码零硬编码」表述。

### 3.4 回归路径（为什么门禁没拦住）
- **`verify_release.ps1`（发布门禁）根本不含硬编码检查项**：9 道为版本/引用/哈希/键对称/键覆盖/卫生/开发者包/XML良构/占位符 —— **无一道扫描硬编码**。实跑 exit 0 全 PASS（`_audit/verify_release_now.txt`）。
- `health_check.ps1` 第 4 组虽命中，但脚本**退出码恒 0**（`[体检完成] 命中待办项合计 5 组`），HIT 仅提示不阻断。
⇒ **「零硬编码」是一条无阻断门的宣称**，这是 EXC-002 能悄悄回归的机制根因。

### 3.5 `health_check` 第 4 组的**口径低报**（24 应为 26）
- `health_check.ps1:188` 正则仅认 `\p{IsCJKUnifiedIdeographs}`（U+4E00–9FFF）：
  - 漏 `InputAtoms.cs:220` `"："`（U+FF1A，全角）
  - 漏 `PawnTableAtoms.cs:146` `"PawnTableDef「"`（U+300C `「`，CJK 标点）
- 复刻比对：`health_check口径=24` vs `宽口径=26`（`_audit/hardcode_scan.py`）。
- 另：`Strip-Comment`（`health_check.ps1:29-35`）在引号数为奇数时**不剥注释**，对含 `//` 的注释行可能漏判。

---

## 4. 占位符一致性
- `verify_release.ps1` 第 9 道实跑：`[PASS] RUIF.Components 占位符一致（比对 60 键）`、`无重复键`。
- 独立复核（`loc_audit.py` §D）：**错配 0**、**序号不连续 0**。
- 带参键共 **12 个**（`Undo.Done{0}` / `Common.More{0}` / `Common.Page{0}{1}` / `Common.Wheel{0}` / `Nav.StackDepth{0}` / `Hotkey.Conflict{0}` / `Industrial.Bottleneck{0}{1}` / `Points.Remaining{0}` / `Points.Overdrawn{0}` / `Power.Net{0}` / `Wealth.Total{0}` / `Tale.Spread{0}` / `Bill.*`（无参） / `Title.Granted{0}` / `Ideo.Members{0}` / `Ideo.Leader{0}` / `Caravan.Food{0}{1}` / `Policy.AppliedTo{0}` / `Eta.Remaining{0}` / `Relative.{Minutes,Hours,Days}{0}`），中英**逐一成对且序号一致**。
- 无「英文有 `{0}` 中文没有」的错配。
- **附带**：`Tr.cs:64` 的**三参重载**（`Get(key, a1, a2, a3)`）经实参个数分布分析（`_audit/tr_arity.py`：`{1:40, 2:19, 3:3}`）**无任何调用点 = 死代码**，且其注释称唯一使用点是「设置页的 XML 配置来源行」——该设置页已随主题/设置层删除（`DevDocs/体检报告_2026-09-12.md:161-162`）⇒ **注释指向已移除功能**。

---

## 5. 绘制作用域与全局状态泄漏点

### 5.1 全局状态写入点清单（**全部**在 `Core/Draw.cs`，被 `using` 作用域包裹）
| 状态 | 写入点 | 恢复点 | 保护 |
|---|---|---|---|
| `GUI.matrix` | `Draw.cs:52` | `Draw.cs:55` `Dispose` | ✅ struct + `using` |
| `GUI.color` | `Draw.cs:89` | `Draw.cs:91` | ✅ |
| `GUI.enabled` | `Draw.cs:98` | `Draw.cs:100` | ✅ |
| `Text.Font` | `Draw.cs:107` | `Draw.cs:109` | ✅ |
| `Text.Anchor` | `Draw.cs:116` | `Draw.cs:118` | ✅ |
| `Text.WordWrap` | `Draw.cs:158` | `Draw.cs:170`（**显式 `try/finally`**） | ✅ |

- `Source/Layout` 与 `Source/Components` 中**零** 直接 `GUI.color= / GUI.enabled= / GUI.matrix= / Text.Anchor= / Text.Font=` 写入（grep 全库唯一命中即 `Draw.cs`）。
- 29 处作用域工厂调用（`Draw.Color/Enabled/Anchor/Font/Rotate`）**100% 位于 `using (...)` 语句上**，无「取了作用域不用」的泄漏点。
- **结论：颜色/字体/锚点/矩阵的「异常路径染色泄漏到全游戏」路径不存在** → PASS。

### 5.2 EXCEPTION：3 处裸 `GUI.BeginGroup/EndGroup` **无 try/finally**（P1 风险）
| 位置 | 配对 | 中间体 |
|---|---|---|
| `NavAtoms.cs:211` → `:213`（`CollapsibleTree`） | 裸配对 | `DrawNode()` 递归 + 每行 `Widgets.ButtonInvisible` |
| `NavAtoms.cs:368` → `:382`（`CommandPalette`） | 裸配对 | n 行循环绘制 + `ButtonInvisible` + 字符串拼接 |
| `NavAtomsB.cs:215` → `:234`（`JumpList`） | 裸配对 | n 行循环 + `Substring(0,1)` + `ButtonInvisible` |

- 风险语义：IMGUI 无自动裁剪，`BeginGroup/EndGroup` 是 **GUIClip 入栈/出栈**。中间任一帧抛异常且未捕获 ⇒ 出栈丢失 ⇒ 日志刷 `You are pushing more GUIClips than you are popping` ⇒ **整局 GUI 卡死（无法行动）**。
- 与框架自身纪律不一致：`Draw.*` 全部用 `using`（编译期展开为 `try/finally`），但这 3 处绕过了作用域机制。
- 建议：为 `Layout` 增加 `using (Layout.Group(rect))` 型 `IDisposable` 作用域，或把这 3 处包 `try { … } finally { GUI.EndGroup(); }`。
- 另：`Layout.BeginScroll/EndScroll`（`Layout.cs:42/50`）同样是**裸配对**，且全库无组件实际使用；消费方一旦在两者之间抛异常同样断栈。建议同步提供 `using (Layout.Scroll(...))`。

### 5.3 EXCEPTION：组件层 2 个 `static` 可变字段（违反「组件零状态」）
| 位置 | 字段 | 性质 |
|---|---|---|
| `NavAtoms.cs:123` | `private static int pendingMorePick = -1;` | **跨帧**状态（FloatMenu lambda 回调写入，下一帧消费）；同一帧内两个 TabBar 实例会**串台** |
| `NavAtoms.cs:217` | `private static TreeNode clicked;` | 单帧中转（`CollapsibleTree` 开头清零、`DrawNode` 内写入）；**不可重入**（嵌套调用互相污染） |

- 注释自述原因（`NavAtoms.cs:103`）："lambda 不能捕 ref 参数，经静态 pending 中转"——属已知妥协，但与 README §1「组件无持久状态」/ 第一原则「组件自身零状态」不一致，且 `pendingMorePick` 会**跨帧存活**。
- 建议：改为「调用方持 `int` 引用 + 显式索引」或把菜单选择回写改为 `ref`/返回值，去掉静态中转。

---

## 6. 设计令牌集中度扫描

### 6.1 结论：主要通道达标，但存在 1 处**真缺口** + 1 处**白名单漂移**
命令：`pwsh -File RUIF\_scripts\health_check.ps1`（第 5 组）+ 独立全库 `new Color` grep。

**全库 `new Color(` 命中（排除 obj）** = 21 处，分布：
| 位置 | 定性 |
|---|---|
| `Tokens.cs:72-102`（16 色） | ✅ 令牌真源 |
| `Draw.cs:19,205`（`ColorScope`/`WithAlpha`） | ✅ 内核（白名单内） |
| `InputExAtoms.cs:61` | ✅ 白名单内（既有标注） |
| `TexFmt.cs:80`（`Ramp` 插值）、`TexAtoms.cs:75,76`（渐隐带端点） | ⚠️ **派生/插值**，非设计色 —— 合理但**未白名单** ⇒ 门禁噪声（B 类） |
| **`TexFmt.cs:96-100`（`Heat()` 5 档色阶）** | ❌ **真缺口**：`冷蓝/青/黄绿/琥珀/红` 5 个硬编码设计色，**不在 `Tokens` 内** |

- `Tokens` 自述「16 色（唯一真源）」、README §6「所有组件绘制一律取此处的令牌」、README §7「颜色一律走 `Tokens`」——**`TexFmt.Heat` 直接违反**：调用点 `TexAtoms.cs:122`（热力格网）绕过令牌取色。建议提升为 `Tokens.Heat0..4` 或 `Tokens.Heat(t01)`。
- **文档互链失效**：`Charts.cs:95` 写「禁裸 `new Color`（例外白名单见 `Tokens.cs` 注释）」、`Industrial.cs:111` 写「颜色只映射令牌（… 禁裸 new Color）」，但 **`Tokens.cs` 内并无任何白名单注释**（grep `白名单|whitelist` 于 `Tokens.cs` = 0 命中）；真正的白名单只存在于 `health_check.ps1:204`。⇒ 第三方开发者按注释去 `Tokens.cs` 找白名单会**找不到**。
- **正面**：`Industrial.cs` 三张色表全部映射令牌（`ZoneColor:251-252 → Alive/Warn/Dead`、`LedColor:272-276`、`StateColor:356-359`），除 `Color.white`（Unity 常量，贴图绘制作用域）外无裸色 ⇒ 达标。

### 6.2 EXCEPTION：度量令牌未被自己的组件遵守
- `Measure` 类提供 `RowHeight/ControlHeight/Width/Fits/Clamp`（`Measure.cs:25-62`），但**全库仅 1 处使用**：`Industrial.cs:239 Measure.Clamp`。
- `Source/Components` 中硬编码尺寸常量 **15 个** `const float`，样例：`NavAtoms.cs:221 RowH = 22f`、`NavAtoms.cs:366 RowH = 24f`、`NavAtomsB.cs:213 RowH = 24f`、`NavAtomsB.cs:21 Gap = 3f`、`NavAtomsB.cs:157 BtnW = 30f`、`NavCandAtoms.cs:20 gap = 2f`、`DataCandFmt.cs:17 TreeOpW = 18f`、`Industrial.cs:123 GaugeLabelBand = 15f`。
- 与「尺寸一律取自框架度量，禁止手写魔数」（GOV-006 / 铁律 1）不一致：框架**提供了** `Measure` 却未在自己的 153 件组件上落实。若后续要按 `Measure.*` 派生并缓存为 `static readonly`，这是一次全组件层重构。

### 6.3 `PawnTableAtoms.cs:110` 组件读游戏状态（红线）
```csharp
public static IEnumerable<Pawn> CurrentMapColonists()
{ Map m = Find.CurrentMap; return m == null ? new Pawn[0] : m.mapPawns.FreeColonists; }
```
- 违反 README §7「组件内**禁读游戏状态**」。`health_check` 第 5 组实跑命中：`PawnTableAtoms.cs:110 Map m = Find.CurrentMap;`。
- 缓解项：该原子自述是「原版框架桥」，且为**默认参数**（`pawnsGetter ?? CurrentMapColonists`）——调用方可显式传入回避。建议：把 `CurrentMapColonists` 移到调用方（或标注为「集成件、非纯组件」并在 README 红线里显式豁免），否则红线应判该项不达标。

---

## 7. 随包文档误导段落清单（DevDocs，`文件:行号`）

devdocs 实跑门禁：`verify_release.ps1` 第 7 道 PASS（6 项必备文件 + BUILD-INFO 1.1.2 + 13 文件同步），**但内容正确性不在门禁范围**。

### 7.1 `终极宝典v1.0-fixed.html`（随包，93 处描述主题层）— **P1 误导**
| 行 | 原文（节选） | 为何误导 |
|---|---|---|
| `:323` | `<span class="s">语义令牌 · 主题变量</span>` | 导航项「主题变量」——框架已无主题变量 |
| `:404` | 「框架地基：设计令牌与绘制原子。对应 RUIF 的 **Theme** / Draw / Layout 三层」 | 断言存在 `RUIF.Theme` 层（已删除） |
| `:425` | 「在 v1.0 Vanilla/Dark 双主题基础上…第三方**主题注册**时一次赋值」 | 断言存在主题注册能力（已删除） |
| `:749` | 「色值取 Quality 令牌组，**跨主题**可辨」 | 同上 |
| `:1210` `:1290` | `主题：` + `id="n-th-dark"/"c-th-vanilla"` 主题切换按钮 | 宝典自带主题切换器，暗示框架支持换肤 |
| `:1313` | 「全部通过**主题令牌**取色，禁 `new Color`」 | 令牌名应为 `Tokens.*` |
| `:1318` | 「**主题令牌**：Dark（默认）/ Vanilla」 | 现无主题档位 |

> 该文件是**随包分发给第三方**的「全量实例与布局配方」主参考（`sync_devdocs.ps1:54` 白名单）。

### 7.2 版本/口径事实错误（随包件）
| 文件:行 | 原文 | 实测 |
|---|---|---|
| `DevDocs/index.html:178` | 「**当前版本**：`v1.1.0`（2026-09-12）… 公共 API 面 **73 类型 / 438 成员**」 | 实际 `v1.1.2`（`About.xml:6`），**114 类型 / 615 成员** |
| `DevDocs/index.html:98` | 「（**v1.1.0** 源码工程内编译通过）」 | 同上 |
| `DevDocs/组件库全量目录.md:4` | 「**113 个公开类型 / 603 个公开成员**」 | 实测 **114 / 615**（`API参考.md` 目录 114 条；561 `public` 成员 + 54 枚举成员） |
| `About/About.xml:17,30` | 「113 类型 / 603 成员」 | 同上，**发布面（Mod 列表 + 创意工坊简介）事实错误** |
| `DevDocs/终极宝典…` / `预览图` | `v1.0.1` | 实际 `v1.1.2` |

### 7.3 版本叙事自相矛盾（随包件之间）
| 出处 | 说法 |
|---|---|
| `DevDocs/index.html:130,167,182` | 「旧换肤层**已于 v1.0.0 公开发布前**整体移除」 |
| `DevDocs/接入指南…html:189-190` | 「旧 `Theme.Register/Apply` 已于 **v1.0.0** 公开发布前整体移除」 |
| `DevDocs/CHANGELOG.md:125` | `## v1.0.1 · 回归纯组件库（移除主题 / 换肤层）` ⇒ 移除**发生在 v1.0.1** |
| `DevDocs/体检报告_v2.2.1.md` / `体检报告_2026-09-11.md` | 描述 v2.2.x **存在**主题/装饰层与 `Defs/RUIFSettings.xml` |
⇒ 「v1.0.0 之前」与「v1.0.1 时移除」与「v2.2.x 仍有」**三者不能同时为真**。

### 7.4 5 份旧版体检报告随包（`sync_devdocs.ps1:62` 按模式全部纳入）
`体检报告_v2.0.0.md`(5 命中) · `体检报告_v2.1.1.md`(8) · `体检报告_v2.2.1.md`(11) · `体检报告_2026-09-11.md`(5) · `体检报告_2026-09-12.md`(8) —— 均以「存在主题层 / `Defs/RUIFSettings.xml`」为语境（例：`体检报告_2026-09-12.md:161-162` 列出已删的 `Source/Theme/`、`Config/RUIFSettings.cs`、`Defs/RUIFSettings.xml`）。
- `体检报告_2026-09-12.md:3` 自标「版本 **v1.0.0**」，与发布区 `v1.1.2` 差 3 个版本。
- 立场：**历史记录可保留**，但应加**显著抬头**「本报告描述已移除的 v2.x 内部迭代线，与当前 v1.1.2 无关」；否则第三方按报告找 `RUIFSettings` 会扑空。

### 7.5 正确的部分（PASS）
- `DevDocs/API参考.md`：`版本 v1.1.2`、类型 114 条、**主题/换肤命中 0** ⇒ **PASS**（与实现同源）。
- `DevDocs/index.html` 的「与旧版的差异」(L130)/「把令牌当可换肤用」(L167)/「不含换肤引擎」(L182)、`零基础上手教程.html:364-366`、`接入指南…html:189`、`组件库全量目录.md:13`：均为**纠正性**表述（明确「无主题/换肤」）⇒ 内容方向正确，仅版本号陈旧。
- `DevDocs/CHANGELOG.md` ↔ `docs/06-发布记录/CHANGELOG.md`：**MD5 完全一致**（`b4ec7dc246b86d8d2cb96365bfa7d532`）⇒ **无双份漂移**（问项答复：同一内容）。
  - 但 CHANGELOG 自身有结构缺陷：**`## v1.0.0` 出现两次**（`:490` 首次公开发布 2026-09-11；`:814` S5 收口 2026-09-09），`v0.3.0-alpha` 13 次；且版本序**倒挂**（`:795 v1.1.0` 排在 `:553 v2.2.0` 之后）。

### 7.6 门禁为何看不见以上（漏报链）
- `health_check.ps1:166` 的「已移除特性残留」只 grep **`Gallery|DevTools|F11`** ⇒ `主题/换肤/Theme/RUIFSettings` 残留**一律漏判**。
- `health_check.ps1:142-145` 的「文档引用的 `RUIF.*` 类型是否存在」只认 **`RUIF.` 带点**写法；`终极宝典:404` 写的是「RUIF **的** Theme」，`:1313` 写「主题令牌」⇒ 全部逃逸。
- `health_check.ps1:148-152` 对**未知类型名直接 `continue`**，只管「文档提到不存在的 API」，不管「文档提到已删除的层」⇒ **方向性盲区**。
- ⇒ 现网 `health_check` 第 3 组输出 `[OK] 不含已移除特性的残留引用（Gallery/DevTools/F11）`，而 93 处 theme 残留安然通过。

---

## 8. 封面图与定位一致性判定

**判定：PASS WITH EXCEPTION（定位相符 / 统计与版本陈旧）**

| 维度 | 结论 | 依据 |
|---|---|---|
| 是否描述已移除能力（换肤/主题截图） | **否** | 读图：封面为「分层架构 / 组件库 153 件 / 设计令牌 16 色 / 接入与交付」四区，无主题色板、无「切换主题」控件、无 skin 截图 |
| 与「纯组件供应、无主题层」定位 | **相符** | 封面文案「纯组件供应」脉络一致；「设计令牌 16 色」区块正是 `Tokens` 的 16 色（仍成立） |
| **版本号** | ❌ 陈旧 | 图上右上角 **`v1.0.1`**；实际 `v1.1.2`（`About.xml:6`） |
| **统计数字** | ❌ 陈旧 | 图上 `73 TYPES · 435 MEMBERS`；实际 **114 类型 / 615 成员** |
| 时间戳 | 2026-09-12 | `About/Preview.png` 339,277 B，与两区同哈希 |

**「取色自已删除的主题预设」说法不成立**：`_scripts/gen_preview.py` 全库对 `主题|换肤|Theme|preset|装饰|decor` 的命中仅 2 处，且均为**否定式**注释——`:168`「扫描线：自绘装饰，**与任何主题无关**」、`:296`「自动对比，**不依赖任何主题**」。取色来自 `gen_preview.py:54` 的**硬编码 `TOKENS` 列表**（自述"取自 `Tokens.cs`"，实为**手工抄写**，违反脚本自述的"数据同源（不手抄颜色与事实）"）。
⇒ README §5 L73「**待更新：旧版取色自已删除的主题预设**」与 §8 L110 属**错误归因**：真正的问题是**硬编码事实陈旧**，不是主题残留。

---

## 9. 脚本漂移与门禁假信号风险

### 9.1 `gen_preview.py` — **当前不可运行**（P2）
| 位置 | 问题 |
|---|---|
| `:40` `VERSION = "v1.0.1"` | 硬编码版本，落后 3 个版本 |
| `:44` `STAT_TYPES, STAT_MEMBERS = 73, 435` | 硬编码统计，与实测 114/615 不符 |
| `:45` `COMPONENTS = [(...)]` | 手工维护的分类计数 |
| `:54-64` `TOKENS = [...]` + `TOK = dict(TOKENS)` | **手抄** 16 色，未读 `Tokens.cs` |
| `:37` `REPO_ROOT = dirname(RUIF_ROOT)` → `FONT_DIR = …\11\Fonts` | **该目录不存在**（`ls` = `No such file or directory`；仓库内亦无 `RUIF/Fonts`） |
| `:214` `load_font("Cinzel-Variable.ttf", …)` | 回退到 `C:\Windows\Fonts\Cinzel-Variable.ttf` —— **该字体不存在**（仅 `consola*/msyh*` 在）⇒ `ImageFont.truetype` 抛错 ⇒ **脚本无法产出封面** |

⇒ 后果：封面**不可复现**、**无法随版本更新**（图上 `v1.0.1` 即证据）；且与 README L5「`_scripts/` 全部以 `$PSScriptRoot` 推导根，**位置无关**」矛盾（`FONT_DIR` 指向仓库外的 `11\Fonts`）。

### 9.2 `health_check.ps1` — 无 theme 逻辑，但**白名单与口径陈旧**（P2）
- **无主题/装饰逻辑**：全库对 `主题|换肤|Theme|装饰|decor|DrawDecor` 的命中仅 `:123` 一处**注释**（解释为何排除历史文档）。⇒ README §8 L110「`health_check.ps1` 仍含主题/装饰相关逻辑」**不实**（§5 L74 已自陈该组"随装饰层一并移除"，两处自相矛盾）。
- **假信号（误报）**：`:204 $colorWhitelist = @('Tokens.cs','Draw.cs','InputExAtoms.cs')` 未纳入后加入的 `TexAtoms.cs`/`TexFmt.cs` ⇒ 实跑误报 8 处 `new Color`（`TexAtoms.cs:75,76`、`TexFmt.cs:80,96-100`），其中仅 `TexFmt.Heat` 5 处是真缺口，另 3 处是合法派生。**HIT 与真缺陷混装**会训练人忽略 HIT。
- **漏报（假阴性）**：
  - 第 4 组正则低报（24 应为 26，见 §3.5）；
  - 第 3 组「已移除特性残留」只认 `Gallery|DevTools|F11` ⇒ 93 处 theme 残留漏判（见 §7.6）；
  - 第 3 组对未知类型名 `continue` ⇒ 无法发现「文档提到已删除的层」。
- **无阻断力**：脚本恒 `exit 0`（实跑 `[体检完成] 命中待办项合计 5 组`），无 `-Blocking` 语义。对比 `verify_release.ps1` 有 `$script:fail` + `exit 1`。

### 9.3 `verify_release.ps1` — 实跑 exit 0（9 道全 PASS），但有 3 处盲区
- **无硬编码门**（见 §3.4）——最关键的缺口。
- **镜像校验只比文件**（`:190-200` 遍历 `Get-ChildItem -Recurse -File`）⇒ **目录级残留漏判**：发布区仍存 **空 `Defs/` 与 `Textures/`**（`Mods\RUIF\Defs`、`Mods\RUIF\Textures`，均 0 文件），而**开发区根本没有这两个目录**。这与同报告 `体检报告_2026-09-12.md:186` 揭示的同类根因（"`Sample` 不在 `sync_ruif.ps1` 白名单 ⇒ 镜像清理够不着它"）同族，且与 README §1「框架只供给组件」定位违和（随包留空 `Defs/`）。`sync_ruif.ps1` 的镜像清理（`:85-95`）同样只删文件、不删空目录。
- **门禁道数表述漂移**：`verify_release.ps1:6`「九道检查」，README L112「**十道**门禁校验」；权威侧证 `DevDocs/体检报告_2026-09-12.md:6`「门禁道数已随 `Defs/RUIFSettings.xml` 的移除收敛为 **9 道**，本报告正文的「十道」为当时表述」⇒ **README 为陈旧方**。
- 键覆盖命名空间窄（`:172` `RUIF\.Comp\.`，见 §2.4）。

### 9.4 其它产物事实错误（README / About，发布面）
| 位置 | 原文 | 实测 |
|---|---|---|
| `README.md:20` | 「当前 **v1.1.0**」 | `v1.1.2`（`CHANGELOG.md:8` 有 v1.1.2 条目） |
| `README.md:22` | 单测 → **171/171** | Tests 目录 `[Test]`+`[TestCase]` = **197**（与主理人实测一致） |
| `README.md:112` | 发布包「**19 文件**」· 「**十道**门禁」 | **21 文件**（`_audit/verify_release_now.txt` 实跑）；**9 道** |
| `README.md:26` / `About.xml:18,31` | 「源码**零硬编码**」 | 实测 24–26 处中文硬编码 |
| `README.md:73,110` | gen_preview/health_check 含主题逻辑 | 均不含（见 §9.1/9.2） |

---

## 10. 阻塞项（分级）+ 建议动作

### P1（阻止交付）
1. **用户可见中文硬编码 21 处**（§3.2），违反 GOV-010 且与 README §2 / About 宣称「零硬编码」直接矛盾。
   → 全部改走 `RUIF.Comp.*` Keyed（新增约 18–20 键，中英各一份），或把文案改为**调用方传入参数**（这批组件本就是「纯 presentational，数据由调用方传入」，后者更贴合定位）。
   → 同时**给 `verify_release.ps1` 增第 10 道「硬编码门」**（复用 `health_check` 第 4 组逻辑但**阻断**），并修 `health_check.ps1:188` 正则（加 `\u3000-\u303f\uff00-\uffef`）。否则修完还会再回归。
2. **`DevDocs/终极宝典v1.0-fixed.html` 93 处描述已移除的 Theme 层**（§7.1），随包误导第三方。
   → 删除/重写主题相关段（`:323 :404 :425 :749 :1210 :1290 :1313 :1318`），把 `Theme` 层描述改为 `Tokens` 层；主题切换按钮移除。
3. **3 处裸 `GUI.BeginGroup/EndGroup` 无 try/finally**（§5.2），异常路径可致**整局 GUI 卡死**。
   → 补 `try/finally` 或引入 `Layout.Group()` / `Layout.Scroll()` 作用域；`Layout.BeginScroll/EndScroll` 同步补作用域版。
4. **发布面事实错误**（§7.2 / §9.4）：`About.xml` 113/603、`README` v1.1.0 / 171 / 19 文件 / 十道、`index.html` v1.1.0 / 73·438、`组件库全量目录.md` 113/603。
   → 统一改为 114 类型 / 615 成员（或声明口径）、v1.1.2、197 单测、21 文件、9 道。

### P2（必须修复或明确风险）
5. `TexFmt.Heat` 5 档色阶出 `Tokens`（§6.1）；同步修 `Charts.cs:95`/`Industrial.cs:111` 指向的「`Tokens.cs` 注释白名单」——该注释**不存在**。
6. `gen_preview.py` 不可运行 + 事实陈旧（§9.1）：补 `Cinzel-Variable.ttf` 到随仓库 `Fonts/`（或改字体来源），并把 `VERSION`/`STAT_*`/`COMPONENTS`/`TOKENS` 改为**从 `About.xml` + dll 反射 + `Tokens.cs` 读取**（脚本自述的"数据同源"目前未落实）。
7. `health_check.ps1` 白名单/口径（§9.2）：`$colorWhitelist` 补 `TexAtoms.cs`/`TexFmt.cs`（附理由）；第 3 组「已移除特性残留」扩到 `Theme|RUIFSettings|换肤|主题|ThemePresets|VanillaUiSkin|DrawDecor`；给体检脚本加 `-Blocking` 退出码。
8. 5 份旧版体检报告随包（§7.4）：加显著抬头声明其描述的版本线已不存在。
9. 发布区空 `Defs/` `Textures/` 目录（§9.3）：`sync_ruif.ps1` 补空目录清理；`verify_release.ps1` 镜像校验加目录比对。
10. 组件层 2 个 `static` 可变字段（§5.3）；`PawnTableAtoms.cs:110` 读游戏状态（§6.3）——二者均与 README 红线冲突，应改或**显式登记例外**。
11. `Tr.cs:64` 三参重载为死代码 + 注释指向已移除的设置页（§4）；`dev` 根残留陈旧运行产物 `_hc_out.txt`（显示旧的「无中文硬编码」OK）。
12. `CHANGELOG.md` 重复版本标题（`v1.0.0` ×2）与版本序倒挂（§7.5）。

### P3（后续优化）
13. 组件层 15 个硬编码尺寸常量 + `Measure` 仅 1 处使用（§6.2）：按 `Measure.*` 派生并缓存为 `static readonly`。
14. `health_check.ps1:29-35 Strip-Comment` 在引号数为奇数时不剥注释（§3.5）。

### 未执行项（诚实登记）
- **NOT RUN**：任何「HTML 预览 ↔ 游戏实机 1:1 截图比对」（无实机截图、无 LAND 任务派发）⇒ GATE-LAND 记 `N/A`，不伪造。
- **NOT RUN**：`About/Preview.png` 与 `_preview_test.png` 的像素级差异比对（无基准）。
- 实机侧（游戏内加载后组件观感、DPI 分档、Six-state 交互）**超出本轮只读静态审计能力**，由实机验收承担。
