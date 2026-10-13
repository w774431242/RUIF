using UnityEngine;

namespace RUIF
{
    /// <summary>
    /// ⑥ 工业管理候选层绘制转发：具名方法 Gauge / LedMatrix / BottleneckRibbon / MachineStateTimeline
    /// （与 <see cref="IndustrialCandFmt"/> 的 Build 工厂同名，已 == 对应 Build 名），纯转发既有 <see cref="Industrial"/>，禁复制逻辑。
    /// 候选 readonly struct 嵌套定义于本 static class（对齐 StatusCandAtoms.StatusPulseDot 写法）；
    /// 为避免与同名绘制方法（Gauge / LedMatrix / BottleneckRibbon / MachineStateTimeline）发生 C# 类型-成员命名冲突，
    /// struct 以族前缀命名为 <c>IndustrialGauge</c> / <c>IndustrialLedMatrix</c> / <c>IndustrialBottleneckRibbon</c> / <c>IndustrialMachineStateTimeline</c>。
    /// U4：仅消费传入 rect，行高 / 颜色映射交还 Industrial（沿用 Measure.RowHeight / Tokens）。
    /// </summary>
    public static class IndustrialCandAtoms
    {
        /// <summary>指针表盘候选（值参数包）。</summary>
        public readonly struct IndustrialGauge
        {
            /// <summary>当前值。</summary>
            public readonly float v;
            /// <summary>量程下限。</summary>
            public readonly float lo;
            /// <summary>量程上限。</summary>
            public readonly float hi;
            /// <summary>左上标题（null/空不显示）。</summary>
            public readonly string label;
            /// <summary>警告阈值（0~1）。</summary>
            public readonly float warn01;
            /// <summary>危险阈值（0~1）。</summary>
            public readonly float dead01;

            public IndustrialGauge(float v, float lo, float hi, string label, float warn01, float dead01)
            {
                this.v = v;
                this.lo = lo;
                this.hi = hi;
                this.label = label;
                this.warn01 = warn01;
                this.dead01 = dead01;
            }
        }

        /// <summary>状态灯阵候选（值参数包）。</summary>
        public readonly struct IndustrialLedMatrix
        {
            /// <summary>每台设备状态（0 熄 / 1 正常 / 2 警告 / 3 故障 / ≥4 归故障）。</summary>
            public readonly int[] statuses;
            /// <summary>列数（行数按长度自动推算）。</summary>
            public readonly int cols;

            public IndustrialLedMatrix(int[] statuses, int cols)
            {
                this.statuses = statuses;
                this.cols = cols;
            }
        }

        /// <summary>瓶颈带候选（值参数包）。color 默认 Tokens.Info（溯源 Tokens）。</summary>
        public readonly struct IndustrialBottleneckRibbon
        {
            /// <summary>各段标签（可空，段过窄时不绘制）。</summary>
            public readonly string[] stageLabels;
            /// <summary>各段耗时。</summary>
            public readonly float[] stageTimes;
            /// <summary>非瓶颈段颜色（默认溯源 Tokens.Info）。</summary>
            public readonly Color color;

            public IndustrialBottleneckRibbon(string[] stageLabels, float[] stageTimes, Color color)
            {
                this.stageLabels = stageLabels;
                this.stageTimes = stageTimes;
                this.color = color;
            }
        }

        /// <summary>设备状态时序候选（值参数包）。</summary>
        public readonly struct IndustrialMachineStateTimeline
        {
            /// <summary>行首标签（可空）。</summary>
            public readonly string[] labels;
            /// <summary>多行状态段（rows[i] 与 durations[i] 平行）。</summary>
            public readonly MachineState[][] rows;
            /// <summary>各段相对时长（与 rows 平行）。</summary>
            public readonly float[][] durations;

            public IndustrialMachineStateTimeline(string[] labels, MachineState[][] rows, float[][] durations)
            {
                this.labels = labels;
                this.rows = rows;
                this.durations = durations;
            }
        }

        /// <summary>指针表盘：转发 Industrial.Gauge。</summary>
        public static void Gauge(Rect r, IndustrialGauge cand)
            => Industrial.Gauge(r, cand.v, cand.lo, cand.hi, cand.label, cand.warn01, cand.dead01);

        /// <summary>状态灯阵：转发 Industrial.LedMatrix（颜色由 Industrial.LedColor→Tokens 决定）。</summary>
        public static void LedMatrix(Rect r, IndustrialLedMatrix cand)
            => Industrial.LedMatrix(r, cand.statuses, cand.cols);

        /// <summary>瓶颈带：转发 Industrial.BottleneckRibbon（标签行高 / 条高由 Industrial 内按 Measure 决定）。</summary>
        public static void BottleneckRibbon(Rect r, IndustrialBottleneckRibbon cand)
            => Industrial.BottleneckRibbon(r, cand.stageLabels, cand.stageTimes, cand.color);

        /// <summary>设备状态时序：转发 Industrial.MachineStateTimeline（颜色由 Industrial.StateColor→Tokens 决定）。</summary>
        public static void MachineStateTimeline(Rect r, IndustrialMachineStateTimeline cand)
            => Industrial.MachineStateTimeline(r, cand.labels, cand.rows, cand.durations);
    }
}
