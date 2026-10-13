using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
	/// <summary>
	/// 互动组件验证页（1.3.0 新增面）—— 把《互动组件统一化方案》的表现层契约放到<b>实机 IMGUI</b> 下，
	/// 让问题自己暴露出来（这是验证台存在的唯一意义）。
	///
	/// <para><b>本页只做「接线与布局」</b>：所有组件绘制一律走 RUIF 公开 API，
	/// 页内<b>不自造任何第二套配色 / 画法</b> —— 自造一套就等于在验证自己，而不是在验证框架。</para>
	///
	/// <para><b>覆盖面</b>（引用一律按章节号：《方案》多轮并发编辑，行号会漂移）：
	/// ① 按钮族 6 态 × 4 tone（§5.3）· ② <c>IconButton</c> 4 tone（§5.9）
	/// ③ 选择件三态（§5.12）· ④ <c>RadioGroup</c> 纵向互斥（§5.12）
	/// ⑤ <c>Segmented</c> 的 <c>tone</c> 对照（§5.3 的 A 类要求）· ⑥ <c>WindowFrame</c> 骨架与 n=1 特例（§5.1/§5.2）
	/// ⑦ 降级阶梯 D0/D1/D3/D4/D6（§5.6；D2 为**预留档**、本批无对象 —— 见该段注记）· ⑧ 禁用态量化差（§5.4）。</para>
	///
	/// <para><b>状态归属</b>：组件一律零状态（架构部硬约束）⇒ 本页作为调用方持有全部状态，经 <c>ref</c> 传入。
	/// 这本身就是「组件零状态」在真实消费方视角下的验证。</para>
	///
	/// <para><b>焦点态</b>：平台无 UI 焦点类型（§4.4），按钮族聚焦是<b>显式态</b> ⇒ 本页用
	/// <c>focusRing: true</c> 主动驱动，<b>不</b>断言 Tab 遍历 / 键盘导航（§6 ①，B 类引擎降级）。</para>
	/// </summary>
	internal sealed class BenchButtons : IBench
	{
		// ══════════════════════════════════════════════════════════════════
		// 度量（一律派生；禁手写魔数）
		// ══════════════════════════════════════════════════════════════════

		/// <summary>单元间距（<c>Tokens.SpaceXs</c> = 8）—— 全页统一，不再引入「看起来差不多」的第二个值。</summary>
		private const float G = Tokens.SpaceXs;

		/// <summary><c>Atoms.SectionFrame</c> 的上下内边距合计（= 实测 <c>Inset(outer, 8, 12)</c> 垂直 24，B-R10 反编译取证）。</summary>
		private const float FramePadV = Tokens.SpaceSm * 2f;

		/// <summary>标准图标钮本体边长（§5.9：与 <c>ToolbarIcon</c> 一致 = 28×28）。资产规格常量，非排版魔数。</summary>
		private const float IconBtnSize = 28f;

		/// <summary>§5.6 的 D2/D3 档阈值下限（28f）。</summary>
		private const float NarrowFloor = 28f;

		/// <summary>焦点环外扩量（§5.5：外扩 2px ⇒ 调用方须为末位单元右侧留出该边距，否则外环被邻件/容器边框压住）。</summary>
		private const float RingOutset = 2f;

		/// <summary>禁用态对照矩阵的单元数（正常/禁用 × Default/Danger —— 与 Draw7 的 4 次调用一一对应）。</summary>
		private const int DisabledCells = 4;

		// ⚠ 以下刻意用「静态属性」而**不是** static readonly：
		//   Measure 的文档（Measure.cs:16-18）明确要求行高**不得缓存** —— Text.TinyFontSupported
		//   运行期可变（玩家可在选项里关掉小字体），「首次访问即定值」会冻错值。
		//   Measure.Width 同为「不缓存」设计（Measure.cs:61-75，其原缓存键不含字号，跨字号取错值）。
		//   ⇒ 这里每次访问都向引擎实取；代价是 Text 真值表查表 / CalcSize，可接受（本页是开发工具页）。

		/// <summary>纯文本行高（Tiny 真值，引擎实测）。</summary>
		private static float NoteH => Measure.RowHeight(GameFont.Tiny);

		/// <summary>可点行高 = RowHeight + 8（按钮 / 列表行，Measure.cs:58）。</summary>
		private static float CtrlH => Measure.ControlHeight(Tokens.FontLabel);

		/// <summary>选择件盒边长：§5.12 = <c>Measure.RowHeight(Tokens.FontLabel)</c>（≈18，<b>不是</b>原版 24）。</summary>
		private static float BoxW => Measure.RowHeight(Tokens.FontLabel);

		/// <summary>演示按钮标准宽 = 最长标签实测宽 + 两侧内边距（PadX 8×2 = SpaceXs×2，§5.2）。</summary>
		private static float BtnW => Measure.Width(LabelPrimary, Tokens.FontLabel) + Tokens.SpaceXs * 2f;

		/// <summary>
		/// 原版 <c>Text.ClampTextWithEllipsis</c> 的固定保留量（13f，<c>Verse.Text.cs:221-242</c> 反编译实测，1.6）：
		/// 其「不截断」的判定是 <c>W(text) ≤ rect.width − 13f</c>（另：<c>text.Length ≤ 4</c> 直接原样返回）。
		/// 这是<b>引擎契约常量</b>（非排版魔数，来历唯一）。
		/// ⇒ 任何走 <c>Draw.EllipsisLabel</c> 的<b>定宽</b>列，rect 宽必须 = 实测文本宽 + 本值；
		/// 否则会「量准了也照样出省略号」—— 本项目实机事故即此：tone 标签列显示 De… / Acc… / Dan…。
		/// </summary>
		private const float EllipsisReserve = 13f;

		/// <summary>tone 标签列宽 = 最长 tone 名<b>实测</b>宽 + 引擎省略号保留量（两者都是硬下限，缺一即被截断）。</summary>
		private static float ToneLabW => MaxWidth(ToneNames) + EllipsisReserve;

		/// <summary>S1 一行内放下「tone 标签 + 4 个状态单元」所需宽（含 §5.5 的焦点环外边距）。</summary>
		private static float S1InlineNeed => ToneLabW + 4f * BtnW + 4f * G + RingOutset;

		/// <summary>数组内最长标签的实测宽 —— 用于等宽单元，避免逐格宽度不一导致右边界参差。</summary>
		private static float MaxWidth(string[] labels)
		{
			float m = 0f;
			for (int i = 0; i < labels.Length; i++) m = Mathf.Max(m, Measure.Width(labels[i], Tokens.FontLabel));
			return m;
		}

		/// <summary>取整显示（页面注记用；数字等宽由 Draw 的字号保证）。</summary>
		private static float Num(float v) => Mathf.RoundToInt(v);

		// ══════════════════════════════════════════════════════════════════
		// 样例数据（明确标注为样例；非运行时数据）
		// ══════════════════════════════════════════════════════════════════

		private const string LabelPrimary = "主要操作";
		private const string LabelDisabled = "不可用";
		private const string LabelFocus = "聚焦态";
		private const string LabelLong = "保存并应用全部修改";

		private static readonly ButtonTone[] Tones =
		{
			ButtonTone.Default, ButtonTone.Accent, ButtonTone.Danger, ButtonTone.Warn
		};

		private static readonly string[] ToneNames = { "Default", "Accent", "Danger", "Warn" };

		/// <summary>4 个状态单元的名字（顺序即绘制顺序）。</summary>
		private static readonly string[] CellNames = { "默认·悬停·按下", "已选", "禁用", "聚焦" };

		/// <summary>
		/// ① 段的<b>列名行</b>文本（构建期拼好，绘制路径零分配）。
		/// <para>刻意显式写出列数：原先把「列名 + 说明」连排一行，读者会把说明也数成列（实机反馈：像 7 列）。
		/// 现在说明另起一行（见 <c>Draw0</c>），本行<b>只</b>列列名。</para>
		/// <para>说明：<see cref="CellNames"/> 是 <c>static readonly</c>，故本串恒定 ⇒ 可安全缓存（非 Measure 量，不受字号切换影响）。</para>
		/// </summary>
		private static readonly string ColHeaderLine =
			"列名（共 " + CellNames.Length + " 列）｜" + string.Join(" ｜ ", CellNames);

		private static readonly string[] SegOptions = { "许可", "警戒", "禁止" };

		private static readonly string[] RadioOptions = { "低", "中", "高", "自定义" };
		private static readonly string[] RadioOptionsB = { "仅好友", "全部", "仅自己", "跟随全局" };

		private static readonly string[] CbLabels = { "启用警报", "记录日志", LabelDisabled };
		private static readonly string[] TriLabels = { "全不选", "全选", "部分选中" };

		private static readonly string[] FooterLabels3 = { "取消", "应用", "确定" };
		private static readonly string[] FooterLabels1 = { "关闭" };
		private static readonly string[] FooterLabels5 = { "取消", "应用", "确定", "重置", "关闭" };

		// ══════════════════════════════════════════════════════════════════
		// 交互状态（归调用方；组件零状态）
		// ══════════════════════════════════════════════════════════════════

		/// <summary>S1：每个 tone 一个「已选」开关（<c>ToggleButton</c> 的 ref 状态）。</summary>
		private readonly bool[] toneOn = new bool[4];

		/// <summary>S2：图标钮命中计数（验证返回值真的到手）。</summary>
		private int iconHits;

		/// <summary>S3：勾选框二态状态。</summary>
		private readonly bool[] cbOn = { true, false, false };

		/// <summary>S3：勾选框三态状态（Off / On / Partial 三态齐全）。</summary>
		private readonly TriState[] cbTri = { TriState.Off, TriState.On, TriState.Partial };

		/// <summary>S4：两个纵向单选组的当前项（组间各自独立 ⇒ 多实例不互相污染）。</summary>
		private int radioA, radioB = 2;

		/// <summary>S5：分段件当前段（两个可用组<b>同下标</b> ⇒ 颜色差异只可能来自 tone）。</summary>
		private int segDefault = 1, segDanger = 1, segDisabled = 1;

		/// <summary>S6/S7：底栏与窄容器按钮的命中计数。</summary>
		private int footerHits;

		/// <summary>已记过异常的段名（每段只报一次，避免每帧刷屏）。</summary>
		private readonly HashSet<string> faulted = new HashSet<string>();

		/// <inheritdoc />
		public bool Scrollable => true;

		// ══════════════════════════════════════════════════════════════════
		// ⚠ 接口适配区（本页唯一的不确定面）
		//
		// 《方案》§3.4 冻结的是按钮族 / 选择件 / Segmented 重载的签名；
		// WindowFrame 与 WindowFmt 的签名与成员名**未冻结**（§5.2 只给常量表，
		// §5.4 只说「void + out contentRect/footerRect」）。本类按 §5.1/§5.2/§5.4 的语义推定，
		// 并把**全部调用点收敛于此** —— 机制部给出真实签名后，只需改本类内部，
		// 验证页其余部分零改动。
		// ══════════════════════════════════════════════════════════════════

		private static class FrameApi
		{
			public static void Draw(Rect rect, string title, out Rect content, out Rect footer)
				=> WindowFrame.Draw(rect, title, out content, out footer);

			public static float TitleHeight => WindowFmt.TitleHeight;
			public static float ContentInset => WindowFmt.ContentInset;
			public static float FooterHeight => WindowFmt.FooterHeight;
			public static float FooterInsetX => WindowFmt.FooterInsetX;
			public static float FooterButtonHeight => WindowFmt.FooterButtonHeight;

			// 机制部实际实现：无 Vector2 成员，实为 SingleButtonWidth (120f) + SingleButtonHeight (32f)
			// 两个 float const —— 就地合并（本页只在此处消费 §5.2 的 120×32 常量对，验证页其余零改动）。
			public static Vector2 SingleButtonSize
				=> new Vector2(WindowFmt.SingleButtonWidth, WindowFmt.SingleButtonHeight);

			public static float FooterButtonWidth(float w, int n) => WindowFmt.FooterButtonWidth(w, n);

			// §5.2 冻结的槽位几何 —— 调用方**不自算** footer 按钮矩形，直接用框架的同一实现。
			// （自算 = 同功能两处实现：§5.2 一改，验证页的「实证」就悄悄失真。）
			public static Rect FooterButtonRect(Rect footerRect, int n, int i)
				=> WindowFmt.FooterButtonRect(footerRect, n, i);

			public static Rect SingleButtonRect(Rect footerRect) => WindowFmt.SingleButtonRect(footerRect);
		}

		// ══════════════════════════════════════════════════════════════════
		// 布局计划
		//
		// Height() 与绘制用**同一个行高累加器**（栈上结构体，零堆分配）按同一顺序累加
		// ⇒ 高度与绘制不可能对不上。对不上的表现是「最后一行被裁掉」，而 IMGUI 不会报错。
		// ══════════════════════════════════════════════════════════════════

		/// <summary>栈上行高累加器：自动计入行间 <see cref="G"/>。</summary>
		private struct VSum
		{
			public float Sum;
			public int Count;

			public void Add(float h)
			{
				if (Count > 0) Sum += G;
				Sum += h;
				Count++;
			}
		}

		/// <summary>等宽单元分档：可用宽能容下几列（1..cells）。</summary>
		private static int Cols(float avail, int cells, float cellW)
		{
			int fit = Mathf.FloorToInt((avail + G) / (cellW + G));
			return Mathf.Clamp(fit, 1, Mathf.Max(1, cells));
		}

		/// <summary>按列数算行数。</summary>
		private static int RowsOf(int cells, int cols) => Mathf.CeilToInt(cells / (float)Mathf.Max(1, cols));

		/// <summary>网格总高（<c>Layout.Grid</c> 的行进量 = cellH + gap）。</summary>
		private static float GridH(int cells, int cols, float cellH)
		{
			int rows = RowsOf(cells, cols);
			return rows * cellH + (rows - 1) * G;
		}

		// ── 各段内部量（Height 与绘制共享，口径唯一）──────────────────────

		/// <summary>S1 是否单行（宽够则 tone 标签与 4 个状态单元同行）。</summary>
		private static bool S1Inline(float w) => w >= S1InlineNeed;

		/// <summary>S1 每个 tone 行占高（单行 = CtrlH；窄档 = 标签行 + 网格）。</summary>
		private static float S1ToneH(float w)
			=> S1Inline(w) ? CtrlH : NoteH + G + GridH(4, Cols(w, 4, BtnW), CtrlH);

		/// <summary>图标钮单元高（本体 28 高于可点行高 26 ⇒ 取大者，否则本体被压）。</summary>
		private static float IconCellH => Mathf.Max(CtrlH, IconBtnSize);

		private static float CbCellW => BoxW + G + MaxWidth(CbLabels);
		private static float TriCellW => BoxW + G + MaxWidth(TriLabels);

		/// <summary>S4 一个单选组所需高（§5.12：总需高 = n × rowH，行间不加 gap）。</summary>
		private static float RadioGroupH => RadioOptions.Length * CtrlH;

		/// <summary>S4 一个单选组所需宽。</summary>
		private static float RadioGroupW => BoxW + G + Mathf.Max(MaxWidth(RadioOptions), MaxWidth(RadioOptionsB)) + 2f * G;

		/// <summary>S5 分段件宽度（按最长选项实测，每段两侧留呼吸）。</summary>
		private static float SegW => SegOptions.Length * (MaxWidth(SegOptions) + 2f * G);

		/// <summary>一个窗口骨架所需高（§5.2：标题区 + 内容内边距×2 + 内容 + 底带）。</summary>
		private static float FrameH => FrameApi.TitleHeight + FrameApi.ContentInset + 2f * NoteH + FrameApi.ContentInset + FrameApi.FooterHeight;

		/// <summary>降级试管容器高（比控件高多一圈呼吸，让「窄容器」本身可见）。</summary>
		private static float TubeH => CtrlH + 2f * Tokens.SpaceXxs;

		/// <summary>D4 垂直堆叠区高（n × 带高 + 条间呼吸）。</summary>
		private static float StackH => FooterLabels5.Length * FrameApi.FooterButtonHeight + (FooterLabels5.Length - 1) * Tokens.SpaceXxs;

		// ══════════════════════════════════════════════════════════════════
		// 高度：逐段累加（顺序与 Render 完全一致）
		// ══════════════════════════════════════════════════════════════════

		/// <inheritdoc />
		public float Height(float width)
		{
			// Layout.BeginScroll 恒扣 ScrollbarThickness（Layout.cs:44：view.width = outer.width − 18）
			// ⇒ 计划必须按「扣完的内容宽」算，才能与 Render 拿到的 area.width 逐像素一致。
			float w = Mathf.Max(0f, width - Tokens.ScrollbarThickness);
			float total = 0f;
			for (int i = 0; i < 8; i++)
			{
				if (i > 0) total += Tokens.SpaceMd;   // 与 Layout.Column(area, Tokens.SpaceMd) 同间距
				total += SectionH(i, w);
			}
			return total;
		}

		private static float SectionH(int index, float w)
		{
			var st = new VSum();
			switch (index)
			{
				case 0:
					st.Add(NoteH);                                  // 列名行（**只**放 4 个列名）
					st.Add(NoteH);                                  // 注记行（说明与列名分行 ⇒ 不再被读成 7 列）
					for (int t = 0; t < 4; t++) st.Add(S1ToneH(w));   // 4 个 tone 行
					break;
				case 1:
					st.Add(NoteH);
					st.Add(GridH(6, Cols(w, 6, IconBtnSize), IconCellH));
					break;
				case 2:
					st.Add(NoteH);
					st.Add(GridH(3, Cols(w, 3, CbCellW), CtrlH));
					st.Add(NoteH);
					st.Add(GridH(3, Cols(w, 3, TriCellW), CtrlH));
					break;
				case 3:
					st.Add(NoteH);
					if (w >= 2f * RadioGroupW + G) st.Add(RadioGroupH);
					else
					{
						st.Add(NoteH); st.Add(RadioGroupH);
						st.Add(NoteH); st.Add(RadioGroupH);
					}
					break;
				case 4:
					st.Add(NoteH);
					for (int i = 0; i < 3; i++) { st.Add(NoteH); st.Add(CtrlH); }
					break;
				case 5:
					st.Add(NoteH); st.Add(FrameH);
					st.Add(NoteH); st.Add(FrameH);
					break;
				case 6:
					st.Add(NoteH);
					st.Add(TubeH); st.Add(TubeH);                   // D0 / D1
					st.Add(NoteH);                                  // D2 预留档说明（§5.6 勘误）
					st.Add(TubeH); st.Add(TubeH); st.Add(TubeH);    // D2 / D3 / D6
					st.Add(NoteH); st.Add(StackH);                  // D4
					break;
				default:
					st.Add(NoteH);
					st.Add(GridH(DisabledCells, Cols(w, DisabledCells, BtnW), CtrlH));
					st.Add(NoteH); st.Add(CtrlH);                   // Keyed 禁用提示探针
					st.Add(NoteH); st.Add(NoteH); st.Add(NoteH);     // 量化差说明 ×3
					break;
			}
			return FramePadV + st.Sum;
		}

		// ══════════════════════════════════════════════════════════════════
		// 绘制
		// ══════════════════════════════════════════════════════════════════

		/// <inheritdoc />
		public void Render(Rect area)
		{
			float w = area.width;
			var col = Layout.Column(area, Tokens.SpaceMd);

			// ⚠ 逐段 try/catch：一段抛异常不得拖垮整页，更不能逃出 DoWindowContents
			//   （那会断裂 BeginScroll/EndScroll 与 GUIClip 的入栈出栈 ⇒ 整局 GUI 卡死）。
			//   容器底色由 Atoms.SectionFrame 先画、内容后画 —— 顺序不可颠倒。
			Draw0(col.Take(SectionH(0, w)), w);
			Draw1(col.Take(SectionH(1, w)), w);
			Draw2(col.Take(SectionH(2, w)), w);
			Draw3(col.Take(SectionH(3, w)), w);
			Draw4(col.Take(SectionH(4, w)), w);
			Draw5(col.Take(SectionH(5, w)), w);
			Draw6(col.Take(SectionH(6, w)), w);
			Draw7(col.Take(SectionH(7, w)), w);
		}

		/// <summary>段级异常隔离（只报一次）。</summary>
		private void Fault(string name, Exception e)
		{
			if (faulted.Add(name))
				Log.ErrorOnce("[RUIF_Demo] BenchButtons 段「" + name + "」抛异常，已隔离（其余段不受影响）：\n" + e, name.GetHashCode());
		}

		/// <summary>段隔离占位（画「— + 原因」，而不是静默消失）。</summary>
		private static void FaultPlaceholder(Rect outer)
			=> Draw.Label(outer, "— 本段已隔离（详见日志）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft);

		// ── ① 按钮族：6 态 × 4 tone（§5.3）──────────────────────────────

		private void Draw0(Rect outer, float w)
		{
			var c = Layout.Column(Atoms.SectionFrame(outer,
				"① 按钮族 · PrimaryButton / ToggleButton —— 6 态 × 4 tone（§5.3）"), G);
			try
			{
				// ① 列名行：**只**列 4 个列名（并显式标出「共 4 列」）——与下方 4 列一一对应。
				Draw.EllipsisLabel(c.Take(NoteH), ColHeaderLine,
					GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);

				// ② 注记行（**另起一行**）：原先「列名 + 说明」连排同一行，读者会把说明也数成列
				//    （实机反馈：看起来像 7 列）。两行靠**色阶**分层（列名 Text / 说明 Dim）⇒ 一眼可分。
				//    说明内容原样保留（未删减）：悬停/按住的真实鼠标通路、focusRing 显式驱动、§6① 不承诺 Tab 遍历。
				// Default / Hover / Pressed 只能由真实鼠标产生 —— 这正是验证台存在的意义。
				Draw.EllipsisLabel(c.Take(NoteH),
					"交互说明：悬停看 Hover、按住看 Pressed；聚焦 = focusRing:true 显式驱动（§6① 不承诺 Tab 遍历）",
					GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

				for (int t = 0; t < Tones.Length; t++)
				{
					Rect line = c.Take(S1ToneH(w));
					if (S1Inline(w))
					{
						var row = Layout.Row(line, G);
						Draw.EllipsisLabel(row.Take(ToneLabW), ToneNames[t], GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
						for (int i = 0; i < CellNames.Length; i++) BtnCell(row.Take(BtnW), t, i);
					}
					else
					{
						var body = Layout.Column(line, G);
						Draw.EllipsisLabel(body.Take(NoteH), ToneNames[t], GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
						int cols = Cols(w, CellNames.Length, BtnW);
						var g = Layout.Grid(body.Take(GridH(CellNames.Length, cols, CtrlH)), cols, CtrlH, G);
						for (int i = 0; i < CellNames.Length; i++) BtnCell(g.Next(), t, i);
					}
				}
			}
			catch (Exception e) { Fault("① 按钮族", e); FaultPlaceholder(outer); }
		}

		/// <summary>4 个状态单元：0 实时（Default/Hover/Pressed）｜1 已选｜2 禁用｜3 聚焦。</summary>
		private void BtnCell(Rect r, int toneIndex, int cell)
		{
			ButtonTone tone = Tones[toneIndex];
			switch (cell)
			{
				case 0:
					if (ButtonAtoms.PrimaryButton(r, LabelPrimary, true, tone)) footerHits++;
					break;
				case 1:
					ButtonAtoms.ToggleButton(r, "已选", ref toneOn[toneIndex], true, tone);
					break;
				case 2:
					ButtonAtoms.PrimaryButton(r, LabelDisabled, false, tone);
					break;
				default:
					ButtonAtoms.PrimaryButton(r, LabelFocus, true, tone, focusRing: true);
					break;
			}
		}

		// ── ② IconButton：4 tone + 禁用 + 聚焦（§5.9）───────────────────

		private void Draw1(Rect outer, float w)
		{
			var c = Layout.Column(Atoms.SectionFrame(outer,
				"② IconButton · 4 tone + 禁用 + 聚焦（rect 即按钮本体 28×28，§5.9）"), G);
			try
			{
				Draw.EllipsisLabel(c.Take(NoteH),
					"字形限 BMP 非 emoji 几何符号（§5.9 负面清单：✗ emoji / ✗ PNG 才能表达的图标）；本轮命中 " + iconHits + " 次",
					GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

				int cols = Cols(w, 6, IconBtnSize);
				var g = Layout.Grid(c.Take(GridH(6, cols, IconCellH)), cols, IconCellH, G);
				for (int t = 0; t < 4; t++)
					if (ButtonAtoms.IconButton(CenterIn(g.Next(), IconBtnSize), GlyphFor(Tones[t]), true, Tones[t])) iconHits++;
				if (ButtonAtoms.IconButton(CenterIn(g.Next(), IconBtnSize), "⟳", false, ButtonTone.Default)) iconHits++;
				if (ButtonAtoms.IconButton(CenterIn(g.Next(), IconBtnSize), "⚠", true, ButtonTone.Warn, focusRing: true)) iconHits++;
			}
			catch (Exception e) { Fault("② IconButton", e); FaultPlaceholder(outer); }
		}

		private static string GlyphFor(ButtonTone tone)
		{
			switch (tone)
			{
				case ButtonTone.Accent: return "＋";
				case ButtonTone.Danger: return "×";
				case ButtonTone.Warn: return "⚠";
				default: return "⟳";
			}
		}

		/// <summary>在单元内居中的方形按钮本体（单元可能比本体宽 / 高）。</summary>
		private static Rect CenterIn(Rect cell, float size)
		{
			float s = Mathf.Min(size, Mathf.Min(cell.width, cell.height));
			return new Rect(cell.center.x - s * 0.5f, cell.center.y - s * 0.5f, s, s);
		}

		// ── ③ 选择件：Checkbox / CheckboxTri（§5.12）────────────────────

		private void Draw2(Rect outer, float w)
		{
			var c = Layout.Column(Atoms.SectionFrame(outer,
				"③ 选择件 · Checkbox / CheckboxTri —— 盒 + 框内符号；盒在左、标签在右（§5.12）"), G);
			try
			{
				Draw.EllipsisLabel(c.Take(NoteH),
					"Checkbox：开 / 关 / 禁用 ｜ 可点区 = 整行（盒 + 间距 + 标签并集），非仅盒子 ｜ 盒边长 = RowHeight(FontLabel)（不是原版 24）",
					GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

				int ca = Cols(w, 3, CbCellW);
				var ga = Layout.Grid(c.Take(GridH(3, ca, CtrlH)), ca, CtrlH, G);
				for (int i = 0; i < CbLabels.Length; i++)
					InputAtoms.Checkbox(ga.Next(), CbLabels[i], ref cbOn[i], i != 2, ButtonTone.Default);

				Draw.EllipsisLabel(c.Take(NoteH),
					"CheckboxTri 三态齐全：Off 空盒 ｜ On 几何对勾（三点折线，非字符 ✓）｜ Partial 居中实心方块；Partial 标签保持 Text（半选不宣称已选）",
					GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

				int ct = Cols(w, 3, TriCellW);
				var gb = Layout.Grid(c.Take(GridH(3, ct, CtrlH)), ct, CtrlH, G);
				for (int i = 0; i < TriLabels.Length; i++)
					InputAtoms.CheckboxTri(gb.Next(), TriLabels[i], ref cbTri[i], true, ButtonTone.Default);
			}
			catch (Exception e) { Fault("③ 选择件", e); FaultPlaceholder(outer); }
		}

		// ── ④ RadioGroup：纵向互斥（§5.12）──────────────────────────────

		private void Draw3(Rect outer, float w)
		{
			var c = Layout.Column(Atoms.SectionFrame(outer,
				"④ RadioGroup · 纵向互斥（圆点在左、标签在右；总需高 = n × rowH，禁止压缩 rowH）"), G);
			try
			{
				Draw.EllipsisLabel(c.Take(NoteH),
					"两组并排、各自独立（多实例不互相污染）：tone: Default ｜ tone: Danger —— 当前 " + radioA + " / " + radioB
					+ "；rowH 传 null ⇒ 组件按 Measure.ControlHeight 实取（不给 22f 之类字面默认值）",
					GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

				if (w >= 2f * RadioGroupW + G)
				{
					var row = Layout.Row(c.Take(RadioGroupH), G);
					InputAtoms.RadioGroup(row.Take(RadioGroupW), RadioOptions, ref radioA, null, true, ButtonTone.Default);
					InputAtoms.RadioGroup(row.Take(RadioGroupW), RadioOptionsB, ref radioB, null, true, ButtonTone.Danger);
				}
				else
				{
					Draw.EllipsisLabel(c.Take(NoteH), "tone: Default", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
					InputAtoms.RadioGroup(c.Take(RadioGroupH), RadioOptions, ref radioA, null, true, ButtonTone.Default);
					Draw.EllipsisLabel(c.Take(NoteH), "tone: Danger", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft);
					InputAtoms.RadioGroup(c.Take(RadioGroupH), RadioOptionsB, ref radioB, null, true, ButtonTone.Danger);
				}
			}
			catch (Exception e) { Fault("④ RadioGroup", e); FaultPlaceholder(outer); }
		}

		// ── ⑤ Segmented 的 tone 对照（本批 A 类要求）────────────────────

		private void Draw4(Rect outer, float w)
		{
			var c = Layout.Column(Atoms.SectionFrame(outer,
				"⑤ Segmented 增 tone 重载 —— A 类重点：选中段必须随 tone 变（§5.3 落地要求）"), G);
			try
			{
				Draw.EllipsisLabel(c.Take(NoteH),
					"两个可用组**同下标**（都选第 2 段）⇒ 差异只可能来自 tone。若 Danger 组选中段仍是蓝色 accent ⇒ tone 形同虚设（命中反劣质第 5 条「语义色被换成单色阶」）。",
					GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

				SegRow(c.Take(NoteH + G + CtrlH), "tone: Default（选中段应为 Accent）", ref segDefault, true, ButtonTone.Default, Tokens.Muted);
				SegRow(c.Take(NoteH + G + CtrlH), "tone: Danger（选中段应为 Dead 红）", ref segDanger, true, ButtonTone.Danger, Tokens.Dead);
				SegRow(c.Take(NoteH + G + CtrlH), "tone: Warn + enabled:false（禁用：剥除 tone + 音效静音）", ref segDisabled, false, ButtonTone.Warn, Tokens.Dim);
			}
			catch (Exception e) { Fault("⑤ Segmented", e); FaultPlaceholder(outer); }
		}

		private static void SegRow(Rect line, string caption, ref int index, bool enabled, ButtonTone tone, Color capInk)
		{
			var body = Layout.Column(line, G);
			Draw.EllipsisLabel(body.Take(NoteH), caption, GameFont.Tiny, capInk, TextAnchor.MiddleLeft);
			Rect seg = body.Take(CtrlH);
			InputAtoms.Segmented(new Rect(seg.x, seg.y, Mathf.Min(SegW, seg.width), seg.height),
				SegOptions, ref index, enabled, tone);
		}

		// ── ⑥ WindowFrame 骨架（§5.1 / §5.2）───────────────────────────

		private void Draw5(Rect outer, float w)
		{
			var c = Layout.Column(Atoms.SectionFrame(outer,
				"⑥ WindowFrame · 只画 chrome（标题条 / 分隔线 / 底栏槽），交出 contentRect / footerRect"), G);
			try
			{
				Draw.EllipsisLabel(c.Take(NoteH),
					"骨架自身不触发（纯结构件）；底栏按钮由调用方用 WindowFmt 常量摆放。n = 3 等分 W/n−"
					+ Num(FrameApi.FooterInsetX) + "（FooterGap = 0）｜ n = 1 回落 "
					+ Num(FrameApi.SingleButtonSize.x) + "×" + Num(FrameApi.SingleButtonSize.y)
					+ " 居中（§5.1 / §5.2）",
					GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

				FrameDemo(c.Take(FrameH), "殖民地概览 · 空骨架（n = 3，主按钮最右）", FooterLabels3);

				Draw.EllipsisLabel(c.Take(NoteH),
					"n = 1 特例：单按钮不拉伸到近满宽，回落 Dialog_Confirm.ButtonSize = "
					+ Num(FrameApi.SingleButtonSize.x) + "×" + Num(FrameApi.SingleButtonSize.y)
					+ " 居中 ｜ 全页按钮命中累计 " + footerHits + " 次（S1 实时钮 / 本段底栏 / ⑦ 试管与堆叠同计）",
					GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

				FrameDemo(c.Take(FrameH), "确认关闭 · 单按钮特例（n = 1）", FooterLabels1);
			}
			catch (Exception e) { Fault("⑥ WindowFrame", e); FaultPlaceholder(outer); }
		}

		/// <summary>画一个窗口骨架，并把按钮条摆进它交出的 footerRect（骨架只交 rect，按钮由调用方摆）。</summary>
		private void FrameDemo(Rect frame, string title, string[] labels)
		{
			FrameApi.Draw(frame, title, out Rect contentRect, out Rect footerRect);

			// contentRect 已含骨架内边距（§5.2 ContentInset = 18）⇒ 直接往里面填，不再套 Inset
			Draw.EllipsisLabel(contentRect, "contentRect —— 调用方在此填任意内容（骨架不干预内容语义）",
				GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

			int n = labels.Length;
			if (n == 1)
			{
				// n == 1 特例几何**不自算**：直接用 §5.2 冻结的 SingleButtonRect（120×32 居中）。
				Rect btn = FrameApi.SingleButtonRect(footerRect);
				if (ButtonAtoms.PrimaryButton(btn, labels[0])) footerHits++;
				return;
			}

			for (int i = 0; i < n; i++)
			{
				// 槽位几何**不自算**：直接用 §5.2 冻结的 FooterButtonRect（x = 槽起+10、w = W/n−10、h = 35）。
				// 主按钮最右（§4.5 Dialog_MessageBox：主按钮最右、其次最左、再次居中）⇒ 末位用 Accent。
				Rect btn = FrameApi.FooterButtonRect(footerRect, n, i);
				if (ButtonAtoms.PrimaryButton(btn, labels[i], true,
					i == n - 1 ? ButtonTone.Accent : ButtonTone.Default)) footerHits++;
			}
		}

		// ── ⑦ 降级阶梯 D0 / D1 / D2 / D3 / D4 / D6（§5.6）──────────────

		private enum Probe { Primary, Icon }

		private void Draw6(Rect outer, float w)
		{
			var c = Layout.Column(Atoms.SectionFrame(outer,
				"⑦ 空间预算与降级阶梯 —— 窄容器必须肉眼可见地降级，而不是硬挤 / 静默截断 / 画到容器外（§5.6）"), G);
			try
			{
				Draw.EllipsisLabel(c.Take(NoteH),
					"阈值按 Measure.Width(label, FontLabel) 实测判定，不按字符数估算（Tiny 下 CJK 80f ≈ 3 字）。每支试管标出派生宽与命中档。",
					GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

				// D0 阈：§5.6 = W(label) + 32（32 = PadX 8×2 + icon 18 + Gap 6）。
				// 这里用 SpaceLg(24) + SpaceXs(8) 表达该 32，使 D0 在「有图标 / 无图标」两种口径下都成立（宽档不产生歧义）。
				float d0 = Measure.Width(LabelPrimary, Tokens.FontLabel) + Tokens.SpaceLg + Tokens.SpaceXs;
				float d1 = Measure.Width(LabelLong, Tokens.FontLabel);              // ∈[48, W+32) ⇒ D1
				float d23 = CtrlH + Tokens.SpaceXxs;                               // ∈[28,48) ⇒ D2 / D3（同宽 A/B）
				float d6 = BoxW;                                                  // < 28 ⇒ D6

				Tube(c.Take(TubeH), d0, "D0 原样 ｜ w = W(label)+32 = " + Num(d0) + " ⇒ 不截断", Probe.Primary, LabelPrimary);
				Tube(c.Take(TubeH), d1, "D1 文本省略 ｜ w=" + Num(d1) + " ≈ W(长标签) ⇒ EllipsisLabel + 悬停给全称", Probe.Primary, LabelLong);

				// §5.6 勘误（主理人裁决）：D2「仅图标」在本批**无对象** —— PrimaryButton 无图标通道、
				// IconButton 无文字 ⇒ 「藏文字、图标居中」这一分支根本不存在。保留并排只为对比 D2/D3 的
				// **空间门槛**，计入「方案勘误 / 预留档」，**不是**「降级演示」。
				Draw.EllipsisLabel(c.Take(NoteH),
					"D2 为**预留档**：本批组件无「图标+文字」形态，该档**无对象**；此处并排仅为展示 D2/D3 的**空间门槛**差异。",
					GameFont.Tiny, Tokens.Warn, TextAnchor.MiddleLeft);

				Tube(c.Take(TubeH), d23, "D2 预留档（A 支 = IconButton：本身只有图标，非「藏了文字」）｜ w=" + Num(d23), Probe.Icon, "⟳");
				Tube(c.Take(TubeH), d23, "D3 收窄内边距（B 支 = PrimaryButton：无图标）｜ 同宽 ⇒ PadX 8→4 / Gap 6→2（下限 4/2，不归零）", Probe.Primary, LabelPrimary);
				Tube(c.Take(TubeH), d6, "D6 省略该件 ｜ w=" + Num(d6) + " < 28 ⇒ 画「—」+ 原因占位并登记缺口", Probe.Primary, LabelPrimary);

				float n = FooterLabels5.Length;
				float band = FrameApi.FooterButtonWidth(Tokens.SidebarWidth, (int)n);
				Draw.EllipsisLabel(c.Take(NoteH),
					"D4 换档 ｜ n=" + n + "，容器 = SidebarWidth → w_btn = W/n−10 = " + Num(band)
					+ " < 64 ⇒ 底栏带不可用，改**垂直堆叠**（每行全宽钮，总高 n×35f；✗ 不压带高、✗ 不做带内换行）",
					GameFont.Tiny, band < 64f ? Tokens.Warn : Tokens.Dim, TextAnchor.MiddleLeft);

				FooterStack(c.Take(StackH));
			}
			catch (Exception e) { Fault("⑦ 降级阶梯", e); FaultPlaceholder(outer); }
		}

		/// <summary>画一支「试管」：可见的窄容器（1px 边框）+ 注明派生宽与命中档，组件按该宽绘制。</summary>
		private void Tube(Rect tube, float testW, string caption, Probe kind, string label)
		{
			float cw = Mathf.Min(testW, tube.width);
			var box = new Rect(tube.x, tube.y, cw, tube.height);
			Draw.Box(box, Tokens.Window);
			Draw.Border(box, Tokens.BorderHair);

			Draw.EllipsisLabel(new Rect(box.xMax + G, box.y, Mathf.Max(0f, tube.xMax - box.xMax - G), tube.height),
				caption, GameFont.Tiny, testW < NarrowFloor ? Tokens.Dead : Tokens.Dim, TextAnchor.MiddleLeft);

			Rect inner = new Rect(box.x, box.y + Tokens.SpaceXxs, cw, Mathf.Max(0f, box.height - 2f * Tokens.SpaceXxs));
			if (kind == Probe.Icon)
			{
				if (ButtonAtoms.IconButton(inner, label ?? "⟳", true, ButtonTone.Default)) footerHits++;
			}
			else if (ButtonAtoms.PrimaryButton(inner, label, true, ButtonTone.Default)) footerHits++;
		}

		/// <summary>D4：底栏带的垂直堆叠降级（每行全宽钮）。</summary>
		private void FooterStack(Rect area)
		{
			var box = new Rect(area.x, area.y, Mathf.Min(Tokens.SidebarWidth, area.width), area.height);
			Draw.Box(box, Tokens.Window);
			Draw.Border(box, Tokens.BorderHair);

			float y = box.y;
			for (int i = 0; i < FooterLabels5.Length; i++)
			{
				var r = new Rect(box.x, y, box.width, FrameApi.FooterButtonHeight);
				if (ButtonAtoms.PrimaryButton(r, FooterLabels5[i], true,
					i == FooterLabels5.Length - 1 ? ButtonTone.Accent : ButtonTone.Default)) footerHits++;
				y += FrameApi.FooterButtonHeight + Tokens.SpaceXxs;
			}
		}

		// ── ⑧ 禁用态量化差（§5.4）──────────────────────────────────────

		private void Draw7(Rect outer, float w)
		{
			var c = Layout.Column(Atoms.SectionFrame(outer,
				"⑧ 禁用态量化差 —— 禁用不得只是「看起来能点」（§5.4 表，6 处差异）"), G);
			try
			{
				Draw.EllipsisLabel(c.Take(NoteH),
					"正常 ｜ 禁用（Default）｜ 正常 ｜ 禁用（Danger）—— 注意「危险钮禁用后不得再显示红边」（Disabled 优先于一切且剥除 tone）",
					GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

				int cols = Cols(w, DisabledCells, BtnW);
				var g = Layout.Grid(c.Take(GridH(DisabledCells, cols, CtrlH)), cols, CtrlH, G);
				ButtonAtoms.PrimaryButton(g.Next(), LabelPrimary, true, ButtonTone.Default);
				ButtonAtoms.PrimaryButton(g.Next(), LabelDisabled, false, ButtonTone.Default);
				ButtonAtoms.PrimaryButton(g.Next(), LabelPrimary, true, ButtonTone.Danger);
				ButtonAtoms.PrimaryButton(g.Next(), LabelDisabled, false, ButtonTone.Danger);   // tone 必须被剥除

				// §5.8 键「RUIF.Comp.Button.Disabled」的**真实消费点**：禁用态**悬停提示** ——
				// 不是 label == null 回退（ButtonAtoms 对 null 标签只是 label ?? string.Empty，画空串）。
				// 组件内不内置英文默认串 ⇒ 提示文案必须走 Keyed 管道；Tr 为 internal，示例 mod
				// **不能**直接 Tr.Get，只能在实机上悬停观察（本环境无法跑游戏 ⇒ 该项标 NOT RUN）。
				Draw.EllipsisLabel(c.Take(NoteH),
					"Keyed 探针：**悬停**下方禁用钮 ⇒ TooltipHandler 弹「RUIF.Comp.Button.Disabled」（label = null 只会画空串，不回退）",
					GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				ButtonAtoms.PrimaryButton(c.Take(CtrlH), LabelDisabled, false, ButtonTone.Default);

				Draw.EllipsisLabel(c.Take(NoteH),
					"① fill：PanelRaised(0.141,0.157,0.188) → Panel(0.114,0.125,0.149)　相对亮度 −19%",
					GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				Draw.EllipsisLabel(c.Take(NoteH),
					"② text/icon：Text(0.851,0.867,0.890) → Dim(0.431,0.463,0.510)　对比度 −46%　③ border：剥除 tone 回落 BorderSoft（语义色消失）",
					GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				Draw.EllipsisLabel(c.Take(NoteH),
					"④ hover 零反馈　⑤ 完全自绘、不置 GUI.enabled=false（不污染全局 / 不吞 TooltipHandler）　⑥ 悬停音效静音：ButtonInvisible(rect, doMouseoverSound: enabled)",
					GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
			}
			catch (Exception e) { Fault("⑧ 禁用态", e); FaultPlaceholder(outer); }
		}
	}
}
