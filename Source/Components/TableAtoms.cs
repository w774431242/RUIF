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

        /// <summary>
        /// 阈值带着色：&lt;0.34 低 / &lt;0.67 中 / 否则高（颜色取自令牌）。
        /// <para>边界：0.34 与 0.67 归入**高档**（<c>&lt;</c> 开区间下界）。</para>
        /// </summary>
        public static Color BandColor(float ratio01)
        {
            float r = Mathf.Clamp01(ratio01);
            if (r < 0.34f) return Tokens.Dead;
            if (r < 0.67f) return Tokens.Warn;
            return Tokens.Alive;
        }

        /// <summary>
        /// 列样式表 → 该列**表头/数据格**共用的锚点（预览数值列表头亦右对齐）。
        ///
        /// <para>越界 / <paramref name="cols"/> 为 null ⇒ <see cref="TextAnchor.MiddleLeft"/>
        /// （与无列样式时的旧行为完全一致）。</para>
        /// <para>锚点**只保留水平分量**（行内一律垂直居中）：<c>default(TextAnchor)</c> 是
        /// <c>UpperLeft</c>，零初始化的 <c>StatCol</c> 若不归一化会把文本顶到行顶。</para>
        /// </summary>
        public static TextAnchor ColumnAlignOf(StatCol[] cols, int col)
        {
            if (cols == null || col < 0 || col >= cols.Length) return TextAnchor.MiddleLeft;
            switch (cols[col].Align)
            {
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                case TextAnchor.LowerCenter:
                    return TextAnchor.MiddleCenter;
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                case TextAnchor.LowerRight:
                    return TextAnchor.MiddleRight;
                default:                       // MiddleLeft / UpperLeft / LowerLeft / 零初始化
                    return TextAnchor.MiddleLeft;
            }
        }

        /// <summary>该列是否用 <see cref="Tokens.Dim"/> 前景（预览 `#` / 占比 / 参与季数列为 `--dim`）。
        /// 越界 / 无列样式 ⇒ false（= 旧行为：统一 <see cref="Tokens.Text"/>）。</summary>
        public static bool IsDimColumn(StatCol[] cols, int col)
            => cols != null && col >= 0 && col < cols.Length && cols[col].Dim;

        /// <summary>该列是否走阈值带色格。<paramref name="bandCol"/> &lt; 0 表示**无带色列**。</summary>
        public static bool IsBandCol(int bandCol, int col)
            => bandCol >= 0 && col == bandCol;

        /// <summary>
        /// 带色列的**显示文本**：优先调用方提供的文本（如千分位 <c>6,980</c>）；
        /// 缺失（null / 空 / 下标越界）回退 <c>(value*scale).ToString("0.#")</c>；两者都缺 ⇒ 空串。
        /// <para><paramref name="scale"/> 供"比例值按百分比显示"等场景换算（默认 1 = 原值）。</para>
        /// </summary>
        public static string BandTextOf(float[] values, string[] texts, int row, float scale = 1f)
        {
            if (row < 0) return string.Empty;
            if (texts != null && row < texts.Length && !string.IsNullOrEmpty(texts[row]))
            {
                return texts[row];
            }
            if (values != null && row < values.Length)
            {
                return (values[row] * scale).ToString("0.#");
            }
            return string.Empty;
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

        /// <summary>
        /// 阈值着色数值单元（**旧签名 · 保持字节级等价**）：文本由 <c>value.ToString("0.#")</c> 生成。
        /// 委托到文本透传重载（单一实现）。
        /// </summary>
        public static void ValueCell(Rect rect, float value, float reference)
            => ValueCell(rect, value.ToString("0.#"), value, reference);

        /// <summary>
        /// 阈值着色数值单元（**文本透传**）：底为带色 22% 叠加、文字用同色右对齐。
        /// <para>给千分位等**已格式化**文本用（如 <c>"6,980"</c>）——格式化规则归调用方（LOC-002：
        /// 数字格式与语言相关，不写死在组件里）。</para>
        /// </summary>
        public static void ValueCell(Rect rect, string text, float value, float reference)
        {
            Color c = TableFmt.BandColor(TableFmt.ValueRatio(value, reference));
            Draw.Box(rect, Draw.WithAlpha(c, 0.22f));
            Draw.Label(rect, text ?? string.Empty, Tokens.FontLabel, c, TextAnchor.MiddleRight);
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
