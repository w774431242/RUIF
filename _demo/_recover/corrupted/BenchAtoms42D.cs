using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// 扩展原子族 · 输入（演武场编号 <b>43–56</b>）。
    ///
    /// <para>来源：深挖发现的 RUIF 输入族四类 —— <c>InputAtoms</c>(43–48) /
    /// <c>InputExAtoms</c>(49–54) / <c>InputPlusAtoms</c>(55) / <c>InputCandAtoms</c>(56)。
    /// 与 A0/A0+ 的 01–42 分开陈列：这些件已在 RUIF 内实现，但此前未落地到本验证台。</para>
    ///
    /// <para><b>每个 rect 的最小尺寸与「外扩绘制」都按反编译实测给足</b>（依据：
    /// <c>docs/02-组件图鉴/扩展原子族_重复性检测.md</c> §4「落地前必须遵守的绘制契约」）——
    /// 这些件的内部公式各不相同，凭签名猜必错。</para>
    ///
    /// <para><b>重复性处置（同上报告）</b>：43 号改走框架的 <see cref="InputAtoms.Segmented"/>，
    /// 已删除本验证台此前手写的同名 helper（真重复 D1）；进度条 helper 仅保留无阈值语义的用途（D15）。</para>
    /// </summary>
    internal sealed partial class BenchAtoms42
    {
        // ── 交互回补批的其余状态（集中声明在本文件：partial 类允许字段跨文件，
        //    放在 D 可避免与 A 的字段块反复冲突；且随件声明，不留「未使用」警告） ──
        private int budgetSpent = 72;   // 40 预算已用
        private int ovIdx;              // 22 溢出文本长短档
        private int ruleIdx;            // 10 分段名索引
        private string shQuery = "建造"; // 35 搜索关键词（真输入框）
        private bool insetRaised;       // 12 凹/凸
        private int researchIdx;        // 37 研究队列项
        private int anchorIdx = 4;      // 19 九宫锚（默认居中）
        private int cornerIdx;          // 08 角框选中目标
        private readonly bool[] secOpen = { true, false, true };   // 32 三条的折叠态
        private float thinScroll;       // 42 细滚动条位置（可拖）
        private float frozenScroll;     // 27 冻结列右区横向滚动量

        /// <summary>42 号滚动内容（官方事件词表）。</summary>
        internal static readonly string[] EventWords =
        {
            "袭击", "寒流", "虫灾", "贸易商队", "太阳耀斑", "极光", "日蚀", "枯萎病", "猎杀人类", "心灵低语", "陨石撞击", "家养动物加入"
        };

        /// <summary>
        /// 交互件的**本体悬停反馈**（精美度②「反馈」· 六态之悬停态）。
        ///
        /// <para>**调用时机**：组件绘制**之后**（作为覆盖层描边），传入组件自身使用的那个 rect。
        /// 传 `box` / `Isset(r, dy, h)` 这类**与组件调用完全相同的表达式**，避免本体与反馈框错位。</para>
        ///
        /// <para>**只对「RUIF 侧不自绘 hover」的件调用**：组件自带的（`CycleButton` 悬停转 accent 边框、
        /// `WheelAdjust` 悬停 `DrawHighlight`、原版 `ButtonText`）**不要重复画** —— 否则会盖掉它自己的悬停语义。</para>
        ///
        /// <para>三层悬停分工：卡片层 = 「可指向」；演示区层 = 「本块可操作」（<c>Atom.Interactive</c>）；
        /// 本函数 = 「**本体就在这里**」。三者叠加才是完整的六态之悬停。</para>
        /// </summary>
        private static void HoverFrame(Rect r)
        {
            if (Mouse.IsOver(r)) Draw.Border(r, Tokens.Accent);
        }

        /// <summary>研究项目名（取自官方 ResearchProjectDef 词表）——37 号跳过本项用。</summary>
        internal static readonly string[] Research =
        {
            "电力", "蓄电池", "水栽培", "空调", "枪械制作", "锻造", "岩石切割", "酿酒"
        };

        // ── 43–56 的交互状态：一律由本面持有，原子自身零状态（GATE-UI） ──────
        private int segIdx;
        private bool tglOn;
        private int stepVal = 12;
        private float sliderVal = 0.62f;
        private bool keyArmed;
        private readonly bool[] tagPicked = { true, false, false, true, false, false, false, false };
        private float dsLo = 25f, dsHi = 70f;
        private Color cpColor = new Color(0.35f, 0.71f, 0.89f);
        private float cpH = 0.573f, cpS = 0.61f, cpV = 0.89f;
        private int cycIdx;
        private string editText = "迫降乡";
        private string editBuffer = "迫降乡";
        private bool editing;
        private readonly int[] pgVals = { 3, 2, 1, 1, 2, 4, 0, 1, 1, 1, 3, 2 };
        private readonly int[] ttStates = new int[24];
        private float ticksVal = 9f;
        // ⚠ 语义是**真实值**（组件按 (v−min)/(max−min) 算比例并 ToString("F1") 回显）
        //   —— 早期误传 0.64/0.78（想表达 64%/78%）导致显示成「0.6% / 0.8%」（本轮实测踩到）
        private readonly float[] batchVals = { 21f, 64f, 78f };
        private readonly InputCandAtoms.SliderCfg[] batchCfg =
        {
            new InputCandAtoms.SliderCfg { label = "温度上限", min = -10f, max = 40f, unit = "°C" },
            new InputCandAtoms.SliderCfg { label = "亮度",     min = 0f,   max = 100f, unit = "%" },
            new InputCandAtoms.SliderCfg { label = "音量",     min = 0f,   max = 100f, unit = "%" }
        };

        /// <summary>技能等级词表（官方 0–20 档，55 号刻度吸附用）。</summary>
        private static readonly string[] SkillRanks =
        {
            "一窍不通", "不明就里", "初学乍练", "一知半解", "初窥门径", "恍然大悟", "略有小成", "渐入佳境",
            "得心应手", "出类拔萃", "游刃有馀", "良工巧匠", "别具匠心", "炉火纯青", "一代大师", "登峰造极",
            "蜚声内外", "一代宗师", "蜚声寰宇", "举世无双", "点石成金"
        };

        private void BuildExt()
        {
            var f = new Family
            {
                Title = "扩展原子族 · 输入",
                Sub = "14 件 · InputAtoms / InputExAtoms / InputPlusAtoms / InputCandAtoms（深挖补录，编号 43–56）"
            };

            // ─────────────────────────── 43 · Segmented
            f.Add(new Atom
            {
                No = "43", Name = "Segmented 分段选择", Api = "InputAtoms.Segmented(Rect, string[], ref int) → bool",
                Interactive = true,
                Desc = "互斥分段选择 —— 取代原版缺失的 RadioButtonGroup。自绘 PanelRaised 底 + 段间分隔线，返回是否变化。",
                Note = "⚠ 与验证台本地 helper 真重复（报告 D1）：本件已改走框架实现并删除本地版本",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var opts = new[] { "全部", "只看异常", "只看空闲" };
                    InputAtoms.Segmented(Isset(r, 0f, CtrlTiny), opts, ref segIdx);

                    HoverFrame(Isset(r, 0f, CtrlTiny));
                    Draw.EllipsisLabel(new Rect(r.x, r.y + CtrlTiny + 2f, r.width, LineTiny),
                        "index = " + segIdx + " · 与游戏内验证台同号 43", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 44 · ToggleSwitch
            f.Add(new Atom
            {
                , Name = "Segmented 分段选择", Api = "InputAtoms.Segmented(Rect, string[], ref int) → bool",
                Interactive = true,
                Desc = "互斥分段选择 —— 取代原版缺失的 RadioButtonGroup。自绘 PanelRaised 底 + 段间分隔线，返回是否变化。",
                Note = "⚠ 与验证台本地 helper 真重复（报告 D1）：本件已改走框架实现并删除本地版本",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var opts = new[] { "全部", "只看异常", "只看空闲" };
                    InputAtoms.Segmented(Isset(r, 0f, CtrlTiny), opts, ref segIdx);

                    HoverFrame(Isset(r, 0f, CtrlTiny));
                    Draw.EllipsisLabel(new Rect(r.x, r.y + CtrlTiny + 2f, r.width, LineTiny),
                        "index = " + segIdx + " · 与游戏内验证台同号 43", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 44 · ToggleSwitch
            f.Add(new Atom
            {
                4444No = "45"No = "45", Name = "Stepper 数值步进", Api = "InputAtoms.Stepper(Rect, ref int, int min, int max, int step) → bool",
                Interactive = true,
                MinW = 60f,   // 内部最小宽 60：低于此值走 D6 占位
                Desc = "−/＋ 步进，内置 min/max 夹取（越界由 InputFmt.StepValue 处理）。左钮 26 宽、右钮绝对定位在 xMax−26。",
                Note = "⚠ 与未落地的 NumericField 重叠（报告 D3）：点击步进 vs 键入，同一值的两条输入通道",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    var box = row.Take(Mathf.Min(150f, row.Rest.width));
                    if (InputAtoms.Stepper(box, ref stepVal, 0, 20, 1)) { /* 就地回写 */ }

                    HoverFrame(box);
                    Draw.EllipsisLabel(row.Rest,
                        "0–20 · 技能等级口径 · 现 " + stepVal, GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 46 · SliderWithValue（外扩件）
            f.Add(new Atom
            {
                , Name = "Stepper 数值步进", Api = "InputAtoms.Stepper(Rect, ref int, int min, int max, int step) → bool",
                Interactive = true,
                MinW = 60f,   // 内部最小宽 60：低于此值走 D6 占位
                Desc = "−/＋ 步进，内置 min/max 夹取（越界由 InputFmt.StepValue 处理）。左钮 26 宽、右钮绝对定位在 xMax−26。",
                Note = "⚠ 与未落地的 NumericField 重叠（报告 D3）：点击步进 vs 键入，同一值的两条输入通道",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    var box = row.Take(Mathf.Min(150f, row.Rest.width));
                    if (InputAtoms.Stepper(box, ref stepVal, 0, 20, 1)) { /* 就地回写 */ }

                    HoverFrame(box);
                    Draw.EllipsisLabel(row.Rest,
                        "0–20 · 技能等级口径 · 现 " + stepVal, GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 46 · SliderWithValue（外扩件）
            f.Add(new Atom
            {
                4646No = "47"No = "47", Name = "HotkeyBind 键位绑定", Api = "InputAtoms.HotkeyBind(Rect, string label, string currentKey) → bool",
                Interactive = true,
                MinW = 254f,   // 内部最小宽 254：低于此值走 D6 占位
                Desc = "「动作 + 当前键位 + 改键」三格。内部固定总宽 254（标签 90 + 键框 84 + 按钮 64 + 间距）。",
                Note = "⚠ 内部自排固定宽 254（报告 §4）：窗口窄于该值会被裁切，需自行降级",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    var box = row.Take(Mathf.Min(254f, row.Rest.width));
                    if (InputAtoms.HotkeyBind(box, "快速存档", keyArmed ? "F5（监听中）" : "F5")) keyArmed = !keyArmed;

                    HoverFrame(box);
                    Draw.EllipsisLabel(row.Rest,
                        "⚠ 捕获需官方 KeyPrefs 流程，本件只做显示 + 回调", GameFont.Tiny, Tokens.Warn, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 48 · TagPicker
            f.Add(new Atom
            {
                , Name = "HotkeyBind 键位绑定", Api = "InputAtoms.HotkeyBind(Rect, string label, string currentKey) → bool",
                Interactive = true,
                MinW = 254f,   // 内部最小宽 254：低于此值走 D6 占位
                Desc = "「动作 + 当前键位 + 改键」三格。内部固定总宽 254（标签 90 + 键框 84 + 按钮 64 + 间距）。",
                Note = "⚠ 内部自排固定宽 254（报告 §4）：窗口窄于该值会被裁切，需自行降级",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    var box = row.Take(Mathf.Min(254f, row.Rest.width));
                    if (InputAtoms.HotkeyBind(box, "快速存档", keyArmed ? "F5（监听中）" : "F5")) keyArmed = !keyArmed;

                    HoverFrame(box);
                    Draw.EllipsisLabel(row.Rest,
                        "⚠ 捕获需官方 KeyPrefs 流程，本件只做显示 + 回调", GameFont.Tiny, Tokens.Warn, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 48 · TagPicker
            f.Add(new Atom
            {
                4848No = "49"No = "49", Name = "DoubleSlider 双端范围条", Api = "InputExAtoms.DoubleSlider(Rect, ref float lo, ref float hi, float min, float max, float roundTo) → bool",
                Interactive = true,
                MinW = 140f,   // 内部最小宽 140：低于此值走 D6 占位
                Desc = "范围条（高固定 12）+ Lower/Upper 两行滑条（各 18）。对齐原版 FloatRange / IntRange / QualityRange。",
                Note = "最小高 54、实用最小宽 140（宽不足时滑条只剩 10px）",
                BodyH = 58f,
                Paint = r =>
                {
                    InputExAtoms.DoubleSlider(Isset(r, 0f, 54f), ref dsLo, ref dsHi, 0f, 100f, 1f);

                    HoverFrame(Isset(r, 0f, 54f));
                    Draw.EllipsisLabel(new Rect(r.x, r.y + 56f, r.width, LineTiny),
                        "区间 " + Mathf.RoundToInt(dsLo) + " – " + Mathf.RoundToInt(dsHi), GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 50 · ColorPicker
            f.Add(new Atom
            {
                , Name = "DoubleSlider 双端范围条", Api = "InputExAtoms.DoubleSlider(Rect, ref float lo, ref float hi, float min, float max, float roundTo) → bool",
                Interactive = true,
                MinW = 140f,   // 内部最小宽 140：低于此值走 D6 占位
                Desc = "范围条（高固定 12）+ Lower/Upper 两行滑条（各 18）。对齐原版 FloatRange / IntRange / QualityRange。",
                Note = "最小高 54、实用最小宽 140（宽不足时滑条只剩 10px）",
                BodyH = 58f,
                Paint = r =>
                {
                    InputExAtoms.DoubleSlider(Isset(r, 0f, 54f), ref dsLo, ref dsHi, 0f, 100f, 1f);

                    HoverFrame(Isset(r, 0f, 54f));
                    Draw.EllipsisLabel(new Rect(r.x, r.y + 56f, r.width, LineTiny),
                        "区间 " + Mathf.RoundToInt(dsLo) + " – " + Mathf.RoundToInt(dsHi), GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 50 · ColorPicker
            f.Add(new Atom
            {
                5050No = "51"No = "51", Name = "CycleButton 循环枚举钮", Api = "InputExAtoms.CycleButton(Rect, string label, string[] values, ref int index) → bool",
                Interactive = true,
                MinW = 190f,   // 内部最小宽 190：低于此值走 D6 占位
                Desc = "单击循环到下一个枚举值 —— 占地最小、点击最少的状态切换件。**悬停时边框转 accent**（本件自带 hover 反馈）。",
                Note = "⚠ 与 43 号 Segmented 部分重叠（报告 D5）：选项 ≤4 且空间足用 43，空间紧张用本件",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var opts = new[] { "任意", "工作", "娱乐", "休息" };
                    var row = Layout.Row(r, 8f);
                    var box = row.Take(Mathf.Min(190f, row.Rest.width));
                    InputExAtoms.CycleButton(box, "作息", opts, ref cycIdx);
                    Draw.EllipsisLabel(row.Rest,
                        "index = " + cycIdx + " → " + opts[cycIdx], GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 52 · EditableLabel
            f.Add(new Atom
            {
                , Name = "CycleButton 循环枚举钮", Api = "InputExAtoms.CycleButton(Rect, string label, string[] values, ref int index) → bool",
                Interactive = true,
                MinW = 190f,   // 内部最小宽 190：低于此值走 D6 占位
                Desc = "单击循环到下一个枚举值 —— 占地最小、点击最少的状态切换件。**悬停时边框转 accent**（本件自带 hover 反馈）。",
                Note = "⚠ 与 43 号 Segmented 部分重叠（报告 D5）：选项 ≤4 且空间足用 43，空间紧张用本件",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var opts = new[] { "任意", "工作", "娱乐", "休息" };
                    var row = Layout.Row(r, 8f);
                    var box = row.Take(Mathf.Min(190f, row.Rest.width));
                    InputExAtoms.CycleButton(box, "作息", opts, ref cycIdx);
                    Draw.EllipsisLabel(row.Rest,
                        "index = " + cycIdx + " → " + opts[cycIdx], GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 52 · EditableLabel
            f.Add(new Atom
            {
                5252No = "53"No = "53", Name = "PriorityGrid 优先级网格", Api = "InputExAtoms.PriorityGrid(Rect, string[] rowLabels, string[] colLabels, int[] values, out int hitIndex) → bool",
                Interactive = true,
                Desc = "列头（高 16）+ 行标签 + 每格一个整数值，点格子递进。工作优先级 / 技能分配 / 政策矩阵通用。",
                Note = "最小高 = 18 + rows×rowH；与 TimetableMatrix / PaintStrokeGrid 构成「矩阵族」（报告 D7/D8）",
                BodyH = 80f, MinW = 90f, MinH = 78f,
                Paint = r =>
                {
                    int hit;
                    // 列头必须与预览一致（"P1".."P4"）；格子配色由组件内部的 PriorityColor 语义四色决定
                    InputExAtoms.PriorityGrid(Isset(r, 0f, 78f), new[] { "建造", "采矿", "手工" }, new[] { "P1", "P2", "P3", "P4" }, pgVals, out hit);

                    HoverFrame(Isset(r, 0f, 78f));
                }
            });

            // ─────────────────────────── 54 · TimetableMatrix
            f.Add(new Atom
            {
                , Name = "PriorityGrid 优先级网格", Api = "InputExAtoms.PriorityGrid(Rect, string[] rowLabels, string[] colLabels, int[] values, out int hitIndex) → bool",
                Interactive = true,
                Desc = "列头（高 16）+ 行标签 + 每格一个整数值，点格子递进。工作优先级 / 技能分配 / 政策矩阵通用。",
                Note = "最小高 = 18 + rows×rowH；与 TimetableMatrix / PaintStrokeGrid 构成「矩阵族」（报告 D7/D8）",
                BodyH = 80f,
                Paint = r =>
                {
                    int hit;
                    // 列头必须与预览一致（"P1".."P4"）；格子配色由组件内部的 PriorityColor 语义四色决定
                    InputExAtoms.PriorityGrid(Isset(r, 0f, 78f), new[] { "建造", "采矿", "手工" }, new[] { "P1", "P2", "P3", "P4" }, pgVals, out hit);

                    HoverFrame(Isset(r, 0f, 78f));
                }
            });

            // ─────────────────────────── 54 · TimetableMatrix
            f.Add(new Atom
            {
                5454No = "55"No = "55", Name = "SliderTicks 刻度吸附滑条", Api = "InputPlusAtoms.SliderTicks(Rect, ref float, float[] ticks, float min, float max) → bool",
                Interactive = true,
                MinW = 178f,   // 内部最小宽 178：低于此值走 D6 占位
                Desc = "拖动后吸附到最近刻度 —— 技能等级 / 品质档位 / 离散参数的专用滑条。",
                Note = "⚠ 与 46 号部分重叠（报告 D4）：本件是 SliderWithValue 的超集（多 ticks），连续量用 46、离散档用本件。宽需 ≥ 178",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    var box = row.Take(Mathf.Min(280f, row.Rest.width));
                    InputPlusAtoms.SliderTicks(box, ref ticksVal, new[] { 0f, 5f, 10f, 15f, 20f }, 0f, 20f);

                    HoverFrame(box);
                    int lv = Mathf.Clamp(Mathf.RoundToInt(ticksVal), 0, 20);
                    // ⚠ SliderTicks **内部已画当前值**（Draw.Label 在其 hStack.Rest 上）——
                    //   这里只补等级名，不得再拼数值，否则出现「10 10」重复（本轮实测踩到）
                    Draw.EllipsisLabel(row.Rest,
                        SkillRanks[lv], GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 56 · BatchSliders
            f.Add(new Atom
            {
                , Name = "SliderTicks 刻度吸附滑条", Api = "InputPlusAtoms.SliderTicks(Rect, ref float, float[] ticks, float min, float max) → bool",
                Interactive = true,
                MinW = 178f,   // 内部最小宽 178：低于此值走 D6 占位
                Desc = "拖动后吸附到最近刻度 —— 技能等级 / 品质档位 / 离散参数的专用滑条。",
                Note = "⚠ 与 46 号部分重叠（报告 D4）：本件是 SliderWithValue 的超集（多 ticks），连续量用 46、离散档用本件。宽需 ≥ 178",
                BodyH = CtrlTiny,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    var box = row.Take(Mathf.Min(280f, row.Rest.width));
                    InputPlusAtoms.SliderTicks(box, ref ticksVal, new[] { 0f, 5f, 10f, 15f, 20f }, 0f, 20f);

                    HoverFrame(box);
                    int lv = Mathf.Clamp(Mathf.RoundToInt(ticksVal), 0, 20);
                    // ⚠ SliderTicks **内部已画当前值**（Draw.Label 在其 hStack.Rest 上）——
                    //   这里只补等级名，不得再拼数值，否则出现「10 10」重复（本轮实测踩到）
                    Draw.EllipsisLabel(row.Rest,
                        SkillRanks[lv], GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ─────────────────────────── 56 · BatchSliders
            f.Add(new Atom
            {
                5656