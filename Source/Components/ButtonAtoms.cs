using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// 按钮语义色调（方案 §3.3 / §3.4）—— <see cref="Tokens"/> 之上的<b>语义别名层</b>，
    /// <b>不是颜色真源、不是独立取值</b>。
    ///
    /// <para><see cref="Default"/> 与 <see cref="Accent"/> 同映射 <see cref="Tokens.Accent"/>；
    /// <see cref="Danger"/> → <see cref="Tokens.Dead"/>；<see cref="Warn"/> → <see cref="Tokens.Warn"/>。
    /// 与 <see cref="InputState"/> <b>正交</b>：危险态是 tone 旁路，<b>不新增第 7 态</b>（避免状态爆炸）。
    /// 因颜色仍全部经 <see cref="Tokens"/>，本层不破「禁 <c>new Color</c>、禁裸 <c>GUI.color</c>」红线。</para>
    /// </summary>
    public enum ButtonTone
    {
        /// <summary>默认（观感同 <see cref="Accent"/>，映射 <see cref="Tokens.Accent"/>）。</summary>
        Default,
        /// <summary>强调 / 主操作（映射 <see cref="Tokens.Accent"/>）。</summary>
        Accent,
        /// <summary>危险 / 破坏性操作（映射 <see cref="Tokens.Dead"/>）。</summary>
        Danger,
        /// <summary>警告 / 注意（映射 <see cref="Tokens.Warn"/>）。</summary>
        Warn
    }

    /// <summary>
    /// 按钮落位档（方案 §5.6 单按钮降级阶梯 D0/D1/D6 的纯计算结果）。
    /// <para>D2（仅图标）不在本枚举内：本批冻结签名里的按钮要么<b>只带文字标签</b>（<c>PrimaryButton</c> /
    /// <c>ToggleButton</c>）、要么<b>只带字形</b>（<c>IconButton</c>）—— 「图标 + 文字」组合不存在，
    /// 故「藏文字」与 D0 的「图标 + 文字」项在本批不可达（已回报主理人）。</para>
    /// </summary>
    internal enum ButtonFit
    {
        /// <summary>D0 / D3：完整标签。</summary>
        Full,
        /// <summary>D1：省略号标签（调用方须补完整悬停提示）。</summary>
        Ellipsis,
        /// <summary>D6：省略该件，画「—」占位。</summary>
        Placeholder
    }

    /// <summary>
    /// 落位计划（<see cref="ButtonFmt.Plan"/> 的返回值）。
    /// <b>纯值类型</b> ⇒ 热路径零堆分配（方案 A9）。
    /// </summary>
    internal readonly struct ButtonPlan
    {
        /// <summary>落位档。</summary>
        internal readonly ButtonFit Fit;
        /// <summary>内容区水平内边距（D0/D1 = 8f；D3 = 4f，<b>不得归零</b>）。</summary>
        internal readonly float PadX;

        internal ButtonPlan(ButtonFit fit, float padX)
        {
            Fit = fit;
            PadX = padX;
        }
    }

    /// <summary>
    /// 按钮族的<b>纯计算层</b>（方案 §3.5「状态机 / tone→Tokens | 纯函数 ButtonFmt（可离线单测）」）。
    ///
    /// <para>与 <see cref="InputFmt"/> 同风范：<b>只做 state → 令牌映射与阈值判定，不触任何引擎状态</b>
    /// ⇒ 可在无 Unity 宿主的离线单测里断言（<c>Tests/</c> 由质量部负责）。</para>
    ///
    /// <para><b>⛔ 不在本类出现 <c>new Color</c> / 裸 <c>GUI.color</c></b>：一切取色都经 <see cref="Tokens"/>
    /// 与 <see cref="Draw.WithAlpha"/>；<see cref="ButtonTone"/> 只是语义别名。</para>
    /// </summary>
    public static class ButtonFmt
    {
        // ------------------------------------------------------------ 度量常量（方案 §5.6 / §5.9）

        /// <summary>
        /// D0「完整标签」的宽度加宽量（32f）。
        /// <para>出处：§5.6「<c>w ≥ Measure.Width(label, FontLabel) + 32f</c>（32 = PadX 8×2 + icon 18 + Gap 6）」。
        /// 本批按钮无「图标 + 文字」组合，但<b>仍按冻结阈值逐值取 32f</b> —— 阈值与方案 1:1，避免门禁比对时出现
        /// 第二套数字（代价仅是省略号提前约 16f 触发，无观感损失）。</para>
        /// </summary>
        public const float FullWidthPad = 32f;

        /// <summary>D6 阈值（28f）：宽度不足此值 ⇒ 省略该件，画「—」占位（§5.6）。</summary>
        public const float MinUsableWidth = 28f;

        /// <summary>D1 / D2 / D3 的分界宽度（48f，§5.6）。</summary>
        public const float EllipsisFloorWidth = 48f;

        /// <summary>D3 收窄后的内容内边距（4f，下限，<b>不得归零</b>，§5.6）。</summary>
        public const float NarrowPadX = 4f;

        /// <summary>焦点环透明度（0.55f；与既有 <c>InputAtoms.StateSample</c> 逐值一致，§5.5）。</summary>
        public const float FocusRingAlpha = 0.55f;

        // ------------------------------------------------------------ tone → 令牌（唯一语义通道）

        /// <summary>
        /// tone → 语义色。<b>唯一映射点</b>：<c>Default|Accent → Tokens.Accent</c> ·
        /// <c>Danger → Tokens.Dead</c> · <c>Warn → Tokens.Warn</c>（§3.3 / §5.3）。
        /// </summary>
        public static Color Tone(ButtonTone tone)
        {
            switch (tone)
            {
                case ButtonTone.Danger: return Tokens.Dead;
                case ButtonTone.Warn: return Tokens.Warn;
                default: return Tokens.Accent;      // Default | Accent
            }
        }

        /// <summary>tone 的低透明度变体（<c>Draw.WithAlpha(Tone, 0.16f)</c>）—— 按下 / 选中底的填充色（§5.3）。</summary>
        public static Color ToneSoft(ButtonTone tone) => Draw.WithAlpha(Tone(tone), 0.16f);

        /// <summary>焦点环颜色（<c>Draw.WithAlpha(Tone, 0.55f)</c>，§5.5）。</summary>
        public static Color FocusRingColor(ButtonTone tone) => Draw.WithAlpha(Tone(tone), FocusRingAlpha);

        // ------------------------------------------------------------ state × tone → 四通道（§5.3 的 24 格）

        /// <summary>
        /// 收集状态（禁用优先，沿用 <see cref="InputFmt.Resolve"/> 的既有优先级）。
        /// <para><b>⚠ <see cref="InputState.Focused"/> 不由本方法产出</b>：方案 §3.4 明确
        /// <c>InputFmt.Resolve(hovered, pressed, selected, enabled)</c> <b>不加 focused 参数</b>
        /// （加了 = SemVer 破坏 + 会误产生该态）。焦点态由调用方在命中调用后自行叠加（见 <see cref="ButtonAtoms"/>）。</para>
        /// </summary>
        public static InputState Resolve(bool hovered, bool pressed, bool selected, bool enabled)
            => InputFmt.Resolve(hovered, pressed, selected, enabled);

        /// <summary>
        /// 6 态 × tone → 四通道取色（方案 §5.3 表格<b>逐格</b>实现；Default tone 列 100% 复现既有
        /// <c>InputAtoms.StateSample</c>，无「同一状态两套配色」）。
        ///
        /// <para>要点：① <see cref="InputState.Disabled"/> <b>优先于一切</b>且<b>剥除 tone</b>（危险钮禁用后不得再显红边）；
        /// ② <see cref="ButtonTone.Danger"/>/<see cref="ButtonTone.Warn"/> 只改<b>描边 + 文字/图标</b>，
        /// <b>fill 不染色</b>（危险感来自边框与字，不来自红底）。</para>
        /// </summary>
        public static void Channels(InputState state, ButtonTone tone,
            out Color fill, out Color border, out Color text, out Color icon)
        {
            Color t = Tone(tone);
            switch (state)
            {
                case InputState.Hover:
                    fill = Tokens.CardHover; border = t; text = Tokens.Text; icon = Tokens.Muted; break;
                case InputState.Pressed:
                case InputState.Selected:
                    fill = ToneSoft(tone); border = t; text = t; icon = t; break;
                case InputState.Disabled:
                    // ⚠ 剥除 tone：border 回落 BorderSoft（与既有 StateSample 的 Disabled 分支一致 —— 方案 §5.3 以既有实现为权威）
                    fill = Tokens.Panel; border = Tokens.BorderSoft; text = Tokens.Dim; icon = Tokens.Dim; break;
                case InputState.Focused:
                    fill = Tokens.PanelRaised; border = t; text = Tokens.Text; icon = Tokens.Muted; break;
                default:
                    fill = Tokens.PanelRaised; border = Tokens.BorderSoft; text = Tokens.Text; icon = Tokens.Muted; break;
            }
        }

        // ------------------------------------------------------------ 命中（§5.4 有意偏离）

        /// <summary>
        /// 命中后的最终判定：<b>事后 AND</b>。
        ///
        /// <para><b>⛔ 调用方必须先无条件调用命中 API、再调本方法</b> —— 不得写成
        /// <c>enabled &amp;&amp; Widgets.ButtonInvisible(...)</c> 的短路形式：短路会在 <c>enabled</c> 跨帧变化时<b>跳号</b>
        /// （<c>GUI.Button</c> 的 control-ID 序被打乱）⇒ hot/active 控件错配、丢点击。
        /// 这是<b>有意偏离</b> vanilla <c>CustomButtonText</c> 的 <c>active &amp;&amp; …</c> 短路由（该处无跨帧 ID 依赖，故安全），
        /// 已登记为偏离而非疏漏。</para>
        /// </summary>
        public static bool Clicked(bool enabled, bool hit) => enabled && hit;

        // ------------------------------------------------------------ 焦点（§5.5 单一画法）

        /// <summary>是否画焦点环：<paramref name="focusRing"/>（调用方显式）或按名查询命中（键盘通路）。二者可独立、可并用。</summary>
        public static bool ShouldRing(bool focusRing, bool namedFocused) => focusRing || namedFocused;

        /// <summary>
        /// 焦点环几何：外扩 2px → <b>过 <see cref="Px.Snap"/></b>（xMin/yMin floor、xMax/yMax ceil）。
        /// <para>为什么必须过 Px：UI 缩放 &gt;1 时外扩 2px 会落半像素 ⇒ 1px 环被双线性插值糊成 2px 灰边（§5.5）。</para>
        /// </summary>
        public static Rect FocusRingRect(Rect scope)
        {
            const float Out = WindowFmt.FocusRingOutset;
            return Px.Snap(new Rect(scope.x - Out, scope.y - Out, scope.width + Out * 2f, scope.height + Out * 2f));
        }

        // ------------------------------------------------------------ 降级阶梯（§5.6，阈值一律实测宽度传入）

        /// <summary>
        /// 单按钮落位计划（§5.6 D0/D1/D3/D6）。
        ///
        /// <para><b>阈值必须由调用方按 <c>Measure.Width(label, Tokens.FontLabel)</c> 实测后传入</b> ——
        /// 本方法<b>不接受字符数估算</b>（Tiny 下拉丁 80f ≈ 10 字符，<b>CJK 80f ≈ 3 字</b>）。
        /// 本方法本身是纯函数（只做区间判定）⇒ 可离线单测。</para>
        /// </summary>
        /// <param name="width">控件可用宽度（字面值，不做任何假设）。</param>
        /// <param name="labelWidth"><c>Measure.Width(label, Tokens.FontLabel)</c> 的实测结果。</param>
        internal static ButtonPlan Plan(float width, float labelWidth)
        {
            // D6：不足最小可用宽度 ⇒ 省略该件（下限 4/2 的内边距，不得归零）
            if (width < MinUsableWidth) return new ButtonPlan(ButtonFit.Placeholder, NarrowPadX);

            // D0：完整标签（冻结阈值 = 实测标签宽 + 32f）
            if (width >= labelWidth + FullWidthPad) return new ButtonPlan(ButtonFit.Full, WindowFmt.ButtonPadX);

            // D1：48f 起走省略号 + 完整悬停提示
            if (width >= EllipsisFloorWidth) return new ButtonPlan(ButtonFit.Ellipsis, WindowFmt.ButtonPadX);

            // D3：收窄内边距（8 → 4，下限不得归零）后再试一次
            if (width >= labelWidth + NarrowPadX * 2f) return new ButtonPlan(ButtonFit.Full, NarrowPadX);
            return new ButtonPlan(ButtonFit.Ellipsis, NarrowPadX);
        }
    }

    /// <summary>
    /// B1 · 按钮族（方案 §5.4 / §5.3）：<see cref="PrimaryButton"/> / <see cref="ToggleButton"/> / <see cref="IconButton"/>。
    ///
    /// <para><b>参照锚</b>：vanilla <c>Widgets.CustomButtonText</c>（原版自己的全自绘按钮实现）= 自绘底/边/字 + 自带命中。
    /// 本族是它的<b>令牌化等价物 + 音效门控修正</b>，<b>不是新范式</b>。</para>
    ///
    /// <para><b>三条不可协商的实现纪律</b>：
    /// ① 禁用态 <b>100% 自绘</b>（不依赖原版 <c>active</c>、<b>不置 <c>GUI.enabled=false</c></b>）；
    /// ② 命中<b>总是调用 + 事后 AND</b>（见 <see cref="ButtonFmt.Clicked"/>）；
    /// ③ <c>controlName != null</c> 时 <c>SetNextControlName</c> 与命中调用<b>紧邻</b>（中间不得夹其他控件创建，§6 硬约束 ①）。</para>
    ///
    /// <para><b>零状态</b>：全部交互值由调用方持有（<c>ref</c>）或由引擎查询，组件内<b>无任何 <c>static</c> 可变字段</b>。</para>
    /// </summary>
    public static class ButtonAtoms
    {
        /// <summary>
        /// 单一<b>焦点环画法</b>（§5.5）—— 按钮族与选择件<b>共用</b>，禁止另造第二套。
        /// </summary>
        /// <param name="scope">环所覆盖的作用域（按钮族 = <c>rect</c> 本身；选择件 = <b>整行可点区</b>）。</param>
        /// <param name="tone">语义色调（决定环色）。</param>
        public static void FocusRing(Rect scope, ButtonTone tone)
            => RUIF.Draw.Border(ButtonFmt.FocusRingRect(scope), ButtonFmt.FocusRingColor(tone));

        /// <summary>
        /// 实心按钮（主操作 / 普通操作）。返回是否被点击。
        /// </summary>
        /// <param name="rect">按钮矩形（调用方须预留 <b>≥2px 外边</b>，否则焦点外环被邻件遮盖，§5.5）。</param>
        /// <param name="label">标签文案（<b>调用方须自行 <c>Tr.Get</c></b>；组件内不内置英文默认串）。</param>
        /// <param name="enabled">是否可用。禁用时<b>不产生事件、不消费事件、不响悬停音</b>，并 100% 自绘灰化。</param>
        /// <param name="tone">语义色调（危险钮禁用后<b>剥除红边</b>）。</param>
        /// <param name="focusRing">强制画焦点环（供调用方自建导航时显式告知）。</param>
        /// <param name="controlName">键盘通路名（<c>null</c> = 不登记）；按名焦点命中时<b>自动画环</b>。</param>
        public static bool PrimaryButton(Rect rect, string label, bool enabled = true,
            ButtonTone tone = ButtonTone.Default, bool focusRing = false, string controlName = null)
        {
            bool hovered = enabled && Mouse.IsOver(rect);
            bool pressed = hovered && PressedNow();
            InputState state = ButtonFmt.Resolve(hovered, pressed, false, enabled);
            ButtonFmt.Channels(state, tone, out Color fill, out Color border, out Color text, out _);

            // ① 自绘底 + 边（vanilla CustomButtonText 范式：先画后命中）
            RUIF.Draw.Box(rect, fill);
            RUIF.Draw.Border(rect, border);

            // ② 命中：命名与控件创建【紧邻】；总是调用（禁用也要调用，保 control-ID 序稳定）
            if (controlName != null) GUI.SetNextControlName(controlName);
            bool hit = Widgets.ButtonInvisible(rect, doMouseoverSound: enabled);

            // ③ 焦点态叠加（§5.3 Focused 行：同 Default 底 + Tone 边 + 外环）—— 查询必须在控件创建【之后】
            ApplyFocusOverlay(rect, tone, state, enabled, focusRing, controlName);

            // ④ 文本（降级阶梯按实测宽度判定）
            DrawTextContent(rect, label, text);

            // ⑤ 禁用态 tooltip 兜底（§5.8 键 RUIF.Comp.Button.Disabled；本族 Primary/Toggle/Icon 各 1 处
            //    + InputAtoms 选择件 1 处 = 共 4 处活跃调用点，非"唯一"）
            if (!enabled && Mouse.IsOver(rect)) TooltipHandler.TipRegion(rect, Tr.Get("RUIF.Comp.Button.Disabled"));

            return ButtonFmt.Clicked(enabled, hit);
        }

        /// <summary>
        /// 二态切换按钮（<b>按钮外形 + 自带文字标签</b>；与拨杆形 <c>InputAtoms.ToggleSwitch</c>、纯图标 <c>InteractAtoms.ToolbarIcon</c> 三者判别见方案 §3.2）。
        /// </summary>
        /// <returns><b>是否发生变化</b>（§5.4 ② 勘误后口径）—— 与 <c>ToggleSwitch</c> / <c>Segmented</c> / 选择件族同向统一；
        /// 切换后的<b>新值从 <paramref name="on"/> 读</b>（<c>ref</c> 传入，返回值不携带该信息）。</returns>
        /// <param name="rect">按钮矩形（预留 ≥2px 外边）。</param>
        /// <param name="label">标签文案（调用方自行 <c>Tr.Get</c>）。</param>
        /// <param name="on">二态值，由调用方持有（组件零状态）。选中态 = <see cref="InputState.Selected"/> 行。</param>
        /// <param name="enabled">是否可用（禁用时不产生状态变更、不消费事件）。</param>
        /// <param name="tone">语义色调。</param>
        /// <param name="focusRing">强制画焦点环。</param>
        /// <param name="controlName">键盘通路名（<c>null</c> = 不登记）。</param>
        public static bool ToggleButton(Rect rect, string label, ref bool on, bool enabled = true,
            ButtonTone tone = ButtonTone.Default, bool focusRing = false, string controlName = null)
        {
            bool hovered = enabled && Mouse.IsOver(rect);
            bool pressed = hovered && PressedNow();
            InputState state = ButtonFmt.Resolve(hovered, pressed, enabled && on, enabled);
            ButtonFmt.Channels(state, tone, out Color fill, out Color border, out Color text, out _);

            RUIF.Draw.Box(rect, fill);
            RUIF.Draw.Border(rect, border);

            if (controlName != null) GUI.SetNextControlName(controlName);
            bool hit = Widgets.ButtonInvisible(rect, doMouseoverSound: enabled);

            ApplyFocusOverlay(rect, tone, state, enabled, focusRing, controlName);
            DrawTextContent(rect, label, text);
            if (!enabled && Mouse.IsOver(rect)) TooltipHandler.TipRegion(rect, Tr.Get("RUIF.Comp.Button.Disabled"));

            if (ButtonFmt.Clicked(enabled, hit)) { on = !on; return true; }
            return false;                               // §5.4（勘误后）：返回是否发生变化；新值从 on（ref）读
        }

        /// <summary>
        /// 图标按钮（<b>仅字形</b>：工具栏密排 / 底栏图标）。
        /// <para>字形限 <b>BMP、非 emoji</b> 的几何/符号（方案 §5.9 字形可用范围）；⛔ 不得用英文单词冒充图标。</para>
        /// </summary>
        /// <param name="rect">按钮矩形（标准图标钮本体 28×28，底栏 32×32 —— 由调用方按 §5.9 决定）。</param>
        /// <param name="glyph">单个字形（调用方自行 <c>Tr.Get</c> 或取自定义表）。</param>
        /// <param name="enabled">是否可用。</param>
        /// <param name="tone">语义色调（字形 <c>Muted → Tone</c>）。</param>
        /// <param name="focusRing">强制画焦点环。</param>
        /// <param name="controlName">键盘通路名（<c>null</c> = 不登记）。</param>
        public static bool IconButton(Rect rect, string glyph, bool enabled = true,
            ButtonTone tone = ButtonTone.Default, bool focusRing = false, string controlName = null)
        {
            bool hovered = enabled && Mouse.IsOver(rect);
            bool pressed = hovered && PressedNow();
            InputState state = ButtonFmt.Resolve(hovered, pressed, false, enabled);
            ButtonFmt.Channels(state, tone, out Color fill, out Color border, out _, out Color icon);

            RUIF.Draw.Box(rect, fill);
            RUIF.Draw.Border(rect, border);

            if (controlName != null) GUI.SetNextControlName(controlName);
            bool hit = Widgets.ButtonInvisible(rect, doMouseoverSound: enabled);

            ApplyFocusOverlay(rect, tone, state, enabled, focusRing, controlName);

            // D6：宽度不足 28f ⇒ 「—」+ 原因占位；其余情况字形居中（仅图标件无「藏文字」档）
            if (rect.width < ButtonFmt.MinUsableWidth)
                RUIF.Draw.Label(rect, "—", Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleCenter);
            else
                RUIF.Draw.Label(rect, glyph ?? string.Empty, Tokens.FontLabel, icon, TextAnchor.MiddleCenter);

            if (!enabled && Mouse.IsOver(rect)) TooltipHandler.TipRegion(rect, Tr.Get("RUIF.Comp.Button.Disabled"));

            return ButtonFmt.Clicked(enabled, hit);
        }

        // ------------------------------------------------------------ 内部

        /// <summary>
        /// 按住态探针（<b>全族唯一实现</b>）：鼠标在控件内且左键正按下 / 拖拽。
        /// <para>红线段：一律 <c>Mouse.IsOver</c>（含窗口与滚动区门禁），不用裸 <c>rect.Contains(mousePos)</c>；
        /// 一律 <c>Event.current</c>（<c>InputLegacyModule</c> 不在引用白名单，写 <c>Input.*</c> 编译不过）。</para>
        /// </summary>
        internal static bool PressedNow()
        {
            Event ev = Event.current;
            if (ev == null) return false;
            return ev.isMouse && ev.button == 0
                && (ev.type == EventType.MouseDown || ev.type == EventType.MouseDrag);
        }

        /// <summary>
        /// 焦点态叠加（§5.3 Focused 行 + §5.5 环）：<b>描边改 Tone + 画外环</b>；fill / text 沿用已画内容
        /// （Hover 行描边本就是 Tone，Selected / Pressed 行的 text 本就是 Tone ⇒ 叠加无冲突，不会覆盖出第三套配色）。
        /// <para><b>Disabled 优先于一切</b>：禁用件即便被授予焦点名也<b>不画环、不改边</b>（§5.3）。</para>
        /// </summary>
        private static void ApplyFocusOverlay(Rect rect, ButtonTone tone, InputState state, bool enabled,
            bool focusRing, string controlName)
        {
            if (!enabled) return;
            bool named = controlName != null && GUI.GetNameOfFocusedControl() == controlName;
            if (!ButtonFmt.ShouldRing(focusRing, named)) return;

            // Hover / Pressed / Selected 行的描边本已是 Tone ⇒ 不重复描边（避免多余绘制）
            bool borderAlreadyTone = state == InputState.Hover || state == InputState.Pressed
                                     || state == InputState.Selected;
            if (!borderAlreadyTone) RUIF.Draw.Border(rect, ButtonFmt.Tone(tone));
            FocusRing(rect, tone);
        }

        /// <summary>标签绘制（按 §5.6 实测阈值走 D0 / D1 / D6；<b>只走单行 + 省略，禁自动折行</b>）。</summary>
        private static void DrawTextContent(Rect rect, string label, Color text)
        {
            float labelWidth = Measure.Width(label ?? string.Empty, Tokens.FontLabel);
            ButtonPlan plan = ButtonFmt.Plan(rect.width, labelWidth);

            if (plan.Fit == ButtonFit.Placeholder)
            {
                RUIF.Draw.Label(rect, "—", Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleCenter);
                return;
            }

            Rect content = new Rect(rect.x + plan.PadX, rect.y,
                Mathf.Max(0f, rect.width - plan.PadX * 2f), rect.height);

            if (plan.Fit == ButtonFit.Full)
            {
                RUIF.Draw.Label(content, label ?? string.Empty, Tokens.FontLabel, text, TextAnchor.MiddleCenter);
                return;
            }

            // D1：省略号 + 完整悬停提示（首选 Draw.EllipsisLabel —— 走原版 LabelEllipses，不新增依赖）
            RUIF.Draw.EllipsisLabel(content, label ?? string.Empty, Tokens.FontLabel, text, TextAnchor.MiddleCenter);
            if (!string.IsNullOrEmpty(label) && Mouse.IsOver(rect)) TooltipHandler.TipRegion(rect, label);
        }
    }
}
