using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>输入扩展控件纯计算单测。</summary>
    [TestFixture]
    public class InputExAtomsTests
    {
        [Test]
        public void NextPriority_CyclesZeroToFour()
        {
            Assert.AreEqual(1, InputExFmt.NextPriority(0));
            Assert.AreEqual(2, InputExFmt.NextPriority(1));
            Assert.AreEqual(4, InputExFmt.NextPriority(3));
            Assert.AreEqual(0, InputExFmt.NextPriority(4));   // 回到空
        }

        [Test]
        public void NextTimetableState_CyclesThree()
        {
            Assert.AreEqual(1, InputExFmt.NextTimetableState(0));
            Assert.AreEqual(2, InputExFmt.NextTimetableState(1));
            Assert.AreEqual(0, InputExFmt.NextTimetableState(2));
        }

        [Test]
        public void NextIndex_Wraps()
        {
            Assert.AreEqual(1, InputExFmt.NextIndex(0, 3));
            Assert.AreEqual(0, InputExFmt.NextIndex(2, 3));
            Assert.AreEqual(0, InputExFmt.NextIndex(0, 0));   // 空保护
        }

        [Test]
        public void ScrubValue_AppliesStepAndClamps()
        {
            Assert.AreEqual(30f, InputExFmt.ScrubValue(10f, 20f, 1f, 0f, 100f));
            Assert.AreEqual(100f, InputExFmt.ScrubValue(10f, 999f, 1f, 0f, 100f));
            Assert.AreEqual(0f, InputExFmt.ScrubValue(10f, -999f, 1f, 0f, 100f));
        }

        [Test]
        public void DropIndex_MapsPointerToRow()
        {
            var list = new Rect(0, 0, 100, 90);
            Assert.AreEqual(0, InputExFmt.DropIndex(list, 3, 30f, 10f));
            Assert.AreEqual(2, InputExFmt.DropIndex(list, 3, 30f, 70f));
            Assert.AreEqual(-1, InputExFmt.DropIndex(list, 3, 30f, 200f));
        }

        [Test]
        public void Reorder_MovesElement()
        {
            var items = new[] { "a", "b", "c", "d" };
            InputExFmt.Reorder(items, 0, 2);
            Assert.AreEqual("b,c,a,d", string.Join(",", items));
            InputExFmt.Reorder(items, 2, 0);
            Assert.AreEqual("a,b,c,d", string.Join(",", items));
        }

        [Test]
        public void Hsv_RoundTripsPrimaryColors()
        {
            Color red = InputExFmt.HsvColor(0f, 1f, 1f);
            Assert.AreEqual(1f, red.r, 0.01f);
            Assert.AreEqual(0f, red.g, 0.01f);
            Assert.AreEqual(0f, red.b, 0.01f);

            Color gray = InputExFmt.HsvColor(0.5f, 0f, 0.5f);
            Assert.AreEqual(0.5f, gray.r, 0.01f);
            Assert.AreEqual(gray.r, gray.g, 0.01f);

            Color black = InputExFmt.HsvColor(0.3f, 1f, 0f);
            Assert.AreEqual(0f, black.r, 0.001f);
        }

        [Test]
        public void RgbToHsv_ReadsRedAndGray()
        {
            InputExFmt.RgbToHsv(new Color(1f, 0f, 0f), out float h, out float s, out float v);
            Assert.AreEqual(0f, h, 0.02f);
            Assert.AreEqual(1f, s, 0.02f);
            Assert.AreEqual(1f, v, 0.02f);

            InputExFmt.RgbToHsv(new Color(0.4f, 0.4f, 0.4f), out h, out s, out v);
            Assert.AreEqual(0f, s, 0.02f);
            Assert.AreEqual(0.4f, v, 0.02f);
        }

        [Test]
        public void Hex_FormatsSixDigits()
        {
            Assert.AreEqual("FF0000", InputExFmt.Hex(new Color(1f, 0f, 0f)));
            Assert.AreEqual("000000", InputExFmt.Hex(Color.black));
            Assert.AreEqual("FFFFFF", InputExFmt.Hex(Color.white));
        }
    }
}
