using UnityEngine;
using LudeonTK;

namespace RUIF
{
    /// <summary>
    /// L0 · 像素对齐（Pixel snap）。把坐标吸附到 UI 缩放网格，消除亚像素模糊。
    ///
    /// <para><b>为什么必须有这一层</b>：RimWorld 的 UI 缩放（<c>Prefs.UIScale</c>）非整数时，
    /// 1px 描边/分隔线落在半像素上会被双线性插值糊成 2px 灰边 —— 观感立刻"不是原版"。
    /// 原版做法是让每个坐标过一遍 <c>UIScaling.AdjustCoordToUIScaling*</c>
    /// （实证：<c>Verse.TabRecord.Draw</c> 用它对页签中段做 xMin/xMax 对齐）。本类即该做法的收口。</para>
    ///
    /// <para><b>⛔ 不在此处调 <c>GUIUtility.RotateAroundPivot</c> 族</b>：与旋转无关，本类只做坐标吸附。</para>
    ///
    /// <para><b>命名空间注意</b>：底层 <c>UIScaling</c> 位于 <c>LudeonTK</c>（Dev 域），
    /// 但**生产代码在用**（<c>TabRecord.Draw</c>），故此处直接引用并以此注释留档；
    /// 消费方只需依赖本类，不必自行 <c>using LudeonTK</c>。</para>
    /// </summary>
    public static class Px
    {
        /// <summary>把矩形四边吸附到 UI 缩放网格（xMin/yMin 取 floor，xMax/yMax 取 ceil ⇒ 只会变大不会切内容）。</summary>
        public static Rect Snap(Rect r) => UIScaling.AdjustRectToUIScaling(r);

        /// <summary>单坐标向下吸附（起点用）。</summary>
        public static float Floor(float coord) => UIScaling.AdjustCoordToUIScalingFloor(coord);

        /// <summary>单坐标向上吸附（终点用）。</summary>
        public static float Ceil(float coord) => UIScaling.AdjustCoordToUIScalingCeil(coord);

        /// <summary>点吸附（起点语义）。</summary>
        public static Vector2 Floor(Vector2 v) => new Vector2(Floor(v.x), Floor(v.y));

        /// <summary>
        /// 把 <c>[a, b]</c> 区间吸附后返回<b>不短于原区间</b>的长度 —— 用于分隔线/描边这类"1px 必须真是 1px"的场景。
        /// <para>直接对长度取整会把 3.4px 压成 3px 并在相邻处留缝；此处对齐两端后相减，
        /// 保证覆盖范围只扩不缩。</para>
        /// </summary>
        public static float Span(float a, float b) => Ceil(b) - Floor(a);

        /// <summary>水平线用的吸附矩形（高 1px，落在整像素行上）。</summary>
        public static Rect HLine(float x, float y, float length)
        {
            float sx = Floor(x);
            return new Rect(sx, Floor(y), Span(x, x + length), 1f);
        }

        /// <summary>垂直线用的吸附矩形（宽 1px，落在整像素列上）。</summary>
        public static Rect VLine(float x, float y, float length)
        {
            float sy = Floor(y);
            return new Rect(Floor(x), sy, 1f, Span(y, y + length));
        }
    }
}
