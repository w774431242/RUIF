namespace RUIF
{
    /// <summary>
    /// ⑥ 命令面板候选层（参数对象 struct + Build，纯增量）：桥接既有 <see cref="CommandPalette"/>（位于 NavAtoms.cs）。
    /// 仅打包「值参数」；跨帧状态持有者（query / sel）按本工程 <c>LangVersion=9.0</c> 约束，
    /// 以 <c>ref</c> 形式走 <see cref="CommandCandAtoms"/> 入口（见该文件说明），不进 struct 字段。
    /// 候选 readonly struct 已迁移至 <see cref="CommandCandAtoms"/> 内嵌套定义（对齐 StatusCandAtoms.StatusPulseDot 写法，
    /// 以族前缀 <c>CommandDraw</c> / <c>CommandFilter</c> 规避与同名绘制方法的 C# 命名冲突）；
    /// 本文件<b>只放纯计算 Build 工厂、无 struct 定义</b>（R3）。绘制 / 过滤转发在 <see cref="CommandCandAtoms"/>，零绘制逻辑（GATE-ARCH）。
    /// </summary>
    public static class CommandCandFmt
    {
        /// <summary>命令面板绘制候选：返回 <see cref="CommandCandAtoms.CommandDraw"/>；query / sel 由调用方经入口 ref 持有（R8）。</summary>
        public static CommandCandAtoms.CommandDraw Draw(Command[] commands, Command[] filtered, int[] scores)
            => new CommandCandAtoms.CommandDraw(commands, filtered, scores);

        /// <summary>命令过滤候选（值参数包，纯计算，无 ref 状态）：返回 <see cref="CommandCandAtoms.CommandFilter"/>。</summary>
        public static CommandCandAtoms.CommandFilter Filter(string query, Command[] source, Command[] dest, int[] scores)
            => new CommandCandAtoms.CommandFilter(query, source, dest, scores);
    }
}
