using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
	/// <summary>
	/// 分类「工业与官方」· <b>9 张卡</b> —— 覆盖 <c>RUIF.Industrial</c>(8) + <c>RUIF.OfficialDataAtoms</c>(4)。
	/// RUIF 已有实现，本批只做落地与验收。
	///
	/// <para><b>一张卡覆盖多个方法</b>：`Industrial` 里有 4 个是**纯色/角度函数**
	/// （`ZoneColor` / `LedColor` / `StateColor` / `GaugeNeedleDeg`），单独各占一张卡是浪费 ——
	/// 故合成 <c>#93 语义色阶总览</c>，一屏看全三套色阶 + 指针角度读数。</para>
	///
	/// <para>几何逐条取自反编译：<c>Gauge</c> 弧半径 <c>min(w×0.42, h−20)</c>；<c>LedMatrix</c>
	/// 仅 <c>status ≥ 3</c> 描边；<c>BottleneckRibbon</c> 有标签时下预留 18、段宽 ≤ 24 不画标签；
	/// <c>MachineStateTimeline</c> 行高 <c>max(8, h/行数−4)</c>、标签占左 <c>min(72, w×0.3)</c>。</para>
	/// </summary>
	internal sealed partial class BenchAtoms42
	{
		/// <summary>工业与官方族 · 9 张卡。注册于 <c>Build()</c>，页签「工业与官方」。</summary>
		private void BuildIndustrial()
		{
			Family f = new Family
			{
				Title = "工业与官方 · Industrial / OfficialDataAtoms",
				Sub = "9 卡覆盖 12 个方法 · Gauge / LedMatrix / BottleneckRibbon / MachineStateTimeline / 语义色阶 / 财富·记录·历史·传说"
			};

			// ────────────────────────────────────────────── 89 · Gauge
			f.Add(new Atom
			{
				No = "89", Name = "Gauge 仪表盘",
				Api = "Industrial.Gauge(Rect, float v, float lo, float hi, string label, float warn01 = 0.6f, float dead01 = 0.85f) → void",
				Desc = "半圆仪表：弧 + 指针 + 中轴 + 大字读数；配色走 ZoneColor 语义色阶（警告 / 危险阈值可配）。",
				Note = "几何：弧半径 = **min(w×0.42, h−20)** ⇒ **需要近似正方形的较高区域**；label 画顶部 14 高；读数在弧上方 y−24、FontValue(Medium) 20 高｜指针随帧摆动，越过 0.85 变危险色",
				BodyH = 132f, MinW = 130f, MinH = 132f,
				Paint = r =>
				{
					if (r.width < 90f || r.height < 110f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					float side = Mathf.Min(r.width, r.height - 16f);
					Rect g = new Rect(r.x, r.y + 16f, side, r.height - 16f);
					// 值改为「点击切换」而非逐帧摆动：Gauge 现在走**贴图烘焙**，
					// 逐帧变化会让缓存每帧失效（每次重算 ~1.5 万像素）；静态值则 1 次烘焙、长期命中。
					Industrial.Gauge(g, gaugePct * 10f, 0f, 100f, "冷却回路负荷");
					if (Widgets.ButtonInvisible(g)) gaugePct = (gaugePct + 3) % 11;
					HoverFrame(g);
					Draw.EllipsisLabel(new Rect(r.x, r.yMax - LineTiny, r.width, LineTiny),
						"值 = " + (gaugePct * 10) + "%（点图切换）· warn01 = 0.6 · dead01 = 0.85", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 90 · LedMatrix
			f.Add(new Atom
			{
				No = "90", Name = "LedMatrix 状态灯阵",
				Api = "Industrial.LedMatrix(Rect, int[] statuses, int cols) → void",
				Desc = "状态灯阵：每格一色，故障态额外描边。机群 / 电网 / 设备墙的紧凑总览。",
				Note = "几何：rows = ceil(n / cols)，**每格由 status 直接决定**；仅 **status ≥ 3**（Fault / Refuel / Offline）加 Tokens.Text 描边 ⇒ 故障一眼可见｜12 格 6 列 = 2 行",
				BodyH = 64f, MinW = 170f, MinH = 64f,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect m = col.Take(44f);
					if (m.width < 100f || m.height < 30f) { Draw.EllipsisLabel(m, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Industrial.LedMatrix(m, LedStatuses, 6);
					HoverFrame(m);
					Draw.EllipsisLabel(col.Take(LineTiny), "12 格 · 6 列｜状态 " + string.Join(" ", LedStatuses) , GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 91 · BottleneckRibbon
			f.Add(new Atom
			{
				No = "91", Name = "BottleneckRibbon 瓶颈缎带",
				Api = "Industrial.BottleneckRibbon(Rect, string[] stageLabels, float[] stageTimes, Color? color = null) → void",
				Desc = "工序耗时占比条：每段宽 = 耗时占比，**最长段（瓶颈）自动加红描边**。",
				Note = "几何：带宽 = **min(22, h − 18)**（有标签时下方预留 **18** 给说明行）；段宽 = w × 占比 − 2；**段宽 ≤ 24 时不画段标签**（防叠字）",
				BodyH = 66f, MinW = 240f, MinH = 66f,
				Paint = r =>
				{
					if (r.width < 160f || r.height < 44f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Industrial.BottleneckRibbon(r, StageLabels, StageTimes, null);
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 92 · MachineStateTimeline
			f.Add(new Atom
			{
				No = "92", Name = "MachineStateTimeline 机态时间轴",
				Api = "Industrial.MachineStateTimeline(Rect, string[] labels, MachineState[][] rows, float[][] durations) → void",
				Desc = "多行机态时间轴：每行一台设备，色块按时长占比排布，色走 StateColor 语义色阶。",
				Note = "几何：行高 = **max(8, h / 行数 − 4)**；标签占左 **min(72, w×0.3)**；段宽 = max(1, 剩余宽 × (时长/总时长) − 1)；**Fault 段加 Tokens.Text 描边**｜4 台设备",
				BodyH = 124f, MinW = 250f, MinH = 124f,
				Paint = r =>
				{
					if (r.width < 180f || r.height < 80f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Industrial.MachineStateTimeline(r, MachineLabels, MachineRows, MachineDurations);
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 93 · 语义色阶总览（4 个纯函数）
			f.Add(new Atom
			{
				No = "93", Name = "语义色阶总览",
				Api = "ZoneColor(int) · LedColor(int) · StateColor(MachineState) · GaugeNeedleDeg(float)",
				Desc = "Industrial 的 4 个**纯函数**一次看全：三套语义色阶 + 指针角度。四个方法单独各占一卡是浪费，合成一卡。",
				Note = "ZoneColor 用于 Gauge（按负荷占比分档）、LedColor 用于 LedMatrix（按状态码）、StateColor 用于时间轴（按机态）、GaugeNeedleDeg 把 0–1 映射成角度｜色块直接调函数取色，非硬编码",
				BodyH = 100f, MinW = 210f, MinH = 100f,
				Paint = r =>
				{
					var col = Layout.Column(r, 3f);
					if (r.width < 150f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					ColorRow(col.Take(22f), "ZoneColor", 4, i => Industrial.ZoneColor(i));
					ColorRow(col.Take(22f), "LedColor", 5, i => Industrial.LedColor(i));
					ColorRow(col.Take(22f), "StateColor", 5, i => Industrial.StateColor((MachineState)i));
					Draw.EllipsisLabel(col.Take(LineTiny),
						"GaugeNeedleDeg: 0→" + Mathf.RoundToInt(Industrial.GaugeNeedleDeg(0f)) + "°  0.5→" + Mathf.RoundToInt(Industrial.GaugeNeedleDeg(0.5f)) + "°  1→" + Mathf.RoundToInt(Industrial.GaugeNeedleDeg(1f)) + "°",
						GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 94 · WealthBreakdown
			f.Add(new Atom
			{
				No = "94", Name = "WealthBreakdown 财富构成",
				Api = "OfficialDataAtoms.WealthBreakdown(Rect, string[] labels, float[] values, Color[] tones) → void",
				Desc = "财富构成条：各分项按占比铺满，色彩由调用方给（tones）。",
				Note = "数据全由调用方提供（组件零数据）—— labels / values / tones 三个数组等长｜5 个分项，占比自动归一",
				BodyH = 74f, MinW = 230f, MinH = 74f,
				Paint = r =>
				{
					if (r.width < 150f || r.height < 44f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					OfficialDataAtoms.WealthBreakdown(r, WealthLabels, WealthValues, WealthTones);
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 95 · RecordsTable
			f.Add(new Atom
			{
				No = "95", Name = "RecordsTable 记录表",
				Api = "OfficialDataAtoms.RecordsTable(Rect, string[] labels, string[] values) → void",
				Desc = "两列表格：左标签右数值（原版 RecordsTracker 的紧凑呈现）。",
				Note = "labels / values 等长即可，行高由组件按行数均分｜6 行样例（殖民者统计口径）",
				BodyH = 130f, MinW = 190f, MinH = 130f,
				Paint = r =>
				{
					if (r.width < 140f || r.height < 90f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					OfficialDataAtoms.RecordsTable(r, RecLabels, RecValues);
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 96 · HistoryFeed
			f.Add(new Atom
			{
				No = "96", Name = "HistoryFeed 历史事件流",
				Api = "OfficialDataAtoms.HistoryFeed(Rect, string[] dates, string[] texts, string[] kinds) → void",
				Desc = "按时间倒序的事件流：日期 + 正文 + 类别角标。",
				Note = "三个数组等长（dates / texts / kinds）；类别角标用于区分事件类型（袭击 / 贸易 / 天气…）｜5 条样例",
				BodyH = 138f, MinW = 220f, MinH = 138f,
				Paint = r =>
				{
					if (r.width < 150f || r.height < 100f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					OfficialDataAtoms.HistoryFeed(r, HistDates, HistTexts, HistKinds);
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 97 · TaleCard
			f.Add(new Atom
			{
				No = "97", Name = "TaleCard 传说卡",
				Api = "OfficialDataAtoms.TaleCard(Rect, string teller, string text, int times) → void",
				Desc = "传说卡：讲述者 + 正文 + 传颂次数。",
				Note = "times 为传颂次数（原版 Tale 口径）｜文本全由调用方给，组件零数据",
				BodyH = 96f, MinW = 220f, MinH = 96f,
				Paint = r =>
				{
					if (r.width < 150f || r.height < 70f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					OfficialDataAtoms.TaleCard(r, "蓝 雪牛", "在第三次机械袭击中独自守住东墙，直到援军抵达。", 7);
					HoverFrame(r);
				}
			});

			families.Add(f);
		}

		/// <summary>画一行「名称 + n 个色块」—— 用来把多个纯色函数合成一卡。</summary>
		private static void ColorRow(Rect r, string name, int n, Func<int, Color> pick)
		{
			var row = Layout.Row(r, 4f);
			Draw.EllipsisLabel(row.Take(74f), name, GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
			for (int i = 0; i < n; i++)
			{
				Rect c = row.Take(Mathf.Min(28f, row.Rest.width));
				if (c.width < 6f) break;
				Draw.Box(c, pick(i));
				Draw.Border(c, Tokens.BorderSoft);
			}
		}

		// ── 工业与官方族交互状态（归调用方）────────────────────────────────
		/// <summary>89 Gauge 的当前值档（0..10 ⇒ 0%–100%，点图切换）—— 静态值让贴图缓存长期命中。</summary>
		private int gaugePct = 8;

		// ── 工业与官方族样例数据（标注为样例；非运行时数据）──────────────────
		/// <summary>灯阵状态码示例（12 格；≥3 为故障/补给/离线，会被描边）。</summary>
		private static readonly int[] LedStatuses = { 0, 0, 0, 3, 0, 0, 0, 0, 3, 0, 0, 0 };
		/// <summary>瓶颈缎带的工序名（样例）。</summary>
		private static readonly string[] StageLabels = { "取材", "粗加工", "精加工", "装配", "质检" };
		/// <summary>瓶颈缎带的耗时（样例；第四条最长为瓶颈）。</summary>
		private static readonly float[] StageTimes = { 2f, 4f, 6f, 11f, 3f };
		/// <summary>时间轴行标签（4 台设备）。</summary>
		private static readonly string[] MachineLabels = { "熔炉 A", "熔炉 B", "锻造台", "车床" };
		/// <summary>时间轴机态（4 行 × 4 段）。</summary>
		private static readonly MachineState[][] MachineRows =
		{
			new[] { MachineState.Run, MachineState.Idle, MachineState.Refuel, MachineState.Run },
			new[] { MachineState.Run, MachineState.Run, MachineState.Fault, MachineState.Offline },
			new[] { MachineState.Idle, MachineState.Run, MachineState.Run, MachineState.Idle },
			new[] { MachineState.Refuel, MachineState.Run, MachineState.Idle, MachineState.Fault }
		};
		/// <summary>时间轴各段时长（与 MachineRows 同形）。</summary>
		private static readonly float[][] MachineDurations =
		{
			new[] { 5f, 2f, 1f, 6f },
			new[] { 4f, 4f, 3f, 3f },
			new[] { 2f, 7f, 4f, 1f },
			new[] { 1f, 6f, 2f, 5f }
		};

		/// <summary>财富构成标签（样例）。</summary>
		private static readonly string[] WealthLabels = { "建筑", "物品", "家具", "动物", "地块" };
		/// <summary>财富构成数值（样例）。</summary>
		private static readonly float[] WealthValues = { 48f, 26f, 14f, 8f, 4f };
		/// <summary>财富构成色（样例：走语义色阶，非硬编码单色）。</summary>
		private static readonly Color[] WealthTones = { Tokens.Accent, Tokens.Info, Tokens.Alive, Tokens.Warn, Tokens.Muted };

		/// <summary>记录表标签（样例）。</summary>
		private static readonly string[] RecLabels = { "殖民者数", "总击杀", "最远射程", "饮食消耗", "建造数", "研究完成" };
		/// <summary>记录表数值（样例）。</summary>
		private static readonly string[] RecValues = { "12", "497", "86.4 m", "2 431", "1 208", "43" };

		/// <summary>历史事件日期（样例）。</summary>
		private static readonly string[] HistDates = { "第 12 象 · 3 日", "第 12 象 · 9 日", "第 13 象 · 1 日", "第 13 象 · 6 日", "第 13 象 · 14 日" };
		/// <summary>历史事件正文（样例）。</summary>
		private static readonly string[] HistTexts =
		{
			"一支贸易商队抵达，出售了零部件与药品。",
			"机械袭击：12 台机械体从东侧进攻，被全部击退。",
			"寒流持续三天，作物大面积冻伤。",
			"新殖民者「赫 松鼠」加入，技能为建造 12。",
			"驯服了一只大地懒，编入搬运队伍。"
		};
		/// <summary>历史事件类别角标（样例）。</summary>
		private static readonly string[] HistKinds = { "贸易", "袭击", "天气", "加入", "驯服" };
	}
}
