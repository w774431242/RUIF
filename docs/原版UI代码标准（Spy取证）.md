# RUIF · 原版 UI 代码标准（Spy 取证）

> 日期：2026-09-10（第一轮：UI 标准底座；同日**第二轮：统计数据源 / 复杂图表 / 复杂组件族**）
> 工具：`ilspycmd 8.2.0.7535` + 反射（`Assembly.GetTypes()`，全程序集 **16157** 个类型）
> 目标程序集：`E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll`（RimWorld 1.6 实机）
> 目的：把 RimWorld 原版程序集中**支撑 RUIF 组件的原生 UI 代码**全部盘点、反编译、提取为标准签名与硬规则，作为后续制作原生 UI 时直接引用的「环世界程序集代码标准」。
> 与《API签名核实清单.md》的关系：那份是**组件粒度**的编码唯一依据（24 条核实）；本文是**程序集粒度**的标准底座（类地图 + 分层 API + 代码规则 + 常量表）。两者冲突时以更新日期新者为准。
>
> **第二轮补充（2026-09-10）**：新增 **§8 可统计的数据源**、**§9 复杂图表实现标准（原版曲线引擎 `SimpleCurveDrawer`）**、**§10 复杂组件族规模与范式**；
> 并修正三处旧结论：**§1.1 规模口径**（原 220/80/47/50 为「全名包含」匹配数，非顶层类型数）、**§4 图表支撑**（`❌ 原版无对应` → **`✅` 有完整曲线引擎**）、**§7 易错点**（新增 9 条）。

---

## 1. 程序集 UI 地图

全程序集共 **15202** 个类型。UI 体系分两块：`Verse` 基础设施（可被任何窗口复用）与 `RimWorld` 业务窗口族。

### 1.1 窗口族规模（RimWorld 侧）

| 前缀 | 数量 | 职责 |
|---|---|---|
| `RimWorld.Dialog_*` | **123** | 模态/非模态对话窗（详情卡、设置、谈判等） |
| `RimWorld.ITab_*` | **39**（+ 基类 `InspectTabBase` / `ITab`） | 检视面板页签（选中对象的子页） |
| `RimWorld.MainTabWindow_*` | **15**（+ 基类 `MainTabWindow`） | 底栏主页面（工作/研究/限制等） |
| `RimWorld.Page_*` | **11**（+ 基类 `Page`） | 全屏流程页（开局向导、菜单） |
| `RimWorld.Alert_*` | **132** | 警报 |
| `RimWorld.Designator_*` | **81** | 指令 |
| `RimWorld.Command_*` | **28** | 命令按钮 |

> ⚠️ **数字口径（第二轮取证修正）**：本表为**顶层类型**计数（反射 `IsPublic` 实测）。此前记录的 220/80/47/50 是「**全名包含**该串」的匹配数（含嵌套类与编译器生成类，如 `Dialog_Trade.<>c__DisplayClass`；实测 `Dialog_` 的全名包含计数为 **300**），**不是顶层类型数**。引用规模请以本表与 §10.1 为准。
> ⚠️ **命名空间实证**：`RimWorld.Dialog` 类型**不存在**（反编译 0 字节）——Dialog 族没有独立基类，全部直接继承 `Verse.Window`。基类归属：`Window/WindowStack`、`TabRecord`、`Listing 族`、`Widgets` 在 `Verse`；`MainTabWindow`、`ITab`（继承 `InspectTabBase`）、`Page` 在 `RimWorld`。

### 1.2 Verse UI 基础设施类型 → RUIF 层对应

| 原版类型 | 职责 | RUIF 对应层 |
|---|---|---|
| `Widgets`（179 静态成员） | 全部控件绘制 + 交互判定 | L2 原语的直接底层 |
| `GenUI` | Rect 数学、测量缓存、悬停色常量 | Layout / Draw 的底层 |
| `Text` + `GameFont` | 全局字体三态 + 测量 | Theme 字体令牌 |
| `Listing` / `Listing_Standard` / `Listing_Tree` 族 | 纵向流式游标布局 | `Layout.VStack` 原型 |
| `WidgetRow` | 横向流式布局（自动折行） | `Layout.HStack` 原型 |
| `Window` / `WindowStack` | 窗口生命周期 + 栈管理 | 页面预设 / 导航 |
| `MainTabWindow` / `ITab` / `Page` | 三类窗口骨架 | 页面预设 P1–P12 参照 |
| `TabRecord` | 页签条数据 + 绘制 | 导航 Segmented/Tabs |
| `FloatMenu` / `FloatMenuOption` | 上下文菜单 / `Widgets.Dropdown<T,P>` | InPlaceDropdown / 菜单 |
| `TooltipHandler` + `TipSignal` | 悬浮提示（延迟 0.45s） | 反馈 Tooltip |
| `Mouse` | 输入阻挡判定 + 悬停判定 | 交互门禁 |
| `ScrollPositioner` | 滚轮兴趣区滚动 | 滚轮扩展组件 |
| `SolidColorMaterials` / `TexUI` | 纯色材质/贴图工厂、公共贴图 | Draw 底层 |
| `GenText` / `GenColor` | 文本工具 / 颜色工具 | `*Fmt` 纯计算层参照 |

---

## 2. 取证方法与复现命令

```powershell
# ① 列全类型（15202 条，输出格式 "Class <FullTypeName>"）
ilspycmd 'E:\...\Assembly-CSharp.dll' -l c > types.txt

# ② 反编译单个类型（产物 <Namespace>.<Type>.decompiled.cs；0 字节 = 类型不存在）
ilspycmd $dll -t Verse.Widgets -o $env:TEMP\rw_spy

# ③ 从产物提取签名（规避 ANSI 高亮：用 Where-Object -match，不用 Select-String 高亮输出）
Get-Content $out\Verse.Widgets.decompiled.cs |
  Select-String '^\s*public static ' | ForEach-Object { $_.Line.Trim() }
```

铁律（沿用《API签名核实清单》）：**反编译产物 0 字节 = 类型不存在**（退出码不可靠）；产物为一次性取证，放 `%TEMP%\rw_spy`，不随 Mod 分发。

---

## 3. 分层 API 标准（实证签名）

以下签名全部直接摘自反编译产物（`%TEMP%\rw_spy\`），空格已归一化。

### 3.1 绘制基元 —— `Verse.Widgets` / `GenUI` / `SolidColorMaterials` / `TexUI`

**框 / 线 / 底**

```csharp
public static void DrawBoxSolid(Rect rect, Color color)          // 内部重置 GUI.color —— 唯一安全的纯色底
public static void DrawBoxSolidWithOutline(Rect rect, Color solidColor, Color outlineColor, int outlineThickness = 1)
public static void DrawBox(Rect rect, int thickness = 1, Texture2D lineTexture = null)
public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
public static void DrawLineHorizontal(float x, float y, float length, Color color = default)
public static void DrawLineVertical(float x, float y, float length)   // 注意：无 color 重载
public static void DrawHighlight(Rect rect) / DrawHighlight(Rect, float opacity)
public static void DrawHighlightIfMouseover(Rect rect)
public static void DrawHighlightSelected(Rect rect)
public static void DrawLightHighlight(Rect rect)
public static void DrawAltRect(Rect rect)                        // 斑马纹（RUIF ZebraRow 参照）
```

**文本标签**

```csharp
public static void Label(Rect rect, string label)
public static void Label(Rect rect, TaggedString label)
public static void Label(float x, ref float curY, float width, string text, TipSignal tip = default)  // 游标式
public static void Label(Rect rect, ref float y, string text, TipSignal tip = default)
public static void LabelEllipses(Rect rect, string label)        // 溢出省略
public static void LabelCacheHeight(ref Rect rect, string label, bool renderLabel = true, bool forceInvalidation = false)
public static void LabelScrollable(Rect rect, string label, ref Vector2 scrollbarPosition, ...)
public static void LabelWithIcon(Rect rect, string label, Texture2D labelIcon, float labelIconScale = 1f)
public static bool LabelFit(Rect rect, string label)             // 缩字适配
public static void ListSeparator(ref float curY, float width, string label)  // 分区标题（另有 RectDivider 重载）
```

**按钮（禁用语义 = `active:false`，无 disabled 参数）**

```csharp
public static bool ButtonText(Rect rect, string label, bool drawBackground = true, bool doMouseoverSound = true, bool active = true, TextAnchor? overrideTextAnchor = null)
public static bool ButtonText(Rect rect, string label, bool drawBackground, bool doMouseoverSound, Color textColor, bool active = true, TextAnchor? overrideTextAnchor = null)
public static DraggableResult ButtonTextDraggable(Rect rect, string label, ...)
public static bool ButtonTextSubtle(Rect rect, string label, float barPercent = 0f, float textLeftMargin = -1f, SoundDef mouseoverSound = null, Vector2 functionalSizeOffset = default, Color? labelColor = null, bool highlight = false)
public static bool CustomButtonText(ref Rect rect, string label, Color bgColor, Color textColor, Color borderColor, ...)
public static bool ButtonImage(Rect butRect, Texture2D tex, [Color baseColor, [Color mouseoverColor]], bool doMouseoverSound = true, string tooltip = null)
public static bool ButtonImageWithBG(Rect butRect, Texture2D image, Vector2? imageSize = null)
public static bool ButtonInvisible(Rect butRect, bool doMouseoverSound = true)
public static bool CloseButtonFor(Rect rectToClose) / BackButtonFor(Rect rectToBack)
public static bool InfoCardButton(float x, float y, Thing|Def|Faction|WorldObject|Hediff target)   // 11 个重载族
```

**复选 / 单选（有 disabled 参数）**

```csharp
public static void Checkbox(Vector2 topLeft, ref bool checkOn, float size = 24f, bool disabled = false, bool paintable = false, Texture2D texChecked = null, Texture2D texUnchecked = null)
public static void CheckboxLabeled(Rect rect, string label, ref bool checkOn, bool disabled = false, ...)
public static MultiCheckboxState CheckboxMulti(Rect rect, MultiCheckboxState state)   // On/Off/Partial 三态
public static bool RadioButton(Vector2 topLeft, bool chosen, bool disabled = false)
public static bool RadioButtonLabeled(Rect rect, string labelText, bool chosen, bool disabled = false)
```

**滑杆 / 进度**

```csharp
// 两代签名并存：
public static float HorizontalSlider(Rect rect, float value, float min, float max, bool middleAlignment = false, string label = null, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f)  // 返回值语义
public static float HorizontalSlider(Rect rect, ref float value, FloatRange range, string label = null, float roundTo = -1f)                                                                                                 // ref 语义
public static void FloatRange(Rect rect, int id, ref FloatRange range, float min = 0f, float max = 1f, string labelKey = null, ToStringStyle valueStyle = ToStringStyle.FloatTwo, float gap = 0f, GameFont sliderLabelFont = GameFont.Small, Color? sliderLabelColor = null, float roundTo = 0f)
public static void IntRange(Rect rect, int id, ref IntRange range, int min = 0, int max = 100, string labelKey = null, int minWidth = 0)
public static Rect FillableBar(Rect rect, float fillPercent)                                   // 返回填充 Rect 供叠字
public static Rect FillableBar(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder)
public static void FillableBarLabeled(Rect rect, float fillPercent, int labelWidth, string label)
public static void DraggableBar(Rect barRect, ..., ref bool draggingBar, float barValue, ref float targetValue, IEnumerable<float> bandPercentages = null, int increments = 20, float min = 0f, float max = 1f)
```

**滚动 / 图标 / 下拉 / 颜色**

```csharp
public static void BeginScrollView(Rect outRect, ref Vector2 scrollPosition, Rect viewRect, bool showScrollbars = true)  // 必与 EndScrollView 配对
public static void EndScrollView()
public static void AdjustRectsForScrollView(Rect parentRect, ref Rect outRect, ref Rect viewRect)
public static void BeginGroup(Rect rect) / EndGroup()            // Listing 底座，必配对
public static void DefIcon(Rect rect, Def def, ThingDef stuffDef = null, float scale = 1f, ..., float alpha = 1f)
public static void ThingIcon(Rect rect, Thing thing, float alpha = 1f, Rot4? rot = null, bool stackOfOne = false, float scale = 1f, bool grayscale = false)
public static void DrawTextureFitted(Rect outerRect, Texture tex, float scale, float alpha = 1f)   // 3 重载
public static void DrawTextureRotated(Vector2 center, Texture tex, float angle, float scale = 1f, Material material = null)
public static void Dropdown<Target, Payload>(Rect rect, Target target, Func<Target, Payload> getPayload, Func<Target, IEnumerable<DropdownMenuElement<Payload>>> menuGenerator, string buttonLabel = null, Texture2D buttonIcon = null, ..., bool paintable = false)
public static bool ColorSelector(Rect rect, ref Color color, List<Color> colors, out float height, Texture icon = null, int colorSize = 22, int colorPadding = 2, Action<Color, Rect> extraOnGUI = null)
public static void ColorTemperatureBar(Rect rect, ref Color color, ref bool dragging, float? colorValueOverride = null)
public static bool ColorTextfields(ref RectAggregator aggregator, ref Color color, ref string[] buffers, ...)
public static void MouseAttachedLabel(string label, float xOffset = 0f, float yOffset = 0f, Color? colorOverride = null)
public static void WidgetsOnGUI()                                // 框架级收口（悬停音效等）
```

**`GenUI`（Rect 数学 + 常量 + 测量缓存）**

```csharp
public static readonly Color MouseoverColor = new(0.3f, 0.7f, 0.9f);
public static readonly Color SubtleMouseoverColor = new(0.7f, 0.7f, 0.7f);
public static readonly Color FillableBar_Green = new(0.404f, 0.702f, 0.286f);
public static readonly Color FillableBar_Empty  = new(0.03f, 0.035f, 0.05f);
public static readonly Vector2 MaxWinSize = new(1010f, 754f);
public static Vector2 GetSizeCached(this string s)      // 文本测量缓存（每帧昂贵 → 必须走缓存）
public static float   GetWidthCached(this string s) / GetHeightCached(this string s)
public static Rect Rounded(this Rect r) / RoundedCeil(this Rect r)   // 亚像素抗锯齿
public static float DistFromRect(Rect r, Vector2 p)
public static void SetLabelAlign(TextAnchor a) / ResetLabelAlign()
public static float BackgroundDarkAlphaForText()
public static void DrawMouseAttachment(Texture iconTex, string text = "", ...)   // 鼠标跟随物
```

**`SolidColorMaterials`（纯色工厂）**：`SimpleSolidColorMaterial(Color, bool careAboutVertexColors)`、`NewSolidColorTexture(r,g,b,a)`、`NewSolidColorTexture(Color)`。
**`TexUI`（公共贴图库）**：`HighlightTex`、`HighlightSelectedTex`、`ArrowTex(Right/Left/Concave)`、`WinExpandWidget`、`GrayBg`、`TextBGBlack`、`GrayTextBG`、`FloatMenuOptionBG`、`FastFillTex/FastFillStyle`、`GrayscaleGUI`。

### 3.2 文本体系 —— `Text` / `GameFont`

```csharp
public enum GameFont { Tiny, Small, Medium }     // 仅三档；NumFonts=3；SmallFontHeight=22f
public static GameFont Font  { get; set; }       // 全局状态；Tiny 不受支持时自动降级 Small（TinyFontSupported：长任务/语言不支持/禁用/SteamDeck 均 false）
public static TextAnchor Anchor { get; set; }    // 全局状态
public static bool WordWrap { get; set; }
public static float LineHeight { get; }          // 随当前 Font
public static float LineHeightOf(GameFont font)
public static float CalcHeight(string text, float width)   // 绘制前测高
public static Vector2 CalcSize(string text)
public static string ClampTextWithEllipsis(Rect rect, string text)
public static GUIStyle CurFontStyle { get; }     // fontStyles[3]、textFieldStyles[3]、textAreaStyles[3]
```

> 规则：字体/对齐/换行是**进程级全局态**，绘制前设置、画完不还原是原版常态 —— RUIF 用 `Draw.Scope` 显式配对还原，是对原版惯例的收口升级。

### 3.3 布局体系 —— `Listing` 族（纵向流）/ `WidgetRow`（横向流）

**`Verse.Listing`（基类，全文实证）—— RUIF VStack 的原版原型：**

```csharp
public abstract class Listing {
    public float verticalSpacing = 2f;
    public const float ColumnSpacing = 17f;
    public const float DefaultGap = 12f;
    public const float DefaultIndent = 12f;
    protected Rect listingRect; protected float curY; protected float curX;
    public float CurHeight { get; }
    public void NewColumn()        // curY 归零、curX += ColumnWidth + 17f（记录 maxHeightColumnSeen）
    protected void NewColumnIfNeeded(float neededHeight)   // 超出 listingRect.height 自动换列
    public Rect GetRect(float height, float widthPct = 1f) // 取格 + curY += height（游标推进核心）
    public void Gap(float gapHeight = 12f)
    public void GapLine(float gapHeight = 12f)             // 40% alpha 分隔线（GUI.color 保存→乘→还原）
    public void Indent(float gapWidth = 12f) / Outdent(float gapWidth = 12f)
    public virtual void Begin(Rect rect)   // 内部 Widgets.BeginGroup(rect) —— 配对进入局部坐标系
    public virtual void End()              // Widgets.EndGroup()
}
```

**`Verse.Listing_Standard`（业务标准件）：** `ctor(GameFont)`；`Label/LabelDouble/SubLabel(widthPct)`；`RadioButton(string label, bool active, float tabIn = 0f, string tooltip = null, float? tooltipDelay = null[, bool disabled])`；`CheckboxLabeled(string label, ref bool checkOn, string tooltip, float height, float labelPct)`；`ButtonText(label, highlightTag, widthPct = 1f)`；`ButtonTextLabeled(Pct)`；`TextEntry(text, lineCount = 1)`；`TextFieldNumeric<T>(ref val, ref buffer, min = 0, max = 1e9)`；`Slider(val, min, max)`；`SliderLabeled(label, val, min, max, labelPct = 0.5f, tooltip)`；`IntAdjuster/IntSetter/IntEntry`；`BeginSection(height, sectionBorder = 4f, bottomBorder = 4f)` / `EndSection(listing)`。

**`Verse.WidgetRow`（横向流）—— RUIF HStack 的原版原型：**

```csharp
public class WidgetRow {
    public const float IconSize = 24f, DefaultGap = 4f, LabelGap = 2f, ButtonExtraSpace = 16f;
    public WidgetRow(float x, float y, UIDirection growDirection = UIDirection.RightThenUp, float maxWidth = 99999f, float gap = 4f)
    public void Gap(float width)
    public bool ButtonIcon(Texture2D tex, string tooltip = null, Color? mouseoverColor = null, Color? backgroundColor = null, ..., float overrideSize = -1f)
    public bool ButtonIconWithBG(Texture2D texture, float width = -1f, ...)
    public void ToggleableIcon(ref bool toggleable, Texture2D tex, string tooltip, ...)
    public Rect Icon(Texture tex, string tooltip = null, float contractedBy = 0f)
    public bool ButtonText(string label, string tooltip = null, bool drawBackground = true, bool doMouseoverSound = true, bool active = true, float? fixedWidth = null)
    public Rect Label(string text, float width = -1f, string tooltip = null, float height = -1f)
    public Rect LabelEllipses(string text, float width, ...)
    public Rect FillableBar(float width, float height, float fillPct, string label, Texture2D fillTex, Texture2D bgTex = null)
}
public enum UIDirection { LeftThenUp, LeftThenDown, RightThenUp, RightThenDown }
```

> 对照：原版纵向流（Listing）**自动换列**、横向流（WidgetRow）**自动折行**（maxWidth + growDirection）；RUIF VStack/HStack 取其游标思想但**不自动换列/折行**（Take/Rest 显式游标），Grid/Split/Inset 无原版对应（原版手写 Rect 切分）→ RUIF 自研补位。

### 3.4 窗口体系 —— `Window` / `WindowStack` / `MainTabWindow` / `ITab` / `Page` / `TabRecord`

**`Verse.Window`（全部窗口的基类）**

```csharp
public abstract class Window {
    // 行为开关（子类构造器里赋值）
    public WindowLayer layer = WindowLayer.Dialog;   // 枚举：GameUI/Dialog/SubSuper/Super
    public bool doCloseX, doCloseButton, closeOnAccept = true, closeOnCancel = true, closeOnClickedOutside;
    public bool forcePause, preventCameraMotion = true, doWindowBackground = true, onlyOneOfTypeAllowed = true;
    public bool absorbInputAroundWindow, resizeable, draggable, drawShadow = true, focusWhenOpened = true;
    public SoundDef soundAppear, soundClose, soundAmbient;
    public bool grayOutIfOtherDialogOpen, preventSave, openMenuOnCancel, onlyDrawInDevMode;
    // 常量
    public const float StandardMargin = 18f; public const float FooterRowHeight = 55f;
    public static readonly Vector2 CloseButSize = new(120f, 40f);      // 关闭/确认按钮标准尺寸
    public static readonly Vector2 QuickSearchSize = new(240f, 24f);
    public virtual Vector2 InitialSize => new(500f, 500f);             // 子类覆写
    protected virtual float Margin => 18f;
    // 生命周期（钩子序：PreOpen → [SetInitialSizeAndPosition] → WindowOnGUI/DoWindowContents/ExtraOnGUI/LateWindowOnGUI → OnCloseRequest → PreClose → PostClose）
    public abstract void DoWindowContents(Rect inRect);                // 唯一必须实现的绘制点
    public virtual void PreOpen() / PostOpen() / PreClose() / PostClose() / ExtraOnGUI() / WindowOnGUI();
    public virtual bool OnCloseRequest();
    protected virtual void SetInitialSizeAndPosition();
    public virtual void OnAcceptKeyPressed() / OnCancelKeyPressed();   // closeOnAccept/Cancel 时默认关闭
    public virtual void Notify_ResolutionChanged() / Notify_ClickOutsideWindow();
    public bool IsOpen => Find.WindowStack.IsOpen(this);
    public void Close(bool doCloseSound = true);
}
```

**`Verse.WindowStack`（全局唯一 `Find.WindowStack`）**：`Add(window)`；`ImmediateWindow(ID, rect, layer, doWindowFunc, doBackground, absorbInputAroundWindow, shadowAlpha, ...)`；`TryRemove(Type|Window)`；`IsOpen<T>()`；`WindowOfType<T>()`；`TryGetWindow<T>`；`GetWindowAt(Vector2)`；聚合开关 `WindowsForcePause / WindowsPreventCameraMotion / AnySearchWidgetFocused / NonImmediateDialogWindowOpen`。

**`RimWorld.MainTabWindow`（底栏页骨架，全文实证）**：`layer = GameUI`、`doCloseButton/doCloseX = false`、`preventCameraMotion = false`、`soundClose = SoundDefOf.TabClose`；`RequestedTabSize => (1010, 684)`；`Anchor => MainTabWindowAnchor.Left | Right`；`InitialSize` 自动 clamp 到 `screenHeight - 35`（35 = 底栏高）；`SetInitialSizeAndPosition` 贴底 + 左右锚定。

**`RimWorld.ITab`（检视页签基类，全文实证）**：继承 `InspectTabBase`；`SelObject / AllSelObjects / SelThing / SelPawn`（取自 `Find.Selector`）；`StillValid`（须挂当前 Inspect 主页签）；`CloseTab()` 播 `SoundDefOf.TabClose`。

**`RimWorld.Page`（全屏流程页基类）**：`prev / next` 链；`StandardSize => (1020, 764)`；`TitleAreaHeight = 45f`；`BottomButHeight = 38f`、`BottomButSize = (150, 38)`；骨架方法 `DrawPageTitle(rect)`、`GetMainRect(rect, extraTopSpace, ignoreTitle)`、`DoBottomButtons(rect, nextLabel, midLabel, midAct, showNext, doNextOnKeypress)`；`OnCancelKeyPressed → DoBack`。

**`Verse.TabRecord`（页签条）**：`label / clickedAction / selected | selectedGetter / labelColor`；`Draw(Rect)`；`GetTip()`。原版各 `MainTabWindow`（如研究页）用 `TabRecord[]` 手写页签条——RUIF Segmented/Tabs 的参照。

**`RimWorld.MainButtonDef`**：`workerClass = typeof(MainButtonWorker_ToggleTab)`、`tabWindowClass`、`order`、`defaultHotKey`、`iconPath`、`closesWorldView`、`validWithoutMap` —— 底栏按钮 = Def 驱动。

### 3.5 交互支撑 —— `Mouse` / `TooltipHandler` / `ScrollPositioner` / `FloatMenu`

```csharp
public static class Mouse {
    public static bool IsInputBlockedNow { get; }   // = 滚动区吞输入(mouseOverScrollViewStack.Peek()==false)
                                                    //   || WindowStack.MouseObscuredNow || !CurrentWindowGetsInput
    public static bool IsOver(Rect rect)            // rect.Contains(mousePosition) && !IsInputBlockedNow —— 悬停判定唯一正确姿势
}
public static class TooltipHandler {
    public static void TipRegion(Rect rect, TipSignal tip)
    public static void TipRegion(Rect rect, Func<string> textGetter, int uniqueId)   // 延迟求值 + 去重 id
    public static void TipRegionByKey(Rect rect, string key, params NamedArgument[])  // 翻译键
    public static void DoTooltipGUI()               // 全局统一收口绘制
}
public struct TipSignal {
    public const float DefaultDelay = 0.45f;        // 悬停 0.45s 才显示
    public string text; public Func<string> textGetter; public int uniqueId;
    public TooltipPriority priority; public float delay;
}
public class ScrollPositioner {                      // 滚轮"兴趣区"模式（视口外不滚走）
    public void Arm(bool armed = true); public void RegisterInterestRect(Rect rect);
    public void Scroll(ref Vector2 scrollPos, Vector2 outRectSize, bool scrollHorizontally = true, bool scrollVertically = true);
}
// FloatMenuOption 主构造（5 重载择一）：
public FloatMenuOption(string label, Action action, MenuOptionPriority priority = MenuOptionPriority.Default,
    Action<Rect> mouseoverGuiAction = null, Thing revalidateClickTarget = null, float extraPartWidth = 0f,
    Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null,
    bool playSelectionSound = true, int orderInPriority = 0)
public enum MultiCheckboxState { On, Off, Partial }
public enum DraggableResult { /* Widgets.Draggable 系列返回：拖拽语义三态 */ }
```

---

## 4. RUIF 组件 ↔ 原版支撑对照总表

图例：✅ 原版直接支撑 · ⚠️ 原版有模式参照（实现需自建） · ❌ 原版无对应（RUIF 自研承诺）

| RUIF 层 / 类别 | RUIF API | 原版支撑 | 级别 |
|---|---|---|---|
| **Core.Draw** | `Box` | `Widgets.DrawBoxSolid` | ✅ |
| | `Border` | `DrawBox(1px)` / `DrawBoxSolidWithOutline` | ✅ |
| | `LineH / LineV` | `DrawLineHorizontal / DrawLineVertical` | ✅ |
| | `Label(rect,text,font,color,anchor)` | `Widgets.Label` + `Text.Font/Anchor`（先设后画） | ✅ |
| | `WithAlpha(c,a)` | `new Color(r,g,b,a)`；原版惯例 `GUI.color * new Color(1,1,1,a)`（见 `Listing.GapLine`） | ✅ |
| | `Scope`（color/enabled/font/anchor/matrix 配对还原） | 原版无封装（手写保存-还原），Scope 为收口升级 | ⚠️ |
| **Layout** | `VStack`（Take/Rest） | `Listing`（curY 游标 + `GetRect` + 自动换列 `NewColumnIfNeeded`） | ⚠️ |
| | `HStack`（Take/Rest） | `WidgetRow`（RightThenUp + maxWidth 自动折行） | ⚠️ |
| | `Grid` | 无（原版用 Listing 多列 / 手写 for） | ❌ |
| | `Split` / `Inset` / `Atoms.AspectBox/Anchor9/SectionFrame` | 无（原版手写 Rect 切分；`BeginSection` 是 SectionFrame 的精神原型） | ❌ |
| | `BeginScroll/EndScroll` | `Widgets.BeginScrollView/EndScrollView`（ref Vector2 配对） | ✅ |
| **Theme** | `Palette` 16 令牌 | 原版无集中令牌——散落常量：`GenUI.MouseoverColor/FillableBar_Green`、`Widgets.InactiveColor/WindowBGFillColor`、`TexUI.Research* 色族` | ⚠️ |
| | 字体令牌 | `GameFont{Tiny,Small,Medium}` + `Text.Font` 全局态 + `TinyFontSupported` 自动降级 | ✅ |
| **原子·按钮** | 全部按钮族 | `ButtonText(±textColor/Subtle/Custom)` / `ButtonImage(WithBG)` / `ButtonInvisible` / `InfoCardButton` | ✅ |
| **原子·选择** | Checkbox/Switch/TriState | `Checkbox(Vector2,ref,size=24,disabled)` / `CheckboxLabeled` / `CheckboxMulti`（On/Off/Partial） | ✅ |
| | Segmented / 单选 | `RadioButton(Vector2,chosen,disabled)` / `RadioButtonLabeled` / `Listing_Standard.RadioButton(±disabled)` | ✅ |
| **原子·滑杆/进度** | SliderWithValue/DoubleSlider | `HorizontalSlider`（**ref 与返回值两代签名并存**）/ `FloatRange/IntRange` | ✅ |
| | ProgressBar/Meter | `FillableBar(±tex/bg/border)` / `FillableBarLabeled` / `DraggableBar` | ✅ |
| **原子·视觉** | ZebraRow | `DrawAltRect` | ✅ |
| | 悬停高亮 | `DrawHighlight(IfMouseover/Selected)/DrawLightHighlight` | ✅ |
| | LoadingSpinner | `DrawTextureRotated(center,tex,angle,scale)` | ✅ |
| | Swatch/GradientBar | `ColorSelector` / `SolidColorMaterials.NewSolidColorTexture`（16 段手绘） | ⚠️ |
| | OverflowFade/Ellipses | `LabelEllipses` / `Text.ClampTextWithEllipsis` / `GenUI.DrawTextWinterShadow` | ✅ |
| **输入** | NumericField/QuickAmounts | `Widgets.TextFieldNumeric(ref val, ref buffer, min, max)` / `Listing_Standard.IntSetter/IntAdjuster/IntEntry` | ✅ |
| | PathField/MultiLineEditor | `Listing_Standard.TextEntry(text, lineCount)` | ✅ |
| | InPlaceDropdown | `Widgets.Dropdown<Target,Payload>` / `FloatMenuOption(label, action, priority, extraPartOnGUI, ...)` | ✅ |
| | ColorPicker/RGB-HSV | `ColorSelector` / `ColorTemperatureBar` / `ColorTextfields` + `Widgets.HSVColorWheelTex` | ✅ |
| | WheelAdjust/NumberScrubber | `ScrollPositioner`（滚轮兴趣区模式） | ⚠️ |
| | DragListRow/GripHandle/Reorder | 无通用原版（原版 `DraggableResult` 系列是按钮级拖拽，非列表重排） | ❌ |
| | PriorityGrid/TimetableMatrix | 原版在 `ITab_*` 内手写表格（工作优先/作息表），无可复用组件 | ⚠️ |
| **数据展示** | PawnCard/SkillBars/NeedsPanel/HealthRows | 原版在 `MainTabWindow_Inspect` + `ITab_Pawn_*` 手写（`FillableBar` 为底），无组件化 | ⚠️ |
| | RecordsTable | 数据源 `RimWorld.Pawn_RecordsTracker`（API清单 #10） | ✅(数据) |
| | HeatCalendar/HeatmapGrid | 无 | ❌ |
| **导航** | Tabs/Segmented | `TabRecord(label, clickedAction, selected).Draw(rect)` + 各 MainTabWindow 手写页签条 | ⚠️ |
| | 页面骨架 P1–P12 | `Page`（Title/MainRect/DoBottomButtons）+ `MainTabWindow`（贴底 1010×684）+ `Window`（DoWindowContents） | ✅(模式) |
| | WindowStack 弹窗管理 | `Find.WindowStack`（Add/ImmediateWindow/TryRemove/IsOpen） | ✅ |
| **反馈** | Tooltip | `TooltipHandler.TipRegion(rect, TipSignal)`（延迟 0.45s、uniqueId 去重、TipRegionByKey 走翻译） | ✅ |
| | MessageFeed/LetterCard | 数据源 `Verse.Messages` / `Verse.LetterStack` | ✅(数据) |
| | 菜单浮层 | `FloatMenu` / `FloatMenuOption` / `FloatMenuGrid` | ✅ |
| **图表** | Sparkline/Line/MultiLine/Area/Scatter | **`Verse.SimpleCurveDrawer`**：原版**完整曲线引擎**（多序列叠加 / 图例 / 坐标刻度 / 悬停取值 / 事件标记 / 曲线↔屏幕坐标换算） | ✅ |
| | 进度条 / 单条填充 | `Widgets.FillableBar(±tex/bg/border)`、`FillableBarLabeled`、`GenDraw.DrawFillableBar` | ✅ |
| | 阈值反解（第几天破线） | `SimpleCurve.EvaluateInverted` | ✅ |
| | Bar / StackedBar | 无专用 API —— `Widgets.DrawBoxSolid` 自绘每柱 Rect | ⚠️ |
| | Donut / Pie / Heatmap / 桑基 / 雷达 | 无原生（自绘：`Widgets.DrawLine` / `GenDraw.DrawLineBetween`） | ❌ |
| | `ChartsFmt` 纯计算 | `SimpleCurve.Evaluate / MinY / MaxY / ClampToCurve` 即原版先例 | ✅(模式) |
| **工业** | QueueEditor/PaintStrokeGrid 等 | 无（Work Tab 类 mod 的领域） | ❌ |
| **工具层 `*Fmt`** | 纯计算静态类模式 | `GenText` / `GenColor` / `GenMath` 即原版先例 | ✅(模式) |

---

## 5. 原版代码标准 14 条（反编译实证提炼）

1. **布局即游标**：原版纵向布局 = `curY += height`（`Listing.GetRect`），横向 = `curX += width`（`WidgetRow`）。Begin/End 必配对（`BeginGroup/EndGroup`、`BeginScrollView/EndScrollView`、`BeginSection/EndSection`）。
2. **状态 ref 回写**：控件几乎全部 `ref` 语义（`Checkbox(ref bool)`、`TextFieldNumeric(ref val, ref buffer)`、`HorizontalSlider(ref value)`、`FloatRange(ref range)`）——即时模式所有权在调用方，组件无持久状态。**RUIF 红线同源**。
3. **全局绘制态显式存取**：`Text.Font/Anchor`、`GUI.color/enabled/matrix` 是全局的；原版惯例 = 用前设、用完还原（`Listing.GapLine` 是标准样板：存 `GUI.color` → 乘 alpha → 还原）。RUIF `Draw.Scope` 把它做成可 dispos 收口。
4. **禁用语义不统一是原版事实**：`ButtonText` 无 disabled（用 `active:false`）；`RadioButton/Checkbox` 有 `disabled` 参数。RUIF 统一为显式 disabled token，底层按原版语义分派。
5. **悬停判定门禁**：一律 `Mouse.IsOver(rect)`（内部含 `IsInputBlockedNow` 三重判定：滚动区吞输入 / 窗口遮挡 / 非输入窗口）。绕过它直接 `rect.Contains(mouse)` 会穿窗。
6. **Tooltip 模式**：`TipRegion(rect, TipSignal)` 注册、`DoTooltipGUI()` 全局收口、延迟 0.45s、`uniqueId` 去重、翻译用 `TipRegionByKey`。tip 内容可为 `Func<string>` 延迟求值。
7. **文本测量必须走缓存**：`GenUI.GetWidthCached/GetSizeCached` + `Widgets.ClearLabelCache`。每帧裸算 `CalcSize` 是性能事故。
8. **字号体系封闭三档**：`GameFont{Tiny,Small,Medium}`，`SmallFontHeight=22f`；Tiny 不可用时自动降 Small（`TinyFontSupported`）。自定义字号在原版不存在——主题字体只能映射到三档。
9. **窗口生命周期钩子序固定**：`PreOpen → SetInitialSizeAndPosition → [WindowOnGUI → DoWindowContents → ExtraOnGUI → LateWindowOnGUI] → OnCloseRequest → PreClose → PostClose`；输入开关用字段声明式（`doCloseX/closeOnCancel/absorbInputAroundWindow/forcePause/preventCameraMotion`）而非命令式。
10. **标准尺寸常量**（写死在基类）：`StandardMargin=18`、`CloseButSize=(120,40)`、`FooterRowHeight=55`、`Page.StandardSize=(1020,764)`、`MainTabWindow.RequestedTabSize=(1010,684)`（贴底、底栏高 35）、`GenUI.MaxWinSize=(1010,754)`、`Checkbox size=24`、`WidgetRow.IconSize=24/DefaultGap=4`、`Listing.DefaultGap=12/DefaultIndent=12/ColumnSpacing=17`、`TipSignal.DefaultDelay=0.45s`。RUIF 主题 spacing token 应与这些值对齐以获得原生手感。
11. **声音声明式**：按钮悬停音默认开（`doMouseoverSound=true`）；窗口出现/关闭音 = 字段赋值（`soundAppear/soundClose`）；页签关闭 = `SoundDefOf.TabClose`。
12. **图标绘制走统一入口**：`DefIcon/ThingIcon/GetIconFor`（含 style/材质/grayscale/alpha）；自由贴图用 `DrawTextureFitted`（适配）或 `DrawTextureRotated`（旋转）；纯色贴图用 `SolidColorMaterials` 工厂。
13. **拖拽语义用返回枚举**：`ButtonTextDraggable/ButtonInvisibleDraggable → DraggableResult`；进度拖拽 `DraggableBar(ref draggingBar, ref targetValue)`。
14. **公共贴图从 TexUI 取**：`HighlightTex/ArrowTex/GrayBg/FastFillTex/GrayscaleGUI` 等，不要自切等价贴图。

---

## 6. 核心常量速查表

| 常量 | 值 | 出处 |
|---|---|---|
| `Text.SmallFontHeight` | 22f | Verse.Text |
| `Window.StandardMargin` | 18f | Verse.Window |
| `Window.CloseButSize` | (120, 40) | Verse.Window |
| `Window.FooterRowHeight` | 55f | Verse.Window |
| `GenUI.MaxWinSize` | (1010, 754) | Verse.GenUI |
| `MainTabWindow.RequestedTabSize` | (1010, 684) | RimWorld.MainTabWindow |
| 底栏高度 | 35（`screenHeight - 35` 锚定） | MainTabWindow.SetInitialSizeAndPosition |
| `Page.StandardSize` | (1020, 764) | RimWorld.Page |
| `Page.TitleAreaHeight / BottomButHeight` | 45f / 38f | RimWorld.Page |
| `Listing.DefaultGap / DefaultIndent / ColumnSpacing` | 12f / 12f / 17f | Verse.Listing |
| `Listing.verticalSpacing` | 2f | Verse.Listing |
| `WidgetRow.IconSize / DefaultGap / LabelGap / ButtonExtraSpace` | 24f / 4f / 2f / 16f | Verse.WidgetRow |
| `Checkbox 默认尺寸` | 24f | Verse.Widgets |
| `TipSignal.DefaultDelay` | 0.45f | Verse.TipSignal |
| `GenUI.MouseoverColor` | (0.3, 0.7, 0.9) | Verse.GenUI |
| `GenUI.FillableBar_Green / _Empty` | (0.404, 0.702, 0.286) / (0.03, 0.035, 0.05) | Verse.GenUI |
| `Listing_Standard.BeginSection(sectionBorder, bottomBorder)` | 4f / 4f | Verse.Listing_Standard |
| `Alert.Width` | 154f | RimWorld.Alert（警报条图标块宽） |
| `Alert.InfoRectWidth` | 330f | RimWorld.Alert（警报展开信息宽） |
| `MainTabWindow_History.RequestedTabSize` | (1010, 640) | RimWorld.MainTabWindow_History |
| `HistoryAutoRecorderDef.recordTicksFrequency` | 60000（= 1 游戏天） | RimWorld.HistoryAutoRecorderDef |
| 历史曲线 X 轴单位 | 游戏天数（`tick / 60000f`） | RimWorld.HistoryAutoRecorderGroup.DrawGraph |
| `WealthWatcher.WealthFactor_Slave` | 0.75f | RimWorld.WealthWatcher |

---

## 7. 已核实不存在 / 易错点（Spy 阶段新发现）

| 项 | 结论 |
|---|---|
| `RimWorld.Dialog` | **不存在**（0 字节）——Dialog 族直接继承 `Verse.Window`，无中间基类 |
| `Verse.Page` | 不存在——`Page` 在 `RimWorld` 命名空间 |
| `HorizontalSlider` | **两代签名并存**：`ref + FloatRange` 与 `返回值 + min/max`，混用会编译错或语义错 |
| `DrawLineVertical` | 无带 `Color` 的重载（横向有）——竖线要颜色须先设 `GUI.color` |
| `RadioButton`（Widgets 直调） | 参数是 `Vector2 topLeft` 而非 Rect；要带标签走 `RadioButtonLabeled(Rect,...)` 或 `Listing_Standard.RadioButton`（后者才有 tooltip/disabled 完整参数） |
| `FillableBar` | 返回 `Rect`（填充区域）——叠百分比文字要继续用返回值 |
| `BeginGroup` | `Listing.Begin` 内部调用它——**手动包 Listing 时不要再套 BeginGroup**（双重坐标系） |
| `Mouse.IsOver` | 已含窗口/滚动区门禁——组件层禁止用 `Event.current.mousePosition + Contains` 裸判 |
| `SimpleCurveDrawerStyle` | 可传 `null`（等价默认样式）；想改任意一项都得**自己 new 一个**（没有「只改一项」的便捷入口） |
| `CurveMark` | 是 **struct**（值类型）：`new CurveMark(x, message, color)`，装进 `List<CurveMark>` 传给 `SimpleCurveDrawer` |
| `SimpleCurve` 的 X 单位 | 原版历史曲线用**游戏天数**（`j * recordTicksFrequency / 60000f`）——自建曲线必须沿用，否则刻度与原版对不上 |
| `ITab` | `FillTab()` 是 **protected abstract**（唯一必实现）；`DoTabGUI()` 是 public **非虚**（框架调用，不要覆写） |
| `GizmoGridDrawer.DrawGizmoGrid*` | 悬停窗口是 **`out Gizmo mouseoverGizmo`** 出参，不是返回值；`highlightFunc/lowlightFunc` 是每帧回调 |
| `RecordDefOf` / `HistoryEventDefOf` | 都在 **`RimWorld`** 命名空间（`Verse.*` 版本不存在），分别 **49** / **144** 个字段 |
| `LudeonTK.ComplexCurve` | 在 **`LudeonTK`**（Dev/Debug 命名空间）：是**编辑器**用的曲线（包 `AnimationCurve`、可增删键、`SmoothTangents`）；生产环境只读展示请用 `Verse.SimpleCurve` |
| 「图表无原版对应」 | **旧结论已被第二轮取证推翻**（见 §9）：曲线类图表有完整引擎 `SimpleCurveDrawer`；仅饼/热力/桑基/雷达需自绘 |

---

## 8. 可统计的数据源（第二轮取证）

> 本节回答「原版有哪些**现成的统计数据**可以拿来画」。全部条目均来自反射 + ilspycmd 实测签名。

### 8.1 历史自动记录器族 —— 开箱即用的时间序列

`RimWorld.HistoryAutoRecorderWorker*` 实测共 **11 个 public 子类**（即 11 个内置统计维度）：

| Worker 类 | 统计维度 |
|---|---|
| `HistoryAutoRecorderWorker_WealthTotal` | 总财富 |
| `HistoryAutoRecorderWorker_WealthItems` | 物品财富 |
| `HistoryAutoRecorderWorker_WealthBuildings` | 建筑财富 |
| `HistoryAutoRecorderWorker_WealthPawns` | 殖民者财富 |
| `HistoryAutoRecorderWorker_FreeColonists` | 自由殖民者数 |
| `HistoryAutoRecorderWorker_Prisoners` | 囚犯数 |
| `HistoryAutoRecorderWorker_ColonistMood` | 殖民者平均心情 |
| `HistoryAutoRecorderWorker_ThreatPoints` | 威胁点数 |
| `HistoryAutoRecorderWorker_Adaptation` | 适应度 |
| `HistoryAutoRecorderWorker_PopAdaptation` | 人口适应 |
| `HistoryAutoRecorderWorker_PopIntent` | 人口意图 |

`HistoryAutoRecorderDef : Def`（**Def 字段 = 消费 Mod 可配置项**）：

```csharp
public Type   workerClass;
public int    recordTicksFrequency = 60000;   // 采样间隔；60000 tick = 1 游戏天
public Color  graphColor = Color.green;       // 曲线颜色
public string graphLabelY;                    // Y 轴标签
public string valueFormat;                    // 数值格式（"F0" / "0%" …）
public HistoryAutoRecorderWorker Worker       // 惰性实例化
public static HistoryAutoRecorderDef Named(string defName)
```

`HistoryAutoRecorderGroup : IExposable`（**曲线容器**）：

```csharp
public HistoryAutoRecorderGroupDef def;
public List<HistoryAutoRecorder> recorders;
public float GetMaxDay();
public void  Tick();
public void  DrawGraph(Rect graphRect, Rect legendRect, FloatRange section, List<CurveMark> marks);  // ★ 一行出图
public void  AddOrRemoveHistoryRecorders();
```

### 8.2 记录系统 —— 49 个内置统计项

`RecordDef : Def`：

```csharp
public RecordType type;                              // Int / Float / Time
public Type       workerClass = typeof(RecordWorker);
public List<JobDef> measuredTimeJobs;                // type=Time 时统计这些 Job 的耗时
public int        displayOrder = 99999;
public RecordWorker Worker
```

`Pawn_RecordsTracker : IExposable`：

```csharp
public float GetValue(RecordDef def);
public int   GetAsInt(RecordDef def);
public void  Increment(RecordDef def);
public void  AddTo(RecordDef def, float value);
public void  RecordsTickInterval(int delta);         // 周期采样（Time 类记录）
public void  RecordsTickMothballed(int interval);
```

**规模**：`RimWorld.RecordDefOf` 共 **49** 个字段（= 可直接展示的记录项）；`RecordDefOf` 在 `RimWorld` 命名空间（`Verse.RecordDefOf` **不存在**）。

### 8.3 历史事件 —— 144 个事件词表

| 类型 | 说明 |
|---|---|
| `RimWorld.HistoryEventDefOf` | **144** 个内置事件定义（字段数实测） |
| `RimWorld.HistoryEvent` | 单条事件实例（含 `tale`，可提供 `CurveMark` 的文本/颜色，见 §9.6） |
| `RimWorld.HistoryEventsManager` | 挂载于 `History.historyEventsManager` |
| `RimWorld.HistoryEventArgsNames` | 事件参数名常量表（做事件详情 UI 时用） |
| `RimWorld.ExternalHistory` / `ExternalHistoryUtility` | 外部历史（跨存档统计） |

### 8.4 财富统计 `WealthWatcher`

```csharp
public const float WealthFactor_Slave = 0.75f;      // 奴隶财富折算系数
public int   HealthTotal      { get; }
public float WealthTotal      { get; }
public float WealthItems      { get; }
public float WealthBuildings  { get; }
public float WealthFloorsOnly { get; }
public float WealthPawns      { get; }
public WealthWatcher(Map map);
public void  ForceRecount(bool allowDuringInit = false);
public static float GetEquipmentApparelAndInventoryWealth(Pawn p);
public static bool  WealthItemsFilter(IThingHolder x);
```

### 8.5 统计源使用红线（PERF）

1. **采样必须周期化**：照原版 `recordTicksFrequency` / `RecordsTickInterval` 的节奏，**禁止**在 `DoWindowContents` 里每帧遍历全图/全体重算。
2. **重算按 tick 缓存**：原版 `HistoryAutoRecorderGroup.DrawGraph` 的写法是标准答案 ——
   ```csharp
   if (ticksGame != cachedGraphTickCount) { cachedGraphTickCount = ticksGame; /* 重建 curves */ }
   ```
   ★ 这与 RUIF 的 `TrCache`（语言戳失效）是同一套「有来源 / 有失效 / 有上限」思路。

---

## 9. 复杂图表实现标准 —— 原版曲线引擎 `SimpleCurveDrawer`

> ⭐⭐ **本轮最重要的纠正**：§4 原先把「图表」判为 `❌ 原版无对应`。**实测推翻**：原版存在**完整的曲线图表引擎** `Verse.SimpleCurveDrawer`（15 326 字节 / 12 个 public 方法 / 另有 `Style` `DrawInfo` `View` 三个配套类型）。原版历史面板就是它的消费方。→ 图表层的定位应从「全自研」改为「**封装原版引擎 + 自研缺口部分**」。

### 9.1 绘制入口（`public static class SimpleCurveDrawer`）

```csharp
DrawCurve(Rect rect, SimpleCurve curve, SimpleCurveDrawerStyle style = null, List<CurveMark> marks = null, Rect legendScreenRect = default(Rect))
DrawCurve(Rect rect, SimpleCurveDrawInfo curve, SimpleCurveDrawerStyle style = null, List<CurveMark> marks = null, Rect legendScreenRect = default(Rect))
DrawCurves(Rect rect, List<SimpleCurveDrawInfo> curves, SimpleCurveDrawerStyle style = null, List<CurveMark> marks = null, Rect legendRect = default(Rect))   // ★ 多序列叠加
DrawCurveLines(Rect rect, SimpleCurveDrawInfo curve, bool drawPoints, Rect viewRect, bool useAALines, bool pointsRemoveOptimization)
DrawCurveMeasures(Rect rect, Rect viewRect, Rect graphRect, int xLabelsCount, int yLabelsCount, bool xIntegersOnly, bool yIntegersOnly)   // 坐标刻度
DrawCurvesLegend(Rect rect, List<SimpleCurveDrawInfo> curves)                        // 图例
DrawCurveMousePoint(List<SimpleCurveDrawInfo> curves, Rect screenRect, Rect viewRect, string labelX)   // ★ 悬停取值
DrawCurveMarks(Rect rect, Rect viewRect, List<CurveMark> marks)                      // ★ 事件标记
DrawGraphBackgroundLines(Rect rect, Rect viewRect)                                   // 背景网格
CurveToScreenCoordsInsideScreenRect(Rect rect, Rect viewRect, Vector2 curvePoint)    // ★ 曲线 → 屏幕
ScreenToCurveCoords(Rect rect, Rect viewRect, Vector2 screenPoint)                   // ★ 屏幕 → 曲线（做交互命中）
```

### 9.2 样式（`SimpleCurveDrawerStyle`，19 项实测属性）

| 属性 | 作用 |
|---|---|
| `DrawBackground` / `DrawBackgroundLines` | 背景填充 / 网格线 |
| `DrawMeasures` | 坐标刻度 |
| `MeasureLabelsXCount` / `MeasureLabelsYCount` | 刻度数量 |
| `XIntegersOnly` / `YIntegersOnly` | 刻度取整（人数、天数等离散量必须开） |
| `LabelX` | X 轴标签 |
| `DrawPoints` | 数据点 |
| `DrawLegend` | 图例 |
| `DrawCurveMousePoint` | 悬停取值浮标 |
| `OnlyPositiveValues` | 裁掉负半轴 |
| `UseFixedSection` / `FixedSection` | 固定 X 区间（缩放/时间窗口）|
| `UseFixedScale` / `FixedScale` | 固定 Y 比例 |
| `UseAntiAliasedLines` | 抗锯齿接线 |
| `PointsRemoveOptimization` | 点抽稀优化（大数据量用） |

### 9.3 数据与曲线模型

```csharp
public class SimpleCurveDrawInfo {          // 一条序列
    public SimpleCurve curve;
    public string label = string.Empty;     // 图例名
    public Color  color = Color.green;      // 线色
    public string valueFormat;              // 数值格式
}

public class SimpleCurve : IEnumerable<CurvePoint>, IExposable {
    public List<CurvePoint> Points { get; }
    public int   PointsCount { get; }
    public float MinY / MaxY { get; }
    public void  Add(float x, float y, bool sort = true);
    public void  SortPoints();
    public float Evaluate(float x);                 // 取值
    public float EvaluateInverted(float y);         // ★ 反解 x（阈值检测/交点）
    public float ClampToCurve(float value);
    public void  SetPoints(IEnumerable<CurvePoint> newPoints);
    public SimpleCurveView View { get; }
}

public struct CurveMark {                   // ★ 时间轴上的事件钉（struct！）
    public float  X     { get; }
    public string Message { get; }
    public Color  Color { get; }
    public CurveMark(float x, string message, Color color);
}

public class SimpleCurveView {              // 视图状态
    public Rect rect;
    public IEnumerable<float> DebugInputValues { get; }
    public void SetViewRectAround(SimpleCurve curve);
    public void SetDebugInput(object key, float value);
}
```

### 9.4 实证范例：原版历史面板的完整链路

`MainTabWindow_History` 的实际调用（反编译原文，已剔除无关行）：

```csharp
historyAutoRecorderGroup = Find.History.Groups().FirstOrDefault();
...
marks.Add(new CurveMark(x, tale.ShortSummary, tale.def.historyGraphColor));   // 事件 → 曲线钉
historyAutoRecorderGroup.DrawGraph(graphRect, legendRect, graphSection, marks);  // ★ 一行出图
```

时间范围切换 = 4 个按钮喂不同 `FloatRange`（原版文案键）：`Last30Days` / `Last100Days` / `Last300Days` / `AllDays`。

`HistoryAutoRecorderGroup.DrawGraph` 内部（实测节选）：

```csharp
int ticksGame = Find.TickManager.TicksGame;
if (ticksGame != cachedGraphTickCount) {          // ★ 每帧只重建一次曲线
    cachedGraphTickCount = ticksGame;
    curves.Clear();
    for (int i = 0; i < recorders.Count; i++) {
        var r = recorders[i];
        var info = new SimpleCurveDrawInfo {
            color = r.def.graphColor, label = r.def.LabelCap, valueFormat = r.def.valueFormat,
            curve = new SimpleCurve()
        };
        for (int j = 0; j < r.records.Count; j++)
            info.curve.Add(new CurvePoint((float)j * (float)r.def.recordTicksFrequency / 60000f, r.records[j]), sort: false);
        info.curve.SortPoints();
        if (r.records.Count == 1)                  // 单点也要画得出来
            info.curve.Add(new CurvePoint(1.6666667E-05f, r.records[0]));
        curves.Add(info);
    }
}
```

⭐ **X 轴单位 = 游戏天数**：`j * recordTicksFrequency / 60000f`（1 天 = 60 000 tick，`60000/60000 = 1`）。消费 Mod 自建曲线时应沿用它，否则时间轴刻度对不上原版。

### 9.5 可实现图表清单（按原版支撑程度）

| 图表 | 原版支撑 | 落法 |
|---|---|---|
| 单折线 / 多折线叠加 | ✅ | `DrawCurve` / `DrawCurves` |
| 图例 | ✅ | `style.DrawLegend`、`DrawCurvesLegend` |
| 坐标刻度（整数/小数） | ✅ | `style.DrawMeasures` + `XIntegersOnly`/`YIntegersOnly` + `MeasureLabelsXCount/YCount` |
| 背景网格 | ✅ | `style.DrawBackgroundLines`、`DrawGraphBackgroundLines` |
| 数据点 | ✅ | `style.DrawPoints` |
| **悬停取值** | ✅ | `style.DrawCurveMousePoint` |
| **事件标记（时间轴钉）** | ✅ | `List<CurveMark>`（原版把 Tale 画成钉） |
| 时间窗口 / 固定视图 | ✅ | `UseFixedSection` + `FixedSection`、`UseFixedScale` + `FixedScale`、`SimpleCurveView.SetViewRectAround` |
| **交互命中（点到曲线）** | ✅ | `ScreenToCurveCoords` + `CurveToScreenCoordsInsideScreenRect` |
| 进度条 / 单条填充 | ✅ | `Widgets.FillableBar(±tex/bg/border)`、`FillableBarLabeled`、`GenDraw.DrawFillableBar(FillableBarRequest)` |
| 阈值反解（如「第几天破 1 万财富」） | ✅ | `SimpleCurve.EvaluateInverted` |
| 柱状 / 堆叠柱 | ⚠️ | 无专用 API：`Widgets.DrawBoxSolid` 自绘（每柱一个 Rect），基线用 `DrawLineHorizontal` |
| 饼 / 环 | ❌ | 自绘（`Widgets.DrawLine` 扇形扫描；`GenDraw.DrawAimPie` 是地图向、可借用几何） |
| 热力图 / 日历热力 | ❌ | 自绘网格（逐格 `DrawBoxSolid`） |
| 桑基 / 帕累托 / 雷达 | ❌ | 自绘（`Widgets.DrawLine` + `GenDraw.DrawLineBetween` 连线） |

### 9.6 RUIF 落地建议（据此调整分层）

1. `Chart.Line / MultiLine / Sparkline` **直接桥接 `SimpleCurveDrawer`** —— 曲线渲染零自研、零风险、自动获得图例/刻度/悬停/标记。
2. `ChartStyle` 映射 `SimpleCurveDrawerStyle` 的 19 项 + 主题色（`Palette`）→ 12 套风格自动生效。
3. `ChartsFmt` 保留纯计算（归一化、分桶、插值、坐标换算），**渲染一律交原版引擎**。
4. 只有饼/环/热力/桑基/雷达需要自研几何 —— 这部分才继续沿用「纯计算 + 自绘」模式。
5. 交互式图表（缩放/拖动）用 `SimpleCurveView` + `ScreenToCurveCoords`，不要自己维护变换矩阵。

---

## 10. 复杂组件族规模与范式（第二轮取证）

### 10.1 规模（口径：**顶层类型**，反射实测）

| 前缀 | 顶层 public | 顶层 internal | 职责 |
|---|---|---|---|
| `Dialog_*` | **123** | 0 | 对话窗（全屏 / 窗口） |
| `Alert_*` | **132** | 0 | 警报 |
| `Designator_*` | **81** | 0 | 指令 |
| `FloatMenuOption`（顶层） | 55 | 0 | 上下文菜单项（另有大量嵌套类） |
| `ITab_*` | **39** | 0 | 检视页签 |
| `Command_*` | 28 | 1 | 命令按钮 |
| `MainTabWindow_*` | **15** | 0 | 底栏主页 |
| `Page_*` | **11** | 0 | 全屏流程页 |
| `Listing_*` | 7 | 0 | 布局族 |
| `QuickSearch*` | 3 | 0 | 快捷搜索 |

> ⚠️ **口径修正（重要）**：本文 §1.1 原记 `Dialog_ 220 / ITab_ 80 / MainTabWindow_ 47 / Page_ 50`，与本表实测（123 / 39 / 15 / 11）**不符**。原因已查明：原数字疑似用了「全名包含该串」的匹配（实测 `Dialog_` 的**全名包含**计数为 **300**，其中绝大多数是嵌套类与编译器生成类如 `Dialog_Trade.<>c__DisplayClass`），**不是顶层类型数**。引用规模时请以本表为准。

### 10.2 范式契约（做等价组件时的最小实现）

**ITab / InspectTabBase（检视页签）**

```csharp
public abstract class InspectTabBase {
    public string labelKey; protected Vector2 size; public string tutorTag;
    protected abstract float PaneTopY { get; }
    protected abstract bool  StillValid { get; }
    protected abstract void  CloseTab();
    protected abstract void  FillTab();        // ★ 只实现这个 = 画出页签内容
    protected virtual  void  UpdateSize();
    protected virtual  void  ExtraOnGUI();
    public void DoTabGUI();                    // 框架调用（非虚）
    public virtual void TabTick(); public virtual void TabUpdate();
}
public abstract class ITab : InspectTabBase {  // RimWorld 侧便捷基类
    protected object SelObject       => Find.Selector.SingleSelectedObject;
    protected List<object> AllSelObjects => Find.Selector.SelectedObjects;
    protected Thing SelThing         => Find.Selector.SingleSelectedThing;
    protected virtual Pawn SelPawn   => SelThing as Pawn;
}
```

**Alert（警报）**

```csharp
public abstract class Alert {
    public const float Width = 154f;            // ★ 图标块宽（警报条排版基准）
    public const float InfoRectWidth = 330f;    // ★ 展开信息宽
    protected AlertPriority defaultPriority;
    protected string defaultLabel, defaultExplanation;
    protected bool requireRoyalty / requireIdeology / requireBiotech / requireAnomaly;
    protected virtual Color BGColor => Color.clear;
    public abstract AlertReport GetReport();    // ★ 唯一必须实现
    public virtual TaggedString GetExplanation();
    public virtual string GetLabel();
    public virtual AlertPriority Priority => defaultPriority;
    public bool Active { get; }  public float Height { get; }
    public virtual string GetJumpToTargetsText => "ClickToJumpToProblem".Translate();
}
```

**MainTabWindow（底栏主页）**

```csharp
public abstract class MainTabWindow : Window {
    public MainButtonDef def;
    public virtual Vector2 RequestedTabSize => new Vector2(1010f, 684f);
    public virtual MainTabWindowAnchor Anchor => MainTabWindowAnchor.Left;
}
```
（`MainTabWindow_History` 覆写为 `(1010f, 640f)` —— 历史页比默认矮 44。）

**GizmoGridDrawer（命令网格）**

```csharp
public static class GizmoGridDrawer {
    public static HashSet<KeyCode> drawnHotKeys;         // 热键去重（跨帧）
    public static Func<Gizmo, bool> customActivator;
    public static float HeightDrawnRecently { get; }
    public static void DrawGizmoGridFor(IEnumerable<object> selectedObjects, out Gizmo mouseoverGizmo);
    public static void DrawGizmoGrid(IEnumerable<Gizmo> gizmos, float startX, out Gizmo mouseoverGizmo,
        Func<Gizmo, bool> customActivatorFunc = null,
        Func<Gizmo, bool> highlightFunc = null,
        Func<Gizmo, bool> lowlightFunc = null,
        bool multipleSelected = false);
}
```
★ 悬停对象是 **`out` 出参**（不是返回值）；`highlightFunc`/`lowlightFunc` 是**每帧回调**（可做脉冲高亮）。

### 10.3 可实现的复杂组件候选（按族归类）

| 候选组件 | 参照原版 | 关键支撑 |
|---|---|---|
| 历史统计面板 | `MainTabWindow_History` | §8.1 + §9.4（一行 `DrawGraph`；时间窗口按钮） |
| 殖民者统计表 | `Pawn_RecordsTracker` + `RecordDefOf`(49) | `Widgets.FillableBar` + `Listing_Standard` |
| 曲线图表族（折线/多线/迷你图） | `SimpleCurveDrawer` | §9.1–9.3（**零自研渲染**） |
| 警报中心 | `Alert_*`(132) | `Alert.GetReport/GetExplanation` + `Width=154`/`InfoRectWidth=330` |
| 命令网格 | `GizmoGridDrawer` | `DrawGizmoGrid`（自带高亮/低亮/热键去重） |
| 检视页签扩展 | `ITab_*`(39) | `InspectTabBase.FillTab()` + `PaneTopY/StillValid` |
| 全屏流程页 | `Page_*`(11) | `Page.Title / MainRect / DoBottomButtons` |
| 对话窗族 | `Dialog_*`(123) | `Window` 生命周期（§5 第 9 条） |
| 上下文菜单 / 菜单网格 | `FloatMenu` / `FloatMenuOption` | 优先级排序 + `extraPartOnGUI` 自定义右侧内容 |
| 事件时间轴 | `HistoryEventDefOf`(144) + `CurveMark` | 事件 → 曲线钉（§9.4） |
| 财富/资源概览 | `WealthWatcher` + `ResourceCounter` | 5 个财富分项 + `ForceRecount` |

### 10.4 复杂组件的实现红线

1. **不重造原版已有的整图能力**：曲线一律 `SimpleCurveDrawer`，进度条一律 `Widgets.FillableBar`，图标一律 `DefIcon/ThingIcon`。
2. **重算必须按 tick 缓存**（§8.5 / §9.4）——原版历史面板的标准写法。
3. **`out` / `ref` 签名照抄原版**：如 `DrawGizmoGrid(..., out Gizmo mouseoverGizmo)`、`Checkbox(ref bool)`，外层封装不得吞掉语义。
4. **复用原版常量**：`Alert.Width=154`、`Alert.InfoRectWidth=330`、`MainTabWindow_History.RequestedTabSize=(1010,640)`、`HistoryAutoRecorderDef.recordTicksFrequency=60000`。
5. **枚举/词表优先用 DefOf**：`RecordDefOf`(49)、`HistoryEventDefOf`(144) 已给出稳定 defName，别自造字符串。

### 10.5 复现命令（本节全部数字）

```powershell
# 统计族规模 + DefOf 字段数（反射，含口径对比）
$dll='<Managed>\Assembly-CSharp.dll'
$mg=Split-Path $dll
foreach($n in @('UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.IMGUIModule.dll','UnityEngine.TextRenderingModule.dll')){ try{[void][Reflection.Assembly]::LoadFrom("$mg\$n")}catch{} }
try { $a=[Reflection.Assembly]::LoadFrom($dll); $types=$a.GetTypes() } catch [Reflection.ReflectionTypeLoadException] { $types=$_.Exception.Types | Where-Object { $_ } }
foreach($p in @('Dialog_','ITab_','MainTabWindow_','Page_','Alert_','Designator_','Command_','Listing_')){
  $top=@($types | Where-Object { $_.Name -like "$p*" })
  "{0,-16} 顶层={1} public={2} 全名含={3}" -f $p, $top.Count, @($top|Where-Object IsPublic).Count,
    @($types | Where-Object { $_.FullName -match [regex]::Escape($p) }).Count
}
foreach($n in @('RimWorld.RecordDefOf','RimWorld.HistoryEventDefOf')){
  $t=$types | Where-Object { $_.FullName -eq $n } | Select-Object -First 1
  if($t){ "{0} = {1} 字段" -f $n, @($t.GetFields([Reflection.BindingFlags]'Public,Static')).Count }
}
```
