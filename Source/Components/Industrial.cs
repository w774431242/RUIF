using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// 工业管理原子纯计算（可离线单测，v1.2 第一批 T1 四原子）：
    /// 表盘归一/分区、瓶颈检测、占比归一、状态游程归并、灯阵格几何。
    /// 不触 GUI、不读输入、无全局状态——与 Fmt 家族同一铁律。
    /// </summary>
    public static class IndustrialFmt
    {
        /// <summary>表盘归一：v ∈ [lo,hi] → [0,1]（钳制；退化区间返回 0，不除零）。</summary>
        public static float GaugeNorm(float v, float lo, float hi)
        {
            if (hi - lo <= 1e-5f) return 0f;
            return Mathf.Clamp01((v - lo) / (hi - lo));
        }

        /// <summary>
        /// 表盘分区：v01 与阈值比较 → 0 正常 / 1 警告 / 2 危险。
        /// 仅当 dead01 &gt; warn01 时危险区才生效（阈值倒置的坏配置不误报危险，fail-safe）。
        /// </summary>
        public static int GaugeZone(float v01, float warn01, float dead01)
        {
            v01 = Mathf.Clamp01(v01);
            warn01 = Mathf.Clamp01(warn01);
            dead01 = Mathf.Clamp01(dead01);
            if (v01 < warn01) return 0;
            return dead01 > warn01 && v01 >= dead01 ? 2 : 1;
        }

        /// <summary>瓶颈索引：耗时最大段；null / 空 / 全非正返回 -1。</summary>
        public static int BottleneckIndex(float[] stageTimes)
        {
            if (stageTimes == null || stageTimes.Length == 0) return -1;
            int idx = -1;
            float max = 0f;
            for (int i = 0; i < stageTimes.Length; i++)
                if (stageTimes[i] > max) { max = stageTimes[i]; idx = i; }
            return idx;
        }

        /// <summary>
        /// 占比归一：values 归一到和为 1 写入 outBuf，返回有效段数（不超出 buf 容量）。
        /// 负值按 0 处理；总和 ≤ 0（全空/全负）退化为均匀 1/n——绘制端永远拿得到合法占比。
        /// </summary>
        public static int Shares(float[] values, float[] outBuf)
        {
            int n = values == null ? 0 : values.Length;
            if (outBuf == null || outBuf.Length == 0 || n == 0) return 0;
            n = Mathf.Min(n, outBuf.Length);
            float total = 0f;
            for (int i = 0; i < n; i++) total += Mathf.Max(0f, values[i]);
            if (total <= 1e-6f)
            {
                for (int i = 0; i < n; i++) outBuf[i] = 1f / n;
                return n;
            }
            for (int i = 0; i < n; i++) outBuf[i] = Mathf.Max(0f, values[i]) / total;
            return n;
        }

        /// <summary>
        /// 状态游程归并：相邻同状态合并为一段，段长写入 outBuf，返回段数（截断至 buf 容量）。
        /// 供「逐 tick 状态采样 → 分段时序带」的预处理，纯 C# 可离线验证。
        /// </summary>
        public static int MergeRunLengths(int[] states, int[] outBuf)
        {
            if (states == null || states.Length == 0 || outBuf == null || outBuf.Length == 0) return 0;
            int segs = 0, run = 1;
            for (int i = 1; i <= states.Length; i++)
            {
                if (i < states.Length && states[i] == states[i - 1]) { run++; continue; }
                if (segs < outBuf.Length) outBuf[segs] = run;
                segs++;
                run = 1;
            }
            return Mathf.Min(segs, outBuf.Length);
        }

        /// <summary>灯阵格几何：第 i 格矩形（均分行列，1px 缝隙）。</summary>
        public static Rect LedCell(Rect r, int i, int cols, int rows)
        {
            cols = Mathf.Max(1, cols);
            rows = Mathf.Max(1, rows);
            int cx = i % cols, cy = i / cols;
            float cw = r.width / cols, ch = r.height / rows;
            return new Rect(r.x + cx * cw + 0.5f, r.y + cy * ch + 0.5f,
                Mathf.Max(1f, cw - 1f), Mathf.Max(1f, ch - 1f));
        }

        /// <summary>状态段总时长（归一化分母）；null / 全非正返回 1（除零守护，退化为每段等宽）。</summary>
        public static float SegmentsTotal(float[] durations)
        {
            if (durations == null) return 1f;
            float t = 0f;
            for (int i = 0; i < durations.Length; i++) if (durations[i] > 0f) t += durations[i];
            return t <= 1e-6f ? 1f : t;
        }
    }

    /// <summary>设备状态（工业管理语义，与 <see cref="Industrial.StateColor"/> 一一对应）。</summary>
    public enum MachineState { Run, Idle, Fault, Refuel, Offline }

    /// <summary>
    /// 工业管理原子（v1.2 第一批 T1 四原子，v1.0 API 冻结之上的纯增量）：
    /// Gauge 指针表盘 / LEDMatrix 状态灯阵 / BottleneckRibbon 瓶颈带 / MachineStateTimeline 设备状态时序。
    ///
    /// 铁律：
    /// ① 颜色只映射令牌（ZoneColor/LedColor/StateColor 三张色表，禁裸 new Color）；
    /// ② 不读 Mouse/Event——纯输出组件，数据由调用方给（孪生取数走 DataBridge，见设计提案）；
    /// ③ 零贴图：全部 Draw.Box / Draw.Border / Widgets.DrawLine 直绘；
    /// ④ 零逐帧堆分配（占比缓冲框架内静态复用）。
    /// </summary>
    public static class Industrial
    {
        private static readonly float[] shareBuf = new float[16];   // 占比复用缓冲（IMGUI 单线程，安全）

        // ---- Gauge 指针表盘 ----

        /// <summary>
        /// 指针表盘：上半环分区弧（正常/警告/危险）+ 指针 + 中心数值。
        /// v01 落入的分区决定数值文本颜色；warn01/dead01 为阈值（0-1，见 <see cref="IndustrialFmt.GaugeZone"/>）。
        /// </summary>
        public static void Gauge(Rect r, float v, float lo, float hi, string label = null,
            float warn01 = 0.6f, float dead01 = 0.85f)
        {
            float v01 = IndustrialFmt.GaugeNorm(v, lo, hi);
            int zone = IndustrialFmt.GaugeZone(v01, warn01, dead01);
            Vector2 c = new Vector2(r.center.x, r.yMax - 8f);
            float radius = Mathf.Min(r.width * 0.42f, r.height - 20f);
            // 分区弧：每 4° 一小格，颜色按该角度所属分区（旋转短弧段，同 DonutChart 先例）
            for (float a = 180f; a < 360f; a += 4f)
            {
                float t01 = (a - 180f) / 180f;
                Color zoneCol = ZoneColor(IndustrialFmt.GaugeZone(t01, warn01, dead01));
                using (Draw.Rotate(a, c))
                    Draw.Box(new Rect(c.x + radius - 8f, c.y - 1.5f, 8f, 3f), zoneCol);
            }
            // 指针 + 轴心
            float rad = GaugeNeedleDeg(v01) * Mathf.Deg2Rad;
            Vector2 tip = new Vector2(c.x + Mathf.Cos(rad) * (radius - 12f),
                c.y + Mathf.Sin(rad) * (radius - 12f));
            Widgets.DrawLine(c, tip, Tokens.Text, 2f);
            Draw.Box(new Rect(c.x - 2.5f, c.y - 2.5f, 5f, 5f), Tokens.Text);
            // 标签（左上）+ 数值（轴心上方，随分区变色）
            if (!string.IsNullOrEmpty(label))
                Draw.Label(new Rect(r.x, r.y, r.width, 14f), label,
                    Tokens.FontLabel, Tokens.Muted, TextAnchor.UpperLeft);
            Draw.Label(new Rect(c.x - radius, c.y - 24f, radius * 2f, 20f),
                Mathf.RoundToInt(v01 * 100f) + "%",
                Tokens.FontValue, ZoneColor(zone), TextAnchor.MiddleCenter);
        }

        /// <summary>指针角度（度）：v01=0 → 180°（左），v01=1 → 360°（右），上半环经过顶部。</summary>
        public static float GaugeNeedleDeg(float v01) => 180f + 180f * Mathf.Clamp01(v01);

        /// <summary>表盘分区色：0 正常 / 1 警告 / 2 危险（只映射令牌）。</summary>
        public static Color ZoneColor(int zone)
            => zone <= 0 ? Tokens.Alive : zone == 1 ? Tokens.Warn : Tokens.Dead;

        // ---- LEDMatrix 状态灯阵 ----

        /// <summary>状态灯阵：一格一机，颜色即状态；警告/故障格加描边提醒。</summary>
        public static void LedMatrix(Rect r, int[] statuses, int cols)
        {
            if (statuses == null || statuses.Length == 0) return;
            cols = Mathf.Max(1, cols);
            int rows = Mathf.CeilToInt(statuses.Length / (float)cols);
            for (int i = 0; i < statuses.Length; i++)
            {
                Rect cell = IndustrialFmt.LedCell(r, i, cols, rows);
                Draw.Box(cell, LedColor(statuses[i]));
                if (statuses[i] >= 2)
                    Draw.Border(cell, statuses[i] >= 3 ? Tokens.Text : Tokens.Border);
            }
        }

        /// <summary>灯阵状态色：0 熄 / 1 正常 / 2 警告 / 3 故障（≥4 归故障）。</summary>
        public static Color LedColor(int status)
            => status <= 0 ? Tokens.BorderHair
             : status == 1 ? Tokens.Alive
             : status == 2 ? Tokens.Warn
             : Tokens.Dead;

        // ---- BottleneckRibbon 瓶颈带 ----

        /// <summary>
        /// 瓶颈带：各段耗时占比条带，最大段标红描边并在下方标注占比。
        /// 段数上限 16（框架复用缓冲）；标签仅在该段足够宽（&gt;24px）时绘制。
        /// </summary>
        public static void BottleneckRibbon(Rect r, string[] stageLabels, float[] stageTimes,
            Color? color = null)
        {
            if (stageTimes == null || stageTimes.Length == 0) return;
            Color c = color ?? Tokens.Info;
            int bottleneck = IndustrialFmt.BottleneckIndex(stageTimes);
            float barH = Mathf.Min(22f, r.height - (bottleneck >= 0 ? 18f : 2f));
            int m = IndustrialFmt.Shares(stageTimes, shareBuf);
            float x = r.x;
            for (int i = 0; i < m; i++)
            {
                float w = Mathf.Max(1f, r.width * shareBuf[i] - 2f);
                Rect seg = new Rect(x, r.y, w, barH);
                if (i == bottleneck)
                {
                    Draw.Box(seg, Tokens.Dead);
                    Draw.Border(new Rect(seg.x - 1f, seg.y - 1f, seg.width + 2f, seg.height + 2f), Tokens.Text);
                }
                else
                {
                    Draw.Box(seg, c);
                }
                if (stageLabels != null && i < stageLabels.Length && w > 24f)
                    Draw.Label(new Rect(x, r.y + barH / 2f - 7f, w, 14f), stageLabels[i],
                        Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
                x += w + 2f;
            }
            if (bottleneck >= 0 && stageLabels != null && bottleneck < stageLabels.Length)
                Draw.Label(new Rect(r.x, r.y + barH + 3f, r.width, 14f),
                    Tr.Get("RUIF.Comp.Industrial.Bottleneck", stageLabels[bottleneck], (shareBuf[bottleneck] * 100f).ToString("0")),
                    Tokens.FontLabel, Tokens.Dead, TextAnchor.UpperLeft);
        }

        // ---- MachineStateTimeline 设备状态时序 ----

        /// <summary>
        /// 设备状态时序：多行 × 状态段（rows[i] 与 durations[i] 平行，durations 为相对时长）。
        /// 故障段加文本色描边；labels 可选（行首标签列）。
        /// </summary>
        public static void MachineStateTimeline(Rect r, string[] labels, MachineState[][] rows,
            float[][] durations)
        {
            if (rows == null || rows.Length == 0) return;
            bool hasLabels = labels != null && labels.Length > 0;
            float labelW = hasLabels ? Mathf.Min(72f, r.width * 0.3f) : 0f;
            float rowH = Mathf.Max(8f, r.height / rows.Length - 4f);
            for (int i = 0; i < rows.Length; i++)
            {
                float y = r.y + i * (rowH + 4f);
                if (hasLabels && i < labels.Length)
                    Draw.Label(new Rect(r.x, y, labelW, rowH), labels[i],
                        Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
                MachineState[] seg = rows[i];
                if (seg == null || seg.Length == 0) continue;
                float[] dur = durations != null && i < durations.Length ? durations[i] : null;
                float total = IndustrialFmt.SegmentsTotal(dur);
                float trackX = r.x + labelW, trackW = Mathf.Max(0f, r.width - labelW);
                float x = trackX;
                for (int k = 0; k < seg.Length; k++)
                {
                    float d = dur != null && k < dur.Length ? Mathf.Max(0f, dur[k]) : 0f;
                    float w = Mathf.Max(1f, trackW * (d / total) - 1f);
                    Rect s = new Rect(x, y, w, rowH);
                    Draw.Box(s, StateColor(seg[k]));
                    if (seg[k] == MachineState.Fault)
                        Draw.Border(new Rect(s.x - 1f, s.y - 1f, s.width + 2f, s.height + 2f), Tokens.Text);
                    x += w + 1f;
                }
            }
        }

        /// <summary>设备状态色：运行绿 / 怠机弱线 / 故障红 / 加料黄 / 离线灰（只映射令牌）。</summary>
        public static Color StateColor(MachineState s)
            => s == MachineState.Run ? Tokens.Alive
             : s == MachineState.Idle ? Tokens.BorderSoft
             : s == MachineState.Fault ? Tokens.Dead
             : s == MachineState.Refuel ? Tokens.Warn
             : Tokens.Dim;
    }
}
