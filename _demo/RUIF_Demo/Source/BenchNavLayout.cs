using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// 组件验证台 · 导航 / 版式布局域（实机渲染面）。
    ///
    /// <para>覆盖 15 件组件：</para>
    /// <list type="bullet">
    ///   <item><description>nav 候选层 <see cref="NavCandAtoms"/>（5）：MainTabStrip / InspectTabs / WizardSteps / SearchBox / PolicyList</description></item>
    ///   <item><description>nav 原生层 <see cref="NavAtoms"/>（4）：TabBar / Breadcrumb / Pager / CollapsibleTree</description></item>
    ///   <item><description>nav 原生层 B <see cref="NavAtomsB"/>（5）：TimeSpeedControl / LayerSwitch / JumpList / SidebarCollapse / FocusSearch</description></item>
    ///   <item><description>layout 候选层 <see cref="LayoutCandAtoms"/>（1）：CockpitFrame</description></item>
    /// </list>
    ///
    /// <para>说明：候选层组件（<c>NavCandAtoms</c> / <c>LayoutCandAtoms</c>）的官方签名直接接收
    /// <c>string[]</c> 与 <c>ref</c> 选中态——本域没有「<c>NavCandFmt.X()</c> 构建 cand」的环节
    /// （<c>NavCandFmt</c> 仅含配色 Tone 辅助函数，已在组件内部调用）。故此处按源码真实签名直传。</para>
    ///
    /// <para>高度由 <see cref="Height"/> 静态估算，与 <see cref="Render"/> 的垂直堆叠口径一致；
    /// 所有交互态以实例字段跨帧持有（<c>ref</c> 回调给组件），组件本身零状态。</para>
    /// </summary>
    internal sealed class BenchNavLayout : IBench
    {
        // ── 间距 / 尺寸基准（与 Render 内的 Take 完全一致，避免高宽漂移）────
        private const float Gap = Tokens.SpaceLg;     // 区块间距 24
        private const float InGap = Tokens.SpaceSm;   // 区块内行距 12
        private const float CapH = 16f;               // 每个组件上方签名说明行高

        private const float S1 = 462f;   // NavCandAtoms 区块外高（10 项：内容 330 + 9×InGap 108 + Frame 24 = 462，B-R11 按 VStack 每项后加 gap 语义重算）
        private const float S2 = 440f;   // NavAtoms 区块外高（8 项：内容 332 + 7×12 84 + 24 = 440）
        private const float S3 = 446f;   // NavAtomsB 区块外高（10 项：内容 314 + 108 + 24 = 446）
        private const float S4 = 280f;   // LayoutCandAtoms.CockpitFrame 区块外高（组件吃 Rest 自适应，留白安全）

        // ── 交互态（跨帧持有，组件零状态）────────────────────────────────
        private int mainTabSel;
        private int inspectSel;
        private int wizardStep;
        private string searchText = "";
        private int policySel;

        private int tabSel;
        private float tabIndicator;
        private int pagerPage;
        private readonly int[] pagerBuf = new int[64];
        private int breadcrumbJump = -1;
        private readonly TreeNode navTree = BuildTree();

        private TimeSpeed timeSpeed = TimeSpeed.Normal;
        private int layerSel;
        private int jumpHit = -1;
        private bool sidebarOpen = true;
        private string focusQuery = "";

        private int cockpitPanel;

        private static readonly NavAtomsB.RecentEntry[] recent = NewRecent();

        // ══════════════════════════════════════════════════════════════════
        // IBench
        // ══════════════════════════════════════════════════════════════════

        public bool Scrollable => true;

        public float Height(float width)
        {
            // 顶部说明行 + 4 个区块，区块间 Gap
            return 34f + Gap + S1 + Gap + S2 + Gap + S3 + Gap + S4;
        }

        public void Render(Rect area)
        {
            var col = Layout.Column(area, Gap);

            // ── 顶部说明行
            Draw.Label(col.Take(34f),
                "导航 / 版式布局域 · 15 件组件实机渲染（候选层 + 原生层），点按即交互",
                Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleLeft);

            // ── S1：导航候选层 NavCandAtoms
            RenderNavCand(col.Take(S1));

            // ── S2：导航原生层 NavAtoms
            RenderNavAtoms(col.Take(S2));

            // ── S3：导航原生层 B NavAtomsB
            RenderNavAtomsB(col.Take(S3));

            // ── S4：版式候选层 LayoutCandAtoms
            RenderLayoutCand(col.Take(S4));
        }

        // ══════════════════════════════════════════════════════════════════
        // S1 · NavCandAtoms（候选层，直传 string[] + ref 选中态）
        // ══════════════════════════════════════════════════════════════════

        private void RenderNavCand(Rect section)
        {
            Rect inner = Atoms.SectionFrame(section, "导航候选层 · NavCandAtoms（主标签页 / 检视页签 / 向导步骤 / 搜索框 / 策略列表）");
            var c = Layout.Column(inner, InGap);

            // 83-ish · MainTabStrip
            Draw.Label(c.Take(CapH), "NavCandAtoms.MainTabStrip(rect, labels, ref sel)", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavCandAtoms.MainTabStrip(c.Take(30f), new[] { "概览", "殖民者", "研究", "事件" }, ref mainTabSel);

            // InspectTabs
            Draw.Label(c.Take(CapH), "NavCandAtoms.InspectTabs(rect, labels, ref sel)", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavCandAtoms.InspectTabs(c.Take(26f), new[] { "属性", "装备", "技能", "记录" }, ref inspectSel);

            // WizardSteps
            Draw.Label(c.Take(CapH), "NavCandAtoms.WizardSteps(rect, steps, ref step)", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavCandAtoms.WizardSteps(c.Take(46f), new[] { "选地", "奠基", "殖民", "扩张" }, ref wizardStep);

            // SearchBox
            Draw.Label(c.Take(CapH), "NavCandAtoms.SearchBox(rect, ref text)", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavCandAtoms.SearchBox(c.Take(28f), ref searchText);

            // PolicyList
            Draw.Label(c.Take(CapH), "NavCandAtoms.PolicyList(rect, policies, ref sel) · sel=" + policySel, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavCandAtoms.PolicyList(c.Take(120f), new[] { "默认政策", "战时配给", "和平发展", "封闭自守" }, ref policySel);
        }

        // ══════════════════════════════════════════════════════════════════
        // S2 · NavAtoms（原生层，直传 Rect / 回调）
        // ══════════════════════════════════════════════════════════════════

        private void RenderNavAtoms(Rect section)
        {
            Rect inner = Atoms.SectionFrame(section, "导航原生层 · NavAtoms（TabBar / Breadcrumb / Pager / CollapsibleTree）");
            var c = Layout.Column(inner, InGap);

            // TabBar（indicatorX 由调用方持有，组件内部插值）
            Draw.Label(c.Take(CapH), "NavAtoms.TabBar(rect, tabs, ref sel, ref indicatorX)", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavAtoms.TabBar(c.Take(32f), new[] { "概览", "殖民者", "资源", "电力", "日志" }, ref tabSel, ref tabIndicator);

            // Breadcrumb（回调 onJump 在组件内命中）
            Draw.Label(c.Take(CapH), "NavAtoms.Breadcrumb(rect, path, onJump) · 上次跳转=" + breadcrumbJump, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavAtoms.Breadcrumb(c.Take(28f), new[] { "殖民地", "存储区", "钢铁" }, idx => breadcrumbJump = idx);

            // Pager（pageBuf 复用缓冲，零分配）
            Draw.Label(c.Take(CapH), "NavAtoms.Pager(rect, total, pageSize, ref page, pageBuf)", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavAtoms.Pager(c.Take(28f), 12, 5, ref pagerPage, pagerBuf);

            // CollapsibleTree（Expanded / Dead 等状态挂在 TreeNode，跨帧保留）
            Draw.Label(c.Take(CapH), "NavAtoms.CollapsibleTree(rect, root) · 展开态在节点上", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavAtoms.CollapsibleTree(c.Take(180f), navTree);
        }

        // ══════════════════════════════════════════════════════════════════
        // S3 · NavAtomsB（原生层 B）
        // ══════════════════════════════════════════════════════════════════

        private void RenderNavAtomsB(Rect section)
        {
            Rect inner = Atoms.SectionFrame(section, "导航原生层 B · NavAtomsB（TimeSpeedControl / LayerSwitch / JumpList / SidebarCollapse / FocusSearch）");
            var c = Layout.Column(inner, InGap);

            // TimeSpeedControl（ref TimeSpeed）
            Draw.Label(c.Take(CapH), "NavAtomsB.TimeSpeedControl(rect, ref speed) · " + timeSpeed, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavAtomsB.TimeSpeedControl(c.Take(32f), ref timeSpeed);

            // LayerSwitch
            Draw.Label(c.Take(CapH), "NavAtomsB.LayerSwitch(rect, layers, ref sel)", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavAtomsB.LayerSwitch(c.Take(30f), new[] { "地图", "世界", "档案" }, ref layerSel);

            // JumpList（返回点击索引）
            Draw.Label(c.Take(CapH), "NavAtomsB.JumpList(rect, recent, max) · 命中=" + jumpHit, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            jumpHit = NavAtomsB.JumpList(c.Take(100f), recent, 8);

            // SidebarCollapse（窄把手：edge 矩形内绘制，ref open）
            Draw.Label(c.Take(CapH), "NavAtomsB.SidebarCollapse(edge, ref open) · open=" + sidebarOpen, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            Rect row = c.Take(44f);
            var edge = new Rect(row.x, row.y, 34f, row.height);
            NavAtomsB.SidebarCollapse(edge, ref sidebarOpen);

            // FocusSearch（ref query；本窗口聚焦时 Ctrl+F 聚焦、Esc 归还）
            Draw.Label(c.Take(CapH), "NavAtomsB.FocusSearch(rect, ref query)", Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            NavAtomsB.FocusSearch(c.Take(28f), ref focusQuery);
        }

        // ══════════════════════════════════════════════════════════════════
        // S4 · LayoutCandAtoms（版式候选层）
        // ══════════════════════════════════════════════════════════════════

        private void RenderLayoutCand(Rect section)
        {
            Rect inner = Atoms.SectionFrame(section, "版式候选层 · LayoutCandAtoms.CockpitFrame（分层钻取孪生驾驶舱外壳）");
            var c = Layout.Column(inner, InGap);
            Draw.Label(c.Take(CapH), "LayoutCandAtoms.CockpitFrame(rect, kpis, panels, ref activePanel) · active=" + cockpitPanel, Tokens.FontLabel, Tokens.Muted, TextAnchor.MiddleLeft);
            LayoutCandAtoms.CockpitFrame(c.Rest,
                new[] { "容量 82%", "负载 1.4k", "在线 12" },
                new[] { "概览", "电网", "仓储", "世界" },
                ref cockpitPanel);
        }

        // ── 样例数据构造 ────────────────────────────────────────────────

        private static TreeNode BuildTree()
        {
            var root = new TreeNode("殖民者") { Tag = "12" };
            var a = root.Add(new TreeNode("蓝 雪牛") { Tag = "3" });
            a.Add(new TreeNode("钢铁长剑") { Tag = "装备" });
            a.Add(new TreeNode("射击 14") { Tag = "技能" });
            root.Add(new TreeNode("赤 眼镜蛇") { Tag = "2" });
            var dead = root.Add(new TreeNode("阵亡者") { Tag = "5491", Dead = true, Expanded = false });
            return root;
        }

        private static NavAtomsB.RecentEntry[] NewRecent()
        {
            return new[]
            {
                new NavAtomsB.RecentEntry("蓝 雪牛", "人", 60f),
                new NavAtomsB.RecentEntry("钢铁长剑", "装", 300f),
                new NavAtomsB.RecentEntry("研究台", "建", 1200f),
                new NavAtomsB.RecentEntry("商队事件", "役", 3600f),
            };
        }
    }
}
