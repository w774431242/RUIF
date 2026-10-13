using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>⑤ 进度候选组件的单入口绘制。每个 Draw 为具名方法（与 <see cref="ProgressCandFmt"/> 的 Build 同名），
    /// 仅把候选 struct 的位置参数原样转发给对应的 ProgressAtoms 原子；不持有状态（U3）、不切行高（U4）、不复制任何绘制逻辑（防漂移）。
    /// ZoomIndicator 的 Draw 保留原 bool 返回（U2）。参数对象（readonly struct）嵌套定义于本 static class 内。</summary>
    public static class ProgressCandAtoms
    {
        /// <summary>斜向擦除候选。progress01 必填；color 默认 <see cref="Tokens.Accent"/>；rows 默认 10 / slant 默认 0.35f（与 ProgressAtoms.FillWipe 一致）。</summary>
        public readonly struct ProgressFillWipe
        {
            public readonly float progress01;
            public readonly Color color;
            public readonly int rows;
            public readonly float slant;
            public ProgressFillWipe(float progress01, Color color, int rows, float slant)
            {
                this.progress01 = progress01; this.color = color; this.rows = rows; this.slant = slant;
            }
        }

        /// <summary>斜向擦除：转发至 <see cref="ProgressAtoms.FillWipe(Rect, float, Color?, int, float)"/>。</summary>
        public static void FillWipe(Rect rect, ProgressFillWipe cand) => ProgressAtoms.FillWipe(rect, cand.progress01, cand.color, cand.rows, cand.slant);

        /// <summary>容量条候选。used/capacity 必填；fill 可空（空→由 ProgressAtoms 按 r&gt;0.85?Warn:Alive 解析）。</summary>
        public readonly struct ProgressCapacityBar
        {
            public readonly float used;
            public readonly float capacity;
            public readonly Color? fill;
            public ProgressCapacityBar(float used, float capacity, Color? fill)
            {
                this.used = used; this.capacity = capacity; this.fill = fill;
            }
        }

        /// <summary>容量条：转发至 <see cref="ProgressAtoms.CapacityBar(Rect, float, float, Color?)"/>。</summary>
        public static void CapacityBar(Rect rect, ProgressCapacityBar cand) => ProgressAtoms.CapacityBar(rect, cand.used, cand.capacity, cand.fill);

        /// <summary>弧形进度环候选。progress01 必填；track 默认 <see cref="Tokens.BorderSoft"/>；fill 默认 <see cref="Tokens.Accent"/>；centerLabel 可空；sweepDeg 默认 360f / thickness01 默认 0.22f（与 ProgressAtoms.ProgressRing 一致）。</summary>
        public readonly struct ProgressRing
        {
            public readonly float progress01;
            public readonly Color track;
            public readonly Color fill;
            public readonly string centerLabel;
            public readonly float sweepDeg;
            public readonly float thickness01;
            public ProgressRing(float progress01, Color track, Color fill, string centerLabel, float sweepDeg, float thickness01)
            {
                this.progress01 = progress01; this.track = track; this.fill = fill;
                this.centerLabel = centerLabel; this.sweepDeg = sweepDeg; this.thickness01 = thickness01;
            }
        }

        /// <summary>弧形进度环：转发至 <see cref="ProgressAtoms.ProgressRing(Rect, float, Color?, Color?, string, float, float)"/>。Build 方法名为 Ring，本 Draw 同名 Ring（与 struct ProgressRing 区分）。</summary>
        public static void Ring(Rect rect, ProgressRing cand) => ProgressAtoms.ProgressRing(rect, cand.progress01, cand.track, cand.fill, cand.centerLabel, cand.sweepDeg, cand.thickness01);

        /// <summary>缩放指示候选。zoom 必填；resetValue 默认 1f（与 ProgressAtoms.ZoomIndicator 一致）；Draw 返回是否点击「适配」。</summary>
        public readonly struct ProgressZoomIndicator
        {
            public readonly float zoom;
            public readonly float resetValue;
            public ProgressZoomIndicator(float zoom, float resetValue)
            {
                this.zoom = zoom; this.resetValue = resetValue;
            }
        }

        /// <summary>缩放指示：转发至 <see cref="ProgressAtoms.ZoomIndicator(Rect, float, float)"/>；保留原 bool 返回（U2）。</summary>
        public static bool ZoomIndicator(Rect rect, ProgressZoomIndicator cand) => ProgressAtoms.ZoomIndicator(rect, cand.zoom, cand.resetValue);
    }
}
