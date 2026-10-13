using UnityEngine;

namespace RUIF
{
    /// <summary>⑩ 工业域绑定候选组件的纯计算（可离线单测）。</summary>
    public static class DomainCandFmt
    {
        /// <summary>电路节点配色（0 发电 / 1 储能 / 2 负载）。</summary>
        public static Color NodeKindColor(int kind) => kind == 0 ? Tokens.Alive : kind == 1 ? Tokens.Accent : Tokens.Warn;

        /// <summary>格网单元配色（归一值 → 阈值三档）。</summary>
        public static Color CellTone(float norm01) => TableFmt.BandColor(norm01);

        /// <summary>生产阶段进度配色。</summary>
        public static Color ProgressTone(float p01) => TableFmt.BandColor(p01);
    }
}
