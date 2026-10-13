using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace RUIF.Demo
{
    /// <summary>
    /// 宝典 A1/A2 展示件（编号 <b>57–62</b>）。
    ///
    /// <para>来源：<c>docs/02-组件图鉴/终极宝典v1.0-fixed.html</c> 的 <c>d-base</c> 域尾部 6 件 ——
    /// <c>StatChip</c> / <c>DeltaBadge</c> / <c>AvatarPawn</c> / <c>QualityStar</c> / <c>Skeleton</c> / <c>KeyCap</c>。
    /// 宝典自述这 6 件「**不计入 42 原子**」，属展示件。</para>
    ///
    /// <para><b>落地口径（后续批次沿用）</b>：RUIF **不含**这 6 件的内置原子（第 14 轮深挖已取证：全库 0 命中）。
    /// 因此本文件演示的是「**用已有原子组合出该组件**」——这正是宝典作为「设计图鉴」应有的用法。
    /// 故 <c>Api</c> 写**组合范式**而非方法签名，<c>Note</c> 注明基座原子。</para>
    ///
    /// <para>规格同源：几何 / 配色 / 文案全部取自宝典 HTML 对应 <c>.demo</c> 块（L714–L766），未凭直觉编造。
    /// 少数差异按 A/B/C 定责登记（见 <c>QualityStar</c> 的 C 类说明）。</para>
    /// </summary>
    internal sealed partial class BenchAtoms42
    {
        /// <summary>宝典 A1/A2 六件（57–62）。注册入口 —— 已在 <c>BenchAtoms42.cs</c> 的 <c>Build()</c> 里调用。</summary>
        private void BuildBase1()
        {
            var f = new Family
            {
                Title = "宝典 · A1/A2 展示件",
                Sub = "6 件 · 组合范式（RUIF 无内置件，用已有原子组装）· 编号 57–62"
            };

            // ═══════════════════════════════════════════ 57 · StatChip 状态芯片
            f.Add(new Atom
            {
                No = "57", Name = "StatChip 状态芯片",
                Api = "组合范式 · StatChip(icon, label, value, trend)",
                Desc = "图标 + 标签 + 大数值 + 趋势箭头的紧凑指标件，仪表盘原子单元。",
                Note = "基座：Box + Border + 单元矩形五段排版｜图标位用汉字（IMGUI 字库对 emoji 不可靠，接图标族后可替换）",
                BodyH = 34f, MinW = 210f, MinH = 34f,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    StatChip(row, "人", "人口", "12", "▲2", Tokens.Alive);
                    StatChip(row, "战", "威胁", "3.4", "▼0.6", Tokens.Dead);
                    StatChip(row, "物", "物资", "982", "—", Tokens.Dim);
                }
            });

            // ═══════════════════════════════════════════ 58 · DeltaBadge 涨跌徽章
            f.Add(new Atom
            {
                , Name = "StatChip 状态芯片",
                Api = "组合范式 · StatChip(icon, label, value, trend)",
                Desc = "图标 + 标签 + 大数值 + 趋势箭头的紧凑指标件，仪表盘原子单元。",
                Note = "基座：Box + Border + 单元矩形五段排版｜图标位用汉字（IMGUI 字库对 emoji 不可靠，接图标族后可替换）",
                BodyH = 34f, MinW = 210f, MinH = 34f,
                Paint = r =>
                {
                    var row = Layout.Row(r, 8f);
                    StatChip(row, "人", "人口", "12", "▲2", Tokens.Alive);
                    StatChip(row, "战", "威胁", "3.4", "▼0.6", Tokens.Dead);
                    StatChip(row, "物", "物资", "982", "—", Tokens.Dim);
                }
            });

            // ═══════════════════════════════════════════ 58 · DeltaBadge 涨跌徽章
            f.Add(new Atom
            {
                5858No = "59"No = "59", Name = "AvatarPawn 殖民者头像环",
                Api = "组合范式 · AvatarPawn(Texture portrait, int tier)",
                Desc = "肖像 + 职称档位色环（对齐职称档位色）+ 状态角标。环与角标为原生绘制，肖像由消费方提供。",
                Note = "宝典原文：肖像贴图由消费 Mod 提供｜本件无贴图，用首字代肖像位；色环 3 档对齐宝典演示（正高 / 副高 / 阵亡）",
                BodyH = 62f, MinW = 200f, MinH = 62f,
                Interactive = true,
                Paint = r =>
                {
                    var row = Layout.Row(new Rect(r.x, r.y, r.width, 42f), 14f);
                    string[] initials = { "张", "李", "王" };
                    Color[] rings = { Tokens.Alive, Tokens.Info, Tokens.Dead };
                    string[] tiers = { "正高", "副高", "阵亡" };
                    for (int i = 0; i < initials.Length; i++)
                    {
                        Rect unit = row.Take(46f);
                        if (unit.width < 30f) break;
                        var col = Layout.Column(unit, 2f);
                        Avatar(col.Take(34f), initials[i], rings[i], avatarSel == i);
                        Draw.EllipsisLabel(col.Rest, tiers[i], GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleCenter);
                        if (Widgets.ButtonInvisible(unit)) avatarSel = i;
                        HoverFrame(unit);
                    }
                    Draw.EllipsisLabel(new Rect(r.x, r.yMax - LineTiny, r.width, LineTiny),
                        "点选切换 · 环色 = 职称档位（金环正高 / 蓝环副高 / 红环阵亡）", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                }
            });

            // ═══════════════════════════════════════════ 60 · QualityStar 品质星阶
            f.Add(new Atom
            {
                , Name = "AvatarPawn 殖民者头像环",
                Api = "组合范式 · AvatarPawn(Texture portrait, int tier)",
                Desc = "肖像 + 职称档位色环（对齐职称档位色）+ 状态角标。环与角标为原生绘制，肖像由消费方提供。",
                Note = "宝典原文：肖像贴图由消费 Mod 提供｜本件无贴图，用首字代肖像位；色环 3 档对齐宝典演示（正高 / 副高 / 阵亡）",
                BodyH = 62f, MinW = 200f, MinH = 62f,
                Interactive = true,
                Paint = r =>
                {
                    var row = Layout.Row(new Rect(r.x, r.y, r.width, 42f), 14f);
                    string[] initials = { "张", "李", "王" };
                    Color[] rings = { Tokens.Alive, Tokens.Info, Tokens.Dead };
                    string[] tiers = { "正高", "副高", "阵亡" };
                    for (int i = 0; i < initials.Length; i++)
                    {
                        Rect unit = row.Take(46f);
                        if (unit.width < 30f) break;
                        var col = Layout.Column(unit, 2f);
                        Avatar(col.Take(34f), initials[i], rings[i], avatarSel == i);
                        Draw.EllipsisLabel(col.Rest, tiers[i], GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleCenter);
                        if (Widgets.ButtonInvisible(unit)) avatarSel = i;
                        HoverFrame(unit);
                    }
                    Draw.EllipsisLabel(new Rect(r.x, r.yMax - LineTiny, r.width, LineTiny),
                        "点选切换 · 环色 = 职称档位（金环正高 / 蓝环副高 / 红环阵亡）", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                }
            });

            // ═══════════════════════════════════════════ 60 · QualityStar 品质星阶
            f.Add(new Atom
            {
                6060No = "61"No = "61", Name = "Skeleton 骨架屏",
                Api = "组合范式 · Skeleton(SkeletonLayout)",
                Desc = "数据未就绪时的呼吸占位（透明度 0.5~1.0 正弦插值，IMGUI 每帧重绘天然支持）。替代「空白等待」。",
                Note = "点一下切「加载完成」：同一矩形内换成真实内容 —— 空态与就绪态共用一次布局",
                BodyH = 96f, MinW = 190f, MinH = 96f,
                Interactive = true,
                Paint = r =>
                {
                    if (Widgets.ButtonInvisible(r)) skelLoaded = !skelLoaded;
                    HoverFrame(r);
                    if (skelLoaded)
                    {
                        var col = Layout.Column(r, 4f);
                        var head = Layout.Row(col.Take(LineSmall + 6f), 8f);
                        Avatar(head.Take(28f), "蓝", Tokens.Accent, false);
                        Draw.EllipsisLabel(head.Rest, Names[0], GameFont.Small, Tokens.Text, TextAnchor.MiddleLeft);
                        Draw.EllipsisLabel(col.Take(LineTiny), "心情 72% · 健康 94% · 无病症", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                        Shapes.GradientRule(col.Take(LineTiny), Tokens.Accent, 10);
                        Draw.EllipsisLabel(col.Take(LineTiny), "数据已就绪（再点回到骨架态）", GameFont.Tiny, Tokens.Alive, TextAnchor.MiddleLeft);
                        return;
                    }
                    // 0.5~1.0 正弦呼吸：骨架期的「活着」表达，避免看起来像卡死
                    float breathe = Mathf.Lerp(0.5f, 1f, (Mathf.Sin(t * 2.2f) + 1f) * 0.5f);
                    var bars = Layout.Column(r, 4f);
                    var head2 = Layout.Row(bars.Take(34f), 9f);
                    Draw.Box(head2.Take(34f), Draw.WithAlpha(Tokens.BorderSoft, breathe));   // 头像位（方形降级，登记见 Note）
                    var hb = Layout.Column(head2.Rest, 6f);
                    Rect b1 = hb.Take(9f);
                    Draw.Box(new Rect(b1.x, b1.y, b1.width * 0.60f, b1.height), Draw.WithAlpha(Tokens.BorderSoft, breathe));
                    Rect b2 = hb.Take(7f);
                    Draw.Box(new Rect(b2.x, b2.y, b2.width * 0.85f, b2.height), Draw.WithAlpha(Tokens.BorderSoft, breathe * 0.7f));
                    Draw.Box(bars.Take(8f), Draw.WithAlpha(Tokens.BorderSoft, breathe * 0.8f));
                    Draw.EllipsisLabel(new Rect(r.x, r.yMax - LineTiny, r.width, LineTiny),
                        "加载中… 骨架占位（点一下切到就绪态）", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ═══════════════════════════════════════════ 62 · KeyCap 键位帽
            f.Add(new Atom
            {
                , Name = "Skeleton 骨架屏",
                Api = "组合范式 · Skeleton(SkeletonLayout)",
                Desc = "数据未就绪时的呼吸占位（透明度 0.5~1.0 正弦插值，IMGUI 每帧重绘天然支持）。替代「空白等待」。",
                Note = "点一下切「加载完成」：同一矩形内换成真实内容 —— 空态与就绪态共用一次布局",
                BodyH = 96f, MinW = 190f, MinH = 96f,
                Interactive = true,
                Paint = r =>
                {
                    if (Widgets.ButtonInvisible(r)) skelLoaded = !skelLoaded;
                    HoverFrame(r);
                    if (skelLoaded)
                    {
                        var col = Layout.Column(r, 4f);
                        var head = Layout.Row(col.Take(LineSmall + 6f), 8f);
                        Avatar(head.Take(28f), "蓝", Tokens.Accent, false);
                        Draw.EllipsisLabel(head.Rest, Names[0], GameFont.Small, Tokens.Text, TextAnchor.MiddleLeft);
                        Draw.EllipsisLabel(col.Take(LineTiny), "心情 72% · 健康 94% · 无病症", GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
                        Shapes.GradientRule(col.Take(LineTiny), Tokens.Accent, 10);
                        Draw.EllipsisLabel(col.Take(LineTiny), "数据已就绪（再点回到骨架态）", GameFont.Tiny, Tokens.Alive, TextAnchor.MiddleLeft);
                        return;
                    }
                    // 0.5~1.0 正弦呼吸：骨架期的「活着」表达，避免看起来像卡死
                    float breathe = Mathf.Lerp(0.5f, 1f, (Mathf.Sin(t * 2.2f) + 1f) * 0.5f);
                    var bars = Layout.Column(r, 4f);
                    var head2 = Layout.Row(bars.Take(34f), 9f);
                    Draw.Box(head2.Take(34f), Draw.WithAlpha(Tokens.BorderSoft, breathe));   // 头像位（方形降级，登记见 Note）
                    var hb = Layout.Column(head2.Rest, 6f);
                    Rect b1 = hb.Take(9f);
                    Draw.Box(new Rect(b1.x, b1.y, b1.width * 0.60f, b1.height), Draw.WithAlpha(Tokens.BorderSoft, breathe));
                    Rect b2 = hb.Take(7f);
                    Draw.Box(new Rect(b2.x, b2.y, b2.width * 0.85f, b2.height), Draw.WithAlpha(Tokens.BorderSoft, breathe * 0.7f));
                    Draw.Box(bars.Take(8f), Draw.WithAlpha(Tokens.BorderSoft, breathe * 0.8f));
                    Draw.EllipsisLabel(new Rect(r.x, r.yMax - LineTiny, r.width, LineTiny),
                        "加载中… 骨架占位（点一下切到就绪态）", GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ═══════════════════════════════════════════ 62 · KeyCap 键位帽
            f.Add(new Atom
            {
                6262