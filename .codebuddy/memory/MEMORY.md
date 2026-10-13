# 长期记忆 · RUIF 项目（压缩版 2026-09-21）

## 架构与约定
- 组件 = `public static class XxxAtoms`（无状态纯绘制，首参 `Rect`，数据调用方传）；配套 `XxxFmt` 纯计算可离线单测。作图 `Draw.*`/布局 `Layout.*`；`Tokens.*` 颜色唯一来源；本地化 `Tr.Get`。
- 位置：`Layout/`、`Core/`（Tokens）、`Components/`（csproj glob 收录）；`Source/RUIF.csproj` 输出直进 `Assemblies/`。框架门禁 `dotnet build Source/RUIF.csproj -v quiet` 0 错 0 警；Demo 门禁 `_demo/RUIF_Demo/Source/RUIF_Demo.csproj`。
- 四层架构 498 public static / 113 类型：①基座 Draw/Layout/Measure/Px/Tokens/Tr/Grid/Atoms/Shapes/TexAtoms/TexCache ②原子 TextAtoms/StatusAtoms/VisualAtoms/TableAtoms/PanelAtoms/InteractAtoms/ProgressAtoms/MiscAtoms ③复合 ~120+ ④桥 PawnTable/Reorder/HiAtoms。另 8 个 `*Cand*`。
- 公开 API 面 113 类型/603 成员；改公开 API 跑 `_scripts/gen_api_reference.ps1`；发布跑 `verify_release.ps1`。版本手写 `Source/Core/AssemblyInfo.cs`（1.1.0.0，GenerateAssemblyInfo=false）。
- 框架测试 `dotnet test Tests/RUIF.Tests.csproj`（197 例）；`Assert.IsFalse(cond,msg)` 预求值消息串 ⇒ 失败信息改 if+Assert.Fail。

## 红线 / 坑
- 悬停判定组件层一律 `Verse.Mouse.IsOver(rect)`，禁裸 mousePosition+Contains。Unity 版无 `UnityEngine.Input`（拖拽用 MouseDown/MouseDrag + mousePosition.x）。
- 组件零状态：选中/数值 `ref` 或数组回写。原版 `ButtonText` 自带 hover 的件勿重复挂 HoverFrame；HoverFrame rect 表达式须与组件调用完全一致（提取局部变量）。
- 行高禁魔数：`Measure.RowHeight`（Tiny 18/Small 22/Medium 30）、可点行 `Measure.ControlHeight`=+8。`VStack/HStack` 字段私有，只 `Take()/Rest`。
- `Text.Font` 陷阱：`SectionFrame`/`RuleWithLabel` 内部 CalcSize 测量但 Tiny 绘制 ⇒ 调用前 `using (Draw.Font(Tokens.FontLabel))`。
- rect 宽度四型：①自适应（TextAtoms/Shapes/Progress）②本体（PointCostBadge/BubbleTag/ToolbarIcon）③内部自排（ZoomIndicator≥174/DensitySwitch≥176/MiniProfileChip/StatRow）④视口（ThinScrollBar 外侧+1px、contentH≤height 不画；ScrollFadeHint false 不画）。
- 参数序易错：`PointCostBadge(budget,spent)`、`CapacityBar(used,capacity)`、`ValueCell(value,reference)`；`CornerFrame.armLen`/`OverflowFade.fadeWidth`/`ResizeHandle.x`（0-1 比例）静默夹取。
- `GUI.enabled` 只被原版控件尊重 ⇒ RUIF 禁用态由调用方压暗（WithAlpha(Window,0.62)+Dim）；若用须 try/finally 恢复。
- 输入族：`SliderWithValue` 值标签在外上方 18px；`HotkeyBind` 固定宽 254；`DoubleSlider` ≥54 高；`ColorPicker` ≥83 高；`SliderTicks` 宽≥178 自画值+刻度、SnapToTicks 只有档位可达；`ToggleSwitch` 命中区全 rect；`TagPicker` 不折行；`MiniProfileChip` 高≥36。
- `Atoms.SectionFrame` 返回内容区（Inset(8,12)，标题框外上方 y−8 ⇒ 上留 8px）；段高 = `FramePadV + rows*rowH` 推导。
- 溢出+后画遮盖高频：尺寸取框架度量、循环先判剩余高、布局量不写字面量、平行数组上界由 `.Length` 派生。
- 导航：`NavAtoms.TabBar` 的 `ref indicatorX` 是动画位（调用方插值）；`Pager.pageBuf`、`CommandPalette` filtered/scores 是复用缓冲；`Breadcrumb` 用 `Action<int>`（命中在组件内）；`AnchorDots` 右侧留 74px。
- `NavAtomsB.NavBackBar` 只返回 NavAction 不改栈（« » 用 Draw.Enabled 压暗；Back/Forward 职责在调用方；「≡」内部自带 FloatMenu+JumpTo）。`NavStack`：Push 截断前进分支、容量 32、JumpTo(index)；`JumpList`→int（-1 无）。
- `Verse.TabRecord.Draw` 只画不点，命中由调用方 `Widgets.ButtonInvisible` 补。

## 工具 / 流程铁律
- 同一文件多次编辑顺序执行；`replace_in_file` 偶发假成功 ⇒ 关键补丁 Select-String 复核。编辑与构建不可同批发送（构建跑在编辑之前，耗时异常短即信号）。
- 验证新代码编译进 DLL：字节级检索 `#Strings` 堆；产物大小变化不可靠。能用工具/编译器判的绝不用手写正则脚本；批量改写先副本 diff、逐文件、构建验收。
- PowerShell 单引号不解释 `` `r`n ``。抽检 Select-String pattern 必须按原文措辞。ilspycmd 8.2：一次一类型、0 字节=不存在、输出过滤词会漏 `public const` 行（补查 `const float`）；Unity Rect 产出 `((Rect)(ref x))` 非法 C#。
- `RUIF.xml` M: 条目无返回类型 ⇒ 用 `_scripts/ruif_api.ps1`（`&` 调用，勿 `pwsh -Command` 吞 `$_`）。
- 排错入口 `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`；重复异常只取首条完整堆栈。
- IMGUI 铁律：绘制抛异常 ⇒ GUIClip 失衡整局卡死 ⇒ 逐件 try/catch（验证台双层隔离）+ 出栈 finally + `Log.ErrorOnce`。
- 缓存键必须含「所有会改变输出的输入」（饼图吃到环图贴图事故：kind 写成常量）。
- CS0649 = 字段从未赋值 ⇒ 让它真可达而非删除。`new T{Prop=v}` 先构造后赋 Prop ⇒ 配置影响构造期行为的一律用属性（BenchAtoms42.OnlyTab 实机事故）。

## 交付底线（⭐）
- 精美 = 层次≥3 + 六态反馈 + 对齐 + 呼吸(内边距≥令牌) + 准确(配色/数值/几何/自画四项一致)；反劣质 12 条。与预览不一样时 A/B/C 定责（A=我做错必修/B=引擎降级登记/C=预览缺失补），不许混。
- 交互成熟度 I0 只读→I2 起步（可切换/编辑+状态回写），交互路径不得低于 I2。I0 白名单：纯排版/纯装饰/数据驱动/纯动画。悬停三层：卡片层/演示区层（`Atom.Interactive` 显式声明）/本体层（HoverFrame）。
- 显式声明优于嗅探：交互性加显式标记，别扫源码猜（已 4 次误判；命中可能在 ①本件 ②helper ③组件内部）。
- 降级阶梯 D0 原样→D1 省略→D2 减条目→D3 收窄→D4 换档→D5 成对裁剪→D6 省略并登记；禁硬挤/静默截断/画到容器外/压行高。拖拽/可移动件不挂自动动画。
- 「只在 rect 内画自己那笔」的原子（PulseDot/Glyph/Swatch/PinBadge）不排版标签 ⇒ 标签「先取单元矩形→二次分行」，禁 `new Rect(prev.xMax+N,…)` 绝对定位。
- 签名匹配 ≠ 数据自洽：逐实参核对算术（PowerGrid 1820−2480≠「盈余」事故）；计数文案口径必须与字段实际累加点一致（footerHits 事故）。

## 规模与口径
- 原版 `Verse.Widgets` 179 成员；原版 UI 全可调面 ≈550。真缺口仅 G1 图标族 + G2 分组裁剪；半覆盖：Checkbox 三态/RadioButton（Segmented 取代）/Listing 族。已覆盖勿误判：滑条族/取色/InfoCard→DataCandAtoms.InfoCard。
- 「42 基础原子」= 演武场策展口径（A0 23 + A0+ 19，验证台 01–42 同号）；43–56=输入族补录；`RUIF_NUM` 是编号+计数双职责唯一来源（GOV-006）。排序键：族内主体类(0)→*Cand(1)→*Fmt(2)。
- 两份权威文档：`终极宝典v1.0-fixed.html`（全量 158 件，✅=设计完成度）；`原子组件演武场.html`（L0/L1 51 件，✅=演示完成度）。
- 验证台全库 109 件 · 18+ 页签：原子 42 · 输入 15(43–56,63) · 数据 19 · 图表 10(67–76) · 反馈 6(77–82) · 导航 8(83–88+108/109) · 工业与官方 9(89–97) · 桥接 + 独立面。`DemoWindow.MakeTabBenches()` 注册 IBench[]；BenchAtoms42 partial 按族分文件；新增族 = 新 partial 文件 + `Build()` 1 行 + `families.Add(f)`（漏了整族静默不显示）。`[XX-nn]` 由 `StdIds.gen.cs` 自动反查。
- 桥接族（PawnTable/Reorder/Hi 共 10 件）依赖非滚动坐标，`IBench.Scrollable=false` 独立面验收，无法卡片化。
- 42 件真名对照：SortHeaderCell→TableAtoms.HeaderCell；StatusGlyph→StatusAtoms.Glyph；ExpandCaret→StatusAtoms.Caret；AnchorFrame→Atoms.Anchor；LoadingSpinner→StatusAtoms.Spinner；FormattedNumber→TextFmt.FormatNumber。易错：`InteractAtoms.GripHandle` ≠ `ReorderAtoms.Grip`。完整表 `_demo/RUIF_Demo/README.md`。
- RUIF 原子绘制契约（反编译实测）：FillWipe 自铺底+实心条；CapacityBar 默认 Warn/Alive、超载 4 红竖块；ZoomIndicator 自带排版（≥174）；PointCostBadge 宽高即本体；BubbleTag 尾巴在下方 8px；ToolbarIcon 即 28×28 本体。

## 贴图 / 旋转路线（TexCache）
- 运行时贴图 = 渐变/热力/饼环/桑基唯一解：`TexFmt` 算像素 → `TexCache.TryGet/Publish` → 一次 DrawTexture；禁止绘制函数里 `new Texture2D`；文字不烘焙（Draw.Label）。`TexFmt.Line/Polyline` 距离场抗锯齿 + 单测已落（197 例）。
- ⛔ 滚动视图内飞散两机制：① `Widgets.DrawLine` 走 GL 矩阵与滚动平移互不感知 ⇒ 滚动内必错（13 处隐患）；② `Draw.Rotate` 高频 GUI.matrix 存读漂移。唯一双绕开 = 未旋转 `GUI.DrawTexture`；非滚动上下文两者可用。
- `TexCache.KeyOf(kind,a..e)` 位段键与 `KeySector(colorIndex,startDeg,sweepDeg,inner01,size)`（1° 量化）是新代码标准键构造；旧 `Key`/`KeyVersioned` 位段重叠勿用。环/饼走 `TexAtoms.Sector` 逐扇区（4 draw call）；`Industrial.Gauge` 已烘焙单贴图。旋转结案：仅 `StatusAtoms.Caret`（单次）保留 Draw.Rotate。

## 验证台与数据集成
- 实机验证 `_demo/RUIF_Demo/`：入口 ①游戏内底栏/F9（仅 UIRoot_Play）②标题界面 DemoMod 设置页。验证面方法不可命名 `Draw`（CS0119 遮蔽洪水）。
- `PawnTable` 桥：基类 `PawnTable`（行集合 getter 给）；原版 Def 名 Assign/Work/Restrict/Animals/Wildlife；不可嵌 RUIF 滚动视口（双滚动条）。
- `Industrial` 纯函数合成「语义色阶总览」卡（纯函数类合并总览）；`MachineState={Run,Idle,Fault,Refuel,Offline}`。Gauge 弧半径 `min(w×0.42,h−20)`；LedMatrix 仅 status≥3 描边；BottleneckRibbon 标签下留 18、段宽≤24 不画；MachineStateTimeline 行高 `max(8,h/n−4)`、标签占左 `min(72,w×0.3)`。
- 组件零数据、调用方喂数据 ⇒ 有实现的数据类组件不需要 DataBridge（曾 24 件误判阻断，实测仅 GeneCard/MiniMap 真无）。
- 原版可复用件：`Verse.ReorderableWidget`（Mod 零 Harmony）、`PawnTable+PawnColumnWorker`、`Widgets.DrawAtlas/DrawTextureFitted/ColorSelector`、`LudeonTK.UIScaling`、`Verse.UIHighlighter`。已落地桥：Px/TexFmt/TexCache/TexAtoms（失败降级 12 段折线）/ReorderFmt+ReorderAtoms（insertIndex 差一已修）/HiAtoms。
- 原版 UI 取证权威：`docs/03-API与引擎取证/原版UI代码标准（Spy取证）.md`；状态登记 `docs/02-组件图鉴/组件库全量目录.md`。官方中文语言包 tar 取真实术语，禁臆造。

## 自动化 / 落地工程
- 调度小时级 ⇒ 每次唤醒连推 4 单元（15 分钟/格），节拍 `W W A` 相位跨轮保持（记任务单 §4）。双文件驱动：任务单定「做什么」、进度账本定「怎么做」，冲突以任务单指针为准；A 单元产出必须「修好了什么+下一格首任务」；每轮末 git add 两份交接文件防断链。
- 需运行时数据的件登记 §4 阻断显示「—+原因」，不硬做。
- 恢复版 WBS：R-01~R-07 全 ✅ + BenchButtons R-09 抽检 ✅；R-08 阻断（#83 旋转探针/73·74 显示异常待用户回图）；R-09 = 每轮抽检长期循环。
- 预览落地走 `11-预览落地标准化规范.md`（LAND-*）：「1:1」=内容宽基准对齐非像素相等；先量后写；字号三档 10–12 Tiny/13–15 Small/18–22 Medium；`SetInitialSizeAndPosition()` 强制贴左贴底。
