using UnityEngine;
using System;

namespace RUIF
{
    /// <summary>
    /// ⑮ 官方数据候选组件（A/B/C 三族）的纯计算 Build 工厂。
    /// <para>候选 readonly struct 同构于 DataCand：定义于 <see cref="OfficialDataCandAtoms"/>（*CandAtoms 静态类内，
    /// 参见 <see cref="DataCandAtoms"/> 的 BillEntry / FilterNode / TradeRow）；本文件同构 DataCandFmt——
    /// <b>只放纯计算 Build 工厂、无 struct 定义</b>。绘制转发在 <see cref="OfficialDataCandAtoms"/>.Build 中，
    /// 本文件零绘制逻辑（GATE-ARCH：候选层复用 Atoms/Fmt）。</para>
    /// <para>契约（GATE-UI U1-U5）：Build 工厂纯计算、无状态；颜色默认溯源 Tokens（如 LetterCard tone=Tokens.Accent，
    /// 禁字面色相，U2）；字段类型严格对齐 OfficialDataAtoms* 的方法签名以保证 Draw 转发零转换。</para>
    /// </summary>
    public static class OfficialDataCandFmt
    {
        /// <summary>GeneChips 的 gap 默认值（6f），溯源 OfficialDataAtomsC.GeneChips(Rect, …, float gap = 6f) 位置参数默认值。</summary>
        private const float GeneChipsGapDefault = 6f;

        // ---- A 族 → OfficialDataAtoms ----

        public static OfficialDataCandAtoms.WealthBreakdownCand WealthBreakdownCand(string[] labels, float[] values, Color[] tones)
            => new OfficialDataCandAtoms.WealthBreakdownCand(labels, values, tones);

        public static OfficialDataCandAtoms.RecordsTableCand RecordsTableCand(string[] labels, string[] values)
            => new OfficialDataCandAtoms.RecordsTableCand(labels, values);

        public static OfficialDataCandAtoms.HistoryFeedCand HistoryFeedCand(string[] dates, string[] texts, string[] kinds)
            => new OfficialDataCandAtoms.HistoryFeedCand(dates, texts, kinds);

        public static OfficialDataCandAtoms.TaleCardCand TaleCardCand(string teller, string text, int times)
            => new OfficialDataCandAtoms.TaleCardCand(teller, text, times);

        // ---- B 族 → OfficialDataAtomsB ----

        public static OfficialDataCandAtoms.LetterCardCand LetterCardCand(string title, string summary, string time, Color? tone = null)
            => new OfficialDataCandAtoms.LetterCardCand(title, summary, time, tone ?? Tokens.Accent);

        public static OfficialDataCandAtoms.BillListCand BillListCand(string[] names, float[] done, float[] target, bool[] paused)
            => new OfficialDataCandAtoms.BillListCand(names, done, target, paused);

        public static OfficialDataCandAtoms.QuestBoardCand QuestBoardCand(string[] titles, float[] progress, float[] daysLeft)
            => new OfficialDataCandAtoms.QuestBoardCand(titles, progress, daysLeft);

        public static OfficialDataCandAtoms.TitleCardCand TitleCardCand(string name, string granted, string[] permits, bool[] ok)
            => new OfficialDataCandAtoms.TitleCardCand(name, granted, permits, ok);

        public static OfficialDataCandAtoms.PsycastTreeCand PsycastTreeCand(string[] names, int[] levels, bool[] unlocked)
            => new OfficialDataCandAtoms.PsycastTreeCand(names, levels, unlocked);

        // ---- C 族 → OfficialDataAtomsC ----

        public static OfficialDataCandAtoms.IdeoCardCand IdeoCardCand(string name, string leader, int members, string[] precepts)
            => new OfficialDataCandAtoms.IdeoCardCand(name, leader, members, precepts);

        public static OfficialDataCandAtoms.CaravanPanelCand CaravanPanelCand(string[] members, float load01, float foodDays, string eta)
            => new OfficialDataCandAtoms.CaravanPanelCand(members, load01, foodDays, eta);

        public static OfficialDataCandAtoms.GeneChipsCand GeneChipsCand(string[] genes, int[] complexity, bool[] positive, float? gap = null)
            => new OfficialDataCandAtoms.GeneChipsCand(genes, complexity, positive, gap ?? GeneChipsGapDefault);

        public static OfficialDataCandAtoms.MessageFeedCand MessageFeedCand(string[] lines, string[] times)
            => new OfficialDataCandAtoms.MessageFeedCand(lines, times);

        public static OfficialDataCandAtoms.PolicyCardCand PolicyCardCand(string name, string summary, int applied)
            => new OfficialDataCandAtoms.PolicyCardCand(name, summary, applied);
    }
}
