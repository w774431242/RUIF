using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
	/// <summary>
	/// 分类「反馈」· <b>6 件</b> —— 全部来自 <c>RUIF.FeedbackAtoms</c>（RUIF 已有实现，本批只做落地与验收）。
	///
	/// <para><b>本族的共同特征：组件「自画底 + 自画按钮」，且 4 件带内部命中检测</b>（返回 <c>bool</c>）。
	/// 故调用方**不得**再套底、**不得**再自己加 <c>ButtonInvisible</c> —— 否则会重复交互（点一次触发两次）。</para>
	///
	/// <para>几何逐条取自反编译实现：<c>Toast</c> 左侧 3px 色条 + alpha 由调用方给；
	/// <c>AlertBanner</c> 右侧预留 90px 给按钮；<c>ConfirmDialog</c> 取消键定宽 70、危险态边框转红；
	/// <c>RichTooltip</c> stats/requirements 各 14px 行；<c>ProgressOverlay</c>/<c>EmptyState</c> 内容块居中。</para>
	/// </summary>
	internal sealed partial class BenchAtoms42
	{
		/// <summary>反馈族 · 6 件。注册于 <c>Build()</c>，页签「反馈」。</summary>
		private void BuildFeedback()
		{
			Family f = new Family
			{
				Title = "反馈 · FeedbackAtoms",
				Sub = "6 件 · Toast / AlertBanner / ConfirmDialog / RichTooltip / ProgressOverlay / EmptyState（4 件自带命中检测）"
			};

			// ────────────────────────────────────────────── 77 · Toast
			f.Add(new Atom
			{
				No = "77", Name = "Toast 浮动通知",
				Api = "FeedbackAtoms.Toast(Rect, string title, string sub, ToastLevel level, float alpha) → void",
				Desc = "浮动通知条：左侧 3px 语义色条 + 标题 / 副标题两行；**透明度由调用方给**（淡入淡出归调用方）。",
				Note = "几何：自画底 WithAlpha(Card, alpha) + 左边 3px 色条 + 边框；Inset(10,4)、标题行 16、副标行 13 ⇒ 每条约需 42 高｜alpha 随帧呼吸，点一条「钉住」为 1.0",
				// B-R13 修：Take 链 42×3 + gap4×3 + LineTiny18 = 156，原 BodyH/MinH=150 溢 6（末行说明被裁）→ 156
				BodyH = 156f, MinW = 190f, MinH = 156f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 4f);
					ToastLevel[] lv = { ToastLevel.Ok, ToastLevel.Warn, ToastLevel.Danger };
					string[] tt = { "已保存", "负载偏高", "存储区已满" };
					string[] ss = { "蓝图已写入磁盘", "电力净剩余 12 W", "草药无处可放，已掉落" };
					for (int i = 0; i < lv.Length; i++)
					{
						Rect box = col.Take(42f);
						if (box.width < 80f) break;
						// alpha 由调用方决定 —— 这里用正弦呼吸演示「淡入淡出归调用方」
						float a = toastPinned == i ? 1f : Mathf.Clamp01(0.55f + 0.45f * Mathf.Sin(t * 1.6f + i * 1.2f));
						FeedbackAtoms.Toast(box, tt[i], ss[i], lv[i], a);
						if (Widgets.ButtonInvisible(box)) toastPinned = toastPinned == i ? -1 : i;
						HoverFrame(box);
					}
					Draw.EllipsisLabel(col.Take(LineTiny), "alpha 随帧呼吸（点一条钉住为 1.0）—— 淡入淡出是调用方的责任", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 78 · AlertBanner
			f.Add(new Atom
			{
				No = "78", Name = "AlertBanner 警报横幅",
				Api = "FeedbackAtoms.AlertBanner(Rect, AlertPriority level, string msg, string actionLabel = null) → bool",
				Desc = "一条警报横幅，四档优先级；右侧可带一个动作按钮，**返回该按钮是否被点**。",
				Note = "几何：左边竖线 + 底 12% 色；消息占 max(10, w − 90) ⇒ **右侧预留 90px 给按钮**；actionLabel 为空则直接 return false（不画按钮）｜点「处理」看计数累加",
				BodyH = 142f, MinW = 250f, MinH = 142f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 4f);
					AlertPriority[] ps = { AlertPriority.Low, AlertPriority.Normal, AlertPriority.Warn, AlertPriority.Urgent };
					string[] ms = { "例行巡检完成", "零件库存低于阈值", "三名殖民者患有流感", "袭击将在 1 天内抵达" };
					for (int i = 0; i < ps.Length; i++)
					{
						Rect box = col.Take(28f);
						if (box.width < 140f) break;
						// 命中检测在组件内部 ⇒ 只用返回值，不再自己 ButtonInvisible
						if (FeedbackAtoms.AlertBanner(box, ps[i], ms[i], "处理")) alertClicks++;
						HoverFrame(box);
					}
					Draw.EllipsisLabel(col.Take(LineTiny), "「处理」累计点击 " + alertClicks + " 次（返回值即命中）", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 79 · ConfirmDialog
			f.Add(new Atom
			{
				No = "79", Name = "ConfirmDialog 确认弹窗",
				Api = "FeedbackAtoms.ConfirmDialog(Rect, string title, string msg, bool danger, string cancelLabel = null, string okLabel = null) → bool",
				Desc = "确认对话框：标题 + 说明 + 取消 / 确定两键；`danger` 为真时边框转红、确定文案变「确认」。",
				Note = "几何：自画底 Panel + 边框（danger ? Dead : BorderSoft）；Inset(10,8)、标题行 20、消息行 30、取消键**定宽 70** ⇒ 整框至少 **168×104**｜默认文案走 Tr.Get，本件显式传入以固定演示｜点标题区切 danger",
				BodyH = 128f, MinW = 200f, MinH = 128f,
				Interactive = true,
				Paint = r =>
				{
					var col = Layout.Column(r, 2f);
					Rect box = col.Take(104f);
					if (box.width < 160f) { Draw.EllipsisLabel(box, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					if (FeedbackAtoms.ConfirmDialog(box, cdDanger ? "拆除建筑" : "覆盖蓝图", "此操作不可撤销，是否继续？", cdDanger, "取消", cdDanger ? "确认" : "确定"))
						cdResult = "确定";
					// 取消键是组件内部画的，调用方只拿「是否确定」；故再给一个显式开关演示 danger
					Rect tg = new Rect(box.x, box.y, box.width, 20f);
					if (Widgets.ButtonInvisible(tg)) { cdDanger = !cdDanger; cdResult = "（切换 danger）"; }
					HoverFrame(box);
					Draw.EllipsisLabel(col.Take(LineTiny), "danger = " + cdDanger + " · 上次选择：" + cdResult + "｜点标题行切 danger", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
				}
			});

			// ────────────────────────────────────────────── 80 · RichTooltip
			f.Add(new Atom
			{
				No = "80", Name = "RichTooltip 富提示",
				Api = "FeedbackAtoms.RichTooltip(Rect, string title, string[] stats, string[] requirements) → void",
				Desc = "带属性表与需求清单的富提示框（悬停详情 / 建造需求）。",
				Note = "几何：自画底 Card + **accent 边框**；Inset(8,5)、标题行 18（accent）、stats 与 requirements **各 14px/行**，requirements 自动加「· 」前缀｜高度随条目数线性增长",
				BodyH = 120f, MinW = 190f, MinH = 120f,
				Paint = r =>
				{
					if (r.width < 150f || r.height < 90f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					FeedbackAtoms.RichTooltip(r, "钢铁长剑", TtStats, TtReqs);
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 81 · ProgressOverlay
			f.Add(new Atom
			{
				No = "81", Name = "ProgressOverlay 任务进度",
				Api = "FeedbackAtoms.ProgressOverlay(Rect, string title, float progress01, string step) → void",
				Desc = "半透明遮罩 + 居中进度块，用于长任务（生成地图 / 批量重算）。",
				Note = "几何：自画底 WithAlpha(Window, 0.72) 半透明遮罩；内容块宽 = **min(300, w×0.7) 居中**；标题 16、进度条 8、步骤行 Rest ⇒ 覆盖层需给整块区域｜进度随帧推进，点一下复位",
				BodyH = 100f, MinW = 200f, MinH = 100f,
				Interactive = true,
				Paint = r =>
				{
					if (r.width < 120f || r.height < 60f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					poProgress = Mathf.Clamp01(poProgress + (poHeld ? 0f : Time.deltaTime * 0.18f));
					FeedbackAtoms.ProgressOverlay(r, poHeld ? "已暂停（重算寻路网格）" : "正在重算寻路网格", poProgress,
						"第 " + Mathf.FloorToInt(poProgress * 100f) + " / 100 区块");
					// 交互：运行中点 = 暂停 / 继续；跑满后点 = 重新开始 —— 三种状态都可达，无常驻死状态
					if (Widgets.ButtonInvisible(r))
					{
						if (poProgress >= 1f) { poProgress = 0f; poHeld = false; }
						else poHeld = !poHeld;
					}
					HoverFrame(r);
				}
			});

			// ────────────────────────────────────────────── 82 · EmptyState
			f.Add(new Atom
			{
				No = "82", Name = "EmptyState 空状态",
				Api = "FeedbackAtoms.EmptyState(Rect, string title, string sub, string cta = null) → bool",
				Desc = "空状态占位：顶部装饰条 + 标题 + 说明 + 行动按钮，**返回按钮是否被点**。",
				Note = "几何：内容块宽 = **min(300, w×0.8) 居中**；顶部装饰条 3px（宽 28，accent，居中）；标题 18、副标 14、按钮 **120×22 居中**｜cta 为空则不画按钮",
				BodyH = 104f, MinW = 200f, MinH = 104f,
				Interactive = true,
				Paint = r =>
				{
					if (r.width < 140f || r.height < 70f) { Draw.EllipsisLabel(r, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
					if (FeedbackAtoms.EmptyState(r, "尚未选中任何殖民者", "在上方列表里点一个名字即可查看详情", esHasCta ? "打开列表" : null))
						esClicks++;
					HoverFrame(r);
					Draw.EllipsisLabel(new Rect(r.x, r.yMax - LineTiny, r.width, LineTiny),
						"cta " + (esHasCta ? "有" : "无") + " · 累计点击 " + esClicks + " 次（点右下角切 cta）", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
					// cta 开关放在右下角，避免与组件自画的居中按钮抢命中
					Rect tg = new Rect(r.xMax - 46f, r.yMax - LineTiny, 46f, LineTiny);
					if (Widgets.ButtonInvisible(tg)) esHasCta = !esHasCta;
				}
			});

			families.Add(f);
		}

		// ── 反馈族样例数据（标注为样例；非运行时数据）────────────────────────
		/// <summary>富提示的属性行（样例）。</summary>
		private static readonly string[] TtStats =
		{
			"伤害 14.2（锐器）", "攻击间隔 1.6 秒", "品质 优秀（命中率 +8%）"
		};
		/// <summary>富提示的需求行（样例）。requirements 由组件加「· 」前缀。</summary>
		private static readonly string[] TtReqs =
		{
			"钢铁 ×18", "零部件 ×2", "锻造台（二级以上）"
		};

		// ── 反馈族交互状态（归调用方；组件零状态）──────────────────────────
		/// <summary>77 被钉住的 Toast 序号（-1 = 无）。</summary>
		private int toastPinned = -1;
		/// <summary>78 「处理」按钮累计点击数（由组件返回值驱动）。</summary>
		private int alertClicks;
		/// <summary>79 危险态开关。</summary>
		private bool cdDanger;
		/// <summary>79 上次选择结果（组件只返回「是否确定」）。</summary>
		private string cdResult = "（尚未选择）";
		/// <summary>81 当前进度。</summary>
		private float poProgress;
		/// <summary>81 是否暂停推进（预留）。</summary>
		private bool poHeld;
		/// <summary>82 是否有行动按钮。</summary>
		private bool esHasCta = true;
		/// <summary>82 按钮累计点击数。</summary>
		private int esClicks;
	}
}

// =====================================================================
// 以下为新落地：分类「反馈」独立验证面（实现 IBench）。
// 上方 BenchAtoms42 的 partial（BuildFeedback 及其交互字段）原样保留，
// 因其被 BenchAtoms42Restored.cs 的 Build() 调用，移除会导致整工程无法编译。
// =====================================================================

namespace RUIF.Demo
{
    using RUIF;
    using UnityEngine;

    /// <summary>
    /// 分类「反馈」· <b>15 件</b>：
    /// 候选层 <see cref="FeedbackCandAtoms"/>（FloatMenu / AlertTray）2 件 +
    /// 原生 <see cref="FeedbackAtoms"/>（AlertBanner / ConfirmDialog / RichTooltip / ProgressOverlay / EmptyState）5 件 +
    /// 原生 <see cref="FeedbackAtomsB"/>（UndoToast / StickyNote / AlertBlinker / CriticalOverlay / StepProgressList / InlineError / BatchConfirm / ActionFeedback）8 件。
    /// 候选层先构造数据（Fmt/数据形态）再经 Atoms 绘制；原生原子直接给定示例数据渲染。
    /// 全部来自 RUIF 已有实现，本面只做落地与验收。
    /// </summary>
    public sealed class BenchFeedback : IBench
    {
        /// <inheritdoc />
        public bool Scrollable => true;

        private const float HeadH = 20f;   // 标题行
        private const float Gap = 12f;     // 组件间距

        // 各组件本体高度（与 Render 中的 Take 严格一致，供 Height 估算）
        private static readonly float[] BodyH =
        {
            150f, // FloatMenu
            120f, // AlertTray
            44f,  // AlertBanner
            110f, // ConfirmDialog
            120f, // RichTooltip
            120f, // ProgressOverlay
            110f, // EmptyState
            40f,  // UndoToast
            80f,  // StickyNote
            64f,  // AlertBlinker
            90f,  // CriticalOverlay
            150f, // StepProgressList
            34f,  // InlineError
            110f, // BatchConfirm
            44f   // ActionFeedback
        };

        /// <inheritdoc />
        public float Height(float w)
        {
            float h = 0f;
            for (int i = 0; i < BodyH.Length; i++) h += HeadH + BodyH[i] + Gap;
            return h;
        }

        /// <inheritdoc />
        public void Render(Rect area)
        {
            var col = Layout.Column(area, Gap);

            // ── 候选层：FloatMenu（先构造 labels 数据，再 Atoms 绘制）
            // ⚠ 返回值 = 选中索引（≥0 表示本帧被点选）—— A@R-03 增强：不再丢弃，标题行回显
            RenderOne(ref col, "FloatMenu 浮动菜单" + FmPickedLabel(), BodyH[0], r =>
            {
                int hover = -1;
                int picked = FeedbackCandAtoms.FloatMenu(r,
                    new[] { "打开列表", "聚焦殖民者", "切换剧情", "保存并退出" }, ref hover);
                if (picked >= 0) fmPicked = picked;
            });

            // ── 候选层：AlertTray（先构造 AlertEntry[]，再 Atoms 绘制）
            RenderOne(ref col, "AlertTray 警报托盘", BodyH[1], r =>
            {
                var alerts = new[]
                {
                    new FeedbackCandAtoms.AlertEntry { label = "食物储备不足", severity = 2 },
                    new FeedbackCandAtoms.AlertEntry { label = "寒潮预警", severity = 1 },
                    new FeedbackCandAtoms.AlertEntry { label = "例行巡检完成", severity = 0 },
                };
                FeedbackCandAtoms.AlertTray(r, alerts);
            });

            // ── 原生反馈原子（FeedbackAtoms）
            RenderOne(ref col, "AlertBanner 警报横幅", BodyH[2], r =>
                FeedbackAtoms.AlertBanner(r, AlertPriority.Warn, "电力储备偏低，建议补充燃料。", "处理"));

            RenderOne(ref col, "ConfirmDialog 确认弹窗", BodyH[3], r =>
                FeedbackAtoms.ConfirmDialog(r, "拆除建筑", "此操作不可撤销，是否继续？", true, "取消", "确认"));

            RenderOne(ref col, "RichTooltip 富提示", BodyH[4], r =>
                FeedbackAtoms.RichTooltip(r, "钢铁长剑",
                    new[] { "伤害 14.2（锐器）", "攻击间隔 1.6 秒" },
                    new[] { "钢铁 ×18", "零部件 ×2" }));

            RenderOne(ref col, "ProgressOverlay 进度遮罩", BodyH[5], r =>
                FeedbackAtoms.ProgressOverlay(r, "正在重算寻路网格", 0.6f, "第 60 / 100 区块"));

            RenderOne(ref col, "EmptyState 空状态", BodyH[6], r =>
                FeedbackAtoms.EmptyState(r, "尚未选中任何殖民者", "在上方列表里点一个名字即可查看详情。", "打开列表"));

            // ── 原生反馈原子 B 族（FeedbackAtomsB）
            RenderOne(ref col, "UndoToast 撤销浮条", BodyH[7], r =>
                FeedbackAtomsB.UndoToast(r, "删除蓝图", 1f));

            RenderOne(ref col, "StickyNote 便签", BodyH[8], r =>
                FeedbackAtomsB.StickyNote(r, "记得在日落前喂狗。", new Color(1f, 0.95f, 0.6f)));

            RenderOne(ref col, "AlertBlinker 分级闪烁", BodyH[9], r =>
                FeedbackAtomsB.AlertBlinker(r, Time.time, AlertPriority.Urgent));

            RenderOne(ref col, "CriticalOverlay 严重遮罩", BodyH[10], r =>
                FeedbackAtomsB.CriticalOverlay(r, "存储空间已满，无法保存。", "查看"));

            RenderOne(ref col, "StepProgressList 步骤清单", BodyH[11], r =>
                FeedbackAtomsB.StepProgressList(r,
                    new[] { "采集原料", "冶炼金属", "锻造部件", "交付成品" }, 2));

            RenderOne(ref col, "InlineError 行内错误", BodyH[12], r =>
                FeedbackAtomsB.InlineError(r, "名称重复，请更换。", "修正"));

            RenderOne(ref col, "BatchConfirm 批量确认", BodyH[13], r =>
                FeedbackAtomsB.BatchConfirm(r, "批量拆除", "建筑 ×12 · 家具 ×5", "取消", "执行"));

            RenderOne(ref col, "ActionFeedback 操作回执", BodyH[14], r =>
                FeedbackAtomsB.ActionFeedback(r, true, 1f));
        }

        // ── FloatMenu 选中状态（归调用方；A@R-03 增强补齐）──────────────────
        /// <summary>FloatMenu 最近一次点选的项索引（-1 = 尚未点选）。</summary>
        private int fmPicked = -1;

        /// <summary>标题行回显片段：未点选时为空，点选后显示「· 选中 N」。</summary>
        private string FmPickedLabel() => fmPicked >= 0 ? " · 选中 " + (fmPicked + 1) : "";

        /// <summary>统一：取标题行 + 卡片底 + 内缩后把本体区域交给 paint 绘制。</summary>
        private static void RenderOne(ref VStack col, string title, float bodyH, Action<Rect> paint)
        {
            Rect head = col.Take(HeadH);
            Draw.Label(head, title, Tokens.FontLabel, Tokens.Text, TextAnchor.MiddleLeft);
            Rect body = col.Take(bodyH);
            Draw.Box(body, Tokens.Card);
            Draw.Border(body, Tokens.BorderSoft);
            paint(Layout.Inset(body, 8f, 6f));
        }
    }
}
