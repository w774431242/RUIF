using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>排序状态（三态循环：无 → 升 → 降 → 无）。</summary>
    public enum SortState { None, Asc, Desc }

    /// <summary>行密度三档（Numbers / Work Tab 实证）。</summary>
    public enum RowDensity { Compact, Normal, Cozy }

    /// <summary>表格原子的纯计算（无绘制依赖，可离线单测；热路径零分配）。</summary>
    public static class TableFmt
    {
        /// <summary>排序三态循环：无 → 升 → 降 → 无。</summary>
        public static SortState NextSort(SortState s)
            => s switch
            {
                SortState.None => SortState.Asc,
                SortState.Asc => SortState.Desc,
                _ => SortState.None
            };

        /// <summary>行密度 → 行高（Compact 22 / Normal 28 / Cozy 34）。</summary>
        public static float RowHeight(RowDensity d)
            => d switch { RowDensity.Compact => 22f, RowDensity.Cozy => 34f, _ => 28f };

        /// <summary>值/参考值 → 0~1（参考值非正返回 0）。</summary>
        public static float ValueRatio(float value, float reference)
            => reference <= 0f ? 0f : Mathf.Clamp01(value / reference);

        /// <summary>阈值带着色：&lt;0.34 低 / &lt;0.67 中 / 否则高（颜色取自令牌）。</summary>
        public static Color BandColor(float ratio01)
        {
            float r = Mathf.Clamp01(ratio01);
            if (r < 0.34f) return Tokens.Dead;
            if (r < 0.67f) return Tokens.Warn;
            return Tokens.Alive;
        }

        /// <summary>列宽拖动结果（钳制在 min~max）。</summary>
        public static float ResizeWidth(float startWidth, float dx, float min, float max)
            => Mathf.Clamp(startWidth + dx, Mathf.Max(1f, min), Mathf.Max(min + 1f, max));

        /// <summary>
        /// 按 shares 比例切列宽，写入调用方缓冲（<b>零分配</b>）；最后一列吃掉剩余宽度。
        /// </summary>
        public static void ColumnSplit(Rect row, float[] shares, float gap, float[] widths)
        {
            int n = Mathf.Min(shares?.Length ?? 0, widths?.Length ?? 0);
            if (n == 0) return;
            float used = 0f;
            for (int i = 0; i < n - 1; i++)
            {
                widths[i] = Mathf.Max(0f, shares[i] * row.width);
                used += widths[i] + gap;
            }
            widths[n - 1] = Mathf.Max(0f, row.width - used);
        }

        /// <summary>冻结列：返回右侧内容区的水平绘制偏移（负值表示内容左移）。</summary>
        public static float FrozenContentOffsetX(float scrollX) => -Mathf.Max(0f, scrollX);
    }

    /// <summary>
    /// 表格原子（批4 第一批，Numbers / Work Tab 实证）：排序表头、阈值着色单元、列宽手柄、冻结列、密度切换。
    /// 约束：不持有状态——排序态/列宽/拖拽态全部由调用方持有并传入（GATE-UI）。
    /// </summary>
    public static class TableAtoms
    {
        /// <summary>可点击排序表头；返回是否被点击（调用方用 <see cref="TableFmt.NextSort"/> 推进状态）。</summary>
        public static bool HeaderCell(Rect rect, string label, SortState state)
        {
            if (Mouse.IsOver(rect)) Widgets.DrawHighlight(rect);
            string arrow = state == SortState.Asc ? " ▲" : state == SortState.Desc ? " ▼" : "";
            Draw.Label(rect, (label ?? string.Empty) + arrow, Tokens.FontLabel,
                state == SortState.None ? Tokens.Muted : Tokens.Accent, TextAnchor.MiddleLeft);
            return Widgets.ButtonInvisible(rect);
        }

        /// <summary>阈值着色数值单元（底为带色 22% 叠加）。</summary>
        public static void ValueCell(Rect rect, float value, float reference)
        {
            Color c = TableFmt.BandColor(TableFmt.ValueRatio(value, reference));
            Draw.Box(rect, Draw.WithAlpha(c, 0.22f));
            Draw.Label(rect, value.ToString("0.#"), Tokens.FontLabel, c, TextAnchor.MiddleRight);
        }

        /// <summary>
        /// 列宽拖拽手柄（竖向把手）；返回是否按下。
        /// 拖拽过程与状态由调用方持有：按下记起点，按住时按 <see cref="TableFmt.ResizeWidth"/> 更新列宽。
        /// </summary>
        public static bool ResizeHandle(Rect area, float x)
        {
            var r = new Rect(x - 3f, area.y, 7f, area.height);
            Draw.Box(r, Tokens.BorderSoft);
            return Widgets.ButtonInvisible(r);
        }

        /// <summary>冻结首列：切出固定区与可滚区（内容绘制时套 <see cref="TableFmt.FrozenContentOffsetX"/>）。</summary>
        public static void FrozenSplit(Rect outer, float frozenWidth, out Rect frozen, out Rect scrollArea)
        {
            float ratio = Mathf.Clamp01(frozenWidth / Mathf.Max(1f, outer.width));
            Layout.Split(outer, ratio, 0f, out frozen, out scrollArea);
        }

        /// <summary>行密度三档切换；返回是否变化。</summary>
        public static bool DensitySwitch(Rect rect, ref RowDensity density)
        {
            var row = Layout.Row(rect, Tokens.SpaceXs);
            bool changed = false;
            if (Widgets.ButtonText(row.Take(56f), Tr.Get("RUIF.Comp.Density.Compact"), true, false, density != RowDensity.Compact)) { density = RowDensity.Compact; changed = true; }
            if (Widgets.ButtonText(row.Take(56f), Tr.Get("RUIF.Comp.Density.Normal"), true, false, density != RowDensity.Normal)) { density = RowDensity.Normal; changed = true; }
            if (Widgets.ButtonText(row.Take(56f), Tr.Get("RUIF.Comp.Density.Cozy"), true, false, density != RowDensity.Cozy)) { density = RowDensity.Cozy; changed = true; }
            return changed;
        }
    }
}
