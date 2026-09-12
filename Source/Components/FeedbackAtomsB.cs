using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// S4 反馈原子（D2/D3 · 通知中心 / 撤销浮条 / 便签 / 分级闪烁 / S0 遮罩 / 步骤清单 / 行内错误 / 批量确认 / 操作回执）。
    /// 无状态；时间与选中态由调用方传入。MessageFeed 已在 OfficialDataAtomsC 落地。
    /// </summary>
    public static class FeedbackAtomsB
    {
        /// <summary>通知中心条目（未读高亮；点击返回跳转）。</summary>
        public static bool NotificationEntry(Rect rect, string text, string time, bool unread)
        {
            if (unread) Draw.Box(rect, Draw.WithAlpha(Tokens.Accent, 0.08f));
            var row = Layout.Row(rect, 6f);
            Draw.Label(row.Take(Mathf.Max(10f, rect.width - 56f)), text ?? string.Empty,
                Tokens.FontLabel, unread ? Tokens.Text : Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.Label(row.Rest, time ?? string.Empty, Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleRight);
            return Widgets.ButtonInvisible(rect);
        }

        /// <summary>
        /// 撤销浮条（Gmail 式）；返回是否点击撤销。<paramref name="alpha"/> 由调用方按 TTL 驱动（1=不透明）。
        ///
        /// 🔧 修复（2026-09-10 体检）：原实现用 <c>GUI.color</c> 做整体淡出，但**两条路径都不生效**——
        /// ① <c>Widgets.DrawBoxSolid</c> 内部是「存 → 设为传入色 → 画 → 还原」，会覆盖外层 tint（ilspycmd 实证）；
        /// ② <c>Draw.Label</c> 自带 Color 作用域，同样覆盖 tint。
        /// 因此淡出从未真正生效，只留下"裸写全局绘制状态、异常时不还原（染色泄漏到全游戏）"的风险。
        /// 现改为**直接对颜色乘 alpha**：淡出真正生效，且不再触碰全局状态。
        /// </summary>
        public static bool UndoToast(Rect rect, string action, float alpha)
        {
            float a = Mathf.Clamp01(alpha);
            Draw.Box(rect, Draw.WithAlpha(Tokens.Card, a));
            Draw.Border(rect, Draw.WithAlpha(Tokens.BorderSoft, a));
            var row = Layout.Row(rect, Tokens.SpaceXs);
            Draw.Label(row.Take(Mathf.Max(10f, rect.width - 70f)), Tr.Get("RUIF.Comp.Undo.Done", action ?? string.Empty),
                Tokens.FontLabel, Draw.WithAlpha(Tokens.Text, a), TextAnchor.MiddleLeft);
            return Widgets.ButtonText(row.Rest, Tr.Get("RUIF.Comp.Common.Undo"), true, false, true);
        }

        /// <summary>便签（低透明度便签纸；旋转由调用方用 Draw.Rotate 包裹）。</summary>
        public static void StickyNote(Rect rect, string text, Color tone)
        {
            Draw.Box(rect, tone);
            Draw.Label(Layout.Inset(rect, 5f, 4f), text ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.UpperLeft);
        }

        /// <summary>分级闪烁边框（紧急快闪红 / 常规慢闪黄 / 低常亮灰）。</summary>
        public static void AlertBlinker(Rect rect, float t, AlertPriority priority)
        {
            Color tone = priority switch { AlertPriority.Urgent => Tokens.Dead, AlertPriority.Warn => Tokens.Warn, _ => Tokens.Muted };
            Draw.Border(rect, Draw.WithAlpha(tone, FeedbackFmt.BlinkAlpha(t, priority)));
        }

        /// <summary>S0 严重遮罩：红罩 + 消息 + 单一动作按钮；返回动作是否点击。</summary>
        public static bool CriticalOverlay(Rect rect, string msg, string actionLabel)
        {
            Draw.Box(rect, Draw.WithAlpha(Tokens.Dead, 0.14f));
            Draw.Border(rect, Tokens.Dead);
            var col = Layout.Column(rect, 6f);
            Draw.Label(col.Take(18f), msg ?? string.Empty, Tokens.FontLabel, Tokens.Dead, TextAnchor.MiddleCenter);
            float bw = 140f;
            Rect btn = col.Take(24f);
            return Widgets.ButtonText(new Rect(rect.center.x - bw / 2f, btn.y, bw, 22f),
                actionLabel ?? Tr.Get("RUIF.Comp.Common.View"), true, false, true);
        }

        /// <summary>步骤清单（✓ 完成 / ● 进行 / ○ 待办）。</summary>
        public static void StepProgressList(Rect rect, string[] steps, int current)
        {
            if (steps == null) return;
            var col = Layout.Column(rect, 2f);
            float rowH = rect.height / steps.Length;
            for (int i = 0; i < steps.Length; i++)
            {
                string mark = FeedbackFmt.StepMark(i, current);
                Color c = i < current ? Tokens.Alive : (i == current ? Tokens.Accent : Tokens.Dim);
                var row = Layout.Row(col.Take(rowH), 6f);
                Draw.Label(row.Take(16f), mark, Tokens.FontLabel, c, TextAnchor.MiddleCenter);
                Draw.Label(row.Rest, steps[i] ?? string.Empty, Tokens.FontLabel,
                    i <= current ? Tokens.Text : Tokens.Dim, TextAnchor.MiddleLeft);
            }
        }

        /// <summary>行内校验错误（红字 + 可选修正链接）；返回修正链接是否点击。</summary>
        public static bool InlineError(Rect rect, string msg, string fixLabel = null)
        {
            var row = Layout.Row(rect, 6f);
            string s = "✗ " + (msg ?? string.Empty);
            float w = 0f;
            using (Draw.Font(Tokens.FontLabel)) w = Text.CalcSize(s).x;
            Draw.Label(row.Take(w + 4f), s, Tokens.FontLabel, Tokens.Dead, TextAnchor.MiddleLeft);
            if (string.IsNullOrEmpty(fixLabel)) return false;
            var link = new Rect(rect.x, rect.y, Mathf.Min(80f, row.Rest.width), rect.height);
            Draw.Label(link, fixLabel, Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleLeft);
            Draw.LineH(link.x, link.yMax - 1f, link.width, Tokens.Accent);
            return Widgets.ButtonInvisible(link);
        }

        /// <summary>批量确认摘要（对象类型分布 + 数量）；返回确认是否点击。</summary>
        public static bool BatchConfirm(Rect rect, string title, string summary,
            string cancelLabel = null, string okLabel = null)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 8f, 5f), 4f);
            Draw.Label(col.Take(18f), title ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(col.Take(14f), summary ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            var row = Layout.Row(col.Rest, Tokens.SpaceXs);
            if (Widgets.ButtonText(row.Take(70f), cancelLabel ?? Tr.Get("RUIF.Comp.Common.Cancel"))) return false;
            return Widgets.ButtonText(row.Rest, okLabel ?? Tr.Get("RUIF.Comp.Common.Execute"), true, false, true);
        }

        /// <summary>
        /// 操作回执：按钮内 1.2s 的 ✓/✗ 微反馈。<paramref name="alpha"/> 由调用方按 TTL 驱动（1=不透明）。
        ///
        /// 🔧 修复（2026-09-10 体检）：原实现**完全没用 alpha**（注释承诺淡出、实现固定 0.18f）→ 微反馈不会淡出。
        /// 现把 alpha 乘进底色/边框/字形颜色。
        /// </summary>
        public static void ActionFeedback(Rect rect, bool ok, float alpha)
        {
            float a = Mathf.Clamp01(alpha);
            Color c = ok ? Tokens.Alive : Tokens.Dead;
            Draw.Box(rect, Draw.WithAlpha(c, 0.18f * a));
            Draw.Border(rect, Draw.WithAlpha(c, a));
            Draw.Label(rect, ok ? "✓" : "✗", Tokens.FontBody, Draw.WithAlpha(c, a), TextAnchor.MiddleCenter);
        }
    }
}
