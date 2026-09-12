using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>
    /// 折叠树节点（CollapsibleTree 数据载体）。
    /// 展开态/选中态等交互状态挂在节点上，由调用方持有并跨帧复用（GATE-UI：组件本身无状态）。
    /// </summary>
    public class TreeNode
    {
        /// <summary>节点标题。</summary>
        public string Label;
        /// <summary>右侧计数 / 说明角标（如 "3"、"18 杀"）。</summary>
        public string Tag;                       // 右侧计数/说明角标（如 "3"、"18 杀"、"5491 殁"）
        /// <summary>展开态（由调用方持有并跨帧复用）。</summary>
        public bool Expanded = true;
        /// <summary>亡者标记：置灰 + 划线。</summary>
        public bool Dead;                        // 亡者：置灰 + 划线
        /// <summary>子节点；null = 叶节点。</summary>
        public List<TreeNode> Children;
        /// <summary>调用方自由挂载的数据。</summary>
        public object Payload;                   // 调用方自由挂载

        public TreeNode(string label) => Label = label;

        /// <summary>追加子节点并返回它（便于链式构建）。</summary>
        public TreeNode Add(TreeNode child)
        {
            if (Children == null) Children = new List<TreeNode>();
            Children.Add(child);
            return child;
        }
    }

    /// <summary>
    /// S4 · E1 导航核心 6 件（TabBar / Breadcrumb / Pager / CollapsibleTree / AnchorDots）。
    /// 无状态；选中页/当前节/指示条位置等一律由调用方以 ref 持有。
    /// </summary>
    public static class NavAtoms
    {
        // ---- TabBar 顶栏页签 ----

        /// <summary>
        /// 页签条：底部指示条随选中页滑动（caller 持 ref indicatorX）；宽度不足自动收进「更多 n」菜单。
        /// </summary>
        public static void TabBar(Rect rect, string[] tabs, ref int sel, ref float indicatorX)
        {
            if (tabs == null || tabs.Length == 0) return;
            sel = Mathf.Clamp(sel, 0, tabs.Length - 1);

            const float UnitW = 6.5f, PadPerTab = 20f, MoreW = 58f;
            int total = tabs.Length;

            // 估算总宽；溢出时折叠
            float need = 0f;
            for (int i = 0; i < total; i++) need += NavFmt.TabWidth(tabs[i], UnitW, PadPerTab);
            bool overflow = need > rect.width;
            int visible = total;
            if (overflow)
            {
                float budget = rect.width - MoreW, used = 0f;
                for (int i = 0; i < total; i++)
                {
                    float w = NavFmt.TabWidth(tabs[i], UnitW, PadPerTab);
                    if (used + w > budget) break;
                    used += w;
                    visible = i + 1;
                }
                visible = Mathf.Max(1, visible);
            }

            float x = rect.x;
            Rect selRect = default;
            for (int i = 0; i < visible; i++)
            {
                float w = Mathf.Min(NavFmt.TabWidth(tabs[i], UnitW, PadPerTab), rect.xMax - x);
                var tab = new Rect(x, rect.y, w, rect.height - 3f);
                bool on = i == sel;
                if (on)
                {
                    selRect = tab;
                    Draw.Label(tab, tabs[i], Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleCenter);
                }
                else
                {
                    if (Mouse.IsOver(tab)) Draw.Box(tab, Tokens.CardHover);
                    Draw.Label(tab, tabs[i], Tokens.FontLabel, Mouse.IsOver(tab) ? Tokens.Text : Tokens.Muted, TextAnchor.MiddleCenter);
                    if (Widgets.ButtonInvisible(tab)) sel = i;
                }
                x += w;
            }

            // 指示条（0.2s 级滑动插值，位置由调用方持有）
            float target = selRect.center.x - 12f;
            indicatorX = NavFmt.Approach(indicatorX, target, 0.2f);
            Draw.Box(new Rect(indicatorX, rect.yMax - 3f, 24f, 3f), Tokens.Accent);

            if (overflow)
            {
                // 消费上一帧「更多」菜单的选择（lambda 不能捕 ref 参数，经静态 pending 中转）
                if (pendingMorePick >= 0)
                {
                    sel = Mathf.Clamp(pendingMorePick, 0, total - 1);
                    pendingMorePick = -1;
                }
                var more = new Rect(rect.xMax - MoreW, rect.y, MoreW, rect.height - 3f);
                if (Widgets.ButtonText(more, Tr.Get("RUIF.Comp.Common.More", total - visible)))
                {
                    var opts = new List<FloatMenuOption>();
                    for (int i = visible; i < total; i++)
                    {
                        int idx = i;
                        opts.Add(new FloatMenuOption(tabs[i], () => pendingMorePick = idx));
                    }
                    Find.WindowStack.Add(new FloatMenu(opts));
                }
            }
        }

        private static int pendingMorePick = -1;

        // ---- Breadcrumb 面包屑 ----

        /// <summary>层级回退导航：各级可点跳转（onJump），末级为当前位。</summary>
        public static void Breadcrumb(Rect rect, string[] path, Action<int> onJump)
        {
            if (path == null || path.Length == 0) return;
            const float SepW = 14f, PadX = 6f, UnitW = 6.5f;
            float x = rect.x;
            for (int i = 0; i < path.Length; i++)
            {
                float w = Mathf.Min(NavFmt.LabelUnits(path[i]) * UnitW + PadX * 2f, rect.xMax - x);
                var cell = new Rect(x, rect.y, w, rect.height);
                bool last = i == path.Length - 1;
                bool over = !last && Mouse.IsOver(cell);
                if (over) Draw.Box(cell, Tokens.CardHover);
                Color c = last ? Tokens.Text : over ? Tokens.Accent : Tokens.Muted;
                Draw.Label(cell, path[i], Tokens.FontLabel, c, TextAnchor.MiddleLeft);
                if (!last)
                {
                    if (Widgets.ButtonInvisible(cell)) onJump?.Invoke(i);
                    x += w;
                    if (x + SepW <= rect.xMax)
                    {
                        Draw.Label(new Rect(x, rect.y, SepW, rect.height),
                            NavFmt.Separator, Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
                        x += SepW;
                    }
                }
                if (x >= rect.xMax) break;
            }
        }

        // ---- Pager 分页器 ----

        /// <summary>
        /// 分页条：首/末/步进/页码（省略号折叠）。pageBuf 传调用方缓冲可零分配（传 null 则内部临时分配）。
        /// </summary>
        public static void Pager(Rect rect, int total, int pageSize, ref int page, int[] pageBuf = null)
        {
            int pages = NavFmt.TotalPages(total, pageSize);
            page = NavFmt.ClampPage(page, pages);

            const float BtnW = 26f, EllW = 14f;
            var row = Layout.Row(rect, 2f);

            using (Draw.Enabled(page > 0))
                if (Widgets.ButtonText(row.Take(BtnW), "«")) page = 0;
            using (Draw.Enabled(page > 0))
                if (Widgets.ButtonText(row.Take(BtnW), "‹")) page--;

            int[] buf = pageBuf ?? new int[11];
            NavFmt.PageList(pages, page, buf, out int count);
            for (int k = 0; k < count; k++)
            {
                int p = buf[k];
                if (p < 0)
                {
                    Draw.Label(row.Take(EllW), "…", Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleCenter);
                    continue;
                }
                var b = row.Take(BtnW);
                if (p == page)
                {
                    Draw.Box(b, Tokens.AccentSoft);
                    Draw.Border(b, Tokens.Accent);
                    Draw.Label(b, (p + 1).ToString(), Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleCenter);
                }
                else if (Widgets.ButtonText(b, (p + 1).ToString()))
                    page = p;
            }

            using (Draw.Enabled(page < pages - 1))
                if (Widgets.ButtonText(row.Take(BtnW), "›")) page++;
            using (Draw.Enabled(page < pages - 1))
                if (Widgets.ButtonText(row.Take(BtnW), "»")) page = pages - 1;

            Draw.Label(row.Rest, Tr.Get("RUIF.Comp.Common.Page", page + 1, pages), Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleRight);
        }

        // ---- CollapsibleTree 折叠树 ----

        /// <summary>折叠树：返回本帧被点击的节点（null = 无点击）。展开态存于 TreeNode（调用方持有）。</summary>
        public static TreeNode CollapsibleTree(Rect rect, TreeNode root)
        {
            clicked = null;
            if (root == null) return null;
            GUI.BeginGroup(rect);
            DrawNode(root, 0f, 0f, rect.width);
            GUI.EndGroup();
            return clicked;
        }

        private static TreeNode clicked;

        private static float DrawNode(TreeNode node, float x, float y, float w)
        {
            const float RowH = 22f, Indent = 16f, CaretS = 14f;
            bool hasKids = node.Children != null && node.Children.Count > 0;
            var row = new Rect(x, y, w, RowH);
            bool over = Mouse.IsOver(row);
            if (over) Draw.Box(row, Tokens.CardHover);

            if (hasKids)
            {
                var caret = new Rect(x, y + (RowH - CaretS) / 2f, CaretS, CaretS);
                if (Widgets.ButtonInvisible(caret)) node.Expanded = !node.Expanded;
                StatusAtoms.Caret(caret, node.Expanded, Tokens.Muted);
            }

            Color c = node.Dead ? Tokens.Dim : over ? Tokens.Accent : Tokens.Text;
            Draw.Label(row, node.Label, Tokens.FontLabel, c, TextAnchor.MiddleLeft);
            if (!string.IsNullOrEmpty(node.Tag))
                Draw.Label(row, node.Tag + "  ", Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleRight);
            if (node.Dead)
                Draw.LineH(x + 16f, y + RowH * 0.55f,
                    Mathf.Min(w - 16f, NavFmt.LabelUnits(node.Label) * 6.5f + 2f),
                    Draw.WithAlpha(Tokens.Dim, 0.8f));
            if (Widgets.ButtonInvisible(row)) clicked = node;

            y += RowH;
            if (hasKids && node.Expanded)
                for (int i = 0; i < node.Children.Count; i++)
                    y = DrawNode(node.Children[i], x + Indent, y, w - Indent);
            return y;
        }

        // ---- AnchorDots 侧边锚点 ----

        /// <summary>长页面右缘锚点（几何方点，零贴图）：当前节放大高亮 + 标签浮现，点击跳转（写回 current）。</summary>
        public static void AnchorDots(Rect rect, string[] sections, ref int current)
        {
            if (sections == null || sections.Length == 0) return;
            current = Mathf.Clamp(current, 0, sections.Length - 1);

            const float DotS = 7f, DotBig = 10f, Gap = 6f, LabelW = 64f;
            float h = sections.Length * (DotBig + Gap) - Gap;
            float y = rect.y + Mathf.Max(0f, (rect.height - h) / 2f);
            float cx = rect.xMax - DotBig;

            for (int i = 0; i < sections.Length; i++)
            {
                bool on = i == current;
                bool over = Mouse.IsOver(new Rect(rect.xMax - DotBig - LabelW, y, DotBig + LabelW, DotBig));
                float s = on ? DotBig : DotS;
                var dot = new Rect(cx + (DotBig - s) / 2f, y + (DotBig - s) / 2f, s, s);
                Draw.Box(dot, on ? Tokens.Accent : over ? Tokens.Muted : Tokens.BorderSoft);
                if (on || over)
                    Draw.Label(new Rect(rect.xMax - DotBig - LabelW, y - 1f, LabelW - 4f, DotBig + 2f),
                        sections[i], Tokens.FontLabel, on ? Tokens.Accent : Tokens.Muted, TextAnchor.MiddleRight);
                if (over && Widgets.ButtonInvisible(new Rect(rect.xMax - DotBig - LabelW, y, DotBig + LabelW, DotBig)))
                    current = i;
                y += DotBig + Gap;
            }
        }
    }

    /// <summary>命令项（CommandPalette 数据载体，动作由调用方挂接）。</summary>
    public class Command
    {
        /// <summary>命令标题。</summary>
        public string Label;
        /// <summary>分组名（用于列表分区）。</summary>
        public string Group;
        /// <summary>执行动作。</summary>
        public Action Run;

        public Command(string label, Action run, string group = null)
        {
            Label = label;
            Run = run;
            Group = group;
        }
    }

    /// <summary>
    /// CommandPalette 命令面板（⚠️ v1 简化版）。
    /// 现已支持：嵌入窗口内绘制 + 模糊过滤 + ↑↓/回车/Esc 键盘操作；
    /// 全局 Ctrl+K 唤起与游戏快捷键仲裁留 v1.1（需独立 Window 承载）。
    /// </summary>
    public static class CommandPalette
    {
        private const string CtrlName = "RUIF.CommandPalette.Query";

        /// <summary>按模糊分过滤并降序；写入调用方缓冲（零分配），返回命中数。</summary>
        public static int Filter(string query, Command[] source, Command[] dest, int[] scores)
        {
            if (dest == null || scores == null || dest.Length == 0) return 0;
            int n = 0;
            if (source != null)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    int s = NavFmt.FuzzyScore(query, source[i]?.Label);
                    if (s < 0) continue;
                    int pos = Mathf.Min(n, dest.Length - 1);        // 缓冲满时挤掉末位
                    while (pos > 0 && scores[pos - 1] < s)
                    {
                        scores[pos] = scores[pos - 1];
                        dest[pos] = dest[pos - 1];
                        pos--;
                    }
                    scores[pos] = s;
                    dest[pos] = source[i];
                    n = Mathf.Min(n + 1, dest.Length);
                }
            }
            return n;
        }

        /// <summary>面板主体；返回被执行的命令（仅执行帧非空）。</summary>
        public static Command Draw(Rect rect, Command[] commands, ref string query,
            ref int sel, Command[] filtered, int[] scores)
        {
            var executed = (Command)null;
            RUIF.Draw.Box(rect, Tokens.Panel);
            RUIF.Draw.Border(rect, Tokens.Accent);

            var col = Layout.Column(Layout.Inset(rect, 6f, 6f), 4f);
            Rect search = col.Take(28f);

            int n = Filter(query, commands, filtered, scores);
            sel = n == 0 ? 0 : Mathf.Clamp(sel, 0, n - 1);

            // 键盘操作须在 TextField 消费事件之前接管（↑↓/回车/Esc）
            bool focused = GUI.GetNameOfFocusedControl() == CtrlName;
            var ev = Event.current;
            if (focused && ev != null && ev.type == EventType.KeyDown)
            {
                if (ev.keyCode == KeyCode.DownArrow) { sel = Mathf.Min(sel + 1, Mathf.Max(0, n - 1)); ev.Use(); }
                else if (ev.keyCode == KeyCode.UpArrow) { sel = Mathf.Max(sel - 1, 0); ev.Use(); }
                else if (ev.keyCode == KeyCode.Return || ev.keyCode == KeyCode.KeypadEnter)
                {
                    if (n > 0) { executed = filtered[sel]; GUI.FocusControl(null); }
                    ev.Use();
                }
                else if (ev.keyCode == KeyCode.Escape) { GUI.FocusControl(null); ev.Use(); }
            }

            GUI.SetNextControlName(CtrlName);
            query = Widgets.TextField(search, query);

            const float RowH = 24f;
            Rect list = col.Take(n * RowH + 4f);
            GUI.BeginGroup(list);
            for (int i = 0; i < n; i++)
            {
                var r = new Rect(0f, i * RowH, list.width, RowH);
                bool on = i == sel;
                if (on) { RUIF.Draw.Box(r, Tokens.AccentSoft); RUIF.Draw.LineV(0f, r.y, RowH, Tokens.Accent); }
                else if (Mouse.IsOver(r)) RUIF.Draw.Box(r, Tokens.CardHover);
                string text = string.IsNullOrEmpty(filtered[i].Group)
                    ? filtered[i].Label
                    : filtered[i].Group + " · " + filtered[i].Label;
                RUIF.Draw.Label(new Rect(8f, r.y, r.width - 8f, RowH), text,
                    Tokens.FontLabel, on ? Tokens.Accent : Tokens.Text, TextAnchor.MiddleLeft);
                if (Widgets.ButtonInvisible(r)) { executed = filtered[i]; GUI.FocusControl(null); }
            }
            GUI.EndGroup();

            RUIF.Draw.Label(col.Rest, Tr.Get("RUIF.Comp.Nav.CommandHint"), Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
            return executed;
        }
    }
}
