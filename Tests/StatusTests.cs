using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>状态原子纯计算单测（v1.1 补齐：此前 StatusAtoms 为唯一无测试组件组）。</summary>
    [TestFixture]
    public class StatusFmtTests
    {
        // ---- PulseAlpha ----

        [Test]
        public void PulseAlpha_AtT0_IsMidpoint()
        {
            // t=0：sin0=0 → pulse=0.5 → alpha = min + (1-min)*0.5 = (1+min)/2
            Assert.AreEqual(0.725f, StatusFmt.PulseAlpha(0f, 0.45f), 0.001f);
        }

        [Test]
        public void PulseAlpha_StaysWithinMinToOne()
        {
            for (float t = 0f; t < 6f; t += 0.13f)
            {
                float a = StatusFmt.PulseAlpha(t, 0.45f);
                Assert.GreaterOrEqual(a, 0.45f - 0.001f);
                Assert.LessOrEqual(a, 1f + 0.001f);
            }
        }

        [Test]
        public void PulseAlpha_ZeroFloor_ReachesFullRange()
        {
            Assert.LessOrEqual(StatusFmt.PulseAlpha(0.393f, 0f), 1.001f);   // 峰值≈1
            Assert.GreaterOrEqual(StatusFmt.PulseAlpha(1.18f, 0f), -0.001f); // 谷值≈0
        }

        // ---- DotRect ----

        [Test]
        public void DotRect_LandscapeHost_CentersSquare()
        {
            Rect dot = StatusFmt.DotRect(new Rect(10f, 10f, 30f, 20f));
            Assert.AreEqual(20f, dot.width, 0.001f);
            Assert.AreEqual(20f, dot.height, 0.001f);
            Assert.AreEqual(25f, dot.center.x, 0.001f);
            Assert.AreEqual(20f, dot.center.y, 0.001f);
        }

        [Test]
        public void DotRect_TallHost_TakesShortSide()
        {
            Rect dot = StatusFmt.DotRect(new Rect(0f, 0f, 8f, 24f));
            Assert.AreEqual(8f, dot.width, 0.001f);
            Assert.AreEqual(8f, dot.height, 0.001f);
            Assert.AreEqual(0f, dot.x, 0.001f);
            Assert.AreEqual(8f, dot.y, 0.001f);
        }

        // ---- TrailAlpha ----

        [Test]
        public void TrailAlpha_HeadSegment_IsBrightest()
        {
            Assert.AreEqual(1f, StatusFmt.TrailAlpha(3, 3, 8), 0.001f);
        }

        [Test]
        public void TrailAlpha_WrapsAroundRing()
        {
            // head=0, i=1, n=8 → behind=7 → 1-7/8=0.125 → 钳到 0.15
            Assert.AreEqual(0.15f, StatusFmt.TrailAlpha(1, 0, 8), 0.001f);
            // head=0, i=7 → behind=1 → 1-1/8=0.875
            Assert.AreEqual(0.875f, StatusFmt.TrailAlpha(7, 0, 8), 0.001f);
        }

        [Test]
        public void TrailAlpha_NegativeIndex_HandledByRingMath()
        {
            // i=-1, head=0, n=8 → behind=((0-(-1))%8+8)%8=1 → 0.875
            Assert.AreEqual(0.875f, StatusFmt.TrailAlpha(-1, 0, 8), 0.001f);
        }

        [Test]
        public void TrailAlpha_OneSegment_AlwaysFull()
        {
            Assert.AreEqual(1f, StatusFmt.TrailAlpha(0, 0, 1), 0.001f);
        }

        // ---- GlyphOf ----

        [Test]
        public void GlyphOf_MapsKindToGlyphAndToken()
        {
            StatusFmt.GlyphOf(StatusKind.Ok, out string ok, out Color cOk);
            Assert.AreEqual("✓", ok);
            Assert.AreEqual(Tokens.Alive, cOk);

            StatusFmt.GlyphOf(StatusKind.Info, out string info, out Color cInfo);
            Assert.AreEqual("•", info);
            Assert.AreEqual(Tokens.Info, cInfo);

            StatusFmt.GlyphOf(StatusKind.Warn, out string warn, out Color cWarn);
            Assert.AreEqual("!", warn);
            Assert.AreEqual(Tokens.Warn, cWarn);

            StatusFmt.GlyphOf(StatusKind.Error, out string err, out Color cErr);
            Assert.AreEqual("✕", err);
            Assert.AreEqual(Tokens.Dead, cErr);
        }

        [Test]
        public void GlyphOf_UnknownKind_FallsBackToPaused()
        {
            StatusFmt.GlyphOf((StatusKind)99, out string glyph, out Color color);
            Assert.AreEqual("‖", glyph);
            Assert.AreEqual(Tokens.Muted, color);
        }
    }
}
