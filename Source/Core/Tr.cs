using Verse;

namespace RUIF
{
    /// <summary>
    /// 组件文案的**唯一取用点**（v2.1.0「Keyed 化」；v2.1.1 加有界缓存）。
    ///
    /// 为什么集中两点：
    /// ① 统一做 <c>TaggedString → string</c> 转换 —— 否则每个调用点都要写 <c>.ToString()</c>，
    ///    且 <c>??</c> 会因 <c>string</c> 与 <c>TaggedString</c> 混用而编译不过；
    /// ② 键名集中出现，便于门禁的"键覆盖扫描"与人工审查；
    /// ③ ③（v2.1.1）绘制路径的翻译解析统一走 <see cref="TrCache"/> —— 组件每一帧都会取文案，
    ///    引擎字典查找与带参格式化在命中后全部消失。这是 PERF-001（热路径禁重复本地化解析）的落地实现。
    ///
    /// ⚠️ <b>带参翻译必须走 <c>Translate(NamedArgument)</c></b>：1.6 里
    /// <c>Translate(this string, params object[])</c> 已标 <c>[Obsolete]</c>（CS0618），
    /// 而 <c>NamedArgument</c> 具备 <c>int/float/string/char/long/double/uint/byte/ulong/StringBuilder/Thing</c>
    /// 的隐式转换 —— 因此 <c>Tr.Get("K", 3)</c> 这类写法既安全又不装箱到 object。
    /// 占位符按位置取 <c>{0}</c>/<c>{1}</c>。
    ///
    /// ⚠️ <b>int 参数有专用重载（v2.1.1）</b>：<c>Get(string, int)</c> 精确匹配优先级高于
    /// <c>Get(string, NamedArgument)</c> 的用户定义隐式转换，因此<b>调用点无需改动</b>即自动享受缓存；
    /// float/string 实参仍走 <c>NamedArgument</c> 重载（不缓存，理由见 <see cref="TrCache"/>）。
    ///
    /// ⚠️ <c>Translate</c> 需要语言数据（游戏内恒有）。**离线单测不得调用本类** ——
    /// 因此凡"既有语义又有文案"的函数都拆成「纯语义函数（可测）+ 文案函数（走本类）」。
    /// </summary>
    internal static class Tr
    {
        /// <summary>
        /// 有界缓存：512 条，语言戳取 <c>LanguageDatabase.activeLanguage</c>（语言切换即整体失效）。
        /// </summary>
        private static readonly TrCache Cache = new TrCache(512, () => LanguageDatabase.activeLanguage);

        /// <summary>无参取值（命中即同一 string 实例）。</summary>
        internal static string Get(string key)
        {
            return Cache.Get(key, Plain);
        }

        /// <summary>int 单参取值（无分配复合键缓存；绘制路径里最常见的一类）。</summary>
        internal static string Get(string key, int value)
        {
            return Cache.Get(key, value, IntArg);
        }

        /// <summary>单个非 int 参数（float / string / Thing 等）—— 不缓存，直接走引擎。</summary>
        internal static string Get(string key, NamedArgument arg1)
        {
            return key.Translate(arg1).ToString();
        }

        /// <summary>两个参数 —— 不缓存，直接走引擎（组合爆炸，缓存无意义）。</summary>
        internal static string Get(string key, NamedArgument arg1, NamedArgument arg2)
        {
            return key.Translate(arg1, arg2).ToString();
        }

        /// <summary>
        /// 三个参数 —— 同样直接走引擎。当前唯一使用点是设置页的「XML 配置来源」行
        /// （该行同时受语言与配置影响，缓存收益极小；保留此重载是为了<b>所有翻译统一经 <see cref="Tr"/></b>
        /// 这一纪律，便于门禁扫描与后续统一改造）。
        /// </summary>
        internal static string Get(string key, NamedArgument arg1, NamedArgument arg2, NamedArgument arg3)
        {
            return key.Translate(arg1, arg2, arg3).ToString();
        }

        private static string Plain(string key)
        {
            return key.Translate().ToString();
        }

        private static string IntArg(string key, int value)
        {
            return key.Translate(value).ToString();
        }
    }
}
