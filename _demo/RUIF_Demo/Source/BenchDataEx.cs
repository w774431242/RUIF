using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
	/// <summary>
	/// 分类「数据」· <b>DataExAtoms 10 件</b>（98–107）—— 第三方最高频的一批（电网 / 天气 / 库存 / 房间 / 任务 / 势力 / 生物 / 档案…）。
	///
	/// <para><b>这一族的共同形态：组件零数据，全部由调用方喂数组/标量</b>。故落地只需给「已标注的样例数据」，
	/// <b>不需要任何运行时数据源</b>（这是《任务单》§4 纠正后的口径）。</para>
	///
	/// <para>几何逐条取自反编译：`ProfilePreset` 右侧两按钮**固定占 128 宽**；`CompareBars` 标签占左 **76**、
	/// **赢的一方用 Accent**；`StorageBreakdown` **图例行在 y+18**（总高需 ≥ 40）；`PowerGrid` 净功率
	/// **≥0 走 Alive、否则 Warn**；`WeatherStrip` 每格 `w/项数 − 3`、底 22% 色 + 居中标签。</para>
	///
	/// <para>未落地：<c>NodeGraph</c>（列/行/边/状态 + ref selected，需大画布）与 <c>DockHost</c>（返回 int，停靠系统）——
	/// 两者结构复杂、需独立大区域，**登记为下一批**。</para>
	/// </summary>
	internal sealed partial class BenchAtoms42
	{
		/// <summary>数据扩展族 · 10 件。注册于 <c>Build()</c>，页签「数据」。</summary>
		private void BuildDataEx()
		{
			Family f = new Family
			{
				Title = "数据 · DataExAtoms",
				Sub = "10 件 · PowerGrid / WeatherStrip / StorageBreakdown / RoomStats / CompareBars / BuffRow / QuestCard / FactionCard / CreatureCard / ProfilePreset"
			};

			// ────────────────────────────────────────────── 98 · PowerGrid
			f.Add(new Atom
			{
				No = "98", Name = "PowerGrid 电力概览",
				Api = "DataExAtoms.PowerGrid(Rect, float produced, float consumed, float stored, float capacity) → void",
				Desc = "电力概览：净功率行 + 储能条。**净功率 ≥0 走 Alive 色、否则 Warn** ⇒ 缺电一眼可见。",
				Note = "几何：Column gap 3；净功率行 16 高；储能条 12 高，填充按 stored/capacity（capacity ≤0 时按 0 处理）｜点一下切换「盈余 / 缺电」两组样例值",
				BodyH = 66f, MinW = 210f, MinH = 66f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect box = col.Take(34f);
					if (box.width < 140f) { Draw.EllipsisLabel(box, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					// ⚠ A@R-05 修：原 produced=1820 < consumed=2480 ⇒ 净 −660 与「盈余 +1 820 W」文案矛盾（数据自洽），改 4300（4300−2480=+1820）
								DataExAtoms.PowerGrid(box, pgShort ? 4300f : 940f, 2480f, pgShort ? 6400f : 900f, 8000f);
					if (Widgets.ButtonInvisible(box)) pgShort = !pgShort;
					HoverFrame(box);
					Draw.EllipsisLabel(col.Take(LineTiny), pgShort ? "盈余 +1 820 W · 储能 80%" : "缺电 −1 540 W · 储能 11%", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 99 · WeatherStrip
			f.Add(new Atom
			{
				No = "99", Name = "WeatherStrip 天气条带",
				Api = "DataExAtoms.WeatherStrip(Rect, string[] labels, int[] kinds) → void",
				Desc = "天气条带：等分格，每格底色 22% + 边框 + 居中标签；`kinds` 决定格色。",
				Note = "几何：Row gap 3；**每格宽 = rect.width / 项数 − 3**；底色 WithAlpha(色, 0.22)、边框同色｜7 天样例（kinds 覆盖晴/雨/雪/风暴）",
				BodyH = 50f, MinW = 220f, MinH = 50f,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect s = col.Take(26f);
					if (s.width < 140f) { Draw.EllipsisLabel(s, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					DataExAtoms.WeatherStrip(s, WeatherLabels, WeatherKinds);
					HoverFrame(s);
					Draw.EllipsisLabel(col.Take(LineTiny), "kinds 决定格子语义色（0 晴 / 1 雨 / 2 雪 / 3 风暴）", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 100 · StorageBreakdown
			f.Add(new Atom
			{
				No = "100", Name = "StorageBreakdown 库存构成",
				Api = "DataExAtoms.StorageBreakdown(Rect, string[] labels, float[] values, Color[] tones) → void",
				Desc = "库存构成：顶部堆叠条 + 下方图例（色块 + 名称 + 百分比）。",
				Note = "几何：**图例行固定在 y+18、高 14** ⇒ **总高需 ≥ 40**；每项 = 8×8 色块 + 「名称 百分比」（走 x+12）｜4 个分项，tones 由调用方给",
				BodyH = 60f, MinW = 240f, MinH = 60f,
				Paint = r =>
				{
					if (r.width < 160f || r.height < 40f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					DataExAtoms.StorageBreakdown(r, StoreLabels, StoreValues, StoreTones);
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 101 · RoomStats
			f.Add(new Atom
			{
				No = "101", Name = "RoomStats 房间属性条",
				Api = "DataExAtoms.RoomStats(Rect, string[] names, float[] metrics) → void",
				Desc = "房间属性条组：每行「标签 + 条」，条值走 metrics（0–1）。",
				Note = "几何：Column gap 2；每行 = h / 项数 − 2；**标签占左 60**；条填充 Clamp01(metrics[i])｜4 项（美观 / 清洁 / 空间 / 温度）",
				BodyH = 102f, MinW = 190f, MinH = 102f,
				Paint = r =>
				{
					if (r.width < 140f || r.height < 70f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					DataExAtoms.RoomStats(r, RoomNames, RoomMetrics);
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 102 · CompareBars
			f.Add(new Atom
			{
				No = "102", Name = "CompareBars 双列对比条",
				Api = "DataExAtoms.CompareBars(Rect, string leftLabel, string rightLabel, float a, float b, float max) → void",
				Desc = "两行对比条（本季 vs 上季）：**数值大的一方自动用 Accent**，另一方用 Info。",
				Note = "几何：Column gap 4；每行 = h/2；**标签占左 76**；条右端叠数值（MiddleRight）｜a=1 240 / b=860，max=2 000 ⇒ 本季应高亮",
				BodyH = 68f, MinW = 230f, MinH = 68f,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect box = col.Take(44f);
					if (box.width < 150f) { Draw.EllipsisLabel(box, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					DataExAtoms.CompareBars(box, "本季产出", "上季产出", 1240f, 860f, 2000f);
					HoverFrame(box);
					Draw.EllipsisLabel(col.Take(LineTiny), "赢的一方（本季）自动 Accent —— 颜色由组件判，不用调用方给", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 103 · BuffRow
			f.Add(new Atom
			{
				No = "103", Name = "BuffRow 增减益行",
				Api = "DataExAtoms.BuffRow(Rect, string label, float value) → void",
				Desc = "单行增减益：标签左对齐、数值右对齐（定宽 64）。",
				Note = "几何：`Row(rect, 8)` 后取 Rest 画标签；**数值固定在 xMax−64、宽 64 右对齐** ⇒ 宽需 ≥ ~140｜value 正负号由组件处理",
				// B-R12 修：Take 链 22+1+22+1+18=64，原 BodyH/MinH=50 溢 14（末行说明被裁 4px）→ 66
				BodyH = 66f, MinW = 170f, MinH = 66f,
				Paint = r =>
				{
					var col = Layout.Column(r, 1f);
					Rect a = col.Take(22f);
					if (a.width < 120f) { Draw.EllipsisLabel(a, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					DataExAtoms.BuffRow(a, "心情 · 舒适卧室", 8f);
					DataExAtoms.BuffRow(col.Take(22f), "心情 · 饥饿", -6f);
					Draw.EllipsisLabel(col.Take(LineTiny), "两条增减益并排；数值右对齐到同一列", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 104 · QuestCard
			f.Add(new Atom
			{
				No = "104", Name = "QuestCard 任务卡",
				Api = "DataExAtoms.QuestCard(Rect, string title, int done, int total, string[] objectives) → void",
				Desc = "任务卡：标题 + 进度条 + 目标清单（**自画底与边框**）。",
				Note = "几何：自画 Card 底 + BorderSoft 边；Inset(8,6) + gap 3；标题 **20**(FontBody)；进度条 **10**（比例 = done/total）；每条目标 **16** 行、前缀「· 」｜3 个目标、完成 2/5",
				BodyH = 104f, MinW = 210f, MinH = 104f,
				Paint = r =>
				{
					if (r.width < 150f || r.height < 80f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					DataExAtoms.QuestCard(r, "断供的商队", 2, 5, QuestObjectives);
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 105 · FactionCard
			f.Add(new Atom
			{
				No = "105", Name = "FactionCard 势力卡",
				Api = "DataExAtoms.FactionCard(Rect, string name, float goodwill) → void",
				Desc = "势力卡：名称 + 好感条（`goodwill` 取绝对值映射条长）。",
				Note = "几何：自画 Card 底 + 边框；Inset(8,6) + gap 3；标题 **20**(FontBody)；好感条占**剩余整宽**、填充 Clamp01(|goodwill|)｜点一下切「友善 / 敌对」看条色是否随符号变",
				BodyH = 78f, MinW = 200f, MinH = 78f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect box = col.Take(54f);
					if (box.width < 150f) { Draw.EllipsisLabel(box, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					DataExAtoms.FactionCard(box, fcAlly ? "流浪者商队" : "敌对部落", fcAlly ? 68f : -55f);
					if (Widgets.ButtonInvisible(box)) fcAlly = !fcAlly;
					HoverFrame(box);
					Draw.EllipsisLabel(col.Take(LineTiny), "goodwill = " + (fcAlly ? "+68（友善）" : "−55（敌对）") + "｜点卡切换", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 106 · CreatureCard
			f.Add(new Atom
			{
				No = "106", Name = "CreatureCard 生物卡",
				Api = "DataExAtoms.CreatureCard(Rect, string name, int training, bool bonded) → void",
				Desc = "生物卡：名称（含羁绊标记）+ 训练度条（满 5 级）。",
				Note = "几何：自画 Card 底 + 边框；标题 **20**(FontBody)；训练条 **10**、填充 = training / 5｜training = 4、bonded = true（羁绊标记由组件加在标题里）",
				BodyH = 76f, MinW = 200f, MinH = 76f,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect box = col.Take(52f);
					if (box.width < 150f) { Draw.EllipsisLabel(box, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					DataExAtoms.CreatureCard(box, "蓝 雪牛", 4, true);
					HoverFrame(box);
					Draw.EllipsisLabel(col.Take(LineTiny), "training 4 / 5 · bonded = true", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 107 · ProfilePreset
			f.Add(new Atom
			{
				No = "107", Name = "ProfilePreset 配置档案",
				Api = "DataExAtoms.ProfilePreset(Rect, string[] names, ref int index, out bool save) → bool",
				Desc = "配置档案选择行：标签 + 当前档名 + 右侧「切换 / 保存」两键；`out save` 区分「换了档」与「存了档」。",
				Note = "几何：标签占左 **56**；**右侧两键固定占 128 宽**（xMax−132 起，62 + 66）⇒ 总宽需 ≥ ~200、高 ≥ 26｜点「切换」循环档名，点「保存」看计数",
				BodyH = 52f, MinW = 220f, MinH = 52f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect box = col.Take(26f);
					if (box.width < 190f) { Draw.EllipsisLabel(box, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					bool save;
					if (DataExAtoms.ProfilePreset(box, PresetNames, ref psIndex, out save)) psSaved++;
					HoverFrame(box);
					Draw.EllipsisLabel(col.Take(LineTiny), "当前档「" + PresetNames[psIndex] + "」· 保存 " + psSaved + " 次（out save 区分两种操作）", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			families.Add(f);
		}

		// ── DataEx 样例数据（标注为样例；非运行时数据）────────────────────
		private static readonly string[] WeatherLabels = { "一", "二", "三", "四", "五", "六", "日" };
		private static readonly int[] WeatherKinds = { 0, 0, 1, 1, 3, 2, 0 };
		private static readonly string[] StoreLabels = { "钢铁", "零部件", "食物", "药品" };
		private static readonly float[] StoreValues = { 0.42f, 0.27f, 0.21f, 0.10f };
		private static readonly Color[] StoreTones = { Tokens.Info, Tokens.Accent, Tokens.Alive, Tokens.Warn };
		private static readonly string[] RoomNames = { "美观", "清洁", "空间", "温度" };
		private static readonly float[] RoomMetrics = { 0.72f, 0.88f, 0.54f, 0.61f };
		private static readonly string[] QuestObjectives = { "交付 120 钢铁", "交付 40 药品", "保持 3 天无袭击" };
		private static readonly string[] PresetNames = { "默认", "极简", "表演", "挑战" };

		// ── DataEx 交互状态（归调用方）────────────────────────────────────
		/// <summary>98 是否展示「盈余」组。</summary>
		private bool pgShort = true;
		/// <summary>105 是否友善。</summary>
		private bool fcAlly = true;
		/// <summary>107 当前档序号（由 ref 传入）。</summary>
		private int psIndex;
		/// <summary>107 保存次数（由 out save 驱动）。</summary>
		private int psSaved;
	}
}
