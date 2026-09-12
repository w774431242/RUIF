using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>工业管理原子纯计算单测（v1.2 T1 四原子批次）。</summary>
    [TestFixture]
    public class IndustrialTests
    {
        // ---- GaugeNorm ----

        [Test]
        public void GaugeNorm_MapsAndClamps()
        {
            Assert.AreEqual(0.5f, IndustrialFmt.GaugeNorm(50f, 0f, 100f), 0.001f);
            Assert.AreEqual(1f, IndustrialFmt.GaugeNorm(150f, 0f, 100f), 0.001f);   // 超界钳制
            Assert.AreEqual(0f, IndustrialFmt.GaugeNorm(-5f, 0f, 100f), 0.001f);
        }

        [Test]
        public void GaugeNorm_DegenerateRange_ReturnsZero()
        {
            Assert.AreEqual(0f, IndustrialFmt.GaugeNorm(42f, 5f, 5f), 0.001f);      // 不除零
        }

        // ---- GaugeNeedleDeg ----

        [Test]
        public void GaugeNeedleDeg_SweepsUpperHalfCircle()
        {
            Assert.AreEqual(180f, Industrial.GaugeNeedleDeg(0f), 0.001f);           // 左
            Assert.AreEqual(270f, Industrial.GaugeNeedleDeg(0.5f), 0.001f);         // 顶
            Assert.AreEqual(360f, Industrial.GaugeNeedleDeg(1f), 0.001f);           // 右
            Assert.AreEqual(180f, Industrial.GaugeNeedleDeg(-2f), 0.001f);          // 负值钳制
        }

        // ---- GaugeZone ----

        [Test]
        public void GaugeZone_AscendingThresholds_Classify()
        {
            Assert.AreEqual(0, IndustrialFmt.GaugeZone(0.30f, 0.6f, 0.85f));        // 正常
            Assert.AreEqual(1, IndustrialFmt.GaugeZone(0.70f, 0.6f, 0.85f));        // 警告
            Assert.AreEqual(2, IndustrialFmt.GaugeZone(0.90f, 0.6f, 0.85f));        // 危险
            Assert.AreEqual(2, IndustrialFmt.GaugeZone(0.85f, 0.6f, 0.85f));        // 恰在阈值即入区（>= 语义）
            Assert.AreEqual(1, IndustrialFmt.GaugeZone(0.849f, 0.6f, 0.85f));       // 阈值前一刻 → 警告
        }

        [Test]
        public void GaugeZone_InvertedThresholds_NeverDead()
        {
            // 坏配置（dead01 ≤ warn01）：fail-safe 不误报危险
            Assert.AreEqual(0, IndustrialFmt.GaugeZone(0.2f, 0.6f, 0.4f));
            Assert.AreEqual(1, IndustrialFmt.GaugeZone(0.9f, 0.6f, 0.4f));
        }

        // ---- BottleneckIndex ----

        [Test]
        public void BottleneckIndex_ReturnsMaxStage()
        {
            Assert.AreEqual(1, IndustrialFmt.BottleneckIndex(new[] { 1f, 3f, 2f }));
            Assert.AreEqual(2, IndustrialFmt.BottleneckIndex(new[] { 0.5f, 0.5f, 9f }));
        }

        [Test]
        public void BottleneckIndex_EmptyOrNonPositive_ReturnsMinus1()
        {
            Assert.AreEqual(-1, IndustrialFmt.BottleneckIndex(null));
            Assert.AreEqual(-1, IndustrialFmt.BottleneckIndex(new float[0]));
            Assert.AreEqual(-1, IndustrialFmt.BottleneckIndex(new[] { 0f, -1f }));  // 全非正
        }

        // ---- Shares ----

        [Test]
        public void Shares_NormalizesToSum1()
        {
            var buf = new float[4];
            int n = IndustrialFmt.Shares(new[] { 1f, 1f, 2f }, buf);
            Assert.AreEqual(3, n);
            Assert.AreEqual(0.25f, buf[0], 0.001f);
            Assert.AreEqual(0.25f, buf[1], 0.001f);
            Assert.AreEqual(0.50f, buf[2], 0.001f);
        }

        [Test]
        public void Shares_NegativeValues_TreatedAsZero()
        {
            var buf = new float[3];
            IndustrialFmt.Shares(new[] { -3f, 1f, 3f }, buf);
            Assert.AreEqual(0f, buf[0], 0.001f);
            Assert.AreEqual(0.25f, buf[1], 0.001f);
            Assert.AreEqual(0.75f, buf[2], 0.001f);
        }

        [Test]
        public void Shares_AllNonPositive_UniformFallback()
        {
            var buf = new float[4];
            int n = IndustrialFmt.Shares(new[] { -1f, 0f, -2f, 0f }, buf);
            Assert.AreEqual(4, n);
            Assert.AreEqual(0.25f, buf[0], 0.001f);                                 // 均匀兜底，绘制端总有合法占比
            Assert.AreEqual(0.25f, buf[3], 0.001f);
        }

        [Test]
        public void Shares_TruncatesToBufferCapacity()
        {
            var buf = new float[2];
            int n = IndustrialFmt.Shares(new[] { 1f, 1f, 1f, 1f }, buf);
            Assert.AreEqual(2, n);                                                  // 不越界写
        }

        [Test]
        public void Shares_NullOrEmpty_ReturnsZero()
        {
            Assert.AreEqual(0, IndustrialFmt.Shares(null, new float[4]));
            Assert.AreEqual(0, IndustrialFmt.Shares(new float[0], new float[4]));
            Assert.AreEqual(0, IndustrialFmt.Shares(new[] { 1f }, null));           // buf 空不崩
        }

        // ---- MergeRunLengths ----

        [Test]
        public void MergeRunLengths_MergesAdjacentSameState()
        {
            var buf = new int[8];
            int segs = IndustrialFmt.MergeRunLengths(new[] { 1, 1, 2, 2, 2, 1 }, buf);
            Assert.AreEqual(3, segs);
            Assert.AreEqual(2, buf[0]);
            Assert.AreEqual(3, buf[1]);
            Assert.AreEqual(1, buf[2]);
        }

        [Test]
        public void MergeRunLengths_AllSame_SingleSegment()
        {
            var buf = new int[8];
            int segs = IndustrialFmt.MergeRunLengths(new[] { 3, 3, 3, 3 }, buf);
            Assert.AreEqual(1, segs);
            Assert.AreEqual(4, buf[0]);
        }

        [Test]
        public void MergeRunLengths_EmptyOrNull_ReturnsZero()
        {
            Assert.AreEqual(0, IndustrialFmt.MergeRunLengths(null, new int[4]));
            Assert.AreEqual(0, IndustrialFmt.MergeRunLengths(new int[0], new int[4]));
            Assert.AreEqual(0, IndustrialFmt.MergeRunLengths(new[] { 1 }, null));   // buf 空不崩
        }

        [Test]
        public void MergeRunLengths_TruncatesToBufferCapacity()
        {
            var buf = new int[2];
            int segs = IndustrialFmt.MergeRunLengths(new[] { 1, 2, 1, 2, 1 }, buf);
            Assert.AreEqual(2, segs);                                               // 计数截断至容量
            Assert.AreEqual(1, buf[0]);
            Assert.AreEqual(1, buf[1]);
        }

        // ---- LedCell ----

        [Test]
        public void LedCell_GridGeometry()
        {
            var r = new Rect(10f, 20f, 100f, 50f);
            Rect c0 = IndustrialFmt.LedCell(r, 0, 5, 2);
            Assert.AreEqual(10.5f, c0.x, 0.001f);                                   // 0.5f 缝隙偏移
            Assert.AreEqual(20.5f, c0.y, 0.001f);
            Assert.AreEqual(19f, c0.width, 0.001f);                                 // (100/5)-1
            Assert.AreEqual(24f, c0.height, 0.001f);                                // (50/2)-1
            Rect c7 = IndustrialFmt.LedCell(r, 7, 5, 2);
            Assert.AreEqual(2, (int)((c7.x - 10f) / 20f));                          // 第 7 格在 5 列网格的第 2 列（7%5）
            Assert.AreEqual(1, (int)((c7.y - 20f) / 25f));                          // 第 2 行
        }

        [Test]
        public void LedCell_DegenerateGrid_ClampsToSingleCell()
        {
            var r = new Rect(0f, 0f, 10f, 10f);
            Rect c = IndustrialFmt.LedCell(r, 0, 0, 0);                             // 0 行列 → 钳 1×1
            Assert.AreEqual(9f, c.width, 0.001f);                            // 10/1 - 1px 缝
            Assert.AreEqual(9f, c.height, 0.001f);
        }

        // ---- SegmentsTotal ----

        [Test]
        public void SegmentsTotal_SumsPositive()
        {
            Assert.AreEqual(6f, IndustrialFmt.SegmentsTotal(new[] { 1f, 2f, 3f }), 0.001f);
            Assert.AreEqual(3f, IndustrialFmt.SegmentsTotal(new[] { 1f, -2f, 2f }), 0.001f);   // 负值不计
        }

        [Test]
        public void SegmentsTotal_NullOrAllNonPositive_Returns1()
        {
            Assert.AreEqual(1f, IndustrialFmt.SegmentsTotal(null), 0.001f);         // 守护：分母恒正
            Assert.AreEqual(1f, IndustrialFmt.SegmentsTotal(new[] { 0f, -1f }), 0.001f);
        }

        // ---- 色表（令牌一致性）----

        [Test]
        public void ZoneColor_MapsTokensOnly()
        {
            Assert.AreEqual(Tokens.Alive, Industrial.ZoneColor(0));
            Assert.AreEqual(Tokens.Warn, Industrial.ZoneColor(1));
            Assert.AreEqual(Tokens.Dead, Industrial.ZoneColor(2));
            Assert.AreEqual(Tokens.Alive, Industrial.ZoneColor(-1));                 // 越界归正常
        }

        [Test]
        public void StateColor_And_LedColor_MapsTokensOnly()
        {
            Assert.AreEqual(Tokens.Alive, Industrial.StateColor(MachineState.Run));
            Assert.AreEqual(Tokens.Dead, Industrial.StateColor(MachineState.Fault));
            Assert.AreEqual(Tokens.Warn, Industrial.StateColor(MachineState.Refuel));
            Assert.AreEqual(Tokens.Alive, Industrial.LedColor(1));
            Assert.AreEqual(Tokens.Warn, Industrial.LedColor(2));
            Assert.AreEqual(Tokens.Dead, Industrial.LedColor(5));                    // ≥4 归故障
        }
    }
}
