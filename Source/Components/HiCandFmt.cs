using UnityEngine;

namespace RUIF
{
    /// <summary>
    /// ⑥ 新功能脉冲高亮候选层（纯桥接转发）：<see cref="HiAtoms"/> 桥接 <c>Verse.UIHighlighter</c>，无样式字段，
    /// 故仅保留 Fmt 静态转发壳，不强造空 struct（NOTE-L2）。绘制逻辑零复制，全部转发 HiAtoms。
    ///
    /// <para><b>跨帧协议（与原 Atoms 一致）</b>：本帧调 <see cref="Tag"/> 登记，下一帧在目标控件处调
    /// <see cref="Opportunity"/> 才画脉冲光晕；单独调 Opportunity 无效果。</para>
    /// </summary>
    public static class HiCandFmt
    {
        /// <summary>登记一个待高亮项（本帧调用；tag 用稳定字符串，如 "RUIF.Panel.Research"）。</summary>
        public static void Tag(string tag) => HiAtoms.Tag(tag);

        /// <summary>在目标控件处绘制脉冲高亮（须在 <see cref="Tag"/> 的下一帧调用）。</summary>
        public static void Opportunity(Rect rect, string tag) => HiAtoms.Opportunity(rect, tag);
    }
}
