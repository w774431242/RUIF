using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>文本格式化纯函数单测（无引擎依赖）。</summary>
    [TestFixture]
    public class TextFmtTests
    {
        [TestCase(999, "999")]
        [TestCase(0, "0")]
        [TestCase(-350, "-350")]
        [TestCase(12480, "12.5k")]
        [TestCase(284000, "284k")]
        [TestCase(2500000, "2.5M")]
        [TestCase(25000000, "25M")]
        [TestCase(1500000000, "1.5B")]
        public void FormatNumber_ScalesByMagnitude(double value, string expected)
        {
            Assert.AreEqual(expected, TextFmt.FormatNumber(value));
        }

        [TestCase("虫群自地下涌来", 0f, 0)]
        [TestCase("虫群自地下涌来", 0.5f, 3)]     // 7 字 × 0.5 = 3.5 → floor 3
        [TestCase("虫群自地下涌来", 1f, 7)]
        [TestCase("", 1f, 0)]
        public void TypewriterChars_ProgressesByRatio(string text, float p, int expected)
        {
            Assert.AreEqual(expected, TextFmt.TypewriterChars(text, p));
        }

        [Test]
        public void TypewriterChars_ClampsOutOfRange()
        {
            Assert.AreEqual(0, TextFmt.TypewriterChars("abc", -1f));
            Assert.AreEqual(3, TextFmt.TypewriterChars("abc", 5f));
        }
    }
}
