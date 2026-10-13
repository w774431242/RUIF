using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>⑦ 导航候选组件（原 📋，官方实证支持，现落地）：主标签页框架 / 检视页签组 / 页面向导 /
    /// 搜索面板 / 策略管理器。皆为纯 presentational 基础件（无状态，选中态由调用方 ref 持有）；
    /// 接原版窗口生命周期（<c>MainTabWindow</c>/<c>ITab</c>/<c>Page</c>）属集成件，留作游戏实机验收。
    /// 官方依据：<c>MainTabWindow</c>(15) · <c>ITab_*</c>(48) · <c>Page_*</c>(15) ·
    /// <c>Dialog_Search</c>/<c>MapSearch</c> · <c>Dialog_ManagePolicies</c>（见《原版UI代码标准（Spy取证）》）。</summary>
    public static class NavCandAtoms
    {
        // ---- 主标签页框架（底栏）----

        /// <summary>主标签页底栏：标签 pill，选中=抬起强调；点击选中。生命周期接入 MainTabWindow 属集成件。</summary>
        public static void MainTabStrip(Rect rect, string[] labels, ref int sel)
        {
            if (labels == null) return;
            sel = Mathf.Clamp(sel, 0, labels.Length - 1);
            const float gap = 2f, padX = 14f, unit = 6.5f;
            float x = rect.x + 4f;
            for (int i = 0; i < labels.Length; i++)
            {
                float w = Mathf.Min(NavFmt.LabelUnits(labels[i]) * unit + padX * 2f, rect.xMax - x);
                if (w <= 0) break;
                Rect tab = new Rect(x, rect.y + 2f, w, rect.height - 4f);
                bool on = i == sel;
                Draw.Box(tab, on ? Tokens.AccentSoft : (Verse.Mouse.IsOver(tab) ? Tokens.CardHover : Tokens.Panel));
                if (on) Draw.Border(tab, Tokens.Accent);
                Draw.Label(tab, labels[i], Tokens.FontLabel, NavCandFmt.MainTabTone(on), TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(tab)) sel = i;
                x += w + gap;
            }
        }

        // ---- 检视页签组（迷你签）----

        /// <summary>检视页签组：检视面板内的迷你横向页签，选中=下划线强调；点击选中。</summary>
        public static void InspectTabs(Rect rect, string[] labels, ref int sel)
        {
            if (labels == null) return;
            sel = Mathf.Clamp(sel, 0, labels.Length - 1);
            const float gap = 4f, padX = 10f, unit = 6f;
            float x = rect.x;
            for (int i = 0; i < labels.Length; i++)
            {
                float w = Mathf.Min(NavFmt.LabelUnits(labels[i]) * unit + padX * 2f, rect.xMax - x);
                if (w <= 0) break;
                Rect tab = new Rect(x, rect.y, w, rect.height);
                bool on = i == sel;
                if (on)
                {
                    Draw.Label(tab, labels[i], Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleCenter);
                    Draw.Box(new Rect(tab.x + 4f, tab.yMax - 2f, tab.width - 8f, 2f), Tokens.Accent);
                }
                else
                {
                    if (Verse.Mouse.IsOver(tab)) Draw.Box(tab, Tokens.CardHover);
                    Draw.Label(tab, labels[i], Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
                    if (Widgets.ButtonInvisible(tab)) sel = i;
                }
                x += w + gap;
            }
        }

        // ---- 页面向导（步骤条）----

        /// <summary>页面向导步骤条：编号节点 + 连接线，已完成/当前/待办三态；点击跳步（写回 step）。</summary>
        public static void WizardSteps(Rect rect, string[] steps, ref int step)
        {
            if (steps == null || steps.Length == 0) return;
            int n = steps.Length;
            step = Mathf.Clamp(step, 0, n - 1);
            const float circle = 16f;
            float slot = rect.width / n;
            float prevCx = 0f;
            for (int i = 0; i < n; i++)
            {
                float cx = rect.x + slot * (i + 0.5f);
                if (i > 0)
                    Draw.LineH(prevCx + circle / 2f, rect.y + circle / 2f,
                        (cx - circle / 2f) - (prevCx + circle / 2f), NavCandFmt.WizardConnectorTone(i, step));
                var node = new Rect(cx - circle / 2f, rect.y, circle, circle);
                Color tone = NavCandFmt.WizardStepTone(i, step);
                Draw.Box(node, i == step ? Tokens.PanelRaised : tone);
                if (i == step) Draw.Border(node, Tokens.Accent);
                Draw.Label(node, (i + 1).ToString(), Tokens.FontLabel, NavCandFmt.WizardNumberTone(i, step), TextAnchor.MiddleCenter);
                Draw.Label(new Rect(cx - slot / 2f, rect.y + circle + 2f, slot, rect.height - circle - 2f),
                    steps[i] ?? string.Empty, Tokens.FontLabel, tone, TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(node)) step = i;
                prevCx = cx;
            }
        }

        // ---- 搜索面板（输入框）----

        /// <summary>搜索输入框：文本输入 + 占位符 + 清除按钮（×）；空文本显示「搜索…」。</summary>
        public static void SearchBox(Rect rect, ref string text)
        {
            text = text ?? string.Empty;
            Draw.Box(rect, Tokens.PanelRaised);
            Draw.Border(rect, Tokens.BorderSoft);
            float clearW = text.Length > 0 ? 18f : 0f;
            float padL = 6f;
            Rect field = new Rect(rect.x + padL, rect.y, rect.width - padL - clearW - 2f, rect.height);
            text = Widgets.TextField(field, text);
            if (text.Length == 0)
                Draw.Label(new Rect(field.x + 4f, rect.y, field.width - 4f, rect.height), "搜索…", Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
            if (clearW > 0f)
            {
                Rect xb = new Rect(rect.xMax - clearW, rect.y, clearW, rect.height);
                Draw.Label(xb, "×", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(xb)) text = string.Empty;
            }
        }

        // ---- 策略管理器（策略列表）----

        /// <summary>策略管理器：策略列表，选中=强调软底 + 单选点；右侧「⋮⋮」为排序握把（presentational）。</summary>
        public static void PolicyList(Rect rect, string[] policies, ref int sel)
        {
            if (policies == null) return;
            var col = Layout.Column(rect, 3f);
            float rowH = Mathf.Min(28f, rect.height / Mathf.Max(1, policies.Length));
            for (int i = 0; i < policies.Length; i++)
            {
                Rect row = col.Take(rowH);
                bool on = i == sel;
                Draw.Box(row, NavCandFmt.PolicyRowTone(on));
                if (on) Draw.Border(row, Tokens.Accent);
                Rect dot = new Rect(row.x + 4f, row.y + (row.height - 14f) / 2f, 14f, 14f);
                Draw.Box(dot, on ? Tokens.Accent : Tokens.PanelRaised);
                Draw.Border(dot, Tokens.BorderSoft);
                if (on) Draw.Label(dot, "●", Tokens.FontLabel, Tokens.Window, TextAnchor.MiddleCenter);
                Draw.Label(new Rect(row.x + 24f, row.y, row.width - 24f - 24f, row.height),
                    policies[i] ?? string.Empty, Tokens.FontLabel, on ? Tokens.Text : Tokens.Muted, TextAnchor.MiddleLeft);
                Draw.Label(new Rect(row.xMax - 22f, row.y, 20f, row.height), "⋮⋮", Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(row)) sel = i;
            }
        }
    }
}
