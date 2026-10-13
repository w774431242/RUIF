using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// <see cref="BenchAtoms42"/> 的 A0+ 五族（表格 5 / 面板 4 / 交互 4 / 进度 3 / 杂项 3 = 19 件）。
    ///
    /// <para><b>本文件是「绘制契约误用」的修正现场</b>：以下 7 处曾因凭签名猜语义而画错，
    /// 现全部按反编译出的真实契约调用（依据见 docs/02-组件图鉴/原子落地基准表.md §2）：</para>
    /// <list type="number">
    /// <item><c>FillWipe</c>：自铺 <c>BorderSoft</c> 满底 + 分段实心条 ⇒ 调用方<b>不得</b>再叠底、不得与 label 叠压</item>
    /// <item><c>CapacityBar</c>：默认色是 <c>ratio&gt;0.85 ? Warn : Alive</c>（非 Accent）；超载画 <b>4 个红竖块</b>（非虚线）⇒ 传 null 让组件自己表达</item>
    /// <item><c>ZoomIndicator</c>：<b>内部自带</b> <c>Layout.Row</c> 排 [标签34][读数50][按钮64] ⇒ 调用方只需给够宽</item>
    /// <item><c>PointCostBadge</c>：宽高即徽章本体 ⇒ 按 <c>.pill</c> 取「文本宽 + 18」</item>
    /// <item><c>BubbleTag</c>：<paramref name="bubble"/> <b>仅为主体</b>，尾巴画在 <c>yMax</c> 下方 8px ⇒ 高度只给一行</item>
    /// <item><c>ThinScrollBar</c>：条画在 <c>viewRect.xMax + 1f</c>（<b>rect 外侧</b>），且 <c>contentH ≤ height</c> 时不画 ⇒ 传视口 rect</item>
    /// <item><c>ToolbarIcon</c>：rect 即按钮本体 ⇒ 按 <c>.iconbtn</c> 给 <b>28×28</b></item>
    /// </list>
    /// </summary>
    internal sealed partial class BenchAtoms42
    {
        // 表格演示数据（官方技能/工作词表驱动；排序为真实重排）
        private readonly float[] cellMood = { 62f, 38f, 88f, 51f, 74f, 45f };
        private readonly int[] cellKill = { 3, 17, 0, 9, 5, 12 };
        private readonly int[] cellWork = { 18400, 42100, 9700, 26300, 33900, 15200 };

        // ─────────────────────────────────────── A0+ · 表格原子族 5 件
        private void BuildTable()
        {
            var f = new Family { Title = "A0+ · 表格原子族", Sub = "5 件 · TableAtoms / TableFmt（Numbers / Work Tab 实证）" };

            f.Add(new Atom
            {
                No = "24", Name = "SortHeaderCell 排序表头单元", Api = "TableAtoms.HeaderCell(Rect, string, SortState) → bool · TableFmt.NextSort",
                Interactive = true,
                Badge = "ok",
                Desc = "点表头循环排序（无 → 升 → 降 → 无），三角指示。返回 true 表示该次点击命中——这是真排序，下面这张表会跟着重排。",
                Note = "列宽 34% / 22% / 22% / 22%（对齐预览 .ttab 四列）· 数据每 6 秒刷新一次且保持当前排序",
                BodyH = 152f, MinW = 60f, MinH = 18f,
                Paint = r =>
                {
                    // 表头（.ttab th{padding:4px 7px;background:raised} L286-288）
                    float[] w = { 0.34f, 0.22f, 0.22f, 0.22f };
                    var head = Layout.Row(new Rect(r.x, r.y, r.width, 20f), 0f);
                    for (int i = 0; i < TableColumns.Length; i++)
                    {
                        Rect cell = head.Take(r.width * w[i]);
                        if (TableAtoms.HeaderCell(cell, TableColumns[i], sortStates[i]))
                            sortStates[i] = TableFmt.NextSort(sortStates[i]);
                    }
                    // 数据行（真实重排：以当前激活列排序）
                    int active = -1, dir = 0;
                    for (int i = 0; i < sortStates.Length; i++)
                        if (sortStates[i] != SortState.None) { active = i; dir = sortStates[i] == SortState.Asc ? 1 : -1; }
                    int[] order = { 0, 1, 2, 3, 4, 5 };
                    if (active > 0)
                    {
                        float[] key = active == 1 ? cellMood : active == 2 ? Array.ConvertAll(cellWork, v => (float)v) : Array.ConvertAll(cellKill, v => (float)v);
                        Array.Sort(order, (a, b) => (key[a].CompareTo(key[b])) * dir);
                    }
                    var rows = Layout.Column(new Rect(r.x, r.y + 22f, r.width, r.height - 22f), 0f);
                    for (int k = 0; k < order.Length; k++)
                    {
                        int idx = order[k];
                        var line = Layout.Row(rows.Take(21f), 0f);
                        Draw.EllipsisLabel(line.Take(r.width * w[0]), Names[idx], GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                        TableAtoms.ValueCell(line.Take(r.width * w[1]), cellMood[idx], 100f);          // 阈值着色：值 / 参照值
                        TableAtoms.ValueCell(line.Take(r.width * w[2]), cellWork[idx], 45000f);
                        TableAtoms.ValueCell(line.Take(r.width * w[3]), cellKill[idx], 20f);
                        Draw.LineH(r.x, rows.Y - 1f, r.width, Tokens.BorderHair);
                    }
                }
            });

            f.Add(new Atom
            {
                , Name = "SortHeaderCell 排序表头单元", Api = "TableAtoms.HeaderCell(Rect, string, SortState) → bool · TableFmt.NextSort",
                Interactive = true,
                Badge = "ok",
                Desc = "点表头循环排序（无 → 升 → 降 → 无），三角指示。返回 true 表示该次点击命中——这是真排序，下面这张表会跟着重排。",
                Note = "列宽 34% / 22% / 22% / 22%（对齐预览 .ttab 四列）· 数据每 6 秒刷新一次且保持当前排序",
                BodyH = 152f,
                Paint = r =>
                {
                    // 表头（.ttab th{padding:4px 7px;background:raised} L286-288）
                    float[] w = { 0.34f, 0.22f, 0.22f, 0.22f };
                    var head = Layout.Row(new Rect(r.x, r.y, r.width, 20f), 0f);
                    for (int i = 0; i < TableColumns.Length; i++)
                    {
                        Rect cell = head.Take(r.width * w[i]);
                        if (TableAtoms.HeaderCell(cell, TableColumns[i], sortStates[i]))
                            sortStates[i] = TableFmt.NextSort(sortStates[i]);
                    }
                    // 数据行（真实重排：以当前激活列排序）
                    int active = -1, dir = 0;
                    for (int i = 0; i < sortStates.Length; i++)
                        if (sortStates[i] != SortState.None) { active = i; dir = sortStates[i] == SortState.Asc ? 1 : -1; }
                    int[] order = { 0, 1, 2, 3, 4, 5 };
                    if (active > 0)
                    {
                        float[] key = active == 1 ? cellMood : active == 2 ? Array.ConvertAll(cellWork, v => (float)v) : Array.ConvertAll(cellKill, v => (float)v);
                        Array.Sort(order, (a, b) => (key[a].CompareTo(key[b])) * dir);
                    }
                    var rows = Layout.Column(new Rect(r.x, r.y + 22f, r.width, r.height - 22f), 0f);
                    for (int k = 0; k < order.Length; k++)
                    {
                        int idx = order[k];
                        var line = Layout.Row(rows.Take(21f), 0f);
                        Draw.EllipsisLabel(line.Take(r.width * w[0]), Names[idx], GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                        TableAtoms.ValueCell(line.Take(r.width * w[1]), cellMood[idx], 100f);          // 阈值着色：值 / 参照值
                        TableAtoms.ValueCell(line.Take(r.width * w[2]), cellWork[idx], 45000f);
                        TableAtoms.ValueCell(line.Take(r.width * w[3]), cellKill[idx], 20f);
                        Draw.LineH(r.x, rows.Y - 1f, r.width, Tokens.BorderHair);
                    }
                }
            });

            f.Add(new Atom
            {
                2525No = "26"No = "26", Name = "ColumnResize 列宽手柄", Api = "TableAtoms.ResizeHandle(Rect area, float x) → bool · TableFmt.ResizeWidth",
                Interactive = true,
                Desc = "列间竖向手柄，按住横向拖动调列宽（宽度存列配置）。MouseDrag 事件 + clamp。",
                Note = "按住竖手柄横向拖 · 预览 .colres{width:8px}（基准表 §1.4）",
                BodyH = 40f, MinW = 8f, MinH = 18f,
                Paint = r =>
                {
                    var area = new Rect(r.x, r.y, r.width, 34f);
                    Draw.Box(area, Tokens.Card);
                    Draw.Border(area, Tokens.BorderSoft);
                    int split = Mathf.RoundToInt(area.width * resizeX);
                    Draw.EllipsisLabel(new Rect(area.x + 8f, area.y, split, area.height), "名字列", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                    Draw.EllipsisLabel(new Rect(area.x + split + 12f, area.y, area.width - split - 14f, area.height), "数值列 " + Mathf.RoundToInt(resizeX * 100f) + "%", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                    if (TableAtoms.ResizeHandle(area, resizeX))
                    {
                        if (Event.current != null && Event.current.type == EventType.MouseDrag)
                            resizeX = Mathf.Clamp(TableFmt.ResizeWidth(resizeX + Event.current.delta.x / Mathf.Max(1f, area.width), 0.15f, 0.85f, 0f), 0.15f, 0.85f);
                    }
                }
            });

            f.Add(new Atom
            {
                , Name = "ColumnResize 列宽手柄", Api = "TableAtoms.ResizeHandle(Rect area, float x) → bool · TableFmt.ResizeWidth",
                Interactive = true,
                Desc = "列间竖向手柄，按住横向拖动调列宽（宽度存列配置）。MouseDrag 事件 + clamp。",
                Note = "按住竖手柄横向拖 · 预览 .colres{width:8px}（基准表 §1.4）",
                BodyH = 40f,
                Paint = r =>
                {
                    var area = new Rect(r.x, r.y, r.width, 34f);
                    Draw.Box(area, Tokens.Card);
                    Draw.Border(area, Tokens.BorderSoft);
                    int split = Mathf.RoundToInt(area.width * resizeX);
                    Draw.EllipsisLabel(new Rect(area.x + 8f, area.y, split, area.height), "名字列", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                    Draw.EllipsisLabel(new Rect(area.x + split + 12f, area.y, area.width - split - 14f, area.height), "数值列 " + Mathf.RoundToInt(resizeX * 100f) + "%", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                    if (TableAtoms.ResizeHandle(area, resizeX))
                    {
                        if (Event.current != null && Event.current.type == EventType.MouseDrag)
                            resizeX = Mathf.Clamp(TableFmt.ResizeWidth(resizeX + Event.current.delta.x / Mathf.Max(1f, area.width), 0.15f, 0.85f, 0f), 0.15f, 0.85f);
                    }
                }
            });

            f.Add(new Atom
            {
                2727No = "28"No = "28", Name = "DensitySwitch 密度三档", Api = "TableAtoms.DensitySwitch(Rect, ref RowDensity) → bool · TableFmt.RowHeight",
                Interactive = true,
                Desc = "紧凑 / 标准 / 舒适三档行高全局切换（22 / 28 / 34f），大表格按屏幅自选——Work Tab 缩放实证。",
                Note = "返回 true 表示档位被切换；行高由 TableFmt.RowHeight 派生，不写死数字",
                BodyH = 124f, MinW = 176f, MinH = 26f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 6f);
                    if (TableAtoms.DensitySwitch(col.Take(26f), ref density)) { /* 档位已切换，下方按新档重排 */ }
                    float rh = TableFmt.RowHeight(density);
                    var rows = Layout.Column(col.Take(90f), 0f);
                    for (int i = 0; i < 6 && rows.Rest.height > rh * 0.5f; i++)
                    {
                        var line = Layout.Row(rows.Take(rh), 0f);
                        Draw.EllipsisLabel(line.Take(r.width * 0.6f), WorkTypes[i], GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                        Draw.EllipsisLabel(line.Rest, (i + 1) + " 号位", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleRight);
                        Draw.LineH(r.x, rows.Y - 1f, r.width, Tokens.BorderHair);
                    }
                }
            });

            families.Add(f);
        }

        // ─────────────────────────────────────── A0+ · 面板原子族 4 件
        private void BuildPanel()
        {
            var f = new Family { Title = "A0+ · 面板原子族", Sub = "4 件 · PanelAtoms（RimHUD 实证）" };

            f.Add(new Atom
            {
                , Name = "DensitySwitch 密度三档", Api = "TableAtoms.DensitySwitch(Rect, ref RowDensity) → bool · TableFmt.RowHeight",
                Interactive = true,
                Desc = "紧凑 / 标准 / 舒适三档行高全局切换（22 / 28 / 34f），大表格按屏幅自选——Work Tab 缩放实证。",
                Note = "返回 true 表示档位被切换；行高由 TableFmt.RowHeight 派生，不写死数字",
                BodyH = 124f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 6f);
                    if (TableAtoms.DensitySwitch(col.Take(26f), ref density)) { /* 档位已切换，下方按新档重排 */ }
                    float rh = TableFmt.RowHeight(density);
                    var rows = Layout.Column(col.Take(90f), 0f);
                    for (int i = 0; i < 6 && rows.Rest.height > rh * 0.5f; i++)
                    {
                        var line = Layout.Row(rows.Take(rh), 0f);
                        Draw.EllipsisLabel(line.Take(r.width * 0.6f), WorkTypes[i], GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                        Draw.EllipsisLabel(line.Rest, (i + 1) + " 号位", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleRight);
                        Draw.LineH(r.x, rows.Y - 1f, r.width, Tokens.BorderHair);
                    }
                }
            });

            families.Add(f);
        }

        // ─────────────────────────────────────── A0+ · 面板原子族 4 件
        private void BuildPanel()
        {
            var f = new Family { Title = "A0+ · 面板原子族", Sub = "4 件 · PanelAtoms（RimHUD 实证）" };

            f.Add(new Atom
            {
                2929No = "30"No = "30", Name = "MiniProfileChip 迷你档案条", Api = "PanelAtoms.MiniProfileChip(Rect, string initial, string name, float mood01, float health01)",
                Desc = "单行高密档案：微头像 + 名 + 双 micro 条（心情 / 健康）——事件流与队列行内嵌 pawn 信息。",
                Note = "两条 micro 条分别走心情 / 健康色阶；数据随帧漂移",
                // ⚠ 契约：MiniProfileChip 内部 = Inset(4,2) + Column[名字行, Row[两条 MiniBar]]，
                //   名字行高 = max(12, rect.height × 0.5)，而名字走 Tiny 需 18 ⇒ rect 高必须 ≥ 36，
                //   否则名字行只有 12 高、溢出被下方 mini 条盖住（本轮事故）。
                BodyH = 76f, MinW = 120f, MinH = 36f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 4f);
                    PanelAtoms.MiniProfileChip(col.Take(36f), "蓝", Names[0], 0.72f + 0.04f * Mathf.Sin(t * 0.9f), 0.94f);
                    PanelAtoms.MiniProfileChip(col.Take(36f), "赤", Names[1], 0.38f + 0.05f * Mathf.Sin(t * 1.3f), 0.81f);
                }
            });

            f.Add(new Atom
            {
                , Name = "MiniProfileChip 迷你档案条", Api = "PanelAtoms.MiniProfileChip(Rect, string initial, string name, float mood01, float health01)",
                Desc = "单行高密档案：微头像 + 名 + 双 micro 条（心情 / 健康）——事件流与队列行内嵌 pawn 信息。",
                Note = "两条 micro 条分别走心情 / 健康色阶；数据随帧漂移",
                // ⚠ 契约：MiniProfileChip 内部 = Inset(4,2) + Column[名字行, Row[两条 MiniBar]]，
                //   名字行高 = max(12, rect.height × 0.5)，而名字走 Tiny 需 18 ⇒ rect 高必须 ≥ 36，
                //   否则名字行只有 12 高、溢出被下方 mini 条盖住（本轮事故）。
                BodyH = 76f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 4f);
                    PanelAtoms.MiniProfileChip(col.Take(36f), "蓝", Names[0], 0.72f + 0.04f * Mathf.Sin(t * 0.9f), 0.94f);
                    PanelAtoms.MiniProfileChip(col.Take(36f), "赤", Names[1], 0.38f + 0.05f * Mathf.Sin(t * 1.3f), 0.81f);
                }
            });

            f.Add(new Atom
            {
                3131No = "32"No = "32", Name = "SectionBar 面板分段条", Api = "PanelAtoms.SectionBar(Rect, string title, string count, bool expanded)",
                Desc = "HUD 面板的薄分段头（可折叠）：高 20f，标题居左 + 计数 + 折叠箭头，比 SectionTitle 更密。",
                Note = "计数取官方警报 / 想法 / 历史记录器词表 · 点条折叠（交互回补）",
                BodyH = 118f, MinW = 90f, MinH = 18f,   // 3 条（含间隙）+ 最多 3 行展开体
                Paint = r =>
                {
                    string[] titles = { "警报", "想法", "历史记录器" };
                    string[] counts = { "11 项", "15 项", "11 项" };
                    string[] body =
                    {
                        "食物不足 / 药品不足 / 崩溃风险（高）/ 衣衫褴褛 …",
                        "很高的期望 / 合理的期望 / 大麻亢奋 / 宿醉（轻微）…",
                        "殖民者心情 / 财富（物品）/ 威胁点数 / 人口适应 …"
                    };
                    var col = Layout.Column(r, 3f);
                    for (int i = 0; i < titles.Length; i++)
                    {
                        Rect bar = col.Take(19f);
                        PanelAtoms.SectionBar(bar, titles[i], counts[i], secOpen[i]);
                        HoverFrame(bar);                           // 本体悬停：鼠标在哪一条上，哪一条描 accent 边
                        // 交互回补：点条折叠 —— SectionBar 只换 ▾/▸ 字形，**折叠体由调用方画**
                        if (Widgets.ButtonInvisible(bar)) secOpen[i] = !secOpen[i];
                        if (secOpen[i]) Draw.EllipsisLabel(col.Take(LineTiny), body[i], GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                    }
                }
            });

            families.Add(f);
        }

        // ─────────────────────────────────────── A0+ · 交互原子族 4 件
        private void BuildInteract()
        {
            var f = new Family { Title = "A0+ · 交互原子族", Sub = "4 件 · InteractAtoms（Mod Manager / DragList 实证）" };

            f.Add(new Atom
            {
                , Name = "SectionBar 面板分段条", Api = "PanelAtoms.SectionBar(Rect, string title, string count, bool expanded)",
                Desc = "HUD 面板的薄分段头（可折叠）：高 20f，标题居左 + 计数 + 折叠箭头，比 SectionTitle 更密。",
                Note = "计数取官方警报 / 想法 / 历史记录器词表 · 点条折叠（交互回补）",
                BodyH = 118f,   // 3 条（含间隙）+ 最多 3 行展开体
                Paint = r =>
                {
                    string[] titles = { "警报", "想法", "历史记录器" };
                    string[] counts = { "11 项", "15 项", "11 项" };
                    string[] body =
                    {
                        "食物不足 / 药品不足 / 崩溃风险（高）/ 衣衫褴褛 …",
                        "很高的期望 / 合理的期望 / 大麻亢奋 / 宿醉（轻微）…",
                        "殖民者心情 / 财富（物品）/ 威胁点数 / 人口适应 …"
                    };
                    var col = Layout.Column(r, 3f);
                    for (int i = 0; i < titles.Length; i++)
                    {
                        Rect bar = col.Take(19f);
                        PanelAtoms.SectionBar(bar, titles[i], counts[i], secOpen[i]);
                        HoverFrame(bar);                           // 本体悬停：鼠标在哪一条上，哪一条描 accent 边
                        // 交互回补：点条折叠 —— SectionBar 只换 ▾/▸ 字形，**折叠体由调用方画**
                        if (Widgets.ButtonInvisible(bar)) secOpen[i] = !secOpen[i];
                        if (secOpen[i]) Draw.EllipsisLabel(col.Take(LineTiny), body[i], GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                    }
                }
            });

            families.Add(f);
        }

        // ─────────────────────────────────────── A0+ · 交互原子族 4 件
        private void BuildInteract()
        {
            var f = new Family { Title = "A0+ · 交互原子族", Sub = "4 件 · InteractAtoms（Mod Manager / DragList 实证）" };

            f.Add(new Atom
            {
                3333No = "34"No = "34", Name = "EyeToggle 可见性切换", Api = "InteractAtoms.EyeToggle(Rect, bool visible, Color?) → bool",
                Interactive = true,
                Desc = "切换行 / 列可见性（隐藏态半透明 + 斜杠），列配置、图层显隐、面板模块开关通用。",
                Note = "返回新状态由调用方回写（组件自身零状态）· 预览 .iconbtn{28×28}",
                BodyH = 132f, MinW = 18f, MinH = 18f,
                Paint = r =>
                {
                    string[] tabs = { "角色", "健康", "需求", "装备", "社交", "日志" };
                    var col = Layout.Column(r, 3f);
                    for (int i = 0; i < tabs.Length; i++)
                    {
                        var line = Layout.Row(col.Take(19f), 7f);
                        Rect eye = line.Take(24f);          // HStack 成员是 Rest/X/Y，字段私有 ⇒ 用 Take 取整行
                        bool vis = i == 0 ? eyeOn : i != 2;
                        if (InteractAtoms.EyeToggle(eye, vis) && i == 0) eyeOn = !eyeOn;
                        // 铁律 2：标签走单元矩形（line 游标已含 7px 间隔），不用绝对坐标算宽
                        Draw.EllipsisLabel(line.Rest,
                            tabs[i] + " 页签" + (vis ? "" : "（已隐藏）"), GameFont.Tiny, vis ? Tokens.Text : Tokens.Dim, TextAnchor.MiddleLeft);
                    }
                }
            });

            f.Add(new Atom
            {
                , Name = "EyeToggle 可见性切换", Api = "InteractAtoms.EyeToggle(Rect, bool visible, Color?) → bool",
                Interactive = true,
                Desc = "切换行 / 列可见性（隐藏态半透明 + 斜杠），列配置、图层显隐、面板模块开关通用。",
                Note = "返回新状态由调用方回写（组件自身零状态）· 预览 .iconbtn{28×28}",
                BodyH = 132f,
                Paint = r =>
                {
                    string[] tabs = { "角色", "健康", "需求", "装备", "社交", "日志" };
                    var col = Layout.Column(r, 3f);
                    for (int i = 0; i < tabs.Length; i++)
                    {
                        var line = Layout.Row(col.Take(19f), 7f);
                        Rect eye = line.Take(24f);          // HStack 成员是 Rest/X/Y，字段私有 ⇒ 用 Take 取整行
                        bool vis = i == 0 ? eyeOn : i != 2;
                        if (InteractAtoms.EyeToggle(eye, vis) && i == 0) eyeOn = !eyeOn;
                        // 铁律 2：标签走单元矩形（line 游标已含 7px 间隔），不用绝对坐标算宽
                        Draw.EllipsisLabel(line.Rest,
                            tabs[i] + " 页签" + (vis ? "" : "（已隐藏）"), GameFont.Tiny, vis ? Tokens.Text : Tokens.Dim, TextAnchor.MiddleLeft);
                    }
                }
            });

            f.Add(new Atom
            {
                3535No = "36"No = "36", Name = "ToolbarIconButton 图标工具钮", Api = "InteractAtoms.ToolbarIcon(Rect, string glyph, bool active) → bool",
                Interactive = true,
                Desc = "方形单图标钮（激活态 accent 底 + 2px 内描边），悬停 Tooltip、长按可弹出备选。",
                Note = "按钮本体即 28×28（预览 .iconbtn）· 首钮可切换激活态",
                BodyH = 30f, MinW = 28f, MinH = 28f,
                Paint = r =>
                {
                    var row = Layout.Row(r, 5f);
                    string[] glyphs = { "E", "M", "D", "L", "Z" };
                    for (int i = 0; i < glyphs.Length; i++)
                    {
                        Rect b = row.Take(28f);                                  // .iconbtn{28×28} L306
                        if (b.width < 20f) break;
                        bool on = iconOn && i == 0;
                        if (InteractAtoms.ToolbarIcon(b, glyphs[i], on) && i == 0) iconOn = !iconOn;
                    }
                }
            });

            families.Add(f);
        }

        // ─────────────────────────────────────── A0+ · 进度与容量 3 件
        private void BuildProgress()
        {
            var f = new Family { Title = "A0+ · 进度与容量", Sub = "3 件 · ProgressAtoms" };

            f.Add(new Atom
            {
                , Name = "ToolbarIconButton 图标工具钮", Api = "InteractAtoms.ToolbarIcon(Rect, string glyph, bool active) → bool",
                Interactive = true,
                Desc = "方形单图标钮（激活态 accent 底 + 2px 内描边），悬停 Tooltip、长按可弹出备选。",
                Note = "按钮本体即 28×28（预览 .iconbtn）· 首钮可切换激活态",
                BodyH = 30f,
                Paint = r =>
                {
                    var row = Layout.Row(r, 5f);
                    string[] glyphs = { "E", "M", "D", "L", "Z" };
                    for (int i = 0; i < glyphs.Length; i++)
                    {
                        Rect b = row.Take(28f);                                  // .iconbtn{28×28} L306
                        if (b.width < 20f) break;
                        bool on = iconOn && i == 0;
                        if (InteractAtoms.ToolbarIcon(b, glyphs[i], on) && i == 0) iconOn = !iconOn;
                    }
                }
            });

            families.Add(f);
        }

        // ─────────────────────────────────────── A0+ · 进度与容量 3 件
        private void BuildProgress()
        {
            var f = new Family { Title = "A0+ · 进度与容量", Sub = "3 件 · ProgressAtoms" };

            f.Add(new Atom
            {
                3737No = "38"No = "38", Name = "CapacityBar 容量叠印条", Api = "ProgressAtoms.CapacityBar(Rect, float used, float capacity, Color?)",
                Desc = "容量占用叠印（搬运 / 仓储 / 背包）：接近满载变黄，超载时右端出现红色溢出竖块。",
                Note = "⚠ 契约：默认色 = ratio>0.85 ? Warn : Alive（非 Accent）；超载溢出 = 4 个红竖块（非虚线）⇒ 传 null 让组件自表达",
                BodyH = 66f, MinW = 2f, MinH = 2f,
                Paint = r =>
                {
                    string[] labels = { "背包", "仓库", "冷藏柜" };
                    float[] used = { 780f, 1000f, 1180f };
                    var col = Layout.Column(r, 4f);
                    for (int i = 0; i < labels.Length; i++)
                    {
                        var line = Layout.Row(col.Take(18f), 8f);
                        Draw.EllipsisLabel(line.Take(44f), labels[i], GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                        Rect bar = line.Take(line.Rest.width - 46f);
                        ProgressAtoms.CapacityBar(bar, used[i], 1000f, null);        // 不传色：走组件自带的 Alive/Warn 语义
                        Draw.EllipsisLabel(line.Rest, Mathf.RoundToInt(used[i] / 10f) + "%", GameFont.Tiny, i == 2 ? Tokens.Dead : Tokens.Muted, TextAnchor.MiddleRight);
                    }
                }
            });

            f.Add(new Atom
            {
                , Name = "CapacityBar 容量叠印条", Api = "ProgressAtoms.CapacityBar(Rect, float used, float capacity, Color?)",
                Desc = "容量占用叠印（搬运 / 仓储 / 背包）：接近满载变黄，超载时右端出现红色溢出竖块。",
                Note = "⚠ 契约：默认色 = ratio>0.85 ? Warn : Alive（非 Accent）；超载溢出 = 4 个红竖块（非虚线）⇒ 传 null 让组件自表达",
                BodyH = 66f,
                Paint = r =>
                {
                    string[] labels = { "背包", "仓库", "冷藏柜" };
                    float[] used = { 780f, 1000f, 1180f };
                    var col = Layout.Column(r, 4f);
                    for (int i = 0; i < labels.Length; i++)
                    {
                        var line = Layout.Row(col.Take(18f), 8f);
                        Draw.EllipsisLabel(line.Take(44f), labels[i], GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                        Rect bar = line.Take(line.Rest.width - 46f);
                        ProgressAtoms.CapacityBar(bar, used[i], 1000f, null);        // 不传色：走组件自带的 Alive/Warn 语义
                        Draw.EllipsisLabel(line.Rest, Mathf.RoundToInt(used[i] / 10f) + "%", GameFont.Tiny, i == 2 ? Tokens.Dead : Tokens.Muted, TextAnchor.MiddleRight);
                    }
                }
            });

            f.Add(new Atom
            {
                3939No = "40"No = "40", Name = "PointCostBadge 点数徽章", Api = "MiscAtoms.PointCostBadge(Rect, int budget, int spent)",
                Desc = "预算点数徽章（剩余 / 用尽 / 超支三态自动换色），角色编辑器式配额展示；预算管理类界面通用。",
                Note = "⚠ 契约：参数序是 (budget, spent)；宽高即徽章本体 ⇒ 按 .pill 取「文本宽 + 18」",
                BodyH = 68f, MinW = 60f, MinH = 22f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 6f);
                    const int budget = 200;
                    // 交互回补：± 按钮。到界时 active:false → **原版自带的禁用外观**（唯一不必压暗的禁用通道）
                    var line = Layout.Row(col.Take(24f), 6f);
                    // .pill{padding:1px 9px} L214 ⇒ 宽 = 文本宽 + 18，高度 22
                    float w = Mathf.Min(r.width, Text.CalcSize("剩余 128 点").x + 18f);
                    Rect slot = line.Take(w);
                    if (Widgets.ButtonText(line.Take(26f), "−", true, true, budgetSpent > 0)) budgetSpent = Mathf.Max(0, budgetSpent - 16);
                    if (Widgets.ButtonText(line.Take(26f), "+", true, true, budgetSpent < budget + 40)) budgetSpent = Mathf.Min(budget + 40, budgetSpent + 16);
                    MiscAtoms.PointCostBadge(new Rect(slot.x, slot.y, Mathf.Min(w, slot.width), slot.height), budget, budgetSpent);
                    var bar = col.Take(11f);                                          // .kbar 高 11 L207
                    // D15 收尾：预算条**有阈值语义**（超支）⇒ 改用 CapacityBar，走它自带的
                    // 「>0.85 转 Warn + 超载右端 4 个红竖块」，不再手写三元色（那是「语义色被手写替代」）
                    ProgressAtoms.CapacityBar(bar, budgetSpent, budget, null);
                    Draw.EllipsisLabel(col.Take(LineTiny), "预算 " + budget + " · 已分配 " + budgetSpent,
                        GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                , Name = "PointCostBadge 点数徽章", Api = "MiscAtoms.PointCostBadge(Rect, int budget, int spent)",
                Desc = "预算点数徽章（剩余 / 用尽 / 超支三态自动换色），角色编辑器式配额展示；预算管理类界面通用。",
                Note = "⚠ 契约：参数序是 (budget, spent)；宽高即徽章本体 ⇒ 按 .pill 取「文本宽 + 18」",
                BodyH = 68f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 6f);
                    const int budget = 200;
                    // 交互回补：± 按钮。到界时 active:false → **原版自带的禁用外观**（唯一不必压暗的禁用通道）
                    var line = Layout.Row(col.Take(24f), 6f);
                    // .pill{padding:1px 9px} L214 ⇒ 宽 = 文本宽 + 18，高度 22
                    float w = Mathf.Min(r.width, Text.CalcSize("剩余 128 点").x + 18f);
                    Rect slot = line.Take(w);
                    if (Widgets.ButtonText(line.Take(26f), "−", true, true, budgetSpent > 0)) budgetSpent = Mathf.Max(0, budgetSpent - 16);
                    if (Widgets.ButtonText(line.Take(26f), "+", true, true, budgetSpent < budget + 40)) budgetSpent = Mathf.Min(budget + 40, budgetSpent + 16);
                    MiscAtoms.PointCostBadge(new Rect(slot.x, slot.y, Mathf.Min(w, slot.width), slot.height), budget, budgetSpent);
                    var bar = col.Take(11f);                                          // .kbar 高 11 L207
                    // D15 收尾：预算条**有阈值语义**（超支）⇒ 改用 CapacityBar，走它自带的
                    // 「>0.85 转 Warn + 超载右端 4 个红竖块」，不再手写三元色（那是「语义色被手写替代」）
                    ProgressAtoms.CapacityBar(bar, budgetSpent, budget, null);
                    Draw.EllipsisLabel(col.Take(LineTiny), "预算 " + budget + " · 已分配 " + budgetSpent,
                        GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                4141No = "42"No = "42", Name = "ThinScrollBar 细滚动条", Api = "MiscAtoms.ThinScrollBar(Rect viewRect, float contentH, float scrollY, float width)",
                Desc = "自绘细滚动条（细轨 + 圆角滑块），替代原版宽灰条——深色主题下违和感根除。",
                Note = "⚠ 契约：条画在 viewRect.xMax + 1f（rect 外侧）；contentH ≤ height 时完全不画 ⇒ 传视口 rect",
                BodyH = 76f, MinW = 120f, MinH = 40f,
                Paint = r =>
                {
                    // 视口 rect 宽度要留出条的位置（条落在右侧外 1px）
                    var view = new Rect(r.x, r.y, Mathf.Max(40f, r.width - 12f), 76f);
                    const float contentH = 220f;
                    float maxScroll = Mathf.Max(1f, contentH - view.height);
                    Draw.Box(view, Tokens.Panel);
                    Draw.Border(view, Tokens.BorderSoft);
                    // 交互回补：条可拖。命中区 = view 右侧 +1、宽 8（与 ThinScrollBar 内部落点一致）；
                    // **去掉自动滚动** —— 自动滚动会与拖动互相打架。
                    var track = new Rect(view.xMax + 1f, view.y, 8f, view.height);
                    if (Mouse.IsOver(track) && Event.current.isMouse && Event.current.button == 0
                        && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
                    {
                        thinScroll = Mathf.Clamp01((Event.current.mousePosition.y - view.y) / Mathf.Max(1f, view.height)) * maxScroll;
                    }
                    // 本体悬停：细条命中区只有 8px 宽，描边能显著提升「这里可以拖」的可发现性
                    HoverFrame(track);
                    // 按滚动位置切片显示（只画可见行，不「画完再裁」）
                    var col = Layout.Column(new Rect(view.x + 4f, view.y + 3f, view.width - 8f, view.height - 6f), 2f);
                    int first = Mathf.Clamp(Mathf.FloorToInt(thinScroll / LineTiny), 0, EventWords.Length - 1);
                    for (int i = first; i < EventWords.Length && col.Rest.height >= LineTiny; i++)
                        Draw.EllipsisLabel(col.Take(LineTiny), EventWords[i] + "　" + (120 + i * 37), GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                    MiscAtoms.ThinScrollBar(view, contentH, thinScroll, 8f);
                }
            });

            families.Add(f);
        }
    }
}
, Name = "ThinScrollBar 细滚动条", Api = "MiscAtoms.ThinScrollBar(Rect viewRect, float contentH, float scrollY, float width)",
                Desc = "自绘细滚动条（细轨 + 圆角滑块），替代原版宽灰条——深色主题下违和感根除。",
                Note = "⚠ 契约：条画在 viewRect.xMax + 1f（rect 外侧）；contentH ≤ height 时完全不画 ⇒ 传视口 rect",
                BodyH = 76f,
                Paint = r =>
                {
                    // 视口 rect 宽度要留出条的位置（条落在右侧外 1px）
                    var view = new Rect(r.x, r.y, Mathf.Max(40f, r.width - 12f), 76f);
                    const float contentH = 220f;
                    float maxScroll = Mathf.Max(1f, contentH - view.height);
                    Draw.Box(view, Tokens.Panel);
                    Draw.Border(view, Tokens.BorderSoft);
                    // 交互回补：条可拖。命中区 = view 右侧 +1、宽 8（与 ThinScrollBar 内部落点一致）；
                    // **去掉自动滚动** —— 自动滚动会与拖动互相打架。
                    var track = new Rect(view.xMax + 1f, view.y, 8f, view.height);
                    if (Mouse.IsOver(track) && Event.current.isMouse && Event.current.button == 0
                        && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
                    {
                        thinScroll = Mathf.Clamp01((Event.current.mousePosition.y - view.y) / Mathf.Max(1f, view.height)) * maxScroll;
                    }
                    // 本体悬停：细条命中区只有 8px 宽，描边能显著提升「这里可以拖」的可发现性
                    HoverFrame(track);
                    // 按滚动位置切片显示（只画可见行，不「画完再裁」）
                    var col = Layout.Column(new Rect(view.x + 4f, view.y + 3f, view.width - 8f, view.height - 6f), 2f);
                    int first = Mathf.Clamp(Mathf.FloorToInt(thinScroll / LineTiny), 0, EventWords.Length - 1);
                    for (int i = first; i < EventWords.Length && col.Rest.height >= LineTiny; i++)
                        Draw.EllipsisLabel(col.Take(LineTiny), EventWords[i] + "　" + (120 + i * 37), GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                    MiscAtoms.ThinScrollBar(view, contentH, thinScroll, 8f);
                }
            });

            families.Add(f);
        }
    }
}
