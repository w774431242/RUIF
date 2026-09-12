using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// L4 · 布局编排（S1 内核）。
    ///
    /// 立场：<b>即时模式 + 轻量布局助手</b>——不做保留模式虚拟树，不做自动 diff。
    /// 布局对象是 struct，按帧创建、零堆分配；窗口只声明结构，坐标由这里计算。
    ///
    /// 用法示例：
    /// <code>
    /// var col = Layout.Column(outer, gap: Tokens.Gap);
    /// Rect head = col.Take(30f); ...
    /// </code>
    /// </summary>
    public static class Layout
    {
        /// <summary>垂直栈：自上而下依次取行。</summary>
        public static VStack Column(Rect outer, float gap = 0f) => new VStack(outer, gap);

        /// <summary>水平行：自左向右依次取列。</summary>
        public static HStack Row(Rect outer, float gap = 0f) => new HStack(outer, gap);

        /// <summary>网格：固定列数，按序取格。</summary>
        public static Grid Grid(Rect outer, int cols, float cellHeight, float gap = 0f)
            => new Grid(outer, cols, cellHeight, gap);

        /// <summary>左右分栏（主从布局骨架）。ratio 为左侧占比。</summary>
        public static void Split(Rect outer, float ratio, float gap, out Rect left, out Rect right)
        {
            float lw = (outer.width - gap) * Mathf.Clamp01(ratio);
            left = new Rect(outer.x, outer.y, lw, outer.height);
            right = new Rect(outer.x + lw + gap, outer.y, Mathf.Max(0f, outer.width - lw - gap), outer.height);
        }

        /// <summary>
        /// 开始滚动视图；返回内容区坐标系（起点 0,0）。必须与 <see cref="EndScroll"/> 配对。
        /// showScrollbars=false 时可配合 MiscAtoms.ThinScrollBar 自绘细滚动条。
        /// </summary>
        public static Rect BeginScroll(Rect outer, float contentHeight, ref Vector2 scroll, bool showScrollbars = true)
        {
            Rect view = new Rect(0f, 0f, Mathf.Max(0f, outer.width - Tokens.ScrollbarThickness), contentHeight);
            Widgets.BeginScrollView(outer, ref scroll, view, showScrollbars);
            return view;
        }

        /// <summary>结束滚动视图。</summary>
        public static void EndScroll() => Widgets.EndScrollView();

        /// <summary>内缩（padding）。</summary>
        public static Rect Inset(Rect outer, float padX, float padY)
            => new Rect(outer.x + padX, outer.y + padY, Mathf.Max(0f, outer.width - padX * 2f), Mathf.Max(0f, outer.height - padY * 2f));
    }

    /// <summary>垂直栈游标。</summary>
    public struct VStack
    {
        private readonly float x, w, bottom, gap;
        private float y;

        public VStack(Rect outer, float gap)
        {
            x = outer.x; w = outer.width; y = outer.y; bottom = outer.yMax; this.gap = gap;
        }

        /// <summary>取一段高度；超出底部时自动裁剪，不溢出容器。</summary>
        public Rect Take(float height)
        {
            float room = Mathf.Max(0f, bottom - y);
            Rect r = new Rect(x, y, w, Mathf.Min(height, room));
            y += height + gap;
            return r;
        }

        /// <summary>剩余空间。</summary>
        public Rect Rest => new Rect(x, y, w, Mathf.Max(0f, bottom - y));

        /// <summary>当前游标位置（调试/叠加绘制用）。</summary>
        public float Y => y;
    }

    /// <summary>水平行游标。</summary>
    public struct HStack
    {
        private readonly float y, h, right, gap;
        private float x;

        public HStack(Rect outer, float gap)
        {
            x = outer.x; y = outer.y; h = outer.height; right = outer.xMax; this.gap = gap;
        }

        /// <summary>取一段宽度；超出右边界时自动裁剪。</summary>
        public Rect Take(float width)
        {
            float room = Mathf.Max(0f, right - x);
            Rect r = new Rect(x, y, Mathf.Min(width, room), h);
            x += width + gap;
            return r;
        }

        /// <summary>剩余空间。</summary>
        public Rect Rest => new Rect(x, y, Mathf.Max(0f, right - x), h);

        /// <summary>当前游标位置（调试/叠加绘制用）。</summary>
        public float X => x;
    }

    /// <summary>网格游标（固定列数）。</summary>
    public struct Grid
    {
        private readonly float x, y, w, cellH, gap;
        private readonly int cols;
        private int index;

        public Grid(Rect outer, int cols, float cellHeight, float gap)
        {
            x = outer.x; y = outer.y; w = outer.width;
            this.cols = Mathf.Max(1, cols); cellH = cellHeight; this.gap = gap; index = 0;
        }

        /// <summary>取下一格。</summary>
        public Rect Next()
        {
            float cw = (w - gap * (cols - 1)) / cols;
            int row = index / cols;
            int col = index % cols;
            index++;
            return new Rect(x + col * (cw + gap), y + row * (cellH + gap), Mathf.Max(0f, cw), cellH);
        }

        /// <summary>已取格数（调试用）。</summary>
        public int Index => index;
    }
}
