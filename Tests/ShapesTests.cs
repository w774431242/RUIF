using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>几何/状态原子的纯计算单测。</summary>
    [TestFixture]
    public class ShapesTests
    {
        [Test]
        public void GradientAlpha_DecreasesFromOne()
        {
            Assert.AreEqual(1f, ShapesFmt.GradientAlpha(0, 4), 0.001f);
            Assert.AreEqual(0.25f, ShapesFmt.GradientAlpha(3, 4), 0.001f);
            Assert.AreEqual(0f, ShapesFmt.GradientAlpha(4, 4), 0.001f);   // 越界→0
            Assert.AreEqual(0f, ShapesFmt.GradientAlpha(0, 0), 0.001f);   // 空→0
        }

        [Test]
        public void CaretAngle_MapsProgressToQuarterTurn()
        {
            Assert.AreEqual(0f, ShapesFmt.CaretAngle(0f), 0.001f);
            Assert.AreEqual(45f, ShapesFmt.CaretAngle(0.5f), 0.001f);
            Assert.AreEqual(90f, ShapesFmt.CaretAngle(1f), 0.001f);
            Assert.AreEqual(90f, ShapesFmt.CaretAngle(3f), 0.001f);       // 越界钳制
        }

        [Test]
        public void SpinnerHead_AdvancesAndWraps()
        {
            Assert.AreEqual(0, ShapesFmt.SpinnerHead(8, 0f));
            Assert.AreEqual(1, ShapesFmt.SpinnerHead(8, 0.25f));
            Assert.AreEqual(0, ShapesFmt.SpinnerHead(8, 2f));             // 8 步回到 0
            Assert.AreEqual(0, ShapesFmt.SpinnerHead(0, 1f));             // 空段保护
        }

        [Test]
        public void RuleSplit_DividesSymmetrically()
        {
            ShapesFmt.RuleSplit(100f, 20f, 5f, out float leftW, out float labelX, out float rightW);
            // avail = 100-20-10 = 70 → 左 35；labelX = 40；右 = 100-40-20 = 40
            Assert.AreEqual(35f, leftW, 0.001f);
            Assert.AreEqual(40f, labelX, 0.001f);
            Assert.AreEqual(40f, rightW, 0.001f);
        }

        [Test]
        public void RuleSplit_LabelWiderThanTotal_ClampsToZero()
        {
            ShapesFmt.RuleSplit(10f, 50f, 5f, out float leftW, out float labelX, out float rightW);
            Assert.AreEqual(0f, leftW, 0.001f);
            Assert.AreEqual(0f, rightW, 0.001f);
        }
    }
}
