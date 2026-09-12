using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>导航原子纯计算单测（NavFmt）。</summary>
    [TestFixture]
    public class NavFmtTests
    {
        [Test]
        public void LabelUnits_CjkCountsDouble()
        {
            Assert.AreEqual(0, NavFmt.LabelUnits(null));
            Assert.AreEqual(2, NavFmt.LabelUnits("AB"));
            Assert.AreEqual(4, NavFmt.LabelUnits("殖民"));
            Assert.AreEqual(5, NavFmt.LabelUnits("殖民x"));
        }

        [Test]
        public void TabWidth_UnitsTimesWidthPlusPad()
        {
            Assert.AreEqual(2f * 6f + 4f, NavFmt.TabWidth("AB", 6f, 4f), 0.001f);
            Assert.AreEqual(4f * 6f + 4f, NavFmt.TabWidth("殖民", 6f, 4f), 0.001f);
        }

        [Test]
        public void TotalPages_And_ClampPage()
        {
            Assert.AreEqual(9, NavFmt.TotalPages(267, 30));
            Assert.AreEqual(1, NavFmt.TotalPages(0, 30));
            Assert.AreEqual(1, NavFmt.TotalPages(50, 0));
            Assert.AreEqual(0, NavFmt.ClampPage(-5, 9));
            Assert.AreEqual(8, NavFmt.ClampPage(99, 9));
            Assert.AreEqual(0, NavFmt.ClampPage(0, 0));
        }

        [Test]
        public void PageList_FitsAllWhenSmall()
        {
            var buf = new int[11];
            NavFmt.PageList(3, 1, buf, out int count);
            Assert.AreEqual(3, count);
            Assert.AreEqual(0, buf[0]);
            Assert.AreEqual(1, buf[1]);
            Assert.AreEqual(2, buf[2]);
        }

        [Test]
        public void PageList_FoldsWithEllipsis()
        {
            var buf = new int[7];       // 缓冲小于总页数 → 触发省略号折叠
            NavFmt.PageList(10, 0, buf, out int count);
            Assert.AreEqual(0, buf[0]);
            Assert.AreEqual(1, buf[1]);
            Assert.AreEqual(-1, buf[2]);
            Assert.AreEqual(9, buf[3]);

            NavFmt.PageList(10, 4, buf, out count);
            Assert.AreEqual(7, count);
            Assert.AreEqual(0, buf[0]);
            Assert.AreEqual(-1, buf[1]);
            Assert.AreEqual(3, buf[2]);
            Assert.AreEqual(4, buf[3]);
            Assert.AreEqual(5, buf[4]);
            Assert.AreEqual(-1, buf[5]);
            Assert.AreEqual(9, buf[6]);

            NavFmt.PageList(10, 9, buf, out count);
            Assert.AreEqual(0, buf[0]);
            Assert.AreEqual(-1, buf[1]);
            Assert.AreEqual(8, buf[2]);
            Assert.AreEqual(9, buf[3]);

            // 缓冲足够大 → 全量展开
            var big = new int[11];
            NavFmt.PageList(10, 0, big, out count);
            Assert.AreEqual(10, count);
        }

        [Test]
        public void FuzzyScore_MatchPrefixAndConsecutive()
        {
            Assert.AreEqual(0, NavFmt.FuzzyScore(string.Empty, "任意"));
            Assert.AreEqual(-1, NavFmt.FuzzyScore("zz", "晋升：张三"));
            Assert.GreaterOrEqual(NavFmt.FuzzyScore("晋升", "晋升：张三 → 正高级"), 0);
            // 前缀命中优于分散命中
            Assert.Greater(NavFmt.FuzzyScore("a", "abc"), NavFmt.FuzzyScore("a", "xay"));
            // 连续命中优于跳跃命中
            Assert.Greater(NavFmt.FuzzyScore("ab", "ab"), NavFmt.FuzzyScore("ab", "acb"));
        }

        [Test]
        public void Filter_OrdersByScoreDescending()
        {
            var src = new[]
            {
                new Command("打开画廊", null),
                new Command("晋升：张三", null),
                new Command("晋升：李四", null),
            };
            var dst = new Command[8];
            var scores = new int[8];
            int n = CommandPalette.Filter("晋升", src, dst, scores);
            Assert.AreEqual(2, n);
            Assert.AreEqual("晋升：张三", dst[0].Label);
            Assert.GreaterOrEqual(scores[0], scores[1]);

            n = CommandPalette.Filter(string.Empty, src, dst, scores);
            Assert.AreEqual(3, n);
            n = CommandPalette.Filter("zzz", src, dst, scores);
            Assert.AreEqual(0, n);
        }

        /// <summary>
        /// v2.1.0 Keyed 化后：<c>RelativeTime</c> 依赖语言数据（离线不可调用），
        /// 因此单测改为验证**语义分档** <see cref="NavFmt.TimeBucketOf"/>（边界值仍然被真正覆盖），
        /// 文案正确性交给发布门禁的"翻译键覆盖 + 中英键对称"两道检查。
        /// </summary>
        [Test]
        public void TimeBucketOf_Boundaries()
        {
            Assert.AreEqual(TimeBucket.JustNow, NavFmt.TimeBucketOf(30f));
            Assert.AreEqual(TimeBucket.JustNow, NavFmt.TimeBucketOf(59.9f), "60s 以下一律 刚刚");
            Assert.AreEqual(TimeBucket.Minutes, NavFmt.TimeBucketOf(60.1f), "跨过 60s 才是 分钟档");
            Assert.AreEqual(TimeBucket.Minutes, NavFmt.TimeBucketOf(90f));
            Assert.AreEqual(TimeBucket.Hours, NavFmt.TimeBucketOf(7200f));
            Assert.AreEqual(TimeBucket.Days, NavFmt.TimeBucketOf(3f * 86400f));
            Assert.AreEqual(TimeBucket.JustNow, NavFmt.TimeBucketOf(-5f), "负数按 0 处理 → 刚刚");
        }

        [Test]
        public void Approach_InterpolatesTowardTarget()
        {
            Assert.AreEqual(10f, NavFmt.Approach(0f, 10f, 1f), 0.001f);
            float a = NavFmt.Approach(0f, 10f, 0.2f);
            Assert.Greater(a, 0f);
            Assert.Less(a, 10f);
            Assert.Less(NavFmt.Approach(a, 10f, 0.2f), 10f);
        }

        [Test]
        public void CountNodes_Tree()
        {
            var root = new TreeNode("root");
            var a = root.Add(new TreeNode("a"));
            a.Add(new TreeNode("a1"));
            root.Add(new TreeNode("b"));
            Assert.AreEqual(4, NavFmt.CountNodes(root));
            Assert.AreEqual(0, NavFmt.CountNodes(null));
        }

        [Test]
        public void NavStack_PushTruncatesForwardAndClamps()
        {
            var s = new NavAtomsB.NavStack();
            Assert.IsFalse(s.CanBack);
            s.Push("事件流");
            s.Push("张三");
            s.Push("生涯工时");
            Assert.AreEqual(3, s.Depth);
            Assert.IsFalse(s.CanForward);

            Assert.AreEqual("张三", s.Back());
            Assert.IsTrue(s.CanForward);
            s.Push("总账");                       // 截断前进分支
            Assert.IsFalse(s.CanForward);
            Assert.AreEqual(3, s.Depth);
            Assert.AreEqual("总账", s.Current);

            Assert.AreEqual("张三", s.Back());       // 退一步
            Assert.AreEqual("总账", s.Forward());    // 进一步
            Assert.AreEqual("张三", s.Back());
            Assert.AreEqual("事件流", s.Back());     // 退到最早一条

            // 超限丢最旧
            var big = new NavAtomsB.NavStack();
            for (int i = 0; i < NavFmt.StackLimit + 5; i++) big.Push("p" + i);
            Assert.AreEqual(NavFmt.StackLimit, big.Depth);
            Assert.AreEqual("p" + (NavFmt.StackLimit + 4), big.Current);
        }
    }
}
