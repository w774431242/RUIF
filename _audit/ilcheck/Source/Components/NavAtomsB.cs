using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF
{
    /// <summary>NavBackBar 的动作结果。</summary>
    public enum NavAction { None, Back, Forward }

    /// <summary>
    /// S4 · E3 导航扩展 6 件（TimeSpeedControl / LayerSwitch / NavBackBar / JumpList / SidebarCollapse / FocusSearch）。
    /// 无状态；返回栈实例、流速、层选、折叠态等由调用方持有。
    /// </summary>
    public static class NavAtomsB
    {
        // ---- TimeSpeedControl 时间流速组 ----

        /// <summary>时间流速四态钮（官方 TimeSpeed 语义：暂停/1x/2x/3x，几何雪佛龙零贴图）。</summary>
        public static void TimeSpeedControl(Rect rect, ref TimeSpeed speed)
        {
            const float Gap = 3f;
            float w = (rect.width - Gap * 3f) / 4f;
            var row = Layout.Row(rect, Gap);
            SpeedCell(row.Take(w), TimeSpeed.Paused, ref speed, Tr.Get("RUIF.Comp.Nav.Time.Paused"));
            SpeedCell(row.Take(w), TimeSpeed.Normal, ref speed, Tr.Get("RUIF.Comp.Nav.Time.Normal"));
            SpeedCell(row.Take(w), TimeSpeed.Fast, ref speed, Tr.Get("RUIF.Comp.Nav.Time.Fast"));
            SpeedCell(row.Take(w), TimeSpeed.Superfast, ref speed, Tr.Get("RUIF.Comp.Nav.Time.Superfast"));
        }

        private static void SpeedCell(Rect r, TimeSpeed s, ref TimeSpeed cur, string tip)
        {
            bool on = cur == s;
            if (on) { Draw.Box(r, Tokens.AccentSoft); Draw.Border(r, Tokens.Accent); }
            else if (Mouse.IsOver(r)) Draw.Box(r, Tokens.CardHover);
            Color c = on ? Tokens.Accent : Tokens.Muted;
            var g = new Rect(r.center.x - 13f, r.center.y - 6f, 26f, 12f);
            if (s == TimeSpeed.Paused)
            {
                Draw.Box(new Rect(g.x + g.width * 0.32f - 1.5f, g.y, 3f, g.height), c);
                Draw.Box(new Rect(g.x + g.width * 0.68f - 1.5f, g.y, 3f, g.height), c);
            }
            else
            {
                int arrows = s == TimeSpeed.Normal ? 1 : s == TimeSpeed.Fast ? 2 : 3;
                float cw = g.width / 3f;
                for (int i = 0; i < arrows; i++)
                    Chevron(new Rect(g.x + (3 - arrows) * cw / 2f + i * cw, g.y, cw, g.height), c, false);
            }
            TooltipHandler.TipRegion(r, tip);
            if (Widgets.ButtonInvisible(r)) cur = s;
        }

        /// <summary>右向雪佛龙（两条折线，零贴图；flip=true 时镜像为左向）。</summary>
        private static void Chevron(Rect r, Color c, bool flip)
        {
            float midY = r.y + r.height / 2f;
            float x0 = flip ? r.xMax : r.x;
            float x1 = flip ? r.x : r.xMax;
            using (Draw.Color(c))
            {
                Widgets.DrawLine(new Vector2(x0, r.y), new Vector2(x1, midY), c, 1.5f);
                Widgets.DrawLine(new Vector2(x1, midY), new Vector2(x0, r.yMax), c, 1.5f);
            }
        }

        // ---- LayerSwitch 域切换 ----

        /// <summary>三域（或多域）切换：当前域高亮 + 底部指示线。层集合由调用方定义（地图/世界/档案等）。</summary>
        public static void LayerSwitch(Rect rect, string[] layers, ref int sel)
        {
            if (layers == null || layers.Length == 0) return;
            sel = Mathf.Clamp(sel, 0, layers.Length - 1);
            float w = rect.width / layers.Length;
            for (int i = 0; i < layers.Length; i++)
            {
                var r = new Rect(rect.x + i * w, rect.y, w, rect.height);
                bool on = i == sel;
                if (on) { Draw.Box(r, Tokens.AccentSoft); Draw.LineH(r.x, r.yMax - 2f, r.width, Tokens.Accent); }
                else if (Mouse.IsOver(r)) Draw.Box(r, Tokens.CardHover);
                Draw.Label(r, layers[i], Tokens.FontLabel, on ? Tokens.Accent : Tokens.Muted, TextAnchor.MiddleCenter);
                if (Widgets.ButtonInvisible(r)) sel = i;
            }
        }

        // ---- NavStack 返回栈 ----

        /// <summary>页内导航历史（容量 NavFmt.StackLimit，调用方持有实例）。</summary>
        public class NavStack
        {
            private readonly List<string> entries = new List<string>(NavFmt.StackLimit + 1);
            private int pos = -1;

            /// <summary>栈深（历史条数）。</summary>
            public int Depth => entries.Count;
            /// <summary>当前位置下标（-1 = 空）。</summary>
            public int Position => pos;
            /// <summary>当前页标签；空栈为 null。</summary>
            public string Current => pos >= 0 && pos < entries.Count ? entries[pos] : null;
            /// <summary>是否可回退。</summary>
            public bool CanBack => pos > 0;
            /// <summary>是否可前进。</summary>
            public bool CanForward => pos < entries.Count - 1;
            /// <summary>只读历史序列（供下拉展示）。</summary>
            public IReadOnlyList<string> Entries => entries;

            /// <summary>推进到新页：截断前进分支后追加；超限丢最旧。</summary>
            public void Push(string label)
            {
                if (entries.Count > pos + 1) entries.RemoveRange(pos + 1, entries.Count - pos - 1);
                entries.Add(label ?? string.Empty);
                pos = entries.Count - 1;
                if (entries.Count > NavFmt.StackLimit)
                {
                    entries.RemoveAt(0);
                    pos--;
                }
            }

            /// <summary>回退；不可退返回 null。</summary>
            public string Back()
            {
                if (!CanBack) return null;
                pos--;
                return Current;
            }

            /// <summary>前进；不可进返回 null。</summary>
            public string Forward()
            {
                if (!CanForward) return null;
                pos++;
                return Current;
            }

            /// <summary>跳到历史第 index 条（0 = 最早）。</summary>
            public string JumpTo(int index)
            {
                if (index < 0 || index >= entries.Count) return null;
                pos = index;
                return Current;
            }

            /// <summary>清空历史并复位当前位置。</summary>
            public void Clear()
            {
                entries.Clear();
                pos = -1;
            }
        }

        /// <summary>
        /// 返回栈操作条：←→（«/»）双钮 + 当前位置与栈深 + 历史下拉。
        /// 返回本帧动作（Back/Forward 由调用方对状态机执行实际跳转）。
        /// </summary>
        public static NavAction NavBackBar(Rect rect, NavStack stack, string currentLabel)
        {
            const float BtnW = 30f;
            var row = Layout.Row(rect, 3f);
            var act = NavAction.None;
            bool has = stack != null;

            using (Draw.Enabled(has && stack.CanBack))
                if (Widgets.ButtonText(row.Take(BtnW), "«")) act = NavAction.Back;
            using (Draw.Enabled(has && stack.CanForward))
                if (Widgets.ButtonText(row.Take(BtnW), "»")) act = NavAction.Forward;

            if (has)
            {
                Rect body = row.Rest;
                Rect hist = new Rect(body.xMax - BtnW, body.y, BtnW, body.height);
                Rect labelRect = new Rect(body.x, body.y, body.width - BtnW - 3f, body.height);
                string path = currentLabel ?? stack.Current ?? "—";
                Draw.Label(labelRect, path + Tr.Get("RUIF.Comp.Nav.StackDepth", stack.Depth), Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleLeft);
                if (Widgets.ButtonText(hist, "≡"))
                {
                    var opts = new List<FloatMenuOption>();
                    for (int i = stack.Depth - 1; i >= 0; i--)
                    {
                        int idx = i;
                        string mark = idx == stack.Position ? Tr.Get("RUIF.Comp.Common.Current") : string.Empty;
                        opts.Add(new FloatMenuOption(stack.Entries[idx] + mark, () => stack.JumpTo(idx)));
                    }
                    Find.WindowStack.Add(new FloatMenu(opts));
                }
            }
            return act;
        }

        // ---- JumpList 最近查看 ----

        /// <summary>JumpList 条目。</summary>
        public struct RecentEntry
        {
            /// <summary>条目标题。</summary>
            public string Title;
            /// <summary>类别（徽标取首字，如 人/役/厂）。</summary>
            public string Kind;      // 类别徽取首字（如 人/役/厂）
            /// <summary>距今时长（秒），用于相对时间显示。</summary>
            public float AgeSec;

            public RecentEntry(string title, string kind, float ageSec)
            {
                Title = title;
                Kind = kind;
                AgeSec = ageSec;
            }
        }

        /// <summary>最近查看清单（图标徽 + 名 + 相对时间）；返回点击的条目索引（-1 = 无）。</summary>
        public static int JumpList(Rect rect, RecentEntry[] recent, int max = 8)
        {
            int n = recent == null ? 0 : Mathf.Min(recent.Length, max);
            const float RowH = 24f;
            int hit = -1;
            GUI.BeginGroup(rect);
            for (int i = 0; i < n; i++)
            {
                var r = new Rect(0f, i * RowH, rect.width, RowH);
                bool over = Mouse.IsOver(r);
                if (over) Draw.Box(r, Tokens.CardHover);

                var badge = new Rect(4f, r.y + 4f, 16f, 16f);
                Draw.Box(badge, Draw.WithAlpha(Tokens.Accent, 0.25f));
                string first = string.IsNullOrEmpty(recent[i].Kind) ? "·" : recent[i].Kind.Substring(0, 1);
                Draw.Label(badge, first, Tokens.FontLabel, Tokens.Accent, TextAnchor.MiddleCenter);

                Draw.Label(new Rect(26f, r.y, Mathf.Max(0f, rect.width - 92f), RowH),
                    recent[i].Title, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
                Draw.Label(new Rect(rect.width - 62f, r.y, 58f, RowH),
                    NavFmt.RelativeTime(recent[i].AgeSec), Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleRight);

                if (over && Widgets.ButtonInvisible(r)) hit = i;
            }
            GUI.EndGroup();
            return hit;
        }

        // ---- SidebarCollapse 侧栏折叠把手 ----

        /// <summary>主从布局分界处的折叠把手：点击切换 open；展开宽度动画由调用方以 NavFmt.Approach 驱动。</summary>
        public static void SidebarCollapse(Rect edge, ref bool open)
        {
            bool over = Mouse.IsOver(edge);
            Draw.Box(edge, over ? Tokens.CardHover : Tokens.Panel);
            Draw.Border(edge, over ? Tokens.Accent : Tokens.BorderSoft);

            float cx = edge.center.x, cy = edge.center.y;
            using (Draw.Color(Tokens.Muted))
            {
                Widgets.DrawLineHorizontal(cx - 4f, cy - 4f, 8f);
                Widgets.DrawLineHorizontal(cx - 4f, cy, 8f);
                Widgets.DrawLineHorizontal(cx - 4f, cy + 4f, 8f);
            }
            // 方向雪佛龙：展开态显示左向（收起），收起态显示右向
            Chevron(new Rect(cx - 4f, edge.y + 3f, 8f, 8f), over ? Tokens.Accent : Tokens.Dim, open);
            if (Widgets.ButtonInvisible(edge)) open = !open;
        }

        // ---- FocusSearch 聚焦搜索 ----

        private const string SearchCtrl = "RUIF.FocusSearch.Field";

        /// <summary>
        /// 聚焦搜索框（⚠️ 窗口内语义）：本窗口持有键盘焦点时 Ctrl+F 聚焦本框、Esc 归还焦点。
        /// 返回当前是否聚焦。
        /// </summary>
        public static bool FocusSearch(Rect rect, ref string query)
        {
            var ev = Event.current;
            bool focused = GUI.GetNameOfFocusedControl() == SearchCtrl;

            if (ev != null && ev.type == EventType.KeyDown)
            {
                if (!focused && ev.keyCode == KeyCode.F && ev.control)
                {
                    GUI.FocusControl(SearchCtrl);
                    ev.Use();
                    focused = true;
                }
                else if (focused && ev.keyCode == KeyCode.Escape)
                {
                    GUI.FocusControl(null);
                    ev.Use();
                    focused = false;
                }
            }

            GUI.SetNextControlName(SearchCtrl);
            query = Widgets.TextField(rect, query);
            Draw.Border(rect, focused ? Tokens.Accent : Tokens.BorderSoft);
            if (focused)
                Draw.Label(new Rect(rect.xMax - 26f, rect.y, 22f, rect.height), "Esc", Tokens.FontLabel, Tokens.Dim, TextAnchor.MiddleRight);
            return GUI.GetNameOfFocusedControl() == SearchCtrl;
        }
    }
}
