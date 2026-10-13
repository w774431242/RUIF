using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// L0 · 运行时适配层：所有会修改全局绘制状态的调用都必须经过这里。
    ///
    /// 设计要点：
    /// ① 作用域是 <b>struct</b> —— 零堆分配，IMGUI 每帧调用不产生 GC；
    /// ② 进入时保存旧值，Dispose 时恢复 —— 靠 C# 的 using 嵌套形成"栈"，
    ///    避免手写 try/finally 漏恢复导致染色泄漏到全游戏；
    /// ③ 内建 1.6 已知约束（DrawBoxSolid 会重置 GUI.color，故 tint 直接传参）。
    /// </summary>
    public static class Draw
    {
        // ---- 作用域工厂 ----
        /// <summary>GUI.color 作用域工厂（退出时自动恢复，防染色泄漏）。</summary>
        public static ColorScope Color(Color c) => new ColorScope(c);
        /// <summary>GUI.enabled 作用域工厂（禁用态统一入口）。</summary>
        public static EnabledScope Enabled(bool value) => new EnabledScope(value);
        /// <summary>Text.Font 作用域工厂。</summary>
        public static FontScope Font(GameFont font) => new FontScope(font);
        /// <summary>Text.Anchor 作用域工厂。</summary>
        public static AnchorScope Anchor(TextAnchor anchor) => new AnchorScope(anchor);

        /// <summary>绘制旋转作用域（Caret 展开动画、贴图旋转绘制等）。</summary>
        public static RotateScope Rotate(float angle, Vector2 pivot) => new RotateScope(angle, pivot);

        /// <summary>
        /// GUI.matrix 旋转作用域：进入时**在当前坐标系内绕 pivot 旋转**，退出时恢复原矩阵。
        ///
        /// <para><b>⛔ 不再使用 <c>GUIUtility.RotateAroundPivot</c>（2026-09-12 根因修复）</b>：
        /// 该 Unity API 把 pivot 当作「**应用旋转前的 GUI 空间**」坐标 —— 其内部实现是先把矩阵归零、
        /// 再合成 <c>T(p) · R · T(-p) · M</c> ⇒ **p 是在外层矩阵之前解释的**，而不是调用点所在的局部空间。
        /// 于是只要外层已有平移/裁剪作用域（窗口偏移、<c>GUI.BeginClip</c>、<c>GUI.BeginGroup</c>、
        /// 滚动视图），"按 rect 局部坐标传 pivot"就会**整体错位**：元素画在容器内、旋转却发生在容器外，
        /// 结果被甩到容器之外（实测事故：<c>Industrial.Gauge</c> 的 45×N 个 8×3 分区色块飞满整屏，
        /// 见 `PersonalChronicle` 侧 `制造系统中心_RUIF改造方案.md` §10）。</para>
        ///
        /// <para><b>正确语义</b>：构造**局部空间**旋转矩阵并**后乘**当前矩阵
        /// （<c>M' = M · T(p) · R · T(-p)</c>）—— 语义即"先把绘制点绕**局部 pivot** 旋转，再做外层变换"，
        /// 与调用点（<c>Draw.Box</c> / <c>Draw.Label</c> 的落点）严格同空间，
        /// 且与外层矩阵是平移、缩放还是旋转**无关**。调用点因此只需传 rect 局部坐标。</para>
        /// </summary>
        public readonly struct RotateScope : System.IDisposable
        {
            private readonly Matrix4x4 prev;
            public RotateScope(float angle, Vector2 pivot)
            {
                prev = GUI.matrix;
                GUI.matrix = prev * LocalRotation(angle, pivot);
            }
            /// <summary>恢复进入前的 GUI.matrix。</summary>
            public void Dispose() => GUI.matrix = prev;
        }

        /// <summary>
        /// **局部空间**旋转矩阵（绕 <paramref name="pivot"/> 旋转 <paramref name="angle"/> 度）：
        /// <c>T(pivot) · R(angle) · T(-pivot)</c>。纯函数（不读写任何 GUI 状态）⇒ 可离线单测
        /// （见 `Tests/DrawRotateTests.cs`）。
        ///
        /// <para><b>用途与语义</b>：<see cref="RotateScope"/> 把它**后乘**当前 <c>GUI.matrix</c>，
        /// 得到"在调用点自身坐标系内绕局部 pivot 旋转"的语义 —— 这正是 <c>Draw.Box/Label</c> 传 rect
        /// 局部坐标时需要的；无论外层矩阵是平移、缩放还是旋转都不受影响。
        /// 对比：<c>GUIUtility.RotateAroundPivot</c> 的 pivot 是"旋转前的 GUI 空间"语义，
        /// 在外层有平移/裁剪作用域时会整体错位（详见 <see cref="RotateScope"/> 注释）。</para>
        /// </summary>
        internal static Matrix4x4 LocalRotation(float angle, Vector2 pivot)
        {
            // 显式展开 T(p)·R·T(-p)（结果 = 绕 p 旋转：q → p + R·(q-p)）：
            // ⚠️ **不用 Quaternion.Euler / Matrix4x4.TRS** —— 前者经 `Internal_FromEulerRad` 是 ECall，
            //    在非 Unity 宿主（离线单测）里会抛 `SecurityException`（本框架 ECall 铁律同源）。
            float rad = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Matrix4x4(
                new Vector4(cos, sin, 0f, 0f),                                  // 列 0
                new Vector4(-sin, cos, 0f, 0f),                                 // 列 1
                new Vector4(0f, 0f, 1f, 0f),                                    // 列 2
                new Vector4(pivot.x - cos * pivot.x + sin * pivot.y,            // 列 3 = 平移
                            pivot.y - sin * pivot.x - cos * pivot.y, 0f, 1f));
        }

        /// <summary>GUI.color 作用域。</summary>
        public readonly struct ColorScope : System.IDisposable
        {
            private readonly Color prev;
            public ColorScope(Color c) { prev = GUI.color; GUI.color = c; }
            /// <summary>恢复进入前的 GUI.color。</summary>
            public void Dispose() => GUI.color = prev;
        }

        /// <summary>GUI.enabled 作用域（禁用态统一走这里，替代各控件自造 disabled 参数）。</summary>
        public readonly struct EnabledScope : System.IDisposable
        {
            private readonly bool prev;
            public EnabledScope(bool value) { prev = GUI.enabled; GUI.enabled = value; }
            /// <summary>恢复进入前的 GUI.enabled。</summary>
            public void Dispose() => GUI.enabled = prev;
        }

        /// <summary>Text.Font 作用域。</summary>
        public readonly struct FontScope : System.IDisposable
        {
            private readonly GameFont prev;
            public FontScope(GameFont font) { prev = Text.Font; Text.Font = font; }
            /// <summary>恢复进入前的 Text.Font。</summary>
            public void Dispose() => Text.Font = prev;
        }

        /// <summary>Text.Anchor 作用域。</summary>
        public readonly struct AnchorScope : System.IDisposable
        {
            private readonly TextAnchor prev;
            public AnchorScope(TextAnchor anchor) { prev = Text.Anchor; Text.Anchor = anchor; }
            /// <summary>恢复进入前的 Text.Anchor。</summary>
            public void Dispose() => Text.Anchor = prev;
        }

        // ---- 绘制原语 ----

        /// <summary>实心矩形。注意 1.6 的 DrawBoxSolid 会重置 GUI.color，故颜色直接传参而非靠外层着色。</summary>
        public static void Box(Rect rect, Color color) => Widgets.DrawBoxSolid(rect, color);

        /// <summary>四边 1px 描边。</summary>
        public static void Border(Rect rect, Color color)
        {
            using (Color(color))
            {
                Widgets.DrawLineHorizontal(rect.x, rect.y, rect.width);
                Widgets.DrawLineHorizontal(rect.x, rect.yMax - 1f, rect.width);
                Widgets.DrawLineVertical(rect.x, rect.y, rect.height);
                Widgets.DrawLineVertical(rect.xMax - 1f, rect.y, rect.height);
            }
        }

        /// <summary>水平分隔线。</summary>
        public static void LineH(float x, float y, float length, Color color)
        {
            using (Color(color)) Widgets.DrawLineHorizontal(x, y, length);
        }

        /// <summary>垂直分隔线（卡片左侧 accent 色条即两条竖线并排）。</summary>
        public static void LineV(float x, float y, float length, Color color)
        {
            using (Color(color)) Widgets.DrawLineVertical(x, y, length);
        }

        /// <summary>文本（自管 Font / Anchor / Color 三态）。</summary>
        public static void Label(Rect rect, string text, GameFont font, Color color, TextAnchor anchor)
        {
            // ⚠ 2026-09-15 修复：显式**禁用自动换行**（保存 / 恢复 `Text.WordWrap`）。
            // 原版 `Widgets.Label` 遵循**全局** `Text.WordWrap`；一旦链路上某处把它设为 true
            // 而未恢复，调用方的**定宽标签**就会被折行（消费方实机缺陷：5 字标签「现役殖民者」
            // 被拆成两行、错乱压到图形上）。本方法作为 L0 文本出口，负责把该全局状态钉死。
            bool prevWrap = Text.WordWrap;
            Text.WordWrap = false;
            try
            {
                using (Font(font))
                using (Anchor(anchor))
                using (Color(color))
                {
                    Widgets.Label(rect, text ?? string.Empty);
                }
            }
            finally
            {
                Text.WordWrap = prevWrap;
            }
        }

        /// <summary>
        /// 9 宫格贴图（四角各 25% 的 UV 约定，与 <c>Widgets.DrawAtlas</c> 一致）。
        /// </summary>
        public static void Atlas(Rect rect, Texture2D tex) => Widgets.DrawAtlas(rect, tex);

        /// <summary>
        /// 9 宫格贴图 + <c>GUI.color</c> 着色。
        /// ⚠️ 只对<b>中性色</b>贴图有意义（自带配色的贴图用语义色去乘会画出脏色）。
        /// </summary>
        public static void Atlas(Rect rect, Texture2D tex, Color tint)
        {
            using (Color(tint)) Widgets.DrawAtlas(rect, tex);
        }

        /// <summary>
        /// 溢出省略标签：放不下时截断并补省略号，而不是让文字被矩形<b>硬切</b>。
        /// <para>这是"文本被切割"的常规解 —— 硬切一半的字比"…"难看得多，也更难判断是布局错了还是内容真的长。
        /// 底层 <c>Widgets.LabelEllipses</c>（内部走原版 <c>Text.ClampTextWithEllipsis</c>）。</para>
        /// <para>与 <see cref="Label"/> 的差别仅在"放不下时的行为"：本方法给你省略号，<see cref="Label"/> 给你裁切。</para>
        /// </summary>
        public static void EllipsisLabel(Rect rect, string text, GameFont font, Color color, TextAnchor anchor)
        {
            using (Font(font))
            using (Anchor(anchor))
            using (Color(color))
            {
                Widgets.LabelEllipses(rect, text ?? string.Empty);
            }
        }

        /// <summary>保留 RGB 覆写 alpha（Pill / Badge 的底色派生用）。</summary>
        public static Color WithAlpha(Color c, float alpha) => new Color(c.r, c.g, c.b, alpha);
    }
}
