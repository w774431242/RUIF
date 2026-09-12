using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>C2+ 官方数据系统组件（S3 任务④ · 5-9 件）。数据由调用方注入，组件内不读游戏状态。</summary>
    public static class OfficialDataAtomsB
    {
        /// <summary>⑤ 信件归档卡（官方 LetterStack 语义：类型色条 + 标题 + 摘要 + 时间）。</summary>
        public static void LetterCard(Rect rect, string title, string summary, string time, Color tone)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.LineV(rect.x, rect.y, rect.height, tone);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 10f, 5f), 3f);
            var head = Layout.Row(col.Take(18f), 6f);
            Draw.Label(head.Take(Mathf.Max(10f, rect.width - 90f)), title ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(head.Rest, time ?? string.Empty, Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleRight);
            Draw.Label(col.Rest, summary ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
        }

        /// <summary>⑥ 账单队列（官方 BillStack 语义：配方 + 完成/目标 + 暂停态）。</summary>
        public static void BillList(Rect rect, string[] names, float[] done, float[] target, bool[] paused)
        {
            int n = Mathf.Min(Mathf.Min(names == null ? 0 : names.Length, done == null ? 0 : done.Length), Mathf.Min(target == null ? 0 : target.Length, paused == null ? 0 : paused.Length));
            if (n == 0) return;
            var col = Layout.Column(rect, 2f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                var row = Layout.Row(col.Take(rowH - 2f), 6f);
                float ratio = OfficialFmt.BillRatio(done[i], target[i]);
                Color tc = paused[i] ? Tokens.Dim : (ratio >= 1f ? Tokens.Alive : Tokens.Text);
                Draw.Label(row.Take(rect.width * 0.5f), names[i] ?? string.Empty, Tokens.FontLabel, tc, TextAnchor.MiddleLeft);
                Draw.Label(row.Take(90f), done[i].ToString("0") + "/" + target[i].ToString("0"), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
                string st = paused[i] ? Tr.Get("RUIF.Comp.Bill.Paused") : (ratio >= 1f ? Tr.Get("RUIF.Comp.Bill.Done") : Tr.Get("RUIF.Comp.Bill.InProgress"));
                Draw.Label(row.Rest, st, Tokens.FontLabel, paused[i] ? Tokens.Dim : tc, TextAnchor.MiddleRight);
            }
        }

        /// <summary>⑦ 任务板（官方 QuestScriptDef 语义：缩略卡 + 进度条 + 剩余时间）。</summary>
        public static void QuestBoard(Rect rect, string[] titles, float[] progress, float[] daysLeft)
        {
            int n = Mathf.Min(Mathf.Min(titles == null ? 0 : titles.Length, progress == null ? 0 : progress.Length), daysLeft == null ? 0 : daysLeft.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 3f);
            float cardH = (rect.height - (n - 1) * 4f) / n;
            for (int i = 0; i < n; i++)
            {
                Rect card = col.Take(cardH);
                Draw.Box(card, Tokens.Card);
                Draw.Border(card, Tokens.BorderSoft);
                var row = Layout.Row(card, 6f);
                Draw.Label(row.Take(Mathf.Max(10f, card.width - 120f)), titles[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Rect bar = row.Take(80f);
                Draw.Box(bar, Tokens.BorderSoft);
                Draw.Box(new Rect(bar.x, bar.y, bar.width * Mathf.Clamp01(progress[i]), bar.height), Tokens.Accent);
                Draw.Label(row.Rest, OfficialFmt.EtaLabel(daysLeft[i]), Tokens.FontLabel, Tokens.Warn, TextAnchor.MiddleRight);
            }
        }

        /// <summary>⑧ 头衔卡（官方 RoyalTitle 语义：名称 + 授予时间 + 许可勾叉）。</summary>
        public static void TitleCard(Rect rect, string name, string granted, string[] permits, bool[] ok)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 8f, 5f), 3f);
            Draw.Label(col.Take(18f), "👑 " + (name ?? string.Empty), Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleLeft);
            if (granted != null) Draw.Label(col.Take(14f), Tr.Get("RUIF.Comp.Title.Granted", granted), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            int m = Mathf.Min(permits == null ? 0 : permits.Length, ok == null ? 0 : ok.Length);
            for (int i = 0; i < m; i++)
            {
                Color c = OfficialFmt.CheckColor(ok[i]);
                Draw.Label(col.Take(16f), (ok[i] ? "✓ " : "✕ ") + (permits[i] ?? string.Empty), Tokens.FontLabel, c, TextAnchor.MiddleLeft);
            }
        }

        /// <summary>⑨ 心能树（官方 Psycast/Ability 语义：分层节点，已习得实心 / 锁定暗）。</summary>
        public static void PsycastTree(Rect rect, string[] names, int[] levels, bool[] unlocked)
        {
            int n = Mathf.Min(Mathf.Min(names == null ? 0 : names.Length, levels == null ? 0 : levels.Length), unlocked == null ? 0 : unlocked.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 3f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                var row = Layout.Row(col.Take(rowH - 3f), 6f);
                float dot = Mathf.Min(18f, rowH - 6f);
                Rect dotRect = new Rect(rect.x, rect.y + i * rowH + (rowH - dot) / 2f, dot, dot);
                if (unlocked[i])
                {
                    Draw.Box(dotRect, Tokens.Accent);
                    Draw.Label(dotRect, levels[i].ToString(), Tokens.FontLabel, Tokens.Window, TextAnchor.MiddleCenter);
                }
                else
                {
                    Draw.Box(dotRect, Tokens.PanelRaised);
                    Draw.Border(dotRect, Tokens.BorderSoft);
                }
                Rect label = row.Take(dot + 8f);
                Draw.Label(label, string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                Draw.Label(row.Rest, names[i] ?? string.Empty, Tokens.FontLabel, unlocked[i] ? Tokens.Text : Tokens.Dim, TextAnchor.MiddleLeft);
            }
        }
    }
}
