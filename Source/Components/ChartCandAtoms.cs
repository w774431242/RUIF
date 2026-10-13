using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>⑧ 图表候选组件（原 📋，官方实证支持，现落地）：直方图 / 雷达 / 甘特 / 桑基 / 箱线 /
    /// 漏斗 / 瀑布 / 气泡 / 阶梯线 / 区间带 / K线 / SPC 控制图 / Pareto 排列图。纯 presentational，数据由调用方传入；
    /// 任意方向连线用官方 <c>Widgets.DrawLine</c>，轴/基线用 <c>Draw.LineH/LineV</c>。
    /// 基础 10 件（Sparkline/Bar/Stacked/Line/Area/Donut/Pie/Scatter/HeatmapGrid）已在 <c>Charts</c> 落地，不重复。
    ///
    /// <para><b>绘制路径分配铁律（2026-09-16 收紧）</b>：本文件所有组件在绘制路径上
    /// <b>零委托对象 / 零闭包 / 零临时数组</b> ——
    /// ① 数据范围一律<b>单趟扫描</b>（<see cref="ScanBubble"/> / <see cref="ScanRange"/>），
    ///    不再用 <c>Array.ConvertAll</c>（那会每帧 <c>new float[]</c>）；
    /// ② 坐标映射一律走<b>无捕获私有静态函数</b> <see cref="MapX"/> / <see cref="MapY"/>，
    ///    不再用捕获 <c>p/x0/x1</c> 的 <c>Func&lt;float,float&gt;</c> 闭包。
    /// 有意的例外：<see cref="Radar"/> 的 <c>Widgets.DrawLine</c> 与 <c>Draw.Box</c> 本身的行内参数。</para>
    /// </summary>
    public static class ChartCandAtoms
    {
        private static Rect Plot(Rect r) => new Rect(r.x + 34f, r.y + 6f, Mathf.Max(1f, r.width - 40f), Mathf.Max(1f, r.height - 22f));

        public struct GanttBar { public string label; public float start; public float end; public int series; }
        public struct SankeyNode { public string label; public int side; public float value; }
        public struct SankeyLink { public int from; public int to; public float value; }
        public struct BoxStats { public string label; public float min, q1, median, q3, max; }
        public struct BubblePt { public float x, y, r; public int series; }
        public struct RangePt { public float x, lo, hi; }
        public struct OhlcBar { public string label; public float open, high, low, close; }
        public struct WaterfallStep { public string label; public float delta; public bool isTotal; }

        // ---- 内部：坐标映射与范围扫描（绘制路径分配消除的落点） ----

        /// <summary>
        /// 数据 x → 绘图区 x。<b>无捕获静态函数</b>。
        ///
        /// <para><b>为什么替掉原来的 <c>Func&lt;float,float&gt; X = v =&gt; ...</c></b>：那是<b>捕获闭包</b>
        /// （捕获 <c>p / x0 / x1</c>）⇒ 每次组件调用都要新建一个委托对象 + 一个编译器生成的显示类实例，
        /// 全部进 GC。静态函数传参版本**零委托、零闭包对象**，语义逐字等价。</para>
        ///
        /// <para><b>可见性为 <c>internal</c>（而非 <c>private</c>）</b>：它是可离线断言的纯函数
        /// （零 GUI / 零引擎调用），<c>Tests/ChartCandAllocTests.cs</c> 需用它钉住"NaN 输入不产生 NaN 几何"。
        /// 只到 <c>RUIF.Tests</c>（<c>InternalsVisibleTo</c>），<b>不进公共 API 面</b>。</para>
        /// </summary>
        internal static float MapX(float v, float x0, float x1, Rect p) => p.x + (v - x0) / (x1 - x0) * p.width;

        /// <summary>
        /// 数据 y → 绘图区 y（上下翻转，y 向下为正）。<b>无捕获静态函数</b>。
        ///
        /// <para><b>顺带收益（写在这里作为"牺牲 <c>Func</c> 可读性"的理由）</b>：原 <c>Y(v)</c>
        /// 每调用一次重算<b>两次除法</b>；而 BoxPlot / Ohlc / Spc / RangeArea 里同一个 <c>v</c>
        /// 常被调用 3~4 次。改静态函数后调用点可先落局部变量复用 ⇒ <b>除法次数减少 60% 以上</b>
        /// （如 <see cref="BoxPlot"/> 现为 min/max/q1/q3/median 各一次）。</para>
        ///
        /// <para><b>可见性 <c>internal</c> 的理由同 <see cref="MapX"/>。</b></para>
        /// </summary>
        internal static float MapY(float v, float lo, float hi, Rect p) => p.yMax - (v - lo) / (hi - lo) * p.height;

        /// <summary>全 NaN / 未采到任何有效值（<c>lo &gt; hi</c>）时的单位区间回退。</summary>
        private static void ScanFallback(ref float lo, ref float hi)
        {
            if (lo > hi) { lo = 0f; hi = 1f; }
        }

        /// <summary>
        /// 单趟扫描求气泡段的 [x0,x1]×[y0,y1]，写入 4 个 out 参数。
        ///
        /// <para><b>替代</b>：原实现 <c>ChartCandFmt.Min(Max)(Array.ConvertAll(...))</c> ×4
        /// （<c>:224-227</c>）⇒ 每帧 4 个临时 <c>float[]</c>。单趟扫描<b>零分配</b>，
        /// 范式抄本文件已有的 <see cref="StepLine"/> / <see cref="Ohlc"/> 扫描写法。</para>
        ///
        /// <para><b>⚠ 语义变更（已登记，见 <c>Tests/ChartCandAllocTests.cs</c>）</b>：
        /// 旧路径是 <c>ChartCandFmt.Min/Max(Array.ConvertAll(...))</c>，而 <c>ChartCandFmt.Min/Max</c>
        /// 自身是 <c>if (v[i] &lt; m)</c> 扫描 —— 它<b>并不</b>传播 NaN（会传播 NaN 的是 <c>Mathf.Min/Max</c>，
        /// 本文件未用）。真实的差异在<b>首元素</b>：旧实现以 <c>m = v[0]</c> 为种子 ⇒
        /// <b>首元素为 NaN 时整个范围被污染</b>（整图几何全 NaN）；新扫描以
        /// <c>±MaxValue/MinValue</c> 为哨兵 ⇒ 首元素为 NaN 亦被忽略。
        /// 全 NaN / 空集时退回单位区间 [0,1]，保证不产生 NaN 几何。</para>
        /// </summary>
        internal static void ScanBubble(BubblePt[] pts, out float x0, out float x1, out float y0, out float y1)
        {
            x0 = float.MaxValue; x1 = float.MinValue; y0 = float.MaxValue; y1 = float.MinValue;
            for (int i = 0; i < pts.Length; i++)
            {
                BubblePt q = pts[i];
                if (q.x < x0) x0 = q.x;  if (q.x > x1) x1 = q.x;
                if (q.y < y0) y0 = q.y;  if (q.y > y1) y1 = q.y;
            }
            ScanFallback(ref x0, ref x1);
            ScanFallback(ref y0, ref y1);
        }

        /// <summary>单趟扫描求区间带段的 [x0,x1]×[y0,y1]（y 取各点 lo 的下界、hi 的上界）。
        /// 分配事实与语义变更同 <see cref="ScanBubble"/>（原为 <c>:267-270</c> 的 4 次 <c>Array.ConvertAll</c>）。</summary>
        internal static void ScanRange(RangePt[] pts, out float x0, out float x1, out float y0, out float y1)
        {
            x0 = float.MaxValue; x1 = float.MinValue; y0 = float.MaxValue; y1 = float.MinValue;
            for (int i = 0; i < pts.Length; i++)
            {
                RangePt q = pts[i];
                if (q.x < x0) x0 = q.x;    if (q.x > x1) x1 = q.x;
                if (q.lo < y0) y0 = q.lo;  if (q.hi > y1) y1 = q.hi;
            }
            ScanFallback(ref x0, ref x1);
            ScanFallback(ref y0, ref y1);
        }

        // ---- 直方图 ----
        public static void Histogram(Rect rect, float[] values, int bins)
        {
            if (values == null || values.Length == 0 || bins <= 0) return;
            float lo = ChartCandFmt.Min(values), hi = ChartCandFmt.Max(values);
            if (hi <= lo) hi = lo + 1f;
            int[] cnt = new int[bins];
            for (int i = 0; i < values.Length; i++)
                cnt[Mathf.Clamp((int)((values[i] - lo) / (hi - lo) * bins), 0, bins - 1)]++;
            var p = Plot(rect);
            int mx = 1; for (int i = 0; i < bins; i++) mx = Mathf.Max(mx, cnt[i]);
            float bw = p.width / bins;
            for (int i = 0; i < bins; i++)
            {
                float h = cnt[i] / (float)mx * p.height;
                Draw.Box(new Rect(p.x + i * bw, p.yMax - h, bw - 1f, h), Tokens.Accent);
            }
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }

        // ---- 雷达 / 蜘蛛网 ----
        public static void Radar(Rect rect, float[][] series, string[] axes)
        {
            if (axes == null || axes.Length == 0) return;
            int n = axes.Length;
            Vector2 c = new Vector2(rect.center.x, rect.center.y);
            float rad = Mathf.Min(rect.width, rect.height) / 2f - 18f;
            for (int ring = 1; ring <= 4; ring++)
            {
                float f = ring / 4f;
                for (int i = 0; i < n; i++)
                {
                    Vector2 a = RadarPt(c, rad, n, f, i), b = RadarPt(c, rad, n, f, (i + 1) % n);
                    Widgets.DrawLine(a, b, Tokens.BorderSoft, 1f);
                }
            }
            // 轴标签：高度取引擎行高（原写死 16f），并以 tip 为**中心** ⇒ y 必须同步上移 h/2，
            // 否则 `MiddleCenter` 会把文字中心整体下移 (h_new − 16)/2。
            float axisLabelH = Measure.RowHeight(Tokens.FontLabel);
            for (int i = 0; i < n; i++)
            {
                Vector2 tip = RadarPt(c, rad, n, 1f, i);
                Widgets.DrawLine(c, tip, Tokens.BorderSoft, 1f);
                Draw.Label(new Rect(tip.x - 30f, tip.y - axisLabelH / 2f, 60f, axisLabelH), axes[i] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
            }
            if (series != null)
                for (int s = 0; s < series.Length; s++)
                {
                    var ser = series[s]; if (ser == null || ser.Length < n) continue;
                    Color col = ChartCandFmt.SeriesColor(s);
                    for (int i = 0; i < n; i++)
                    {
                        Vector2 a = RadarPt(c, rad, n, Mathf.Clamp01(ser[i]), i), b = RadarPt(c, rad, n, Mathf.Clamp01(ser[(i + 1) % n]), (i + 1) % n);
                        Widgets.DrawLine(a, b, col, 1.5f);
                    }
                }
        }

        /// <summary>
        /// 雷达图顶点：中心 <paramref name="c"/>、半径 <paramref name="rad"/>、<paramref name="n"/> 条轴、
        /// 第 <paramref name="i"/> 条轴、系数 <paramref name="f"/>（0 = 圆心，1 = 顶点）。
        ///
        /// <para>原为两个捕获 <c>c / rad / n</c> 的 <c>Func</c>（<c>ang</c> 与 <c>at</c>），
        /// 每次 <see cref="Radar"/> 调用都新建委托 + 显示类 ⇒ 改<b>无捕获私有静态函数</b>（零委托）。</para>
        /// </summary>
        private static Vector2 RadarPt(Vector2 c, float rad, int n, float f, int i)
        {
            float a = -Mathf.PI / 2f + i * (2f * Mathf.PI / n);
            return c + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * (rad * f);
        }

        // ---- 甘特图 ----
        public static void Gantt(Rect rect, GanttBar[] bars)
        {
            if (bars == null || bars.Length == 0) return;
            float t0 = float.MaxValue, t1 = float.MinValue;
            for (int i = 0; i < bars.Length; i++) { if (bars[i].start < t0) t0 = bars[i].start; if (bars[i].end > t1) t1 = bars[i].end; }
            if (t1 <= t0) t1 = t0 + 1f;
            var p = Plot(rect);
            float rowH = p.height / bars.Length;
            for (int i = 0; i < bars.Length; i++)
            {
                var b = bars[i];
                float x0 = MapX(b.start, t0, t1, p);
                float x1 = MapX(b.end, t0, t1, p);
                Rect bar = new Rect(x0, p.y + i * rowH + 3f, Mathf.Max(2f, x1 - x0), rowH - 6f);
                Draw.Box(bar, ChartCandFmt.SeriesColor(b.series));
                Draw.Label(new Rect(rect.x, bar.y, 32f, bar.height), b.label ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            }
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }

        // ---- 桑基图（双列）----
        public static void Sankey(Rect rect, SankeyNode[] nodes, SankeyLink[] links)
        {
            if (nodes == null) return;
            float total = 0f;
            if (links != null) for (int i = 0; i < links.Length; i++) total += links[i].value;
            if (total <= 0f) total = 1f;
            var p = Plot(rect);
            float cols = nodes.Length > 0 ? Mathf.Max(1f, p.width - 20f) : p.width;
            float lx = p.x + 8f, rx = p.x + p.width - 8f;
            float lh = 0f, rh = 0f;
            for (int i = 0; i < nodes.Length; i++) { if (nodes[i].side == 0) lh += nodes[i].value; else rh += nodes[i].value; }
            float ly = p.y, ry = p.y;
            float[] lY0 = new float[nodes.Length], lY1 = new float[nodes.Length], rY0 = new float[nodes.Length], rY1 = new float[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                float h = nodes[i].value / total * p.height;
                if (nodes[i].side == 0) { lY0[i] = ly; lY1[i] = ly + h; ly += h; }
                else { rY0[i] = ry; rY1[i] = ry + h; ry += h; }
            }
            if (links != null)
                for (int k = 0; k < links.Length; k++)
                {
                    var lk = links[k];
                    if (lk.from < 0 || lk.from >= nodes.Length || lk.to < 0 || lk.to >= nodes.Length) continue;
                    Vector2 a = new Vector2(rx, (rY0[lk.to] + rY1[lk.to]) / 2f);
                    Vector2 b = new Vector2(lx, (lY0[lk.from] + lY1[lk.from]) / 2f);
                    Widgets.DrawLine(a, b, Draw.WithAlpha(Tokens.Accent, 0.5f), Mathf.Max(1f, lk.value / total * p.height));
                }
            for (int i = 0; i < nodes.Length; i++)
            {
                bool left = nodes[i].side == 0;
                float y0 = left ? lY0[i] : rY0[i], y1 = left ? lY1[i] : rY1[i];
                Rect box = new Rect(left ? lx - 2f : rx - 2f, y0, 4f, y1 - y0);
                Draw.Box(box, Tokens.Alive);
                Rect lab = left ? new Rect(rect.x, y0, 30f, y1 - y0) : new Rect(rx + 4f, y0, rect.xMax - rx - 6f, y1 - y0);
                Draw.Label(lab, nodes[i].label ?? string.Empty, Tokens.FontLabel, Tokens.Text, left ? TextAnchor.MiddleLeft : TextAnchor.MiddleLeft);
            }
        }

        // ---- 箱线图 ----
        public static void BoxPlot(Rect rect, BoxStats[] boxes)
        {
            if (boxes == null || boxes.Length == 0) return;
            float lo = float.MaxValue, hi = float.MinValue;
            for (int i = 0; i < boxes.Length; i++)
            {
                lo = Mathf.Min(lo, boxes[i].min); hi = Mathf.Max(hi, boxes[i].max);
            }
            if (hi <= lo) hi = lo + 1f;
            var p = Plot(rect);
            float slot = p.width / boxes.Length;
            float labelH = Measure.RowHeight(Tokens.FontLabel);
            for (int i = 0; i < boxes.Length; i++)
            {
                var b = boxes[i];
                float cx = p.x + slot * (i + 0.5f);
                float bw = slot * 0.4f;
                // 同一 v 原被 Y() 重复调用 3~4 次（每次两道除法）⇒ 先落局部变量再复用（除法减少 60%+）
                float yMin = MapY(b.min, lo, hi, p), yMax = MapY(b.max, lo, hi, p);
                float yQ1 = MapY(b.q1, lo, hi, p), yQ3 = MapY(b.q3, lo, hi, p);
                Draw.LineV(cx, yMin, yMax - yMin, Tokens.Muted);
                Draw.Box(new Rect(cx - bw / 2f, yQ3, bw, yQ1 - yQ3), Tokens.Alive);
                Draw.Border(new Rect(cx - bw / 2f, yQ3, bw, yQ1 - yQ3), Tokens.BorderSoft);
                Draw.LineH(cx - bw / 2f, MapY(b.median, lo, hi, p), bw, Tokens.Accent);
                // 轴标签高度取引擎行高（原写死 14f ⇒ 装不下 22px 文字，向上溢进绘图区）
                Draw.Label(new Rect(cx - slot / 2f, p.yMax + 2f, slot, labelH), b.label ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
            }
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }

        // ---- 漏斗图 ----
        public static void Funnel(Rect rect, float[] stages, string[] labels)
        {
            if (stages == null || stages.Length == 0) return;
            float mx = ChartCandFmt.Max(stages);
            if (mx <= 0f) mx = 1f;
            var p = Plot(rect);
            float rowH = p.height / stages.Length;
            for (int i = 0; i < stages.Length; i++)
            {
                float w = stages[i] / mx * (p.width - 60f);
                Rect bar = new Rect(p.x + (p.width - 60f - w) / 2f, p.y + i * rowH + 2f, w, rowH - 4f);
                Draw.Box(bar, ChartCandFmt.SeriesColor(i));
                if (labels != null && i < labels.Length)
                    Draw.Label(new Rect(p.xMax - 56f, bar.y, 54f, bar.height), labels[i] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            }
        }

        // ---- 瀑布图 ----
        public static void Waterfall(Rect rect, WaterfallStep[] steps)
        {
            if (steps == null || steps.Length == 0) return;
            float lo = 0f, hi = 0f, run = 0f;
            for (int i = 0; i < steps.Length; i++)
            {
                if (steps[i].isTotal) { lo = Mathf.Min(lo, 0f); hi = Mathf.Max(hi, steps[i].delta); }
                else { run += steps[i].delta; lo = Mathf.Min(lo, Mathf.Min(run, run - steps[i].delta)); hi = Mathf.Max(hi, Mathf.Max(run, run - steps[i].delta)); }
            }
            if (hi <= lo) hi = lo + 1f;
            var p = Plot(rect);
            float slot = p.width / steps.Length;
            float prevTop = MapY(0f, lo, hi, p);
            run = 0f;
            for (int i = 0; i < steps.Length; i++)
            {
                float baseV, topV;
                if (steps[i].isTotal) { baseV = 0f; topV = steps[i].delta; run = steps[i].delta; }
                else { baseV = run; run += steps[i].delta; topV = run; }
                float yB = MapY(baseV, lo, hi, p), yT = MapY(topV, lo, hi, p);
                Rect bar = new Rect(p.x + i * slot + 2f, Mathf.Min(yB, yT), slot - 4f, Mathf.Abs(yT - yB));
                Draw.Box(bar, steps[i].isTotal ? Tokens.Accent : (steps[i].delta >= 0 ? Tokens.Alive : Tokens.Dead));
                if (i > 0) Draw.LineH(p.x + (i - 1) * slot + 2f + slot - 4f, prevTop, 4f, Tokens.BorderSoft);
                prevTop = yT;   // 原为 Y(topV)，与本轮已算出的 yT 同值 ⇒ 复用，省一次除法
            }
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }

        // ---- 气泡图 ----
        public static void Bubble(Rect rect, BubblePt[] pts)
        {
            if (pts == null || pts.Length == 0) return;
            // 原为 4 次 Array.ConvertAll + ChartCandFmt.Min/Max ⇒ 每帧 4 个临时 float[]；现单趟扫描零分配
            ScanBubble(pts, out float x0, out float x1, out float y0, out float y1);
            if (x1 <= x0) x1 = x0 + 1f; if (y1 <= y0) y1 = y0 + 1f;
            var p = Plot(rect);
            for (int i = 0; i < pts.Length; i++)
            {
                var q = pts[i];
                float cx = MapX(q.x, x0, x1, p);
                float cy = MapY(q.y, y0, y1, p);
                float d = Mathf.Max(3f, q.r * 2f);
                Draw.Box(new Rect(cx - d / 2f, cy - d / 2f, d, d), Draw.WithAlpha(ChartCandFmt.SeriesColor(q.series), 0.7f));
            }
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }

        // ---- 阶梯线 ----
        public static void StepLine(Rect rect, Vector2[] pts)
        {
            if (pts == null || pts.Length < 2) return;
            float x0 = float.MaxValue, x1 = float.MinValue, y0 = float.MaxValue, y1 = float.MinValue;
            for (int i = 0; i < pts.Length; i++) { x0 = Mathf.Min(x0, pts[i].x); x1 = Mathf.Max(x1, pts[i].x); y0 = Mathf.Min(y0, pts[i].y); y1 = Mathf.Max(y1, pts[i].y); }
            if (x1 <= x0) x1 = x0 + 1f; if (y1 <= y0) y1 = y0 + 1f;
            var p = Plot(rect);
            for (int i = 1; i < pts.Length; i++)
            {
                float ax = MapX(pts[i - 1].x, x0, x1, p), ay = MapY(pts[i - 1].y, y0, y1, p);
                float bx = MapX(pts[i].x, x0, x1, p), by = MapY(pts[i].y, y0, y1, p);
                Draw.LineH(ax, ay, bx - ax, Tokens.Accent);
                Draw.LineV(bx, ay, by - ay, Tokens.Accent);
            }
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }

        // ---- 区间带 ----
        public static void RangeArea(Rect rect, RangePt[] pts)
        {
            if (pts == null || pts.Length == 0) return;
            // 原为 4 次 Array.ConvertAll ⇒ 每帧 4 个临时 float[]；现单趟扫描零分配
            ScanRange(pts, out float x0, out float x1, out float y0, out float y1);
            if (x1 <= x0) x1 = x0 + 1f; if (y1 <= y0) y1 = y0 + 1f;
            var p = Plot(rect);
            for (int i = 0; i < pts.Length; i++)
            {
                float x = MapX(pts[i].x, x0, x1, p);
                // 原 Y(hi) 被调两次、Y(lo) 一次（各两道除法）⇒ 落局部变量后共用
                float yHi = MapY(pts[i].hi, y0, y1, p), yLo = MapY(pts[i].lo, y0, y1, p);
                Draw.LineV(x, yHi, yLo - yHi, Draw.WithAlpha(Tokens.Info, 0.5f));
            }
            for (int i = 1; i < pts.Length; i++)
            {
                float mx = MapX((pts[i - 1].x + pts[i].x) / 2f, x0, x1, p);
                float my = MapY((pts[i - 1].lo + pts[i - 1].hi + pts[i].lo + pts[i].hi) / 4f, y0, y1, p);
                if (i > 1)
                {
                    float px = MapX((pts[i - 2].x + pts[i - 1].x) / 2f, x0, x1, p);
                    Draw.LineH(px, my, mx - px, Tokens.Accent);
                }
            }
        }

        // ---- K 线 / 蜡烛图 ----
        public static void Ohlc(Rect rect, OhlcBar[] bars)
        {
            if (bars == null || bars.Length == 0) return;
            float lo = float.MaxValue, hi = float.MinValue;
            for (int i = 0; i < bars.Length; i++) { lo = Mathf.Min(lo, bars[i].low); hi = Mathf.Max(hi, bars[i].high); }
            if (hi <= lo) hi = lo + 1f;
            var p = Plot(rect);
            float slot = p.width / bars.Length;
            for (int i = 0; i < bars.Length; i++)
            {
                var b = bars[i];
                float cx = p.x + slot * (i + 0.5f);
                bool up = b.close >= b.open;
                Color col = up ? Tokens.Alive : Tokens.Dead;
                // 原 Y() 本轮被调 4 次（每次两道除法）⇒ high/low 落局部变量复用
                float yHigh = MapY(b.high, lo, hi, p), yLow = MapY(b.low, lo, hi, p);
                Draw.LineV(cx, yHigh, yLow - yHigh, col);
                Draw.LineH(cx - slot * 0.18f, MapY(b.open, lo, hi, p), slot * 0.36f, col);
                Draw.LineH(cx - slot * 0.18f, MapY(b.close, lo, hi, p), slot * 0.36f, col);
            }
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }

        // ---- SPC 控制图 ----
        public static void Spc(Rect rect, float[] samples, float mean, float ucl, float lcl)
        {
            if (samples == null || samples.Length == 0) return;
            float lo = Mathf.Min(lcl, ChartCandFmt.Min(samples));
            float hi = Mathf.Max(ucl, ChartCandFmt.Max(samples));
            if (hi <= lo) hi = lo + 1f;
            var p = Plot(rect);
            Draw.LineH(p.x, MapY(mean, lo, hi, p), p.width, Tokens.Muted);
            Draw.LineH(p.x, MapY(ucl, lo, hi, p), p.width, Tokens.Warn);
            Draw.LineH(p.x, MapY(lcl, lo, hi, p), p.width, Tokens.Warn);
            float step = p.width / samples.Length;
            for (int i = 0; i < samples.Length; i++)
            {
                float x = p.x + step * (i + 0.5f);
                float ys = MapY(samples[i], lo, hi, p);
                bool outOf = samples[i] > ucl || samples[i] < lcl;
                Draw.Box(new Rect(x - 2f, ys - 2f, 4f, 4f), outOf ? Tokens.Dead : Tokens.Accent);
                if (i > 0) Draw.LineH(p.x + step * (i - 0.5f), MapY(samples[i - 1], lo, hi, p), step, Tokens.Accent);
            }
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }

        // ---- Pareto 排列图 ----
        public static void Pareto(Rect rect, float[] values, string[] labels)
        {
            if (values == null || values.Length == 0) return;
            int n = values.Length;
            int[] idx = new int[n]; for (int i = 0; i < n; i++) idx[i] = i;
            for (int a = 0; a < n; a++) for (int b = a + 1; b < n; b++) if (values[idx[b]] > values[idx[a]]) { int t = idx[a]; idx[a] = idx[b]; idx[b] = t; }
            float mx = ChartCandFmt.Max(values); if (mx <= 0f) mx = 1f;
            float[] cum = new float[n]; float acc = 0f; for (int i = 0; i < n; i++) { acc += values[idx[i]]; cum[i] = acc; }
            float sum = acc; if (sum <= 0f) sum = 1f;
            var p = Plot(rect);
            float slot = p.width / n;
            float labelH = Measure.RowHeight(Tokens.FontLabel);
            for (int i = 0; i < n; i++)
            {
                float h = values[idx[i]] / mx * p.height;
                Rect bar = new Rect(p.x + i * slot + 2f, p.yMax - h, slot - 4f, h);
                Draw.Box(bar, Tokens.Accent);
                // 轴标签高度取引擎行高（原写死 14f ⇒ 装不下 22px 文字，向上溢进绘图区）
                if (labels != null && idx[i] < labels.Length)
                    Draw.Label(new Rect(p.x + i * slot, p.yMax + 2f, slot, labelH), labels[idx[i]] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
            }
            for (int i = 1; i < n; i++)
            {
                float y0 = p.yMax - cum[i - 1] / sum * p.height;
                float y1 = p.yMax - cum[i] / sum * p.height;
                Draw.LineH(p.x + slot * (i - 0.5f), y0, slot, Tokens.Dead);
                Draw.LineV(p.x + slot * (i + 0.5f), y1, y0 - y1, Tokens.Dead);
            }
            if (n > 0) Draw.LineV(p.x + slot * 0.5f, p.yMax - cum[0] / sum * p.height, 0f, Tokens.Dead);
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }
    }
}
