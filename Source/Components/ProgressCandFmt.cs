using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>⑤ 进度候选组件（批4-2a 原子 + v1.1 弧形进度环）的纯计算 Build。
    /// 参数对象（readonly struct）已迁移至 <see cref="ProgressCandAtoms"/> 内嵌套定义；此处仅把可选参数归一到令牌默认值，
    /// 返回对应的嵌套参数对象。可选 color 默认溯源 Tokens（fill→Tokens.Accent、track→Tokens.BorderSoft）。
    /// CapacityBar.fill 的默认（r&gt;0.85?Warn:Alive）为 ProgressAtoms 内部条件默认，候选层保持可空、原样转发由原子解析（U2：默认值溯源 Fmt，不越俎代庖）。</summary>
    public static class ProgressCandFmt
    {
        public static ProgressCandAtoms.ProgressFillWipe FillWipe(float progress01, Color? color = null, int rows = 10, float slant = 0.35f)
            => new ProgressCandAtoms.ProgressFillWipe(progress01, color ?? Tokens.Accent, rows, slant);

        public static ProgressCandAtoms.ProgressCapacityBar CapacityBar(float used, float capacity, Color? fill = null)
            => new ProgressCandAtoms.ProgressCapacityBar(used, capacity, fill);

        // 原子方法名为 ProgressRing，为避免与 struct ProgressRing 同名混淆，Build 命名为 Ring；struct 仍名为 ProgressRing（嵌套于 ProgressCandAtoms）。
        public static ProgressCandAtoms.ProgressRing Ring(float progress01, Color? track = null, Color? fill = null, string centerLabel = null, float sweepDeg = 360f, float thickness01 = 0.22f)
            => new ProgressCandAtoms.ProgressRing(progress01, track ?? Tokens.BorderSoft, fill ?? Tokens.Accent, centerLabel, sweepDeg, thickness01);

        public static ProgressCandAtoms.ProgressZoomIndicator ZoomIndicator(float zoom, float resetValue = 1f)
            => new ProgressCandAtoms.ProgressZoomIndicator(zoom, resetValue);
    }
}
