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

        /// <summary>GUI.matrix 旋转作用域：进入时绕 pivot 旋转，退出时恢复原矩阵。</summary>
        public readonly struct RotateScope : System.IDisposable
        {
            private readonly Matrix4x4 prev;
            public RotateScope(float angle, Vector2 pivot)
            {
                prev = GUI.matrix;
                GUIUtility.RotateAroundPivot(angle, pivot);
            }
            /// <summary>恢复进入前的 GUI.matrix。</summary>
            public void Dispose() => GUI.matrix = prev;
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
            using (Font(font))
            using (Anchor(anchor))
            using (Color(color))
            {
                Widgets.Label(rect, text ?? string.Empty);
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

        /// <summary>保留 RGB 覆写 alpha（Pill / Badge 的底色派生用）。</summary>
        public static Color WithAlpha(Color c, float alpha) => new Color(c.r, c.g, c.b, alpha);
    }
}
