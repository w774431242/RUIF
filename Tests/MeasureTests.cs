using NUnit.Framework;
using Verse;

namespace RUIF.Tests
{
    /// <summary>
    /// B3-a 离线单测面：<see cref="Measure.DegradeTiny"/> 的静默降级规则
    /// （镜像 <c>Verse.Text</c> 的 <c>Font</c> setter，<c>Verse.Text.cs:31-35</c>）。
    ///
    /// <para><b>⛔ 本文件（以及任何单测）不得调用 <c>Measure.RowHeight</c> / <c>Measure.ControlHeight</c></b>：
    /// 那条路径会触碰 <c>Verse.Text</c> 的静态构造 → <c>Resources.Load</c>（Unity ECall），
    /// 在离线 net48 宿主会抛异常，并把<b>整个测试程序集</b>带崩（不仅仅是这一条用例失败）。
    /// <c>Text.LineHeightOf(Tiny)</c> 的数值真值只能在实机观测 —— 方案书 §10 已登记为 NOT RUN，<b>不猜</b>。</para>
    ///
    /// <para>可见性说明：<see cref="Measure"/> 为 public，<see cref="Measure.DegradeTiny"/> 为 internal，
    /// 由 <c>Source/Core/AssemblyInfo.cs</c> 的 <c>InternalsVisibleTo("RUIF.Tests")</c> 放行，无需额外声明。</para>
    /// </summary>
    [TestFixture]
    public class MeasureTests
    {
        // ---- DegradeTiny：方案书要求的 4 个基本例 ----

        [Test]
        public void DegradeTiny_TinyNotSupported_BecomesSmall()
            => Assert.AreEqual(GameFont.Small, Measure.DegradeTiny(GameFont.Tiny, false));

        [Test]
        public void DegradeTiny_TinySupported_StaysTiny()
            => Assert.AreEqual(GameFont.Tiny, Measure.DegradeTiny(GameFont.Tiny, true));

        [Test]
        public void DegradeTiny_Small_UnaffectedByTinySupport()
        {
            Assert.AreEqual(GameFont.Small, Measure.DegradeTiny(GameFont.Small, false));
            Assert.AreEqual(GameFont.Small, Measure.DegradeTiny(GameFont.Small, true));
        }

        [Test]
        public void DegradeTiny_Medium_UnaffectedByTinySupport()
        {
            Assert.AreEqual(GameFont.Medium, Measure.DegradeTiny(GameFont.Medium, false));
            Assert.AreEqual(GameFont.Medium, Measure.DegradeTiny(GameFont.Medium, true));
        }

        // ---- 补强：穷举所有字号档，钉住"只有 Tiny 会被改写" ----

        [Test]
        public void DegradeTiny_IsIdentityForEveryFontWhenTinySupported()
        {
            var all = new[] { GameFont.Tiny, GameFont.Small, GameFont.Medium };
            for (int i = 0; i < all.Length; i++)
                Assert.AreEqual(all[i], Measure.DegradeTiny(all[i], true),
                    "Tiny 受支持时任何档位都不得被改写（包括 Tiny 自身）");
        }

        [Test]
        public void DegradeTiny_IsPure_RepeatedCallsGiveSameAnswer()
        {
            // 纯函数 ⇒ 无隐藏状态：正反调用序列不得互相污染（这正是"不能做 static readonly 缓存"的对照面）
            Assert.AreEqual(GameFont.Small, Measure.DegradeTiny(GameFont.Tiny, false));
            Assert.AreEqual(GameFont.Tiny, Measure.DegradeTiny(GameFont.Tiny, true));
            Assert.AreEqual(GameFont.Small, Measure.DegradeTiny(GameFont.Tiny, false));
        }
    }
}
