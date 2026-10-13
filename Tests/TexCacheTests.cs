using NUnit.Framework;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>
    /// 贴图缓存键的唯一性单测 —— 2026-09-13「**饼图取到环图贴图、中间带孔**」事故的对策。
    ///
    /// <para><b>为什么必须有这组测试</b>：键的唯一性是**纯约定、零编译期检查**的性质 ——
    /// 少放一个判别量进键，编译器不报、代码看起来也像做了处理，只在运行期表现为
    /// 「两张不同的图共用同一张贴图」。故用**性质测试**覆盖：任一判别量变化 ⇒ 键必须变化。</para>
    ///
    /// <para>⚠ 本组只覆盖**合法取值域内**的行为。越界路径会走 <c>Log.ErrorOnce</c>（依赖 Unity 运行时），
    /// 不属于纯计算单测的范围。另：本组只用 <c>Mathf.RoundToInt / Clamp</c>（均为托管实现，非 ECall），
    /// 符合框架「离线单测不得触碰 ECall」的纪律。</para>
    /// </summary>
    [TestFixture]
    public class TexCacheTests
    {
        // ------------------------------------------------------------ KeyOf：结构性

        [Test]
        public void KeyOf_SameInputs_ProducesSameKey()
        {
            Assert.AreEqual(TexCache.KeyOf(0x53, 1, 2, 3, 4, 5),
                            TexCache.KeyOf(0x53, 1, 2, 3, 4, 5));
        }

        [Test]
        public void KeyOf_EachParamPosition_ChangesKey()
        {
            long b0 = TexCache.KeyOf(0x53, 1, 2, 3, 4, 5);
            Assert.AreNotEqual(b0, TexCache.KeyOf(0x54, 1, 2, 3, 4, 5), "kind 未进键");
            Assert.AreNotEqual(b0, TexCache.KeyOf(0x53, 9, 2, 3, 4, 5), "a 未进键");
            Assert.AreNotEqual(b0, TexCache.KeyOf(0x53, 1, 9, 3, 4, 5), "b 未进键");
            Assert.AreNotEqual(b0, TexCache.KeyOf(0x53, 1, 2, 9, 4, 5), "c 未进键");
            Assert.AreNotEqual(b0, TexCache.KeyOf(0x53, 1, 2, 3, 9, 5), "d 未进键");
            Assert.AreNotEqual(b0, TexCache.KeyOf(0x53, 1, 2, 3, 4, 9), "e 未进键");
        }

        [Test]
        public void KeyOf_NoCollision_OverSampleSpace()
        {
            // 性质测试：合法域内抽样（约 2 万组合），键集合必须无重复。
            var seen = new System.Collections.Generic.Dictionary<long, string>();
            for (int kind = 0; kind <= 255; kind += 17)
                for (int a = 0; a <= 2047; a += 137)
                    for (int b = 0; b <= 360; b += 31)
                        for (int c = 0; c <= 100; c += 17)
                        {
                            long k = TexCache.KeyOf(kind, a, b, c);
                            string who = kind + "/" + a + "/" + b + "/" + c;
                            // ⚠ 不能写成 Assert.IsFalse(seen.ContainsKey(k), "…" + seen[k]) ——
                            //    NUnit 会**预先求值**消息串 ⇒ 正常情况下 k 不在字典里，直接抛 KeyNotFoundException。
                            //    （本测试初版就踩了这个坑，靠真的跑一遍才发现。）
                            string prev;
                            if (seen.TryGetValue(k, out prev)) Assert.Fail("键碰撞：" + who + " 与 " + prev);
                            seen[k] = who;
                        }
            Assert.Greater(seen.Count, 100, "样本量过小 ⇒ 测试无意义");
        }

        [Test]
        public void KeyOf_MarkerBit_IsolatesFromLegacyConstructors()
        {
            // 标记位（bit63）= 1 ⇒ 新键恒为负数；旧构造器最高位为 0 ⇒ 两者永不撞键。
            Assert.Less(TexCache.KeyOf(0, 0, 0, 0, 0, 0), 0L, "KeyOf 的标记位应为 1");
            Assert.GreaterOrEqual(TexCache.KeyVersioned(3, 12345, 128, 64), 0L, "旧 KeyVersioned 最高位应为 0");
            Assert.GreaterOrEqual(TexCache.Key(1, UnityEngine.Color.red, UnityEngine.Color.blue, 128, 16), 0L,
                                  "旧 Key 最高位应为 0");
        }

        // ------------------------------------------------------------ KeySector：事故回归

        private static readonly UnityEngine.Color ColA = new UnityEngine.Color(0.20f, 0.45f, 0.90f, 1f);
        private static readonly UnityEngine.Color ColB = new UnityEngine.Color(0.90f, 0.30f, 0.12f, 1f);

        /// <summary>★ 事故回归：孔洞比必须进键（环图 vs 饼图）。</summary>
        [Test]
        public void KeySector_HoleRatioMustAffectKey()
        {
            // 2026-09-13：DonutChart 算出孔洞比却传了常量 ⇒ 环图(0.25) 与饼图(0.0) 共用贴图，
            // 表现为「饼图中间有个孔」。本断言保证该回归**永久不可能再发生**。
            long ring = TexCache.KeySector(ColA, -90f, 136.8f, 0.25f, 128);
            long pie = TexCache.KeySector(ColA, -90f, 136.8f, 0.0f, 128);
            Assert.AreNotEqual(ring, pie, "孔洞比没进键 ⇒ 环图与饼图会共用贴图");
        }

        [Test]
        public void KeySector_EachParamChangesKey()
        {
            long b0 = TexCache.KeySector(ColA, -90f, 136.8f, 0.5f, 128);
            Assert.AreNotEqual(b0, TexCache.KeySector(ColB, -90f, 136.8f, 0.5f, 128), "色值未进键");
            Assert.AreNotEqual(b0, TexCache.KeySector(ColA, 0f, 136.8f, 0.5f, 128), "起角未进键");
            Assert.AreNotEqual(b0, TexCache.KeySector(ColA, -90f, 90f, 0.5f, 128), "张角未进键");
            Assert.AreNotEqual(b0, TexCache.KeySector(ColA, -90f, 136.8f, 0.25f, 128), "孔洞比未进键");
            Assert.AreNotEqual(b0, TexCache.KeySector(ColA, -90f, 136.8f, 0.5f, 256), "边长未进键");
        }

        /// <summary>★ P1 回归：换主题（色值变）⇒ 键必须变 ⇒ 缓存自动失效，不必手动 Clear()。</summary>
        [Test]
        public void KeySector_ThemeColorChange_InvalidatesCache()
        {
            long before = TexCache.KeySector(new UnityEngine.Color(0.20f, 0.45f, 0.90f, 1f), -90f, 90f, 0.5f, 128);
            long after = TexCache.KeySector(new UnityEngine.Color(0.30f, 0.45f, 0.90f, 1f), -90f, 90f, 0.5f, 128);
            Assert.AreNotEqual(before, after, "色值没进键 ⇒ 换主题后会复用旧色贴图（原实现传调色板序号就有此缺陷）");
        }

        [Test]
        public void KeySector_AnglesQuantizedToOneDegree()
        {
            // 1° 量化：同 1° 内同键（缓存友好）；跨 1° 不同键（绝不会画错）。
            Assert.AreEqual(TexCache.KeySector(ColA, -90.2f, 136.6f, 0.5f, 128),
                            TexCache.KeySector(ColA, -90.4f, 136.9f, 0.5f, 128), "0.4° 内应同键");
            Assert.AreNotEqual(TexCache.KeySector(ColA, -90f, 136f, 0.5f, 128),
                               TexCache.KeySector(ColA, -89f, 136f, 0.5f, 128), "跨 1° 必须不同键");
        }

        [Test]
        public void KeySector_NegativeAndWrappedAnglesAreNormalized()
        {
            // 起角归一化到 [0,720)：-90° 与 630° 是同一角，应同键。
            Assert.AreEqual(TexCache.KeySector(ColA, -90f, 100f, 0.5f, 128),
                            TexCache.KeySector(ColA, 630f, 100f, 0.5f, 128));
        }

        // ------------------------------------------------------------ 旧构造器缺陷回归

        /// <summary>★ 缺陷回归：`Key` 的尺寸必须真的进键（原实现 `w ≥ 16` 整段溢出丢弃）。</summary>
        [Test]
        public void Key_SizeMustAffectKey()
        {
            // 原实现 (w & 0xFFFF) << 60 在 w ≥ 16 时溢出 ⇒ w=16 / 32 / 48 生成同一个键。
            long k16 = TexCache.Key(1, UnityEngine.Color.red, UnityEngine.Color.blue, 16, 8);
            long k32 = TexCache.Key(1, UnityEngine.Color.red, UnityEngine.Color.blue, 32, 8);
            long k48 = TexCache.Key(1, UnityEngine.Color.red, UnityEngine.Color.blue, 48, 8);
            Assert.AreNotEqual(k16, k32, "w 未进键");
            Assert.AreNotEqual(k32, k48, "w 未进键");
            Assert.AreNotEqual(TexCache.Key(1, UnityEngine.Color.red, UnityEngine.Color.blue, 16, 8),
                               TexCache.Key(1, UnityEngine.Color.red, UnityEngine.Color.blue, 16, 16), "h 未进键");
        }

        /// <summary>★ 缺陷回归：`KeyVersioned` 的 version 不再有 2^22 上限。</summary>
        [Test]
        public void KeyVersioned_LargeVersionDoesNotCollideWithSize()
        {
            long a = TexCache.KeyVersioned(3, 1 << 22, 128, 64);
            long b = TexCache.KeyVersioned(3, 0, 129, 64);
            Assert.AreNotEqual(a, b, "version 高位与尺寸位段相撞（原实现 version<<20 与 h<<42 重叠）");
        }
    }
}
