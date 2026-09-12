// 《零基础上手教程》正文示例的逐字编译件（§3 第一个窗口 / §10 完整示例 / §4 布局速成补漏）
// 与 docs/零基础上手教程.html 的代码块保持逐字一致；改文档 → 同步到这里 → 重跑 verify_samples.ps1
using System;
using RUIF;
using UnityEngine;
using Verse;

namespace TutorialConsumer
{
    // ===== 教程 §3：画第一个窗口 =====
    public class Dialog_MyFirstWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(560f, 320f);   // 窗口初始大小

        public override void DoWindowContents(Rect inRect)
        {
            // 1) 竖着切三行：标题 / 内容 / 按钮
            var col = Layout.Column(inRect, Tokens.Gap);
            Rect head = col.Take(30f);
            Rect body = col.Take(200f);
            Rect foot = col.Rest;

            // 2) 标题
            Draw.Label(head, "我的第一个 RUIF 窗口", Tokens.FontValue, Tokens.Text, TextAnchor.MiddleLeft);

            // 3) 内容：一块面板 + 两行统计
            Draw.Box(body, Tokens.Panel);
            Draw.Border(body, Tokens.Border);
            Rect inner = Layout.Inset(body, Tokens.CardPadX, Tokens.CardPadY);
            var rows = Layout.Column(inner, Tokens.SpaceXs);
            PanelAtoms.SectionBar(rows.Take(Tokens.SectionTitleHeight), "殖民地概览", "2 项", true);
            PanelAtoms.StatRow(rows.Take(22f), "人口", "7", 7f / 12f, Tokens.Accent);
            PanelAtoms.StatRow(rows.Take(22f), "食物", "1 240");

            // 4) 按钮：RUIF 不提供通用按钮，直接用原版 Widgets
            if (Widgets.ButtonText(foot, "确定")) Close();
        }
    }

    // ===== 教程 §10：完整示例“殖民地状态”窗口 =====
    public class Dialog_ColonyPanel : Window
    {
        private readonly float[] series = { 3f, 6f, 2f, 9f, 4f, 7f };
        private readonly float[] shares = { 0.4f, 0.2f, 0.2f, 0.2f };
        private Vector2 scroll;
        private int tab;
        private float tabX;
        private int segIndex;
        private float amount = 0.5f;

        public override Vector2 InitialSize => new Vector2(780f, 560f);

        public override void DoWindowContents(Rect inRect)
        {
            var col = Layout.Column(inRect, Tokens.Gap);
            Rect head = col.Take(30f);
            Draw.Label(head, "殖民地状态", Tokens.FontValue, Tokens.Text, TextAnchor.MiddleLeft);

            // 可滚动内容区
            Rect view = Layout.BeginScroll(col.Take(400f), 640f, ref scroll);
            var v = Layout.Column(view, Tokens.BlockGap);

            // 顶部选项卡
            NavAtoms.TabBar(v.Take(30f), new[] { "总览", "库存", "人员" }, ref tab, ref tabX);

            // 统计面板
            Rect card = v.Take(120f);
            Draw.Box(card, Tokens.Panel);
            Draw.Border(card, Tokens.Border);
            var c = Layout.Column(Layout.Inset(card, Tokens.CardPadX, Tokens.CardPadY), Tokens.SpaceXs);
            PanelAtoms.SectionBar(c.Take(Tokens.SectionTitleHeight), "今日概览", "3 项", true);
            PanelAtoms.StatRow(c.Take(22f), "人口", "7", 7f / 12f, Tokens.Accent);
            PanelAtoms.StatRow(c.Take(22f), "食物", "1 240", 0.62f);
            PanelAtoms.StatRow(c.Take(22f), "电力", "-320 W");

            // 图表一行
            var ch = Layout.Row(v.Take(80f), Tokens.SpaceMd);
            Charts.Sparkline(ch.Take(160f), series, Tokens.Accent);
            Charts.LineChart(ch.Take(240f), series);
            Charts.DonutChart(ch.Take(90f), shares, 0.55f, "占比");

            // 输入一行（值由窗口持有）
            var ip = Layout.Row(v.Take(28f), Tokens.SpaceSm);
            if (InputAtoms.Segmented(ip.Take(210f), new[] { "日", "周", "月" }, ref segIndex)) { }
            if (InputAtoms.SliderWithValue(ip.Rest, ref amount, 0f, 1f, 0.01f)) { }

            // 反馈条
            FeedbackAtoms.Toast(v.Take(48f), "已记录", "3 条新档案", ToastLevel.Ok, 1f);

            Layout.EndScroll();

            if (Widgets.ButtonText(col.Rest, "关闭")) Close();
        }
    }

    // ===== 教程 §4 布局速成 / §5 配方表里出现、上面未覆盖的调用 =====
    internal static class LayoutExtras
    {
        internal static void SplitAndGrid(Rect rect)
        {
            Layout.Split(rect, 0.3f, Tokens.Gap, out Rect left, out Rect right);
            left.ToString();
            right.ToString();

            var g = Layout.Grid(rect, 3, 90f, Tokens.GridGap);
            while (g.Index < 3)
            {
                Rect cell = g.Next();
                cell.ToString();
            }

            Rect inner = Atoms.SectionFrame(rect, "装备清单");
            inner.ToString();
        }

        internal static void Dropdown(Rect rect, string[] options, int cur, Action<int> onPicked)
        {
            if (InputPlusAtoms.InPlaceDropdown(rect, "选择", options, cur, onPicked)) { }
        }
    }
}
