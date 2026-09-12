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
                Draw.Label(pill, status, Tokens.FontLabel, Tokens.Alive, TextAnchor.MiddleCenter);
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
                Draw.Label(chip, traits[i] ?? string.Empty, Tokens.FontLabel, c, TextAnchor.MiddleCenter);
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
                Draw.Label(row.Take(72f), names[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Rect bar = row.Rest;
                Bar(bar, DataFmt.Bar01(values[i], maxValue), i < 3 ? Tokens.Accent : Tokens.Info);
                Draw.Label(bar, values[i].ToString("0"), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
            }
        }

        /// <summary>统计表（表头 + 斑马纹行 + 比例列宽）。</summary>
        public static void StatTable(Rect rect, string[] headers, string[][] rows, float[] shares, float rowH = 20f)
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
                    Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                hx += widths[i];
            }
            Draw.LineH(rect.x, rect.y + rowH, rect.width, Tokens.BorderSoft);

            int n = rows?.Length ?? 0;
            for (int r = 0; r < n; r++)
            {
                float y = rect.y + rowH + r * rowH;
                if (y + rowH > rect.yMax) break;
                if (r % 2 == 1) Draw.Box(new Rect(rect.x, y, rect.width, rowH), Draw.WithAlpha(Tokens.Text, 0.04f));
                float x = rect.x;
                for (int c = 0; c < headers.Length; c++)
                {
                    string text = rows[r] != null && c < rows[r].Length ? rows[r][c] : string.Empty;
                    Draw.Label(new Rect(x, y, widths[c], rowH), text, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
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
