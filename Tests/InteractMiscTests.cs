using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    [TestFixture]
    public class InteractAtomsTests
    {
        [Test]
        public void EyeRect_CentersInHost()
        {
            var host = new Rect(0, 0, 40, 20);
            Rect r = InteractFmt.EyeRect(host, 10f);
            Assert.AreEqual(15f, r.x, 0.001f);
            Assert.AreEqual(5f, r.y, 0.001f);
            Assert.AreEqual(10f, r.width, 0.001f);
        }

        [Test]
        public void HighlightSegmentCount_SplitsAroundHits()
        {
            // 规则：普通段/命中段交替，<b>不产生空段</b>
            Assert.AreEqual(1, InteractFmt.HighlightSegmentCount("abc", ""));            // 空查询 → 1 段
            Assert.AreEqual(1, InteractFmt.HighlightSegmentCount("建造", "建造"));        // 整串命中 → 1 段
            Assert.AreEqual(2, InteractFmt.HighlightSegmentCount("建造高级", "建造"));    // 命中 + 剩余
            Assert.AreEqual(3, InteractFmt.HighlightSegmentCount("XabcX", "X"));          // 命中 + 中间 + 命中
            Assert.AreEqual(5, InteractFmt.HighlightSegmentCount("aXbXc", "X"));          // 普通·命中·普通·命中·普通
        }

        [Test]
        public void GripDotSize_DividesByRowsCols()
        {
            InteractFmt.GripDotSize(new Rect(0, 0, 14, 22), 3, 2, 2f, out float w, out float h);
            Assert.AreEqual(6f, w, 0.001f);      // (14-2)/2
            Assert.AreEqual(6f, h, 0.001f);      // (22-4)/3
        }
    }

    [TestFixture]
    public class MiscAtomsTests
    {
        [Test]
        public void Points_RespectsBudget()
        {
            Assert.AreEqual(6, MiscFmt.PointsRemaining(10, 4));
            Assert.IsFalse(MiscFmt.PointsOverdrawn(10, 4));
            Assert.AreEqual(-2, MiscFmt.PointsRemaining(10, 12));
            Assert.IsTrue(MiscFmt.PointsOverdrawn(10, 12));
        }

        [Test]
        public void BubbleTail_AnchorsBottomCenter()
        {
            var b = new Rect(0, 0, 100, 30);
            Rect t = MiscFmt.BubbleTail(b, 8f);
            Assert.AreEqual(46f, t.x, 0.001f);
            Assert.AreEqual(29f, t.y, 0.001f);
        }

        [Test]
        public void ScrollThumbHeight_FullWhenNoOverflow()
        {
            Assert.AreEqual(200f, MiscFmt.ScrollThumbHeight(200f, 200f, 150f), 0.001f);   // 内容少 → 占满
            Assert.AreEqual(100f, MiscFmt.ScrollThumbHeight(200f, 200f, 400f), 0.001f);   // 1/2 视口比
            Assert.GreaterOrEqual(MiscFmt.ScrollThumbHeight(200f, 20f, 2000f), 16f);      // 最小 16
        }

        [Test]
        public void ScrollThumbY_TravelsTrack()
        {
            Assert.AreEqual(0f, MiscFmt.ScrollThumbY(0f, 200f, 50f, 0f), 0.001f);
            Assert.AreEqual(150f, MiscFmt.ScrollThumbY(0f, 200f, 50f, 1f), 0.001f);
        }
    }
}
