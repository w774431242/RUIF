using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>⑩ 工业域绑定候选组件（原 📋，官方实证支持，现落地）：DataBridge 数据桥 / GridMap 俯视状态格网 /
    /// CircuitTopology 电路拓扑 / ProductionFlow 生产链看板。皆为纯 presentational 视觉骨架（数字孪生方向）；
    /// 接真实域（PowerDomain/StockDomain/MapDomain/BillStack）属集成件，留作游戏实机验收。
    /// 官方依据：<c>PowerNet</c> · <c>Map</c>/<c>Zone</c>/<c>TerrainDef</c> · <c>BillStack</c>（见《工业管理方向扩展设计提案》）。</summary>
    public static class DomainCandAtoms
    {
        public struct BridgeSnapshot { public string domain; public string stamp; public float[] series; }
        public struct CircuitNode { public string label; public int kind; } // 0 发电 / 1 储能 / 2 负载
        public struct ProductionStage { public string label; public float progress; } // 0~1

        // ---- DataBridge 数据桥 ----
        /// <summary>数据桥面板：各域低频采样快照卡（域 + 时间戳 + 迷你趋势）。</summary>
        public static void DataBridgePanel(Rect rect, BridgeSnapshot[] snaps)
        {
            Draw.Box(rect, Tokens.Panel);
            Draw.Border(rect, Tokens.BorderSoft);
            Draw.Label(new Rect(rect.x + 4f, rect.y + 2f, rect.width - 8f, 16f), "DataBridge · 低频采样快照",
                Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleLeft);
            if (snaps == null) return;
            var col = Layout.Column(Layout.Inset(rect, 4f, 22f), 4f);
            float h = col.Rest.height / Mathf.Max(1, snaps.Length);
            for (int i = 0; i < snaps.Length; i++)
            {
                Rect card = col.Take(h);
                Draw.Box(card, Tokens.Card);
                Draw.Label(new Rect(card.x + 4f, card.y + 2f, card.width - 8f, 14f),
                    (snaps[i].domain ?? "?") + "  " + (snaps[i].stamp ?? ""), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                if (snaps[i].series != null && snaps[i].series.Length > 1)
                {
                    Rect plot = new Rect(card.x + 4f, card.y + 18f, card.width - 8f, card.height - 20f);
                    float lo = ChartCandFmt.Min(snaps[i].series), hi = ChartCandFmt.Max(snaps[i].series);
                    if (hi <= lo) hi = lo + 1f;
                    for (int k = 1; k < snaps[i].series.Length; k++)
                    {
                        float ax = plot.x + (k - 1f) / (snaps[i].series.Length - 1) * plot.width;
                        float bx = plot.x + k / (snaps[i].series.Length - 1) * plot.width;
                        float ay = plot.yMax - (snaps[i].series[k - 1] - lo) / (hi - lo) * plot.height;
                        float by = plot.yMax - (snaps[i].series[k] - lo) / (hi - lo) * plot.height;
                        Draw.LineH(ax, ay, bx - ax, Tokens.Info);
                        Draw.LineV(bx, ay, by - ay, Tokens.Info);
                    }
                }
            }
        }

        // ---- GridMap 俯视状态格网 ----
        /// <summary>俯视状态格网：int[,] 单元→阈值配色（0=空）。</summary>
        public static void GridMap(Rect rect, int[,] grid)
        {
            if (grid == null) return;
            int rows = grid.GetLength(0), cols = grid.GetLength(1);
            if (rows == 0 || cols == 0) return;
            int mx = 1;
            for (int r = 0; r < rows; r++) for (int c = 0; c < cols; c++) mx = Mathf.Max(mx, grid[r, c]);
            Draw.Box(rect, Tokens.Panel);
            float cw = rect.width / cols, ch = rect.height / rows;
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                {
                    Rect cell = new Rect(rect.x + c * cw, rect.y + r * ch, cw - 1f, ch - 1f);
                    int v = grid[r, c];
                    if (v <= 0) Draw.Box(cell, Tokens.Card);
                    else Draw.Box(cell, DomainCandFmt.CellTone(v / (float)mx));
                }
        }

        // ---- CircuitTopology 电路拓扑 ----
        /// <summary>电路拓扑：节点按 kind 分三列（发电/储能/负载），连线用官方 DrawLine。</summary>
        public static void CircuitGraph(Rect rect, CircuitNode[] nodes, int[] from, int[] to)
        {
            if (nodes == null) return;
            Draw.Box(rect, Tokens.Panel);
            Draw.Border(rect, Tokens.BorderSoft);
            float[] colX = { rect.x + rect.width * 0.12f, rect.x + rect.width * 0.5f, rect.x + rect.width * 0.88f };
            int[] cnt = new int[3]; int[] seen = new int[nodes.Length];
            for (int i = 0; i < nodes.Length; i++) if (nodes[i].kind >= 0 && nodes[i].kind < 3) cnt[nodes[i].kind]++;
            int[] cur = new int[3];
            Vector2[] pos = new Vector2[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                int k = Mathf.Clamp(nodes[i].kind, 0, 2);
                float y = rect.y + (cnt[k] <= 1 ? rect.height / 2f : rect.height * (cur[k] + 0.5f) / cnt[k]);
                pos[i] = new Vector2(colX[k], y);
                cur[k]++;
            }
            if (from != null && to != null)
                for (int e = 0; e < from.Length && e < to.Length; e++)
                {
                    if (from[e] < 0 || from[e] >= nodes.Length || to[e] < 0 || to[e] >= nodes.Length) continue;
                    Widgets.DrawLine(pos[from[e]], pos[to[e]], DomainCandFmt.NodeKindColor(nodes[from[e]].kind), 1.5f);
                }
            for (int i = 0; i < nodes.Length; i++)
            {
                Rect box = new Rect(pos[i].x - 26f, pos[i].y - 12f, 52f, 24f);
                Draw.Box(box, DomainCandFmt.NodeKindColor(nodes[i].kind));
                Draw.Label(box, nodes[i].label ?? string.Empty, Tokens.FontLabel, Tokens.Window, TextAnchor.MiddleCenter);
            }
        }

        // ---- ProductionFlow 生产链看板 ----
        /// <summary>生产链看板：阶段盒左→右流水，箭头连接，进度填充。</summary>
        public static void ProductionFlow(Rect rect, ProductionStage[] stages)
        {
            if (stages == null || stages.Length == 0) return;
            Draw.Box(rect, Tokens.Panel);
            Draw.Border(rect, Tokens.BorderSoft);
            float gap = 18f;
            float w = (rect.width - gap * (stages.Length - 1)) / stages.Length;
            for (int i = 0; i < stages.Length; i++)
            {
                float x = rect.x + i * (w + gap);
                Rect box = new Rect(x, rect.y + 10f, w, rect.height - 20f);
                Draw.Box(box, Tokens.Card);
                Draw.Box(new Rect(box.x, box.y, box.width, box.height * Mathf.Clamp01(stages[i].progress)),
                    Draw.WithAlpha(DomainCandFmt.ProgressTone(stages[i].progress), 0.35f));
                Draw.Border(box, Tokens.BorderSoft);
                Draw.Label(box, stages[i].label ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
                if (i < stages.Length - 1)
                {
                    Vector2 a = new Vector2(x + w, box.center.y), b = new Vector2(x + w + gap, box.center.y);
                    Widgets.DrawLine(a, b, Tokens.Muted, 1.5f);
                    Draw.LineV(b.x, b.y - 4f, 8f, Tokens.Muted);
                }
            }
        }
    }
}
