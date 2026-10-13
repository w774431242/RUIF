using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>① 文本候选组件（S2 第一批原子）的纯计算 Build。
    /// 参数对象（readonly struct）已迁移至 <see cref="TextCandAtoms"/> 内嵌套定义；此处仅把可选参数归一到令牌默认值，
    /// 返回对应的嵌套参数对象。字段 = TextAtoms 对应原子的位置参数，默认值溯源 Tokens（valueFont→Tokens.FontValue、defaultFont→Tokens.FontBody、color→Tokens.Text）。
    /// RichTextLine 复用既有 <see cref="RichSpan"/> 数据 struct（U1：数据数组沿用既有数据 struct）。</summary>
    public static class TextCandFmt
    {
        // valueFont 默认 GameFont.Medium == Tokens.FontValue（令牌为属性，不能作为默认参数常量，故用等价的枚举字面量并注释溯源）。
        public static TextCandAtoms.TextUnitLabel UnitLabel(string value, string unit, GameFont valueFont = GameFont.Medium, Color? color = null)
            => new TextCandAtoms.TextUnitLabel(value, unit, valueFont, color ?? Tokens.Text);

        public static TextCandAtoms.TextTypewriter Typewriter(string text, float progress01, GameFont font, Color color, TextAnchor anchor = TextAnchor.MiddleLeft)
            => new TextCandAtoms.TextTypewriter(text, progress01, font, color, anchor);

        public static TextCandAtoms.TextSubtitleLine SubtitleLine(string main, string sub) => new TextCandAtoms.TextSubtitleLine(main, sub);

        // defaultFont 默认 GameFont.Small == Tokens.FontBody。
        public static TextCandAtoms.TextRichTextLine RichTextLine(RichSpan[] spans, TextAnchor anchor = TextAnchor.MiddleLeft, GameFont defaultFont = GameFont.Small)
            => new TextCandAtoms.TextRichTextLine(spans, anchor, defaultFont);
    }
}
