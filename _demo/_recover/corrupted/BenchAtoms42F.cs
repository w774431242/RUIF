using System;
using UnityEngine;
using Verse;

namespace RUIF.Demo
{
    /// <summary>
    /// 宝典落地 · W-001 起（d-input 收尾 + d-data 数据展示）。
    ///
    /// <para>来源：<c>docs/02-组件图鉴/终极宝典v1.0-fixed.html</c>。规格（几何 / 配色 / 文案 / 行为）
    /// 均从对应 <c>.demo</c> 块与原子反编译实现反读，**不凭直觉编**。</para>
    ///
    /// <para>落地口径（账本 §2.5）：先用 <c>ruif_api.ps1</c> 查 RUIF 有无该原子 ——
    /// 有 ⇒ 真名 + 实测契约；无 ⇒ 组合范式。</para>
    /// </summary>
    internal sealed partial class BenchAtoms42
    {
        /// <summary>W-001 · d-input 收尾。注册入口 —— 已在 <c>BenchAtoms42.cs</c> 的 <c>Build()</c> 里调用。</summary>
        private void BuildDInput()
        {
            var f = new Family
            {
                Title = "宝典 · d-input 输入",
                Sub = "1 件 · InputAtoms.SearchField（防抖搜索框）· 编号 63"
            };

            // ═══════════════════════════════════════════ 63 · SearchField 搜索框
            f.Add(new Atom
            {
                No = "63", Name = "SearchField 搜索框",
                Api = "InputAtoms.SearchField(Rect, ref query, ref buffer, placeholder, ref lastInputTime, delay) → bool",
                Desc = "包装 Widgets.TextField：buffer 即时回显，query 防抖 200ms 后才更新；返回 query 是否刚更新。",
                Note = "⚠ 宝典 sig 写 SearchField(Rect, ref query, bool clearable)，RUIF 实现无 clearable、不画清空钮 ⇒ 清空钮由调用方绘制（本件已补）｜placeholder 由原子自画，勿重复",
                BodyH = 68f, MinW = 220f, MinH = 68f,
                Interactive = true,
                Paint = r =>
                {
                    var col = Layout.Column(r, 2f);

                    Rect lineRect = col.Take(CtrlTiny);
                    var line = Layout.Row(lineRect, 4f);
                    float clrW = 24f;
                    if (line.Rest.width < clrW + 44f) { Draw.EllipsisLabel(lineRect, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
                    Rect fld = line.Take(line.Rest.width - clrW - 4f);
                    Rect clr = new Rect(fld.xMax + 4f, fld.y, clrW, fld.height);

                    InputAtoms.SearchField(fld, ref sfQuery, ref sfBuffer, "搜索殖民者…", ref sfLast);

                    // 清空钮：自带三态。⛔ 不用 HoverFrame —— 它无条件描 accent，会让禁用态也显示悬停 ⇒ 语义错
                    bool canClear = !string.IsNullOrEmpty(sfBuffer);
                    bool over = canClear && Mouse.IsOver(clr);
                    Color cc = !canClear ? Tokens.Dim : over ? Tokens.Accent : Tokens.Muted;
                    Draw.Box(clr, Tokens.Card);
                    Draw.Border(clr, cc);
                    Draw.Label(clr, "×", GameFont.Small, cc, TextAnchor.MiddleCenter);
                    if (canClear && Widgets.ButtonInvisible(clr)) { sfBuffer = ""; sfQuery = ""; }

                    Draw.EllipsisLabel(col.Take(LineTiny),
                        "实时 buffer：" + (sfBuffer.Length == 0 ? "（空）" : "「" + sfBuffer + "」") +
                        "　防抖 query：" + (sfQuery.Length == 0 ? "（空）" : "「" + sfQuery + "」"),
                        GameFont.Tiny, sfBuffer != sfQuery ? Tokens.Warn : Tokens.Dim, TextAnchor.MiddleLeft);

                    int hits = sfQuery.Length == 0 ? Names.Length : 0;
                    string first = "";
                    for (int i = 0; i < Names.Length; i++)
                    {
                        if (sfQuery.Length != 0 && Names[i].IndexOf(sfQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            hits++;
                            if (first.Length < 30) first += (first.Length > 0 ? " · " : "") + Names[i];
                        }
                    }
                    Rect resRect = col.Take(LineTiny);
                    Draw.EllipsisLabel(resRect,
                        "命中 " + hits + " / " + Names.Length + (hits > 0 && sfQuery.Length > 0 ? "：" + first : ""),
                        GameFont.Tiny, hits > 0 ? Tokens.Alive : Tokens.Dead, TextAnchor.MiddleLeft);
                    if (sfQuery.Length > 0 && Widgets.ButtonInvisible(resRect)) { sfBuffer = ""; sfQuery = ""; }
                }
            });

            families.Add(f);
        }

        /// <summary>W-002 / W-003 / W-004 · d-data 前 3 件。已在 <c>Build()</c> 里调用。</summary>
        private void BuildDData()
        {
            var f = new Family
            {
                Title = "宝典 · d-data 数据展示",
                Sub = "3 件 · DataAtoms.PawnCard / SkillBars / NeedsPanel（真名落地）· 编号 64–66"
            };

            // ═══════════════════════════════════════════ 64 · PawnCard 殖民者卡（W-002）
            f.Add(new Atom
            {
                , Name = "SearchField 搜索框",
                Api = "InputAtoms.SearchField(Rect, ref query, ref buffer, placeholder, ref lastInputTime, delay) → bool",
                Desc = "包装 Widgets.TextField：buffer 即时回显，query 防抖 200ms 后才更新；返回 query 是否刚更新。",
                Note = "⚠ 宝典 sig 写 SearchField(Rect, ref query, bool clearable)，RUIF 实现无 clearable、不画清空钮 ⇒ 清空钮由调用方绘制（本件已补）｜placeholder 由原子自画，勿重复",
                BodyH = 68f, MinW = 220f, MinH = 68f,
                Interactive = true,
                Paint = r =>
                {
                    var col = Layout.Column(r, 2f);

                    Rect lineRect = col.Take(CtrlTiny);
                    var line = Layout.Row(lineRect, 4f);
                    float clrW = 24f;
                    if (line.Rest.width < clrW + 44f) { Draw.EllipsisLabel(lineRect, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
                    Rect fld = line.Take(line.Rest.width - clrW - 4f);
                    Rect clr = new Rect(fld.xMax + 4f, fld.y, clrW, fld.height);

                    InputAtoms.SearchField(fld, ref sfQuery, ref sfBuffer, "搜索殖民者…", ref sfLast);

                    // 清空钮：自带三态。⛔ 不用 HoverFrame —— 它无条件描 accent，会让禁用态也显示悬停 ⇒ 语义错
                    bool canClear = !string.IsNullOrEmpty(sfBuffer);
                    bool over = canClear && Mouse.IsOver(clr);
                    Color cc = !canClear ? Tokens.Dim : over ? Tokens.Accent : Tokens.Muted;
                    Draw.Box(clr, Tokens.Card);
                    Draw.Border(clr, cc);
                    Draw.Label(clr, "×", GameFont.Small, cc, TextAnchor.MiddleCenter);
                    if (canClear && Widgets.ButtonInvisible(clr)) { sfBuffer = ""; sfQuery = ""; }

                    Draw.EllipsisLabel(col.Take(LineTiny),
                        "实时 buffer：" + (sfBuffer.Length == 0 ? "（空）" : "「" + sfBuffer + "」") +
                        "　防抖 query：" + (sfQuery.Length == 0 ? "（空）" : "「" + sfQuery + "」"),
                        GameFont.Tiny, sfBuffer != sfQuery ? Tokens.Warn : Tokens.Dim, TextAnchor.MiddleLeft);

                    int hits = sfQuery.Length == 0 ? Names.Length : 0;
                    string first = "";
                    for (int i = 0; i < Names.Length; i++)
                    {
                        if (sfQuery.Length != 0 && Names[i].IndexOf(sfQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            hits++;
                            if (first.Length < 30) first += (first.Length > 0 ? " · " : "") + Names[i];
                        }
                    }
                    Rect resRect = col.Take(LineTiny);
                    Draw.EllipsisLabel(resRect,
                        "命中 " + hits + " / " + Names.Length + (hits > 0 && sfQuery.Length > 0 ? "：" + first : ""),
                        GameFont.Tiny, hits > 0 ? Tokens.Alive : Tokens.Dead, TextAnchor.MiddleLeft);
                    if (sfQuery.Length > 0 && Widgets.ButtonInvisible(resRect)) { sfBuffer = ""; sfQuery = ""; }
                }
            });

            families.Add(f);
        }

        /// <summary>W-002 / W-003 / W-004 · d-data 前 3 件。已在 <c>Build()</c> 里调用。</summary>
        private void BuildDData()
        {
            var f = new Family
            {
                Title = "宝典 · d-data 数据展示",
                Sub = "3 件 · DataAtoms.PawnCard / SkillBars / NeedsPanel（真名落地）· 编号 64–66"
            };

            // ═══════════════════════════════════════════ 64 · PawnCard 殖民者卡（W-002）
            f.Add(new Atom
            {
                6464No = "65"No = "65", Name = "SkillBars 技能条组",
                Api = "DataAtoms.SkillBars(Rect, string[] names, float[] levels, maxLevel = 20f, columns = 2) → void",
                Desc = "技能条组：深度 0~20 语义色阶，行内叠印等级数字。",
                Note = "⚠ 宝典 desc 声称「10/20 刻度线 + 燃烧标识 + 无兴趣置灰」，**实现里没有**（只有条 + 数值）——登记为 A 类差异｜点条切 columns 1↔2，演示列数自适应｜数据为宝典预览样例（5 项，columns=2 时第 6 格自然留空）",
                BodyH = 82f, MinW = 230f, MinH = 82f,
                Interactive = true,
                Paint = r =>
                {
                    int cols = sbOneCol ? 1 : 2;
                    float h = Mathf.Ceil(SkillNames.Length / (float)cols) * (LineTiny + 2f);
                    Rect bars = new Rect(r.x, r.y, r.width, h);
                    if (bars.width < 120f) { Draw.EllipsisLabel(bars, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
                    DataAtoms.SkillBars(bars, SkillNames, SkillBase, 20f, cols);
                    if (Widgets.ButtonInvisible(bars)) sbOneCol = !sbOneCol;
                    HoverFrame(bars);
                    Draw.EllipsisLabel(new Rect(r.x, bars.yMax + 2f, r.width, LineTiny),
                        "columns = " + cols + " · maxLevel = 20｜点条切换列数",
                        GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ═══════════════════════════════════════════ 66 · NeedsPanel 需求面板（W-004）
            f.Add(new Atom
            {
                , Name = "SkillBars 技能条组",
                Api = "DataAtoms.SkillBars(Rect, string[] names, float[] levels, maxLevel = 20f, columns = 2) → void",
                Desc = "技能条组：深度 0~20 语义色阶，行内叠印等级数字。",
                Note = "⚠ 宝典 desc 声称「10/20 刻度线 + 燃烧标识 + 无兴趣置灰」，**实现里没有**（只有条 + 数值）——登记为 A 类差异｜点条切 columns 1↔2，演示列数自适应｜数据为宝典预览样例（5 项，columns=2 时第 6 格自然留空）",
                BodyH = 82f, MinW = 230f, MinH = 82f,
                Interactive = true,
                Paint = r =>
                {
                    int cols = sbOneCol ? 1 : 2;
                    float h = Mathf.Ceil(SkillNames.Length / (float)cols) * (LineTiny + 2f);
                    Rect bars = new Rect(r.x, r.y, r.width, h);
                    if (bars.width < 120f) { Draw.EllipsisLabel(bars, "空间不足（D6）", GameFont.Tiny, Tokens.Dead, TextAnchor.MiddleLeft); return; }
                    DataAtoms.SkillBars(bars, SkillNames, SkillBase, 20f, cols);
                    if (Widgets.ButtonInvisible(bars)) sbOneCol = !sbOneCol;
                    HoverFrame(bars);
                    Draw.EllipsisLabel(new Rect(r.x, bars.yMax + 2f, r.width, LineTiny),
                        "columns = " + cols + " · maxLevel = 20｜点条切换列数",
                        GameFont.Tiny, Tokens.Dim, TextAnchor.MiddleLeft);
                }
            });

            // ═══════════════════════════════════════════ 66 · NeedsPanel 需求面板（W-004）
            f.Add(new Atom
            {
                6666