using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RUIF;

namespace RUIF.Demo
{
    /// <summary>
    /// 分类「官方数据候选」· <b>14 件</b> —— 全部经 <see cref="OfficialDataCandFmt"/> 构造、
    /// 再由 <see cref="OfficialDataCandAtoms.Build(Rect,*)"/> 渲染（候选层复用 Atoms 既有绘制，零复制）。
    ///
    /// <para>14 件：WealthBreakdown / RecordsTable / HistoryFeed / TaleCard、
    /// LetterCard / BillList / QuestBoard / TitleCard / PsycastTree、
    /// IdeoCard / CaravanPanel / GeneChips / MessageFeed / PolicyCard。
    /// ⚠ 反编译核实（2026-09-21）：框架并无 OfficialDataCandAtomsB/C 类型，
    /// A/B/C 三族的 Cand 全部是 <see cref="OfficialDataCandAtoms"/> 的嵌套 readonly 结构体，
    /// 经 <see cref="OfficialDataCandFmt"/> 构造、再由 <see cref="OfficialDataCandAtoms.Build(Rect,*)"/> 各重载渲染。</para>
    ///
    /// <para>每个候选独立构造并渲染：先用 <c>OfficialDataCandFmt.X(...)</c> 造出
    /// <c>OfficialDataCandAtoms.XxxCand</c>，再 <c>OfficialDataCandAtoms.Build(rect, cand)</c> 转发绘制，
    /// 用合理示例数据填充 cand 字段。</para>
    /// </summary>
    public sealed class BenchOfficial : IBench
    {
        /// <inheritdoc />
        public bool Scrollable => true;

        private const float HeadH = 20f;   // 标题行
        private const float Gap = 12f;     // 组件间距

        // 各候选本体高度（与 Render 中的 Take 严格一致，供 Height 估算）
        //
        // ⚠ 2026-10-05 实机取证修正：其中 5 件的「行高 = rect.height / n」型组件（BillList / MessageFeed /
        //   HistoryFeed）与「高 = rect.height」型组件（GeneChips 的芯片高、CaravanPanel 的内缩余量）
        //   会被**给定高度直接撑开** —— 原来按「卡片观感」给的 130–150f 在实机上表现为：
        //   BillList 3 行占 150px（50px 行距，进度条孤零零吊在每行底部）、GeneChips 的芯片被拉成
        //   150px 高的色柱、HistoryFeed 3 行 18px 内容摊在 150px 里留下近 90px 空白。
        //   高度必须按**内容自然高**给：行数 × 行高 + 间距。
        private static readonly float[] BodyH =
        {
            130f, // WealthBreakdown（自绘总额行 + 4 行图例，按 rect 自适应）
            120f, // RecordsTable（4 行 × 30）
            70f,  // HistoryFeed（3 行 × 18 + 2 × 3 间距 = 60，留 10 余量）
            90f,  // TaleCard
            100f, // LetterCard
            96f,  // BillList（3 行 × 32：标签居中 + 2px 底进度条）
            96f,  // CaravanPanel（24 头像 + 4 + 16 负重 + 4 + 18 口粮 + 上下内缩 12 = 78）
            34f,  // GeneChips（芯片高 = rect.height ⇒ 给徽章高，不是面板高）
            170f, // IdeoCard
            84f,  // MessageFeed（3 行 × 26 + 2 × 2 间距 = 82）
            110f, // PolicyCard
            160f, // PsycastTree
            150f, // QuestBoard
            150f  // TitleCard
        };

        /// <inheritdoc />
        public float Height(float w)
        {
            float h = 0f;
            for (int i = 0; i < BodyH.Length; i++) h += HeadH + BodyH[i] + Gap;
            return h;
        }

        /// <inheritdoc />
        public void Render(Rect area)
        {
            var col = Layout.Column(area, Gap);

            // ── ① WealthBreakdown 财富构成（A 族）
            RenderOne(ref col, "① WealthBreakdown 财富构成", BodyH[0], r =>
            {
                var cand = OfficialDataCandFmt.WealthBreakdownCand(
                    new[] { "银币", "物资", "建筑", "生物" },
                    new[] { 3200f, 5400f, 1800f, 900f },
                    new[] { Tokens.Alive, Tokens.Warn, Tokens.Accent, Tokens.Info });
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ② RecordsTable 记录表（A 族）
            RenderOne(ref col, "② RecordsTable 记录表", BodyH[1], r =>
            {
                var cand = OfficialDataCandFmt.RecordsTableCand(
                    new[] { "击杀", "治疗", "建造", "收割" },
                    new[] { "128", "340", "56", "77" });
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ③ HistoryFeed 历史事件流（A 族）
            RenderOne(ref col, "③ HistoryFeed 历史事件流", BodyH[2], r =>
            {
                var cand = OfficialDataCandFmt.HistoryFeedCand(
                    new[] { "第 12 春", "第 12 夏", "第 12 秋" },
                    new[] { "击退部落袭击", "瘟疫宣告结束", "与商队达成贸易" },
                    new[] { "战斗", "健康", "外交" });
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ④ TaleCard 传说卡（A 族）
            RenderOne(ref col, "④ TaleCard 传说卡", BodyH[3], r =>
            {
                var cand = OfficialDataCandFmt.TaleCardCand("艾拉", "据说她独自守住了西门，直到援军抵达。", 3);
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑤ LetterCard 信件归档卡（B 族，tone 默认 Accent）
            RenderOne(ref col, "⑤ LetterCard 信件归档卡", BodyH[4], r =>
            {
                var cand = OfficialDataCandFmt.LetterCardCand("袭击警报", "一批部落民正在接近殖民地边缘。", "刚刚");
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑥ BillList 账单队列（B 族）
            RenderOne(ref col, "⑥ BillList 账单队列", BodyH[5], r =>
            {
                var cand = OfficialDataCandFmt.BillListCand(
                    new[] { "钢铁长剑", "木板", "餐椅" },
                    new[] { 12f, 30f, 5f },
                    new[] { 20f, 30f, 8f },
                    new[] { false, false, true });
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑦ CaravanPanel 商队面板（C 族）
            RenderOne(ref col, "⑦ CaravanPanel 商队面板", BodyH[6], r =>
            {
                var cand = OfficialDataCandFmt.CaravanPanelCand(
                    new[] { "艾拉", "鲍勃", "猎犬" }, 0.72f, 4.5f, "2 天后抵达");
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑧ GeneChips 基因芯片（C 族，gap 用默认）
            RenderOne(ref col, "⑧ GeneChips 基因芯片", BodyH[7], r =>
            {
                var cand = OfficialDataCandFmt.GeneChipsCand(
                    new[] { "耐力", "夜视", "喷酸" },
                    new[] { 2, 1, 3 },
                    new[] { true, true, false });
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑨ IdeoCard 文化卡（C 族）
            RenderOne(ref col, "⑨ IdeoCard 文化卡", BodyH[8], r =>
            {
                var cand = OfficialDataCandFmt.IdeoCardCand(
                    "圣火会", "大祭司", 7, new[] { "崇火", "素食", "守夜" });
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑩ MessageFeed 瞬息消息流（C 族）
            RenderOne(ref col, "⑩ MessageFeed 瞬息消息流", BodyH[9], r =>
            {
                var cand = OfficialDataCandFmt.MessageFeedCand(
                    new[] { "殖民者已康复", "新的交易请求", "狼群出没" },
                    new[] { "10:12", "09:40", "08:05" });
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑪ PolicyCard 政策卡（C 族）
            RenderOne(ref col, "⑪ PolicyCard 政策卡", BodyH[10], r =>
            {
                var cand = OfficialDataCandFmt.PolicyCardCand("宵禁", "夜晚禁止殖民者外出活动。", 4);
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑫ PsycastTree 心能树（B 族）
            RenderOne(ref col, "⑫ PsycastTree 心能树", BodyH[11], r =>
            {
                var cand = OfficialDataCandFmt.PsycastTreeCand(
                    new[] { "火球", "灵视", "念力" },
                    new[] { 1, 2, 3 },
                    new[] { true, true, false });
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑬ QuestBoard 任务板（B 族）
            RenderOne(ref col, "⑬ QuestBoard 任务板", BodyH[12], r =>
            {
                var cand = OfficialDataCandFmt.QuestBoardCand(
                    new[] { "寻回圣物", "护送商队" },
                    new[] { 0.4f, 0.8f },
                    new[] { 3f, 1f });
                OfficialDataCandAtoms.Build(r, cand);
            });

            // ── ⑭ TitleCard 头衔卡（B 族）
            RenderOne(ref col, "⑭ TitleCard 头衔卡", BodyH[13], r =>
            {
                var cand = OfficialDataCandFmt.TitleCardCand(
                    "男爵", "第 12 秋授予", new[] { "征税", "征兵", "巡游" }, new[] { true, false, true });
                OfficialDataCandAtoms.Build(r, cand);
            });
        }

        /// <summary>统一：取标题行 + 卡片底 + 内缩后把本体区域交给 paint 绘制。</summary>
        private static void RenderOne(ref VStack col, string title, float bodyH, Action<Rect> paint)
        {
            Rect head = col.Take(HeadH);
            Draw.Label(head, title, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            Rect body = col.Take(bodyH);
            Draw.Box(body, Tokens.Card);
            Draw.Border(body, Tokens.BorderSoft);
            paint(Layout.Inset(body, 8f, 6f));
        }
    }
}
