using NUnit.Framework;
using UnityEngine;

namespace RUIF.Tests
{
    /// <summary>
    /// v1.3.0 <see cref="WindowFmt"/>（方案 §3.4 冻结件 / §5.2 常量表）的<b>纯几何 + 常量</b>离线单测。
    ///
    /// <para><b>可离线</b>：本文件只做 <c>Rect</c> 与 <c>Mathf</c>（均为托管实现）运算，零 GUI、零 ECall、
    /// 零引擎状态 ⇒ 不触碰 <c>Verse</c> 静态构造。<b>⛔ 不测</b> <see cref="WindowFrame.Draw"/>（绘制入口，
    /// 调 <c>RUIF.Draw.*</c>，属 NOT RUN 面）。</para>
    ///
    /// <para><b>本文件钉住的三条设计要点</b>（本批 D0/D4 换档与底栏槽模型）：
    /// ① 冻结式 <c>FooterButtonWidth(W,n) = W/n − 10f</c> 且<b>主按钮（末槽）右缘贴带右缘</b>；
    /// ② <c>NeedsStack</c> 阈值 = <c>W/n − 10f &lt; 80f</c>（D0 ↔ D4 分界，恰等于 80 不换档）；
    /// ③ <c>n == 1</c> 走 120×32 <b>居中</b>特例（回落 <c>Dialog_Confirm.ButtonSize</c>）。</para>
    /// </summary>
    [TestFixture]
    public class WindowFmtTests
    {
        private const float Eps = 1e-4f;

        // 与内容区宽 400 对齐的底栏带（贴底 35f）
        private static Rect MakeFooter(float w = 400f) => new Rect(0f, 265f, w, 35f);

        private static void AssertRect(Rect e, Rect a, string msg)
        {
            Assert.AreEqual(e.x, a.x, Eps, msg + " .x");
            Assert.AreEqual(e.y, a.y, Eps, msg + " .y");
            Assert.AreEqual(e.width, a.width, Eps, msg + " .width");
            Assert.AreEqual(e.height, a.height, Eps, msg + " .height");
        }

        // ------------------------------------------------------------ 常量表（§5.2 冻结值）

        [Test]
        public void Constants_MatchFrozenTable()
        {
            Assert.AreEqual(42f, WindowFmt.TitleHeight, Eps, "TitleHeight = Dialog_MessageBox.TitleHeight");
            Assert.AreEqual(18f, WindowFmt.ContentInset, Eps, "ContentInset = Verse.Window.StandardMargin");
            Assert.AreEqual(35f, WindowFmt.FooterHeight, Eps, "FooterHeight = Dialog_MessageBox 底带高");
            Assert.AreEqual(35f, WindowFmt.FooterButtonHeight, Eps, "FooterButtonHeight == FooterHeight");
            Assert.AreEqual(10f, WindowFmt.FooterInsetX, Eps, "FooterInsetX = Dialog_MessageBox");
            Assert.AreEqual(80f, WindowFmt.MinButtonWidth, Eps, "MinButtonWidth 触发 D4 的阈值");
            Assert.AreEqual(120f, WindowFmt.SingleButtonWidth, Eps, "n==1 特例宽 = Dialog_Confirm.ButtonSize");
            Assert.AreEqual(32f, WindowFmt.SingleButtonHeight, Eps, "n==1 特例高 = Dialog_Confirm.ButtonSize");
            Assert.AreEqual(2f, WindowFmt.FocusRingOutset, Eps, "焦点环外扩");
            Assert.AreEqual(8f, WindowFmt.ButtonPadX, Eps, "按钮内容内边距 = Tokens.SpaceXs");
            Assert.AreEqual(Tokens.RuleHeight, WindowFmt.RuleHeight, Eps, "RuleHeight 必须 = Tokens.RuleHeight（不另立数字）");
        }

        // ------------------------------------------------------------ 三块矩形

        [Test]
        public void TitleRect_SnapsToTopWithFullWidthAndTitleHeight()
        {
            AssertRect(new Rect(10f, 20f, 400f, 42f), WindowFmt.TitleRect(new Rect(10f, 20f, 400f, 300f)), "TitleRect");
        }

        [Test]
        public void FooterRect_SnapsToBottomWithFooterHeight()
        {
            AssertRect(new Rect(10f, 285f, 400f, 35f), WindowFmt.FooterRect(new Rect(10f, 20f, 400f, 300f)), "FooterRect");
        }

        [Test]
        public void ContentRect_IsBetweenTitleAndFooterWithInsets()
        {
            // x = 10+18 ; top = 20+42+1 ; bottom = 320−35−1 ; w = 400−36 ; h = bottom−top
            AssertRect(new Rect(28f, 63f, 364f, 221f), WindowFmt.ContentRect(new Rect(10f, 20f, 400f, 300f)), "ContentRect");
        }

        [Test]
        public void ContentRect_TooShort_ClampsHeightToZero_NoOverflow()
        {
            // 高 50 < TitleHeight+2*RuleHeight+FooterHeight ⇒ 必夹到 0（不外溢、不抛）
            Rect c = WindowFmt.ContentRect(new Rect(0f, 0f, 100f, 50f));
            Assert.AreEqual(0f, c.height, Eps, "高度不足必须夹到 0，绝不外溢");
            Assert.AreEqual(64f, c.width, Eps, "宽度 = 100 − 2*18");
        }

        [Test]
        public void ContentRect_TooNarrow_ClampsWidthToZero()
        {
            Rect c = WindowFmt.ContentRect(new Rect(0f, 0f, 20f, 300f));
            Assert.AreEqual(0f, c.width, Eps, "宽 20 < 2*ContentInset ⇒ 夹到 0");
        }

        // ------------------------------------------------------------ 底栏槽模型

        [Test]
        public void FooterSlotX_EquallyDividesAndClampsIndex()
        {
            Rect f = MakeFooter();
            Assert.AreEqual(0f, WindowFmt.FooterSlotX(f, 5, 0), Eps, "i=0");
            Assert.AreEqual(160f, WindowFmt.FooterSlotX(f, 5, 2), Eps, "i=2");
            Assert.AreEqual(320f, WindowFmt.FooterSlotX(f, 5, 4), Eps, "i=n-1");
            Assert.AreEqual(0f, WindowFmt.FooterSlotX(f, 5, -1), Eps, "i<0 夹到 0");
            Assert.AreEqual(320f, WindowFmt.FooterSlotX(f, 5, 9), Eps, "i>n-1 夹到 n-1");
        }

        [Test]
        public void FooterSlotX_NonPositiveN_FallsBackToBandLeft()
        {
            Rect f = MakeFooter();
            Assert.AreEqual(f.x, WindowFmt.FooterSlotX(f, 0, 3), Eps, "n=0 回落带左缘（不除零）");
            Assert.AreEqual(f.x, WindowFmt.FooterSlotX(f, -2, 1), Eps, "n<0 同上");
        }

        [Test]
        public void FooterButtonWidth_IsWidthOverNMinusInset()
        {
            Assert.AreEqual(70f, WindowFmt.FooterButtonWidth(400f, 5), Eps, "400/5 − 10");
            Assert.AreEqual(90f, WindowFmt.FooterButtonWidth(400f, 4), Eps, "400/4 − 10");
            Assert.AreEqual(390f, WindowFmt.FooterButtonWidth(400f, 1), Eps, "400/1 − 10");
            Assert.AreEqual(0f, WindowFmt.FooterButtonWidth(400f, 0), Eps, "n<=0 ⇒ 0（不除零）");
            Assert.AreEqual(0f, WindowFmt.FooterButtonWidth(400f, -3), Eps, "n<0 ⇒ 0");
        }

        [Test]
        public void FooterButtonRect_PrimaryButtonRightEdgeFlushWithBandRight()
        {
            Rect f = MakeFooter();
            // 末槽（主按钮）= n-1：xMax 必须逐值贴带右缘（§5.1 裁决 ①）
            Rect primary = WindowFmt.FooterButtonRect(f, 5, 4);
            Assert.AreEqual(f.xMax, primary.xMax, Eps, "主按钮右缘贴带右缘");
            Assert.AreEqual(330f, primary.x, Eps, "x = 槽起点 + 10f");
            Assert.AreEqual(70f, primary.width, Eps, "w = W/n − 10f");
            Assert.AreEqual(f.y, primary.y, Eps, "y = 带顶");
            Assert.AreEqual(35f, primary.height, Eps, "h = 35f");
        }

        [Test]
        public void FooterButtonRect_SlotModel_HoldsForAllButtons()
        {
            // 冻结式 + 右对齐 + 不重叠/间隙 = FooterInsetX（10f），逐 n 逐槽核对
            float[] widths = { 200f, 399f, 400f, 501f };
            int[] counts = { 2, 3, 5, 8 };
            foreach (float w in widths)
            {
                foreach (int n in counts)
                {
                    if (w / n - 10f <= 0f) continue;              // 退化槽不参与（Mathf.Max(0,·) 已覆盖）
                    Rect f = MakeFooter(w);
                    for (int i = 0; i < n; i++)
                    {
                        Rect r = WindowFmt.FooterButtonRect(f, n, i);
                        string tag = "W=" + w + " n=" + n + " i=" + i;
                        Assert.AreEqual(w / n - 10f, r.width, Eps, tag + " w 冻结式");
                        Assert.AreEqual(WindowFmt.FooterSlotX(f, n, i) + 10f, r.x, Eps, tag + " x = 槽 + inset");
                        Assert.AreEqual(35f, r.height, Eps, tag + " h");
                        if (i + 1 < n)
                        {
                            Rect nxt = WindowFmt.FooterButtonRect(f, n, i + 1);
                            Assert.AreEqual(WindowFmt.FooterInsetX, nxt.x - r.xMax, Eps,
                                tag + " 相邻间隙 = inset（不重叠）");
                        }
                    }
                    Assert.AreEqual(f.xMax, WindowFmt.FooterButtonRect(f, n, n - 1).xMax, Eps,
                        "W=" + w + " n=" + n + " 末槽右缘贴带右缘");
                }
            }
        }

        [Test]
        public void SingleButtonRect_Is120x32CenteredInBand()
        {
            Rect f = MakeFooter();
            Rect s = WindowFmt.SingleButtonRect(f);
            Assert.AreEqual(120f, s.width, Eps, "宽 120");
            Assert.AreEqual(32f, s.height, Eps, "高 32");
            Assert.AreEqual(f.center.x, s.center.x, Eps, "水平居中");
            Assert.AreEqual(f.center.y, s.center.y, Eps, "垂直居中");
            Assert.IsTrue(s.y >= f.y && s.yMax <= f.yMax, "不得越出底栏带");
        }

        // ------------------------------------------------------------ D0 ↔ D4 换档阈值

        [Test]
        public void NeedsStack_ThresholdIsWidthOverNMinusTenBelow80()
        {
            Assert.IsTrue(WindowFmt.NeedsStack(400f, 5), "70 < 80 ⇒ 换档 D4");
            Assert.IsFalse(WindowFmt.NeedsStack(500f, 5), "90 >= 80 ⇒ 留 D0");
            Assert.IsFalse(WindowFmt.NeedsStack(450f, 5), "恰 = 80 ⇒ 边界不换档（< 而非 <=）");
            Assert.IsTrue(WindowFmt.NeedsStack(449f, 5), "79.8 < 80 ⇒ 换档");
        }

        [Test]
        public void NeedsStack_NonPositiveN_IsStacked()
        {
            // FooterButtonWidth(n<=0) = 0 < 80 ⇒ true（调用方按 n>=1 处理，此处钉住纯函数行为）
            Assert.IsTrue(WindowFmt.NeedsStack(400f, 0), "n=0 ⇒ 0 < 80");
        }

        [Test]
        public void StackedHeight_IsNrowsOfFooterHeight()
        {
            Assert.AreEqual(105f, WindowFmt.StackedHeight(3), Eps, "3 × 35");
            Assert.AreEqual(0f, WindowFmt.StackedHeight(0), Eps, "n=0 ⇒ 0");
            Assert.AreEqual(0f, WindowFmt.StackedHeight(-2), Eps, "n<0 ⇒ 0");
        }

        [Test]
        public void StackedButtonRect_FullWidthRowsTopDownWithClampedIndex()
        {
            Rect stack = new Rect(5f, 10f, 400f, 105f);
            AssertRect(new Rect(5f, 10f, 400f, 35f), WindowFmt.StackedButtonRect(stack, 3, 0), "i=0");
            AssertRect(new Rect(5f, 45f, 400f, 35f), WindowFmt.StackedButtonRect(stack, 3, 1), "i=1");
            AssertRect(new Rect(5f, 80f, 400f, 35f), WindowFmt.StackedButtonRect(stack, 3, 2), "i=n-1");
            Assert.AreEqual(WindowFmt.StackedButtonRect(stack, 3, 2), WindowFmt.StackedButtonRect(stack, 3, 9),
                "i>n-1 夹到 n-1");
            Assert.AreEqual(WindowFmt.StackedButtonRect(stack, 3, 0), WindowFmt.StackedButtonRect(stack, 3, -4),
                "i<0 夹到 0");
        }

        [Test]
        public void StackedButtonRect_LastRowBottomMatchesStackedHeight()
        {
            Rect stack = new Rect(0f, 0f, 400f, 105f);
            Assert.AreEqual(stack.y + WindowFmt.StackedHeight(3), WindowFmt.StackedButtonRect(stack, 3, 2).yMax, Eps,
                "堆叠块末行底 == StackedHeight 覆盖范围（自洽）");
        }
    }
}
