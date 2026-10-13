using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// <see cref="BenchAtoms42"/> 的状态原子族（5 件）+ 布局与视觉族（6 件）。
    ///
    /// <para>所有演示区域尺寸取自《原子落地基准表》§1.4（预览 CSS 选择器 + 行号反读），
    /// 例如 <c>.dot{10×10}</c>(L263)、<c>.glyph{17×17}</c>(L264)、<c>.spinner{22×22}</c>(L265)、
    /// <c>.anchorstage{height:74px}</c>(L276)、<c>.swatch{28×22}</c>(L280)、<c>.gradbar{height:11px}</c>(L282)。</para>
    /// </summary>
    internal sealed partial class BenchAtoms42
    {
        private void BuildStatus()
        {
            var f = new Family { Title = "A0 · 原子层 —— 状态原子", Sub = "5 件 · StatusAtoms（alpha/scale 插值，无 CSS 过渡）" };

            f.Add(new Atom
            {
                No = "13", Name = "PulseDot 呼吸状态点", Api = "StatusAtoms.PulseDot(Rect, Color, float t, float minAlpha)",
                Desc = "绿在线 / 黄忙碌 / 红告急，alpha 与 scale 双插值脉动。任意锚点的存活指示。",
                Note = "三个点共用同一条动画但错相位——同相位会像灯泡，错相位才像呼吸",
                BodyH = 24f, MinW = 2f, MinH = 2f,
                Paint = r =>
                {
                    // ⚠ 契约：PulseDot 只在 rect 内画点（取 min(w,h) 正方形），**不画标签**。
                    // 标签的排版是调用方的责任 —— 必须「先取单元矩形，再在单元内二次分行」。
                    // 直接用绝对坐标画标签会让它溢出单元宽，被下一个点压住（本轮事故）。
                    var row = Layout.Row(r, 20f);
                    string[] names = { "在线", "忙碌", "告急" };
                    Color[] cols = { Tokens.Alive, Tokens.Warn, Tokens.Dead };
                    for (int i = 0; i < names.Length; i++)
                    {
                        Rect unit = row.Take(10f + 7f + Measure.Width(names[i], GameFont.Tiny));
                        if (unit.width < 10f) break;
                        var inner = Layout.Row(unit, 7f);
                        StatusAtoms.PulseDot(inner.Take(10f), cols[i], t + i * 0.55f);   // .dot{10×10} L263
                        Draw.EllipsisLabel(inner.Rest, names[i], GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                    }
                }
            });

            f.Add(new Atom
            {
                , Name = "PulseDot 呼吸状态点", Api = "StatusAtoms.PulseDot(Rect, Color, float t, float minAlpha)",
                Desc = "绿在线 / 黄忙碌 / 红告急，alpha 与 scale 双插值脉动。任意锚点的存活指示。",
                Note = "三个点共用同一条动画但错相位——同相位会像灯泡，错相位才像呼吸",
                BodyH = 24f,
                Paint = r =>
                {
                    // ⚠ 契约：PulseDot 只在 rect 内画点（取 min(w,h) 正方形），**不画标签**。
                    // 标签的排版是调用方的责任 —— 必须「先取单元矩形，再在单元内二次分行」。
                    // 直接用绝对坐标画标签会让它溢出单元宽，被下一个点压住（本轮事故）。
                    var row = Layout.Row(r, 20f);
                    string[] names = { "在线", "忙碌", "告急" };
                    Color[] cols = { Tokens.Alive, Tokens.Warn, Tokens.Dead };
                    for (int i = 0; i < names.Length; i++)
                    {
                        Rect unit = row.Take(10f + 7f + Measure.Width(names[i], GameFont.Tiny));
                        if (unit.width < 10f) break;
                        var inner = Layout.Row(unit, 7f);
                        StatusAtoms.PulseDot(inner.Take(10f), cols[i], t + i * 0.55f);   // .dot{10×10} L263
                        Draw.EllipsisLabel(inner.Rest, names[i], GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                    }
                }
            });

            f.Add(new Atom
            {
                1414No = "15"No = "15", Name = "LoadingSpinner 旋转加载", Api = "StatusAtoms.Spinner(Rect, float t, Color?, int segments, float radius, float size)",
                Desc = "自绘分段旋转。另有 DrawTextureRotated 贴图重载，走 GUIUtility.RotateAroundPivot 捷径。",
                Note = "进度累积到 100% 自动停转并转 ✓ · 可重播（交互回补）",
                BodyH = 22f + 4f + 11f + 4f + CtrlTiny,
                Paint = r =>
                {
                    var row = Layout.Row(r, 10f);
                    Rect sp = row.Take(22f);                                        // .spinner{22×22} L265
                    if (!spinDone)
                    {
                        StatusAtoms.Spinner(sp, t, Tokens.Accent);
                    }
                    else
                    {
                        Draw.Box(sp, Draw.WithAlpha(Tokens.Alive, 0.16f));
                        Draw.Label(sp, "✓", GameFont.Small, Tokens.Alive, TextAnchor.MiddleCenter);
                    }
                    var col = Layout.Column(row.Rest, 4f);
                    KBar(col.Take(11f), spinP / 100f, spinDone ? Tokens.Alive : Tokens.Accent);   // .kbar 高 11 L207
                    var line = Layout.Row(col.Take(col.Rest.height), 6f);
                    // 交互回补：重播 —— 复位进度即重跑（复用已有 spinP/spinDone，无需新字段）
                    if (Widgets.ButtonText(line.Take(64f), "重播")) { spinP = 0f; spinDone = false; }
                    Draw.EllipsisLabel(line.Rest, spinDone ? "归档完成 · 事件日志已写入" : "正在归档 事件日志…",
                        GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                , Name = "LoadingSpinner 旋转加载", Api = "StatusAtoms.Spinner(Rect, float t, Color?, int segments, float radius, float size)",
                Desc = "自绘分段旋转。另有 DrawTextureRotated 贴图重载，走 GUIUtility.RotateAroundPivot 捷径。",
                Note = "进度累积到 100% 自动停转并转 ✓ · 可重播（交互回补）",
                BodyH = 22f + 4f + 11f + 4f + CtrlTiny,
                Paint = r =>
                {
                    var row = Layout.Row(r, 10f);
                    Rect sp = row.Take(22f);                                        // .spinner{22×22} L265
                    if (!spinDone)
                    {
                        StatusAtoms.Spinner(sp, t, Tokens.Accent);
                    }
                    else
                    {
                        Draw.Box(sp, Draw.WithAlpha(Tokens.Alive, 0.16f));
                        Draw.Label(sp, "✓", GameFont.Small, Tokens.Alive, TextAnchor.MiddleCenter);
                    }
                    var col = Layout.Column(row.Rest, 4f);
                    KBar(col.Take(11f), spinP / 100f, spinDone ? Tokens.Alive : Tokens.Accent);   // .kbar 高 11 L207
                    var line = Layout.Row(col.Take(col.Rest.height), 6f);
                    // 交互回补：重播 —— 复位进度即重跑（复用已有 spinP/spinDone，无需新字段）
                    if (Widgets.ButtonText(line.Take(64f), "重播")) { spinP = 0f; spinDone = false; }
                    Draw.EllipsisLabel(line.Rest, spinDone ? "归档完成 · 事件日志已写入" : "正在归档 事件日志…",
                        GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                1616No = "17"No = "17", Name = "ExpandCaret 展开箭头", Api = "StatusAtoms.Caret(Rect, bool expanded, Color?) / Caret(Rect, float progress01, Color?)",
                Interactive = true,
                Desc = "▸ / ▾ 旋转插值——CollapsibleTree / SectionFrame / 折叠组的共享原子。",
                Note = "bool 重载（插值旋转）+ float 重载（0–1 进度）；工坊实证 20 项工作类型",
                BodyH = 58f, MinW = 8f, MinH = 8f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 4f);
                    Rect headRect = col.Take(24f);            // HStack 字段私有 ⇒ 保留整行矩形做命中检测
                    var head = Layout.Row(headRect, 7f);
                    var caret = head.Take(16f);
                    StatusAtoms.Caret(caret, caretOpen);
                    if (Widgets.ButtonInvisible(headRect)) caretOpen = !caretOpen;
                    Draw.EllipsisLabel(head.Rest, "工作优先级（" + WorkTypes.Length + " 项）", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                    // 折叠体：展开时列出官方 WorkTypeDef 词表
                    var body = col.Take(30f);
                    if (caretOpen)
                    {
                        var chips = Layout.Row(body, 4f);
                        for (int i = 0; i < WorkTypes.Length && chips.Rest.width > 30f; i++)
                        {
                            float w = Text.CalcSize(WorkTypes[i]).x + 10f;
                            Rect chip = chips.Take(Mathf.Min(w, chips.Rest.width));
                            Draw.Box(chip, Tokens.Card);
                            Draw.Border(chip, Tokens.BorderSoft);
                            Draw.Label(chip, WorkTypes[i], GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleCenter);
                        }
                    }
                }
            });

            families.Add(f);
        }

        private void BuildVisual()
        {
            var f = new Family { Title = "A0 · 原子层 —— 布局与视觉原子", Sub = "6 件 · Atoms / VisualAtoms（比例 / 锚定 / 取色 / 图例 / 截断 / 角标）" };

            f.Add(new Atom
            {
                , Name = "ExpandCaret 展开箭头", Api = "StatusAtoms.Caret(Rect, bool expanded, Color?) / Caret(Rect, float progress01, Color?)",
                Interactive = true,
                Desc = "▸ / ▾ 旋转插值——CollapsibleTree / SectionFrame / 折叠组的共享原子。",
                Note = "bool 重载（插值旋转）+ float 重载（0–1 进度）；工坊实证 20 项工作类型",
                BodyH = 58f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 4f);
                    Rect headRect = col.Take(24f);            // HStack 字段私有 ⇒ 保留整行矩形做命中检测
                    var head = Layout.Row(headRect, 7f);
                    var caret = head.Take(16f);
                    StatusAtoms.Caret(caret, caretOpen);
                    if (Widgets.ButtonInvisible(headRect)) caretOpen = !caretOpen;
                    Draw.EllipsisLabel(head.Rest, "工作优先级（" + WorkTypes.Length + " 项）", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                    // 折叠体：展开时列出官方 WorkTypeDef 词表
                    var body = col.Take(30f);
                    if (caretOpen)
                    {
                        var chips = Layout.Row(body, 4f);
                        for (int i = 0; i < WorkTypes.Length && chips.Rest.width > 30f; i++)
                        {
                            float w = Text.CalcSize(WorkTypes[i]).x + 10f;
                            Rect chip = chips.Take(Mathf.Min(w, chips.Rest.width));
                            Draw.Box(chip, Tokens.Card);
                            Draw.Border(chip, Tokens.BorderSoft);
                            Draw.Label(chip, WorkTypes[i], GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleCenter);
                        }
                    }
                }
            });

            families.Add(f);
        }

        private void BuildVisual()
        {
            var f = new Family { Title = "A0 · 原子层 —— 布局与视觉原子", Sub = "6 件 · Atoms / VisualAtoms（比例 / 锚定 / 取色 / 图例 / 截断 / 角标）" };

            f.Add(new Atom
            {
                1818No = "19"No = "19", Name = "AnchorFrame 九宫锚定", Api = "Atoms.Anchor(Rect parent, Anchor9, Vector2 size, float margin) → Rect",
                Desc = "声明式定位原子：九宫九锚（贴边 / 居中 / 拉伸）+ 边距。L4 布局的定位地基。",
                Note = "点舞台任意位置吸附最近锚 · 规格对齐 .anchorstage{74px} + .tgt{26×18}",
                BodyH = 74f, MinW = 40f, MinH = 40f,
                Paint = r =>
                {
                    Draw.Border(r, Tokens.BorderSoft);                              // .anchorstage L276
                    Anchor9[] a9 =
                    {
                        Anchor9.TopLeft, Anchor9.TopCenter, Anchor9.TopRight,
                        Anchor9.MidLeft, Anchor9.Center, Anchor9.MidRight,
                        Anchor9.BottomLeft, Anchor9.BottomCenter, Anchor9.BottomRight
                    };
                    for (int i = 0; i < a9.Length; i++)
                    {
                        Rect a = Atoms.Anchor(r, a9[i], new Vector2(26f, 18f), 4f);   // .tgt{26×18} L277
                        Draw.Box(a, i == anchorIdx ? Tokens.Accent : Tokens.PanelRaised);
                        Draw.Border(a, Tokens.BorderSoft);
                        Draw.Label(a, a9[i].ToString().Substring(0, 2), GameFont.Tiny, i == anchorIdx ? Tokens.Window : Tokens.Dim, TextAnchor.MiddleCenter);
                    }
                    // 交互回补：点舞台 → 吸附最近锚（按鼠标位置算 3×3 索引，与 Anchor9 声明顺序同序）
                    if (Widgets.ButtonInvisible(r))
                    {
                        Vector2 mp = Event.current.mousePosition;
                        int cx = mp.x < r.x + r.width / 3f ? 0 : (mp.x > r.x + r.width * 2f / 3f ? 2 : 1);
                        int cy = mp.y < r.y + r.height / 3f ? 0 : (mp.y > r.y + r.height * 2f / 3f ? 2 : 1);
                        anchorIdx = cy * 3 + cx;
                    }
                }
            });

            f.Add(new Atom
            {
                , Name = "AnchorFrame 九宫锚定", Api = "Atoms.Anchor(Rect parent, Anchor9, Vector2 size, float margin) → Rect",
                Desc = "声明式定位原子：九宫九锚（贴边 / 居中 / 拉伸）+ 边距。L4 布局的定位地基。",
                Note = "点舞台任意位置吸附最近锚 · 规格对齐 .anchorstage{74px} + .tgt{26×18}",
                BodyH = 74f,
                Paint = r =>
                {
                    Draw.Border(r, Tokens.BorderSoft);                              // .anchorstage L276
                    Anchor9[] a9 =
                    {
                        Anchor9.TopLeft, Anchor9.TopCenter, Anchor9.TopRight,
                        Anchor9.MidLeft, Anchor9.Center, Anchor9.MidRight,
                        Anchor9.BottomLeft, Anchor9.BottomCenter, Anchor9.BottomRight
                    };
                    for (int i = 0; i < a9.Length; i++)
                    {
                        Rect a = Atoms.Anchor(r, a9[i], new Vector2(26f, 18f), 4f);   // .tgt{26×18} L277
                        Draw.Box(a, i == anchorIdx ? Tokens.Accent : Tokens.PanelRaised);
                        Draw.Border(a, Tokens.BorderSoft);
                        Draw.Label(a, a9[i].ToString().Substring(0, 2), GameFont.Tiny, i == anchorIdx ? Tokens.Window : Tokens.Dim, TextAnchor.MiddleCenter);
                    }
                    // 交互回补：点舞台 → 吸附最近锚（按鼠标位置算 3×3 索引，与 Anchor9 声明顺序同序）
                    if (Widgets.ButtonInvisible(r))
                    {
                        Vector2 mp = Event.current.mousePosition;
                        int cx = mp.x < r.x + r.width / 3f ? 0 : (mp.x > r.x + r.width * 2f / 3f ? 2 : 1);
                        int cy = mp.y < r.y + r.height / 3f ? 0 : (mp.y > r.y + r.height * 2f / 3f ? 2 : 1);
                        anchorIdx = cy * 3 + cx;
                    }
                }
            });

            f.Add(new Atom
            {
                2020No = "21"No = "21", Name = "GradientBar 渐变图例条", Api = "VisualAtoms.GradientBar(Rect, Color lo, Color hi, int segments)",
                Desc = "渐变强度图例（HeatCalendar / 双端滑条的共伴），分段绘制即可平滑，零贴图。",
                Note = "预览 .gradbar{height:11px}，两端标注 0 与 100 件/日",
                BodyH = 32f, MinW = 16f, MinH = 1f,
                Paint = r =>
                {
                    var bar = new Rect(r.x, r.y, r.width, 11f);
                    VisualAtoms.GradientBar(bar, Tokens.Accent, Tokens.Alive, 16);   // .gradbar 高 11 L282
                    // 交互回补：悬停读数（判定优先于绘制 —— 不悬停就不画指示线）
                    bool over = Mouse.IsOver(bar);
                    gradHover = over ? Mathf.Clamp01((Event.current.mousePosition.x - bar.x) / Mathf.Max(1f, bar.width)) : -1f;
                    if (gradHover >= 0f)
                    {
                        Draw.Box(new Rect(bar.x + gradHover * bar.width - 1f, bar.y - 2f, 2f, 15f), Tokens.Text);   // .gradbar i 规格
                    }
                    Draw.EllipsisLabel(new Rect(r.x, r.y + 14f, 60f, LineTiny), "0", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                    Draw.EllipsisLabel(new Rect(r.xMax - 90f, r.y + 14f, 90f, LineTiny), "100 件/日", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleRight);
                    Draw.EllipsisLabel(new Rect(r.x + 62f, r.y + 14f, Mathf.Max(0f, r.width - 156f), LineTiny),
                        gradHover >= 0f ? "读数 " + Mathf.RoundToInt(gradHover * 100f) + " 件/日" : "悬停读数",
                        GameFont.Tiny, gradHover >= 0f ? Tokens.Text : Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                , Name = "GradientBar 渐变图例条", Api = "VisualAtoms.GradientBar(Rect, Color lo, Color hi, int segments)",
                Desc = "渐变强度图例（HeatCalendar / 双端滑条的共伴），分段绘制即可平滑，零贴图。",
                Note = "预览 .gradbar{height:11px}，两端标注 0 与 100 件/日",
                BodyH = 32f,
                Paint = r =>
                {
                    var bar = new Rect(r.x, r.y, r.width, 11f);
                    VisualAtoms.GradientBar(bar, Tokens.Accent, Tokens.Alive, 16);   // .gradbar 高 11 L282
                    // 交互回补：悬停读数（判定优先于绘制 —— 不悬停就不画指示线）
                    bool over = Mouse.IsOver(bar);
                    gradHover = over ? Mathf.Clamp01((Event.current.mousePosition.x - bar.x) / Mathf.Max(1f, bar.width)) : -1f;
                    if (gradHover >= 0f)
                    {
                        Draw.Box(new Rect(bar.x + gradHover * bar.width - 1f, bar.y - 2f, 2f, 15f), Tokens.Text);   // .gradbar i 规格
                    }
                    Draw.EllipsisLabel(new Rect(r.x, r.y + 14f, 60f, LineTiny), "0", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                    Draw.EllipsisLabel(new Rect(r.xMax - 90f, r.y + 14f, 90f, LineTiny), "100 件/日", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleRight);
                    Draw.EllipsisLabel(new Rect(r.x + 62f, r.y + 14f, Mathf.Max(0f, r.width - 156f), LineTiny),
                        gradHover >= 0f ? "读数 " + Mathf.RoundToInt(gradHover * 100f) + " 件/日" : "悬停读数",
                        GameFont.Tiny, gradHover >= 0f ? Tokens.Text : Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                2222No = "23"No = "23", Name = "PinBadge 钉选角标", Api = "VisualAtoms.PinBadge(Rect host, bool pinned, Color?, float size, float margin)",
                Interactive = true,
                Desc = "钉选 / 置顶状态角标（AnchorFrame 右上定位的示范用户）。",
                Note = "点卡片切换钉选；钉住的面板在真实场景里会跨页签常驻",
                BodyH = 30f, MinW = 16f, MinH = 16f,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    bool[] pins = { pinA, pinB, pinC };
                    string[] names = { "殖民者档案", "事件日志", "研究队列" };
                    for (int i = 0; i < names.Length; i++)
                    {
                        Rect host = row.Take(Mathf.Min(132f, row.Rest.width));
                        if (host.width < 40f) break;
                        Draw.Box(host, Tokens.PanelRaised);
                        Draw.Border(host, Tokens.BorderSoft);
                        Draw.EllipsisLabel(new Rect(host.x + 6f, host.y, host.width - 22f, host.height), names[i], GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                        VisualAtoms.PinBadge(host, pins[i]);
                        if (!Widgets.ButtonInvisible(host)) continue;
                        if (i == 0) pinA = !pinA; else if (i == 1) pinB = !pinB; else pinC = !pinC;
                    }
                }
            });

            families.Add(f);
        }
    }
}
, Name = "PinBadge 钉选角标", Api = "VisualAtoms.PinBadge(Rect host, bool pinned, Color?, float size, float margin)",
                Interactive = true,
                Desc = "钉选 / 置顶状态角标（AnchorFrame 右上定位的示范用户）。",
                Note = "点卡片切换钉选；钉住的面板在真实场景里会跨页签常驻",
                BodyH = 30f,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    bool[] pins = { pinA, pinB, pinC };
                    string[] names = { "殖民者档案", "事件日志", "研究队列" };
                    for (int i = 0; i < names.Length; i++)
                    {
                        Rect host = row.Take(Mathf.Min(132f, row.Rest.width));
                        if (host.width < 40f) break;
                        Draw.Box(host, Tokens.PanelRaised);
                        Draw.Border(host, Tokens.BorderSoft);
                        Draw.EllipsisLabel(new Rect(host.x + 6f, host.y, host.width - 22f, host.height), names[i], GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                        VisualAtoms.PinBadge(host, pins[i]);
                        if (!Widgets.ButtonInvisible(host)) continue;
                        if (i == 0) pinA = !pinA; else if (i == 1) pinB = !pinB; else pinC = !pinC;
                    }
                }
            });

            families.Add(f);
        }
    }
}
