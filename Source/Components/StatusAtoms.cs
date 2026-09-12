using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>状态图形种类（颜色由令牌决定，不在此硬编码色值）。</summary>
    public enum StatusKind { Ok, Info, Warn, Error, Paused }

    /// <summary>
    /// 状态原子纯计算（单测面）：不触 GUI、不读输入；颜色经令牌解析。
    /// 与 <see cref="StatusAtoms"/> 共文件（Fmt + Atoms 同置，工程惯例）。
    /// </summary>
    public static class StatusFmt
    {
        /// <summary>呼吸点 alpha：[minAlpha, 1] 内正弦脉动；t=0 时为 (1+minAlpha)/2。</summary>
        public static float PulseAlpha(float t, float minAlpha = 0.45f)
        {
            float pulse = 0.5f + 0.5f * Mathf.Sin(t * 4f);
            return minAlpha + (1f - minAlpha) * pulse;
        }

        /// <summary>在 rect 内居中取正方形（边长=短边），避免被行高拉成竖条。</summary>
        public static Rect DotRect(Rect rect)
        {
            float s = Mathf.Min(rect.width, rect.height);
            return new Rect(rect.center.x - s / 2f, rect.center.y - s / 2f, s, s);
        }

        /// <summary>
        /// Spinner 尾迹 alpha：距头部 behind 段 → 1 - behind/n，下限 0.15。
        /// i 允许任意 int（负数/越界按环回处理）。
        /// </summary>
        public static float TrailAlpha(int i, int head, int segments)
        {
            int n = Mathf.Max(1, segments);
            int behind = ((head - i) % n + n) % n;
            float a = 1f - (behind / (float)n);
            return Mathf.Max(0.15f, a);
        }

        /// <summary>状态图形映射：kind → 字形 + 令牌色。</summary>
        public static void GlyphOf(StatusKind kind, out string glyph, out Color color)
        {
            switch (kind)
            {
                case StatusKind.Ok: glyph = "✓"; color = Tokens.Alive; return;
                case StatusKind.Info: glyph = "•"; color = Tokens.Info; return;
                case StatusKind.Warn: glyph = "!"; color = Tokens.Warn; return;
                case StatusKind.Error: glyph = "✕"; color = Tokens.Dead; return;
                default: glyph = "‖"; color = Tokens.Muted; return;
            }
        }
    }

    /// <summary>
    /// 状态原子（S2 第二批）：呼吸点、状态图形、加载指示器、展开箭头、滚动渐隐提示。
    /// 全部无状态；动画参数由调用方传入（t / progress01），便于单测与暂停。
    /// 纯计算已抽至 <see cref="StatusFmt"/>（单测面），本类只做绘制编排。
    /// </summary>
    public static class StatusAtoms
    {
        /// <summary>
        /// 呼吸状态点（绿在线 / 黄忙碌 / 红告急）：alpha 正弦脉动。
        /// 在传入 rect 内居中绘制一个正方形点（边长=短边），避免被行高拉成竖条。
        /// </summary>
        public static void PulseDot(Rect rect, Color color, float t, float minAlpha = 0.45f)
        {
            float pulse = 0.5f + 0.5f * Mathf.Sin(t * 4f);
            float a = minAlpha + (1f - minAlpha) * pulse;
            float s = Mathf.Min(rect.width, rect.height);
            var dot = new Rect(rect.center.x - s / 2f, rect.center.y - s / 2f, s, s);
            Draw.Box(dot, Draw.WithAlpha(color, a));
        }

        /// <summary>状态图形（✓ / • / ! / ✕ / ‖），颜色取自令牌。</summary>
        public static void Glyph(Rect rect, StatusKind kind)
        {
            string glyph;
            Color color;
            switch (kind)
            {
                case StatusKind.Ok: glyph = "✓"; color = Tokens.Alive; break;
                case StatusKind.Info: glyph = "•"; color = Tokens.Info; break;
                case StatusKind.Warn: glyph = "!"; color = Tokens.Warn; break;
                case StatusKind.Error: glyph = "✕"; color = Tokens.Dead; break;
                default: glyph = "‖"; color = Tokens.Muted; break;
            }
            Draw.Label(rect, glyph, Tokens.FontBody, color, TextAnchor.MiddleCenter);
        }

        /// <summary>
        /// 加载指示器（**零贴图**版本）：8 段绕圆分布，头部最亮、尾部渐暗。
        /// 注意：圆形点本身用小方块近似；如需正圆需贴图（v1.1 素材项）。
        /// </summary>
        public static void Spinner(Rect rect, float t, Color? color = null, int segments = 8,
            float radius = 8f, float size = 3f)
        {
            Color c = color ?? Tokens.Accent;
            int n = Mathf.Max(1, segments);
            int head = ShapesFmt.SpinnerHead(n, t);
            Vector2 center = rect.center;
            for (int i = 0; i < n; i++)
            {
                float ang = (i / (float)n) * Mathf.PI * 2f;
                Vector2 p = new Vector2(center.x + Mathf.Cos(ang) * radius, center.y + Mathf.Sin(ang) * radius);
                Draw.Box(new Rect(p.x - size / 2f, p.y - size / 2f, size, size),
                    Draw.WithAlpha(c, StatusFmt.TrailAlpha(i, head, n)));
            }
        }

        /// <summary>加载指示器（贴图版本，走官方 <b>Widgets.DrawTextureRotated</b>——签名已核实）。</summary>
        public static void Spinner(Rect rect, float t, Texture2D tex, float scale = 1f, float speed = 240f)
        {
            if (tex == null) { Spinner(rect, t); return; }
            float angle = (t * speed) % 360f;
            Widgets.DrawTextureRotated(rect.center, tex, angle, scale);
        }

        /// <summary>
        /// 展开/折叠箭头（推荐用法）：由展开状态驱动。
        /// ⚠️ 这是<b>过渡动画</b>——progress01 应随展开状态变化而变化，<b>禁止持续循环</b>（会变成绕圆心打转）。
        /// 平滑过渡的动画进度由调用方存自己的字段并随帧插值。
        /// </summary>
        public static void Caret(Rect rect, float progress01, Color? color = null)
        {
            using (Draw.Rotate(ShapesFmt.CaretAngle(progress01), rect.center))
                Draw.Label(rect, "▸", Tokens.FontBody, color ?? Tokens.Muted, TextAnchor.MiddleCenter);
        }

        /// <summary>布尔态简写：折叠 ▸ / 展开 ▾（无过渡帧）。</summary>
        public static void Caret(Rect rect, bool expanded, Color? color = null)
            => Caret(rect, expanded ? 1f : 0f, color);

        /// <summary>
        /// 滚动渐隐提示（底部条 + ▼）。
        /// ⚠️ 已知代价：真渐变需要横向渐变遮罩贴图（v1.1 素材项）；当前实现为纯色条+箭头（可用但无渐隐）。
        /// </summary>
        public static void ScrollFadeHint(Rect viewport, bool moreBelow)
        {
            if (!moreBelow) return;
            float h = 14f;
            var strip = new Rect(viewport.x, viewport.yMax - h, viewport.width, h);
            Draw.Box(strip, Tokens.Window);
            Draw.Label(strip, "▼", Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleCenter);
        }
    }
}
