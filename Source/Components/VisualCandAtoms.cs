using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>② 布局视觉候选组件的单入口绘制。每个 Draw 为具名方法（与 <see cref="VisualCandFmt"/> 的 Build 同名），
    /// 仅把候选 struct 的位置参数原样转发给对应的 VisualAtoms 原子；不持有状态（U3）、不切行高（U4）、不复制任何绘制逻辑（防漂移）。
    /// 参数对象（readonly struct）嵌套定义于本 static class 内。</summary>
    public static class VisualCandAtoms
    {
        /// <summary>色卡候选。color 必填；selected 默认 false。</summary>
        public readonly struct VisualSwatch
        {
            public readonly Color color;
            public readonly bool selected;
            public VisualSwatch(Color color, bool selected) { this.color = color; this.selected = selected; }
        }

        /// <summary>色卡：转发至 <see cref="VisualAtoms.Swatch(Rect, Color, bool)"/>。</summary>
        public static void Swatch(Rect rect, VisualSwatch cand) => VisualAtoms.Swatch(rect, cand.color, cand.selected);

        /// <summary>渐变条候选。lo/hi 必填；segments 默认 16（与 VisualAtoms.GradientBar 一致）。</summary>
        public readonly struct VisualGradientBar
        {
            public readonly Color lo;
            public readonly Color hi;
            public readonly int segments;
            public VisualGradientBar(Color lo, Color hi, int segments) { this.lo = lo; this.hi = hi; this.segments = segments; }
        }

        /// <summary>渐变条：转发至 <see cref="VisualAtoms.GradientBar(Rect, Color, Color, int)"/>。</summary>
        public static void GradientBar(Rect rect, VisualGradientBar cand) => VisualAtoms.GradientBar(rect, cand.lo, cand.hi, cand.segments);

        /// <summary>钉选角标候选。pinned 必填；color 默认 <see cref="Tokens.Accent"/>；size 默认 12f / margin 默认 2f（与 VisualAtoms.PinBadge 一致）。</summary>
        public readonly struct VisualPinBadge
        {
            public readonly bool pinned;
            public readonly Color color;
            public readonly float size;
            public readonly float margin;
            public VisualPinBadge(bool pinned, Color color, float size, float margin)
            {
                this.pinned = pinned; this.color = color; this.size = size; this.margin = margin;
            }
        }

        /// <summary>钉选角标：转发至 <see cref="VisualAtoms.PinBadge(Rect, bool, Color?, float, float)"/>。</summary>
        public static void PinBadge(Rect rect, VisualPinBadge cand) => VisualAtoms.PinBadge(rect, cand.pinned, cand.color, cand.size, cand.margin);

        /// <summary>右缘渐隐文字候选。text/bgColor 必填；font 默认 <see cref="Tokens.FontBody"/>（=GameFont.Small）；color 默认 <see cref="Tokens.Text"/>；fadeWidth 默认 24f / segments 默认 8（与 VisualAtoms.OverflowFade 一致）。</summary>
        public readonly struct VisualOverflowFade
        {
            public readonly string text;
            public readonly Color bgColor;
            public readonly GameFont font;
            public readonly Color color;
            public readonly float fadeWidth;
            public readonly int segments;
            public VisualOverflowFade(string text, Color bgColor, GameFont font, Color color, float fadeWidth, int segments)
            {
                this.text = text; this.bgColor = bgColor; this.font = font;
                this.color = color; this.fadeWidth = fadeWidth; this.segments = segments;
            }
        }

        /// <summary>右缘渐隐文字：转发至 <see cref="VisualAtoms.OverflowFade(Rect, string, Color, GameFont, Color?, float, int)"/>。</summary>
        public static void OverflowFade(Rect rect, VisualOverflowFade cand) => VisualAtoms.OverflowFade(rect, cand.text, cand.bgColor, cand.font, cand.color, cand.fadeWidth, cand.segments);
    }
}
