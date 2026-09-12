# RUIF Changelog

> 格式：`版本 — 日期` + `新增 / 变更 / 修复` + `Gate 结果摘要`
> SemVer：主版本内公共 API 冻结；删改 API 先 `[Obsolete]` 保留一个主版本（I5 起生效）

---

## v1.0.1 · 回归纯组件库（移除主题 / 换肤层）+ 开发者包精简 — 2026-09-12

> **发布定位**：v1.0.0（2026-09-11）之后的全部未发布内容在本版一次性收口；三项均为**收窄**性质（删除能力 / 精简交付物），无新增公共 API。
> **SemVer 判定**：本次含**公共 API 删除**（`Theme.*` / `DrawDecor` / `VanillaUiSkin` / `RUIFSettings` 等），严格口径应记 MAJOR；
> 判为 **PATCH** 的依据是：① 该层自 v1.0.0 冻结起**从未被任何 Mod 消费**（仓库内零引用）；② 框架**尚未在 Workshop / GitHub 公开发布**，不存在需要迁移窗口的既有消费者。
> ⚠️ 下次出现同类删除时，若框架已公开发布，必须回到 `[Obsolete]` 保留一个主版本的常规路径。
> **本节结构**：① 开发者包精简 · ② 移除主题 / 换肤层 · ③ 已撤销的中间线（09-11 先增、09-12 后删）· ④ 发布 Gate 与回滚。

### ① 开发者包精简：删除与宝典重复的《组件设计图鉴》

> 目标：对外包不留重复件。`组件设计图鉴.html`（240 KB）的内容已被《终极宝典v1.0-fixed.html》**完整吸收**，故整卷退场。

**变更（文档 / 随包件）**
- 删除 `docs/组件设计图鉴.html`，并从开发者包白名单移除。依据（可复核）：
  ① `docs/_build_crown.py` 本就把该卷当**主卷**按 `<h2 id>` 逐段抽取后融合进宝典（id 加 `m-` 前缀）；
  ② 该卷 **11 个章节**（`tokens·a1·b1·c1·d1·e1·pat·p1·plan·x-a·fdeep`）在宝典中**逐一对应**，宝典另有图表卷、新增卷与装饰层历史章节；
  ③ 组件卡数量 **170 → 182**（不丢件）。
- 同类先例：`图表组件图鉴.html`、`新增组件图鉴.html` 早已不随包，本卷属遗漏。
- 顺带移除一处**过期误导**：该卷仍是 v0.3 口径（顶部 5 套主题实时切换、侧栏「落地进度 0/6」快照），而换肤层已于 v1.0.0 移除；宝典相应章节已标注「⛔ 已于 v1.0.0 移除 · 历史章节」。
- 同步引用点：`_scripts/sync_devdocs.ps1`（白名单 + 头注释）、`_scripts/verify_release.ps1`（第 7 道必备文件清单 6→5 项）、
  `docs/开发者入口.html`（§三④「看效果」、§五「文档地图」改指宝典）、`README.md`（§4 文档索引、§8 历史件说明）。
- `docs/_build_crown.py` 保留为历史构建器（已在脚本抬头注明源卷已并入宝典、勿再执行）。
- **`About/About.xml` 描述重写为简明规格表**（中英对称、一行一条）：删掉会**原文显示星号**的 `**` 标记（RimWorld 与 Steam 都按纯文本渲染），并把具体 API 名（`Api.IsAvailable` 等）从简介挪回接入指南；简介只留六个卖点 —— 一行上手 / 零件齐全 / 纯组件供应 / 零前置依赖 / 中英双语 / 离线文档随包 —— 末行为来源标注。描述总行数 27 → 25，但每条都是短句，观感从"技术清单"变成"规格表"。

**新增（随包）：零基础上手教程 + 文档示例真编译验证**
- **`docs/零基础上手教程.html`**（面向**没写过 RimWorld 界面**的第三方开发者）：「怎么用这份教程」→ 三个概念 → 接依赖两模式（硬依赖 `Private=false` + `modDependencies` + `Api.IsAvailable` 门控；软依赖反射探测 + `[MethodImpl(NoInlining)]` 隔离范本）→ **画第一个窗口**（逐行讲解）→ 布局速成 → **组件配方表**（面板/文字/状态/输入/数据图表/导航反馈，按"我想…"查）→ **六个必踩的坑** → 本地化 → **报错对照表** → 文档地图 → §10 附约 120 行完整示例。
- **验证工程 `_verify/consumer_docs/`（落地接入审计建议 S-1）**：把教程与入口里的示例**逐字**收进一个真实消费者工程，对**发布区 `RUIF.dll`** 真编译（`verify_samples.ps1` 一条命令，0 错 0 警）。**建立当日即揪出 3 个真缺陷**：

| 级别 | 缺陷 | 影响 | 修法 |
|---|---|---|---|
| P2 | `gen_api_reference.ps1` 把多维数组打成 `float[]`（只判 `IsArray`，未取 `GetArrayRank`） | `API参考.md` 的 `Charts.HeatmapGrid` 签名错误 ⇒ 消费方照抄必得 `CS1503: 无法从 float[] 转换为 float[*,*]`（全 API 唯一一处） | 按 rank 还原为 `[,]`，已重新生成 API参考 |
| P2 | 教程草稿误用 `AlertPriority.High` | 该枚举真实成员是 `Low/Normal/Warn/Urgent`，**没有** `High` | 改用 `Urgent`，并作为"别猜 API 名"的反例写进文档 |
| P3 | `RUIF.Command` 与 `Verse.Command` 同名 | 消费者同时 `using RUIF; using Verse;` 时 `Command` 报 `CS0104` | 教程「坑 4」给出全限定写法，并在验证工程里固化 |

**修复（发布链路 · 严重）**
- **`About\PublishedFileId.txt` 被同步脚本当垃圾删掉 ⇒ 创意工坊出现重复条目**：`sync_ruif.ps1` 第 ④ 步镜像清理的规则是「发布区存在、开发区不存在 ⇒ 删」，而该文件**只由游戏在上传成功后写入发布区**（开发区本来就没有）⇒ 每次同步都被删；下一次上传因找不到条目 ID，被 Steam 当成**新作品**另建一个条目（现象 = "同一个 Mod 在工坊里变成两个"）。
  **修**：① `sync_ruif.ps1` 增加 `Keep` 白名单（`About\PublishedFileId.txt` 不参与镜像清理），并在同步结尾把它**回写开发区**，防止再次丢失；② `verify_release.ps1` 的镜像检查同样放过该文件，并新增断言「工坊条目 ID 受保护：两区都存在且内容一致」，任一不满足即 **FAIL 阻止发布**。
  **顺带记录一个自踩的坑**：`Keep` 里存的是「相对 Mod 根」的路径（`About\PublishedFileId.txt`），而清理循环里的 `$rel` 只相对当前子目录 —— 第一次比较漏拼目录名，导致例外永不命中（门禁仍报 FAIL）；已修正并复验通过。

**Gate 结果摘要**
- `verify_release.ps1`：**九道全 PASS**（第 7 道必备文件：6 项 → 删《组件设计图鉴》后 5 项 → **加入《零基础上手教程》后 6 项**）。
  > 口径更正：现行门禁为 **9 道**（第 10 道「Def 字段 ↔ C# 字段」随 `Defs/RUIFSettings.xml` 一并移除）。`docs/健康评估与发布记录.md` §8/§10 写的"十道"是 v2.1.1 / v2.2.1 时期的历史陈述，保留不改。
- `health_check.ps1` 组 3「文档 ↔ 实现一致性」：审计面料减少 1 件，**无新增命中**。

---

### ② 移除主题 / 换肤层，回归纯组件库（公共 API 删除）

> 目标：把 RUIF 还原为**纯组件供应**——组件库不应内置换肤机制。本次为**公共 API 删除**。
> 因该层从未被任何 Mod 消费（仓库内零引用），故不保留 `[Obsolete]` 壳。

**变更（删除）**
- 删除主题引擎：`Source/Theme/` 整目录 —— `Theme.cs`（注册表 / Register·Apply·TryGet / vanilla·dark 两套 / 12 套内置）、
  `ThemeDecor`、`ThemePresets`（+BuiltIn）、`VanillaUiSkin`（原版 UI 反射换色）、`ThemeAssets`、
  `ThemeContrast`、`ThemeMotion`、`DrawDecor`。
- 删除设置页与 XML 配置：`Core/RUIFMod.cs`（主题画廊设置页）、`Config/RUIFSettings.cs` + `RUIFSettingsDef.cs`、
  `Defs/RUIFSettings.xml`。
- 删除主题贴图资产 `Textures/ThemeAssets/`（14 套）与设置 / 主题名语言键（`Languages/*/Keyed/RUIF_UI.xml`）。
- 删除主题相关单测（ThemeTests / ThemeDecorTests / VanillaUiSkinTests / ContrastTests / SettingsTests / SettingsDataTests）
  与换肤示例工程（`Sample/`、`DevDocs/_Template/`）。
- 代码层残留清理：`MiscAtoms.ThemedScrollBar` → `ThinScrollBar`；`Core/Draw.cs` 与各组件注释去掉主题字样。

**新增（保留能力 · 中性命名）**
- **`RUIF.Tokens`**（`Source/Core/Tokens.cs`）：组件库唯一、固定的设计令牌（16 色 + 间距 + 字号 + 尺寸），
  全部为编译期常量 / 只读静态值，运行期不可变。取代原 `Theme` 的「令牌」职责，去掉「可切换」语义。
- **`RUIF.Api`**（`Source/Core/Api.cs`）：从 `RUIFMod` 析出的版本探测门面（`VersionString` / `Version` / `IsAvailable`），
  与主题无关，独立保留。

**迁移**
- 消费方若有 `Theme.X` → 改为 `Tokens.X`（同义，取值即原 `dark` 基线）。
- `Theme.Apply / Register / TryGet / ActiveId / VanillaId` 等动态 API 已删除；不存在替代——框架不再支持换肤。

**Gate 结果摘要**
- 编译：`Source/RUIF.csproj` → 0 错 0 警；`Tests/RUIF.Tests.csproj` → 0 错 0 警。
- 单测：**171/171 通过**（主题 / 设置相关用例随功能一并移除）。
- 对 Personal Chronicle 主 mod：**零影响**（主 mod 从未引用 RUIF）。

---

### ③ 已撤销的中间线：XML 热重载 + 全量主题清单 + 原版 UI 全局皮肤注入（2026-09-11 新增 → 09-12 整体移除）

> ⚠️ 本节内容**不构成 v1.0.1 的可用能力**：它记录的是同一未发布周期内的「先增后删」——09-11 按当时方案新增的特性，于次日发现需引入 Harmony 前置依赖、违反「零前置依赖」红线，遂连同本节的公共 API 一并移除。保留原文仅为让决策链可追溯（**不要据此编码**）。

> ⚠️ **未升版本号、未发布**：版本一致性与数字口径（README / About / 封面用的 `90 类型 / 570 成员`）需在下次真正发布时统一重生成。
> 本次已把 `DevDocs/` 与发布区同步到当前构建（门禁十道全 PASS），但**两条主特性的实机效果尚未验证**。

**修复（可读性 · 7 套主题文字对比度不足 —— 用户反馈「有几个主题看不见字」）**
- **症状**：用户报告「有几个主题颜色和字体对比度非常浅，无法肉眼看见字体」。
- **量化体检**：正文其实都合格（9.5~17:1），**看不见的是两类小字**：
  - `dim`（禁用 / 三级文字）：**6 套主题只有 2.29~2.90:1** —— vanilla 2.29 / dark 2.55 / sakura 2.31 /
    parchment 2.30 / nordic 2.44 / bloodmoon 2.72（WCAG 非正文下限 3.0）；
  - 浅色主题把**语义色当小图标 / 标签文字**用：sakura.warn **1.96:1**（`#E69D4F` 压在近白面板 `#FCF3F4` 上）、
    sakura.alive 2.13、parchment.warn 2.18、nordic.warn 2.07、bloodmoon.dead 2.20 ……（全部 < 3.0）。
- **修法（保色相、只调明度、取最小改动量）**：7 套主题共 **24 处**，只动**文字类令牌**
  （`text` / `muted` / `dim` / `accent` / `info` / `alive` / `dead` / `warn`），
  **底色 / 边框 / 装饰配方 / 资产几何一律不动** ⇒ 主题观感不变，只是"字看得清了"。
  判定底色取 {`panel`, `raised`, `card`, `window`} 的**最差**（文字可能落在任一表面上）。
  修改同时落到三处真源：`ThemePresets.BuiltIn.cs`（12 套预设）+ `Theme.cs`（vanilla / dark）+
  出厂 `Defs/RUIFSettings.xml`（224 条色值）—— 后者的「XML ↔ C# 预设逐位一致」单测把住了同步。
- **新增回归护栏 `RUIF.ThemeContrast`（公开 · 纯函数 · 可离线单测）**：
  `Luminance` / `Ratio` / `WorstTextContrast` / `FindWeakPair` + 阈值常量（正文 4.5 / 三级文字 3.0 / 语义色文字 3.0）。
  两个消费点：① 单测 `ContrastTests` 逐套体检 **14 套内置主题**；② `RUIFSettings.RegisterTheme` 对
  XML / 第三方主题在注册期体检，**只告警不夹取**（配色是作者的意图，框架不擅自改色）。
  - 护栏自带**反向用例**：把 sakura 修复前的 `warn` 值塞回去，必须报警 —— 否则它只是个"永远通过"的假测试。
- **如实登记的两条判据边界**：① `text` 压 `accent` 实底不计入（框架不这么画，且那种场景应由调用方选前景色）；
  ② 半透明底衬（`WithAlpha(warn, 0.14)` 的徽章底 + warn 文字）实测最差再降 ~0.2 ⇒ 语义色按 3.0 判定、修到 **≥3.35** 留余量。
- 单测 **230** 项（+4）；280 张资产因调色板变化全部重生成（`--check` 逐像素一致，门禁 6.2 复核）。

**新增（表现面 · RUIF 自有容器 / 按钮贴图族：覆盖面从「原版」扩到「框架自己」）**
- **补的缺口**：RUIF 自己的容器（面板 / 卡片 / 凹槽）此前**只有程序化绘制、没有任何贴图** ——
  而原版压根没有「容器贴图」这种东西（窗口 / 面板 / 卡片全是纯色画的），所以主题切换换不掉框架界面的"底"。
- **新增资产族 `ruif.*`（8 件）**：容器面 `ruif.panel` / `ruif.panel.raised` / `ruif.card` / `ruif.inset`
  （9 宫格，画布 64，槽位分别取 `Panel` / `PanelRaised` / `Card` / `Window` 令牌 —— 凹槽用 `Window`，
  它在所有 14 套主题里都比 `Panel` 更暗）+ 按钮底图 `ruif.button.normal` / `.hover` / `.pressed` / `.subtle`。
  这类资产**没有原版落点**（`Owner == null`）：不参与原版字段注入，只经 `ThemeAssets` 取用，
  因此 `applyToVanillaUi=false` 时同样可用。资产总数 **12 → 20 件**，全主题导出 **280 张 PNG**。
  - 容器面**不画顶部高光带**（与按钮的区别）：容器面积大，那条带子会被九宫格拉伸成一道横贯面板的亮筋（视觉校核图实测）。
  - ⚠️ id 命名踩过一次契约：`ruif.panelRaised` 驼峰会被 `AssetIds_AreLowercaseDottedAndUnique` 判违规 ⇒ 改 `ruif.panel.raised`（全小写点分层）。
- **绘制接入（`DrawDecor`）**：容器绘制用 9 宫格贴图替代"底色 + 形状"那一步，
  纹样 / 边框 / 霓虹 / 角标 / 动效**仍由装饰层叠加** ⇒ 换贴图后主题身份不丢；
  顺带是性能改善（程序化底色最多 48 行渐层 = 48 次绘制 ⇒ 贴图固定 9 次）。
  - 开关 `RUIFSettingsDef.texturedSurfaces`（**默认 true**，XML 一行可关）；关掉即回到纯程序化绘制。
  - **预览路径刻意不取贴图**：`Panel(rect, palette, decor, …)` 只拿到一份调色板、拿不到主题 id，
    硬用当前主题的图会画出"配色是 A、底纹是 B"的错配画廊 ⇒ 该重载固定走程序化。
  - **fail-safe**：贴图缺失（只拷了 dll / 主题没导出过）时 `ThemeAssets.SurfaceOrNull` 返回 null ⇒ 自动回退程序化，不会画成空白。
- **新公开面**：`RUIF.ThemeAssets`（门面：`AssetIds` / `AssetCount` / `Get(id)` / `Get(themeId,id)` /
  `SurfaceOrNull`（受开关门控）/ `DrawAtlas(rect,id[,tint])`（9 宫格，**返回是否真画了**）/ `PrewarmAllThemes` /
  `ExportAssets` + 12 个稳定 id 常量）与 L0 原语 `Draw.Atlas(rect, tex[, tint])`。用法：
  `if (!ThemeAssets.DrawAtlas(rect, ThemeAssets.Panel)) { /* 自行回退程序化绘制 */ }`。
- **两处内部适配**：`EnsureCanvas` 改为只以 **9 宫格按钮**探画布（其余族自带固定画布，不再依赖原版资源）；
  `ApplyAssets` 跳过无落点资产（不计 miss，避免污染诊断读数）。
- 离线生成器同步支持（解析 `null` 落点 + `Surface` 配方）；校核工具升级为**一次产出两张图**
  （矩阵总览 + **九宫格展开图**，后者按 `Widgets.DrawAtlas` 几何把资产展开成 300×96，含原版按钮对照）。
- 单测 **226** 项（新增 1 项：门面与目录同源 + id 常量存在 + 离线宿主三条 fail-safe 契约）。

**新增（表现面 · 界面贴图资产库 4 → 12 件：全主题 168 张 PNG 算法绘制导出）**
- **覆盖面**：此前只换 4 张按钮底图（普通 / 悬停 / 按下 / 淡色），其余界面元素仍是原版色。扩到 **12 件**：
  - `ui.highlight`（`TexUI.HighlightTex`）—— 悬停高亮，全程序集 **~180 处** `Widgets.DrawHighlight` 都走它；
  - `ui.highlight.selected`（`TexUI.HighlightSelectedTex`）、`ui.highlight.soft`（`Widgets.LightHighlight`）；
  - `ui.title.bg`（`TexUI.TitleBGTex`，窗口标题条）、`ui.row.alt`（`Widgets.AltTexture`，隔行底纹）；
  - `ui.bar.fill`（`Widgets.BarFullTexHor`）与 `ui.bar.bg`（`Widgets.DefaultBarBgTex`）—— `Widgets.FillableBar` 默认档；
  - `ui.slider.handle`（`Widgets.SliderHandle`）—— 形状随主题（圆角 / 切角 + 描边）。
  - **α 逐位对齐原版**（0.10 / 0.18 / 0.05 / 0.04 …），只把"白"换成主题色 ⇒ 视觉权重不变、只换色相。
    唯一例外是 `ui.bar.bg`：原版底槽是**不透明纯黑**（无可对齐的 α），首版照搬 `Palette.Panel` 后
    在**视觉校核图**上暴露出"浅色主题下空槽整条消失"（Panel ≈ 窗口色）⇒ 改为 `Text @ 0.14` 的**半透明覆盖**：
    在任何底色上都成立，且深色主题变亮、浅色主题变暗（方向随主题，不会反）。
  - **收录判据（两条）**：① 只收表面类（底图 / 高亮 / 把手），**形状承载语义的一律不碰**
    （`TexButton` 图标 / 勾选框与单选框的 ✔ ● 符号 / 箭头 / 阴影 / `BaseContent` 纯色基准）；
    ② **目标贴图不得被调用方 `GUI.color` 着色** —— 故 `Widgets.SliderRailAtlas`（先
    `GUI.color = RangeControlTextColor`）与 `TexUI.RectHighlight`（`DrawStrongHighlight`/
    `DrawTextHighlight` 都先设 `GUI.color`）刻意不收：那些槽位已随颜色白名单换成主题色，再重绘会**双重着色**。
- **算法绘制 · 离线导出**：新增 `_scripts/gen_theme_assets.py` —— 把运行时绘制算法搬到离线侧，
  **调色板 / 装饰 / 资产配方三者全部解析自 C# 真源**（不手抄任何数值），按 float32 语义重放算法，
  为 **14 套主题 × 12 件 = 168 张 PNG**（约 33 KB）落盘到 `Textures/ThemeAssets/<themeId>/<assetId>.png` + `manifest.txt`。
  - **一致性已实测**：与游戏内 `ExportAssets` 导出的 56 张按钮资产**逐像素全等**（`--verify`）⇒ 离线算法可信；
    `--check` 可随时校验磁盘资产与 C# 真源是否仍一致（门禁新增 6.2 已接入，防"改了配方忘了重新生成"）。
  - 命名规范 v1 不变：资产 id 小写点分层 · 贴图名 `RUIF.<themeId>.<assetId>` · 缓存键 `<themeId>|<assetId>` ·
    磁盘路径 `<Mod根>/Textures/ThemeAssets/...`；三级来源（内存缓存 → 磁盘 PNG → 算法生成）不变。
- **修复（新增资产才暴露的隐藏 bug）**：`EnsureOriginal` 过去只按**资源路径**重新载入原图，而新增的 7 件资产
  原版都是**运行时纯色贴图**（`SolidColorMaterials.NewSolidColorTexture`，**没有 ContentFinder 路径**）
  ⇒ 原图快照永远取不到、资产被整体跳过（命中数恒为 miss）。现改为**先抓字段当前值、再按路径回退**：
  该方法只在写入前被调用（先抓后写 / 先还原后回收）⇒ 抓到的必然是原版值，不会抓到自己的生成物。
- **ECall 隔离**：绘制整体收进 `[MethodImpl(MethodImplOptions.NoInlining)]` 的 `PaintAsset` ——
  `new Texture2D` / `SetPixels32` / `Object.name` / `hideFlags` 全是引擎接触点，
  非 Unity 宿主 JIT 含 ECall 的方法体会抛 `SecurityException`（前置守卫与 try/catch 都拦不住）。
- 公共 API **零变化**（新增成员全为 private）⇒ 无需走版本一致性重生成链。
- 单测：`VanillaUiSkinTests` 新增 2 项（资产 id 契约逐个钉死 + 命名规范可反解性）⇒ 全量 **225** 项通过。

**新增（配置面 · XML 热重载：改完存盘即生效，无需重启游戏）**
- `RUIFSettings.CheckForFileChanges()`：按文件修改时间轮询（**2 秒**），变化即重读并应用；
  挂在绘制路径（`DrawDecor.Panel`）与设置页上 —— 这是「零 Harmony」下唯一可用的每帧时机。
- `RUIFSettings.ReloadFromFile()`：立刻重读并应用；设置页新增「立即重新加载 XML」按钮与热重载状态行。
- **解析复用引擎**：`DirectXmlToObject.ObjectFromXml<RUIFSettingsDef>(node, doPostLoad: false)` ——
  实证 `ClassTypeOf<T>` 在节点无 `Class` 属性时直接返回 `typeof(T)`，**不校验节点名**，
  故可把文件里的 `<RUIF.RUIFSettingsDef>` 子节点原样交出，得到与启动期**同一套字段映射代码**的结果，无需自写 XML 解析器。
  （注意不能用 `DirectXmlLoader.ItemFromXmlFile<T>`：它取 `DocumentElement`，而本文件根是 `<Defs>`。）
- 失败策略：解析/读取失败 ⇒ **保持当前主题不变** + 告警，绝不把 UI 打到无主题状态。
- ⚠️ **节流刻意用 `DateTime.UtcNow` 而非 `Time.realtimeSinceStartup`**：后者是引擎 ECall，
  在非 Unity 宿主会抛 `SecurityException`，而本方法在绘制路径上**不允许含任何可能抛出的引擎调用**。

**新增（配置面 · XML 默认注册全部主题）**
- `Defs/RUIFSettings.xml` 的 `<themes>` 默认列出 **14 套主题 × 完整 16 色 = 224 条**（vanilla / dark + 12 套风格预设），
  打开即可直接改色；每套 `<basedOn>` 写自身 id（= 以代码内置预设为底再逐项覆盖）。
- **色值由 C# 真源机械生成**（临时脚本一次性产出后删除），并新增单测
  `SettingsDef_ShippedFile_RegistersEveryBuiltInTheme` 逐位比对 XML ↔ `ThemePresets` 内置预设，防止手抄/生成漂移。
- 使用要点写进 XML 注释：**删掉某套的整条 `<li>` 即回落到代码内置预设**（推荐：只想微调就少删、想完全接管才保留全套）。

**修复（关键 · 否则全量清单会毁掉装饰）**
- `RUIFSettings.RegisterTheme`（原 `RegisterXmlTheme`，改为 **public** 以便消费方运行期注册与离线单测）
  以前调用单参 `Theme.Register(id, palette)`，而该重载**会显式写入 `ThemeDecor.Plain`** ——
  一旦 XML 用 `<basedOn>自身 id</basedOn>` 重列内置主题，12 套风格主题的霓虹 / 切角 / 扫描线 / 动效会被**整体抹平**。
  现改为从 `basedOn` 取装饰：`Theme.TryGetDecor(basedOn)`，未命中回退 `Plain`。并加回归护栏单测
  `RegisterTheme_SameIdAsPreset_KeepsPresetDecor`。

**新增（能力面 · 零 Harmony / 零第三方依赖）**
- `RUIF.VanillaUiSkin`：把当前主题的配色注入**原版 UI**（窗口 / 面板 / 列表 / 浮动菜单 / 研究树 / 命令条低光态 / 正文文字色）。
  - **手段**：反射覆写原版 `static readonly` 颜色字段（`Verse.Widgets` 18 · `Verse.TexUI` 18 · `Verse.GenUI` 4 ·
    `Verse.FloatMenuOption` 5 · 其余零散 chrome），共 **64 个槽位**；文字样式走**公开可变对象**
    `Verse.Text.fontStyles / textFieldStyles / textAreaStyles / textAreaReadOnlyStyles`（数组引用只读、元素可变 ⇒ 无需反射）。
  - **触发**：`Defs/RUIFSettings.xml` 新增 `<applyToVanillaUi>`，**默认 `false`** ⇒ 出厂状态下框架对原版零影响。
  - **可逆**：首次写入前快照原值，`Restore()` 完整还原；`SetEnabled(false)` 幂等还原。
  - **容错**：逐字段 `try` + 字段不存在即跳过（不整体失败）；`ColorWritable` 为「写入后回读比对」的**实测探针**，
    结论写进 Player.log 与设置页状态行（离线无法断言 Mono 是否允许改写 `initonly`）。
- `Theme.CurrentPalette`：暴露当前生效的完整调色板（供整表消费；单点取色仍用既有令牌属性）。
- 设置页新增「原版 UI 皮肤」状态行（Keyed 中英各 4 键）：显示命中 `n/64` 与"引擎不支持反射改写"提示。

**新增（表现面 · 按钮底图贴图重着色 —— 实机反馈"部分界面还是原版"后补）**
- **问题**：只改颜色字段时，**建筑菜单左列的分类按钮**与**屏幕底部的主按钮栏**始终是原版浅米色。
  引擎取证：这两处都走 `Widgets.ButtonTextSubtle` / `MainButtonWorker.DoButton` →
  `Widgets.DrawAtlas(rect, Widgets.ButtonSubtleAtlas)` —— **画的是 9 宫格贴图，不是颜色**；
  同样地 `Widgets.ButtonText` → `DrawButtonGraphic` → `ButtonBGAtlas` / `…Mouseover` / `…Click`。
- **解法**：反射覆写 4 个 `static readonly Texture2D`（`Widgets.ButtonBGAtlas` / `ButtonBGAtlasMouseover` /
  `ButtonBGAtlasClick` / `ButtonSubtleAtlas`）。新增配置项 **`<vanillaTextureMode>`**（`off` / `recolor` / `paint`，**默认 `paint`**）：
  - `recolor` —— 按**亮度两点仿射映射**生成同尺寸重着色副本：暗部 → `Palette.Border`、亮部 → `Palette.PanelRaised`，
    **alpha 原样保留**；需读原图像素（`isReadable`），AssetBundle 来源可能不可读而自动跳过。
  - `paint` —— **按主题算法重画**：把整张贴图当作"缩小的按钮"画一个完整的圆角/切角矩形 + 描边
    （角半径 = 角块尺寸 `w/4 × h/4`，恰好让角块内是一段完整圆弧；描边宽度取 `ThemeDecor.EdgeThickness`；
    圆角/切角取 `ThemeDecor.Corner`；顶部高光朝 `CardHover` 偏——该令牌在浅色与深色主题里都比面更亮，方向不会反）。
    **不读原图像素（只用宽高）⇒ 不受 `isReadable` 限制，任何情况下都能生效。**
  - 两者都**严格保持原画布尺寸**：`DrawAtlas` 的角块屏幕尺寸 `a = 贴图宽 × 0.25`（被 `rect/2` 夹取）由引擎计算，改尺寸会改变按钮比例。
- ⚠️ **本次明确不做**（用户裁决：只换贴图资产）：**图标 / 图表等"形状承载语义"的资产一律不碰** ——
  算法画不出同语义图形，改了会让玩家认不出、且与其他 mod 的图标风格冲突。

**新增（架构 · 「界面贴图资产库」：全主题 + 算法生成 + 标准化命名）**
- 需求：*「全主题的界面贴图资产用算法生成并标准化命名」*。原实现是「4 个硬编码槽位 + 只处理当前主题」，
  现升级为**声明式资产目录 + 全主题缓存**：
  - **资产目录**（`VanillaUiSkin.CreateAssets()`，增删一行即增删一件资产）：资产 = `稳定 id` + `原版落点（字段 + 资源路径）` + `绘制变体`。
  - **标准化命名（v1 契约，公开且不得随版本漂移）**：
    | 概念 | 形式 | 例 |
    |---|---|---|
    | 资产 id | 小写点分层，不含翻译文本 | `ui.button.normal` / `ui.button.hover` / `ui.button.pressed` / `ui.button.subtle` |
    | 贴图名 | `RUIF.<themeId>.<assetId>` | `RUIF.dark.ui.button.normal` |
    | 缓存键 | `<themeId>\|<assetId>` | `dark\|ui.button.normal` |
    **themeId 不含点（与 defName 同规则）⇒ 贴图名可反解**：第 2 段是主题、其余是资产 id。
  - **「全主题」**：`PrewarmAllThemes()` 在启用时一次性为**每一套已注册主题**生成整套资产并缓存（成本极低）；
    之后切到任何主题都是**零生成开销**。公开 `GetAsset(themeId, assetId)` 供消费方取任意主题的任意资产。
  - **公开面**：`AssetIds` / `AssetCount` / `CachedAssetCount` / `GetAsset` / `PrewarmAllThemes` / `AssetCacheKey` / `AssetPrefix`。
- **同时收敛风险面（同类安全改造）**：缓存成为资产的**唯一生命周期所有者** ——
  缓存里的贴图**永不单独销毁**，只在 `Restore` 时整体回收（且回收前逐张复核"是否仍被字段引用"）。
  这从结构上消除了上一条事故的成因。
- ⚠️ **取消 `recolor` 模式**（`vanillaTextureMode` 现为 `off` / `paint`）：该路径要读原图像素
  （受 `Texture2D.isReadable` 制约），且会引入**第二种生成物生命周期** —— 刚经历一次"生成物被回收 ⇒ UI 全崩"，
  不值得为它扩大风险面。想要原版外观就用 `off`。
- 单测 **+7（211 → 218）**：新增 `VanillaUiSkinTests` 锁定命名契约（id 小写点分且唯一、缓存键格式、名前缀可反解）
  与**离线不抛**契约（未知主题/资产/空入参 → null；离线宿主预热 → 0）。

**变更（架构 · 资产来源改为「磁盘优先，算法生成为回退」）**
- 需求：*"用算法把全部主题的贴图资产绘制到本地，然后对代码层进行修复"*。
  `GetAsset` 现为**三级**：**内存缓存 → 磁盘 PNG → 算法生成**。
  - **磁盘优先**：`LoadFromDisk` 读 `<Mod 根>/Textures/ThemeAssets/<themeId>/<assetId>.png`
    （`File.ReadAllBytes` + `LoadImage`），命中即用 —— 运行时不再逐像素绘制，且资产**可被人工编辑/替换**。
  - **回退生成**：磁盘缺失时仍走 `PaintButtonTexture` ⇒ 框架"没导出过"也开箱即用。
  - **为什么不走 `ContentFinder`**：它只在游戏启动时索引贴图 ⇒ 导出后必须重启才可见；
    直接读文件让「导出 → 立即生效」闭环（本项目已吃过一次"改了看起来没生效"的亏）。
  - **路径唯一真源**：`ExportAssets` 与 `AssetFilePath` 共用同一个 `ExportFolder` 常量 ⇒
    "导出的路径"与"读取的路径"结构上不可能分叉；扩展名同理（`AssetFileExtension`）。
  - 新增诊断：`DiskAssetCount` / `PaintedAssetCount`，日志出现 `src=diskN/paintN`。
- ⚠️ **操作流程两步（导出物落在发布区，必须回迁）**：游戏内 `GetMod<RUIFMod>().Content.RootDir`
  指向**发布区** ⇒ 导出写进 `Mods\RUIF\Textures\ThemeAssets\`；而 `sync_ruif.ps1` 的镜像清理规则是
  "发布区不得有开发区没有的文件"，门禁第 6 道同向 ⇒ **必须先把导出的 PNG 回迁到开发区
  `RUIF/Textures/ThemeAssets/`，再跑 `sync_ruif.ps1`**，否则会被当残留删除且门禁 FAIL。
- 单测 **+2（221 → 223）**：`AssetFileExtension` 契约、`AssetFilePath` 离线/空入参不抛。

**新增（交付面 · 主题贴图资产落盘导出）**
- `VanillaUiSkin.ExportAssets(rootDir = null)`：把**全部已注册主题**的整套界面贴图资产导出成 PNG 到
  **`<Mod 根>/Textures/ThemeAssets/<themeId>/<assetId>.png`**，并写 `manifest.txt`（`themeId → assetId → 尺寸` 对照表）。
  设置页新增「导出主题贴图资产」按钮 + 结果行（Keyed 中英各 4 键）。
- **命名规范无损映射**：内存规范名 `RUIF.<themeId>.<assetId>` ↔ 磁盘路径 `<themeId>/<assetId>.png`。
- **落点选择有据**：`Textures/` 已在 `sync_ruif.ps1` 的同步白名单里，且门禁第 6 道的方向是
  **「发布区 ⊆ 开发区」（只查残留）** ⇒ 导出物会随包一致同步，不会让门禁 FAIL。
- **显式动作，绝不自动导出**：RUIF 的前提是"零磁盘资产"，落盘只为 **审阅 / 二次编辑 / 存档**。
  导出物会随同步进入发布包（几十 KB 量级）——这是"框架不再纯零资产"的一处**受控例外**。
- 新增引擎模块引用 `UnityEngine.ImageConversionModule`（`Texture2D.EncodeToPNG` 是它的扩展方法；
  纯引擎模块，非第三方依赖，门禁第 2 道仍 PASS）。
- 单测 **+3（218 → 221）**：导出目录契约、离线宿主 0 张不抛、非法/不存在根目录不抛。

**修复（通用铁律 · 非 Unity 宿主 jit 含 ECall 的方法体即抛 SecurityException）**
- 现象：新增 `ExportAssets` 后，**两个离线单测在"根本走不到编码"的路径上也抛**
  `SecurityException: ECall 方法必须打包到系统模块中`。
- 结论（经两轮实证，**推翻"ECall 只在执行时抛"的直觉**）：
  1. 把 `tex.EncodeToPNG()`（ImageConversion 的 ECall）直接写进 `ExportAssets` 的方法体 ⇒ 该方法在
     非 Unity 宿主 JIT 时即抛；**前置守卫与 try/catch 都拦不住**（连提前 `return` 的路径也抛）。
  2. 隔离成独立方法后**仍然抛** —— 因为它只有一条语句、是典型内联候选，JIT 把它内联回调用方，
     ECall 又回到了调用方体内 ⇒ **`[MethodImpl(MethodImplOptions.NoInlining)]` 是必需品**。
  3. 同理 `tex.width` / `tex.height`（`Texture` 的 ECall 属性）也不得出现在公共方法体里 ——
     已改用已就绪的纯数据 `canvasW/canvasH`。
- 另修：`Path.Combine` 遇非法路径字符会抛 `ArgumentException` ⇒ 导出入口整体容错，
  兑现"绝不抛异常（可挂按钮回调）"的契约。

**修复（严重 · 运行时生成的贴图被引擎回收 ⇒ 整个 UI 崩）**
- **现象**：实机"疯狂报错" —— Player.log 里
  `GUI Error: You are pushing more GUIClips than you are popping` ×58、
  `Exception filling window for RimWorld.Dialog_Options / Dialog_ModSettings : NullReferenceException`、
  `NullReferenceException at UnityEngine.Texture.get_width()` ← `Verse.Widgets.DrawAtlas` ← `Widgets.DrawButtonGraphic`。
- **根因（时序取证，Player.log 相邻行）**：
  ```
  [RUIF] settings applied: ... tex=4/4(Paint) ...
  Unloading 97 unused Assets to reduce memory usage. Loaded Objects now: 31313.   ← Resources.UnloadUnusedAssets()
  Exception filling window ... NullReferenceException at Texture.get_width()
  ```
  **`new Texture2D(...)` 运行时创建的贴图不在任何 AssetBundle / 场景 / Resources 里**，
  RimWorld 调用 `Resources.UnloadUnusedAssets()` 时把它们当"未使用资源"回收；
  而原版静态字段仍指向这些已失效对象 ⇒ `DrawAtlas` 读 `.width` 直接 NRE ⇒ GUIClip 栈被拉开 ⇒ 全线崩。
- **修复**：① 生成物一律 `hideFlags = HideFlags.DontSave`（Unity 官方要求：运行时资源必须标记不可保存，
  否则会被 `UnloadUnusedAssets` 回收）；② 回收改为**只销毁"确认已不在任何目标字段里"的生成物**
  （`ReleaseGenerated()` 用 `ReferenceEquals` 逐个比对字段当前值），彻底消除误销毁路径；
  ③ `Texture2D` 槽位记录**资源路径**，原图引用万一失效时用 `ContentFinder.Get(path)` 重新载入兜底；
  ④ 新增诊断计数 `LostTextureCount`（正常为 0），出现在日志 `lost=` 里。
- **刻意不收**：`Verse.TexButton.*`（84 张图标，改了会变色块）、`BaseContent.WhiteTex/BlackTex`
  （被 `GUI.color` 当纯色基准，改了污染全局）、`Widgets.ShadowAtlas`、`TexUI` 遮罩与箭头。
- 可逆：保留原始贴图引用，`Restore()` 写回；换主题时 `Destroy` 上一张生成物避免泄漏；
  逐字段 `try` + `isReadable` / 像素数上限保护（不可读或超限即跳过，不整体失败）。

**⭐ 能力边界（写进代码注释与 XML 注释，避免被当成缺陷）**
1. **只能改「颜色」与「按钮底图贴图」，不能改绘制行为** —— `ThemeDecor` 的渐层 / 霓虹 / 切角 / 扫描线 / 动效属绘制行为，对原版 UI **无效**；
   原版 UI 只会变成「主题配色的原版造型」。图标贴图**也不改**（改了会变纯色块）。若要做到装饰一致，需要 Harmony 补丁（本框架红线外）。
2. **语义色刻意不动** —— `Verse.ColorLibrary` 的 83 个命名色、`ColoredText` 语义文本色、健康 / 阵营 / 危险状态色
   一律不改，避免「红 = 危险」这类信息失真。

**风险登记（受控例外）**
- 这是本框架唯一的**写入引擎全局状态**行为 ⇒ 默认关闭 + 可逆 + 逐字段容错 + 实机探针，四项同时成立才允许开启。
- **时序红线**：绝不在 `[StaticConstructorOnStartup]` 阶段触碰 `Verse.Text`（其静态构造器读 `GUI.skin`，
  提前触发可能初始化失败并外溢成 `TypeInitializationException`）。文字样式一律经 `VanillaUiSkin.PumpTextStyles()`
  在绘制期应用（调用点在 `DrawDecor.Panel` 与设置页）。

**质量面**
- 编译 **0 错 0 警** · 单测 **211/211**（+8）· 静态体检八组（HIT 仍为既有 2 项）· 发布门禁**十道全 PASS** ·
  公共 API **91 类型 / 593 成员**（原 90 / 570）。程序集引用面新增 `System.Xml`（BCL，非第三方 Mod 依赖）。
- **NOT RUN（如实）**：实机效果与 `ColorWritable` 探针 —— 需重启游戏后：
  ① 查 `Player.log` 的 `[RUIF] settings applied: ... vanillaUi enabled=True injected=? writable=?`；
  ② 改一行 `<themes>` 里的颜色存盘，确认约 2 秒后界面自动变色（热重载）；
  ③ 目视原版窗口 / 面板是否换色（`applyToVanillaUi=true` 时）。

### ④ 发布 Gate 结果与回滚方案（REL-007 / REL-009）

**Gate 结果（`verify_release.ps1` 实跑 · 9 道全 PASS · 退出码 0）**

| 道次 | 检查 | 结果 |
|---|---|---|
| 1 | 版本一致性 | `dll Api.VersionString = 1.0.1` ↔ `About modVersion v1.0.1` ✔ |
| 2 | 引用白名单 | 6 项（`mscorlib` / `System.Core` / `Assembly-CSharp` / `UnityEngine.{CoreModule,IMGUIModule,TextRenderingModule}`）· **零 `0Harmony`** |
| 3 | 两区哈希 | `RUIF.dll` MD5 `0473559FDEE597328AED4636DFF2B112` 开发区 = 发布区 |
| 4 | 键集合对称 | 中英各 **60** 键，双向零差异 |
| 5 | 翻译键覆盖 | 源码 60 键全覆盖 · **死键 0** |
| 6 | 发布区卫生 | 无 `*.bak_*` / 无残留 · 发布区 **19 文件** · 镜像一致 |
| 7 | 开发者包 | **6 项**必备文件齐备（含《零基础上手教程》） · `BUILD-INFO = 1.0.1` · 发布区 DevDocs **13 文件** |
| 8 | XML 良构 | 3 文件良构 · `packageId` / `modVersion` 齐备 |
| 9 | 占位符 / 重复键 | 60 键占位符一致 · 无重复键 |

**其它证据**：编译 **0 错 0 警**（`Source/RUIF.csproj`）· 单测 **171/171**（net48 NUnit）· `health_check.ps1` **命中 0 组**（含「文档引用的 `RUIF.*` API 全部存在」「不含已移除特性的残留引用」）· **文档示例真编译 0 错 0 警**（`_verify/consumer_docs/verify_samples.ps1`，对发布区 dll）· `DevDocs` **13 文件 / 665.6 KB**。

**回滚方案（REL-007）**
- **上一稳定版本**：v1.0.0（2026-09-11）。
- **本版改动面**：仅 `Api.VersionString` 常量 + 文档 / 交付物；**绘制与组件代码零改动** ⇒ 行为等价。
- **回滚触发条件**：消费方在 v1.0.1 下出现组件绘制异常（本版未触碰绘制路径，概率极低）。
- **是否影响 Save**：**否**（框架无存档字段、无 Def、不注册 UI 入口、不写文件）。
- **如何禁用可选 Compatibility**：**N/A**（无兼容补丁、无 Harmony 依赖）。
- **已知问题与规避**：见 `docs/健康评估与发布记录.md` §11.4（均为既有登记项，非本版引入）。

**随包文档版本标识纪律（本版建立）**：随包件里「**当前版本 / 本包版本**」的表述统一为 **v1.0.1**（开发者入口 §八、接入指南 §一、宝典抬头/页脚、`API参考.md` 自动生成、封面 `Preview.png`）；
凡「**自 v1.0.0 起 / 已于 v1.0.0 移除 / v1.0.0 反射实测**」等**历史锚点一律保留原样**——改写历史锚点等同于伪造变更史。
另：封面与文档的公开成员数统一为 **435**（`gen_api_reference.ps1` 口径，含枚举成员；体检脚本的 381 为不含枚举成员口径，两者并存但对外一律用 435）。

---

## v1.0.0 · 首次公开发布 — 2026-09-11

**版本策略**
- 内部迭代线（v2.0.0 – v2.2.1）作为**首次公开版本**发布，对外统一为 **v1.0.0**：
  首个稳定公开版。`RUIFMod.VersionString` / `About.xml` / 示例 `About.xml` 三处同步为 `1.0.0`（门禁第 1 道强制一致）。
- 下方 v2.0.0 – v2.2.1 条目保留为**内部开发历史**（API 面与功能即 v1.0.0 的内容）。

**首次发布包含**
- **能力面**：内核（Draw 零分配作用域 / Layout 版式游标 / Theme 令牌真源）+ **153 件组件**（原子 42 · 数据 40 · 输入 28 · 反馈 17 · 导航 12 · 图表 10 · 工业 4）+ 装饰层 6 维 + **12 套主题**（程序化绘制，零贴图 / 零 shader / 零资产依赖）。
- **接入面**：硬依赖（`modDependencies` + `Api.IsAvailable` 版本门控）与软依赖（反射探测 + 隔离桥，框架缺失仍可加载降级）两种模式；示例源码随包分发（`Sample/`，非独立 Mod），可编译副本见 `DevDocs/_Template/`。
- **交付面**：`DevDocs/` 离线开发者包（API 参考 / 接入指南 / 组件图鉴 / 终极宝典 / 体检报告）/ 中英双语 Keyed / XML 配置入口 / `About/Preview.png` 程序化生成封面。
- **质量面**：编译 0 错 0 警 · 单测 **203/203** · 静态体检八组（2 项 HIT 均为已定性）· 发布门禁**十道全 PASS** · 实机启动验证通过（`[RUIF] v1.0.0 loaded`）。

**发布前调整（同日）**
- 演示工程由独立 Mod 收编进框架包（消除 Mod 列表中的第二项）；新增 `About/Preview.png`；门禁第 6 道新增两条架构断言固化该结构。

---

## v2.2.1 · 发布前第三次全面 review 修复：设置页翻译走缓存 + 门禁口径补全 + 装饰上限护栏 — 2026-09-10

**修复（性能 · PERF-001 自洽性缺口）**
- 设置页与主题画廊的 **15 处翻译调用**原本直接走 `".Translate()"`，未经过 v2.1.1 为绘制路径建立的有界缓存
  （`TrCache`）。v2.2.0 加入 12 张画廊卡片后每帧调用进一步上升 —— 属"框架自身不守自家纪律"的缺口。
  现全部统一经 `Tr`（新增三参重载，保持"所有翻译经 `Tr`"的可扫描性）；设置页每帧翻译查找约 **25 次 → 0 次**（缓存命中）。

**修复（发布链路 · 门禁覆盖缺口）**
- 由本次重构暴露：设置页键改为 `Tr.Get("…")` 后，第 5 道扫描命名空间 `RUIF.(UI|Comp|Theme).` 不再匹配，
  源码键覆盖数从 **74 掉到 60**（等于设置页 14 个键失去"源码是否引用"校验）。已补 `Settings` 命名空间并复验恢复。

**新增（性能防退化护栏）**
- `health_check.ps1` **第 8 组「装饰绘制上限预算」**：`DrawDecor` 的 6 个上限常量（渐层 48 / 形状 56 / 扫描线 64 /
  网格 44 / 点阵 220 / 斜纹 40）就是"每帧绘制预算"，被调大现在会在体检里显形。

**全面 review 结论（本次范围：性能 · 健康 · 技术债务 · API · 分层 · 本地化 · 发布链路 · 文档）**
- 性能：装饰成本模型 —— 最花哨单面板 ≈**144 quads/帧**；设置页 12 张画廊卡 ≈**720 quads/帧**（可接受，仅设置页打开时）；
  绘制路径分配 0；容器分配 7 处全在点击分支/惰性初始化。
- 健康：死 API 0 / 红线 0 / 中文硬编码 0 / XML 注释覆盖 71.8% / 最大文件 559 行（ARC-013 阈值 800）。
- 技术债登记 6 项（TD-1 注释覆盖为最高，其余为已登记取舍或低风险观察项）。
- 报告：`docs/体检报告_v2.2.1.md`。

**变更（发布结构 · 演示收编进框架包）— 2026-09-11**
- 演示工程**不再作为独立 Mod 分发**：原 `Mods\RUIF.Sample`（会在游戏 Mod 列表出现一项）已移除；
  示例源码随框架包分发到 `RUIF/Sample/`（包内**二级**目录 ⇒ 游戏只扫 `Mods\` 一级，故不加载、不列表）；
  可编译副本（含 `About.xml` 与 `csproj`）仍在 `DevDocs/_Template/`。
- 门禁（`verify_release.ps1` 第 6 道）新增两条架构断言：① 不得存在独立 Mod `Mods\RUIF.Sample`；
  ② `RUIF/Sample` 不得出现 dll / `About.xml`（否则会被识别成可加载 Mod）。`sync_ruif.ps1` 负责迁移删除旧目录并做同样安全闸。
- 发布侧统一排除 `obj`/`bin`（IDE 语言服务会为模板 `csproj` 自动生成缓存）；门禁的"模板无缓存"
  检查改为校验**发布区**那份，开发区缓存只给 WARN（属本机噪声，不进包）。
- 脚本健壮性：同步/门禁改用 .NET 删除 API（不依赖回收站）并在删除前清只读 ——
  `docs/终极宝典` 带只读保护属性，其副本会继承，导致后续递归清理被拒（Access denied）。
- **新增发布素材 `About/Preview.png`（1376×768）**：由 `_scripts/gen_preview.py` **程序化生成**（Python + Pillow），
  配色取 12 套主题真值、装饰参数与 `CyberpunkDecor` 对齐（扫描线 7px/α0.10 / Neon 边 / 切角 / 四角标记）；
  封面内容本身就是框架能力的自我演示（12 张主题卡 + 一张 UI 示意：标题栏 · 卡片列 · 折线图 · 进度条 · 按钮 · 语义状态点）。
  零贴图、可复跑（4× 超采样抗锯齿，`--ss` 可调）。

**Gate 结果摘要**
- 编译 0 错 0 警；单测 **203/203**；公共 API **90 类型 / 570 成员**（**无 API 变更**，`Tr` 为 internal）
- `verify_release.ps1` **十道全 PASS**（源码键覆盖恢复 74）；`health_check.ps1` **八组**（2 项 HIT 均为已定性可接受项）
- Roslynator **296 条全 info（0 warning / 0 error）**；两区哈希 `1CDE51CD49B11013EF4128B1E83D3C93` / `91B443A4D73EF180E84C062E4BF73219`
- **NOT RUN**：实机启动、语言切换失效验证、12 套观感与动效频率、装饰成本实机 Profile

---

## v2.2.0 · 装饰层 + 12 套热门风格主题 + 设置页主题画廊 — 2026-09-10

**新增（公共 API，无破坏性变更）**
- **装饰层**：主题从"16 色调色板"扩展为"调色板 + **装饰配方**"，全部**程序化绘制**（零贴图 / 零 shader / 零资产依赖）。
  - `ThemeDecor`（6 维度配方）：底色 `SurfaceStyle`（纯色 / 上亮渐层 / 下亮渐层）、边框 `EdgeStyle`（无 / 极细 / 单线 / 双线 / 虚线 / **霓虹发光**）、
    角形 `CornerStyle`（直角 / 切角 / 圆角 / 裁角）、纹样 `OverlayStyle`（无 / 扫描线 / 黄黑斜纹 / 网格 / 点阵）、
    动效 `MotionStyle`（无 / 呼吸 / 流光 / 掠光 / 闪烁）、角标 `TickStyle`（无 / 四角标记 / 左右导轨 / 方括号），
    外加线宽 / 切角 / 纹样密度 / 动效速度 / 发光强度等标量；`Sanitize` 把任意来源的越界值夹回合法区间（坏配方只会"变朴素"）。
  - `DrawDecor`：组合器 `Panel` / `PanelRaised` / `Card` / `Bar`（**一句 `DrawDecor.Panel(rect)` 即跟随当前主题**）+ 单元原语
    `Surface` / `Edge` / `Overlay` / `Ticks` / `Glow` / `Flow` / `Sweep`。
  - `ThemeMotion`：无状态动效相位**纯函数**（`Phase` / `Pulse` / `Flicker` / `GradientT` / `PerimeterT`）——
    组件不保存相位，时间由调用方传入（传 0 即冻结），全部可离线单测。
- **12 套热门风格预设**（`ThemePresets`）：赛博朋克霓虹 / 霓虹黑色电影 / 极光 / 蒸汽波 / 复古终端 CRT / 军用 HUD /
  工业警示 / 血月 / 鎏金奢华 / 樱花 / 羊皮纸 / 北欧极简 —— 每套完整 16 色 + 各自的招牌装饰。
- `Theme` 新增 `Register(id, palette, decor)` / `TryGetDecor(id)` / `ActiveDecor`；`Apply` 现在**颜色与装饰一起切换**；
  旧 `Register(id, palette)` 保持可用，装饰自动退化为 `Plain`（**对既有主题零视觉影响**）。

**变更（设置页 = 主题画廊）**
- 每套预设一张卡片，卡片**用它自己的调色板与装饰配方绘制**（预览不必先切换主题），含关键令牌色卡条、当前主题标记、点选即换肤；
  卡片动效相位取实时时钟 —— 在设置页里就能看到流光 / 呼吸 / 扫描线。第三方注册的主题仍以文字按钮列出（回退显示 id）。

**变更（示例范本）**
- `RUIF.Sample` 演示装饰层：窗口底一句 `DrawDecor.Panel(inRect)`，卡片用 `DrawDecor.Card(...)`；新增「下一套主题」按钮，一轮看完 12 套风格。

**文档**
- `docs/终极宝典v1.0-fixed.html`（随包分发的实例大全）新增 **「装饰层与热门风格主题（v2.2.0）」** 章节：
  6 维度装饰对照表 + **12 套风格的色卡预览卡**（纯 CSS 实现，不触碰宝典自身的 canvas 主题色表，零回归风险）
  + 接入 / 切换 / 预览 / 继承四段用法；侧栏导航、顶栏与页脚版本标注同步到 v2.2.0。
- `_build_crown.py` 的版本串同步更新，并补写「交付物是 `-fixed` 版、重跑构建后需重新套用修复」的防分叉说明。

**性能纪律（全部有界，PERF-005）**
- 纹样：扫描线 ≤64 条、网格 ≤44/轴、点阵 ≤220 个、斜纹 ≤40 条；渐层 ≤48 行、形状遮罩 ≤56 行（按高度自适应步长）—— 面板拉得再大也不会画出上千条线。
- 绘制路径**零堆分配**（全 struct 参数，循环内不 new）；动效不引入状态字段。

**Gate 结果摘要**
- 编译 0 错 0 警（框架 + 示例 + 测试三工程）；单测 **203/203**（新增 20 例：装饰契约 / 预设数据完整性 / 动效纯函数）；
- 公共 API **90 类型 / 570 成员**（+11 / +92），**无破坏性变更**（旧签名全部保留）；
- `verify_release.ps1` **十道全 PASS**；`health_check.ps1` 中文硬编码 0 / 红线 0；
- **NOT RUN**：实机观感验收（12 套风格在真实 DPI 与字体下的可读性、动效频率体感、切角卡片内长文案余量）。

---

## v2.1.1 · 发布前第二次体检修复：翻译缓存 + 设置页本地化/去分配 + 门禁扩到十道 — 2026-09-10

**修复（性能 · PERF-001 / PERF-002）**
- **绘制路径翻译接有界缓存**：新增 `Source/Core/TrCache.cs`，`Source/Core/Tr.cs` 接线。此前 **62 处**绘制路径翻译调用点
  （全部位于 `Draw.Label(...)` / `Widgets.ButtonText(...)` 的实参位置）**每帧执行** —— 无参者每帧 1 次引擎字典查找，
  带参者（21 处）每帧 1 次 `Formatted` **字符串分配**（ilspycmd 实证：`Translate(key, arg)` = `key.Translate().Formatted(arg)`）。
  现在命中即 **0 查找 0 分配**；语言切换（`LanguageDatabase.activeLanguage` 引用变化）自动整体失效；容量 512 条满则清空重建。
  三要素与"为什么不缓存 float/string 参数"的可辩护取舍写在 `TrCache` 注释里。
- **新增 `Tr.Get(string, int)` 重载**：重载决议中精确匹配优先于 `NamedArgument` 隐式转换 → 原本传 `int` 的调用点
  **一行都不用改**即走缓存（8 处）。
- **设置页去每帧分配**：`RUIFSettings.SourceSummary()`（每帧字符串拼接）与 `RUIFMod.DevDocsPath()`
  （每帧 `LoadedModManager.GetMod` 查找 + `Path.Combine`）改为惰性缓存；**失败结果同样缓存**，避免每帧重复写警告。

**修复（本地化 · GOV-010 / LOC-002）**
- `RUIFMod.SettingsCategory()` 由硬编码 `"RUI Framework"` 改为 `"RUIF.Settings.Category".Translate()`。
- 设置页「XML 配置」行不再直接显示 `SourceSummary()` 的英文开发者诊断串，改走 Keyed 参数化
  `RUIF.Settings.Source.Def`（{0}=DefName、{1}=themeId、{2}=主题数）/ `.None`；中英各 +3 键（8 → 11，合计 71）。
- 诊断文本本身（告警、`ValidNames`、`SourceSummary`）登记为 **EXC-003**（开发者向，边界与关闭条件见健康评估 §3）。

**变更（门禁 · 九道 → 十道）**
- 第 5 道扫描前**剔除注释**：此前 `Tr.cs` 文档注释里的示例 `Tr.Get("K", 3)` 会被当作真实键 → 假"缺翻译" FAIL。
- **新增第 10 道**：同键中英**占位符集合一致性**（LOC-004，少一个 `{1}` 会静默吞参）+ **文件内重复键**
  （LOC-005，XML 重复键不报错、后者静默覆盖前者）。

**变更（测试）**
- 新增 `Tests/TrCacheTests.cs`（9 用例：命中返回同一实例 / int 键命中 / (键,值) 隔离 / Ordinal 键 / 语言戳失效 /
  容量有界 / 不缓存 null / 构造校验 / 语言戳委托缺失容错）。
- 新增 `Tests/SettingsDataTests.cs`（4 用例）：三个公开设置数据类的默认值与契约 —— 静态体检"零测试引用的公开类型"由 9 → 6。

**Gate 结果摘要**
- 编译 0 错 0 警（框架 + 示例 + 测试）；单测 **186/186**；公共 API **79 类型 / 478 成员**（**无 API 变更**，`TrCache`/`Tr` 均为 internal）。
- `verify_release.ps1` **十道全 PASS**（71 键对称、占位符一致、两区哈希一致、`BUILD-INFO` 2.1.1）。
- `health_check.ps1`：中文硬编码 **0**；文档↔实现一致（无已移除特性残留）；零测试引用公开类型 9 → 6。
- **NOT RUN**：实机验收（`[RUIF] v2.1.1 loaded.`、切换语言后文案即时跟随、设置页显示）—— 详见 `docs/体检报告_v2.1.1.md` §7.3。

---

## v2.1.0 · 组件层 Keyed 化（EXC-002 关闭）+ 语义/文案解耦 — 2026-09-10

**修复（EXC-002 关闭）**
- **组件层 67 处硬编码中文全部改为 Keyed**（15 个文件）—— 新增 `Languages/{ChineseSimplified,English}/Keyed/RUIF_Components.xml`
  （命名空间 **`RUIF.Comp.*`，60 键**）。此前任何语言环境下调用这些组件都会看到中文（如 `DensitySwitch` 的「紧凑/标准/舒适」、
  `ConfirmDialog` 的「确认/取消」、`ZoomIndicator` 的「缩放/适配」）。
- 取用点统一为框架内部 **`Source/Core/Tr.cs`**（`Tr.Get(key)` / `Tr.Get(key, arg)`）：
  ① 集中做 `TaggedString → string` 转换（否则 `??` 会因两种类型混用编译不过）；
  ② 键名集中出现，便于门禁扫描与人工审查。
- **带参翻译走正确的 1.6 API**：`Translate(this string, params object[])** 已标 `[Obsolete]`**（CS0618），
  改走 `Translate(NamedArgument)`（`NamedArgument` 具备 int/float/string/… 隐式转换，占位符按位置取 `{0}`/`{1}`）。
  实测：编译 **0 警**，未出现 CS0618。
- 参数化默认值不再写死文案：`ConfirmDialog` / `BatchConfirm` 的 `cancelLabel`/`okLabel` 默认值由 `"取消"`/`"确认"` 改为 `null`，
  方法内回退到 Keyed（源码级兼容，调用方无感）。
- **新增公共 API（纯语义，供消费者自行渲染 + 保住离线可测性）**：
  `OfficialFmt.RelationBandOf(float)` + `enum RelationBand`（五档）、`NavFmt.TimeBucketOf(float)` + `enum TimeBucket`（四档）。
  文案函数 `RelationLabel` / `EtaLabel` / `RelativeTime` 签名不变，内部改走 Keyed。

**变更（测试口径）**
- 单测不能触碰 `Translate`（离线无语言数据 → 会抛异常），因此三处断言文案的用例改为断言**语义**：
  `RelativeTime_Buckets` → `TimeBucketOf_Boundaries`、`RelationLabel_FiveBands` → `RelationBandOf_FiveBands`（并补边界值：75 / 74.9 / 59.9s / 60.1s），
  `EtaLabel_Formats` 删除（其唯一逻辑是单行分支，文案交给门禁）。
- 门禁第 5 道的键命名空间扫描扩展为 **`RUIF.(UI|Comp).`**（否则新键会被漏检）。
- `health_check.ps1`：文档覆盖率分母不再计入枚举成员（`public static literal` 不属"需要注释的 API 面"）。

**Gate 结果摘要**
- 编译 0 错 0 警（框架 + 示例 + 测试三工程）；单测 **173/173**；公共 API **79 类型**。
- `health_check.ps1` 第 4 组 **「框架源码无中文硬编码文案」**（67 → 0）—— EXC-002 关闭的验收标准。
- 九道门禁全 PASS（键集合对称 → 各 **68** 键；键覆盖扫描含 `RUIF.Comp.*`）。
- 发布区 DevDocs 更新；**NOT RUN**：实机验证（启用后组件文案随语言变化）。

---

## v2.0.0 · 框架化收敛：移除演示区 + XML 设置 — 2026-09-10

**移除（公共 API 删除 → 主版本）**
- **组件演示区（Gallery）整体移出编译集**：`Source/DevTools/` → `RUIF/_archive/DevTools_Gallery/`
  （源码保留、不编译、不进发布包；恢复步骤见 `_archive/README.md`），csproj 去掉 `DevTools\**\*.cs`。
  删除的公共类型：`RUIF.DevTools.Dialog_Gallery` / `MainTabWindow_Gallery` / `GalleryContent` / `GalleryPages`。
- **底栏 RUIF 按钮删除**：`Defs/MainButtons/RUIF_Gallery.xml` 及其 DefInjected 一并移除 —— 框架不再注册任何 UI 入口。
- 连带清理：`RUIF.UI.Cat.01-10` / `RUIF.UI.Preset.01-12` 共 **22 个死键**（由门禁死键审计发现）从中英语言文件删除。
- **EXC-001 关闭**：随包不再有任何硬编码界面文案（Gallery 是其唯一来源）。

**新增 · XML 设置（只暴露真正被支持的内容）**
- `Defs/RUIFSettings.xml`（defName 固定 `RUIF_Settings`）+ `Source/Config/`：
  `RUIFSettingsDef`（themeId / logOnStartup / themes）· `RUIFThemeXml` · `RUIFColorXml` · `RUIFSettings`（应用器）。
- 生效时机：`[StaticConstructorOnStartup]`（**Def 加载完成后**执行一次）—— 刻意不用 Mod 构造器（那时 `DefDatabase` 还是空的）。
- 失败策略：永不抛异常、永不阻断加载；未知主题 id / 未知颜色名 / 疑似混用颜色写法 → 夹回合法值 + 告警（设置页可见）。
- 合法颜色名严格对应 `Palette` 的 16 个字段；写错名字只告警不崩，未列出的颜色继承 `basedOn`。
- 新增公共 API：`Theme.TryGet(string, out Palette)`（供 `basedOn` 复用已注册主题）与 `RUIFSettings.*`。
- 设置页：版本 / 当前主题 / XML 配置来源与告警 / 打开离线开发者文档 / 在已注册主题间切换（注明重启后以 XML 为准）。

**新增 · 绘制范本（替代 Gallery 的教育作用）**
- `Sample/Source/Dialog_SampleRuifWindow.cs`：用 RUIF 组件画一个最小窗口
  （布局游标 → 令牌取色 → `PanelAtoms` / `ProgressAtoms` / `StatusAtoms` → 交互动画参数），
  作为**唯一可运行的"怎么画"范本**；示例设置页按钮由"打开 Gallery"改为"打开示例窗口"。

**修复（发布链路漏洞，均被门禁或实跑抓到）**
- `sync_ruif.ps1` 新增**镜像清理**：发布区里"开发区已删除"的文件会被删除 —— 否则删掉的 Def / 语言文件仍被游戏加载，
  表现为"删了却还在"（实测清掉 3 个残留：`Defs/MainButtons/RUIF_Gallery.xml` + 2 个 DefInjected）。
- `verify_release.ps1` 新增**第 8 道「XML 良构」**（About / Defs / Languages / Patches 全量 + About 必备字段）：
  本轮它抓到并修复了 About.xml 里一个未转义的 `&`（会让整个 Mod 元数据解析失败、Mod 显示异常）。
- `verify_release.ps1` 第 6 道扩展**镜像一致性**断言；第 7 道模板比对改为**动态枚举**（Sample 新增文件自动纳入）。

**变更**
- 版本 `v1.2.1 → v2.0.0`（`RUIFMod.VersionString` / About.xml / Sample About 同步）。
- DevDocs 白名单加入 **《终极宝典v1.0-fixed.html》** → 开发者包 **17 文件 ≈702 KB**。
- `docs/开发者入口.html` 按"纯框架 + XML 设置"重写（去掉 F11/Gallery，新增 §四 XML 设置与颜色写法规则）。

**修复（体检发现 · 同日）**　详见 `docs/体检报告_v2.0.0.md`
- **三处"alpha 承诺了却没生效"**（`FeedbackAtoms.Toast` / `FeedbackAtomsB.ActionFeedback` / `FeedbackAtomsB.UndoToast`）：
  前两者**完全没用 alpha 参数**（注释承诺淡出、实现固定不透明）；`UndoToast` 用 `GUI.color` 做整体淡出，
  但 `Widgets.DrawBoxSolid` 内部「存→设色→画→还原」会覆盖外层 tint、`Draw.Label` 自带 Color 作用域同样覆盖
  （ilspycmd 实证）→ **淡出从未生效**。统一改为把 alpha 乘进颜色（不再触碰全局绘制状态）。
- **全局绘制状态泄漏风险**：`UndoToast` 手写 `GUI.color = … / = prev` 且无 try/finally —— 异常时染色会泄漏到全游戏 UI。随上一条一并消除（框架内现仅 `Draw` 作用域写 `GUI.color`，可脚本断言）。
- **XML 设置应用器加固**：`ApplyFromDefs` 整体 try/catch + 静态构造路径日志容错 —— 承诺"永不阻断加载"必须在结构上成立（`[StaticConstructorOnStartup]` 抛出会导致该静态类型初始化失败并外溢）。实测触发情形：非 Unity 宿主里 `Verse.Log` 内部 ECall 抛 `SecurityException`。
- 删除死代码 `InputExAtoms.PriorityMax`（未使用私有常量）。
- 新增 `_scripts/health_check.ps1`（七组静态体检：注释覆盖 / 死 API / 文档↔实现 / 硬编码 / 红线 / 分配热点 / 测试覆盖）与 `Tests/SettingsTests.cs`（6 用例，含 `ValidNames()`↔`Palette` 16 字段契约）。
- **登记 EXC-002**：组件层 **67 处中文硬编码**（15 个文件）—— v2.0.0 关闭的 EXC-001 只覆盖了当时的 Gallery，**组件层从未被扫描**；本报告予以纠正，处置方案待定（见体检报告 §4）。

**Gate 结果摘要**
- 编译 0 错 0 警（框架 + 示例 + 测试三工程）；单测 **174/174**；公共 API **77 类型 / 467 成员**。
- 八道门禁全 PASS（详见 `docs/健康评估与发布记录.md`）；Roslynator **240 条全为 info 级（0 warning / 0 error）**。
- **NOT RUN**：实机启动验证（沙箱无法起游戏进程）—— 移交 PM（启用后看 Player.log `[RUIF] v2.0.0 loaded.`、
  设置页主题切换、**确认底栏不再有 RUIF 按钮**）。

---

## v1.2.1 · 发布前加固（对外暴露 API 的阻断项清零） — 2026-09-10

**修复（真缺陷 · 间歇性）**
- **`Api.Version` 初始化竞态**：原实现由 `RUIFMod` 构造器调 `Api.SetVersion` 赋值，但
  1.6 `Verse.LoadedModManager.CreateModClasses()` 遍历的是 `typeof(Mod).InstantiableDescendantsAndSelf()`
  —— **类型枚举顺序，而非 Mod 加载顺序**（ilspycmd 实证）。因此消费 Mod 在自己的构造期探测
  `Api.IsAvailable()` 会**随机**拿到 0.0.0 → 报「框架已装却不可用」的假阴性。
  修复：版本改为**类型初始化时**由编译期常量 `RUIFMod.VersionString` 解析（`static readonly`），
  彻底去掉对 `RUIFMod` 构造时序的依赖。`Api.SetVersion`（internal）与 `Core/AssemblyInfo.cs`
  的 `InternalsVisibleTo` 一并移除 —— 框架不再有 internal 暴露面。

**修复（接入语义 · 消除"假降级"）**
- `RUIF.Sample` 原实现既声明 `modDependencies`、又在构造器里 `if (Api.IsAvailable(...)) else 降级` ——
  这是**假的优雅降级**：`Api` 位于 RUIF.dll 内，框架缺失时 JIT 构造器即抛
  `FileNotFoundException`/`TypeLoadException`，else 分支永不执行。
  重写为**真软依赖参考实现**：`About.xml` 去掉 `modDependencies`（只留 `loadAfter`）；
  新增 `RuifProbe.cs`（纯反射 + 字符串探测 `RUIF.Api`，自身不引用任何 RUIF 类型）与
  `RuifBridge.cs`（唯一出现 RUIF 类型的地方，方法全部 `NoInlining`，防 JIT 内联把 RUIF
  依赖泄回调用方）；设置页改走 Keyed 文案，按探测结果隐藏 Gallery 入口。

**新增（对外暴露 API 的必要件）**
- `docs/API参考.md`：反射全量导出 **77 个公开类型 / 455 个公开成员**（含真实签名与类型说明），
  生成器 `_scripts/gen_api_reference.ps1`（预加载引擎程序集后再反射，否则派生自
  `Verse.Window`/`Verse.Mod` 的类型会静默漏掉）。
- `Source/RUIF.csproj` 开启 `GenerateDocumentationFile` → 产出 `Assemblies/RUIF.xml`，
  消费方引用 dll 即获 IDE 悬停文档（`NoWarn` 仅豁免 CS1591/1573/1574 三条文档完备性提示）。
- `Languages/{English,ChineseSimplified}/DefInjected/MainButtonDef/RUIF_Gallery.xml`：
  Gallery 主按钮描述原先在 Def 里写死中文，非中文语言玩家会看到中文 → 改 Def 英文兜底 + 双语 DefInjected。
- `Sample/Languages/*`：示例自身文案本地化，作为最佳实践范本（不再依赖 EXC）。
- `_scripts/verify_release.ps1`：发布门禁脚本（版本一致性 / dll 哈希 / 程序集引用白名单 /
  中英 Keyed 键集合逐项比对 / 禁止 Sample 内嵌 RUIF.dll / 发布区无残留垃圾文件）。

**新增（第三方开发者入门：解决「Gallery 不是学习入口」）**
- 起因：F11 Gallery 只能在「框架已启用且游戏在跑」时看，一旦它不在（或框架被禁用），
  第三方开发者手里没有任何可读入口 —— 这不能作为唯一的"知道怎么用"的途径。
- `docs/开发者入口.html` + 生成器 `_scripts/sync_devdocs.ps1` → 随包分发 **`DevDocs/`**（15 文件 / ≈375 KB）：
  `index.html`（30 秒定位 / 依赖两模式 / 5 分钟上手 / 文档地图 / 怎么查 API / **六个常见坑** / 版本承诺）、
  `API参考.md`、`接入指南与引擎可行性审计.html`、`组件设计图鉴.html`、`组件库全量目录.md`、`CHANGELOG.md`、
  `BUILD-INFO.txt`、`_Template/`（＝官方 Sample：`About` + `Source` + `Languages`，**已剔除 `obj/`·`bin/` 与 dll**）。
  **白名单机制**：内部设计文档（方案草案 / 评估 / 落地管线）一律**不进**开发者包。
- 游戏内入口（**不依赖 Gallery**）：`RUIFMod` 新增设置页 —— 版本号 + `RUIF.Settings.*`（中英）+
  「打开开发者文档（浏览器）」「打开组件画廊（F11）」两钮；新增公共 API `RUIFMod.DevDocsPath()`
  （消费 Mod 可复用同一入口）；启动日志追加一行 DevDocs 路径便于定位。
- 门禁新增**第 7 道「开发者包完整性」**：必备文件齐全 / `BUILD-INFO` 版本 == dll 版本 /
  `_Template` 与 Sample **逐文件哈希一致**（防模板漂移）/ 模板无 dll 与 `obj`·`bin` / 发布区 `DevDocs` 已同步。

**变更**
- 版本 `v1.2.0 → v1.2.1`（`RUIFMod.VersionString` / `About.xml` / Sample About 同步）。
- `README.md` 按实测重写（原停留在 v0.3「设计草案、无 Assemblies、不可运行」）。
- `docs/健康评估与发布记录.md` 更新为 v1.2.1 的 Gate 判定；`docs/组件库全量目录.md` 基线更新为反射实测值。

**Gate 结果摘要**
- 编译 0 错 0 警（框架 + 示例 + 测试三工程）；单测 **168/168**（`ApiTests` 重写为 5 用例，
  新增「不构造 RUIFMod 也能拿到版本」的回归断言）。
- `verify_release.ps1` 全项 PASS（详见 `docs/健康评估与发布记录.md`）。
- 公共 API **无破坏性变更**（`Api.Version`/`Api.IsAvailable` 签名不变；`SetVersion` 为 internal）。
- **NOT RUN（沙箱无法起游戏进程，留实机）**：F11 Gallery 12 页走查 · 禁用 RUIF 后 Sample
  降级实测 · Player.log `[RUIF] v1.2.1 loaded.` 验证。

---

## v1.2.0 · 第一批 T1 四原子落地 + 反馈类 Gallery 接线 — 2026-09-09

**新增**
- `Source/Components/Industrial.cs`：`IndustrialFmt` 纯计算 8 函数（GaugeNorm / GaugeZone / BottleneckIndex / Shares / MergeRunLengths / LedCell / SegmentsTotal / 状态段守护）+ `Industrial` 绘制件 4 件（Gauge 指针表盘 / LEDMatrix 状态灯阵 / BottleneckRibbon 瓶颈带 / MachineStateTimeline 设备状态时序）+ `MachineState` 枚举。全令牌色表（ZoneColor / LedColor / StateColor）、零贴图、零逐帧堆分配、不读输入。
- `Tests/IndustrialTests.cs`：22 用例（分区判定与阈值倒置 fail-safe、占比归一与均匀兜底、游程归并、灯阵几何、色表令牌一致性）。
- Gallery 新增 ⑩ 工业分类（中英 Keyed `RUIF.UI.Cat.10`）；⑤ 反馈类从 Pending 占位改为 15 件真实接线演示（Toast 三态 / AlertBanner / ConfirmDialog / RichTooltip / ProgressOverlay / EmptyState / NotificationEntry / UndoToast / StickyNote / AlertBlinker 三速 / StepProgressList / InlineError / BatchConfirm）。
- `ContentH` 4400 → 5300（+⑩ 工业 ≈560f、⑤反馈真实演示 ≈+330f）。
- 版本 v1.1.0 → v1.2.0（RUIFMod.cs / About.xml）。

**Gate 结果摘要**
- 编译 0 错 0 警；单测 146 → 168（+22 全过）；两区 dll 哈希同步一致。
- 遗留：⑩ 工业 Gallery 实机走查 NOT RUN；DataBridge（二/三批前置）动手前需接口冻结评审。


## v1.1.0 · 图表原子落地（T0+T1）+ StatusFmt 测试化 — 2026-09-09

**新增**
- `Source/Components/Charts.cs`：`ChartsFmt`（线性标度 / SeriesMax·SeriesRange / 1·2·5 NiceTicks / 柱几何 / 扇区角 / 直方 bin / 热力色，全部纯计算可单测）+ `Charts` 10 件：Sparkline、BarChartV、BarChartH、StackedBarChartV、LineChart、AreaChart、DonutChart、PieChart、ScatterChart、HeatmapGrid。全部令牌消费、零输入读取、零贴图、Nice 刻度缓冲静态复用零逐帧分配。
- `Theme.Series(int)`：数据可视化系列色（令牌循环派生，负索引安全）。
- Gallery 新增 ⑨ 图表分类（10 件全演示；中英 Keyed 增 `RUIF.UI.Cat.09`）。
- `Tests/StatusTests.cs`（12 用例）+ `Tests/ChartsTests.cs`（14 用例）。

**变更**
- StatusAtoms 纯计算抽出 `StatusFmt`（PulseAlpha / DotRect / TrailAlpha / GlyphOf），公共签名不变——不破坏 v1.0 API 冻结。
- Theme.cs 类注释固化「数据可视化例外白名单」④ 项（WithAlpha / Series / 数据插值 Lerp / 取色器 HsvColor）。
- 评估风险清零：P1 StatusAtoms 测试缺口已补；P2 DataExAtoms 的 `Event.` 确认为停靠命中测试（DataExFmt.DockZone，交互逻辑，合规）。

**Gate 结果摘要**
- 编译 0 错 0 警；单测 146/146 全过（StatusTests 12 + ChartsTests 14 新用例）；Gallery ⑨ 实机走查 NOT RUN（沿用 v1.0 遗留清单）。
- 环境修复：Tests 工程补拷 `UnityEngine.SharedInternalsModule.dll`（NUnit 对 Color 结构体做失败消息格式化时的隐式依赖）。

---

## v1.0.0 · S5 收口：P1-P12 页面复刻验收 + 语言子包 + SemVer 冻结 — 2026-09-09

**新增（页面复刻验收）**（`DevTools/GalleryPages`，全部由已落地组件拼装，**未新增任何组件能力**）
- **P1 总督仪表盘**（M2）：KPI 带 5 徽章 + 工时产出条形图 + 事件流 + 工时榜/需求风向/警报三联
- **P2 殖民者档案**（M1）：头像 + 特质 chips + 双 StatRow + 核心技能双列 + 晋升条件 ✓✓✗ + 生涯时间轴
- **P3 战役志**（M2 变体）：对抗 hero（VS 双条）+ 三联大数字卡 + 接敌合并 chips + 功勋榜/缴获
- **P4 传承谱系**（M5）：双世家 CollapsibleTree 并排（展开可交互、亡者划线）+ 传承链进度带
- **P5 车间指挥台**（M6）：KPI 带 + 12 道工序链 chip 流（瓶颈态）+ 交付阈值滑条（可交互）+ 生产队列 QueueEditor
- **P6 设置中枢**（M1 变体）：Segmented 页签（内容随页切换）+ ToggleSwitch ×3 + 密度滑条 + 采样 Stepper + **主题即时换肤（真实生效）**
- **P7 工作调度台**（M7 · Work Tab 实证）：ProfilePreset + PriorityGrid + TimetableMatrix 全可交互
- **P8 研究图谱**（M9 · ResearchPal 实证）：SearchField 过滤 + NodeGraph（选中态）+ 进度/推荐双卡
- **P9 贸易中心**（M8 变体）：双清单点选（高亮）+ 交易平衡条 + 清空/成交（成交 FlashOverlay 反馈）
- **P10 电力中枢**（M2 变体）：供需三行条 + **警报语义区分（未接电网 vs 供电不足，1.6 电网链结论）** + 24h 用电曲线
- **P11 健康中心**（M1）：病患队列（危重度着色）+ HealthRows 身体状况 + 手术计划
- **P12 层叠工作区**（M10 · RimHUD 实证）：左停靠事件流 + 中电力卡 + 浮动档案 HUD + 底部 QuickDock + DockHost

**新增（语言子包）**
- `Languages/English/Keyed/RUIF_UI.xml` + `Languages/ChineseSimplified/Keyed/RUIF_UI.xml`
- 键：`RUIF.UI.Cat.01-08`（分类名）/ `RUIF.UI.Preset.01-12`（预设名）/ `RUIF.UI.Gallery.*`（索引/显示全部）
- Gallery 分类索引与预设选择器已切 `.Translate()`；页内演示数据不属于框架文案（不进 Keyed）

**变更（SemVer 冻结）**
- 版本 `v0.3.0-alpha → v1.0.0`（`RUIFMod.VersionString` + About.xml 同步）
- **API 冻结生效**：删改公共 API 须先 `[Obsolete]` 保留一个主版本
- About.xml 描述更新为发布口径（交付清单 + 接入方式 + SemVer 承诺，中英双语）
- 示例消费 Mod 断言升级 `IsAvailable("1.0")`（版本门面演示），Sample About 同步 v1.0.0

**Gate 摘要（GATE-REL）**
- 编译 0 错 0 警（框架 + 示例）；单测 122/122；图鉴侧栏 ⑧ 预设 ●12/12（46 链 0 死链）
- 两区同步：`Mods\RUIF`（含 Languages）+ `Mods\RUIF.Sample`，dll 哈希一致
- **NOT RUN（沙箱无法起游戏进程，留实机）**：F11 Gallery 12 页实机走查；**禁用 RUIF → 示例 Mod 降级实测**；Player.log `[RUIF] v1.0.0 loaded.` 验证

**进度**：P1-P12 复刻 12/12 · 语言子包 ✅ · SemVer 冻结 ✅。**S0-S5 全部完成**；v1.1 待办 = ⚠️ 素材/算法件（Charts/RelationGraph 弧形/MiniMap/MultiLineEditor 真多行）+ CommandPalette 全局焦点仲裁。

---

## v0.3.0-alpha · S4 任务⑥ E 导航 12 件 — 2026-09-09

**新增**（`Components/NavAtoms` + `NavAtomsB` + `NavFmt`）

E1 导航核心 6：
- `TabBar` 顶栏页签（底部指示条 0.2s 级滑动插值，位置由调用方持 `ref indicatorX`；溢出自动收进「更多 n」官方 FloatMenu）
- `Breadcrumb` 面包屑（各级可点 `Action<int> onJump`，末级为当前位）
- `Pager` 分页器（首/末/步进/页码，省略号折叠；`pageBuf` 传调用方缓冲零分配）
- `CollapsibleTree` 折叠树（`TreeNode` 数据载体，展开态/亡者划线/计数角标由调用方持有；返回本帧点击节点）
- `AnchorDots` 侧边锚点（几何方点零贴图，当前节放大 + 标签浮现，点击写回 `ref current`）
- `CommandPalette` ⚠️ v1 简化：窗口内嵌入绘制 + 模糊过滤（`NavFmt.FuzzyScore`）+ ↑↓/回车/Esc 键盘操作（在 TextField 消费前接管）；全局 Ctrl+K 唤起与快捷键仲裁留 v1.1

E3 时间·域·返回栈 6：
- `TimeSpeedControl` 时间流速组（官方 `Verse.TimeSpeed` 四态；暂停双杠 + 1/2/3 雪佛龙，零贴图）
- `LayerSwitch` 域切换（层集合由调用方定义，当前域高亮 + 底部指示线）
- `NavBackBar` + `NavStack` 返回栈（容量 32，Push 截断前进分支 + 超限丢最旧；«/» 双钮 + ≡ 历史 FloatMenu，返回 `NavAction`）
- `JumpList` 最近查看（类别徽 + 相对时间 `RelativeTime`，返回点击索引）
- `SidebarCollapse` 折叠把手（点击切 `ref open`，宽度动画由调用方以 `NavFmt.Approach` 驱动）
- `FocusSearch` 聚焦搜索（⚠️ 窗口内语义：Ctrl+F 聚焦 / Esc 归还焦点）

纯计算 `NavFmt`：`LabelUnits`（CJK 记 2 单位）/ `TabWidth` / `TotalPages` / `ClampPage` / `PageList`（-1=省略号，调用方缓冲零分配）/ `FuzzyScore`（子序列 + 连续 + 前缀加成）/ `RelativeTime` / `Approach` / `CountNodes` / `StackLimit`

**Gallery**：⑥ 导航 Nav 从占位清单替换为 12 件全实机演示（页签滑动/面包屑截断/分页/传承树/锚点/命令面板/流速/域切换/返回栈/最近查看/侧栏折叠/Ctrl+F）

**测试**
- 单测 **122/122 通过**（新增 NavFmt 10 项：LabelUnits/TabWidth/TotalPages+ClampPage/PageList 折叠/FuzzyScore/Filter 排序/RelativeTime/Approach/CountNodes/NavStack 截断与超限）
- 编译 0 错 0 警；发布区 dll 哈希一致

**修复（踩坑记录）**
- `CommandPalette` 类内调 `Draw.*` 被**同名方法遮蔽**（CS0119）→ 全限定 `RUIF.Draw.*`（命名铁律第二次踩中）
- `Event.used` 属性在当前引擎引用集**不存在**（CS1061）→ 直接判 `ev.type == EventType.KeyDown`
- TabBar「更多」菜单 lambda 写 `ref sel` → **CS1628** → 静态 `pendingMorePick` 中转（同 InputPlusAtoms 的 InPlaceDropdown 教训）
- `NavStack.pos` 私有被操作条访问 → 暴露只读 `Position`

**进度**：**导航 12/12 封顶**；E1+E3 完成。S4 剩余：⑤ 反馈 17 + Gallery 完整版（三档代码）

---

**新增**（`Components/InputPlusAtoms`）
- `QuickAmounts` 快捷数量（1/10/100/最大，受 max 钳制）
- `SliderTicks` 刻度滑条（滑后吸附最近刻度 + 刻度点）
- `WheelAdjust` 滚轮调值（ScrollWheel + `Event.current.Use()` 防穿透；方向约定集中在 `InputPlusFmt.WheelStep`，一处可反转）
- `NumericField` 数值输入（解析失败红框，不改写 value）
- `PathField` 路径输入（非法字符校验 + ✓/✕ 状态）
- `InPlaceDropdown` 原生下拉（官方 **FloatMenu/FloatMenuOption**；**用回调不用 ref**——lambda 不能捕获 ref 参数，CS1628）
- `FilterRow` 逐列过滤输入
- `MultiSelectList` 多选列表（Ctrl 切换 + Shift 区间 + 普通单选）
- `QueueEditor` 队列编辑（顶置 ↑ / 移除 ×）
- `PaintStrokeGrid` 涂抹网格（Work Tab 实证，点击循环优先级）
- `KeyConflictHint` 键位冲突提示（重复键 → 冲突项计数 + 阻断提示）
- `MultiLineEditor` ⚠️ v1 简化：按行拆分多个单行框（真多行编辑留 v1.1）
- 纯计算 `InputPlusFmt`：`QuickAmount` / `SnapToTicks` / `WheelStep` / `ToggleAt` / `SelectRange` / `EnqueueInt` / `RemoveAtInt` / `MoveToFrontInt` / `TryParseClamp` / `IsValidPath` / `NormalizePath` / `HasConflict` / `ConflictCount` / `SplitLines`

**测试**
- 单测 **92/92 通过**（新增 9：QuickAmount / SnapToTicks / WheelStep / MultiSelect / Queue / TryParseClamp / Path / Conflict / SplitLines）

**修复（踩坑记录）**
- `InPlaceDropdown` 在 lambda 中写 `ref index` → **CS1628**（不能在 lambda 中捕获 ref 参数）→ 改为**回调式** `Action<int> onPicked`
- 测试里误用 `q[0..3]` 段表达式（非法）→ 改为显式取元素

**进度**：**输入 28/28 封顶**（B1 8 + B2 8 + B2+ 12）；数据 9/40

---

## v0.3.0-alpha · S3-1b B2 输入扩展 8 件 — 2026-09-09

**新增**（`Components/InputExAtoms`，工坊实证）
- `DragListRow` + `GripHit` —— 拖拽重排行（把手点阵 + 落点示意；拖拽态由调用方持有）
- `DoubleSlider` —— 双端范围滑条（区间示意条 + 下限/上限两条官方滑条，**lo&gt;hi 自动交换**）
- `ColorPicker` —— HSV 取色器（三滑条 + 预览 + 十六进制；**手写 HSV↔RGB**，不依赖未核实的 `Color.HSVToRGB`）
- `CycleButton` —— 循环按钮（点击在多个值间循环）
- `EditableLabel` —— 行内编辑（常态→点击→编辑→Enter 或 ✓ 提交）
- `NumberScrubber` —— 数值刷（按住横向拖动改值）
- `PriorityGrid` —— 优先级网格（Work Tab 实证，点击循环 1-4）
- `TimetableMatrix` —— 作息矩阵（Work Tab 实证，人员 × 24 刻，点击切换三态）
- 纯计算 `InputExFmt`：`NextPriority` / `NextTimetableState` / `NextIndex` / `ScrubValue` / `DropIndex` / `Reorder` / `HsvColor` / `RgbToHsv` / `Hex`

**测试**
- 单测 **83/83 通过**（新增 9：NextPriority / NextTimetableState / NextIndex / ScrubValue / DropIndex / Reorder / Hsv / RgbToHsv / Hex）

**修复（踩坑记录）**
- `InputExAtoms` 误调用 `InputFmt.GripDotSize` → 实际在 **`InteractFmt`**（CS0117）；教训：**复用纯计算助手先确认所在类**
- Gallery 输入节新增 B2 演示区（可拖拽重排、可拖范围、可取色、可循环、可编辑、可数值刷、可点优先级与作息矩阵），并保留 B2+ 12 件待落地清单

**进度**：原子层 42/42 · 输入 16/28（B1 8 + B2 8）· 数据 9/40

---

## v0.3.0-alpha · S3 任务④ C2+ 官方数据系统 14 件 — 2026-09-09

**新增**（`Components/OfficialFmt` + `OfficialDataAtoms` / `B` / `C` 三个静态类，数据由调用方注入）
- `WealthBreakdown` 财富构成（WealthWatcher 语义：分段堆叠 + 总额 + 图例）
- `RecordsTable` 记录表（Pawn_RecordsTracker 语义：键值两列表）
- `HistoryFeed` 历史事件流（日期头 + 事件行 + 类型 pill）
- `TaleCard` 传说卡 / `LetterCard` 信件归档卡（类型色条）/ `BillList` 账单队列（完成/目标/暂停）
- `QuestBoard` 任务板（QuestScriptDef 语义：缩略卡+进度+ETA）/ `TitleCard` 头衔卡（许可勾叉）
- `PsycastTree` 心能树（分层节点）/ `IdeoCard` 文化卡 / `CaravanPanel` 商队面板（负重+口粮+ETA）
- `GeneChips` 基因芯片（类别色条+复杂度）/ `MessageFeed` 瞬息消息（Verse.Messages）/ `PolicyCard` 政策卡
- 纯计算 `OfficialFmt`：`RelationLabel`（五档边界）/ `RelationColor` / `CheckColor` / `BillRatio` / `EtaLabel` / `ComplexityLabel` / `WealthTotal` / `HexWithHash`

**测试**
- 单测 **106/106 通过**（新增 6：RelationLabel 五档 / RelationColor / BillRatio / EtaLabel / WealthTotal / HexWithHash）

**修复（踩坑记录）**
- **HStack 游标不是 Rect**：误用 `row.x / row.center / row.yMax / row.width`（CS1061/CS0122）→ 改用外层 rect 几何计算。**教训：Layout 游标只暴露 Take/Rest/前进量，不要当 Rect 用**

**进度**：原子层 42/42 · 输入 28/28 · **数据 40/40 封顶**

---

## v0.3.0-alpha · S3 任务③ C2 数据扩展 12 件 — 2026-09-09

**新增**（`Components/DataExAtoms` + 纯计算 `DataExFmt`）
- `NodeGraph` 节点图（ResearchPal 实证）：网格节点 + **L 形连线**（先横后竖，无需斜线）+ 选中高亮
- `DockHost` 停靠宿主（RimHUD 实证）：折叠把手 + **四缘吸附落区提示**（`DockZone` 纯计算可测）
- `ProfilePreset` 预设档案（套用/存为）
- `BuffRow` 增减益行（正绿负红）/ `QuestCard` 任务卡（进度+目标清单）/ `FactionCard` 阵营卡（声望条，负值红）
- `CreatureCard` 生物卡（训练度五档）/ `RoomStats` 房间属性条组 / `PowerGrid` 电力概览（净值+电池水位）
- `WeatherStrip` 天气条带（五类令牌色）/ `StorageBreakdown` 库存构成堆叠条+图例 / `CompareBars` A/B 对比（优胜高亮）
- 纯计算 `DataExFmt`：`LinkMid` / `NodeRect` / `DockZone` / `PresetIndex` / `Sum` / `Ratio` / `Share` / `NetBalance` / `CompareWinner` / `RelationTone` / `TrainingTone` / `WeatherTone` / `ShareRatios`（写调用方缓冲零分配）

**测试**
- 单测 **100/100 通过**（新增 8：LinkMid / NodeRect / DockZone / PresetIndex / Sum+Share / ShareRatios / NetBalance+Compare / Tones）

**修复（流程教训）**
- 分段构建时混入 `。`（中文句号）与 `**` 行尾污染 → 用 **PowerShell 脚本批量清理**（行尾 `。`→`;`、行尾 `**` 移除），比手改 14+ 处更可靠

**进度**：原子层 42/42 · 输入 28/28 · **数据 21/40**（C1 9 + C2 12）

---

## v0.3.0-alpha · S3-1 基础输入 8 件 + S3-2 C1 数据组件 9 件 — 2026-09-09

**新增 · S3-1 基础输入 8 件**（`Components/InputAtoms`）
- `Segmented` 分段选择（**取代原版缺失的 RadioButtonGroup**）
- `ToggleSwitch` 开关（Track + Knob）
- `Stepper` 数值步进器（− 值 +）
- `SliderWithValue` 滑条 + 实时数值（走官方 **HorizontalSlider 的 ref 语义**，不是返回 float）
- `SearchField` 搜索框（**防抖 200ms**：buffer 即时回显，query 停止输入后才更新）
- `TagPicker` 标签多选（chips）
- `HotkeyBind` ⚠️ 键位显示 + 改键触发（捕获属官方 KeyPrefs 与焦点仲裁，此处只做显示与回调；`currentKey` 由调用方提供，避免依赖未核实字段）
- `StateSample` + `InputState` **六态规范**（默认/悬停/按下/选中/禁用/焦点）
- 纯计算 `InputFmt`：`Resolve` / `StepValue` / `Slider01` / `SliderValue`(roundTo) / `SegmentIndex` / `SegmentRect` / `DebounceReady`

**新增 · S3-2 C1 数据组件纯 ✅ 9 件**（`Components/DataAtoms`）
- `PawnCard` / `SkillBars`（技能十档色） / `NeedsPanel` / `TraitChips` / `HealthRows` / `HeatCalendar`（分档着色，零素材） / `RankList`（前三徽记） / `StatTable`（表头+斑马纹+比例列宽） / `TimelinePro`（节点+日期+文本）
- 纯计算 `DataFmt`：`Bar01` / `SkillColor` / `RowRect` / `HeatLevel` / `RankMedal`
- ⚠️ 未含（待素材或原型，v1.1）：MoodGauge 弧形 / GearSlots / InventoryGrid / RelationGraph / Charts

**Gallery**
- 输入节、数据节升级为**真实可交互演示**（分段/开关/步进/滑条/搜索防抖/标签多选/六态；人物卡/技能条/需求/特性/健康行/热力日历/排行/统计表/时间轴），并保留各自待落地清单

**测试**
- 单测 **74/74 通过**（输入 7 + 数据 5）

**进度**：原子层 42/42 · 输入 8/28 · 数据 9/40

---

## v0.3.0-alpha · S2 批4-2b 收口 + Gallery 大演示台 — 2026-09-09

**新增（原子层收口 42/42）**
- `Components/InteractAtoms`（Mod Manager / 工坊实证）：
  - `GripHandle` 拖拽把手（点阵几何绘制）
  - `EyeToggle` 可见性眼（外框+瞳点；不可见画斜杠，**分段方块近似**，无贴图）
  - `SearchHighlight` 命中高亮（命中段 accent 底 + 普通段正文色，逐段测量）
  - `ToolbarIcon` 工具图标钮（激活态 accent 底+描边）
  - 纯计算 `InteractFmt`：`EyeRect` / `HighlightSegmentCount`（**不产生空段**）/ `GripDotSize`
- `Components/MiscAtoms`：
  - `PointCostBadge` 预算点数（剩余绿 / 用尽黄 / 超支红）
  - `BubbleTag` 气泡标签（矩形+阶梯小尾，无圆角无贴图）
  - `ThemedScrollBar` 主题化细滚动条（配合 `Layout.BeginScroll(..., showScrollbars:false)`）
  - 纯计算 `MiscFmt`：`PointsRemaining/Overdrawn` / `BubbleTail` / `ScrollThumbHeight`（最小 16）/ `ScrollThumbY`
- `Layout.BeginScroll` 新增 `showScrollbars` 参数（支持关掉原生条、改用自绘主题条）

**Gallery 重做为「大演示台」**
- 窗口 → **1180×1040**；布局改为 **左 20% 分类索引 + 右可滚演示区**（自绘细滚动条）
- **左索引**：8 个一级分类，每条显示「名称 · 已落地/总数 · 类名」，可点选只看该节
- **右演示区**：每节含 **组名 + 类名 + 已落地/总数** 标注；令牌/基础组件为**真实可交互演示**（排序表头、拖列宽、密度切换、闪边动画、斜向擦除、超载容量条、命中高亮…）；输入/数据/反馈/导航为**待落地清单**（逐条列出组件名与 S* 阶段）；版式/预设列出 M1-M12、P1-P12

**测试**
- 单测 **62/62 通过**（新增 7：EyeRect / HighlightSegmentCount / GripDotSize / Points / BubbleTail / ScrollThumbHeight / ScrollThumbY）

**修复（踩坑记录）**
- 断言把整串命中算成 2 段、多命中算多 1 段 → 实现正确（**不产生空段**），改断言；教训同前：**先核算式再改代码**

**进度**：**原子层 42/42 封顶**；S3 组件层（输入 28 + 数据 40）待启动

---

## v0.3.0-alpha · S2 批4-2a 面板 + 进度容量 7 件 — 2026-09-09

**新增**
- `Components/PanelAtoms`（RimHUD 实证）：
  - `StatRow` —— 标签 + 数值 + 可选微型条
  - `MiniProfileChip` —— 色块头像 + 名字 + 两条 micro 状态条
  - `FlashOverlay` —— 警示闪边（边框 alpha 脉动）
  - `SectionBar` —— 薄分段头（折叠箭头 + 标题 + 右侧计数）
  - 纯计算 `PanelFmt`：`FlashAlpha` / `RowSplit`
- `Components/ProgressAtoms`：
  - `FillWipe` —— 斜向进度擦除（**零贴图**：按行计算斜切宽度）
  - `CapacityBar` —— 容量条（>85% 转黄；超载时右端红色虚线段）
  - `ZoomIndicator` —— 缩放读数 + 适配复位按钮
  - 纯计算 `ProgressFmt`：`CapacityRatio` / `OverflowRatio` / `ZoomLabel` / `WipeWidth`
- Gallery 新增「面板 + 进度容量」演示区（闪边动画 / 斜向擦除动画 / 正常与超载容量条），窗口 → 620×1000

**测试**
- 单测 **55/55 通过**（新增 8：FlashAlpha / RowSplit ×2 / CapacityRatio / OverflowRatio / ZoomLabel / WipeWidth ×2）

**修复（真缺陷，由单测暴露）**
- `WipeWidth` 原公式在 **progress=0 时底部行仍有正偏移** → 会画出一条残留三角（0% 不该显示任何东西）
- 修复：倾斜量乘以 `edge = 1 - |2p-1|`，使倾斜在 **0% 与 100% 处归零**——0% 不画、100% 每行铺满

**进度**：原子层 33/42

---

## v0.3.0-alpha · S2 批4-1 表格原子 5 件 — 2026-09-09

**新增**
- `Components/TableAtoms`（Numbers / Work Tab 实证）：
  - `TableAtoms.HeaderCell` —— 排序表头（悬停高亮 + ▲▼，点击返回，状态由调用方推进）
  - `TableAtoms.ValueCell` —— 阈值着色单元（底 22% 带色叠加，数值右对齐）
  - `TableAtoms.ResizeHandle` —— 列宽拖拽手柄（**状态由调用方持有**，组件无隐藏状态）
  - `TableAtoms.FrozenSplit` + `TableFmt.FrozenContentOffsetX` —— 首列冻结
  - `TableAtoms.DensitySwitch` —— 行密度三档（紧凑 22 / 标准 28 / 舒适 34）
  - 纯计算 `TableFmt`：`NextSort` 三态循环 / `RowHeight` / `ValueRatio` / `BandColor` 阈值带 / `ResizeWidth` 钳制 / `ColumnSplit`（**写调用方缓冲，零分配**）/ `FrozenContentOffsetX`
- Gallery 新增「表格原子」演示区：可点排序表头 + 阈值着色 + **可拖拽列宽** + 密度切换（窗口 → 600×860）

**测试**
- 单测 **47/47 通过**（新增 8：NextSort / RowHeight / ValueRatio / BandColor / ResizeWidth / ColumnSplit ×2 / FrozenOffset）

**修复（踩坑记录）**
- `DrawTableSection` 内又写 `Draw.Box/Label/LineH` 触发 **CS0119**（本类 `Draw` 方法遮蔽 `RUIF.Draw`）—— 同坑第二次，已全限定
- `UnityEngine.Input` 在当前引用集下不可用（CS0103）→ 改用 **IMGUI `EventType.MouseUp`** 判断拖拽结束（同时避免额外引用）

**进度**：原子层 26/42

---

## v0.3.0-alpha · S2 实机反馈修复：Caret 过渡语义 — 2026-09-09

**修复（实机反馈）**
- `StatusAtoms.Caret` 在 Gallery 演示中被做成**持续循环旋转**（progress 锯齿循环），观感为"绕圆心打转的坏组件"
- 根因：**演示用法错误**——`Caret(progress01)` 的设计语义是**展开状态切换的过渡动画**，动画进度应由展开状态变化驱动，禁止持续循环
- 修复：①新增 `Caret(Rect, bool expanded, Color?)` 布尔态重载（推荐用法，折叠 ▸ / 展开 ▾）②Gallery 演示改回静态两态 ③组件注释写明"progress 应随状态变化，禁止循环"
- 附带确认：`Draw.Rotate` 的 `GUI.matrix` 进出栈配对正确，无旋转泄漏（截图中的旋转仅发生在箭头自身）

**Gate 摘要**
- GATE-UI：PASS（语义修正后组件状态齐备；演示与组件语义一致）

---

## v0.3.0-alpha · S2 布局视觉原子第三批 — 2026-09-09

**新增**
- `Components/VisualAtoms`（布局视觉，第三批完结）：
  - `Swatch` 色卡（实心 + 描边；选中态加 accent 外环）
  - `GradientBar`（lo→hi 分段插值，**零贴图渐变**）
  - `PinBadge` 钉选角标 —— **几何绘制（钉头+钉柄），不用 emoji**（原版字体不保证有 📌 字形）
  - `OverflowFade` 右缘渐隐 —— **用背景色分段叠加递增 alpha 实现淡出**（✅ 但需调用方传入已知背景色；真 alpha 遮罩版留 v1.1 素材项）
  - 纯计算 `VisualsFmt`：`LerpT` / `FadeAlpha` / `PinRect`
- Gallery 新增「布局视觉原子」演示区（4 色卡 + 渐变条 + 钉选卡 + 渐隐文字），窗口放大至 560×650

**测试**
- 单测 **39/39 通过**（新增 3：LerpT / FadeAlpha / PinRect）

**原子层进度**：21/42（批1 文本 6 + 批2 几何/状态 11 + 批3 视觉 4；`SectionFrame`/`AspectBox`/`Anchor` 已于 S1 落地）
**剩余**：批4 · 工坊实证原子 19 件

---

## v0.3.0-alpha · S2 几何/状态原子第二批 — 2026-09-09

**新增**
- `Core/Draw`：新增 **`Rotate` 旋转作用域**（`GUI.matrix` 进/出栈），供 Caret 展开动画与贴图旋转绘制使用
- `Components/Shapes`（几何/装饰）：`CornerFrame`（四角 L 型 HUD 框）/ `GradientRule`（**分段 alpha，零贴图渐隐**）/ `RuleWithLabel`（── 记录 ──）/ `Watermark` / `InsetPanel`（上左暗下右亮凹陷）
  - 纯计算 `ShapesFmt`：`GradientAlpha` / `CaretAngle` / `SpinnerHead` / `RuleSplit`
- `Components/StatusAtoms`（状态）：`PulseDot`（alpha 正弦脉动）/ `Glyph`（✓•!✕‖ 令牌取色）/ **`Spinner` 零贴图版**（8 段绕圆、头部渐亮；另有贴图版走官方 `Widgets.DrawTextureRotated`）/ `Caret`（0→90° 旋转）/ `ScrollFadeHint`⚠️（当前为纯色条+▼，真渐隐需遮罩贴图，v1.1 素材项）
- Gallery 新增「几何/状态原子」演示区（呼吸点/状态图形/加载指示器/展开箭头均为 `Time` 驱动动画）

**测试**
- 单测 **36/36 通过**（新增 5：GradientAlpha / CaretAngle / SpinnerHead / RuleSplit ×2）

**修复（踩坑记录）**
- `Shapes.cs` 缺 `using System;` → `Math.Floor` 未解析（CS0103），导致 dll 未产出并引发测试级联失败；补 using 后全绿

**Gate 摘要**
- GATE-UI：PASS（原子无持久状态；颜色全部走令牌；动画参数为调用方传入，可暂停/可测）

---

## v0.3.0-alpha · S2 文本原子第一批 — 2026-09-09

**新增**
- `Components/TextAtoms`：
  - `TextFmt.FormatNumber` —— 数值缩写（<1e4 整数 / ≥1e4 k / ≥1e6 M / ≥1e9 B），纯函数
  - `TextFmt.TypewriterChars` —— 打字机进度→可见字符数（纯函数）
  - `TextAtoms.DateFull` —— 官方 `RimWorld.GenDate.DateFullStringAt` 直通（命名空间已核实）
  - `TextAtoms.UnitLabel` —— 数值右对齐 + Tiny 灰单位缀右
  - `TextAtoms.Typewriter` —— progress01 驱动逐字显示（动画进度由调用方 Time 控制）
  - `TextAtoms.SubtitleLine` —— 主文案 + 小注双行
  - `TextAtoms.RichSpan` / `RichTextLine` —— 多段混排（各自颜色/字体），两遍测量支持左/中/右锚 —— **原标 ⚠️，实际用分段绘制即可，升 ✅**
- Gallery 新增「文本原子」演示区（打字机用 `Time.realtimeSinceStartup` 驱动循环动画）

**测试**
- 单测 **31/31 通过**（新增 FormatNumber 8 用例 + TypewriterChars 4 用例 + 越界钳制 1 用例）

**修复（踩坑记录）**
- 打字机断言把「虫群自地下涌来」数成 8 字（实为 7）→ 改断言不改实现

---

## v0.3.0-alpha · S1 内核 — 2026-09-09

**新增**
- `Core/Draw`（L0）：`Color`/`Enabled`/`Font`/`Anchor` 四个**零分配 struct 作用域**；原语 `Box`/`Border`/`LineH`/`LineV`/`Label`/`WithAlpha`
- `Layout/Layout`（L4）：`Column(VStack)` / `Row(HStack)` / `Grid` / `Split` / `BeginScroll·EndScroll` / `Inset` —— 即时模式轻量布局助手，**不做保留模式、不做自动 diff**
- `Theme/Theme`（L1）：Vanilla（默认，贴合原版暖灰）/ Dark 两套**具体色值**令牌；间距 · 字号 · 组件度量常量；`Register(id, palette)` / `Apply(id)`（**未知 id 保持当前不变**，fail-safe）
- `Layout/Atoms`：`AspectBox`（锁比例居中）/ `Anchor`（九宫锚定）/ `SectionFrame`（带标题组框）
- `DevTools/GalleryContent`：Gallery 改画**真实渲染**（组框 + VStack 行 + 进度条 + 3×2 网格 + 主题切换按钮），页签与对话框共用

**变更**
- `MainTabWindow_Gallery` / `Dialog_Gallery` 的 DoWindowContents 改为调用 `GalleryContent.Draw`

**测试**
- 单测 **18/18 通过**（原 5 + 布局/原子 8 + 主题 5）：VStack/HStack/Grid/Split/Inset/AspectBox/Anchor + 主题切换/注册/未知 id 回退/AccentSoft/WithAlpha

**修复（踩坑记录）**
- CS0119：`GalleryContent.Draw` 方法与静态类 `RUIF.Draw` 同名 → 类内 `Draw.Box` 被遮蔽，改全限定 `RUIF.Draw.*`
- 单测算术：`Split` 断言写错 1.0（实现正确，断言修正）

**Gate 摘要**
- GATE-ARCH：PASS（Core/Layout/Theme 单向，DevTools 依赖前三者，无反向）
- GATE-UI：PASS（组件无持久状态；令牌集中；`Draw` 作用域保证状态必恢复）
- GATE-TEST：PASS（0 错 0 警 + 18/18 单测）

---

## v0.3.0-alpha — 2026-09-09（I0 地基冲刺）

**新增**
- 工程骨架：`RUIF/Source/RUIF.csproj`（net48 / x64；引用 Assembly-CSharp、UnityEngine、IMGUIModule、TextRenderingModule；**核心不引 Harmony**）
- `RUIFMod`：框架入口，登记版本并写加载日志
- `RUIF.Api`：`Version` / `IsAvailable(minVersion)` —— 消费 Mod 版本断言与降级入口
- `DevTools.Dialog_Gallery`：占位 Gallery 窗口 + `Open()`
- **游戏内一键入口**：`Defs/MainButtons/RUIF_Gallery.xml`（底栏主按钮页签 + `MainTabWindow_Gallery`，热键 F11）
- `Tests/RUIF.Tests.csproj` + `ApiTests`（5 用例，覆盖默认不可用/初始化/最低版本/非法串）
- `Sample/`：示例消费 Mod（引用 RUIF.dll 但 `Private=false`，包内不内嵌；构造期降级断言）
- `_scripts/sync_ruif.ps1`：开发区 → 发布区同步（进程守卫 + 白名单 6 目录 + dll MD5 校验）
- 文档：`API签名核实清单.md`（24 条 API 实证，纠正 5 处命名空间）、`落地管线方案.md`（I0–I5）、`组件库落地方案.md`（S0–S6）、`健康评估与发布记录.md`、`组件设计图鉴.html`（152 组件）

**变更**
- About.xml：版本 → `v0.3.0-alpha`；描述修正为"Alpha 骨架（I0），含 Assemblies 可加载，无组件可供调用"

**Gate 摘要**
- GATE-ARCH：PASS（Core 纯净、无 Harmony、受控静态）
- GATE-LOC：PASS WITH EXCEPTION（EXC-001：占位文案硬编码，有效期至 I1）
- GATE-TEST：PASS（编译 0 错 0 警；单测 5/5；Vanilla 启动经用户确认可加载）
- GATE-REL：PASS WITH EXCEPTION（示例消费 Mod 与 Changelog 本版补齐）

**已知限制**
- 无任何组件（组件自 S2 原子层起落地）
- 无 Languages（用户可见文本临时硬编码，EXC-001）
