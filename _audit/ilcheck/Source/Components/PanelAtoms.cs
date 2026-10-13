using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>面板/行原子的纯计算（可离线单测）。</summary>
    public static class PanelFmt
    {
        /// <summary>闪烁/脉动 alpha（与 PulseDot 同公式，便于统一观感）。</summary>
        public static float FlashAlpha(float t, float minAlpha = 0.45f)
            => minAlpha + (1f - minAlpha) * (0.5f + 0.5f * Mathf.Sin(t * 4f));

        /// <summary>行拆分为「标签 | 数值」两段。</summary>
        public static void RowSplit(Rect row, float labelRatio, float gap, out Rect label, out Rect value)
        {
            float lw = Mathf.Max(0f, row.width - gap) * Mathf.Clamp01(labelRatio);
            label = new Rect(row.x, row.y, lw, row.height);
            value = new Rect(row.x + lw + gap, row.y, Mathf.Max(0f, row.width - lw - gap), row.height);
        }
    }

    /// <summary>
    /// 面板原子（批4-2a，RimHUD 实证）：统计行 / 迷你档案条 / 警示闪边 / 分段头。
    /// 全部无状态；动画参数由调用方传入。
    /// 图标一律使用<b>短文本</b>（不用 emoji，规避原版字体字形缺失）。
    /// </summary>
    public static class PanelAtoms
    {
        /// <summary>统计行：标签 + 数值（可选微型条）。</summary>
        public static void StatRow(Rect rect, string label, string value, float? bar01 = null, Color? barColor = null)
        {
            PanelFmt.RowSplit(rect, 0.55f, Tokens.SpaceXs, out Rect l, out Rect v);
            Draw.Label(l, label ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            if (!bar01.HasValue)
            {
                Draw.Label(v, value ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
                return;
            }
            var vrow = Layout.Row(v, 4f);
            Rect bar = vrow.Take(34f);
            Draw.Box(bar, Tokens.BorderSoft);
            Draw.Box(new Rect(bar.x, bar.y, bar.width * Mathf.Clamp01(bar01.Value), bar.height),
                barColor ?? Tokens.Accent);
            Draw.Label(vrow.Rest, value ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleRight);
        }

        /// <summary>迷你档案条：色块头像 + 名字 + 两条 micro 状态条。</summary>
        public static void MiniProfileChip(Rect rect, string initial, string name, float mood01, float health01)
        {
            Draw.Box(rect, Tokens.Card);
            Draw.Border(rect, Tokens.BorderSoft);
            var row = Layout.Row(Layout.Inset(rect, 4f, 2f), 6f);

            float avSize = Mathf.Min(rect.height - 4f, 26f);
            Rect av = row.Take(avSize);
            Draw.Box(av, Tokens.Accent);
            Draw.Label(av, initial ?? string.Empty, Tokens.FontLabel, Tokens.Window, TextAnchor.MiddleCenter);

            var col = Layout.Column(row.Rest, 1f);
            Draw.Label(col.Take(Mathf.Max(12f, rect.height * 0.5f)), name ?? string.Empty,
                Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            var bars = Layout.Row(col.Rest, 4f);
            MiniBar(bars.Take(30f), mood01, Tokens.Alive);
            MiniBar(bars.Rest, health01, Tokens.Info);
        }

        private static void MiniBar(Rect r, float v, Color c)
        {
            Draw.Box(r, Tokens.BorderSoft);
            Draw.Box(new Rect(r.x, r.y, r.width * Mathf.Clamp01(v), r.height), c);
        }

        /// <summary>警示闪边：边框 alpha 脉动（用于致命病症 / 临近崩溃）。</summary>
        public static void FlashOverlay(Rect rect, float t, Color? color = null, float thickness = 2f)
        {
            Color c = color ?? Tokens.Dead;
            float a = PanelFmt.FlashAlpha(t);
            Draw.Border(rect, Draw.WithAlpha(c, a));
            if (thickness > 1f)
                Draw.Border(new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f),
                    Draw.WithAlpha(c, a * 0.55f));
        }

        /// <summary>薄分段头（HUD 面板用）：折叠箭头 + 标题 + 右侧计数。</summary>
        public static void SectionBar(Rect rect, string title, string count, bool expanded)
        {
            Draw.Box(rect, Tokens.PanelRaised);
            var row = Layout.Row(Layout.Inset(rect, 6f, 0f), Tokens.SpaceXs);
            Draw.Label(row.Take(16f), expanded ? "▾" : "▸", Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleCenter);
            Draw.Label(row.Rest, title ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            if (!string.IsNullOrEmpty(count))
                Draw.Label(new Rect(rect.xMax - 46f, rect.y, 40f, rect.height), count,
                    Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleRight);
        }
    }
}
