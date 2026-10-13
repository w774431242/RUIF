using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    // ══════════════════════════════════════════════════════════════════════
    // ⑥ 殖民者表：原版 PawnTable 框架桥
    //
    // ⚠️ 本面同样【不放滚动视口】：PawnTable 自带内部滚动（scrollPosition + 滚动条），
    //   再套一层 BeginScroll 会出现双滚动条与滚轮争抢。
    // ══════════════════════════════════════════════════════════════════════
    internal sealed class BenchPawnTable : IBench
    {
        private static readonly string[] DefNames = { "Assign", "Work", "Restrict", "Animals" };

        private readonly PawnTableHost host = new PawnTableHost();
        private string current = "Assign";

        public bool Scrollable => false;

        public float Height(float width) => 340f;

        public void Render(Rect area)
        {
            var col = Layout.Column(area, Tokens.SpaceSm);

            // ── 工具栏：切换原版 PawnTableDef
            Rect bar = col.Take(30f);
            var row = Layout.Row(bar, 6f);
            Draw.Label(row.Take(80f), "PawnTableDef", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            for (int i = 0; i < DefNames.Length; i++)
            {
                string d = DefNames[i];
                bool on = d == current;
                if (Widgets.ButtonText(row.Take(88f), (on ? "● " : "") + d) && !on)
                {
                    current = d;
                    host.Reset();          // 换 Def ⇒ 丢弃旧表，下次 Ensure 重建
                }
            }
            if (Widgets.ButtonText(row.Take(60f), "刷新")) host.MarkDirty();
            Draw.EllipsisLabel(row.Rest, "列集合 / 表头三态排序 / 行高 / 内部滚动 —— 全部由原版 PawnTable 提供，RUIF 只做宿主与框。",
                GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

            // ── 表本体（填满剩余区域；尺寸变化由宿主自动 SetFixedSize）
            Rect tableRect = col.Rest;
            if (tableRect.height < 80f) return;
            PawnTableAtoms.PawnTableFrame(tableRect, host, current);
        }
    }
}
