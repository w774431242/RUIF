using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// RUIF 组件验证台主窗口。
    ///
    /// <para><b>页签条刻意用原版 <c>Verse.TabRecord</c> 绘制</b>（而非自绘）——
    /// 这是第二轮取证里把「页签族」从 ⚠️ 升级为 ✅ 的那条依据的现场验证：
    /// <c>TabRecord.Draw(Rect)</c> 自带图集三段绘制、悬停黄色位移、选中态 1px 下划线、
    /// 以及 <c>UIScaling</c> 像素对齐。</para>
    ///
    /// <para><b>内容区</b>：每个验证面是一个独立小类，自带高度与绘制，
    /// 并统一由 <see cref="Layout.BeginScroll"/> 承载（列表可滚动）。</para>
    /// </summary>
    public class DemoWindow : Window
    {
        // 单页签：本窗口现在是「42 件基础原子」的专用验收面，与
        // docs/02-组件图鉴/原子组件演武场.html 逐件一一对应（同编号 01–42）。
        //
        // 其余验证面（组件画廊 / 贴图管线 / 拖拽重排 / 脉冲高亮 / 殖民者表 / 诊断）
        // 已从页签栏移除 —— 它们的 Bench 类**原样保留**，需要时把名字加回本数组即可恢复
        // （例：{ "42 原子", "诊断" }），不必改任何绘制代码。
        // 页签 = **8 个组件分类**（口径与《RUIF组件总目》docs/03-API与引擎取证 一致）
        //        + **3 个必须非滚动坐标的验证面**。
        //
        // ⚠ 为什么后三个不能并进分类页签：它们的验收对象是**原版控件的桥**
        //   （`ReorderableWidget` / `PawnTable` / `UIHighlighter`），而这些原版控件依赖
        //   「`UI.GUIToScreenPoint` 记录的矩形 + 未平移的事件坐标系」——一旦放进滚动视图，
        //   事件坐标被平移，插入位判定就整体错位。故 `IBench.Scrollable` 必须返回 false，
        //   也就无法与卡片体系（滚动）共处一个页签。
        private static readonly string[] TabLabels =
        {
            "原子", "输入", "数据", "图表", "反馈", "导航", "工业与官方", "桥接",
            "拖拽重排", "脉冲高亮", "殖民者表",
            // 1.3.0 互动组件族（按钮 / 选择件 / 骨架 / 降级阶梯）—— 引用 §3.4 冻结签名，
            // 由 BenchButtons 单页承载。追加在末尾 ⇒ 既有 11 个页签的下标不变。
            "互动",
            // ── L2 全量覆盖补全（2026-09）：覆盖《标准规范组件预览》剩余族，
            //    候选层 + 原生 Atoms 全部落地。追加在末尾 ⇒ 既有 12 个页签下标不变。
            //    实现见各 BenchXxx.cs（均为 Scrollable => true，走滚动视口路径）。
            "视觉扩展", "导航版式", "数据族", "输入族", "官方", "反馈族",
            // ── 2026-10-05 审计补回：三个面此前被移出页签栏，但类仍在包内 ⇒ 其覆盖的
            //    6 件 TexAtoms 贴图原子、17 个编译期类型断言、约 30 件组合范式全部**不可达**，
            //    构成"框架有、验证台看不到"的真实缺口。按 R2「恢复面只需把名字加回 TabLabels」
            //    的设计意图加回，代码零改动。追加在末尾 ⇒ 既有 18 个页签下标不变。
            "诊断", "贴图管线", "组件画廊",
            // ── 2026-10-05 审计新增：覆盖补全面。审计口径 173 个公开绘制方法 → 16 件从未被任何面调用，
            //    本面把它们（连同同族 2 件共 18 件）逐个画出来，缺口归零。见 BenchCoverage.cs 的类注释。
            "覆盖补全"
        };

        private static IBench[] MakeTabBenches()
        {
            var a = new IBench[TabLabels.Length];
            for (int i = 0; i < BenchAtoms42.AllTabs.Length; i++) a[i] = new BenchAtoms42 { OnlyTab = BenchAtoms42.AllTabs[i] };
            a[8] = new BenchReorder();       // 非滚动面 · 原版 ReorderableWidget
            a[9] = new BenchHighlight();     // 非滚动面 · 原版 UIHighlighter（两帧协议）
            a[10] = new BenchPawnTable();    // 非滚动面 · 原版 PawnTable
            a[11] = new BenchButtons();      // 滚动面 · 互动组件族（1.3.0 新增面）
            // ── L2 全量覆盖补全（2026-09）：标准规范组件预览剩余族，全部 Scrollable => true
            a[12] = new BenchVisualExtra();  // 视觉扩展原生 Atoms：Shapes/Misc/Progress 等 11 件
            a[13] = new BenchNavLayout();    // 导航与版式族：Nav + Layout 候选/原生 15 件
            a[14] = new BenchData();         // 数据展示族：候选 + 原生 Data/DataEx 27 件
            a[15] = new BenchInput();        // 输入交互族：候选 + 原生 Input/InputEx/InputPlus/Interact 29 件
            a[16] = new BenchOfficial();     // 官方数据候选族：OfficialData A/B/C 三系 14 件
            a[17] = new BenchFeedback();     // 反馈族：候选 + 原生 Feedback/FeedbackB 15 件
            // ── 2026-10-05 审计补回：三个原本被移出页签栏的面
            //   （原实现只在**字段**里 new 出来却从不渲染 —— 既掩盖了覆盖缺口，又是纯死负载）
            a[18] = new BenchDiagnostics();  // 诊断：17 个 typeof 编译期断言 + Px 像素对齐 A/B + 令牌色板
            a[19] = new BenchTextures();     // 贴图管线：TexAtoms 6 件（真渐变 / 真渐隐 / 热力格网 / 饼环）
            a[20] = new BenchGallery();      // 组件画廊：族级组合用法（文本/状态/进度/布局/数据/导航）
            // ── 2026-10-05 审计新增：覆盖补全面（18 件「框架有、从未被调用」的原子）
            a[21] = new BenchCoverage();
            return a;
        }

        private readonly List<TabRecord> tabs = new List<TabRecord>();
        /// <summary>每个页签一个实例：分类页签是 `BenchAtoms42`（只构建并渲染本类族），后三个是**非滚动面**。
        /// 空分类会在渲染层给「待落地」占位卡。</summary>
        private readonly IBench[] tabBenches = MakeTabBenches();

        private int tab;
        private Vector2 scroll;
        /// <summary>已记过异常的验证面（只报一次，避免每帧刷屏）。</summary>
        private readonly HashSet<string> reported = new HashSet<string>();
        /// <summary>当前已隔离的验证面名 —— 写进页脚，让「这面坏了」可见而非静默。</summary>
        private string faulted;

        /// <inheritdoc />
        public override Vector2 InitialSize => new Vector2(1040f, 720f);

        /// <summary>构造验证台窗口。</summary>
        public DemoWindow()
        {
            doCloseX = true;
            doCloseButton = false;
            resizeable = true;
            draggable = true;
            absorbInputAroundWindow = true;
            closeOnCancel = true;
            onlyOneOfTypeAllowed = true;
        }

        /// <inheritdoc />
        // ══════════════════════════════════════════════════════════════════
        // 页眉与页签栏度量（一律派生，禁散落魔数）
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// 页眉行高（30f = <c>Tokens.SectionTitleHeight</c>，与全框架标题条同档）。
        /// <para><b>为什么必须显式取高</b>：<c>HStack.Take(width)</c> 返回的高度是<b>容器满高</b>
        /// （<c>Layout.HStack.h = outer.height</c>）⇒ 直接拿它画 <c>MiddleLeft</c> 文本，
        /// 标题会落到容器垂直正中。必须先 <c>Layout.Column(inRect).Take(Height)</c> 切出真行。</para>
        /// </summary>
        private const float HeadH = Tokens.SectionTitleHeight;

        /// <summary>
        /// 单行页签的宽度下限（90f）：最长标签「工业与官方」5 字在 <c>GameFont.Small</c> 下约 75px，
        /// 而 <c>TabDrawer</c> 每两个页签重叠 10px（<c>TabHoriztonalOverlap</c>）。
        /// 低于此值即折行 —— 判定口径与 <c>TabDrawer.GetOverflowTabHeight</c> 同源。
        /// </summary>
        private const float MinTabWidth = 90f;

        /// <summary><c>TabDrawer</c> 的行推进量（31f，原版 <c>Verse.TabDrawer.DrawTabs(…, rows, …)</c> 内硬编码）。</summary>
        private const float TabRowStride = 31f;

        /// <summary>
        /// 页脚带高（20f）。<b>必须从内容区扣掉</b>：原实现把页脚 rect 放在
        /// <c>inRect.yMax − 2f</c>（高 18）配 <c>LowerLeft</c> ⇒ 文字基线落在窗口外，页脚永远看不见；
        /// 同时内容区也没为它让位。现在两者都按本常量对齐。
        /// </summary>
        private const float FootH = 20f;

        /// <summary>页签行数（1–3）。≥2 时由 <c>TabDrawer</c> 折行绘制，页签不会再被窗口右边界静默裁掉。</summary>
        private static int TabRowCount(int count, float width)
            => Mathf.Clamp(Mathf.CeilToInt(count * MinTabWidth / Mathf.Max(1f, width)), 1, 3);

        /// <inheritdoc />
        public override void DoWindowContents(Rect inRect)
        {
            // ── 页眉（显式 30px 行 + 行内二次分行，避免标题被画到容器垂直正中）
            var headCol = Layout.Column(inRect, 0f);
            var headRow = Layout.Row(headCol.Take(HeadH), Tokens.SpaceSm);
            Draw.Label(headRow.Take(Mathf.Max(120f, inRect.width - 150f)), "RUIF · 组件验证台",
                GameFont.Medium, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(headRow.Rest, "底栏按钮 / F10", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleRight);

            // ── 页签栏（原版 Verse.TabDrawer）—— 仅 2 个以上页签时才画（单页签画一条只有一项的栏是死 UI）
            float bodyTop = inRect.y + HeadH + Tokens.SpaceXs;
            if (TabLabels.Length > 1)
            {
                if (tabs.Count == 0)
                {
                    for (int i = 0; i < TabLabels.Length; i++)
                    {
                        int idx = i;
                        tabs.Add(new TabRecord(TabLabels[i], () => { tab = idx; }, () => tab == idx));
                    }
                }

                int rows = TabRowCount(tabs.Count, inRect.width);
                float tabTop = bodyTop;
                // TabDrawer 把页签画在传入矩形 y 的**上方 32px**（内部 rect.y -= 32）⇒ 传入的是「页签底边」
                float bed = tabTop + TabRowStride * (rows - 1) + TabDrawer.TabHeight;
                float tabAreaH = TabRowStride * (rows - 1) + TabDrawer.TabHeight;

                // 自绘整块页签底：原版多行版内部只给**最后一行**画 DrawMenuSection，首行以上会悬空。
                Widgets.DrawMenuSection(new Rect(inRect.x, tabTop, inRect.width, tabAreaH));

                // ⚠ 命中检测与 clickedAction **都在 TabDrawer 内部**
                //    （反编译 Verse.TabDrawer:142-161：Widgets.ButtonInvisible → clickedAction），
                //    调用方不得再补一次 ButtonInvisible —— 会双重命中、重复切换。
                // ⚠ TabDrawer 内部会设 Text.Font = Small、Text.Anchor = MiddleCenter，收尾只把 Anchor
                //    复位成 UpperLeft、**不复位 Font** ⇒ 调用前后自行保存/恢复，避免字体状态泄漏到内容区。
                GameFont prevFont = Text.Font;
                TextAnchor prevAnchor = Text.Anchor;
                TabDrawer.DrawTabs(new Rect(inRect.x, bed, inRect.width, TabDrawer.TabHeight), tabs, rows, inRect.width);
                Text.Font = prevFont;
                Text.Anchor = prevAnchor;

                bodyTop = bed + 1f + Tokens.SpaceXs;
            }

            // ── 内容区（先判空间：窗口被拖到极小时画占位而非硬挤 —— 往 0/负高矩形里画内容必被后续容器盖住）
            float bodyH = inRect.height - (bodyTop - inRect.y) - FootH;
            if (bodyH < Tokens.SectionTitleHeight)
            {
                Draw.Label(new Rect(inRect.x, bodyTop, inRect.width, Mathf.Max(0f, bodyH)),
                    "窗口过小 —— 拖动右下角放大（当前 " + Mathf.RoundToInt(inRect.width) + "×"
                    + Mathf.RoundToInt(inRect.height) + "）",
                    GameFont.Tiny, Tokens.Warn, TextAnchor.MiddleCenter);
                return;      // 此处尚未 BeginScroll ⇒ 直接返回不会破坏裁剪栈
            }

            var body = new Rect(inRect.x, bodyTop, inRect.width, bodyH);
            Draw.Box(body, Tokens.Window);
            Draw.Border(body, Tokens.BorderSoft);
            Rect inner = Layout.Inset(body, Tokens.PanelPadding, Tokens.PanelPadding);

            IBench bench = Current();
            float contentH = 0f;
            try { contentH = bench.Height(inner.width); }
            catch (Exception e) { ReportOnce(bench, e); }

            if (bench.Scrollable)
            {
                Rect view = Layout.BeginScroll(inner, contentH, ref scroll);
                try { if (view.width > 0f) bench.Render(view); }
                catch (Exception e) { ReportOnce(bench, e); }
                finally { Layout.EndScroll(); }   // ⚠️ 必须无条件出栈
            }
            else
            {
                // 非滚动面：原版 ReorderableWidget 用 UI.GUIToScreenPoint 记录行矩形做插入位判定，
                // 而滚动视图会平移 Event.current.mousePosition 的坐标系 —— 两者混用会让插入位整体偏移。
                try { bench.Render(inner); }
                catch (Exception e) { ReportOnce(bench, e); }
            }

            // ── 页脚：内容高与滚动位置（同时验证 BeginScroll 游标是否正常）
            Draw.Label(new Rect(inRect.x, inRect.yMax - FootH + 2f, inRect.width, FootH - 2f),
                       "内容高 " + Mathf.RoundToInt(contentH) + "px · 滚动 " + Mathf.RoundToInt(scroll.y) + "px" +
                       (faulted == null ? "" : "　⚠ " + faulted + " 已隔离（本次会话内不再重绘，详见日志）"),
                       GameFont.Tiny, faulted == null ? Tokens.Dim : Tokens.Dead, TextAnchor.LowerLeft);
        }

        /// <summary>
        /// 验证面级异常隔离：只记一次日志，并把「本面已坏」写进页脚，而不是每帧重抛。
        ///
        /// <para>代价说明：IMGUI 的 <c>BeginScroll</c>/<c>EndScroll</c> 与 GUIClip 都要求严格成对。
        /// 异常一旦逃出 <see cref="DoWindowContents"/>，出栈就断了，结果是
        /// 「You are pushing more GUIClips than you are popping」刷屏 + <b>整局 GUI 卡死（无法行动）</b>。
        /// 所以隔离必须做在窗口这一层，且出栈要放在 <c>finally</c>。</para>
        /// </summary>
        private void ReportOnce(IBench bench, Exception e)
        {
            string key = bench.GetType().Name;
            faulted = key;
            if (reported.Add(key))
            {
                Log.ErrorOnce("[RUIF_Demo] 验证面 " + key + " 抛异常，已隔离（其余页签不受影响）：\n" + e, key.GetHashCode());
            }
        }

        private IBench Current()
        {
            // 页签 = 组件分类；每个分类一个实例（只构建并渲染本类的族）。
            // 其余验证面（画廊 / 贴图 / 重排 / 高亮 / 殖民者表 / 诊断）的 Bench 类**原样保留**，
            // 需要时把名字加回 TabLabels 并在此补 case 即可恢复 —— 不必改任何绘制代码。
            return tabBenches[Mathf.Clamp(tab, 0, tabBenches.Length - 1)];
        }
    }

    /// <summary>验证面统一接口（自带高度与绘制）。</summary>
    public interface IBench
    {
        /// <summary>是否放进滚动视口。<b>用到原版 ReorderableWidget 的面必须返回 false</b>（坐标系前提）。</summary>
        bool Scrollable { get; }

        /// <summary>内容区所需高度（按可用宽度算）。</summary>
        float Height(float width);

        /// <summary>
        /// 绘制内容（<paramref name="area"/> 为可绘制视口）。
        /// <para>⚠️ 方法名刻意不叫 <c>Draw</c>：实例方法 <c>Draw(Rect)</c> 会在类内遮蔽静态类
        /// <c>RUIF.Draw</c>，导致 <c>Draw.Label(...)</c> 等全部报 CS0119。</para>
        /// </summary>
        void Render(Rect area);
    }
}
