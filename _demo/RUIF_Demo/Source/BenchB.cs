using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    // ══════════════════════════════════════════════════════════════════════
    // ③ 拖拽重排
    //
    // ⚠️ 本面刻意【不放进滚动视口】：原版 ReorderableWidget 用
    //   UI.GUIToScreenPoint 记录行矩形用于插入位判定，而滚动视图会平移
    //   Event.current.mousePosition 的坐标系 —— 两者混用会让插入位偏移。
    //   这正是"桥接原版成件时必须遵守其使用前提"的一个实例。
    // ══════════════════════════════════════════════════════════════════════
    internal sealed class BenchReorder : IBench
    {
        private readonly List<string> items = new List<string>
        {
            "建造", "烹饪", "制作", "医生", "灭火", "种植", "驯兽", "搬运"
        };

        private const float RowH = 26f;
        private int groupId;
        private string lastMsg = "（尚未拖动；按住任意行向下/上拖）";
        private int lastFrom = -1, lastTo = -1;

        public bool Scrollable => false;

        public float Height(float width) => items.Count * RowH + 300f;

        public void Render(Rect area)
        {
            var col = Layout.Column(area, Tokens.SpaceMd);

            // ── 说明
            var hcol = Layout.Column(Atoms.SectionFrame(col.Take(114f),
                "列表拖拽重排（ReorderAtoms · 桥 Verse.ReorderableWidget）"), 3f);
            Draw.EllipsisLabel(hcol.Take(Measure.RowHeight(GameFont.Tiny)), "原版白送：5px 起拖阈值 · 起拖/放下音效 · 2px 白色插入线 · 被拖行高亮。", GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.EllipsisLabel(hcol.Take(Measure.RowHeight(GameFont.Tiny)), "组 id 由本面持有（ref int）—— 非 Repaint 帧 NewGroup 返回 -1，直接用它登记会导致拖拽永远起不来。", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            Draw.EllipsisLabel(hcol.Take(Measure.RowHeight(GameFont.Tiny)), "行数变化会触发原版引擎的 StopDragging()（reorderables.Count 变化时自动中断）。", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

            // ── 列表本体
            Rect lb = Atoms.SectionFrame(col.Take(items.Count * RowH + 60f), "工作类型（拖动重排）");
            var listArea = new Rect(lb.x, lb.y, lb.width, items.Count * RowH);

            var reorderCand = ReorderCandFmt.List(items.Count, RowH, DrawRow, OnReorder);
            ReorderCandAtoms.List(listArea, reorderCand, ref groupId);

            // ── 回显
            var fcol = Layout.Column(Atoms.SectionFrame(col.Take(98f), "回调语义回显（ReorderFmt）"), 3f);
            Draw.EllipsisLabel(fcol.Take(Measure.RowHeight(GameFont.Tiny)), lastMsg, GameFont.Tiny, Tokens.Text, TextAnchor.MiddleLeft);
            Draw.EllipsisLabel(fcol.Take(Measure.RowHeight(GameFont.Tiny)),
                lastFrom < 0 ? "LandingIndex / WillChange：等待首次拖动"
                             : "from=" + lastFrom + " insert=" + lastTo
                               + "　LandingIndex=" + ReorderFmt.LandingIndex(lastFrom, lastTo)
                               + "　WillChange=" + ReorderFmt.WillChange(lastFrom, lastTo)
                               + "（insert == from 或 from+1 时原版不回调，就是插回原位）",
                GameFont.Tiny, Tokens.Info, TextAnchor.MiddleLeft);
            Draw.Label(fcol.Take(Measure.RowHeight(GameFont.Tiny)), "顺序：" + string.Join(" › ", items.ToArray()), GameFont.Tiny, Tokens.Alive, TextAnchor.MiddleLeft);
        }

        private void DrawRow(int index, Rect row, bool dragging)
        {
            Draw.Box(row, dragging ? Tokens.CardHover : Tokens.Card);
            Draw.Border(row, dragging ? Tokens.Accent : Tokens.BorderSoft);
            var r = Layout.Row(row, 8f);
            ReorderCandAtoms.Grip(r.Take(RowH), ReorderCandFmt.Grip(dragging));
            Draw.Label(r.Take(34f), (index + 1).ToString("00"), GameFont.Tiny,
                dragging ? Tokens.Accent : Tokens.Dim, TextAnchor.MiddleLeft);
            Draw.Label(r.Rest, items[index], GameFont.Small, Tokens.Text, TextAnchor.MiddleLeft);
            if (dragging)
            {
                Draw.Label(new Rect(row.xMax - 110f, row.y, 104f, row.height), "正在拖动", GameFont.Tiny, Tokens.Accent, TextAnchor.MiddleRight);
            }
        }

        private void OnReorder(int from, int to)
        {
            lastFrom = from;
            lastTo = to;
            int land = ReorderFmt.LandingIndex(from, to);
            ReorderFmt.Move(items, from, to);
            lastMsg = "回调 Move(from=" + from + ", insert=" + to + ") → 落到下标 " + land
                      + "　（原版就给了 insert 语义，不做 -1 修正会整体偏一位）";
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // ④ 原子画廊：手选一批真实签名调用，用于回归"框架没被改坏"
    // ══════════════════════════════════════════════════════════════════════
    internal sealed class BenchGallery : IBench
    {
        private readonly GalleryExtra extra = new GalleryExtra();
        private float t;

        public bool Scrollable => true;

        public float Height(float width) => 1240f;

        public void Render(Rect area)
        {
            t += Time.deltaTime;
            var col = Layout.Column(area, Tokens.SpaceMd);

            // ── 文本原子
            var c1 = Layout.Column(Atoms.SectionFrame(col.Take(122f), "文本原子 TextAtoms"), 4f);
            var urow = Layout.Row(c1.Take(Measure.RowHeight(GameFont.Small)), Tokens.SpaceLg);
            TextCandAtoms.UnitLabel(urow.Take(150f), TextCandFmt.UnitLabel("12 480", "件", GameFont.Small, Tokens.Accent));
            TextCandAtoms.UnitLabel(urow.Take(150f), TextCandFmt.UnitLabel("469", "W", GameFont.Small, Tokens.Info));
            TextCandAtoms.UnitLabel(urow.Take(150f), TextCandFmt.UnitLabel("86", "%", GameFont.Small, Tokens.Alive));
            TextCandAtoms.SubtitleLine(c1.Take(34f), TextCandFmt.SubtitleLine("总工程师 · 部落工匠", "职称档位 T4 · 背景：部落猎手"));
            TextCandAtoms.Typewriter(c1.Take(26f), TextCandFmt.Typewriter("第一块太阳能板接通的瞬间，电池指示灯亮了。", 26f, GameFont.Small, Tokens.Muted, TextAnchor.MiddleLeft));

            // ── 状态原子
            var c2 = Layout.Column(Atoms.SectionFrame(col.Take(140f), "状态原子 StatusAtoms"), 8f);
            var dotRow = Layout.Row(c2.Take(20f), Tokens.SpaceLg);
            StatusCandAtoms.PulseDot(dotRow.Take(14f), StatusCandFmt.PulseDot(Tokens.Alive, t));
            StatusCandAtoms.PulseDot(dotRow.Take(14f), StatusCandFmt.PulseDot(Tokens.Warn, t + 0.55f));
            StatusCandAtoms.PulseDot(dotRow.Take(14f), StatusCandFmt.PulseDot(Tokens.Dead, t + 1.1f));
            Draw.Label(dotRow.Take(200f), "PulseDot ×3（错相位呼吸）", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            StatusCandAtoms.Spinner(dotRow.Take(24f), StatusCandFmt.Spinner(t, Tokens.Accent));
            Draw.Label(dotRow.Rest, "Spinner", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

            var glyphRow = Layout.Row(c2.Take(Measure.RowHeight(GameFont.Small)), Tokens.SpaceMd);
            StatusKind[] kinds = { StatusKind.Ok, StatusKind.Info, StatusKind.Warn, StatusKind.Error, StatusKind.Paused };
            for (int i = 0; i < kinds.Length; i++)
            {
                Rect gr = glyphRow.Take(20f);
                StatusCandAtoms.Glyph(gr, StatusCandFmt.Glyph(kinds[i]));
                Draw.Label(new Rect(gr.x + 20f, gr.y, 44f, gr.height), kinds[i].ToString(), GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
            }
            var caretRow = Layout.Row(c2.Take(20f), Tokens.SpaceLg);
            StatusAtoms.Caret(caretRow.Take(Measure.RowHeight(GameFont.Tiny)), true);
            Draw.Label(caretRow.Take(80f), "Caret 展开", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            StatusAtoms.Caret(caretRow.Take(Measure.RowHeight(GameFont.Tiny)), false);
            Draw.Label(caretRow.Take(80f), "Caret 折叠", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            Rect fadeHost = caretRow.Rest;
            StatusCandAtoms.ScrollFadeHint(new Rect(fadeHost.x, fadeHost.y, 120f, 20f), StatusCandFmt.ScrollFadeHint(true));
            Draw.Label(new Rect(fadeHost.x + 122f, fadeHost.y, 140f, 20f), "ScrollFadeHint", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

            // ── 进度与容量
            var c3 = Layout.Column(Atoms.SectionFrame(col.Take(196f), "进度与容量 ProgressAtoms"), 8f);
            float wipe01 = Mathf.Repeat(t * 0.18f, 1.06f);
            ProgressCandAtoms.FillWipe(c3.Take(42f), ProgressCandFmt.FillWipe(Mathf.Clamp01(wipe01), Tokens.Accent, 10, 0.35f));
            var capRow = Layout.Row(c3.Take(18f), Tokens.SpaceMd);
            ProgressCandAtoms.CapacityBar(capRow.Take(170f), ProgressCandFmt.CapacityBar(780f, 1000f, Tokens.Info));
            ProgressCandAtoms.CapacityBar(capRow.Take(170f), ProgressCandFmt.CapacityBar(1180f, 1000f, Tokens.Dead));
            Draw.Label(capRow.Rest, "CapacityBar 正常 / 超载", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            var ringRow = Layout.Row(c3.Take(64f), Tokens.SpaceMd);
            float ring01 = Mathf.Repeat(t * 0.22f, 1f);
            ProgressCandAtoms.Ring(ringRow.Take(60f), ProgressCandFmt.Ring(ring01, Tokens.BorderSoft, Tokens.Alive));
            ProgressCandAtoms.Ring(ringRow.Take(60f), ProgressCandFmt.Ring(0.72f, null, Tokens.Accent, "72%"));
            ProgressCandAtoms.ZoomIndicator(ringRow.Rest, ProgressCandFmt.ZoomIndicator(0.86f, 1f));

            // ── 布局原子
            var c4 = Layout.Column(Atoms.SectionFrame(col.Take(160f), "布局原子 Atoms / Layout"), 8f);
            var lrow = Layout.Row(c4.Take(56f), Tokens.SpaceMd);
            Rect aspect = Atoms.AspectBox(lrow.Take(96f), 16f / 9f);
            Draw.Box(aspect, Tokens.PanelRaised);
            Draw.Border(aspect, Tokens.BorderSoft);
            Draw.Label(aspect, "16:9", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleCenter);

            Rect stage = lrow.Rest;
            stage = new Rect(stage.x, stage.y, Mathf.Min(320f, stage.width), stage.height);
            Draw.Border(stage, Tokens.BorderHair);
            Anchor9[] anchors = { Anchor9.TopLeft, Anchor9.TopRight, Anchor9.BottomLeft, Anchor9.BottomRight };
            for (int i = 0; i < anchors.Length; i++)
            {
                Rect a = Atoms.Anchor(stage, anchors[i], new Vector2(34f, 16f), 4f);
                Draw.Box(a, Tokens.Accent);
                Draw.Label(a, anchors[i].ToString().Substring(0, 2), GameFont.Tiny, Tokens.Window, TextAnchor.MiddleCenter);
            }
            Draw.Label(new Rect(stage.x, stage.yMax + 2f, stage.width, 16f), "Atoms.Anchor 四角锚定", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);

            var gridRow = Layout.Row(c4.Take(30f), 0f);
            Draw.Label(gridRow.Rest, "Layout.Grid / Row / Column / Split / Inset 见各验证面布局本身（本台全部由它们搭出）", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);

            // ── 补充三族（数据展示 / 面板与几何 / 导航与杂项）
            extra.DataSection(col.Take(GalleryExtra.DataH));
            extra.PanelGeoSection(col.Take(GalleryExtra.PanelGeoH));
            extra.NavMiscSection(col.Take(GalleryExtra.NavMiscH));
        }
    }
}
