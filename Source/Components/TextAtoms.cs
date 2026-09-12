using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// 纯文本格式化（无绘制依赖 → 可离线单测）。
    /// 规则确定、无隐藏状态；游戏内时间/地点相关格式走 <see cref="TextAtoms"/> 的官方包装。
    /// </summary>
    public static class TextFmt
    {
        /// <summary>
        /// 数值缩写：&lt;1e4 原样整数；≥1e4 → k（1 位小数）；≥1e6 → M（≤2 位）；≥1e9 → B。
        /// 例：999→"999"，12 480→"12.5k"，284 000→"284k"，2 500 000→"2.5M"。
        /// </summary>
        public static string FormatNumber(double value)
        {
            double a = Math.Abs(value);
            if (a >= 1e9) return Sig(value / 1e9, "0.##") + "B";
            if (a >= 1e6) return Sig(value / 1e6, "0.##") + "M";
            if (a >= 1e4) return Sig(value / 1e3, "0.#") + "k";
            return Math.Round(value).ToString("0", System.Globalization.CultureInfo.InvariantCulture);
        }

        private static string Sig(double v, string fmt)
            => v.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>打字机进度 → 可见字符数（progress01 ∈ [0,1]；纯函数可测）。</summary>
        public static int TypewriterChars(string text, float progress01)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            return (int)Math.Floor(text.Length * Mathf.Clamp01(progress01) + 1e-4);
        }
    }

    /// <summary>混排行的一段（颜色/字体可选；未指定走 Tokens 默认）。</summary>
    public readonly struct RichSpan
    {
        /// <summary>该段文本。</summary>
        public readonly string Text;
        /// <summary>该段颜色；null = 用 <see cref="Tokens.Text"/>。</summary>
        public readonly Color? Color;
        /// <summary>该段字体；null = 用调用方传入的默认字体。</summary>
        public readonly GameFont? Font;

        public RichSpan(string text, Color? color = null, GameFont? font = null)
        {
            Text = text; Color = color; Font = font;
        }
    }

    /// <summary>
    /// 文本原子（S2 第一批）的绘制件。
    /// 约束：不持有状态；颜色/字体一律走令牌或调用方传参；绘制态经 <see cref="Draw"/> 作用域恢复。
    /// </summary>
    public static class TextAtoms
    {
        /// <summary>
        /// 游戏内日期（官方 <b>RimWorld.GenDate</b> 直通——命名空间已反编译核实，不是 Verse）。
        /// location 传地图/世界坐标（影响时区呈现）；无上下文时可传 Vector3.zero。
        /// </summary>
        public static string DateFull(long absTicks, Vector3 location)
            => RimWorld.GenDate.DateFullStringAt(absTicks, location);

        /// <summary>
        /// 单位标签：数值右对齐 + 单位（Tiny 灰）缀在右侧。
        /// </summary>
        public static void UnitLabel(Rect rect, string value, string unit,
            GameFont valueFont = GameFont.Medium, Color? color = null)
        {
            Color c = color ?? Tokens.Text;
            float unitW = 0f;
            using (Draw.Font(Tokens.FontLabel))
                unitW = Text.CalcSize(unit ?? string.Empty).x;
            if (unitW > 0f)
            {
                var unitRect = new Rect(rect.xMax - unitW, rect.y, unitW, rect.height);
                Draw.Label(unitRect, unit ?? string.Empty, Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleRight);
                rect.width -= unitW + 2f;
            }
            Draw.Label(rect, value ?? string.Empty, valueFont, c, TextAnchor.MiddleRight);
        }

        /// <summary>
        /// 打字机：按 progress01 显示前 N 个字符（动画进度由调用方用 Time 驱动；跳过=传 1）。
        /// </summary>
        public static void Typewriter(Rect rect, string text, float progress01,
            GameFont font, Color color, TextAnchor anchor = TextAnchor.MiddleLeft)
        {
            int n = TextFmt.TypewriterChars(text, progress01);
            Draw.Label(rect, (text ?? string.Empty).Substring(0, n), font, color, anchor);
        }

        /// <summary>主文案 + 小注双行（本地化对照 / 来源标注）。</summary>
        public static void SubtitleLine(Rect rect, string main, string sub)
        {
            var col = Layout.Column(rect, 1f);
            Draw.Label(col.Take(Tokens.FontBodyLineHeight), main ?? string.Empty,
                Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(col.Take(16f), sub ?? string.Empty,
                Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
        }

        /// <summary>
        /// 混排行：多段顺序绘制（各自颜色/字体），两遍测量（先总宽定锚点，再逐段画）。
        /// 返回实际绘制宽度。超出 rect 宽度的部分不裁剪（调用方控制 span 总长）。
        /// </summary>
        public static float RichTextLine(Rect rect, RichSpan[] spans,
            TextAnchor anchor = TextAnchor.MiddleLeft, GameFont defaultFont = GameFont.Small)
        {
            if (spans == null || spans.Length == 0) return 0f;
            float lineH = Tokens.FontBodyLineHeight;

            // 第一遍：总宽
            float total = 0f;
            foreach (var s in spans)
            {
                using (Draw.Font(s.Font ?? defaultFont))
                    total += Text.CalcSize(s.Text ?? string.Empty).x;
            }

            // 锚点偏移
            float x = rect.x;
            if (anchor == TextAnchor.MiddleRight) x = rect.xMax - total;
            else if (anchor == TextAnchor.MiddleCenter) x = rect.x + (rect.width - total) / 2f;

            // 第二遍：逐段画（垂直居中于行高）
            float y = rect.y + (rect.height - lineH) / 2f;
            foreach (var s in spans)
            {
                GameFont f = s.Font ?? defaultFont;
                Color c = s.Color ?? Tokens.Text;
                using (Draw.Font(f))
                {
                    Vector2 size = Text.CalcSize(s.Text ?? string.Empty);
                    Draw.Label(new Rect(x, y + (lineH - size.y) / 2f, size.x + 1f, size.y),
                        s.Text ?? string.Empty, f, c, TextAnchor.MiddleLeft);
                    x += size.x;
                }
            }
            return total;
        }
    }
}
