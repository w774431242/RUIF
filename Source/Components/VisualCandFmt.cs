using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>② 布局视觉候选组件（S2 第三批原子）的纯计算 Build。
    /// 参数对象（readonly struct）已迁移至 <see cref="VisualCandAtoms"/> 内嵌套定义；此处仅把可选参数归一到令牌默认值，
    /// 返回对应的嵌套参数对象。字段 = VisualAtoms 对应原子的位置参数；color 必填项直接存 Color，可选 color 默认溯源 Tokens。
    /// 默认值（size=12f / margin=2f / fadeWidth=24f / segments=8 等）与 VisualAtoms 内部默认一致。</summary>
    public static class VisualCandFmt
    {
        public static VisualCandAtoms.VisualSwatch Swatch(Color color, bool selected = false)
            => new VisualCandAtoms.VisualSwatch(color, selected);

        public static VisualCandAtoms.VisualGradientBar GradientBar(Color lo, Color hi, int segments = 16)
            => new VisualCandAtoms.VisualGradientBar(lo, hi, segments);

        public static VisualCandAtoms.VisualPinBadge PinBadge(bool pinned, Color? color = null, float size = 12f, float margin = 2f)
            => new VisualCandAtoms.VisualPinBadge(pinned, color ?? Tokens.Accent, size, margin);

        public static VisualCandAtoms.VisualOverflowFade OverflowFade(string text, Color bgColor, GameFont font = GameFont.Small, Color? color = null, float fadeWidth = 24f, int segments = 8)
            => new VisualCandAtoms.VisualOverflowFade(text, bgColor, font, color ?? Tokens.Text, fadeWidth, segments);
    }
}
