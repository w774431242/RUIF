using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>⑥ 反馈候选组件（原 📋，官方实证支持，现落地）：浮动菜单 / 警报中心·通知托盘。无状态；
    /// 数据由调用方传入。官方依据：<c>FloatMenu</c>/<c>FloatMenuOption</c>/<c>FloatMenuGrid</c>（菜单浮层）、
    /// <c>AlertsReadout</c>/<c>Messages</c>/<c>LetterStack</c>（警报/通知，仅数据形态）
    /// （见《原版UI代码标准（Spy取证）》）。</summary>
    public static class FeedbackCandAtoms
    {
        // ---- 浮动菜单 ----

        /// <summary>浮动菜单：选项列表 + 悬停高亮 + 点击返回命中索引（命中无则返回 -1）。</summary>
        public static int FloatMenu(Rect rect, string[] labels, ref int hover)
        {
            Draw.Box(rect, Tokens.Panel);
            Draw.Border(rect, Tokens.BorderSoft);
            int clicked = -1;
            hover = -1;
            if (labels == null) return clicked;
            var col = Layout.Column(Layout.Inset(rect, 2f, 2f), 2f);
            float rowH = Mathf.Min(24f, (rect.height - 4f) / Mathf.Max(1, labels.Length));
            for (int i = 0; i < labels.Length; i++)
            {
                Rect row = col.Take(rowH);
                bool over = Verse.Mouse.IsOver(row);
                if (over) hover = i;
                Draw.Box(row, FeedbackCandFmt.FloatMenuRowTone(over));
                Draw.Label(new Rect(row.x + 8f, row.y, row.width - 12f, row.height),
                    labels[i] ?? string.Empty, Tokens.FontLabel, over ? Tokens.Text : Tokens.Muted, TextAnchor.MiddleLeft);
                if (Widgets.ButtonInvisible(row)) clicked = i;
            }
            return clicked;
        }

        // ---- 警报中心 / 通知托盘 ----

        /// <summary>警报条目。</summary>
        public struct AlertEntry
        {
            /// <summary>警报标签。</summary>
            public string label;
            /// <summary>严重度（0 提示 / 1 警告 / 2 危险）。</summary>
            public int severity;
        }

        /// <summary>警报中心/通知托盘：左色条 + 标签 + 严重度标记的警报卡列表。</summary>
        public static void AlertTray(Rect rect, AlertEntry[] alerts)
        {
            if (alerts == null || alerts.Length == 0) return;
            var col = Layout.Column(rect, 3f);
            float rowH = rect.height / alerts.Length;
            for (int i = 0; i < alerts.Length; i++)
            {
                Rect row = col.Take(rowH - 3f);
                Color tone = FeedbackCandFmt.AlertSeverityTone(alerts[i].severity);
                Draw.Box(row, Tokens.Card);
                Draw.Box(new Rect(row.x, row.y, 4f, row.height), tone);
                Draw.Border(row, Tokens.BorderSoft);
                var rr = Layout.Row(Layout.Inset(row, 10f, 4f), 6f);
                Draw.Label(rr.Take(rr.Rest.width - 56f), alerts[i].label ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                string tag = alerts[i].severity >= 2 ? "危险" : alerts[i].severity == 1 ? "警告" : "提示";
                Draw.Label(rr.Rest, tag, Tokens.FontLabel, tone, TextAnchor.MiddleRight);
            }
        }
    }
}
