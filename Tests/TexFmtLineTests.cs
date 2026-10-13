using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>
    /// `TexFmt.Line` / `Polyline` 的**像素正确性**单测（2026-09-13 新增）。
    ///
    /// <para><b>存在意义</b>：这两个方法是替掉原版 `Widgets.DrawLine` 的 —— 后者内部改 **GL 矩阵**，
    /// 在滚动视图内会把元素画到错位置（「飞散」）。替换的正确性**只能靠像素级断言**守护
    /// （滚动视图上下文无法离线单测，那部分由实机验收清单覆盖）。</para>
    ///
    /// <para>本组只用托管实现（`Mathf.Sqrt/FloorToInt/Clamp` 等），符合框架「离线单测不得触碰 ECall」的纪律。</para>
    /// </summary>
    [TestFixture]
    public class TexFmtLineTests
    {
        private const int W = 32, H = 32;

        private static Color32[] Empty() => new Color32[W * H];

        private static byte A(Color32[] buf, int x, int y) => buf[y * W + x].a;

        // ---- Line ----

        [Test]
        public void Line_Horizontal_MarksLineAndLeavesFarPixelsEmpty()
        {
            var buf = Empty();
            TexFmt.Line(buf, W, H, 4f, 16.5f, 28f, 16.5f, Color.white, 2f);
            Assert.Greater(A(buf, 16, 16), 0, "线段中点的像素应被写入");
            Assert.Greater(A(buf, 6, 16), 0, "线段起点附近应被写入");
            Assert.AreEqual(0, A(buf, 16, 2), "远离线段的像素必须保持为空");
            Assert.AreEqual(0, A(buf, 16, 30), "远离线段的像素必须保持为空");
        }

        [Test]
        public void Line_Diagonal_MarksBothEndsAndMiddle()
        {
            var buf = Empty();
            TexFmt.Line(buf, W, H, 4f, 4f, 28f, 28f, Color.white, 1.5f);
            Assert.Greater(A(buf, 4, 4), 0, "起点");
            Assert.Greater(A(buf, 16, 16), 0, "中点");
            Assert.Greater(A(buf, 27, 27), 0, "终点附近");
            Assert.AreEqual(0, A(buf, 27, 5), "另一侧对角线方向应为空");
        }

        [Test]
        public void Line_ThickerCoversMorePixels()
        {
            var thin = Empty();
            var thick = Empty();
            TexFmt.Line(thin, W, H, 2f, 16f, 30f, 16f, Color.white, 1f);
            TexFmt.Line(thick, W, H, 2f, 16f, 30f, 16f, Color.white, 6f);
            Assert.Greater(CountNonEmpty(thick), CountNonEmpty(thin), "更粗的线应覆盖更多像素");
        }

        [Test]
        public void Line_OutsideBuffer_IsClippedNotThrowing()
        {
            var buf = Empty();
            Assert.DoesNotThrow(() => TexFmt.Line(buf, W, H, -50f, -50f, 100f, 100f, Color.white, 2f));
            Assert.Greater(CountNonEmpty(buf), 0, "穿过缓冲的线仍应画出可见部分");
        }

        [Test]
        public void Line_DegenerateZeroLength_DoesNotThrow()
        {
            var buf = Empty();
            Assert.DoesNotThrow(() => TexFmt.Line(buf, W, H, 10f, 10f, 10f, 10f, Color.white, 2f));
        }

        [Test]
        public void Line_NullOrZeroSize_IsSafe()
        {
            Assert.DoesNotThrow(() => TexFmt.Line(null, W, H, 0f, 0f, 10f, 10f, Color.white, 1f));
            Assert.DoesNotThrow(() => TexFmt.Line(new Color32[4], 2, 2, 0f, 0f, 10f, 10f, Color.white, 1f));
            Assert.DoesNotThrow(() => TexFmt.Line(new Color32[0], 0, 0, 0f, 0f, 10f, 10f, Color.white, 1f));
        }

        // ---- Polyline ----

        [Test]
        public void Polyline_MarksEverySegment()
        {
            var buf = Empty();
            TexFmt.Polyline(buf, W, H,
                new[] { 3f, 16f, 28f }, new[] { 28f, 4f, 28f }, Color.white, 2f);
            Assert.Greater(A(buf, 3, 27), 0, "第 1 段起点");
            Assert.Greater(A(buf, 15, 5), 0, "第 2 段（上升段）中点附近");
            Assert.Greater(A(buf, 27, 27), 0, "第 3 段终点附近");
        }

        [Test]
        public void Polyline_MismatchedArrays_UsesShorter()
        {
            var buf = Empty();
            Assert.DoesNotThrow(() => TexFmt.Polyline(buf, W, H,
                new[] { 2f, 16f, 28f }, new[] { 28f }, Color.white, 1.5f));
        }

        // ---- 不变量 ----

        [Test]
        public void Line_DoesNotWriteOutsideBuffer()
        {
            // 越界端点 + 粗线：只验证「不越界写、数组长度不变、被穿过的区域有内容」。
            // ⚠ 不要断言角点为空 —— (0,0) 正在对角线上，必然被写入（本测试初版就写错了这个断言）。
            var buf = Empty();
            TexFmt.Line(buf, W, H, -10f, -10f, W + 10f, H + 10f, Color.white, 4f);
            Assert.AreEqual(W * H, buf.Length, "缓冲长度不得改变");
            Assert.Greater(A(buf, 16, 16), 0, "穿过的中心区域应有内容");
            Assert.AreEqual(0, A(buf, 0, 31), "左下角远离该对角线，应为空");
        }

        private static int CountNonEmpty(Color32[] buf)
        {
            int n = 0;
            for (int i = 0; i < buf.Length; i++) if (buf[i].a > 0) n++;
            return n;
        }
    }
}
