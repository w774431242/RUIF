using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>输入扩展控件（B2）的纯计算（可离线单测；多处刻意手写算法，避免依赖未核实的 Unity 转换 API）。</summary>
    public static class InputExFmt
    {
        /// <summary>优先级循环：0(空) → 1 → 2 → 3 → 4 → 0。</summary>
        public static int NextPriority(int v) => v <= 0 ? 1 : (v >= 4 ? 0 : v + 1);

        /// <summary>作息状态循环：0 工作 / 1 睡眠 / 2 任意。</summary>
        public static int NextTimetableState(int s) => ((s % 3) + 3) % 3 == s ? (s + 1) % 3 : 0;

        /// <summary>索引循环 +1（n ≤ 0 返回 0）。</summary>
        public static int NextIndex(int i, int n) => n <= 0 ? 0 : (i + 1) % n;

        /// <summary>拖拽刷数值：起点值 + 横向位移 × 步长（钳制）。</summary>
        public static float ScrubValue(float startValue, float dx, float step, float min, float max)
            => Mathf.Clamp(startValue + dx * step, min, max);

        /// <summary>拖拽落点索引（-1 表示区域外）。</summary>
        public static int DropIndex(Rect list, int count, float rowH, float pointerY)
        {
            if (count <= 0 || rowH <= 0f) return -1;
            int i = Mathf.FloorToInt((pointerY - list.y) / rowH);
            return (i < 0 || i >= count) ? -1 : i;
        }

        /// <summary>数组内移动元素（from → to）。</summary>
        public static void Reorder(string[] items, int from, int to)
        {
            if (items == null || from == to) return;
            if (from < 0 || from >= items.Length || to < 0 || to >= items.Length) return;
            string tmp = items[from];
            if (from < to) { for (int i = from; i < to; i++) items[i] = items[i + 1]; }
            else { for (int i = from; i > to; i--) items[i] = items[i - 1]; }
            items[to] = tmp;
        }

        /// <summary>手写 HSV → RGB（h/s/v ∈ [0,1]），避免依赖 Color.HSVToRGB。</summary>
        public static Color HsvColor(float h, float s, float v, float alpha = 1f)
        {
            h = Mathf.Repeat(h, 1f);
            s = Mathf.Clamp01(s);
            v = Mathf.Clamp01(v);
            float c = v * s;
            float x = c * (1f - Mathf.Abs(Mathf.Repeat(h * 6f, 2f) - 1f));
            float m = v - c;
            float r = 0f, g = 0f, b = 0f;
            switch (Mathf.FloorToInt(h * 6f) % 6)
            {
                case 0: r = c; g = x; break;
                case 1: r = x; g = c; break;
                case 2: g = c; b = x; break;
                case 3: g = x; b = c; break;
                case 4: r = x; b = c; break;
                default: r = c; b = x; break;
            }
            return new Color(r + m, g + m, b + m, alpha);
        }

        /// <summary>手写 RGB → HSV，供取色器初始化滑块。</summary>
        public static void RgbToHsv(Color c, out float h, out float s, out float v)
        {
            float r = c.r, g = c.g, b = c.b;
            float max = Mathf.Max(r, Mathf.Max(g, b));
            float min = Mathf.Min(r, Mathf.Min(g, b));
            v = max;
            float d = max - min;
            if (d <= 0f) { h = 0f; s = 0f; return; }
            s = max <= 0f ? 0f : d / max;
            if (max - r < 1e-5f) h = (g - b) / d;
            else if (max - g < 1e-5f) h = 2f + (b - r) / d;
            else h = 4f + (r - g) / d;
            h /= 6f;
            if (h < 0f) h += 1f;
        }

        /// <summary>十六进制色值（不含 #）。</summary>
        public static string Hex(Color c)
        {
            int r = Mathf.RoundToInt(Mathf.Clamp01(c.r) * 255f);
            int g = Mathf.RoundToInt(Mathf.Clamp01(c.g) * 255f);
            int b = Mathf.RoundToInt(Mathf.Clamp01(c.b) * 255f);
            return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
    }

    /// <summary>
    /// 输入扩展控件（S3-1b，B2 · 工坊实证）：拖拽列表 / 双端范围滑条 / 取色器 /
    /// 循环按钮 / 行内编辑 / 数值刷 / 优先级网格 / 作息矩阵。
    /// 无状态——拖拽与编辑态由调用方持有。
    /// </summary>
    public static class InputExAtoms
    {
        // （2026-09-10 体检：删除未使用的私有常量 PriorityMax —— 优先级上限实际由 InputExFmt.NextPriority 决定）

        /// <summary>拖拽列表的单行（仅绘制）；命中判断见 <see cref="GripHit"/>。</summary>
        public static void DragListRow(Rect row, string label, bool dragging, bool dropTarget)
        {
            if (dragging)
            {
                Draw.Box(row, Tokens.AccentSoft);
                Draw.LineV(row.x, row.y, row.height, Tokens.Accent);
            }
            else if (dropTarget) Draw.LineH(row.x, row.yMax - 2f, row.width, Tokens.Accent);
            else if (Mouse.IsOver(row)) Widgets.DrawHighlight(row);

            var grip = new Rect(row.x + 4f, row.center.y - 9f, 12f, 18f);
            InteractFmt.GripDotSize(grip, 3, 2, 2f, out float w, out float h);   // 点阵尺寸复用 InteractFmt
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 2; c++)
                    Draw.Box(new Rect(grip.x + c * (w + 2f), grip.y + r * (h + 2f), w, h), Tokens.Muted);

            Draw.Label(new Rect(row.x + 22f, row.y, Mathf.Max(0f, row.width - 26f), row.height),
                label ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
        }

        /// <summary>行左侧把手区是否被按下（调用方据此开始拖拽）。</summary>
        public static bool GripHit(Rect row)
            => Widgets.ButtonInvisible(new Rect(row.x, row.y, 20f, row.height));

        /// <summary>
        /// 双端范围滑条（v1 简化版：区间示意条 + 下限/上限两条官方滑条；lo&gt;hi 时自动交换）。
        /// </summary>
        public static bool DoubleSlider(Rect rect, ref float lo, ref float hi, float min, float max, float roundTo = -1f)
        {
            var col = Layout.Column(rect, 3f);
            Rect bar = col.Take(12f);
            Draw.Box(bar, Tokens.BorderSoft);
            float x0 = bar.x + bar.width * InputFmt.Slider01(lo, min, max);
            float x1 = bar.x + bar.width * InputFmt.Slider01(hi, min, max);
            Draw.Box(new Rect(x0, bar.y, Mathf.Max(1f, x1 - x0), bar.height), Tokens.Accent);

            bool changed = SliderRow(col.Take(18f), Tr.Get("RUIF.Comp.Common.Lower"), ref lo, min, max, roundTo);
            changed |= SliderRow(col.Rest, Tr.Get("RUIF.Comp.Common.Upper"), ref hi, min, max, roundTo);
            if (lo > hi) { float t = lo; lo = hi; hi = t; changed = true; }
            return changed;
        }

        private static bool SliderRow(Rect row, string name, ref float value, float min, float max, float roundTo)
        {
            var r = Layout.Row(row, 6f);
            Draw.Label(r.Take(38f), name, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Rect s = r.Take(Mathf.Max(10f, row.width - 100f));
            float before = value;
            Widgets.HorizontalSlider(s, ref value, new FloatRange(min, max), null, roundTo);
            Draw.Label(r.Rest, value.ToString("0.##"), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
            return Mathf.Abs(value - before) > 0.0001f;
        }

        /// <summary>取色器（HSV 三条滑条 + 预览 + 十六进制）；返回是否变化（h/s/v 由调用方持有）。</summary>
        public static bool ColorPicker(Rect rect, ref Color color, ref float h, ref float s, ref float v)
        {
            InputExFmt.RgbToHsv(color, out h, out s, out v);
            var col = Layout.Column(rect, 3f);
            var head = Layout.Row(col.Take(20f), 6f);
            Rect preview = head.Take(30f);
            Draw.Box(preview, color);
            Draw.Border(preview, Tokens.BorderSoft);
            Draw.Label(head.Rest, "#" + InputExFmt.Hex(color), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            return DoubleSliderLike(col, ref h, ref s, ref v, out color);
        }

        private static bool DoubleSliderLike(VStack col, ref float h, ref float s, ref float v, out Color color)
        {
            bool changed = SliderRow(col.Take(18f), "H", ref h, 0f, 1f, -1f);
            changed |= SliderRow(col.Take(18f), "S", ref s, 0f, 1f, -1f);
            changed |= SliderRow(col.Rest, "V", ref v, 0f, 1f, -1f);
            color = InputExFmt.HsvColor(h, s, v, 1f);
            return changed;
        }

        /// <summary>循环按钮（点击在多个值间循环，取代下拉的轻量方案）。</summary>
        public static bool CycleButton(Rect rect, string label, string[] values, ref int index)
        {
            int n = values?.Length ?? 0;
            string v = n > 0 ? values[Mathf.Clamp(index, 0, n - 1)] : string.Empty;
            Draw.Box(rect, Tokens.PanelRaised);
            Draw.Border(rect, Mouse.IsOver(rect) ? Tokens.Accent : Tokens.BorderSoft);
            var row = Layout.Row(rect, 6f);
            if (!string.IsNullOrEmpty(label))
                Draw.Label(row.Take(84f), label, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.Label(row.Rest, v + " ▾", Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
            if (Widgets.ButtonInvisible(rect) && n > 0)
            {
                index = InputExFmt.NextIndex(index, n);
                return true;
            }
            return false;
        }

        /// <summary>行内编辑文本：常态显示，点击进入编辑（Enter 或点 ✓ 提交）。</summary>
        public static bool EditableLabel(Rect rect, ref string text, ref bool editing, ref string buffer)
        {
            if (editing)
            {
                var row = Layout.Row(rect, 4f);
                buffer = Widgets.TextField(row.Rest, buffer);
                bool commit = false;
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) commit = true;
                if (Widgets.ButtonText(row.Take(0f), string.Empty)) { }
                // 提交按钮（最右 34f）
                var btn = new Rect(rect.xMax - 34f, rect.y, 34f, rect.height);
                if (Widgets.ButtonText(btn, "✓") || commit)
                {
                    text = buffer;
                    editing = false;
                    return true;
                }
                return false;
            }

            if (Mouse.IsOver(rect)) Widgets.DrawHighlight(rect);
            Draw.Label(rect, (text ?? string.Empty) + "  ✎", Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            if (Widgets.ButtonInvisible(rect))
            {
                editing = true;
                buffer = text ?? string.Empty;
            }
            return false;
        }

        /// <summary>数值刷（按住横向拖动改值）；scrubbing 与起点由调用方持有。</summary>
        public static void NumberScrubber(Rect rect, ref float value, float step, float min, float max,
            ref bool scrubbing, ref float startX, ref float startVal)
        {
            if (Mouse.IsOver(rect)) Widgets.DrawHighlight(rect);
            Draw.Box(rect, Tokens.PanelRaised);
            Draw.Border(rect, scrubbing ? Tokens.Accent : Tokens.BorderSoft);
            Draw.Label(rect, "⇄ " + value.ToString("0.##"), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
            if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect))
            {
                scrubbing = true;
                startX = Event.current.mousePosition.x;
                startVal = value;
            }
            if (scrubbing)
            {
                if (Event.current.type == EventType.MouseUp) scrubbing = false;
                else value = InputExFmt.ScrubValue(startVal, Event.current.mousePosition.x - startX, step, min, max);
            }
        }

        /// <summary>
        /// 优先级网格（Work Tab 实证）：行×列单元，点击返回命中索引（调用方用 <see cref="InputExFmt.NextPriority"/> 推进）。
        /// </summary>
        public static bool PriorityGrid(Rect rect, string[] rowLabels, string[] colLabels, int[] values,
            out int hitIndex)
        {
            hitIndex = -1;
            int rows = rowLabels?.Length ?? 0;
            int cols = colLabels?.Length ?? 0;
            if (rows == 0 || cols == 0) return false;

            float labelW = 64f;
            float cellH = rect.height / rows;
            float cellW = (rect.width - labelW) / cols;
            bool any = false;
            for (int c = 0; c < cols; c++)
                Draw.Label(new Rect(rect.x + labelW + c * cellW, rect.y, cellW, 16f),
                    colLabels[c] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);

            for (int r = 0; r < rows; r++)
            {
                Draw.Label(new Rect(rect.x, rect.y + 18f + r * cellH, labelW, cellH),
                    rowLabels[r] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                for (int c = 0; c < cols; c++)
                {
                    int idx = r * cols + c;
                    var cell = new Rect(rect.x + labelW + c * cellW + 1f, rect.y + 18f + r * cellH + 1f, cellW - 2f, cellH - 2f);
                    int v = values != null && idx < values.Length ? values[idx] : 0;
                    Color col = v <= 0 ? Tokens.PanelRaised : Draw.WithAlpha(PriorityColor(v), 0.35f);
                    Draw.Box(cell, col);
                    Draw.Border(cell, v <= 0 ? Tokens.BorderSoft : PriorityColor(v));
                    if (v > 0)
                        Draw.Label(cell, v.ToString(), Tokens.FontLabel, PriorityColor(v), TextAnchor.MiddleCenter);
                    if (Widgets.ButtonInvisible(cell)) { hitIndex = idx; any = true; }
                }
            }
            return any;
        }

        private static Color PriorityColor(int v)
            => v switch { 1 => Tokens.Alive, 2 => Tokens.Info, 3 => Tokens.Warn, _ => Tokens.Accent };

        /// <summary>
        /// 作息矩阵（Work Tab 实证）：人员 × 24 刻；点击返回命中索引（调用方用 <see cref="InputExFmt.NextTimetableState"/> 推进）。
        /// </summary>
        public static bool TimetableMatrix(Rect rect, string[] rowLabels, int[] states, out int hitIndex)
        {
            hitIndex = -1;
            int rows = rowLabels?.Length ?? 0;
            const int cols = 24;
            if (rows == 0) return false;

            float labelW = 56f;
            float cellH = rect.height / rows;
            float cellW = (rect.width - labelW) / cols;
            bool any = false;
            for (int r = 0; r < rows; r++)
            {
                Draw.Label(new Rect(rect.x, rect.y + r * cellH, labelW, cellH),
                    rowLabels[r] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                for (int c = 0; c < cols; c++)
                {
                    int idx = r * cols + c;
                    var cell = new Rect(rect.x + labelW + c * cellW + 1f, rect.y + r * cellH + 1f, cellW - 2f, cellH - 2f);
                    int st = states != null && idx < states.Length ? states[idx] : 0;
                    Color col = st switch { 0 => Tokens.Alive, 1 => Tokens.Info, _ => Tokens.PanelRaised };
                    Draw.Box(cell, Draw.WithAlpha(col, st == 2 ? 0.25f : 0.55f));
                    if (Widgets.ButtonInvisible(cell)) { hitIndex = idx; any = true; }
                }
            }
            return any;
        }
    }
}
