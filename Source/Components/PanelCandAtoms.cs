using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>④ 面板候选组件的单入口绘制。每个 Draw 为具名方法（与 <see cref="PanelCandFmt"/> 的 Build 同名），
    /// 仅把候选 struct 的位置参数原样转发给对应的 PanelAtoms 原子；不持有状态（U3）、不切行高（U4）、不复制任何绘制逻辑（防漂移）。
    /// 参数对象（readonly struct）嵌套定义于本 static class 内。</summary>
    public static class PanelCandAtoms
    {
        /// <summary>统计行候选。label/value 必填；bar01 可空（空=不画条）；barColor 默认 <see cref="Tokens.Accent"/>。</summary>
        public readonly struct PanelStatRow
        {
            public readonly string label;
            public readonly string value;
            public readonly float? bar01;
            public readonly Color barColor;
            public PanelStatRow(string label, string value, float? bar01, Color barColor)
            {
                this.label = label; this.value = value; this.bar01 = bar01; this.barColor = barColor;
            }
        }

        /// <summary>统计行：转发至 <see cref="PanelAtoms.StatRow(Rect, string, string, float?, Color?)"/>。</summary>
        public static void StatRow(Rect rect, PanelStatRow cand) => PanelAtoms.StatRow(rect, cand.label, cand.value, cand.bar01, cand.barColor);

        /// <summary>迷你档案条候选。initial/name/mood01/health01 必填。</summary>
        public readonly struct PanelMiniProfileChip
        {
            public readonly string initial;
            public readonly string name;
            public readonly float mood01;
            public readonly float health01;
            public PanelMiniProfileChip(string initial, string name, float mood01, float health01)
            {
                this.initial = initial; this.name = name; this.mood01 = mood01; this.health01 = health01;
            }
        }

        /// <summary>迷你档案条：转发至 <see cref="PanelAtoms.MiniProfileChip(Rect, string, string, float, float)"/>。</summary>
        public static void MiniProfileChip(Rect rect, PanelMiniProfileChip cand) => PanelAtoms.MiniProfileChip(rect, cand.initial, cand.name, cand.mood01, cand.health01);

        /// <summary>警示闪边候选。t 必填；color 默认 <see cref="Tokens.Dead"/>；thickness 默认 2f（与 PanelAtoms.FlashOverlay 一致）。</summary>
        public readonly struct PanelFlashOverlay
        {
            public readonly float t;
            public readonly Color color;
            public readonly float thickness;
            public PanelFlashOverlay(float t, Color color, float thickness)
            {
                this.t = t; this.color = color; this.thickness = thickness;
            }
        }

        /// <summary>警示闪边：转发至 <see cref="PanelAtoms.FlashOverlay(Rect, float, Color?, float)"/>。</summary>
        public static void FlashOverlay(Rect rect, PanelFlashOverlay cand) => PanelAtoms.FlashOverlay(rect, cand.t, cand.color, cand.thickness);

        /// <summary>分段头候选。title/count/expanded 必填（count 空串=不画计数）。</summary>
        public readonly struct PanelSectionBar
        {
            public readonly string title;
            public readonly string count;
            public readonly bool expanded;
            public PanelSectionBar(string title, string count, bool expanded)
            {
                this.title = title; this.count = count; this.expanded = expanded;
            }
        }

        /// <summary>分段头：转发至 <see cref="PanelAtoms.SectionBar(Rect, string, string, bool)"/>。</summary>
        public static void SectionBar(Rect rect, PanelSectionBar cand) => PanelAtoms.SectionBar(rect, cand.title, cand.count, cand.expanded);
    }
}
