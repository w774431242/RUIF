using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>③ 状态候选组件（S2 第二批原子）的纯计算 Build。
    /// 参数对象（readonly struct）已迁移至 <see cref="StatusCandAtoms"/> 内嵌套定义（对齐 DataCandAtoms 的 BillEntry/FilterNode/TradeRow 写法）；
    /// 此处仅负责把可选参数归一到令牌默认值，返回对应的嵌套参数对象。候选层不出现任何 new Color 字面色相。
    /// 绘制由 <see cref="StatusCandAtoms"/> 的具名 Draw 方法原样转发给 StatusAtoms，零绘制逻辑。</summary>
    public static class StatusCandFmt
    {
        public static StatusCandAtoms.StatusPulseDot PulseDot(Color? color = null, float t = 0f, float minAlpha = 0.45f)
            => new StatusCandAtoms.StatusPulseDot(color ?? Tokens.Alive, t, minAlpha);

        public static StatusCandAtoms.StatusGlyph Glyph(StatusKind kind) => new StatusCandAtoms.StatusGlyph(kind);

        public static StatusCandAtoms.StatusSpinner Spinner(float t, Color? color = null, int segments = 8, float radius = 8f, float size = 3f, Texture2D tex = null)
            => new StatusCandAtoms.StatusSpinner(t, color ?? Tokens.Accent, segments, radius, size, tex);

        public static StatusCandAtoms.StatusCaret Caret(float progress01, Color? color = null)
            => new StatusCandAtoms.StatusCaret(progress01, color ?? Tokens.Muted);

        public static StatusCandAtoms.StatusScrollFadeHint ScrollFadeHint(bool moreBelow) => new StatusCandAtoms.StatusScrollFadeHint(moreBelow);
    }
}
