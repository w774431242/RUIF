using System;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>行绘制委托：<paramref name="dragging"/> = 该行正被拖动（由原版引擎判定）。</summary>
    public delegate void ReorderRowDrawer(int index, Rect row, bool dragging);

    /// <summary>
    /// A0+ · 列表重排原子族 —— 桥接原版通用重排引擎 <c>Verse.ReorderableWidget</c>。
    ///
    /// <para><b>第二轮取证结论</b>：原《原版 UI 代码标准》把「列表拖拽重排」判为 ❌（无通用原版）
    /// 是<b>误判</b> —— <c>ReorderableWidget</c> 是完整成件，且框架钩子
    /// （<c>ReorderableWidgetOnGUI_Before/AfterWindowStack</c>）已由 <c>RimWorld.UIRoot_Play</c>
    /// 每帧调用，<b>Mod 无需 Harmony</b>。</para>
    ///
    /// <para><b>白送的原版手感</b>：5px 起拖阈值、起拖/放下音效（<c>SoundDefOf.DragElement</c> /
    /// <c>DropElement</c>）、2px 白色插入线、被拖行高亮。</para>
    ///
    /// <para><b>三条用法铁律（照抄原版 <c>ColonistBar</c> 的 <c>cachedReorderableGroups</c> 范式）</b>：</para>
    /// <list type="number">
    /// <item><b>组 id 必须由调用方缓存</b>（本类的 <c>ref groupId</c>）：<c>NewGroup</c> 在非 Repaint 帧
    /// 返回 <c>-1</c>，直接用它去登记会让后续 MouseDown 落在无效组上，<b>拖拽永远起不来</b>；</item>
    /// <item><b>每帧都要重新登记</b>：框架在 <c>...AfterWindowStack</c> 里
    /// <c>groups.Clear() / reorderables.Clear()</c>，<b>上一帧的登记不会保留</b>；</item>
    /// <item><b>行数变化会中断拖拽</b>：<c>reorderables.Count != lastFrameReorderableCount</c> 时引擎自动
    /// <c>StopDragging()</c>，列表增删行时要有心理准备（或自行延后提交）。</item>
    /// </list>
    /// </summary>
    public static class ReorderAtoms
    {
        private static readonly Action<int, int> Noop = (a, b) => { };

        /// <summary>
        /// 垂直行列表重排。<paramref name="groupId"/> 为调用方持有的持久状态（字段/捕获变量），
        /// 首次传 0 即可。返回「本帧是否处于拖拽中」。
        /// </summary>
        /// <param name="area">整组命中区（原版按此矩形判定拖动开始位置）。</param>
        /// <param name="count">行数。</param>
        /// <param name="rowHeight">行高（像素）。</param>
        /// <param name="draw">行绘制回调（每行每帧调用一次）。</param>
        /// <param name="onReorder">提交回调 <c>(fromIndex, insertIndex)</c>；建议内部转交 <see cref="ReorderFmt.Move{T}"/>。</param>
        /// <param name="highlightDragged">是否由引擎绘制被拖行高亮（默认开）。</param>
        public static bool List(Rect area, int count, float rowHeight, ref int groupId, ReorderRowDrawer draw,
                                Action<int, int> onReorder = null, bool highlightDragged = true)
        {
            if (draw == null || count <= 0 || rowHeight <= 0f) return false;

            int g = ReorderableWidget.NewGroup(onReorder ?? Noop, ReorderableDirection.Vertical, area,
                                               -1f, null, true);
            if (g >= 0) groupId = g;                     // 铁律 1：仅 Repaint 帧刷新 id，非 Repaint 沿用旧值
            if (groupId < 0) groupId = 0;

            for (int i = 0; i < count; i++)
            {
                var row = new Rect(area.x, area.y + i * rowHeight, area.width, rowHeight);
                bool dragging = ReorderableWidget.Reorderable(groupId, row, false, highlightDragged);
                draw(i, row, dragging);
            }
            return ReorderableWidget.Dragging;
        }

        /// <summary>
        /// 行首握把（三点式）。命中区与行点击分离是原版 <c>GripHandle</c> 语义 —— 本原子只负责画出把手的
        /// <b>可拖视觉提示</b>，真正的命中判定由 <see cref="List"/> 的整组矩形完成。
        /// </summary>
        public static void Grip(Rect rect, bool dragging)
        {
            var c = dragging ? Tokens.Accent : Tokens.Dim;
            float cx = rect.center.x, cy = rect.center.y;
            for (int i = -1; i <= 1; i++)
            {
                Draw.Box(new Rect(cx - 1f, cy + i * 4f - 1f, 2f, 2f), c);
            }
        }
    }
}
