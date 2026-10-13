using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// 组件画廊的补充段：数据展示 / 面板与几何 / 导航与杂项。
    /// 单独成类只为控制单文件体积；所有调用都是真实签名（签名取自 <c>Assemblies/RUIF.xml</c> 反射快照）。
    /// </summary>
    internal sealed class GalleryExtra
    {
        private readonly string[] skills = { "射击", "格斗", "建造", "采矿", "手工", "艺术" };
        private readonly float[] skillVals = { 0.35f, 0.20f, 0.85f, 0.60f, 0.45f, 0.70f };
        private readonly string[] needs = { "心情", "饮食", "休息", "娱乐" };
        private readonly float[] needVals = { 0.62f, 0.78f, 0.41f, 0.88f };

        private int tabIdx = 1;
        private float tabScroll;
        private int page = 2;
        private readonly int[] pageSizes = { 10, 25, 50, 100 };
        private int dot = 1;

        public const float DataH = 132f;
        public const float PanelGeoH = 168f;
        public const float NavMiscH = 150f;

        // ── 数据展示 DataAtoms
        public void DataSection(Rect area)
        {
            Rect b = Atoms.SectionFrame(area, "数据展示 DataAtoms（技能条 / 需求面板）");
            var row = Layout.Row(b, Tokens.SpaceLg);
            Rect left = row.Take((b.width - Tokens.SpaceLg) * 0.55f);
            Rect right = row.Rest;

            Draw.Label(new Rect(left.x, left.y, left.width, 16f), "SkillBars（0–20 级色阶）", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            DataAtoms.SkillBars(new Rect(left.x, left.y + 18f, left.width, left.height - 18f), skills, skillVals, 20f, 12);

            Draw.Label(new Rect(right.x, right.y, right.width, 16f), "NeedsPanel（需求条）", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            DataAtoms.NeedsPanel(new Rect(right.x, right.y + 18f, right.width, right.height - 18f), needs, needVals);
        }

        // ── 面板与几何 PanelAtoms / Shapes
        public void PanelGeoSection(Rect area)
        {
            Rect b = Atoms.SectionFrame(area, "面板与几何 PanelAtoms / Shapes");
            var col = Layout.Column(b, 6f);

            // StatRow ×3
            var srow = Layout.Row(col.Take(20f), Tokens.SpaceMd);
            PanelAtoms.StatRow(srow.Take(150f), "心情", "62%", 0.62f, Tokens.Accent);
            PanelAtoms.StatRow(srow.Take(150f), "健康", "96%", 0.96f, Tokens.Alive);
            PanelAtoms.StatRow(srow.Take(150f), "疲劳", "78%", 0.78f, Tokens.Warn);

            // CornerFrame + Watermark 并排
            var crow = Layout.Row(col.Take(52f), Tokens.SpaceMd);
            Rect cf = crow.Take(170f);
            Shapes.CornerFrame(cf, Tokens.Accent, 14f, 2f);
            Draw.Label(cf, "CornerFrame", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleCenter);

            Rect wm = crow.Take(170f);
            Shapes.InsetPanel(wm);
            Shapes.Watermark(wm, "PC", 0.10f);
            Draw.Label(wm, "InsetPanel + Watermark", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleCenter);

            Rect rule = crow.Rest;
            Shapes.RuleWithLabel(new Rect(rule.x, rule.y, rule.width, 16f), "战斗履历", Tokens.Border, 1f);
            Draw.Label(new Rect(rule.x, rule.y + 18f, rule.width, 16f), "RuleWithLabel", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

            // MiniProfileChip ×2
            var mrow = Layout.Row(col.Take(20f), Tokens.SpaceMd);
            PanelAtoms.MiniProfileChip(mrow.Take(220f), "蓝 雪牛", "建造 12 · 出类拔萃", 0.72f, 0.94f);
            PanelAtoms.MiniProfileChip(mrow.Take(220f), "赤 眼镜蛇", "射击 15 · 登峰造极", 0.38f, 0.81f);
        }

        // ── 导航与杂项 NavAtoms / MiscAtoms
        public void NavMiscSection(Rect area)
        {
            Rect b = Atoms.SectionFrame(area, "导航与杂项 NavAtoms / MiscAtoms");
            var col = Layout.Column(b, 7f);

            Rect tabR = col.Take(28f);
            NavAtoms.TabBar(tabR, new[] { "总览", "技能", "需求", "社交" }, ref tabIdx, ref tabScroll);

            var brRow = Layout.Row(col.Take(20f), Tokens.SpaceMd);
            NavAtoms.Breadcrumb(brRow.Take(280f), new[] { "殖民地", "迫降乡", "殖民者", "蓝 雪牛" }, null);
            NavAtoms.AnchorDots(brRow.Take(120f), new[] { "一", "二", "三", "四" }, ref dot);

            var pgRow = Layout.Row(col.Take(22f), Tokens.SpaceMd);
            NavAtoms.Pager(pgRow.Take(240f), 12, page, ref page, pageSizes);
            MiscAtoms.PointCostBadge(pgRow.Take(120f), 72, 200);

            Rect bub = col.Rest;
            if (bub.height > 24f) MiscAtoms.BubbleTag(new Rect(bub.x, bub.y, 260f, 24f), "宿醉（轻微）", Tokens.Warn);
        }
    }
}
