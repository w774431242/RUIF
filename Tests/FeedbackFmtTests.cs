using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>反馈原子纯计算单测。</summary>
    [TestFixture]
    public class FeedbackFmtTests
    {
        [Test]
        public void ToastAlpha_FadesNearEnd()
        {
            Assert.AreEqual(1f, FeedbackFmt.ToastAlpha(0f, 4f), 0.001f);
            Assert.AreEqual(1f, FeedbackFmt.ToastAlpha(3f, 4f, 0.5f), 0.001f);
            Assert.AreEqual(0.5f, FeedbackFmt.ToastAlpha(3.75f, 4f, 0.5f), 0.001f);
            Assert.AreEqual(0f, FeedbackFmt.ToastAlpha(4f, 4f), 0.001f);
            Assert.AreEqual(0f, FeedbackFmt.ToastAlpha(9f, 4f), 0.001f);   // 超时 → 不可见
        }

        [Test]
        public void MergeVisible_ClampsAndCounts()
        {
            Assert.AreEqual(4, FeedbackFmt.MergeVisible(4, 4));
            Assert.AreEqual(4, FeedbackFmt.MergeVisible(9, 4));
            Assert.AreEqual(0, FeedbackFmt.MergeVisible(0, 4));
            Assert.AreEqual(5, FeedbackFmt.MergeHidden(9, 4));
            Assert.AreEqual(0, FeedbackFmt.MergeHidden(4, 4));
        }

        [Test]
        public void BlinkAlpha_ByPriority()
        {
            // 低优先级常亮（不闪）
            Assert.AreEqual(1f, FeedbackFmt.BlinkAlpha(0f, AlertPriority.Low), 0.001f);
            Assert.AreEqual(1f, FeedbackFmt.BlinkAlpha(9f, AlertPriority.Low), 0.001f);
            // 紧急快闪 / 常规慢闪：均在 0.35~1 范围内脉动
            float urgentMin = 1f;
            for (float t = 0f; t < 1.6f; t += 0.1f)
            {
                float a = FeedbackFmt.BlinkAlpha(t, AlertPriority.Urgent);
                Assert.GreaterOrEqual(a, 0.35f - 0.01f);
                Assert.LessOrEqual(a, 1f + 0.01f);
                urgentMin = Mathf.Min(urgentMin, a);
            }
            Assert.Less(urgentMin, 0.5f);
        }

        [Test]
        public void StepMark_ThreeStates()
        {
            Assert.AreEqual("✓", FeedbackFmt.StepMark(0, 2));
            Assert.AreEqual("●", FeedbackFmt.StepMark(2, 2));
            Assert.AreEqual("○", FeedbackFmt.StepMark(3, 2));
        }

        [Test]
        public void BatchSummary_JoinsTypes()
        {
            Assert.AreEqual("殖民者×8 · 动物×3", FeedbackFmt.BatchSummary(
                new[] { "殖民者", "动物" }, new[] { 8, 3 }));
            Assert.AreEqual(string.Empty, FeedbackFmt.BatchSummary(null, null));
        }
    }
}
