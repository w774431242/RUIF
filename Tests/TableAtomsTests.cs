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

        // ── v1.2.0：列样式 / 阈值带色列（向后兼容扩展）────────────────────────────

        /// <summary>① 不传列样式（cols = null）⇒ 全部左对齐（= v1.1.2 旧行为）。</summary>
        [Test]
        public void ColumnAlignOf_NullCols_AllMiddleLeft()
        {
            for (int c = 0; c < 8; c++)
            {
                Assert.AreEqual(TextAnchor.MiddleLeft, TableFmt.ColumnAlignOf(null, c));
            }
        }

        /// <summary>② 越界列（含负下标）回退左对齐。</summary>
        [Test]
        public void ColumnAlignOf_OutOfRange_FallsBackMiddleLeft()
        {
            var cols = new[] { new StatCol { Align = TextAnchor.MiddleRight } };
            Assert.AreEqual(TextAnchor.MiddleLeft, TableFmt.ColumnAlignOf(cols, -1));
            Assert.AreEqual(TextAnchor.MiddleLeft, TableFmt.ColumnAlignOf(cols, 1));
            Assert.AreEqual(TextAnchor.MiddleRight, TableFmt.ColumnAlignOf(cols, 0));
        }

        /// <summary>锚点只保留水平分量：零初始化（UpperLeft）与 UpperRight 都被归一化。</summary>
        [Test]
        public void ColumnAlignOf_NormalizesVerticalOnlyAnchors()
        {
            var cols = new[]
            {
                new StatCol(),                                            // 零初始化 = UpperLeft
                new StatCol { Align = TextAnchor.UpperRight },
                new StatCol { Align = TextAnchor.LowerCenter }
            };
            Assert.AreEqual(TextAnchor.MiddleLeft, TableFmt.ColumnAlignOf(cols, 0));
            Assert.AreEqual(TextAnchor.MiddleRight, TableFmt.ColumnAlignOf(cols, 1));
            Assert.AreEqual(TextAnchor.MiddleCenter, TableFmt.ColumnAlignOf(cols, 2));
        }

        [Test]
        public void IsDimColumn_NullOrOutOfRange_IsFalse()
        {
            var cols = new[] { new StatCol { Dim = true }, new StatCol() };
            Assert.IsFalse(TableFmt.IsDimColumn(null, 0));
            Assert.IsFalse(TableFmt.IsDimColumn(cols, -1));
            Assert.IsFalse(TableFmt.IsDimColumn(cols, 2));
            Assert.IsTrue(TableFmt.IsDimColumn(cols, 0));
            Assert.IsFalse(TableFmt.IsDimColumn(cols, 1));
        }

        /// <summary>③ bandCol &lt; 0 ⇒ 任何列都不走带色格。</summary>
        [Test]
        public void IsBandCol_Negative_IsFalse()
        {
            Assert.IsFalse(TableFmt.IsBandCol(-1, 0));
            Assert.IsFalse(TableFmt.IsBandCol(-1, 2));
            Assert.IsTrue(TableFmt.IsBandCol(2, 2));
            Assert.IsFalse(TableFmt.IsBandCol(2, 1));
        }

        /// <summary>⑦ 带色列文本由调用方透传（千分位不写死在组件里）。</summary>
        [Test]
        public void BandTextOf_PrefersCallerText()
        {
            var values = new[] { 6980f, 2310f };
            var texts = new[] { "6,980", "2,310" };
            Assert.AreEqual("6,980", TableFmt.BandTextOf(values, texts, 0));
            Assert.AreEqual("2,310", TableFmt.BandTextOf(values, texts, 1));
        }

        /// <summary>④ texts 为 null / 该行为空 ⇒ 回退 "0.#"；scale 参与换算。</summary>
        [Test]
        public void BandTextOf_FallsBackToFormat()
        {
            var values = new[] { 12.5f, 0f };
            Assert.AreEqual("12.5", TableFmt.BandTextOf(values, null, 0));
            Assert.AreEqual(string.Empty, TableFmt.BandTextOf(null, null, 0));
            Assert.AreEqual("0", TableFmt.BandTextOf(values, null, 1));
            // 比例值按百分比显示：0.64 ≈ 64%（"0.#"）
            Assert.AreEqual("64", TableFmt.BandTextOf(new[] { 0.64f }, null, 0, 100f));
            // 文本数组存在但该行为空串 ⇒ 回退到值
            Assert.AreEqual("12.5", TableFmt.BandTextOf(values, new[] { string.Empty, "x" }, 0));
        }

        /// <summary>⑤ bandValues / bandTexts 短于 rows / 下标越界 ⇒ 不抛、给空串。</summary>
        [Test]
        public void BandTextOf_ShortArrays_NoOverrun()
        {
            var values = new[] { 1f };
            var texts = new[] { "a" };
            Assert.AreEqual(string.Empty, TableFmt.BandTextOf(values, texts, 5));
            Assert.AreEqual(string.Empty, TableFmt.BandTextOf(null, texts, 1));   // 文本也没有 + 值数组缺失
            Assert.AreEqual(string.Empty, TableFmt.BandTextOf(values, null, -1));
            Assert.AreEqual("a", TableFmt.BandTextOf(null, texts, 0));            // 文本优先，值数组可缺
        }

        /// <summary>⑥ BandColor 在 0.34 / 0.67 边界归入**高档**（&lt; 开区间下界）。</summary>
        [Test]
        public void BandColor_Boundaries_034_067()
        {
            Assert.AreEqual(Tokens.Dead, TableFmt.BandColor(0.339f));
            Assert.AreEqual(Tokens.Warn, TableFmt.BandColor(0.34f));
            Assert.AreEqual(Tokens.Warn, TableFmt.BandColor(0.669f));
            Assert.AreEqual(Tokens.Alive, TableFmt.BandColor(0.67f));
            Assert.AreEqual(Tokens.Alive, TableFmt.BandColor(1f));
        }
    }
}
