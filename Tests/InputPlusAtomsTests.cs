using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>B2+ 输入控件纯计算单测。</summary>
    [TestFixture]
    public class InputPlusAtomsTests
    {
        [Test]
        public void QuickAmount_RespectsMax()
        {
            Assert.AreEqual(1f, InputPlusFmt.QuickAmount(0f, QuickAmountKind.One, 250f), 0.001f);
            Assert.AreEqual(10f, InputPlusFmt.QuickAmount(0f, QuickAmountKind.Ten, 250f), 0.001f);
            Assert.AreEqual(100f, InputPlusFmt.QuickAmount(0f, QuickAmountKind.Hundred, 250f), 0.001f);
            Assert.AreEqual(250f, InputPlusFmt.QuickAmount(0f, QuickAmountKind.Max, 250f), 0.001f);
            Assert.AreEqual(5f, InputPlusFmt.QuickAmount(0f, QuickAmountKind.Ten, 5f), 0.001f);   // 超上限 → 取 max
        }

        [Test]
        public void SnapToTicks_Nearest()
        {
            var ticks = new[] { 0f, 25f, 50f };
            Assert.AreEqual(25f, InputPlusFmt.SnapToTicks(26f, ticks), 0.001f);
            Assert.AreEqual(50f, InputPlusFmt.SnapToTicks(40f, ticks), 0.001f);
            Assert.AreEqual(7f, InputPlusFmt.SnapToTicks(7f, null), 0.001f);   // 无刻度 → 原值
        }

        [Test]
        public void WheelStep_AppliesStepAndClamps()
        {
            Assert.AreEqual(5f, InputPlusFmt.WheelStep(10f, 1f, 5f, 0f, 100f), 0.001f);     // delta>0 → 减
            Assert.AreEqual(15f, InputPlusFmt.WheelStep(10f, -1f, 5f, 0f, 100f), 0.001f);    // delta<0 → 加
            Assert.AreEqual(100f, InputPlusFmt.WheelStep(99f, -1f, 5f, 0f, 100f), 0.001f);   // 上钳
            Assert.AreEqual(0f, InputPlusFmt.WheelStep(1f, 1f, 5f, 0f, 100f), 0.001f);       // 下钳
        }

        [Test]
        public void MultiSelect_ToggleAndRange()
        {
            var sel = new bool[4];
            InputPlusFmt.ToggleAt(sel, 2);
            Assert.IsTrue(sel[2]);
            InputPlusFmt.SelectRange(sel, 0, 2);
            Assert.IsTrue(sel[0] && sel[1] && sel[2]);
            Assert.IsFalse(sel[3]);
        }

        [Test]
        public void Queue_EnqueueRemoveMoveFront()
        {
            var q = new int[4];
            int count = 0;
            InputPlusFmt.EnqueueInt(q, ref count, 1);
            InputPlusFmt.EnqueueInt(q, ref count, 2);
            InputPlusFmt.EnqueueInt(q, ref count, 3);
            Assert.AreEqual(3, count);
            InputPlusFmt.MoveToFrontInt(q, count, 2);
            Assert.AreEqual("3,1,2", string.Join(",", new[] { q[0], q[1], q[2] }));
            InputPlusFmt.RemoveAtInt(q, ref count, 1);
            Assert.AreEqual(2, count);
            Assert.AreEqual("3,2", string.Join(",", new[] { q[0], q[1] }));
        }

        [Test]
        public void TryParseClamp_ParsesAndClamps()
        {
            float v;
            Assert.IsTrue(InputPlusFmt.TryParseClamp("42", 0f, 250f, out v));
            Assert.AreEqual(42f, v, 0.001f);
            Assert.IsTrue(InputPlusFmt.TryParseClamp("999", 0f, 250f, out v));
            Assert.AreEqual(250f, v, 0.001f);      // 钳制
            Assert.IsFalse(InputPlusFmt.TryParseClamp("abc", 0f, 250f, out v));
            Assert.IsFalse(InputPlusFmt.TryParseClamp("", 0f, 250f, out v));
        }

        [Test]
        public void Path_Validates()
        {
            Assert.IsTrue(InputPlusFmt.IsValidPath("C:/RimWorld/导出"));
            Assert.IsFalse(InputPlusFmt.IsValidPath("C:/a<b"));
            Assert.IsFalse(InputPlusFmt.IsValidPath(""));
            Assert.AreEqual("C:/a/b", InputPlusFmt.NormalizePath("C:\\a\\b"));
        }

        [Test]
        public void Conflict_DetectsDuplicates()
        {
            Assert.IsTrue(InputPlusFmt.HasConflict(new[] { "F11", "F11", "F1" }));
            Assert.IsFalse(InputPlusFmt.HasConflict(new[] { "F11", "F1" }));
            Assert.AreEqual(1, InputPlusFmt.ConflictCount(new[] { "F11", "F11", "F1" }));
            Assert.AreEqual(0, InputPlusFmt.ConflictCount(null));
        }

        [Test]
        public void SplitLines_HandlesNullAndLimit()
        {
            Assert.AreEqual(1, InputPlusFmt.SplitLines(null, 4).Length);
            Assert.AreEqual(2, InputPlusFmt.SplitLines("a\nb", 4).Length);
            Assert.AreEqual(2, InputPlusFmt.SplitLines("a\nb\nc", 2).Length);   // 超限裁剪
        }
    }
}
