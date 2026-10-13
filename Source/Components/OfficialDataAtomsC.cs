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
                // ⚠ 修复（2026-10-05 实机取证）：原实现是
                //      Draw.Box(av.Take(22f), Accent);              → 底色块 [x, x+22)
                //      Draw.Label(av.Rest, string.Empty, …);         → 空标签（游标不推进，纯空转）
                //      Draw.Label(av.Take(22f), initial, …);         → 首字落到 [x+26, x+48)
                //   第二次 Take 落在底色块**右侧 4px 的空地**上 ⇒ 蓝方块里没有字、
                //   字被画到方块旁边的空白处（截图里三个「纯蓝方块」即此）。
                //   头像块必须**一次取值、底色与首字共用同一 rect**。
                Rect chip = av.Take(22f);
                Draw.Box(chip, Tokens.Accent);
                Draw.Label(chip, initial, Tokens.FontLabel, Tokens.Window, TextAnchor.MiddleCenter);
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
                // 文字用 Tokens.Text（近白）而不是语义色 c：底色是 c 的 14% 叠印，
                // 同色系叠同色系时负向芯片（Dead 红底 + 红字）在实机上几乎读不出来。
                // 语义仍由**左边条 + 边框 + 底色**三处承载，不依赖字色。
                Draw.Label(new Rect(chip.x + 8f, chip.y, chip.width - 10f, chip.height), s, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
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
