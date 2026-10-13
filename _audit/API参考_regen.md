# RUIF 对外 API 参考

> 版本 **v1.1.2** · 自动生成于 2026-09-16 13:40 · 生成器 `RUIF/_scripts/gen_api_reference.ps1`
> 断言口径：v1.0.0 起 API 冻结 —— 删改公共 API 须先 `[Obsolete]` 保留一个主版本。
>
> 类型名规则：`RUIF.*` 用短名；引擎类型用全限定名（`UnityEngine.Rect` / `Verse.GameFont` 等）。
> 消费方引用 `RUIF.dll` 后，同目录的 `RUIF.xml` 会提供 IDE 悬停文档（成员级摘要以源码注释为准）。

## 目录

- `RUIF.AlertPriority` - enum
- `RUIF.Anchor9` - enum
- `RUIF.Api` - static class
- `RUIF.Atoms` - static class
- `RUIF.ChartCandAtoms` - static class
- `RUIF.ChartCandAtoms.BoxStats` - struct
- `RUIF.ChartCandAtoms.BubblePt` - struct
- `RUIF.ChartCandAtoms.GanttBar` - struct
- `RUIF.ChartCandAtoms.OhlcBar` - struct
- `RUIF.ChartCandAtoms.RangePt` - struct
- `RUIF.ChartCandAtoms.SankeyLink` - struct
- `RUIF.ChartCandAtoms.SankeyNode` - struct
- `RUIF.ChartCandAtoms.WaterfallStep` - struct
- `RUIF.ChartCandFmt` - static class
- `RUIF.Charts` - static class
- `RUIF.ChartsFmt` - static class
- `RUIF.Command` - class
- `RUIF.CommandPalette` - static class
- `RUIF.DataAtoms` - static class
- `RUIF.DataCandAtoms` - static class
- `RUIF.DataCandAtoms.BillEntry` - struct
- `RUIF.DataCandAtoms.FilterNode` - struct
- `RUIF.DataCandAtoms.TradeRow` - struct
- `RUIF.DataCandFmt` - static class
- `RUIF.DataExAtoms` - static class
- `RUIF.DataExFmt` - static class
- `RUIF.DataFmt` - static class
- `RUIF.DomainCandAtoms` - static class
- `RUIF.DomainCandAtoms.BridgeSnapshot` - struct
- `RUIF.DomainCandAtoms.CircuitNode` - struct
- `RUIF.DomainCandAtoms.ProductionStage` - struct
- `RUIF.DomainCandFmt` - static class
- `RUIF.Draw` - static class
- `RUIF.Draw.AnchorScope` - struct
- `RUIF.Draw.ColorScope` - struct
- `RUIF.Draw.EnabledScope` - struct
- `RUIF.Draw.FontScope` - struct
- `RUIF.Draw.RotateScope` - struct
- `RUIF.FeedbackAtoms` - static class
- `RUIF.FeedbackAtomsB` - static class
- `RUIF.FeedbackCandAtoms` - static class
- `RUIF.FeedbackCandAtoms.AlertEntry` - struct
- `RUIF.FeedbackCandFmt` - static class
- `RUIF.FeedbackFmt` - static class
- `RUIF.Grid` - struct
- `RUIF.HiAtoms` - static class
- `RUIF.HStack` - struct
- `RUIF.Industrial` - static class
- `RUIF.IndustrialFmt` - static class
- `RUIF.InputAtoms` - static class
- `RUIF.InputCandAtoms` - static class
- `RUIF.InputCandAtoms.SliderCfg` - struct
- `RUIF.InputCandFmt` - static class
- `RUIF.InputExAtoms` - static class
- `RUIF.InputExFmt` - static class
- `RUIF.InputFmt` - static class
- `RUIF.InputPlusAtoms` - static class
- `RUIF.InputPlusFmt` - static class
- `RUIF.InputState` - enum
- `RUIF.InteractAtoms` - static class
- `RUIF.InteractFmt` - static class
- `RUIF.Layout` - static class
- `RUIF.LayoutCandAtoms` - static class
- `RUIF.MachineState` - enum
- `RUIF.Measure` - static class
- `RUIF.MiscAtoms` - static class
- `RUIF.MiscFmt` - static class
- `RUIF.NavAction` - enum
- `RUIF.NavAtoms` - static class
- `RUIF.NavAtomsB` - static class
- `RUIF.NavAtomsB.NavStack` - class
- `RUIF.NavAtomsB.RecentEntry` - struct
- `RUIF.NavCandAtoms` - static class
- `RUIF.NavCandFmt` - static class
- `RUIF.NavFmt` - static class
- `RUIF.OfficialDataAtoms` - static class
- `RUIF.OfficialDataAtomsB` - static class
- `RUIF.OfficialDataAtomsC` - static class
- `RUIF.OfficialFmt` - static class
- `RUIF.PanelAtoms` - static class
- `RUIF.PanelFmt` - static class
- `RUIF.PawnTableAtoms` - static class
- `RUIF.PawnTableHost` - class
- `RUIF.ProgressAtoms` - static class
- `RUIF.ProgressFmt` - static class
- `RUIF.Px` - static class
- `RUIF.QuickAmountKind` - enum
- `RUIF.RelationBand` - enum
- `RUIF.ReorderAtoms` - static class
- `RUIF.ReorderFmt` - static class
- `RUIF.ReorderRowDrawer` - class
- `RUIF.RichSpan` - struct
- `RUIF.RowDensity` - enum
- `RUIF.Shapes` - static class
- `RUIF.ShapesFmt` - static class
- `RUIF.SortState` - enum
- `RUIF.StatusAtoms` - static class
- `RUIF.StatusFmt` - static class
- `RUIF.StatusKind` - enum
- `RUIF.TableAtoms` - static class
- `RUIF.TableCandAtoms` - static class
- `RUIF.TableFmt` - static class
- `RUIF.TexAtoms` - static class
- `RUIF.TexCache` - static class
- `RUIF.TexFmt` - static class
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
public const string VersionString = "1.1.2"
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

## `RUIF.ChartCandAtoms`

`public static class ChartCandAtoms`

> ⑧ 图表候选组件（原 📋，官方实证支持，现落地）：直方图 / 雷达 / 甘特 / 桑基 / 箱线 / 漏斗 / 瀑布 / 气泡 / 阶梯线 / 区间带 / K线 / SPC 控制图 / Pareto 排列图。纯 presentational，数据由调用方传入； 任意方向连线用官方 Widgets.DrawLine，轴/基线用 Draw.LineH/LineV。 基础 10 件（Sparkline/Bar/Stacked/Line/Area/Donut/Pie/Scatter/HeatmapGrid）已在 Charts 落地，不重复。

```csharp
public static void BoxPlot(UnityEngine.Rect rect, BoxStats[] boxes)
public static void Bubble(UnityEngine.Rect rect, BubblePt[] pts)
public static void Funnel(UnityEngine.Rect rect, float[] stages, string[] labels)
public static void Gantt(UnityEngine.Rect rect, GanttBar[] bars)
public static void Histogram(UnityEngine.Rect rect, float[] values, int bins)
public static void Ohlc(UnityEngine.Rect rect, OhlcBar[] bars)
public static void Pareto(UnityEngine.Rect rect, float[] values, string[] labels)
public static void Radar(UnityEngine.Rect rect, float[][] series, string[] axes)
public static void RangeArea(UnityEngine.Rect rect, RangePt[] pts)
public static void Sankey(UnityEngine.Rect rect, SankeyNode[] nodes, SankeyLink[] links)
public static void Spc(UnityEngine.Rect rect, float[] samples, float mean, float ucl, float lcl)
public static void StepLine(UnityEngine.Rect rect, UnityEngine.Vector2[] pts)
public static void Waterfall(UnityEngine.Rect rect, WaterfallStep[] steps)
```

## `RUIF.ChartCandAtoms.BoxStats`

`public struct BoxStats`

```csharp
public string label
public float max
public float median
public float min
public float q1
public float q3
```

## `RUIF.ChartCandAtoms.BubblePt`

`public struct BubblePt`

```csharp
public float r
public int series
public float x
public float y
```

## `RUIF.ChartCandAtoms.GanttBar`

`public struct GanttBar`

```csharp
public float end
public string label
public int series
public float start
```

## `RUIF.ChartCandAtoms.OhlcBar`

`public struct OhlcBar`

```csharp
public float close
public float high
public string label
public float low
public float open
```

## `RUIF.ChartCandAtoms.RangePt`

`public struct RangePt`

```csharp
public float hi
public float lo
public float x
```

## `RUIF.ChartCandAtoms.SankeyLink`

`public struct SankeyLink`

```csharp
public int from
public int to
public float value
```

## `RUIF.ChartCandAtoms.SankeyNode`

`public struct SankeyNode`

```csharp
public string label
public int side
public float value
```

## `RUIF.ChartCandAtoms.WaterfallStep`

`public struct WaterfallStep`

```csharp
public float delta
public bool isTotal
public string label
```

## `RUIF.ChartCandFmt`

`public static class ChartCandFmt`

```csharp
public static UnityEngine.Color BandColor(float ratio01)
public static void Cumulative(float[] vals, float[] buf)
public static float Max(float[] v)
public static float Min(float[] v)
public static UnityEngine.Color SeriesColor(int i)
```

## `RUIF.Charts`

`public static class Charts`

> 图表原子（v1.1 T0+T1 批次，v1.0 API 冻结之上的纯增量）： 走势 / 柱状(V/H) / 堆叠柱 / 折线 / 面积 / 环形 / 饼 / 散点 / 热力网格，共 10 件。 铁律： ① 颜色只经令牌 / ，禁裸 new Color（例外白名单见 Tokens.cs 注释）； ② 不读 Mouse/Event——图表是纯输出组件，交互由调用方在 rect 上自行判定； ③ 零贴图：全部 Draw.Box / Widgets.DrawLine 直绘（与 Spinner 小方块近似同一先例）； ④ 刻度缓冲区框架内静态复用（tickBuf），每帧零堆分配。 已知取舍：AreaChart 为逐段垂直填充（阶梯近似），Donut 以旋转短弧段拼环—— 高保真平滑留待 v1.2 素材/算法件评估（对齐图鉴 T2 口径）。

```csharp
public static void AreaChart(UnityEngine.Rect r, float[] values, Nullable<Color> color = null)
public static void BarChartH(UnityEngine.Rect r, float[] values, string[] labels = null, Nullable<Color> color = null)
public static void BarChartV(UnityEngine.Rect r, float[] values, string[] labels = null, Nullable<Color> color = null)
public static void DonutChart(UnityEngine.Rect r, float[] values, float hole01 = 0.5, string centerLabel = null, UnityEngine.Color[] colors = null, float centerLabelY01 = 0.5, bool centerLabelAlignToHole = false)
public static void HeatmapGrid(UnityEngine.Rect r, float[,] grid)
public static void LineChart(UnityEngine.Rect r, float[] values, Nullable<Color> color = null, bool dots = true, bool grid = true)
public static void LineChart(UnityEngine.Rect r, float[][] series, UnityEngine.Color[] colors, bool dots = true, bool grid = true)
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

## `RUIF.DataCandAtoms`

`public static class DataCandAtoms`

> ⑤ 数据展示候选组件（原 📋，官方实证支持，现落地）：账单队列 / 物品过滤器树 / 资源读取面板 / 信息卡浮窗 / 存储内容面板 / 贸易面板。无状态；数据与选中态由调用方持有。 官方依据：BillStack·Dialog_BillConfig / Listing_TreeThingFilter / Listing_ResourceReadout / Dialog_InfoCard / ITab_Contents / Dialog_Trade（见《原版UI代码标准（Spy取证）》）。

```csharp
public static void BillList(UnityEngine.Rect rect, BillEntry[] bills)
public static void InfoCard(UnityEngine.Rect rect, string title, string subtitle, string[] lines)
public static void ResourceReadout(UnityEngine.Rect rect, string[] categories, int[] counts)
public static void StorageContents(UnityEngine.Rect rect, string[] labels, int[] counts)
public static void ThingFilterTree(UnityEngine.Rect rect, FilterNode[] nodes, bool[] expanded, bool[] checkStates)
public static void TradePanel(UnityEngine.Rect rect, TradeRow[] mine, TradeRow[] theirs)
```

## `RUIF.DataCandAtoms.BillEntry`

`public struct BillEntry`

```csharp
public int done
public string label
public bool paused
public bool priority
public int target
```

## `RUIF.DataCandAtoms.FilterNode`

`public struct FilterNode`

```csharp
public int depth
public bool hasChildren
public string label
```

## `RUIF.DataCandAtoms.TradeRow`

`public struct TradeRow`

```csharp
public int count
public string label
public UnityEngine.Color tone
```

## `RUIF.DataCandFmt`

`public static class DataCandFmt`

```csharp
public const float TreeOpW = 18f
public static float BillRatio(int done, int target)
public static UnityEngine.Color CountTone(int count)
public static UnityEngine.Color TradeTone(int delta)
public static UnityEngine.Rect TreeRow(UnityEngine.Rect list, int index, float rowH, int depth, float indent = 14)
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

## `RUIF.DomainCandAtoms`

`public static class DomainCandAtoms`

> ⑩ 工业域绑定候选组件（原 📋，官方实证支持，现落地）：DataBridge 数据桥 / GridMap 俯视状态格网 / CircuitTopology 电路拓扑 / ProductionFlow 生产链看板。皆为纯 presentational 视觉骨架（数字孪生方向）； 接真实域（PowerDomain/StockDomain/MapDomain/BillStack）属集成件，留作游戏实机验收。 官方依据：PowerNet · Map/Zone/TerrainDef · BillStack（见《工业管理方向扩展设计提案》）。

```csharp
public static void CircuitGraph(UnityEngine.Rect rect, CircuitNode[] nodes, int[] from, int[] to)
public static void DataBridgePanel(UnityEngine.Rect rect, BridgeSnapshot[] snaps)
public static void GridMap(UnityEngine.Rect rect, int[,] grid)
public static void ProductionFlow(UnityEngine.Rect rect, ProductionStage[] stages)
```

## `RUIF.DomainCandAtoms.BridgeSnapshot`

`public struct BridgeSnapshot`

```csharp
public string domain
public float[] series
public string stamp
```

## `RUIF.DomainCandAtoms.CircuitNode`

`public struct CircuitNode`

```csharp
public int kind
public string label
```

## `RUIF.DomainCandAtoms.ProductionStage`

`public struct ProductionStage`

```csharp
public string label
public float progress
```

## `RUIF.DomainCandFmt`

`public static class DomainCandFmt`

```csharp
public static UnityEngine.Color CellTone(float norm01)
public static UnityEngine.Color NodeKindColor(int kind)
public static UnityEngine.Color ProgressTone(float p01)
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
public static void EllipsisLabel(UnityEngine.Rect rect, string text, Verse.GameFont font, UnityEngine.Color color, UnityEngine.TextAnchor anchor)
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

> GUI.matrix 旋转作用域：进入时**在当前坐标系内绕 pivot 旋转**，退出时恢复原矩阵。 ⛔ 不再使用 GUIUtility.RotateAroundPivot（2026-09-12 根因修复）： 该 Unity API 把 pivot 当作「**应用旋转前的 GUI 空间**」坐标 —— 其内部实现是先把矩阵归零、 再合成 T(p) · R · T(-p) · M ⇒ **p 是在外层矩阵之前解释的**，而不是调用点所在的局部空间。 于是只要外层已有平移/裁剪作用域（窗口偏移、GUI.BeginClip、GUI.BeginGroup、 滚动视图），"按 rect 局部坐标传 pivot"就会**整体错位**：元素画在容器内、旋转却发生在容器外， 结果被甩到容器之外（实测事故：Industrial.Gauge 的 45×N 个 8×3 分区色块飞满整屏， 见 `PersonalChronicle` 侧 `制造系统中心_RUIF改造方案.md` §10）。正确语义：构造**局部空间**旋转矩阵并**后乘**当前矩阵 （M' = M · T(p) · R · T(-p)）—— 语义即"先把绘制点绕**局部 pivot** 旋转，再做外层变换"， 与调用点（Draw.Box / Draw.Label 的落点）严格同空间， 且与外层矩阵是平移、缩放还是旋转**无关**。调用点因此只需传 rect 局部坐标。

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

## `RUIF.FeedbackCandAtoms`

`public static class FeedbackCandAtoms`

> ⑥ 反馈候选组件（原 📋，官方实证支持，现落地）：浮动菜单 / 警报中心·通知托盘。无状态； 数据由调用方传入。官方依据：FloatMenu/FloatMenuOption/FloatMenuGrid（菜单浮层）、 AlertsReadout/Messages/LetterStack（警报/通知，仅数据形态） （见《原版UI代码标准（Spy取证）》）。

```csharp
public static void AlertTray(UnityEngine.Rect rect, AlertEntry[] alerts)
public static int FloatMenu(UnityEngine.Rect rect, string[] labels, ref int hover)
```

## `RUIF.FeedbackCandAtoms.AlertEntry`

`public struct AlertEntry`

```csharp
public string label
public int severity
```

## `RUIF.FeedbackCandFmt`

`public static class FeedbackCandFmt`

```csharp
public static UnityEngine.Color AlertSeverityTone(int level)
public static UnityEngine.Color FloatMenuRowTone(bool hover)
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

## `RUIF.HiAtoms`

`public static class HiAtoms`

> A0 · 新功能脉冲高亮原子族 —— 桥接 Verse.UIHighlighter（第二轮取证新增发现）。 跨帧协议（照抄，否则永不显示）：高亮是两帧协作的 ——本帧（通常是"打开某面板"的那一帧）调 登记一个 tag；下一帧绘制该 tag 对应控件时调 ， 引擎才会画脉冲光晕（判定条件是 Time.frameCount == 登记帧 + 1）。单独调 不会有任何效果 —— 这是"高亮不出现"的唯一根因。视觉参数（原版实测）：脉冲频率 1.2、振幅 0.7（Pulser.PulseBrightness）， 底色为白色透明叠加（Widgets.DrawTextHighlight，默认外扩 4px）。定位：用于"新手引导 / 版本新功能提示"，属可选装饰，缺失不影响功能。

```csharp
public static void Opportunity(UnityEngine.Rect rect, string tag)
public static void Tag(string tag)
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

## `RUIF.InputCandAtoms`

`public static class InputCandAtoms`

> ④ 输入候选组件（原 📋，官方实证支持，现落地）：批量滑杆配置窗 / 区域·允许区编辑器 / 键位绑定面板。无状态；值与选中态由调用方持有（ref / 数组原地回写）。官方依据： Listing_Standard.Slider/Widgets.HorizontalSlider · Area/AllowedArea/ RadioButtonLabeled · KeyBindingDef/Dialog_KeyBindings/KeyBindingSlot （见《原版UI代码标准（Spy取证）》）。

```csharp
public static void AreaEditor(UnityEngine.Rect rect, string[] areas, ref int selected)
public static void BatchSliders(UnityEngine.Rect rect, SliderCfg[] cfg, float[] values)
public static void KeyBindPanel(UnityEngine.Rect rect, string[] actions, string[] keys, bool[] listening)
```

## `RUIF.InputCandAtoms.SliderCfg`

`public struct SliderCfg`

```csharp
public string label
public float max
public float min
public string unit
```

## `RUIF.InputCandFmt`

`public static class InputCandFmt`

```csharp
public static UnityEngine.Color AreaRowTone(bool selected)
public static UnityEngine.Color KeybindSlotTone(bool listening)
public static float SliderValueFromPos(float mouseX, UnityEngine.Rect track, float min, float max)
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

## `RUIF.LayoutCandAtoms`

`public static class LayoutCandAtoms`

> ⑨ 版式候选组件（原 📋，现落地）：分层钻取孪生驾驶舱（Cockpit）外壳——KPI 顶条 + 左导航 + 主面板 + 底部钻取抽屉。纯 presentational 布局骨架；KPI→M3 抽屉→GridMap/CircuitTopology 的真实数据钻取属集成件。 官方依据：M3 抽屉钻取版式 / MainTabWindow 指挥台形态。

```csharp
public static void CockpitFrame(UnityEngine.Rect rect, string[] kpis, string[] panels, ref int activePanel)
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

## `RUIF.Measure`

`public static class Measure`

> L0 · 文本测量与行高（"参数空间"的量化入口）。 为什么必须有这一层：布局里最常见的两类事故 —— 文本被硬切、组件与标题错位 —— 根因都是行高靠手写魔数。中文在不同字号下的实际行高并不等于"数字 + 2"， 而原版字号只有三档（GameFont{Tiny,Small,Medium}，Text.SmallFontHeight = 22f）， 所以完全可以给出确定的行高表，而不是每个调用点自己估。测量一律走缓存：宽度用 GenUI.GetWidthCached（原版自带 string→尺寸缓存）。 ⛔ 不要在这里封装 Text.CalcHeight：原版没有它的缓存版， 提供包装等于邀请别人每帧调用 —— 这是性能事故（取证 §5 第 7 条：文本测量必须走缓存）。 需要精确高度时自行调用并**自己缓存**。与 的分工： 管"坐标落在哪个物理像素"， 本类管"一段文本需要多少逻辑高度/宽度"。

```csharp
public static string Clamp(string text, float width, Verse.GameFont font)
public static float ControlHeight(Verse.GameFont font)
public static bool Fits(string text, float width, Verse.GameFont font)
public static float RowHeight(Verse.GameFont font)
public static float RowHeight(Verse.GameFont left, Verse.GameFont right)
public static float Width(string text, Verse.GameFont font)
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

## `RUIF.NavCandAtoms`

`public static class NavCandAtoms`

> ⑦ 导航候选组件（原 📋，官方实证支持，现落地）：主标签页框架 / 检视页签组 / 页面向导 / 搜索面板 / 策略管理器。皆为纯 presentational 基础件（无状态，选中态由调用方 ref 持有）； 接原版窗口生命周期（MainTabWindow/ITab/Page）属集成件，留作游戏实机验收。 官方依据：MainTabWindow(15) · ITab_*(48) · Page_*(15) · Dialog_Search/MapSearch · Dialog_ManagePolicies（见《原版UI代码标准（Spy取证）》）。

```csharp
public static void InspectTabs(UnityEngine.Rect rect, string[] labels, ref int sel)
public static void MainTabStrip(UnityEngine.Rect rect, string[] labels, ref int sel)
public static void PolicyList(UnityEngine.Rect rect, string[] policies, ref int sel)
public static void SearchBox(UnityEngine.Rect rect, ref string text)
public static void WizardSteps(UnityEngine.Rect rect, string[] steps, ref int step)
```

## `RUIF.NavCandFmt`

`public static class NavCandFmt`

```csharp
public static UnityEngine.Color MainTabTone(bool active)
public static UnityEngine.Color PolicyRowTone(bool selected)
public static UnityEngine.Color WizardConnectorTone(int i, int step)
public static UnityEngine.Color WizardNumberTone(int i, int step)
public static UnityEngine.Color WizardStepTone(int i, int step)
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

## `RUIF.PawnTableAtoms`

`public static class PawnTableAtoms`

```csharp
public static IEnumerable<Pawn> CurrentMapColonists()
public static string CycleSort(string currentKey, bool currentDesc, string clickedKey, out bool descending)
public static bool PawnTableFrame(UnityEngine.Rect rect, PawnTableHost host, string tableDefName, Func<IEnumerable<Pawn>> pawnsGetter = null, bool showStatusLine = true)
```

## `RUIF.PawnTableHost`

`public class PawnTableHost`

> A0+ · 表格族的原版框架桥（第二轮取证 §1.2 ❌→✅ 的落地件）。 为什么是"桥"而不是"重写"：原版 RimWorld.PawnTable 是一个完整表格引擎 —— PawnTableOnGUI(position) 一行出表，内含：Def 驱动的列集合（def.columns 过滤 Worker.VisibleCurrently）、列宽协商（GetMinWidth/GetMaxWidth/GetOptimalWidth）、 逐列行高、内部滚动、以及内置三态排序（点表头 desc → asc → 清除，带 SoundDefOf.Tick_High/Tick_Low 音效与排序图标）。自研一遍只会更差。⚠️ 硬边界（实测）：PawnTable 的行类型硬编码为 Pawn （构造签名 PawnTable(PawnTableDef, Func<IEnumerable<Pawn>>, int, int)、 PawnsListForReading）。因此本原子只服务以 Pawn 为行的表；非 Pawn 数据表只能借它的 范式（Def 列 + Worker 单元 + 宽度协商 + 三态排序）另建，不能套用本类。状态归属：表实例由调用方的 持有（框架自身零状态）。 宿主内部只在"Def 变了 / 尺寸变了"时重建或 SetFixedSize， 避免每帧 SetDirty() 触发 RecacheIfDirty() 全表重算。⚠️ 不要嵌进 RUIF 的滚动视口：PawnTable 自带 scrollPosition 与内部 滚动条，再套一层 会出现双滚动条与滚轮争抢。

```csharp
public bool CreateFailed { get; }
public string LastError { get; set; }
public RimWorld.PawnTable Table { get; }
public string TableDefName { get; }
public RimWorld.PawnTable Ensure(string tableDefName, Func<IEnumerable<Pawn>> pawnsGetter, float w, float h)
public void MarkDirty()
public void Reset()
```

## `RUIF.ProgressAtoms`

`public static class ProgressAtoms`

```csharp
public static void CapacityBar(UnityEngine.Rect rect, float used, float capacity, Nullable<Color> fill = null)
public static void FillWipe(UnityEngine.Rect rect, float progress01, Nullable<Color> color = null, int rows = 10, float slant = 0.35)
public static void ProgressRing(UnityEngine.Rect rect, float progress01, Nullable<Color> track = null, Nullable<Color> fill = null, string centerLabel = null, float sweepDeg = 360, float thickness01 = 0.22)
public static bool ZoomIndicator(UnityEngine.Rect rect, float zoom, float resetValue = 1)
```

## `RUIF.ProgressFmt`

`public static class ProgressFmt`

```csharp
public static float CapacityRatio(float used, float capacity)
public static float OverflowRatio(float used, float capacity)
public static int RingFilledSegments(int segments, float progress01)
public static int RingSegments(float sweepDeg, float perSegDeg = 3)
public static float WipeWidth(float progress01, float width, int row, int rows, float slant = 0.35)
public static string ZoomLabel(float zoom)
```

## `RUIF.Px`

`public static class Px`

> L0 · 像素对齐（Pixel snap）。把坐标吸附到 UI 缩放网格，消除亚像素模糊。 为什么必须有这一层：RimWorld 的 UI 缩放（Prefs.UIScale）非整数时， 1px 描边/分隔线落在半像素上会被双线性插值糊成 2px 灰边 —— 观感立刻"不是原版"。 原版做法是让每个坐标过一遍 UIScaling.AdjustCoordToUIScaling* （实证：Verse.TabRecord.Draw 用它对页签中段做 xMin/xMax 对齐）。本类即该做法的收口。⛔ 不在此处调 GUIUtility.RotateAroundPivot 族：与旋转无关，本类只做坐标吸附。命名空间注意：底层 UIScaling 位于 LudeonTK（Dev 域）， 但**生产代码在用**（TabRecord.Draw），故此处直接引用并以此注释留档； 消费方只需依赖本类，不必自行 using LudeonTK。

```csharp
public static float Ceil(float coord)
public static float Floor(float coord)
public static UnityEngine.Vector2 Floor(UnityEngine.Vector2 v)
public static UnityEngine.Rect HLine(float x, float y, float length)
public static UnityEngine.Rect Snap(UnityEngine.Rect r)
public static float Span(float a, float b)
public static UnityEngine.Rect VLine(float x, float y, float length)
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

## `RUIF.ReorderAtoms`

`public static class ReorderAtoms`

> A0+ · 列表重排原子族 —— 桥接原版通用重排引擎 Verse.ReorderableWidget。 第二轮取证结论：原《原版 UI 代码标准》把「列表拖拽重排」判为 ❌（无通用原版） 是误判 —— ReorderableWidget 是完整成件，且框架钩子 （ReorderableWidgetOnGUI_Before/AfterWindowStack）已由 RimWorld.UIRoot_Play 每帧调用，Mod 无需 Harmony。白送的原版手感：5px 起拖阈值、起拖/放下音效（SoundDefOf.DragElement / DropElement）、2px 白色插入线、被拖行高亮。三条用法铁律（照抄原版 ColonistBar 的 cachedReorderableGroups 范式）：组 id 必须由调用方缓存（本类的 ref groupId）：NewGroup 在非 Repaint 帧 返回 -1，直接用它去登记会让后续 MouseDown 落在无效组上，拖拽永远起不来；每帧都要重新登记：框架在 ...AfterWindowStack 里 groups.Clear() / reorderables.Clear()，上一帧的登记不会保留；行数变化会中断拖拽：reorderables.Count != lastFrameReorderableCount 时引擎自动 StopDragging()，列表增删行时要有心理准备（或自行延后提交）。

```csharp
public static void Grip(UnityEngine.Rect rect, bool dragging)
public static bool List(UnityEngine.Rect area, int count, float rowHeight, ref int groupId, ReorderRowDrawer draw, Action<int, int> onReorder = null, bool highlightDragged = true)
```

## `RUIF.ReorderFmt`

`public static class ReorderFmt`

> A0+ · 重排的纯计算层（可离线单测，不碰 GUI）。 为什么单独抽出来：原版 ReorderableWidget 回调给的是 (fromIndex, insertIndex)，其中 insertIndex 是「元素移走之前」的插入下标 —— 直接 RemoveAt(from); Insert(to, item) 在 to > from 时会偏一位， 是这类交互最经典的一处差一错误。把它固化成纯函数 + 单测，比在每个调用点重推一遍可靠。回调语义（与原版实测一致）：ReorderableWidget 在 to == from 或 to == from + 1（插回原位）时不会触发回调，本层同样提前返回。

```csharp
public static int LandingIndex(int fromIndex, int insertIndex)
public static void Move<T>(IList<T> list, int fromIndex, int insertIndex)
public static int[] Moved(int[] order, int fromIndex, int insertIndex)
public static bool WillChange(int fromIndex, int insertIndex)
```

## `RUIF.ReorderRowDrawer`

`public class ReorderRowDrawer : System.MulticastDelegate`

> 行绘制委托： = 该行正被拖动（由原版引擎判定）。

```csharp
public System.IAsyncResult BeginInvoke(int index, UnityEngine.Rect row, bool dragging, System.AsyncCallback callback, object object)
public void EndInvoke(System.IAsyncResult result)
public void Invoke(int index, UnityEngine.Rect row, bool dragging)
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

## `RUIF.TableCandAtoms`

`public static class TableCandAtoms`

> 表格体系对接候选组件（原 📋，官方实证支持，现落地）：把 TableAtoms 接入 MainTabWindow_PawnTable（PawnTableDef 表格体系）的 UI 脚手架——可排序表头 + 冻结首列 + 密度切换的殖民民表框架。纯 presentational；真实 Pawn 数据绑定属集成件。 官方依据：MainTabWindow_PawnTable / PawnTableDef / PawnColumnWorker。

```csharp
public static void PawnTableFrame(UnityEngine.Rect rect, string[] headers, string[][] rows, ref int sortCol, SortState[] sorts, ref RowDensity density)
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

## `RUIF.TexAtoms`

`public static class TexAtoms`

> A0 · 贴图原子族：把「原版没有的能力」（真渐变 / 真渐隐 / 任意画布 / 饼环）用 算像素 + 上屏的方式补齐。 为什么走贴图而不是描线：原版 UI 侧无渐变 API、无多边形填充 API （第二轮取证：全类型表检索 Gradient 只命中 Verse.Noise.GradientPresets）。 而原版自己造贴图有先例（SolidColorMaterials.NewSolidColorTexture）。 贴图路线的附加好处是一次 draw call —— 热力图 365 格从 365 次 DrawBoxSolid 降到 1 次 GUI.DrawTexture。降级契约：贴图创建失败（非主线程 / 尺寸非法）时不静默 —— 渐变退化为 12 段折线，其余直接不画并已由 TexCache 记录一次错误。绘制前一律把 GUI.color 置白：颜色已烘进像素， 若沿用外层的染色状态会被二次相乘（这是"渐变发暗"的常见根因）。

```csharp
public static bool Canvas(UnityEngine.Rect rect, long key, int w, int h, UnityEngine.Color32[] pixels, string name = null)
public static void FadeLeft(UnityEngine.Rect rect, UnityEngine.Color bgColor, float width = 24)
public static void FadeRight(UnityEngine.Rect rect, UnityEngine.Color bgColor, float width = 24)
public static void GradientRect(UnityEngine.Rect rect, UnityEngine.Color lo, UnityEngine.Color hi, bool horizontal = true)
public static void Heat(UnityEngine.Rect rect, int version, float[,] grid, float lo, float hi, bool flipY = true)
public static void Sector(UnityEngine.Rect rect, long key, float startDeg, float sweepDeg, float inner01, UnityEngine.Color color, int size = 128)
```

## `RUIF.TexCache`

`public static class TexCache`

> L0 · 运行时贴图仓库：把 算出的像素缓冲变成可绘制的 Texture2D 并缓存。 三条纪律（原版实证）：① 必须主线程（原版 SolidColorMaterials.NewSolidColorTexture 自己就查 UnityData.IsInMainThread 并 Log.Error）；② 必须命名（原版写成 "SolidColorTex-" + color，便于资产审计）； ③ 必须缓存（原版 SimpleSolidColorMaterial 就是 static Dictionary 模式； 每帧 new Texture2D 会持续吃显存）。API 形状 = TryGet / Publish 两段式（而非传 Func）： 调用方先查缓存，未命中才计算像素 —— 这样每帧的热路径上既无闭包分配， 也不会白算一遍只在数据变化时才需要的像素。缓存键一律用 （ 把 kind/两色/尺寸压进一个 long），零字符串分配。⛔ 不用 MaterialAllocator：它是 internal，Mod 不可访问； 需要材质走 SolidColorMaterials.SimpleSolidColorMaterial（公开且自带缓存）。

```csharp
public const int MaxEntries = 128
public static void Clear()
public static long Key(int kind, UnityEngine.Color a, UnityEngine.Color b, int w, int h)
public static long KeyData(int kind, long dataHash, int a = 0, int b = 0, int c = 0)
public static long KeyOf(int kind, int a = 0, int b = 0, int c = 0, int d = 0, int e = 0)
public static long KeySector(UnityEngine.Color color, float startDeg, float sweepDeg, float inner01, int size)
public static long KeyVersioned(int kind, int version, int w, int h)
public static UnityEngine.Texture2D Publish(long key, int w, int h, UnityEngine.Color32[] pixels, string name = null)
public static bool TryGet(long key, out UnityEngine.Texture2D tex)
```

## `RUIF.TexFmt`

`public static class TexFmt`

> L0 · 贴图像素缓冲的纯计算层：只做数组运算，不创建任何 Unity 宿主对象 ⇒ 可离线单测（传进 w/h 与颜色，断言像素值），符合框架「Fmt = 纯计算」分层约定。 存在理由（第二轮取证结论）：原版 UI 侧没有渐变 API、没有多边形填充 API （全类型表检索 Gradient 仅命中 Verse.Noise.GradientPresets，属地形噪声）。 而原版自己造贴图是有先例的 —— SolidColorMaterials.NewSolidColorTexture 内部即 new Texture2D(2,2) + SetPixel×4 + Apply()。 因此「真渐变 / 真渐隐 / 热力图 / 饼环 / 桑基」的统一解是：**本层算像素 → TexCache 上屏**。坐标约定：Unity Texture2D 的像素 (0,0) 在左下角。 因此 的 top 参数写到缓冲的末行；上屏时若发现上下颠倒， 是此处语义被误用，而非绘制端问题。数学只用 System.Math：Mathf 的部分成员经 ECall 实现， 在非 Unity 宿主（离线单测）会抛异常（本框架 ECall 铁律同源）。

```csharp
public static UnityEngine.Color32[] AlphaFade(int w, UnityEngine.Color rgb, float from = 1, float to = 0)
public static UnityEngine.Color32 Blend(UnityEngine.Color32 dst, UnityEngine.Color src, float coverage01)
public static float Coverage(float d, float rx = 1, float ry = 1)
public static void FillDisc(UnityEngine.Color32[] buf, int w, int h, float cx, float cy, float rx, float ry, UnityEngine.Color c)
public static void FillRect(UnityEngine.Color32[] buf, int w, int h, int x0, int y0, int x1, int y1, UnityEngine.Color c)
public static void FillSector(UnityEngine.Color32[] buf, int w, int h, float cx, float cy, float rx, float ry, float startDeg, float sweepDeg, float inner01, UnityEngine.Color c)
public static UnityEngine.Color Heat(float t01)
public static UnityEngine.Color32[] HGradient(int w, UnityEngine.Color left, UnityEngine.Color right)
public static int Index(int x, int y, int w)
public static void Line(UnityEngine.Color32[] buf, int w, int h, float x0, float y0, float x1, float y1, UnityEngine.Color c, float thickness = 1.5)
public static void Polyline(UnityEngine.Color32[] buf, int w, int h, float[] xs, float[] ys, UnityEngine.Color c, float thickness = 1.5)
public static UnityEngine.Color Ramp(UnityEngine.Color lo, UnityEngine.Color hi, float t)
public static UnityEngine.Color32[] Solid(int w, int h, UnityEngine.Color fill)
public static UnityEngine.Color32[] VGradient(int h, UnityEngine.Color top, UnityEngine.Color bottom)
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

