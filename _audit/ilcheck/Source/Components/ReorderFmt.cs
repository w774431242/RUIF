using System.Collections.Generic;

namespace RUIF
{
    /// <summary>
    /// A0+ · 重排的<b>纯计算层</b>（可离线单测，不碰 GUI）。
    ///
    /// <para><b>为什么单独抽出来</b>：原版 <c>ReorderableWidget</c> 回调给的是
    /// <c>(fromIndex, insertIndex)</c>，其中 <c>insertIndex</c> 是<b>「元素移走之前」的插入下标</b>
    /// —— 直接 <c>RemoveAt(from); Insert(to, item)</c> 在 <c>to &gt; from</c> 时会偏一位，
    /// 是这类交互最经典的一处差一错误。把它固化成纯函数 + 单测，比在每个调用点重推一遍可靠。</para>
    ///
    /// <para><b>回调语义（与原版实测一致）</b>：<c>ReorderableWidget</c> 在
    /// <c>to == from</c> 或 <c>to == from + 1</c>（插回原位）时<b>不会</b>触发回调，本层同样提前返回。</para>
    /// </summary>
    public static class ReorderFmt
    {
        /// <summary>元素最终落在的下标（<paramref name="insertIndex"/> 为移走前的插入位）。</summary>
        public static int LandingIndex(int fromIndex, int insertIndex)
            => insertIndex > fromIndex ? insertIndex - 1 : insertIndex;

        /// <summary>这次回调是否真的会改变顺序（插回原位返回 false，与原版回调门槛一致）。</summary>
        public static bool WillChange(int fromIndex, int insertIndex)
            => insertIndex != fromIndex && insertIndex != fromIndex + 1;

        /// <summary>
        /// 原地重排：把 <paramref name="fromIndex"/> 处的元素移到 <paramref name="insertIndex"/>。
        /// 越界 / 插回原位一律静默返回（交互层不该因边界抛异常中断整帧绘制）。
        /// </summary>
        public static void Move<T>(IList<T> list, int fromIndex, int insertIndex)
        {
            if (list == null) return;
            int n = list.Count;
            if (fromIndex < 0 || fromIndex >= n) return;
            if (insertIndex < 0 || insertIndex > n) return;
            if (!WillChange(fromIndex, insertIndex)) return;

            T item = list[fromIndex];
            list.RemoveAt(fromIndex);
            int land = LandingIndex(fromIndex, insertIndex);
            if (land < 0) land = 0;
            if (land > list.Count) land = list.Count;
            list.Insert(land, item);
        }

        /// <summary>索引顺序数组的重排版本（返回新数组，不改原数组）。</summary>
        public static int[] Moved(int[] order, int fromIndex, int insertIndex)
        {
            if (order == null) return null;
            var copy = (int[])order.Clone();
            Move(copy, fromIndex, insertIndex);
            return copy;
        }
    }
}
