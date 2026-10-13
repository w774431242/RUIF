using NUnit.Framework;

namespace RUIF.Tests
{
    /// <summary>
    /// `ProgressAtoms.ProgressRing`（v1.1 首件 · 零贴图版）的**纯计算回归**。
    ///
    /// <para>绘制面不可离线跑（GUI），但"段数 / 已填充段数"是本件唯一的量化口径
    /// （也是跨端一致性与"环形进度 vs 线性进度"对齐的依据），故抽成
    /// <see cref="ProgressFmt.RingSegments"/> / <see cref="ProgressFmt.RingFilledSegments"/> 单测锁定。</para>
    /// </summary>
    public class ProgressRingTests
    {
        [Test]
        public void RingSegments_IsClamped_AndThreeDegreesPerSegment()
        {
            Assert.AreEqual(120, ProgressFmt.RingSegments(360f), "整圈 = 360 / 3 = 120 段");
            Assert.AreEqual(30, ProgressFmt.RingSegments(90f), "开口环 = 90 / 3 = 30 段");
            Assert.AreEqual(12, ProgressFmt.RingSegments(1f), "极小扫掠角被下限夹到 12 段（不断环）");
            Assert.AreEqual(120, ProgressFmt.RingSegments(9999f), "扫掠角上限 360° ⇒ 仍 120 段（不因越界而变密）");
            Assert.AreEqual(180, ProgressFmt.RingSegments(360f, 2f), "每段 2° 时整圈 = 180 段（上限可达）");
            Assert.AreEqual(12, ProgressFmt.RingSegments(360f, 90f), "每段 90° 时被下限夹到 12 段");
        }

        [Test]
        public void RingFilledSegments_CoversEmptyHalfAndFull()
        {
            Assert.AreEqual(0, ProgressFmt.RingFilledSegments(120, 0f), "0% = 空环（一段都不填）");
            Assert.AreEqual(120, ProgressFmt.RingFilledSegments(120, 1f), "100% = 满环");
            Assert.AreEqual(60, ProgressFmt.RingFilledSegments(120, 0.5f), "50% = 一半");
            Assert.AreEqual(0, ProgressFmt.RingFilledSegments(120, -3f), "越界按钳制处理");
            Assert.AreEqual(120, ProgressFmt.RingFilledSegments(120, 7f), "越界按钳制处理");
        }

        [Test]
        public void RingFilledSegments_IsMonotonic()
        {
            int last = -1;
            for (int i = 0; i <= 20; i++)
            {
                int cur = ProgressFmt.RingFilledSegments(120, i / 20f);
                Assert.GreaterOrEqual(cur, last, "占比增大时填充段数不得回退");
                last = cur;
            }
        }
    }
}
