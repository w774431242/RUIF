using System;
using UnityEngine;
using Verse;

namespace RUIF.DevTools
{
    /// <summary>
    /// Gallery 演示台：顶栏分类分页器（NavAtoms.Pager）+ 单页演示区（可滚动 + 自绘细滚动条）。
    /// 每次只渲染一个分类（section），组件增减不再撑大左侧索引、也不依赖全局魔数滚动高度（高度两遍测量自动取得）。
    /// 每一节都标注：<b>组名 · 类名 · 已落地/总数</b>，便于对照图鉴核对实现状态。
    /// 注意：本类是调用方，持有演示所需状态；原子组件本身仍无状态。
    /// </summary>
    public static class GalleryContent
    {
        // ---- 分类数据（名称走 Keyed 语言子包；与下方 10 个 Section 一一对应） ----
        private static readonly string[] catKeys =
        {
            "RUIF.UI.Cat.01", "RUIF.UI.Cat.02", "RUIF.UI.Cat.03", "RUIF.UI.Cat.04",
            "RUIF.UI.Cat.05", "RUIF.UI.Cat.06", "RUIF.UI.Cat.07", "RUIF.UI.Cat.08",
            "RUIF.UI.Cat.09", "RUIF.UI.Cat.10"
        };
        private static readonly string[] catCls =
        {
            "RUIF.Theme", "RUIF.*Atoms", "RUIF.Input*", "RUIF.Data*", "RUIF.FeedbackAtoms*",
            "RUIF.NavAtoms*", "M1-M12", "RUIF.DevTools.GalleryPages", "RUIF.Charts", "RUIF.Industrial"
        };
        private static readonly int[] catCount = { 1, 56, 28, 40, 17, 12, 12, 12, 10, 4 };
        private static readonly int[] catDone  = { 1, 42, 28, 40, 15, 12, 12, 12, 10, 4 };

        private static int pagePreset;

        private static int selected = 0;           // 当前页（分类索引，0 基）
        private static int lastSelected = -1;       // 翻页时重置滚动位置，避免旧页滚动量带进新页
        private static Vector2 bodyScroll;

        // 顶栏分类分页器与 SectionNav 内的示例分页器分开持有缓冲，避免互相覆盖
        private static readonly int[] navPageBuf = new int[16];

        // 各 Section 真实高度缓存（两遍测量取得，免全局魔数）
        private static readonly float[] secH = new float[10];
        private static int secMeasured = -2;
        private static float secMeasureW;

        // ---- 表格演示状态 ----
        private static readonly string[] tableHeaders = { "姓名", "工时", "产出" };
        private static readonly string[] rowNames = { "张三", "李四", "王五" };
        private static readonly float[] workHours = { 469f, 402f, 377f };
        private static readonly float[] outputs = { 1284f, 977f, 1102f };
        private static readonly float[] colShares = { 0.34f, 0.33f, 0.33f };
        private static readonly float[] colWidths = new float[3];
        private static readonly SortState[] sorts = new SortState[3];
        private static RowDensity density = RowDensity.Normal;
        private static bool dragging;
        private static float dragStartX, dragStartW;

        // ---- 工业演示数据（T1 四原子；孪生取数走 DataBridge，此处为静态只读演示）----
        private static readonly int[] ledStatuses =
        {
            1, 1, 1, 2, 1, 1, 1, 1, 3, 1,
            1, 2, 1, 1, 1, 1, 0, 1, 1, 1
        };
        private static readonly string[] flowStages = { "采集", "精炼", "组装", "出口" };
        private static readonly float[] flowTimes = { 1.1f, 2.6f, 1.5f, 0.7f };
        private static readonly string[] machineNames = { "发电机 A", "发电机 B", "电精炼台" };
        private static readonly MachineState[][] machineRows =
        {
            new[] { MachineState.Run, MachineState.Refuel, MachineState.Run },
            new[] { MachineState.Run, MachineState.Fault, MachineState.Run, MachineState.Idle },
            new[] { MachineState.Idle, MachineState.Run, MachineState.Fault, MachineState.Run },
        };
        private static readonly float[][] machineDurs =
        {
            new[] { 6.2f, 1.0f, 2.8f },
            new[] { 4.0f, 1.4f, 1.6f, 3.0f },
            new[] { 2.0f, 5.0f, 0.8f, 2.2f },
        };

        // ---- 反馈演示状态（v1.2 接线，替换 Pending 占位）----
        private static bool fbUnread = true;
        private static float fbProgress = 0.6f;

        // ---- 输入控件演示状态 ----
        private static int segIndex = 1;
        private static bool toggleOn = true;
        private static int stepVal = 12;
        private static float sliderVal = 72f;
        private static string searchQuery = string.Empty;
        private static string searchBuf = string.Empty;
        private static float searchLast = -10f;
        private static readonly string[] tags = { "战斗", "生产", "社交" };
        private static readonly bool[] picked = { true, true, false };

        // ---- 数据展示演示数据 ----
        private static readonly string[] skillNames = { "射击", "近战", "建造", "采矿", "烹饪", "艺术" };
        private static readonly float[] skillLevels = { 14f, 6f, 18f, 9f, 11f, 0f };
        private static readonly string[] needNames = { "心情", "饮食", "休息", "娱乐" };
        private static readonly float[] needVals = { 0.82f, 0.64f, 0.91f, 0.23f };
        private static readonly string[] traitNames = { "乐观", "夜猫子", "工作狂" };
        private static readonly bool[] traitPos = { true, false, true };
        private static readonly string[] hpParts = { "左腿", "头部", "全身" };
        private static readonly string[] hpStates = { "开放伤口", "已固定", "营养不良" };
        private static readonly float[] hpSev = { 0.8f, 0.2f, 0.35f };
        private static readonly float[] heatVals =
        {
            0.2f, 0.5f, 0.9f, 0f, 0.4f, 0.7f, 1f, 0.3f, 0.6f, 0.8f, 0.1f, 0.55f, 0.75f, 0.35f,
            0.9f, 0.45f, 0.2f, 0.65f, 0.85f, 0.15f, 0.5f, 0.95f, 0.3f, 0.7f, 0.4f, 0.6f, 0.25f, 0.8f
        };
        private static readonly string[] rankNames = { "张三", "李四", "王五" };
        private static readonly float[] rankVals = { 469f, 402f, 377f };
        private static readonly string[] tblHeaders = { "姓名", "工时", "产出" };
        private static readonly string[][] tblRows =
        {
            new[] { "张三", "469h", "1284" }, new[] { "李四", "402h", "977" }, new[] { "王五", "377h", "1102" }
        };
        private static readonly string[] tlDates = { "5501", "5503", "5505" };
        private static readonly string[] tlTexts = { "加入殖民地", "建造合金工坊", "虫灾夜战（合并 3 事件）" };

        // ---- 输入扩展（B2）演示状态 ----
        private static readonly string[] dragItems = { "精密加工", "枪管加工", "表面处理" };
        private static int dragFrom = -1;
        private static float rangeLo = 20f, rangeHi = 80f;
        private static Color pickColor = new Color(1f, 0.6f, 0.2f);
        private static float pickH, pickS, pickV;
        private static int sortMode;
        private static string editable = "战备物资柜";
        private static bool editing;
        private static string editBuf = string.Empty;
        private static float scrubVal = 42f;
        private static bool scrubbing;
        private static float scrubStartX, scrubStartVal;
        private static readonly string[] prioRows = { "张三", "李四", "王五" };
        private static readonly string[] prioCols = { "灭火", "建造", "采矿", "烹饪" };
        private static readonly int[] prioVals = { 1, 2, 0, 3, 0, 1, 3, 0, 2, 0, 0, 1 };
        private static readonly string[] ttRows = { "张三", "李四" };
        private static readonly int[] ttStates = new int[48];

        // ---- 输入续（B2+）演示状态 ----
        private static float quickVal = 1f, tickVal = 25f, wheelVal = 50f;
        private static float numVal = 12f;
        private static string numBuf = "12";
        private static string pathVal = "C:/RimWorld/导出/";
        private static int dropIdx;
        private static readonly string[] filterLabels = { "姓名", "工时", "产出" };
        private static readonly string[] filterVals = { string.Empty, string.Empty, string.Empty };
        private static readonly string[] msItems = { "张三", "李四", "王五", "赵六" };
        private static readonly bool[] msSel = new bool[4];
        private static int msAnchor = -1;
        private static readonly int[] queueVals = { 0, 1, 2 };
        private static int queueCount = 3;
        private static readonly int[] paintVals = new int[40];
        private static readonly string[] boundKeys = { "F11", "F11", "F1" };   // 故意含重复以演示冲突提示
        private static string mlText = "仓库第二台空调待装";
        private static string mlBuf = "仓库第二台空调待装";

        // ---- 导航（E1/E3）演示状态 ----
        private static readonly string[] navTabs = { "总览", "职业", "社交", "战役", "总账", "设置", "关于" };
        private static int tabSel = 1;
        private static float tabIndX;
        private static string[] crumbPath = { "殖民地档案", "人物", "张三", "生涯工时" };
        private static int pagerPage = 3;
        private static readonly int[] pageBuf = new int[11];
        private static TreeNode treeRoot;
        private static readonly string[] anchorSections = { "概览", "技能", "社会", "战役" };
        private static int anchorCur;
        private static readonly Command[] paletteCmds =
        {
            new Command("晋升：张三 → 正高级", null, "职称"),
            new Command("晋升：李四 → 中级", null, "职称"),
            new Command("打开职称考试", null),
            new Command("导出战役志", null),
            new Command("切换主题 Dark", () => Theme.Apply(Theme.DarkId)),
        };
        private static string palQuery = string.Empty;
        private static int palSel;
        private static readonly Command[] palFiltered = new Command[8];
        private static readonly int[] palScores = new int[8];
        private static string palExec;
        private static readonly string[] viewLayers = { "地图", "世界", "档案" };
        private static int layerSel = 2;
        private static TimeSpeed demoSpeed = TimeSpeed.Normal;
        private static readonly NavAtomsB.NavStack navStack = new NavAtomsB.NavStack();
        private static readonly NavAtomsB.RecentEntry[] recents =
        {
            new NavAtomsB.RecentEntry("张三 · 生涯工时", "人", 5f),
            new NavAtomsB.RecentEntry("虫灾夜战", "役", 310f),
            new NavAtomsB.RecentEntry("总装车间", "厂", 3700f),
        };
        private static bool sidebarOpen = true;
        private static float sidebarRatio = 0.32f;
        private static string focusQuery = "虫灾";

        static GalleryContent()
        {
            // 作息演示：前 16 刻工作，后 8 刻睡眠
            for (int i = 0; i < ttStates.Length; i++) ttStates[i] = (i % 24) < 16 ? 0 : 1;
            // 涂抹演示：预置几个不同优先级
            paintVals[3] = 1; paintVals[9] = 2; paintVals[15] = 3; paintVals[22] = 4;
            // 传承树演示：二代含亡者
            treeRoot = new TreeNode("张氏 开国者") { Tag = "3" };
            treeRoot.Add(new TreeNode("张老 · 初代") { Tag = "18 杀" });
            treeRoot.Add(new TreeNode("张中 · 二代"))
                .Add(new TreeNode("张幼 · 三代") { Tag = "5491 殁", Dead = true });
            var liBranch = new TreeNode("李氏分支") { Tag = "2" };
            liBranch.Add(new TreeNode("李一 · 初代"));
            liBranch.Add(new TreeNode("李二 · 初代") { Tag = "工匠" });
            // 返回栈演示：钻取至末级
            navStack.Push("事件流");
            navStack.Push("张三");
            navStack.Push("生涯工时");
        }

        public static void Draw(Rect inRect)
        {
            RUIF.Draw.Box(inRect, Theme.Window);
            Rect pad = Layout.Inset(inRect, Theme.SpaceMd, Theme.SpaceSm);
            const float barH = 30f;
            Rect bar = new Rect(pad.x, pad.y, pad.width, barH);
            Rect body = new Rect(pad.x, pad.y + barH + Theme.SpaceSm, pad.width,
                Mathf.Max(0f, pad.height - barH - Theme.SpaceSm));
            if (selected != lastSelected)            // 翻页：滚动位置归零，并强制重测高度
            {
                bodyScroll = Vector2.zero;
                lastSelected = selected;
                secMeasured = -2;
            }
            DrawTopBar(bar);
            DrawBody(body);
        }

        // ================= 顶栏：分类分页器 =================
        // 用 NavAtoms.Pager 逐类翻页，每次只渲染一个分类（section），
        // 避免左侧索引随组件增长而膨胀、以及全局长滚动区的脆弱性。
        private static void DrawTopBar(Rect r)
        {
            RUIF.Draw.Box(r, Theme.Panel);
            RUIF.Draw.Border(r, Theme.BorderSoft);
            var row = Layout.Row(Layout.Inset(r, Theme.SpaceSm, 2f), Theme.SpaceXs);
            RUIF.Draw.Label(row.Take(260f),
                catKeys[selected].Translate() + "  ·  " + (selected + 1) + "/" + catKeys.Length,
                Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
            NavAtoms.Pager(row.Take(300f), catKeys.Length, 1, ref selected, navPageBuf);
            RUIF.Draw.Label(row.Rest,
                "分页器逐类浏览，每类单独成页便于调整",
                Theme.FontLabel, Theme.Dim, TextAnchor.MiddleRight);
        }

        // ================= 右侧：演示区（按当前页渲染 + 自绘细滚动条） =================
        private static void DrawBody(Rect r)
        {
            float w = r.width - Theme.ScrollbarThickness;
            float h = SectionHeight(selected, w) + 24f;        // 安全余量：吸收组件对 Take 高度的轻微溢出，避免底部裁切
            Rect view = Layout.BeginScroll(r, h, ref bodyScroll, false);   // 关原生条
            var col = Layout.Column(view, Theme.SpaceMd);
            DrawSection(selected, ref col);
            Layout.EndScroll();
            MiscAtoms.ThemedScrollBar(r, h, bodyScroll.y);                 // 自绘细条
        }

        // 分发：每个分类对应一个 Section（与 catKeys 索引一一对应）
        private static void DrawSection(int idx, ref VStack col)
        {
            switch (idx)
            {
                case 0: SectionTokens(ref col); break;
                case 1: SectionBase(ref col); break;
                case 2: SectionInput(ref col); break;
                case 3: SectionData(ref col); break;
                case 4: SectionFeedback(ref col); break;
                case 5: SectionNav(ref col); break;
                case 6: SectionLayout(ref col); break;
                case 7: SectionPresets(ref col); break;
                case 8: SectionCharts(ref col); break;
                case 9: SectionIndustrial(ref col); break;
            }
        }

        // 两遍测量：先在屏幕外渲染一次取真实高度，避免全局魔数 ContentH，
        // 组件增减后无需手工重估滚动区高度。仅当宽度变化时才重新测量。
        private static float SectionHeight(int idx, float w)
        {
            if (secMeasured == idx && Mathf.Abs(secMeasureW - w) < 0.5f) return secH[idx];
            var col = Layout.Column(new Rect(0f, -100000f, Mathf.Max(1f, w), 1e7f), Theme.SpaceMd);
            DrawSection(idx, ref col);
            float h = col.Y + 100000f;
            secH[idx] = h;
            secMeasured = idx;
            secMeasureW = w;
            return h;
        }

        // ---- 小工具 ----
        private static void Head(ref VStack col, string title, string cls, int done, int count)
        {
            Shapes.RuleWithLabel(col.Take(26f), title, Theme.Accent);
            RUIF.Draw.Label(col.Take(16f), cls + "　已落地 " + done + "/" + count,
                Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
        }

        private static void Info(ref VStack col, string group, string cls, int done, int count)
        {
            var row = Layout.Row(col.Take(16f), Theme.SpaceXs);
            RUIF.Draw.Label(row.Take(150f), group + "　" + done + "/" + count,
                Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
            RUIF.Draw.Label(row.Rest, cls, Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
        }

        // ================= ① 令牌 =================
        private static void SectionTokens(ref VStack col)
        {
            Head(ref col, "① 令牌 Tokens", "RUIF.Theme（Vanilla 默认 / Dark，Register+Apply）", 1, 1);
            Rect row = col.Take(24f);
            var r = Layout.Row(row, Theme.SpaceXs);
            RUIF.Draw.Label(r.Take(96f), "主题：", Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            if (Widgets.ButtonText(r.Take(72f), "Vanilla", true, false, Theme.ActiveId != Theme.VanillaId))
                Theme.Apply(Theme.VanillaId);
            if (Widgets.ButtonText(r.Take(72f), "Dark", true, false, Theme.ActiveId != Theme.DarkId))
                Theme.Apply(Theme.DarkId);
            RUIF.Draw.Label(r.Rest, "当前：" + Theme.ActiveId, Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);

            Rect sw = col.Take(26f);
            var sr = Layout.Row(sw, Theme.SpaceXs);
            VisualAtoms.Swatch(sr.Take(22f), Theme.Accent, true);
            VisualAtoms.Swatch(sr.Take(22f), Theme.Info);
            VisualAtoms.Swatch(sr.Take(22f), Theme.Alive);
            VisualAtoms.Swatch(sr.Take(22f), Theme.Warn);
            VisualAtoms.Swatch(sr.Take(22f), Theme.Dead);
            VisualAtoms.GradientBar(sr.Rest, Theme.Info, Theme.Accent, 18);
        }

        // ================= ② 基础组件 =================
        private static void SectionBase(ref VStack col)
        {
            Head(ref col, "② 基础组件 Base",
                "RUIF.TextAtoms / Shapes / StatusAtoms / VisualAtoms / PanelAtoms / ProgressAtoms / TableAtoms / InteractAtoms / MiscAtoms", 42, 56);

            // 文本原语
            Info(ref col, "文本原语 6", "TextFmt.FormatNumber · TextAtoms.UnitLabel/Typewriter/SubtitleLine/RichTextLine", 6, 6);
            Rect t1 = col.Take(22f);
            var tr = Layout.Row(t1, Theme.SpaceXs);
            RUIF.Draw.Label(tr.Take(160f),
                TextFmt.FormatNumber(12480) + " / " + TextFmt.FormatNumber(284000) + " / " + TextFmt.FormatNumber(2500000),
                Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
            TextAtoms.UnitLabel(tr.Take(90f), "469", "h/天");
            TextAtoms.RichTextLine(tr.Rest, new[]
            {
                new RichSpan("张三 完成了 "), new RichSpan("杰作级", Theme.Accent), new RichSpan(" 突击步枪")
            });
            Rect t2 = col.Take(22f);
            float tw = Mathf.Clamp01(Mathf.Repeat(Time.realtimeSinceStartup * 0.22f, 1.25f));
            var tr2 = Layout.Row(t2, Theme.SpaceXs);
            TextAtoms.Typewriter(tr2.Take(240f), "虫群自地下涌来，围墙在震颤……", tw, Theme.FontBody, Theme.Text);
            TextAtoms.SubtitleLine(tr2.Rest, "总工程师", "Chief Engineer · T4");

            // 几何与装饰
            Info(ref col, "几何与装饰 6", "Shapes.CornerFrame / GradientRule / RuleWithLabel / Watermark / InsetPanel", 6, 6);
            Rect g1 = col.Take(32f);
            var gr = Layout.Row(g1, Theme.SpaceSm);
            Shapes.CornerFrame(gr.Take(70f), Theme.Accent);
            Shapes.InsetPanel(gr.Take(70f));
            Shapes.Watermark(gr.Rest, "RUIF", 0.22f);
            Shapes.GradientRule(col.Take(6f), Theme.Accent, 16);
            Shapes.RuleWithLabel(col.Take(16f), "记录", Theme.BorderSoft);

            // 状态原子
            Info(ref col, "状态原子 5", "StatusAtoms.PulseDot / Glyph / Spinner / Caret / ScrollFadeHint", 5, 5);
            Rect s1 = col.Take(26f);
            var sr = Layout.Row(s1, Theme.SpaceXs);
            StatusAtoms.PulseDot(sr.Take(12f), Theme.Alive, Time.realtimeSinceStartup);
            StatusAtoms.PulseDot(sr.Take(12f), Theme.Warn, Time.realtimeSinceStartup * 1.6f);
            StatusAtoms.PulseDot(sr.Take(12f), Theme.Dead, Time.realtimeSinceStartup * 0.8f);
            StatusAtoms.Glyph(sr.Take(18f), StatusKind.Ok);
            StatusAtoms.Glyph(sr.Take(18f), StatusKind.Warn);
            StatusAtoms.Glyph(sr.Take(18f), StatusKind.Error);
            StatusAtoms.Glyph(sr.Take(18f), StatusKind.Paused);
            StatusAtoms.Spinner(sr.Take(24f), Time.realtimeSinceStartup);
            StatusAtoms.Caret(sr.Take(16f), false, Theme.Accent);
            StatusAtoms.Caret(sr.Take(16f), true, Theme.Accent);
            StatusAtoms.ScrollFadeHint(sr.Rest, true);

            // 布局视觉
            Info(ref col, "布局视觉 6", "Atoms.AspectBox / Atoms.Anchor / VisualAtoms.Swatch/GradientBar/PinBadge/OverflowFade", 6, 6);
            Rect v1 = col.Take(28f);
            var vr = Layout.Row(v1, Theme.SpaceXs);
            Rect card = vr.Take(130f);
            RUIF.Draw.Box(card, Theme.Card);
            RUIF.Draw.Border(card, Theme.BorderSoft);
            VisualAtoms.PinBadge(card, true);
            RUIF.Draw.Label(new Rect(card.x + 6f, card.y, card.width - 12f, card.height),
                "已钉选", Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
            VisualAtoms.OverflowFade(vr.Rest,
                "超长的殖民地事件描述在行尾优雅淡出，不需要省略号",
                Theme.Window, Theme.FontLabel, Theme.Text, 30f, 10);

            // 面板原子
            Info(ref col, "面板原子 4", "PanelAtoms.StatRow / MiniProfileChip / FlashOverlay / SectionBar", 4, 4);
            PanelAtoms.StatRow(col.Take(20f), "饥饿", "64% · 晚膳 0.4h 后", 0.64f, Theme.Alive);
            PanelAtoms.StatRow(col.Take(20f), "休息", "91% · 22:00 就寝");
            PanelAtoms.MiniProfileChip(col.Take(32f), "张", "张三", 0.82f, 0.64f);
            PanelAtoms.SectionBar(col.Take(20f), "战斗履历", "6 项", true);
            Rect fo = col.Take(32f);
            RUIF.Draw.Box(fo, Theme.Card);
            PanelAtoms.FlashOverlay(fo, Time.realtimeSinceStartup);
            RUIF.Draw.Label(new Rect(fo.x + 6f, fo.y, fo.width - 12f, fo.height),
                "! 边框脉动：临近精神崩溃", Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);

            // 进度容量
            Info(ref col, "进度容量 3", "ProgressAtoms.FillWipe / CapacityBar / ZoomIndicator", 3, 3);
            Rect p1 = col.Take(24f);
            var pr = Layout.Row(p1, Theme.SpaceXs);
            ProgressAtoms.FillWipe(pr.Take(90f), Mathf.Clamp01(Mathf.Repeat(Time.realtimeSinceStartup * 0.22f, 1.25f)), Theme.Accent);
            ProgressAtoms.CapacityBar(pr.Take(90f), 660f, 1000f);
            ProgressAtoms.CapacityBar(pr.Take(90f), 1100f, 1000f);      // 超载
            ProgressAtoms.ZoomIndicator(pr.Rest, 0.86f);

            // 表格原子
            Info(ref col, "表格原子 5", "TableAtoms.HeaderCell / ValueCell / ResizeHandle / FrozenSplit / DensitySwitch", 5, 5);
            DrawTableSection(col.Take(150f));

            // 交互原子
            Info(ref col, "交互原子 4", "InteractAtoms.GripHandle / EyeToggle / SearchHighlight / ToolbarIcon", 4, 4);
            Rect i1 = col.Take(26f);
            var ir = Layout.Row(i1, Theme.SpaceXs);
            InteractAtoms.GripHandle(ir.Take(18f));
            InteractAtoms.EyeToggle(ir.Take(22f), true);
            InteractAtoms.EyeToggle(ir.Take(22f), false);
            InteractAtoms.SearchHighlight(ir.Take(180f), "张三 建造 18 建造师", "建造", Theme.FontLabel, Theme.Accent);
            InteractAtoms.ToolbarIcon(ir.Take(26f), "≡", true);
            InteractAtoms.ToolbarIcon(ir.Take(26f), "×");

            // 杂项原子
            Info(ref col, "杂项原子 3", "MiscAtoms.PointCostBadge / BubbleTag / ThemedScrollBar（左侧条即实例）", 3, 3);
            Rect m1 = col.Take(30f);
            var mr = Layout.Row(m1, Theme.SpaceXs);
            MiscAtoms.PointCostBadge(mr.Take(96f), 10, 4);
            MiscAtoms.PointCostBadge(mr.Take(96f), 10, 12);
            MiscAtoms.BubbleTag(mr.Take(96f), "没饭了");
            MiscAtoms.BubbleTag(mr.Rest, "想睡");
        }

        // ---- 表格演示（可拖列宽 / 可排序 / 密度切换） ----
        private static void DrawTableSection(Rect rect)
        {
            var col = Layout.Column(rect, 4f);
            Rect headRow = col.Take(20f);
            Rect densRow = col.Take(22f);

            TableFmt.ColumnSplit(headRow, colShares, 0f, colWidths);
            float x = headRow.x;
            for (int i = 0; i < tableHeaders.Length; i++)
            {
                var cell = new Rect(x, headRow.y, colWidths[i], headRow.height);
                if (TableAtoms.HeaderCell(cell, tableHeaders[i], sorts[i]))
                    sorts[i] = TableFmt.NextSort(sorts[i]);
                x += colWidths[i];
            }
            RUIF.Draw.LineH(headRow.x, headRow.yMax - 1f, headRow.width, Theme.BorderSoft);

            float rh = TableFmt.RowHeight(density);
            for (int r = 0; r < rowNames.Length; r++)
            {
                Rect row = col.Take(rh);
                if (r % 2 == 1) RUIF.Draw.Box(row, RUIF.Draw.WithAlpha(Theme.Text, 0.04f));
                var rrow = Layout.Row(row, 0f);
                RUIF.Draw.Label(rrow.Take(colWidths[0]), rowNames[r], Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
                TableAtoms.ValueCell(rrow.Take(colWidths[1]), workHours[r], 500f);
                TableAtoms.ValueCell(rrow.Rest, outputs[r], 1300f);
            }

            float handleX = headRow.x + colWidths[0];
            if (TableAtoms.ResizeHandle(new Rect(headRow.x, headRow.y, headRow.width, rect.height), handleX))
            {
                dragging = true;
                dragStartX = Event.current.mousePosition.x;
                dragStartW = colWidths[0];
            }
            if (dragging)
            {
                if (Event.current.type == EventType.MouseUp) dragging = false;
                else
                {
                    colWidths[0] = TableFmt.ResizeWidth(dragStartW, Event.current.mousePosition.x - dragStartX, 60f, 220f);
                    colShares[0] = colWidths[0] / Mathf.Max(1f, headRow.width);
                }
            }

            TableAtoms.DensitySwitch(densRow, ref density);
        }

        // ================= ③-⑥ 待落地分类（说明 + 组件清单） =================
        private static void Pending(ref VStack col, string title, string cls, string stage, string[] comps)
        {
            Head(ref col, title, cls + "（" + stage + " 落地）", 0, comps.Length);
            for (int i = 0; i < comps.Length; i++)
                RUIF.Draw.Label(col.Take(16f), "· " + comps[i], Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
        }

        private static void SectionInput(ref VStack col)
        {
            Head(ref col, "③ 输入控件 Input", "RUIF.InputAtoms（基础输入 8 已落地；B2 / B2+ 待 S3 后续）", 8, 28);
            Info(ref col, "基础输入 8", "InputAtoms.Segmented / ToggleSwitch / Stepper / SliderWithValue / SearchField / TagPicker / HotkeyBind / StateSample", 8, 8);

            Rect r1 = col.Take(26f);
            var row = Layout.Row(r1, Theme.SpaceSm);
            InputAtoms.Segmented(row.Take(152f), new[] { "日", "周", "季", "年" }, ref segIndex);
            InputAtoms.ToggleSwitch(row.Take(86f), ref toggleOn);
            InputAtoms.Stepper(row.Take(112f), ref stepVal, 0, 20);
            InputAtoms.HotkeyBind(row.Rest, "打开画廊", "F11");

            Rect r2 = col.Take(30f);
            InputAtoms.SliderWithValue(r2, ref sliderVal, 0f, 250f, 1f, v => v.ToString("0") + " 件");

            Rect r3 = col.Take(24f);
            var row3 = Layout.Row(r3, Theme.SpaceSm);
            InputAtoms.SearchField(row3.Take(180f), ref searchQuery, ref searchBuf, "搜索…", ref searchLast);
            InteractAtoms.SearchHighlight(row3.Rest, "张三 建造 18 · 建造师 · 李四 建造 12",
                string.IsNullOrEmpty(searchQuery) ? "建造" : searchQuery, Theme.FontLabel, Theme.Accent);

            Rect r4 = col.Take(24f);
            InputAtoms.TagPicker(r4, tags, picked);

            // 六态规范
            Rect r5 = col.Take(24f);
            var st = Layout.Row(r5, 4f);
            float w = (r5.width - 20f) / 6f;
            InputAtoms.StateSample(st.Take(w), "默认", InputState.Default);
            InputAtoms.StateSample(st.Take(w), "悬停", InputState.Hover);
            InputAtoms.StateSample(st.Take(w), "按下", InputState.Pressed);
            InputAtoms.StateSample(st.Take(w), "选中", InputState.Selected);
            InputAtoms.StateSample(st.Take(w), "禁用", InputState.Disabled);
            InputAtoms.StateSample(st.Rest, "焦点", InputState.Focused);

            SectionInputEx(ref col);

            SectionInputPlus(ref col);

            Info(ref col, "输入 28 件", "B1 基础输入 8 + B2 扩展 8 + B2+ 续 12 —— 全部落地（⚠️ 两项为简化版：MultiLineEditor 多行、HotkeyBind 改键捕获，留 v1.1）", 28, 28);
        }

        /// <summary>B2+ 输入续 12 件演示。</summary>
        private static void SectionInputPlus(ref VStack col)
        {
            Info(ref col, "输入续 12", "InputPlusAtoms.QuickAmounts / SliderTicks / WheelAdjust / NumericField / PathField / InPlaceDropdown / FilterRow / MultiSelectList / QueueEditor / PaintStrokeGrid / KeyConflictHint / MultiLineEditor", 12, 12);

            Rect r1 = col.Take(26f);
            var a = Layout.Row(r1, Theme.SpaceXs);
            InputPlusAtoms.QuickAmounts(a.Take(240f), "数量", ref quickVal, 250f);
            InputPlusAtoms.SliderTicks(a.Take(230f), ref tickVal, new[] { 0f, 25f, 50f, 75f, 100f }, 0f, 100f);
            InputPlusAtoms.WheelAdjust(a.Rest, ref wheelVal, 5f, 0f, 100f);

            Rect r2 = col.Take(26f);
            var b = Layout.Row(r2, Theme.SpaceXs);
            InputPlusAtoms.NumericField(b.Take(150f), ref numVal, 0f, 250f, ref numBuf);
            InputPlusAtoms.PathField(b.Take(220f), ref pathVal);
            InputPlusAtoms.InPlaceDropdown(b.Rest, "下拉", new[] { "按工时", "按产出", "按评级" }, dropIdx, i => dropIdx = i);

            RUIF.Draw.Label(col.Take(14f), "列过滤（FilterRow）", Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            InputPlusAtoms.FilterRow(col.Take(26f), filterLabels, filterVals);

            Rect r4 = col.Take(100f);
            var c = Layout.Row(r4, Theme.SpaceSm);
            InputPlusAtoms.MultiSelectList(c.Take(260f), msItems, msSel, ref msAnchor, 23f);
            InputPlusAtoms.QueueEditor(c.Rest, queueVals, ref queueCount, new[] { "突击步枪", "电荷步枪", "重型狙击" }, 26f);

            RUIF.Draw.Label(col.Take(14f), "涂抹网格（PaintStroke · Work Tab 实证 · 点击循环）",
                Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            if (InputPlusAtoms.PaintStrokeGrid(col.Take(90f), 5, 8, paintVals, out int pi))
                paintVals[pi] = InputExFmt.NextPriority(paintVals[pi]);

            InputPlusAtoms.KeyConflictHint(col.Take(24f), boundKeys);

            RUIF.Draw.Label(col.Take(14f), "多行编辑（MultiLineEditor ⚠️ v1 简化：按行拆分单行框）",
                Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            InputPlusAtoms.MultiLineEditor(col.Take(74f), ref mlText, ref mlBuf, 4);
        }

        /// <summary>B2 输入扩展 8 件演示（拖拽列表 / 范围滑条 / 取色器 / 循环钮 / 行内编辑 / 数值刷 / 优先级网格 / 作息矩阵）。</summary>
        private static void SectionInputEx(ref VStack col)
        {
            Info(ref col, "输入扩展 8", "InputExAtoms.DragList / DoubleSlider / ColorPicker / CycleButton / EditableLabel / NumberScrubber / PriorityGrid / TimetableMatrix", 8, 8);

            // 拖拽列表
            Rect dl = col.Take(66f);
            float rowH = 20f;
            var rows = Layout.Column(dl, 2f);
            for (int i = 0; i < dragItems.Length; i++)
            {
                Rect row = rows.Take(rowH);
                InputExAtoms.DragListRow(row, dragItems[i], dragFrom == i, false);
                if (InputExAtoms.GripHit(row)) dragFrom = i;
            }
            if (dragFrom >= 0 && Event.current.type == EventType.MouseUp)
            {
                int to = InputExFmt.DropIndex(dl, dragItems.Length, rowH + 2f, Event.current.mousePosition.y);
                if (to >= 0 && to != dragFrom) InputExFmt.Reorder(dragItems, dragFrom, to);
                dragFrom = -1;
            }

            // 范围滑条 + 取色器
            Rect ds = col.Take(52f);
            var dsr = Layout.Row(ds, Theme.SpaceSm);
            InputExAtoms.DoubleSlider(dsr.Take(230f), ref rangeLo, ref rangeHi, 0f, 100f, 1f);
            InputExAtoms.ColorPicker(dsr.Rest, ref pickColor, ref pickH, ref pickS, ref pickV);

            // 循环钮 + 行内编辑 + 数值刷
            Rect ce = col.Take(26f);
            var cer = Layout.Row(ce, Theme.SpaceXs);
            InputExAtoms.CycleButton(cer.Take(180f), "排序", new[] { "按工时", "按产出", "按评级" }, ref sortMode);
            InputExAtoms.EditableLabel(cer.Take(170f), ref editable, ref editing, ref editBuf);
            InputExAtoms.NumberScrubber(cer.Rest, ref scrubVal, 0.5f, 0f, 100f, ref scrubbing, ref scrubStartX, ref scrubStartVal);

            // 优先级网格（Work Tab 实证）
            RUIF.Draw.Label(col.Take(14f), "优先级网格（Work Tab 实证 · 点击循环 1-4）",
                Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            if (InputExAtoms.PriorityGrid(col.Take(80f), prioRows, prioCols, prioVals, out int pi))
                prioVals[pi] = InputExFmt.NextPriority(prioVals[pi]);

            // 作息矩阵（Work Tab 实证）
            RUIF.Draw.Label(col.Take(14f), "作息矩阵（Work Tab 实证 · 点击切换）",
                Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            if (InputExAtoms.TimetableMatrix(col.Take(54f), ttRows, ttStates, out int ti))
                ttStates[ti] = InputExFmt.NextTimetableState(ttStates[ti]);
        }

        // ---- C2 数据扩展演示状态 ----
        private static readonly string[] gLabels = { "基础制造", "金属加工", "合金学", "装配线", "电气化" };
        private static readonly int[] gCol = { 0, 0, 1, 2, 3 };
        private static readonly int[] gRow = { 0, 1, 1, 1, 1 };
        private static readonly int[] gFrom = { 0, 1, 2, 3 };
        private static readonly int[] gTo = { 2, 2, 3, 4 };
        private static readonly int[] gState = { 1, 1, 0, 0, 0 };
        private static int selNode = -1;
        private static bool dockCollapsed;
        private static int presetIdx;
        private static readonly string[] presetNames = { "全员施工", "战时体制", "休闲日" };
        private static readonly string[] questObj = { "护送至据点", "全员生还", "货物损失 ≤20%" };
        private static readonly string[] roomNames = { "印象", "清洁", "空间", "美观" };
        private static readonly float[] roomVals = { 0.7f, 0.35f, 0.9f, 0.6f };
        private static readonly string[] wLabels = { "今日", "明日", "后日", "第4日", "第5日" };
        private static readonly int[] wKinds = { 0, 1, 2, 3, 4 };
        private static readonly string[] storeLabels = { "食品", "材料", "药品", "武器" };
        private static readonly float[] storeVals = { 131f, 108f, 45f, 60f };

        private static void SectionDataEx(ref VStack col)
        {
            Info(ref col, "C2 数据扩展 12", "DataExAtoms.NodeGraph / DockHost / ProfilePreset / BuffRow / QuestCard / FactionCard / CreatureCard / RoomStats / PowerGrid / WeatherStrip / StorageBreakdown / CompareBars", 12, 12);

            Rect a = col.Take(120f);
            var ar = Layout.Row(a, Theme.SpaceSm);
            DataExAtoms.NodeGraph(ar.Take(520f), gLabels, gCol, gRow, gFrom, gTo, gState, ref selNode, 96f, 28f, 12f);
            DataExAtoms.DockHost(ar.Rest, "面板", ref dockCollapsed);

            DataExAtoms.ProfilePreset(col.Take(24f), presetNames, ref presetIdx, out bool _);
            DataExAtoms.BuffRow(col.Take(18f), "吃了美味食物", 8f);
            DataExAtoms.BuffRow(col.Take(18f), "疼痛（左腿）", -12f);

            Rect c = col.Take(86f);
            var cr = Layout.Row(c, Theme.SpaceXs);
            DataExAtoms.QuestCard(cr.Take(230f), "商队护卫", 2, 3, questObj);
            DataExAtoms.FactionCard(cr.Take(180f), "伯利部落", 78f);
            DataExAtoms.CreatureCard(cr.Rest, "土豆", 3, true);

            Rect d = col.Take(66f);
            var dr = Layout.Row(d, Theme.SpaceSm);
            DataExAtoms.RoomStats(dr.Take(230f), roomNames, roomVals);
            DataExAtoms.PowerGrid(dr.Rest, 1420f, 1130f, 2280f, 3000f);

            DataExAtoms.WeatherStrip(col.Take(30f), wLabels, wKinds);
            DataExAtoms.StorageBreakdown(col.Take(40f), storeLabels, storeVals, new[] { Theme.Accent, Theme.Info, Theme.Alive, Theme.Warn });
            DataExAtoms.CompareBars(col.Take(56f), "方案A 走廊防御", "方案B 外墙迎击", 74f, 91f, 100f);
        }

        private static void SectionData(ref VStack col)

        {
            Head(ref col, "④ 数据展示 Data", "RUIF.DataAtoms（C1 纯 ✅ 9 已落地；⚠️ 与 C2/C2+ 待 S3 后续）", 9, 40);
            Info(ref col, "C1 数据组件 9", "PawnCard / SkillBars / NeedsPanel / TraitChips / HealthRows / HeatCalendar / RankList / StatTable / TimelinePro", 9, 14);

            Rect a = col.Take(56f);
            var ar = Layout.Row(a, Theme.SpaceSm);
            DataAtoms.PawnCard(ar.Take(250f), "张", "张三", "总工程师", 0.82f, 0.64f, "在岗");
            DataAtoms.SkillBars(ar.Rest, skillNames, skillLevels, 20f, 2);

            Rect b = col.Take(74f);
            var br = Layout.Row(b, Theme.SpaceSm);
            DataAtoms.NeedsPanel(br.Take(230f), needNames, needVals);
            DataAtoms.HealthRows(br.Rest, hpParts, hpStates, hpSev);

            Rect c = col.Take(24f);
            DataAtoms.TraitChips(c, traitNames, traitPos);

            Rect d = col.Take(62f);
            var dr = Layout.Row(d, Theme.SpaceSm);
            DataAtoms.HeatCalendar(dr.Take(230f), heatVals, 14, 11f, 3f);
            DataAtoms.RankList(dr.Rest, rankNames, rankVals, 500f);

            Rect e = col.Take(84f);
            DataAtoms.StatTable(e, tblHeaders, tblRows, new[] { 0.34f, 0.33f, 0.33f }, 20f);

            Rect f = col.Take(72f);
            DataAtoms.TimelinePro(f, tlDates, tlTexts, new[] { Theme.Alive, Theme.Accent, Theme.Dead }, 22f);

            Pending(ref col, "⚠️ 与 C2 / C2+ 待落地", "RUIF.Components", "S3 后续 / v1.1",
                new[]
                {
                    "⚠️ 需素材或原型：MoodGauge 弧形 / GearSlots / InventoryGrid / RelationGraph / Charts",
                    "C2 数据扩展 12：NodeGraph✅ / DockHost✅ / ProfilePreset✅ / BuffRow / QuestCard / FactionCard / CreatureCard / RoomStats / PowerGrid / WeatherStrip / StorageBreakdown / CompareBars",
                    "C2+ 官方数据系统 14：WealthBreakdown / RecordsTable / HistoryFeed / TaleCard / LetterCard / BillList / QuestBoard / TitleCard / PsycastTree / IdeoCard / CaravanPanel / GeneCard / MessageFeed / PolicyCard"
                });
        }

        private static void SectionFeedback(ref VStack col)
        {
            Head(ref col, "⑤ 反馈 Feedback", "RUIF.FeedbackAtoms / FeedbackAtomsB（D1 核心 + 交互扩展 + 官方消息语义）", 15, 17);

            Info(ref col, "D1 反馈核心 6", "ToastStack / AlertBanner / ConfirmDialog / RichTooltip / ProgressOverlay / EmptyState", 6, 6);

            // Toast 三态（左条色随级别）
            Rect t1 = col.Take(46f);
            var tr = Layout.Row(t1, Theme.SpaceSm);
            FeedbackAtoms.Toast(tr.Take(120f), "建造完成", "卧室 ×1 已就绪", ToastLevel.Ok, 1f);
            FeedbackAtoms.Toast(tr.Take(120f), "库存偏低", "简单餐 ×12", ToastLevel.Warn, 1f);
            FeedbackAtoms.Toast(tr.Rest, "殖民地失火", "立即扑救", ToastLevel.Danger, 1f);

            // 警报条（可点击）+ 确认框（可交互）+ 富提示
            Rect t2 = col.Take(64f);
            var fr = Layout.Row(t2, Theme.SpaceSm);
            FeedbackAtoms.AlertBanner(fr.Take(170f), AlertPriority.Warn, "电池电量不足", "前往");
            FeedbackAtoms.ConfirmDialog(fr.Take(190f), "拆除仓库？", "将移除全部存储物", true);
            FeedbackAtoms.RichTooltip(fr.Rest, "精英动力甲",
                new[] { "护甲-锋利 62%", "移动 +0.12c" }, new[] { "需要 动力甲研究" });

            // 进度遮罩 + 空状态
            Rect t3 = col.Take(56f);
            var pr = Layout.Row(t3, Theme.SpaceSm);
            FeedbackAtoms.ProgressOverlay(pr.Take(150f), "研究中…", fbProgress, "微电子 3/5");
            FeedbackAtoms.EmptyState(pr.Rest, "暂无信件", "殖民地风平浪静", "打开历史");

            Info(ref col, "交互扩展 / 官方消息语义 9", "NotificationEntry / UndoToast / StickyNote / AlertBlinker / CriticalOverlay / StepProgressList / InlineError / BatchConfirm / ActionFeedback", 9, 11);

            // 通知条目 + 撤销 Toast + 便签
            Rect t4 = col.Take(44f);
            var nr = Layout.Row(t4, Theme.SpaceSm);
            if (FeedbackAtomsB.NotificationEntry(nr.Take(160f), "陌生人到达", "1 小时前", fbUnread))
                fbUnread = false;
            FeedbackAtomsB.UndoToast(nr.Take(140f), "已删除 账单 ×1", 1f);
            FeedbackAtomsB.StickyNote(nr.Rest, "记得接通电池！", Theme.Warn);

            // 警报脉冲三速 + 步骤列表 + 行内错误（可交互）
            Rect t5 = col.Take(40f);
            var br = Layout.Row(t5, Theme.SpaceSm);
            FeedbackAtomsB.AlertBlinker(br.Take(26f), Time.time, AlertPriority.Urgent);
            FeedbackAtomsB.AlertBlinker(br.Take(26f), Time.time, AlertPriority.Normal);
            FeedbackAtomsB.AlertBlinker(br.Take(26f), Time.time, AlertPriority.Low);
            RUIF.Draw.Label(br.Take(84f), "急/常/低", Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
            FeedbackAtomsB.StepProgressList(br.Take(120f), new[] { "选址", "搬运", "建造" }, 1);
            FeedbackAtomsB.InlineError(br.Rest, "电力不足：+200W", "扩容");

            // 批量确认摘要（可交互）
            Rect t6 = col.Take(44f);
            FeedbackAtomsB.BatchConfirm(t6, "清理 12 件物品",
                FeedbackFmt.BatchSummary(new[] { "服饰", "石块", "肉类" }, new[] { 6, 4, 2 }));
        }

        private static void SectionIndustrial(ref VStack col)
        {
            Head(ref col, "⑩ 工业 Industrial", "RUIF.Industrial / IndustrialFmt（v1.2 T1 四原子 · 数字孪生表达层 · 零贴图）", 4, 4);

            Info(ref col, "T1 原子 4", "Gauge / LEDMatrix / BottleneckRibbon / MachineStateTimeline —— 官方数据面见 工业管理方向扩展设计提案.md（DataBridge 动手前先接口冻结评审）", 4, 4);

            // Gauge ×3：正常 / 警告 / 危险分区
            Rect g1 = col.Take(100f);
            var gr = Layout.Row(g1, Theme.SpaceSm);
            Industrial.Gauge(gr.Take(150f), 68f, 0f, 100f, "发电负载 PowerNet");
            Industrial.Gauge(gr.Take(150f), 74f, 0f, 100f, "警告阈值示例");
            Industrial.Gauge(gr.Rest, 96f, 0f, 100f, "危险阈值示例");

            // LEDMatrix：20 格（含熄/正常/警告/故障四态）
            Info(ref col, "灯阵", "Industrial.LedMatrix（一格一机，颜色即状态）", 1, 1);
            Rect l1 = col.Take(64f);
            RUIF.Draw.Box(l1, Theme.Card);
            Industrial.LedMatrix(Layout.Inset(l1, 4f, 4f), ledStatuses, 10);

            // BottleneckRibbon：四段流程，精炼为瓶颈
            Rect b1 = col.Take(64f);
            Industrial.BottleneckRibbon(b1, flowStages, flowTimes);

            // MachineStateTimeline：三机 × 24h 状态段（故障段描边）
            Rect m1 = col.Take(96f);
            Industrial.MachineStateTimeline(m1, machineNames, machineRows, machineDurs);
        }

        private static void SectionNav(ref VStack col)
        {
            Head(ref col, "⑥ 导航 Nav", "RUIF.NavAtoms / NavAtomsB / CommandPalette + NavFmt（纯计算）", 12, 12);

            Info(ref col, "E1 导航核心 6", "TabBar / Breadcrumb / Pager / CollapsibleTree / AnchorDots / CommandPalette⚠️", 6, 6);

            NavAtoms.TabBar(col.Take(26f), navTabs, ref tabSel, ref tabIndX);

            Rect t2 = col.Take(22f);
            NavAtoms.Breadcrumb(t2, crumbPath, i =>
            {
                // 点任意上级 → 截断为其末级（模拟回退钻取）
                if (i >= 0 && i < crumbPath.Length - 1)
                {
                    var list = new System.Collections.Generic.List<string>(crumbPath);
                    list.RemoveRange(i + 1, list.Count - i - 1);
                    crumbPath = list.ToArray();
                }
            });

            NavAtoms.Pager(col.Take(26f), 267, 30, ref pagerPage, pageBuf);

            Rect t4 = col.Take(130f);
            var tr = Layout.Row(t4, Theme.SpaceSm);
            Rect treeRect = tr.Take(300f);
            RUIF.Draw.Box(treeRect, Theme.Card);
            NavAtoms.CollapsibleTree(treeRect, treeRoot);
            Rect anchorHost = tr.Rest;
            RUIF.Draw.Box(anchorHost, Theme.Card);
            NavAtoms.AnchorDots(anchorHost, anchorSections, ref anchorCur);

            Rect t5 = col.Take(140f);
            var executed = CommandPalette.Draw(t5, paletteCmds, ref palQuery, ref palSel, palFiltered, palScores);
            if (executed != null) palExec = executed.Label;
            RUIF.Draw.Label(col.Take(16f),
                palExec == null ? "（↑↓ 选择 · 回车 执行 · Esc 失焦；⚠️ 全局 Ctrl+K 仲裁留 v1.1）" : "已执行：" + palExec,
                Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);

            Info(ref col, "E3 时间·域·返回栈 6", "TimeSpeedControl / LayerSwitch / NavBackBar / JumpList / SidebarCollapse / FocusSearch", 6, 6);

            Rect e1 = col.Take(26f);
            var er = Layout.Row(e1, Theme.SpaceSm);
            NavAtomsB.TimeSpeedControl(er.Take(160f), ref demoSpeed);
            NavAtomsB.LayerSwitch(er.Rest, viewLayers, ref layerSel);

            NavAtomsB.NavBackBar(col.Take(24f), navStack, "生涯工时");

            Rect e3 = col.Take(80f);
            var jr = Layout.Row(e3, Theme.SpaceSm);
            NavAtomsB.JumpList(jr.Take(240f), recents, 8);
            Rect sideDemo = jr.Rest;
            const float HandleW = 12f;
            Rect inner = Layout.Inset(sideDemo, 3f, 3f);
            sidebarRatio = NavFmt.Approach(sidebarRatio, sidebarOpen ? 0.32f : 0.055f, 0.2f);
            Layout.Split(inner, sidebarRatio, 0f, out Rect sideCol, out Rect contentCol);
            RUIF.Draw.Box(contentCol, Theme.Window);
            RUIF.Draw.Label(Layout.Inset(contentCol, 10f, 0f),
                sidebarOpen ? "内容区（侧栏展开）" : "内容区（侧栏已收起）",
                Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
            if (sidebarOpen && sideCol.width > HandleW + 24f)
            {
                Rect sideBody = new Rect(sideCol.x, sideCol.y, sideCol.width - HandleW, sideCol.height);
                RUIF.Draw.Box(sideBody, Theme.Panel);
                RUIF.Draw.Border(sideBody, Theme.BorderSoft);
                RUIF.Draw.Label(sideBody, "侧栏", Theme.FontLabel, Theme.Muted, TextAnchor.MiddleCenter);
            }
            NavAtomsB.SidebarCollapse(new Rect(sideCol.xMax - HandleW, sideCol.y, HandleW, sideCol.height), ref sidebarOpen);

            Rect e4 = col.Take(24f);
            bool searchFocused = NavAtomsB.FocusSearch(e4, ref focusQuery);
            RUIF.Draw.Label(col.Take(16f),
                searchFocused ? "搜索框已聚焦（Esc 归还焦点）" : "本窗口内按 Ctrl+F 聚焦页内搜索",
                Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
        }

        // ================= ⑦ 版式模式 / ⑧ 页面预设 =================
        private static void SectionLayout(ref VStack col)
        {
            Head(ref col, "⑦ 版式模式 Layout", "M1-M12（设计完成，详析见 F+ 深挖）", 12, 12);
            string[] ms =
            {
                "M1 主从布局（Split 0.32 · Numbers 实证）", "M2 三段仪表盘（KPI+主区+三联）",
                "M3 抽屉钻取（右侧 44% 滑入）", "M4 向导分步（步条+固定高内容）",
                "M5 卡片瀑布（2/3/4 列断点）", "M6 指挥台 HUD（状态带+主区+命令栏）",
                "M7 调度工作台（Work Tab 原型）", "M8 A/B 对比（中轴 VS+差异行）",
                "M9 图谱工作区（ResearchPal 原型）", "M10 层叠工作区（RimHUD 原型）",
                "M11 时间机（粒度+密度+视口）", "M12 教练模式（挖孔+步进+跳过）"
            };
            for (int i = 0; i < ms.Length; i++)
                RUIF.Draw.Label(col.Take(16f), "· " + ms[i], Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
        }

        private static void SectionPresets(ref VStack col)
        {
            Head(ref col, "⑧ 页面预设 Presets", "RUIF.DevTools.GalleryPages（P1-P12 由已落地组件拼装复刻）", 12, 12);

            // 12 预设选择钮（标题走 Keyed）
            Rect chooser = col.Take(56f);
            var grid = Layout.Grid(chooser, 6, 26f, 4f);
            for (int i = 0; i < GalleryPages.Count; i++)
            {
                Rect b = grid.Next();
                if (Widgets.ButtonText(b, GalleryPages.Title(i), true, false, pagePreset != i))
                    pagePreset = i;
            }
            RUIF.Draw.Label(col.Take(14f),
                "页面验收：搭不起页面 = 组件不合格 —— 每页均由 S2/S3/S4 组件拼装",
                Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);

            GalleryPages.Draw(pagePreset, col.Take(400f));
        }

        // ================= ⑨ 图表 =================

        // ---- 图表演示数据（v1.1 Charts 批次；静态只读，无逐帧分配） ----
        private static readonly float[] chartSpark = { 3f, 5f, 4f, 8f, 7f, 10f, 9f, 12f };
        private static readonly float[] chartBarV = { 469f, 402f, 377f, 331f, 296f };
        private static readonly string[] chartBarVLabels = { "张三", "李四", "王五", "赵六", "钱七" };
        private static readonly float[] chartBarH = { 0.9f, 0.6f, 0.75f, 0.4f };
        private static readonly string[] chartBarHLabels = { "心情", "饮食", "休息", "娱乐" };
        private static readonly float[][] chartStacked =
        {
            new[] { 8f, 6f, 9f, 7f },    // 系列0：建造
            new[] { 5f, 7f, 4f, 8f },    // 系列1：采矿
            new[] { 3f, 2f, 6f, 4f }     // 系列2：烹饪
        };
        private static readonly float[] chartLine = { 12f, 18f, 15f, 26f, 22f, 31f, 28f, 36f };
        private static readonly float[] chartArea = { 5f, 9f, 7f, 14f, 12f, 19f, 16f, 23f };
        private static readonly float[] chartDonut = { 34f, 27f, 22f, 17f };
        private static readonly Vector2[] chartScatter =
        {
            new Vector2(0.15f, 0.30f), new Vector2(0.30f, 0.55f), new Vector2(0.42f, 0.38f),
            new Vector2(0.55f, 0.72f), new Vector2(0.62f, 0.50f), new Vector2(0.78f, 0.85f),
            new Vector2(0.85f, 0.62f), new Vector2(0.92f, 0.78f)
        };
        private static readonly float[,] chartHeat =
        {
            { 0.9f, 0.4f, 0.2f, 0.0f, 0.1f, 0.3f, 0.6f, 0.8f },
            { 0.7f, 0.8f, 0.5f, 0.2f, 0.0f, 0.1f, 0.3f, 0.6f },
            { 0.3f, 0.6f, 0.9f, 0.7f, 0.3f, 0.0f, 0.1f, 0.2f },
            { 0.0f, 0.2f, 0.5f, 0.8f, 0.6f, 0.3f, 0.1f, 0.0f }
        };

        private static void SectionCharts(ref VStack col)
        {
            Head(ref col, "⑨ 图表 Charts", "RUIF.ChartsFmt / Charts（v1.1 T0+T1 · 共享 Nice 轴 · 零贴图）", 10, 10);

            Info(ref col, "走势 1", "Charts.Sparkline（局部幅度自适应）", 1, 1);
            Charts.Sparkline(col.Take(36f), chartSpark, dots: true);

            Info(ref col, "柱状 2", "Charts.BarChartV / BarChartH（共享 Nice 网格）", 2, 2);
            Charts.BarChartV(col.Take(96f), chartBarV, chartBarVLabels);
            Charts.BarChartH(col.Take(70f), chartBarH, chartBarHLabels);

            Info(ref col, "堆叠柱 1", "Charts.StackedBarChartV（系列色 = Theme.Series）", 1, 1);
            Charts.StackedBarChartV(col.Take(96f), chartStacked);

            Info(ref col, "折线/面积 2", "Charts.LineChart / AreaChart（Area 为阶梯近似，见类注释取舍）", 2, 2);
            Charts.LineChart(col.Take(72f), chartLine);
            Charts.AreaChart(col.Take(72f), chartArea);

            Info(ref col, "环形/饼 2", "Charts.DonutChart / PieChart（旋转短弧拼环，中心 KPI）", 2, 2);
            Rect donutRow = col.Take(110f);
            var dr = Layout.Row(donutRow, Theme.SpaceMd);
            Charts.DonutChart(dr.Take(110f), chartDonut, 0.5f, "68%");
            Charts.PieChart(dr.Take(110f), chartDonut);
            RUIF.Draw.Label(dr.Rest, "环/饼同源：Pie = Donut(hole=0)\n系列色 Theme.Series(0..n) 令牌循环",
                Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);

            Info(ref col, "散点 1", "Charts.ScatterChart（数据坐标系 domain/range）", 1, 1);
            Charts.ScatterChart(col.Take(96f), chartScatter, new Vector2(0f, 1f), new Vector2(0f, 1f));

            Info(ref col, "热力 1", "Charts.HeatmapGrid（Card→Alive 令牌插值，白名单③）", 1, 1);
            Charts.HeatmapGrid(col.Take(88f), chartHeat);
        }
    }
}