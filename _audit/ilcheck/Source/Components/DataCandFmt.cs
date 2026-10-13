using UnityEngine;

namespace RUIF
{
    /// <summary>⑤ 数据展示候选组件的纯计算（可离线单测）。</summary>
    public static class DataCandFmt
    {
        /// <summary>账单进度（目标非正返回 0，钳制 0~1）。</summary>
        public static float BillRatio(int done, int target)
            => target <= 0 ? 0f : Mathf.Clamp01((float)done / target);

        /// <summary>树节点行矩形（按 depth 缩进）。</summary>
        public static Rect TreeRow(Rect list, int index, float rowH, int depth, float indent = 14f)
            => new Rect(list.x + depth * indent, list.y + index * rowH, list.width - depth * indent, rowH);

        /// <summary>树节点左侧操作区（caret / checkbox）宽度。</summary>
        public const float TreeOpW = 18f;

        /// <summary>库存/资源计数 → 语义色（0 灰，其余按规模取 Accent/Info/Alive）。</summary>
        public static Color CountTone(int count)
        {
            if (count <= 0) return Tokens.Dim;
            if (count < 25) return Tokens.Info;
            if (count < 100) return Tokens.Alive;
            return Tokens.Accent;
        }

        /// <summary>贸易差额色（我方盈余绿 / 逆差红）。</summary>
        public static Color TradeTone(int delta) => delta >= 0 ? Tokens.Alive : Tokens.Dead;
    }
}
