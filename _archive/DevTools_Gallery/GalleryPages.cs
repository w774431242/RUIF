using UnityEngine;
using Verse;

namespace RUIF.DevTools
{
    /// <summary>
    /// S5 · P1-P12 页面级预设复刻（验收方式：页面由已落地组件拼装，搭不起 = 组件不合格）。
    /// 本类只做组装与演示状态持有；不新增任何组件能力。
    /// 页面标题走 Keyed（RUIF.UI.Preset.*）；页内演示数据为示例内容，不属于框架文案。
    /// </summary>
    public static class GalleryPages
    {
        public const int Count = 12;
        private const float HeadH = 26f;

        /// <summary>页面标题（Keyed）。</summary>
        public static string Title(int idx)
            => ("RUIF.UI.Preset." + (idx + 1).ToString("00")).Translate();

        public static void Draw(int idx, Rect rect)
        {
            switch (idx)
            {
                case 0: P1Governor(rect); break;
                case 1: P2Dossier(rect); break;
                case 2: P3Battle(rect); break;
                case 3: P4Legacy(rect); break;
                case 4: P5Workshop(rect); break;
                case 5: P6Settings(rect); break;
                case 6: P7WorkTab(rect); break;
                case 7: P8Research(rect); break;
                case 8: P9Trade(rect); break;
                case 9: P10Power(rect); break;
                case 10: P11Medbay(rect); break;
                default: P12Docking(rect); break;
            }
        }

        // ---- 页面公共件 ----

        /// <summary>页头：左侧 accent 竖条 + 标题，右侧灰色注记。</summary>
        private static void PageHead(Rect rect, int idx, string note)
        {
            RUIF.Draw.Box(rect, Theme.Panel);
            RUIF.Draw.Border(rect, Theme.BorderSoft);
            RUIF.Draw.LineV(rect.x, rect.y, rect.height, Theme.Accent);
            var row = Layout.Row(Layout.Inset(rect, 8f, 2f), 6f);
            RUIF.Draw.Label(row.Take(300f), Title(idx), Theme.FontBody, Theme.Text, TextAnchor.MiddleLeft);
            RUIF.Draw.Label(row.Rest, note, Theme.FontLabel, Theme.Dim, TextAnchor.MiddleRight);
        }

        /// <summary>卡片容器：Card 底 + 细边，返回内缩后的内容区。</summary>
        private static Rect Card(Rect rect, string title = null)
        {
            RUIF.Draw.Box(rect, Theme.Card);
            RUIF.Draw.Border(rect, Theme.BorderHair);
            if (string.IsNullOrEmpty(title)) return Layout.Inset(rect, 6f, 4f);
            var col = Layout.Column(rect, 3f);
            RUIF.Draw.LineH(rect.x, rect.y + 18f, rect.width, Theme.BorderHair);
            RUIF.Draw.Label(new Rect(rect.x + 6f, rect.y + 2f, rect.width - 12f, 16f),
                title, Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            col.Take(19f);
            return col.Rest;
        }

        /// <summary>KPI 徽章带（标签 + 数值 + 涨跌）。</summary>
        private static void KpiRow(Rect rect, string[] labels, string[] values, Color[] tones)
        {
            var row = Layout.Row(rect, Theme.SpaceXs);
            float w = (rect.width - Theme.SpaceXs * (labels.Length - 1)) / labels.Length;
            for (int i = 0; i < labels.Length; i++)
            {
                var cell = row.Take(w);
                RUIF.Draw.Box(cell, Theme.Card);
                RUIF.Draw.Border(cell, Theme.BorderHair);
                var col = Layout.Column(Layout.Inset(cell, 7f, 2f), 1f);
                RUIF.Draw.Label(col.Take(13f), labels[i], Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
                var vr = Layout.Row(col.Take(16f), 4f);
                RUIF.Draw.Label(vr.Take(60f), values[i], Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
                RUIF.Draw.Label(vr.Rest, tones[i] == Theme.Alive ? "▲" : tones[i] == Theme.Dead ? "▼" : "—",
                    Theme.FontLabel, tones[i], TextAnchor.MiddleRight);
            }
        }

        /// <summary>条形图（零贴图；highlight 索引以 accent 高亮）。</summary>
        private static void BarChart(Rect plot, float[] vals, int[] highlights, Color baseColor)
        {
            float bw = plot.width / vals.Length;
            for (int i = 0; i < vals.Length; i++)
            {
                float h = Mathf.Max(2f, plot.height * Mathf.Clamp01(vals[i]));
                bool hi = highlights != null && System.Array.IndexOf(highlights, i) >= 0;
                RUIF.Draw.Box(new Rect(plot.x + i * bw + 1f, plot.yMax - h, bw - 2f, h),
                    hi ? Theme.Accent : RUIF.Draw.WithAlpha(baseColor, 0.72f));
            }
        }

        /// <summary>键值行（左文本 + 右灰色注记）。</summary>
        private static void KvRow(Rect rect, string left, string right, Color leftColor)
        {
            var row = Layout.Row(rect, 6f);
            RUIF.Draw.Label(row.Take(rect.width * 0.62f), left, Theme.FontLabel, leftColor, TextAnchor.MiddleLeft);
            RUIF.Draw.Label(row.Rest, right ?? string.Empty, Theme.FontLabel, Theme.Dim, TextAnchor.MiddleRight);
        }

        /// <summary>状态点 + 文本行。</summary>
        private static void DotRow(Rect rect, Color dot, string text)
        {
            StatusAtoms.PulseDot(new Rect(rect.x + 2f, rect.y + rect.height / 2f - 4f, 8f, 8f), dot, Time.realtimeSinceStartup);
            RUIF.Draw.Label(new Rect(rect.x + 16f, rect.y, rect.width - 16f, rect.height), text,
                Theme.FontLabel, dot == Theme.Dead || dot == Theme.Warn ? dot : Theme.Text, TextAnchor.MiddleLeft);
        }

        // ================= P1 · 总督仪表盘（M2 三段） =================

        private static readonly float[] p1Trend = { 0.30f, 0.45f, 0.38f, 0.60f, 0.52f, 0.75f, 0.68f, 0.90f, 0.82f, 0.96f, 0.74f, 0.58f };
        private static readonly int[] p1Peaks = { 7, 9 };

        private static void P1Governor(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 0, "第 3 年 季夏 · 第 913221 刻");

            KpiRow(col.Take(36f),
                new[] { "人口", "平均心情", "威胁", "财富", "野生动物" },
                new[] { "12", "71%", "3.4", "284k", "17" },
                new[] { Theme.Alive, Theme.Dead, Theme.Muted, Theme.Alive, Theme.Muted });

            Rect mid = col.Take(128f);
            var mr = Layout.Row(mid, Theme.SpaceSm);
            Rect trend = Card(mr.Take(mid.width * 0.62f), "工时产出趋势（近 60 天）");
            BarChart(new Rect(trend.x, trend.y, trend.width, trend.height - 16f), p1Trend, p1Peaks, Theme.Info);
            RUIF.Draw.Label(new Rect(trend.x, trend.yMax - 15f, trend.width, 14f),
                "▬ 工时　▬ 产出峰值", Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);

            Rect feed = Card(mr.Rest, "最新事件流");
            var fcol = Layout.Column(feed, 1f);
            KvRow(fcol.Take(20f), "张三 完成杰作级突击步枪", "2h", Theme.Text);
            KvRow(fcol.Take(20f), "商队抵达 · 伯利部落", "5h", Theme.Text);
            KvRow(fcol.Take(20f), "虫灾预警 · 冷冻库下方", "6h", Theme.Dead);
            KvRow(fcol.Rest, "李四 完成论文《装配线平衡》", "1d", Theme.Text);

            Rect bot = col.Take(88f);
            var br = Layout.Row(bot, Theme.SpaceSm);
            float b3 = (bot.width - Theme.SpaceSm * 2f) / 3f;
            DataAtoms.RankList(Card(br.Take(b3), "工时榜"), p1RankNames, p1RankVals, 500f);
            DataAtoms.NeedsPanel(Card(br.Take(b3), "需求风向"), p1NeedNames, p1NeedVals);
            Rect alerts = Card(br.Rest, "警报");
            var acol = Layout.Column(alerts, 2f);
            DotRow(acol.Take(20f), Theme.Dead, "虫巢 3h 内破土");
            DotRow(acol.Take(20f), Theme.Warn, "娱乐均值偏低");
            DotRow(acol.Rest, Theme.Info, "4 人可晋升");
        }

        private static readonly string[] p1RankNames = { "张三", "李四", "王五" };
        private static readonly float[] p1RankVals = { 469f, 402f, 377f };
        private static readonly string[] p1NeedNames = { "心情", "娱乐", "美观" };
        private static readonly float[] p1NeedVals = { 0.71f, 0.34f, 0.58f };

        // ================= P2 · 殖民者档案（M1 全页详情） =================

        private static void P2Dossier(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 1, "殖民地档案 › 人物 › 张三");

            Rect hero = col.Take(72f);
            var hr = Layout.Row(hero, Theme.SpaceSm);
            Rect avatar = hr.Take(64f);
            RUIF.Draw.Box(avatar, RUIF.Draw.WithAlpha(Theme.Accent, 0.18f));
            RUIF.Draw.Border(avatar, Theme.Accent);
            RUIF.Draw.Label(avatar, "张", Theme.FontValue, Theme.Accent, TextAnchor.MiddleCenter);

            Rect bioCard = Card(hr.Rest);
            var bio = Layout.Column(bioCard, 3f);
            DataAtoms.TraitChips(bio.Take(22f), new[] { "总工程师", "乐观主义者", "夜猫子" }, new[] { true, true, false });
            var prow = Layout.Row(bio.Take(18f), 8f);
            PanelAtoms.StatRow(prow.Take((bioCard.width - 8f) * 0.46f), "心情", "82", 0.82f, Theme.Alive);
            PanelAtoms.StatRow(prow.Rest, "健康", "64", 0.64f, Theme.Info);
            RUIF.Draw.Label(bio.Rest, "从业 3 年 · 工时 469h · 良品率 52% → 目标 60%",
                Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);

            Rect mid = col.Take(86f);
            var mr2 = Layout.Row(mid, Theme.SpaceSm);
            DataAtoms.SkillBars(Card(mr2.Take(mid.width * 0.52f), "核心技能"),
                p2SkillNames, p2SkillLevels, 20f, 2);
            Rect promo = Card(mr2.Rest, "晋升条件 · 院士");
            var pcol = Layout.Column(promo, 2f);
            KvRow(pcol.Take(17f), "✓ 工时 ≥ 400h（469）", null, Theme.Alive);
            KvRow(pcol.Take(17f), "✓ 论文已发表 ×2", null, Theme.Alive);
            KvRow(pcol.Rest, "✗ 良品率 ≥ 60%（52%）", null, Theme.Dead);

            DataAtoms.TimelinePro(col.Rest, p2TlDates, p2TlTexts,
                new[] { Theme.Alive, Theme.Accent, Theme.Dead }, 22f);
        }

        private static readonly string[] p2SkillNames = { "建造", "射击", "智识", "艺术" };
        private static readonly float[] p2SkillLevels = { 18f, 14f, 13f, 0f };
        private static readonly string[] p2TlDates = { "5501", "5503", "5505" };
        private static readonly string[] p2TlTexts = { "加入殖民地", "建造合金工坊（里程碑）", "虫灾夜战（合并 ×3）" };

        // ================= P3 · 战役志（M2 变体：对抗 hero） =================

        private static void P3Battle(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 2, "5505 春 14 日 · 胜利");

            Rect vs = col.Take(34f);
            float vsW = 30f;
            var vr = Layout.Row(vs, 8f);
            Rect left = vr.Take((vs.width - vsW - 16f) / 2f);
            RUIF.Draw.Label(new Rect(left.x, left.y, left.width, 13f), "我方 · 6 人参战", Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            RUIF.Draw.Box(new Rect(left.x, left.y + 15f, left.width * 0.78f, 10f), Theme.Alive);
            RUIF.Draw.Box(new Rect(left.x, left.y + 15f, left.width, 10f), RUIF.Draw.WithAlpha(Theme.Text, 0.06f));
            RUIF.Draw.Label(vr.Take(vsW), "VS", Theme.FontBody, Theme.Accent, TextAnchor.MiddleCenter);
            Rect right = vr.Rest;
            RUIF.Draw.Label(new Rect(right.x, right.y, right.width, 13f), "虫群 · 23 只", Theme.FontLabel, Theme.Muted, TextAnchor.MiddleRight);
            RUIF.Draw.Box(new Rect(right.x, right.y + 15f, right.width, 10f), Theme.Dead);

            Rect bigRect = col.Take(52f);
            float bigW = (bigRect.width - Theme.SpaceSm * 2f) / 3f;
            var big = Layout.Row(bigRect, Theme.SpaceSm);
            BigNum(big.Take(bigW), "23", "击杀", Theme.Alive);
            BigNum(big.Take(bigW), "2", "负伤", Theme.Warn);
            BigNum(big.Take(bigW), "0", "阵亡", Theme.Info);

            Rect tl = Card(col.Take(38f), "战斗时间线（3 次接敌合并）");
            var chips = Layout.Row(tl, Theme.SpaceXs);
            foreach (string s in p3Chips)
            {
                var c = chips.Take(Mathf.Min(230f, chips.Rest.width));
                RUIF.Draw.Box(c, RUIF.Draw.WithAlpha(Theme.Accent, 0.10f));
                RUIF.Draw.Border(c, Theme.BorderSoft);
                RUIF.Draw.Label(c, s, Theme.FontLabel, Theme.Text, TextAnchor.MiddleCenter);
            }

            Rect bot = col.Take(80f);
            var br = Layout.Row(bot, Theme.SpaceSm);
            Rect merit = Card(br.Take(bot.width / 2f), "功勋榜");
            var mcol = Layout.Column(merit, 1f);
            KvRow(mcol.Take(19f), "① 张三 · 狙杀 9", "精确率 82%", Theme.Text);
            KvRow(mcol.Take(19f), "② 李四 · 火力压制", "压制 3 波", Theme.Text);
            KvRow(mcol.Rest, "③ 王五 · 医疗救回 2", "0 死亡", Theme.Text);
            Rect loot = Card(br.Rest, "战利缴获");
            var lcol = Layout.Column(loot, 1f);
            KvRow(lcol.Take(19f), "虫胶 ×86 · 甲壳素 ×34", "已入仓", Theme.Text);
            KvRow(lcol.Rest, "损坏护甲 ×2（待修复）", "仓库", Theme.Text);
        }

        private static readonly string[] p3Chips = { "21:40 首次接敌 · 冷冻库走廊", "22:15 虫群破墙 · 走廊失守", "23:02 增援抵达 · 反推成功" };

        private static void BigNum(Rect rect, string num, string label, Color tone)
        {
            RUIF.Draw.Box(rect, Theme.Card);
            RUIF.Draw.Border(rect, Theme.BorderHair);
            var col = Layout.Column(rect, 0f);
            RUIF.Draw.Label(col.Take(rect.height - 15f), num, Theme.FontValue, tone, TextAnchor.MiddleCenter);
            RUIF.Draw.Label(col.Rest, label, Theme.FontLabel, Theme.Dim, TextAnchor.MiddleCenter);
        }

        // ================= P4 · 传承谱系（M5 双世家并排） =================

        private static readonly TreeNode p4Zhang = BuildP4Zhang();
        private static readonly TreeNode p4Li = BuildP4Li();

        private static TreeNode BuildP4Zhang()
        {
            var root = new TreeNode("张老 · 初代（5500–）") { Tag = "18 杀 · 建造 12" };
            root.Add(new TreeNode("张中 · 二代") { Tag = "匠作 9 件" });
            root.Add(new TreeNode("张小 · 三代") { Tag = "在学" });
            root.Add(new TreeNode("张幼 · 三代（5491 殁 · 虫灾）") { Dead = true });
            return root;
        }

        private static TreeNode BuildP4Li()
        {
            var root = new TreeNode("李一 · 初代（5500–）");
            root.Add(new TreeNode("李二 · 二代") { Tag = "手术 41 台" });
            root.Add(new TreeNode("李三 · 三代"));
            return root;
        }

        private static void P4Legacy(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 3, "两支血脉 · 5 代 · 仅血脉视图");

            Rect trees = col.Take(150f);
            var tr = Layout.Row(trees, Theme.SpaceSm);
            Rect zhang = Card(tr.Take(trees.width / 2f), "张家 · 铁匠世家〔传承锻造者〕");
            NavAtoms.CollapsibleTree(zhang, p4Zhang);
            Rect li = Card(tr.Rest, "李家 · 医者世家");
            NavAtoms.CollapsibleTree(li, p4Li);

            Rect chain = Card(col.Rest, "传承链进度 · 铁匠世家 → 第 3 代");
            var ccol = Layout.Column(chain, 4f);
            LegacyBar(ccol.Take(20f), "代际", 0.6f, "3/5", Theme.Accent);
            LegacyBar(ccol.Take(20f), "技艺", 0.74f, "74%", Theme.Info);
            LegacyBar(ccol.Rest, "声望", 0.45f, "45%", Theme.Alive);
        }

        private static void LegacyBar(Rect rect, string label, float p01, string val, Color tone)
        {
            var row = Layout.Row(rect, 8f);
            RUIF.Draw.Label(row.Take(44f), label, Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            Rect bar = row.Take(rect.width - 96f);
            RUIF.Draw.Box(bar, RUIF.Draw.WithAlpha(Theme.Text, 0.07f));
            RUIF.Draw.Box(new Rect(bar.x, bar.y, bar.width * p01, bar.height), tone);
            RUIF.Draw.Label(row.Rest, val, Theme.FontLabel, Theme.Text, TextAnchor.MiddleRight);
        }

        // ================= P5 · 车间指挥台（M6） =================

        private static float p5Threshold = 72f;
        private static readonly int[] p5Queue = { 0, 1, 2 };
        private static int p5QueueCount = 3;

        private static void P5Workshop(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 4, "班次 白班 · 在岗 4/6");

            KpiRow(col.Take(36f),
                new[] { "当日产出", "良品率", "队列积压", "物料" },
                new[] { "34", "68%", "7", "充足" },
                new[] { Theme.Alive, Theme.Alive, Theme.Warn, Theme.Muted });

            Rect chain = Card(col.Take(40f), "12 道工序链 · 当前瓶颈：⑥ 枪管");
            var flow = Layout.Row(Layout.Inset(chain, 2f, 0f), 2f);
            for (int i = 0; i < p5Steps.Length; i++)
            {
                bool done = i < 5, cur = i == 5;
                var c = flow.Take(Mathf.Min(86f, flow.Rest.width));
                RUIF.Draw.Box(c, cur ? RUIF.Draw.WithAlpha(Theme.Accent, 0.14f) : RUIF.Draw.WithAlpha(Theme.Text, 0.04f));
                RUIF.Draw.Border(c, done ? Theme.Alive : cur ? Theme.Accent : Theme.BorderHair);
                RUIF.Draw.Label(c, (done ? "✓ " : cur ? "◉ " : "") + p5Steps[i], Theme.FontLabel,
                    done ? Theme.Alive : cur ? Theme.Accent : Theme.Muted, TextAnchor.MiddleCenter);
                if (i < p5Steps.Length - 1)
                    RUIF.Draw.Label(flow.Take(10f), "›", Theme.FontLabel, Theme.Dim, TextAnchor.MiddleCenter);
            }

            Rect bot = col.Take(92f);
            var br = Layout.Row(bot, Theme.SpaceSm);
            Rect thr = Card(br.Take(bot.width * 0.4f), "交付阈值");
            InputAtoms.SliderWithValue(Layout.Inset(thr, 4f, 0f), ref p5Threshold, 0f, 250f, 1f,
                v => v.ToString("0") + " 件");
            InputPlusAtoms.QueueEditor(Card(br.Rest, "生产队列"), p5Queue, ref p5QueueCount, p5QueueNames, 24f);
        }

        private static readonly string[] p5Steps = { "冶炼", "锻造", "热处理", "粗加工", "精密", "枪管", "表面", "木托", "供弹", "总装" };
        private static readonly string[] p5QueueNames = { "突击步枪", "电荷步枪", "重型狙击" };

        // ================= P6 · 设置中枢（M1 变体） =================

        private static int p6Tab;
        private static bool p6Auto = true, p6Keep = true, p6Log;
        private static float p6Density = 50f;
        private static int p6Interval = 600;

        private static void P6Settings(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 5, "Esc 返回");

            InputAtoms.Segmented(col.Take(24f), new[] { "通用", "记录规则", "主题", "性能" }, ref p6Tab);

            Rect body = col.Take(108f);
            var br = Layout.Row(body, Theme.SpaceSm);
            if (p6Tab == 2)
            {
                Rect themeCard = Card(br.Take(body.width / 2f), "主题（点击即时换肤）");
                var tcol = Layout.Column(themeCard, 4f);
                var trow = Layout.Row(tcol.Take(26f), Theme.SpaceXs);
                if (Widgets.ButtonText(trow.Take(78f), "Vanilla", true, false, Theme.ActiveId != Theme.VanillaId))
                    Theme.Apply(Theme.VanillaId);
                if (Widgets.ButtonText(trow.Take(78f), "Dark", true, false, Theme.ActiveId != Theme.DarkId))
                    Theme.Apply(Theme.DarkId);
                RUIF.Draw.Label(tcol.Take(16f), "当前：" + Theme.ActiveId, Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
                RUIF.Draw.Label(tcol.Rest, "第三方主题经 Theme.Register 扩展点挂接", Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
            }
            else
            {
                Rect toggles = Card(br.Take(body.width / 2f), p6Tab == 0 ? "通用开关" : p6Tab == 1 ? "记录规则" : "性能开关");
                var gcol = Layout.Column(toggles, 5f);
                ToggleLine(gcol.Take(22f), "自动归档战斗事件", ref p6Auto);
                ToggleLine(gcol.Take(22f), "阵亡者保留档案", ref p6Keep);
                ToggleLine(gcol.Rest, "详细战斗日志", ref p6Log);

                Rect nums = Card(br.Rest, "采样");
                var ncol = Layout.Column(nums, 5f);
                RUIF.Draw.Label(ncol.Take(15f), "事件密度", Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
                InputAtoms.SliderWithValue(ncol.Take(22f), ref p6Density, 0f, 100f, 5f, v => v.ToString("0") + "%");
                var srow = Layout.Row(ncol.Rest, 8f);
                InputAtoms.Stepper(srow.Take(120f), ref p6Interval, 60, 3000);
                RUIF.Draw.Label(srow.Rest, "刻（" + p6Interval / 60 + " 秒）", Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
            }

            RUIF.Draw.Label(Card(col.Rest, "提示"), "语言子包：框架文案走 Keyed（RUIF.UI.*），演示数据不属框架文案。",
                Theme.FontLabel, Theme.Dim, TextAnchor.MiddleLeft);
        }

        private static void ToggleLine(Rect rect, string label, ref bool on)
        {
            var row = Layout.Row(rect, 8f);
            RUIF.Draw.Label(row.Take(rect.width - 66f), label, Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
            InputAtoms.ToggleSwitch(row.Rest, ref on);
        }

        // ================= P7 · 工作调度台（M7 · Work Tab 实证） =================

        private static int p7Preset;
        private static readonly int[] p7Prio = { 1, 2, 0, 4, 0, 0, 1, 3, 0, 2, 0, 0, 1, 3, 0 };
        private static readonly int[] p7Timetable = new int[72];   // 3 人 × 24 刻

        private static void P7WorkTab(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 6, "12 人 · 4 档优先级 · 左键循环");

            DataExAtoms.ProfilePreset(col.Take(24f),
                new[] { "全员施工", "战时体制", "休闲日" }, ref p7Preset, out bool _);

            RUIF.Draw.Label(col.Take(14f), "优先级网格（点击循环 1→4 · 0=禁用）",
                Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            if (InputExAtoms.PriorityGrid(col.Take(92f), p7Workers, p7Jobs, p7Prio, out int pi))
                p7Prio[pi] = InputExFmt.NextPriority(p7Prio[pi]);

            RUIF.Draw.Label(col.Take(14f), "作息矩阵 · 张三（▉工作 ▉睡眠 ▉任何）",
                Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            if (InputExAtoms.TimetableMatrix(col.Rest, p7Workers, p7Timetable, out int ti))
                p7Timetable[ti] = InputExFmt.NextTimetableState(p7Timetable[ti]);
        }

        private static readonly string[] p7Workers = { "张三", "李四", "王五" };
        private static readonly string[] p7Jobs = { "灭火", "建造", "采矿", "烹饪", "医疗" };

        // ================= P8 · 研究图谱（M9 · ResearchPal 实证） =================

        private static int p8Sel = 2;
        private static string p8Query = string.Empty, p8Buf = string.Empty;
        private static float p8SearchLast = -10f;

        private static void P8Research(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 7, "34 项目 · 已完成 19 · 滚轮缩放 86%");

            var tools = Layout.Row(col.Take(26f), Theme.SpaceXs);
            InputAtoms.SearchField(tools.Take(190f), ref p8Query, ref p8Buf, "过滤…", ref p8SearchLast);
            InteractAtoms.ToolbarIcon(tools.Take(26f), "⤢", false);
            InteractAtoms.ToolbarIcon(tools.Rest, "≡", false);

            Rect graph = Card(col.Take(140f), null);
            DataExAtoms.NodeGraph(graph, p8Nodes, p8Col, p8Row, p8From, p8To, p8State, ref p8Sel, 96f, 26f, 14f);

            Rect bot = col.Rest;
            var br = Layout.Row(bot, Theme.SpaceSm);
            Rect prog = Card(br.Take(bot.width * 0.55f), "进行中：精密机加工（72%）");
            var pr = Layout.Column(prog, 4f);
            RUIF.Draw.Label(pr.Take(15f), "剩余 2 天 6 小时 · 双人研究加速中", Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
            ProgressAtoms.CapacityBar(pr.Take(10f), 720f, 1000f);
            Rect next = Card(br.Rest, "推荐下一步");
            var nc = Layout.Column(next, 2f);
            KvRow(nc.Take(17f), "→ 装配线（解锁 6 项下游）", null, Theme.Alive);
            KvRow(nc.Rest, "→ 电气化（当前产线瓶颈）", null, Theme.Alive);
        }

        private static readonly string[] p8Nodes = { "基础制造", "金属加工", "合金学", "电气化", "装配线" };
        private static readonly int[] p8Col = { 0, 0, 1, 2, 3 };
        private static readonly int[] p8Row = { 0, 1, 0, 0, 0 };
        private static readonly int[] p8From = { 0, 1, 2, 3 };
        private static readonly int[] p8To = { 2, 2, 4, 4 };
        private static readonly int[] p8State = { 1, 1, 0, 0, 0 };

        // ================= P9 · 贸易中心（M8 变体：左我右商） =================

        private static int p9SelMine = 1, p9SelTheirs = -1;
        private static float p9Flash = -10f;

        private static void P9Trade(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 8, "伯利部落商队 · 谈判技巧 +8");

            Rect lists = col.Take(112f);
            var lr = Layout.Row(lists, Theme.SpaceSm);
            Rect mine = Card(lr.Take(lists.width / 2f), "我方出售（点选加入交易）");
            var mc = Layout.Column(mine, 1f);
            TradeRow(mc.Take(22f), "米 750 ×2 堆", "▲ 点选", p9SelMine == 0, 0, ref p9SelMine);
            TradeRow(mc.Take(22f), "药品 42　已选 20", "+340 银", p9SelMine == 1, 1, ref p9SelMine);
            TradeRow(mc.Rest, "杰作突击步枪 ×1", "品质加成 ×2.1", p9SelMine == 2, 2, ref p9SelMine);
            Rect theirs = Card(lr.Rest, "对方出售");
            var tc = Layout.Column(theirs, 1f);
            TradeRow(tc.Take(22f), "钢材 250", "−180 银", p9SelTheirs == 0, 0, ref p9SelTheirs);
            TradeRow(tc.Take(22f), "零部件 60", "−260 银", p9SelTheirs == 1, 1, ref p9SelTheirs);
            TradeRow(tc.Rest, "牲畜 ×2（可驯）", "−420 银", p9SelTheirs == 2, 2, ref p9SelTheirs);

            Rect bar = col.Take(30f);
            RUIF.Draw.Box(bar, Theme.Panel);
            RUIF.Draw.Border(bar, Theme.BorderSoft);
            var act = Layout.Row(Layout.Inset(bar, 8f, 3f), 8f);
            RUIF.Draw.Label(act.Take(bar.width - 170f),
                "交易平衡：我方净赚 +80 银（含谈判加成 ×1.08）", Theme.FontLabel, Theme.Alive, TextAnchor.MiddleLeft);
            if (Widgets.ButtonText(act.Take(70f), "清空")) { p9SelMine = -1; p9SelTheirs = -1; }
            if (Widgets.ButtonText(act.Rest, "成交")) p9Flash = Time.realtimeSinceStartup;
            if (Time.realtimeSinceStartup - p9Flash < 0.8f)
                PanelAtoms.FlashOverlay(bar, p9Flash);
        }

        private static void TradeRow(Rect rect, string text, string right, bool selected, int idx, ref int sel)
        {
            if (selected) RUIF.Draw.Box(rect, RUIF.Draw.WithAlpha(Theme.Accent, 0.14f));
            else if (Mouse.IsOver(rect)) RUIF.Draw.Box(rect, Theme.CardHover);
            RUIF.Draw.Label(rect, text, Theme.FontLabel, selected ? Theme.Accent : Theme.Text, TextAnchor.MiddleLeft);
            RUIF.Draw.Label(rect, right, Theme.FontLabel, selected ? Theme.Accent : Theme.Dim, TextAnchor.MiddleRight);
            if (Mouse.IsOver(rect) && Widgets.ButtonInvisible(rect)) sel = idx;
        }

        // ================= P10 · 电力中枢（M2 变体） =================

        private static readonly float[] p10Curve = { 0.30f, 0.26f, 0.24f, 0.34f, 0.58f, 0.76f, 0.88f, 0.96f, 0.82f, 0.64f, 0.46f, 0.34f };

        private static void P10Power(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 9, "主网 3 · 盈余 +290W");

            Rect top = col.Take(92f);
            var tr = Layout.Row(top, Theme.SpaceSm);
            Rect supply = Card(tr.Take(top.width * 0.5f), "主网 3 · 供需");
            PowerRow(Layout.Column(supply, 5f).Take(18f), "发电", 0.72f, "1420W", Theme.Alive);
            PowerRow(Layout.Column(supply, 5f).Take(18f), "耗电", 0.53f, "1130W", Theme.Info);
            PowerRow(Layout.Column(supply, 5f).Take(18f), "电池", 0.76f, "76%", Theme.Accent);

            Rect alerts = Card(tr.Rest, "警报语义（1.6 电网链）");
            var acol = Layout.Column(alerts, 3f);
            DotRow(acol.Take(19f), Theme.Dead, "冷冻库段：真·未接电网（PowerNet==null）");
            DotRow(acol.Take(19f), Theme.Warn, "农机段：已接网但供电不足（!PowerOn）");
            DotRow(acol.Rest, Theme.Alive, "主网：正常 · 电池 76% 充放中");

            Rect curve = Card(col.Rest, "24h 用电曲线 · 峰值 18:00 太阳能退坡");
            BarChart(new Rect(curve.x, curve.y, curve.width, curve.height - 14f), p10Curve, new[] { 7 }, Theme.Info);
        }

        private static void PowerRow(Rect rect, string label, float p01, string val, Color tone)
        {
            var row = Layout.Row(rect, 8f);
            RUIF.Draw.Label(row.Take(44f), label, Theme.FontLabel, Theme.Muted, TextAnchor.MiddleLeft);
            Rect bar = row.Take(rect.width - 110f);
            RUIF.Draw.Box(bar, RUIF.Draw.WithAlpha(Theme.Text, 0.07f));
            RUIF.Draw.Box(new Rect(bar.x, bar.y, bar.width * p01, bar.height), tone);
            RUIF.Draw.Label(row.Rest, val, Theme.FontLabel, Theme.Text, TextAnchor.MiddleRight);
        }

        // ================= P11 · 健康中心（M1 主从） =================

        private static void P11Medbay(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 10, "3 名病患 · 2 医待命 · 医疗 12");

            Rect queue = Card(col.Take(74f), "病患队列（按危重度排序）");
            var qcol = Layout.Column(queue, 2f);
            KvRow(qcol.Take(19f), "● 张三 · 左腿开放伤口 · 失血 34%", "手术台 1 · 王五主刀", Theme.Dead);
            KvRow(qcol.Take(19f), "● 李四 · 前臂割伤 · 已处理", "恢复中 · 2.1d", Theme.Warn);
            KvRow(qcol.Rest, "● 赵六 · 术前镇静", "手术台 2 · 待王五", Theme.Info);

            Rect bot = col.Rest;
            var br = Layout.Row(bot, Theme.SpaceSm);
            Rect status = Card(br.Take(bot.width * 0.55f), "张三 · 身体状况");
            DataAtoms.HealthRows(status, p11Parts, p11States, p11Sev);
            Rect plan = Card(br.Rest, "手术计划");
            var pcol = Layout.Column(plan, 2f);
            KvRow(pcol.Take(19f), "① 缝合左腿（成功率 92%）", "排程", Theme.Text);
            KvRow(pcol.Take(19f), "② 移除固定架", "术后 2d", Theme.Text);
            KvRow(pcol.Rest, "药品消耗预估：Medicine ×2", null, Theme.Dim);
        }

        private static readonly string[] p11Parts = { "左腿", "整体" };
        private static readonly string[] p11States = { "开放伤口", "失血" };
        private static readonly float[] p11Sev = { 0.8f, 0.34f };

        // ================= P12 · 层叠工作区（M10 · RimHUD 实证） =================

        private static bool p12DockCollapsed;

        private static void P12Docking(Rect rect)
        {
            var col = Layout.Column(rect, Theme.SpaceSm);
            PageHead(col.Take(HeadH), 11, "布局档案「指挥官」 · Ctrl+K");

            Rect canvas = col.Take(168f);
            RUIF.Draw.Box(canvas, Theme.Window);
            RUIF.Draw.Border(canvas, Theme.BorderHair);
            float pad = 8f;

            Rect left = new Rect(canvas.x + pad, canvas.y + pad, canvas.width * 0.26f, canvas.height - pad * 2f - 26f);
            RUIF.Draw.Box(left, Theme.Card);
            RUIF.Draw.Border(left, Theme.BorderSoft);
            var lcol = Layout.Column(Layout.Inset(left, 6f, 4f), 2f);
            RUIF.Draw.Label(lcol.Take(16f), "事件流（左停靠）", Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
            KvRow(lcol.Take(16f), "虫灾预警", "6h", Theme.Dead);
            KvRow(lcol.Rest, "商队抵达", "5h", Theme.Text);

            Rect mid = new Rect(left.xMax + 8f, canvas.y + pad, canvas.width * 0.2f, 64f);
            RUIF.Draw.Box(mid, Theme.Card);
            RUIF.Draw.Border(mid, Theme.BorderSoft);
            var mcol = Layout.Column(Layout.Inset(mid, 6f, 3f), 3f);
            RUIF.Draw.Label(mcol.Take(15f), "电力", Theme.FontLabel, Theme.Text, TextAnchor.MiddleLeft);
            RUIF.Draw.Label(mcol.Take(12f), "盈余 +290W", Theme.FontLabel, Theme.Alive, TextAnchor.MiddleLeft);
            Rect p10bar = mcol.Take(6f);
            RUIF.Draw.Box(p10bar, RUIF.Draw.WithAlpha(Theme.Text, 0.07f));
            RUIF.Draw.Box(new Rect(p10bar.x, p10bar.y, p10bar.width * 0.76f, 6f), Theme.Accent);

            Rect hud = new Rect(canvas.xMax - pad - canvas.width * 0.36f, canvas.y + pad, canvas.width * 0.36f, 74f);
            RUIF.Draw.Box(hud, Theme.Card);
            RUIF.Draw.Border(hud, Theme.Accent);
            var hcol = Layout.Column(Layout.Inset(hud, 8f, 4f), 4f);
            RUIF.Draw.Label(hcol.Take(15f), "档案 HUD（浮动）", Theme.FontLabel, Theme.Accent, TextAnchor.MiddleLeft);
            PanelAtoms.MiniProfileChip(hcol.Rest, "张", "张三 · 心情 82 · 建造中", 0.82f, 0.64f);

            Rect dock = new Rect(canvas.x + pad, canvas.yMax - pad - 24f, canvas.width - pad * 2f - 96f, 24f);
            RUIF.Draw.Box(dock, Theme.Panel);
            RUIF.Draw.Border(dock, Theme.BorderSoft);
            var drow = Layout.Row(Layout.Inset(dock, 8f, 3f), 6f);
            foreach (string icon in new[] { "家", "人", "战", "厂", "表" })
            {
                var c = drow.Take(26f);
                RUIF.Draw.Box(c, RUIF.Draw.WithAlpha(Theme.Text, 0.05f));
                RUIF.Draw.Label(c, icon, Theme.FontLabel, Theme.Muted, TextAnchor.MiddleCenter);
            }
            RUIF.Draw.Label(drow.Rest, "QuickDock · 拖入任意窗钉住", Theme.FontLabel, Theme.Dim, TextAnchor.MiddleRight);
            DataExAtoms.DockHost(new Rect(dock.xMax + 4f, dock.y, 88f, dock.height), "停靠", ref p12DockCollapsed);
        }
    }
}
