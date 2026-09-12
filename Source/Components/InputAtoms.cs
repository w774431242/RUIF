using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>输入控件的六态（GATE-UI：状态齐全——默认/悬停/按下/选中/禁用/焦点）。</summary>
    public enum InputState { Default, Hover, Pressed, Selected, Disabled, Focused }

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
    }

    /// <summary>
    /// 基础输入控件（S3-1）：分段选择、开关、步进器、滑条、搜索框、标签多选、键位显示、六态规范。
    /// 全部<b>无状态</b>——值/缓冲/时间戳均由调用方持有并传入（GATE-UI）。
    /// 已知官方要点：<b>Widgets.ButtonText 没有 disabled 参数，禁用走 active:false</b>；
    /// <b>HorizontalSlider 是 ref 语义</b>（不是返回 float）。
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

        /// <summary>开关（Track + Knob）；返回是否切换。</summary>
        public static bool ToggleSwitch(Rect rect, ref bool on)
        {
            Rect track = new Rect(rect.x, rect.center.y - 9f, 36f, 18f);
            bool enabled = GUI.enabled;
            RUIF.Draw.Box(track, on ? Tokens.Accent : Tokens.BorderSoft);
            float k = 15f;
            Rect knob = new Rect(on ? track.xMax - k - 2f : track.x + 2f, track.y + 1.5f, k, k);
            RUIF.Draw.Box(knob, on ? Tokens.Window : Tokens.PanelRaised);
            if (enabled && !string.IsNullOrEmpty(rect.width > 40f ? "x" : null))
            {
                RUIF.Draw.Label(new Rect(track.xMax + 6f, rect.y, Mathf.Max(0f, rect.width - 42f), rect.height),
                    on ? Tr.Get("RUIF.Comp.Common.On") : Tr.Get("RUIF.Comp.Common.Off"), Tokens.FontLabel, on ? Tokens.Text : Tokens.Muted, TextAnchor.MiddleLeft);
            }
            return Widgets.ButtonInvisible(rect);
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
    }
}
