using UnityEngine;

namespace RUIF
{
    /// <summary>
    /// ⑥ 命令面板候选层绘制 / 过滤转发：具名方法 Draw / Filter（与 <see cref="CommandCandFmt"/> 的 Build 工厂同名，已 == 对应 Build 名），
    /// 纯转发既有 <see cref="CommandPalette"/>，禁复制逻辑。候选 readonly struct 嵌套定义于本 static class（对齐 StatusCandAtoms.StatusPulseDot 写法）；
    /// 为避免与同名绘制方法（Draw / Filter）发生 C# 类型-成员命名冲突，struct 以族前缀命名为 <c>CommandDraw</c> / <c>CommandFilter</c>。
    ///
    /// <para><b>写回状态以入口 ref 参数承载（R8）</b>：本工程 <c>LangVersion=9.0</c> 不支持 ref 字段；
    /// 按 R8，跨帧状态 query / sel <b>不得</b>置于候选 struct 内作为 ref 字段，而以入口 <c>ref</c> 参数承载
    /// （与原 CommandPalette.Draw 同形），正确写回归调用方。候选 struct 仅打包值参数（commands / filtered / scores）。
    /// Filter 无 ref 状态，入口即 (cand)。</para>
    /// </summary>
    public static class CommandCandAtoms
    {
        /// <summary>命令面板绘制候选（值参数包）。query / sel 由调用方经入口 ref 持有（R8）。</summary>
        public readonly struct CommandDraw
        {
            /// <summary>全部命令源。</summary>
            public readonly Command[] commands;
            /// <summary>过滤结果缓冲（调用方持有，零分配）。</summary>
            public readonly Command[] filtered;
            /// <summary>模糊分缓冲（调用方持有，零分配）。</summary>
            public readonly int[] scores;

            public CommandDraw(Command[] commands, Command[] filtered, int[] scores)
            {
                this.commands = commands;
                this.filtered = filtered;
                this.scores = scores;
            }
        }

        /// <summary>命令过滤候选（值参数包，纯计算，无 ref 状态）。</summary>
        public readonly struct CommandFilter
        {
            /// <summary>查询串。</summary>
            public readonly string query;
            /// <summary>命令源。</summary>
            public readonly Command[] source;
            /// <summary>结果缓冲。</summary>
            public readonly Command[] dest;
            /// <summary>分缓冲。</summary>
            public readonly int[] scores;

            public CommandFilter(string query, Command[] source, Command[] dest, int[] scores)
            {
                this.query = query;
                this.source = source;
                this.dest = dest;
                this.scores = scores;
            }
        }

        /// <summary>面板主体：转发 CommandPalette.Draw；返回被执行的命令（仅执行帧非空）。</summary>
        public static Command Draw(Rect rect, CommandDraw cand, ref string query, ref int sel)
            => CommandPalette.Draw(rect, cand.commands, ref query, ref sel, cand.filtered, cand.scores);

        /// <summary>按模糊分过滤并降序：转发 CommandPalette.Filter；返回命中数。</summary>
        public static int Filter(CommandFilter cand)
            => CommandPalette.Filter(cand.query, cand.source, cand.dest, cand.scores);
    }
}
