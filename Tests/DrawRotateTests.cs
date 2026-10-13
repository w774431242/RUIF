using NUnit.Framework;
using UnityEngine;

namespace RUIF.Tests
{
    /// <summary>
    /// `Draw.Rotate` 的**局部空间旋转语义**回归（2026-09-12 根因修复 · RUIF v1.0.2）。
    ///
    /// <para><b>事故背景</b>：旧实现直接调 <c>GUIUtility.RotateAroundPivot(angle, pivot)</c>，
    /// 而该 Unity API 把 pivot 当作「**应用旋转前的 GUI 空间**」坐标 ⇒ 只要外层已有平移/裁剪作用域
    /// （窗口偏移、<c>GUI.BeginClip</c>、<c>GUI.BeginGroup</c>、滚动视图），按 rect 局部坐标传 pivot
    /// 就会整体错位：元素画在容器里、旋转却发生在容器外，结果被甩出容器
    /// （实测：`Industrial.Gauge` 的 45×N 个 8×3 分区色块飞满整屏）。</para>
    ///
    /// <para><b>本测试锁定的不变量</b>：<c>M_outer · LocalRotation(angle, pivot)</c> 与 <c>M_outer</c>
    /// 无关地**只在局部**发生旋转 —— ① pivot 自身不动；② 任意点到 pivot 的距离不变；
    /// ③ 90° 时方向与原方向正交。若有人改回"屏幕空间 pivot"实现，断言 ② 会立刻失败
    /// （半径会被外层平移量放大到数百像素）。</para>
    /// </summary>
    public class DrawRotateTests
    {
        private const float Tol = 1e-3f;

        [Test]
        public void LocalRotation_IdentityAtZeroDegrees()
        {
            var pivot = new Vector2(40f, 15f);
            Vector3 mapped = Draw.LocalRotation(0f, pivot).MultiplyPoint3x4(new Vector3(70f, 15f, 0f));
            Assert.AreEqual(70f, mapped.x, Tol);
            Assert.AreEqual(15f, mapped.y, Tol);
        }

        [Test]
        public void LocalRotation_KeepsPivotFixed()
        {
            var pivot = new Vector2(40f, 15f);
            Vector3 mapped = Draw.LocalRotation(123f, pivot).MultiplyPoint3x4(new Vector3(pivot.x, pivot.y, 0f));
            Assert.AreEqual(pivot.x, mapped.x, Tol);
            Assert.AreEqual(pivot.y, mapped.y, Tol);
        }

        [Test]
        public void LocalRotation_OutsideTranslation_KeepsRadius()
        {
            var pivot = new Vector2(40f, 15f);
            var point = new Vector2(70f, 15f);      // 半径 30（pivot 右侧）
            // 模拟"外层已有平移作用域"（窗口偏移 / BeginClip 之后的坐标系）。
            // 显式构造（不用 Matrix4x4.Translate：避免任何 ECall 依赖，保证离线可跑）。
            Matrix4x4 outer = new Matrix4x4(
                new Vector4(1f, 0f, 0f, 0f),
                new Vector4(0f, 1f, 0f, 0f),
                new Vector4(0f, 0f, 1f, 0f),
                new Vector4(500f, 620f, 0f, 1f));
            Matrix4x4 full = outer * Draw.LocalRotation(90f, pivot);

            Vector3 mapped = full.MultiplyPoint3x4(new Vector3(point.x, point.y, 0f));
            Vector3 mappedPivot = full.MultiplyPoint3x4(new Vector3(pivot.x, pivot.y, 0f));

            Assert.AreEqual(30f, Vector3.Distance(mapped, mappedPivot), Tol,
                "旋转不得改变到 pivot 的距离（旧实现会因 pivot 落在屏幕空间而放大到数百像素）");

            var rel = new Vector2(mapped.x - mappedPivot.x, mapped.y - mappedPivot.y);
            Assert.AreEqual(0f, Vector2.Dot(rel, new Vector2(30f, 0f)), 0.05f,
                "旋转 90° 后方向应与原方向正交");
        }
    }
}
