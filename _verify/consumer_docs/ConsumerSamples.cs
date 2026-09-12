// RUIF 文档示例真编译验证工程（消费者视角）
// 目的：把《零基础上手教程》《开发者入口》里的示例**逐字**放进来，对一个真实的消费者 Mod 编译一次。
// 依据：docs/接入审计报告_2026-09-12.md 的 S-1 建议 —— 文档示例必须真编译，禁止"看起来对"。
// 用法：pwsh -File RUIF/_verify/consumer_docs/build.ps1   （引用发布区 RUIF.dll）
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RUIF;
using UnityEngine;
using Verse;

namespace TutorialConsumer
{
    // =====================================================================================
    // 第 2 节 · 最小可运行窗口（教程正文逐字使用）
    // =====================================================================================
    public class Dialog_Tutorial : Window
    {
        private string query = string.Empty;
        private string queryBuf = string.Empty;
        private float lastInputTime;
        private int segIndex;
        private bool switchOn;
        private int stepper = 3;
        private float amount = 0.5f;
        private float numeric = 12f;
        private string numericBuf = "12";
        private Vector2 scroll;
        private int tab;
        private float tabX;
        private TimeSpeed speed = TimeSpeed.Normal;
        private RowDensity density = RowDensity.Normal;
        private SortState sort = SortState.None;
        private readonly bool[] picked = { false, false, false };
        private readonly float[] series = { 3f, 6f, 2f, 9f, 4f, 7f };
        private readonly float[] shares = { 0.4f, 0.2f, 0.2f, 0.2f };
        private float zoom = 1f;

        public override Vector2 InitialSize => new Vector2(780f, 560f);

        public override void DoWindowContents(Rect inRect)
        {
            // ① 一个纵向游标：把窗口切成一行一行
            var col = Layout.Column(inRect, Tokens.Gap);
            Rect head = col.Take(30f);
            Draw.Label(head, "我的第一个 RUIF 窗口", Tokens.FontValue, Tokens.Text, TextAnchor.MiddleLeft);

            // ② 内容区（可滚动）
            Rect body = col.Rest;
            Rect view = Layout.BeginScroll(body, 640f, ref scroll);

            var v = Layout.Column(view, Tokens.BlockGap);
            DrawStatuses(v.Take(30f));
            DrawInputs(v.Take(96f));
            DrawCharts(v.Take(80f));

            Layout.EndScroll();

            // ③ 底部按钮：RUIF 不提供通用按钮，直接用原版 Widgets
            if (Widgets.ButtonText(col.Take(30f), "确定")) Close();
        }

        // =====================================================================================
        // 第 4.1 节 · 面板与卡片
        // =====================================================================================
        private void DrawPanel(Rect area)
        {
            Draw.Box(area, Tokens.Panel);
            Draw.Border(area, Tokens.Border);
            Rect inner = Layout.Inset(area, Tokens.CardPadX, Tokens.CardPadY);

            var col = Layout.Column(inner, Tokens.SpaceXs);
            PanelAtoms.SectionBar(col.Take(Tokens.SectionTitleHeight), "殖民地概览", "3 项", true);
            PanelAtoms.StatRow(col.Take(22f), "人口", "7", 7f / 12f, Tokens.Accent);
            PanelAtoms.StatRow(col.Take(22f), "食物", "1 240", 0.62f);
            PanelAtoms.StatRow(col.Take(22f), "电力", "-320 W");
        }

        private void DrawSectionFrame(Rect area)
        {
            Rect inner = Atoms.SectionFrame(area, "装备清单");
            Rect box = Atoms.AspectBox(area, 16f / 9f);
            Rect corner = Atoms.Anchor(area, Anchor9.TopRight, new Vector2(16f, 16f), 4f);
            Shapes.InsetPanel(inner);
            Shapes.CornerFrame(area, Tokens.Accent);
            Shapes.GradientRule(inner.TopPartPixels(1f), Tokens.Accent);
            Shapes.RuleWithLabel(inner.TopPartPixels(18f), "本页 3 条", Tokens.Muted);
            Shapes.Watermark(area, "草稿");
            VisualAtoms.PinBadge(box, true);
            VisualAtoms.GradientBar(corner, Tokens.Accent, Tokens.Alive);
            VisualAtoms.Swatch(corner, Tokens.Warn, true);
            VisualAtoms.OverflowFade(corner, "很长的名称会被淡出", Tokens.Panel);
        }

        // =====================================================================================
        // 第 4.2 节 · 文本与状态
        // =====================================================================================
        private void DrawTexts(Rect row)
        {
            var h = Layout.Row(row, Tokens.SpaceSm);
            TextAtoms.UnitLabel(h.Take(150f), TextFmt.FormatNumber(12480), "银");
            TextAtoms.SubtitleLine(h.Take(220f), "夜班效率", "↑ 12%");
            TextAtoms.Typewriter(h.Take(200f), "正在记录…", 0.5f, Tokens.FontBody, Tokens.Muted);
            string pretty = TextFmt.FormatNumber(1530000);
            Draw.Label(h.Rest, pretty, Tokens.FontBody, Tokens.Info, TextAnchor.MiddleLeft);
        }

        private void DrawStatuses(Rect row)
        {
            var h = Layout.Row(row, Tokens.SpaceMd);
            StatusAtoms.PulseDot(h.Take(22f), Tokens.Alive, 1.5f);
            StatusAtoms.Glyph(h.Take(20f), StatusKind.Warn);
            StatusAtoms.Spinner(h.Take(24f), 0.4f);
            StatusAtoms.Caret(h.Take(16f), true);
            StatusAtoms.ScrollFadeHint(h.Take(40f), true);
            ProgressAtoms.CapacityBar(h.Take(140f), 640f, 1000f, Tokens.Accent);
            if (ProgressAtoms.ZoomIndicator(h.Take(90f), zoom)) zoom = 1f;
            PanelAtoms.FlashOverlay(h.Take(60f), 0.5f, Tokens.Dead);
            PanelAtoms.MiniProfileChip(h.Rest, "小", "小明", 0.8f, 0.6f);
        }

        // =====================================================================================
        // 第 4.3 节 · 图表（纯输出，不读鼠标）
        // =====================================================================================
        private void DrawCharts(Rect area)
        {
            var h = Layout.Row(area, Tokens.SpaceMd);
            Charts.Sparkline(h.Take(160f), series, Tokens.Accent);
            Charts.LineChart(h.Take(220f), series);
            Charts.BarChartV(h.Take(180f), series, new[] { "一", "二", "三", "四", "五", "六" });
            Charts.DonutChart(h.Take(90f), shares, 0.55f, "占比");
            // ⚠️ HeatmapGrid 收的是**二维数组** grid[行,列]（API参考 一度误印为 float[]，已修）
            Charts.HeatmapGrid(h.Rest, new[,] { { 0.1f, 0.4f, 0.9f }, { 0.3f, 0.6f, 0.2f }, { 0.8f, 0.5f, 0.7f } });
        }

        // =====================================================================================
        // 第 4.4 节 · 数据展示
        // =====================================================================================
        private void DrawData(Rect area)
        {
            var h = Layout.Row(area, Tokens.SpaceMd);
            DataAtoms.SkillBars(h.Take(260f), new[] { "射击", "建造", "医疗", "种植" }, new[] { 12f, 8f, 5f, 14f });
            DataAtoms.RankList(h.Take(180f), new[] { "小明", "阿花" }, new[] { 42f, 31f }, 60f);
            DataAtoms.TraitChips(h.Take(200f), new[] { "夜猫子", "嗜血" }, new[] { true, false });
            DataAtoms.StatTable(h.Rest, new[] { "项目", "数值" }, new[] { new[] { "钢材", "640" }, new[] { "木材", "210" } }, new[] { 0.7f, 0.3f }, 20f);
        }

        private void DrawOfficialData(Rect area)
        {
            var h = Layout.Row(area, Tokens.SpaceMd);
            OfficialDataAtoms.RecordsTable(h.Take(240f), new[] { "击杀", "工时" }, new[] { "12", "860" });
            OfficialDataAtomsB.BillList(h.Take(240f), new[] { "烤肉" }, new[] { 3f }, new[] { 10f }, new[] { false });
            OfficialDataAtomsC.MessageFeed(h.Rest, new[] { "袭击已结束" }, new[] { "3 分钟前" });
        }

        // =====================================================================================
        // 第 4.5 节 · 输入控件（全部无状态：值由你持有）
        // =====================================================================================
        private void DrawInputs(Rect area)
        {
            var col = Layout.Column(area, Tokens.SpaceXs);
            var r1 = Layout.Row(col.Take(28f), Tokens.SpaceSm);
            if (InputAtoms.Segmented(r1.Take(210f), new[] { "总览", "库存", "人员" }, ref segIndex)) { }
            if (InputAtoms.ToggleSwitch(r1.Take(56f), ref switchOn)) { }
            if (InputAtoms.Stepper(r1.Take(130f), ref stepper, 0, 10)) { }

            var r2 = Layout.Row(col.Take(24f), Tokens.SpaceSm);
            if (InputAtoms.SliderWithValue(r2.Take(220f), ref amount, 0f, 1f, 0.01f, p => p.ToString("P0"))) { }
            if (InputAtoms.SearchField(r2.Rest, ref query, ref queryBuf, "搜索…", ref lastInputTime)) { }

            var r3 = Layout.Row(col.Rest, Tokens.SpaceSm);
            if (InputAtoms.TagPicker(r3.Take(180f), new[] { "A", "B", "C" }, picked)) { }
            if (InputPlusAtoms.QuickAmounts(r3.Take(140f), "数量", ref numeric, 100f)) { }
            if (InputPlusAtoms.WheelAdjust(r3.Take(120f), ref numeric, 1f, 0f, 100f)) { }
            if (InputPlusAtoms.NumericField(r3.Rest, ref numeric, 0f, 100f, ref numericBuf)) { }
        }

        // =====================================================================================
        // 第 4.6 节 · 导航、表格、反馈
        // =====================================================================================
        private void DrawNavigation(Rect area)
        {
            var col = Layout.Column(area, Tokens.SpaceXs);
            NavAtoms.TabBar(col.Take(30f), new[] { "人物", "物品", "战役" }, ref tab, ref tabX);
            NavAtoms.Breadcrumb(col.Take(22f), new[] { "档案馆", "人物", "小明" }, i => { });
            NavAtoms.Pager(col.Take(24f), 120, 20, ref tab);
            NavAtoms.AnchorDots(col.Take(20f), new[] { "A", "B", "C" }, ref tab);
            NavAtomsB.LayerSwitch(col.Take(24f), new[] { "查看", "编排" }, ref tab);
            Navigation_Parent.Make(col.Take(30f), ref speed);
        }

        private void DrawTables(Rect area)
        {
            var col = Layout.Column(area, Tokens.SpaceXs);
            var head = Layout.Row(col.Take(22f), Tokens.SpaceXs);
            if (TableAtoms.HeaderCell(head.Take(120f), "名称", sort)) sort = TableFmt.NextSort(sort);
            Rect frozen;
            Rect scrollArea;
            TableAtoms.FrozenSplit(area, 140f, out frozen, out scrollArea);
            TableAtoms.ValueCell(frozen, 42f, 100f);
            TableAtoms.ResizeHandle(scrollArea, scrollArea.x + 140f);
            if (TableAtoms.DensitySwitch(col.Take(24f), ref density)) { }
        }

        private void DrawFeedback(Rect area)
        {
            var col = Layout.Column(area, Tokens.SpaceXs);
            FeedbackAtoms.Toast(col.Take(48f), "已记录", "3 条新档案", ToastLevel.Ok, 1f);
            if (FeedbackAtoms.AlertBanner(col.Take(30f), AlertPriority.Urgent, "食物不足", "去处理")) { }
            if (FeedbackAtoms.ConfirmDialog(col.Take(70f), "确认删除？", "不可撤销", true)) { }
            if (FeedbackAtoms.EmptyState(col.Take(60f), "暂无档案", "先去搞点事", "刷新")) { }
            FeedbackAtoms.ProgressOverlay(col.Take(50f), "存档中…", 0.6f, "写入人物档案");
            FeedbackAtomsB.StepProgressList(col.Take(50f), new[] { "扫描", "归档" }, 1);
            FeedbackAtomsB.StickyNote(col.Take(40f), "记得交班", Tokens.Warn);
            if (FeedbackAtomsB.NotificationEntry(col.Take(26f), "新事件", "刚刚", true)) { }
        }

        // =====================================================================================
        // 第 4.7 节 · 工业件与命令面板
        // =====================================================================================
        private void DrawIndustrial(Rect area)
        {
            var h = Layout.Row(area, Tokens.SpaceMd);
            Industrial.Gauge(h.Take(120f), 62f, 0f, 100f, "负载", 0.6f, 0.85f);
            Industrial.LedMatrix(h.Take(160f), new[] { 0, 1, 2, 0 }, 4);
            h.Rest.ToString();
        }

        // ⚠️ `Command` 在 RUIF 与 Verse 里同名 ⇒ 同时 `using RUIF; using Verse;` 时必须写 RUIF.Command（否则 CS0104）
        private void DrawCommandPalette(Rect area, string q, int sel)
        {
            var cmds = new List<RUIF.Command>();
            var filtered = new RUIF.Command[8];
            var scores = new int[8];
            RUIF.Command hit = CommandPalette.Draw(area, cmds.ToArray(), ref q, ref sel, filtered, scores);
            if (hit != null) hit.Run?.Invoke();
        }
    }

    // =====================================================================================
    // 第 5 节 · 三个必踩的坑（正面写法）
    // =====================================================================================
    public class Tab_WithDrawMethod
    {
        // ⚠️ 类内自己定义了 Draw 方法：此时裸写 Draw.Label 会解析到"本类的方法组"（CS0119）。
        // ✅ 正解 = 全限定 RUIF.Draw.*
        public void Draw(Rect r)
        {
            RUIF.Draw.Label(r, "全限定调用", Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
            RUIF.Draw.Box(r, Tokens.Panel);
        }

        // ✅ 颜色一律取令牌，或由令牌派生（禁 new Color / 裸改 GUI.color）
        public void Colors(Rect r)
        {
            Color hover = Tokens.PanelRaised;
            Draw(r);                                       // 本类方法：ok
            RUIF.Draw.Border(r, RUIF.Draw.WithAlpha(hover, 0.5f));
            using (RUIF.Draw.Color(hover)) RUIF.Draw.Box(r, hover);
            using (RUIF.Draw.Font(Tokens.FontValue)) RUIF.Draw.Label(r, "大号", Tokens.FontValue, Tokens.Text, TextAnchor.MiddleCenter);
            using (RUIF.Draw.Anchor(TextAnchor.UpperRight)) RUIF.Draw.Label(r, "右上", Tokens.FontLabel, Tokens.Muted, TextAnchor.UpperRight);
            using (RUIF.Draw.Enabled(false)) Widgets.ButtonText(r, "禁用态按钮");
        }
    }

    // =====================================================================================
    // 第 6 节 · 软依赖：探测走反射，RUIF 调用隔离在独立方法里
    // =====================================================================================
    internal static class RuifProbe
    {
        private static bool? cached;

        internal static bool Available
        {
            get
            {
                if (!cached.HasValue) cached = FindApiType() != null;
                return cached.Value;
            }
        }

        // 只扫类型名字符串，不引用 RUIF 类型 —— 框架没装也不会抛
        private static Type FindApiType()
        {
            foreach (System.Reflection.Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = asm.GetType("RUIF.Api", false);
                if (t != null) return t;
            }
            return null;
        }
    }

    internal static class RuifBridge
    {
        // 所有会触发 RUIF 类型加载的调用都关在这里
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static bool HasApi(string minVersion)
        {
            return Api.IsAvailable(minVersion);
        }
    }

    internal static class Navigation_Parent
    {
        internal static void Make(Rect r, ref TimeSpeed s)
        {
            NavAtomsB.TimeSpeedControl(r, ref s);
        }
    }
}
