using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// 验证台的<b>标题界面入口</b>（Mod 设置页）。
    ///
    /// <para><b>为什么需要它（实证）</b>：底栏按钮走 <c>RimWorld.UIRoot_Play</c> 的
    /// <c>mainButtonsRoot.MainButtonsOnGUI()</c>，热键走同处的 <c>HandleLowPriorityShortcuts()</c> ——
    /// 而标题界面是 <c>Verse.UIRoot_Entry</c>，它<b>完全不引用</b> <c>MainButtonsRoot</c> /
    /// <c>MainButtonDef</c>（只做 <c>DoMainMenu()</c> + <c>WindowStackOnGUI()</c>）。
    /// 结论：<b>底栏按钮与 F9 在标题界面都不会出现</b>，<c>validWithoutMap</c> 管的是
    /// "没有当前地图时是否禁用"（星球视图等），与主菜单无关。</para>
    ///
    /// <para>而 Mod 设置页走 <c>Find.WindowStack</c>，在 Entry 场景同样可用 ⇒ 这是开档前
    /// 唯一的正规入口。<b>零 Harmony</b>：<c>Verse.Mod</c> 是原版公开扩展点。</para>
    /// </summary>
    public class DemoMod : Mod
    {
        /// <summary>构造（RimWorld 通过 <c>ModContentPack</c> 反射实例化）。</summary>
        public DemoMod(ModContentPack content) : base(content)
        {
        }

        /// <inheritdoc />
        public override string SettingsCategory() => "RUIF 组件验证台";

        /// <inheritdoc />
        public override void DoSettingsWindowContents(Rect inRect)
        {
            var col = Layout.Column(inRect, Tokens.SpaceMd);

            Rect head = col.Take(46f);
            Draw.Label(head, "RUIF · 组件验证台", GameFont.Medium, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(new Rect(head.x, head.yMax - 14f, head.width, 16f),
                "开发期实机验证工具（不随框架发布）", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

            Rect info = col.Take(96f);
            var ic = Layout.Column(Atoms.SectionFrame(info, "打开方式"), 3f);
            Draw.Label(ic.Take(18f), "① 进游戏后：底栏「组件验证台」按钮，或按 F9", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(ic.Take(18f), "② 任意时刻（含标题界面）：本页下方按钮", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(ic.Take(18f), "底栏与 F9 由 UIRoot_Play 提供，标题界面是 UIRoot_Entry —— 那里不画底栏，属原版结构。",
                GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

            Rect btnRow = col.Take(34f);
            var row = Layout.Row(btnRow, Tokens.SpaceSm);
            if (Widgets.ButtonText(row.Take(180f), "打开组件验证台"))
            {
                DemoWindow existing = Find.WindowStack.WindowOfType<DemoWindow>();
                if (existing != null) Find.WindowStack.TryRemove(existing, doCloseSound: false);
                else Find.WindowStack.Add(new DemoWindow());
            }
            Draw.Label(row.Rest, "验证台窗口可在标题界面直接打开（WindowStack 在 Entry 场景同样可用）",
                GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

            Rect note = col.Take(72f);
            Rect nb = Atoms.SectionFrame(note, "六个验证面");
            var ncol = Layout.Column(nb, 3f);
            Draw.EllipsisLabel(ncol.Take(18f),
                "组件画廊 · 贴图管线 · 拖拽重排 · 脉冲高亮 · 殖民者表 · 诊断", GameFont.Tiny, Tokens.Info, TextAnchor.MiddleLeft);
            Draw.EllipsisLabel(ncol.Take(18f),
                "改动 RUIF 后重跑：dotnet build Source/RUIF.csproj && pwsh _scripts/sync_ruif.ps1",
                GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
        }
    }
}
