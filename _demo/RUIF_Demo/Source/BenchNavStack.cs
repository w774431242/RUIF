using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
	/// <summary>
	/// 分类「导航」· 追加 2 件（108/109）—— <c>NavAtomsB</c> 导航扩展的<b>活体闭环</b>演示。
	/// 本文件是 <c>BenchAtoms42</c> 的追加 partial 族，注册于 <c>Build()</c>「导航」段（1 行）。
	///
	/// <para><b>契约取证（ilspycmd 实测，勿凭签名猜）</b>：<c>NavBackBar</c> <b>只返回动作、不改栈</b> ——
	/// « » 按钮用 <c>Draw.Enabled(CanBack/CanForward)</c> 压暗，回退/前进的 <c>stack.Back()/Forward()</c>
	/// 职责在调用方（这正是此前 BenchNavLayout 未演示它的原因：需要真实栈闭环才成立）；
	/// 「≡」历史下拉由组件内部弹 FloatMenu 并直接 <c>JumpTo(idx)</c>（命中在组件内）。
	/// <c>JumpList</c> 返回点击条目索引（-1 = 无）—— 本卡把它<b>真实消费</b>为 <c>NavStack.Push</c>，
	/// 关闭账本 §4「List/Jump 返回值未消费」增强候选项的导航侧。</para>
	/// </summary>
	internal sealed partial class BenchAtoms42
	{
		// ── 卡 108 状态（字段随件声明，全部可达 · 0 死状态）
		private readonly NavAtomsB.NavStack backStack = new NavAtomsB.NavStack();
		private string backLastAct = "（尚无操作）";
		private int backPushIdx;

		private static readonly string[] BackSeedPages = { "殖民者总览", "研究台 · 电路", "世界地图", "事件日志" };
		private static readonly string[] BackPushPool = { "动物管理", "装备配给", "风暴警报", "商队出发", "食谱计划" };

		// ── 卡 109 状态
		private readonly NavAtomsB.NavStack jumpStack = new NavAtomsB.NavStack();
		private NavAtomsB.RecentEntry[] jumpRecent;
		private int jumpLast = -1;

		/// <summary>懒初始化的最近查看清单（RecentEntry(title, kind, ageSec)，契约见 NavAtomsB）。</summary>
		private NavAtomsB.RecentEntry[] JumpRecent
		{
			get
			{
				if (jumpRecent == null)
					jumpRecent = new[]
					{
						new NavAtomsB.RecentEntry("蓝 雪牛", "人", 60f),
						new NavAtomsB.RecentEntry("钢铁长剑", "装", 300f),
						new NavAtomsB.RecentEntry("研究台", "建", 1200f),
						new NavAtomsB.RecentEntry("商队事件", "役", 3600f)
					};
				return jumpRecent;
			}
		}

		/// <summary>导航扩展族 · 2 件。注册于 <c>Build()</c>，页签「导航」。</summary>
		private void BuildNavStack()
		{
			Family f = new Family
			{
				Title = "导航扩展 · NavAtomsB 活体闭环",
				Sub = "2 件 · NavBackBar（真实栈回退/前进）/ JumpList → NavStack（返回值真实消费）"
			};

			// ────────────────────────────────────────────── 108 · NavBackBar
			f.Add(new Atom
			{
				No = "108", Name = "NavBackBar 返回栏（活体栈）",
				Api = "NavAtomsB.NavBackBar(Rect, NavStack stack, string currentLabel) → NavAction",
				Desc = "« » 历史回退/前进 + 深度显示 + 「≡」历史下拉。**组件只报动作、不改栈** —— Back()/Forward() 由调用方执行。",
				Note = "取证：按钮用 Draw.Enabled(CanBack/CanForward) 压暗；「≡」由组件内部弹 FloatMenu 并 JumpTo（命中在组件内）｜点「推进新页」看前进分支被截断",
				BodyH = 86f, MinW = 260f, MinH = 86f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 3f);
					if (backStack.Depth == 0)
						foreach (string p in BackSeedPages) backStack.Push(p);

					var act = NavAtomsB.NavBackBar(col.Take(30f), backStack, null);
					if (act == NavAction.Back && backStack.CanBack) { backStack.Back(); backLastAct = "Back → " + backStack.Current; }
					else if (act == NavAction.Forward && backStack.CanForward) { backStack.Forward(); backLastAct = "Forward → " + backStack.Current; }

					Rect pushRow = col.Take(26f);
					if (Widgets.ButtonText(pushRow, "推进新页 ▸"))
					{
						string page = BackPushPool[backPushIdx % BackPushPool.Length];
						backStack.Push(page);
						backPushIdx++;
						backLastAct = "Push「" + page + "」（前进分支已截断）";
					}
					// ButtonText 自带 hover，按红线不重复挂 HoverFrame
					Draw.EllipsisLabel(col.Take(LineTiny),
						"depth=" + backStack.Depth + " pos=" + backStack.Position + " cur=" + (backStack.Current ?? "—") + " · " + backLastAct,
						GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 109 · JumpList → NavStack
			f.Add(new Atom
			{
				No = "109", Name = "JumpList 最近查看（返回值消费）",
				Api = "NavAtomsB.JumpList(Rect, RecentEntry[] recent, int max = 8) → int",
				Desc = "最近查看清单（徽 + 名 + 相对时间），返回点击条目索引（-1 = 无）。**本卡把返回值真实消费**：命中即 Push 入导航栈。",
				Note = "几何：行高由组件内部按条目数分配（4 条给 100 高即可）｜命中在组件内，AgeSec 走 NavFmt.RelativeTime 分桶着色",
				BodyH = 148f, MinW = 260f, MinH = 148f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 3f);
					Draw.EllipsisLabel(col.Take(LineTiny), "点条目 ⇒ 返回值 Push 入导航栈（真实消费，非只回显）",
						GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

					Rect listRect = col.Take(100f);   // 与 JumpList 完全相同的 rect 表达式（HoverFrame 红线）
					int hit = NavAtomsB.JumpList(listRect, JumpRecent, 8);
					if (hit >= 0 && hit != jumpLast)
					{
						jumpLast = hit;
						jumpStack.Push(JumpRecent[hit].Title);
					}
					HoverFrame(listRect);
					Draw.EllipsisLabel(col.Take(LineTiny),
						jumpLast < 0
							? "（未选择）"
							: "命中 #" + jumpLast + "「" + JumpRecent[jumpLast].Title + "」→ 栈 depth=" + jumpStack.Depth + " cur=" + (jumpStack.Current ?? "—"),
						GameFont.Tiny, jumpLast < 0 ? Tokens.Dim : Tokens.Alive, TextAnchor.MiddleLeft);
				}
			});

			families.Add(f);
		}
	}
}
