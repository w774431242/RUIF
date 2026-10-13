using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
	/// <summary>
	/// 分类「导航」· <b>6 件</b> —— 来自 <c>RUIF.NavAtoms</c>(5) + <c>RUIF.CommandPalette</c>(1)。
	/// RUIF 已有实现，本批只做落地与验收。
	///
	/// <para>几何逐条取自反编译实现：<c>TabBar</c> 指示器画在 <c>yMax−3</c>（宽 24 高 3）、
	/// 标签宽走 <c>NavFmt.TabWidth</c>、超宽降级为「更多」按钮；<c>Breadcrumb</c> 每段宽
	/// <c>LabelUnits × 6.5 + 12</c>、分隔符「›」占 14；<c>Pager</c> 四个方向键各 26 宽、页码键 26 宽、省略号 14；
	/// <c>CollapsibleTree</c> 缩进 16、<c>Dead</c> 节点用 <c>LineH</c> 划线；<c>AnchorDots</c> 悬停时在右侧
	/// <c>xMax−74</c> 显示标签区。</para>
	///
	/// <para><b>两个「调用方必须持有状态」的点</b>：<c>TabBar</c> 的 <c>ref indicatorX</c> 是<b>指示器动画位置</b>
	/// （要自己插值，组件只画）；<c>Pager</c> 的 <c>int[] pageBuf</c> 是<b>复用缓冲</b>（避免每帧分配）。</para>
	/// </summary>
	internal sealed partial class BenchAtoms42
	{
		/// <summary>导航族 · 6 件。注册于 <c>Build()</c>，页签「导航」。</summary>
		private void BuildNav()
		{
			Family f = new Family
			{
				Title = "导航 · NavAtoms / CommandPalette",
				Sub = "6 件 · TabBar / Breadcrumb / Pager / CollapsibleTree / AnchorDots / CommandPalette"
			};

			// ────────────────────────────────────────────── 83 · TabBar
			f.Add(new Atom
			{
				No = "83", Name = "TabBar 顶栏页签",
				Api = "NavAtoms.TabBar(Rect, string[] tabs, ref int sel, ref float indicatorX) → void",
				Desc = "顶栏页签 + 底部滑动指示条。**指示器动画位置由调用方持有**（ref 传入，组件只负责画）。",
				Note = "几何：标签宽走 NavFmt.TabWidth(label, 6.5, 20)；**指示器画在 yMax−3、宽 24、高 3**；超宽时降级为「更多」按钮｜点标签切换，指示器自行插值追上去",
				BodyH = 62f, MinW = 230f, MinH = 62f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect bar = col.Take(34f);
					if (bar.width < 160f) { Draw.EllipsisLabel(bar, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					// 指示器插值：目标是当前标签的左缘，由调用方（本类）每帧逼近 —— 组件只画
					float target = NavTabX(NavTabs, navTab, bar);
					navIndicator += (target - navIndicator) * Mathf.Min(1f, Time.deltaTime * 12f);
					NavAtoms.TabBar(bar, NavTabs, ref navTab, ref navIndicator);
					HoverFrame(bar);
					Draw.EllipsisLabel(col.Take(LineTiny), "sel = " + navTab + "（「" + NavTabs[navTab] + "」）· 指示器宽度固定 24", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 84 · Breadcrumb
			f.Add(new Atom
			{
				No = "84", Name = "Breadcrumb 面包屑",
				Api = "NavAtoms.Breadcrumb(Rect, string[] path, Action<int> onJump) → void",
				Desc = "层级路径 + 点击任意段跳回。**用回调**（Action<int>）而不是返回值 —— 命中检测在组件内部。",
				Note = "几何：每段宽 = NavFmt.LabelUnits(label) × 6.5 + 12，上限受 rect.xMax 约束；分隔符「›」占 **14** 宽、右对齐｜点任意段记录跳转层级",
				BodyH = 48f, MinW = 200f, MinH = 48f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect bar = col.Take(26f);
					if (bar.width < 120f) { Draw.EllipsisLabel(bar, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					NavAtoms.Breadcrumb(bar, NavPath, idx => navJumped = idx);   // 回调：命中在组件内
					HoverFrame(bar);
					Draw.EllipsisLabel(col.Take(LineTiny),
						navJumped < 0 ? "点任意一段跳转（回调 onJump）" : "上次跳转到第 " + navJumped + " 段：「" + NavPath[navJumped] + "」",
						GameFont.Tiny, navJumped < 0 ? Tokens.Dim : Tokens.Alive, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 85 · Pager
			f.Add(new Atom
			{
				No = "85", Name = "Pager 分页器",
				Api = "NavAtoms.Pager(Rect, int total, int pageSize, ref int page, int[] pageBuf = null) → void",
				Desc = "分页器：首/上/页码/下/末，当前页高亮。**pageBuf 是调用方提供的复用缓冲**（避免每帧分配）。",
				Note = "几何：Row gap 2；**« ‹ › » 各 26 宽**、页码键 26 宽、省略号 14 宽；首/上键在 page=0 时走 Draw.Enabled(false) 置灰｜12 项 / 每页 5 ⇒ 3 页",
				BodyH = 48f, MinW = 250f, MinH = 48f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect bar = col.Take(26f);
					if (bar.width < 180f) { Draw.EllipsisLabel(bar, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					NavAtoms.Pager(bar, NavTotal, NavPageSize, ref navPage, navPageBuf);
					HoverFrame(bar);
					Draw.EllipsisLabel(col.Take(LineTiny), "第 " + (navPage + 1) + " / " + Mathf.CeilToInt(NavTotal / (float)NavPageSize) + " 页（共 " + NavTotal + " 项）", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 86 · CollapsibleTree
			f.Add(new Atom
			{
				No = "86", Name = "CollapsibleTree 折叠树",
				Api = "NavAtoms.CollapsibleTree(Rect, TreeNode root) → TreeNode",
				Desc = "可折叠树（缩进 + 展开箭头 + 右侧角标）。`TreeNode.Expanded` 由调用方持有，跨帧复用。",
				Note = "几何：缩进 16；**Dead 节点用 LineH 在 y+12.1 划线**（置灰 + 删除线）；Tag 右对齐｜点行折叠/展开；「阵亡者」演示 Dead 态（默认折叠）",
				BodyH = 118f, MinW = 210f, MinH = 118f,
				Interactive = true,
				Paint = r =>
				{
					if (r.width < 150f || r.height < 80f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					NavAtoms.CollapsibleTree(r, navTree);      // Expanded 状态就在 navTree 里，跨帧保留
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 87 · AnchorDots
			f.Add(new Atom
			{
				No = "87", Name = "AnchorDots 侧边锚点",
				Api = "NavAtoms.AnchorDots(Rect, string[] sections, ref int current) → void",
				Desc = "侧边锚点组：点定位到某节，悬停时在右侧展开该节名称。",
				Note = "几何：悬停判定区 = 右侧 **xMax−74 起、74×10**；点区同宽 ⇒ **右侧需预留 74px** 且各点纵向均分 rect 高｜6 节，当前节高亮",
				BodyH = 100f, MinW = 150f, MinH = 100f,
				Interactive = true,
				Paint = r =>
				{
					var row = Layout.Row(r, 6f);
					Rect dots = row.Take(Mathf.Min(26f, row.Rest.width));
					if (dots.width < 8f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					NavAtoms.AnchorDots(r, NavSections, ref navCurrent);
					HoverFrame(r);
					Draw.EllipsisLabel(new Rect(r.x, r.yMax - LineTiny, r.width, LineTiny),
						"current = " + navCurrent + "（「" + NavSections[navCurrent] + "」）｜悬停看节名", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 88 · CommandPalette
			f.Add(new Atom
			{
				No = "88", Name = "CommandPalette 命令面板",
				Api = "CommandPalette.Draw(Rect, Command[] commands, ref string query, ref int sel, Command[] filtered, int[] scores) → Command",
				Desc = "命令面板：输入过滤 + 结果列表 + 选中执行；返回被执行的 Command（无则 null）。",
				Note = "两段式：**Filter(query, source, dest, scores) 先筛，再 Draw(...)**；filtered/scores 都是**调用方提供的复用缓冲**（无每帧分配）｜6 条命令、3 个分组；输入即过滤",
				BodyH = 148f, MinW = 230f, MinH = 148f,
				Interactive = true,
				Paint = r =>
				{
					if (r.width < 170f || r.height < 110f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					// 先筛（写进复用缓冲），再画
					int n = CommandPalette.Filter(cmdQuery, NavCommands, cmdFiltered, cmdScores);
					Command hit = CommandPalette.Draw(r, NavCommands, ref cmdQuery, ref cmdSel, cmdFiltered, cmdScores);
					if (hit != null) cmdLast = hit.Label;
					HoverFrame(r);
					Draw.EllipsisLabel(new Rect(r.x, r.yMax - LineTiny, r.width, LineTiny),
						"命中 " + n + " / " + NavCommands.Length + " · sel = " + cmdSel + (cmdLast == null ? "" : " · 上次执行：「" + cmdLast + "」"),
						GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			families.Add(f);
		}

		/// <summary>估算第 idx 个标签在 TabBar 中的左缘（用于指示器插值目标）—— 与组件内部同一算式口径。</summary>
		private static float NavTabX(string[] tabs, int idx, Rect bar)
		{
			float x = bar.x;
			for (int i = 0; i < idx && i < tabs.Length; i++) x += Mathf.Min(tabs[i].Length * 6.5f + 20f, 120f);
			return x + 2f;
		}

		// ── 导航族样例数据（标注为样例；非运行时数据）────────────────────────
		private static readonly string[] NavTabs = { "概览", "殖民者", "资源", "电力", "日志" };
		private static readonly string[] NavPath = { "殖民地", "存储区", "钢铁" };
		private static readonly string[] NavSections = { "概要", "属性", "装备", "技能", "健康", "社交" };
		private const int NavTotal = 12;
		private const int NavPageSize = 5;

		/// <summary>演示用折叠树（3 层；「阵亡者」为 Dead 态且默认折叠）。</summary>
		private static readonly TreeNode navTree = BuildDemoTree();

		private static TreeNode BuildDemoTree()
		{
			TreeNode root = new TreeNode("殖民者");
			root.Tag = "12";
			TreeNode a = root.Add(new TreeNode("蓝 雪牛"));
			a.Tag = "3";
			a.Add(new TreeNode("钢铁长剑")).Tag = "装备";
			a.Add(new TreeNode("射击 14")).Tag = "技能";
			TreeNode b = root.Add(new TreeNode("赤 眼镜蛇"));
			b.Tag = "2";
			TreeNode dead = root.Add(new TreeNode("阵亡者"));
			dead.Tag = "5491";
			dead.Dead = true;
			dead.Expanded = false;
			return root;
		}

		/// <summary>演示用命令表（3 个分组）。</summary>
		private static readonly Command[] NavCommands =
		{
			new Command("打开研究台", null, "生产"),
			new Command("打开建造菜单", null, "生产"),
			new Command("招募囚犯", null, "社交"),
			new Command("派遣商队", null, "世界"),
			new Command("整理存储区", null, "生产"),
			new Command("切换到电力视图", null, "视图")
		};

		// ── 导航族交互状态（归调用方；组件零状态）──────────────────────────
		/// <summary>83 选中标签。</summary>
		private int navTab;
		/// <summary>83 指示器动画位置（由本类插值，组件只画）。</summary>
		private float navIndicator;
		/// <summary>84 上次跳转到的层级（-1 = 尚未点过）。</summary>
		private int navJumped = -1;
		/// <summary>85 当前页。</summary>
		private int navPage;
		/// <summary>85 复用缓冲（组件不自己分配）。</summary>
		private readonly int[] navPageBuf = new int[64];
		/// <summary>87 当前节。</summary>
		private int navCurrent;
		/// <summary>88 查询词。</summary>
		private string cmdQuery = "";
		/// <summary>88 选中项。</summary>
		private int cmdSel;
		/// <summary>88 上次执行（返回值）。</summary>
		private string cmdLast;
		/// <summary>88 复用缓冲 ×2（无每帧分配）。</summary>
		private readonly Command[] cmdFiltered = new Command[64];
		private readonly int[] cmdScores = new int[64];
	}
}
