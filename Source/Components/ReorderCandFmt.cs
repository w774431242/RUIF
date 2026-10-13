using System;
using UnityEngine;

namespace RUIF
{
    /// <summary>
    /// ⑥ 列表重排候选层（参数对象 struct + Build，纯增量）：桥接既有 <see cref="ReorderAtoms"/>。
    /// 仅打包「值参数」；跨帧状态持有者（groupId）按本工程 <c>LangVersion=9.0</c> 约束，
    /// 以 <c>ref</c> 形式走 <see cref="ReorderCandAtoms.List"/> 入口（见该文件说明），不进 struct 字段。
    /// 候选 readonly struct 已迁移至 <see cref="ReorderCandAtoms"/> 内嵌套定义（对齐 DataCandAtoms.BillEntry / StatusCandAtoms.StatusPulseDot 写法，
    /// 并以族前缀 <c>ReorderList</c> / <c>ReorderGrip</c> 规避与同名绘制方法的 C# 命名冲突）；
    /// 本文件同构 StatusCandFmt——<b>只放纯计算 Build 工厂、无 struct 定义</b>（R3）。绘制转发在 <see cref="ReorderCandAtoms"/>，零绘制逻辑（GATE-ARCH）。
    /// </summary>
    public static class ReorderCandFmt
    {
        /// <summary>垂直行列表重排候选：返回 <see cref="ReorderCandAtoms.ReorderList"/>；groupId 不在包内，由入口 ref 承载（R8）。</summary>
        public static ReorderCandAtoms.ReorderList List(int count, float rowHeight, ReorderRowDrawer draw,
            Action<int, int> onReorder = null, bool highlightDragged = true)
            => new ReorderCandAtoms.ReorderList(count, rowHeight, draw, onReorder, highlightDragged);

        /// <summary>行首握把候选：返回 <see cref="ReorderCandAtoms.ReorderGrip"/>；color 默认 dragging?Accent:Dim（溯源 Tokens，禁散落魔法数）。</summary>
        public static ReorderCandAtoms.ReorderGrip Grip(bool dragging, Color? color = null)
            => new ReorderCandAtoms.ReorderGrip(dragging, color ?? (dragging ? Tokens.Accent : Tokens.Dim));
    }
}
