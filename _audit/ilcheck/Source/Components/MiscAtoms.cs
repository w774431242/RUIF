using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>杂项原子的纯计算（可离线单测）。</summary>
    public static class MiscFmt
    {
        /// <summary>预算剩余点数。</summary>
        public static int PointsRemaining(int budget, int spent) => budget - spent;
        /// <summary>是否超支。</summary>
        public static bool PointsOverdrawn(int budget, int spent) => spent > budget;

        /// <summary>气泡尾（小三角）的近似矩形。</summary>
        public static Rect BubbleTail(Rect bubble, float size)
            => new Rect(bubble.center.x - size / 2f, bubble.yMax - 1f, size, size * 0.7f);

        /// <summary>滚动条滑块高度（内容不超出时占满轨道）。</summary>
        public static float ScrollThumbHeight(float trackH, float viewH, float contentH)
        {
            if (contentH <= viewH) return trackH;
            return Mathf.Max(16f, trackH * Mathf.Clamp01(viewH / contentH));
        }

        /// <summary>滑块在轨道中的 Y 位置。</summary>
        public static float ScrollThumbY(float trackY, float trackH, float thumbH, float scroll01)
            => trackY + (trackH - thumbH) * Mathf.Clamp01(scroll01);
    }

    /// <summary>
    /// 杂项原子（批4-2b）：预算点数徽章、气泡标签、细滚动条。
    /// </summary>
    public static class MiscAtoms
    {
        /// <summary>预算点数徽章：剩余（绿）/ 用尽（黄）/ 超支（红）。</summary>
        public static void PointCostBadge(Rect rect, int budget, int spent)
        {
            int rem = MiscFmt.PointsRemaining(budget, spent);
            bool over = MiscFmt.PointsOverdrawn(budget, spent);
            Color c = over ? Tokens.Dead : (rem <= 0 ? Tokens.Warn : Tokens.Alive);
            Draw.Box(rect, Draw.WithAlpha(c, 0.18f));
            Draw.Border(rect, c);
            string s = over ? Tr.Get("RUIF.Comp.Points.Overdrawn", -rem) : Tr.Get("RUIF.Comp.Points.Remaining", rem);
            Draw.Label(rect, s, Tokens.FontLabel, c, TextAnchor.MiddleCenter);
        }

        /// <summary>气泡标签：矩形主体 + 下方小三角（分段方块近似，无贴图无圆角）。</summary>
        public static void BubbleTag(Rect bubble, string text, Color? color = null)
        {
            Color c = color ?? Tokens.PanelRaised;
            Draw.Box(bubble, c);
            Draw.Border(bubble, Tokens.BorderSoft);
            Rect tail = MiscFmt.BubbleTail(bubble, 8f);
            Draw.Box(tail, c);
            Draw.Box(new Rect(bubble.center.x - 2f, bubble.yMax + tail.height - 1f, 4f, 2f), c);
            Draw.Label(bubble, text ?? string.Empty, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
        }

        /// <summary>
        /// 细滚动条：绘制在滚动视口右侧（配合 <c>Layout.BeginScroll(..., showScrollbars:false)</c>）。
        /// </summary>
        public static void ThinScrollBar(Rect viewRect, float contentH, float scrollY, float width = 8f)
        {
            if (contentH <= viewRect.height) return;
            var track = new Rect(viewRect.xMax + 1f, viewRect.y, width, viewRect.height);
            Draw.Box(track, Draw.WithAlpha(Tokens.Text, 0.06f));
            float th = MiscFmt.ScrollThumbHeight(track.height, viewRect.height, contentH);
            float max = contentH - viewRect.height;
            float y = MiscFmt.ScrollThumbY(track.y, track.height, th, max > 0f ? scrollY / max : 0f);
            Draw.Box(new Rect(track.x, y, track.width, th), Tokens.Accent);
        }
    }
}
