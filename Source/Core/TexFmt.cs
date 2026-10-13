using UnityEngine;

namespace RUIF
{
    /// <summary>
    /// L0 · 贴图像素缓冲的<b>纯计算层</b>：只做数组运算，不创建任何 Unity 宿主对象
    /// ⇒ 可离线单测（传进 w/h 与颜色，断言像素值），符合框架「Fmt = 纯计算」分层约定。
    ///
    /// <para><b>存在理由（第二轮取证结论）</b>：原版 UI 侧<b>没有</b>渐变 API、<b>没有</b>多边形填充 API
    /// （全类型表检索 <c>Gradient</c> 仅命中 <c>Verse.Noise.GradientPresets</c>，属地形噪声）。
    /// 而原版自己造贴图是有先例的 —— <c>SolidColorMaterials.NewSolidColorTexture</c> 内部即
    /// <c>new Texture2D(2,2)</c> + <c>SetPixel×4</c> + <c>Apply()</c>。
    /// 因此「真渐变 / 真渐隐 / 热力图 / 饼环 / 桑基」的统一解是：**本层算像素 → TexCache 上屏**。</para>
    ///
    /// <para><b>坐标约定</b>：Unity <c>Texture2D</c> 的像素 (0,0) 在<b>左下角</b>。
    /// 因此 <see cref="VGradient"/> 的 <c>top</c> 参数写到缓冲的<b>末行</b>；上屏时若发现上下颠倒，
    /// 是此处语义被误用，而非绘制端问题。</para>
    ///
    /// <para><b>数学只用 <c>System.Math</c></b>：<c>Mathf</c> 的部分成员经 ECall 实现，
    /// 在非 Unity 宿主（离线单测）会抛异常（本框架 ECall 铁律同源）。</para>
    /// </summary>
    public static class TexFmt
    {
        /// <summary>缓冲下标（行主序，y=0 为纹理底部）。</summary>
        public static int Index(int x, int y, int w) => y * w + x;

        // ---------------------------------------------------------------- 纯色 / 渐变

        /// <summary>整幅纯色缓冲。</summary>
        public static Color32[] Solid(int w, int h, Color fill)
        {
            if (w <= 0 || h <= 0) return null;
            Color32 c = fill;
            var buf = new Color32[w * h];
            for (int i = 0; i < buf.Length; i++) buf[i] = c;
            return buf;
        }

        /// <summary>水平线性渐变（x=0 取 <paramref name="left"/>，x=w-1 取 <paramref name="right"/>）。</summary>
        public static Color32[] HGradient(int w, Color left, Color right)
        {
            if (w <= 0) return null;
            var buf = new Color32[w];
            for (int x = 0; x < w; x++) buf[x] = Ramp(left, right, w <= 1 ? 0f : (float)x / (w - 1));
            return buf;
        }

        /// <summary>垂直线性渐变（缓冲末行 = <paramref name="top"/>，首行 = <paramref name="bottom"/>）。</summary>
        public static Color32[] VGradient(int h, Color top, Color bottom)
        {
            if (h <= 0) return null;
            var buf = new Color32[h];
            for (int y = 0; y < h; y++)
            {
                // y=0 在底部 ⇒ 该行对应 bottom 侧
                buf[y] = Ramp(bottom, top, h <= 1 ? 0f : (float)y / (h - 1));
            }
            return buf;
        }

        /// <summary>alpha 渐隐带（水平；用于 OverflowFade 的真·渐隐遮罩）。保留 RGB 只改 A。</summary>
        public static Color32[] AlphaFade(int w, Color rgb, float from = 1f, float to = 0f)
        {
            if (w <= 0) return null;
            var buf = new Color32[w];
            for (int x = 0; x < w; x++)
            {
                Color c = rgb;
                c.a = from + (to - from) * (w <= 1 ? 0f : (float)x / (w - 1));
                buf[x] = c;
            }
            return buf;
        }

        /// <summary>线性插值（t 自动 clamp01）。</summary>
        public static Color Ramp(Color lo, Color hi, float t)
        {
            if (t <= 0f) return lo;
            if (t >= 1f) return hi;
            return new Color(lo.r + (hi.r - lo.r) * t,
                             lo.g + (hi.g - lo.g) * t,
                             lo.b + (hi.b - lo.b) * t,
                             lo.a + (hi.a - lo.a) * t);
        }

        /// <summary>
        /// 五档离散色阶（值 → 色）—— 热力图/阈值条的共用色阶。
        /// <para>用<b>离散档</b>而非连续插值：一是快（省一次插值），二是原版色感本来就是"档"，
        /// 三是连续渐变在低分屏上会出现肉眼可见的条带。</para>
        /// </summary>
        public static Color Heat(float t01)
        {
            int band = (int)(Clamp01(t01) * 4.999f);   // 0..4
            switch (band)
            {
                case 0: return new Color(0.259f, 0.412f, 0.639f);   // 冷蓝
                case 1: return new Color(0.310f, 0.667f, 0.612f);   // 青
                case 2: return new Color(0.545f, 0.741f, 0.376f);   // 黄绿
                case 3: return new Color(0.898f, 0.706f, 0.286f);   // 琥珀
                default: return new Color(0.851f, 0.325f, 0.310f);  // 红
            }
        }

        // ---------------------------------------------------------------- 形状填充（在缓冲上作画）

        /// <summary>矩形填充（缓冲坐标系，左闭右开；自动裁到边界）。</summary>
        public static void FillRect(Color32[] buf, int w, int h, int x0, int y0, int x1, int y1, Color c)
        {
            if (buf == null || w <= 0 || h <= 0) return;
            if (x0 > x1) { int t = x0; x0 = x1; x1 = t; }
            if (y0 > y1) { int t = y0; y0 = y1; y1 = t; }
            if (x0 < 0) x0 = 0;
            if (y0 < 0) y0 = 0;
            if (x1 > w) x1 = w;
            if (y1 > h) y1 = h;
            Color32 cc = c;
            for (int y = y0; y < y1; y++)
            {
                int row = y * w;
                for (int x = x0; x < x1; x++) buf[row + x] = cc;
            }
        }

        /// <summary>
        /// 椭圆填充（<paramref name="rx"/>/<paramref name="ry"/> 为像素半径）。
        /// <para><b>抗锯齿</b>：边缘 1px 做覆盖率软过渡（<see cref="Coverage"/>），
        /// 这是"自绘圆看起来不像原版"最常见的根因 —— 硬边圆在像素栅格上会有明显阶梯。</para>
        /// </summary>
        public static void FillDisc(Color32[] buf, int w, int h, float cx, float cy, float rx, float ry, Color c)
        {
            if (buf == null || w <= 0 || h <= 0 || rx <= 0f || ry <= 0f) return;
            int x0 = Mathf.Max(0, (int)System.Math.Floor(cx - rx) - 1);
            int x1 = Mathf.Min(w, (int)System.Math.Ceiling(cx + rx) + 1);
            int y0 = Mathf.Max(0, (int)System.Math.Floor(cy - ry) - 1);
            int y1 = Mathf.Min(h, (int)System.Math.Ceiling(cy + ry) + 1);
            for (int y = y0; y < y1; y++)
            {
                for (int x = x0; x < x1; x++)
                {
                    float dx = (x + 0.5f - cx) / rx;
                    float dy = (y + 0.5f - cy) / ry;
                    float d = (float)System.Math.Sqrt(dx * dx + dy * dy);   // <1 在内部
                    float cov = Coverage(d, rx, ry);
                    if (cov <= 0f) continue;
                    buf[y * w + x] = Blend(buf[y * w + x], c, cov);
                }
            }
        }

        /// <summary>
        /// 扇环填充（饼图 / 环图共用）——
        /// <paramref name="startDeg"/> 起角、<paramref name="sweepDeg"/> 张角（度，逆时针为正，0° = 正右），
        /// <paramref name="inner01"/> 内半径占比（0 = 饼，&gt;0 = 环）。
        /// </summary>
        public static void FillSector(Color32[] buf, int w, int h, float cx, float cy, float rx, float ry,
                                      float startDeg, float sweepDeg, float inner01, Color c)
        {
            if (buf == null || w <= 0 || h <= 0 || rx <= 0f || ry <= 0f) return;
            if (sweepDeg <= 0f) return;
            if (sweepDeg >= 360f) sweepDeg = 359.99f;
            const float deg2rad = (float)(System.Math.PI / 180.0);
            float a0 = startDeg * deg2rad;
            float sweepRad = sweepDeg * deg2rad;
            // ⚠ 2026-09-15 修复（消费方实机：单段满圆呈 C 形缺口，缺口恰在 270°~360°）：
            //   原实现用 `a1 = (startDeg + sweepDeg)·deg2rad`，而 `a0` **未归一化**
            //   （startDeg = −90° ⇒ a0 = −π/2）；判定侧 `rel = ang − a0` 只做「加 2π」归一化
            //   ⇒ rel 的取值域被整体抬高 π/2，越过 `a1 − a0`（= 2π）的那部分**被误跳过**。
            //   修法：满圆**跳过角度判定**（纯按半径填充）；部分圆先把 a0 归一化到 [0, 2π)
            //   再与 sweepRad 同基准比较（startDeg 为负时同样正确）。
            bool fullCircle = sweepDeg >= 359.99f;
            const float twoPiC = (float)(System.Math.PI * 2.0);
            float a0n = a0 % twoPiC;
            if (a0n < 0f)
            {
                a0n += twoPiC;
            }
            float inner = Clamp01(inner01);

            int x0 = Mathf.Max(0, (int)System.Math.Floor(cx - rx) - 1);
            int x1 = Mathf.Min(w, (int)System.Math.Ceiling(cx + rx) + 1);
            int y0 = Mathf.Max(0, (int)System.Math.Floor(cy - ry) - 1);
            int y1 = Mathf.Min(h, (int)System.Math.Ceiling(cy + ry) + 1);
            float twoPi = (float)(System.Math.PI * 2.0);

            for (int y = y0; y < y1; y++)
            {
                for (int x = x0; x < x1; x++)
                {
                    float px = x + 0.5f - cx;
                    float py = y + 0.5f - cy;
                    float nx = px / rx, ny = py / ry;
                    float d = (float)System.Math.Sqrt(nx * nx + ny * ny);
                    if (d > 1f) continue;

                    if (!fullCircle)
                    {
                        float ang = (float)System.Math.Atan2(py, px);
                        if (ang < 0f) ang += twoPi;
                        float rel = ang - a0n;
                        if (rel < 0f) rel += twoPi;
                        if (rel > sweepRad) continue;
                    }

                    float cov = Coverage(d, rx, ry);
                    if (inner > 0f)
                    {
                        // 环内孔：距内缘的像素数（孔外为正）
                        float pxIn = (d - inner) * System.Math.Min(rx, ry);
                        if (pxIn <= -0.5f) continue;
                        cov = System.Math.Min(cov, pxIn >= 0.5f ? 1f : pxIn + 0.5f);
                    }
                    if (cov <= 0f) continue;
                    buf[y * w + x] = Blend(buf[y * w + x], c, cov);
                }
            }
        }

        /// <summary>
        /// 把一条**任意角度线段**画进像素缓冲（距离场抗锯齿，与 <see cref="Coverage"/> 同风格）。
        ///
        /// <para><b>⛔ 为什么必须用它替代原版的 <c>Widgets.DrawLine</c>（2026-09-13 根因修复）</b>：
        /// 后者内部是 <c>GL.PushMatrix / GL.MultMatrix / GL.PopMatrix</c> —— 改的是 <b>GL 矩阵</b>，
        /// 而 <c>GUI.matrix</c>（含滚动视图的平移）**不会被 GL 矩阵感知** ⇒
        /// **在滚动视图内位置全错（元素「飞散」）**。本方法把线段在 **CPU 上算进像素缓冲**，
        /// 调用方再用一次 <c>GUI.DrawTexture</c>（或 <see cref="TexAtoms.Canvas"/>）绘制
        /// ⇒ <b>零矩阵、零 GL 依赖</b>，在任何坐标上下文内都正确。</para>
        ///
        /// <para><paramref name="x0"/>…<paramref name="y1"/> 与 <paramref name="buf"/> **同一坐标系**
        /// （贴图局部像素坐标：通常先用 <c>rect</c> 减去贴图原点、再乘「像素/点」缩放）。
        /// 超出缓冲的部分自动裁剪。</para>
        /// </summary>
        public static void Line(Color32[] buf, int w, int h,
                                float x0, float y0, float x1, float y1, Color c, float thickness = 1.5f)
        {
            if (buf == null || w <= 0 || h <= 0) return;
            float half = Mathf.Max(0.05f, thickness * 0.5f);
            int bx0 = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(x0, x1) - half - 1f));
            int bx1 = Mathf.Min(w, Mathf.CeilToInt(Mathf.Max(x0, x1) + half + 1f));
            int by0 = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(y0, y1) - half - 1f));
            int by1 = Mathf.Min(h, Mathf.CeilToInt(Mathf.Max(y0, y1) + half + 1f));
            float dx = x1 - x0, dy = y1 - y0;
            float len2 = dx * dx + dy * dy;
            for (int y = by0; y < by1; y++)
            {
                float py = y + 0.5f - y0;
                for (int x = bx0; x < bx1; x++)
                {
                    float px = x + 0.5f - x0;
                    float t = len2 > 1e-6f ? Clamp01((px * dx + py * dy) / len2) : 0f;   // 投影参数（线段内钳制）
                    float qx = px - dx * t, qy = py - dy * t;
                    float d = Mathf.Sqrt(qx * qx + qy * qy);                             // 点到线段距离
                    float cov = Clamp01(half + 0.5f - d);                                // 半宽 + 0.5px 过渡带
                    if (cov <= 0f) continue;
                    int idx = y * w + x;
                    buf[idx] = Blend(buf[idx], c, cov);
                }
            }
        }

        /// <summary>
        /// 折线：逐段调用 <see cref="Line"/>（<paramref name="xs"/> 与 <paramref name="ys"/> 等长取短）。
        /// <para>⚠ 相邻段在**接缝处会叠加混合** —— 不透明色无影响；**半透明色会在拐点略深**。
        /// 需要严格一致的半透明折线时，应改为「一次性距离场（对所有段取最小距离）后再混合」。</para>
        /// </summary>
        public static void Polyline(Color32[] buf, int w, int h, float[] xs, float[] ys, Color c, float thickness = 1.5f)
        {
            if (buf == null || xs == null || ys == null) return;
            int n = Mathf.Min(xs.Length, ys.Length);
            for (int i = 1; i < n; i++)
                Line(buf, w, h, xs[i - 1], ys[i - 1], xs[i], ys[i], c, thickness);
        }

        /// <summary>
        /// 边缘覆盖率：<c>d</c> 为归一化距离（1 = 边界）。
        /// 返回 [0,1]，边界处 0.5px 过渡带 ⇒ 简易抗锯齿。
        /// </summary>
        public static float Coverage(float d, float rx = 1f, float ry = 1f)
        {
            float scale = System.Math.Max(0.0001f, System.Math.Min(rx, ry));
            float px = (1f - d) * scale;          // 距边界的像素数（内正外负）
            if (px >= 0.5f) return 1f;
            if (px <= -0.5f) return 0f;
            return px + 0.5f;
        }

        /// <summary>按覆盖率把 <paramref name="src"/> 叠到 <paramref name="dst"/> 上（src 自身 alpha 参与）。</summary>
        public static Color32 Blend(Color32 dst, Color src, float coverage01)
        {
            float a = Clamp01(src.a * coverage01);
            if (a <= 0f) return dst;
            return new Color32(
                (byte)Mathf.RoundToInt((src.r * a + dst.r / 255f * (1f - a)) * 255f),
                (byte)Mathf.RoundToInt((src.g * a + dst.g / 255f * (1f - a)) * 255f),
                (byte)Mathf.RoundToInt((src.b * a + dst.b / 255f * (1f - a)) * 255f),
                (byte)Mathf.RoundToInt(System.Math.Min(1f, dst.a / 255f + a) * 255f));
        }

        private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);
    }
}
