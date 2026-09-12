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
    /// 进度与容量原子（批4-2a）：斜向擦除 / 容量条（含超载虚线）/ 缩放指示。
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
