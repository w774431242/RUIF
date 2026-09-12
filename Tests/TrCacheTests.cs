using System;
using NUnit.Framework;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>
    /// <see cref="TrCache"/>（翻译有界缓存）单测 —— v2.1.1 第二次体检的性能修复回归锁。
    ///
    /// 为什么这些断言必须存在：缓存是"以时间复杂度换正确性风险"的典型装置，四种语义
    /// （命中 / 语言失效 / 容量上限 / 不缓存 null）任一失效都会变成**线上才看得见的 bug**：
    /// · 命中失效        → 每帧仍走引擎字典查找与格式化分配（性能修复形同虚设）；
    /// · 语言失效失效    → 玩家切语言后界面仍是旧语言（最典型的缓存 bug）；
    /// · 容量无上限      → 无界静态增长（AI-004 禁止项）；
    /// · null 被缓存     → 翻译环境暂缺被固化成永久空白。
    ///
    /// 本类不触碰 UnityEngine/Verse：语言戳与取值都由委托注入，故可在无引擎环境下离线断言。
    /// </summary>
    [TestFixture]
    public class TrCacheTests
    {
        [Test]
        public void Get_PlainKey_SecondCallIsServedFromCache()
        {
            int calls = 0;
            var cache = new TrCache(16, () => null);

            string first = cache.Get("RUIF.Comp.Common.OK", k => { calls++; return "OK"; });
            string second = cache.Get("RUIF.Comp.Common.OK", k => { calls++; return "OK"; });

            Assert.AreEqual("OK", first);
            Assert.AreEqual("OK", second);
            Assert.AreEqual(1, calls, "第二次必须命中缓存，不得再调 factory");
            Assert.AreSame(first, second, "命中应返回同一 string 实例（零分配的证据）");
            Assert.AreEqual(1, cache.Count);
        }

        [Test]
        public void Get_IntVariant_SecondCallIsServedFromCache()
        {
            int calls = 0;
            var cache = new TrCache(16, () => null);

            Assert.AreEqual("3", cache.Get("RUIF.Comp.Common.Page", 3, (k, n) => { calls++; return n.ToString(); }));
            Assert.AreEqual("3", cache.Get("RUIF.Comp.Common.Page", 3, (k, n) => { calls++; return n.ToString(); }));
            Assert.AreEqual(1, calls, "int 复合键命中后不得再调 factory");
        }

        [Test]
        public void Get_IntVariant_SeparatesByKeyAndValue()
        {
            var cache = new TrCache(32, () => null);
            Func<string, int, string> f = (k, n) => k + "#" + n;

            Assert.AreEqual("A#1", cache.Get("A", 1, f));
            Assert.AreEqual("A#2", cache.Get("A", 2, f));
            Assert.AreEqual("B#1", cache.Get("B", 1, f));

            Assert.AreEqual("A#1", cache.Get("A", 1, f), "同键同值应命中");
            Assert.AreEqual(3, cache.Count, "三个不同的 (键,值) 组合应各占一条");
        }

        [Test]
        public void Get_KeyComparisonIsOrdinal()
        {
            var cache = new TrCache(16, () => null);
            Assert.AreEqual("upper", cache.Get("K", k => "upper"));
            Assert.AreEqual("lower", cache.Get("k", k => "lower"), "键比较必须大小写敏感（Ordinal）");
            Assert.AreEqual(2, cache.Count);
        }

        [Test]
        public void LanguageStampChange_InvalidatesEverything()
        {
            object lang = new object();
            var cache = new TrCache(16, () => lang);
            int calls = 0;

            cache.Get("K", k => { calls++; return "V1"; });
            cache.Get("K", k => { calls++; return "V1"; });
            Assert.AreEqual(1, calls, "同一语言戳内应命中");
            Assert.AreEqual(0, cache.LanguageInvalidations, "首次绑定语言戳不算失效");

            lang = new object();   // 模拟切换语言：语言戳引用变化
            Assert.AreEqual("V2", cache.Get("K", k => { calls++; return "V2"; }));
            Assert.AreEqual(2, calls, "语言变化必须重新取值");
            Assert.AreEqual(1, cache.LanguageInvalidations);
        }

        [Test]
        public void CapacityFlush_KeepsEntryCountBounded()
        {
            var cache = new TrCache(4, () => null);
            for (int i = 0; i < 50; i++) cache.Get("K" + i, k => k);

            Assert.LessOrEqual(cache.Count, cache.Capacity, "条目数恒不得超过容量上限（无界增长即 AI-004 违规）");
            Assert.Greater(cache.CapacityFlushes, 0, "超限时应发生清空重建");
        }

        [Test]
        public void NullResult_IsNotCached()
        {
            int calls = 0;
            var cache = new TrCache(16, () => null);

            Assert.IsNull(cache.Get("K", k => { calls++; return null; }));
            Assert.IsNull(cache.Get("K", k => { calls++; return null; }));

            Assert.AreEqual(2, calls, "null 不得被缓存：翻译环境暂缺不能被固化成永久结果");
            Assert.AreEqual(0, cache.Count);
        }

        [Test]
        public void Constructor_RejectsNonPositiveCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TrCache(0, () => null));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TrCache(-1, () => null));
        }

        [Test]
        public void Constructor_NullLanguageProvider_IsTolerated()
        {
            var cache = new TrCache(8, null);
            Assert.AreEqual("V", cache.Get("K", k => "V"), "语言戳委托缺失时应退化为'永不失效'而非抛异常");
        }
    }
}
