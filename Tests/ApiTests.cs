using System;
using NUnit.Framework;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>
    /// 版本门面离线单测（不触碰引擎绘制，纯逻辑）。
    ///
    /// ⭐ 核心回归点：<see cref="Api.Version"/> 必须<b>不依赖 RUIFMod 构造器是否已执行</b>。
    /// 本测试工程从不构造 <c>RUIFMod</c>（构造 Mod 需要 ModContentPack）；若版本仍为 0.0.0，
    /// 即说明又退回了「靠 Mod 构造器赋值」的写法 ——
    /// 1.6 <c>LoadedModManager.CreateModClasses()</c> 按<b>类型枚举顺序</b>而非 Mod 加载顺序建类，
    /// 消费 Mod 完全可能先于 RUIFMod 构造器执行，从而随机拿到「框架已装却报不可用」的假阴性。
    /// </summary>
    [TestFixture]
    public class ApiTests
    {
        private static readonly Version Expected = Version.Parse(Api.VersionString);

        [Test]
        public void Version_InitializedWithoutConstructingMod()
        {
            Assert.AreEqual(Expected, Api.Version, "Api.Version 必须在类型初始化时即可用，不得依赖 RUIFMod 构造");
            Assert.AreNotEqual(0, Api.Version.Major, "版本主号不得为 0（0.0.0 会被判为「框架不可用」）");
        }

        [Test]
        public void VersionString_IsParseableSemVer()
        {
            Version parsed;
            Assert.IsTrue(Version.TryParse(Api.VersionString, out parsed), "VersionString 必须是可解析 SemVer");
            Assert.GreaterOrEqual(parsed.Major, 1, "v1.0.0 起进入 API 冻结，主版本不得回退到 0");
        }

        [Test]
        public void IsAvailable_True_WhenFrameworkPresent()
        {
            Assert.IsTrue(Api.IsAvailable());
            Assert.IsTrue(Api.IsAvailable(null));
            Assert.IsTrue(Api.IsAvailable(string.Empty));
        }

        [Test]
        public void IsAvailable_RespectsMinimumVersion()
        {
            Assert.IsTrue(Api.IsAvailable("1.0"));
            Assert.IsTrue(Api.IsAvailable(Api.VersionString));   // 等于当前版本 → 可用
            Assert.IsFalse(Api.IsAvailable("99.0"));                 // 高于当前版本 → 调用方应降级
        }

        [Test]
        public void IsAvailable_InvalidMinVersion_IsFalse()
        {
            Assert.IsFalse(Api.IsAvailable("not-a-version"));
        }
    }
}
