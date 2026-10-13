using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// 工业管理原子纯计算（可离线单测，v1.2 第一批 T1 四原子）：
    /// 表盘归一/分区、瓶颈检测、占比归一、状态游程归并、灯阵格几何。
    /// 不触 GUI、不读输入、无全局状态——与 Fmt 家族同一铁律。
    /// </summary>
    public static class IndustrialFmt
    {
        /// <summary>表盘归一：v ∈ [lo,hi] → [0,1]（钳制；退化区间返回 0，不除零）。</summary>
        public static float GaugeNorm(float v, float lo, float hi)
        {
            if (hi - lo <= 1e-5f) return 0f;
            return Mathf.Clamp01((v - lo) / (hi - lo));
        }

        /// <summary>
        /// 表盘分区：v01 与阈值比较 → 0 正常 / 1 警告 / 2 危险。
        /// 仅当 dead01 &gt; warn01 时危险区才生效（阈值倒置的坏配置不误报危险，fail-safe）。
        /// </summary>
        public static int GaugeZone(float v01, float warn01, float dead01)
        {
            v01 = Mathf.Clamp01(v01);
            warn01 = Mathf.Clamp01(warn01);
            dead01 = Mathf.Clamp01(dead01);
            if (v01 < warn01) return 0;
            return dead01 > warn01 && v01 >= dead01 ? 2 : 1;
        }

        /// <summary>瓶颈索引：耗时最大段；null / 空 / 全非正返回 -1。</summary>
        public static int BottleneckIndex(float[] stageTimes)
        {
            if (stageTimes == null || stageTimes.Length == 0) return -1;
            int idx = -1;
            float max = 0f;
            for (int i = 0; i < stageTimes.Length; i++)
                if (stageTimes[i] > max) { max = stageTimes[i]; idx = i; }
            return idx;
        }

        /// <summary>
        /// 占比归一：values 归一到和为 1 写入 outBuf，返回有效段数（不超出 buf 容量）。
        /// 负值按 0 处理；总和 ≤ 0（全空/全负）退化为均匀 1/n——绘制端永远拿得到合法占比。
        /// </summary>
        public static int Shares(float[] values, float[] outBuf)
        {
            int n = values == null ? 0 : values.Length;
            if (outBuf == null || outBuf.Length == 0 || n == 0) return 0;
            n = Mathf.Min(n, outBuf.Length);
            float total = 0f;
            for (int i = 0; i < n; i++) total += Mathf.Max(0f, values[i]);
            if (total <= 1e-6f)
            {
                for (int i = 0; i < n; i++) outBuf[i] = 1f / n;
                return n;
            }
            for (int i = 0; i < n; i++) outBuf[i] = Mathf.Max(0f, values[i]) / total;
            return n;
        }

        /// <summary>
        /// 状态游程归并：相邻同状态合并为一段，段长写入 outBuf，返回段数（截断至 buf 容量）。
        /// 供「逐 tick 状态采样 → 分段时序带」的预处理，纯 C# 可离线验证。
        /// </summary>
        public static int MergeRunLengths(int[] states, int[] outBuf)
        {
            if (states == null || states.Length == 0 || outBuf == null || outBuf.Length == 0) return 0;
            int segs = 0, run = 1;
            for (int i = 1; i <= states.Length; i++)
            {
                if (i < states.Length && states[i] == states[i - 1]) { run++; continue; }
                if (segs < outBuf.Length) outBuf[segs] = run;
                segs++;
                run = 1;
            }
            return Mathf.Min(segs, outBuf.Length);
        }

        /// <summary>灯阵格几何：第 i 格矩形（均分行列，1px 缝隙）。</summary>
        public static Rect LedCell(Rect r, int i, int cols, int rows)
        {
            cols = Mathf.Max(1, cols);
            rows = Mathf.Max(1, rows);
            int cx = i % cols, cy = i / cols;
            float cw = r.width / cols, ch = r.height / rows;
            return new Rect(r.x + cx * cw + 0.5f, r.y + cy * ch + 0.5f,
                Mathf.Max(1f, cw - 1f), Mathf.Max(1f, ch - 1f));
        }

        /// <summary>状态段总时长（归一化分母）；null / 全非正返回 1（除零守护，退化为每段等宽）。</summary>
        public static float SegmentsTotal(float[] durations)
        {
            if (durations == null) return 1f;
            float t = 0f;
            for (int i = 0; i < durations.Length; i++) if (durations[i] > 0f) t += durations[i];
            return t <= 1e-6f ? 1f : t;
        }
    }

    /// <summary>设备状态（工业管理语义，与 <see cref="Industrial.StateColor"/> 一一对应）。</summary>
    public enum MachineState { Run, Idle, Fault, Refuel, Offline }

    /// <summary>
    /// 工业管理原子（v1.2 第一批 T1 四原子，v1.0 API 冻结之上的纯增量）：
    /// Gauge 指针表盘 / LEDMatrix 状态灯阵 / BottleneckRibbon 瓶颈带 / MachineStateTimeline 设备状态时序。
    ///
    /// 铁律：
    /// ① 颜色只映射令牌（ZoneColor/LedColor/StateColor 三张色表，禁裸 new Color）；
    /// ② 不读 Mouse/Event——纯输出组件，数据由调用方给（孪生取数走 DataBridge，见设计提案）；
    /// ③ 零贴图：全部 Draw.Box / Draw.Border / Widgets.DrawLine 直绘；
    /// ④ 零逐帧堆分配（占比缓冲框架内静态复用）。
    /// </summary>
    public static class Industrial
    {
        private static readonly float[] shareBuf = new float[16];   // 占比复用缓冲（IMGUI 单线程，安全）

        // ---- Gauge 指针表盘 ----

        /// <summary>
        /// 表盘顶部<b>标题带</b>高度（= 标签字号的<b>引擎行高</b> + 2px 余量）：穹顶最高点不得越过它，见 <see cref="Gauge"/>。
        ///
        /// <para><b>⚠ 必须是属性，不能是 <c>const</c> / <c>static readonly</c></b>：
        /// <c>const</c> 不能调方法；<c>static readonly</c> 会把 <c>Text.TinyFontSupported</c>
        /// 在类型初始化时<b>冻成定值</b> —— 而后者运行期可变（玩家可随时勾选「禁用小字体」）
        /// ⇒ 玩家切一次选项就拿到错行高。属性表达式每次调用重新求值，
        /// 与 <see cref="Measure.RowHeight(GameFont)"/> 同源。</para>
        ///
        /// <para>历史值 <c>15f</c> 是"14px 标签带 + 1"的手写估计，与标签矩形高度
        /// （原写死 <c>14f</c>）各写一份 ⇒ 两处一旦不同步就重现「标题被遮挡」。现已同源。</para>
        /// </summary>
        private static float GaugeLabelBand => Measure.RowHeight(Tokens.FontLabel) + 2f;

        /// <summary>刻度总格数（<b>每格 5%</b> ⇒ 21 个刻度位；主刻度每 25%，两端天然是主刻度）。</summary>
        private const int GaugeTickSteps = 20;

        /// <summary>
        /// 指针表盘：上半环分区弧（正常/警告/危险）+ **主/次刻度** + 指针 + 中心数值。
        /// v01 落入的分区决定数值文本颜色；warn01/dead01 为阈值（0-1，见 <see cref="IndustrialFmt.GaugeZone"/>）。
        ///
        /// <para><b>径向分层</b>（由外向内，互不压叠）：
        /// 环带 <c>[radius−8, radius]</c> → 刻度基线 <c>radius−11</c> → 主刻度到 <c>radius−17</c> /
        /// 次刻度到 <c>radius−14</c> → 指针长 ≤ <c>radius−19</c> → 轴心圆 2.5。</para>
        ///
        /// <para><b>① 标题带（v1.1.2 · 实机「标题被遮挡」根因）</b>：左上标签独占顶部
        /// <see cref="GaugeLabelBand"/> px，穹顶最高点被钳在该带下沿。
        /// 原实现半径取 <c>r.height−20</c>、轴心 <c>r.yMax−8</c> ⇒ 弧顶落在 <c>r.y+12</c>，
        /// 与 14px 高的标签带**重叠 2px**；标签左对齐、长度一旦越过卡片中线，
        /// <b>末字正好压在外圈弧顶上</b>（弧是不透明亮色 → 文字读不出来）。
        /// 修法 = 把"弧顶 ≤ 标题带下沿"做成几何约束（<c>radius ≤ c.y − (r.y + LabelBand)</c>），
        /// 而不是靠调用方把标题改短。</para>
        ///
        /// <para><b>2026-09-16 补</b>：带高与标签矩形高度此后<b>同源</b>（都 = <c>Measure.RowHeight(FontLabel)</c>），
        /// 历史值 15f / 14f 两个手写数已撤除 —— 上面那句"14px 高的标签带"是<b>旧值</b>，
        /// 真值随引擎行高与玩家「禁用小字体」选项变化。</para>
        ///
        /// <para><b>② 刻度密度（v1.1.2）</b>：原为 12.5% 一格（9 位）⇒ 相邻两格之间的量级读不出来；
        /// 改为 <b>5% 一格（21 位）</b>，每 25% 为主刻度（更长、更亮），
        /// 与 <c>warn01/dead01</c> 的 60% / 85% 分区边界一起构成可读量程。</para>
        /// </summary>
        public static void Gauge(Rect r, float v, float lo, float hi, string label = null,
            float warn01 = 0.6f, float dead01 = 0.85f)
        {
            float v01 = IndustrialFmt.GaugeNorm(v, lo, hi);
            int zone = IndustrialFmt.GaugeZone(v01, warn01, dead01);
            Vector2 c = new Vector2(r.center.x, r.yMax - 6f);
            // 轴心下沉到 yMax−6 换取 2px 高度，半径再被「标题带下沿」钳一次 ⇒ 弧顶恰 = r.y + GaugeLabelBand
            // （GaugeLabelBand 是属性：值随引擎行高与「禁用小字体」选项变化，不再是定值 15）。
            float radius = Mathf.Min(r.width * 0.42f, c.y - (r.y + GaugeLabelBand));
            if (radius > 2f)
            {
                // ★ 贴图烘焙：分区弧 + 刻度 + 指针 + 轴心**全部算进一张缓冲**，最后**一次** GUI.DrawTexture。
                //
                // ⛔ 为什么不能用前两种写法（2026-09-13 两条实证）：
                //   ① 逐段 Draw.Rotate（原实现，45 次/帧 GUI.matrix 存取）⇒ 在**滚动视图内飞散**；
                //   ② 改用 Widgets.DrawLine（我试过）⇒ **同样飞散** —— 它内部是
                //      GL.PushMatrix/GL.MultMatrix，改的是 **GL 矩阵**，与含滚动平移的 GUI.matrix 互不感知。
                // ✅ 唯一同时绕开两条机制的方式：**CPU 算像素 + 未旋转的 GUI.DrawTexture**（零矩阵、零 GL）。
                //
                // ⛔⛔ 2026-09-13 二次根因（消费者实机截图：表盘**整体倒置**，呈「开口朝上」的 ∪、指针甩到卡片顶部）：
                //   像素缓冲的 y 语义与 GUI **相反** —— <see cref="TexFmt"/> / <see cref="TexCache"/> 约定
                //   **y=0 在纹理左下**（`SetPixels32` 的原始顺序，见 TexFmt 类注释「坐标约定」）；
                //   而本方法此前把 GUI 空间（y 向下为正）的 cy 与 sin 符号**原样**写进缓冲
                //   ⇒ 整张表盘沿 y 轴镜像（x 不受影响，故只有"上下颠倒"这一种表现）。
                //   修法：**一律先算 GUI 局部坐标，再由 `th − 1 − y` 转成缓冲行号**（"屏幕上方" = "行号更大"）；
                //   所有几何（弧 / 刻度 / 指针）**必须走同一转换**，否则刻度会与弧反向。
                int tw = Mathf.Max(8, Mathf.CeilToInt(r.width));
                int th = Mathf.Max(8, Mathf.CeilToInt(r.height));
                // ⛔ 键构造此前**越界**（实机 Player.log 每次首次烘焙都报一次）：
                //   KeyOf 的每个槽只有 11 位 [0,2047]，而阈值槽 = warn×100×100 + dead = 60×100+85 = **6085**
                //   ⇒ 被夹紧到 2047（日志原话"夹紧后可能与其它值共用键"）。改用 KeyData：入参无限幅
                //   ⇒ 阈值原样进键，既不丢判别量、也不再刷日志。
                //   ⚠️ **zone 必须留在键里**：pct 是 1% 量化，两个落进同一 pct 桶的 v01 可能跨过阈值
                //   ⇒ 分区色不同而 pct 相同（"zone 可由 (pct,warn,dead) 推出"并不成立）。
                long key = TexCache.KeyData(0x54,
                    ((long)zone << 32) | (uint)Mathf.RoundToInt(v01 * 100f),           // 分区 + 指针位置：1% 量化
                    tw, th,
                    Mathf.RoundToInt(warn01 * 100f) * 100 + Mathf.RoundToInt(dead01 * 100f));   // 阈值也进键（否则改阈值会复用旧弧色）
                Texture2D tex;
                if (!TexCache.TryGet(key, out tex))
                {
                    Color32[] px = new Color32[tw * th];
                    float cx = c.x - r.x;                       // x 轴：GUI 与缓冲同向，无需变换
                    float cy = th - 1f - (c.y - r.y);           // y 轴：GUI(向下为正) → 缓冲(向上为正)
                    // 径向分层（由外向内），保证弧 / 刻度 / 指针三者不压叠
                    float rArcIn = radius - 8f, rTickBase = radius - 11f;
                    float rTickMajor = radius - 17f, rTickMinor = radius - 14f;
                    float needleLen = Mathf.Max(4f, radius - 19f);
                    for (float a = 180f; a < 360f; a += 4f)
                    {
                        float t01 = (a - 180f) / 180f;
                        Color zc = ZoneColor(IndustrialFmt.GaugeZone(t01, warn01, dead01));
                        float ra2 = a * Mathf.Deg2Rad;
                        float ca = Mathf.Cos(ra2), sa = -Mathf.Sin(ra2);   // 缓冲空间：屏幕上方 = 行号增大
                        TexFmt.Line(px, tw, th,
                            cx + ca * rArcIn, cy + sa * rArcIn,
                            cx + ca * radius, cy + sa * radius, zc, 3f);
                    }
                    // 刻度（补「没有刻度 ⇒ 读不出量级」缺口）：0–100% **每 5% 一格**
                    // （GaugeTickSteps=20 ⇒ 21 个刻度位），每 25% 为主刻度（沿线更长、更亮）。
                    // 次刻度压到 1px 宽：5% 的栅格在半径 40 上相邻间距仅约 4.7px，1.5px 会糊成一片。
                    if (rTickBase > 3f)
                    {
                        for (int i = 0; i <= GaugeTickSteps; i++)
                        {
                            bool major = (i % 5) == 0;                                 // 每 25%
                            float ra3 = (180f + 180f * (i / (float)GaugeTickSteps)) * Mathf.Deg2Rad;
                            float ca = Mathf.Cos(ra3), sa = -Mathf.Sin(ra3);
                            float r1 = major ? rTickMajor : rTickMinor;
                            TexFmt.Line(px, tw, th,
                                cx + ca * rTickBase, cy + sa * rTickBase,
                                cx + ca * r1, cy + sa * r1,
                                major ? Tokens.Dim : Tokens.BorderSoft, major ? 2f : 1f);
                        }
                    }
                    float rad = GaugeNeedleDeg(v01) * Mathf.Deg2Rad;
                    TexFmt.Line(px, tw, th, cx, cy,
                        cx + Mathf.Cos(rad) * needleLen, cy - Mathf.Sin(rad) * needleLen,
                        Tokens.Text, 2f);
                    TexFmt.FillDisc(px, tw, th, cx, cy, 2.5f, 2.5f, Tokens.Text);
                    tex = TexCache.Publish(key, tw, th, px, "Gauge");
                }
                if (tex != null)
                    using (Draw.Color(Color.white)) GUI.DrawTexture(r, tex);
            }
            // 标签（左上，**独占标题带**）+ 数值（轴心上方，随分区变色）—— 两者是 GUI 直绘，
            // **不参与**上面的缓冲烘焙，因此不受镜像问题影响（实机截图中「100%」读数方向一直是对的）。
            if (!string.IsNullOrEmpty(label))
            {
                // +4 让开消费者画在卡片左缘的 2px accent 色条；按可用宽截断 ⇒ 长标题也不可能溢出卡外。
                float labelWidth = Mathf.Max(8f, r.width - 8f);
                // 标签矩形高与 GaugeLabelBand **引用同一表达式**（同源 ⇒ 不会再各自漂移）。
                // ⚠ 原写死 14f：`GUI.Label` 不裁切，14px 容器装 22px 文字 ⇒ 文字底边外溢到 r.y+22，
                //   而弧顶被钳在 r.y + GaugeLabelBand ⇒ 白送 7px 压弧（实机「标题被遮挡」根因）。
                float band = Measure.RowHeight(Tokens.FontLabel);
                Draw.Label(new Rect(r.x + 4f, r.y, labelWidth, band),
                    Measure.Clamp(label, labelWidth, Tokens.FontLabel),
                    Tokens.FontLabel, Tokens.Muted, TextAnchor.UpperLeft);
            }
            // 数值字（FontValue = Medium，引擎行高 ≠ 22）矩形高度取引擎真值。
            // ⚠ 必须加竖直补偿 -(vh − 22f) / 2f：`MiddleCenter` 下容器高度变化会整体平移文字中心；
            //   原写死 22f 而 Medium 实际行高更大 ⇒ 文字整体下移 (vh−22)/2 px，压到轴心与指针上。
            // 几何量 c.y − 26f / radius 均来自本方法开头的分层推导，本次不动。
            float vh = Measure.RowHeight(Tokens.FontValue);
            Draw.Label(new Rect(c.x - radius, c.y - 26f - (vh - 22f) / 2f, radius * 2f, vh),
                Mathf.RoundToInt(v01 * 100f) + "%",
                Tokens.FontValue, ZoneColor(zone), TextAnchor.MiddleCenter);
        }

        /// <summary>指针角度（度）：v01=0 → 180°（左），v01=1 → 360°（右），上半环经过顶部。</summary>
        public static float GaugeNeedleDeg(float v01) => 180f + 180f * Mathf.Clamp01(v01);

        /// <summary>表盘分区色：0 正常 / 1 警告 / 2 危险（只映射令牌）。</summary>
        public static Color ZoneColor(int zone)
            => zone <= 0 ? Tokens.Alive : zone == 1 ? Tokens.Warn : Tokens.Dead;

        // ---- LEDMatrix 状态灯阵 ----

        /// <summary>状态灯阵：一格一机，颜色即状态；警告/故障格加描边提醒。</summary>
        public static void LedMatrix(Rect r, int[] statuses, int cols)
        {
            if (statuses == null || statuses.Length == 0) return;
            cols = Mathf.Max(1, cols);
            int rows = Mathf.CeilToInt(statuses.Length / (float)cols);
            for (int i = 0; i < statuses.Length; i++)
            {
                Rect cell = IndustrialFmt.LedCell(r, i, cols, rows);
                Draw.Box(cell, LedColor(statuses[i]));
                if (statuses[i] >= 2)
                    Draw.Border(cell, statuses[i] >= 3 ? Tokens.Text : Tokens.Border);
            }
        }

        /// <summary>灯阵状态色：0 熄 / 1 正常 / 2 警告 / 3 故障（≥4 归故障）。</summary>
        public static Color LedColor(int status)
            => status <= 0 ? Tokens.BorderHair
             : status == 1 ? Tokens.Alive
             : status == 2 ? Tokens.Warn
             : Tokens.Dead;

        // ---- BottleneckRibbon 瓶颈带 ----

        /// <summary>
        /// 瓶颈带：各段耗时占比条带，最大段标红描边并在下方标注占比。
        /// 段数上限 16（框架复用缓冲）；标签仅在该段足够宽（&gt;24px）时绘制。
        /// </summary>
        public static void BottleneckRibbon(Rect r, string[] stageLabels, float[] stageTimes,
            Color? color = null)
        {
            if (stageTimes == null || stageTimes.Length == 0) return;
            Color c = color ?? Tokens.Info;
            int bottleneck = IndustrialFmt.BottleneckIndex(stageTimes);
            // 标签行字高（原写死 14f —— 装不下 22px 文字，`GUI.Label` 不裁切 ⇒ 压在条带上）；
            // 段内标签居中与该行高同源，下方瓶颈注脚也用同一值。
            float labelRowH = Measure.RowHeight(Tokens.FontLabel);
            // 条带高 = 正文行高（原写死 22f，是 Small 的估计值，改用引擎真值）；
            // 有注脚时为其预留「标签行高 + 2px」（原写死 18f，同样是估计值）。
            float barH = Mathf.Min(Measure.RowHeight(Tokens.FontBody),
                r.height - (bottleneck >= 0 ? labelRowH + 2f : 2f));
            int m = IndustrialFmt.Shares(stageTimes, shareBuf);
            float x = r.x;
            for (int i = 0; i < m; i++)
            {
                float w = Mathf.Max(1f, r.width * shareBuf[i] - 2f);
                Rect seg = new Rect(x, r.y, w, barH);
                if (i == bottleneck)
                {
                    Draw.Box(seg, Tokens.Dead);
                    Draw.Border(new Rect(seg.x - 1f, seg.y - 1f, seg.width + 2f, seg.height + 2f), Tokens.Text);
                }
                else
                {
                    Draw.Box(seg, c);
                }
                if (stageLabels != null && i < stageLabels.Length && w > 24f)
                {
                    // ⚠ 2026-10-05 修复（消费方 PersonalChronicle 实机缺陷）：
                    //   原用 Draw.Label —— 它是**硬切/溢出**语义（GUI.Label 不裁剪，MiddleCenter 锚点下
                    //   超宽文字会向两侧溢出矩形）⇒ 段内长标签压住相邻段。消费方截图实证：负载条上
                    //   一段孤立白字「待机（电力）」飘在条带右侧，实际是它所在段太窄装不下。
                    //   改用 EllipsisLabel（走 Widgets.LabelEllipses → 原版 Text.ClampTextWithEllipsis）：
                    //   放不下即补省略号，**永不越界**。API 与调用方契约不变。
                    Draw.EllipsisLabel(new Rect(x, r.y + barH / 2f - labelRowH / 2f, w, labelRowH),
                        stageLabels[i], Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleCenter);
                }
                x += w + 2f;
            }
            if (bottleneck >= 0 && stageLabels != null && bottleneck < stageLabels.Length)
                Draw.Label(new Rect(r.x, r.y + barH + 3f, r.width, labelRowH),
                    Tr.Get("RUIF.Comp.Industrial.Bottleneck", stageLabels[bottleneck], (shareBuf[bottleneck] * 100f).ToString("0")),
                    Tokens.FontLabel, Tokens.Dead, TextAnchor.UpperLeft);
        }

        // ---- MachineStateTimeline 设备状态时序 ----

        /// <summary>
        /// 设备状态时序：多行 × 状态段（rows[i] 与 durations[i] 平行，durations 为相对时长）。
        /// 故障段加文本色描边；labels 可选（行首标签列）。
        /// </summary>
        public static void MachineStateTimeline(Rect r, string[] labels, MachineState[][] rows,
            float[][] durations)
        {
            if (rows == null || rows.Length == 0) return;
            bool hasLabels = labels != null && labels.Length > 0;
            float labelW = hasLabels ? Mathf.Min(72f, r.width * 0.3f) : 0f;
            // 有行首标签时，行高下限必须是「标签行高」（原写死 8f ⇒ 行标签被硬切）；
            // 无标签时保留 8f 的纯几何下限（纯色段不需要字高）。
            float rowH = Mathf.Max(hasLabels ? Measure.RowHeight(Tokens.FontLabel) : 8f,
                r.height / rows.Length - 4f);
            for (int i = 0; i < rows.Length; i++)
            {
                float y = r.y + i * (rowH + 4f);
                if (hasLabels && i < labels.Length)
                    Draw.Label(new Rect(r.x, y, labelW, rowH), labels[i],
                        Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
                MachineState[] seg = rows[i];
                if (seg == null || seg.Length == 0) continue;
                float[] dur = durations != null && i < durations.Length ? durations[i] : null;
                float total = IndustrialFmt.SegmentsTotal(dur);
                float trackX = r.x + labelW, trackW = Mathf.Max(0f, r.width - labelW);
                float x = trackX;
                for (int k = 0; k < seg.Length; k++)
                {
                    float d = dur != null && k < dur.Length ? Mathf.Max(0f, dur[k]) : 0f;
                    float w = Mathf.Max(1f, trackW * (d / total) - 1f);
                    Rect s = new Rect(x, y, w, rowH);
                    Draw.Box(s, StateColor(seg[k]));
                    if (seg[k] == MachineState.Fault)
                        Draw.Border(new Rect(s.x - 1f, s.y - 1f, s.width + 2f, s.height + 2f), Tokens.Text);
                    x += w + 1f;
                }
            }
        }

        /// <summary>设备状态色：运行绿 / 怠机弱线 / 故障红 / 加料黄 / 离线灰（只映射令牌）。</summary>
        public static Color StateColor(MachineState s)
            => s == MachineState.Run ? Tokens.Alive
             : s == MachineState.Idle ? Tokens.BorderSoft
             : s == MachineState.Fault ? Tokens.Dead
             : s == MachineState.Refuel ? Tokens.Warn
             : Tokens.Dim;
    }
}
