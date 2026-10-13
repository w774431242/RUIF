using UnityEngine;

namespace RUIF
{
    /// <summary>
    /// ⑥ 工业管理候选层（参数对象 struct + Build，纯增量）：桥接既有 <see cref="Industrial"/>。
    /// 全部为值参数包（U4：行高 / 颜色映射由 Industrial 内部按 Tokens / Measure 决定，
    /// 候选不切行高、不散落魔法数，warn01/dead01 默认溯源为具名常量）。
    /// 候选 readonly struct 已迁移至 <see cref="IndustrialCandAtoms"/> 内嵌套定义（对齐 StatusCandAtoms.StatusPulseDot 写法，
    /// 并以族前缀 <c>IndustrialGauge</c> / <c>IndustrialLedMatrix</c> / <c>IndustrialBottleneckRibbon</c> / <c>IndustrialMachineStateTimeline</c>
    /// 规避与同名绘制方法的 C# 命名冲突）；
    /// 本文件<b>只放纯计算 Build 工厂、无 struct 定义</b>（R3）。绘制转发在 <see cref="IndustrialCandAtoms"/>，零绘制逻辑（GATE-ARCH）。
    /// </summary>
    public static class IndustrialCandFmt
    {
        /// <summary>表盘警告阈值默认（溯源：与 Industrial.Gauge 原始默认同源，禁散落魔法数）。</summary>
        private const float GaugeWarnDefault = 0.6f;
        /// <summary>表盘危险阈值默认（溯源同上）。</summary>
        private const float GaugeDeadDefault = 0.85f;

        /// <summary>指针表盘候选：返回 <see cref="IndustrialCandAtoms.IndustrialGauge"/>；warn01/dead01 默认溯源具名常量。</summary>
        public static IndustrialCandAtoms.IndustrialGauge Gauge(float v, float lo, float hi, string label = null,
            float warn01 = GaugeWarnDefault, float dead01 = GaugeDeadDefault)
            => new IndustrialCandAtoms.IndustrialGauge(v, lo, hi, label, warn01, dead01);

        /// <summary>状态灯阵候选：返回 <see cref="IndustrialCandAtoms.IndustrialLedMatrix"/>。</summary>
        public static IndustrialCandAtoms.IndustrialLedMatrix LedMatrix(int[] statuses, int cols)
            => new IndustrialCandAtoms.IndustrialLedMatrix(statuses, cols);

        /// <summary>瓶颈带候选：返回 <see cref="IndustrialCandAtoms.IndustrialBottleneckRibbon"/>；color 默认 Tokens.Info。</summary>
        public static IndustrialCandAtoms.IndustrialBottleneckRibbon BottleneckRibbon(string[] stageLabels, float[] stageTimes, Color? color = null)
            => new IndustrialCandAtoms.IndustrialBottleneckRibbon(stageLabels, stageTimes, color ?? Tokens.Info);

        /// <summary>设备状态时序候选：返回 <see cref="IndustrialCandAtoms.IndustrialMachineStateTimeline"/>。</summary>
        public static IndustrialCandAtoms.IndustrialMachineStateTimeline MachineStateTimeline(string[] labels, MachineState[][] rows, float[][] durations)
            => new IndustrialCandAtoms.IndustrialMachineStateTimeline(labels, rows, durations);
    }
}
