using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>③ 状态候选组件的单入口绘制。每个 Draw 为具名方法（与 <see cref="StatusCandFmt"/> 的 Build 同名），
    /// 仅把候选 struct 的位置参数原样转发给对应的 StatusAtoms 原子；不持有状态（U3）、不切行高（U4）、不复制任何绘制逻辑（防漂移）。
    /// 参数对象（readonly struct）嵌套定义于本 static class 内，对齐 DataCandAtoms 的 BillEntry/FilterNode/TradeRow 写法。</summary>
    public static class StatusCandAtoms
    {
        /// <summary>呼吸点候选。color 默认 <see cref="Tokens.Alive"/>；t 默认 0（静止于正弦中值）；minAlpha 默认 0.45f（与 <see cref="StatusFmt.PulseAlpha"/> 默认一致）。</summary>
        public readonly struct StatusPulseDot
        {
            /// <summary>点颜色（已溯源令牌）。</summary>
            public readonly Color color;
            /// <summary>动画相位（调用方驱动）。</summary>
            public readonly float t;
            /// <summary>最小 alpha（脉动下限）。</summary>
            public readonly float minAlpha;
            public StatusPulseDot(Color color, float t, float minAlpha) { this.color = color; this.t = t; this.minAlpha = minAlpha; }
        }

        /// <summary>呼吸点：转发至 <see cref="StatusAtoms.PulseDot(Rect, Color, float, float)"/>。</summary>
        public static void PulseDot(Rect rect, StatusPulseDot cand)
            => StatusAtoms.PulseDot(rect, cand.color, cand.t, cand.minAlpha);

        /// <summary>状态图形候选。kind 决定字形 + 令牌色；色由 <see cref="StatusAtoms.Glyph"/> / <see cref="StatusFmt.GlyphOf"/> 映射，候选层不存色。</summary>
        public readonly struct StatusGlyph
        {
            /// <summary>状态种类。</summary>
            public readonly StatusKind kind;
            public StatusGlyph(StatusKind kind) { this.kind = kind; }
        }

        /// <summary>状态图形：转发至 <see cref="StatusAtoms.Glyph(Rect, StatusKind)"/>。</summary>
        public static void Glyph(Rect rect, StatusGlyph cand) => StatusAtoms.Glyph(rect, cand.kind);

        /// <summary>加载指示器候选。color 默认 <see cref="Tokens.Accent"/>；segments=8 / radius=8f / size=3f 与 StatusAtoms.Spinner 默认一致；tex 非空走贴图重载。</summary>
        public readonly struct StatusSpinner
        {
            public readonly float t;
            public readonly Color color;
            public readonly int segments;
            public readonly float radius;
            public readonly float size;
            public readonly Texture2D tex;
            public StatusSpinner(float t, Color color, int segments, float radius, float size, Texture2D tex)
            {
                this.t = t; this.color = color; this.segments = segments;
                this.radius = radius; this.size = size; this.tex = tex;
            }
        }

        /// <summary>加载指示器：tex 非空 → 贴图重载；否则 → 零贴图 float 重载（color 已在 Build 中溯源 Tokens.Accent）。</summary>
        public static void Spinner(Rect rect, StatusSpinner cand)
        {
            if (cand.tex != null) StatusAtoms.Spinner(rect, cand.t, cand.tex);
            else StatusAtoms.Spinner(rect, cand.t, cand.color, cand.segments, cand.radius, cand.size);
        }

        /// <summary>展开箭头候选。progress01 驱动旋转过渡；color 默认 <see cref="Tokens.Muted"/>。</summary>
        public readonly struct StatusCaret
        {
            public readonly float progress01;
            public readonly Color color;
            public StatusCaret(float progress01, Color color) { this.progress01 = progress01; this.color = color; }
        }

        /// <summary>展开箭头：转发至 <see cref="StatusAtoms.Caret(Rect, float, Color?)"/>。</summary>
        public static void Caret(Rect rect, StatusCaret cand) => StatusAtoms.Caret(rect, cand.progress01, cand.color);

        /// <summary>滚动渐隐提示候选。moreBelow 决定是否绘制；viewport 即传入 rect（h=14f 为 StatusAtoms 内部常量，不入候选）。</summary>
        public readonly struct StatusScrollFadeHint
        {
            public readonly bool moreBelow;
            public StatusScrollFadeHint(bool moreBelow) { this.moreBelow = moreBelow; }
        }

        /// <summary>滚动渐隐提示：转发至 <see cref="StatusAtoms.ScrollFadeHint(Rect, bool)"/>。</summary>
        public static void ScrollFadeHint(Rect rect, StatusScrollFadeHint cand) => StatusAtoms.ScrollFadeHint(rect, cand.moreBelow);
    }
}
