using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>⑤ 数据展示候选组件（原 📋，官方实证支持，现落地）：账单队列 / 物品过滤器树 /
    /// 资源读取面板 / 信息卡浮窗 / 存储内容面板 / 贸易面板。无状态；数据与选中态由调用方持有。
    /// 官方依据：<c>BillStack</c>·<c>Dialog_BillConfig</c> / <c>Listing_TreeThingFilter</c> /
    /// <c>Listing_ResourceReadout</c> / <c>Dialog_InfoCard</c> / <c>ITab_Contents</c> /
    /// <c>Dialog_Trade</c>（见《原版UI代码标准（Spy取证）》）。</summary>
    public static class DataCandAtoms
    {
        // ---- 账单队列 ----

        /// <summary>账单条目（生产队列组件的数据模型）。</summary>
        public struct BillEntry
        {
            /// <summary>配方/任务标签。</summary>
            public string label;
            /// <summary>已完成量。</summary>
            public int done;
            /// <summary>目标量（≤0 表示无限/不显示进度）。</summary>
            public int target;
            /// <summary>是否高优先。</summary>
            public bool priority;
            /// <summary>是否暂停。</summary>
            public bool paused;
        }

        /// <summary>账单队列：配方 + 目标/完成 + 优先/暂停标记 + 底进度条。</summary>
        public static void BillList(Rect rect, BillEntry[] bills)
        {
            if (bills == null || bills.Length == 0) return;
            var col = Layout.Column(rect, 3f);
            float rowH = rect.height / bills.Length;
            for (int i = 0; i < bills.Length; i++)
            {
                Rect row = col.Take(rowH - 3f);
                if (i % 2 == 1) Draw.Box(row, Draw.WithAlpha(Tokens.Text, 0.04f));
                var r = Layout.Row(row, 8f);
                Color baseT = bills[i].paused ? Tokens.Dim : Tokens.Text;
                Draw.Label(r.Take(row.width - 150f), bills[i].label ?? string.Empty, Tokens.FontLabel, baseT, TextAnchor.MiddleLeft);
                string prog = bills[i].target > 0 ? bills[i].done + "/" + bills[i].target : "—";
                Draw.Label(r.Take(66f), prog, Tokens.FontLabel, bills[i].paused ? Tokens.Dim : Tokens.Muted, TextAnchor.MiddleRight);
                if (bills[i].priority) Draw.Label(r.Take(34f), "▲优先", Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleRight);
                if (bills[i].paused) Draw.Label(r.Rest, "⏸ 暂停", Tokens.FontLabel, Tokens.Warn, TextAnchor.MiddleRight);

                Rect bar = new Rect(row.x, row.yMax - 2f, row.width, 2f);
                Draw.Box(bar, Tokens.BorderSoft);
                float share = DataCandFmt.BillRatio(bills[i].done, bills[i].target);
                Draw.Box(new Rect(bar.x, bar.y, bar.width * share, bar.height), bills[i].paused ? Tokens.Dim : Tokens.Alive);
            }
        }

        // ---- 物品过滤器树 ----

        /// <summary>过滤器树节点（ThingFilter 树选择的数据模型）。</summary>
        public struct FilterNode
        {
            /// <summary>显示标签。</summary>
            public string label;
            /// <summary>缩进层级（0 为根）。</summary>
            public int depth;
            /// <summary>是否有子节点（决定 caret）。</summary>
            public bool hasChildren;
        }

        /// <summary>物品过滤器树：逐节点 caret 展开 + 勾选框 + 标签；点击切换展开/勾选态。</summary>
        public static void ThingFilterTree(Rect rect, FilterNode[] nodes, bool[] expanded, bool[] checkStates)
        {
            if (nodes == null) return;
            float rowH = 20f;
            int visible = 0;
            for (int i = 0; i < nodes.Length; i++)
            {
                // 父未展开时隐藏其下级（按 depth 线性跳过）
                if (i > 0 && nodes[i].depth > 0)
                {
                    int p = i - 1;
                    while (p >= 0 && nodes[p].depth >= nodes[i].depth) p--;
                    if (p >= 0 && nodes[p].depth == nodes[i].depth - 1 && expanded != null && p < expanded.Length && !expanded[p])
                    {
                        continue; // 父折叠，跳过
                    }
                }
                Rect row = DataCandFmt.TreeRow(rect, visible++, rowH, nodes[i].depth);
                if (row.yMax > rect.yMax) break;
                if (visible % 2 == 1) Draw.Box(row, Draw.WithAlpha(Tokens.Text, 0.04f));

                float opX = row.x;
                if (nodes[i].hasChildren)
                {
                    Rect caret = new Rect(opX, row.y, DataCandFmt.TreeOpW, rowH);
                    bool open = expanded != null && i < expanded.Length && expanded[i];
                    Draw.Label(caret, open ? "▾" : "▸", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
                    if (Widgets.ButtonInvisible(caret) && expanded != null && i < expanded.Length) expanded[i] = !expanded[i];
                    opX += DataCandFmt.TreeOpW;
                }
                Rect box = new Rect(opX, row.y + 3f, 14f, 14f);
                bool on = checkStates != null && i < checkStates.Length && checkStates[i];
                Draw.Box(box, on ? Tokens.Accent : Tokens.PanelRaised);
                Draw.Border(box, on ? Tokens.Accent : Tokens.BorderSoft);
                if (on) Draw.Label(box, "✓", Tokens.FontLabel, Tokens.Window, TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(box) && checkStates != null && i < checkStates.Length) checkStates[i] = !checkStates[i];
                opX += 18f;

                Draw.Label(new Rect(opX, row.y, row.width - (opX - row.x), rowH),
                    nodes[i].label ?? string.Empty, Tokens.FontLabel, on ? Tokens.Text : Tokens.Muted, TextAnchor.MiddleLeft);
            }
        }

        // ---- 资源读取面板 ----

        /// <summary>资源读取面板：分类 + 计数（斑马纹，计数按规模取色）。</summary>
        public static void ResourceReadout(Rect rect, string[] categories, int[] counts)
        {
            int n = Mathf.Min(categories == null ? 0 : categories.Length, counts == null ? 0 : counts.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 2f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                Rect row = col.Take(rowH - 2f);
                if (i % 2 == 1) Draw.Box(row, Draw.WithAlpha(Tokens.Text, 0.04f));
                var r = Layout.Row(row, 8f);
                Draw.Label(r.Take(row.width - 70f), categories[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Draw.Label(r.Rest, counts[i].ToString(), Tokens.FontLabel, DataCandFmt.CountTone(counts[i]), TextAnchor.MiddleRight);
            }
        }

        // ---- 信息卡浮窗 ----

        /// <summary>信息卡浮窗：标题 + 副标题 + 明细行（实体详情卡）。</summary>
        public static void InfoCard(Rect rect, string title, string subtitle, string[] lines)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            Draw.Box(new Rect(rect.x, rect.y, 4f, rect.height), Tokens.Accent); // 左侧 accent 条
            var col = Layout.Column(Layout.Inset(rect, 12f, 8f), 4f);
            Draw.Label(col.Take(22f), title ?? string.Empty, Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
            if (!string.IsNullOrEmpty(subtitle))
                Draw.Label(col.Take(16f), subtitle, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            if (lines == null) return;
            for (int i = 0; i < lines.Length; i++)
                Draw.Label(col.Take(16f), "· " + (lines[i] ?? string.Empty), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
        }

        // ---- 存储内容面板 ----

        /// <summary>存储内容面板：物品标签 + 数量（ITab_Contents 形态）。</summary>
        public static void StorageContents(Rect rect, string[] labels, int[] counts)
        {
            int n = Mathf.Min(labels == null ? 0 : labels.Length, counts == null ? 0 : counts.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 2f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                Rect row = col.Take(rowH - 2f);
                if (i % 2 == 1) Draw.Box(row, Draw.WithAlpha(Tokens.Text, 0.04f));
                var r = Layout.Row(row, 8f);
                Draw.Label(r.Take(row.width - 70f), labels[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Draw.Label(r.Rest, counts[i].ToString(), Tokens.FontLabel, DataCandFmt.CountTone(counts[i]), TextAnchor.MiddleRight);
            }
        }

        // ---- 贸易面板 ----

        /// <summary>贸易行（左我方 / 右商队）。</summary>
        public struct TradeRow
        {
            /// <summary>物品标签。</summary>
            public string label;
            /// <summary>数量（正=我方持有 / 商队提供）。</summary>
            public int count;
            /// <summary>语义色（盈余绿 / 逆差红 / 常规）。</summary>
            public Color tone;
        }

        /// <summary>贸易面板：左我方 / 右商队双栏 + 表头。</summary>
        public static void TradePanel(Rect rect, TradeRow[] mine, TradeRow[] theirs)
        {
            Layout.Split(rect, 0.5f, Tokens.SpaceXs, out Rect left, out Rect right);
            Draw.Box(left, Tokens.Panel);
            Draw.Box(right, Tokens.Panel);
            Draw.Label(new Rect(left.x + 8f, left.y, left.width - 16f, 22f), "我方库存", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.Label(new Rect(right.x + 8f, right.y, right.width - 16f, 22f), "商队提供", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.LineV(rect.x + rect.width / 2f, rect.y, rect.height, Tokens.BorderSoft);

            var lc = Layout.Column(Layout.Inset(left, 8f, 26f), 2f);
            var rc = Layout.Column(Layout.Inset(right, 8f, 26f), 2f);
            float lh = mine == null ? 0 : left.height / Mathf.Max(1, mine.Length);
            float rh = theirs == null ? 0 : right.height / Mathf.Max(1, theirs.Length);
            if (mine != null)
                for (int i = 0; i < mine.Length; i++)
                {
                    Rect row = lc.Take(lh - 2f);
                    var r = Layout.Row(row, 8f);
                    Draw.Label(r.Take(row.width - 56f), mine[i].label ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                    Draw.Label(r.Rest, mine[i].count.ToString(), Tokens.FontLabel, mine[i].tone, TextAnchor.MiddleRight);
                }
            if (theirs != null)
                for (int i = 0; i < theirs.Length; i++)
                {
                    Rect row = rc.Take(rh - 2f);
                    var r = Layout.Row(row, 8f);
                    Draw.Label(r.Take(row.width - 56f), theirs[i].label ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                    Draw.Label(r.Rest, theirs[i].count.ToString(), Tokens.FontLabel, theirs[i].tone, TextAnchor.MiddleRight);
                }
        }
    }
}
