using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>基础输入控件纯计算单测。</summary>
    [TestFixture]
    public class InputAtomsTests
    {
        [Test]
        public void Resolve_DisabledWins()
        {
            Assert.AreEqual(InputState.Disabled, InputFmt.Resolve(true, true, true, false));
            Assert.AreEqual(InputState.Pressed, InputFmt.Resolve(false, true, false, true));
            Assert.AreEqual(InputState.Selected, InputFmt.Resolve(false, false, true, true));
            Assert.AreEqual(InputState.Hover, InputFmt.Resolve(true, false, false, true));
            Assert.AreEqual(InputState.Default, InputFmt.Resolve(false, false, false, true));
        }

        [Test]
        public void StepValue_ClampsAndSteps()
        {
            Assert.AreEqual(6, InputFmt.StepValue(5, 1, 0, 10));
            Assert.AreEqual(10, InputFmt.StepValue(11, 1, 0, 10));   // 上钳
            Assert.AreEqual(0, InputFmt.StepValue(-5, 1, 0, 10));    // 下钳
            Assert.AreEqual(45, InputFmt.StepValue(50, -5, 0, 100));
        }

        [Test]
        public void Slider01_AndValue_RoundTrip()
        {
            Assert.AreEqual(0.5f, InputFmt.Slider01(50f, 0f, 100f), 0.001f);
            Assert.AreEqual(0f, InputFmt.Slider01(50f, 50f, 50f), 0.001f);   // 退化区间保护
            Assert.AreEqual(50f, InputFmt.SliderValue(0.5f, 0f, 100f, -1f), 0.001f);
            Assert.AreEqual(2f, InputFmt.SliderValue(0.5f, 0f, 5f, 1f), 0.001f);   // roundTo=1 → 2 或 3（取整）
        }

        [Test]
        public void SliderValue_RoundsToStep()
        {
            float v = InputFmt.SliderValue(0.5f, 0f, 100f, 10f);
            Assert.AreEqual(50f, v, 0.001f);
            float v2 = InputFmt.SliderValue(0.51f, 0f, 100f, 10f);
            Assert.AreEqual(50f, v2, 0.001f);   // 51 → 取整到 50
        }

        [Test]
        public void SegmentIndex_HitTests()
        {
            var r = new Rect(0, 0, 120, 20);
            Assert.AreEqual(0, InputFmt.SegmentIndex(r, 3, 10f));
            Assert.AreEqual(1, InputFmt.SegmentIndex(r, 3, 50f));
            Assert.AreEqual(2, InputFmt.SegmentIndex(r, 3, 110f));
            Assert.AreEqual(-1, InputFmt.SegmentIndex(r, 3, 200f));   // 越界
            Assert.AreEqual(-1, InputFmt.SegmentIndex(r, 0, 10f));    // 空段保护
        }

        [Test]
        public void SegmentRect_DividesEvenly()
        {
            var r = new Rect(0, 0, 120, 20);
            Rect s1 = InputFmt.SegmentRect(r, 3, 1);
            Assert.AreEqual(40f, s1.x, 0.001f);
            Assert.AreEqual(40f, s1.width, 0.001f);
        }

        [Test]
        public void DebounceReady_AfterDelay()
        {
            Assert.IsFalse(InputFmt.DebounceReady(1.0f, 0.9f, 0.2f));
            Assert.IsTrue(InputFmt.DebounceReady(1.2f, 0.9f, 0.2f));
        }
    }
}
