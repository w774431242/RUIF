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
        private static readonly string[] TabLabels = { "42 原子" };

        private readonly List<TabRecord> tabs = new List<TabRecord>();
        private readonly BenchDiagnostics diag = new BenchDiagnostics();
        private readonly BenchTextures tex = new BenchTextures();
        private readonly BenchReorder reorder = new BenchReorder();
        private readonly BenchGallery gallery = new BenchGallery();
        private readonly BenchHighlight hilt = new BenchHighlight();
        private readonly BenchPawnTable pawnTable = new BenchPawnTable();
        private readonly BenchAtoms42 atoms42 = new BenchAtoms42();

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
        public override void DoWindowContents(Rect inRect)
        {
            // ── 标题
            var head = Layout.Row(inRect, Tokens.SpaceSm);
            Rect titleRect = head.Take(inRect.width - 160f);
            Draw.Label(titleRect, "RUIF · 组件验证台", GameFont.Medium, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.Label(head.Rest, "底栏按钮 / F9", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleRight);

            // ── 页签条（原版 TabRecord）——仅有 2 个以上页签时才绘制：
            //    单页签时画一条只有一项的页签栏属于死 UI。恢复页签只需往 TabLabels 加名字，栏会自动回来。
            float bodyTop;
            if (TabLabels.Length > 1)
            {
                var bar = new Rect(inRect.x, inRect.y + 30f, inRect.width, 32f);
                Widgets.DrawMenuSection(bar);
                if (tabs.Count == 0)
                {
                    for (int i = 0; i < TabLabels.Length; i++)
                    {
                        int idx = i;
                        tabs.Add(new TabRecord(TabLabels[i], () => { tab = idx; }, () => tab == idx));
                    }
                }
                float tx = bar.x + 2f;
                Text.Font = GameFont.Small;
                for (int i = 0; i < tabs.Count; i++)
                {
                    float tw = Text.CalcSize(TabLabels[i]).x + 26f;
                    var tr = new Rect(tx, bar.y, tw, bar.height);
                    tabs[i].Draw(tr);
                    // ⚠️ 必须由调用方做命中检测：Verse.TabRecord.Draw(rect) **只负责绘制**
                    //    （已验证其反编译产物里没有 ButtonInvisible / MouseUp），
                    //    clickedAction 是留给调用方在命中时自行调用的。漏了这一步 ⇒ 页签点不动。
                    if (Widgets.ButtonInvisible(tr) && tabs[i].clickedAction != null) tabs[i].clickedAction();
                    tx += tw;
                }
                bodyTop = bar.yMax + 10f;
            }
            else
            {
                bodyTop = inRect.y + 34f;
            }

            // ── 内容区
            var body = new Rect(inRect.x, bodyTop, inRect.width, inRect.height - (bodyTop - inRect.y) - 4f);
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

            // ── 页脚：帧率与滚动位置（同时验证 BeginScroll 游标是否正常）
            Draw.Label(new Rect(inRect.x, inRect.yMax - 2f, inRect.width, 18f),
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
            switch (tab)
            {
                case 1: return gallery;
                case 2: return tex;
                case 3: return reorder;
                case 4: return hilt;
                case 5: return pawnTable;
                case 6: return diag;
                default: return atoms42;
            }
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
