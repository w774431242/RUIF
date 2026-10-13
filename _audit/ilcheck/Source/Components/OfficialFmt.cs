using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// 关系档位（**语义**，与文案解耦）。消费者可据此自行渲染文案/图标；
    /// 也让"分档边界"这一核心语义可以离线单测（文案函数依赖语言数据，测不了）。
    /// </summary>
    public enum RelationBand { Ally, Friendly, Neutral, Hostile, DeadHostile }

    /// <summary>官方数据系统组件的纯计算（可离线单测）。</summary>
    public static class OfficialFmt
    {
        /// <summary>声望 → 关系档位（边界：75/25/-25/-75）。</summary>
        public static RelationBand RelationBandOf(float g)
        {
            if (g >= 75f) return RelationBand.Ally;
            if (g >= 25f) return RelationBand.Friendly;
            if (g >= -25f) return RelationBand.Neutral;
            if (g >= -75f) return RelationBand.Hostile;
            return RelationBand.DeadHostile;
        }

        /// <summary>声望 → 关系文案（已本地化：<c>RUIF.Comp.Relation.*</c>）。</summary>
        public static string RelationLabel(float g)
        {
            switch (RelationBandOf(g))
            {
                case RelationBand.Ally: return Tr.Get("RUIF.Comp.Relation.Ally");
                case RelationBand.Friendly: return Tr.Get("RUIF.Comp.Relation.Friendly");
                case RelationBand.Neutral: return Tr.Get("RUIF.Comp.Relation.Neutral");
                case RelationBand.Hostile: return Tr.Get("RUIF.Comp.Relation.Hostile");
                default: return Tr.Get("RUIF.Comp.Relation.DeadHostile");
            }
        }

        /// <summary>声望 → 令牌色（友好绿 / 中性灰 / 敌对红）。</summary>
        public static Color RelationColor(float g)
        {
            if (g >= 25f) return Tokens.Alive;
            if (g >= -25f) return Tokens.Muted;
            return Tokens.Dead;
        }

        /// <summary>许可/条件勾叉色。</summary>
        public static Color CheckColor(bool ok) => ok ? Tokens.Alive : Tokens.Dead;

        /// <summary>账单完成度 0~1（目标非正 → 视为已完成）。</summary>
        public static float BillRatio(float done, float target)
            => target <= 0f ? 1f : Mathf.Clamp01(done / target);

        /// <summary>任务剩余天数文案（≤0 已到期；已本地化：<c>RUIF.Comp.Eta.*</c>）。</summary>
        public static string EtaLabel(float days)
            => days <= 0f
               ? Tr.Get("RUIF.Comp.Eta.Expired")
               : Tr.Get("RUIF.Comp.Eta.Remaining", days.ToString("0.#"));

        /// <summary>基因复杂度点数文案（正负均可）。</summary>
        public static string ComplexityLabel(int p) => (p >= 0 ? "+" : "") + p.ToString("0");

        /// <summary>财富总额。</summary>
        public static float WealthTotal(float[] parts) => DataExFmt.Sum(parts);

        /// <summary>十六进制色值（含 #）。</summary>
        public static string HexWithHash(Color c) => "#" + InputExFmt.Hex(c);
    }
}
