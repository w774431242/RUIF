# RUIF · 组件验证台（开发用）

游戏内实机验证 RUIF 组件是否真的"有效"——像素、手感、性能都只能在游戏里看。
**不随框架发布**，`_demo/` 与 `_scripts/sync_ruif.ps1` 的 `RUIF` 目标互不干涉。

## 构建与同步

```powershell
# 1) 先构建框架（演示工程引用的是 Assemblies\RUIF.dll 成品）
dotnet build Source/RUIF.csproj -v quiet

# 2) 构建验证台
dotnet build _demo/RUIF_Demo/Source/RUIF_Demo.csproj -v quiet

# 3) 同步到游戏（同时更新 RUIF；游戏运行中会中止）
pwsh _scripts/sync_ruif.ps1
```

同步会写入 `Mods\RUIF_Demo`（只带 `About` / `Assemblies` / `Defs`，不带源码）。
游戏内需同时启用 **RUI Framework** 与 **RUIF · 组件验证台** 两个 Mod。

## 打开方式（两个入口）

| 入口 | 位置 | 可用范围 |
|---|---|---|
| ① 底栏按钮 / **F10** | 游戏内底栏「组件验证台」 | **仅游戏内**（地图视图 + 星球视图） |
| ② Mod 设置页 | 选项 → Mod 设置 → 「RUIF 组件验证台」→ 打开按钮 | **任意时刻，含标题界面** |

> ⚠️ **底栏与 F10 在标题界面不会出现**，这不是 bug：底栏由 `RimWorld.UIRoot_Play` 的
> `mainButtonsRoot.MainButtonsOnGUI()` 绘制、热键由同处 `HandleLowPriorityShortcuts()` 处理；
> 而标题界面是 `Verse.UIRoot_Entry`，它**不引用** `MainButtonsRoot` / `MainButtonDef`
> （只做 `DoMainMenu()` + `WindowStackOnGUI()`）。
>
> 顺带纠正一个常见误解：`validWithoutMap = true` 的真实含义是「**没有当前地图时也不禁用**」
> （星球视图、地图卸载态），**不是**「主菜单可用」。
>
> 开档前的入口因此只能走 Mod 设置页 —— 它基于 `Find.WindowStack`，而 `WindowStack` 在 Entry 场景同样可用。

## 验证面（22 个页签 · 2026-10-05 审计后）

**页签栏按可用宽度自动折行**（1–3 行，判定公式与 `TabDrawer.GetOverflowTabHeight` 同源）。
原实现是手写累加宽度，页签多到 18 个时末尾 3 个会被**窗口右边界静默裁掉**（实机截图已复现），
现已改用原版 `Verse.TabDrawer.DrawTabs(rect, tabs, rows, maxTabWidth)` 折行绘制 ——
命中检测与 `clickedAction` 均由它内部完成（`Verse.TabDrawer:142-161`），调用方不得再补一次。

**覆盖口径（可复核）**：框架侧 `*Atoms / Shapes / Charts / Industrial / WindowFrame / CommandPalette /
PawnTableHost` 共 **173 个公开绘制方法**（口径 = `Assemblies/RUIF.dll` 反编译后按类型提取 `public` 方法）。
审计方法：对每个方法名在 `_demo/RUIF_Demo/Source/*.cs` 做「方法名级」匹配（可捕捉 `host.Reset()`
这类实例调用）。审计结论：**16 件从未被任何面调用**，现已由「覆盖补全」面全部纳入 → **缺口归零**。

| 页签 | 验证什么 | 判据 |
|---|---|---|
| **原子 / 输入 / 数据 / 图表 / 反馈 / 导航 / 工业与官方 / 桥接**（8 个分类页签） | **逐件**验证 `原子组件演武场.html` 的 56 件原子 + 候选层组合范式（共 110 张卡），族序与件序同网页一致 | 每张卡 = 卡头（编号 + 名称 + 真实 API 签名）+ 描述 + 演示容器（规格见 `docs/02-组件图鉴/原子落地基准表.md`）；空分类给「待落地」占位卡（不是坏了） |
| **拖拽重排** | `ReorderCandAtoms.List`（桥原版 `ReorderableWidget`） | 应能听到起拖/放下音效、看到 2px 白色插入线与被拖行高亮；下方回显 `LandingIndex` / `WillChange` |
| **脉冲高亮** | `HiAtoms`（桥 `Verse.UIHighlighter`）两帧协议 | 点「重新登记高亮」后，目标控件在**下一帧**才闪光晕——这是原版判定条件，不是 bug |
| **殖民者表** | `PawnTableAtoms`（桥 `RimWorld.PawnTable`） | 可切 `Assign` / `Work` / `Restrict` / `Animals` 四个**原版** PawnTableDef；点表头应循环「降 → 升 → 清除」并响 `Tick_High/Tick_Low`；底部状态行回显列数/行数/当前排序 |
| **互动** | 按钮族 6 态 × 4 tone、`IconButton`、选择件三态、`RadioGroup`、`WindowFrame` 骨架、降级阶梯 D0/D1/D3/D6 | 禁用态量化差；`FocusRing` 外扩 2px 不糊边；窄宽件应走省略号/占位而非硬切 |
| **视觉扩展 / 导航版式 / 数据族 / 输入族 / 官方 / 反馈族**（6 个 L2 补全面） | 《标准规范组件预览》剩余族的原生 + 候选件 | 各段用 `SectionFrame` 分组；组件自画内容与预览一致（如 `SliderTicks` 自画当前值，调用方不得重复补文本） |
| **诊断** | 编译期类型断言（17 类）、`Prefs.UIScale`、像素对齐 A/B、令牌 16 色 | 上下两条线对比：上面是裸浮点坐标（非整数缩放下发虚），下面是 `Px.HLine` 吸附后（干净 1px） |
| **贴图管线** | `VisualAtoms.GradientBar`（16 段折线）vs `TexAtoms.GradientRect`（贴图）并排；`FadeRight` 真渐隐；`Heat` 热力格网；`Sector` 饼/环 | 分段版能看到 16 级台阶，贴图版连续；改「重算数据」版本号才重建像素（缓存键生效） |
| **组件画廊** | 族级展示：文本 / 状态 / 进度容量 / 布局 / 数据展示 / 面板与几何 / 导航与杂项约 30 个组件的组合用法 | 呼吸点应错相位、转圈应转、`FillWipe` 应斜向推进、`SkillBars` 应按等级变色 |
| **覆盖补全**（2026-10-05 新增） | 18 件「框架有、从未被调用」的原子：`OfficialDataAtomsB` 5 件（LetterCard/BillList/QuestBoard/TitleCard/PsycastTree）、`InputAtoms.StateSample`、`InputExAtoms.DragListRow`/`GripHit`、`InputPlusAtoms.PaintStrokeGrid`、`ButtonAtoms.FocusRing`、`FeedbackAtomsB.NotificationEntry`、`WindowFrame.DefaultCloseLabel`、`TableCandAtoms.PawnTableFrame`、`PawnTableAtoms.CurrentMapColonists`/`CycleSort`、`PawnTableHost.Ensure`、`TexAtoms.Canvas`/`FadeLeft` | 六态对照一行 6 格齐全；拖拽把手可按下（`GripHit` 回 true）；涂抹网格点格改档；通知条目点击由未读转已读；`Ensure` 成功/失败两态都有回显 |

### 「诊断 / 贴图管线 / 组件画廊」三面的历史

这三个面曾被移出页签栏，但 **Bench 类一直编译在包内** —— 问题在于 `DemoWindow` 里它们只被
`new` 出来当字段持有、**从不渲染**：既掩盖了覆盖缺口（6 件 `TexAtoms` 贴图原子长期不可达），
又是纯死负载。2026-10-05 审计把名字加回 `TabLabels` 并改由 `tabBenches` 持有，**代码零改动即恢复**。

## 56 件原子 ↔ 真实 API 对照表（01–42 见下，43–56 见文末）

> **43–56 是深挖补录的扩展输入族**（`InputAtoms` / `InputExAtoms` / `InputPlusAtoms` / `InputCandAtoms`），
> 与演武场同号。**每个 rect 的最小尺寸与外扩绘制按反编译实测给足**，依据见
> `docs/02-组件图鉴/扩展原子族_重复性检测.md` §4。

| # | 件 | API |
|---|---|---|
| 43 | Segmented | `InputAtoms.Segmented(Rect, string[], ref int) → bool` |
| 44 | ToggleSwitch | `InputAtoms.ToggleSwitch(Rect, ref bool) → bool`（track 固定 36×18，命中区是整个 rect） |
| 45 | Stepper | `InputAtoms.Stepper(Rect, ref int, int min, int max, int step) → bool` |
| 46 | SliderWithValue | `InputAtoms.SliderWithValue(Rect, ref float, min, max, roundTo, Func<float,string>) → bool` ⚠ **上扩 18 / 下扩 4** |
| 47 | HotkeyBind | `InputAtoms.HotkeyBind(Rect, string label, string currentKey) → bool` ⚠ 内部固定总宽 **254** |
| 48 | TagPicker | `InputAtoms.TagPicker(Rect, string[] all, bool[] picked, float gap) → bool`（不折行） |
| 49 | DoubleSlider | `InputExAtoms.DoubleSlider(Rect, ref lo, ref hi, min, max, roundTo) → bool` ⚠ 最小高 **54** |
| 50 | ColorPicker | `InputExAtoms.ColorPicker(Rect, ref Color, ref h, ref s, ref v) → bool` ⚠ 最小高 **83** |
| 51 | CycleButton | `InputExAtoms.CycleButton(Rect, label, values, ref index) → bool`（自带 hover 反馈） |
| 52 | EditableLabel | `InputExAtoms.EditableLabel(Rect, ref text, ref editing, ref buffer) → bool` |
| 53 | PriorityGrid | `InputExAtoms.PriorityGrid(Rect, rows, cols, int[] values, out int hitIndex) → bool`（高 = 18 + rows×rowH） |
| 54 | TimetableMatrix | `InputExAtoms.TimetableMatrix(Rect, rows, int[] states, out int hitIndex) → bool` |
| 55 | SliderTicks | `InputPlusAtoms.SliderTicks(Rect, ref float, float[] ticks, min, max) → bool` ⚠ 宽需 **≥178** |
| 56 | BatchSliders | `InputCandAtoms.BatchSliders(Rect, SliderCfg[], float[])`（**组合件**，不计入原子口径） |

### 重复性处置（同报告）

- **D1**：43 号改走 `InputAtoms.Segmented`，**本地同名 helper 已删除**（真重复）
- **D15**：本地 `KBar` 仅保留「无阈值语义的纯进度」用途；有阈值请用 `ProgressAtoms.CapacityBar`
- **D2/D4/D5/D9**：44 ↔ 34、55 ↔ 46、51 ↔ 43、56 ↔ 46 为**同族分工**，非重复

---

## 42 件原子 ↔ 真实 API 对照表

网页卡片名 → RUIF 实际类型与方法（签名均取自 `Assemblies/RUIF.xml` 反射快照，非凭记忆）。

| # | 网页卡片 | RUIF API |
|---|---|---|
| 01 | FormattedNumber | `TextFmt.FormatNumber(double) → string` |
| 02 | TimeStamp | `TextAtoms.DateFull(long, Vector3)` + `NavFmt.RelativeTime(float)` |
| 03 | UnitLabel | `TextAtoms.UnitLabel(Rect, string, string, GameFont, Color?)` |
| 04 | RichTextLine | `TextAtoms.RichTextLine(Rect, RichSpan[], TextAnchor, GameFont) → float`（`new RichSpan(text, Color?, GameFont?)`） |
| 05 | Typewriter | `TextAtoms.Typewriter(Rect, string, float progress01, GameFont, Color, TextAnchor)` + `TextFmt.TypewriterChars` |
| 06 | SubtitleLine | `TextAtoms.SubtitleLine(Rect, string main, string sub)` |
| 07 | SectionFrame | `Atoms.SectionFrame(Rect, string) → Rect`（**返回内容区**） |
| 08 | CornerFrame | `Shapes.CornerFrame(Rect, Color, float armLen, float thickness)` |
| 09 | GradientRule | `Shapes.GradientRule(Rect, Color, int segments)` |
| 10 | RuleWithLabel | `Shapes.RuleWithLabel(Rect, string, Color, float gap)` |
| 11 | Watermark | `Shapes.Watermark(Rect, string mark, float alpha)` |
| 12 | InsetPanel | `Shapes.InsetPanel(Rect)` |
| 13 | PulseDot | `StatusAtoms.PulseDot(Rect, Color, float t, float minAlpha)` |
| 14 | StatusGlyph | `StatusAtoms.Glyph(Rect, StatusKind)`（`Ok/Info/Warn/Error/Paused`） |
| 15 | LoadingSpinner | `StatusAtoms.Spinner(Rect, float t, Color?, int segments, float radius, float size)`（另有贴图重载） |
| 16 | ScrollFadeHint | `StatusAtoms.ScrollFadeHint(Rect viewport, bool moreBelow)` |
| 17 | ExpandCaret | `StatusAtoms.Caret(Rect, bool expanded, Color?)` / `Caret(Rect, float progress01, Color?)` |
| 18 | AspectBox | `Atoms.AspectBox(Rect outer, float ratio) → Rect` |
| 19 | AnchorFrame | `Atoms.Anchor(Rect parent, Anchor9, Vector2 size, float margin) → Rect` |
| 20 | Swatch | `VisualAtoms.Swatch(Rect, Color, bool selected)` |
| 21 | GradientBar | `VisualAtoms.GradientBar(Rect, Color lo, Color hi, int segments)` |
| 22 | OverflowFade | `VisualAtoms.OverflowFade(Rect, string, Color bgColor, GameFont, Color?, float fadeWidth, int segments)` |
| 23 | PinBadge | `VisualAtoms.PinBadge(Rect host, bool pinned, Color?, float size, float margin)` |
| 24 | SortHeaderCell | `TableAtoms.HeaderCell(Rect, string, SortState) → bool` + `TableFmt.NextSort(SortState)`（`None/Asc/Desc`） |
| 25 | ValueCell | `TableAtoms.ValueCell(Rect, float value, float reference)` |
| 26 | ColumnResize | `TableAtoms.ResizeHandle(Rect area, float x) → bool` + `TableFmt.ResizeWidth` |
| 27 | FrozenColumn | `TableAtoms.FrozenSplit(Rect, float frozenWidth, out Rect frozen, out Rect scrollArea)` |
| 28 | DensitySwitch | `TableAtoms.DensitySwitch(Rect, ref RowDensity) → bool` + `TableFmt.RowHeight(RowDensity)`（`Compact/Normal/Cozy`） |
| 29 | StatRow | `PanelAtoms.StatRow(Rect, string label, string value, float? bar01, Color?)` |
| 30 | MiniProfileChip | `PanelAtoms.MiniProfileChip(Rect, string initial, string name, float mood01, float health01)` |
| 31 | FlashOverlay | `PanelAtoms.FlashOverlay(Rect, float t, Color?, float thickness)` |
| 32 | SectionBar | `PanelAtoms.SectionBar(Rect, string title, string count, bool expanded)` |
| 33 | GripHandle | `InteractAtoms.GripHandle(Rect, int rows, int cols, Color?) → bool` |
| 34 | EyeToggle | `InteractAtoms.EyeToggle(Rect, bool visible, Color?) → bool` |
| 35 | SearchHighlight | `InteractAtoms.SearchHighlight(Rect, string text, string query, GameFont, Color?)` |
| 36 | ToolbarIconButton | `InteractAtoms.ToolbarIcon(Rect, string glyph, bool active) → bool` |
| 37 | FillWipe | `ProgressAtoms.FillWipe(Rect, float progress01, Color?, int rows, float slant)` |
| 38 | CapacityBar | `ProgressAtoms.CapacityBar(Rect, float used, float capacity, Color?)` |
| 39 | ZoomIndicator | `ProgressAtoms.ZoomIndicator(Rect, float zoom, float resetValue) → bool` |
| 40 | PointCostBadge | `MiscAtoms.PointCostBadge(Rect, int budget, int spent)`（**参数序是 budget, spent**） |
| 41 | BubbleTag | `MiscAtoms.BubbleTag(Rect bubble, string text, Color?)` |
| 42 | ThinScrollBar | `MiscAtoms.ThinScrollBar(Rect viewRect, float contentH, float scrollY, float width)` |

> 两点易错处已在表中标出：**07 的返回值就是内容区**（不要再套 `Inset`）；**40 的参数序是 (budget, spent)** 而非 (cost, budget)。

## 三条必须知道的实现前提（都在代码注释里）

1. **拖拽面与殖民者表刻意不放进滚动视口**。
   - 拖拽：原版 `ReorderableWidget` 用 `UI.GUIToScreenPoint` 记录行矩形做插入位判定，
     而 `BeginScrollView` 会平移 `Event.current.mousePosition` 的坐标系——两者混用会让插入位整体偏移。
   - 表格：`PawnTable` 自带 `scrollPosition` 与滚动条，再套一层会出现双滚动条 + 滚轮争抢。
   
   `IBench.Scrollable` 就是为此存在的开关（这两面返回 `false`）。
2. **验证面方法叫 `Render` 而非 `Draw`**。实例方法 `Draw(Rect)` 会在类内**遮蔽静态类 `RUIF.Draw`**，
   导致 `Draw.Label(...)` 全报 CS0119（本工程真的踩过，62 个错误）。
3. **`Verse.TabRecord.Draw(rect)` 只画不点**。它只存 `clickedAction` 字段，**命中检测由调用方负责**
   （反编译产物里没有 `ButtonInvisible` / `MouseUp`）。漏了这一步 ⇒ 页签看起来正常但点不动
   （本工程踩过：表现是"只能停在默认页"）。正确写法：`Draw(tr)` 之后跟一句
   `if (Widgets.ButtonInvisible(tr)) tab.clickedAction();`
3. **热力/扇环的缓存键必须把数据版本编进去**（`TexCache.KeyVersioned`），否则会一直画旧图。

## 与框架的边界

- 本工程**只引用 `Assemblies\RUIF.dll`**（不是 ProjectReference），与第三方 Mod 的接入方式一致 ——
  因此"它能编译通过"本身就是「RUIF 公开 API 对消费方可用」的回归证据。
- `BenchDiagnostics` 里的 17 个 `typeof` 是**编译期断言**：任一类型从公开面消失，本工程立刻编不过。
