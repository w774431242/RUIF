using System;
using UnityEngine;

namespace RUIF
{
    /// <summary>
    /// ⑥ 列表重排候选层绘制转发：具名方法 List / Grip（与 <see cref="ReorderCandFmt"/> 的 Build 工厂同名，已 == 对应 Build 名），
    /// 纯转发既有 <see cref="ReorderAtoms"/>，禁复制逻辑。候选 readonly struct 嵌套定义于本 static class（对齐 DataCandAtoms.BillEntry / StatusCandAtoms.StatusPulseDot 写法）；
    /// 为避免与同名绘制方法（List / Grip）发生 C# 类型-成员命名冲突，struct 以族前缀命名为 <c>ReorderList</c> / <c>ReorderGrip</c>。
    ///
    /// <para><b>写回状态以入口 ref 参数承载（R8）</b>：跨帧状态持有者 groupId 不置于候选 struct 内——
    /// R8 禁止候选 struct 持 ref 字段（亦守护 R3「Fmt 纯计算」边界），而是以入口 <c>ref int groupId</c> 参数承载，
    /// 与原 <see cref="ReorderAtoms.List"/> 的持有方式完全同形，且正确写回归调用方。
    /// 此为<b>正式形态</b>（非 C#9 权宜）：候选 struct 仅打包值参数（count / rowHeight / draw / onReorder / highlightDragged）。
    /// 本工程 <c>LangVersion=9.0</c> 同样不支持 ref 字段，与 R8 一致，无版本升级诉求。</para>
    /// </summary>
    public static class ReorderCandAtoms
    {
        /// <summary>列表重排候选（值参数包）。跨帧状态 groupId 由调用方经入口 ref 持有（R8）。</summary>
        public readonly struct ReorderList
        {
            /// <summary>行数。</summary>
            public readonly int count;
            /// <summary>行高（像素）。</summary>
            public readonly float rowHeight;
            /// <summary>行绘制回调（每行每帧一次）。</summary>
            public readonly ReorderRowDrawer draw;
            /// <summary>提交回调 (fromIndex, insertIndex)；null 时内部退化为 Noop。</summary>
            public readonly Action<int, int> onReorder;
            /// <summary>是否由引擎绘制被拖行高亮（默认开）。</summary>
            public readonly bool highlightDragged;

            public ReorderList(int count, float rowHeight, ReorderRowDrawer draw,
                Action<int, int> onReorder, bool highlightDragged)
            {
                this.count = count;
                this.rowHeight = rowHeight;
                this.draw = draw;
                this.onReorder = onReorder;
                this.highlightDragged = highlightDragged;
            }
        }

        /// <summary>行首握把候选（值参数包，无状态）。color 默认 dragging?Accent:Dim（溯源 Tokens）。</summary>
        public readonly struct ReorderGrip
        {
            /// <summary>该行是否正被拖动。</summary>
            public readonly bool dragging;
            /// <summary>握把颜色（默认与 ReorderAtoms.Grip 同源：dragging?Accent:Dim，溯源 Tokens）。</summary>
            public readonly Color color;

            public ReorderGrip(bool dragging, Color color)
            {
                this.dragging = dragging;
                this.color = color;
            }
        }

        /// <summary>垂直行列表重排：转发 ReorderAtoms.List；返回本帧是否处于拖拽中。</summary>
        public static bool List(Rect area, ReorderList cand, ref int groupId)
            => ReorderAtoms.List(area, cand.count, cand.rowHeight, ref groupId, cand.draw, cand.onReorder, cand.highlightDragged);

        /// <summary>行首握把（三点式）：转发 ReorderAtoms.Grip（颜色由 Atoms 内部按 dragging 决定，与 cand.color 同源）。</summary>
        public static void Grip(Rect rect, ReorderGrip cand)
            => ReorderAtoms.Grip(rect, cand.dragging);
    }
}
