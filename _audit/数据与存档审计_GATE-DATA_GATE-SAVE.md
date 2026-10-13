# RUIF 数据与存档审计报告（GATE-DATA / GATE-SAVE）

- 审计人：数据与存档部 · 史可溯
- 审计对象：RUIF（RimWorld UI Framework）v1.1.2
  - 开发根 `E:\SteamLibrary\steamapps\common\RimWorld\11\RUIF`
  - 发布区 `E:\SteamLibrary\steamapps\common\RimWorld\Mods\RUIF`（21 文件 · dll md5 `36efb5809feb1aafc6fbf1619076ff8c`，两区一致）
- 纪律：**只读审计**，未修改项目任何文件；本报告为唯一新增产物（`_audit\`）。
- 结论四态：PASS / PASS WITH EXCEPTION / BLOCKED / N/A（N/A 必写原因）。未执行的标 **NOT RUN**。

---

## 1. 判定总表

| # | 检查项 | 四态 | 证据摘要 |
|---|---|---|---|
| 1 | 框架自身无 Def（C# 类） | **PASS** | `Source/**/*.cs` 无 `class *Def`/`: Def`/`DefModExtension`/`DefOf`；唯二命中在注释（`PawnTableAtoms.cs:16`、`ReorderAtoms.cs:18` 的 `SoundDefOf` 引用） |
| 2 | 框架自身无 Def（XML） | **PASS** | 开发根无 `Defs/` 目录（`ls Defs` → No such file）；发布区 `Defs/` **0 文件**；全盘 XML 仅 `About.xml`+`Languages/**/Keyed/*.xml` |
| 3 | 无 Mod/ModSettings 子类 | **PASS** | `Source` 内 `grep -E ":\s*(Verse\.)?(Mod|ModSettings)\b"` → 0 命中；唯一 `: Mod` 在独立包 `_demo/RUIF_Demo/Source/DemoMod.cs:19` |
| 4 | 无 GameComponent/WorldComponent/MapComponent | **PASS** | 源码 0 命中 **且发布区 dll 二进制 0 命中** |
| 5 | 无 `[StaticConstructorOnStartup]` | **PASS** | 源码 0 命中 **且 dll 二进制 0 命中** |
| 6 | 无 `IExposable`/`ExposeData`/`Scribe_*`/`LookMode` | **PASS** | 源码 0 命中；dll 二进制 `IExposable=0 ExposeData=0 Scribe_=0 LookMode=0` |
| 7 | 「无存档字段、无运行时持久状态」承诺 | **PASS** | 同上 3–6 条；框架侧零持久化面 |
| 8 | 「组件无持久状态」承诺 | **PASS WITH EXCEPTION** | 组件不入存档成立；但存在 1 个可变静态字段 + 2 个静态复用缓冲（见 §4） |
| 9 | `TrCache` 容量 512 为强制 | **PASS** | `Tr.cs:33` `new TrCache(512, …)`；`TrCache.cs:85-91` `Store()` 达容量即清空重建；`Tests/TrCacheTests.cs:91-98` 断言 `Count ≤ Capacity` |
| 10 | `TrCache` 超限策略明确 | **PASS WITH EXCEPTION** | 策略 = **全清重建（flush-all）**，非 LRU；见 §5 |
| 11 | `TrCache` 语言切换自动失效 | **PASS WITH EXCEPTION（需机制核实）** | `Tr.cs:33` 语言戳 = `() => LanguageDatabase.activeLanguage`；`TrCache.cs:93-102` 用 `ReferenceEquals` 判定。前提「切语言 ⇒ 引用变」**需 rimworld-mechanics 反编译核实** |
| 12 | 缓存失效可诊断 | **FAIL（P2）** | `LanguageInvalidations` 为 `internal`（仅单测可见），生产无日志/无 UI；「失效永不触发」这一最危险模式**无自检** |
| 13 | `TexCache` 有界 | **PASS WITH EXCEPTION** | `MaxEntries=128`（`TexCache.cs:27`）+ FIFO 淘汰（`:224-237`）；**按条目数有界、非按字节**，见 §6 |
| 14 | 缓存键越界不静默 | **PASS WITH EXCEPTION** | `KeyOf` 位段夹紧 + `Log.ErrorOnce`（`:124-128`、`:140-146`）✅；**例外** `FoldColor`（`:166-170`）静默有损折叠 ❌ |
| 15 | 缓存键单射性 | **PASS WITH EXCEPTION** | `KeyOf` 合法域内**可证明单射**（`TexCacheTests.cs:42-61` 抽样 2 万无碰撞）；`Key`/`KeyVersioned`/`KeyData` 为哈希键，**不可证单射**且共用 63 位空间 |
| 16 | 缓存可重建 | **PASS** | TrCache：权威 = `LanguageDatabase`（构造注入 factory，`TrCache.cs:16`）；TexCache：权威 = 像素源，`Publish` 重烘焙；`TryGet` 对已 Destroy 贴图判未命中并清键（`:179-181`） |
| 17 | `Tokens` 运行期不可变 | **PASS** | 16 色 `static readonly`（`Tokens.cs:72-102`）、间距/尺寸 `const`、`FontXxx` 只读表达式、`AccentSoft`/`Series` 只读派生；**无任何可写 `static` 字段** |
| 18 | 组件禁 `new Color`（README:99 红线） | **PASS WITH EXCEPTION** | 3 处命中，2 处为派生色、1 处为硬编码黑（见 §7） |
| 19 | 组件禁裸 `GUI.color`（README:99 红线） | **PASS** | 命中 3 处**全在注释**；实际写入经 `Draw.Color(…)` 作用域（`TexAtoms.cs:68/98/141`） |
| 20 | Def/Runtime/Save 三态划分 | **PASS** | 见 §8 |
| 21 | 设置系统移除的存档/用户配置影响 | **PASS** | 见 §2 |
| 22 | 文档残留 / 数据声明漂移 | **PASS WITH EXCEPTION** | 4 处（见 §2.3、§9） |
| 23 | GATE-DATA / GATE-SAVE 在最新 Gate 表内 | **FAIL（P2，流程）** | `docs/05-…/健康评估与发布记录.md` §12.2 / §14.2 两张「当前有效判定」表**均无 GATE-DATA/GATE-SAVE 行** |

**总判定：GATE-DATA = PASS WITH EXCEPTION；GATE-SAVE = N/A（原因：框架不参与存档）。无 P0、无 P1。**

---

## 2. Def 面实测结论（含设置系统残留评估）

### 2.1 结论
- 框架**当前零 Def**（C# 类与 XML 双零）。发布区 `Defs/`、`Textures/` 为**空目录**（各 0 文件）。
- 空目录**不影响 RimWorld Def 加载**（`ModContentPack` 枚举 `Defs/**/*.xml`，空目录 = 0 个 Def，无报错）。
- 空目录成因可复现：`_scripts/sync_ruif.ps1:34` 白名单仍含 `Defs`/`Textures`，`:9` 的**镜像清理只删文件、不删目录** ⇒ 目录被保留。故为**有意保留白名单槽位 + 清理逻辑不删空目录**的合成结果，非误删。

### 2.2 是「移除」还是「误删」——判定：**移除（有完整变更记录）**
| 证据 | 内容 |
|---|---|
| `README.md:25` | 明确写「主题 / 换肤：**无**（已移除 … `RUIFSettings`）」 |
| `docs/06-发布记录/CHANGELOG.md:180-183` | 明确记录删除 `Core/RUIFMod.cs`（设置页）、`Config/RUIFSettings.cs` + `RUIFSettingsDef.cs`、`Defs/RUIFSettings.xml`，以及相关单测 |
| `docs/05-…/健康评估与发布记录.md:324`（§11.1，v1.0.1 当前有效判定） | `GATE-DATA \| PASS \| 框架无 Def、无存档字段（设置系统已随换肤层移除）` |
| 源码目录树 | `Source/` 仅 `Core/ Layout/ Components/`，无 `Config/`、无 `Theme/` |
| 全盘搜索 | `RUIFSettings*` **仅存在于** `PersonalChronicle/_backups/ruif_pre_strip_20260912/`（变更前备份）⇒ 运行路径**零残留** |

> ⚠️ 审计局限（诚实登记）：本仓库 git **仅 1 次 squash commit**（`01106e5 Initial commit: RUIF v1.0.1`），`git log -- Source/Config/* Defs/RUIFSettings.xml` 为空 ⇒ **该移除无法通过 git 复核**，仅有 CHANGELOG / docs 文档链佐证。属文档级证据，非版本控制级证据。

### 2.3 对既有存档 / 用户配置的影响——**零存档影响**
推理链（逐条可复核）：
1. `RUIF_Settings` 是 **Def**（`RUIFSettingsDef`），非 `ModSettings`；框架内**无 `: Mod` 子类**（§1 第 3 条），故 **从不写入存档的 `<modSettings>` 节点**。
2. 其 XML 经 `DirectXmlToObject.ObjectFromXml<RUIFSettingsDef>(node, doPostLoad:false)` 读取（`CHANGELOG.md:297`），**不走 Scribe**、无 `ExposeData` ⇒ 不产生持久字段。
3. 它不是 ThingDef/PawnKindDef/BillDef 等**可被存档引用**的 Def；只被启动期应用器读入静态令牌。
⇒ **删字段/删 Def 的迁移动作 = 无**；无需 Migration、无 schema 版本变更、旧档可直接加载。

**残留风险（P3，低概率）**：若用户**手工**在 `Mods/RUIF/Defs/` 保留了旧 `RUIFSettings.xml`，则 Def 加载期找不到类型 `RUIF.RUIFSettingsDef` ⇒ 一条红色 XML 错误（RimWorld 记日志后跳过）。**不损坏存档**。缓解：镜像清理已删除该文件（实测全盘零残留）。

### 2.4 文档中描述设置系统的残留（`文件:行号`）
| 文件:行号 | 内容 | 判定 |
|---|---|---|
| `README.md:25` | 列明「已移除 `RUIFSettings`」 | ✅ **不是残留**（是移除声明） |
| `docs/05-…/健康评估与发布记录.md:30` | `GATE-DATA PASS \| 唯一 Def 为配置项 RUIF_Settings` | ⚠️ 属 §2「Gate 判定表（**v2.0.0**）」，章节标题已限定版本 ⇒ **历史记录，非当前声明**（可接受，但建议加「已废弃」标记） |
| 同上 `:37 / :62 / :89 / :92 / :137 / :163 / :195` | 均描述 `Defs/RUIFSettings.xml` | ⚠️ 同属 v2.0.0/v2.1.1/v2.2.1 历史段落（`:354` 有口径更正说明）⇒ 可接受 |
| **`Source/Core/Tr.cs:60-62`** | 注释：「当前唯一使用点是**设置页的「XML 配置来源」行**」 | ❌ **当前代码内的过期残留**；该设置页已删除。且三参重载 `Tr.Get(k,a1,a2,a3)` **全源码 0 调用点**（`grep "Tr.Get([^)]*,[^)]*,[^)]*,"` → 无命中）⇒ 死 API + 失实注释（P3） |
| **`Source/Components/Charts.cs:391-392`** | 「调色板按 `Tokens.Series` 的 6 色**序号**编键 ⇒ 若切换主题改变了这 6 个色值，需 `TexCache.Clear()`」 | ❌ **与实现矛盾**（见 §6.3，P2） |

---

## 3. 存档面扫描结果（命令 + 命中）

### 3.1 源码扫描
```bash
grep -rniE "scribe|exposeData|IExposable|LoadMode|PostLoadInit|GameComponent|WorldComponent|MapComponent|StaticConstructorOnStartup|GameAndMapInit" Source --include=*.cs
# → [F] 无任何命中
grep -rnE ":\s*(Verse\.)?(Mod|ModSettings|ThingComp|Comp|GameComponent|WorldComponent|MapComponent|Def)\b" Source --include=*.cs
# → [G] 无任何命中
```
**命中：0。**

### 3.2 发布区 dll 二进制扫描（防止「源码干净、dll 带料」）
```bash
D="Mods/RUIF/Assemblies/RUIF.dll"
for p in IExposable ExposeData Scribe_ Verse.Def Def GameComponent WorldComponent MapComponent ModSettings StaticConstructorOnStartup Patches LookMode; do
  echo "$p : $(grep -ac "$p" "$D")"; done
```
| 符号 | 命中 |
|---|---|
| `IExposable` | **0** |
| `ExposeData` | **0** |
| `Scribe_*` | **0** |
| `Verse.Def` | **0** |
| `GameComponent` / `WorldComponent` / `MapComponent` | **0** / **0** / **0** |
| `ModSettings` | **0** |
| `StaticConstructorOnStartup` | **0** |
| `Patches` | **0** |
| `LookMode` | **0** |
| `Def` | 1（子串命中，展开后为 `PawnTableDef` / `PawnColumnDef` / `DefDatabase` / `tableDefName` / `get_TableDefName` / `GetValueOrDefault`） |

**解读**：唯一 `Def` 相关符号是框架对**原版 Def 的只读查询**——`PawnTableAtoms.cs:58` `DefDatabase<PawnTableDef>.GetNamedSilentFail(tableDefName)`，且带 null 守卫 + 可读 `LastError`（`:61`）。**声明 Def 数为 0**；`Verse.Def` 不可见 ⇒ 无 Def 派生类型。

### 3.3 存档面结论
**「无存档字段、无运行时持久状态」承诺 —— 成立（源码 + 二进制双重证据）。**
⇒ **GATE-SAVE = N/A**。原因：框架不实现 `IExposable`、不定义 `Comp`/`GameComponent`/`ModSettings`、不声明 `Def`，因此**不产生任何可持久化数据契约**，无 schema、无版本、无迁移对象。

---

## 4. 静态状态与静态构造风险

### 4.1 静态构造路径
- `[StaticConstructorOnStartup]`：**0**（源码 + dll）。
- 显式静态构造函数（`static Type(){}`）：**0**。
- 静态字段初始化器：仅 4 处，**全部为纯托管、无引擎调用**：
  - `Api.cs:29` `static readonly Version version = ParseVersion(VersionString);`（纯字符串解析）
  - `TexCache.cs:29-30` `Dictionary`/`List` 空容器
  - `Tr.cs:33` `new TrCache(512, () => LanguageDatabase.activeLanguage)`（**委托不立即求值** ⇒ 不触发引擎）
  - `Tokens.cs:72-102` 16 个 `Color` 字面量
⇒ **无静态构造异常外溢风险**。历史记录所述 `RUIFSettings.ApplyFromDefs` 的整体 try/catch 容错（非 Unity 宿主 `Verse.Log` 抛 `SecurityException`，见 `CHANGELOG.md:707`）—— **该类已随设置系统一并删除**（全盘搜索零残留）。风险随之消失。

### 4.2 静态可变状态（AI-004 / Architecture Review 触发条件）
```bash
grep -rnE "^[^/]*\bstatic\b[^=;]*;" Source --include=*.cs | grep -v "/obj/" | grep -v readonly | grep -v const
# → Source/Components/NavAtoms.cs:217:        private static TreeNode clicked;
```
| 位置 | 性质 | 风险 | 四态 |
|---|---|---|---|
| **`NavAtoms.cs:217`** `private static TreeNode clicked;` | **可变静态字段** | ① `CollapsibleTree` 以 `clicked = null` 起、递归内 `clicked = node` 写、末尾返回（`:209/214/242`）⇒ **非重入安全**（嵌套调用互相覆盖）；② 持有 `TreeNode` 引用至下次调用（轻微保活）；③ 不构成「无界」静态单例（单引用），故**未直接触犯 AI-004 的「无界」限定**，但命中「**Static Mutable State = Yes ⇒ 触发 Architecture Review**」条件；④ **不入存档** ⇒ 与 GATE-SAVE 无关（本项归 GATE-ARCH/P2） | **PASS WITH EXCEPTION** |
| `Charts.cs:104` `static readonly float[] tickBuf = new float[8]` | 静态复用缓冲（contents 可变） | 固定 8 长度，IMGUI 单线程；`Charts.cs:98` 注释已声明动机（每帧零堆分配）。有界 | 可接受（建议登记 EXC） |
| `Industrial.cs:118` `static readonly float[] shareBuf = new float[16]` | 同上 | 固定 16 长度 | 同上 |

**注**：`PawnTableAtoms.cs:118` 注释把调用方持有的 `PawnTableHost` 称作「**持久状态**」——语义应为「**跨帧 UI 状态**」（消费者持有），**非存档持久**。建议改词以免与存档术语混淆。

---

## 5. `TrCache` 有界性与失效语义推演

### 5.1 有界性：**真实强制，策略 = 全清重建**
`TrCache.cs:85-91`
```csharp
private void Store()
{
    if (plain.Count + withInt.Count < capacity) return;
    plain.Clear(); withInt.Clear(); CapacityFlushes++;
}
```
- `Store()` 在**插入前**调用 ⇒ 插入后 `Count ≤ capacity`。反例穷举：`Count = capacity-1` → 不 flush → 插入后 = `capacity`；下一次未命中 → flush → 插入后 = 1。**上界恒成立**。
- 超限策略 = **flush-all（全清）**，**非 LRU**、非拒绝插入。诊断计数 `CapacityFlushes`（`:56`）。
- 单测锁定：`TrCacheTests.cs:91-98`（capacity=4，灌 50 键，断言 `Count ≤ Capacity` 且 `CapacityFlushes > 0`）。
- ⚠️ **代价（P2）**：清空丢**全部**热条目。若真实键空间 > 512（当前「组件 60 键 + int 参数变体」远小于 512 ⇒ **当前安全**），将出现周期性全量重算抖动。建议：改分段/近似 LRU，或把容量提为可配置常量并在 `CapacityFlushes` 增长时告警。

### 5.2 失效语义与依据
`Tr.cs:33` + `TrCache.cs:93-102`
```csharp
private static readonly TrCache Cache = new TrCache(512, () => LanguageDatabase.activeLanguage);
...
private void Sync()
{
    object current = langProvider();
    if (stampBound && ReferenceEquals(current, langStamp)) return;
    if (stampBound) LanguageInvalidations++;
    stampBound = true; langStamp = current; plain.Clear(); withInt.Clear();
}
```
判定依据 = **对 `LanguageDatabase.activeLanguage` 取引用相等（`ReferenceEquals`）**，**不是**版本号、**不是** `LoadedLanguage` 内部字段比较。
- 首次绑定不算失效（`stampBound=false` 分支）——`TrCacheTests.cs:82` 锁定。
- 失效即**整体清空**（非按语言维度分桶）。
- null provider 退化为「永不失效」而非抛异常（`TrCacheTests.cs:120-125` 锁定）。

### 5.3 「失效是否可能失效」——推演（三层）
| 失效模式 | 是否可能 | 说明 |
|---|---|---|
| **① 切语言但引用不变** | **取决于 RimWorld 实现** | 若 `LanguageDatabase.SelectLanguage` **重新赋值** `activeLanguage`（加载新的 `LoadedLanguage` 实例）⇒ 引用变 ⇒ 失效触发 ✅。若**原地改写同一实例** ⇒ 引用不变 ⇒ **缓存永不失效，玩家切语言后界面仍是旧语言** ❌。**知识库 `RimWorld模组开发知识库` 中 `LanguageDatabase`/`activeLanguage`/`LoadedLanguage` 命中 0**（我已 `grep` 全 `references/`）⇒ **无法自证，标记「需 rimworld-mechanics 反编译核实」**，我不猜。 |
| **② 缓存键未含语言维度** | **确实不含** | 键 = `key`（+ int 参数），**无语言分量**。⇒ **正确性 100% 依赖 ①的失效触发**；一旦 ① 失效，跨语言污染（旧语言串）直接可见且**无兜底**。 |
| **③ 翻译在无语言切换时变化** | 可能（低概率） | 第三方 Mod 动态改写翻译/注入语言包但不改 `activeLanguage` ⇒ 不失效。属极端边界。 |

### 5.4 可诊断性：**不足（P2）**
- `LanguageInvalidations`（`:52-53`）与 `CapacityFlushes`（`:55-56`）均为 `internal`，**仅 `RUIF.Tests` 可见**（`AssemblyInfo.cs:17` `InternalsVisibleTo`）；生产环境**无日志、无 UI 暴露**。
- ⇒ 最危险的失效模式「**该失效却永不失效**」**没有任何自检**（因为它无法自证应为「失效」）。建议：**在语言切换事件/语言戳变化时打一条 `Log.Message`**（低成本、可诊断），或在诊断页暴露两个计数器。

---

## 6. `TexCache` 键设计与无界增长风险

### 6.1 键族与隔离
| 构造器 | 实现 | 取值范围 | 单射性 |
|---|---|---|---|
| `KeyOf`（`:122-137`） | **位段拼接**：`[63]标记位=1`，`kind[0..7]`，`a..e` 各 11 位 `[8..62]` | `kind∈[0,255]`，`a..e∈[0,2047]` | **合法域内可证明单射**（段不重叠）；越界**夹紧 + `Log.ErrorOnce`**（`:124-128`、`:140-146`） |
| `Key`（`:48-57`） | FNV 风格 `Hash64`，**已修** `cb.a` 漏位 + `w<<60` 溢出 | 无范围限制 | **哈希，不可证单射**；最高位清 0 |
| `KeyVersioned`（`:66-73`） | `Hash64`，**已修** `version≥2^22` 与尺寸位段重叠 | 无范围限制 | 同上 |
| `KeyData`（`:91-99`） | `Hash64(kind, dataHash, a, b, c)` | 无范围限制 | 同上；**v1.1.2 修 `Industrial.Gauge` 键位槽事故的落地件** |
| `KeySector`（`:157-163`） | 转调 `KeyOf`：`FoldColor(color)`、起角 mod 720、张角/孔洞比/边长夹紧 | 见下 | 依赖 `KeyOf` 单射 + **`FoldColor` 有损** |

**隔离结论（准确表述）**：`KeyOf` 最高位恒 1（负数）↔ `Key`/`KeyVersioned`/`KeyData` 最高位恒 0（非负）⇒ **`KeyOf` 与三者严格隔离**（`TexCacheTests.cs:63-71` 锁定）。但 **`Key`/`KeyVersioned`/`KeyData` 三者共用同一 63 位哈希空间，彼此不隔离、不可证无碰撞**。三者同时喂入**同一个 `map`** ⇒ 理论上跨构造器碰撞 = 静默串味。已文档化（`:43-44`、`:88-89`），风险已登记，**我接受为 P2 已登记风险**。

### 6.2 越界夹紧策略
- `Seg`（`:140-146`）：越界 → 夹紧 + `Log.ErrorOnce`（seed 唯一）⇒ **符合「越界不静默」**。历史事故（`Industrial.Gauge` 阈值 `6085` 被夹进 `[0,2047]` 与其它值共用键、首帧每次刷告警）**已由 `KeyData` 路由解决**：`grep` 确认 `Industrial.cs:181` 现走 `TexCache.KeyData(0x54, …)`（哈希，无 11 位上界），**不再走 `KeyOf`** ⇒ 事故路径已消除。✅

### 6.3 ❌ 未消除的静默有损路径（P2）：`FoldColor`
`TexCache.cs:166-170`
```csharp
private static int FoldColor(Color c)
{
    Color32 q = c;
    return (q.r * 31 + q.g * 17 + q.b * 7 + q.a) & 0x7FF;   // 2^32 → 11 位，静默、无日志
}
```
- 把 RGBA **静默折叠**成 11 位（`:169` 注释自述「**6 色调色板内无误撞风险**」）。**该声称我已独立验算**：`Tokens.Series` 的 6 色折叠值 = Accent 820 / Info 566 / Alive 579 / Warn 44 / Dead 737 / Muted 142 —— **两两互异，最小间隔 13** ✅ ⇒ **对内置 6 色成立**。
- ❌ **但对任意色值不成立**：`KeySector` 是 `public` 且 `DonutChart(…, Color[] colors)`（`Charts.cs:395/411`）**接受消费者任意 `Color`** ⇒ 不同色值可折到同一 11 位槽 ⇒ **贴图串色**（环形图某段取到别段颜色的贴图）。
- 与 `Seg` 的处理方式**不对称**（`Seg` 越界必报，`FoldColor` 有损必静默）⇒ 建议：① 折叠后若发生碰撞无法事后发现，故至少**在诊断页/首次使用时记一次提示**；或 ② 改用 `Charts.cs:167` `ChartFoldRgb` 的 24 位折叠 + `KeyData`（该路径已无 11 位上界）。

### 6.4 ❌ 文档与实现矛盾（P2，同 §2.4）
| 位置 | 陈述 |
|---|---|
| `Charts.cs:391-392` | 「调色板按 `Tokens.Series` 的 6 色**序号**编键 ⇒ 若切换主题改变了这 6 个色值，**需 `TexCache.Clear()`**」 |
| `Charts.cs:411-412`（实现） | 实际传入 **`segColor`（真实 Color）** → `KeySector(segColor, …)` |
| `TexCache.cs:151-152` | 「**色值直接进键（不是调色板序号）** ⇒ 主题切换后键自动变化、缓存自动失效，**调用方不必记得调 `TexCache.Clear()`**（原实现传序号，换主题后会一直画旧色）」 |

⇒ **`Charts.cs:391-392` 是未随 2026-09-13 修复更新的过期注释**，且与 `TexCache.cs:151-152` **直接冲突**。今日无实际危害（换肤层已移除），但**未来维护者若按注释「改回序号键」将重新引入「换主题仍画旧色」缺陷**。建议：删除/更正该注释。

### 6.5 容量与无界增长
- `MaxEntries = 128`（`:27`），`EvictIfNeeded()`（`:224-237`）`while (order.Count > MaxEntries)` 淘汰 `order[0]`（**FIFO/插入序，非 LRU**），并 `Object.Destroy`（`:234`）。
- 键一致性：`Publish` 同步写 `map` 与 `order`（`:206-207`）；`TryGet` 对已 Destroy 贴图 `map.Remove` + `order.Remove`（`:180-181`）；`Clear()` 双清（`:213-222`）⇒ **无键泄漏路径**。✅
- ✅ **无随组件种类/尺寸无界增长**：条目数恒 ≤ 128。
- ⚠️ **但有界维度是「条目数」而非「字节数」（P2）**：`Publish(w, h, …)` 的 `w/h` 由调用方给定，单条纹理 = `w*h*4` 字节。框架内实际最大用例：`Charts` 环形 `Size=128`（128²×4 = 64KB）、`Sparkline` `tw×th ≈ rect 尺寸`（如 400×200 → 320KB）、`TexAtoms.Heat` `cols×rows`。**最坏 128 × 大尺寸**（消费者可传大 `rect`）⇒ 内存上界**未在代码或文档中界定**。建议：把「上限 = 128 条**或** 累计像素预算」写进注释/文档，或对 `w*h` 设上限并告警。
- ⚠️ **长会话无主动回收**：框架内 **`TexCache.Clear()` 无任何调用点**（`grep` 仅命中注释 `Charts.cs:392`、`TexCache.cs:152`）。⇒ 跨存档/跨殖民地切换时旧贴图**保留至被 FIFO 淘汰**。因键为内容派生（数据变则键变）⇒ **正确性不受影响**，仅内存驻留。可接受，但建议在「新档/换档」时机调用一次 `Clear()`（需 GATE-COMP/REL 侧确认时机 API——`LongEventHandler` 之类的调用时机**需 rimworld-mechanics 核实**）。

---

## 7. 不可变性 / 红线扫描结果

### 7.1 `Tokens` 运行期不可变 —— **成立（PASS）**
```bash
grep -rnE "\bstatic\b" Source/Core/Tokens.cs | grep -v "static class"
```
- 16 色：全部 `public static readonly Color`（`Tokens.cs:72-102`）⇒ **引用不可变**（`Color` 为值类型 struct，`readonly` 语义即彻底不可变）。
- 间距/尺寸/行高：`public const float`（`:16-58`）⇒ 编译期内联常量。
- 字号：`FontLabel/FontBody/FontValue` 为**只读表达式体属性**（`:63/65/67`）。
- 派生：`AccentSoft => Draw.WithAlpha(Accent, 0.16f)`（`:105`）、`Series(int)`（`:108`）——每次调用**新算**，无缓存、无可写状态。
- **`Tokens` 中无任何可写 `static` 字段** ⇒ 「编译期常量 / 只读静态值，运行期不可变」**声明属实** ✅

### 7.2 红线：组件禁 `new Color`（README:99）—— 3 处命中
```bash
grep -rn "GUI\.color\|new Color(" Source/Components Source/Layout --include=*.cs
```
| 位置 | 内容 | 判定 |
|---|---|---|
| `InputExAtoms.cs:61` | `return new Color(r + m, g + m, b + m, alpha);` | ✅ **算法输出**（手写 HSV→RGB，`:43`），非硬编码主题色 |
| `TexAtoms.cs:75` | `var ka = new Color(bg.r, bg.g, bg.b, from);` | ✅ **由入参色派生 alpha**，非硬编码 |
| **`TexAtoms.cs:76`** | `var kb = new Color(0f, 0f, 0f, to);` | ⚠️ **硬编码黑字面量**（渐隐丢弃端）。功能上是「透明端占位」，但字面上违反「颜色一律走 `Tokens`」⇒ **P3**，建议提取为 `Tokens` 常量或加注释豁免 |

**未发现**「绕过 `Tokens` 取主题色」的实质违规（框架已无换肤层）。

### 7.3 红线：组件禁裸 `GUI.color`（README:99）—— **PASS**
命中 3 处**全在注释**（`FeedbackAtoms.cs:62`、`FeedbackAtomsB.cs:26`、`TexAtoms.cs:19`，均为「原实现用 `GUI.color` 不生效/被覆盖」的踩坑记录）。实际写入一律经 `Draw.Color(...)` 作用域：`TexAtoms.cs:68`、`:98`、`:141`（`using` 保证成对恢复）。

---

## 8. Def / Runtime / Save 三态数据边界分类结论

### 8.1 分类表
| 数据 | 语义 | 四态归属 | 稳定 ID | 默认值 | 入 Save | 版本 | 迁移动作 |
|---|---|---|---|---|---|---|---|
| `Tokens.*`（16 色 / 间距 / 字号 / 尺寸） | 设计令牌基线 | **Def 级静态**（实为编译期常量/只读静态） | 成员名（代码标识） | 内联字面量 | **否** | 无 | 无 |
| `Tokens.Series(i)` / `AccentSoft` | 由基线派生 | **Def 级静态（纯函数）** | — | — | **否** | 无 | 无 |
| `Api.VersionString` / `Version` | 版本门面 | **Def 级静态** | `"1.1.2"` | — | **否** | 无 | 无 |
| `TrCache` 内容 | 翻译结果缓存 | **Runtime**（派生缓存） | 翻译键 + int 参数 | 空（未命中即不存） | **否** | 容量 512 | 语言戳变化整体清空 |
| `TexCache` 内容 | 烘焙贴图缓存 | **Runtime**（派生缓存 + 引擎资源） | `long` 复合键（内容派生） | — | **否** | 容量 128 | FIFO 淘汰 / `Clear()` / 外部 Destroy 自动失效 |
| `NavAtoms.clicked` | 折叠树本帧点击回传 | **Runtime（帧内瞬态）** | — | `null`（每次进入置空） | **否** | 无 | 无 |
| `Charts.tickBuf` / `Industrial.shareBuf` | 每帧复用缓冲 | **Runtime（复用内存）** | — | 零 | **否** | 无 | 无 |
| `PawnTableHost.PawnTable/defName` | 消费者持有的跨帧表宿主 | **Runtime（跨帧 UI 状态，消费方持有）** | `tableDefName`（**原版 Def 稳定名**，非 label） | `null` | **否（框架不提供 Scribe 通道）** | 无 | 无 |
| `UndoScope` / `ColorScope` / `EnabledScope` 等 | 绘制态作用域 | **UI（表现态，`using` 成对恢复）** | — | — | **否** | 无 | 无 |
| 翻译文本（`Languages/**/Keyed/*.xml`） | 文案 | **UI（本地化资源）** | `RUIF.Comp.*` 键 | 中英随包 | **否** ✅ | 无 | 无 |
| **Save 面** | — | — | — | — | **空集** | — | — |

### 8.2 边界结论
1. **Save 面为空集** —— 框架不参与存档（§3 双证据）。
2. **Def 面实际为空集** —— `Tokens` 虽语义上属「Def 级静态配置」，但**实现在代码内**（非 XML Def），无常量可配置性；这是「纯组件库」定位的必然选择。
3. **Runtime 面全部为「可重建派生」** —— 两个缓存 + 三个瞬态；**无一入 Save、无一入 Def、无一入 UI 生命周期**：`TrCache`/`TexCache` 均在 `Core/`（L0），`Components/` 只消费。
4. ⚠️ **一处边界模糊（移交 GATE-UI/ARCH 裁决）**：`PawnTableAtoms.CurrentMapColonists()`（`PawnTableAtoms.cs:108-112`）读 `Find.CurrentMap` / `mapPawns.FreeColonists` —— **框架组件内读游戏实时状态**，与 `README.md:99` 红线「框架只消费 Read Model 快照，组件内**禁读游戏状态**」字面冲突。它作为**默认便捷参数**存在（调用方可覆盖 `pawnsGetter`）。**不是存档/数据破坏问题**，但属数据边界声明与实现的偏差 ⇒ 建议由 GATE-UI + GATE-ARCH 定裁定（或修订红线措辞为「除显式标注的桥接件外，组件不得读游戏状态」）。

### 8.3 「若未来要加配置/持久化，当前架构的口子是否足够」
- **现状 = 零口子**：无 `Mod` 子类、无 `ModSettings`、无 `Def`、无 `GameComponent` ⇒ **未来加持久化是纯新增，无旧档迁移负担**（schema 可从版本 1 起算，无历史字段）。这是**最大优点**。
- **但「运行期可变主题」与现有架构不兼容（架构级改造成本，非加字段）**：153 个组件**直接静态引用** `Tokens.X`（如 `Tokens.Panel`、`Draw.Box(rect, Tokens.PanelRaised)`）。要让令牌在运行期可变，必须把所有读取点改为经 Provider / Read Model（GOV-002 组合优先、GOV-005 契约通信），属**跨全库机械改造**。
- **建议的口子设计（若将来要加）**：
  1. `ModSettings`（`Mod` 子类 + `GetSettings<T>`）= Save 面 → **必须有** `int schemaVersion`（首字段写出、默认 1）+ 每个新字段默认值 + 幂等迁移 + 失败可读日志。
  2. `DefModExtension` / 独立 `Def` = Def 静态配置面（只放静态、禁放 `Color` 之类的 UI 表现 —— 若必须，存 `string` 稳定名再由代码映射，**禁存翻译文本**）。
  3. 组件入参 = Runtime 面（现状不变）。
  4. **明令禁止**：把 `Tokens` 的可变副本或翻译文本写进 Save（AI-004：UI/翻译/Texture 不入 Save）。
- **移交机制研发部的两个待核实点**（我不猜 API）：
  - `LanguageDatabase.activeLanguage` 在切语言时是**重新赋值引用**还是**原地改写**（决定 §5.3 的失效是否成立）。
  - `ModSettings` 在存档 `<modSettings>` 中的写入键与「Mod 被移除后」的容错行为（决定未来加设置后的迁移/回滚设计）。

---

## 9. 阻塞项（按 P0/P1/P2/P3 分级）+ 建议动作

### P0（禁止交付）—— **无**

### P1（阻止交付）—— **无**

### P2（必须修复或明确风险）
| ID | 项 | 位置 | 建议动作 |
|---|---|---|---|
| **P2-1** | `TrCache` 语言失效**无生产自检**：「该失效却永不失效」不可察觉；计数器 `internal` 不暴露 | `TrCache.cs:52-56`、`Tr.cs:33` | 语言戳变化时打一条 `Log.Message`（含计数），或诊断页暴露计数器；把 §5.3 的依赖假设写进注释 |
| **P2-2** | `TrCache` 超限策略为**全清**，键空间 > 512 时周期性全量重算抖动 | `TrCache.cs:85-91` | 现容量下不触发；建议改近似 LRU 或等价容量告警（`CapacityFlushes` 阈值） |
| **P2-3** | `FoldColor` **静默有损折叠**（RGBA → 11 位），消费者任意色值可碰撞 ⇒ 贴图串色；与 `Seg` 的「越界必报」不对称 | `TexCache.cs:166-170`、`Charts.cs:395/411` | 改 24 位折叠 + `KeyData`（已有 `Charts.cs:167 ChartFoldRgb` 可复用）；或把「仅支持 6 色板」写入公共 API 文档并加断言 |
| **P2-4** | `TexCache` 上限按**条目数**而非**字节**，单条纹理尺寸由调用方任意给定 ⇒ 内存上界未界定、无主动回收调用点 | `TexCache.cs:27`、`:188-210`、`Clear()` 无调用者 | 文档/注释写明「128 条 × 调用方尺寸」预算；对 `w*h` 设上限并告警；在换档时机调用 `Clear()`（时机 API 需机制核实） |
| **P2-5** | `NavAtoms.clicked` **可变静态字段**（非重入安全）⇒ 命中 Architecture Review 触发条件；`PawnTableAtoms.cs:118` 注释把跨帧 UI 状态误称「持久状态」 | `NavAtoms.cs:217`、`PawnTableAtoms.cs:118` | 改为实例/`ref` 回传或 `[ThreadStatic]`+重入保护；静态可变状态**登记 EXC** 由架构部裁决；修正注释术语 |
| **P2-6** | 最新两张「当前有效判定」Gate 表**缺 GATE-DATA / GATE-SAVE 行**（缺行 ≠ N/A，规范要求 N/A 必写原因） | `docs/05-…/健康评估与发布记录.md` §12.2、§14.2 | 在 Gate 表补行：`GATE-DATA \| PASS WITH EXCEPTION（…）`、`GATE-SAVE \| N-A（框架无存档面，源码+dll 双零证据）` |
| **P2-7** | `Charts.cs:391-392` 缓存失效注释与实现及 `TexCache.cs:151-152` **直接矛盾** | `Charts.cs:391-392` | 更正注释（色值已直接进键，无需 `Clear()`）——防未来「按注释改回序号键」重现缺陷 |

### P3（交付前修正）
| ID | 项 | 位置 |
|---|---|---|
| P3-1 | `Tr.cs:60-62` 过期注释（引用已删设置页）+ 三参 `Tr.Get` **0 调用点**死 API | `Tr.cs:59-67` |
| P3-2 | `README.md` 数据声明漂移：`:20` 版本写 **v1.1.0**（实测 v1.1.2）、`:22` 单测写 **171/171**（实测 197/197）、`:112` 文件数写 **19 文件**（实测 21） | `README.md:20/22/112` |
| P3-3 | `TexAtoms.cs:76` 硬编码 `new Color(0f,0f,0f,to)`（红线条文「颜色一律走 Tokens」） | `TexAtoms.cs:76` |
| P3-4 | 发布区 `Defs/`、`Textures/` **空目录残留**（镜像清理不删空目录）；对游戏无影响 | `Mods/RUIF/Defs`、`Mods/RUIF/Textures` |
| P3-5 | `About.xml` 声称「公开 API **113 类型 / 603 成员**」，与 `docs` §14.2（v1.1.0）「**73 类型 / 438 成员**」不一致 ⇒ **需 #6/#1 核实**（疑为 v1.1.2 新增后未同步 docs） | `About/About.xml`、`docs/05-…:524-534` |
| P3-6 | 本仓库 git **仅 1 次 squash commit** ⇒ 设置系统移除/RUIFSettings 删除**无法 git 复核**，仅有 CHANGELOG/docs 文档链 | `.git` |

> 诚实登记：**以下未执行，标 NOT RUN**（沙箱无游戏进程）——
> 实机启动无红错、`Player.log` 无 RUIF 相关告警、切语言后文案变更、换档后贴图缓存行为、长会话内存观测。

---

## 10. 旧档验证用例

| 用例 | 输入 | 期望 | 结论 |
|---|---|---|---|
| S-1 | v2.x 时期存档（RUIF 曾启用）在 v1.1.2 下加载 | 无「找不到 Def `RUIF_Settings`」、无「找不到类型 `RUIFSettingsDef`」错误 | **推演 PASS**（Def 非存档引用、无 ModSettings）· **实机 NOT RUN** |
| S-2 | 存档中 RUIF 曾启用、现更新至 v1.1.2 | 无 `<modSettings>` 残留清理告警（框架从不写该节点） | **推演 PASS** · **实机 NOT RUN** |
| S-3 | 旧档 + 手工残留 `Mods/RUIF/Defs/RUIFSettings.xml`（P3 边界） | 一条可读红色 XML 错误（类型缺失），存档不损坏 | **推演 PASS**（RimWorld 跳过坏 Def）· **实机 NOT RUN**；实测磁盘**零残留** ⇒ 该用例触发概率极低 |
| S-4 | 英文 ↔ 简中切换后打开含 RUIF 组件的窗口 | 文案随语言变化（验证 `TrCache` 失效） | **NOT RUN**（依赖 P2-1 的机制核实结论） |
| S-5 | 全新档 → 反复切殖民地 / 连续载入不同存档 → 反复绘制图表 | `TexCache` 条目 ≤ 128、无持续增长；环形图各段颜色正确 | **推演 PASS**（FIFO 淘汰）· **实机 NOT RUN** |
| S-6 | 新档加载后立即开关含 `CollapsibleTree` 的窗口 | 每次点击回传正确节点；无跨窗口串点 | **推演 PASS**（帧内瞬态）· **实机 NOT RUN** |

---

## 11. 缓存与重建

| 缓存 | 权威来源 | 重建路径 | 失效条件 |
|---|---|---|---|
| `TrCache`（`Tr.cs:33`，512 条） | `LanguageDatabase`（构造注入 factory，`TrCache.cs:16`） | 清空后按需再调 factory（翻译键 → `.Translate()`） | ① 语言戳引用变化 ⇒ 整体清空（`:93-102`）；② 条目数达 512 ⇒ 全清重建（`:85-91`）；③ null 结果**不缓存**（`:65/79`） |
| `TexCache`（`TexCache.cs:27`，128 条） | 调用方像素源（`Publish` 传入 `Color32[]`） | `Publish` 重新烘焙像素 → `Texture2D` | ① 内容派生键变化（数据/尺寸/颜色变 ⇒ 键变）；② 条目数 > 128 ⇒ FIFO 淘汰最旧（`:224-237`）；③ 贴图被外部 `Destroy` ⇒ `TryGet` 判未命中并清键（`:179-181`）；④ 显式 `Clear()`（**框架内无调用点**） |
| `Tokens` | 无（编译期常量） | 无需重建 | 不存在（不可变） |
| `NavAtoms.clicked` | 无（帧内瞬态） | 每次进入 `CollapsibleTree` 置 `null` | 每次调用 |
| `tickBuf` / `shareBuf` | 无（复用内存） | 每次使用前由计算函数覆写 | 每次使用 |

**缓存可重建性判定：PASS**（两个缓存均可从权威数据无歧义重建；无「缓存即唯一真源」的反模式）。

---

## 12. GATE 自检

### GATE-DATA
| 检查项 | 结果 |
|---|---|
| Def 只放静态 | **N/A**（框架无 Def；`Tokens` 为编译期常量/只读静态 ⇒ 天然静态） |
| Runtime 不入 Def/UI | **PASS**（缓存与瞬态全在 `Core/`；UI 作用域成对恢复） |
| Save 有 ID + 默认值 + 版本 | **N/A**（Save 面为空集，见 §3） |
| 缓存可重建 | **PASS**（§11） |
| 无界静态可变状态（AI-004） | **PASS WITH EXCEPTION**（有界缓冲 ✅；`NavAtoms.clicked` 为有界可变静态 → P2-5 登记） |
| **GATE-DATA 总判定** | **PASS WITH EXCEPTION**（例外 = P2-1…P2-7、P3-1…P3-6） |

### GATE-SAVE
| 检查项 | 结果 |
|---|---|
| schema 版本可识别 | **N/A**（无 schema） |
| 新字段有默认值 | **N/A**（无字段） |
| 旧档可加载 | **PASS**（推演；实机 NOT RUN）——设置系统移除**不影响既有存档**（§2.3 四条推理） |
| Migration 幂等且可诊断 | **N/A**（无迁移对象） |
| **GATE-SAVE 总判定** | **N/A** —— 原因：框架不实现 `IExposable`/`Comp`/`GameComponent`/`ModSettings`、不声明 `Def`；源码与发布区 dll **双重零命中**（§3.1、§3.2）⇒ 无任何可持久化数据契约。 |

---

## 13. 交给下游

- **→ 界面与本地化部**：展示字段与单位语义——
  1. `Tokens` 全部为**只读**：16 色（`Window/Panel/PanelRaised/Card/CardHover/Border/BorderSoft/BorderHair/Text/Muted/Dim/Accent/Info/Alive/Dead/Warn`）+ 派生 `AccentSoft`（alpha 0.16）/ `Series(int)`（6 色轮转）；间距 `SpaceXxs/Xs/Sm/Md/Lg`；字号 `FontLabel/FontBody/FontValue` + `FontBodyLineHeight`；尺寸 13 项。**展示时须标注「不可变、无运行期覆盖」**。
  2. `Px.Floor/Ceil/Snap/Span` 的单位语义 = **UI 缩放后的像素**（`UIScaling.AdjustRectToUIScaling`）——`Charts.cs:387-392` 的注释须同步更正（P2-7）。
  3. ⚠️ `_demo/RUIF_Demo/Source/DemoMod.cs:35-64` 存在多处**中文硬编码**（该包已部署到 `Mods/RUIF_Demo`）⇒ 请按 GOV-010 判定归属（GATE-LOC 范围）。
- **→ 质量与发布部**：存读档回归用例 = §10 的 S-1…S-6（S-1/S-2/S-3 为存读档回归必跑；S-4/S-5/S-6 为缓存与瞬态回归）；另需补 **P3-2 的 README 数据声明核对**与 **P3-5 的 API 计数核实**。
- **→ 机制研发部（`rimworld-mechanics`）待核实 2 项**：
  1. `LanguageDatabase.activeLanguage` 在切语言时**是否重新赋值引用**（决定 `TrCache` 失效是否真实生效，§5.3 ①）；
  2. 换档/载入存档的**可用回调时机**（供 `TexCache.Clear()` 选点，§6.5）。
- **→ 架构部（`rimworld-architect`）待裁决 2 项**：
  1. `NavAtoms.cs:217` 可变静态状态（§4.2，AI-004 / Architecture Review 触发条件）；
  2. `PawnTableAtoms.CurrentMapColonists()` 读 `Find.CurrentMap` 与 `README.md:99`「组件禁读游戏状态」红线的边界裁定（§8.2 第 4 点）。
