using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// 分类「输入」· <b>29 件</b> —— 候选层 (InputCandAtoms) 2 件 + 原生 (InputAtoms / InputExAtoms /
    /// InputPlusAtoms / InteractAtoms) 27 件。
    ///
    /// <para>全部调用 RUIF 公共静态类的冻结签名（与 Source/Components 下 .cs 实测一致）。
    /// 交互态（开关 / 步进值 / 滑条值 / 缓冲 / 选中下标 / 拖拽状态等）<b>一律归本类持有</b>，
    /// 按各组件 ref 参数与委托签名传入——组件零状态。</para>
    ///
    /// <para>布局：<c>Scrollable => true</c>，内容区由 <c>Layout.Column</c> 垂直堆叠；
    /// <see cref="Height"/> 与 <see cref="Render"/> 共用同一套 Take 高度常量，确保滚动游标与内容不漂移。</para>
    /// </summary>
    internal sealed class BenchInput : IBench
    {
        // ══════════════════════════════════════════════════════════════════════
        // 布局常量（Height 与 Render 共用，单来源避免漂移）
        // ══════════════════════════════════════════════════════════════════════
        private const float GAP = 8f;
        private const float HDR = 28f;
        private const float CAP = 16f;

        private const float B_CandArea = 104f;
        private const float B_CandKey = 132f;
        private const float B_Toggle = 26f;
        private const float B_Stepper = 28f;
        private const float B_Slider = 46f;
        private const float B_Search = 26f;
        private const float B_Tag = 28f;
        private const float B_Hotkey = 26f;
        private const float B_Double = 56f;
        private const float B_Color = 96f;
        private const float B_Cycle = 26f;
        private const float B_Edit = 26f;
        private const float B_Scrub = 26f;
        private const float B_Priority = 132f;
        private const float B_Timetable = 132f;
        private const float B_Quick = 26f;
        private const float B_Ticks = 46f;
        private const float B_Wheel = 26f;
        private const float B_Numeric = 26f;
        private const float B_Path = 26f;
        private const float B_Dropdown = 26f;
        private const float B_Filter = 40f;
        private const float B_Multi = 150f;
        private const float B_Queue = 146f;
        private const float B_KeyHint = 26f;
        private const float B_MultiLine = 96f;
        private const float B_Grip = 26f;
        private const float B_Eye = 26f;
        private const float B_SearchHi = 26f;

        // ══════════════════════════════════════════════════════════════════════
        // 样例数据 + 交互状态（归调用方；组件零状态）
        // ══════════════════════════════════════════════════════════════════════

        // —— 候选层 ——
        private static readonly string[] CandAreas = { "整个殖民地", "暖棚区", "冷藏区", "矿洞" };
        private int candAreaSel;
        private static readonly string[] CandActions = { "移动", "攻击", "交互", "建造" };
        private static readonly string[] CandKeys = { "W", "鼠标左键", "E", "B" };
        private readonly bool[] candListening = new bool[4];

        // —— InputAtoms ——
        private bool toggleOn = true;
        private int stepperVal = 3;
        private float sliderVal = 0.6f;
        private string searchQuery = "";
        private string searchBuffer = "";
        private float searchLastInput;
        private static readonly string[] Tags = { "钢铁", "木材", "食物", "药材", "石料" };
        private readonly bool[] tagPicked = { true, false, true, false, false };

        // —— InputExAtoms ——
        private float dsLo = 0.2f;
        private float dsHi = 0.8f;
        private Color cpColor = Color.white;
        private float cpH;
        private float cpS;
        private float cpV;
        private static readonly string[] CycleVals = { "关", "低", "中", "高" };
        private int cycleIdx;
        private string editText = "双击此处可编辑";
        private bool editEditing;
        private string editBuffer = "";
        private float scrubVal = 50f;
        private bool scrubbing;
        private float scrubStartX;
        private float scrubStartVal;
        private static readonly string[] PriRows = { "挖矿", "建造", "治疗", "研究" };
        private static readonly string[] PriCols = { "日", "夜", "紧急" };
        private readonly int[] prioValues = { 2, 0, 1, 3, 0, 0, 1, 2, 0, 0, 3, 2 };
        private static readonly string[] TtRows = { "阿强", "小美", "老王" };
        private readonly int[] ttStates = TimetableDemo();

        // —— InputPlusAtoms ——
        private float qaVal = 1f;
        private float tickVal = 5f;
        private static readonly float[] Ticks = { 0f, 5f, 10f, 15f, 20f };
        private float wheelVal = 50f;
        private float numVal = 10f;
        private string numBuf = "10";
        private string pathVal = "C:/RimWorld/Saves/Colony01";
        private static readonly string[] DropOpts = { "普通", "困难", "极限", "部落挑战" };
        private int dropIdx;
        private static readonly string[] FilterCols = { "名称", "类型", "状态" };
        private readonly string[] filterVals = { "", "", "" };
        private static readonly string[] MsItems = { "钢铁", "木材", "石块", "食物", "药材", "布料" };
        private readonly bool[] msSel = new bool[6];
        private int msAnchor = -1;
        private readonly int[] queue = { 0, 2, 4, 1, 3 };
        private int queueCount = 3;
        private static readonly string[] QueueLabels = { "采集草药", "搬运食物", "巡逻边界", "研究科技", "治疗伤员" };
        private static readonly string[] ConflictKeys = { "F1", "F2", "F1", "F3" };
        private string mlText = "第一行示例文本\n第二行示例文本";
        private string mlBuffer = "第一行示例文本\n第二行示例文本";

        // —— InteractAtoms ——
        private bool eyeVisible = true;
        private bool gripOn;

        // —— 热键重绑（真实键盘捕获：点击「重绑」→ 按新键生效，Esc 取消）——
        private string hotkeyKey = "F5";
        private bool hotkeyListening;

        /// <summary>作息矩阵样例：白天(8–23)工作、其余睡眠，少量任意(2)。</summary>
        private static int[] TimetableDemo()
        {
            var s = new int[3 * 24];
            for (int i = 0; i < s.Length; i++)
            {
                int hour = i % 24;
                s[i] = (hour >= 8 && hour < 23) ? 0 : 1;
            }
            s[5] = 2; s[29] = 2; s[53] = 2;   // 穿插少量「任意」
            return s;
        }

        // ══════════════════════════════════════════════════════════════════════
        // IBench
        // ══════════════════════════════════════════════════════════════════════
        public bool Scrollable => true;

        public float Height(float width)
        {
            // 与 Render 严格同序、同高度地消费 VStack；返回累计 Y 即内容总高。
            var col = Layout.Column(new Rect(0f, 0f, width, 1e6f), GAP);

            col.Take(HDR); col.Take(CAP); col.Take(B_CandArea);
            col.Take(HDR); col.Take(CAP); col.Take(B_CandKey);

            col.Take(HDR);
            col.Take(CAP); col.Take(B_Toggle);
            col.Take(CAP); col.Take(B_Stepper);
            col.Take(CAP); col.Take(B_Slider);
            col.Take(CAP); col.Take(B_Search);
            col.Take(CAP); col.Take(B_Tag);
            col.Take(CAP); col.Take(B_Hotkey);

            col.Take(HDR);
            col.Take(CAP); col.Take(B_Double);
            col.Take(CAP); col.Take(B_Color);
            col.Take(CAP); col.Take(B_Cycle);
            col.Take(CAP); col.Take(B_Edit);
            col.Take(CAP); col.Take(B_Scrub);
            col.Take(CAP); col.Take(B_Priority);
            col.Take(CAP); col.Take(B_Timetable);

            col.Take(HDR);
            col.Take(CAP); col.Take(B_Quick);
            col.Take(CAP); col.Take(B_Ticks);
            col.Take(CAP); col.Take(B_Wheel);
            col.Take(CAP); col.Take(B_Numeric);
            col.Take(CAP); col.Take(B_Path);
            col.Take(CAP); col.Take(B_Dropdown);
            col.Take(CAP); col.Take(B_Filter);
            col.Take(CAP); col.Take(B_Multi);
            col.Take(CAP); col.Take(B_Queue);
            col.Take(CAP); col.Take(B_KeyHint);
            col.Take(CAP); col.Take(B_MultiLine);

            col.Take(HDR);
            col.Take(CAP); col.Take(B_Grip);
            col.Take(CAP); col.Take(B_Eye);
            col.Take(CAP); col.Take(B_SearchHi);

            return col.Y;
        }

        public void Render(Rect area)
        {
            var col = Layout.Column(area, GAP);

            // ───────────────────────── ① 候选层 · InputCandAtoms ─────────────────────────
            Header(ref col, "① 输入候选层 · InputCandAtoms（区域编辑器 / 键位绑定面板）");
            Caption(ref col, "AreaEditor(Rect, string[] areas, ref int selected)");
            InputCandAtoms.AreaEditor(col.Take(B_CandArea), CandAreas, ref candAreaSel);

            Header(ref col, "① 输入候选层 · InputCandAtoms（键位绑定面板）");
            Caption(ref col, "KeyBindPanel(Rect, string[] actions, string[] keys, bool[] listening)");
            InputCandAtoms.KeyBindPanel(col.Take(B_CandKey), CandActions, CandKeys, candListening);

            // ───────────────────────── ② 基础输入 · InputAtoms ─────────────────────────
            Header(ref col, "② 基础输入 · InputAtoms（开关 / 步进 / 滑条 / 搜索 / 标签 / 热键）");

            Caption(ref col, "ToggleSwitch(Rect, ref bool on)");
            InputAtoms.ToggleSwitch(col.Take(B_Toggle), ref toggleOn);

            Caption(ref col, "Stepper(Rect, ref int value, int min, int max, int step = 1)");
            InputAtoms.Stepper(col.Take(B_Stepper), ref stepperVal, 0, 9, 1);

            Caption(ref col, "SliderWithValue(Rect, ref float value, float min, float max, float roundTo = -1, Func<float,string> = null)");
            Rect sliderRect = col.Take(B_Slider);
            InputAtoms.SliderWithValue(new Rect(sliderRect.x, sliderRect.y + 18f, sliderRect.width, sliderRect.height - 18f),
                ref sliderVal, 0f, 1f, -1f, v => (v * 100f).ToString("F0") + "%");

            Caption(ref col, "SearchField(Rect, ref string query, ref string buffer, string placeholder, ref float lastInputTime, float delay = 0.2)");
            InputAtoms.SearchField(col.Take(B_Search), ref searchQuery, ref searchBuffer, "搜索…",
                ref searchLastInput, 0.2f);

            Caption(ref col, "TagPicker(Rect, string[] all, bool[] picked, float gap = 6)");
            InputAtoms.TagPicker(col.Take(B_Tag), Tags, tagPicked, 6f);

            Caption(ref col, "HotkeyBind(Rect, string label, string currentKey)");
            if (InputAtoms.HotkeyBind(col.Take(B_Hotkey), "快速存档", hotkeyListening ? "按新键…" : hotkeyKey))
            {
                hotkeyListening = true;
            }
            if (hotkeyListening && Event.current != null && Event.current.type == EventType.KeyDown
                && Event.current.keyCode != KeyCode.None)
            {
                if (Event.current.keyCode != KeyCode.Escape)
                {
                    hotkeyKey = Event.current.keyCode.ToString();
                }
                hotkeyListening = false;
                Event.current.Use();
            }

            // ───────────────────────── ③ 扩展输入 · InputExAtoms ─────────────────────────
            Header(ref col, "③ 扩展输入 · InputExAtoms（双端滑条 / 取色 / 循环 / 行内编辑 / 数值刷 / 优先级 / 作息）");

            Caption(ref col, "DoubleSlider(Rect, ref float lo, ref float hi, float min, float max, float roundTo = -1)");
            InputExAtoms.DoubleSlider(col.Take(B_Double), ref dsLo, ref dsHi, 0f, 1f, -1f);

            Caption(ref col, "ColorPicker(Rect, ref Color color, ref float h, ref float s, ref float v)");
            InputExAtoms.ColorPicker(col.Take(B_Color), ref cpColor, ref cpH, ref cpS, ref cpV);

            Caption(ref col, "CycleButton(Rect, string label, string[] values, ref int index)");
            InputExAtoms.CycleButton(col.Take(B_Cycle), "难度", CycleVals, ref cycleIdx);

            Caption(ref col, "EditableLabel(Rect, ref string text, ref bool editing, ref string buffer)");
            InputExAtoms.EditableLabel(col.Take(B_Edit), ref editText, ref editEditing, ref editBuffer);

            Caption(ref col, "NumberScrubber(Rect, ref float value, float step, float min, float max, ref bool scrubbing, ref float startX, ref float startVal)");
            InputExAtoms.NumberScrubber(col.Take(B_Scrub), ref scrubVal, 1f, 0f, 100f,
                ref scrubbing, ref scrubStartX, ref scrubStartVal);

            Caption(ref col, "PriorityGrid(Rect, string[] rowLabels, string[] colLabels, int[] values, out int hitIndex)");
            InputExAtoms.PriorityGrid(col.Take(B_Priority), PriRows, PriCols, prioValues, out int pHit);
            if (pHit >= 0 && pHit < prioValues.Length)
            {
                prioValues[pHit] = (prioValues[pHit] + 1) % 4;
            }

            Caption(ref col, "TimetableMatrix(Rect, string[] rowLabels, int[] states, out int hitIndex)");
            InputExAtoms.TimetableMatrix(col.Take(B_Timetable), TtRows, ttStates, out int tHit);
            if (tHit >= 0 && tHit < ttStates.Length)
            {
                ttStates[tHit] = (ttStates[tHit] + 1) % 3;
            }

            // ───────────────────────── ④ 增强输入 · InputPlusAtoms ─────────────────────────
            Header(ref col, "④ 增强输入 · InputPlusAtoms（快捷量 / 刻度滑条 / 滚轮 / 数值 / 路径 / 下拉 / 过滤 / 多选 / 队列 / 冲突 / 多行）");

            Caption(ref col, "QuickAmounts(Rect, string label, ref float value, float max)");
            InputPlusAtoms.QuickAmounts(col.Take(B_Quick), "数量", ref qaVal, 100f);

            Caption(ref col, "SliderTicks(Rect, ref float value, float[] ticks, float min, float max)");
            InputPlusAtoms.SliderTicks(col.Take(B_Ticks), ref tickVal, Ticks, 0f, 20f);

            Caption(ref col, "WheelAdjust(Rect, ref float value, float step, float min, float max, bool invert = false)");
            InputPlusAtoms.WheelAdjust(col.Take(B_Wheel), ref wheelVal, 1f, 0f, 100f, false);

            Caption(ref col, "NumericField(Rect, ref float value, float min, float max, ref string buffer)");
            InputPlusAtoms.NumericField(col.Take(B_Numeric), ref numVal, 0f, 999f, ref numBuf);

            Caption(ref col, "PathField(Rect, ref string path)");
            InputPlusAtoms.PathField(col.Take(B_Path), ref pathVal);

            Caption(ref col, "InPlaceDropdown(Rect, string label, string[] options, int currentIndex, Action<int> onPicked)");
            InputPlusAtoms.InPlaceDropdown(col.Take(B_Dropdown), "预设", DropOpts, dropIdx, i => dropIdx = i);

            Caption(ref col, "FilterRow(Rect, string[] colLabels, string[] filters)");
            Rect filterRect = col.Take(B_Filter);
            InputPlusAtoms.FilterRow(new Rect(filterRect.x, filterRect.y + 13f, filterRect.width, filterRect.height - 13f),
                FilterCols, filterVals);

            Caption(ref col, "MultiSelectList(Rect, string[] items, bool[] selected, ref int anchor, float rowH)");
            InputPlusAtoms.MultiSelectList(col.Take(B_Multi), MsItems, msSel, ref msAnchor, 24f);

            Caption(ref col, "QueueEditor(Rect, int[] queue, ref int count, string[] labels, float rowH)");
            InputPlusAtoms.QueueEditor(col.Take(B_Queue), queue, ref queueCount, QueueLabels, 28f);

            Caption(ref col, "KeyConflictHint(Rect, string[] boundKeys)");
            InputPlusAtoms.KeyConflictHint(col.Take(B_KeyHint), ConflictKeys);

            Caption(ref col, "MultiLineEditor(Rect, ref string text, ref string buffer, int maxLines = 4)");
            InputPlusAtoms.MultiLineEditor(col.Take(B_MultiLine), ref mlText, ref mlBuffer, 4);

            // ───────────────────────── ⑤ 交互原子 · InteractAtoms ─────────────────────────
            Header(ref col, "⑤ 交互原子 · InteractAtoms（拖拽把手 / 可见性眼 / 搜索高亮）");

            Caption(ref col, "GripHandle(Rect, int rows = 3, int cols = 2, Color? color = null)");
            Rect gripRect = col.Take(B_Grip);
            if (InteractAtoms.GripHandle(gripRect, 3, 2, gripOn ? Tokens.Accent : null))
            {
                gripOn = !gripOn;
            }

            Caption(ref col, "EyeToggle(Rect, bool visible, Color? color = null)");
            if (InteractAtoms.EyeToggle(col.Take(B_Eye), eyeVisible, null))
            {
                eyeVisible = !eyeVisible;
            }

            Caption(ref col, "SearchHighlight(Rect, string text, string query, GameFont font = Small, Color? hitColor = null)");
            InteractAtoms.SearchHighlight(col.Take(B_SearchHi), "钢铁长剑的钢铁价格走势", "钢铁", GameFont.Small, null);
        }

        // ── 区块标题 / 行说明（仅绘制，不占用额外布局高度，调用前已 Take 好）──
        // ⚠️ VStack 是 struct，必须以 ref 传入，否则 Take 只改副本、调用方游标不前进。
        private static void Header(ref VStack col, string title)
        {
            Rect r = col.Take(HDR);
            Draw.Box(r, Tokens.PanelRaised);
            Draw.Border(r, Tokens.BorderSoft);
            Draw.Label(Layout.Inset(r, Tokens.CardPadX, 0f), title, Tokens.FontBody, Tokens.Text, TextAnchor.MiddleLeft);
        }

        private static void Caption(ref VStack col, string api)
        {
            Rect r = col.Take(CAP);
            Draw.EllipsisLabel(r, api, GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
        }
    }
}
