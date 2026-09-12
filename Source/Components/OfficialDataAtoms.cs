using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>C2+ 官方数据系统组件（S3 任务④ · 1-4 件）。数据由调用方注入，组件内不读游戏状态。</summary>
    public static class OfficialDataAtoms
    {
        /// <summary>① 财富构成（官方 WealthWatcher 语义：分段堆叠 + 总额 + 图例）。</summary>
        public static void WealthBreakdown(Rect rect, string[] labels, float[] values, Color[] tones)
        {
            int n = Mathf.Min(labels == null ? 0 : labels.Length, Mathf.Min(values == null ? 0 : values.Length, tones == null ? 0 : tones.Length));
            if (n == 0) return;
            float total = OfficialFmt.WealthTotal(values);
            Draw.Label(new Rect(rect.x, rect.y, rect.width, 18f), Tr.Get("RUIF.Comp.Wealth.Total", TextFmt.FormatNumber(total)), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            Rect bar = new Rect(rect.x, rect.y + 20f, rect.width, 14f);
            Draw.Box(bar, Tokens.BorderSoft);
            float x = bar.x;
            for (int i = 0; i < n; i++)
            {
                float w = bar.width * DataExFmt.Share(values[i], total);
                Draw.Box(new Rect(x, bar.y, w, bar.height), tones[i]);
                x += w;
            }
            var leg = Layout.Row(new Rect(rect.x, rect.y + 38f, rect.width, Mathf.Max(0f, rect.height - 40f)), 8f);
            for (int i = 0; i < n; i++)
            {
                Rect item = leg.Take(rect.width / n - 8f);
                Draw.Box(new Rect(item.x, item.y + 4f, 8f, 8f), tones[i]);
                string s = (labels[i] ?? string.Empty) + " " + TextFmt.FormatNumber(values[i]);
                Draw.Label(new Rect(item.x + 12f, item.y, item.width - 12f, 14f), s, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            }
        }

        /// <summary>② 记录表（官方 Pawn_RecordsTracker 语义：键值两列表）。</summary>
        public static void RecordsTable(Rect rect, string[] labels, string[] values)
        {
            int n = Mathf.Min(labels == null ? 0 : labels.Length, values == null ? 0 : values.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 2f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                var row = Layout.Row(col.Take(rowH - 2f), 6f);
                Draw.Label(row.Take(rect.width * 0.55f), labels[i] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                Draw.Label(row.Rest, values[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
                if (i < n - 1) Draw.LineH(rect.x, rect.y + (i + 1) * rowH - 1f, rect.width, Tokens.BorderHair);
            }
        }

        /// <summary>③ 历史事件流（日期头 + 事件行 + 类型 pill）。</summary>
        public static void HistoryFeed(Rect rect, string[] dates, string[] texts, string[] kinds)
        {
            int n = Mathf.Min(Mathf.Min(dates == null ? 0 : dates.Length, texts == null ? 0 : texts.Length), kinds == null ? 0 : kinds.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 3f);
            for (int i = 0; i < n; i++)
            {
                Rect row = col.Take(18f);
                var r = Layout.Row(row, 6f);
                Draw.Label(r.Take(120f), dates[i] ?? string.Empty, Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
                Draw.Label(r.Take(Mathf.Max(10f, row.width - 200f)), texts[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Rect pill = r.Take(64f);
                Draw.Box(pill, Tokens.AccentSoft);
                Draw.Border(pill, Tokens.Accent);
                Draw.Label(pill, kinds[i] ?? string.Empty, Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleCenter);
            }
        }

        /// <summary>④ 传说卡（官方 Tale 系统：讲述者 + 叙述 + 流传次数）。</summary>
        public static void TaleCard(Rect rect, string teller, string text, int times)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 8f, 6f), 3f);
            var head = Layout.Row(col.Take(20f), 6f);
            Draw.Label(head.Take(Mathf.Max(10f, rect.width - 90f)), teller ?? string.Empty, Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleLeft);
            Draw.Label(head.Rest, Tr.Get("RUIF.Comp.Tale.Spread", times.ToString("0")), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleRight);
            Draw.Label(col.Rest, text ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.UpperLeft);
        }
    }
}
