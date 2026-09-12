using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>图表纯计算单测（v1.1 Charts 批次）。</summary>
    [TestFixture]
    public class ChartsTests
    {
        // ---- Scale ----

        [Test]
        public void Scale_MapsLinearlyAndClamps()
        {
            Assert.AreEqual(0.5f, ChartsFmt.Scale(50f, 0f, 100f, 0f, 1f), 0.001f);
            Assert.AreEqual(-1f, ChartsFmt.Scale(200f, 0f, 100f, 1f, -1f), 0.001f);   // 超界钳制
            Assert.AreEqual(0f, ChartsFmt.Scale(-5f, 0f, 100f, 0f, 1f), 0.001f);
        }

        [Test]
        public void Scale_DegenerateRange_ReturnsOutLo()
        {
            Assert.AreEqual(7f, ChartsFmt.Scale(42f, 5f, 5f, 7f, 9f), 0.001f);
        }

        // ---- SeriesMax / SeriesRange ----

        [Test]
        public void SeriesMax_NormalEmptyAndNonPositive()
        {
            Assert.AreEqual(469f, ChartsFmt.SeriesMax(new[] { 469f, 377f, 128f }), 0.001f);
            Assert.AreEqual(1f, ChartsFmt.SeriesMax(null), 0.001f);
            Assert.AreEqual(1f, ChartsFmt.SeriesMax(new float[0]), 0.001f);
            Assert.AreEqual(1f, ChartsFmt.SeriesMax(new[] { -3f, -8f }), 0.001f);      // 全非正 → fallback
            Assert.AreEqual(2f, ChartsFmt.SeriesMax(new[] { -3f, -8f }, 2f), 0.001f);  // 自定义 fallback
        }

        [Test]
        public void SeriesRange_NormalAndDegenerate()
        {
            ChartsFmt.SeriesRange(new[] { 3f, 9f, 5f }, 1f, out float lo, out float hi);
            Assert.AreEqual(3f, lo, 0.001f);
            Assert.AreEqual(9f, hi, 0.001f);

            ChartsFmt.SeriesRange(new[] { 4f, 4f, 4f }, 1f, out float lo2, out float hi2);
            Assert.Greater(hi2 - lo2, 1.9f);          // 退化区间被撑开
            Assert.Less(lo2, 4f);
        }

        // ---- NiceTicks ----

        [Test]
        public void NiceTicks_ZeroToHundred_ProducesNiceSteps()
        {
            var buf = new float[8];
            int n = ChartsFmt.NiceTicks(0f, 100f, buf, 4);
            // raw=25 → mag=10, norm=2.5 → step=20 → 0,20,40,60,80,100
            Assert.AreEqual(6, n);
            Assert.AreEqual(0f, buf[0], 0.001f);
            Assert.AreEqual(20f, buf[1], 0.001f);
            Assert.AreEqual(100f, buf[5], 0.001f);
        }

        [Test]
        public void NiceTicks_RespectsBufferCapacityAndDegenerate()
        {
            var small = new float[2];
            Assert.AreEqual(2, ChartsFmt.NiceTicks(0f, 100f, small, 4));
            Assert.AreEqual(0, ChartsFmt.NiceTicks(5f, 5f, new float[8], 4));   // 退化区间
            Assert.AreEqual(0, ChartsFmt.NiceTicks(0f, 100f, null, 4));         // 空缓冲
        }

        [Test]
        public void NiceTicks_NonZeroOrigin_AlignsToStep()
        {
            var buf = new float[8];
            int n = ChartsFmt.NiceTicks(37f, 101f, buf, 4);
            // raw=16 → mag=10, norm=1.6 → step=20 → 首刻度=ceil(37/20)*20=40
            Assert.GreaterOrEqual(n, 1);
            Assert.AreEqual(40f, buf[0], 0.001f);
        }

        // ---- BarGeom ----

        [Test]
        public void BarGeom_DividesWidthByGap()
        {
            ChartsFmt.BarGeom(2, 4, 100f, 10f, out float bw, out float xo);
            Assert.AreEqual(17.5f, bw, 0.001f);   // (100-30)/4
            Assert.AreEqual(55f, xo, 0.001f);     // 2*(17.5+10)
        }

        [Test]
        public void BarGeom_SingleBarAndGuard()
        {
            ChartsFmt.BarGeom(0, 0, 100f, 10f, out float bw, out _);
            Assert.AreEqual(100f, bw, 0.001f);    // n=0 钳为 1
        }

        // ---- SweepDeg / HistogramBin ----

        [Test]
        public void SweepDeg_ProportionalAndGuarded()
        {
            Assert.AreEqual(90f, ChartsFmt.SweepDeg(25f, 100f), 0.001f);
            Assert.AreEqual(360f, ChartsFmt.SweepDeg(100f, 100f), 0.001f);
            Assert.AreEqual(0f, ChartsFmt.SweepDeg(25f, 0f), 0.001f);       // 除零守护
            Assert.AreEqual(0f, ChartsFmt.SweepDeg(-1f, 100f), 0.001f);
        }

        [Test]
        public void HistogramBin_ClampsAndHandlesDegenerate()
        {
            Assert.AreEqual(3, ChartsFmt.HistogramBin(0.95f, 0f, 1f, 4));
            Assert.AreEqual(0, ChartsFmt.HistogramBin(-1f, 0f, 1f, 4));     // 下越界钳 0
            Assert.AreEqual(3, ChartsFmt.HistogramBin(9f, 0f, 1f, 4));      // 上越界钳 3
            Assert.AreEqual(0, ChartsFmt.HistogramBin(5f, 3f, 3f, 4));      // 退化区间
            Assert.AreEqual(0, ChartsFmt.HistogramBin(5f, 0f, 1f, 0));      // bins=0 → 钳 1
        }

        // ---- HeatColor / Tokens.Series ----

        [Test]
        public void HeatColor_InterpolatesBetweenTokens()
        {
            AssertColor(ChartsFmt.HeatColor(0f), Tokens.Card);
            AssertColor(ChartsFmt.HeatColor(1f), Tokens.Alive);
            AssertColor(ChartsFmt.HeatColor(-2f), Tokens.Card);              // 钳制
        }

        /// <summary>颜色分量断言（避免 NUnit 对 Color 结构体做消息格式化触发引擎内部依赖）。</summary>
        private static void AssertColor(Color a, Color b)
        {
            Assert.AreEqual(b.r, a.r, 0.0001f, "r");
            Assert.AreEqual(b.g, a.g, 0.0001f, "g");
            Assert.AreEqual(b.b, a.b, 0.0001f, "b");
            Assert.AreEqual(b.a, a.a, 0.0001f, "a");
        }

        [Test]
        public void ThemeSeries_CyclesTokensWithoutNewColors()
        {
            Assert.AreEqual(Tokens.Accent, Tokens.Series(0));
            Assert.AreEqual(Tokens.Accent, Tokens.Series(6));                 // 环回
            Assert.AreEqual(Tokens.Muted, Tokens.Series(5));
            Assert.AreEqual(Tokens.Muted, Tokens.Series(-1));                 // 负索引环回
        }
    }
}
