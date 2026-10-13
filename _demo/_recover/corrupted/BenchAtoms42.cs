using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// 42 件基础原子 · 逐件验证台（预览 1:1 落地版）。
    ///
    /// <para><b>1:1 的定义（LAND-002）</b>：不是像素绝对相等，而是<b>内容宽基准对齐</b>——
    /// 同档位下卡数 N 相同、卡内结构相同、字号档位相同。
    /// 预览侧单卡宽 <c>c_p = (W_p − 14×(N−1)) / N</c>，游戏侧同式由可用宽派生。</para>
    ///
    /// <para><b>结构对齐（本轮修正的关键）</b>：HTML 的原子是 <c>.comp</c> <b>卡片网格</b>
    /// （<c>.grid{repeat(auto-fill,minmax(400px,1fr));gap:14px}</c> 行 156），
    /// 一张卡 = <c>.ch</c> 卡头 + <c>.desc</c> 描述 + <c>.demo</c> 演示容器（行 161/179/181）。
    /// 上一版做成「左元信息 + 右通栏演示」的行布局，演示区被拉到 ~700px，
    /// 于是 <c>PointCostBadge</c>/<c>BubbleTag</c>/<c>CapacityBar</c> 这类「紧贴内容」的原子一律被拉变形
    /// —— 结构不对，视觉必然不像。本版按卡片网格重建。</para>
    ///
    /// <para><b>几何与字号全部来自《原子落地基准表》</b>（docs/02-组件图鉴/原子落地基准表.md），
    /// 该表由预览 CSS 的选择器 + 行号反读得出（LAND-001）。所有常数在下方以注释标注行号来源。</para>
    /// </summary>
    internal sealed partial class BenchAtoms42 : IBench
    {
        // ── 卡片规格（LAND-001：全部标注预览 CSS 行号） ────────────────────
        private const float CardGap = 14f;      // .grid{gap:14px}                L156
        private const float PadX = 12f;         // .ch/.desc/.demo 共同横向内边距  L161/179/181
        private const float DescTop = 9f;       // .comp .desc{padding:9px 12px 0} L179
        private const float DemoPad = 11f;      // .demo{padding:11px}            L181
        private const float DemoMT = 10f;       // .demo{margin:10px 12px 12px}   L181
        private const float DemoMB = 12f;       //                                L181
        private const float NoteGap = 7f;       // .demo .dnote{margin-top:7px}   L183
        private const float Border = 2f;        // .comp{border:1px} ×2           L157

        // ── 行高一律取自框架度量，⛔ 不得手写魔数 ──────────────────────────
        // 事故记录：曾把 Tiny 行高写死为 14f，而实际是 18f —— 文字溢出分配高度 4px，
        // 被紧接着绘制的 .demo 底色盖住，表现为「全部组件文字下半截被遮盖」。
        // Measure 类文档原话：「文本被硬切、组件与标题错位，根因都是行高靠手写魔数」。
        private static readonly float LineTiny = Measure.RowHeight(GameFont.Tiny);      // 18
        private static readonly float LineSmall = Measure.RowHeight(GameFont.Small);    // 22
        private static readonly float CtrlTiny = Measure.ControlHeight(GameFont.Tiny);  // 26（可点行高）

        /// <summary>卡头高 = 上下内边距 9 + 名称行(Small) + 签名上边距 2 + 签名行(Tiny)。</summary>
        private static readonly float HeadH = 9f + LineSmall + 2f + LineTiny;           // 60

        /// <summary>单卡最小可读宽：预览 <c>minmax(400px,1fr)</c> → 400 + 14 间隔。LAND-002/005</summary>
        private const float CardUnit = 414f;

        /// <summary>演示容器底色 = <c>color-mix(window 55%, panel)</c>（L181）——引擎无 color-mix，预计算常量。</summary>
        private static readonly Color DemoBg = new Color(
            Tokens.Window.r * 0.55f + Tokens.Panel.r * 0.45f,
            Tokens.Window.g * 0.55f + Tokens.Panel.g * 0.45f,
            Tokens.Window.b * 0.55f + Tokens.Panel.b * 0.45f);

        private sealed class Atom
        {
            public string No;
            public string Name;
            public string Api;          // .sig  → Tiny + Info
            public string Desc = "";    // .desc → Tiny + Muted
            public string Note = "";    // .dnote → Tiny + Dim（可为空）
            public string Badge = "ok"; // ✅ / ⚠️
            public int DescLines = 1;
            public float BodyH = 26f;   // .demo 内「演示主体」高（不含注脚）
            /// <summary>
            /// 最小可读尺寸（skill 铁律 4 / `07` 册 §2）。低于此值走 **D6**：画「— + 原因」占位，
            /// **不硬挤**。默认值覆盖纯文本件；有内部公式下限的件**必须显式声明**
            /// （如 `HotkeyBind` 内部固定总宽 254、`SliderTicks` 宽需 ≥178）。
            /// </summary>
            public float MinW = 60f;
            public float MinH = LineTiny;
            /// <summary>
            /// 该件是否有交互路径（**显式声明**，不靠嗅探源码）。用于在悬停时给演示区强化边框 ——
            /// 让「这块能操作」在鼠标移入时就看得出来（skill 红线 2 · 反馈 · 黑名单 #2）。
            /// </summary>
            public bool Interactive;
            /// <summary>在 demo 内边距之内绘制演示主体（右上角为注脚区，勿越界）。</summary>
            public Action<Rect> Paint;
        }

        private sealed class Family
        {
            public string Title;
            public string Sub;
            public readonly List<Atom> Items = new List<Atom>();
            public void Add(Atom a) { Items.Add(a); }
        }

        private readonly List<Family> families = new List<Family>();
        private readonly List<Atom> cards = new List<Atom>();     // 展平后的全部卡片（供布局与检索）
        private readonly HashSet<string> failed = new HashSet<string>();

        // ── 交互状态（全部由本面持有，框架零状态） ──────────────────────────
        internal static readonly string[] TableColumns = { "名字", "心情", "工时", "击杀" };
        private readonly SortState[] sortStates = new SortState[TableColumns.Length];
        private float t;
        private int numMode;
        private float typeProgress;
        private float swatchSel;
        private bool pinA, pinB, pinC;
        private bool eyeOn;
        private bool iconOn;
        private bool caretOpen;
        private bool moreBelow = true;
        private RowDensity density = RowDensity.Normal;
        private float resizeX = 0.42f;
        private float zoom = 0.86f;
        private float wipe;
        private float flash;
        private bool gripOn;
        private float spinP;
        private bool spinDone;

        // ── 交互回补批（18 件，2026-09-13）的状态：全部由本面持有，原子零状态 ──
        //    约定：**状态字段随件声明** —— 转一件加一件。一次性声明全部会留下「字段未使用」警告（CS0169/0414），
        //    而本工程标准是 0 错 0 警。
        private int tsMode;                    // 02 时间戳档（绝对 / 相对 / 混合）
        private int glyphIdx;                  // 14 语义读数（点图形切换）
        private int aspectIdx = 1;             // 18 比例档
        private float gradHover = -1f;         // 21 图例悬停位置（-1 = 未悬停）
        private int alertLv = 2;               // 31 告警档

        // ── 布局计划缓存（LAND-011：键 = 可用宽） ──────────────────────────
        private float planW = -1f;
        private int planCols;

        public bool Scrollable => true;
        public float Height(float width) { Plan(width); return planTotal + Border; }

        private float planTotal;

        /// <summary>按可用宽派生列数与总高（LAND-011：仅宽度变化时重算）。</summary>
        private void Plan(float width)
        {
            if (Mathf.Abs(planW - width) < 0.5f) return;
            planW = width;
            planCols = Mathf.Clamp(Mathf.FloorToInt((width + CardGap) / CardUnit), 1, 3);  // LAND-005 三档
            float cardW = (width - CardGap * (planCols - 1)) / planCols;
            float h = 0f;
            for (int i = 0; i < families.Count; i++)
            {
                Family f = families[i];
                h += SecH + LineTiny + (string.IsNullOrEmpty(f.Sub) ? 0f : LineTiny) + 6f;
                int rows = Mathf.CeilToInt(f.Items.Count / (float)planCols);
                for (int r = 0; r < rows; r++)
                {
                    float rowH = 0f;
                    for (int k = r * planCols; k < Mathf.Min((r + 1) * planCols, f.Items.Count); k++)
                        rowH = Mathf.Max(rowH, CardH(f.Items[k], cardW));
                    h += rowH + CardGap;
                }
                h += 10f;
            }
            planTotal = h;
        }

        private const float SecH = 26f;   // h3.sec{font-size:13.5px;margin:26px 0 4px}  L153

        /// <summary>卡高 = 边 + 卡头 + 描述 + demo(margin + padding + 主体 + 注脚)。</summary>
        private static float CardH(Atom a, float cardW)
        {
            float desc = string.IsNullOrEmpty(a.Desc) ? 0f : DescTop + a.DescLines * LineTiny;
            float note = string.IsNullOrEmpty(a.Note) ? 0f : NoteGap + LineTiny;
            return Border + HeadH + desc + DemoMT + (DemoPad * 2f + a.BodyH + note) + DemoMB;
        }

        public BenchAtoms42()
        {
            Build();
            for (int i = 0; i < families.Count; i++)
                for (int k = 0; k < families[i].Items.Count; k++) cards.Add(families[i].Items[k]);
        }

        public void Render(Rect area)
        {
            // 动画推进（纯视觉，暂停也走；CSS 动画在引擎侧的降级实现，见基准表 §1.5）
            t += Time.deltaTime;
            typeProgress = Mathf.Repeat(typeProgress + Time.deltaTime * 0.22f, 1.35f);
            wipe = Mathf.Repeat(wipe + Time.deltaTime * 0.16f, 1.05f);
            flash = Mathf.Repeat(flash + Time.deltaTime * 0.9f, 1f);
            if (!spinDone)
            {
                spinP = Mathf.Min(100f, spinP + Time.deltaTime * 7f);
                if (spinP >= 100f) spinDone = true;
            }

            Plan(area.width);
            float cardW = (area.width - CardGap * (planCols - 1)) / planCols;
            float y = area.y;

            for (int i = 0; i < families.Count; i++)
            {
                Family f = families[i];
                // ── 族标题（h3.sec）+ 计数（.cnt）
                Draw.Label(new Rect(area.x, y, area.width - 120f, SecH), f.Title, GameFont.Small, Tokens.Accent, TextAnchor.MiddleLeft);
                Draw.EllipsisLabel(new Rect(area.xMax - 120f, y, 120f, SecH),
                    (f.Items.Count < 10 ? "0" : "") + f.Items[0].No + "–" + f.Items[f.Items.Count - 1].No,
                    GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleRight);
                y += SecH;
                Draw.LineH(area.x, y, area.width, Tokens.Border);
                y += LineTiny;
                if (!string.IsNullOrEmpty(f.Sub))
                {
                    Draw.EllipsisLabel(new Rect(area.x, y, area.width, LineTiny), f.Sub, GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                    y += LineTiny;
                }
                y += 6f;

                // ── 卡片网格（CSS grid：行高 = 该行最高卡）
                int rows = Mathf.CeilToInt(f.Items.Count / (float)planCols);
                for (int r = 0; r < rows; r++)
                {
                    int from = r * planCols, to = Mathf.Min(from + planCols, f.Items.Count);
                    float rowH = 0f;
                    for (int k = from; k < to; k++) rowH = Mathf.Max(rowH, CardH(f.Items[k], cardW));
                    for (int k = from; k < to; k++)
                    {
                        var rect = new Rect(area.x + (k - from) * (cardW + CardGap), y, cardW, CardH(f.Items[k], cardW));
                        PaintCard(rect, f.Items[k]);
                    }
                    y += rowH + CardGap;
                }
                y += 10f;
            }
        }

        /// <summary>画一张 <c>.comp</c> 卡片：卡头 + 描述 + <c>.demo</c> 容器 + 注脚，并按隔离规则调用演示。</summary>
        private void PaintCard(Rect card, Atom a)
        {
            // 复现预览的 .comp:hover{border-color:var(--accent)}（L159）
            // 一次覆盖全部卡片：让「可指向」有视觉回执（skill 红线 2 · 反馈 · 黑名单 #2）
            bool cardHover = Mouse.IsOver(card);
            Draw.Box(card, cardHover ? Tokens.CardHover : Tokens.Panel);
            Draw.Border(card, cardHover ? Tokens.Accent : Tokens.Border);

            // ── 卡头 .ch（L161：display:flex;gap:8px;padding:9px 12px;background:raised;border-bottom:1px solid border）
            var head = new Rect(card.x + 1f, card.y + 1f, card.width - 2f, HeadH);
            Draw.Box(head, Tokens.PanelRaised);
            Draw.LineH(head.x, head.yMax - 1f, head.width, Tokens.Border);
            var nb = new Rect(head.x + PadX, head.y + 9f, 26f, LineTiny + 1f);   // 统一编号徽标（与网页同号）
            Draw.Box(nb, Tokens.Card);
            Draw.Border(nb, failed.Contains(a.No) ? Tokens.Dead : Tokens.BorderSoft);
            Draw.Label(nb, a.No, GameFont.Tiny, failed.Contains(a.No) ? Tokens.Dead : Tokens.Dim, TextAnchor.MiddleCenter);

            var badge = new Rect(head.xMax - PadX - 34f, head.y + 9f, 34f, LineTiny + 1f);
            Draw.Box(badge, a.Badge == "ok" ? Draw.WithAlpha(Tokens.Alive, 0.14f) : Draw.WithAlpha(Tokens.Warn, 0.14f));
            Draw.Border(badge, a.Badge == "ok" ? Tokens.Alive : Tokens.Warn);
            Draw.Label(badge, a.Badge == "ok" ? "✅" : "⚠️", GameFont.Tiny, a.Badge == "ok" ? Tokens.Alive : Tokens.Warn, TextAnchor.MiddleCenter);

            Draw.EllipsisLabel(new Rect(nb.xMax + 5f, head.y + 9f, badge.x - nb.xMax - 10f, LineSmall), a.Name, GameFont.Small, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.EllipsisLabel(new Rect(head.x + PadX, head.y + 9f + LineSmall + 2f, head.width - PadX * 2f, LineTiny), a.Api, GameFont.Tiny, Tokens.Info, TextAnchor.MiddleLeft);

            // ── 描述 .desc（L179）
            float dy = head.yMax;
            if (!string.IsNullOrEmpty(a.Desc))
            {
                Draw.EllipsisLabel(new Rect(card.x + PadX, dy + DescTop, card.width - PadX * 2f, a.DescLines * LineTiny),
                    a.Desc, GameFont.Tiny, Tokens.Muted, TextAnchor.UpperLeft);
                dy += DescTop + a.DescLines * LineTiny;
            }

            // ── 演示容器 .demo（L181：margin:10px 12px 12px; padding:11px; border:1px dashed border-soft）
            float noteH = string.IsNullOrEmpty(a.Note) ? 0f : NoteGap + LineTiny;
            var demo = new Rect(card.x + PadX, dy + DemoMT, card.width - PadX * 2f, DemoPad * 2f + a.BodyH + noteH);
            Draw.Box(demo, DemoBg);
            // 交互件的演示区在悬停时强化边框（显式声明 Interactive，不嗅探源码）——
            // 让「这块能操作」在鼠标移入时就看得出来（红线 2 · 反馈）
            // 非交互件保持静态边框：不给错误的可操作暗示（预览 .demo 本身无 hover）
            Draw.Border(demo, a.Interactive && Mouse.IsOver(demo) ? Tokens.Accent : Tokens.BorderSoft);

            var body = new Rect(demo.x + DemoPad, demo.y + DemoPad, demo.width - DemoPad * 2f, a.BodyH);
            if (body.width > 6f)
            {
                // 空间断言（skill 铁律 4）：低于最小可读尺寸走 D6 画占位，**不硬挤**（硬挤必然重叠）
                if (body.width < a.MinW || body.height < a.MinH) PaintTooSmall(body, a);
                else if (failed.Contains(a.No)) PaintFailed(body, a);
                else
                {
                    // 隔离：单件异常只标红自己 —— 异常逃出 DoWindowContents 会让 IMGUI 裁剪栈失衡，代价是整局 GUI 卡死。
                    try { if (a.Paint != null) a.Paint(body); }
                    catch (Exception e)
                    {
                        failed.Add(a.No);
                        Log.ErrorOnce("[RUIF_Demo] 原子 " + a.No + " " + a.Name + " 的演示抛异常，已隔离（其余卡片不受影响）：\n" + e,
                            ("ruif.atom." + a.No).GetHashCode());
                        PaintFailed(body, a);
                    }
                }
            }
            if (noteH > 0f)
            {
                Draw.EllipsisLabel(new Rect(body.x, body.yMax + NoteGap, body.width, LineTiny), a.Note, GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
            }
        }

        /// <summary>演示失败时的显式标记：让「哪一件坏了」一眼可见，而不是整窗黑掉。</summary>
        private static void PaintFailed(Rect body, Atom a)
        {
            Draw.Border(body, Tokens.Dead);
            Draw.EllipsisLabel(body, "⚠ " + a.No + " 演示异常 · 已隔离 · 详见日志", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleCenter);
        }

        /// <summary>
        /// 降级 **D6**：空间不足时如实说明，**不假装画出来了、也不硬挤**。
        /// 阈值由各件的 <see cref="Atom.MinW"/> / <see cref="Atom.MinH"/> 声明（铁律 4）。
        /// </summary>
        private static void PaintTooSmall(Rect body, Atom a)
        {
            Draw.Border(body, Tokens.BorderSoft);
            Draw.EllipsisLabel(body, "— 空间不足 · " + a.No + " 需 ≥ " + Mathf.RoundToInt(a.MinW) + "×" + Mathf.RoundToInt(a.MinH),
                GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleCenter);
        }

        // 【已删除】本地 Segmented helper —— 它曾是 InputAtoms.Segmented 的手写副本，
        // 属**真重复**（见 docs/02-组件图鉴/扩展原子族_重复性检测.md · D1）：
        // 两套实现会在选中色 / 分隔线 / 段宽算法上各自漂移。现已统一改走框架实现（调用点见 01 号卡）。

        /// <summary>通用进度条（.kbar L207：height:11px；底 soft，填充 accent→info 渐变降级为单色 accent）。</summary>
        private static void KBar(Rect rect, float p01, Color? fill = null)
        {
            Draw.Box(rect, Tokens.BorderHair);
            Draw.Border(rect, Tokens.BorderSoft);
            float w = Mathf.Max(0f, (rect.width - 2f) * Mathf.Clamp01(p01));
            if (w > 0.5f) Draw.Box(new Rect(rect.x + 1f, rect.y + 1f, w, rect.height - 2f), fill ?? Tokens.Accent);
        }

        // ══════════════════════════════════════════════════════════════════
        //  九个族 · 42 件（件序与网页完全一致，编号 01–42）
        // ══════════════════════════════════════════════════════════════════

        private void Build()
        {
            BuildText();
            BuildGeo();
            BuildStatus();
            BuildVisual();
            BuildTable();
            BuildPanel();
            BuildInteract();
            BuildProgress();
            BuildMisc();
            BuildExt();      // 43–56 扩展输入族（深挖补录）
            BuildBase1();    // 57–62 宝典 A1/A2 展示件（宝典落地 B1 批）
            BuildDInput();   // 63 宝典落地 W-001（d-input 收尾）
            BuildDData();    // 64–66 宝典落地 W-002/W-003/W-004（d-data 前 3 件）
        }

        // ─────────────────────────────────────── A0 · 文本原语 6 件
        private void BuildText()
        {
            var f = new Family { Title = "A0 · 原子层 —— 文本原语", Sub = "6 件 · TextFmt / TextAtoms（官方依据：Verse GenText / GenDate / Text.CalcSize）" };

            f.Add(new Atom
            {
                No = "01", Name = "FormattedNumber", Api = "TextFmt.FormatNumber(double) → string",
                Interactive = true,
                Desc = "千分位与单位自适应缩写（12 480 → 12.5k）。官方依据：Verse GenText 工具族。",
                Note = "库存实时增长（钢铁 / 白银 / 零部件）· 切换格式档看同一数值的三种排法",
                BodyH = 26f, MinW = 60f, MinH = 22f,
                Paint = r =>
                {
                    var row = Layout.Row(r, 6f);
                    var chips = Layout.Row(row.Take(r.width - 150f), 6f);
                    double[] vals = { 12480.0, 284312.0, 469.0, 1400000.0 };
                    for (int i = 0; i < vals.Length; i++)
                    {
                        Rect c = chips.Take(Mathf.Min(chips.Rest.width, 88f));
                        Draw.Box(c, Tokens.Card);
                        Draw.Border(c, Tokens.BorderSoft);
                        string s = numMode == 2 ? ((long)vals[i]).ToString()
                                 : numMode == 1 ? ((long)vals[i]).ToString("N0")
                                 : TextFmt.FormatNumber(vals[i]);
                        Draw.EllipsisLabel(c, s, GameFont.Small, Tokens.Accent, TextAnchor.MiddleCenter);
                    }
                    // D1 处置：改走框架实现（本地 helper 已删）
                    InputAtoms.Segmented(Isset(row.Rest, 0f, CtrlTiny), new[] { "缩写", "千分位", "原值" }, ref numMode);
                }
            });

            f.Add(new Atom
            {
                , Name = "FormattedNumber", Api = "TextFmt.FormatNumber(double) → string",
                Interactive = true,
                Desc = "千分位与单位自适应缩写（12 480 → 12.5k）。官方依据：Verse GenText 工具族。",
                Note = "库存实时增长（钢铁 / 白银 / 零部件）· 切换格式档看同一数值的三种排法",
                BodyH = 26f,
                Paint = r =>
                {
                    var row = Layout.Row(r, 6f);
                    var chips = Layout.Row(row.Take(r.width - 150f), 6f);
                    double[] vals = { 12480.0, 284312.0, 469.0, 1400000.0 };
                    for (int i = 0; i < vals.Length; i++)
                    {
                        Rect c = chips.Take(Mathf.Min(chips.Rest.width, 88f));
                        Draw.Box(c, Tokens.Card);
                        Draw.Border(c, Tokens.BorderSoft);
                        string s = numMode == 2 ? ((long)vals[i]).ToString()
                                 : numMode == 1 ? ((long)vals[i]).ToString("N0")
                                 : TextFmt.FormatNumber(vals[i]);
                        Draw.EllipsisLabel(c, s, GameFont.Small, Tokens.Accent, TextAnchor.MiddleCenter);
                    }
                    // D1 处置：改走框架实现（本地 helper 已删）
                    InputAtoms.Segmented(Isset(row.Rest, 0f, CtrlTiny), new[] { "缩写", "千分位", "原值" }, ref numMode);
                }
            });

            f.Add(new Atom
            {
                0202No = "03"No = "03", Name = "UnitLabel", Api = "TextAtoms.UnitLabel(Rect, string, string, GameFont, Color?)",
                Desc = "数值 + 单位对齐排版（数值 Medium 右对齐、单位 Tiny 灰）。KPI 卡与电力条的内嵌原子。",
                Note = "三行共用同一右边界——单位列宽由最长单位撑开，不是各排各的",
                // UnitLabel 用整高居中，valueFont=Medium ⇒ 每行需 30
                BodyH = 92f, MinW = 60f, MinH = 30f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 2f);
                    TextAtoms.UnitLabel(col.Take(Measure.RowHeight(Tokens.FontValue)), "12 480", "件", Tokens.FontValue, Tokens.Accent);
                    TextAtoms.UnitLabel(col.Take(Measure.RowHeight(Tokens.FontValue)), "469", "W", Tokens.FontValue, Tokens.Info);
                    TextAtoms.UnitLabel(col.Take(Measure.RowHeight(Tokens.FontValue)), "86", "%", Tokens.FontValue, Tokens.Alive);
                }
            });

            f.Add(new Atom
            {
                , Name = "UnitLabel", Api = "TextAtoms.UnitLabel(Rect, string, string, GameFont, Color?)",
                Desc = "数值 + 单位对齐排版（数值 Medium 右对齐、单位 Tiny 灰）。KPI 卡与电力条的内嵌原子。",
                Note = "三行共用同一右边界——单位列宽由最长单位撑开，不是各排各的",
                // UnitLabel 用整高居中，valueFont=Medium ⇒ 每行需 30
                BodyH = 92f,
                Paint = r =>
                {
                    var col = Layout.Column(r, 2f);
                    TextAtoms.UnitLabel(col.Take(Measure.RowHeight(Tokens.FontValue)), "12 480", "件", Tokens.FontValue, Tokens.Accent);
                    TextAtoms.UnitLabel(col.Take(Measure.RowHeight(Tokens.FontValue)), "469", "W", Tokens.FontValue, Tokens.Info);
                    TextAtoms.UnitLabel(col.Take(Measure.RowHeight(Tokens.FontValue)), "86", "%", Tokens.FontValue, Tokens.Alive);
                }
            });

            f.Add(new Atom
            {
                0404No = "05"No = "05", Name = "Typewriter 逐字浮现", Api = "TextAtoms.Typewriter(Rect, string, float, GameFont, Color, TextAnchor)",
                Desc = "按进度逐字显示——事件叙事与信件的开场仪式感。点击即显全文（跳过）。",
                Note = "进度随真实时间推进 · 跳过 / 重播（交互回补）",
                BodyH = 54f + CtrlTiny + 4f,   // 打字区 + 按钮行
                Paint = r =>
                {
                    string[] texts =
                    {
                        "殖民地第一天，没有墙，也没有床，只有一堆迫降残骸和四张惊恐的脸。",
                        "医疗帐篷里油灯昏黄，医生用最后一株草药保住了病人的腿。"
                    };
                    var col = Layout.Column(r, 4f);
                    int idx = Mathf.FloorToInt(typeProgress);
                    float p = typeProgress - idx;
                    TextAtoms.Typewriter(col.Take(54f), texts[idx % texts.Length], Mathf.Clamp01(p), Tokens.FontBody, Tokens.Text, TextAnchor.UpperLeft);
                    // 交互回补：跳过 = 直接把进度推到本段末尾；重播 = 回到第一段。
                    // 这样**不需要新状态字段**（进度本身就是状态），也不会与 Render 里的推进逻辑打架。
                    var row = Layout.Row(col.Take(col.Rest.height), 6f);
                    if (Widgets.ButtonText(row.Take(64f), "跳过")) typeProgress = 1.34f;
                    if (Widgets.ButtonText(row.Take(64f), "重播")) typeProgress = 0f;
                    Draw.EllipsisLabel(row.Rest, "第 " + (idx + 1) + " 段 · " + Mathf.RoundToInt(Mathf.Clamp01(p) * 100f) + "%",
                        GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                , Name = "Typewriter 逐字浮现", Api = "TextAtoms.Typewriter(Rect, string, float, GameFont, Color, TextAnchor)",
                Desc = "按进度逐字显示——事件叙事与信件的开场仪式感。点击即显全文（跳过）。",
                Note = "进度随真实时间推进 · 跳过 / 重播（交互回补）",
                BodyH = 54f + CtrlTiny + 4f,   // 打字区 + 按钮行
                Paint = r =>
                {
                    string[] texts =
                    {
                        "殖民地第一天，没有墙，也没有床，只有一堆迫降残骸和四张惊恐的脸。",
                        "医疗帐篷里油灯昏黄，医生用最后一株草药保住了病人的腿。"
                    };
                    var col = Layout.Column(r, 4f);
                    int idx = Mathf.FloorToInt(typeProgress);
                    float p = typeProgress - idx;
                    TextAtoms.Typewriter(col.Take(54f), texts[idx % texts.Length], Mathf.Clamp01(p), Tokens.FontBody, Tokens.Text, TextAnchor.UpperLeft);
                    // 交互回补：跳过 = 直接把进度推到本段末尾；重播 = 回到第一段。
                    // 这样**不需要新状态字段**（进度本身就是状态），也不会与 Render 里的推进逻辑打架。
                    var row = Layout.Row(col.Take(col.Rest.height), 6f);
                    if (Widgets.ButtonText(row.Take(64f), "跳过")) typeProgress = 1.34f;
                    if (Widgets.ButtonText(row.Take(64f), "重播")) typeProgress = 0f;
                    Draw.EllipsisLabel(row.Rest, "第 " + (idx + 1) + " 段 · " + Mathf.RoundToInt(Mathf.Clamp01(p) * 100f) + "%",
                        GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                0606No = "07"No = "07", Name = "SectionFrame 组框", Api = "Atoms.SectionFrame(Rect, string) → Rect（内容区）",
                Desc = "带标题的 fieldset 组框：标题嵌于上边线断口处。返回内容区矩形，调用方直接在返回区排版。",
                Note = "返回值即内容区（已含内边距）——不要再套一层 Inset",
                // 内容区 = BodyH − 组框上下内边距；三行各需 LineTiny(18) ⇒ BodyH 需 ≥ 54 + 内边距
                BodyH = 86f, MinW = 120f, MinH = 62f,
                Paint = r =>
                {
                    // ⚠ SectionFrame 内部用 Text.CalcSize(title) 测断口宽度、却用 Tiny 绘制
                    //   ⇒ 调用前必须把 Text.Font 统一到 Tiny，否则断口按 Small 测出来偏宽
                    Rect body;
                    using (Draw.Font(Tokens.FontLabel)) body = Atoms.SectionFrame(r, "采集规则 · 储存区");
                    var col = Layout.Column(body, 2f);
                    Draw.EllipsisLabel(col.Take(LineTiny), "优先储存　钢铁 / 零部件", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                    Draw.EllipsisLabel(col.Take(LineTiny), "允许腐烂　否", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                    Draw.EllipsisLabel(col.Take(LineTiny), "优先级　　重要", GameFont.Tiny, Tokens.Warn, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                , Name = "SectionFrame 组框", Api = "Atoms.SectionFrame(Rect, string) → Rect（内容区）",
                Desc = "带标题的 fieldset 组框：标题嵌于上边线断口处。返回内容区矩形，调用方直接在返回区排版。",
                Note = "返回值即内容区（已含内边距）——不要再套一层 Inset",
                // 内容区 = BodyH − 组框上下内边距；三行各需 LineTiny(18) ⇒ BodyH 需 ≥ 54 + 内边距
                BodyH = 86f,
                Paint = r =>
                {
                    // ⚠ SectionFrame 内部用 Text.CalcSize(title) 测断口宽度、却用 Tiny 绘制
                    //   ⇒ 调用前必须把 Text.Font 统一到 Tiny，否则断口按 Small 测出来偏宽
                    Rect body;
                    using (Draw.Font(Tokens.FontLabel)) body = Atoms.SectionFrame(r, "采集规则 · 储存区");
                    var col = Layout.Column(body, 2f);
                    Draw.EllipsisLabel(col.Take(LineTiny), "优先储存　钢铁 / 零部件", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                    Draw.EllipsisLabel(col.Take(LineTiny), "允许腐烂　否", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
                    Draw.EllipsisLabel(col.Take(LineTiny), "优先级　　重要", GameFont.Tiny, Tokens.Warn, TextAnchor.MiddleLeft);
                }
            });

            f.Add(new Atom
            {
                0808No = "09"No = "09", Name = "GradientRule 渐变分隔线", Api = "Shapes.GradientRule(Rect, Color, int segments)",
                Desc = "accent → 透明的渐隐分隔线。零贴图实现：8~12 段 alpha 递减折线拼接，纯原生。",
                Note = "预览 .gradrule{height:2px}；段数由调用方给定",
                BodyH = 30f, MinW = 16f, MinH = 1f,
                Paint = r =>
                {
                    Shapes.GradientRule(new Rect(r.x, r.center.y - 1f, r.width, 2f), Tokens.Accent, 12);
                }
            });

            f.Add(new Atom
            {
                , Name = "GradientRule 渐变分隔线", Api = "Shapes.GradientRule(Rect, Color, int segments)",
                Desc = "accent → 透明的渐隐分隔线。零贴图实现：8~12 段 alpha 递减折线拼接，纯原生。",
                Note = "预览 .gradrule{height:2px}；段数由调用方给定",
                BodyH = 30f,
                Paint = r =>
                {
                    Shapes.GradientRule(new Rect(r.x, r.center.y - 1f, r.width, 2f), Tokens.Accent, 12);
                }
            });

            f.Add(new Atom
            {
                1010No = "11"No = "11", Name = "Watermark 水印", Api = "Shapes.Watermark(Rect, string mark, float alpha)",
                Desc = "低透明度底纹徽记（空页面 / 封面页气质件）。绘制在内容之下，随主题令牌变色。",
                Note = "预览 alpha .06~.10 · 内容浮于水印之上",
                BodyH = 52f, MinW = 60f, MinH = 18f,
                Paint = r =>
                {
                    Shapes.InsetPanel(r);
                    Shapes.Watermark(r, "PC", 0.10f);
                    Draw.EllipsisLabel(r, "内容浮于水印之上", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleCenter);
                }
            });

            f.Add(new Atom
            {
                , Name = "Watermark 水印", Api = "Shapes.Watermark(Rect, string mark, float alpha)",
                Desc = "低透明度底纹徽记（空页面 / 封面页气质件）。绘制在内容之下，随主题令牌变色。",
                Note = "预览 alpha .06~.10 · 内容浮于水印之上",
                BodyH = 52f,
                Paint = r =>
                {
                    Shapes.InsetPanel(r);
                    Shapes.Watermark(r, "PC", 0.10f);
                    Draw.EllipsisLabel(r, "内容浮于水印之上", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleCenter);
                }
            });

            f.Add(new Atom
            {
                1212