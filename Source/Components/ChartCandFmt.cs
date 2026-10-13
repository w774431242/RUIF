using UnityEngine;

namespace RUIF
{
    /// <summary>⑧ 图表候选组件的纯计算（可离线单测）。</summary>
    public static class ChartCandFmt
    {
        private static readonly Color[] Palette =
            { Tokens.Accent, Tokens.Alive, Tokens.Warn, Tokens.Info, Tokens.Dead, Tokens.Muted };

        /// <summary>序列配色（循环调色板）。</summary>
        public static Color SeriesColor(int i) => Palette[((i % Palette.Length) + Palette.Length) % Palette.Length];

        /// <summary>阈值着色（复用 TableFmt 三档）。</summary>
        public static Color BandColor(float ratio01) => TableFmt.BandColor(ratio01);

        /// <summary>数组最小值（空返回 0）。</summary>
        public static float Min(float[] v)
        {
            if (v == null || v.Length == 0) return 0f;
            float m = v[0];
            for (int i = 1; i < v.Length; i++) if (v[i] < m) m = v[i];
            return m;
        }

        /// <summary>数组最大值（空返回 1）。</summary>
        public static float Max(float[] v)
        {
            if (v == null || v.Length == 0) return 1f;
            float m = v[0];
            for (int i = 1; i < v.Length; i++) if (v[i] > m) m = v[i];
            return m;
        }

        /// <summary>累计百分比（输入按降序，输出写入 buffer，0~1）。</summary>
        public static void Cumulative(float[] vals, float[] buf)
        {
            int n = Mathf.Min(vals?.Length ?? 0, buf?.Length ?? 0);
            if (n == 0) return;
            float sum = 0f;
            for (int i = 0; i < n; i++) sum += vals[i];
            float acc = 0f;
            for (int i = 0; i < n; i++)
            {
                acc += vals[i];
                buf[i] = sum <= 0f ? 0f : acc / sum;
            }
        }
    }
}
