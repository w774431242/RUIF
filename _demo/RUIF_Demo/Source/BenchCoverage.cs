using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
	/// <summary>
	/// 覆盖补全面（2026-10-05 审计新增）—— 收纳「框架有、其余验证面从未调用」的原子。
	///
	/// <para><b>为什么要这一面</b>：审计口径 = 框架侧 <c>*Atoms / Shapes / Charts / Industrial /
	/// WindowFrame / CommandPalette / PawnTableHost</c> 的 <b>173 个公开绘制方法</b>，逐一在
	/// <c>_demo/RUIF_Demo/Source/*.cs</c> 做「方法名级」匹配（可捕捉 <c>host.Reset()</c> 这类实例调用）。
	/// 结论：<b>16 件从未被任何面调用</b>（另有 2 件同族同因）。没有这一面，它们只能靠"读过源码"
	/// 断言可用，而不是"在真机里画出来过"。</para>
	///
	/// <para><b>覆盖面（18 件 · 签名逐个取自 <c>Source/Components/*.cs</c>）</b>：
	/// ① <see cref="OfficialDataAtomsB"/>：LetterCard / BillList / QuestBoard / TitleCard / PsycastTree；
	/// ② <see cref="InputAtoms.StateSample"/> · <see cref="InputExAtoms.DragListRow"/> ·
	///   <see cref="InputExAtoms.GripHit"/> · <see cref="InputPlusAtoms.PaintStrokeGrid"/>；
	/// ③ <see cref="ButtonAtoms.FocusRing"/> · <see cref="FeedbackAtomsB.NotificationEntry"/> ·
	///   <see cref="WindowFrame.DefaultCloseLabel"/>；
	/// ④ <see cref="TableCandAtoms.PawnTableFrame"/> · <see cref="PawnTableAtoms.CurrentMapColonists"/> ·
	///   <see cref="PawnTableAtoms.CycleSort"/> · <see cref="PawnTableHost.Ensure"/>；
	/// ⑤ <see cref="TexAtoms.Canvas"/> · <see cref="TexAtoms.FadeLeft"/>。</para>
	///
	/// <para><b>交互四问逐件齐备</b>：谁触发（组件内部 <c>ButtonInvisible</c> 或本面）／返回什么
	/// （bool 或新状态）／状态在哪（全部本面字段，组件零状态）／无鼠标时（FocusRing 显式环、
	/// CycleSort 三态循环、StateSample 六态对照）。</para>
	///
	/// <para><b>段高来源</b>：每段 = 各件内部公式反推的最小高之和 + 间隙 + <c>SectionFrame</c> 的 24
	/// （<c>Inset(outer, 8, 12)</c> 实测）。反推依据写在各 <c>Take()</c> 行内。</para>
	/// </summary>
	internal sealed class BenchCoverage : IBench
	{
		// ══════════════════════════════════════════════════════════════════
		// 段高（反推自各组件内部几何；SectionFrame 内容区 = 段高 − 24）
		// ══════════════════════════════════════════════════════════════════

		/// <summary>① 官方数据 B 族：52 + 66 + 72 + 84 + 72 + 间隙 32 = 378，+ 24。</summary>
		private const float H1 = 402f;

		/// <summary>② 输入补充：28 + 130 + 20 + 84 + 间隙 48 = 310 − 24。</summary>
		private const float H2 = 310f;

		/// <summary>③ 交互与反馈：36 + 90 + 22 + 间隙 24 = 172，+ 24。</summary>
		private const float H3 = 196f;

		/// <summary>④ 表格与桥接：180 + 24 + 26 + 24 + 间隙 36 = 290，+ 24。</summary>
		private const float H4 = 314f;

		/// <summary>⑤ 贴图补充：80 + 18 + 60 + 间隙 24 = 182，+ 24。</summary>
		private const float H5 = 206f;

		public bool Scrollable => true;

		public float Height(float width) => H1 + H2 + H3 + H4 + H5 + Tokens.SpaceMd * 4f;

		// ══════════════════════════════════════════════════════════════════
		// 样例数据（明确标注为样例；验证台不接真实游戏状态）
		// ══════════════════════════════════════════════════════════════════

		private static readonly string[] BillNames = { "突击步枪", "防弹背心", "医药包" };
		private static readonly float[] BillDone = { 7f, 2f, 12f };
		private static readonly float[] BillTarget = { 10f, 6f, 12f };
		private static readonly bool[] BillPaused = { false, true, false };

		private static readonly string[] QuestTitles = { "营救失踪的商队", "机械集群清除", "古代遗迹发掘" };
		private static readonly float[] QuestProgress = { 0.35f, 0.8f, 1f };
		private static readonly float[] QuestDays = { 4.5f, 1.2f, -1f };

		private static readonly string[] PermitNames = { "呼叫运输舱", "皇家护卫", "轨道打击" };
		private static readonly bool[] PermitOk = { true, false, false };

		private static readonly string[] PsyNames = { "心灵冲击", "心灵抚慰", "神经热" };
		private static readonly int[] PsyLevels = { 1, 2, 3 };
		private static readonly bool[] PsyUnlocked = { true, true, false };

		/// <summary>六态全集（与 <c>InputState</c> 枚举一一对应，用于验收「状态齐全」）。</summary>
		private static readonly InputState[] States =
		{
			InputState.Default, InputState.Hover, InputState.Pressed,
			InputState.Selected, InputState.Disabled, InputState.Focused
		};

		private static readonly string[] DragLabels = { "床铺", "餐桌", "电池", "火把" };

		private static readonly string[] NoteTexts =
		{
			"研究「电力」已完成", "殖民者「露西」加入了殖民地", "仓库已满：钢铁存放达上限"
		};

		private static readonly string[] NoteTimes = { "刚刚", "3 分钟前", "1 小时前" };

		/// <summary>数据表头（4 列足以验冻结首列 / 均分 / 密度切换）。</summary>
		private static readonly string[] GridHeaders = { "名称", "心情", "健康", "技能" };

		private static readonly string[][] GridRows =
		{
			new[] { "露西", "82%", "100%", "种植 12" },
			new[] { "汉森", "54%", "88%", "建造 9" },
			new[] { "阿黛尔", "91%", "64%", "医疗 15" },
			new[] { "柯尔", "67%", "100%", "射击 7" }
		};

		/// <summary><see cref="PawnTableAtoms.CycleSort"/> 的候选列键（点击推进三态）。</summary>
		private static readonly string[] SortKeys = { "Name", "Mood", "Health" };

		/// <summary>画布像素（类型初始化期一次生成 ⇒ 绘制路径零分配）。</summary>
		private static readonly Color32[] CanvasPixels = MakeCanvas(64, 64);

		// ══════════════════════════════════════════════════════════════════
		// 状态（全部归本面持有 —— 组件零状态）
		// ══════════════════════════════════════════════════════════════════

		/// <summary>拖拽列表当前被把手按下的行下标（-1 = 无）。</summary>
		private int dragRow = -1;

		/// <summary><see cref="InputPlusAtoms.PaintStrokeGrid"/> 的格子值（0 = 空，1–4 = 四档语义色）。</summary>
		private readonly int[] paint = new int[4 * 8];

		/// <summary>焦点按钮是否已点亮（<see cref="ButtonAtoms.FocusRing"/> 需调用方显式驱动）。</summary>
		private bool focusOn;

		/// <summary>通知条目的未读态（点击后置已读）。</summary>
		private readonly bool[] unread = { true, true, false };

		/// <summary>数据表排序态（<see cref="TableCandAtoms.PawnTableFrame"/> 的 ref 参数）。</summary>
		private readonly SortState[] sorts = { SortState.None, SortState.None, SortState.None, SortState.None };
		private int sortCol;
		private RowDensity density = RowDensity.Normal;

		/// <summary><see cref="PawnTableAtoms.CycleSort"/> 的当前键与方向（调用方持有）。</summary>
		private string cycleKey;
		private bool cycleDesc;

		/// <summary>表宿主（<see cref="PawnTableHost.Ensure"/> 的持久状态）。</summary>
		private readonly PawnTableHost host = new PawnTableHost();

		/// <summary>造一块 64×64 示意画布：横渐变 + 纵渐变 + 棋盘格（验「真像素」而非纯色）。</summary>
		private static Color32[] MakeCanvas(int w, int h)
		{
			var px = new Color32[w * h];
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					byte r = (byte)(255f * x / (w - 1));
					byte g = (byte)(255f * y / (h - 1));
					byte b = (byte)(((x / 8 + y / 8) % 2 == 0) ? 210 : 70);
					px[y * w + x] = new Color32(r, g, b, 255);
				}
			}
			return px;
		}

		// ══════════════════════════════════════════════════════════════════
		// 绘制（段函数接收 Rect，游标由本方法推进 —— VStack 是值类型，不能跨方法传游标）
		// ══════════════════════════════════════════════════════════════════

		public void Render(Rect area)
		{
			var col = Layout.Column(area, Tokens.SpaceMd);
			SectionOfficial(col.Take(H1));
			SectionInput(col.Take(H2));
			SectionInteraction(col.Take(H3));
			SectionTable(col.Take(H4));
			SectionTexture(col.Take(H5));
		}

		/// <summary>① 官方数据 B 族 5 件（数据由调用方注入，组件不读游戏状态）。</summary>
		private void SectionOfficial(Rect outer)
		{
			Rect f = Atoms.SectionFrame(outer, "官方数据 B 族 · OfficialDataAtomsB（5 件 · 审计缺口）");
			var c = Layout.Column(f, Tokens.SpaceXs);

			// ⑤ 信件归档卡：内部 = Inset(10,5) + Take(18) + 3 + Rest(18) ⇒ 最小 49
			OfficialDataAtomsB.LetterCard(c.Take(52f), "袭击警报：机械集群",
				"北方 3 个集群已激活，预计 6 小时后接近。", "5500 年 赫象季 第 1 天", Tokens.Dead);

			// ⑥ 账单队列：按 rect.height/n 分行，行内 Tiny 标签需 18 ⇒ n=3 时 h ≥ 60
			OfficialDataAtomsB.BillList(c.Take(66f), BillNames, BillDone, BillTarget, BillPaused);

			// ⑦ 任务板：cardH = (h − (n−1)×4)/n，卡内 Tiny 需 18 ⇒ n=3 时 h ≥ 62
			OfficialDataAtomsB.QuestBoard(c.Take(72f), QuestTitles, QuestProgress, QuestDays);

			// ⑧ 头衔卡：Inset(8,5) + 18 + 14 + 16×2（2 条许可）⇒ 80
			OfficialDataAtomsB.TitleCard(c.Take(84f), "公爵", "5500 年 赫象季 第 1 天", PermitNames, PermitOk);

			// ⑨ 心能树：rowH = h/n，行内 Take(rowH−3) 需 18 ⇒ n=3 时 h ≥ 63
			OfficialDataAtomsB.PsycastTree(c.Take(72f), PsyNames, PsyLevels, PsyUnlocked);
		}

		/// <summary>② 输入补充 4 件。</summary>
		private void SectionInput(Rect outer)
		{
			Rect f = Atoms.SectionFrame(outer, "输入补充 · StateSample / DragListRow / GripHit / PaintStrokeGrid（4 件 · 审计缺口）");
			var c = Layout.Column(f, Tokens.SpaceSm);

			// 六态规范示意：一行 6 格（「示例：Selected」Tiny 实测约 76px ⇒ 96 格宽留足）
			var st = Layout.Row(c.Take(28f), Tokens.SpaceXs);
			for (int i = 0; i < States.Length; i++)
				InputAtoms.StateSample(st.Take(96f), "示例", States[i]);

			// 拖拽列表：DragListRow 只画，GripHit 负责命中（调用方据此起拖 —— 本面用 dragRow 模拟）
			var dl = Layout.Column(c.Take(130f), 6f);   // 4 × 28 + 3 × 6 = 130
			for (int i = 0; i < DragLabels.Length; i++)
			{
				Rect row = dl.Take(28f);
				InputExAtoms.DragListRow(row, DragLabels[i], dragRow == i, false);
				if (InputExAtoms.GripHit(row)) dragRow = (dragRow == i) ? -1 : i;
			}

			Draw.Label(c.Take(20f), "点左侧把手开始拖拽（GripHit 返回 true）· 点下方 4×8 格子改档位（0–4 四档语义色）",
				GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

			// 涂抹网格：cellH = 84/4 = 21 ≥ Tiny 18 + 2
			Rect pg = c.Take(84f);
			int hit;
			if (InputPlusAtoms.PaintStrokeGrid(pg, 4, 8, paint, out hit))
				paint[hit] = (paint[hit] + 1) % 5;
		}

		/// <summary>③ 交互与反馈 3 件。</summary>
		private void SectionInteraction(Rect outer)
		{
			Rect f = Atoms.SectionFrame(outer, "交互与反馈 · FocusRing / NotificationEntry / DefaultCloseLabel（3 件 · 审计缺口）");
			var c = Layout.Column(f, Tokens.SpaceSm);

			// 焦点环：按钮外侧必须预留 ≥ 2px（FocusRingOutset），否则外环被邻件压住
			var fr = Layout.Row(c.Take(36f), Tokens.SpaceLg);
			Rect btn = fr.Take(160f);
			if (ButtonAtoms.PrimaryButton(btn, "点击切换焦点环", true, ButtonTone.Accent)) focusOn = !focusOn;
			if (focusOn) ButtonAtoms.FocusRing(btn, ButtonTone.Accent);
			Draw.EllipsisLabel(fr.Rest, "ButtonAtoms.FocusRing —— 外扩 2px 且过 Px.Snap（UI 缩放非整数时不糊成 2px 灰边）",
				GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

			// 通知条目：3 × 26 + 2 × 6 = 90（内部 = Row(rect,6) + Tiny 18）
			var nc = Layout.Column(c.Take(90f), 6f);
			for (int i = 0; i < NoteTexts.Length; i++)
			{
				if (FeedbackAtomsB.NotificationEntry(nc.Take(26f), NoteTexts[i], NoteTimes[i], unread[i]))
					unread[i] = false;
			}

			// 关闭按钮文案 fallback：null ⇒ 取 Keyed「RUIF.Comp.Window.Close」
			var dr = Layout.Row(c.Take(22f), Tokens.SpaceXs);
			Draw.Label(dr.Take(200f), "WindowFrame.DefaultCloseLabel：", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
			Draw.EllipsisLabel(dr.Rest,
				"\"" + WindowFrame.DefaultCloseLabel("关闭") + "\"　/　\"" + WindowFrame.DefaultCloseLabel(null) + "\"（null 走 Keyed 兜底）",
				GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
		}

		/// <summary>④ 表格与桥接 4 件。</summary>
		private void SectionTable(Rect outer)
		{
			Rect f = Atoms.SectionFrame(outer, "表格与桥接 · TableCandAtoms / PawnTableAtoms / PawnTableHost（4 件 · 审计缺口）");
			var c = Layout.Column(f, Tokens.SpaceSm);

			// 数据表框架：内部 = DensitySwitch(26) + 表头(rowH) + 行(rowH × n) ⇒ 4 行给 180
			TableCandAtoms.PawnTableFrame(c.Take(180f), GridHeaders, GridRows, ref sortCol, sorts, ref density);

			// 当前地图殖民者：无地图时按契约返回空序列（标题界面即此分支）
			var mr = Layout.Row(c.Take(24f), Tokens.SpaceXs);
			int colonists = 0;
			foreach (Pawn p in PawnTableAtoms.CurrentMapColonists()) colonists++;
			Draw.EllipsisLabel(mr.Take(430f),
				"PawnTableAtoms.CurrentMapColonists → " + colonists + " 名自由殖民者"
				+ (Find.CurrentMap == null ? "（无当前地图 ⇒ 空序列，符合契约）" : ""),
				GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
			Draw.Label(mr.Rest,
				"数据表排序态：" + (sortCol >= 0 && sortCol < sorts.Length ? GridHeaders[sortCol] + " " + sorts[sortCol] : "—"),
				GameFont.Tiny, Tokens.Info, TextAnchor.MiddleRight);

			// CycleSort：三态循环 None → Asc → Desc → None
			var cr = Layout.Row(c.Take(26f), Tokens.SpaceXs);
			Draw.Label(cr.Take(190f), "PawnTableAtoms.CycleSort：", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
			for (int i = 0; i < SortKeys.Length; i++)
			{
				string k = SortKeys[i];
				if (Widgets.ButtonText(cr.Take(78f), (cycleKey == k ? "● " : "") + k))
				{
					bool desc;
					cycleKey = PawnTableAtoms.CycleSort(cycleKey, cycleDesc, k, out desc);
					cycleDesc = desc;
				}
			}
			Draw.EllipsisLabel(cr.Rest,
				"→ " + (cycleKey ?? "—") + (cycleKey == null ? "" : (cycleDesc ? " 降序" : " 升序")),
				GameFont.Tiny, Tokens.Accent, TextAnchor.MiddleLeft);

			// PawnTableHost.Ensure：按 Def 名建表（同 Def 不重建；失败时 LastError 给原因）
			var er = Layout.Row(c.Take(24f), Tokens.SpaceXs);
			var built = host.Ensure("Assign", PawnTableAtoms.CurrentMapColonists, 240f, 80f);
			Draw.EllipsisLabel(er.Take(560f),
				"PawnTableHost.Ensure(\"Assign\", …) → "
				+ (built == null ? "null（" + (host.LastError ?? "未知原因") + "）" : "PawnTable 已建 · TableDefName=" + host.TableDefName),
				GameFont.Tiny, built == null ? Tokens.Warn : Tokens.Alive, TextAnchor.MiddleLeft);
			Draw.Label(er.Rest, "MarkDirty / Reset 由「殖民者表」面覆盖，此处只补 Ensure 直调",
				GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleRight);
		}

		/// <summary>⑤ 贴图补充 2 件。</summary>
		private void SectionTexture(Rect outer)
		{
			Rect f = Atoms.SectionFrame(outer, "贴图补充 · TexAtoms.Canvas / FadeLeft（2 件 · 审计缺口）");
			var c = Layout.Column(f, Tokens.SpaceSm);

			// Canvas：自有像素数组 + 版本化缓存键（内容变了必须换 key，否则一直画旧图）
			Rect cv = c.Take(80f);
			long key = TexCache.KeyVersioned(0x71, 1, 64, 64);
			if (!TexAtoms.Canvas(cv, key, 64, 64, CanvasPixels, "RUIF.Demo.Coverage.Canvas"))
				Draw.Label(cv, "画布发布失败（贴图仓库不可用 · 降级态）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleCenter);

			Draw.Label(c.Take(18f),
				"TexAtoms.Canvas —— 64×64 自有像素（横渐变 + 纵渐变 + 棋盘格，验「真像素」而非纯色块）",
				GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

			// FadeLeft：左向渐隐（与 FadeRight 对称；用于「左侧还有内容」的溢出提示）
			Rect fl = c.Take(60f);
			Draw.Box(fl, Tokens.PanelRaised);
			Draw.Label(new Rect(fl.x + 6f, fl.y, Mathf.Max(0f, fl.width - 12f), fl.height),
				"TexAtoms.FadeLeft —— 文本向左渐隐到背景色（右边留白处可见渐隐带）",
				GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
			TexAtoms.FadeLeft(fl, Tokens.PanelRaised, 90f);
		}
	}
}
