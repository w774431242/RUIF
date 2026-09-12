# RUIF 对外 API 参考

> 版本 **v1.0.1** · 自动生成于 2026-09-12 15:58 · 生成器 `RUIF/_scripts/gen_api_reference.ps1`
> 断言口径：v1.0.0 起 API 冻结 —— 删改公共 API 须先 `[Obsolete]` 保留一个主版本。
>
> 类型名规则：`RUIF.*` 用短名；引擎类型用全限定名（`UnityEngine.Rect` / `Verse.GameFont` 等）。
> 消费方引用 `RUIF.dll` 后，同目录的 `RUIF.xml` 会提供 IDE 悬停文档（成员级摘要以源码注释为准）。

## 目录

- `RUIF.AlertPriority` - enum
- `RUIF.Anchor9` - enum
- `RUIF.Api` - static class
- `RUIF.Atoms` - static class
- `RUIF.Charts` - static class
- `RUIF.ChartsFmt` - static class
- `RUIF.Command` - class
- `RUIF.CommandPalette` - static class
- `RUIF.DataAtoms` - static class
- `RUIF.DataExAtoms` - static class
- `RUIF.DataExFmt` - static class
- `RUIF.DataFmt` - static class
- `RUIF.Draw` - static class
- `RUIF.Draw.AnchorScope` - struct
- `RUIF.Draw.ColorScope` - struct
- `RUIF.Draw.EnabledScope` - struct
- `RUIF.Draw.FontScope` - struct
- `RUIF.Draw.RotateScope` - struct
- `RUIF.FeedbackAtoms` - static class
- `RUIF.FeedbackAtomsB` - static class
- `RUIF.FeedbackFmt` - static class
- `RUIF.Grid` - struct
- `RUIF.HStack` - struct
- `RUIF.Industrial` - static class
- `RUIF.IndustrialFmt` - static class
- `RUIF.InputAtoms` - static class
- `RUIF.InputExAtoms` - static class
- `RUIF.InputExFmt` - static class
- `RUIF.InputFmt` - static class
- `RUIF.InputPlusAtoms` - static class
- `RUIF.InputPlusFmt` - static class
- `RUIF.InputState` - enum
- `RUIF.InteractAtoms` - static class
- `RUIF.InteractFmt` - static class
- `RUIF.Layout` - static class
- `RUIF.MachineState` - enum
- `RUIF.MiscAtoms` - static class
- `RUIF.MiscFmt` - static class
- `RUIF.NavAction` - enum
- `RUIF.NavAtoms` - static class
- `RUIF.NavAtomsB` - static class
- `RUIF.NavAtomsB.NavStack` - class
- `RUIF.NavAtomsB.RecentEntry` - struct
- `RUIF.NavFmt` - static class
- `RUIF.OfficialDataAtoms` - static class
- `RUIF.OfficialDataAtomsB` - static class
- `RUIF.OfficialDataAtomsC` - static class
- `RUIF.OfficialFmt` - static class
- `RUIF.PanelAtoms` - static class
- `RUIF.PanelFmt` - static class
- `RUIF.ProgressAtoms` - static class
- `RUIF.ProgressFmt` - static class
- `RUIF.QuickAmountKind` - enum
- `RUIF.RelationBand` - enum
- `RUIF.RichSpan` - struct
- `RUIF.RowDensity` - enum
- `RUIF.Shapes` - static class
- `RUIF.ShapesFmt` - static class
- `RUIF.SortState` - enum
- `RUIF.StatusAtoms` - static class
- `RUIF.StatusFmt` - static class
- `RUIF.StatusKind` - enum
- `RUIF.TableAtoms` - static class
- `RUIF.TableFmt` - static class
- `RUIF.TextAtoms` - static class
- `RUIF.TextFmt` - static class
- `RUIF.TimeBucket` - enum
- `RUIF.ToastLevel` - enum
- `RUIF.Tokens` - static class
- `RUIF.TreeNode` - class
- `RUIF.VisualAtoms` - static class
- `RUIF.VisualsFmt` - static class
- `RUIF.VStack` - struct

## `RUIF.AlertPriority`

`public enum AlertPriority`

```csharp
Low = 0
Normal = 1
Warn = 2
Urgent = 3
```

## `RUIF.Anchor9`

`public enum Anchor9`

> 九宫锚点（布局原子 用）。

```csharp
TopLeft = 0
TopCenter = 1
TopRight = 2
MidLeft = 3
Center = 4
MidRight = 5
BottomLeft = 6
BottomCenter = 7
BottomRight = 8
```

## `RUIF.Api`

`public static class Api`

> 消费 Mod 的探测门面（版本断言）。 ⚠️ 依赖语义：本类型位于 RUIF.dll 内，框架未安装时，任何引用本类型的代码路径会在 JIT 期抛 FileNotFoundException/TypeLoadException——不会走到你的 else 分支。 因此只有两种正确接入方式： ① 硬依赖（推荐）：About.xml 写 modDependencies，用本类型做版本门控； ② 软依赖：主路径只读你自己缓存的 bool，全部 RUIF 调用隔离到独立类型/方法， 探测用反射扫 AppDomain（不引用 RUIF 类型即可完成）。

```csharp
public const string VersionString = "1.0.1"
public static System.Version Version { get; }
public static bool IsAvailable(string minVersion = null)
```

## `RUIF.Atoms`

`public static class Atoms`

```csharp
public static UnityEngine.Rect Anchor(UnityEngine.Rect parent, Anchor9 a, UnityEngine.Vector2 size, float margin = 0)
public static UnityEngine.Rect AspectBox(UnityEngine.Rect outer, float ratio)
public static UnityEngine.Rect SectionFrame(UnityEngine.Rect outer, string title)
```

## `RUIF.Charts`

`public static class Charts`

> 图表原子（v1.1 T0+T1 批次，v1.0 API 冻结之上的纯增量）： 走势 / 柱状(V/H) / 堆叠柱 / 折线 / 面积 / 环形 / 饼 / 散点 / 热力网格，共 10 件。 铁律： ① 颜色只经令牌 / ，禁裸 new Color（例外白名单见 Tokens.cs 注释）； ② 不读 Mouse/Event——图表是纯输出组件，交互由调用方在 rect 上自行判定； ③ 零贴图：全部 Draw.Box / Widgets.DrawLine 直绘（与 Spinner 小方块近似同一先例）； ④ 刻度缓冲区框架内静态复用（tickBuf），每帧零堆分配。 已知取舍：AreaChart 为逐段垂直填充（阶梯近似），Donut 以旋转短弧段拼环—— 高保真平滑留待 v1.2 素材/算法件评估（对齐图鉴 T2 口径）。

```csharp
public static void AreaChart(UnityEngine.Rect r, float[] values, Nullable<Color> color = null)
public static void BarChartH(UnityEngine.Rect r, float[] values, string[] labels = null, Nullable<Color> color = null)
public static void BarChartV(UnityEngine.Rect r, float[] values, string[] labels = null, Nullable<Color> color = null)
public static void DonutChart(UnityEngine.Rect r, float[] values, float hole01 = 0.5, string centerLabel = null)
public static void HeatmapGrid(UnityEngine.Rect r, float[,] grid)
public static void LineChart(UnityEngine.Rect r, float[] values, Nullable<Color> color = null, bool dots = true, bool grid = true)
public static void PieChart(UnityEngine.Rect r, float[] values, string centerLabel = null)
public static void ScatterChart(UnityEngine.Rect r, UnityEngine.Vector2[] points, UnityEngine.Vector2 domain, UnityEngine.Vector2 range, Nullable<Color> color = null)
public static void Sparkline(UnityEngine.Rect r, float[] values, Nullable<Color> color = null, float thickness = 1.5, bool dots = false)
public static void StackedBarChartV(UnityEngine.Rect r, float[][] series)
```

## `RUIF.ChartsFmt`

`public static class ChartsFmt`

```csharp
public static void BarGeom(int i, int n, float w, float gap, out float barW, out float xOff)
public static UnityEngine.Color HeatColor(float t)
public static int HistogramBin(float v, float lo, float hi, int bins)
public static int NiceTicks(float lo, float hi, float[] outBuf, int target = 4)
public static float Scale(float v, float lo, float hi, float outLo, float outHi)
public static float SeriesMax(float[] values, float fallback = 1)
public static void SeriesRange(float[] values, float fallback, out float lo, out float hi)
public static float SweepDeg(float value, float total)
```

## `RUIF.Command`

`public class Command`

```csharp
public string Group
public string Label
public System.Action Run
```

## `RUIF.CommandPalette`

`public static class CommandPalette`

```csharp
public static Command Draw(UnityEngine.Rect rect, Command[] commands, ref string query, ref int sel, Command[] filtered, int[] scores)
public static int Filter(string query, Command[] source, Command[] dest, int[] scores)
```

## `RUIF.DataAtoms`

`public static class DataAtoms`

```csharp
public static void HealthRows(UnityEngine.Rect rect, string[] parts, string[] states, float[] severity)
public static void HeatCalendar(UnityEngine.Rect rect, float[] values, int perRow, float cell = 12, float gap = 3)
public static void NeedsPanel(UnityEngine.Rect rect, string[] names, float[] values)
public static void PawnCard(UnityEngine.Rect rect, string initial, string name, string title, float mood01, float health01, string status)
public static void RankList(UnityEngine.Rect rect, string[] names, float[] values, float maxValue)
public static void SkillBars(UnityEngine.Rect rect, string[] names, float[] levels, float maxLevel = 20, int columns = 2)
public static void StatTable(UnityEngine.Rect rect, string[] headers, string[][] rows, float[] shares, float rowH = 20)
public static void TimelinePro(UnityEngine.Rect rect, string[] dates, string[] texts, UnityEngine.Color[] kinds, float rowH = 22)
public static void TraitChips(UnityEngine.Rect rect, string[] traits, bool[] positive, float gap = 6)
```

## `RUIF.DataExAtoms`

`public static class DataExAtoms`

```csharp
public static void BuffRow(UnityEngine.Rect rect, string label, float value)
public static void CompareBars(UnityEngine.Rect rect, string leftLabel, string rightLabel, float a, float b, float max)
public static void CreatureCard(UnityEngine.Rect rect, string name, int training, bool bonded)
public static int DockHost(UnityEngine.Rect area, string title, ref bool collapsed, float edge = 24)
public static void FactionCard(UnityEngine.Rect rect, string name, float goodwill)
public static bool NodeGraph(UnityEngine.Rect canvas, string[] labels, int[] colOf, int[] rowOf, int[] from, int[] to, int[] state, ref int selected, float nodeW = 96, float nodeH = 30, float gap = 14)
public static void PowerGrid(UnityEngine.Rect rect, float produced, float consumed, float stored, float capacity)
public static bool ProfilePreset(UnityEngine.Rect rect, string[] names, ref int index, out bool save)
public static void QuestCard(UnityEngine.Rect rect, string title, int done, int total, string[] objectives)
public static void RoomStats(UnityEngine.Rect rect, string[] names, float[] metrics)
public static void StorageBreakdown(UnityEngine.Rect rect, string[] labels, float[] values, UnityEngine.Color[] tones)
public static void WeatherStrip(UnityEngine.Rect rect, string[] labels, int[] kinds)
```

## `RUIF.DataExFmt`

`public static class DataExFmt`

```csharp
public static int CompareWinner(float a, float b)
public static int DockZone(UnityEngine.Rect screen, float edge, UnityEngine.Vector2 pointer)
public static UnityEngine.Vector2 LinkMid(UnityEngine.Vector2 from, UnityEngine.Vector2 to)
public static float NetBalance(float produced, float consumed)
public static UnityEngine.Rect NodeRect(UnityEngine.Rect canvas, int col, int row, float nodeW, float nodeH, float gap)
public static int PresetIndex(string[] names, string name)
public static float Ratio(float done, float total)
public static UnityEngine.Color RelationTone(float g)
public static float Share(float value, float total)
public static void ShareRatios(float[] values, float[] ratios)
public static float Sum(float[] values)
public static UnityEngine.Color TrainingTone(int level)
public static UnityEngine.Color WeatherTone(int kind)
```

## `RUIF.DataFmt`

`public static class DataFmt`

```csharp
public static float Bar01(float value, float max)
public static int HeatLevel(float ratio01, int levels = 4)
public static string RankMedal(int rank)
public static UnityEngine.Rect RowRect(UnityEngine.Rect list, int index, float rowH, float gap = 0)
public static UnityEngine.Color SkillColor(float level)
```

## `RUIF.Draw`

`public static class Draw`

> L0 · 运行时适配层：所有会修改全局绘制状态的调用都必须经过这里。 设计要点： ① 作用域是 struct —— 零堆分配，IMGUI 每帧调用不产生 GC； ② 进入时保存旧值，Dispose 时恢复 —— 靠 C# 的 using 嵌套形成"栈"， 避免手写 try/finally 漏恢复导致染色泄漏到全游戏； ③ 内建 1.6 已知约束（DrawBoxSolid 会重置 GUI.color，故 tint 直接传参）。

```csharp
public static AnchorScope Anchor(UnityEngine.TextAnchor anchor)
public static void Atlas(UnityEngine.Rect rect, UnityEngine.Texture2D tex)
public static void Atlas(UnityEngine.Rect rect, UnityEngine.Texture2D tex, UnityEngine.Color tint)
public static void Border(UnityEngine.Rect rect, UnityEngine.Color color)
public static void Box(UnityEngine.Rect rect, UnityEngine.Color color)
public static ColorScope Color(UnityEngine.Color c)
public static EnabledScope Enabled(bool value)
public static FontScope Font(Verse.GameFont font)
public static void Label(UnityEngine.Rect rect, string text, Verse.GameFont font, UnityEngine.Color color, UnityEngine.TextAnchor anchor)
public static void LineH(float x, float y, float length, UnityEngine.Color color)
public static void LineV(float x, float y, float length, UnityEngine.Color color)
public static RotateScope Rotate(float angle, UnityEngine.Vector2 pivot)
public static UnityEngine.Color WithAlpha(UnityEngine.Color c, float alpha)
```

## `RUIF.Draw.AnchorScope`

`public struct AnchorScope`

```csharp
public void Dispose()
```

## `RUIF.Draw.ColorScope`

`public struct ColorScope`

```csharp
public void Dispose()
```

## `RUIF.Draw.EnabledScope`

`public struct EnabledScope`

```csharp
public void Dispose()
```

## `RUIF.Draw.FontScope`

`public struct FontScope`

```csharp
public void Dispose()
```

## `RUIF.Draw.RotateScope`

`public struct RotateScope`

```csharp
public void Dispose()
```

## `RUIF.FeedbackAtoms`

`public static class FeedbackAtoms`

```csharp
public static bool AlertBanner(UnityEngine.Rect rect, AlertPriority level, string msg, string actionLabel = null)
public static bool ConfirmDialog(UnityEngine.Rect rect, string title, string msg, bool danger, string cancelLabel = null, string okLabel = null)
public static bool EmptyState(UnityEngine.Rect rect, string title, string sub, string cta = null)
public static void ProgressOverlay(UnityEngine.Rect rect, string title, float progress01, string step)
public static void RichTooltip(UnityEngine.Rect rect, string title, string[] stats, string[] requirements)
public static void Toast(UnityEngine.Rect rect, string title, string sub, ToastLevel level, float alpha)
```

## `RUIF.FeedbackAtomsB`

`public static class FeedbackAtomsB`

```csharp
public static void ActionFeedback(UnityEngine.Rect rect, bool ok, float alpha)
public static void AlertBlinker(UnityEngine.Rect rect, float t, AlertPriority priority)
public static bool BatchConfirm(UnityEngine.Rect rect, string title, string summary, string cancelLabel = null, string okLabel = null)
public static bool CriticalOverlay(UnityEngine.Rect rect, string msg, string actionLabel)
public static bool InlineError(UnityEngine.Rect rect, string msg, string fixLabel = null)
public static bool NotificationEntry(UnityEngine.Rect rect, string text, string time, bool unread)
public static void StepProgressList(UnityEngine.Rect rect, string[] steps, int current)
public static void StickyNote(UnityEngine.Rect rect, string text, UnityEngine.Color tone)
public static bool UndoToast(UnityEngine.Rect rect, string action, float alpha)
```

## `RUIF.FeedbackFmt`

`public static class FeedbackFmt`

```csharp
public static string BatchSummary(string[] types, int[] counts)
public static float BlinkAlpha(float t, AlertPriority p)
public static int MergeHidden(int total, int visibleMax)
public static int MergeVisible(int total, int visibleMax)
public static string StepMark(int i, int current)
public static float ToastAlpha(float age, float ttl, float fade = 0.5)
```

## `RUIF.Grid`

`public struct Grid`

```csharp
public int Index { get; }
public UnityEngine.Rect Next()
```

## `RUIF.HStack`

`public struct HStack`

```csharp
public UnityEngine.Rect Rest { get; }
public float X { get; }
public UnityEngine.Rect Take(float width)
```

## `RUIF.Industrial`

`public static class Industrial`

```csharp
public static void BottleneckRibbon(UnityEngine.Rect r, string[] stageLabels, float[] stageTimes, Nullable<Color> color = null)
public static void Gauge(UnityEngine.Rect r, float v, float lo, float hi, string label = null, float warn01 = 0.6, float dead01 = 0.85)
public static float GaugeNeedleDeg(float v01)
public static UnityEngine.Color LedColor(int status)
public static void LedMatrix(UnityEngine.Rect r, int[] statuses, int cols)
public static void MachineStateTimeline(UnityEngine.Rect r, string[] labels, MachineState[][] rows, float[][] durations)
public static UnityEngine.Color StateColor(MachineState s)
public static UnityEngine.Color ZoneColor(int zone)
```

## `RUIF.IndustrialFmt`

`public static class IndustrialFmt`

```csharp
public static int BottleneckIndex(float[] stageTimes)
public static float GaugeNorm(float v, float lo, float hi)
public static int GaugeZone(float v01, float warn01, float dead01)
public static UnityEngine.Rect LedCell(UnityEngine.Rect r, int i, int cols, int rows)
public static int MergeRunLengths(int[] states, int[] outBuf)
public static float SegmentsTotal(float[] durations)
public static int Shares(float[] values, float[] outBuf)
```

## `RUIF.InputAtoms`

`public static class InputAtoms`

> 基础输入控件（S3-1）：分段选择、开关、步进器、滑条、搜索框、标签多选、键位显示、六态规范。 全部无状态——值/缓冲/时间戳均由调用方持有并传入（GATE-UI）。 已知官方要点：Widgets.ButtonText 没有 disabled 参数，禁用走 active:false； HorizontalSlider 是 ref 语义（不是返回 float）。

```csharp
public static bool HotkeyBind(UnityEngine.Rect rect, string label, string currentKey)
public static bool SearchField(UnityEngine.Rect rect, ref string query, ref string buffer, string placeholder, ref float lastInputTime, float delay = 0.2)
public static bool Segmented(UnityEngine.Rect rect, string[] options, ref int index)
public static bool SliderWithValue(UnityEngine.Rect rect, ref float value, float min, float max, float roundTo = -1, Func<float, string> format = null)
public static void StateSample(UnityEngine.Rect rect, string label, InputState state)
public static bool Stepper(UnityEngine.Rect rect, ref int value, int min, int max, int step = 1)
public static bool TagPicker(UnityEngine.Rect rect, string[] all, bool[] picked, float gap = 6)
public static bool ToggleSwitch(UnityEngine.Rect rect, ref bool on)
```

## `RUIF.InputExAtoms`

`public static class InputExAtoms`

```csharp
public static bool ColorPicker(UnityEngine.Rect rect, ref UnityEngine.Color color, ref float h, ref float s, ref float v)
public static bool CycleButton(UnityEngine.Rect rect, string label, string[] values, ref int index)
public static bool DoubleSlider(UnityEngine.Rect rect, ref float lo, ref float hi, float min, float max, float roundTo = -1)
public static void DragListRow(UnityEngine.Rect row, string label, bool dragging, bool dropTarget)
public static bool EditableLabel(UnityEngine.Rect rect, ref string text, ref bool editing, ref string buffer)
public static bool GripHit(UnityEngine.Rect row)
public static void NumberScrubber(UnityEngine.Rect rect, ref float value, float step, float min, float max, ref bool scrubbing, ref float startX, ref float startVal)
public static bool PriorityGrid(UnityEngine.Rect rect, string[] rowLabels, string[] colLabels, int[] values, out int hitIndex)
public static bool TimetableMatrix(UnityEngine.Rect rect, string[] rowLabels, int[] states, out int hitIndex)
```

## `RUIF.InputExFmt`

`public static class InputExFmt`

```csharp
public static int DropIndex(UnityEngine.Rect list, int count, float rowH, float pointerY)
public static string Hex(UnityEngine.Color c)
public static UnityEngine.Color HsvColor(float h, float s, float v, float alpha = 1)
public static int NextIndex(int i, int n)
public static int NextPriority(int v)
public static int NextTimetableState(int s)
public static void Reorder(string[] items, int from, int to)
public static void RgbToHsv(UnityEngine.Color c, out float h, out float s, out float v)
public static float ScrubValue(float startValue, float dx, float step, float min, float max)
```

## `RUIF.InputFmt`

`public static class InputFmt`

```csharp
public static bool DebounceReady(float now, float lastInputTime, float delay)
public static InputState Resolve(bool hovered, bool pressed, bool selected, bool enabled)
public static int SegmentIndex(UnityEngine.Rect rect, int count, float x)
public static UnityEngine.Rect SegmentRect(UnityEngine.Rect rect, int count, int index)
public static float Slider01(float value, float min, float max)
public static float SliderValue(float t, float min, float max, float roundTo)
public static int StepValue(int value, int delta, int min, int max)
```

## `RUIF.InputPlusAtoms`

`public static class InputPlusAtoms`

> B2+ 输入控件（S3 任务②）：快捷数量 / 刻度滑条 / 滚轮调值 / 数值输入 / 路径输入 / 原生下拉 / 列过滤 / 多选列表 / 队列编辑 / 涂抹网格 / 键位冲突提示 / 多行编辑。 无状态；所有缓冲与选中态由调用方持有。下拉走官方 FloatMenu / FloatMenuOption（核心 UI 族，已核实）。 ⚠️ MultiLineEditor 为 v1 简化版：按行拆分多个单行框（真多行编辑需自定义 TextEditor，留 v1.1）**

```csharp
public static bool FilterRow(UnityEngine.Rect rect, string[] colLabels, string[] filters)
public static bool InPlaceDropdown(UnityEngine.Rect rect, string label, string[] options, int currentIndex, Action<int> onPicked)
public static bool KeyConflictHint(UnityEngine.Rect rect, string[] boundKeys)
public static bool MultiLineEditor(UnityEngine.Rect rect, ref string text, ref string buffer, int maxLines = 4)
public static bool MultiSelectList(UnityEngine.Rect rect, string[] items, bool[] selected, ref int anchor, float rowH)
public static bool NumericField(UnityEngine.Rect rect, ref float value, float min, float max, ref string buffer)
public static bool PaintStrokeGrid(UnityEngine.Rect rect, int rows, int cols, int[] values, out int hitIndex)
public static bool PathField(UnityEngine.Rect rect, ref string path)
public static bool QueueEditor(UnityEngine.Rect rect, int[] queue, ref int count, string[] labels, float rowH)
public static bool QuickAmounts(UnityEngine.Rect rect, string label, ref float value, float max)
public static bool SliderTicks(UnityEngine.Rect rect, ref float value, float[] ticks, float min, float max)
public static bool WheelAdjust(UnityEngine.Rect rect, ref float value, float step, float min, float max, bool invert = false)
```

## `RUIF.InputPlusFmt`

`public static class InputPlusFmt`

```csharp
public static int ConflictCount(string[] keys)
public static int EnqueueInt(int[] queue, ref int count, int value)
public static bool HasConflict(string[] keys)
public static bool IsMax(float v, float max)
public static bool IsValidPath(string p)
public static void MoveToFrontInt(int[] queue, int count, int index)
public static string NormalizePath(string p)
public static float QuickAmount(float current, QuickAmountKind kind, float max)
public static bool RemoveAtInt(int[] queue, ref int count, int index)
public static void SelectRange(bool[] sel, int anchor, int index)
public static float SnapToTicks(float value, float[] ticks)
public static string[] SplitLines(string text, int maxLines)
public static bool ToggleAt(bool[] sel, int i)
public static bool TryParseClamp(string s, float min, float max, out float value)
public static float WheelStep(float value, float deltaY, float step, float min, float max)
```

## `RUIF.InputState`

`public enum InputState`

```csharp
Default = 0
Hover = 1
Pressed = 2
Selected = 3
Disabled = 4
Focused = 5
```

## `RUIF.InteractAtoms`

`public static class InteractAtoms`

> 交互原子（批4-2b，Mod Manager / 工坊实证）：拖拽把手、可见性眼、搜索命中高亮、工具图标钮。 无状态；命中返回由调用方处理。图标一律用短文本或几何绘制（规避 emoji 字形缺失）。

```csharp
public static bool EyeToggle(UnityEngine.Rect rect, bool visible, Nullable<Color> color = null)
public static bool GripHandle(UnityEngine.Rect rect, int rows = 3, int cols = 2, Nullable<Color> color = null)
public static void SearchHighlight(UnityEngine.Rect rect, string text, string query, Verse.GameFont font = Small, Nullable<Color> hitColor = null)
public static bool ToolbarIcon(UnityEngine.Rect rect, string glyph, bool active = false)
```

## `RUIF.InteractFmt`

`public static class InteractFmt`

```csharp
public static UnityEngine.Rect EyeRect(UnityEngine.Rect host, float size)
public static void GripDotSize(UnityEngine.Rect area, int rows, int cols, float gap, out float w, out float h)
public static int HighlightSegmentCount(string text, string query)
```

## `RUIF.Layout`

`public static class Layout`

> L4 · 布局编排（S1 内核）。 立场：即时模式 + 轻量布局助手——不做保留模式虚拟树，不做自动 diff。 布局对象是 struct，按帧创建、零堆分配；窗口只声明结构，坐标由这里计算。 用法示例： var col = Layout.Column(outer, gap: Tokens.Gap); Rect head = col.Take(30f); ...

```csharp
public static UnityEngine.Rect BeginScroll(UnityEngine.Rect outer, float contentHeight, ref UnityEngine.Vector2 scroll, bool showScrollbars = true)
public static VStack Column(UnityEngine.Rect outer, float gap = 0)
public static void EndScroll()
public static Grid Grid(UnityEngine.Rect outer, int cols, float cellHeight, float gap = 0)
public static UnityEngine.Rect Inset(UnityEngine.Rect outer, float padX, float padY)
public static HStack Row(UnityEngine.Rect outer, float gap = 0)
public static void Split(UnityEngine.Rect outer, float ratio, float gap, out UnityEngine.Rect left, out UnityEngine.Rect right)
```

## `RUIF.MachineState`

`public enum MachineState`

> 设备状态（工业管理语义，与 一一对应）。

```csharp
Run = 0
Idle = 1
Fault = 2
Refuel = 3
Offline = 4
```

## `RUIF.MiscAtoms`

`public static class MiscAtoms`

```csharp
public static void BubbleTag(UnityEngine.Rect bubble, string text, Nullable<Color> color = null)
public static void PointCostBadge(UnityEngine.Rect rect, int budget, int spent)
public static void ThinScrollBar(UnityEngine.Rect viewRect, float contentH, float scrollY, float width = 8)
```

## `RUIF.MiscFmt`

`public static class MiscFmt`

```csharp
public static UnityEngine.Rect BubbleTail(UnityEngine.Rect bubble, float size)
public static bool PointsOverdrawn(int budget, int spent)
public static int PointsRemaining(int budget, int spent)
public static float ScrollThumbHeight(float trackH, float viewH, float contentH)
public static float ScrollThumbY(float trackY, float trackH, float thumbH, float scroll01)
```

## `RUIF.NavAction`

`public enum NavAction`

```csharp
None = 0
Back = 1
Forward = 2
```

## `RUIF.NavAtoms`

`public static class NavAtoms`

```csharp
public static void AnchorDots(UnityEngine.Rect rect, string[] sections, ref int current)
public static void Breadcrumb(UnityEngine.Rect rect, string[] path, Action<int> onJump)
public static TreeNode CollapsibleTree(UnityEngine.Rect rect, TreeNode root)
public static void Pager(UnityEngine.Rect rect, int total, int pageSize, ref int page, int[] pageBuf = null)
public static void TabBar(UnityEngine.Rect rect, string[] tabs, ref int sel, ref float indicatorX)
```

## `RUIF.NavAtomsB`

`public static class NavAtomsB`

```csharp
public static bool FocusSearch(UnityEngine.Rect rect, ref string query)
public static int JumpList(UnityEngine.Rect rect, RecentEntry[] recent, int max = 8)
public static void LayerSwitch(UnityEngine.Rect rect, string[] layers, ref int sel)
public static NavAction NavBackBar(UnityEngine.Rect rect, NavStack stack, string currentLabel)
public static void SidebarCollapse(UnityEngine.Rect edge, ref bool open)
public static void TimeSpeedControl(UnityEngine.Rect rect, ref Verse.TimeSpeed speed)
```

## `RUIF.NavAtomsB.NavStack`

`public class NavStack`

```csharp
public bool CanBack { get; }
public bool CanForward { get; }
public string Current { get; }
public int Depth { get; }
public IReadOnlyList<string> Entries { get; }
public int Position { get; }
public string Back()
public void Clear()
public string Forward()
public string JumpTo(int index)
public void Push(string label)
```

## `RUIF.NavAtomsB.RecentEntry`

`public struct RecentEntry`

```csharp
public float AgeSec
public string Kind
public string Title
```

## `RUIF.NavFmt`

`public static class NavFmt`

```csharp
public const string Separator = "›"
public const int StackLimit = 32
public static float Approach(float current, float target, float smooth)
public static int ClampPage(int page, int totalPages)
public static int CountNodes(TreeNode node)
public static int FuzzyScore(string query, string target)
public static int LabelUnits(string label)
public static void PageList(int totalPages, int page, int[] buf, out int count)
public static string RelativeTime(float ageSec)
public static float TabWidth(string label, float unitWidth, float padPerTab)
public static TimeBucket TimeBucketOf(float ageSec)
public static int TotalPages(int total, int pageSize)
```

## `RUIF.OfficialDataAtoms`

`public static class OfficialDataAtoms`

```csharp
public static void HistoryFeed(UnityEngine.Rect rect, string[] dates, string[] texts, string[] kinds)
public static void RecordsTable(UnityEngine.Rect rect, string[] labels, string[] values)
public static void TaleCard(UnityEngine.Rect rect, string teller, string text, int times)
public static void WealthBreakdown(UnityEngine.Rect rect, string[] labels, float[] values, UnityEngine.Color[] tones)
```

## `RUIF.OfficialDataAtomsB`

`public static class OfficialDataAtomsB`

```csharp
public static void BillList(UnityEngine.Rect rect, string[] names, float[] done, float[] target, bool[] paused)
public static void LetterCard(UnityEngine.Rect rect, string title, string summary, string time, UnityEngine.Color tone)
public static void PsycastTree(UnityEngine.Rect rect, string[] names, int[] levels, bool[] unlocked)
public static void QuestBoard(UnityEngine.Rect rect, string[] titles, float[] progress, float[] daysLeft)
public static void TitleCard(UnityEngine.Rect rect, string name, string granted, string[] permits, bool[] ok)
```

## `RUIF.OfficialDataAtomsC`

`public static class OfficialDataAtomsC`

```csharp
public static void CaravanPanel(UnityEngine.Rect rect, string[] members, float load01, float foodDays, string eta)
public static void GeneChips(UnityEngine.Rect rect, string[] genes, int[] complexity, bool[] positive, float gap = 6)
public static void IdeoCard(UnityEngine.Rect rect, string name, string leader, int members, string[] precepts)
public static void MessageFeed(UnityEngine.Rect rect, string[] lines, string[] times)
public static void PolicyCard(UnityEngine.Rect rect, string name, string summary, int applied)
```

## `RUIF.OfficialFmt`

`public static class OfficialFmt`

```csharp
public static float BillRatio(float done, float target)
public static UnityEngine.Color CheckColor(bool ok)
public static string ComplexityLabel(int p)
public static string EtaLabel(float days)
public static string HexWithHash(UnityEngine.Color c)
public static RelationBand RelationBandOf(float g)
public static UnityEngine.Color RelationColor(float g)
public static string RelationLabel(float g)
public static float WealthTotal(float[] parts)
```

## `RUIF.PanelAtoms`

`public static class PanelAtoms`

> 面板原子（批4-2a，RimHUD 实证）：统计行 / 迷你档案条 / 警示闪边 / 分段头。 全部无状态；动画参数由调用方传入。 图标一律使用短文本（不用 emoji，规避原版字体字形缺失）。

```csharp
public static void FlashOverlay(UnityEngine.Rect rect, float t, Nullable<Color> color = null, float thickness = 2)
public static void MiniProfileChip(UnityEngine.Rect rect, string initial, string name, float mood01, float health01)
public static void SectionBar(UnityEngine.Rect rect, string title, string count, bool expanded)
public static void StatRow(UnityEngine.Rect rect, string label, string value, Nullable<float> bar01 = null, Nullable<Color> barColor = null)
```

## `RUIF.PanelFmt`

`public static class PanelFmt`

```csharp
public static float FlashAlpha(float t, float minAlpha = 0.45)
public static void RowSplit(UnityEngine.Rect row, float labelRatio, float gap, out UnityEngine.Rect label, out UnityEngine.Rect value)
```

## `RUIF.ProgressAtoms`

`public static class ProgressAtoms`

```csharp
public static void CapacityBar(UnityEngine.Rect rect, float used, float capacity, Nullable<Color> fill = null)
public static void FillWipe(UnityEngine.Rect rect, float progress01, Nullable<Color> color = null, int rows = 10, float slant = 0.35)
public static bool ZoomIndicator(UnityEngine.Rect rect, float zoom, float resetValue = 1)
```

## `RUIF.ProgressFmt`

`public static class ProgressFmt`

```csharp
public static float CapacityRatio(float used, float capacity)
public static float OverflowRatio(float used, float capacity)
public static float WipeWidth(float progress01, float width, int row, int rows, float slant = 0.35)
public static string ZoomLabel(float zoom)
```

## `RUIF.QuickAmountKind`

`public enum QuickAmountKind`

```csharp
One = 0
Ten = 1
Hundred = 2
Max = 3
```

## `RUIF.RelationBand`

`public enum RelationBand`

```csharp
Ally = 0
Friendly = 1
Neutral = 2
Hostile = 3
DeadHostile = 4
```

## `RUIF.RichSpan`

`public struct RichSpan`

```csharp
public readonly Nullable<Color> Color
public readonly Nullable<GameFont> Font
public readonly string Text
```

## `RUIF.RowDensity`

`public enum RowDensity`

```csharp
Compact = 0
Normal = 1
Cozy = 2
```

## `RUIF.Shapes`

`public static class Shapes`

```csharp
public static void CornerFrame(UnityEngine.Rect rect, UnityEngine.Color color, float armLen = 14, float thickness = 2)
public static void GradientRule(UnityEngine.Rect rect, UnityEngine.Color color, int segments = 12)
public static void InsetPanel(UnityEngine.Rect rect)
public static void RuleWithLabel(UnityEngine.Rect rect, string label, UnityEngine.Color color, float gap = 6)
public static void Watermark(UnityEngine.Rect rect, string mark, float alpha = 0.06)
```

## `RUIF.ShapesFmt`

`public static class ShapesFmt`

```csharp
public static float CaretAngle(float progress01)
public static float GradientAlpha(int i, int n)
public static void RuleSplit(float totalW, float labelW, float gap, out float leftW, out float labelX, out float rightW)
public static int SpinnerHead(int segments, float t)
```

## `RUIF.SortState`

`public enum SortState`

```csharp
None = 0
Asc = 1
Desc = 2
```

## `RUIF.StatusAtoms`

`public static class StatusAtoms`

> 状态原子（S2 第二批）：呼吸点、状态图形、加载指示器、展开箭头、滚动渐隐提示。 全部无状态；动画参数由调用方传入（t / progress01），便于单测与暂停。 纯计算已抽至 （单测面），本类只做绘制编排。

```csharp
public static void Caret(UnityEngine.Rect rect, float progress01, Nullable<Color> color = null)
public static void Caret(UnityEngine.Rect rect, bool expanded, Nullable<Color> color = null)
public static void Glyph(UnityEngine.Rect rect, StatusKind kind)
public static void PulseDot(UnityEngine.Rect rect, UnityEngine.Color color, float t, float minAlpha = 0.45)
public static void ScrollFadeHint(UnityEngine.Rect viewport, bool moreBelow)
public static void Spinner(UnityEngine.Rect rect, float t, Nullable<Color> color = null, int segments = 8, float radius = 8, float size = 3)
public static void Spinner(UnityEngine.Rect rect, float t, UnityEngine.Texture2D tex, float scale = 1, float speed = 240)
```

## `RUIF.StatusFmt`

`public static class StatusFmt`

> 状态原子纯计算（单测面）：不触 GUI、不读输入；颜色经令牌解析。 与 共文件（Fmt + Atoms 同置，工程惯例）。

```csharp
public static UnityEngine.Rect DotRect(UnityEngine.Rect rect)
public static void GlyphOf(StatusKind kind, out string glyph, out UnityEngine.Color color)
public static float PulseAlpha(float t, float minAlpha = 0.45)
public static float TrailAlpha(int i, int head, int segments)
```

## `RUIF.StatusKind`

`public enum StatusKind`

```csharp
Ok = 0
Info = 1
Warn = 2
Error = 3
Paused = 4
```

## `RUIF.TableAtoms`

`public static class TableAtoms`

```csharp
public static bool DensitySwitch(UnityEngine.Rect rect, ref RowDensity density)
public static void FrozenSplit(UnityEngine.Rect outer, float frozenWidth, out UnityEngine.Rect frozen, out UnityEngine.Rect scrollArea)
public static bool HeaderCell(UnityEngine.Rect rect, string label, SortState state)
public static bool ResizeHandle(UnityEngine.Rect area, float x)
public static void ValueCell(UnityEngine.Rect rect, float value, float reference)
```

## `RUIF.TableFmt`

`public static class TableFmt`

```csharp
public static UnityEngine.Color BandColor(float ratio01)
public static void ColumnSplit(UnityEngine.Rect row, float[] shares, float gap, float[] widths)
public static float FrozenContentOffsetX(float scrollX)
public static SortState NextSort(SortState s)
public static float ResizeWidth(float startWidth, float dx, float min, float max)
public static float RowHeight(RowDensity d)
public static float ValueRatio(float value, float reference)
```

## `RUIF.TextAtoms`

`public static class TextAtoms`

> 文本原子（S2 第一批）的绘制件。 约束：不持有状态；颜色/字体一律走令牌或调用方传参；绘制态经 作用域恢复。

```csharp
public static string DateFull(long absTicks, UnityEngine.Vector3 location)
public static float RichTextLine(UnityEngine.Rect rect, RichSpan[] spans, UnityEngine.TextAnchor anchor = MiddleLeft, Verse.GameFont defaultFont = Small)
public static void SubtitleLine(UnityEngine.Rect rect, string main, string sub)
public static void Typewriter(UnityEngine.Rect rect, string text, float progress01, Verse.GameFont font, UnityEngine.Color color, UnityEngine.TextAnchor anchor = MiddleLeft)
public static void UnitLabel(UnityEngine.Rect rect, string value, string unit, Verse.GameFont valueFont = Medium, Nullable<Color> color = null)
```

## `RUIF.TextFmt`

`public static class TextFmt`

> 纯文本格式化（无绘制依赖 → 可离线单测）。 规则确定、无隐藏状态；游戏内时间/地点相关格式走 的官方包装。

```csharp
public static string FormatNumber(double value)
public static int TypewriterChars(string text, float progress01)
```

## `RUIF.TimeBucket`

`public enum TimeBucket`

```csharp
JustNow = 0
Minutes = 1
Hours = 2
Days = 3
```

## `RUIF.ToastLevel`

`public enum ToastLevel`

```csharp
Ok = 0
Warn = 1
Danger = 2
```

## `RUIF.Tokens`

`public static class Tokens`

```csharp
public static readonly UnityEngine.Color Accent
public static readonly UnityEngine.Color Alive
public const float BlockGap = 12f
public static readonly UnityEngine.Color Border
public static readonly UnityEngine.Color BorderHair
public static readonly UnityEngine.Color BorderSoft
public static readonly UnityEngine.Color Card
public const float CardGap = 8f
public static readonly UnityEngine.Color CardHover
public const float CardPadX = 12f
public const float CardPadY = 8f
public static readonly UnityEngine.Color Dead
public static readonly UnityEngine.Color Dim
public const float FontBodyLineHeight = 22f
public const float Gap = 12f
public const float GridGap = 8f
public const float HeaderHeight = 56f
public static readonly UnityEngine.Color Info
public static readonly UnityEngine.Color Muted
public static readonly UnityEngine.Color Panel
public const float PanelPadding = 12f
public static readonly UnityEngine.Color PanelRaised
public const float ProgressbarH = 12f
public const float RuleHeight = 1f
public const float ScrollbarThickness = 18f
public const float SectionTitleHeight = 30f
public const float SidebarWidth = 208f
public const float SpaceLg = 24f
public const float SpaceMd = 16f
public const float SpaceSm = 12f
public const float SpaceXs = 8f
public const float SpaceXxs = 4f
public static readonly UnityEngine.Color Text
public static readonly UnityEngine.Color Warn
public static readonly UnityEngine.Color Window
public static UnityEngine.Color AccentSoft { get; }
public static Verse.GameFont FontBody { get; }
public static Verse.GameFont FontLabel { get; }
public static Verse.GameFont FontValue { get; }
public static UnityEngine.Color Series(int index)
```

## `RUIF.TreeNode`

`public class TreeNode`

```csharp
public List<TreeNode> Children
public bool Dead
public bool Expanded
public string Label
public object Payload
public string Tag
public TreeNode Add(TreeNode child)
```

## `RUIF.VisualAtoms`

`public static class VisualAtoms`

```csharp
public static void GradientBar(UnityEngine.Rect rect, UnityEngine.Color lo, UnityEngine.Color hi, int segments = 16)
public static void OverflowFade(UnityEngine.Rect rect, string text, UnityEngine.Color bgColor, Verse.GameFont font = Small, Nullable<Color> color = null, float fadeWidth = 24, int segments = 8)
public static void PinBadge(UnityEngine.Rect host, bool pinned, Nullable<Color> color = null, float size = 12, float margin = 2)
public static void Swatch(UnityEngine.Rect rect, UnityEngine.Color color, bool selected = false)
```

## `RUIF.VisualsFmt`

`public static class VisualsFmt`

```csharp
public static float FadeAlpha(int i, int n)
public static float LerpT(int i, int n)
public static UnityEngine.Rect PinRect(UnityEngine.Rect host, float size, float margin)
```

## `RUIF.VStack`

`public struct VStack`

```csharp
public UnityEngine.Rect Rest { get; }
public float Y { get; }
public UnityEngine.Rect Take(float height)
```

