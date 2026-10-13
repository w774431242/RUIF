# RUIF v1.1.2 · 质量门禁与发布总检报告（质量与发布部 · 宋验真）

审计时间：本次会话 | 审计对象：`E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF`（开发根）↔ `...\Mods\RUIF`（发布区）
工具：dotnet 8.0.424 (`D:\toolbox\dotnet\dotnet.exe`)、python3、git、md5sum
证据产物：`_audit/qa/build_ruif.log`、`build_nowarn_cleared.log`、`test_ruif.log`、`cjk_scan.py`

> 说明：`verify_release.ps1` 因第 40 行 `[Reflection.Assembly]::LoadFrom` 被沙箱硬拦截，本环境**无法执行**（NOT RUN，非 FAIL）；
> 本报告用独立等价手段（纯文本 / python / md5 / 字节扫描）复核关键项，逐项标注证据来源。

---

## 1. 四门禁判定总表

| Gate | 结果 | 证据 | 未过项处置 |
|---|---|---|---|
| GATE-TEST | **PASS WITH EXCEPTION** | 编译 0/0、单测 197/197、XML/LOC 实测通过（见 §2） | 实机 Vanilla 启动 = NOT RUN；警告策略非强制（P2） |
| GATE-PERF | **BLOCKED** | 静态命中清单 + 我独立复核（见 §3） | 必需项「可复核性能指标」NOT RUN ⇒ 不得记 PASS；绝对宣称被证伪 |
| GATE-REL | **BLOCKED** | 见 §4 | 交付物过期内聚性 + 对外宣称不实 + 上架材料缺失 |
| GATE-DELTA | **BLOCKED** | 见 §5 | git 无法机械校验 + 无 Change ID + 未标注增量 |
| （复核上游）GATE-ARCH/DATA/SAVE/UI/LAND/LOC/COMP | 采信五部门结论，我未重复取证 | — | UI/LOC = BLOCKED；ARCH = 局部 BLOCKED；COMP = 仓外阻塞 |

四态口径：PASS / PASS WITH EXCEPTION / BLOCKED / NOT APPLICABLE（N/A 必写原因）；未执行项一律记 **NOT RUN**，不降级为 PASS。

---

## 2. GATE-TEST

### 2.1 编译（实测）
```
dotnet build Source/RUIF.csproj -c Release -t:Rebuild   →  0 个警告 / 0 个错误
dotnet build Tests/RUIF.Tests.csproj -c Release -t:Rebuild →  0 个警告 / 0 个错误
```
- `Deterministic=true` 生效：重编译产物 `Assemblies/RUIF.dll` md5 = `36efb5809feb1aafc6fbf1619076ff8c`，与发布区 dll **逐字节一致**（可复现）。
- 与主理人基线一致（0 错 0 警 / 197 通过）。

### 2.2 单测（实测）
```
dotnet test Tests/RUIF.Tests.csproj -c Release → 失败: 0，通过: 197，已跳过: 0，总计: 197（226ms）
```
与基线 197/197 一致。

### 2.3 警告策略核实（关键发现）
- csproj 第 17–18 行注释写「**警告即错误**：框架代码必须 0 错 0 警」，但同处 `<TreatWarningsAsErrors>false</TreatWarningsAsErrors>` ⇒ **注释与配置直接矛盾**。
- 另设 `<NoWarn>$(NoWarn);CS1591;CS1573;CS1574</NoWarn>`。
- **实测（`-p:NoWarn=" "` 清除抑制后重建）**：出现 **130 个警告**（CS1591 缺 XML 注释 128 + CS1573 参数失配 2），全部为文档完备性警告；**无任何代码级警告**。
- 结论：`0 警` 是**靠 NoWarn 抑制文档警告 + WAE 关闭**得到，**不是强制机制**。任何新增代码类警告（未用变量 / 可能的空引用等）**不会**让构建失败。⇒ 属 P2「门禁无强制力」，非 P0/P1 代码缺陷。
- 影响面：`RUIF.xml`（对外 IntelliSense 源）因此可长期残缺而不被门禁拦下——与 §4 的 xml 漂移同源风险。

### 2.4 解析 / 本地化（实测，独立于主理人）
- XML 良构：`About/About.xml`、`Languages/*/Keyed/RUIF_Components.xml`（in-scope 3 文件）全部可解析，bad=0；`Assemblies/RUIF.xml` 亦可解析。
  - 注：主理人记「12/12」的口径在本环境不可复现（in-scope 仅 3 个 XML；若把 `_demo`/`_audit`/`obj` 计入则远超 12）——**该数字来源不明，建议以本报告 3/3 为准**。
- 本地化：中 60 键 / 英 60 键，**集合双向对称（差值 0）**、**0 死键**、**0 重复键**、**0 占位符错配**。
- 引用面：dll 明文字节扫描 `0Harmony`=0、`HarmonyLib`=0、`Newtonsoft`=0（与架构部 PE 级结论互证）。

### 2.5 未执行项
- **Vanilla 启动 = NOT RUN**（沙箱无游戏进程）。
- **存读档 / 迁移 = N/A**：源码 + 发布 dll 双重零命中 `IExposable/ExposeData/Scribe_/GameComponent/ModSettings/Def`，无存档面；「同一存档重复迁移幂等」无适用对象。
- **失败隔离（Provider 失效）= N/A**：纯组件库无 Provider 链、无可选依赖，无可失效标的。

---

## 3. GATE-PERF

### 3.1 证据级别声明
**未跑 Profiler、未跑实机帧分析** ⇒ 本门禁性能结论全部为**静态清单级**，**不构成"实测通过"**。

### 3.2 通得过的部分
- 热路径**无全库扫描**：源码零 `Tick()` / `GameComponent` / `MonoBehaviour.Update()`（框架自身无每帧钩子）。
- 热路径**无反射**：`System.Reflection` 仅出现于 `AssemblyInfo.cs` 的 using（特性用），无运行时 `GetType()`/`Invoke` 调用。
- **无无界分配**：`TrCache` 512 上限（超限全清重建）、`TexCache` 128 上限、`Industrial.shareBuf` 定长 16、`NiceTicks(...float[] outBuf)` 调用方供缓冲。

### 3.3 真违规（我独立复核确认）
| # | 位置 | 事实 | 判定 |
|---|---|---|---|
| P1-a | `ChartCandAtoms.cs:224-227, 267-270` | `System.Array.ConvertAll(pts, p => p.x)` 等 **8 处**：每次调用分配**新数组 + lambda 闭包**，且位于绘制路径 | **真违规**（无界/可避免堆分配） |
| P1-b | `InputCandAtoms.cs:50` | `values[i].ToString("F1") + (cfg[i].unit ?? "")` → 每帧每滑块一次字符串分配 + 拼接 | 真违规（小，per-call） |
| P1-c | `InputPlusAtoms.cs:386-397` | `SplitLines(buffer,...)` 分配数组 + `string.Join("\n", lines)` 再分配 | 真违规（小，per-call） |
| P1-d | `Industrial.cs:313` | `(shareBuf[i]*100f).ToString("0")` 经 `Tr.Get(...)` 参数化 → 每次格式化分配 | 真违规（小，per-call） |
| P2 | `Widgets.DrawLine` 8 处（`ChartCandAtoms:59/65/76/130`、`NavAtomsB:61-62`、`DomainCandAtoms:95/126`） | Charts 族已弃用 `Widgets.DrawLine` 改贴图烘焙，候选原子仍用 ⇒ **双标**（非分配问题，属一致性问题，交架构部裁定） | 待裁定 |

### 3.4 被误报为"堆分配"的项（需要澄清）
`new Vector2(...)` / `new Rect(...)`（如 `ChartCandAtoms:127-128`、`NavAtomsB:61-62`、`DomainCandAtoms:118`）是**值类型构造，不产生堆分配**。机制部"12 组命中"中若含这些，属**高估**；真堆分配以上表 P1-a~d 为准。

### 3.5 裁定
- **「绘制路径零堆分配」这一绝对宣称被证伪**（About.xml 描述 + README 均含此语）。可保留的只是「无**无界**分配 / 不读游戏状态的**大**分配」。
- **GATE-PERF 必需项「至少一份可复核指标」= NOT RUN**（DevDocs 仅见静态推演，无 Profiler 数据、无 ms 指标）。
- ⇒ GATE-PERF = **BLOCKED**（P2 级：可修复；须「修正绝对化宣称 + 补一份可复核指标」）。

---

## 4. GATE-REL

### 4.1 版本一致性（实测 PASS）
| 来源 | 值 |
|---|---|
| `About/About.xml` modVersion | `v1.1.2` |
| `Source/Core/Api.cs` VersionString | `1.1.2` |
| `Source/Core/AssemblyInfo.cs` | `1.1.2.0` |
| `DevDocs/BUILD-INFO.txt` version | `1.1.2` |
| `Assemblies/RUIF.dll` md5 | `36efb580...`（= 发布区） |
⇒ 五处一致 **PASS**。两区 `About.xml` md5 一致（`fb7086c9...`）。

### 4.2 发布包清单与内聚性（BLOCKED）
- 发布区 = **21 文件**：About 3 + Assemblies 3 + Languages 2 + DevDocs 13。结构齐全。
- **交付物过期 / 不内聚（P1）**：`Assemblies/RUIF.xml` 与 `RUIF.pdb` **两区不一致**（dll 一致）：
  - `diff` 28 行；发布区 xml **缺** `Charts.LineChart(Rect, Single[][], Color[], bool, bool)` 多系列重载说明，`Charts.DonutChart` 仍是**旧 4 参数**签名文档（开发区为 7 参数）。
  - 后果：消费者拿到的 `RUIF.xml`（IntelliSense 文档源）与真实 dll **签名不符**，且与 DevDocs `API参考.md`（09-13 生成）口径冲突。
- **残留空目录**：发布区 `Defs/`、`Textures/` 为空目录（`verify_release.ps1` 第 6 道只扫文件，**查不到目录** ⇒ 门禁盲区）。
- 随包 DevDocs 含 **5 份旧体检报告**（09-11/09-12/v2.0.0/v2.1.1/v2.2.1）+ `终极宝典v1.0-fixed.html`（**28 处**主题层残留引用，主题层已于 v1.0.1 删除）。

### 4.3 Changelog / Change Summary / 回滚（部分缺失）
- `DevDocs/CHANGELOG.md` 与 `docs/06-发布记录/CHANGELOG.md` **md5 一致**（`b4ec7dc2...`）⇒ 无双份漂移 **PASS**。
- 当前版条目齐：`v1.1.2`（2026-09-13）+ Gate 摘要 + 逐项修复说明。
- **但**：① 无独立 **Change Summary 文档**；② 无独立 **回滚说明**（回滚段仅散落在 CHANGELOG / 健康评估文档里）；③ v1.1.2 自称「无公共 API 变化（签名/类型/成员数全同）」**与 §4.4 的计数增量矛盾**。

### 4.4 对外宣称不实（对外诚信，阻塞级）
| 位置 | 宣称 | 实测 | 判定 |
|---|---|---|---|
| `About.xml` 描述 | 公开 API **113 类型 / 603 成员** | API参考.md 目录 = **114 类型**（csharp 成员行 615） | 不实（P2） |
| `About.xml` 描述 | **153 件组件**（42/28/40/12/17/10/4） | 三口径 203/168/158 不可复现（机制部） | 不可复现（P2） |
| `About.xml` 描述 | **源码零硬编码** | 实测 **24 处中文硬编码字面量 / 8 文件**（`_audit/qa/cjk_scan.py`）；EXC-002 已于 v2.1.0 关闭 ⇒ 属**未登记回归** | 不实（P1 诚信） |
| `About.xml` 描述 | **绘制路径零堆分配** | §3.3 证伪 | 不实（P2） |
| `About/Preview.png` | **v1.0.1**、`73 TYPES · 435 MEMBERS · 153 COMPONENTS` | 当前 v1.1.2 / 114 类型 | **封面过期且内部自相矛盾**（P2） |
| `README.md:20` | 当前 **v1.1.0** | v1.1.2 | 过期（P3） |

### 4.5 EXC 有效期核查（BLOCKED）
- **无独立 EXC 登记文件**（EXC-001/002/003 仅散落在 `docs/05-.../健康评估与发布记录.md` 与 CHANGELOG）。
- **EXC-003 登记悬空**：登记主体 `RUIFSettings.Warnings/ValidNames/SourceSummary` 已随配置系统移除 ⇒ 例外**无标的**，未见关闭/迁移记录（违反 GOV-012）。架构部独立指出同项。
- 该「当前有效判定」文档（`健康评估与发布记录.md`）**未随包**（DevDocs 内无），消费者/评审看不到例外台账。
- 对比：EXC-001（有效期至 I1）已关闭；EXC-002 已于 v2.1.0 关闭 —— 但 §4.4 的 24 处硬编码说明**关闭结论未被后续回归守住**。

### 4.6 Workshop 上架材料与仓库状态（BLOCKED）
- **无工坊描述 / 标签 / 上架材料**（仅 `About.xml` + `Preview.png`）。
- `About/PublishedFileId.txt` = **3799502474**，两区一致且开发区存在 ⇒ 该 Mod **已上传过工坊**；而 `README.md:112` 写「**未发布到 Workshop / GitHub**」⇒ **直接冲突**。
- `.gitignore:27` 忽略 `About/PublishedFileId.txt`；`git ls-files About/` 仅含 `About.xml` + `Preview.png`（**未跟踪**）⇒ **仓库无法反映发布状态**；clone/CI 环境缺该文件，若误上传将**新建第二个工坊条目**（`verify_release.ps1` 第 6 道虽有断言，但依赖开发树里那份被 ignore 的本地文件，对克隆环境失效）。
- README 自身「19 文件」与发布区 21 文件不符；README「十道门禁」与脚本头「九道」不符（P3）。
- README **不随包** ⇒「简化件免责」「已移除主题层」等声明消费者看不到（机制部/表现部同项）。

⇒ **GATE-REL = BLOCKED**。

---

## 5. GATE-DELTA（变更标注一致性）

### 5.1 git 现状（实测）
- `git log` 仅 **1 个 commit**：`01106e5 Initial commit: RUIF v1.0.1`；`git ls-files` = 124 文件。
- 工作区 **99 项变更**：22 M / 31 D / 46 ??。
- ⇒ **v1.1.2 的全部改动未进入版本控制**，`git diff` **无法**作为变更基线。`scripts/gate-runner.ps1`（标注↔diff 自动校验）**前提不成立**，无法运行。

### 5.2 CHANGELOG 能否替代 git diff？
**不能完整替代，只能部分替代**：
- 可替代部分：CHANGELOG 逐版叙述（v1.0.2 / v1.1.1 / v1.1.2）含修复根因、Gate 摘要、回滚段 —— 可作**变更叙述**来源。
- **不可替代部分**：CHANGELOG 是散文，**无 Change ID、无文件级 Scope、无「改动↔标注」一一映射**；无法回答「改了没标 / 标了没改」。
- **已发现「改了没标」实证**：
  1. 类型数 **113 → 114**（组件库全量目录.md 2026-09-12 基线 113 vs API参考.md 2026-09-13 = 114），而 v1.1.2 自称「成员数全同 / 无 API 变化」⇒ 至少一处公共 API 增量未标注。
  2. **24 处中文硬编码**（EXC-002 已关闭）无任何标注/例外登记。
  3. `RUIF.xml` / `RUIF.pdb` 两区漂移（§4.2）未标注。
  4. 发布区 `Defs/`、`Textures/` 空目录、5 份旧体检报告未标注。

### 5.3 裁定
**GATE-DELTA = BLOCKED**：机械校验不可用（1 commit + 99 未提交），无 Change ID 体系，且存在**已证实的「改了没标」**。CHANGELOG 可作补充叙述，**不足以**支撑验收定位门。

---

## 6. 发布阻塞项汇总（去重后，含我的独立复核认定）

### P0 / P1（任一项即阻止公开发布）
| # | 项 | 来源 | 我的复核 |
|---|---|---|---|
| B1 | 中文硬编码回归 **24 处/8 文件**（用户可见 21 处）；EXC-002 已关闭 ⇒ 未登记回归；About/README「源码零硬编码」为虚假宣称 | UI/LOC 部 | **复核确认**（`_audit/qa/cjk_scan.py` = 24） |
| B2 | 3 处裸 `GUI.BeginGroup/EndGroup` 无 try/finally（`NavAtoms:211-213/368-382`、`NavAtomsB:215-234`）⇒ 异常逃逸断 GUIClip 栈卡死 | UI 部 | 采信（未重复取证） |
| B3 | `Tokens.FontLabel = GameFont.Tiny` + `Measure.RowHeight(Tiny)=18f` 硬编码 ⇒ 开 `DisableTinyText`/Steam Deck 时 Tiny 被原版静默降级为 Small(22px)，**Gauge 标题压弧缺陷回归** | 机制部 P0-1 | 复核确认事实（Tokens.cs:63 / Measure.cs:29）；实机表现 NOT RUN |
| B4 | 定位红线冲突（`PawnTableAtoms:110` 读 `Find.CurrentMap`；3 处 `Find.WindowStack.Add(FloatMenu)`） | 架构/机制部 | **复核确认**（grep 逐条命中） |
| B5 | 对外 API 计数不实（About 113/603 vs 实 114/616）+ 组件 153 不可复现 | 机制部 + 我 | **复核确认**（API参考.md=114） |
| B6 | 变更标注缺失（113→114 未标注；24 硬编码未登记；xml/pdb 漂移未标注）+ git 无法校验 | 我 | **本部门结论** |
| B7 | 交付物过期：发布区 `RUIF.xml` 与 dll 签名不符、`RUIF.pdb` 不同步 | 我 | **复核确认**（diff 28 行） |
| B8 | 主题层公共 API 删除未登记例外（README 口头标记，违反 GOV-012）；EXC-003 标的已删除仍挂 | 架构部 + 我 | 复核确认（无独立 EXC 文件） |
| B9 | 上架材料与仓库状态冲突：README「未发布 Workshop」vs 工坊 ID 3799502474；封面 `Preview.png` 停在 v1.0.1 | 我 | **复核确认** |

### P2（须修复或明确写入风险）
- P2-1 「绘制路径零堆分配」绝对宣称被证伪（§3.3）。
- P2-2 警告策略无强制力（`TreatWarningsAsErrors=false` vs 注释「警告即错误」）；0 警靠 NoWarn 抑制 130 个文档警告。
- P2-3 GATE-PERF 缺可复核指标（NOT RUN）。
- P2-4 `verify_release.ps1` **无硬编码检查项**；`health_check.ps1` 有硬编码组但**无 exit 非零 ⇒ 恒不阻断** ⇒ 公开宣称无门禁保护。
- P2-5 发布区空目录 `Defs/`、`Textures/`；门禁不查目录（盲区）。
- P2-6 `.gitignore` 忽略 `About/PublishedFileId.txt` ⇒ 仓库无法反映发布状态。
- P2-7 `TrCache` 缓存键不含语言维度 + 失效依据 `LanguageDatabase.activeLanguage` 引用相等（数据部）⇒ 切语言文案可能不更新（待机制部裁定）。
- P2-8 `gen_preview.py` 当前不可运行（表现部）；`health_check.ps1` 仍含已删主题/装饰逻辑。

### P3（交付前修正）
- README 版本/文件数/门禁数过低过期；`AssemblyInfo.cs` 注释指向不存在的路径；随包 5 份旧报告 + 宝典 28 处主题层残留引用；「简化件免责只在 README 而 README 不随包」。

### 仓外阻塞（非 RUIF 仓内）
- 工坊条目 `3799502474` 页面「已被移除/仅自己可见 + 与 RimWorld 不兼容」双横幅与作者公开列表**自相矛盾**，须**登录 Steam 客户端**核验（兼容部结论；我无法在沙箱定论）。

---

## 7. 总裁定

> **公开发布（Steam Workshop / GitHub）：不可发。**
> **内部 / 单一消费者（Personal Chronicle）试运行：有条件可发**，条件见下。

裁定理由：编译 / 单测 / XML / 本地化 / 引用面 / 版本一致性 **实测全过**，且架构分层干净、无存档面、无 Harmony —— 无 P0/P1 **代码**阻断项；
但存在 **B1（硬编码回归 + 虚假宣称）、B3（Tiny 压弧回归）、B7（交付物过期）、B9（上架诚实性）、B6（变更标注缺失）** 等 P0/P1 **发布面**阻断项，
且 GATE-PERF / GATE-REL / GATE-DELTA **三门禁 BLOCKED**。按「任一 P0/P1 失败阻止 Release」，**不得进入公开发布**。

**「有条件发（仅内测）」条件清单**（全部满足方可放行内测）：
1. 至少修正 `About.xml` / `About/Preview.png` / README 中**已证伪的绝对宣称**（零硬编码 / 零堆分配 / 113·603 / 153 / 封面 v1.0.1）——对外诚实为不可协商项。
2. 重新 `sync_ruif.ps1` 使发布区 `RUIF.xml` / `RUIF.pdb` 与开发根一致（消除 B7）。
3. 出具这版 v1.1.2 的**变更标注清单**（文件级 Scope + Change ID），至少覆盖 113→114 与 24 处硬编码（消除 B6）。
4. 为 24 处硬编码与 3 处裸 BeginGroup 建立**登记例外或修复计划**（B1/B2）。
5. 确认消费者 Personal Chronicle 侧已同步 1.1.2 且版本门控生效（兼容部已报同步）。

**升级为公开发布的额外前置**：
6. `Tokens.FontLabel/Measure` 的 Tiny 降级路径修复或实机证明不触发（B3）。
7. 清理发布区空目录 + 过期随包文档；补 GATE-PERF 可复核指标；修正警告策略强制性（P2-1/2/3/5）。
8. **登录 Steam 客户端**核验工坊条目 3799502474 状态并据实修订 README（B9 + 仓外阻塞）。
9. 例外台账 EXC-003 关闭/迁移，并将「当前有效判定」文档纳入随包。

---

## 8. NOT RUN 清单（未执行一律不记 PASS）

| 项 | 原因 |
|---|---|
| `verify_release.ps1` 原脚本整体执行 | 第 40 行 `[Reflection.Assembly]::LoadFrom` 被沙箱安全策略硬拦截（命令被拒，无输出，退出码 1）—— 已改用等价手段复核 §2–§4 |
| Vanilla 环境启动（不带第三方 Mod） | 沙箱无游戏进程，无法启动 RimWorld 1.6 |
| 实机存读档 / 迁移 / 幂等 | 同 N/A（无存档面）；无实机环境 |
| 失败隔离实检 | N/A（无 Provider / 可选依赖可失效） |
| Profiler / 帧分析 / 内存指标 | 无实机；GATE-PERF「可复核指标」NOT RUN |
| GATE-PERF 实机绘制行为（Tiny 压弧、GUIClip 卡死） | 无实机；仅静态证据 |
| Steam 工坊条目真实性核验 | 需登录 Steam 客户端（沙箱不可） |
| `health_check.ps1` / `gen_preview.py` 可运行性 | 表现部已报不可运行；本部门未复跑（非本门禁必需项） |
