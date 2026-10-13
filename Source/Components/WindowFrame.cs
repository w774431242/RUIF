using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// 窗口骨架的<b>度量常量表</b>（方案 §5.1 / §5.2）。
    ///
    /// <para><b>基准裁决（§5.1）</b>：采用 <c>Dialog_MessageBox</c> 范式为<b>唯一</b>底栏基准 ——
    /// 它是 RimWorld 绝大多数对话框的共同父范式，用户肌肉记忆即「确认在最右」；
    /// <c>Window.doCloseButton</c> 的 <c>h−55 / 120×40</c> <b>不作并列基准</b>（否则同一窗口出现两套节奏），
    /// 仅把「定宽居中」并入为 <c>n == 1</c> 特例。</para>
    ///
    /// <para><b>原版实测（1.6 反编译逐行核对）</b>：<c>Verse.Dialog_MessageBox.DoWindowContents</c> 尾段 ——
    /// <c>num4 = buttonCText.NullOrEmpty() ? 2 : 3</c> · <c>num5 = inRect.width / num4</c> ·
    /// <c>width2 = num5 − 10f</c> · 主按钮 <c>x = num5 * (num4 − 1) + 10f</c>、<c>y = inRect.height − 35f</c>、
    /// 高 <c>35f</c> ⇒ <b>主按钮最右且右缘贴内容区右缘</b>。本表即该范式的令牌化复现。</para>
    ///
    /// <para><b>与 <see cref="WindowFrame"/> 的分工</b>：本类是<b>纯常量 + 纯几何</b>（可离线单测）；
    /// <see cref="WindowFrame"/> 只负责把 chrome 画出来。</para>
    /// </summary>
    public static class WindowFmt
    {
        // ------------------------------------------------------------ 常量（§5.2 全表）

        /// <summary>标题区高度（42f = <c>Dialog_MessageBox.TitleHeight</c>）。</summary>
        public const float TitleHeight = 42f;

        /// <summary>内容区水平内边距（18f = <c>Verse.Window.StandardMargin</c>）。</summary>
        public const float ContentInset = 18f;

        /// <summary>底栏带高（35f = <c>Dialog_MessageBox</c> 底带 = <c>MainButtonDef.ButtonHeight</c>）。</summary>
        public const float FooterHeight = 35f;

        /// <summary>底栏按钮高（35f，与带高同值）。</summary>
        public const float FooterButtonHeight = 35f;

        /// <summary>底栏水平内缩（10f = <c>Dialog_MessageBox</c>）。</summary>
        public const float FooterInsetX = 10f;

        /// <summary>底栏按钮间距（<b>0f，不加</b>）—— 间距由 <c>W/n − 10f</c> 的余量形成，<b>非叠加</b>。</summary>
        public const float FooterGap = 0f;

        /// <summary>按钮内容内边距（8f = <c>Tokens.SpaceXs</c> 派生，仅按钮内部）。</summary>
        public const float ButtonPadX = 8f;

        /// <summary>图标与文字间距（6f = <c>Tokens.SpaceXxs</c> 派生，仅按钮内部）。</summary>
        public const float ButtonGapInner = 6f;

        /// <summary>底栏按钮最小可用宽（80f，§5.6 降级触发阈值）。</summary>
        public const float MinButtonWidth = 80f;

        /// <summary>焦点环外扩（2f；与既有 <c>InputAtoms.StateSample</c> 逐值一致，§5.2 / §5.5）。</summary>
        public const float FocusRingOutset = 2f;

        /// <summary>分隔线粗细（1f = <c>Tokens.RuleHeight</c>；绘制时须再过 <c>Px.HLine</c>）。</summary>
        public const float RuleHeight = Tokens.RuleHeight;

        /// <summary>单按钮特例尺寸（120×32 = <c>Dialog_Confirm.ButtonSize</c>；<c>n == 1</c> 时居中回落）。</summary>
        public const float SingleButtonWidth = 120f;

        /// <summary>单按钮特例高度（32f = <c>Dialog_Confirm.ButtonSize</c>）。</summary>
        public const float SingleButtonHeight = 32f;

        /// <summary>标题字体档（<c>Tokens.FontValue</c> = Medium；原版标题区即 Medium）。</summary>
        public static GameFont TitleFont => Tokens.FontValue;

        // ------------------------------------------------------------ 纯几何

        /// <summary>标题条矩形（贴顶，满宽）。</summary>
        public static Rect TitleRect(Rect rect) => new Rect(rect.x, rect.y, rect.width, TitleHeight);

        /// <summary>底栏带矩形（贴底，满宽）—— 按钮几何一律以本矩形为基准。</summary>
        public static Rect FooterRect(Rect rect)
            => new Rect(rect.x, rect.yMax - FooterHeight, rect.width, FooterHeight);

        /// <summary>
        /// 内容区矩形：标题条之下、底栏带之上，左右各内缩 <see cref="ContentInset"/>。
        /// <para>高度不足时返回 0 高矩形（<b>不外溢</b>），不抛异常。</para>
        /// </summary>
        public static Rect ContentRect(Rect rect)
        {
            float top = rect.y + TitleHeight + RuleHeight;
            float bottom = rect.yMax - FooterHeight - RuleHeight;
            return new Rect(rect.x + ContentInset, top, Mathf.Max(0f, rect.width - ContentInset * 2f),
                Mathf.Max(0f, bottom - top));
        }

        /// <summary>
        /// 底栏第 <paramref name="i"/> 槽的起点 x（槽宽 = <c>W/n</c>，<b>不含</b> <see cref="FooterInsetX"/>）。
        /// </summary>
        public static float FooterSlotX(Rect footerRect, int n, int i)
        {
            if (n <= 0) return footerRect.x;
            return footerRect.x + footerRect.width / n * Mathf.Clamp(i, 0, n - 1);
        }

        /// <summary>
        /// 底栏按钮宽（<c>W/n − 10f</c>，§5.2 冻结式）。
        /// </summary>
        /// <param name="footerWidth">底栏带宽度（= 内容区宽 <c>W</c>）。</param>
        /// <param name="n">按钮数。</param>
        public static float FooterButtonWidth(float footerWidth, int n)
            => n <= 0 ? 0f : footerWidth / n - FooterInsetX;

        /// <summary>
        /// 底栏按钮矩形（<c>n ≥ 2</c> 的常规路径）。
        ///
        /// <para><b>几何</b>：<c>x = 带左缘 + i·(W/n) + 10f</c>、<c>w = W/n − 10f</c>、<c>h = 35f</c>。
        /// 由此得到三条观感：① <b>主按钮（末位）右缘贴带右缘</b>（右边距 0）；
        /// ② <b>相邻按钮间隙 = <see cref="FooterInsetX"/>（10f）</b>，左边距同为 10f；
        /// ③ 按钮宽逐值等于冻结式 <c>W/n − 10f</c>。</para>
        ///
        /// <para><b>⚠ 与原生的一处有意差异（已登记 · 方案 §5.6 A2 裁定）</b>：原版把 <c>+10f</c> <b>只加在主按钮</b>上
        /// （B / C 落槽起点）⇒ 左边距 0，且相邻间隙<b>不统一</b>（一般 10f、主按钮前 20f）；本实现<b>对称加在每槽</b>
        /// ⇒ 左边距 10f、间隙统一 10f、<b>主按钮右缘仍贴带右缘</b>（该特性保留），整体更规整。</para>
        /// </summary>
        /// <param name="footerRect">底栏带矩形（<see cref="FooterRect"/>）。</param>
        /// <param name="n">按钮总数（≤1 请走 <see cref="SingleButtonRect"/>）。</param>
        /// <param name="i">槽位下标，<b>0 = 最左</b>；主按钮取 <c>n − 1</c>（最右）。</param>
        public static Rect FooterButtonRect(Rect footerRect, int n, int i)
            => new Rect(FooterSlotX(footerRect, n, i) + FooterInsetX, footerRect.y,
                Mathf.Max(0f, FooterButtonWidth(footerRect.width, n)), FooterButtonHeight);

        /// <summary>
        /// <c>n == 1</c> 特例：<b>120×32 居中</b>（§5.1 裁决 ③ —— <c>W/1 − 10f</c> 在单按钮时近满宽，
        /// 与「单一确定 / 关闭」观感不符）。
        /// </summary>
        public static Rect SingleButtonRect(Rect footerRect)
            => new Rect(footerRect.center.x - SingleButtonWidth * 0.5f,
                footerRect.center.y - SingleButtonHeight * 0.5f,
                SingleButtonWidth, SingleButtonHeight);

        /// <summary>
        /// 底栏是否必须换档为 <b>D4 垂直堆叠</b>（§5.6）：单行等分已不足 <see cref="MinButtonWidth"/>。
        /// </summary>
        public static bool NeedsStack(float footerWidth, int n) => FooterButtonWidth(footerWidth, n) < MinButtonWidth;

        /// <summary>D4 堆叠区总高（<c>n × 35f</c>，从内容区扣）。</summary>
        public static float StackedHeight(int n) => FooterHeight * Mathf.Max(0, n);

        /// <summary>
        /// D4 堆叠区中第 <paramref name="i"/> 个按钮矩形（每行<b>全宽</b>钮，自上而下，行距 0）。
        /// <para>⛔ 禁止带内换行、禁止压带高（35f 是度量，压到 22f 必切字）。</para>
        /// </summary>
        public static Rect StackedButtonRect(Rect stackRect, int n, int i)
            => new Rect(stackRect.x, stackRect.y + FooterHeight * Mathf.Clamp(i, 0, Mathf.Max(0, n - 1)),
                stackRect.width, FooterHeight);
    }

    /// <summary>
    /// B2 · 窗口骨架（只有 chrome 的纯 <c>Rect</c> 骨架；方案 §5.1 / §5.2 / §5.4）。
    ///
    /// <para><b>职责边界（红线）</b>：<b>只画 chrome</b>（标题条 / 分隔线 / 底栏槽），交出
    /// <c>contentRect</c> 与 <c>footerRect</c> 由调用方自行填内容与按钮。
    /// <b>不触 <c>WindowStack</c>、不持窗口生命周期、不引用 <c>Verse.Window</c> 类型</b>
    /// ⇒ 组件本身<b>不触发</b>任何交互（悬停反馈归宿主按钮；标题条不响应）。</para>
    ///
    /// <para><b>背景归属</b>：本组件<b>不画窗口底 / 不画外框</b> —— 宿主（如 <c>Verse.Window.doWindowBackground</c>）
    /// 拥有底色与外框；本组件只叠加标题条与两条分隔线，避免与宿主背景形成「双线」。</para>
    ///
    /// <para><b>零状态</b>：无任何字段，无 <c>static</c> 可变状态。</para>
    /// </summary>
    public static class WindowFrame
    {
        /// <summary>
        /// 画骨架 chrome（标题条 + 标题下分隔线 + 底栏上分隔线），交出内容区与底栏槽。
        /// <para>底栏按钮由调用方按 <see cref="WindowFmt.FooterButtonRect"/> 逐个绘制
        /// （主按钮取<b>末位</b> = 最右）；单按钮请用 <see cref="WindowFmt.SingleButtonRect"/>。</para>
        /// </summary>
        /// <param name="rect">骨架外框（通常 = 窗口的 <c>DoWindowContents</c> 入参）。</param>
        /// <param name="title">标题文案（调用方自行 <c>Tr.Get</c>；<c>null</c> 时不画标题但保留标题条高）。</param>
        /// <param name="contentRect">内容区（标题下、底栏上，左右内缩 <see cref="WindowFmt.ContentInset"/>）。</param>
        /// <param name="footerRect">底栏带（贴底 35f 满宽；按钮几何以它为基准）。</param>
        public static void Draw(Rect rect, string title, out Rect contentRect, out Rect footerRect)
        {
            footerRect = WindowFmt.FooterRect(rect);
            contentRect = WindowFmt.ContentRect(rect);
            DrawTitle(rect, title);
            DrawRule(rect.x, rect.y + WindowFmt.TitleHeight, rect.width);   // 标题下分隔线
            DrawRule(rect.x, footerRect.y, rect.width);                    // 底栏槽上缘分隔线
        }

        /// <summary>
        /// 画骨架 chrome 并按按钮数自动走 §5.6 降级：<c>W/n − 10f ≥ 80f</c> 走底栏带（D0），
        /// 否则走 <b>D4 垂直堆叠</b>（每行全宽钮，<c>n × 35f</c> <b>从内容区扣</b>）。
        /// </summary>
        /// <param name="rect">骨架外框。</param>
        /// <param name="title">标题文案。</param>
        /// <param name="footerButtons">底栏按钮数（&lt;1 时按 1 处理）。</param>
        /// <param name="contentRect">内容区（D4 时高度已扣除堆叠区）。</param>
        /// <param name="footerRect">底栏带（D0）或堆叠区（D4）；按钮矩形由调用方自行切分。</param>
        /// <remarks>
        /// <b>D5 的归属</b>：堆叠总高仍超内容区时，本方法把堆叠区<b>夹到内容区高</b>（绝不画到容器外），
        /// 但「<c>BeginClip/EndClip</c> 成对 + 查看全部途径」须由<b>消费方</b>在绘制按钮时自行包裹 ——
        /// 方案 §5.8 未给该入口分配冻结键（新增键上限 ≤4），故本批不内置文案入口。
        /// </remarks>
        public static void Draw(Rect rect, string title, int footerButtons, out Rect contentRect, out Rect footerRect)
        {
            int n = Mathf.Max(1, footerButtons);
            footerRect = WindowFmt.FooterRect(rect);
            contentRect = WindowFmt.ContentRect(rect);

            if (WindowFmt.NeedsStack(footerRect.width, n))
            {
                // D4：底栏带 → 垂直堆叠（从内容区底部扣；总高超出内容区时夹到内容区高，绝不画到容器外）
                float h = Mathf.Min(WindowFmt.StackedHeight(n), contentRect.height);
                footerRect = new Rect(contentRect.x, contentRect.yMax - h, contentRect.width, h);
                contentRect = new Rect(contentRect.x, contentRect.y, contentRect.width,
                    Mathf.Max(0f, contentRect.height - h));
            }

            DrawTitle(rect, title);
            DrawRule(rect.x, rect.y + WindowFmt.TitleHeight, rect.width);   // 标题下分隔线
            DrawRule(rect.x, footerRect.y, rect.width);                    // 底栏槽 / 堆叠区上缘分隔线
        }

        /// <summary>
        /// <c>n == 1</c> 特例按钮的默认文案：调用方 <paramref name="label"/> 为 <c>null</c> 时回退
        /// <c>RUIF.Comp.Window.Close</c>（§5.8 键 <c>RUIF.Comp.Window.Close</c> 的唯一活跃调用点；
        /// <b>组件内不内置英文默认串</b>）。
        /// </summary>
        public static string DefaultCloseLabel(string label) => label ?? Tr.Get("RUIF.Comp.Window.Close");

        // ------------------------------------------------------------ 内部

        private static void DrawTitle(Rect rect, string title)
        {
            Rect bar = WindowFmt.TitleRect(rect);
            RUIF.Draw.Box(bar, Tokens.Panel);
            RUIF.Draw.Label(new Rect(bar.x + WindowFmt.ContentInset, bar.y,
                    Mathf.Max(0f, bar.width - WindowFmt.ContentInset * 2f), bar.height),
                title ?? string.Empty, WindowFmt.TitleFont, Tokens.Text, TextAnchor.MiddleLeft);
        }

        /// <summary>1px 分隔线：必须过 <see cref="Px.HLine"/>（UI 缩放 &gt;1 时不过 Px 会糊成 2px 灰边）。</summary>
        private static void DrawRule(float x, float y, float width)
            => RUIF.Draw.Box(Px.HLine(x, y, width), Tokens.BorderSoft);
    }
}
