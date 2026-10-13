using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using RimWorld;
using LudeonTK;

namespace RUIF.Demo
{
    // ══════════════════════════════════════════════════════════════════════
    // ① 诊断：程序集 / 类型可用性 / 像素对齐对照
    // ══════════════════════════════════════════════════════════════════════
    internal sealed class BenchDiagnostics : IBench
    {
        private static readonly string[] TokenNames =
        {
            "Window", "Panel", "PanelRaised", "Card", "CardHover", "Border", "BorderSoft", "BorderHair",
            "Text", "Muted", "Dim", "Accent", "Info", "Alive", "Dead", "Warn"
        };

        private static readonly Color[] TokenColors =
        {
            Tokens.Window, Tokens.Panel, Tokens.PanelRaised, Tokens.Card, Tokens.CardHover,
            Tokens.Border, Tokens.BorderSoft, Tokens.BorderHair,
            Tokens.Text, Tokens.Muted, Tokens.Dim, Tokens.Accent, Tokens.Info, Tokens.Alive, Tokens.Dead, Tokens.Warn
        };

        /// <summary>
        /// 编译期可用性断言：这些 <c>typeof</c> 只要有一个不存在，本工程就<b>编不过</b> ——
        /// 因此"能构建"本身就是「公开 API 对消费方可用」的硬证据（不需要运行时检测）。
        /// 同时覆盖原版侧承接件（ReorderableWidget / PawnTable / MainButtonWorker / UIHighlighter / UIScaling）。
        /// </summary>
        private static readonly Type[] Probes =
        {
            typeof(Px), typeof(TexFmt), typeof(TexCache), typeof(TexAtoms),
            typeof(ReorderFmt), typeof(ReorderAtoms), typeof(HiAtoms),
            typeof(Draw), typeof(Layout), typeof(Atoms), typeof(Tokens),
            typeof(ReorderableWidget), typeof(PawnTable), typeof(PawnColumnWorker),
            typeof(RimWorld.MainButtonWorker), typeof(UIHighlighter), typeof(UIScaling)
        };

        /// <summary>
        /// <c>Atoms.SectionFrame</c> 的上下内边距合计（SpaceXs 上 + SpaceXs+4 下）。
        /// 段高 = 本值 + 内容高 —— 这样高度由内容推导，不再是手写魔数（手写必然与字体/换行漂移）。
        /// </summary>
        private const float FramePadV = Tokens.SpaceXs * 2f + 4f;

        /// <summary>按行数与行高推导段高。</summary>
        private static float SecH(int rows, float rowH) => FramePadV + rows * rowH;

        /// <summary>断言行数（每行 3 个）+ 1 行说明。</summary>
        private static readonly int AssertRows = (Probes.Length + 2) / 3;
        private static float AssertSectionHeight => SecH(AssertRows + 1, Measure.RowHeight(GameFont.Tiny));

        public bool Scrollable => true;

        public float Height(float width) => 600f;

        public void Render(Rect area)
        {
            var col = Layout.Column(area, Tokens.SpaceMd);

            // ── 程序集信息
            // 关键：SectionFrame **返回内容区**（已含自身内边距）。用它，不要再套一层 Inset + 手写偏移。
            var info = Layout.Column(Atoms.SectionFrame(col.Take(SecH(3, 18f)), "程序集与运行时"), 3f);
            Assembly asm = typeof(Draw).Assembly;
            Type[] allTypes = asm.GetTypes();
            int pubCount = 0;
            for (int i = 0; i < allTypes.Length; i++) if (allTypes[i].IsPublic) pubCount++;
            Line(info.Take(18f), "RUIF 程序集", asm.GetName().Name
                + "　Api " + Api.VersionString
                + "　程序集版本 " + asm.GetName().Version
                + "　类型 " + allTypes.Length + "（public " + pubCount + "）");
            Line(info.Take(18f), "UI 缩放", "Prefs.UIScale = " + Prefs.UIScale.ToString("0.##") + "（像素对齐的必要性正来自非整数缩放）");
            Line(info.Take(18f), "贴图缓存", "TexCache 上限 " + TexCache.MaxEntries + " 条（按插入顺序淘汰；Clear() 可整体释放）");

            // ── 类型可用性（高度按行数算：写死高度会让最后一行被裁，实测踩过）
            var tcol = Layout.Column(Atoms.SectionFrame(col.Take(AssertSectionHeight),
                "编译期类型断言（" + Probes.Length + " 个）"), 2f);
            Draw.EllipsisLabel(tcol.Take(18f), "构建通过 ⇒ 以下外层类型均可被消费方直接引用：", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            for (int i = 0; i < Probes.Length; i += 3)
            {
                var c = new System.Text.StringBuilder();
                for (int k = i; k < i + 3 && k < Probes.Length; k++) c.Append(Probes[k].FullName).Append("   ");
                Draw.EllipsisLabel(tcol.Take(18f), c.ToString(), GameFont.Tiny, Tokens.Info, TextAnchor.MiddleLeft);
            }

            // ── 像素对齐对照
            Rect pb = Atoms.SectionFrame(col.Take(150f), "像素对齐对照（Px）");
            float y0 = pb.y + 6f;
            float x0 = pb.x + 6f;
            for (int i = 0; i < 3; i++)
            {
                float raw = 0.5f + i * 0.5f;
                // 原始浮点坐标：UI 缩放非整数时会被插值糊成 2px 灰边
                Draw.LineH(x0 + raw, y0 + i * 22f, 220f, Tokens.Accent);
                Draw.Label(new Rect(x0, y0 + i * 22f - 8f, 200f, 16f), "raw + " + raw.ToString("0.0") + "px", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                // 对齐后：永远是干净 1px
                Rect sn = Px.HLine(x0 + raw, y0 + i * 22f + 8f, 220f);
                Draw.Box(sn, Tokens.Alive);
                Draw.Label(new Rect(x0 + 232f, y0 + i * 22f - 6f, 260f, 16f),
                    "raw y=" + (y0 + i * 22f).ToString("0.0") + " → Px y=" + sn.y.ToString("0.0") + "  h=" + sn.height.ToString("0.0"),
                    GameFont.Tiny, Tokens.Info, TextAnchor.MiddleLeft);
            }
            Draw.Label(new Rect(pb.x, pb.yMax - 16f, pb.width, 16f),
                "上：直接给浮点坐标（非整数缩放下发虚）　下：Px.HLine 吸附后（干净 1px）",
                GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

            // ── 令牌色卡
            Rect cb = Atoms.SectionFrame(col.Take(84f), "设计令牌 16 色");
            Rect strip = new Rect(cb.x, cb.y + 2f, cb.width, 24f);
            var row = Layout.Row(strip, 4f);
            for (int i = 0; i < TokenColors.Length; i++)
            {
                Rect sw = row.Take(56f);
                Draw.Box(sw, TokenColors[i]);
                Draw.Border(sw, Tokens.BorderHair);
            }
            Draw.Label(new Rect(cb.x, strip.yMax + 2f, cb.width, 16f), string.Join(" · ", TokenNames), GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
        }

        private static void Line(Rect r, string k, string v)
        {
            var row = Layout.Row(r, Tokens.SpaceSm);
            Draw.EllipsisLabel(row.Take(112f), k, GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.EllipsisLabel(row.Rest, v, GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // ② 贴图管线：真渐变 / 真渐隐 / 热力格网 / 饼环 + 与分段实现的 A/B 对照
    // ══════════════════════════════════════════════════════════════════════
    internal sealed class BenchTextures : IBench
    {
        private const int HeatRows = 12;
        private const int HeatCols = 44;
        private const int SectorSize = 128;

        private float[,] heat;
        private int heatVersion;
        private int sectorVersion;
        private float clock;

        public bool Scrollable => true;

        public float Height(float width) => 880f;

        public void Render(Rect area)
        {
            clock += Time.deltaTime;
            if (heat == null) Rebuild();

            var col = Layout.Column(area, Tokens.SpaceMd);

            // ── 渐变 A/B
            Rect b = Atoms.SectionFrame(col.Take(118f), "真渐变 vs 分段（A/B 对照）");
            var half = Layout.Row(new Rect(b.x, b.y, b.width, 40f), Tokens.SpaceMd);
            Rect a1 = half.Take((b.width - Tokens.SpaceMd) * 0.5f);
            Rect a2 = half.Rest;
            VisualAtoms.GradientBar(a1, Tokens.Accent, Tokens.Info, 16);
            Draw.Border(a1, Tokens.BorderHair);
            Draw.Label(new Rect(a1.x, a1.yMax + 2f, a1.width, 16f), "VisualAtoms.GradientBar（16 段折线）", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            TexAtoms.GradientRect(a2, Tokens.Accent, Tokens.Info);
            Draw.Border(a2, Tokens.BorderHair);
            Draw.Label(new Rect(a2.x, a2.yMax + 2f, a2.width, 16f), "TexAtoms.GradientRect（贴图，1 draw call）", GameFont.Tiny, Tokens.Alive, TextAnchor.MiddleLeft);
            Draw.Label(new Rect(b.x, b.yMax - 16f, b.width, 16f), "放大到全宽看台阶：分段版可见 16 段色阶，贴图版连续", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

            // ── 渐隐遮罩
            Rect fb = Atoms.SectionFrame(col.Take(106f), "真渐隐遮罩（FadeRight）");
            var lineRect = new Rect(fb.x, fb.y, fb.width, 26f);
            Draw.Box(lineRect, Tokens.PanelRaised);
            Draw.Label(new Rect(lineRect.x + 4f, lineRect.y, lineRect.width - 8f, lineRect.height),
                "殖民地在第 12 象遭遇持续三天的有毒尘埃，所有外出作业暂停，储藏区的草药开始腐烂",
                GameFont.Small, Tokens.Text, TextAnchor.MiddleLeft);
            TexAtoms.FadeRight(lineRect, Tokens.PanelRaised, 90f);
            Draw.Label(new Rect(fb.x, lineRect.yMax + 2f, fb.width, 16f),
                "渐隐带 RGB 必须等于背后真实底色（此处 Tokens.PanelRaised），否则会出现接缝",
                GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

            // ── 热力格网
            Rect hb = Atoms.SectionFrame(col.Take(192f), "热力格网（TexAtoms.Heat · 单次 draw call）");
            var heatRect = new Rect(hb.x, hb.y, hb.width, HeatRows * 12f);
            TexAtoms.Heat(heatRect, heatVersion, heat, 0f, 1f);
            Draw.Border(heatRect, Tokens.BorderHair);
            var hfoot = Layout.Row(new Rect(hb.x, heatRect.yMax + 4f, hb.width, 22f), Tokens.SpaceSm);
            if (Widgets.ButtonText(hfoot.Take(120f), "重算数据（版本 +1）")) Rebuild();
            Draw.Label(hfoot.Rest, "版本 " + heatVersion + " · " + HeatRows + "×" + HeatCols + " = " + (HeatRows * HeatCols)
                + " 格，仅 1 次 GUI.DrawTexture（改版本号才重算像素）", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

            // ── 饼 / 环
            Rect sb = Atoms.SectionFrame(col.Take(222f), "扇环（TexAtoms.Sector）· 饼图与环图同一原语");
            float size = 150f;
            var pie = new Rect(sb.x, sb.y, size, size);
            var donut = new Rect(pie.xMax + 40f, pie.y, size, size);
            var legend = new Rect(donut.xMax + 30f, pie.y + 20f, Mathf.Max(60f, sb.xMax - donut.xMax - 34f), 16f);

            float[] shares = { 0.42f, 0.31f, 0.27f };
            Color[] cols = { Tokens.Accent, Tokens.Info, Tokens.Alive };
            string[] names = { "建造", "采矿", "手工" };
            float a = 0f;
            var legendCol = Layout.Column(legend, 4f);
            for (int i = 0; i < shares.Length; i++)
            {
                float sweep = shares[i] * 360f;
                int ver = 1000 + i * 7 + sectorVersion;
                long k = TexCache.KeyVersioned(0x51, ver, SectorSize, SectorSize);
                TexAtoms.Sector(pie, k, a, sweep, 0f, cols[i], SectorSize);
                long k2 = TexCache.KeyVersioned(0x52, ver, SectorSize, SectorSize);
                TexAtoms.Sector(donut, k2, a, sweep, 0.55f, cols[i], SectorSize);
                a += sweep;

                Rect lg = legendCol.Take(18f);
                var lrow = Layout.Row(lg, 6f);
                Draw.Box(lrow.Take(12f), cols[i]);
                Draw.Label(lrow.Rest, names[i] + " " + Mathf.RoundToInt(shares[i] * 100f) + "%", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
            }
            var sfoot = Layout.Row(new Rect(sb.x, pie.yMax + 2f, sb.width, 22f), Tokens.SpaceSm);
            if (Widgets.ButtonText(sfoot.Take(110f), "换一组角度")) sectorVersion++;
            Draw.Label(sfoot.Rest, "缓存键把量化角度编进 version ⇒ 角度变了才重建贴图", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.Label(new Rect(pie.x, pie.yMax + 2f, size, 16f), "饼  inner=0", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
            Draw.Label(new Rect(donut.x, donut.yMax + 2f, size, 16f), "环  inner=0.55", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
        }

        private void Rebuild()
        {
            heat = new float[HeatRows, HeatCols];
            for (int r = 0; r < HeatRows; r++)
            {
                for (int c = 0; c < HeatCols; c++)
                {
                    float basev = 0.5f + 0.5f * Mathf.Sin((c * 0.28f) + r * 0.5f + clock * 0.2f);
                    heat[r, c] = Mathf.Clamp01(basev * 0.75f + UnityEngine.Random.value * 0.25f);
                }
            }
            heatVersion++;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // ⑤ 脉冲高亮：原版 UIHighlighter 的两帧协议
    // ══════════════════════════════════════════════════════════════════════
    internal sealed class BenchHighlight : IBench
    {
        private const string Tag = "RUIF.Demo.Highlight";
        private int markedFrame = -1;

        public bool Scrollable => true;

        public float Height(float width) => 300f;

        public void Render(Rect area)
        {
            // 协议第 1 步：本帧登记（这里模拟"进入页签时登记一次"）
            if (markedFrame < 0)
            {
                HiAtoms.Tag(Tag);
                markedFrame = Time.frameCount;
            }

            var col = Layout.Column(area, Tokens.SpaceMd);
            Rect b = Atoms.SectionFrame(col.Take(196f), "脉冲高亮（HiAtoms · 桥 Verse.UIHighlighter）");
            var body = Layout.Column(b, 6f);
            Draw.Label(body.Take(18f), "原版协议是两帧协作：本帧 Tag 登记 → 下一帧 Opportunity 才绘制。", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(body.Take(18f), "单独调 Opportunity 不会有任何效果（判定条件是 frameCount == 登记帧 + 1）。", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

            var btnRow = Layout.Row(body.Take(26f), Tokens.SpaceSm);
            if (Widgets.ButtonText(btnRow.Take(150f), "重新登记高亮"))
            {
                HiAtoms.Tag(Tag);
                markedFrame = Time.frameCount;
            }
            Draw.Label(btnRow.Rest, "当前：登记帧 " + markedFrame + " · 现在 " + Time.frameCount, GameFont.Tiny,
                Time.frameCount == markedFrame + 1 ? Tokens.Alive : Tokens.Dim, TextAnchor.MiddleLeft);

            // 协议第 2 步：目标控件处绘制（只有"登记后的下一帧"会真的亮）
            Rect target = body.Take(56f);
            target = new Rect(target.x, target.y, Mathf.Min(320f, target.width), 48f);
            Draw.Box(target, Tokens.PanelRaised);
            Draw.Border(target, Tokens.BorderSoft);
            Draw.Label(target, "　这里是一个「新功能」目标控件", GameFont.Small, Tokens.Text, TextAnchor.MiddleLeft);
            HiAtoms.Opportunity(target, Tag);
        }
    }
}
