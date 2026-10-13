using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>反馈原子的纯计算（可离线单测）。</summary>
    public static class FeedbackFmt
    {
        /// <summary>Toast 透明度：临近过期淡出（t 为已存活秒数）。</summary>
        public static float ToastAlpha(float age, float ttl, float fade = 0.5f)
        {
            if (age >= ttl) return 0f;
            if (ttl - age <= fade) return (ttl - age) / fade;
            return 1f;
        }

        /// <summary>通知合并：同屏最多 visibleMax 条。</summary>
        public static int MergeVisible(int total, int visibleMax) => Mathf.Max(0, Mathf.Min(total, visibleMax));

        /// <summary>合并徽标数字（隐藏条数，0 表示无合并）。</summary>
        public static int MergeHidden(int total, int visibleMax) => Mathf.Max(0, total - visibleMax);

        /// <summary>警报脉冲 alpha：紧急快闪（0.6s）/ 常规慢闪（1.6s）/ 低常亮。</summary>
        public static float BlinkAlpha(float t, AlertPriority p)
        {
            if (p == AlertPriority.Low) return 1f;
            float period = p == AlertPriority.Urgent ? 0.6f : 1.6f;
            return 0.35f + 0.65f * (0.5f + 0.5f * Mathf.Sin(t * (Mathf.PI * 2f / period)));
        }

        /// <summary>步骤标记：i&lt;current 已完成 ✓，i==current 进行 ●，之后待办 ○。</summary>
        public static string StepMark(int i, int current)
            => i < current ? "✓" : (i == current ? "●" : "○");

        /// <summary>批次摘要文案（类型分布）。</summary>
        public static string BatchSummary(string[] types, int[] counts)
        {
            if (types == null || counts == null) return string.Empty;
            int n = Mathf.Min(types.Length, counts.Length);
            var parts = new string[n];
            for (int i = 0; i < n; i++) parts[i] = types[i] + "×" + counts[i];
            return string.Join(" · ", parts);
        }
    }

    /// <summary>Toast 级别（成功/警告/危险）。</summary>
    public enum ToastLevel { Ok, Warn, Danger }

    /// <summary>警报优先级（官方 Alert 分级语义）。</summary>
    public enum AlertPriority { Low, Normal, Warn, Urgent }

    /// <summary>
    /// S4 反馈原子（D1 核心 · Toast/Alert/Confirm/RichTooltip/ProgressOverlay/EmptyState）。
    /// 无状态；alpha 与时间由调用方传入（GATE-UI）。
    /// </summary>
    public static class FeedbackAtoms
    {
        /// <summary>
        /// 单条 Toast（三态左边条）。<paramref name="alpha"/> 由 <c>FeedbackFmt.ToastAlpha</c> 按 TTL 驱动（1=不透明）。
        ///
        /// 🔧 修复（2026-09-10 体检）：原实现**完全没用 alpha**（注释承诺淡出、实现没做）→ 所有元素固定不透明。
        /// 现改为逐项把 alpha 乘进颜色（不走 <c>GUI.color</c>：<c>Draw.Box</c>/<c>Draw.Label</c> 内部会覆盖它，实测无效）。
        /// </summary>
        public static void Toast(Rect rect, string title, string sub, ToastLevel level, float alpha)
        {
            float a = Mathf.Clamp01(alpha);
            Color tone = level switch { ToastLevel.Warn => Tokens.Warn, ToastLevel.Danger => Tokens.Dead, _ => Tokens.Alive };
            Draw.Box(rect, Draw.WithAlpha(Tokens.Card, a));
            Draw.Box(new Rect(rect.x, rect.y, 3f, rect.height), Draw.WithAlpha(tone, a));
            Draw.Border(rect, Draw.WithAlpha(Tokens.BorderSoft, a));
            var col = Layout.Column(Layout.Inset(rect, 10f, 4f), 2f);
            Draw.Label(col.Take(16f), title ?? string.Empty, Tokens.FontLabel, Draw.WithAlpha(Tokens.Text, a), TextAnchor.MiddleLeft);
            if (!string.IsNullOrEmpty(sub))
                Draw.Label(col.Take(13f), sub, Tokens.FontLabel, Draw.WithAlpha(Tokens.Muted, a), TextAnchor.MiddleLeft);
        }

        /// <summary>警报横幅：info/warn/danger 色带 + 可选动作按钮；返回动作是否点击。</summary>
        public static bool AlertBanner(Rect rect, AlertPriority level, string msg, string actionLabel = null)
        {
            Color tone = level switch { AlertPriority.Urgent => Tokens.Dead, AlertPriority.Warn => Tokens.Warn, _ => Tokens.Info };
            Draw.Box(rect, Draw.WithAlpha(tone, 0.12f));
            Draw.LineV(rect.x, rect.y, rect.height, tone);
            Draw.Border(rect, Tokens.BorderSoft);
            var row = Layout.Row(rect, Tokens.SpaceXs);
            Draw.Label(row.Take(Mathf.Max(10f, rect.width - 90f)), msg ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            if (string.IsNullOrEmpty(actionLabel)) return false;
            return Widgets.ButtonText(row.Rest, actionLabel, true, false, true);
        }

        /// <summary>确认弹窗主体（标准/危险两态）；返回确认按钮是否点击。</summary>
        public static bool ConfirmDialog(Rect rect, string title, string msg, bool danger,
            string cancelLabel = null, string okLabel = null)
        {
            Draw.Box(rect, Tokens.Panel);
            Draw.Border(rect, danger ? Tokens.Dead : Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 10f, 8f), 6f);
            Draw.Label(col.Take(20f), title ?? string.Empty, Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(col.Take(30f), msg ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.UpperLeft);
            var row = Layout.Row(col.Rest, Tokens.SpaceXs);
            if (Widgets.ButtonText(row.Take(70f), cancelLabel ?? Tr.Get("RUIF.Comp.Common.Cancel"))) return false;
            return Widgets.ButtonText(row.Rest, okLabel ?? (danger ? Tr.Get("RUIF.Comp.Common.Confirm") : Tr.Get("RUIF.Comp.Common.OK")), true, false, true);
        }

        /// <summary>富提示：标题 + 键值统计 + 需求勾叉（✓绿 ✗红）。</summary>
        public static void RichTooltip(Rect rect, string title, string[] stats, string[] requirements)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.Accent);
            var col = Layout.Column(Layout.Inset(rect, 8f, 5f), 3f);
            Draw.Label(col.Take(18f), title ?? string.Empty, Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleLeft);
            if (stats != null)
                for (int i = 0; i < stats.Length; i++)
                    Draw.Label(col.Take(14f), stats[i] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            if (requirements != null)
                for (int i = 0; i < requirements.Length; i++)
                    Draw.Label(col.Take(14f), "· " + (requirements[i] ?? string.Empty), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
        }

        /// <summary>进度遮罩：半透明背景 + 标题 + 进度条 + 步骤文案。</summary>
        public static void ProgressOverlay(Rect rect, string title, float progress01, string step)
        {
            Draw.Box(rect, Draw.WithAlpha(Tokens.Window, 0.72f));
            float w = Mathf.Min(300f, rect.width * 0.7f);
            var inner = Layout.Column(new Rect(rect.center.x - w / 2f, rect.y + rect.height * 0.25f, w, rect.height * 0.5f), 5f);
            Draw.Label(inner.Take(16f), title ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
            Rect bar = inner.Take(8f);
            Draw.Box(bar, Tokens.BorderSoft);
            Draw.Box(new Rect(bar.x, bar.y, bar.width * Mathf.Clamp01(progress01), bar.height), Tokens.Accent);
            Draw.Label(inner.Rest, step ?? string.Empty, Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleCenter);
        }

        /// <summary>空状态（accent 短条 + 主标题 + 副文案 + CTA）；返回 CTA 是否点击。</summary>
        public static bool EmptyState(Rect rect, string title, string sub, string cta = null)
        {
            float w = Mathf.Min(300f, rect.width * 0.8f);
            var inner = Layout.Column(new Rect(rect.center.x - w / 2f, rect.y + rect.height * 0.15f, w, rect.height * 0.7f), 4f);
            Draw.Box(new Rect(rect.center.x - 14f, inner.Take(3f).y, 28f, 3f), Tokens.Accent);
            inner.Take(6f);
            Draw.Label(inner.Take(18f), title ?? string.Empty, Tokens.FontBody, Tokens.Text, TextAnchor.MiddleCenter);
            Draw.Label(inner.Take(14f), sub ?? string.Empty, Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleCenter);
            if (string.IsNullOrEmpty(cta)) return false;
            Rect btn = inner.Take(24f);
            return Widgets.ButtonText(new Rect(rect.center.x - 60f, btn.y, 120f, 22f), cta, true, false, true);
        }
    }
}
