using System;

namespace RUIF
{
    /// <summary>
    /// 消费 Mod 的探测门面（版本断言）。
    ///
    /// ⚠️ <b>依赖语义</b>：本类型位于 RUIF.dll 内，<b>框架未安装时，任何引用本类型的代码路径会在 JIT 期抛
    /// <c>FileNotFoundException</c>/<c>TypeLoadException</c></b>——不会走到你的 else 分支。
    /// 因此只有两种正确接入方式：
    /// ① 硬依赖（推荐）：About.xml 写 <c>modDependencies</c>，用本类型做版本门控；
    /// ② 软依赖：主路径只读你自己缓存的 bool，全部 RUIF 调用隔离到独立类型/方法，
    ///    探测用反射扫 <c>AppDomain</c>（不引用 RUIF 类型即可完成）。
    /// </summary>
    public static class Api
    {
        /// <summary>
        /// 框架版本（SemVer，与 About.xml 的 modVersion 保持同步）。
        ///
        /// 版本号在<b>类型初始化时</b>由编译期常量解析得出，不依赖任何 Mod 构造器是否已执行。
        ///
        /// 原因（1.6 引擎事实）：<c>Verse.LoadedModManager.CreateModClasses()</c> 遍历的是
        /// <c>typeof(Mod).InstantiableDescendantsAndSelf()</c> —— <b>类型枚举顺序，而非 Mod 加载顺序</b>。
        /// 若版本靠在 Mod 构造器里赋值，消费 Mod 在自构造期探测就会随机拿到 0.0.0，
        /// 得到「框架已装却报不可用」的假阴性（间歇性、与 loadOrder 无关）。
        /// </summary>
        public const string VersionString = "1.1.2";

        private static readonly Version version = ParseVersion(VersionString);

        private static Version ParseVersion(string s)
        {
            Version parsed;
            return Version.TryParse(s, out parsed) ? parsed : new Version(0, 0, 0);
        }

        /// <summary>当前框架版本（= <see cref="VersionString"/> 解析值）。</summary>
        public static Version Version => version;

        /// <summary>
        /// 当前框架版本不低于 <paramref name="minVersion"/>。
        /// </summary>
        /// <param name="minVersion">最低版本（如 "1.0"）；null/空表示只判断框架是否存在。</param>
        public static bool IsAvailable(string minVersion = null)
        {
            var v = Version;
            if (v.Major == 0 && v.Minor == 0 && v.Build == 0) return false;
            if (string.IsNullOrEmpty(minVersion)) return true;
            Version min;
            return Version.TryParse(minVersion, out min) && v >= min;
        }
    }
}
