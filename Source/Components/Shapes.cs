using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>几何/装饰原子的纯计算（可离线单测）。</summary>
    public static class ShapesFmt
    {
        /// <summary>渐变分段第 i 段的 alpha（0→1 递减）。</summary>
        public static float GradientAlpha(int i, int n)
            => n <= 0 ? 0f : Mathf.Clamp01((n - i) / (float)n);

        /// <summary>展开箭头角度：progress01 ∈ [0,1] → 0°~90°。</summary>
        public static float CaretAngle(float progress01) => 90f * Mathf.Clamp01(progress01);

        /// <summary>加载指示器"头部"段索引（t 为秒；每 0.25s 推进一格）。</summary>
        public static int SpinnerHead(int segments, float t)
        {
            if (segments <= 0) return 0;
            int step = (int)Math.Floor(Mathf.Abs(t) * 4f);
            return step % segments;
        }

        /// <summary>带字分割线：按总宽/标签宽/间距切出「左线-标签-右线」。</summary>
        public static void RuleSplit(float totalW, float labelW, float gap,
            out float leftW, out float labelX, out float rightW)
        {
            float avail = Mathf.Max(0f, totalW - labelW - gap * 2f);
            leftW = avail / 2f;
            labelX = leftW + gap;
            rightW = Mathf.Max(0f, totalW - labelX - labelW);
        }
    }

    /// <summary>
    /// 几何与装饰原子（S2 第二批）：HUD 角框、渐变线、带字线、水印、凹陷面板。
    /// 全部令牌取色、无状态、无分配（除字符串测量）。
    /// </summary>
    public static class Shapes
    {
        /// <summary>四角 L 型框（HUD / 选中态气质）；只画角不画整框。</summary>
        public static void CornerFrame(Rect rect, Color color, float armLen = 14f, float thickness = 2f)
        {
            float L = Mathf.Min(armLen, Mathf.Min(rect.width, rect.height));
            float t = Mathf.Max(1f, thickness);
            // 左上
            Draw.Box(new Rect(rect.x, rect.y, L, t), color);
            Draw.Box(new Rect(rect.x, rect.y, t, L), color);
            // 右上
            Draw.Box(new Rect(rect.xMax - L, rect.y, L, t), color);
            Draw.Box(new Rect(rect.xMax - t, rect.y, t, L), color);
            // 左下
            Draw.Box(new Rect(rect.x, rect.yMax - t, L, t), color);
            Draw.Box(new Rect(rect.x, rect.yMax - L, t, L), color);
            // 右下
            Draw.Box(new Rect(rect.xMax - L, rect.yMax - t, L, t), color);
            Draw.Box(new Rect(rect.xMax - t, rect.yMax - L, t, L), color);
        }

        /// <summary>渐变分隔线：分段递减 alpha，零贴图实现渐隐。</summary>
        public static void GradientRule(Rect rect, Color color, int segments = 12)
        {
            int n = Mathf.Max(1, segments);
            float w = rect.width / n;
            for (int i = 0; i < n; i++)
            {
                Draw.Box(new Rect(rect.x + i * w, rect.y, w + 1f, Mathf.Max(1f, rect.height)),
                    Draw.WithAlpha(color, ShapesFmt.GradientAlpha(i, n)));
            }
        }

        /// <summary>带文字的分割线（── 记录 ──）。</summary>
        public static void RuleWithLabel(Rect rect, string label, Color color, float gap = 6f)
        {
            float labelW;
            using (Draw.Font(Tokens.FontLabel))
                labelW = Text.CalcSize(label ?? string.Empty).x;

            ShapesFmt.RuleSplit(rect.width, labelW, gap, out float leftW, out float labelX, out float rightW);
            float y = rect.y + rect.height / 2f;
            Draw.LineH(rect.x, y, leftW, color);
            Draw.Label(new Rect(rect.x + labelX, rect.y, labelW, rect.height),
                label ?? string.Empty, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleCenter);
            Draw.LineH(rect.x + labelX + labelW + gap, y, rightW, color);
        }

        /// <summary>水印（低透明度大字，画在内容之下）。</summary>
        public static void Watermark(Rect rect, string mark, float alpha = 0.06f)
        {
            Draw.Label(rect, mark ?? string.Empty, GameFont.Medium,
                Draw.WithAlpha(Tokens.Text, alpha), TextAnchor.MiddleCenter);
        }

        /// <summary>凹陷面板：上/左暗、下/右亮，形成内嵌感（与 Panel 凸起成对偶）。</summary>
        public static void InsetPanel(Rect rect)
        {
            Draw.Box(rect, Tokens.Window);
            Draw.LineH(rect.x, rect.y, rect.width, Tokens.BorderSoft);
            Draw.LineV(rect.x, rect.y, rect.height, Tokens.BorderSoft);
            Draw.LineH(rect.x, rect.yMax - 1f, rect.width, Tokens.PanelRaised);
            Draw.LineV(rect.xMax - 1f, rect.y, rect.height, Tokens.PanelRaised);
        }
    }
}
