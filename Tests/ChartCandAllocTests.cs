using NUnit.Framework;
using RUIF;
using UnityEngine;

namespace RUIF.Tests
{
    /// <summary>
    /// P1「绘制路径分配消除」（B3 批次 1）的 <b>语义变更登记</b> 单测。
    ///
    /// <para>改动本身是<b>分配层</b>的（<c>Array.ConvertAll</c> 4 个临时 <c>float[]</c> → 单趟扫描；
    /// 捕获 <c>Func</c> 闭包 → 无捕获静态函数），顺带收窄了 <c>NaN</c> 的污染面。
    /// ⚠ <b>此处更正方案书的一处表述</b>：方案书写"原 <c>ChartCandFmt.Min/Max</c> 遇 NaN 会传播
    /// （<c>Mathf.Min(NaN,x)==NaN</c>）"，实测<b>不成立</b> —— <c>ChartCandFmt.Min/Max</c> 自身是
    /// <c>if (v[i] &lt; m)</c> 扫描，NaN 天然被忽略；会传播 NaN 的是 <c>Mathf.Min/Max</c>（被
    /// <c>StepLine</c>/<c>Ohlc</c>/<c>BoxPlot</c>/<c>Waterfall</c>/<c>Spc</c> 使用，本次未改）。
    /// 旧实现的真实污染点是<b>首元素做种子</b>：<c>float m = v[0]</c>。详见
    /// <see cref="ScanBubble_FirstElementNaN_NoLongerPoisonsTheWholeRange"/>。</para>
    ///
    /// <para>本文件只触 <see cref="ChartCandAtoms.ScanBubble"/> / <see cref="ChartCandAtoms.ScanRange"/> /
    /// <see cref="ChartCandAtoms.MapX"/> / <see cref="ChartCandAtoms.MapY"/> —— 全部是<b>纯函数</b>
    /// （零 GUI / 零引擎调用），故可离线运行；绘图入口本身（<c>Bubble</c> / <c>RangeArea</c>）
    /// 会调 <c>Draw.Box</c> / <c>Widgets.DrawLine</c>，离线宿主不可调用，<b>不在本文件断言范围内</b>。</para>
    /// </summary>
    [TestFixture]
    public class ChartCandAllocTests
    {
        private static ChartCandAtoms.BubblePt Bubble(float x, float y, float r = 1f)
            => new ChartCandAtoms.BubblePt { x = x, y = y, r = r };

        private static ChartCandAtoms.RangePt Range(float x, float lo, float hi)
            => new ChartCandAtoms.RangePt { x = x, lo = lo, hi = hi };

        // ---- ScanBubble ----

        [Test]
        public void ScanBubble_PlainInput_ReturnsExactExtrema()
        {
            var pts = new[] { Bubble(1f, 2f), Bubble(-3f, 9f), Bubble(4f, 0f) };
            ChartCandAtoms.ScanBubble(pts, out float x0, out float x1, out float y0, out float y1);
            Assert.AreEqual(-3f, x0, 1e-5f);
            Assert.AreEqual(4f, x1, 1e-5f);
            Assert.AreEqual(0f, y0, 1e-5f);
            Assert.AreEqual(9f, y1, 1e-5f);
        }

        [Test]
        public void ScanBubble_SinglePoint_ExtremesCollapse()
        {
            var pts = new[] { Bubble(7f, -2f) };
            ChartCandAtoms.ScanBubble(pts, out float x0, out float x1, out float y0, out float y1);
            Assert.AreEqual(7f, x0, 1e-5f);
            Assert.AreEqual(7f, x1, 1e-5f);
            Assert.AreEqual(-2f, y0, 1e-5f);
            Assert.AreEqual(-2f, y1, 1e-5f);
        }

        /// <summary>
        /// 核心语义登记：<b>NaN 被忽略，不再传播</b>。
        /// </summary>
        [Test]
        public void ScanBubble_MixedNaN_IgnoresNaNInsteadOfPropagating()
        {
            var pts = new[] { Bubble(1f, 2f), Bubble(float.NaN, float.NaN), Bubble(3f, 5f) };
            ChartCandAtoms.ScanBubble(pts, out float x0, out float x1, out float y0, out float y1);

            Assert.IsFalse(float.IsNaN(x0), "x0 不得为 NaN");
            Assert.IsFalse(float.IsNaN(x1), "x1 不得为 NaN");
            Assert.IsFalse(float.IsNaN(y0), "y0 不得为 NaN");
            Assert.IsFalse(float.IsNaN(y1), "y1 不得为 NaN");

            Assert.AreEqual(1f, x0, 1e-5f);
            Assert.AreEqual(3f, x1, 1e-5f);
            Assert.AreEqual(2f, y0, 1e-5f);
            Assert.AreEqual(5f, y1, 1e-5f);
        }

        /// <summary>
        /// <b>语义变更的真实边界</b>（2026-09-16 实测更正方案书表述）：
        /// 方案书称"原 <c>ChartCandFmt.Min/Max</c> 遇 NaN 会传播（<c>Mathf.Min(NaN,x)==NaN</c>）"——
        /// 这句<b>只对 <c>Mathf.Min</c> 成立，对 <c>ChartCandFmt.Min/Max</c> 不成立</b>：
        /// 后者本身是 <c>if (v[i] &lt; m)</c> 扫描，NaN 天然被忽略。
        /// <b>唯一的传播路径是"首元素"</b>：<c>float m = v[0]</c> 把首元素当种子 ⇒ 首元素为 NaN 时整个范围变 NaN。
        /// 因此本次 <c>Array.ConvertAll</c> → 单趟扫描的**真实**行为差异是：首元素为 NaN 时不再污染。
        /// </summary>
        [Test]
        public void ScanBubble_FirstElementNaN_NoLongerPoisonsTheWholeRange()
        {
            // 旧实现的传播面（钉住，防止把"已修"当成"从未有问题"）
            float[] seedNaN = { float.NaN, 1f, 3f };
            Assert.IsTrue(float.IsNaN(ChartCandFmt.Min(seedNaN)), "旧实现：首元素 NaN 会被当成种子 ⇒ 范围全 NaN");
            Assert.IsTrue(float.IsNaN(ChartCandFmt.Max(seedNaN)), "旧实现：同上（Max 亦然）");

            // 非首元素的 NaN 旧实现已能忽略（这也是"不是等价重构、而是缩小污染面"的依据）
            float[] tailNaN = { 1f, float.NaN, 3f };
            Assert.AreEqual(1f, ChartCandFmt.Min(tailNaN), 1e-5f);
            Assert.AreEqual(3f, ChartCandFmt.Max(tailNaN), 1e-5f);

            // 新扫描：以 ±MaxValue/MinValue 为哨兵 ⇒ 首元素 NaN 也被忽略
            var pts = new[] { Bubble(float.NaN, float.NaN), Bubble(1f, 2f), Bubble(3f, 5f) };
            ChartCandAtoms.ScanBubble(pts, out float x0, out float x1, out float y0, out float y1);
            Assert.AreEqual(1f, x0, 1e-5f);
            Assert.AreEqual(3f, x1, 1e-5f);
            Assert.AreEqual(2f, y0, 1e-5f);
            Assert.AreEqual(5f, y1, 1e-5f);
        }

        /// <summary>全 NaN ⇒ 回退单位区间，保证后续归一化不产生 NaN 几何。</summary>
        [Test]
        public void ScanBubble_AllNaN_FallsBackToUnitInterval()
        {
            var pts = new[] { Bubble(float.NaN, float.NaN), Bubble(float.NaN, float.NaN) };
            ChartCandAtoms.ScanBubble(pts, out float x0, out float x1, out float y0, out float y1);
            Assert.AreEqual(0f, x0, 1e-5f);
            Assert.AreEqual(1f, x1, 1e-5f);
            Assert.AreEqual(0f, y0, 1e-5f);
            Assert.AreEqual(1f, y1, 1e-5f);
        }

        /// <summary>空数组也安全（helper 自身不得依赖调用方的非空前置条件）。</summary>
        [Test]
        public void ScanBubble_EmptyArray_FallsBackToUnitInterval()
        {
            ChartCandAtoms.ScanBubble(new ChartCandAtoms.BubblePt[0], out float x0, out float x1, out float y0, out float y1);
            Assert.AreEqual(0f, x0, 1e-5f);
            Assert.AreEqual(1f, x1, 1e-5f);
            Assert.AreEqual(0f, y0, 1e-5f);
            Assert.AreEqual(1f, y1, 1e-5f);
        }

        // ---- ScanRange ----

        [Test]
        public void ScanRange_MixedNaN_TakesLoFloorAndHiCeiling()
        {
            var pts = new[]
            {
                Range(2f, 4f, 8f),
                Range(float.NaN, float.NaN, float.NaN),
                Range(6f, 1f, 3f),
            };
            ChartCandAtoms.ScanRange(pts, out float x0, out float x1, out float y0, out float y1);

            Assert.AreEqual(2f, x0, 1e-5f);
            Assert.AreEqual(6f, x1, 1e-5f);
            Assert.AreEqual(1f, y0, 1e-5f);   // y 下界 = 各点 lo 的最小值
            Assert.AreEqual(8f, y1, 1e-5f);   // y 上界 = 各点 hi 的最大值
            Assert.IsFalse(float.IsNaN(x0) || float.IsNaN(x1) || float.IsNaN(y0) || float.IsNaN(y1));
        }

        [Test]
        public void ScanRange_PlainInput_ReturnsExactExtrema()
        {
            var pts = new[] { Range(-5f, -1f, 1f), Range(0f, -4f, 2f), Range(9f, 3f, 10f) };
            ChartCandAtoms.ScanRange(pts, out float x0, out float x1, out float y0, out float y1);
            Assert.AreEqual(-5f, x0, 1e-5f);
            Assert.AreEqual(9f, x1, 1e-5f);
            Assert.AreEqual(-4f, y0, 1e-5f);
            Assert.AreEqual(10f, y1, 1e-5f);
        }

        [Test]
        public void ScanRange_AllNaN_FallsBackToUnitInterval()
        {
            var pts = new[] { Range(float.NaN, float.NaN, float.NaN) };
            ChartCandAtoms.ScanRange(pts, out float x0, out float x1, out float y0, out float y1);
            Assert.AreEqual(0f, x0, 1e-5f);
            Assert.AreEqual(1f, x1, 1e-5f);
            Assert.AreEqual(0f, y0, 1e-5f);
            Assert.AreEqual(1f, y1, 1e-5f);
        }

        // ---- MapX / MapY（无捕获静态映射，替代原捕获闭包） ----

        [Test]
        public void MapX_MapsDomainEndpointsToPlotEdges()
        {
            var p = new Rect(10f, 20f, 100f, 50f);
            Assert.AreEqual(p.x, ChartCandAtoms.MapX(5f, 5f, 15f, p), 1e-3f);
            Assert.AreEqual(p.xMax, ChartCandAtoms.MapX(15f, 5f, 15f, p), 1e-3f);
            Assert.AreEqual(p.x + 50f, ChartCandAtoms.MapX(10f, 5f, 15f, p), 1e-3f);
        }

        [Test]
        public void MapY_FlipsVertically()
        {
            var p = new Rect(10f, 20f, 100f, 50f);
            Assert.AreEqual(p.yMax, ChartCandAtoms.MapY(0f, 0f, 10f, p), 1e-3f);   // 数据下界 → 屏幕下沿
            Assert.AreEqual(p.y, ChartCandAtoms.MapY(10f, 0f, 10f, p), 1e-3f);    // 数据上界 → 屏幕上沿
            Assert.AreEqual(p.center.y, ChartCandAtoms.MapY(5f, 0f, 10f, p), 1e-3f);
        }

        /// <summary>
        /// 「NaN 输入不产生 NaN 几何」的<b>验收断言</b>：混入 NaN 点后，
        /// 所有<b>有效</b>点仍落在绘图区边界上、坐标为有限值（即坐标轴没被 NaN 污染）。
        /// NaN 点自身无位置可言（<c>NaN - x0 = NaN</c>），故不参与定位断言 —— 这是它的固有语义，不是缺陷。
        /// </summary>
        [Test]
        public void BubbleGeometry_WithNaNPresent_ValidPointsStayFiniteAndAligned()
        {
            var pts = new[] { Bubble(1f, 2f), Bubble(float.NaN, float.NaN), Bubble(3f, 5f) };
            ChartCandAtoms.ScanBubble(pts, out float x0, out float x1, out float y0, out float y1);
            if (x1 <= x0) x1 = x0 + 1f;      // 与 ChartCandAtoms.Bubble 内联的退化守护同式
            if (y1 <= y0) y1 = y0 + 1f;

            var p = new Rect(10f, 20f, 100f, 50f);

            Assert.AreEqual(p.x, ChartCandAtoms.MapX(1f, x0, x1, p), 1e-3f);
            Assert.AreEqual(p.xMax, ChartCandAtoms.MapX(3f, x0, x1, p), 1e-3f);
            Assert.AreEqual(p.yMax, ChartCandAtoms.MapY(2f, y0, y1, p), 1e-3f);
            Assert.AreEqual(p.y, ChartCandAtoms.MapY(5f, y0, y1, p), 1e-3f);

            for (int i = 0; i < pts.Length; i++)
            {
                if (float.IsNaN(pts[i].x) || float.IsNaN(pts[i].y)) continue;   // NaN 点无位置，见方法注释
                float cx = ChartCandAtoms.MapX(pts[i].x, x0, x1, p);
                float cy = ChartCandAtoms.MapY(pts[i].y, y0, y1, p);
                Assert.IsFalse(float.IsNaN(cx), "有效点的 x 坐标不得为 NaN");
                Assert.IsFalse(float.IsNaN(cy), "有效点的 y 坐标不得为 NaN");
                Assert.GreaterOrEqual(cx, p.x - 1e-3f);
                Assert.LessOrEqual(cx, p.xMax + 1e-3f);
                Assert.GreaterOrEqual(cy, p.y - 1e-3f);
                Assert.LessOrEqual(cy, p.yMax + 1e-3f);
            }
        }

        /// <summary>全 NaN 输入下，坐标轴也必须是有限区间（回退 [0,1] 的价值所在）。</summary>
        [Test]
        public void BubbleGeometry_AllNaN_StillFiniteAxes()
        {
            var pts = new[] { Bubble(float.NaN, float.NaN) };
            ChartCandAtoms.ScanBubble(pts, out float x0, out float x1, out float y0, out float y1);
            if (x1 <= x0) x1 = x0 + 1f;
            if (y1 <= y0) y1 = y0 + 1f;
            Assert.IsFalse(float.IsNaN(x0) || float.IsNaN(x1) || float.IsNaN(y0) || float.IsNaN(y1));
            Assert.Greater(x1 - x0, 0f);
            Assert.Greater(y1 - y0, 0f);
        }
    }
}
