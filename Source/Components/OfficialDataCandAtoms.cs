using UnityEngine;

namespace RUIF
{
    /// <summary>
    /// ⑮ 官方数据候选组件（A/B/C 三族）的绘制入口与参数对象。
    /// <para>候选 readonly struct 同构于 DataCand：嵌套于本 *CandAtoms 静态类内（参见
    /// <see cref="DataCandAtoms"/> 的 BillEntry / FilterNode / TradeRow）；纯计算 Build 工厂则在
    /// <see cref="OfficialDataCandFmt"/>（同构 DataCandFmt：仅纯计算、无 struct 定义）。
    /// 每个候选对应一个 <c>Draw(Rect, XxxCand)</c> 单入口，内部<b>逐字段转发</b>到既有
    /// <see cref="OfficialDataAtoms"/> / <see cref="OfficialDataAtomsB"/> / <see cref="OfficialDataAtomsC"/>
    /// 对应方法，零绘制逻辑复制（GATE-ARCH：候选层复用 Atoms/Fmt，禁止复制绘制）。</para>
    /// <para>契约（GATE-UI）：U1 readonly struct 参数对象；U2 单入口 Draw(Rect, cand)；U3 纯展示零实例状态；
    /// U4 只消费传入 rect（行高/布局全在既有 Atoms 内）；U5 列表型空数据兜底由既有 Atoms 内部原样保留
    /// （空数组 / 长度不齐时 Atoms 直接 return，不做二次断言）。</para>
    /// </summary>
    public static class OfficialDataCandAtoms
    {
        // ============ A 族：财富 / 记录 / 历史 / 传说（→ OfficialDataAtoms） ============

        /// <summary>财富构成候选（官方 WealthWatcher 语义：分段堆叠 + 总额 + 图例）。</summary>
        public readonly struct WealthBreakdownCand
        {
            public readonly string[] labels;
            public readonly float[] values;
            public readonly Color[] tones;
            public WealthBreakdownCand(string[] labels, float[] values, Color[] tones)
            {
                this.labels = labels;
                this.values = values;
                this.tones = tones;
            }
        }

        /// <summary>记录表候选（官方 Pawn_RecordsTracker 语义：键值两列表）。</summary>
        public readonly struct RecordsTableCand
        {
            public readonly string[] labels;
            public readonly string[] values;
            public RecordsTableCand(string[] labels, string[] values)
            {
                this.labels = labels;
                this.values = values;
            }
        }

        /// <summary>历史事件流候选（日期头 + 事件行 + 类型 pill）。kinds 为已本地化的类型文案。</summary>
        public readonly struct HistoryFeedCand
        {
            public readonly string[] dates;
            public readonly string[] texts;
            public readonly string[] kinds;
            public HistoryFeedCand(string[] dates, string[] texts, string[] kinds)
            {
                this.dates = dates;
                this.texts = texts;
                this.kinds = kinds;
            }
        }

        /// <summary>传说卡候选（官方 Tale 系统：讲述者 + 叙述 + 流传次数）。</summary>
        public readonly struct TaleCardCand
        {
            public readonly string teller;
            public readonly string text;
            public readonly int times;
            public TaleCardCand(string teller, string text, int times)
            {
                this.teller = teller;
                this.text = text;
                this.times = times;
            }
        }

        // ============ B 族：信件 / 账单 / 任务 / 头衔 / 心能（→ OfficialDataAtomsB） ============

        /// <summary>信件归档卡候选（官方 LetterStack 语义：类型色条 + 标题 + 摘要 + 时间）。</summary>
        public readonly struct LetterCardCand
        {
            public readonly string title;
            public readonly string summary;
            public readonly string time;
            public readonly Color tone;
            public LetterCardCand(string title, string summary, string time, Color tone)
            {
                this.title = title;
                this.summary = summary;
                this.time = time;
                this.tone = tone;
            }
        }

        /// <summary>账单队列候选（官方 BillStack 语义：配方 + 完成/目标 + 暂停态）。</summary>
        public readonly struct BillListCand
        {
            public readonly string[] names;
            public readonly float[] done;
            public readonly float[] target;
            public readonly bool[] paused;
            public BillListCand(string[] names, float[] done, float[] target, bool[] paused)
            {
                this.names = names;
                this.done = done;
                this.target = target;
                this.paused = paused;
            }
        }

        /// <summary>任务板候选（官方 QuestScriptDef 语义：缩略卡 + 进度条 + 剩余时间）。</summary>
        public readonly struct QuestBoardCand
        {
            public readonly string[] titles;
            public readonly float[] progress;
            public readonly float[] daysLeft;
            public QuestBoardCand(string[] titles, float[] progress, float[] daysLeft)
            {
                this.titles = titles;
                this.progress = progress;
                this.daysLeft = daysLeft;
            }
        }

        /// <summary>头衔卡候选（官方 RoyalTitle 语义：名称 + 授予时间 + 许可勾叉）。</summary>
        public readonly struct TitleCardCand
        {
            public readonly string name;
            public readonly string granted;
            public readonly string[] permits;
            public readonly bool[] ok;
            public TitleCardCand(string name, string granted, string[] permits, bool[] ok)
            {
                this.name = name;
                this.granted = granted;
                this.permits = permits;
                this.ok = ok;
            }
        }

        /// <summary>心能树候选（官方 Psycast/Ability 语义：分层节点，已习得实心 / 锁定暗）。</summary>
        public readonly struct PsycastTreeCand
        {
            public readonly string[] names;
            public readonly int[] levels;
            public readonly bool[] unlocked;
            public PsycastTreeCand(string[] names, int[] levels, bool[] unlocked)
            {
                this.names = names;
                this.levels = levels;
                this.unlocked = unlocked;
            }
        }

        // ============ C 族：文化 / 商队 / 基因 / 消息 / 政策（→ OfficialDataAtomsC） ============

        /// <summary>文化卡候选（官方 Ideo/Precept 语义：名称 + 信条清单 + 领袖 + 成员数）。</summary>
        public readonly struct IdeoCardCand
        {
            public readonly string name;
            public readonly string leader;
            public readonly int members;
            public readonly string[] precepts;
            public IdeoCardCand(string name, string leader, int members, string[] precepts)
            {
                this.name = name;
                this.leader = leader;
                this.members = members;
                this.precepts = precepts;
            }
        }

        /// <summary>商队面板候选（官方 Caravan 语义：成员 + 负重 + 口粮 + ETA）。</summary>
        public readonly struct CaravanPanelCand
        {
            public readonly string[] members;
            public readonly float load01;
            public readonly float foodDays;
            public readonly string eta;
            public CaravanPanelCand(string[] members, float load01, float foodDays, string eta)
            {
                this.members = members;
                this.load01 = load01;
                this.foodDays = foodDays;
                this.eta = eta;
            }
        }

        /// <summary>基因芯片候选（官方 GeneDef 语义：类别色条 + 复杂度点数）。</summary>
        public readonly struct GeneChipsCand
        {
            public readonly string[] genes;
            public readonly int[] complexity;
            public readonly bool[] positive;
            public readonly float gap;
            public GeneChipsCand(string[] genes, int[] complexity, bool[] positive, float gap)
            {
                this.genes = genes;
                this.complexity = complexity;
                this.positive = positive;
                this.gap = gap;
            }
        }

        /// <summary>瞬息消息流候选（官方 Messages 语义：不打断、按时间排序）。</summary>
        public readonly struct MessageFeedCand
        {
            public readonly string[] lines;
            public readonly string[] times;
            public MessageFeedCand(string[] lines, string[] times)
            {
                this.lines = lines;
                this.times = times;
            }
        }

        /// <summary>政策卡候选（官方 Policies 语义：名称 + 摘要 + 应用人数）。</summary>
        public readonly struct PolicyCardCand
        {
            public readonly string name;
            public readonly string summary;
            public readonly int applied;
            public PolicyCardCand(string name, string summary, int applied)
            {
                this.name = name;
                this.summary = summary;
                this.applied = applied;
            }
        }

        // ============ 绘制入口：逐字段转发到既有 Atoms（U1-U5 不变） ============

        // ---- A 族 → OfficialDataAtoms ----

        /// <summary>财富构成：转发至 OfficialDataAtoms.WealthBreakdown。</summary>
        public static void Build(Rect rect, WealthBreakdownCand cand)
            => OfficialDataAtoms.WealthBreakdown(rect, cand.labels, cand.values, cand.tones);

        /// <summary>记录表：转发至 OfficialDataAtoms.RecordsTable。</summary>
        public static void Build(Rect rect, RecordsTableCand cand)
            => OfficialDataAtoms.RecordsTable(rect, cand.labels, cand.values);

        /// <summary>历史事件流：转发至 OfficialDataAtoms.HistoryFeed。</summary>
        public static void Build(Rect rect, HistoryFeedCand cand)
            => OfficialDataAtoms.HistoryFeed(rect, cand.dates, cand.texts, cand.kinds);

        /// <summary>传说卡：转发至 OfficialDataAtoms.TaleCard。</summary>
        public static void Build(Rect rect, TaleCardCand cand)
            => OfficialDataAtoms.TaleCard(rect, cand.teller, cand.text, cand.times);

        // ---- B 族 → OfficialDataAtomsB ----

        /// <summary>信件归档卡：转发至 OfficialDataAtomsB.LetterCard。</summary>
        public static void Build(Rect rect, LetterCardCand cand)
            => OfficialDataAtomsB.LetterCard(rect, cand.title, cand.summary, cand.time, cand.tone);

        /// <summary>账单队列：转发至 OfficialDataAtomsB.BillList。</summary>
        public static void Build(Rect rect, BillListCand cand)
            => OfficialDataAtomsB.BillList(rect, cand.names, cand.done, cand.target, cand.paused);

        /// <summary>任务板：转发至 OfficialDataAtomsB.QuestBoard。</summary>
        public static void Build(Rect rect, QuestBoardCand cand)
            => OfficialDataAtomsB.QuestBoard(rect, cand.titles, cand.progress, cand.daysLeft);

        /// <summary>头衔卡：转发至 OfficialDataAtomsB.TitleCard。</summary>
        public static void Build(Rect rect, TitleCardCand cand)
            => OfficialDataAtomsB.TitleCard(rect, cand.name, cand.granted, cand.permits, cand.ok);

        /// <summary>心能树：转发至 OfficialDataAtomsB.PsycastTree。</summary>
        public static void Build(Rect rect, PsycastTreeCand cand)
            => OfficialDataAtomsB.PsycastTree(rect, cand.names, cand.levels, cand.unlocked);

        // ---- C 族 → OfficialDataAtomsC ----

        /// <summary>文化卡：转发至 OfficialDataAtomsC.IdeoCard。</summary>
        public static void Build(Rect rect, IdeoCardCand cand)
            => OfficialDataAtomsC.IdeoCard(rect, cand.name, cand.leader, cand.members, cand.precepts);

        /// <summary>商队面板：转发至 OfficialDataAtomsC.CaravanPanel。</summary>
        public static void Build(Rect rect, CaravanPanelCand cand)
            => OfficialDataAtomsC.CaravanPanel(rect, cand.members, cand.load01, cand.foodDays, cand.eta);

        /// <summary>基因芯片：转发至 OfficialDataAtomsC.GeneChips（gap 默认值已在 Build 处溯源）。</summary>
        public static void Build(Rect rect, GeneChipsCand cand)
            => OfficialDataAtomsC.GeneChips(rect, cand.genes, cand.complexity, cand.positive, cand.gap);

        /// <summary>瞬息消息流：转发至 OfficialDataAtomsC.MessageFeed。</summary>
        public static void Build(Rect rect, MessageFeedCand cand)
            => OfficialDataAtomsC.MessageFeed(rect, cand.lines, cand.times);

        /// <summary>政策卡：转发至 OfficialDataAtomsC.PolicyCard。</summary>
        public static void Build(Rect rect, PolicyCardCand cand)
            => OfficialDataAtomsC.PolicyCard(rect, cand.name, cand.summary, cand.applied);
    }
}
