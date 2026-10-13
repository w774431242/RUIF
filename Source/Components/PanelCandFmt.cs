using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>④ 面板候选组件（批4-2a 原子）的纯计算 Build。
    /// 参数对象（readonly struct）已迁移至 <see cref="PanelCandAtoms"/> 内嵌套定义；此处仅把可选参数归一到令牌默认值，
    /// 返回对应的嵌套参数对象。可选 color 默认溯源 Tokens（如 barColor→Tokens.Accent、FlashOverlay→Tokens.Dead）。
    /// 注：StatRow 的 labelRatio/gap/barW 为 PanelAtoms 内部常量，不入候选（候选仅暴露原子公开位置参数，U1）。</summary>
    public static class PanelCandFmt
    {
        public static PanelCandAtoms.PanelStatRow StatRow(string label, string value, float? bar01 = null, Color? barColor = null)
            => new PanelCandAtoms.PanelStatRow(label, value, bar01, barColor ?? Tokens.Accent);

        public static PanelCandAtoms.PanelMiniProfileChip MiniProfileChip(string initial, string name, float mood01, float health01)
            => new PanelCandAtoms.PanelMiniProfileChip(initial, name, mood01, health01);

        public static PanelCandAtoms.PanelFlashOverlay FlashOverlay(float t, Color? color = null, float thickness = 2f)
            => new PanelCandAtoms.PanelFlashOverlay(t, color ?? Tokens.Dead, thickness);

        public static PanelCandAtoms.PanelSectionBar SectionBar(string title, string count, bool expanded)
            => new PanelCandAtoms.PanelSectionBar(title, count, expanded);
    }
}
