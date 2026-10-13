using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>⑨ 版式候选组件（原 📋，现落地）：分层钻取孪生驾驶舱（Cockpit）外壳——KPI 顶条 + 左导航 +
    /// 主面板 + 底部钻取抽屉。纯 presentational 布局骨架；KPI→M3 抽屉→GridMap/CircuitTopology 的真实数据钻取属集成件。
    /// 官方依据：M3 抽屉钻取版式 / <c>MainTabWindow</c> 指挥台形态。</summary>
    public static class LayoutCandAtoms
    {
        /// <summary>分层钻取孪生驾驶舱外壳：返回当前激活面板索引（写回 activePanel）。</summary>
        public static void CockpitFrame(Rect rect, string[] kpis, string[] panels, ref int activePanel)
        {
            if (panels == null || panels.Length == 0) return;
            activePanel = Mathf.Clamp(activePanel, 0, panels.Length - 1);
            var col = Layout.Column(rect, 4f);

            if (kpis != null && kpis.Length > 0)
            {
                Rect kpi = col.Take(54f);
                var kr = Layout.Row(kpi, 4f);
                float kw = (kpi.width - 4f * (kpis.Length - 1)) / kpis.Length;
                for (int i = 0; i < kpis.Length; i++)
                {
                    Rect card = kr.Take(kw);
                    Draw.Box(card, Tokens.Card);
                    Draw.Border(card, Tokens.BorderSoft);
                    Draw.Label(card, kpis[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
                }
            }

            Rect body = col.Take(col.Rest.height - 96f);
            Layout.Split(body, 0.18f, 4f, out Rect nav, out Rect main);

            var ncol = Layout.Column(nav, 2f);
            for (int i = 0; i < panels.Length; i++)
            {
                Rect nr = ncol.Take(26f);
                bool on = i == activePanel;
                Draw.Box(nr, on ? Tokens.AccentSoft : Tokens.Card);
                if (on) Draw.Border(nr, Tokens.Accent);
                Draw.Label(nr, panels[i] ?? string.Empty, Tokens.FontLabel, on ? Tokens.Accent : Tokens.Muted, TextAnchor.MiddleLeft);
                if (Widgets.ButtonInvisible(nr)) activePanel = i;
            }

            Draw.Box(main, Tokens.Panel);
            Draw.Border(main, Tokens.BorderSoft);
            Draw.Label(main, "▣ " + panels[activePanel], Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);

            Rect drawer = col.Rest;
            Draw.Box(drawer, Tokens.Card);
            Draw.Border(drawer, Tokens.BorderSoft);
            Draw.Label(new Rect(drawer.x + 4f, drawer.y + 2f, drawer.width - 8f, 16f),
                "钻取抽屉 · " + panels[activePanel], Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
        }
    }
}
