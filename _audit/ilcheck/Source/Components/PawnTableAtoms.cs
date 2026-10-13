using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace RUIF
{
    /// <summary>
    /// A0+ · 表格族的<b>原版框架桥</b>（第二轮取证 §1.2 ❌→✅ 的落地件）。
    ///
    /// <para><b>为什么是"桥"而不是"重写"</b>：原版 <c>RimWorld.PawnTable</c> 是一个完整表格引擎 ——
    /// <c>PawnTableOnGUI(position)</c> 一行出表，内含：Def 驱动的列集合（<c>def.columns</c> 过滤
    /// <c>Worker.VisibleCurrently</c>）、列宽协商（<c>GetMinWidth/GetMaxWidth/GetOptimalWidth</c>）、
    /// 逐列行高、内部滚动、以及<b>内置三态排序</b>（点表头 desc → asc → 清除，带
    /// <c>SoundDefOf.Tick_High/Tick_Low</c> 音效与排序图标）。自研一遍只会更差。</para>
    ///
    /// <para><b>⚠️ 硬边界（实测）</b>：<c>PawnTable</c> 的行类型<b>硬编码为 <c>Pawn</c></b>
    /// （构造签名 <c>PawnTable(PawnTableDef, Func&lt;IEnumerable&lt;Pawn&gt;&gt;, int, int)</c>、
    /// <c>PawnsListForReading</c>）。因此本原子<b>只服务以 Pawn 为行的表</b>；非 Pawn 数据表只能借它的
    /// 范式（Def 列 + Worker 单元 + 宽度协商 + 三态排序）另建，不能套用本类。</para>
    ///
    /// <para><b>状态归属</b>：表实例由调用方的 <see cref="PawnTableHost"/> 持有（框架自身零状态）。
    /// 宿主内部只在"Def 变了 / 尺寸变了"时重建或 <c>SetFixedSize</c>，
    /// 避免每帧 <c>SetDirty()</c> 触发 <c>RecacheIfDirty()</c> 全表重算。</para>
    ///
    /// <para><b>⚠️ 不要嵌进 RUIF 的滚动视口</b>：<c>PawnTable</c> 自带 <c>scrollPosition</c> 与内部
    /// 滚动条，再套一层 <see cref="Layout.BeginScroll"/> 会出现双滚动条与滚轮争抢。</para>
    /// </summary>
    public sealed class PawnTableHost
    {
        private PawnTable table;
        private string defName;
        private float width = -1f;
        private float height = -1f;
        private bool createFailed;

        /// <summary>当前表实例（未创建时为 null）。</summary>
        public PawnTable Table => table;

        /// <summary>当前绑定的 PawnTableDef 名（未绑定时为 null）。</summary>
        public string TableDefName => defName;

        /// <summary>创建失败原因（Def 缺失 / 地图缺失时为非空，供 UI 显示）。</summary>
        public string LastError { get; private set; }

        /// <summary>
        /// 按需建表 / 改尺寸。返回可绘制的表；返回 null 时 <see cref="LastError"/> 说明原因。
        /// </summary>
        public PawnTable Ensure(string tableDefName, Func<IEnumerable<Pawn>> pawnsGetter, float w, float h)
        {
            if (tableDefName.NullOrEmpty() || pawnsGetter == null || w <= 1f || h <= 1f) return null;

            if (table == null || defName != tableDefName)
            {
                createFailed = false;
                LastError = null;
                PawnTableDef def = DefDatabase<PawnTableDef>.GetNamedSilentFail(tableDefName);
                if (def == null)
                {
                    LastError = "找不到 PawnTableDef「" + tableDefName + "」（Def 名拼写或所属 Mod 未启用）";
                    createFailed = true;
                    return null;
                }
                // 用基类 PawnTable 而非 def.workerClass 子类：行集合由调用方的 getter 提供，
                // 子类（如 PawnTable_PlayerPawns）的额外过滤会与调用方意图冲突。
                table = new PawnTable(def, pawnsGetter, Mathf.RoundToInt(w), Mathf.RoundToInt(h));
                defName = tableDefName;
                width = w;
                height = h;
                table.SetFixedSize(new Vector2(w, h));
                return table;
            }

            if (!Mathf.Approximately(width, w) || !Mathf.Approximately(height, h))
            {
                width = w;
                height = h;
                table.SetFixedSize(new Vector2(w, h));   // 内部会 SetDirty()，故只在尺寸变化时调用
            }
            return table;
        }

        /// <summary>标记数据脏（pawn 列表/数值变化后调用，下一帧重算）。</summary>
        public void MarkDirty()
        {
            if (table != null) table.SetDirty();
        }

        /// <summary>丢弃当前表（下次 <see cref="Ensure"/> 重建）。</summary>
        public void Reset()
        {
            table = null;
            defName = null;
            width = height = -1f;
            createFailed = false;
            LastError = null;
        }

        /// <summary>是否处于创建失败态（供 UI 决定是否重试）。</summary>
        public bool CreateFailed => createFailed;
    }

    /// <summary>表格桥的绘制原子。</summary>
    public static class PawnTableAtoms
    {
        /// <summary>默认行集合：当前地图的自由殖民者（无地图时返回空）。</summary>
        public static IEnumerable<Pawn> CurrentMapColonists()
        {
            Map m = Find.CurrentMap;
            return m == null ? (IEnumerable<Pawn>)new Pawn[0] : m.mapPawns.FreeColonists;
        }

        /// <summary>
        /// 画一张原版 <c>PawnTable</c>（含内置表头排序 / 内部滚动 / 悬停高亮）。
        /// </summary>
        /// <param name="rect">整块区域（表会填满它；尺寸变化时宿主自动重设）。</param>
        /// <param name="host">调用方持有的表宿主（持久状态）。</param>
        /// <param name="tableDefName">原版 Def 名，如 <c>"Assign"</c> / <c>"Work"</c> / <c>"Restrict"</c> / <c>"Animals"</c>。</param>
        /// <param name="pawnsGetter">行集合提供者；null ⇒ 用 <see cref="CurrentMapColonists"/>。</param>
        /// <param name="showStatusLine">是否在底部画一行排序状态回显（验证排序是否真的生效）。</param>
        /// <returns>表是否成功绘制。</returns>
        public static bool PawnTableFrame(Rect rect, PawnTableHost host, string tableDefName,
                                          Func<IEnumerable<Pawn>> pawnsGetter = null, bool showStatusLine = true)
        {
            if (host == null || rect.width <= 2f || rect.height <= 2f) return false;

            Draw.Box(rect, Tokens.Panel);
            Draw.Border(rect, Tokens.BorderSoft);

            float footH = showStatusLine ? 18f : 0f;
            Rect body = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f - footH);

            PawnTable table = host.Ensure(tableDefName, pawnsGetter ?? CurrentMapColonists, body.width, body.height);
            if (table == null)
            {
                Draw.Label(body, host.LastError ?? "（无可用表格）", GameFont.Small, Tokens.Muted, TextAnchor.MiddleCenter);
                return false;
            }

            table.PawnTableOnGUI(body.position);

            if (showStatusLine)
            {
                Rect foot = new Rect(rect.x + 6f, rect.yMax - footH, rect.width - 12f, footH - 2f);
                string s = "PawnTableDef「" + tableDefName + "」 · 列 " + table.Columns.Count
                           + " · 行 " + table.PawnsListForReading.Count
                           + " · 排序 " + (table.SortingBy == null ? "无" : table.SortingBy.defName + (table.SortingDescending ? " ↓" : " ↑"))
                           + "　（点表头循环：降 → 升 → 清除；Shift 点击跳过）";
                Draw.Label(foot, s, GameFont.Tiny, Tokens.Muted, TextAnchor.MiddleLeft);
            }
            return true;
        }

        /// <summary>表头排序的一次循环推进（纯函数，供自建表复用同一语义）。</summary>
        /// <param name="currentKey">当前排序键（null = 未排序）。</param>
        /// <param name="currentDesc">当前是否降序。</param>
        /// <param name="clickedKey">被点击的列键。</param>
        /// <param name="descending">输出：推进后的降序状态。</param>
        /// <returns>推进后的排序键（null = 清除排序）。</returns>
        public static string CycleSort(string currentKey, bool currentDesc, string clickedKey, out bool descending)
        {
            descending = false;
            if (currentKey != clickedKey) { descending = true; return clickedKey; }   // 新列 → 降序
            if (currentDesc) { descending = false; return clickedKey; }               // 降 → 升
            return null;                                                              // 升 → 清除
        }
    }
}
