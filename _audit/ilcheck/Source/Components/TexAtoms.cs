using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// A0 · 贴图原子族：把「原版没有的能力」（真渐变 / 真渐隐 / 任意画布 / 饼环）用
    /// <see cref="TexFmt"/> 算像素 + <see cref="TexCache"/> 上屏的方式补齐。
    ///
    /// <para><b>为什么走贴图而不是描线</b>：原版 UI 侧无渐变 API、无多边形填充 API
    /// （第二轮取证：全类型表检索 <c>Gradient</c> 只命中 <c>Verse.Noise.GradientPresets</c>）。
    /// 而原版自己造贴图有先例（<c>SolidColorMaterials.NewSolidColorTexture</c>）。
    /// 贴图路线的附加好处是<b>一次 draw call</b> —— 热力图 365 格从 365 次 <c>DrawBoxSolid</c>
    /// 降到 1 次 <c>GUI.DrawTexture</c>。</para>
    ///
    /// <para><b>降级契约</b>：贴图创建失败（非主线程 / 尺寸非法）时<b>不静默</b> ——
    /// 渐变退化为 12 段折线，其余直接不画并已由 <c>TexCache</c> 记录一次错误。</para>
    ///
    /// <para><b>绘制前一律把 <c>GUI.color</c> 置白</b>：颜色已烘进像素，
    /// 若沿用外层的染色状态会被二次相乘（这是"渐变发暗"的常见根因）。</para>
    /// </summary>
    public static class TexAtoms
    {
        private const int GradSize = 256;
        private const int FadeSize = 48;

        // ------------------------------------------------------------ 渐变 / 渐隐

        /// <summary>线性渐变底（真渐变，1 次 draw call）。贴图不可用时降级为 12 段折线。</summary>
        public static void GradientRect(Rect rect, Color lo, Color hi, bool horizontal = true)
        {
            int w = horizontal ? GradSize : 1;
            int h = horizontal ? 1 : GradSize;
            long key = TexCache.Key(1, lo, hi, w, h);
            Texture2D t;
            if (!TexCache.TryGet(key, out t))
            {
                t = TexCache.Publish(key, w, h,
                    horizontal ? TexFmt.HGradient(GradSize, lo, hi) : TexFmt.VGradient(GradSize, lo, hi),
                    horizontal ? "GradH" : "GradV");
            }
            if (t != null)
            {
                using (Draw.Color(Color.white)) GUI.DrawTexture(rect, t);
                return;
            }
            FallbackGradient(rect, lo, hi, horizontal);
        }

        /// <summary>
        /// 右缘渐隐遮罩（真·渐隐截断）：在 <paramref name="rect"/> 右端盖一条
        /// 「alpha 0 → 1、RGB = <paramref name="bgColor"/>」的贴图，把文字尾部溶进背景。
        /// <para><paramref name="bgColor"/> 必须是<b>文字背后的真实底色</b>，否则渐隐带会与底色不接。</para>
        /// </summary>
        public static void FadeRight(Rect rect, Color bgColor, float width = 24f) => FadeEdge(rect, bgColor, width, true);

        /// <summary>左缘渐隐遮罩（横向滚动列表的起始侧）。</summary>
        public static void FadeLeft(Rect rect, Color bgColor, float width = 24f) => FadeEdge(rect, bgColor, width, false);

        private static void FadeEdge(Rect rect, Color bgColor, float width, bool right)
        {
            if (rect.width <= 1f || rect.height <= 0f) return;
            float w = Mathf.Min(Mathf.Max(2f, width), rect.width);
            Rect band = right ? new Rect(rect.xMax - w, rect.y, w, rect.height)
                              : new Rect(rect.x, rect.y, w, rect.height);
            Texture2D t = FadeTex(bgColor, right);
            if (t == null) return;
            using (Draw.Color(Color.white)) GUI.DrawTexture(band, t);
        }

        private static Texture2D FadeTex(Color bg, bool right)
        {
            float from = right ? 0f : 1f;
            float to = right ? 1f : 0f;
            var ka = new Color(bg.r, bg.g, bg.b, from);
            var kb = new Color(0f, 0f, 0f, to);
            long key = TexCache.Key(2, ka, kb, FadeSize, 1);
            Texture2D t;
            if (!TexCache.TryGet(key, out t))
            {
                t = TexCache.Publish(key, FadeSize, 1, TexFmt.AlphaFade(FadeSize, bg, from, to), "Fade");
            }
            return t;
        }

        // ------------------------------------------------------------ 任意画布

        /// <summary>
        /// 发布并绘制一块画布（热力图 / 甘特 / 桑基 / 任意自绘几何）。
        /// <para><b>缓存键由调用方给</b>：内容变了必须换 key（把数据版本号编进
        /// <see cref="TexCache.KeyVersioned"/> 是最省事的做法），否则会一直画旧图。</para>
        /// </summary>
        public static bool Canvas(Rect rect, long key, int w, int h, Color32[] pixels, string name = null)
        {
            Texture2D t;
            if (!TexCache.TryGet(key, out t)) t = TexCache.Publish(key, w, h, pixels, name);
            if (t == null) return false;
            using (Draw.Color(Color.white)) GUI.DrawTexture(rect, t);
            return true;
        }

        /// <summary>
        /// 热力格网（值 → 五档色阶）。<paramref name="version"/> 变化才会重算像素。
        /// <paramref name="flipY"/> = true 时 <c>grid[0,*]</c> 画在<b>顶部</b>（表格直觉）。
        /// </summary>
        public static void Heat(Rect rect, int version, float[,] grid, float lo, float hi, bool flipY = true)
        {
            if (grid == null) return;
            int rows = grid.GetLength(0), cols = grid.GetLength(1);
            if (rows <= 0 || cols <= 0) return;
            long key = TexCache.KeyVersioned(3, version, cols, rows);
            Texture2D t;
            if (!TexCache.TryGet(key, out t))
            {
                var px = new Color32[cols * rows];
                float span = Mathf.Max(0.0001f, hi - lo);
                for (int r = 0; r < rows; r++)
                {
                    int y = flipY ? rows - 1 - r : r;
                    for (int c = 0; c < cols; c++)
                    {
                        px[y * cols + c] = TexFmt.Heat((grid[r, c] - lo) / span);
                    }
                }
                t = TexCache.Publish(key, cols, rows, px, "Heat");
            }
            if (t == null) return;

            // 竖直方向保持点采样（格网要清晰），因此用 Nearest 拉伸由 filterMode=Point 保证
            using (Draw.Color(Color.white)) GUI.DrawTexture(rect, t);
        }

        /// <summary>
        /// 扇形 / 扇环（饼图、环图、仪表圆弧）。<paramref name="key"/> 需包含角度与颜色信息
        /// （推荐 <see cref="TexCache.KeyVersioned"/> 把量化后的角度编进 version）。
        /// </summary>
        public static void Sector(Rect rect, long key, float startDeg, float sweepDeg, float inner01, Color color,
                                  int size = 128)
        {
            Texture2D t;
            if (!TexCache.TryGet(key, out t))
            {
                float r = size * 0.5f - 1f;
                var px = new Color32[size * size];
                TexFmt.FillSector(px, size, size, size * 0.5f, size * 0.5f, r, r, startDeg, sweepDeg, inner01, color);
                t = TexCache.Publish(key, size, size, px, "Sector");
            }
            if (t == null) return;
            using (Draw.Color(Color.white)) GUI.DrawTexture(rect, t);
        }

        // ------------------------------------------------------------ 降级

        private static void FallbackGradient(Rect rect, Color lo, Color hi, bool horizontal)
        {
            const int seg = 12;
            for (int i = 0; i < seg; i++)
            {
                float a = (float)i / seg, b = (float)(i + 1) / seg;
                Color c = TexFmt.Ramp(lo, hi, (a + b) * 0.5f);
                Rect piece = horizontal
                    ? new Rect(rect.x + rect.width * a, rect.y, rect.width * (b - a) + 1f, rect.height)
                    : new Rect(rect.x, rect.y + rect.height * a, rect.width, rect.height * (b - a) + 1f);
                Draw.Box(piece, c);
            }
        }
    }
}
