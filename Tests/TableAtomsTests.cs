using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>表格原子纯计算单测。</summary>
    [TestFixture]
    public class TableAtomsTests
    {
        [Test]
        public void NextSort_CyclesNoneAscDesc()
        {
            Assert.AreEqual(SortState.Asc, TableFmt.NextSort(SortState.None));
            Assert.AreEqual(SortState.Desc, TableFmt.NextSort(SortState.Asc));
            Assert.AreEqual(SortState.None, TableFmt.NextSort(SortState.Desc));
        }

        [Test]
        public void RowHeight_MapsThreeDensities()
        {
            Assert.AreEqual(22f, TableFmt.RowHeight(RowDensity.Compact), 0.001f);
            Assert.AreEqual(28f, TableFmt.RowHeight(RowDensity.Normal), 0.001f);
            Assert.AreEqual(34f, TableFmt.RowHeight(RowDensity.Cozy), 0.001f);
        }

        [Test]
        public void ValueRatio_DividesAndClamps()
        {
            Assert.AreEqual(0.5f, TableFmt.ValueRatio(250f, 500f), 0.001f);
            Assert.AreEqual(1f, TableFmt.ValueRatio(999f, 500f), 0.001f);     // 超出 → 钳 1
            Assert.AreEqual(0f, TableFmt.ValueRatio(100f, 0f), 0.001f);       // 参考值非正 → 0
        }

        [Test]
        public void BandColor_ThreeBands()
        {
            Assert.AreEqual(Tokens.Dead, TableFmt.BandColor(0.2f));
            Assert.AreEqual(Tokens.Warn, TableFmt.BandColor(0.5f));
            Assert.AreEqual(Tokens.Alive, TableFmt.BandColor(0.9f));
        }

        [Test]
        public void ResizeWidth_ClampsToRange()
        {
            Assert.AreEqual(80f, TableFmt.ResizeWidth(60f, 20f, 40f, 200f), 0.001f);
            Assert.AreEqual(40f, TableFmt.ResizeWidth(60f, -100f, 40f, 200f), 0.001f);  // 下钳
            Assert.AreEqual(200f, TableFmt.ResizeWidth(60f, 999f, 40f, 200f), 0.001f);  // 上钳
        }

        [Test]
        public void ColumnSplit_LastColumnEatsRemainder()
        {
            var shares = new[] { 0.5f, 0.3f, 0.2f };
            var widths = new float[3];
            TableFmt.ColumnSplit(new Rect(0, 0, 100, 20), shares, 0f, widths);
            Assert.AreEqual(50f, widths[0], 0.001f);
            Assert.AreEqual(30f, widths[1], 0.001f);
            Assert.AreEqual(20f, widths[2], 0.001f);
            Assert.AreEqual(100f, widths[0] + widths[1] + widths[2], 0.001f);
        }

        [Test]
        public void ColumnSplit_WithGap_SubtractsGaps()
        {
            var shares = new[] { 0.5f, 0.5f };
            var widths = new float[2];
            TableFmt.ColumnSplit(new Rect(0, 0, 100, 20), shares, 6f, widths);
            Assert.AreEqual(50f, widths[0], 0.001f);
            Assert.AreEqual(44f, widths[1], 0.001f);   // 100 - 50 - gap 6
        }

        [Test]
        public void FrozenContentOffsetX_NegatesScroll()
        {
            Assert.AreEqual(0f, TableFmt.FrozenContentOffsetX(0f), 0.001f);
            Assert.AreEqual(-30f, TableFmt.FrozenContentOffsetX(30f), 0.001f);
        }
    }
}
