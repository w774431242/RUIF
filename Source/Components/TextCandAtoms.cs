using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>① 文本候选组件的单入口绘制。每个 Draw 为具名方法（与 <see cref="TextCandFmt"/> 的 Build 同名），
    /// 仅把候选 struct 的位置参数原样转发给对应的 TextAtoms 原子；不持有状态（U3）、不切行高（U4）、不复制任何绘制逻辑（防漂移）。
    /// RichTextLine 的 Draw 保留原 float 返回（U2）。参数对象（readonly struct）嵌套定义于本 static class 内。</summary>
    public static class TextCandAtoms
    {
        /// <summary>单位标签候选。value/unit 必填；valueFont 默认 <see cref="Tokens.FontValue"/>（=GameFont.Medium）；color 默认 <see cref="Tokens.Text"/>。</summary>
        public readonly struct TextUnitLabel
        {
            public readonly string value;
            public readonly string unit;
            public readonly GameFont valueFont;
            public readonly Color color;
            public TextUnitLabel(string value, string unit, GameFont valueFont, Color color)
            {
                this.value = value; this.unit = unit; this.valueFont = valueFont; this.color = color;
            }
        }

        /// <summary>单位标签：转发至 <see cref="TextAtoms.UnitLabel(Rect, string, string, GameFont, Color?)"/>。</summary>
        public static void UnitLabel(Rect rect, TextUnitLabel cand)
            => TextAtoms.UnitLabel(rect, cand.value, cand.unit, cand.valueFont, cand.color);

        /// <summary>打字机候选。text/progress01/font/color 必填（font、color 无默认，保持调用方显式意图）；anchor 默认 MiddleLeft。</summary>
        public readonly struct TextTypewriter
        {
            public readonly string text;
            public readonly float progress01;
            public readonly GameFont font;
            public readonly Color color;
            public readonly TextAnchor anchor;
            public TextTypewriter(string text, float progress01, GameFont font, Color color, TextAnchor anchor)
            {
                this.text = text; this.progress01 = progress01; this.font = font;
                this.color = color; this.anchor = anchor;
            }
        }

        /// <summary>打字机：转发至 <see cref="TextAtoms.Typewriter(Rect, string, float, GameFont, Color, TextAnchor)"/>。</summary>
        public static void Typewriter(Rect rect, TextTypewriter cand)
            => TextAtoms.Typewriter(rect, cand.text, cand.progress01, cand.font, cand.color, cand.anchor);

        /// <summary>主副双行候选。main/sub 必填。</summary>
        public readonly struct TextSubtitleLine
        {
            public readonly string main;
            public readonly string sub;
            public TextSubtitleLine(string main, string sub) { this.main = main; this.sub = sub; }
        }

        /// <summary>主副双行：转发至 <see cref="TextAtoms.SubtitleLine(Rect, string, string)"/>。</summary>
        public static void SubtitleLine(Rect rect, TextSubtitleLine cand)
            => TextAtoms.SubtitleLine(rect, cand.main, cand.sub);

        /// <summary>混排文本行候选。spans 复用 <see cref="RichSpan"/>[]；anchor 默认 MiddleLeft；defaultFont 默认 <see cref="Tokens.FontBody"/>（=GameFont.Small）。</summary>
        public readonly struct TextRichTextLine
        {
            public readonly RichSpan[] spans;
            public readonly TextAnchor anchor;
            public readonly GameFont defaultFont;
            public TextRichTextLine(RichSpan[] spans, TextAnchor anchor, GameFont defaultFont)
            {
                this.spans = spans; this.anchor = anchor; this.defaultFont = defaultFont;
            }
        }

        /// <summary>混排文本行：转发至 <see cref="TextAtoms.RichTextLine(Rect, RichSpan[], TextAnchor, GameFont)"/>；保留原 float 返回（U2）。</summary>
        public static float RichTextLine(Rect rect, TextRichTextLine cand)
            => TextAtoms.RichTextLine(rect, cand.spans, cand.anchor, cand.defaultFont);
    }
}
