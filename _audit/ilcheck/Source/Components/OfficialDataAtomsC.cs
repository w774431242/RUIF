using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>C2+ 官方数据系统组件（S3 任务④ · 10-14 件）。数据由调用方注入，组件内不读游戏状态。</summary>
    public static class OfficialDataAtomsC
    {
        /// <summary>⑩ 文化卡（官方 Ideo/Precept 语义：名称 + 信条清单 + 领袖 + 成员数）。</summary>
        public static void IdeoCard(Rect rect, string name, string leader, int members, string[] precepts)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 8f, 5f), 3f);
            var head = Layout.Row(col.Take(18f), 6f);
            Draw.Label(head.Take(Mathf.Max(10f, rect.width - 90f)), name ?? string.Empty, Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(head.Rest, Tr.Get("RUIF.Comp.Ideo.Members", members.ToString("0")), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleRight);
            if (precepts != null)
            {
                for (int i = 0; i < precepts.Length; i++)
                {
                    Draw.Label(col.Take(16f), "· " + (precepts[i] ?? string.Empty), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                }
            }
            if (!string.IsNullOrEmpty(leader))
            {
                Draw.Label(col.Rest, Tr.Get("RUIF.Comp.Ideo.Leader", leader), Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleLeft);
            }
        }

        /// <summary>⑪ 商队面板（官方 Caravan 语义：成员 + 负重 + 口粮 + ETA）。</summary>
        public static void CaravanPanel(Rect rect, string[] members, float load01, float foodDays, string eta)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 8f, 6f), 4f);
            var av = Layout.Row(col.Take(24f), 4f);
            for (int i = 0; i < members.Length; i++)
            {
                string initial = string.IsNullOrEmpty(members[i]) ? "?" : members[i].Substring(0, 1);
                Draw.Box(av.Take(22f), Tokens.Accent);
                Draw.Label(av.Rest, string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                Draw.Label(av.Take(22f), initial, Tokens.FontLabel, Tokens.Window, TextAnchor.MiddleCenter);
            }
            var load = Layout.Row(col.Take(16f), 6f);
            Draw.Label(load.Take(56f), Tr.Get("RUIF.Comp.Caravan.Load"), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.Box(load.Rest, Tokens.BorderSoft);
            Draw.Box(new Rect(load.Rest.x, load.Rest.y, load.Rest.width * Mathf.Clamp01(load01), load.Rest.height), load01 > 0.85f ? Tokens.Warn : Tokens.Alive);
            Draw.Label(col.Rest, Tr.Get("RUIF.Comp.Caravan.Food", foodDays.ToString("0.#"), eta ?? string.Empty), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
        }

        /// <summary>⑫ 基因芯片（官方 GeneDef 语义：类别色条 + 复杂度点数）。</summary>
        public static void GeneChips(Rect rect, string[] genes, int[] complexity, bool[] positive, float gap = 6f)
        {
            int n = Mathf.Min(genes == null ? 0 : genes.Length, complexity == null ? 0 : complexity.Length);
            if (n == 0) return;
            var row = Layout.Row(rect, gap);
            for (int i = 0; i < n; i++)
            {
                float w = 0f;
                using (Draw.Font(Tokens.FontLabel)) w = Text.CalcSize(genes[i] ?? string.Empty).x + 34f;
                Rect chip = row.Take(w);
                Color c = positive[i] ? Tokens.Alive : Tokens.Dead;
                Draw.Box(chip, Draw.WithAlpha(c, 0.14f));
                Draw.Border(chip, c);
                Draw.Box(new Rect(chip.x, chip.y, 3f, chip.height), c);
                string s = (genes[i] ?? string.Empty) + " " + OfficialFmt.ComplexityLabel(complexity[i]);
                Draw.Label(new Rect(chip.x + 8f, chip.y, chip.width - 10f, chip.height), s, Tokens.FontLabel, c, TextAnchor.MiddleLeft);
            }
        }

        /// <summary>⑬ 瞬息消息流（官方 Messages 语义：不打断、按时间排序）。</summary>
        public static void MessageFeed(Rect rect, string[] lines, string[] times)
        {
            int n = Mathf.Min(lines == null ? 0 : lines.Length, times == null ? 0 : times.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 2f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                var row = Layout.Row(col.Take(rowH - 2f), 6f);
                Draw.Label(row.Take(Mathf.Max(10f, rect.width - 64f)), lines[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Draw.Label(row.Rest, times[i] ?? string.Empty, Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleRight);
            }
        }

        /// <summary>⑭ 政策卡（官方 Policies 语义：名称 + 摘要 + 应用人数）。</summary>
        public static void PolicyCard(Rect rect, string name, string summary, int applied)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 8f, 5f), 3f);
            Draw.Label(col.Take(18f), name ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(col.Take(14f), summary ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.Label(col.Rest, Tr.Get("RUIF.Comp.Policy.AppliedTo", applied.ToString("0")), Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleRight);
        }
    }
}
