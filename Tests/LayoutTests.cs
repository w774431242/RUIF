using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>布局数学离线单测（纯几何，不触碰引擎绘制）。</summary>
    [TestFixture]
    public class LayoutTests
    {
        private static Rect R(float x, float y, float w, float h) => new Rect(x, y, w, h);

        [Test]
        public void Column_Take_AdvancesWithGap()
        {
            var col = Layout.Column(R(0, 0, 100, 100), gap: 5f);
            Rect a = col.Take(10f);
            Rect b = col.Take(20f);
            Assert.AreEqual(0f, a.y, 0.001f);
            Assert.AreEqual(10f, a.height, 0.001f);
            Assert.AreEqual(15f, b.y, 0.001f);           // 10 + gap 5
            Assert.AreEqual(20f, b.height, 0.001f);
        }

        [Test]
        public void Column_Take_ClampsAtBottom()
        {
            var col = Layout.Column(R(0, 0, 50, 25), gap: 0f);
            col.Take(20f);
            Rect rest = col.Rest;
            Assert.AreEqual(20f, rest.y, 0.001f);
            Assert.AreEqual(5f, rest.height, 0.001f);
            Rect over = col.Take(999f);                   // 超出容器 → 裁剪不溢出
            Assert.AreEqual(5f, over.height, 0.001f);
        }

        [Test]
        public void Row_Take_AdvancesWithGap()
        {
            var row = Layout.Row(R(0, 0, 100, 20), gap: 4f);
            Rect a = row.Take(30f);
            Rect b = row.Take(20f);
            Assert.AreEqual(0f, a.x, 0.001f);
            Assert.AreEqual(34f, b.x, 0.001f);            // 30 + gap 4
            Assert.AreEqual(20f, b.width, 0.001f);
        }

        [Test]
        public void Grid_Next_LaysOutColumns()
        {
            var grid = Layout.Grid(R(0, 0, 100, 80), cols: 3, cellHeight: 20f, gap: 5f);
            // 列宽 = (100 - 5*2)/3 = 30
            Rect c0 = grid.Next();   // index 0 → 首行首列
            Rect c1 = grid.Next();   // index 1 → 首行次列
            grid.Next();             // index 2 → 首行末列
            Rect c3 = grid.Next();   // index 3 → 换行到第二行首列
            Assert.AreEqual(0f, c0.x, 0.001f);
            Assert.AreEqual(30f, c0.width, 0.001f);
            Assert.AreEqual(35f, c1.x, 0.001f);   // 0 + 30 + gap 5
            Assert.AreEqual(0f, c3.x, 0.001f);    // 换行回到首列
            Assert.AreEqual(25f, c3.y, 0.001f);   // 20 + gap 5
        }

        [Test]
        public void Split_RespectsRatioAndGap()
        {
            Layout.Split(R(0, 0, 200, 50), 0.32f, 10f, out Rect left, out Rect right);
            Assert.AreEqual(60.8f, left.width, 0.001f);   // (200 - gap 10) * 0.32
            Assert.AreEqual(70.8f, right.x, 0.001f);      // 60.8 + gap 10
            Assert.AreEqual(129.2f, right.width, 0.001f); // 200 - 60.8 - gap 10（三段合计=200）
        }

        [Test]
        public void Inset_ShrinksEvenly()
        {
            Rect r = Layout.Inset(R(0, 0, 100, 50), 10f, 5f);
            Assert.AreEqual(10f, r.x, 0.001f);
            Assert.AreEqual(5f, r.y, 0.001f);
            Assert.AreEqual(80f, r.width, 0.001f);
            Assert.AreEqual(40f, r.height, 0.001f);
        }

        [Test]
        public void AspectBox_KeepsRatioAndCenters()
        {
            Rect r = Atoms.AspectBox(R(0, 0, 100, 50), 2f);   // 2:1，容器偏方 → 按宽算高=50 → 正好
            Assert.AreEqual(100f, r.width, 0.001f);
            Assert.AreEqual(50f, r.height, 0.001f);

            Rect r2 = Atoms.AspectBox(R(0, 0, 100, 100), 2f); // 高受限 → 按高算宽=200>100 → 取宽 100 高 50 居中
            Assert.AreEqual(50f, r2.height, 0.001f);
            Assert.AreEqual(25f, r2.y, 0.001f);
        }

        [Test]
        public void Anchor_TopRightAndBottomLeft()
        {
            Rect parent = R(0, 0, 100, 100);
            Rect tr = Atoms.Anchor(parent, Anchor9.TopRight, new Vector2(20, 20));
            Assert.AreEqual(80f, tr.x, 0.001f);
            Assert.AreEqual(0f, tr.y, 0.001f);
            Rect bl = Atoms.Anchor(parent, Anchor9.BottomLeft, new Vector2(20, 20));
            Assert.AreEqual(0f, bl.x, 0.001f);
            Assert.AreEqual(80f, bl.y, 0.001f);
        }
    }
}
