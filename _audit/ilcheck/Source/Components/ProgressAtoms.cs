using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>进度/容量原子的纯计算（可离线单测）。</summary>
    public static class ProgressFmt
    {
        /// <summary>已用 / 容量（容量非正返回 0）。</summary>
        public static float CapacityRatio(float used, float capacity)
            => capacity <= 0f ? 0f : used / capacity;

        /// <summary>超出容量的比例（未超载为 0）。</summary>
        public static float OverflowRatio(float used, float capacity)
            => Mathf.Max(0f, CapacityRatio(used, capacity) - 1f);

        /// <summary>缩放值 → 百分比文本（钳制 0~1000%）。</summary>
        public static string ZoomLabel(float zoom)
            => (Mathf.Clamp(zoom, 0f, 10f) * 100f).ToString("0") + "%";

        /// <summary>
        /// 环形进度的**段数**：按每段 <paramref name="perSegDeg"/> 度（默认 3°，夹在 1–30）自适应，
        /// 结果夹在 **12–180** 段（小尺寸不断环、大尺寸不过密）。纯函数 ⇒ 可离线单测。
        ///
        /// <para>可达范围说明：默认 3°/段时整圈 = 120 段；**180 段上限**只在更细的
        /// <paramref name="perSegDeg"/>（如 2°）时触及 —— 供高分辨率场景调用。</para>
        /// </summary>
        public static int RingSegments(float sweepDeg, float perSegDeg = 3f)
        {
            float per = Mathf.Clamp(perSegDeg, 1f, 30f);
            return Mathf.Clamp(Mathf.CeilToInt(Mathf.Clamp(sweepDeg, 30f, 360f) / per), 12, 180);
        }

        /// <summary>环形进度的**已填充段数**（四舍五入；0 表示空环、= 段数表示满环）。纯函数。</summary>
        public static int RingFilledSegments(int segments, float progress01)
            => Mathf.RoundToInt(Mathf.Max(1, segments) * Mathf.Clamp01(progress01));

        /// <summary>
        /// 斜向擦除：第 row 行（共 rows 行）的填充宽度。
        /// 中间行为基准，上下行按 slant 偏移，形成斜切边缘（零贴图、零裁剪矩阵）。
        /// </summary>
        public static float WipeWidth(float progress01, float width, int row, int rows, float slant = 0.35f)
        {
            float p = Mathf.Clamp01(progress01);
            float t = rows <= 1 ? 0f : row / (float)(rows - 1);
            // 倾斜量在 p=0 / p=1 处归零：保证 0% 什么都不画、100% 每行铺满（否则会残留三角）
            float edge = 1f - Mathf.Abs(2f * p - 1f);
            float offset = (t - 0.5f) * slant * width * edge;
            return Mathf.Clamp(p * width + offset, 0f, width);
        }
    }

    /// <summary>
    /// 进度与容量原子（批4-2a；v1.1 增补**弧形进度环**）：斜向擦除 / 容量条（含超载虚线）/
    /// 缩放指示 / 弧形进度环（ProgressRing）。
    /// 无状态；进度与缩放值由调用方传入。
    /// </summary>
    public static class ProgressAtoms
    {
        /// <summary>斜向进度擦除（分段行绘制，零贴图）。</summary>
        public static void FillWipe(Rect rect, float progress01, Color? color = null,
            int rows = 10, float slant = 0.35f)
        {
            Draw.Box(rect, Tokens.BorderSoft);
            int n = Mathf.Max(2, rows);
            float h = rect.height / n;
            for (int j = 0; j < n; j++)
            {
                float w = ProgressFmt.WipeWidth(progress01, rect.width, j, n, slant);
                if (w <= 0f) continue;
                Draw.Box(new Rect(rect.x, rect.y + j * h, w, h + 1f), color ?? Tokens.Accent);
            }
        }

        /// <summary>容量条：占用填充 + 接近满载变黄 + 超载时右端红色虚线段。</summary>
        public static void CapacityBar(Rect rect, float used, float capacity, Color? fill = null)
        {
            float r = ProgressFmt.CapacityRatio(used, capacity);
            Draw.Box(rect, Tokens.BorderSoft);
            float w = rect.width * Mathf.Min(1f, r);
            Draw.Box(new Rect(rect.x, rect.y, w, rect.height), fill ?? (r > 0.85f ? Tokens.Warn : Tokens.Alive));

            if (ProgressFmt.OverflowRatio(used, capacity) > 0f)
            {
                float segW = Mathf.Max(2f, rect.width / 26f);
                for (int i = 0; i < 4; i++)
                    Draw.Box(new Rect(rect.xMax - (i + 1) * segW * 1.7f, rect.y, segW, rect.height), Tokens.Dead);
            }
        }

        /// <summary>
        /// **弧形进度环**（v1.1 首件 · **零贴图版**）：整圈轨道 + 按 <paramref name="progress01"/>
        /// **顺时针**填充的进度弧，中心可放读数。12 点钟方向起始（与 <see cref="Charts.DonutChart"/> 同口径）。
        ///
        /// <para><b>几何</b>：以 3° 一格的短弧段拼圆（与 DonutChart / Spinner 同一"零贴图"先例）；
        /// 段数与填充段数由 <see cref="ProgressFmt.RingSegments"/> / <see cref="ProgressFmt.RingFilledSegments"/>
        /// 纯函数给出 ⇒ 跨端口径一致、可离线单测。
        /// <b>不触碰 <c>GUI.matrix</c></b>（弧段用 cos/sin 直接定位、保持轴对齐）⇒ 与调用点所在作用域无关，
        /// 不受"外层平移/裁剪"影响。</para>
        ///
        /// <para><b>与 <see cref="Industrial.Gauge"/> 的分工</b>：Gauge 是**半环指针表盘**
        /// （分区色 + 指针，表达"指标健康度"，如完好率/达成率）；本件是**整圈进度环**
        /// （单值 0–1，表达"完成度 / 占用率 / 负载"，如产能利用率、库存水位）。
        /// 若要平滑弧（贴图/折线逼近）仍属 v1.1 素材件，本件是**非贴图原型版**（先可用后美化）。</para>
        /// </summary>
        /// <param name="progress01">进度 0–1（钳制）。</param>
        /// <param name="track">空槽色；null ⇒ <see cref="Tokens.BorderSoft"/>。</param>
        /// <param name="fill">进度色；null ⇒ <see cref="Tokens.Accent"/>。</param>
        /// <param name="centerLabel">中心文本（可选，走 <see cref="Tokens.FontValue"/>）。</param>
        /// <param name="sweepDeg">扫掠角（默认整圈 360°；可用 270° 做"开口环"）。</param>
        /// <param name="thickness01">环厚 / 外半径 比例（默认 0.22，夹在 0.08–0.6）。</param>
        public static void ProgressRing(Rect rect, float progress01, Color? track = null, Color? fill = null,
            string centerLabel = null, float sweepDeg = 360f, float thickness01 = 0.22f)
        {
            float outer = Mathf.Min(rect.width, rect.height) / 2f - 1f;
            if (outer < 6f) return;
            float thick = Mathf.Max(3f, outer * Mathf.Clamp(thickness01, 0.08f, 0.6f));
            float ringMid = outer - thick / 2f;
            Vector2 c = rect.center;
            int segs = ProgressFmt.RingSegments(sweepDeg);
            float step = Mathf.Clamp(sweepDeg, 30f, 360f) / segs;
            float arcLen = Mathf.Max(2f, ringMid * step * Mathf.Deg2Rad + 1f);
            int filled = ProgressFmt.RingFilledSegments(segs, progress01);
            Color trackCol = track ?? Tokens.BorderSoft;
            Color fillCol = fill ?? Tokens.Accent;

            for (int i = 0; i < segs; i++)
            {
                float ang = -90f + (i + 0.5f) * step;   // 12 点钟起始、顺时针
                float rad = ang * Mathf.Deg2Rad;
                Vector2 p = new Vector2(c.x + Mathf.Cos(rad) * ringMid, c.y + Mathf.Sin(rad) * ringMid);
                Draw.Box(new Rect(p.x - thick / 2f, p.y - arcLen / 2f, thick, arcLen),
                    i < filled ? fillCol : trackCol);
            }
            if (!string.IsNullOrEmpty(centerLabel))
                Draw.Label(new Rect(c.x - outer + thick, c.y - 11f, (outer - thick) * 2f, 22f),
                    centerLabel, Tokens.FontValue, Tokens.Text, TextAnchor.MiddleCenter);
        }

        /// <summary>缩放指示 + 复位按钮；返回是否点击了「适配」。</summary>
        public static bool ZoomIndicator(Rect rect, float zoom, float resetValue = 1f)
        {
            var row = Layout.Row(rect, Tokens.SpaceXs);
            Draw.Label(row.Take(34f), Tr.Get("RUIF.Comp.Zoom.Label"), Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.Label(row.Take(50f), ProgressFmt.ZoomLabel(zoom), Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            return Widgets.ButtonText(row.Take(64f), Tr.Get("RUIF.Comp.Zoom.Fit"), true, false,
                Mathf.Abs(zoom - resetValue) > 0.001f);
        }
    }
}
