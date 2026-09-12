using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>面板原子纯计算单测。</summary>
    [TestFixture]
    public class PanelAtomsTests
    {
        [Test]
        public void FlashAlpha_OscillatesInRange()
        {
            for (float t = 0f; t < 6f; t += 0.25f)
            {
                float a = PanelFmt.FlashAlpha(t);
                Assert.GreaterOrEqual(a, 0.45f - 0.001f);
                Assert.LessOrEqual(a, 1f + 0.001f);
            }
        }

        [Test]
        public void RowSplit_DividesByRatioAndGap()
        {
            PanelFmt.RowSplit(new Rect(0, 0, 100, 20), 0.5f, 10f, out Rect label, out Rect value);
            Assert.AreEqual(0f, label.x, 0.001f);
            Assert.AreEqual(45f, label.width, 0.001f);      // (100-10)*0.5
            Assert.AreEqual(55f, value.x, 0.001f);          // 45 + gap 10
            Assert.AreEqual(45f, value.width, 0.001f);
        }

        [Test]
        public void RowSplit_ZeroRatio_AllToValue()
        {
            PanelFmt.RowSplit(new Rect(0, 0, 80, 20), 0f, 5f, out Rect label, out Rect value);
            Assert.AreEqual(0f, label.width, 0.001f);
            Assert.AreEqual(75f, value.width, 0.001f);
        }
    }

    /// <summary>进度/容量原子纯计算单测。</summary>
    [TestFixture]
    public class ProgressAtomsTests
    {
        [Test]
        public void CapacityRatio_DividesAndGuardsZero()
        {
            Assert.AreEqual(0.66f, ProgressFmt.CapacityRatio(660f, 1000f), 0.001f);
            Assert.AreEqual(1.1f, ProgressFmt.CapacityRatio(1100f, 1000f), 0.001f);
            Assert.AreEqual(0f, ProgressFmt.CapacityRatio(100f, 0f), 0.001f);
        }

        [Test]
        public void OverflowRatio_OnlyWhenOverCapacity()
        {
            Assert.AreEqual(0f, ProgressFmt.OverflowRatio(900f, 1000f), 0.001f);
            Assert.AreEqual(0.1f, ProgressFmt.OverflowRatio(1100f, 1000f), 0.001f);
        }

        [Test]
        public void ZoomLabel_FormatsPercent()
        {
            Assert.AreEqual("86%", ProgressFmt.ZoomLabel(0.86f));
            Assert.AreEqual("100%", ProgressFmt.ZoomLabel(1f));
            Assert.AreEqual("40%", ProgressFmt.ZoomLabel(0.4f));
        }

        [Test]
        public void WipeWidth_SlantsTopAndBottom()
        {
            // 10 行、宽 100、slant 0.4：中间行偏移≈0，首行负偏移、末行正偏移
            float first = ProgressFmt.WipeWidth(0.5f, 100f, 0, 10, 0.4f);
            float last = ProgressFmt.WipeWidth(0.5f, 100f, 9, 10, 0.4f);
            Assert.Less(first, 50f);
            Assert.Greater(last, 50f);
            Assert.AreEqual(30f, first, 0.001f);   // 50 - 0.5*0.4*100
            Assert.AreEqual(70f, last, 0.001f);    // 50 + 0.5*0.4*100
        }

        [Test]
        public void WipeWidth_ClampsToBounds()
        {
            // p≤0 → 全行 0（不残留三角）；p≥1 → 全行铺满
            Assert.AreEqual(0f, ProgressFmt.WipeWidth(-1f, 100f, 5, 10, 0.4f), 0.001f);
            Assert.AreEqual(0f, ProgressFmt.WipeWidth(0f, 100f, 9, 10, 0.4f), 0.001f);
            Assert.AreEqual(100f, ProgressFmt.WipeWidth(5f, 100f, 9, 10, 0.4f), 0.001f);
            Assert.AreEqual(100f, ProgressFmt.WipeWidth(1f, 100f, 0, 10, 0.4f), 0.001f);
        }
    }
}
