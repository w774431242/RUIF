using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>输入控件的六态（GATE-UI：状态齐全——默认/悬停/按下/选中/禁用/焦点）。</summary>
    public enum InputState { Default, Hover, Pressed, Selected, Disabled, Focused }

    /// <summary>
    /// 三态选择值（方案 §3.4 / §5.12）。<see cref="Partial"/> 用于「父节点部分选中」——
    /// 二态表达不了，故必须有第三态；视觉 = <b>居中实心方块</b>（<b>不用字符 ✓</b>）。
    /// </summary>
    public enum TriState
    {
        /// <summary>未选。</summary>
        Off,
        /// <summary>已选（视觉 = 几何对勾三点折线）。</summary>
        On,
        /// <summary>部分选中（视觉 = 盒内居中实心方块 <c>box × 0.5</c>；标签<b>保持 Text 不 Tone 化</b>）。</summary>
        Partial
    }

    /// <summary>输入控件的纯计算（可离线单测）。</summary>
    public static class InputFmt
    {
        /// <summary>由原始布尔量解析出六态之一（禁用优先）。</summary>
        public static InputState Resolve(bool hovered, bool pressed, bool selected, bool enabled)
        {
            if (!enabled) return InputState.Disabled;
            if (pressed) return InputState.Pressed;
            if (selected) return InputState.Selected;
            if (hovered) return InputState.Hover;
            return InputState.Default;
        }

        /// <summary>步进值（钳制到 min~max）。</summary>
        public static int StepValue(int value, int delta, int min, int max)
            => Mathf.Clamp(value + delta, min, max);

        /// <summary>值 → 0~1 比例（量程非法返回 0）。</summary>
        public static float Slider01(float value, float min, float max)
            => max <= min ? 0f : Mathf.Clamp01((value - min) / (max - min));

        /// <summary>01 → 实际值（roundTo&gt;0 时按步长取整）。</summary>
        public static float SliderValue(float t, float min, float max, float roundTo)
        {
            float v = min + (max - min) * Mathf.Clamp01(t);
            return roundTo > 0f ? Mathf.Round(v / roundTo) * roundTo : v;
        }

        /// <summary>分段控件：命中索引（-1 表示未命中）。</summary>
        public static int SegmentIndex(Rect rect, int count, float x)
        {
            if (count <= 0) return -1;
            float w = rect.width / count;
            int i = Mathf.FloorToInt((x - rect.x) / w);
            return (i < 0 || i >= count) ? -1 : i;
        }

        /// <summary>分段控件第 index 段的矩形（下标钳制）。</summary>
        public static Rect SegmentRect(Rect rect, int count, int index)
        {
            int n = Mathf.Max(1, count);
            float w = rect.width / n;
            return new Rect(rect.x + Mathf.Clamp(index, 0, n - 1) * w, rect.y, w, rect.height);
        }

        /// <summary>防抖是否就绪（距上次输入已超过 delay 秒）。</summary>
        public static bool DebounceReady(float now, float lastInputTime, float delay)
            => now - lastInputTime >= delay;

        /// <summary>
        /// 选择件单行落位（方案 §5.12）。<b>纯函数</b> ⇒ 可离线单测（只做区间判定，不触引擎）。
        ///
        /// <para>判定序（D0 → D1 → D2 → D6）：
        /// <c>width ≥ box + gap + labelWidth</c> ⇒ <see cref="SelectionFit.Full"/>；
        /// 否则若 <c>width ≥ box + gap + ellipsisWidth</c> ⇒ <see cref="SelectionFit.Ellipsis"/>；
        /// 否则若 <c>width ≥ box</c> ⇒ <see cref="SelectionFit.BoxOnly"/>（<b>盒必须保留</b>，否则无法表达状态）；
        /// 否则 <see cref="SelectionFit.Placeholder"/>。<c>rowHeight &lt; 14f</c> ⇒ <see cref="SelectionFit.Omit"/>。</para>
        ///
        /// <para><b>缩盒不在本方法内</b>：<c>rowHeight &lt; box + 2</c> 时的缩盒（至 <c>rowHeight − 2</c>，下限 12）
        /// 是调用方的几何职责（<c>InputAtoms.SelectionRow</c>），本方法只给落位档。</para>
        /// </summary>
        /// <param name="rowHeight">行高（已含调用方的下限保护结果）。</param>
        /// <param name="box">盒边长基准（= <c>Measure.RowHeight(Tokens.FontLabel)</c>）。</param>
        /// <param name="width">行宽。</param>
        /// <param name="labelWidth">标签实测宽（<c>Measure.Width(label, Tokens.FontLabel)</c>）。</param>
        /// <param name="gap">盒-标签间距（<c>Tokens.SpaceXs</c>）。</param>
        /// <param name="ellipsisWidth">省略号字形自身的实测宽（用作 D1→D2 的派生阈值）。</param>
        public static SelectionFit ResolveSelection(float rowHeight, float box, float width,
            float labelWidth, float gap, float ellipsisWidth)
        {
            if (rowHeight < SelectionOmitRowHeight) return SelectionFit.Omit;
            if (width >= box + gap + labelWidth) return SelectionFit.Full;
            if (width >= box + gap + ellipsisWidth) return SelectionFit.Ellipsis;
            if (width >= box) return SelectionFit.BoxOnly;
            return SelectionFit.Placeholder;
        }

        /// <summary>盒内符号避让与勾线宽（<c>Tokens.RuleHeight × 2</c>，§5.12）。</summary>
        public static float MarkStroke => Tokens.RuleHeight * 2f;

        /// <summary>省略该行的行高阈值（14f，§5.12 D6）。</summary>
        public const float SelectionOmitRowHeight = 14f;

        /// <summary>缩盒下限（12f，§5.12 D3）。</summary>
        public const float SelectionMinBox = 12f;
    }

    /// <summary>选择件落位档（方案 §5.12 降级阶梯）。</summary>
    public enum SelectionFit
    {
        /// <summary>D0：盒 + 完整标签。</summary>
        Full,
        /// <summary>D1：盒 + 省略号标签（附完整悬停提示）。</summary>
        Ellipsis,
        /// <summary>D2：仅盒（藏标签；<b>盒必须保留</b>，否则无法表达状态）。</summary>
        BoxOnly,
        /// <summary>D6：宽度不足盒 ⇒ 画「—」占位。</summary>
        Placeholder,
        /// <summary>D6：行高不足 ⇒ 省略该行（不绘制、不命中）。</summary>
        Omit
    }

    /// <summary>
    /// 基础输入控件（S3-1）：分段选择、开关、步进器、滑条、搜索框、标签多选、键位显示、六态规范；
    /// 以及 v1.3.0 新增的选择件族（勾选 / 三态勾选 / 纵向单选，方案 §5.12）。
    /// 全部<b>无状态</b>——值/缓冲/时间戳均由调用方持有并传入（GATE-UI）。
    /// 已知官方要点：<b>Widgets.ButtonText 没有 disabled 参数，禁用走 active:false</b>；
    /// <b>HorizontalSlider 是 ref 语义</b>（不是返回 float）。
    ///
    /// <para><b>本批（v1.3.0）两条硬纪律</b>：
    /// ① 选择件<b>全部自绘、不桥原版</b>（<c>Widgets.Checkbox/RadioButton/CheckboxMulti</c> 会<b>无条件</b>响悬停音、
    /// 且 <c>disabled:true</c> 不灰化，与统一禁用语义直接冲突）；
    /// ② 禁用态<b>不置 <c>GUI.enabled=false</c></b>、100% 自绘灰化（§5.3 Disabled 行）。</para>
    /// </summary>
    public static class InputAtoms
    {
        /// <summary>分段选择（互斥单选取代原版缺失的 RadioButtonGroup）；返回是否变化。</summary>
        public static bool Segmented(Rect rect, string[] options, ref int index)
        {
            if (options == null || options.Length == 0) return false;
            int n = options.Length;
            RUIF.Draw.Box(rect, Tokens.PanelRaised);
            RUIF.Draw.Border(rect, Tokens.BorderSoft);
            bool changed = false;
            for (int i = 0; i < n; i++)
            {
                Rect cell = InputFmt.SegmentRect(rect, n, i);
                bool sel = i == index;
                if (sel) RUIF.Draw.Box(cell, Tokens.AccentSoft);
                if (i > 0) RUIF.Draw.LineV(cell.x, rect.y, rect.height, Tokens.BorderSoft);
                RUIF.Draw.Label(cell, options[i] ?? string.Empty, Tokens.FontLabel,
                    sel ? Tokens.Accent : Tokens.Muted, TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(cell)) { index = i; changed = true; }
            }
            return changed;
        }

        /// <summary>
        /// 分段选择（补 <paramref name="enabled"/> / <paramref name="tone"/> / 焦点入口）；返回是否变化。
        ///
        /// <para><b>SemVer 安全</b>：旧 3 参签名<b>原样保留</b>；C# 重载解析优先「填默认值更少」者，
        /// 故 <c>Segmented(rect, options, ref index)</c> 仍精确匹配旧重载，无歧义（§3.4）。</para>
        ///
        /// <para><b>A 类一次性正确性要求（§5.3 末段）</b>：选中段的 fill / text <b>必须</b>走
        /// <c>ToneSoft(tone)</c> / <c>Tone(tone)</c> —— 若沿用既有实现的 <c>AccentSoft</c> / <c>Accent</c>，
        /// 则 <c>tone: Danger</c> 的选中段仍是蓝色 accent ⇒ <c>tone</c> 形同虚设。
        /// <b>分隔线保持 <see cref="Tokens.BorderSoft"/></b>（分段件的形态差异，非配色偏差）。</para>
        /// </summary>
        /// <param name="rect">控件矩形。</param>
        /// <param name="options">各段文案（调用方自行 <c>Tr.Get</c>）。</param>
        /// <param name="index">选中下标（由调用方持有，组件零状态）。</param>
        /// <param name="enabled">是否可用。禁用时<b>不消费事件、不响悬停音</b>，并自绘灰化（剥除 tone）。</param>
        /// <param name="tone">语义色调（决定选中段的填充与文字色）。</param>
        /// <param name="focusRing">强制画焦点环（作用域 = 整件 <paramref name="rect"/>）。</param>
        /// <param name="controlName">键盘通路名（登记在<b>首段</b>的控件创建前 —— 整组的焦点入口）。</param>
        public static bool Segmented(Rect rect, string[] options, ref int index, bool enabled = true,
            ButtonTone tone = ButtonTone.Default, bool focusRing = false, string controlName = null)
        {
            if (options == null || options.Length == 0) return false;
            int n = options.Length;
            Color selFill = enabled ? ButtonFmt.ToneSoft(tone) : Tokens.Panel;
            Color selText = enabled ? ButtonFmt.Tone(tone) : Tokens.Dim;
            Color idleText = enabled ? Tokens.Muted : Tokens.Dim;

            RUIF.Draw.Box(rect, enabled ? Tokens.PanelRaised : Tokens.Panel);   // §5.3 Disabled 行：fill → Panel
            RUIF.Draw.Border(rect, Tokens.BorderSoft);                          // Disabled 剥除 tone ⇒ BorderSoft
            bool changed = false;
            for (int i = 0; i < n; i++)
            {
                Rect cell = InputFmt.SegmentRect(rect, n, i);
                bool sel = i == index;
                if (sel) RUIF.Draw.Box(cell, selFill);
                if (i > 0) RUIF.Draw.LineV(cell.x, rect.y, rect.height, Tokens.BorderSoft);
                RUIF.Draw.Label(cell, options[i] ?? string.Empty, Tokens.FontLabel,
                    sel ? selText : idleText, TextAnchor.MiddleCenter);

                // 命名与控件创建必须【紧邻】；命中总是调用（禁用也调用，保 control-ID 序稳定）
                if (i == 0 && controlName != null) GUI.SetNextControlName(controlName);
                bool hit = Widgets.ButtonInvisible(cell, doMouseoverSound: enabled);
                if (ButtonFmt.Clicked(enabled, hit)) { index = i; changed = true; }
            }
            if (enabled && ButtonFmt.ShouldRing(focusRing,
                    controlName != null && GUI.GetNameOfFocusedControl() == controlName))
                ButtonAtoms.FocusRing(rect, tone);
            return changed;
        }

        /// <summary>
        /// 开关（Track + Knob）；返回<b>是否切换</b>。
        ///
        /// <para><b>§9.1 既有缺陷修复（行为修复，无 API 变更）</b>：原实现用 <c>GUI.enabled</c> 决定
        /// 「标签画不画」，而 <c>Widgets.ButtonInvisible</c> <b>不检 <c>GUI.enabled</c></b>（实测：
        /// 音效只受 <c>doMouseoverSound</c> 控制）⇒ 禁用时<b>仍可切换、却无标签</b>。现改为：
        /// ① 标签<b>照常绘制</b>；② 命中判定<b>总是调用 + 事后 AND</b> ⇒ 禁用时<b>不产生状态变更、不消费事件</b>；
        /// ③ 禁用时按 §5.3 的 Disabled 行通道<b>自绘灰化</b>（<b>绝不置 <c>GUI.enabled=false</c></b>）。</para>
        ///
        /// <para><b>几何保留说明</b>：Track/Knob 的既有字面尺寸（36/18/15/2/1.5）<b>原样保留</b> ——
        /// §9.1 明示本批只修行为、不做几何重制；令牌化属另立改动（已登记，见回报）。</para>
        /// </summary>
        public static bool ToggleSwitch(Rect rect, ref bool on)
        {
            Rect track = new Rect(rect.x, rect.center.y - 9f, 36f, 18f);
            bool enabled = GUI.enabled;
            RUIF.Draw.Box(track, !enabled ? Tokens.BorderSoft : (on ? Tokens.Accent : Tokens.BorderSoft));
            float k = 15f;
            Rect knob = new Rect(on ? track.xMax - k - 2f : track.x + 2f, track.y + 1.5f, k, k);
            RUIF.Draw.Box(knob, !enabled ? Tokens.Panel : (on ? Tokens.Window : Tokens.PanelRaised));

            // 标签照常绘制（消除「禁用却无标签」的怪异）；宽度不足时 Draw.Label 自然不可见
            const float LabelGap = 6f;
            float labelWidth = Mathf.Max(0f, rect.width - (track.width + LabelGap));
            RUIF.Draw.Label(new Rect(track.xMax + LabelGap, rect.y, labelWidth, rect.height),
                on ? Tr.Get("RUIF.Comp.Common.On") : Tr.Get("RUIF.Comp.Common.Off"), Tokens.FontLabel,
                !enabled ? Tokens.Dim : (on ? Tokens.Text : Tokens.Muted), TextAnchor.MiddleLeft);

            bool hit = Widgets.ButtonInvisible(rect, doMouseoverSound: enabled);
            bool changed = ButtonFmt.Clicked(enabled, hit);
            if (changed) on = !on;
            return changed;
        }

        // ============================================================ v1.3.0 选择件族（方案 §5.12）

        /// <summary>
        /// 勾选框（<b>方形框 + 几何对勾、控件在左 / 标签在右</b>）；返回<b>是否变化</b>。
        /// <para>对原版的两处<b>有意偏离</b>（均登记）：① 原版 <c>CheckboxLabeled</c> 盒<b>靠右</b>（<c>rect.xMax − 24f</c>），
        /// 本件<b>左置</b>（与既有 <c>ToggleSwitch</c> 同向、与纵向 <c>RadioGroup</c> 自然左对齐）；
        /// ② 盒边长取 <c>Measure.RowHeight(Tokens.FontLabel)</c>（≈18）而非原版字面 <c>24f</c>。</para>
        /// </summary>
        /// <param name="rect"><b>整行</b>矩形（可点区 = 整行，非仅盒子）。高度不足 14f 时按 §5.12 D6 省略该行。</param>
        /// <param name="label">标签文案（调用方自行 <c>Tr.Get</c>；放不下时走省略号 + 完整悬停提示）。</param>
        /// <param name="on">二态值，由调用方持有。</param>
        /// <param name="enabled">是否可用（禁用时不产生状态变更、不消费事件）。</param>
        /// <param name="tone">语义色调（选中 = 三通道 Tone 化，逐格 == <c>StateSample.Selected</c>）。</param>
        /// <param name="focusRing">强制画焦点环（作用域 = <b>整行可点区</b>，§5.12 显式偏离 <c>StateSample</c>）。</param>
        /// <param name="controlName">键盘通路名（<c>null</c> = 不登记）。</param>
        public static bool Checkbox(Rect rect, string label, ref bool on, bool enabled = true,
            ButtonTone tone = ButtonTone.Default, bool focusRing = false, string controlName = null)
        {
            bool click = SelectionRow(rect, label, on, false, enabled, tone, focusRing, controlName, false);
            if (!click) return false;
            on = !on;
            return true;
        }

        /// <summary>
        /// 三态勾选框（父节点「部分选中」）；返回<b>是否变化</b>。
        /// <para>循环序 <c>Off → On → Partial → Off</c>。<see cref="TriState.Partial"/> 的视觉 =
        /// <b>盒内居中实心方块</b>（<c>box × 0.5</c>），且<b>标签保持 <c>Text</c> 不 Tone 化</b>
        /// —— 半选是混合态，标签 Tone 化会与 <see cref="TriState.On"/> 混淆（§5.12 对 §5.3 的唯一扩展）。</para>
        /// </summary>
        /// <param name="rect">整行矩形。</param>
        /// <param name="label">标签文案。</param>
        /// <param name="state">三态值，由调用方持有。</param>
        /// <param name="enabled">是否可用。</param>
        /// <param name="tone">语义色调。</param>
        /// <param name="focusRing">强制画焦点环。</param>
        /// <param name="controlName">键盘通路名（<c>null</c> = 不登记）。</param>
        public static bool CheckboxTri(Rect rect, string label, ref TriState state, bool enabled = true,
            ButtonTone tone = ButtonTone.Default, bool focusRing = false, string controlName = null)
        {
            bool click = SelectionRow(rect, label, state == TriState.On, state == TriState.Partial,
                enabled, tone, focusRing, controlName, false);
            if (!click) return false;
            state = state == TriState.Off ? TriState.On : (state == TriState.On ? TriState.Partial : TriState.Off);
            return true;
        }

        /// <summary>
        /// 纵向单选组（每行一个圆点，至多一个 <see cref="TriState.On"/> 形态的选中）；返回<b>是否变化</b>。
        ///
        /// <para><b>圆点实现（§5.12 主理人裁决）</b>：<c>TexFmt.FillDisc</c> + <c>TexCache</c> +
        /// <c>Draw.Color(tone)</c> 作用域着色 ⇒ <b>1 张中性白 disc 覆盖 6 态 × 4 tone</b>
        /// （按色建键会导致键爆炸，故<b>不走</b> <c>TexCache.KeySector</c>）；纹理不可用时退 <b>12 段折线</b>。
        /// ⛔ 拒绝用 4px 方块「近似」圆。</para>
        ///
        /// <para><b>空间不足</b>：总需高 <c>n × rowH</c>；不足则 <b>D2 减少条目</b>（按剩余高截断 + 「还有 N 条」），
        /// <b>禁止压缩 <c>rowH</c></b>（压缩必切字）。</para>
        /// </summary>
        /// <param name="rect">整组矩形（纵向排布起点）。</param>
        /// <param name="labels">各选项文案（调用方自行 <c>Tr.Get</c>）。</param>
        /// <param name="index">选中下标，由调用方持有（越界值不写入，只影响绘制）。</param>
        /// <param name="rowH">行高；<c>null</c> ⇒ 按 <c>Measure.ControlHeight(Tokens.FontLabel)</c> 实取。
        /// 低于 <c>Measure.RowHeight(Tokens.FontLabel)</c> 时<b>上调并报一次错</b>（<b>不静默夹取</b>）。</param>
        /// <param name="enabled">是否可用。</param>
        /// <param name="tone">语义色调。</param>
        /// <param name="focusRing">强制画焦点环（作用域 = 整组 <paramref name="rect"/>）。</param>
        /// <param name="controlName">键盘通路名（登记在<b>首行</b>的控件创建前）。</param>
        public static bool RadioGroup(Rect rect, string[] labels, ref int index, float? rowH = null,
            bool enabled = true, ButtonTone tone = ButtonTone.Default, bool focusRing = false, string controlName = null)
        {
            if (labels == null || labels.Length == 0) return false;

            float row = ResolveRowHeight(rowH, rect.height);
            if (row < InputFmt.SelectionOmitRowHeight) return false;      // §5.12 D6：行高不足 ⇒ 省略

            bool changed = false;
            int drawn = 0;
            for (int i = 0; i < labels.Length; i++)
            {
                // D2：剩余高不足 ⇒ 减少条目（禁止压缩 rowH）
                if (rect.y + row * (i + 1) > rect.yMax + 0.01f) break;
                Rect line = new Rect(rect.x, rect.y + row * i, rect.width, row);
                bool click = SelectionRow(line, labels[i], index == i, false, enabled, tone,
                    focusRing, i == 0 ? controlName : null, true);
                drawn++;
                if (click) { index = i; changed = true; }
            }

            int hidden = labels.Length - drawn;
            Rect rest = new Rect(rect.x, rect.y + row * drawn, rect.width, Mathf.Max(0f, rect.yMax - (rect.y + row * drawn)));
            if (hidden > 0 && rest.height >= Measure.RowHeight(Tokens.FontLabel))
                RUIF.Draw.Label(rest, Tr.Get("RUIF.Comp.Common.More", hidden), Tokens.FontLabel,
                    enabled ? Tokens.Muted : Tokens.Dim, TextAnchor.MiddleLeft);

            return changed;
        }

        /// <summary>数值步进器（− 值 +）；返回是否变化。</summary>
        public static bool Stepper(Rect rect, ref int value, int min, int max, int step = 1)
        {
            var row = Layout.Row(rect, 2f);
            bool changed = false;
            if (Widgets.ButtonText(row.Take(26f), "−")) { value = InputFmt.StepValue(value, -step, min, max); changed = true; }
            Rect mid = row.Rest;
            RUIF.Draw.Box(mid, Tokens.PanelRaised);
            RUIF.Draw.Border(mid, Tokens.BorderSoft);
            RUIF.Draw.Label(mid, value.ToString(), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
            if (Widgets.ButtonText(row.Take(0f), string.Empty)) { }
            // 加号按钮（放置在最右）
            var right = new Rect(rect.xMax - 26f, rect.y, 26f, rect.height);
            if (Widgets.ButtonText(right, "+")) { value = InputFmt.StepValue(value, step, min, max); changed = true; }
            return changed;
        }

        /// <summary>
        /// 滑条 + 实时数值（走官方 <b>Widgets.HorizontalSlider（ref 语义）</b>）；返回是否拖动中变化。
        /// </summary>
        public static bool SliderWithValue(Rect rect, ref float value, float min, float max,
            float roundTo = -1f, Func<float, string> format = null)
        {
            float before = value;
            var row = Layout.Row(rect, Tokens.SpaceXs);
            Rect sliderRect = row.Rest;
            Widgets.HorizontalSlider(sliderRect, ref value, new FloatRange(min, max), null, roundTo);
            string s = format != null ? format(value) : value.ToString("0.#");
            RUIF.Draw.Label(new Rect(sliderRect.xMax - 60f, sliderRect.y - 18f, 60f, 18f),
                s, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
            if (Mathf.Abs(value - before) > 0.0001f)
            {
                RUIF.Draw.Box(new Rect(sliderRect.x, sliderRect.y + sliderRect.height + 2f,
                    sliderRect.width, 2f), Tokens.Accent);
            }
            return Mathf.Abs(value - before) > 0.0001f;
        }

        /// <summary>
        /// 搜索框（防抖 200ms）：buffer 即时回显，query 在停止输入后才更新；返回 query 是否刚更新。
        /// </summary>
        public static bool SearchField(Rect rect, ref string query, ref string buffer,
            string placeholder, ref float lastInputTime, float delay = 0.2f)
        {
            string prev = buffer;
            buffer = Widgets.TextField(rect, buffer);
            bool changed = false;
            float now = Time.realtimeSinceStartup;
            if (buffer != prev) lastInputTime = now;
            else if (InputFmt.DebounceReady(now, lastInputTime, delay) && buffer != query)
            {
                query = buffer;
                changed = true;
            }
            if (string.IsNullOrEmpty(buffer) && !string.IsNullOrEmpty(placeholder))
                RUIF.Draw.Label(rect, placeholder, Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
            return changed;
        }

        /// <summary>标签多选（chips）；picked 由调用方持有；返回是否有变化。</summary>
        public static bool TagPicker(Rect rect, string[] all, bool[] picked, float gap = 6f)
        {
            if (all == null || picked == null) return false;
            bool changed = false;
            var row = Layout.Row(rect, gap);
            for (int i = 0; i < all.Length; i++)
            {
                bool on = i < picked.Length && picked[i];
                float w = 0f;
                using (Draw.Font(Tokens.FontLabel)) w = Text.CalcSize(all[i] ?? string.Empty).x + 18f;
                Rect chip = row.Take(w);
                RUIF.Draw.Box(chip, on ? Tokens.AccentSoft : Tokens.PanelRaised);
                RUIF.Draw.Border(chip, on ? Tokens.Accent : Tokens.BorderSoft);
                RUIF.Draw.Label(chip, (on ? "✓ " : "") + (all[i] ?? string.Empty), Tokens.FontLabel,
                    on ? Tokens.Accent : Tokens.Muted, TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(chip) && i < picked.Length) { picked[i] = !on; changed = true; }
            }
            return changed;
        }

        /// <summary>
        /// 键位显示 + 改键触发（⚠️ 捕获需官方 KeyPrefs 流程与焦点仲裁，此处只做显示与点击回调）。
        /// currentKey 由调用方从 KeyBindingDef 取得，避免依赖未核实字段。
        /// </summary>
        public static bool HotkeyBind(Rect rect, string label, string currentKey)
        {
            var row = Layout.Row(rect, Tokens.SpaceXs);
            RUIF.Draw.Label(row.Take(90f), label ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Rect keyBox = row.Take(84f);
            RUIF.Draw.Box(keyBox, Tokens.PanelRaised);
            RUIF.Draw.Border(keyBox, Tokens.BorderSoft);
            RUIF.Draw.Label(keyBox, currentKey ?? "—", Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleCenter);
            return Widgets.ButtonText(row.Take(64f), Tr.Get("RUIF.Comp.Common.Rebind"), true, false, true);
        }

        /// <summary>六态规范示意（默认/悬停/按下/选中/禁用/焦点）——用于验收状态齐全。</summary>
        public static void StateSample(Rect rect, string label, InputState state)
        {
            Color border = Tokens.BorderSoft;
            Color fill = Tokens.PanelRaised;
            Color text = Tokens.Text;
            switch (state)
            {
                case InputState.Hover: fill = Tokens.CardHover; border = Tokens.Accent; break;
                case InputState.Pressed: fill = Tokens.AccentSoft; border = Tokens.Accent; text = Tokens.Accent; break;
                case InputState.Selected: fill = Tokens.AccentSoft; border = Tokens.Accent; text = Tokens.Accent; break;
                case InputState.Disabled: fill = Tokens.Panel; text = Tokens.Dim; break;
                case InputState.Focused: border = Tokens.Accent; break;
            }
            RUIF.Draw.Box(rect, fill);
            RUIF.Draw.Border(rect, border);
            if (state == InputState.Focused)
                RUIF.Draw.Border(new Rect(rect.x - 2f, rect.y - 2f, rect.width + 4f, rect.height + 4f),
                    Draw.WithAlpha(Tokens.Accent, 0.55f));
            RUIF.Draw.Label(rect, (label ?? string.Empty) + "：" + state, Tokens.FontLabel, text, TextAnchor.MiddleCenter);
        }

        // ============================================================ 选择件共用实现（§5.12）

        /// <summary>
        /// 行高解析（§5.12，<c>RadioGroup</c> 专用）：<c>null</c> ⇒ <c>Measure.ControlHeight(Tokens.FontLabel)</c> 实取；
        /// 低于 <c>Measure.RowHeight(Tokens.FontLabel)</c> 时<b>上调 + <c>Log.ErrorOnce</c> 报一次</b>（<b>不静默夹取</b>）；
        /// 最后按容器可用高收敛（不溢出容器，与 <c>Layout.Take</c> 同风范）。
        /// </summary>
        private static float ResolveRowHeight(float? rowH, float availableHeight)
        {
            float minRow = Measure.RowHeight(Tokens.FontLabel);
            float want = rowH ?? Measure.ControlHeight(Tokens.FontLabel);
            if (want < minRow)
            {
                Log.ErrorOnce("RUIF.InputAtoms: rowH=" + want + " 低于 Measure.RowHeight(Tokens.FontLabel)=" + minRow
                    + "，已上调（原值被忽略）。请由调用方给出合法行高。",
                    GenText.StableStringHash("RUIF.InputAtoms.RowHeightFloor"));
                want = minRow;
            }
            // 不溢出容器（与 Layout.Take 同风范）
            return Mathf.Min(want, availableHeight);
        }

        /// <summary>
        /// 选择件单行（盒 / 圆点 + 标签 + <b>整行可点区</b> + 焦点环）。返回<b>是否被点击</b>（已过 <c>enabled</c> 门禁）。
        /// <para>几何<b>全部 <see cref="Measure"/> 派生</b>（零字面量）：盒边长 = <c>RowHeight(FontLabel)</c>、
        /// 行高 = 调用方给定矩形高（<c>RadioGroup</c> 默认传 <c>ControlHeight(FontLabel)</c>）、
        /// 间距 = <c>SpaceXs</c>、勾线宽 = <c>RuleHeight × 2</c>。</para>
        /// </summary>
        private static bool SelectionRow(Rect rect, string label, bool isOn, bool isPartial, bool enabled,
            ButtonTone tone, bool focusRing, string controlName, bool radio)
        {
            // 行高 = 给定矩形高（RadioGroup 传进来的每行矩形已按 §5.12 的 rowH 切好；Checkbox 的 rect 即整行）
            float row = rect.height;
            if (row < InputFmt.SelectionOmitRowHeight) return false;      // D6：省略该行

            float boxBase = Measure.RowHeight(Tokens.FontLabel);
            float box = boxBase;
            if (row < boxBase + InputFmt.MarkStroke)                       // D3：缩盒（下限 12，不触边框）
                box = Mathf.Max(InputFmt.SelectionMinBox, row - InputFmt.MarkStroke);

            // 可点区 = 整行（盒 + gap + 标签并集），非仅盒子
            Rect clickRect = rect;
            Rect boxRect = new Rect(rect.x, rect.y + (row - box) * 0.5f, box, box);

            // ---- 状态与通道（与按钮族同一张 §5.3 表：Partial 也走 Selected 通道，仅标签色不同）----
            bool selected = enabled && (isOn || isPartial);
            bool hovered = enabled && Mouse.IsOver(clickRect);
            bool pressed = hovered && ButtonAtoms.PressedNow();
            InputState state = ButtonFmt.Resolve(hovered, pressed, selected, enabled);
            ButtonFmt.Channels(state, tone, out Color fill, out Color border, out Color text, out Color symbol);

            float gap = Tokens.SpaceXs;
            float labelWidth = Measure.Width(label ?? string.Empty, Tokens.FontLabel);
            float ellipsisWidth = Measure.Width("…", Tokens.FontLabel);
            SelectionFit fit = InputFmt.ResolveSelection(row, box, rect.width, labelWidth, gap, ellipsisWidth);

            // ---- 指示器（D6 无空间放盒 ⇒ 不画盒，只画「—」占位）----
            if (fit != SelectionFit.Placeholder)
            {
                Rect boxSnap = Px.Snap(boxRect);
                if (radio) DrawRadio(boxSnap, fill, border, (isOn || isPartial) ? symbol : (Color?)null);
                else
                {
                    RUIF.Draw.Box(boxSnap, fill);
                    RUIF.Draw.Border(boxSnap, border);
                    if (isOn) DrawCheck(boxSnap, symbol);
                    else if (isPartial) DrawPartialSquare(boxSnap, symbol);
                }
            }

            // ---- 标签（Partial 保持 Text：半选不宣称「已选」）----
            if (fit == SelectionFit.Placeholder)
            {
                RUIF.Draw.Label(clickRect, "—", Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
            }
            else if (fit != SelectionFit.BoxOnly)
            {
                var labelRect = new Rect(boxRect.xMax + gap, rect.y,
                    Mathf.Max(0f, rect.width - box - gap), row);
                Color labelColor = isPartial && enabled ? Tokens.Text : text;
                if (fit == SelectionFit.Full)
                    RUIF.Draw.Label(labelRect, label ?? string.Empty, Tokens.FontLabel, labelColor, TextAnchor.MiddleLeft);
                else
                {
                    RUIF.Draw.EllipsisLabel(labelRect, label ?? string.Empty, Tokens.FontLabel, labelColor, TextAnchor.MiddleLeft);
                    if (!string.IsNullOrEmpty(label) && Mouse.IsOver(clickRect)) TooltipHandler.TipRegion(clickRect, label);
                }
            }

            // ---- 命中：整行可点区；命名与控件创建【紧邻】；总是调用 ----
            if (controlName != null) GUI.SetNextControlName(controlName);
            bool hit = Widgets.ButtonInvisible(clickRect, doMouseoverSound: enabled);

            // ---- 焦点环：作用域 = 整行可点区（§5.12 显式偏离 StateSample 的 rect 作用域）----
            if (enabled && ButtonFmt.ShouldRing(focusRing,
                    controlName != null && GUI.GetNameOfFocusedControl() == controlName))
                ButtonAtoms.FocusRing(clickRect, tone);

            if (!enabled && Mouse.IsOver(clickRect)) TooltipHandler.TipRegion(clickRect, Tr.Get("RUIF.Comp.Button.Disabled"));
            return ButtonFmt.Clicked(enabled, hit);
        }

        // ---- 字形（§5.12）：几何优先，纹理仅作「1 张中性白覆盖全色」的载体 ----

        private const int KindCheck = 0x60;      // TexCache.KeyOf 键族（标记位隔离：与 Key/KeyData 空间不撞）
        private const int KindDisc = 0x61;

        /// <summary>
        /// 几何对勾（三点折线，坐标逐值取自 §5.12）：<c>(0.26b,0.52b) → (0.44b,0.70b) → (0.74b,0.28b)</c>。
        /// <para><b>⛔ 不用字符 ✓</b>：状态符号须与盒精确居中且<b>跨字体一致</b>。
        /// <b>⛔ 不用 <c>Widgets.DrawLine</c></b>：它改 GL 矩阵（<c>GL.PushMatrix/MultMatrix</c>），
        /// <c>GUI.matrix</c> 的滚动平移不被感知 ⇒ <b>滚动视图内会「飞散」</b>（既有实证）。
        /// 本实现把折线算进<b>中性白</b>像素缓冲 → 上屏一次 <c>GUI.DrawTexture</c> → <c>Draw.Color</c> 着色。</para>
        /// </summary>
        private static void DrawCheck(Rect box, Color color)
        {
            int size = Mathf.Clamp(Mathf.RoundToInt(box.width), 8, 64);
            long key = TexCache.KeyOf(KindCheck, size);
            Texture2D tex;
            if (!TexCache.TryGet(key, out tex))
            {
                var px = new Color32[size * size];
                float b = size, stroke = InputFmt.MarkStroke;
                // 缓冲 y 轴朝上（Texture2D 原点在左下）⇒ 屏幕 y 需翻转
                TexFmt.Line(px, size, size, 0.26f * b, b - 0.52f * b, 0.44f * b, b - 0.70f * b, Color.white, stroke);
                TexFmt.Line(px, size, size, 0.44f * b, b - 0.70f * b, 0.74f * b, b - 0.28f * b, Color.white, stroke);
                tex = TexCache.Publish(key, size, size, px, "SelCheck");
            }
            if (tex == null) { StampCheck(box, color); return; }      // 纹理不可用 ⇒ 降级
            using (RUIF.Draw.Color(color)) GUI.DrawTexture(box, tex);
        }

        /// <summary>半选：<b>居中实心方块</b>（边长 <c>box × 0.5</c>，不触边框）。</summary>
        private static void DrawPartialSquare(Rect box, Color color)
        {
            float side = box.width * 0.5f;
            Rect sq = Px.Snap(new Rect(box.center.x - side * 0.5f, box.center.y - side * 0.5f, side, side));
            RUIF.Draw.Box(sq, color);
        }

        /// <summary>
        /// 圆点（单选框）：外盘 = 描边色、内盘 = 填充色（形成 1px 环）、选中 = 直径 <c>box × 0.45</c> 的实心盘。
        /// <para>走 §5.12 裁决的 <c>TexFmt.FillDisc</c> + <c>TexCache</c> + <c>Draw.Color</c> 作用域着色 ——
        /// 纹理是<b>中性白</b>，颜色不进键 ⇒ 1 张覆盖 6 态 × 4 tone。</para>
        /// </summary>
        private static void DrawRadio(Rect box, Color fill, Color border, Color? dot)
        {
            DrawDisc(box, border);
            float inset = Tokens.RuleHeight;
            DrawDisc(new Rect(box.x + inset, box.y + inset, Mathf.Max(1f, box.width - inset * 2f),
                Mathf.Max(1f, box.height - inset * 2f)), fill);
            if (!dot.HasValue) return;
            float d = box.width * 0.45f;
            DrawDisc(new Rect(box.center.x - d * 0.5f, box.center.y - d * 0.5f, d, d), dot.Value);
        }

        /// <summary>中性白圆盘（缓存键 = <c>(kind, 直径)</c>；颜色由 <c>Draw.Color</c> 作用域提供）。</summary>
        private static void DrawDisc(Rect rect, Color color)
        {
            int size = Mathf.Clamp(Mathf.RoundToInt(Mathf.Min(rect.width, rect.height)), 6, 64);
            long key = TexCache.KeyOf(KindDisc, size);
            Texture2D tex;
            if (!TexCache.TryGet(key, out tex))
            {
                var px = new Color32[size * size];
                float rad = size * 0.5f - 0.5f;
                TexFmt.FillDisc(px, size, size, size * 0.5f, size * 0.5f, rad, rad, Color.white);
                tex = TexCache.Publish(key, size, size, px, "SelDisc");
            }
            if (tex == null) { StampDisc(rect, color); return; }      // 纹理不可用 ⇒ 12 段折线（TexAtoms 既有范式）
            using (RUIF.Draw.Color(color)) GUI.DrawTexture(Px.Snap(rect), tex);
        }

        /// <summary>纹理不可用时的对勾退化：把两段折线按 1px 步长用 <c>Draw.Box</c> 盖印（零 GL / 零矩阵依赖）。</summary>
        private static void StampCheck(Rect box, Color color)
        {
            Rect b = Px.Snap(box);
            StampLine(new Vector2(b.x + 0.26f * b.width, b.y + 0.52f * b.height),
                      new Vector2(b.x + 0.44f * b.width, b.y + 0.70f * b.height), color, InputFmt.MarkStroke);
            StampLine(new Vector2(b.x + 0.44f * b.width, b.y + 0.70f * b.height),
                      new Vector2(b.x + 0.74f * b.width, b.y + 0.28f * b.height), color, InputFmt.MarkStroke);
        }

        /// <summary>纹理不可用时的圆盘退化：<b>12 段折线</b>近似（照 <c>TexAtoms.FallbackGradient</c> 的 12 段范式）。</summary>
        private static void StampDisc(Rect rect, Color color)
        {
            const int Segments = 12;
            Vector2 c = rect.center;
            float rad = Mathf.Min(rect.width, rect.height) * 0.5f;
            Vector2 prev = new Vector2(c.x + rad, c.y);
            for (int i = 1; i <= Segments; i++)
            {
                float a = Mathf.PI * 2f * i / Segments;
                Vector2 cur = new Vector2(c.x + Mathf.Cos(a) * rad, c.y + Mathf.Sin(a) * rad);
                StampLine(prev, cur, color, Tokens.RuleHeight);
                prev = cur;
            }
        }

        /// <summary>按 1px 步长盖印一条线段（<c>Draw.Box</c> 限定为轴对齐矩形 ⇒ 用正方形戳逐步逼近）。</summary>
        private static void StampLine(Vector2 a, Vector2 b, Color color, float width)
        {
            Vector2 p = new Vector2(Px.Floor(a.x), Px.Floor(a.y));
            Vector2 q = new Vector2(Px.Floor(b.x), Px.Floor(b.y));
            int steps = Mathf.Max(1, Mathf.RoundToInt(Mathf.Max(Mathf.Abs(q.x - p.x), Mathf.Abs(q.y - p.y))));
            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                RUIF.Draw.Box(new Rect(Px.Floor(p.x + (q.x - p.x) * t), Px.Floor(p.y + (q.y - p.y) * t), width, width), color);
            }
        }
    }
}
