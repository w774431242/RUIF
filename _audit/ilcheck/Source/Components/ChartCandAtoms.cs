using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>⑧ 图表候选组件（原 📋，官方实证支持，现落地）：直方图 / 雷达 / 甘特 / 桑基 / 箱线 /
    /// 漏斗 / 瀑布 / 气泡 / 阶梯线 / 区间带 / K线 / SPC 控制图 / Pareto 排列图。纯 presentational，数据由调用方传入；
    /// 任意方向连线用官方 <c>Widgets.DrawLine</c>，轴/基线用 <c>Draw.LineH/LineV</c>。
    /// 基础 10 件（Sparkline/Bar/Stacked/Line/Area/Donut/Pie/Scatter/HeatmapGrid）已在 <c>Charts</c> 落地，不重复。</summary>
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
            System.Func<int, float> ang = i => -Mathf.PI / 2f + i * (2f * Mathf.PI / n);
            System.Func<float, int, Vector2> at = (f, i) => c + new Vector2(Mathf.Cos(ang(i)), Mathf.Sin(ang(i))) * (rad * f);
            for (int ring = 1; ring <= 4; ring++)
            {
                float f = ring / 4f;
                for (int i = 0; i < n; i++)
                {
                    Vector2 a = at(f, i), b = at(f, (i + 1) % n);
                    Widgets.DrawLine(a, b, Tokens.BorderSoft, 1f);
                }
            }
            for (int i = 0; i < n; i++)
            {
                Vector2 tip = at(1f, i);
                Widgets.DrawLine(c, tip, Tokens.BorderSoft, 1f);
                Draw.Label(new Rect(tip.x - 30f, tip.y - 8f, 60f, 16f), axes[i] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
            }
            if (series != null)
                for (int s = 0; s < series.Length; s++)
                {
                    var ser = series[s]; if (ser == null || ser.Length < n) continue;
                    Color col = ChartCandFmt.SeriesColor(s);
                    for (int i = 0; i < n; i++)
                    {
                        Vector2 a = at(Mathf.Clamp01(ser[i]), i), b = at(Mathf.Clamp01(ser[(i + 1) % n]), (i + 1) % n);
                        Widgets.DrawLine(a, b, col, 1.5f);
                    }
                }
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
                float x0 = p.x + (b.start - t0) / (t1 - t0) * p.width;
                float x1 = p.x + (b.end - t0) / (t1 - t0) * p.width;
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
            System.Func<float, float> Y = v => p.yMax - (v - lo) / (hi - lo) * p.height;
            for (int i = 0; i < boxes.Length; i++)
            {
                var b = boxes[i];
                float cx = p.x + slot * (i + 0.5f);
                float bw = slot * 0.4f;
                Draw.LineV(cx, Y(b.min), Y(b.max) - Y(b.min), Tokens.Muted);
                Draw.Box(new Rect(cx - bw / 2f, Y(b.q3), bw, Y(b.q1) - Y(b.q3)), Tokens.Alive);
                Draw.Border(new Rect(cx - bw / 2f, Y(b.q3), bw, Y(b.q1) - Y(b.q3)), Tokens.BorderSoft);
                Draw.LineH(cx - bw / 2f, Y(b.median), bw, Tokens.Accent);
                Draw.Label(new Rect(cx - slot / 2f, p.yMax + 2f, slot, 14f), b.label ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
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
            System.Func<float, float> Y = v => p.yMax - (v - lo) / (hi - lo) * p.height;
            float prevTop = Y(0f);
            run = 0f;
            for (int i = 0; i < steps.Length; i++)
            {
                float baseV, topV;
                if (steps[i].isTotal) { baseV = 0f; topV = steps[i].delta; run = steps[i].delta; }
                else { baseV = run; run += steps[i].delta; topV = run; }
                float yB = Y(baseV), yT = Y(topV);
                Rect bar = new Rect(p.x + i * slot + 2f, Mathf.Min(yB, yT), slot - 4f, Mathf.Abs(yT - yB));
                Draw.Box(bar, steps[i].isTotal ? Tokens.Accent : (steps[i].delta >= 0 ? Tokens.Alive : Tokens.Dead));
                if (i > 0) Draw.LineH(p.x + (i - 1) * slot + 2f + slot - 4f, prevTop, 4f, Tokens.BorderSoft);
                prevTop = Y(topV);
            }
            Draw.LineV(p.x, p.y, p.height, Tokens.BorderSoft);
            Draw.LineH(p.x, p.yMax, p.width, Tokens.BorderSoft);
        }

        // ---- 气泡图 ----
        public static void Bubble(Rect rect, BubblePt[] pts)
        {
            if (pts == null || pts.Length == 0) return;
            float x0 = ChartCandFmt.Min(System.Array.ConvertAll(pts, p => p.x));
            float x1 = ChartCandFmt.Max(System.Array.ConvertAll(pts, p => p.x));
            float y0 = ChartCandFmt.Min(System.Array.ConvertAll(pts, p => p.y));
            float y1 = ChartCandFmt.Max(System.Array.ConvertAll(pts, p => p.y));
            if (x1 <= x0) x1 = x0 + 1f; if (y1 <= y0) y1 = y0 + 1f;
            var p = Plot(rect);
            for (int i = 0; i < pts.Length; i++)
            {
                var q = pts[i];
                float cx = p.x + (q.x - x0) / (x1 - x0) * p.width;
                float cy = p.yMax - (q.y - y0) / (y1 - y0) * p.height;
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
            System.Func<float, float> X = v => p.x + (v - x0) / (x1 - x0) * p.width;
            System.Func<float, float> Y = v => p.yMax - (v - y0) / (y1 - y0) * p.height;
            for (int i = 1; i < pts.Length; i++)
            {
                float ax = X(pts[i - 1].x), ay = Y(pts[i - 1].y);
                float bx = X(pts[i].x), by = Y(pts[i].y);
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
            float x0 = ChartCandFmt.Min(System.Array.ConvertAll(pts, q => q.x));
            float x1 = ChartCandFmt.Max(System.Array.ConvertAll(pts, q => q.x));
            float y0 = ChartCandFmt.Min(System.Array.ConvertAll(pts, q => q.lo));
            float y1 = ChartCandFmt.Max(System.Array.ConvertAll(pts, q => q.hi));
            if (x1 <= x0) x1 = x0 + 1f; if (y1 <= y0) y1 = y0 + 1f;
            var p = Plot(rect);
            System.Func<float, float> X = v => p.x + (v - x0) / (x1 - x0) * p.width;
            System.Func<float, float> Y = v => p.yMax - (v - y0) / (y1 - y0) * p.height;
            for (int i = 0; i < pts.Length; i++)
            {
                float x = X(pts[i].x);
                Draw.LineV(x, Y(pts[i].hi), Y(pts[i].lo) - Y(pts[i].hi), Draw.WithAlpha(Tokens.Info, 0.5f));
            }
            for (int i = 1; i < pts.Length; i++)
            {
                float mx = X((pts[i - 1].x + pts[i].x) / 2f);
                float my = Y((pts[i - 1].lo + pts[i - 1].hi + pts[i].lo + pts[i].hi) / 4f);
                if (i > 1) Draw.LineH(X((pts[i - 2].x + pts[i - 1].x) / 2f), my, mx - X((pts[i - 2].x + pts[i - 1].x) / 2f), Tokens.Accent);
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
            System.Func<float, float> Y = v => p.yMax - (v - lo) / (hi - lo) * p.height;
            for (int i = 0; i < bars.Length; i++)
            {
                var b = bars[i];
                float cx = p.x + slot * (i + 0.5f);
                bool up = b.close >= b.open;
                Color col = up ? Tokens.Alive : Tokens.Dead;
                Draw.LineV(cx, Y(b.high), Y(b.low) - Y(b.high), col);
                Draw.LineH(cx - slot * 0.18f, Y(b.open), slot * 0.36f, col);
                Draw.LineH(cx - slot * 0.18f, Y(b.close), slot * 0.36f, col);
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
            System.Func<float, float> Y = v => p.yMax - (v - lo) / (hi - lo) * p.height;
            Draw.LineH(p.x, Y(mean), p.width, Tokens.Muted);
            Draw.LineH(p.x, Y(ucl), p.width, Tokens.Warn);
            Draw.LineH(p.x, Y(lcl), p.width, Tokens.Warn);
            float step = p.width / samples.Length;
            for (int i = 0; i < samples.Length; i++)
            {
                float x = p.x + step * (i + 0.5f);
                bool outOf = samples[i] > ucl || samples[i] < lcl;
                Draw.Box(new Rect(x - 2f, Y(samples[i]) - 2f, 4f, 4f), outOf ? Tokens.Dead : Tokens.Accent);
                if (i > 0) Draw.LineH(p.x + step * (i - 0.5f), Y(samples[i - 1]), step, Tokens.Accent);
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
            for (int i = 0; i < n; i++)
            {
                float h = values[idx[i]] / mx * p.height;
                Rect bar = new Rect(p.x + i * slot + 2f, p.yMax - h, slot - 4f, h);
                Draw.Box(bar, Tokens.Accent);
                if (labels != null && idx[i] < labels.Length)
                    Draw.Label(new Rect(p.x + i * slot, p.yMax + 2f, slot, 14f), labels[idx[i]] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
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
