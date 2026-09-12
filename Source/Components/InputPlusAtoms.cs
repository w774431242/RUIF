using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>快捷数量档位。</summary>
    public enum QuickAmountKind { One, Ten, Hundred, Max }

    /// <summary>B2+ 输入控件的纯计算（可离线单测）。</summary>
    public static class InputPlusFmt
    {
        /// <summary>按档位取 1 / 10 / 100 / 上限（并夹到 max）。</summary>
        public static float QuickAmount(float current, QuickAmountKind kind, float max)
            => kind switch
            {
                QuickAmountKind.One => Mathf.Min(1f, max),
                QuickAmountKind.Ten => Mathf.Min(10f, max),
                QuickAmountKind.Hundred => Mathf.Min(100f, max),
                _ => max
            };

        /// <summary>是否已到上限（浮点容差判定）。</summary>
        public static bool IsMax(float v, float max) => Mathf.Abs(v - max) < 0.0001f;

        /// <summary>吸附到最近刻度（无刻度则返回原值）。</summary>
        public static float SnapToTicks(float value, float[] ticks)
        {
            if (ticks == null || ticks.Length == 0) return value;
            float best = ticks[0];
            float bestD = Mathf.Abs(value - best);
            foreach (float t in ticks)
            {
                float d = Mathf.Abs(value - t);
                if (d < bestD) { bestD = d; best = t; }
            }
            return best;
        }

        /// <summary>
        /// 滚轮步进。约定：<b>deltaY &gt; 0 → 减小</b>；如与实机相反，调用方传 <c>invert:true</c> 即可（符号集中在此，便于一处修正）。
        /// </summary>
        public static float WheelStep(float value, float deltaY, float step, float min, float max)
            => Mathf.Clamp(value + (deltaY > 0f ? -step : step), min, max);

        /// <summary>Ctrl 点击：切换单项。</summary>
        public static bool ToggleAt(bool[] sel, int i)
        {
            if (sel == null || i < 0 || i >= sel.Length) return false;
            sel[i] = !sel[i];
            return true;
        }

        /// <summary>Shift 点击：选中 anchor~index 区间（含端点）。</summary>
        public static void SelectRange(bool[] sel, int anchor, int index)
        {
            if (sel == null) return;
            int a = Mathf.Clamp(Mathf.Min(anchor, index), 0, sel.Length - 1);
            int b = Mathf.Clamp(Mathf.Max(anchor, index), 0, sel.Length - 1);
            for (int i = a; i <= b; i++) sel[i] = true;
        }

        /// <summary>入队：写入 queue[count] 并自增；满则返回 -1。</summary>
        public static int EnqueueInt(int[] queue, ref int count, int value)
        {
            if (queue == null || count >= queue.Length) return -1;
            queue[count] = value;
            return count++;
        }

        /// <summary>按下标删除并左移后续项；越界返回 false。</summary>
        public static bool RemoveAtInt(int[] queue, ref int count, int index)
        {
            if (queue == null || index < 0 || index >= count) return false;
            for (int i = index; i < count - 1; i++) queue[i] = queue[i + 1];
            count--;
            return true;
        }

        /// <summary>把第 index 项前移到队首（"置顶"）。</summary>
        public static void MoveToFrontInt(int[] queue, int count, int index)
        {
            if (queue == null || index <= 0 || index >= count) return;
            int v = queue[index];
            for (int i = index; i > 0; i--) queue[i] = queue[i - 1];
            queue[0] = v;
        }

        /// <summary>数值文本解析并钳制；失败返回 false（不改写 value）。</summary>
        public static bool TryParseClamp(string s, float min, float max, out float value)
        {
            value = 0f;
            if (string.IsNullOrWhiteSpace(s)) return false;
            float v;
            if (!float.TryParse(s, out v)) return false;
            value = Mathf.Clamp(v, min, max);
            return true;
        }

        /// <summary>路径合法性：非空，且不含 &lt; &gt; | " ? *</summary>
        public static bool IsValidPath(string p)
        {
            if (string.IsNullOrWhiteSpace(p)) return false;
            char[] bad = { '<', '>', '|', '"', '?', '*' };
            foreach (char c in bad)
                if (p.IndexOf(c) >= 0) return false;
            return true;
        }

        /// <summary>路径归一化：反斜杠统一为正斜杠。</summary>
        public static string NormalizePath(string p)
            => (p ?? string.Empty).Replace('\\', '/');
        /// <summary>键位数组是否存在重复（空串忽略）。</summary>
        public static bool HasConflict(string[] keys)
        {
            if (keys == null) return false;
            var seen = new HashSet<string>();
            foreach (string k in keys)
            {
                if (string.IsNullOrEmpty(k)) continue;
                if (!seen.Add(k)) return true;
            }
            return false;
        }

        /// <summary>重复键位的出现次数（不含首次）。</summary>
        public static int ConflictCount(string[] keys)
        {
            if (keys == null) return 0;
            var seen = new HashSet<string>();
            int n = 0;
            foreach (string k in keys)
            {
                if (string.IsNullOrEmpty(k)) continue;
                if (!seen.Add(k)) n++;
            }
            return n;
        }

        /// <summary>按行拆分并截断到 maxLines 行（空文本返回单空行）。</summary>
        public static string[] SplitLines(string text, int maxLines)
        {
            int n = Mathf.Max(1, maxLines);
            if (string.IsNullOrEmpty(text))
            {
                var one = new string[1];
                one[0] = string.Empty;
                return one;
            }
            string[] parts = text.Replace("\r\n", "\n").Split('\n');
            if (parts.Length <= n) return parts;
            var cut = new string[n];
            Array.Copy(parts, cut, n);
            return cut;
        }
    }

    /// <summary>
    /// B2+ 输入控件（S3 任务②）：快捷数量 / 刻度滑条 / 滚轮调值 / 数值输入 / 路径输入 / 原生下拉 / 列过滤 / 多选列表 / 队列编辑 / 涂抹网格 / 键位冲突提示 / 多行编辑。
    /// 无状态；所有缓冲与选中态由调用方持有。下拉走官方 <b>FloatMenu / FloatMenuOption</b>（核心 UI 族，已核实）。
    /// ⚠️ MultiLineEditor 为 v1 简化版：按行拆分多个单行框（真多行编辑需自定义 TextEditor，留 v1.1）**
    /// </summary>
    public static class InputPlusAtoms
    {
        /// <summary>快捷数量行：1 / 10 / 100 / 上限 四个按钮 + 当前值读数。</summary>
        public static bool QuickAmounts(Rect rect, string label, ref float value, float max)
        {
            var row = Layout.Row(rect, 4f);
            Draw.Label(row.Take(44f), label ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            bool changed = false;
            if (Widgets.ButtonText(row.Take(38f), "1")) { value = InputPlusFmt.QuickAmount(value, QuickAmountKind.One, max); changed = true; }
            if (Widgets.ButtonText(row.Take(40f), "10")) { value = InputPlusFmt.QuickAmount(value, QuickAmountKind.Ten, max); changed = true; }
            if (Widgets.ButtonText(row.Take(44f), "100")) { value = InputPlusFmt.QuickAmount(value, QuickAmountKind.Hundred, max); changed = true; }
            if (Widgets.ButtonText(row.Take(50f), Tr.Get("RUIF.Comp.Common.Max"))) { value = max; changed = true; }
            Draw.Label(row.Rest, value.ToString("0"), Tokens.FontLabel, InputPlusFmt.IsMax(value, max) ? Tokens.Accent : Tokens.Text, TextAnchor.MiddleLeft);
            return changed;
        }

        /// <summary>带刻度的滑条：吸附到最近刻度并在下方画出刻度标记。</summary>
        public static bool SliderTicks(Rect rect, ref float value, float[] ticks, float min, float max)
        {
            var row = Layout.Row(rect, 4f);
            Draw.Label(row.Take(38f), Tr.Get("RUIF.Comp.Common.Snap"), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Rect s = row.Take(Mathf.Max(10f, rect.width - 140f));
            float before = value;
            Widgets.HorizontalSlider(s, ref value, new FloatRange(min, max), null, -1f);
            value = InputPlusFmt.SnapToTicks(value, ticks);
            if (ticks != null)
                foreach (float t in ticks)
                {
                    float x = s.x + s.width * InputFmt.Slider01(t, min, max);
                    Draw.Box(new Rect(x - 1f, s.center.y + 5f, 2f, 5f), Tokens.Muted);
                }
            Draw.Label(row.Rest, value.ToString("0"), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
            return Mathf.Abs(value - before) > 0.0001f;
        }

        /// <summary>滚轮调值框：悬停时滚轮步进（invert 反号）。</summary>
        public static bool WheelAdjust(Rect rect, ref float value, float step, float min, float max, bool invert = false)
        {
            if (Mouse.IsOver(rect)) Widgets.DrawHighlight(rect);
            Draw.Box(rect, Tokens.PanelRaised);
            Draw.Border(rect, Tokens.BorderSoft);
            Draw.Label(rect, Tr.Get("RUIF.Comp.Common.Wheel", value.ToString("0.#")), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
            if (Event.current.type == EventType.ScrollWheel && Mouse.IsOver(rect))
            {
                float dy = invert ? -Event.current.delta.y : Event.current.delta.y;
                value = InputPlusFmt.WheelStep(value, dy, step, min, max);
                Event.current.Use();     // 吞掉滚轮事件，避免穿透到下层
                return true;
            }
            return false;
        }

        /// <summary>数值输入框：解析失败红框提示，成功则钳制写回。</summary>
        public static bool NumericField(Rect rect, ref float value, float min, float max, ref string buffer)
        {
            string prev = buffer;
            buffer = Widgets.TextField(rect, buffer);
            bool ok = InputPlusFmt.TryParseClamp(buffer, min, max, out float v);
            Draw.Border(rect, ok ? Tokens.BorderSoft : Tokens.Dead);
            if (buffer == prev) return false;
            if (!ok) return false;
            value = v;
            return true;
        }

        /// <summary>路径输入框：非法字符实时红框 + ✓/✕ 标记。</summary>
        public static bool PathField(Rect rect, ref string path)
        {
            var row = Layout.Row(rect, 4f);
            Rect markRect = row.Take(18f);
            string prev = path;
            path = Widgets.TextField(row.Rest, path);
            bool ok = InputPlusFmt.IsValidPath(path);
            Draw.Label(markRect, ok ? "✓" : "✕", Tokens.FontLabel, ok ? Tokens.Alive : Tokens.Dead, TextAnchor.MiddleCenter);
            if (!ok) Draw.Border(rect, Tokens.Dead);
            return path != prev;
        }

        /// <summary>
        /// 原生下拉（官方 FloatMenu）。<b>用回调而非 ref</b>——C# 不允许在 lambda 中捕获 ref 参数（CS1628）。
        /// </summary>
        public static bool InPlaceDropdown(Rect rect, string label, string[] options, int currentIndex, Action<int> onPicked)
        {
            int n = options?.Length ?? 0;
            string cur = n > 0 ? options[Mathf.Clamp(currentIndex, 0, n - 1)] : string.Empty;
            Draw.Box(rect, Tokens.PanelRaised);
            Draw.Border(rect, Mouse.IsOver(rect) ? Tokens.Accent : Tokens.BorderSoft);
            var row = Layout.Row(rect, 6f);
            if (!string.IsNullOrEmpty(label))
                Draw.Label(row.Take(58f), label, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.Label(row.Rest, cur + " ▾", Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
            if (!Widgets.ButtonInvisible(rect) || n == 0) return false;
            var list = new List<FloatMenuOption>();
            for (int i = 0; i < n; i++)
            {
                int local = i;
                list.Add(new FloatMenuOption(options[i], () => onPicked?.Invoke(local)));
            }
            Find.WindowStack.Add(new FloatMenu(list));
            return true;
        }

        /// <summary>列过滤行：每列一个输入框（列标签画在上方）。</summary>
        public static bool FilterRow(Rect rect, string[] colLabels, string[] filters)
        {
            if (colLabels == null || filters == null) return false;
            int n = Mathf.Min(colLabels.Length, filters.Length);
            if (n == 0) return false;
            var row = Layout.Row(rect, 4f);
            bool changed = false;
            float w = rect.width / n;
            for (int i = 0; i < n; i++)
            {
                Rect cell = row.Take(Mathf.Max(10f, w - 4f));
                Draw.Label(new Rect(cell.x, rect.y - 13f, cell.width, 12f),
                    colLabels[i] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                string prev = filters[i];
                filters[i] = Widgets.TextField(cell, filters[i] ?? string.Empty);
                changed |= filters[i] != prev;
            }
            return changed;
        }

        /// <summary>多选列表：单击单选、Ctrl 切换、Shift 区间选（anchor 由调用方持有）。</summary>
        public static bool MultiSelectList(Rect rect, string[] items, bool[] selected, ref int anchor, float rowH)
        {
            int n = items?.Length ?? 0;
            if (n == 0) return false;
            bool changed = false;
            for (int i = 0; i < n; i++)
            {
                var row = new Rect(rect.x, rect.y + i * rowH, rect.width, rowH);
                bool sel = i < selected.Length && selected[i];
                if (sel)
                {
                    Draw.Box(row, Tokens.AccentSoft);
                    Draw.LineV(row.x, row.y, rowH, Tokens.Accent);
                }
                else if (Mouse.IsOver(row)) Widgets.DrawHighlight(row);
                Draw.Label(new Rect(row.x + 8f, row.y, row.width - 12f, rowH), items[i] ?? string.Empty,
                    Tokens.FontLabel, sel ? Tokens.Accent : Tokens.Text, TextAnchor.MiddleLeft);
                if (!Widgets.ButtonInvisible(row)) continue;
                if (Event.current.shift && anchor >= 0) InputPlusFmt.SelectRange(selected, anchor, i);
                else if (Event.current.control && i < selected.Length) InputPlusFmt.ToggleAt(selected, i);
                else
                {
                    Array.Clear(selected, 0, selected.Length);
                    if (i < selected.Length) selected[i] = true;
                    anchor = i;
                }
                changed = true;
            }
            return changed;
        }

        /// <summary>队列编辑器：逐行 ↑ 置顶 / × 删除。</summary>
        public static bool QueueEditor(Rect rect, int[] queue, ref int count, string[] labels, float rowH)
        {
            if (queue == null) return false;
            bool changed = false;
            for (int i = 0; i < count && i < queue.Length; i++)
            {
                var row = new Rect(rect.x, rect.y + i * rowH, rect.width, rowH);
                if (i % 2 == 1) Draw.Box(row, Draw.WithAlpha(Tokens.Text, 0.04f));
                var r = Layout.Row(row, 4f);
                string name = labels != null && queue[i] >= 0 && queue[i] < labels.Length ? labels[queue[i]] : "?";
                Draw.Label(r.Take(Mathf.Max(0f, rect.width - 60f)), (i + 1) + ". " + name,
                    Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                var up = new Rect(rect.xMax - 56f, row.y, 26f, rowH);
                if (Widgets.ButtonText(up, "↑")) { InputPlusFmt.MoveToFrontInt(queue, count, i); changed = true; }
                var del = new Rect(rect.xMax - 28f, row.y, 28f, rowH);
                if (Widgets.ButtonText(del, "×")) { InputPlusFmt.RemoveAtInt(queue, ref count, i); changed = true; }
            }
            return changed;
        }

        /// <summary>涂抹网格：按值着色，返回被点击的格子下标。</summary>
        public static bool PaintStrokeGrid(Rect rect, int rows, int cols, int[] values, out int hitIndex)
        {
            hitIndex = -1;
            float cellH = rect.height / Mathf.Max(1, rows);
            float cellW = rect.width / Mathf.Max(1, cols);
            bool any = false;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int idx = r * cols + c;
                    var cell = new Rect(rect.x + c * cellW + 1f, rect.y + r * cellH + 1f, cellW - 2f, cellH - 2f);
                    int v = values != null && idx < values.Length ? values[idx] : 0;
                    Color tone = v <= 0 ? Tokens.PanelRaised : Draw.WithAlpha(PriorityTone(v), 0.4f);
                    Draw.Box(cell, tone);
                    Draw.Border(cell, v <= 0 ? Tokens.BorderSoft : PriorityTone(v));
                    if (v > 0) Draw.Label(cell, v.ToString(), Tokens.FontLabel, PriorityTone(v), TextAnchor.MiddleCenter);
                    if (Widgets.ButtonInvisible(cell)) { hitIndex = idx; any = true; }
                }
            }
            return any;
        }

        private static Color PriorityTone(int v)
            => v switch { 1 => Tokens.Alive, 2 => Tokens.Info, 3 => Tokens.Warn, _ => Tokens.Accent };

        /// <summary>键位冲突提示；返回是否存在冲突（调用方据此决定是否阻断保存）。</summary>
        public static bool KeyConflictHint(Rect rect, string[] boundKeys)
        {
            if (!InputPlusFmt.HasConflict(boundKeys))
            {
                Draw.Label(rect, Tr.Get("RUIF.Comp.Hotkey.NoConflict"), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                return false;
            }
            Draw.Box(rect, Draw.WithAlpha(Tokens.Warn, 0.14f));
            Draw.Border(rect, Tokens.Warn);
            Draw.Label(rect, Tr.Get("RUIF.Comp.Hotkey.Conflict", InputPlusFmt.ConflictCount(boundKeys)),
                Tokens.FontLabel, Tokens.Warn, TextAnchor.MiddleLeft);
            return true;
        }

        /// <summary>⚠️ v1 简化：按行拆成多个单行输入框（Shift+Enter 需调用方自行处理）。</summary>
        public static bool MultiLineEditor(Rect rect, ref string text, ref string buffer, int maxLines = 4)
        {
            string[] lines = InputPlusFmt.SplitLines(buffer, maxLines);
            var col = Layout.Column(rect, 2f);
            bool changed = false;
            for (int i = 0; i < lines.Length; i++)
            {
                string prev = lines[i];
                lines[i] = Widgets.TextField(col.Take(22f), lines[i] ?? string.Empty);
                changed |= lines[i] != prev;
            }
            if (!changed) return false;
            buffer = string.Join("\n", lines);
            text = buffer;
            return true;
        }
    }
}
