using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>C2 数据扩展组件的纯计算（可离线单测）。</summary>
    public static class DataExFmt
    {
        /// <summary>L 形连线拐点：from 到 (to.x, from.y) 再到 to。</summary>
        public static Vector2 LinkMid(Vector2 from, Vector2 to) => new Vector2(to.x, from.y);

        /// <summary>节点矩形（网格布局）。</summary>
        public static Rect NodeRect(Rect canvas, int col, int row, float nodeW, float nodeH, float gap)
            => new Rect(canvas.x + col * (nodeW + gap), canvas.y + row * (nodeH + gap), nodeW, nodeH);

        /// <summary>停靠落区判定：0 无 / 1 左 / 2 右 / 3 上 / 4 下。</summary>
        public static int DockZone(Rect screen, float edge, Vector2 pointer)
        {
            if (pointer.x < screen.x + edge) return 1;
            if (pointer.x > screen.xMax - edge) return 2;
            if (pointer.y < screen.y + edge) return 3;
            if (pointer.y > screen.yMax - edge) return 4;
            return 0;
        }

        /// <summary>按名查预设下标（未找到返回 -1）。</summary>
        public static int PresetIndex(string[] names, string name)
        {
            if (names == null) return -1;
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == name) return i;
            }
            return -1;
        }

        /// <summary>数组求和（null 视为 0）。</summary>
        public static float Sum(float[] values)
        {
            float t = 0f;
            if (values == null) return t;
            for (int i = 0; i < values.Length; i++)
            {
                t += values[i];
            }
            return t;
        }

        /// <summary>完成比例（总量非正返回 0，钳制 0~1）。</summary>
        public static float Ratio(float done, float total) => total <= 0f ? 0f : Mathf.Clamp01(done / total);
        /// <summary>占比（总量非正返回 0，钳制 0~1）。</summary>
        public static float Share(float value, float total) => total <= 0f ? 0f : Mathf.Clamp01(value / total);
        /// <summary>净收支 = 产出 − 消耗。</summary>
        public static float NetBalance(float produced, float consumed) => produced - consumed;
        /// <summary>两值比较：相等 0 / 前者大 1 / 后者大 −1（浮点容差）。</summary>
        public static int CompareWinner(float a, float b) => Mathf.Abs(a - b) < 0.0001f ? 0 : (a > b ? 1 : -1);

        /// <summary>声望 → 令牌色（负值敌对红，否则绿）。</summary>
        public static Color RelationTone(float g) => g < 0f ? Tokens.Dead : Tokens.Alive;

        /// <summary>训练度 0~5 → 令牌色。</summary>
        public static Color TrainingTone(int level) => level <= 0 ? Tokens.Dim : (level >= 4 ? Tokens.Accent : Tokens.Info);

        /// <summary>天气种类 → 令牌色（0 晴 1 雨 2 雾 3 雪 4 毒雾）。</summary>
        public static Color WeatherTone(int kind)
        {
            switch (kind)
            {
                case 0: return Tokens.Accent;
                case 1: return Tokens.Info;
                case 2: return Tokens.Muted;
                case 3: return Tokens.Text;
                default: return Tokens.Warn;
            }
        }

        /// <summary>构成比例写入调用方缓冲（零分配）。</summary>
        public static void ShareRatios(float[] values, float[] ratios)
        {
            float total = Sum(values);
            int n = Mathf.Min(values == null ? 0 : values.Length, ratios == null ? 0 : ratios.Length);
            for (int i = 0; i < n; i++)
            {
                ratios[i] = Share(values[i], total);
            }
        }
    }

    /// <summary>
    /// C2 数据扩展组件（S3 任务③）：节点图 / 停靠宿主 / 预设档案 / 增减益行 / 任务卡 / 阵营卡 / 生物卡 / 房间属性 / 电力概览 / 天气条带 / 库存构成 / 对比条。
    /// 无状态；数据与选中态由调用方持有。
    /// </summary>
    public static class DataExAtoms
    {
        /// <summary>节点图（ResearchPal 实证）：节点 + L 形连线；返回是否有点击。</summary>
        public static bool NodeGraph(Rect canvas, string[] labels, int[] colOf, int[] rowOf, int[] from, int[] to,
            int[] state, ref int selected, float nodeW = 96f, float nodeH = 30f, float gap = 14f)
        {
            int n = labels == null ? 0 : labels.Length;
            if (n == 0)
            {
                return false;
            }
            if (from != null && to != null)
            {
                int m = Mathf.Min(from.Length, to.Length);
                for (int e = 0; e < m; e++)
                {
                    int a = from[e];
                    int b = to[e];
                    if (a < 0 || b < 0 || a >= n || b >= n)
                    {
                        continue;
                    }
                    Rect ra = DataExFmt.NodeRect(canvas, colOf[a], rowOf[a], nodeW, nodeH, gap);
                    Rect rb = DataExFmt.NodeRect(canvas, colOf[b], rowOf[b], nodeW, nodeH, gap);
                    Vector2 p1 = new Vector2(ra.xMax, ra.center.y);
                    Vector2 p2 = new Vector2(rb.x, rb.center.y);
                    Vector2 mid = DataExFmt.LinkMid(p1, p2);
                    bool on = state != null && a < state.Length && state[a] > 0;
                    Color lc = on ? Tokens.Accent : Tokens.BorderSoft;
                    Draw.Box(new Rect(p1.x, p1.y - 1f, mid.x - p1.x, 2f), lc);
                    Draw.Box(new Rect(mid.x - 1f, Mathf.Min(mid.y, p2.y), 2f, Mathf.Abs(p2.y - mid.y)), lc);
                }
            }
            bool any = false;
            for (int i = 0; i < n; i++)
            {
                Rect r = DataExFmt.NodeRect(canvas, colOf[i], rowOf[i], nodeW, nodeH, gap);
                int st = state != null && i < state.Length ? state[i] : 0;
                Draw.Box(r, st > 0 ? Draw.WithAlpha(Tokens.Accent, 0.18f) : Tokens.PanelRaised);
                Draw.Border(r, st > 0 ? Tokens.Accent : Tokens.BorderSoft);
                Draw.Label(r, labels[i] ?? string.Empty, Tokens.FontLabel, st > 0 ? Tokens.Accent : Tokens.Muted, TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(r))
                {
                    selected = i;
                    any = true;
                }
            }
            return any;
        }

        /// <summary>停靠宿主（RimHUD 实证）：面板 + 折叠把手 + 吸附落区提示；返回落区（0 无）。</summary>
        public static int DockHost(Rect area, string title, ref bool collapsed, float edge = 24f)
        {
            Draw.Box(area, Tokens.Panel);
            Draw.Border(area, Tokens.BorderSoft);
            Rect head = new Rect(area.x, area.y, area.width, 20f);
            Draw.Box(head, Tokens.PanelRaised);
            string label = (collapsed ? "▸ " : "▾ ") + (title ?? string.Empty);
            Draw.Label(new Rect(head.x + 8f, head.y, head.width - 30f, 20f), label, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            if (Widgets.ButtonInvisible(head))
            {
                collapsed = !collapsed;
            }
            if (collapsed)
            {
                return 0;
            }
            int zone = DataExFmt.DockZone(area, edge, Event.current.mousePosition);
            if (zone > 0)
            {
                Rect hint;
                if (zone == 1)
                {
                    hint = new Rect(area.x, area.y, 6f, area.height);
                }
                else if (zone == 2)
                {
                    hint = new Rect(area.xMax - 6f, area.y, 6f, area.height);
                }
                else if (zone == 3)
                {
                    hint = new Rect(area.x, area.y, area.width, 6f);
                }
                else
                {
                    hint = new Rect(area.x, area.yMax - 6f, area.width, 6f);
                }
                Draw.Box(hint, Draw.WithAlpha(Tokens.Accent, 0.55f));
            }
            return zone;
        }

        /// <summary>预设档案（Work Tab 实证）：套用 / 存为；返回是否有操作。</summary>
        public static bool ProfilePreset(Rect rect, string[] names, ref int index, out bool save)
        {
            save = false;
            var row = Layout.Row(rect, Tokens.SpaceXs);
            Draw.Label(row.Take(56f), Tr.Get("RUIF.Comp.Common.Preset"), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            string cur = names != null && index >= 0 && index < names.Length ? names[index] : "—";
            Draw.Label(row.Rest, cur, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            if (Widgets.ButtonText(new Rect(rect.xMax - 132f, rect.y, 62f, rect.height), Tr.Get("RUIF.Comp.Common.Apply")))
            {
                save = false;
                return true;
            }
            if (Widgets.ButtonText(new Rect(rect.xMax - 66f, rect.y, 66f, rect.height), Tr.Get("RUIF.Comp.Common.SaveAs")))
            {
                save = true;
                return true;
            }
            return false;
        }

        /// <summary>增减益行：标签 + 数值（正绿负红）。</summary>
        public static void BuffRow(Rect rect, string label, float value)
        {
            var row = Layout.Row(rect, Tokens.SpaceXs);
            Draw.Label(row.Rest, label ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Color c = value >= 0f ? Tokens.Alive : Tokens.Dead;
            string s = (value >= 0f ? "+" : "") + value.ToString("0.#");
            Draw.Label(new Rect(rect.xMax - 64f, rect.y, 64f, rect.height), s, Tokens.FontLabel, c, TextAnchor.MiddleRight);
        }

        /// <summary>任务卡：标题 + 进度条 + 目标清单。</summary>
        public static void QuestCard(Rect rect, string title, int done, int total, string[] objectives)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 8f, 6f), 3f);
            Draw.Label(col.Take(20f), title ?? string.Empty, Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
            Rect bar = col.Take(10f);
            Draw.Box(bar, Tokens.BorderSoft);
            Draw.Box(new Rect(bar.x, bar.y, bar.width * DataExFmt.Ratio(done, total), bar.height), Tokens.Accent);
            if (objectives == null) return;
            for (int i = 0; i < objectives.Length; i++)
            {
                Draw.Label(col.Take(16f), "· " + (objectives[i] ?? string.Empty), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            }
        }

        /// <summary>阵营卡：名称 + 声望条（负值红）。</summary>
        public static void FactionCard(Rect rect, string name, float goodwill)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 8f, 6f), 3f);
            Draw.Label(col.Take(20f), name ?? string.Empty, Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
            Rect bar = col.Rest;
            Draw.Box(bar, Tokens.BorderSoft);
            Draw.Box(new Rect(bar.x, bar.y, bar.width * Mathf.Clamp01(Mathf.Abs(goodwill) / 100f), bar.height), DataExFmt.RelationTone(goodwill));
        }

        /// <summary>生物卡：名称 + 训练度 + 绑定标记。</summary>
        public static void CreatureCard(Rect rect, string name, int training, bool bonded)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var col = Layout.Column(Layout.Inset(rect, 8f, 6f), 3f);
            string s = (bonded ? "★ " : "") + (name ?? string.Empty);
            Draw.Label(col.Take(20f), s, Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
            Rect bar = col.Take(10f);
            Draw.Box(bar, Tokens.BorderSoft);
            Draw.Box(new Rect(bar.x, bar.y, bar.width * (training / 5f), bar.height), DataExFmt.TrainingTone(training));
        }

        /// <summary>房间属性条组（印象/清洁/空间/美观）。</summary>
        public static void RoomStats(Rect rect, string[] names, float[] metrics)
        {
            int n = Mathf.Min(names == null ? 0 : names.Length, metrics == null ? 0 : metrics.Length);
            if (n == 0) return;
            var col = Layout.Column(rect, 2f);
            float rowH = rect.height / n;
            for (int i = 0; i < n; i++)
            {
                var row = Layout.Row(col.Take(rowH - 2f), 6f);
                Draw.Label(row.Take(60f), names[i] ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
                Rect bar = row.Rest;
                Draw.Box(bar, Tokens.BorderSoft);
                Draw.Box(new Rect(bar.x, bar.y, bar.width * Mathf.Clamp01(metrics[i]), bar.height), TableFmt.BandColor(metrics[i]));
            }
        }

        /// <summary>电力概览（净值 + 电池水位条）。</summary>
        public static void PowerGrid(Rect rect, float produced, float consumed, float stored, float capacity)
        {
            var col = Layout.Column(rect, 3f);
            float net = DataExFmt.NetBalance(produced, consumed);
            string s = Tr.Get("RUIF.Comp.Power.Net", (net >= 0f ? "+" : "") + net.ToString("0"));
            Draw.Label(col.Take(16f), s, Tokens.FontLabel, net >= 0f ? Tokens.Alive : Tokens.Warn, TextAnchor.MiddleLeft);
            Rect bar = col.Take(12f);
            Draw.Box(bar, Tokens.BorderSoft);
            float share = capacity <= 0f ? 0f : Mathf.Clamp01(stored / capacity);
            Draw.Box(new Rect(bar.x, bar.y, bar.width * share, bar.height), Tokens.Accent);
        }

        /// <summary>天气条带（当前 + 预报）。</summary>
        public static void WeatherStrip(Rect rect, string[] labels, int[] kinds)
        {
            int n = Mathf.Min(labels == null ? 0 : labels.Length, kinds == null ? 0 : kinds.Length);
            if (n == 0) return;
            var row = Layout.Row(rect, 3f);
            for (int i = 0; i < n; i++)
            {
                Rect cell = row.Take(rect.width / n - 3f);
                Color tone = DataExFmt.WeatherTone(kinds[i]);
                Draw.Box(cell, Draw.WithAlpha(tone, 0.22f));
                Draw.Border(cell, tone);
                Draw.Label(cell, labels[i] ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
            }
        }

        /// <summary>库存构成堆叠条 + 图例（零素材）。</summary>
        public static void StorageBreakdown(Rect rect, string[] labels, float[] values, Color[] tones)
        {
            int n = Mathf.Min(labels == null ? 0 : labels.Length, values == null ? 0 : values.Length);
            if (n == 0) return;
            float[] ratios = new float[n];
            DataExFmt.ShareRatios(values, ratios);
            Rect bar = new Rect(rect.x, rect.y, rect.width, 14f);
            Draw.Box(bar, Tokens.BorderSoft);
            float x = bar.x;
            for (int i = 0; i < n; i++)
            {
                float w = bar.width * ratios[i];
                Color tone = tones != null && i < tones.Length ? tones[i] : Tokens.Info;
                Draw.Box(new Rect(x, bar.y, w, bar.height), tone);
                x += w;
            }
            var leg = Layout.Row(new Rect(rect.x, rect.y + 18f, rect.width, 14f), 8f);
            for (int i = 0; i < n; i++)
            {
                Rect item = leg.Take(rect.width / n - 8f);
                Color tone = tones != null && i < tones.Length ? tones[i] : Tokens.Info;
                Draw.Box(new Rect(item.x, item.y + 4f, 8f, 8f), tone);
                string s = (labels[i] ?? string.Empty) + " " + (ratios[i] * 100f).ToString("0") + "%";
                Draw.Label(new Rect(item.x + 12f, item.y, item.width - 12f, 14f), s, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            }
        }

        /// <summary>A/B 对比条（上下并列 + 优胜高亮）。</summary>
        public static void CompareBars(Rect rect, string leftLabel, string rightLabel, float a, float b, float max)
        {
            var col = Layout.Column(rect, 4f);
            float rowH = (rect.height - 4f) / 2f;
            DrawRow(col.Take(rowH), leftLabel, a, max, DataExFmt.CompareWinner(a, b) >= 0 ? Tokens.Accent : Tokens.Info);
            DrawRow(col.Rest, rightLabel, b, max, DataExFmt.CompareWinner(a, b) <= 0 ? Tokens.Accent : Tokens.Info);
        }

        private static void DrawRow(Rect row, string label, float value, float max, Color tone)
        {
            var r = Layout.Row(row, 6f);
            Draw.Label(r.Take(76f), label ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Rect bar = r.Rest;
            Draw.Box(bar, Tokens.BorderSoft);
            float share = max <= 0f ? 0f : Mathf.Clamp01(value / max);
            Draw.Box(new Rect(bar.x, bar.y, bar.width * share, bar.height), tone);
            Draw.Label(bar, value.ToString("0.#"), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
        }
    }
}
