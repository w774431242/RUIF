using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// 组件验证台 · 「视觉补充」面（BenchVisualExtra）。
    ///
    /// <para>渲染一批<b>原生 Atoms（直传 Rect，无候选层）</b>，覆盖 visual / progress / table 三个域：
    /// <see cref="MiscAtoms"/>（PointCostBadge / BubbleTag / ThinScrollBar）、
    /// <see cref="Shapes"/>（CornerFrame / GradientRule / RuleWithLabel / Watermark / InsetPanel）、
    /// <see cref="ProgressAtoms"/>（ProgressRing）、
    /// <see cref="TableAtoms"/>（ValueCell / DensitySwitch）。</para>
    ///
    /// <para>每个组件按真实源码签名调用（以 <c>Source/Components/*.cs</c> 为准）。
    /// 高度与布局用 <see cref="Layout.Column"/>/<see cref="Layout.Row"/> 垂直堆叠，各段用
    /// <see cref="Atoms.SectionFrame"/> 分组，整体不溢出 <c>area</c>。</para>
    /// </summary>
    internal sealed class BenchVisualExtra : IBench
    {
        // ── 各分组高度（与 Render 中的 Take 严格对应）──────────────────────
        // ⚠ SectionFrame 实测返回 Inset(outer, 8, 12)：垂直内缩 24（上下各 12）。
        //   段高 = 内容高 + 24；Prog 段另含内层 Layout.Inset(12,12) 的 24。
        private const float MiscH = 234f;    // PointCostBadge 30 + BubbleTag 46 + ThinScrollBar 110 + 间隙 24 = 210，+24
        private const float ShapesH = 344f;  // CornerFrame 84 + GradientRule 16 + RuleWithLabel 28 + Watermark 60 + InsetPanel 84 + 间隙 48 = 320，+24
        private const float ProgH = 168f;    // ring 120 + 内层 Inset 24 = 144，+24
        private const float TableH = 102f;   // ValueCell 40 + DensitySwitch 26 + 间隙 12 = 78，+24

        /// <summary>DensitySwitch 的三态由调用方持有（组件零状态，ref 传入）。</summary>
        private RowDensity density = RowDensity.Normal;

        public bool Scrollable => true;

        public float Height(float width)
            => MiscH + ShapesH + ProgH + TableH + Tokens.SpaceMd * 3f;

        public void Render(Rect area)
        {
            var col = Layout.Column(area, Tokens.SpaceMd);

            // ────────────────────────────────────────────── 杂项原子 · MiscAtoms
            {
                Rect frame = Atoms.SectionFrame(col.Take(MiscH), "杂项原子 · MiscAtoms");
                var c = Layout.Column(frame, Tokens.SpaceSm);

                // 预算点数徽章：剩余（绿）/ 用尽（黄）/ 超支（红）
                MiscAtoms.PointCostBadge(c.Take(30f), 100, 63);

                // 气泡标签：矩形主体 + 下方小三角
                var bRow = Layout.Row(c.Take(46f), Tokens.SpaceLg);
                Rect bubble = bRow.Take(150f);
                MiscAtoms.BubbleTag(bubble, "示例文本", Tokens.Accent);
                Draw.Label(bRow.Rest, "BubbleTag（气泡标签）", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

                // 细滚动条：画在视口右侧（contentH > 视口高才出现滑块）
                Rect tsRow = c.Take(110f);
                Rect tsView = new Rect(tsRow.x, tsRow.y, Mathf.Max(60f, tsRow.width - 14f), tsRow.height);
                Draw.Box(tsView, Tokens.PanelRaised);
                MiscAtoms.ThinScrollBar(tsView, 480f, 120f);
            }

            // ────────────────────────────────────────────── 几何与装饰 · Shapes
            {
                Rect frame = Atoms.SectionFrame(col.Take(ShapesH), "几何与装饰 · Shapes");
                var c = Layout.Column(frame, Tokens.SpaceSm);

                // 四角 L 型框（HUD / 选中态气质）
                Rect cf = c.Take(84f);
                Shapes.CornerFrame(cf, Tokens.Accent, 14f, 2f);

                // 渐变分隔线（分段递减 alpha）
                Shapes.GradientRule(c.Take(16f), Tokens.Info, 12);

                // 带文字的分割线（── 记录 ──）
                // ⚠ RuleWithLabel 内部按当前 Text.Font 测量但以 Tiny 绘制 ⇒ 调用前包 FontLabel（红线，Restored 同姿势）。
                using (Draw.Font(Tokens.FontLabel))
                {
                    Shapes.RuleWithLabel(c.Take(28f), "记录", Tokens.BorderSoft, 6f);
                }

                // 水印（低透明度大字，画在内容之下）
                Rect wm = c.Take(60f);
                Draw.Box(wm, Tokens.PanelRaised);
                Shapes.Watermark(wm, "RUIF", 0.06f);

                // 凹陷面板（内嵌感）
                Shapes.InsetPanel(c.Take(84f));
            }

            // ────────────────────────────────────────────── 弧形进度环 · ProgressAtoms
            {
                Rect frame = Atoms.SectionFrame(col.Take(ProgH), "弧形进度环 · ProgressAtoms.ProgressRing");
                Rect r = Layout.Inset(frame, Tokens.SpaceSm, Tokens.SpaceSm);
                // 取 120×120 方形区域；开口环（sweepDeg = 270°）+ 中心读数
                Rect ring = new Rect(r.x, r.y, Mathf.Min(120f, r.width), 120f);
                ProgressAtoms.ProgressRing(ring, 0.6f, Tokens.BorderSoft, Tokens.Alive, "60%", 270f, 0.22f);
                Draw.Label(new Rect(ring.xMax + 12f, ring.y, frame.width - ring.width - 24f, ring.height),
                    "progress01 = 0.6 · 开口环 270° · 中心读数 60%", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            }

            // ────────────────────────────────────────────── 表格原子 · TableAtoms
            {
                Rect frame = Atoms.SectionFrame(col.Take(TableH), "表格原子 · TableAtoms");
                var c = Layout.Column(frame, Tokens.SpaceSm);

                // 阈值着色数值单元：低（红）/ 中（黄）/ 高（绿）
                var vRow = Layout.Row(c.Take(40f), Tokens.SpaceSm);
                float cellW = vRow.Rest.width / 3f;
                TableAtoms.ValueCell(vRow.Take(cellW), 0.20f, 1f);
                TableAtoms.ValueCell(vRow.Take(cellW), 0.50f, 1f);
                TableAtoms.ValueCell(vRow.Rest, 0.90f, 1f);

                // 行密度三档切换（Compact / Normal / Cozy）
                TableAtoms.DensitySwitch(c.Take(26f), ref density);
            }
        }
    }
}
