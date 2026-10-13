using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>布局视觉原子的纯计算（可离线单测）。</summary>
    public static class VisualsFmt
    {
        /// <summary>渐变条第 i 段的插值系数（n 段 → 0~1）。</summary>
        public static float LerpT(int i, int n)
            => n <= 1 ? 0f : Mathf.Clamp01(i / (float)(n - 1));

        /// <summary>渐隐遮罩第 i 段的 alpha（自左向右递增，用于"淡出到背景"）。</summary>
        public static float FadeAlpha(int i, int n)
            => n <= 0 ? 0f : Mathf.Clamp01((i + 1) / (float)n);

        /// <summary>钉选角标矩形（贴宿主右上角）。</summary>
        public static Rect PinRect(Rect host, float size, float margin)
            => new Rect(host.xMax - size - margin, host.y + margin, size, size);
    }

    /// <summary>
    /// 布局视觉原子（S2 第三批）：色卡、渐变条、钉选角标、右缘渐隐。
    /// 全部零素材；颜色走令牌或调用方传参。
    /// </summary>
    public static class VisualAtoms
    {
        /// <summary>色卡：实心色块 + 描边；选中态加 accent 外环。</summary>
        public static void Swatch(Rect rect, Color color, bool selected = false)
        {
            Draw.Box(rect, color);
            Draw.Border(rect, selected ? Tokens.Accent : Tokens.BorderSoft);
            if (selected)
                Draw.Border(new Rect(rect.x - 2f, rect.y - 2f, rect.width + 4f, rect.height + 4f), Tokens.Accent);
        }

        /// <summary>渐变条（lo→hi 分段插值，零贴图）。</summary>
        public static void GradientBar(Rect rect, Color lo, Color hi, int segments = 16)
        {
            int n = Mathf.Max(2, segments);
            float w = rect.width / n;
            for (int i = 0; i < n; i++)
            {
                float t = VisualsFmt.LerpT(i, n);
                Draw.Box(new Rect(rect.x + i * w, rect.y, w + 1f, Mathf.Max(1f, rect.height)),
                    Color.Lerp(lo, hi, t));
            }
        }

        /// <summary>
        /// 钉选角标（几何绘制：圆点 + 短柄，避免依赖 emoji 字形——原版字体不一定有 📌）。
        /// </summary>
        public static void PinBadge(Rect host, bool pinned, Color? color = null,
            float size = 12f, float margin = 2f)
        {
            if (!pinned) return;
            Color c = color ?? Tokens.Accent;
            Rect r = VisualsFmt.PinRect(host, size, margin);
            float head = size * 0.55f;
            var headRect = new Rect(r.x + (size - head) / 2f, r.y, head, head);
            Draw.Box(headRect, c);                                              // 钉头（方块近似圆点）
            Draw.Box(new Rect(r.center.x - 1f, r.y + head, 2f, r.height - head), c); // 钉柄
        }

        /// <summary>
        /// 右缘渐隐文字：先画文本，再用<b>背景色</b>分段叠加递增 alpha 实现淡出。
        /// ✅ 但需要调用方传入已知背景色（bgColor）；真 alpha 遮罩版留 v1.1 素材项。
        /// </summary>
        public static void OverflowFade(Rect rect, string text, Color bgColor,
            GameFont font = GameFont.Small, Color? color = null,
            float fadeWidth = 24f, int segments = 8)
        {
            Draw.Label(rect, text ?? string.Empty, font, color ?? Tokens.Text, TextAnchor.MiddleLeft);
            float fw = Mathf.Min(fadeWidth, rect.width);
            if (fw <= 0f) return;
            int n = Mathf.Max(1, segments);
            float w = fw / n;
            for (int i = 0; i < n; i++)
            {
                float a = VisualsFmt.FadeAlpha(i, n);
                Draw.Box(new Rect(rect.xMax - fw + i * w, rect.y, w + 1f, rect.height),
                    Draw.WithAlpha(bgColor, a));
            }
        }
    }
}
