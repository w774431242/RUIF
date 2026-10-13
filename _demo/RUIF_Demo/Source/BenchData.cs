using RUIF;
using Verse;
using UnityEngine;

namespace RUIF.Demo
{
    /// <summary>
    /// 分类「数据」· 实机渲染验证面（IBench）。
    ///
    /// <para>覆盖 data 域全部 27 件：候选层 <c>DataCandAtoms</c> 6 件 +
    /// 原生 <c>DataAtoms</c> 9 件 + 原生 <c>DataExAtoms</c> 12 件。</para>
    ///
    /// <para>组件零状态、全部由调用方喂数据（GATE-UI）。需要调用方持有并在跨帧复用的
    /// 可变状态（候选层树展开/勾选、节点图选中、停靠折叠、预设档位）以实例字段持有。</para>
    ///
    /// <para>布局：<see cref="Layout.Column"/> 垂直栈，每段 = 段标题(18) + 组件矩形；
    /// <see cref="Height"/> 与 <see cref="Render"/> 共用同一套段高常量，避免两者漂移。</para>
    /// </summary>
    public sealed class BenchData : IBench
    {
        // ── 段高（与 Height / Render 共用）───────────────────────────────
        private const float Gap = 10f;
        private const float HeadH = 22f;

        // 段高顺序必须与 Render 中调用顺序严格一致：
        // 候选层 6 → DataAtoms 9 → DataExAtoms 12
        //
        // ⚠ 2026-10-05 实机取证：索引 5（TradePanel）原来给 170f，而它是「行高 = rect.height / n」
        //   型组件（双栏各自等分高度）⇒ 3 行被撑到 45px/行，实机看着三行之间空一大片。
        //   改为内容自然高：(表头 22 + 3 行 × 24 + 上下内缩 16) ≈ 110。
        private static readonly float[] BodyHs =
        {
            120f, 160f, 110f, 120f, 110f, 110f,   // 候选层 6
            90f, 150f, 150f, 44f, 150f, 130f, 150f, 150f, 170f, // DataAtoms 9
            220f, 150f, 44f, 44f, 120f, 90f, 90f, 130f, 60f, 50f, 60f, 70f  // DataExAtoms 12
        };

        // ── 候选层 · 示例数据（含需跨帧复用的可变状态）────────────────────
        private readonly DataCandAtoms.BillEntry[] bills =
        {
            new DataCandAtoms.BillEntry { label = "钢铁长剑", done = 3, target = 10, priority = true, paused = false },
            new DataCandAtoms.BillEntry { label = "零部件", done = 40, target = 50, priority = false, paused = false },
            new DataCandAtoms.BillEntry { label = "土枪", done = 0, target = 0, priority = false, paused = true },
            new DataCandAtoms.BillEntry { label = "砖块", done = 120, target = 200, priority = false, paused = false },
        };

        private readonly DataCandAtoms.FilterNode[] filterNodes =
        {
            new DataCandAtoms.FilterNode { label = "全部物品", depth = 0, hasChildren = true },
            new DataCandAtoms.FilterNode { label = "食物", depth = 1, hasChildren = true },
            new DataCandAtoms.FilterNode { label = "生肉", depth = 2, hasChildren = false },
            new DataCandAtoms.FilterNode { label = "蔬菜", depth = 2, hasChildren = false },
            new DataCandAtoms.FilterNode { label = "原材料", depth = 1, hasChildren = false },
        };
        private readonly bool[] filterExpanded = { true, true, false, false, false };
        private readonly bool[] filterChecks = { true, false, true, false, false };

        private readonly string[] resourceCats = { "钢铁", "食物", "木材", "石头", "零部件", "药品" };
        private readonly int[] resourceCounts = { 320, 140, 80, 260, 55, 22 };

        private readonly string[] infoLines = { "健康 92%", "心情 64%", "技能 · 射击 14" };

        private readonly string[] storageLabels = { "钢铁", "食物", "木材", "石头", "零部件" };
        private readonly int[] storageCounts = { 320, 140, 80, 260, 55 };

        private readonly DataCandAtoms.TradeRow[] tradeMine =
        {
            new DataCandAtoms.TradeRow { label = "钢铁", count = 320, tone = Tokens.Alive },
            new DataCandAtoms.TradeRow { label = "食物", count = 140, tone = Tokens.Info },
            new DataCandAtoms.TradeRow { label = "药品", count = 22, tone = Tokens.Warn },
        };
        private readonly DataCandAtoms.TradeRow[] tradeTheirs =
        {
            new DataCandAtoms.TradeRow { label = "黄金", count = 60, tone = Tokens.Accent },
            new DataCandAtoms.TradeRow { label = "布料", count = 200, tone = Tokens.Info },
            new DataCandAtoms.TradeRow { label = "奴隶", count = 1, tone = Tokens.Warn },
        };

        // ── DataAtoms · 示例数据 ────────────────────────────────────────
        private readonly string[] skillNames = { "射击", "种植", "医疗", "建造", "烹饪", "社交" };
        private readonly float[] skillLevels = { 14f, 8f, 11f, 6f, 5f, 3f };
        private readonly string[] needNames = { "心情", "饮食", "休息", "娱乐", "美观" };
        private readonly float[] needVals = { 0.64f, 0.80f, 0.50f, 0.30f, 0.70f };
        private readonly string[] traitNames = { "勤劳", "急性子", "素食", "神射手" };
        private readonly bool[] traitPos = { true, false, true, true };
        private readonly string[] healthParts = { "头部", "躯干", "腿部", "手臂" };
        private readonly string[] healthStates = { "健康", "擦伤", "扭伤", "健康" };
        private readonly float[] healthSev = { 0f, 0.2f, 0.4f, 0f };
        private readonly float[] heatVals =
        {
            0.1f, 0.4f, 0.7f, 0.9f, 0.2f, 0.5f, 0.8f, 0.3f, 0.6f, 0.1f,
            0.4f, 0.7f, 0.9f, 0.2f, 0.5f, 0.8f, 0.3f, 0.6f, 0.1f, 0.4f,
            0.7f, 0.9f, 0.2f, 0.5f, 0.8f, 0.3f, 0.6f, 0.1f, 0.4f, 0.7f
        };
        private readonly string[] rankNames = { "蓝 雪牛", "赤 眼镜蛇", "灰狼", "旅鼠" };
        private readonly float[] rankVals = { 92f, 80f, 55f, 30f };
        private readonly string[] statHeaders = { "名称", "等级", "状态" };
        private readonly string[][] statRows =
        {
            new[] { "蓝 雪牛", "14", "良好" },
            new[] { "赤 眼镜蛇", "9", "一般" },
            new[] { "灰狼", "6", "受伤" },
        };
        private readonly float[] statShares = { 0.5f, 0.2f, 0.3f };
        private readonly string[] tlDates = { "春1", "春5", "春12", "夏3" };
        private readonly string[] tlTexts = { "建立营地", "首战告捷", "迎来丰收", "与商队结盟" };
        private readonly Color[] tlKinds = { Tokens.Accent, Tokens.Info, Tokens.Alive, Tokens.Warn };

        // ── DataExAtoms · 示例数据（含需跨帧复用的可变状态）──────────────
        private readonly string[] nodeLabels = { "研究A", "研究B", "研究C", "研究D" };
        private readonly int[] nodeCol = { 0, 1, 0, 1 };
        private readonly int[] nodeRow = { 0, 0, 1, 1 };
        private readonly int[] nodeFrom = { 0, 1 };
        private readonly int[] nodeTo = { 2, 3 };
        private readonly int[] nodeState = { 1, 1, 0, 0 };
        private readonly string[] presetNames = { "默认", "极简", "表演", "挑战" };
        private readonly string[] questObj = { "交付 120 钢铁", "交付 40 药品", "保持 3 天无袭击" };
        private readonly string[] roomNames = { "美观", "清洁", "空间", "温度" };
        private readonly float[] roomMetrics = { 0.72f, 0.88f, 0.54f, 0.61f };
        private readonly string[] weatherLabels = { "一", "二", "三", "四", "五", "六", "日" };
        private readonly int[] weatherKinds = { 0, 0, 1, 1, 3, 2, 0 };
        private readonly string[] storeLabels = { "钢铁", "零部件", "食物", "药品" };
        private readonly float[] storeVals = { 0.42f, 0.27f, 0.21f, 0.10f };
        private readonly Color[] storeTones = { Tokens.Info, Tokens.Accent, Tokens.Alive, Tokens.Warn };

        // 跨帧可变的选中/折叠/档位状态（归调用方持有）
        private int nodeSel;
        private bool dockCollapsed;
        private int presetIndex;

        // ── IBench ───────────────────────────────────────────────────────
        /// <inheritdoc />
        public bool Scrollable => true;

        /// <inheritdoc />
        public float Height(float width)
        {
            float h = 0f;
            for (int i = 0; i < BodyHs.Length; i++) h += BodyHs[i] + HeadH + Gap;
            return h;
        }

        /// <inheritdoc />
        public void Render(Rect area)
        {
            var col = Layout.Column(area, Gap);

            // ───────────────── 候选层 · DataCandAtoms（6）─────────────────
            // 1 · BillList：先有 BillEntry[]（Build 形态即直接喂数组）
            DataCandAtoms.BillList(Section(ref col, "DataCandAtoms · BillList 账单队列", 120f), bills);

            // 2 · ThingFilterTree：FilterNode[] + 展开态 + 勾选态（跨帧复用）
            DataCandAtoms.ThingFilterTree(Section(ref col, "DataCandAtoms · ThingFilterTree 物品过滤器树", 160f),
                filterNodes, filterExpanded, filterChecks);

            // 3 · ResourceReadout
            DataCandAtoms.ResourceReadout(Section(ref col, "DataCandAtoms · ResourceReadout 资源读取", 110f),
                resourceCats, resourceCounts);

            // 4 · InfoCard
            DataCandAtoms.InfoCard(Section(ref col, "DataCandAtoms · InfoCard 信息卡", 120f),
                "殖民者 · 蓝 雪牛", "猎人 / 医师", infoLines);

            // 5 · StorageContents
            DataCandAtoms.StorageContents(Section(ref col, "DataCandAtoms · StorageContents 存储内容", 110f),
                storageLabels, storageCounts);

            // 6 · TradePanel：我方 / 商队双栏 TradeRow[]
            DataCandAtoms.TradePanel(Section(ref col, "DataCandAtoms · TradePanel 贸易面板", 170f),
                tradeMine, tradeTheirs);

            // ───────────────── 原生 · DataAtoms（9）──────────────────────
            // 7 · PawnCard
            DataAtoms.PawnCard(Section(ref col, "DataAtoms · PawnCard 人物卡", 90f),
                "雪", "蓝 雪牛", "猎人", 0.64f, 0.92f, "健康");

            // 8 · SkillBars
            DataAtoms.SkillBars(Section(ref col, "DataAtoms · SkillBars 技能条组", 150f),
                skillNames, skillLevels);

            // 9 · NeedsPanel
            DataAtoms.NeedsPanel(Section(ref col, "DataAtoms · NeedsPanel 需求面板", 150f),
                needNames, needVals);

            // 10 · TraitChips
            DataAtoms.TraitChips(Section(ref col, "DataAtoms · TraitChips 特性芯片", 44f),
                traitNames, traitPos);

            // 11 · HealthRows
            DataAtoms.HealthRows(Section(ref col, "DataAtoms · HealthRows 健康行", 150f),
                healthParts, healthStates, healthSev);

            // 12 · HeatCalendar
            DataAtoms.HeatCalendar(Section(ref col, "DataAtoms · HeatCalendar 热力日历", 130f),
                heatVals, 10);

            // 13 · RankList
            DataAtoms.RankList(Section(ref col, "DataAtoms · RankList 排行榜", 150f),
                rankNames, rankVals, 100f);

            // 14 · StatTable（5 参兼容重载）
            DataAtoms.StatTable(Section(ref col, "DataAtoms · StatTable 统计表", 150f),
                statHeaders, statRows, statShares);

            // 15 · TimelinePro
            DataAtoms.TimelinePro(Section(ref col, "DataAtoms · TimelinePro 时间轴", 170f),
                tlDates, tlTexts, tlKinds);

            // ───────────────── 原生 · DataExAtoms（12）────────────────────
            // 16 · NodeGraph（ref selected 跨帧复用）
            DataExAtoms.NodeGraph(Section(ref col, "DataExAtoms · NodeGraph 节点图", 220f),
                nodeLabels, nodeCol, nodeRow, nodeFrom, nodeTo, nodeState, ref nodeSel);

            // 17 · DockHost（ref collapsed 跨帧复用）
            DataExAtoms.DockHost(Section(ref col, "DataExAtoms · DockHost 停靠宿主", 150f),
                "殖民者面板", ref dockCollapsed);

            // 18 · ProfilePreset（ref index + out save）
            bool presetSave;
            DataExAtoms.ProfilePreset(Section(ref col, "DataExAtoms · ProfilePreset 配置档案", 44f),
                presetNames, ref presetIndex, out presetSave);

            // 19 · BuffRow
            DataExAtoms.BuffRow(Section(ref col, "DataExAtoms · BuffRow 增减益行", 44f),
                "心情 · 舒适卧室", 8f);

            // 20 · QuestCard
            DataExAtoms.QuestCard(Section(ref col, "DataExAtoms · QuestCard 任务卡", 120f),
                "断供的商队", 2, 5, questObj);

            // 21 · FactionCard
            DataExAtoms.FactionCard(Section(ref col, "DataExAtoms · FactionCard 势力卡", 90f),
                "流浪者商队", 68f);

            // 22 · CreatureCard
            DataExAtoms.CreatureCard(Section(ref col, "DataExAtoms · CreatureCard 生物卡", 90f),
                "蓝 雪牛", 4, true);

            // 23 · RoomStats
            DataExAtoms.RoomStats(Section(ref col, "DataExAtoms · RoomStats 房间属性", 130f),
                roomNames, roomMetrics);

            // 24 · PowerGrid
            DataExAtoms.PowerGrid(Section(ref col, "DataExAtoms · PowerGrid 电力概览", 60f),
                1820f, 2480f, 6400f, 8000f);

            // 25 · WeatherStrip
            DataExAtoms.WeatherStrip(Section(ref col, "DataExAtoms · WeatherStrip 天气条带", 50f),
                weatherLabels, weatherKinds);

            // 26 · StorageBreakdown
            DataExAtoms.StorageBreakdown(Section(ref col, "DataExAtoms · StorageBreakdown 库存构成", 60f),
                storeLabels, storeVals, storeTones);

            // 27 · CompareBars
            DataExAtoms.CompareBars(Section(ref col, "DataExAtoms · CompareBars 双列对比条", 70f),
                "本季产出", "上季产出", 1240f, 860f, 2000f);
        }

        /// <summary>取一段「段标题 + 组件矩形」；标题画在顶 18px，组件矩形占剩余 bodyH。</summary>
        private static Rect Section(ref VStack col, string title, float bodyH)
        {
            Rect block = col.Take(bodyH + HeadH);
            Draw.Label(new Rect(block.x, block.y, block.width, 18f), title,
                Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            return new Rect(block.x, block.y + HeadH, block.width, bodyH);
        }
    }
}
