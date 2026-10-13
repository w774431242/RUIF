using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// 图表纯计算（单测面，v1.1 T0+T1 批次）：标度、Nice 刻度、柱几何、扇区角、直方 bin、热力色。
    /// 不触 GUI、不读输入、无全局状态——与 Fmt 家族同一铁律。
    /// </summary>
    public static class ChartsFmt
    {
        /// <summary>
        /// 线性标度：v ∈ [lo,hi] → [outLo,outHi]（钳制，不外溢）。
        /// lo==hi（退化区间）返回 outLo，不除零。
        /// </summary>
        public static float Scale(float v, float lo, float hi, float outLo, float outHi)
        {
            if (hi - lo <= 1e-5f) return outLo;
            float t = Mathf.Clamp01((v - lo) / (hi - lo));
            return outLo + (outHi - outLo) * t;
        }

        /// <summary>系列最大值；空/全非正返回 fallback（保证后续除法安全）。</summary>
        public static float SeriesMax(float[] values, float fallback = 1f)
        {
            if (values == null || values.Length == 0) return fallback;
            float m = values[0];
            for (int i = 1; i < values.Length; i++) if (values[i] > m) m = values[i];
            return m <= 0f ? fallback : m;
        }

        /// <summary>系列 [min,max]（走势图局部幅度用）；空系列返回 fallback。</summary>
        public static void SeriesRange(float[] values, float fallback, out float lo, out float hi)
        {
            if (values == null || values.Length == 0) { lo = 0f; hi = fallback; return; }
            lo = values[0]; hi = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] < lo) lo = values[i];
                if (values[i] > hi) hi = values[i];
            }
            if (hi - lo <= 1e-5f) { lo -= 1f; hi += 1f; }   // 退化：撑开单位区间
        }

        /// <summary>
        /// 1/2/5×10^k Nice 刻度：写入 outBuf，返回条数（不超出 buf 容量）。
        /// 每帧调用零堆分配（缓冲区由调用方持有复用）。
        /// </summary>
        public static int NiceTicks(float lo, float hi, float[] outBuf, int target = 4)
        {
            if (outBuf == null || outBuf.Length == 0 || hi - lo <= 1e-5f) return 0;
            float raw = (hi - lo) / Mathf.Max(1, target);
            float mag = Mathf.Pow(10f, Mathf.Floor(Mathf.Log10(Mathf.Max(1e-6f, raw))));
            float norm = raw / mag;
            float step = mag * (norm < 1.5f ? 1f : norm < 3.5f ? 2f : norm < 7.5f ? 5f : 10f);
            int n = 0;
            for (float v = Mathf.Ceil(lo / step) * step; v <= hi + 1e-4f && n < outBuf.Length; v += step)
                outBuf[n++] = v;
            return n;
        }

        /// <summary>柱几何：n 根柱在宽 w 内按 gap 均分，输出第 i 根的宽度与左偏移（0≤i&lt;n）。</summary>
        public static void BarGeom(int i, int n, float w, float gap, out float barW, out float xOff)
        {
            n = Mathf.Max(1, n);
            barW = Mathf.Max(1f, (w - gap * (n - 1)) / n);
            xOff = i * (barW + gap);
        }

        /// <summary>饼/环扇区角宽（度）；value≤0 或 total≤0 返回 0（除零/负值守护）。</summary>
        public static float SweepDeg(float value, float total)
            => (value <= 0f || total <= 1e-6f) ? 0f : 360f * (value / total);

        /// <summary>直方图 bin 索引（钳制到 [0, bins-1]；退化区间归 0 号 bin）。</summary>
        public static int HistogramBin(float v, float lo, float hi, int bins)
        {
            bins = Mathf.Max(1, bins);
            if (hi - lo <= 1e-6f) return 0;
            int b = (int)((v - lo) / (hi - lo) * bins);
            return Mathf.Clamp(b, 0, bins - 1);
        }

        /// <summary>
        /// 热力颜色映射（例外白名单③：Card→Alive 令牌插值，数据语义而非 UI chrome）。
        /// </summary>
        public static Color HeatColor(float t)
            => Color.Lerp(Tokens.Card, Tokens.Alive, Mathf.Clamp01(t));
    }

    /// <summary>
    /// 图表原子（v1.1 T0+T1 批次，v1.0 API 冻结之上的纯增量）：
    /// 走势 / 柱状(V/H) / 堆叠柱 / 折线 / 面积 / 环形 / 饼 / 散点 / 热力网格，共 10 件。
    ///
    /// 铁律：
    /// ① 颜色只经令牌 / <see cref="Tokens.Series"/>，禁裸 new Color（例外白名单见 Tokens.cs 注释）；
    /// ② 不读 Mouse/Event——图表是纯输出组件，交互由调用方在 rect 上自行判定；
    /// ③ 零贴图：全部 Draw.Box / Widgets.DrawLine 直绘（与 Spinner 小方块近似同一先例）；
    /// ④ 刻度缓冲区框架内静态复用（tickBuf），每帧零堆分配。
    /// 已知取舍：AreaChart 为逐段垂直填充（阶梯近似），Donut 以旋转短弧段拼环——
    /// 高保真平滑留待 v1.2 素材/算法件评估（对齐图鉴 T2 口径）。
    /// </summary>
    public static class Charts
    {
        private static readonly float[] tickBuf = new float[8];   // 框架复用缓冲（IMGUI 单线程，安全）

        // ---- 走势 Sparkline（T0 头牌：局部 min/max 自适应幅度） ----

        /// <summary>迷你走势线。自适应局部幅度，适合 KPI 卡内嵌；单系列、无轴。</summary>
        public static void Sparkline(Rect r, float[] values, Color? color = null,
            float thickness = 1.5f, bool dots = false)
        {
            if (values == null || values.Length < 2) return;
            ChartsFmt.SeriesRange(values, 1f, out float lo, out float hi);
            Color c = color ?? Tokens.Accent;
            float stepX = r.width / (values.Length - 1);
            Vector2 prev = Pt(r, 0, values[0], lo, hi, stepX);
            if (dots) Draw.Box(new Rect(prev.x - 1.5f, prev.y - 1.5f, 3f, 3f), c);
            for (int i = 1; i < values.Length; i++)
            {
                Vector2 p = Pt(r, i, values[i], lo, hi, stepX);
                Widgets.DrawLine(prev, p, c, thickness);
                if (dots) Draw.Box(new Rect(p.x - 1.5f, p.y - 1.5f, 3f, 3f), c);
                prev = p;
            }
        }

        // ---- 柱状 ----

        /// <summary>垂直柱状图：共享 Nice 网格 + 可选类目标签（值/标签数取小者）。</summary>
        public static void BarChartV(Rect r, float[] values, string[] labels = null, Color? color = null)
        {
            if (values == null || values.Length == 0) return;
            bool hasLabels = labels != null && labels.Length > 0;
            Rect plot = PlotArea(r, hasLabels);
            float hi = ChartsFmt.SeriesMax(values);
            GridH(plot, hi);
            Color c = color ?? Tokens.Accent;
            float gap = Tokens.GridGap;
            for (int i = 0; i < values.Length; i++)
            {
                ChartsFmt.BarGeom(i, values.Length, plot.width, gap, out float bw, out float xo);
                float h = Mathf.Max(1f, ChartsFmt.Scale(values[i], 0f, hi, 0f, plot.height));
                Draw.Box(new Rect(plot.x + xo, plot.yMax - h, bw, h), c);
                if (hasLabels && i < labels.Length)
                    Draw.Label(new Rect(plot.x + xo - 2f, plot.yMax + 1f, bw + 4f, 13f),
                        labels[i], Tokens.FontLabel, Tokens.Dim, TextAnchor.LowerCenter);
            }
        }

        /// <summary>水平条形图：值越大条越长，标签行内左侧对齐。</summary>
        public static void BarChartH(Rect r, float[] values, string[] labels = null, Color? color = null)
        {
            if (values == null || values.Length == 0) return;
            float hi = ChartsFmt.SeriesMax(values);
            Color c = color ?? Tokens.Info;
            int n = values.Length;
            float rowH = r.height / n;
            float labelW = labels != null && labels.Length > 0 ? Mathf.Min(72f, r.width * 0.3f) : 0f;
            for (int i = 0; i < n; i++)
            {
                Rect row = new Rect(r.x, r.y + i * rowH, r.width, Mathf.Max(4f, rowH - 2f));
                if (labels != null && i < labels.Length)
                    Draw.Label(new Rect(row.x, row.y, labelW, row.height),
                        labels[i], Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
                float trackX = r.x + labelW;
                float trackW = Mathf.Max(0f, r.width - labelW);
                float w = Mathf.Max(1f, ChartsFmt.Scale(values[i], 0f, hi, 0f, trackW));
                Draw.Box(new Rect(trackX, row.y + (row.height - Mathf.Min(row.height, 10f)) / 2f,
                    w, Mathf.Min(row.height, 10f)), c);
            }
        }

        /// <summary>堆叠柱状图：series[系列][列]，系列色走 <see cref="Tokens.Series"/>。</summary>
        public static void StackedBarChartV(Rect r, float[][] series)
        {
            if (series == null || series.Length == 0) return;
            int cols = 0;
            float colMax = 0f;
            for (int j = 0; j < series.Length; j++)
            {
                if (series[j] == null) continue;
                if (series[j].Length > cols) cols = series[j].Length;
            }
            if (cols == 0) return;
            float[] totals = new float[cols];
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < series.Length; j++)
                    if (series[j] != null && i < series[j].Length) totals[i] += series[j][i];
                if (totals[i] > colMax) colMax = totals[i];
            }
            if (colMax <= 0f) colMax = 1f;
            GridH(r, colMax);
            float gap = Tokens.GridGap;
            for (int i = 0; i < cols; i++)
            {
                ChartsFmt.BarGeom(i, cols, r.width, gap, out float bw, out float xo);
                float cum = 0f;
                for (int j = 0; j < series.Length; j++)
                {
                    if (series[j] == null || i >= series[j].Length) continue;
                    float v = series[j][i];
                    if (v <= 0f) continue;
                    float h = ChartsFmt.Scale(v, 0f, colMax, 0f, r.height);
                    Draw.Box(new Rect(r.x + xo, r.yMax - cum - h, bw, Mathf.Max(1f, h)), Tokens.Series(j));
                    cum += h;
                }
            }
        }

        // ---- 折线 / 面积 ----

        /// <summary>折线图：基线 0 起，共享 Nice 网格，可选数据点。</summary>
        public static void LineChart(Rect r, float[] values, Color? color = null,
            bool dots = true, bool grid = true)
        {
            if (values == null || values.Length < 2) return;
            Rect plot = PlotArea(r, false);
            float hi = ChartsFmt.SeriesMax(values);
            if (grid) GridH(plot, hi);
            Color c = color ?? Tokens.Accent;
            float stepX = plot.width / (values.Length - 1);
            Vector2 prev = Pt(plot, 0, values[0], 0f, hi, stepX);
            if (dots) Draw.Box(new Rect(prev.x - 1.5f, prev.y - 1.5f, 3f, 3f), c);
            for (int i = 1; i < values.Length; i++)
            {
                Vector2 p = Pt(plot, i, values[i], 0f, hi, stepX);
                Widgets.DrawLine(prev, p, c, 1.5f);
                if (dots) Draw.Box(new Rect(p.x - 1.5f, p.y - 1.5f, 3f, 3f), c);
                prev = p;
            }
        }

        /// <summary>面积图：折线 + 半透明填充（逐段垂直填充的阶梯近似，已知取舍见类注释）。</summary>
        public static void AreaChart(Rect r, float[] values, Color? color = null)
        {
            if (values == null || values.Length < 2) return;
            Rect plot = PlotArea(r, false);
            float hi = ChartsFmt.SeriesMax(values);
            Color line = color ?? Tokens.Alive;
            Color fill = Draw.WithAlpha(line, 0.18f);
            float stepX = plot.width / (values.Length - 1);
            for (int i = 0; i < values.Length; i++)
            {
                float y = ChartsFmt.Scale(values[i], 0f, hi, plot.yMax, plot.y);
                float w = i == values.Length - 1 ? stepX : stepX + 1f;   // +1 消缝
                Draw.Box(new Rect(plot.x + i * stepX, y, w, plot.yMax - y), fill);
            }
            LineChart(plot, values, line, dots: false, grid: false);
        }

        // ---- 环形 / 饼 ----

        /// <summary>
        /// 环形图：值数组按 <see cref="Tokens.Series"/> 上色，hole01=0 即饼图（见 <see cref="PieChart"/>）。
        /// 中心可选 KPI 文本。12 点钟方向起始，顺时针。
        /// </summary>
        public static void DonutChart(Rect r, float[] values, float hole01 = 0.5f, string centerLabel = null)
        {
            if (values == null || values.Length == 0) return;
            float total = 0f;
            for (int i = 0; i < values.Length; i++) if (values[i] > 0f) total += values[i];
            if (total <= 0f) return;
            Vector2 c = r.center;
            float outer = Mathf.Min(r.width, r.height) / 2f - 1f;
            float holeR = Mathf.Max(0f, outer * Mathf.Clamp01(hole01));
            float thick = Mathf.Max(2f, outer - holeR);
            float ringMid = (outer + holeR) / 2f;
            float start = -90f;
            for (int i = 0; i < values.Length; i++)
            {
                float v = values[i];
                if (v <= 0f) continue;
                float sweep = ChartsFmt.SweepDeg(v, total);
                int samples = Mathf.Max(2, Mathf.CeilToInt(sweep / 3f));   // 每 ~3° 一段
                float stepAng = sweep / samples;
                float arcLen = Mathf.Max(1.5f, ringMid * stepAng * Mathf.Deg2Rad + 0.5f);
                for (int k = 0; k < samples; k++)
                {
                    float ang = start + (k + 0.5f) * stepAng;
                    using (Draw.Rotate(ang, c))
                        Draw.Box(new Rect(c.x + holeR, c.y - arcLen / 2f, thick, arcLen), Tokens.Series(i));
                }
                start += sweep;
            }
            if (!string.IsNullOrEmpty(centerLabel))
                Draw.Label(new Rect(c.x - outer, c.y - 11f, outer * 2f, 22f),
                    centerLabel, Tokens.FontValue, Tokens.Text, TextAnchor.MiddleCenter);
        }

        /// <summary>饼图 = hole01=0 的环形图。</summary>
        public static void PieChart(Rect r, float[] values, string centerLabel = null)
            => DonutChart(r, values, 0f, centerLabel);

        // ---- 散点 / 热力 ----

        /// <summary>散点图：domain/range 为数据坐标系（x/y 各自 min,max），点为小方块。</summary>
        public static void ScatterChart(Rect r, Vector2[] points, Vector2 domain, Vector2 range,
            Color? color = null)
        {
            AxisFrame(r);
            if (points == null) return;
            Color c = color ?? Tokens.Info;
            foreach (Vector2 p in points)
            {
                float x = ChartsFmt.Scale(p.x, domain.x, domain.y, r.x, r.xMax);
                float y = ChartsFmt.Scale(p.y, range.x, range.y, r.yMax, r.y);
                Draw.Box(new Rect(x - 1.5f, y - 1.5f, 3f, 3f), c);
            }
        }

        /// <summary>热力网格：grid[行,列]，按网格内最大值归一后走 <see cref="ChartsFmt.HeatColor"/>。</summary>
        public static void HeatmapGrid(Rect r, float[,] grid)
        {
            if (grid == null) return;
            int rows = grid.GetLength(0), cols = grid.GetLength(1);
            if (rows == 0 || cols == 0) return;
            float max = 0f;
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    if (grid[y, x] > max) max = grid[y, x];
            if (max <= 0f) max = 1f;
            float cw = r.width / cols, ch = r.height / rows;
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    Draw.Box(new Rect(r.x + x * cw + 0.5f, r.y + y * ch + 0.5f,
                        Mathf.Max(1f, cw - 1f), Mathf.Max(1f, ch - 1f)),
                        ChartsFmt.HeatColor(grid[y, x] / max));
        }

        // ---- 内部：坐标助手 ----

        private static Vector2 Pt(Rect r, int i, float v, float lo, float hi, float stepX)
            => new Vector2(r.x + i * stepX, ChartsFmt.Scale(v, lo, hi, r.yMax, r.y));

        /// <summary>绘图区：底部留出标签条（有类目标签时）。</summary>
        private static Rect PlotArea(Rect r, bool labels)
            => new Rect(r.x, r.y + 2f, r.width,
                Mathf.Max(4f, r.height - (labels ? 14f : 0f)));

        /// <summary>横向 Nice 网格线 + 右上角刻度值（hair 线 / dim 标签）。</summary>
        private static void GridH(Rect plot, float hi)
        {
            int n = ChartsFmt.NiceTicks(0f, hi, tickBuf);
            for (int i = 0; i < n; i++)
            {
                float y = ChartsFmt.Scale(tickBuf[i], 0f, hi, plot.yMax, plot.y);
                Draw.LineH(plot.x, y, plot.width, Tokens.BorderHair);
                Draw.Label(new Rect(plot.xMax - 34f, y - 12f, 34f, 12f),
                    tickBuf[i].ToString("0.#"), Tokens.FontLabel, Tokens.Dim, TextAnchor.UpperRight);
            }
        }

        /// <summary>散点坐标框架：左轴 + 底轴（hair 色）。</summary>
        private static void AxisFrame(Rect r)
        {
            Draw.LineV(r.x, r.y, r.height, Tokens.BorderHair);
            Draw.LineH(r.x, r.yMax - 1f, r.width, Tokens.BorderHair);
        }
    }
}
