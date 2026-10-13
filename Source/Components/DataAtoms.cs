using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>数据展示组件的纯计算（可离线单测）。</summary>
    public static class DataFmt
    {
        /// <summary>值 / 上限 → 0~1（上限非正返回 0）。</summary>
        public static float Bar01(float value, float max)
            => max <= 0f ? 0f : Mathf.Clamp01(value / max);

        /// <summary>技能等级（0~20）→ 档位色（每 4 级一档，共 5 档）。</summary>
        public static Color SkillColor(float level)
        {
            float l = Mathf.Clamp(level, 0f, 20f);
            if (l < 4f) return Tokens.Dim;
            if (l < 8f) return Tokens.Muted;
            if (l < 12f) return Tokens.Alive;
            if (l < 16f) return Tokens.Info;
            return Tokens.Accent;
        }

        /// <summary>列表第 index 行的矩形（含行距）。</summary>
        public static Rect RowRect(Rect list, int index, float rowH, float gap = 0f)
            => new Rect(list.x, list.y + index * (rowH + gap), list.width, rowH);

        /// <summary>热力档位（0 = 空，1..levels 为强度档）。</summary>
        public static int HeatLevel(float ratio01, int levels = 4)
        {
            if (ratio01 <= 0f) return 0;
            int n = Mathf.Max(1, levels);
            return Mathf.Clamp(Mathf.CeilToInt(Mathf.Clamp01(ratio01) * n), 1, n);
        }

        /// <summary>名次徽记（前三有样式，其余返回序号）。</summary>
        public static string RankMedal(int rank)
            => rank switch { 0 => "①", 1 => "②", 2 => "③", _ => (rank + 1).ToString() };
    }

    /// <summary>
    /// 统计表**单列样式**：水平锚点 + 是否降调前景。
    ///
    /// <para>为值类型 / 无状态 —— 列样式表由调用方持有并复用（静态 readonly，绘制期零分配），
    /// 组件自身不记忆任何列状态（GATE-UI）。</para>
    /// <para>⚠ <see cref="Align"/> 只取**水平分量**（行内一律垂直居中）；零初始化的
    /// <c>default(StatCol)</c>（= <c>TextAnchor.UpperLeft</c>）会被归一化为左对齐，
    /// 见 <see cref="TableFmt.ColumnAlignOf"/>。</para>
    /// </summary>
    public struct StatCol
    {
        /// <summary>该列锚点（表头与数据格共用）。</summary>
        public TextAnchor Align;

        /// <summary>该列是否用降调前景（<see cref="Tokens.Dim"/>）—— 用于 `#` / 占比 / 参与季数这类次要列。</summary>
        public bool Dim;
    }

    /// <summary>
    /// 数据展示组件（S3-2，C1 中的纯 ✅ 件）：人物卡 / 技能条组 / 需求面板 / 特性芯片 /
    /// 健康行 / 热力日历 / 排行榜 / 统计表 / 时间轴。
    /// 无状态；数据全部由调用方传入。
    /// ⚠️ 未包含：MoodGauge 弧形 / GearSlots / InventoryGrid / RelationGraph / Charts（需素材或算法原型，v1.1）。
    /// </summary>
    public static class DataAtoms
    {
        /// <summary>人物卡：色块头像 + 名字 + 职称 + 心情/健康双条 + 状态胶囊。</summary>
        public static void PawnCard(Rect rect, string initial, string name, string title,
            float mood01, float health01, string status)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var row = Layout.Row(Layout.Inset(rect, 6f, 6f), Tokens.SpaceXs);

            float av = Mathf.Min(rect.height - 12f, 34f);
            Rect avatar = row.Take(av);
            Draw.Box(avatar, Tokens.Accent);
            Draw.Label(avatar, initial ?? string.Empty, Tokens.FontBody, Tokens.Window, TextAnchor.MiddleCenter);

            var col = Layout.Column(row.Rest, 2f);
            var line1 = Layout.Row(col.Take(20f), 6f);
            Draw.Label(line1.Rest, name ?? string.Empty, Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
            if (!string.IsNullOrEmpty(status))
            {
                float sw = 52f;
                Rect pill = new Rect(rect.xMax - 6f - sw, rect.y + 6f, sw, 16f);
                Draw.Box(pill, Draw.WithAlpha(Tokens.Alive, 0.18f));
                Draw.Border(pill, Tokens.Alive);
                // 字色不用 Tokens.Alive：底色是 Alive 的 18% 叠印 ⇒ 同色系叠同色系时状态字发虚。
                // 语义由底色 + 边框承载，字用 Text 保证可读（与 HistoryFeed 的 pill 同一处理）。
                Draw.EllipsisLabel(pill, status, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
            }
            Draw.Label(col.Take(14f), title ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            var bars = Layout.Row(col.Rest, 4f);
            Bar(bars.Take(60f), mood01, Tokens.Alive);
            Bar(bars.Take(60f), health01, Tokens.Info);
        }

        private static void Bar(Rect r, float v, Color c)
        {
            Draw.Box(r, Tokens.BorderSoft);
            Draw.Box(new Rect(r.x, r.y, r.width * Mathf.Clamp01(v), r.height), c);
        }

        /// <summary>技能条组（多列）：名称 + 等级 + 档位色条。</summary>
        public static void SkillBars(Rect rect, string[] names, float[] levels,
            float maxLevel = 20f, int columns = 2)
        {
            int cols = Mathf.Max(1, columns);
            int n = Mathf.Min(names?.Length ?? 0, levels?.Length ?? 0);
            if (n == 0) return;
            int rows = (n + cols - 1) / cols;
            float rowH = rect.height / Mathf.Max(1, rows);
            float colW = rect.width / cols;
            for (int i = 0; i < n; i++)
            {
                int r = i / cols, c = i % cols;
                var cell = new Rect(rect.x + c * colW, rect.y + r * rowH, colW - 6f, rowH - 2f);
                var row = Layout.Row(cell, 4f);
                Draw.Label(row.Take(48f), names[i] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                Rect bar = row.Rest;
                Bar(bar, DataFmt.Bar01(levels[i], maxLevel), DataFmt.SkillColor(levels[i]));
                Draw.Label(bar, levels[i].ToString("0"), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
            }
        }

        /// <summary>需求面板（心情/饮食/休息…）：名称 + 条 + 百分比。</summary>
        public static void NeedsPanel(Rect rect, string[] names, float[] values)
        {
            int n = Mathf.Min(names?.Length ?? 0, values?.Length ?? 0);
            if (n == 0) return;
            var col = Layout.Column(rect, 3f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                var row = Layout.Row(col.Take(rowH - 3f), 6f);
                Draw.Label(row.Take(56f), names[i] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                Rect bar = row.Rest;
                Color c = TableFmt.BandColor(values[i]);
                Bar(bar, values[i], c);
                Draw.Label(bar, (values[i] * 100f).ToString("0") + "%", Tokens.FontLabel, c, TextAnchor.MiddleRight);
            }
        }

        /// <summary>特性芯片（正向绿 / 负向红）。</summary>
        public static void TraitChips(Rect rect, string[] traits, bool[] positive, float gap = 6f)
        {
            int n = Mathf.Min(traits?.Length ?? 0, positive?.Length ?? 0);
            var row = Layout.Row(rect, gap);
            for (int i = 0; i < n; i++)
            {
                float w = 0f;
                using (Draw.Font(Tokens.FontLabel)) w = Text.CalcSize(traits[i] ?? string.Empty).x + 20f;
                Rect chip = row.Take(w);
                Color c = positive[i] ? Tokens.Alive : Tokens.Dead;
                Draw.Box(chip, Draw.WithAlpha(c, 0.16f));
                Draw.Border(chip, c);
                // 同 GeneChips：16% 语义底 + 100% 同色字 ⇒ 负向特性（Dead 红）在实机上读不清。
                // 正向/负向仍由底色与边框区分，字统一用 Text。
                Draw.Label(chip, traits[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
            }
        }

        /// <summary>健康状态行：部位 + 状态 + 严重度条。</summary>
        public static void HealthRows(Rect rect, string[] parts, string[] states, float[] severity)
        {
            int n = Mathf.Min(parts?.Length ?? 0, Mathf.Min(states?.Length ?? 0, severity?.Length ?? 0));
            if (n == 0) return;
            var col = Layout.Column(rect, 2f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                Rect row = col.Take(rowH - 2f);
                if (i % 2 == 1) Draw.Box(row, Draw.WithAlpha(Tokens.Text, 0.04f));
                var r = Layout.Row(row, 6f);
                Draw.Label(r.Take(70f), parts[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Color c = TableFmt.BandColor(1f - Mathf.Clamp01(severity[i]));
                Draw.Label(r.Take(90f), states[i] ?? string.Empty, Tokens.FontLabel, c, TextAnchor.MiddleLeft);
                Bar(r.Rest, severity[i], c);
            }
        }

        /// <summary>热力日历（按值分档着色，零素材）。</summary>
        public static void HeatCalendar(Rect rect, float[] values, int perRow, float cell = 12f, float gap = 3f)
        {
            if (values == null || perRow <= 0) return;
            for (int i = 0; i < values.Length; i++)
            {
                int r = i / perRow, c = i % perRow;
                var cellRect = new Rect(rect.x + c * (cell + gap), rect.y + r * (cell + gap), cell, cell);
                int lv = DataFmt.HeatLevel(values[i], 4);
                if (lv == 0) { Draw.Box(cellRect, Draw.WithAlpha(Tokens.Text, 0.05f)); continue; }
                float a = 0.25f + 0.75f * (lv / 4f);
                Draw.Box(cellRect, Draw.WithAlpha(Tokens.Alive, a));
            }
        }

        /// <summary>排行榜（前三带徽记 + 数值条）。</summary>
        public static void RankList(Rect rect, string[] names, float[] values, float maxValue)
        {
            int n = Mathf.Min(names?.Length ?? 0, values?.Length ?? 0);
            if (n == 0) return;
            var col = Layout.Column(rect, 2f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                var row = Layout.Row(col.Take(rowH - 2f), 6f);
                Draw.Label(row.Take(22f), DataFmt.RankMedal(i), Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleCenter);
                // 名字列固定 72px：中文名「蓝 雪牛」约 36px 放得下，但拉丁/带称号的榜单条目
                // （如「Blue Snow Cow」）会被 Draw.Label 硬裁成半截字 —— 改省略号，末位可见。
                Draw.EllipsisLabel(row.Take(72f), names[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Rect bar = row.Rest;
                Bar(bar, DataFmt.Bar01(values[i], maxValue), i < 3 ? Tokens.Accent : Tokens.Info);
                Draw.Label(bar, values[i].ToString("0"), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
            }
        }

        /// <summary>
        /// 统计表（表头 + 斑马纹行 + 比例列宽）—— **v1.1.2 兼容重载**（5 参）。
        ///
        /// <para><b>为什么必须是独立方法（而不是把后续参数写成可选）</b>：C# 可选参数只是**编译期**
        /// 特性 —— 调用点会把默认值展开成**全部实参**写进 IL。若只保留 10 参版本，1.1.2 编译的
        /// 消费者（含工坊外部 Mod）升级到 1.2.0 后，其 IL 仍按 5 参签名查找 ⇒
        /// <c>MissingMethodException</c>。故本 5 参签名**作为真正的方法保留**，委托到 10 参实现
        /// （单一实现体，见下）。</para>
        ///
        /// <para>语义与 v1.1.2 逐字一致：全列 <see cref="TextAnchor.MiddleLeft"/> +
        /// <see cref="Tokens.Muted"/> / <see cref="Tokens.Text"/>，无逐列样式、无阈值带色。</para>
        /// </summary>
        public static void StatTable(Rect rect, string[] headers, string[][] rows, float[] shares, float rowH = 20f)
            => StatTable(rect, headers, rows, shares, rowH, null, -1, 0f, null, null);

        /// <summary>
        /// 统计表（表头 + 斑马纹行 + 比例列宽），可选**逐列样式**与**阈值带色列**。
        ///
        /// <para>⚠ <b>本重载所有参数均无默认值</b>（关键）：若此处带默认值，4 / 5 实参调用会与上面的
        /// 5 参重载产生 <c>CS0121</c> 重载歧义。唯一性自检 —— 4 实参 → 只匹配 5 参重载；
        /// 5 实参 → 5 参重载无需填默认、本重载要再填 5 个 ⇒ 选 5 参重载；10 实参 → 只匹配本重载。</para>
        ///
        /// <para><b>列样式</b>（<paramref name="cols"/>）：表头单元格与数据格**共用同列锚点**
        /// （预览数值列表头亦右对齐）；<see cref="StatCol.Dim"/> 的列数据格走
        /// <see cref="Tokens.Dim"/>（表头色不变）。越界列回退 <see cref="TextAnchor.MiddleLeft"/>。</para>
        ///
        /// <para><b>阈值带色列</b>（<paramref name="bandCol"/> ≥ 0）：该列每格走
        /// <see cref="TableAtoms.ValueCell(Rect,string,float,float)"/> ⇒ 底色 =
        /// <see cref="TableFmt.BandColor"/> 的 22% 叠加、右对齐带色字，文本优先
        /// <paramref name="bandTexts"/>、缺失回退 <c>"0.#"</c>。带色格**收成行内药丸**
        /// （高 = <see cref="Measure.RowHeight(GameFont)"/> 的标签档、垂直居中），与预览 `.val{padding:1px 4px}`
        /// 的紧凑色块一致 —— 不做满行高色块。</para>
        ///
        /// <para>契约不变：不读 <c>Mouse</c>/<c>Event</c>；**行越出 rect 底边即 break** ⇒ 调用方按
        /// <c>(1 + rows) * rowH</c> 预留；列宽按 <paramref name="shares"/> 比例切分
        /// （<see cref="TableFmt.ColumnSplit"/>，sum 不必为 1，末列吃掉余宽）。</para>
        /// </summary>
        public static void StatTable(Rect rect, string[] headers, string[][] rows, float[] shares, float rowH,
            StatCol[] cols, int bandCol, float bandRef, float[] bandValues, string[] bandTexts)
        {
            if (headers == null) return;
            float[] widths = new float[headers.Length];
            TableFmt.ColumnSplit(rect, shares ?? MakeEven(headers.Length), 0f, widths);

            var headRow = new Rect(rect.x, rect.y, rect.width, rowH);
            Draw.Box(headRow, Tokens.PanelRaised);
            float hx = rect.x;
            for (int i = 0; i < headers.Length; i++)
            {
                Draw.Label(new Rect(hx, rect.y, widths[i], rowH), headers[i] ?? string.Empty,
                    Tokens.FontLabel, Tokens.Muted, TableFmt.ColumnAlignOf(cols, i));
                hx += widths[i];
            }
            Draw.LineH(rect.x, rect.y + rowH, rect.width, Tokens.BorderSoft);

            // 带色格的药丸高度（行内垂直居中）：取标签行高，且不得超过行高。
            float bandH = Mathf.Min(rowH, Measure.RowHeight(Tokens.FontLabel));
            float bandTop = (rowH - bandH) * 0.5f;

            int n = rows?.Length ?? 0;
            for (int r = 0; r < n; r++)
            {
                float y = rect.y + rowH + r * rowH;
                if (y + rowH > rect.yMax) break;
                if (r % 2 == 1) Draw.Box(new Rect(rect.x, y, rect.width, rowH), Draw.WithAlpha(Tokens.Text, 0.04f));
                float x = rect.x;
                for (int c = 0; c < headers.Length; c++)
                {
                    if (TableFmt.IsBandCol(bandCol, c))
                    {
                        float value = bandValues != null && r < bandValues.Length ? bandValues[r] : 0f;
                        TableAtoms.ValueCell(new Rect(x, y + bandTop, widths[c], bandH),
                            TableFmt.BandTextOf(bandValues, bandTexts, r), value, bandRef);
                        x += widths[c];
                        continue;
                    }
                    string text = rows[r] != null && c < rows[r].Length ? rows[r][c] : string.Empty;
                    Draw.Label(new Rect(x, y, widths[c], rowH), text, Tokens.FontLabel,
                        TableFmt.IsDimColumn(cols, c) ? Tokens.Dim : Tokens.Text,
                        TableFmt.ColumnAlignOf(cols, c));
                    x += widths[c];
                }
            }
        }

        private static float[] MakeEven(int n)
        {
            var arr = new float[Mathf.Max(1, n)];
            float v = 1f / Mathf.Max(1, n);
            for (int i = 0; i < arr.Length; i++) arr[i] = v;
            return arr;
        }

        /// <summary>时间轴（节点 + 日期 + 文本，按类型取色）。</summary>
        public static void TimelinePro(Rect rect, string[] dates, string[] texts, Color[] kinds, float rowH = 22f)
        {
            int n = Mathf.Min(dates?.Length ?? 0, Mathf.Min(texts?.Length ?? 0, kinds?.Length ?? 0));
            if (n == 0) return;
            float spineX = rect.x + 8f;
            Draw.LineV(spineX, rect.y, rect.height, Tokens.BorderSoft);
            for (int i = 0; i < n; i++)
            {
                float y = rect.y + i * rowH;
                if (y + rowH > rect.yMax) break;
                Color c = kinds[i];
                Draw.Box(new Rect(spineX - 4f, y + rowH / 2f - 4f, 8f, 8f), c);
                Draw.Label(new Rect(spineX + 12f, y, 68f, rowH), dates[i] ?? string.Empty,
                    Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                Draw.Label(new Rect(spineX + 84f, y, Mathf.Max(0f, rect.width - 92f), rowH),
                    texts[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            }
        }
    }
}
