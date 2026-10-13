using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// L0 · 运行时贴图仓库：把 <see cref="TexFmt"/> 算出的像素缓冲变成可绘制的 <c>Texture2D</c> 并缓存。
    ///
    /// <para><b>三条纪律（原版实证）</b>：① 必须主线程（原版
    /// <c>SolidColorMaterials.NewSolidColorTexture</c> 自己就查 <c>UnityData.IsInMainThread</c> 并
    /// <c>Log.Error</c>）；② 必须命名（原版写成 <c>"SolidColorTex-" + color</c>，便于资产审计）；
    /// ③ 必须缓存（原版 <c>SimpleSolidColorMaterial</c> 就是 <c>static Dictionary</c> 模式；
    /// 每帧 <c>new Texture2D</c> 会持续吃显存）。</para>
    ///
    /// <para><b>API 形状 = TryGet / Publish 两段式</b>（而非传 <c>Func</c>）：
    /// 调用方先查缓存，<b>未命中才</b>计算像素 —— 这样每帧的热路径上既无闭包分配，
    /// 也不会白算一遍只在数据变化时才需要的像素。缓存键一律用 <see cref="long"/>（<see cref="Key"/>
    /// 把 kind/两色/尺寸压进一个 long），零字符串分配。</para>
    ///
    /// <para><b>⛔ 不用 <c>MaterialAllocator</c></b>：它是 <c>internal</c>，Mod 不可访问；
    /// 需要材质走 <c>SolidColorMaterials.SimpleSolidColorMaterial</c>（公开且自带缓存）。</para>
    /// </summary>
    public static class TexCache
    {
        /// <summary>缓存上限，超出按插入顺序淘汰最旧一条。</summary>
        public const int MaxEntries = 128;

        private static readonly Dictionary<long, Texture2D> map = new Dictionary<long, Texture2D>();
        private static readonly List<long> order = new List<long>();

        // ------------------------------------------------------------ 键

        /// <summary>
        /// 由 (kind, 两色, 尺寸) 生成缓存键 —— <b>哈希混合</b>实现。
        ///
        /// <para><b>⛔ 2026-09-13 修正（原实现有真实缺陷）</b>：原实现用位段拼接，而 64 位**装不下**
        /// <c>kind(8) + 两色(32+32) + w + h</c>，于是把 <c>w</c> 放在低 4 位之外 ——
        /// <c>(w &amp; 0xFFFF) &lt;&lt; 60</c> 在 <c>w ≥ 16</c> 时<b>整段溢出被丢弃</b>：
        /// 实测 <c>w=16 / 32 / 48</c> 生成<b>同一个键</b> ⇒ 不同宽度的渐变共用同一张贴图（分辨率失真）。
        /// 同时 <c>cb.a</c> 从未参与运算（漏位）。改哈希后<b>每个入参都真实参与计算</b>。</para>
        ///
        /// <para><b>不可证明单射</b>（哈希固有性质），但已由 <c>Tests/TexCacheTests.cs</c> 覆盖常见组合无碰撞。
        /// 需要「入参即判别量、可证明单射」时请用 <see cref="KeyOf"/>。</para>
        ///
        /// <para>返回值清掉最高位 ⇒ 与 <see cref="KeyOf"/>（最高位恒 1）的键空间<b>严格隔离</b>。</para>
        /// </summary>
        public static long Key(int kind, Color a, Color b, int w, int h)
        {
            Color32 ca = a, cb = b;
            long k = Hash64(0x52554620L, (uint)kind);
            k = Hash64(k, ((long)ca.r << 24) | ((long)ca.g << 16) | ((long)ca.b << 8) | ca.a);
            k = Hash64(k, ((long)cb.r << 24) | ((long)cb.g << 16) | ((long)cb.b << 8) | cb.a);   // 补上原先漏掉的 cb.a
            k = Hash64(k, w);
            k = Hash64(k, h);
            return k & 0x7FFFFFFFFFFFFFFFL;      // 清标记位 ⇒ 与 KeyOf 的键空间隔离
        }

        /// <summary>
        /// 由 (kind, 数据版本, 尺寸) 生成键 —— 用于"内容会变"的画布（热力图/图表）。
        ///
        /// <para><b>⛔ 2026-09-13 修正</b>：原实现 <c>version &lt;&lt; 20</c> 与 <c>h &lt;&lt; 42</c>、<c>w &lt;&lt; 52;</c>
        /// 的位段<b>在 <c>version ≥ 2^22</c> 时重叠</b>（约 419 万；用时间戳/哈希当 version 就会踩到）。
        /// 改哈希后无此上限。返回值同样清掉最高位以与 <see cref="KeyOf"/> 隔离。</para>
        /// </summary>
        public static long KeyVersioned(int kind, int version, int w, int h)
        {
            long k = Hash64(0x52554621L, (uint)kind);
            k = Hash64(k, version);
            k = Hash64(k, w);
            k = Hash64(k, h);
            return k & 0x7FFFFFFFFFFFFFFFL;
        }

        /// <summary>FNV-1a 风格的 64 位混合（零分配）。</summary>
        private static long Hash64(long h, long v)
        {
            unchecked { return (h ^ v) * 1099511628211L; }
        }

        /// <summary>
        /// 结构化键（**长载荷版**）：给「大指纹 + 少量参数」的场景 —— 典型是**图表的数据哈希**
        /// （值数组的指纹远超 <see cref="KeyOf"/> 的 11 位槽）。
        ///
        /// <para><paramref name="dataHash"/> 由调用方算（如对量化后的值做 FNV-1a）——
        /// **数据变则键变**，因此调用方**不需要**额外维护版本号，也就不会出现「数据变了但复用旧贴图」的静默错误。</para>
        ///
        /// <para><paramref name="a"/>/<paramref name="b"/>/<paramref name="c"/> **无取值范围限制**（走哈希而非位段拼接）。
        /// 返回值清最高位 ⇒ 与 <see cref="KeyOf"/>（最高位恒 1）严格隔离。</para>
        /// </summary>
        public static long KeyData(int kind, long dataHash, int a = 0, int b = 0, int c = 0)
        {
            long k = Hash64(0x52554630L, (uint)kind);
            k = Hash64(k, dataHash);
            k = Hash64(k, a);
            k = Hash64(k, b);
            k = Hash64(k, c);
            return k & 0x7FFFFFFFFFFFFFFFL;
        }

        // ------------------------------------------------------------ 结构化键（新代码请优先用它）

        /// <summary>
        /// <b>结构化缓存键构造器（推荐）</b> —— 由若干「判别参数」直接生成键，
        /// <b>调用方不需要知道位段布局，也不需要自己拼位</b>。
        ///
        /// <para><b>为什么要有它</b>：<see cref="Key"/> / <see cref="KeyVersioned"/> 用位段拼接，
        /// 各自带静默陷阱 —— <c>kind</c> 被 <c>&amp; 0xFF</c> 截断；<c>version</c> 左移 20 与 <c>w/h</c>
        /// 位段在 <c>version ≥ 2^22</c> 时重叠；<c>w</c> 左移 60 在大尺寸下溢出。
        /// 调用方一旦手拼，就可能「**算了某个判别量却忘了放进键**」——
        /// 编译器不报、逻辑上又像做了处理，只在运行期表现为「两张不同的图共用同一张贴图」。
        /// （2026-09-13 实证事故：<c>DonutChart</c> 算出孔洞比却没传进键 ⇒ 饼图取到环图贴图、中间带孔。）</para>
        ///
        /// <para><b>位段</b>（共 64 位，段间不重叠）：
        /// <c>[63] = 标记位（恒 1）</c>，<c>kind[0..7]</c>，<c>a/b/c/d/e</c> 各 <c>[11 位]</c>。
        /// 即 <paramref name="kind"/> ∈ [0,255]，<paramref name="a"/>…<paramref name="e"/> ∈ [0,2047]。
        /// <b>标记位保证本构造器产出的键与旧的 <see cref="Key"/> / <see cref="KeyVersioned"/> 永不撞键。</b></para>
        ///
        /// <para><b>越界不静默</b>：任何入参越界都会被夹紧，并 <c>Log.ErrorOnce</c> 报一次
        /// （提示改用更小的量化步长或扩展位段）—— 绝不悄悄丢位。</para>
        /// </summary>
        public static long KeyOf(int kind, int a = 0, int b = 0, int c = 0, int d = 0, int e = 0)
        {
            if (kind < 0 || kind > 255)
            {
                Log.ErrorOnce("RUIF.TexCache.KeyOf: kind=" + kind + " 超出 [0,255]，已夹紧。", 0x52554610);
                kind = kind < 0 ? 0 : 255;
            }
            long k = 1L << 63;                                   // ★ 标记位：与旧构造器隔离
            k |= (uint)kind;                                     // bit  0..7
            k |= (long)Seg("a", a, 0x52554611) << 8;             // bit  8..18
            k |= (long)Seg("b", b, 0x52554612) << 19;            // bit 19..29
            k |= (long)Seg("c", c, 0x52554613) << 30;            // bit 30..40
            k |= (long)Seg("d", d, 0x52554614) << 41;            // bit 41..51
            k |= (long)Seg("e", e, 0x52554615) << 52;            // bit 52..62
            return k;
        }

        /// <summary>11 位位段取值：越界夹紧 + 只报一次日志（绝不静默丢位）。</summary>
        private static int Seg(string name, int v, int seed)
        {
            if (v >= 0 && v <= 2047) return v;
            Log.ErrorOnce("RUIF.TexCache.KeyOf: 参数 " + name + "=" + v + " 超出 [0,2047]，已夹紧 —— " +
                          "夹紧后可能与其它值共用键；请减小量化步长或扩展位段。", seed);
            return v < 0 ? 0 : 2047;
        }

        /// <summary>
        /// <b>扇区贴图键</b>（<c>Charts.DonutChart</c> / 仪表圆弧等）：色值 + 起角 + 张角 + 孔洞比 + 边长。
        ///
        /// <para><b>色值直接进键（不是调色板序号）</b> ⇒ <b>主题切换后键自动变化、缓存自动失效</b>，
        /// 调用方不必记得调 <c>TexCache.Clear()</c>。（原实现传序号，换主题后会一直画旧色。）</para>
        ///
        /// <para>角度量化到 <b>1°</b> —— 环形图上肉眼不可见，但能让动画数据也命中缓存。
        /// 五个判别量全部进键，与「孔洞比不同 ⇒ 键必须不同」直接对应（2026-09-13 事故的对策）。</para>
        /// </summary>
        public static long KeySector(Color color, float startDeg, float sweepDeg, float inner01, int size)
            => KeyOf(0x53,
                     FoldColor(color),
                     ((Mathf.RoundToInt(startDeg) % 720) + 720) % 720,
                     Mathf.Clamp(Mathf.RoundToInt(sweepDeg), 0, 2047),
                     Mathf.Clamp(Mathf.RoundToInt(inner01 * 100f), 0, 100),
                     size);

        /// <summary>把 RGBA 折叠成 11 位（供 <see cref="KeyOf"/> 的位段用）。6 色调色板内无误撞风险。</summary>
        private static int FoldColor(Color c)
        {
            Color32 q = c;
            return (q.r * 31 + q.g * 17 + q.b * 7 + q.a) & 0x7FF;
        }

        // ------------------------------------------------------------ 两段式

        /// <summary>查缓存；命中且贴图存活返回 true。</summary>
        public static bool TryGet(long key, out Texture2D tex)
        {
            if (map.TryGetValue(key, out tex))
            {
                if (tex != null) return true;
                map.Remove(key);        // 被外部 Destroy 过 ⇒ 视为未命中
                order.Remove(key);
            }
            tex = null;
            return false;
        }

        /// <summary>发布像素缓冲为贴图并登记缓存（长度须为 w*h）。主线程外调用会被拒并返回 null。</summary>
        public static Texture2D Publish(long key, int w, int h, Color32[] pixels, string name = null)
        {
            if (w <= 0 || h <= 0 || pixels == null || pixels.Length != w * h) return null;
            if (!UnityData.IsInMainThread)
            {
                Log.ErrorOnce("RUIF.TexCache: 尝试在非主线程创建贴图（已忽略）。", 0x52554601);
                return null;
            }
            Texture2D old;
            if (map.TryGetValue(key, out old) && old != null) Object.Destroy(old);

            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.name = "RUIF.Tex-" + (name ?? key.ToString("X16"));
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = (w == 1 || h == 1) ? FilterMode.Bilinear : FilterMode.Point;
            tex.SetPixels32(pixels);
            tex.Apply(false, false);

            map[key] = tex;
            if (!order.Contains(key)) order.Add(key);
            EvictIfNeeded();
            return tex;
        }

        /// <summary>清空缓存并释放贴图（切换存档 / 主题时可用）。</summary>
        public static void Clear()
        {
            for (int i = 0; i < order.Count; i++)
            {
                Texture2D t;
                if (map.TryGetValue(order[i], out t) && t != null) Object.Destroy(t);
            }
            map.Clear();
            order.Clear();
        }

        private static void EvictIfNeeded()
        {
            while (order.Count > MaxEntries)
            {
                long oldest = order[0];
                order.RemoveAt(0);
                Texture2D t;
                if (map.TryGetValue(oldest, out t))
                {
                    map.Remove(oldest);
                    if (t != null) Object.Destroy(t);
                }
            }
        }
    }
}
