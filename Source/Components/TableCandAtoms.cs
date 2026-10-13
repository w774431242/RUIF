using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>表格体系对接候选组件（原 📋，官方实证支持，现落地）：把 <c>TableAtoms</c> 接入
    /// <c>MainTabWindow_PawnTable</c>（PawnTableDef 表格体系）的 UI 脚手架——可排序表头 + 冻结首列 +
    /// 密度切换的殖民民表框架。纯 presentational；真实 Pawn 数据绑定属集成件。
    /// 官方依据：<c>MainTabWindow_PawnTable</c> / <c>PawnTableDef</c> / <c>PawnColumnWorker</c>。</summary>
    public static class TableCandAtoms
    {
        /// <summary>殖民民表框架：排序表头（点击推进三态）→ 冻结首列 + 密度感知的数据行。</summary>
        public static void PawnTableFrame(Rect rect, string[] headers, string[][] rows,
            ref int sortCol, SortState[] sorts, ref RowDensity density)
        {
            if (headers == null || headers.Length == 0) return;
            var col = Layout.Column(rect, 2f);

            Rect dens = col.Take(26f);
            TableAtoms.DensitySwitch(dens, ref density);

            float rowH = TableFmt.RowHeight(density);
            Rect head = col.Take(rowH);
            float[] shares = new float[headers.Length];
            for (int i = 0; i < headers.Length; i++) shares[i] = 1f / headers.Length;
            float[] widths = new float[headers.Length];
            TableFmt.ColumnSplit(head, shares, 2f, widths);
            float x = head.x;
            for (int i = 0; i < headers.Length; i++)
            {
                Rect hc = new Rect(x, head.y, widths[i], head.height);
                SortState st = sorts != null && i < sorts.Length ? sorts[i] : SortState.None;
                if (TableAtoms.HeaderCell(hc, headers[i], st) && sorts != null && i < sorts.Length)
                {
                    sortCol = i;
                    sorts[i] = TableFmt.NextSort(sorts[i]);
                }
                x += widths[i] + 2f;
            }
            Draw.Border(head, Tokens.BorderSoft);

            Rect body = col.Rest;
            int n = rows == null ? 0 : rows.Length;
            int visible = Mathf.Max(1, Mathf.FloorToInt(body.height / rowH));
            for (int r = 0; r < Mathf.Min(n, visible); r++)
            {
                Rect rr = new Rect(body.x, body.y + r * rowH, body.width, rowH);
                if (r % 2 == 1) Draw.Box(rr, Tokens.Card);
                float cx = rr.x;
                for (int c = 0; c < headers.Length; c++)
                {
                    Rect cc = new Rect(cx, rr.y, widths[c], rr.height);
                    string v = rows[r] != null && c < rows[r].Length ? rows[r][c] : string.Empty;
                    Draw.Label(cc, v, Tokens.FontLabel, c == 0 ? Tokens.Text : Tokens.Muted,
                        c == 0 ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight);
                    cx += widths[c] + 2f;
                }
            }
        }
    }
}
