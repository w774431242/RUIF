using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>布局视觉原子的纯计算单测。</summary>
    [TestFixture]
    public class VisualAtomsTests
    {
        [Test]
        public void LerpT_SpansZeroToOne()
        {
            Assert.AreEqual(0f, VisualsFmt.LerpT(0, 4), 0.001f);
            Assert.AreEqual(0.5f, VisualsFmt.LerpT(1, 3), 0.001f);
            Assert.AreEqual(1f, VisualsFmt.LerpT(3, 4), 0.001f);
            Assert.AreEqual(0f, VisualsFmt.LerpT(0, 1), 0.001f);   // 单段保护
        }

        [Test]
        public void FadeAlpha_IncreasesLeftToRight()
        {
            Assert.AreEqual(0.25f, VisualsFmt.FadeAlpha(0, 4), 0.001f);
            Assert.AreEqual(1f, VisualsFmt.FadeAlpha(3, 4), 0.001f);
            Assert.AreEqual(0f, VisualsFmt.FadeAlpha(0, 0), 0.001f);  // 空段保护
        }

        [Test]
        public void PinRect_AnchorsTopRight()
        {
            var host = new Rect(0f, 0f, 100f, 50f);
            Rect r = VisualsFmt.PinRect(host, 12f, 2f);
            Assert.AreEqual(86f, r.x, 0.001f);      // 100 - 12 - 2
            Assert.AreEqual(2f, r.y, 0.001f);
            Assert.AreEqual(12f, r.width, 0.001f);
        }
    }
}
