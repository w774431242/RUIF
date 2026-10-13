using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
	/// <summary>
	/// 分类「图表」· <b>10 件</b> —— 全部来自 <c>RUIF.Charts</c>。
	///
	/// <para><b>本批只做「落地与验收」，不是从零实现</b>：这 10 件 RUIF 早已有实现（见《RUIF组件总目》CHT 节）。
	/// 这正是《宝典落地任务单》§4 纠正后的口径 —— 复合件多在框架里，落地的活是接线与验收。</para>
	///
	/// <para>几何约束**逐条取自反编译实现**（写在各件 <c>Note</c> 里），未凭签名猜：<br/>
	/// <c>BarChartV</c> 有 labels 时预留下方 13px；<c>BarChartH</c> 标签占左 <c>min(72, w*0.3)</c>；
	/// <c>LineChart</c> 的 grid 在右侧 34px 画刻度数字；<c>Donut</c>/<c>Pie</c> 半径 = <c>min(w,h)/2 − 1</c>（需正方形）；
	/// <c>PieChart</c> 就是 <c>DonutChart(r, values, 0f, centerLabel)</c> 的薄封装；
	/// <c>HeatmapGrid</c> 行列取自 <c>grid.GetLength</c>。</para>
	/// </summary>
	internal sealed partial class BenchAtoms42
	{
		/// <summary>图表族 · 10 件。注册于 <c>Build()</c>，页签「图表」。</summary>
		private void BuildCharts()
		{
			Family f = new Family
			{
				Title = "图表 · Charts",
				Sub = "10 件 · Sparkline / LineChart / AreaChart / BarChartV·H / StackedBarChartV / Donut / Pie / Scatter / HeatmapGrid"
			};

			// ────────────────────────────────────────────── 67 · Sparkline
			f.Add(new Atom
			{
				No = "67", Name = "Sparkline 迷你折线",
				Api = "Charts.Sparkline(Rect, float[] values, Color?, float thickness = 1.5f, bool dots = false) → void",
				Desc = "无轴无标签的迷你趋势线，适合塞进表格单元或卡片角标。",
				Note = "几何：点 = 3×3 方块（rect ± 1.5）、线宽取 thickness｜点图切 dots，演示最后一个参数",
				BodyH = 62f, MinW = 150f, MinH = 62f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(40f);
					if (plot.width < 60f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Charts.Sparkline(plot, ChartVals, Tokens.Accent, 1.5f, sparkDots);
					if (Widgets.ButtonInvisible(plot)) sparkDots = !sparkDots;
					HoverFrame(plot);
					Draw.EllipsisLabel(col.Take(LineTiny), "dots = " + sparkDots + " · 8 点样例序列｜点图切换", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 68 · LineChart
			f.Add(new Atom
			{
				No = "68", Name = "LineChart 折线图",
				Api = "Charts.LineChart(Rect, float[] values, Color?, bool dots = true, bool grid = true) → void",
				Desc = "带坐标网格与刻度数字的折线图（网格画在右侧 34px 区）。",
				Note = "几何：内部 PlotArea(r, labels:false)；grid 控制刻度线 + 右侧 34px 刻度数字｜点图切 grid，再点切 dots",
				BodyH = 92f, MinW = 190f, MinH = 92f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(70f);
					if (plot.width < 110f || plot.height < 40f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Charts.LineChart(plot, ChartVals, Tokens.Info, lineDots, lineGrid);
					if (Widgets.ButtonInvisible(plot)) { if (lineGrid) lineGrid = false; else if (lineDots) lineDots = false; else { lineGrid = true; lineDots = true; } }
					HoverFrame(plot);
					Draw.EllipsisLabel(col.Take(LineTiny), "grid = " + lineGrid + " · dots = " + lineDots + "｜点图循环切换", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 69 · AreaChart
			f.Add(new Atom
			{
				No = "69", Name = "AreaChart 面积图",
				Api = "Charts.AreaChart(Rect, float[] values, Color?) → void",
				Desc = "折线的面积填充版（18% 透明度），强调累计量而非点位。",
				Note = "几何：PlotArea(r, labels:false)；填充 = WithAlpha(color, 0.18)，柱状推进到 yMax",
				BodyH = 82f, MinW = 190f, MinH = 82f,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(60f);
					if (plot.width < 110f || plot.height < 36f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Charts.AreaChart(plot, ChartVals, Tokens.Alive);
					HoverFrame(plot);
					Draw.EllipsisLabel(col.Take(LineTiny), "与折线同一组数据 · 只用面积表达", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 70 · BarChartV
			f.Add(new Atom
			{
				No = "70", Name = "BarChartV 竖柱图",
				Api = "Charts.BarChartV(Rect, float[] values, string[] labels = null, Color? color = null) → void",
				Desc = "竖柱图，带 x 轴标签行（标签由组件自画）。",
				Note = "几何：有 labels 时 PlotArea 预留**下方 13px** 标签行（画在 yMax+1）｜点图切 labels 有无，看预留高度差别",
				BodyH = 92f, MinW = 190f, MinH = 92f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(70f);
					if (plot.width < 110f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Charts.BarChartV(plot, ChartValsShort, barLabels ? ChartLabelsShort : null, Tokens.Accent);
					if (Widgets.ButtonInvisible(plot)) barLabels = !barLabels;
					HoverFrame(plot);
					Draw.EllipsisLabel(col.Take(LineTiny), "labels = " + (barLabels ? "有（占 13px）" : "无") + "｜点图切换", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 71 · BarChartH
			f.Add(new Atom
			{
				No = "71", Name = "BarChartH 横条图",
				Api = "Charts.BarChartH(Rect, float[] values, string[] labels = null, Color? color = null) → void",
				Desc = "横条图，标签在左侧——列名较长时的首选形态。",
				Note = "几何：标签占左 **min(72, w×0.3)**；条本身高 min(行高,10) 垂直居中（行高 = 高度/条数）｜点图切 labels",
				BodyH = 112f, MinW = 210f, MinH = 112f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(90f);
					if (plot.width < 130f || plot.height < 40f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Charts.BarChartH(plot, ChartValsShort, hBarLabels ? ChartLabelsShort : null, Tokens.Info);
					if (Widgets.ButtonInvisible(plot)) hBarLabels = !hBarLabels;
					HoverFrame(plot);
					Draw.EllipsisLabel(col.Take(LineTiny), "labels = " + (hBarLabels ? "有（占左 30% 上限 72px）" : "无") + "｜点图切换", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 72 · StackedBarChartV
			f.Add(new Atom
			{
				No = "72", Name = "StackedBarChartV 堆叠柱",
				Api = "Charts.StackedBarChartV(Rect, float[][] series) → void",
				Desc = "多序列堆叠柱图，色走 Tokens.Series(索引)——序列数不受限。",
				Note = "几何：逐列累加求最大值后再定比例；每段色 = Tokens.Series(m)｜3 个序列 × 6 列样例",
				BodyH = 92f, MinW = 190f, MinH = 92f,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(70f);
					if (plot.width < 110f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Charts.StackedBarChartV(plot, StackSeries);
					HoverFrame(plot);
					Draw.EllipsisLabel(col.Take(LineTiny), "3 序列 × 6 列｜色由 Tokens.Series 分配", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 73 · DonutChart
			f.Add(new Atom
			{
				No = "73", Name = "DonutChart 环形图",
				Api = "Charts.DonutChart(Rect, float[] values, float hole01 = 0.5f, string centerLabel = null) → void",
				Desc = "环形占比图，中心可放标签（由组件自画，FontValue 档）。",
				Note = "几何：半径 = **min(w,h)/2 − 1**（需正方形区域）；内孔 = 半径 × clamp01(hole01)；环宽 ≥ 2；用 Draw.Rotate 画扇段｜点图切 hole01（0.5 ↔ 0.25）",
				BodyH = 112f, MinW = 150f, MinH = 112f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(90f);
					if (plot.width < 60f || plot.height < 60f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					float d = Mathf.Min(plot.width, plot.height);
					Rect sq = new Rect(plot.x, plot.y, d, d);                 // 正方形：DonutChart 取 min(w,h) 作半径基准
					Charts.DonutChart(sq, PieVals, donutHole ? 0.5f : 0.25f, "占用");
					if (Widgets.ButtonInvisible(plot)) donutHole = !donutHole;
					HoverFrame(sq);
					Draw.EllipsisLabel(col.Take(LineTiny), "hole01 = " + (donutHole ? "0.5" : "0.25") + " · 中心标签由组件自画｜点图切换", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 74 · PieChart
			f.Add(new Atom
			{
				No = "74", Name = "PieChart 饼图",
				Api = "Charts.PieChart(Rect, float[] values, string centerLabel = null) → void",
				Desc = "饼图 —— 实现上**就是** DonutChart(hole01 = 0) 的薄封装。",
				Note = "实测：PieChart 内部只有一行 DonutChart(r, values, 0f, centerLabel) ⇒ 两者行为完全一致，只有孔洞不同",
				BodyH = 112f, MinW = 150f, MinH = 112f,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(90f);
					if (plot.width < 60f || plot.height < 60f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					float d = Mathf.Min(plot.width, plot.height);
					Charts.PieChart(new Rect(plot.x, plot.y, d, d), PieVals, null);
					HoverFrame(new Rect(plot.x, plot.y, d, d));
					Draw.EllipsisLabel(col.Take(LineTiny), "与 #73 同一组数据 · 差别只在 hole01 = 0", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 75 · ScatterChart
			f.Add(new Atom
			{
				No = "75", Name = "ScatterChart 散点图",
				Api = "Charts.ScatterChart(Rect, Vector2[] points, Vector2 domain, Vector2 range, Color?) → void",
				Desc = "散点图。**必须显式给 domain / range**——组件不猜数据范围。",
				Note = "几何：点 = 3×3 方块（± 1.5）｜domain = x 轴值域，range = y 轴值域；10 点样例（x 0–100 / y 0–10）",
				BodyH = 92f, MinW = 190f, MinH = 92f,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(70f);
					if (plot.width < 110f || plot.height < 36f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Charts.ScatterChart(plot, ScatterPts, new Vector2(0f, 100f), new Vector2(0f, 10f), Tokens.Warn);
					HoverFrame(plot);
					Draw.EllipsisLabel(col.Take(LineTiny), "domain x∈[0,100] · range y∈[0,10]｜10 点", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 76 · HeatmapGrid
			f.Add(new Atom
			{
				No = "76", Name = "HeatmapGrid 热力网格",
				Api = "Charts.HeatmapGrid(Rect, float[,] grid) → void",
				Desc = "二维热力网格（活动日历 / 时段分布）。行列由二维数组长度决定。",
				Note = "几何：行列取 grid.GetLength(0)/(1)；每格 +0.5 偏移、格尺寸 max(1, cell−1) ⇒ **区域太小会糊成一片**｜8 列 × 5 行样例",
				BodyH = 92f, MinW = 190f, MinH = 92f,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect plot = col.Take(70f);
					if (plot.width < 110f || plot.height < 40f) { Draw.EllipsisLabel(plot, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					Charts.HeatmapGrid(plot, HeatGrid);
					HoverFrame(plot);
					Draw.EllipsisLabel(col.Take(LineTiny), "8 列 × 5 行｜值 0–1 走强度色", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			families.Add(f);
		}

		// ── 图表样例数据（标注为样例；非运行时数据）──────────────────────────
		/// <summary>8 点趋势样例（生产量口径）。</summary>
		private static readonly float[] ChartVals = { 12f, 28f, 19f, 34f, 26f, 41f, 30f, 47f };
		/// <summary>与上同一序列的前 5 点（柱图用，列宽才够）。</summary>
		private static readonly float[] ChartValsShort = { 12f, 28f, 19f, 34f, 26f };
		/// <summary>柱图 x 轴标签（与 ChartValsShort 等长）。</summary>
		private static readonly string[] ChartLabelsShort = { "1月", "2月", "3月", "4月", "5月" };
		/// <summary>堆叠柱样例：3 序列 × 6 列。</summary>
		private static readonly float[][] StackSeries =
		{
			new float[] { 8f, 12f, 6f, 14f, 9f, 11f },
			new float[] { 5f, 7f, 9f, 6f, 8f, 4f },
			new float[] { 3f, 2f, 5f, 4f, 6f, 7f }
		};
		/// <summary>占比样例（环 / 饼共用）。</summary>
		private static readonly float[] PieVals = { 38f, 26f, 19f, 17f };
		/// <summary>散点样例 10 点。</summary>
		private static readonly Vector2[] ScatterPts =
		{
			new Vector2(6f, 1.2f), new Vector2(14f, 2.6f), new Vector2(23f, 2.1f), new Vector2(31f, 4.4f), new Vector2(44f, 3.8f),
			new Vector2(52f, 6.1f), new Vector2(63f, 5.2f), new Vector2(71f, 7.4f), new Vector2(84f, 6.6f), new Vector2(93f, 8.8f)
		};
		/// <summary>热力样例 5 行 × 8 列（值 0–1）。</summary>
		private static readonly float[,] HeatGrid =
		{
			{ 0.1f, 0.3f, 0.6f, 0.9f, 0.7f, 0.4f, 0.2f, 0.1f },
			{ 0.2f, 0.5f, 0.8f, 1.0f, 0.8f, 0.5f, 0.3f, 0.2f },
			{ 0.1f, 0.4f, 0.7f, 0.8f, 0.9f, 0.6f, 0.3f, 0.1f },
			{ 0.3f, 0.6f, 0.5f, 0.7f, 0.6f, 0.8f, 0.4f, 0.2f },
			{ 0.2f, 0.3f, 0.4f, 0.5f, 0.5f, 0.3f, 0.2f, 0.1f }
		};

		// ── 图表件交互状态（归调用方）────────────────────────────────────────
		/// <summary>67 是否画点。</summary>
		private bool sparkDots;
		/// <summary>68 是否画网格。</summary>
		private bool lineGrid = true;
		/// <summary>68 是否画点。</summary>
		private bool lineDots = true;
		/// <summary>70 是否带 x 轴标签。</summary>
		private bool barLabels = true;
		/// <summary>71 是否带左侧标签。</summary>
		private bool hBarLabels = true;
		/// <summary>73 环孔比例档（true = 0.5）。</summary>
		private bool donutHole = true;
	}
}
