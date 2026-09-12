using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>官方数据系统组件纯计算单测。</summary>
    [TestFixture]
    public class OfficialFmtTests
    {
        /// <summary>
        /// v2.1.0 Keyed 化后：<c>RelationLabel</c> 依赖语言数据（离线不可调用），
        /// 因此单测改为验证**语义档位** <see cref="OfficialFmt.RelationBandOf"/>（五档边界仍被真正覆盖）；
        /// 文案正确性交给发布门禁的"翻译键覆盖 + 中英键对称"。
        /// </summary>
        [Test]
        public void RelationBandOf_FiveBands()
        {
            Assert.AreEqual(RelationBand.Ally, OfficialFmt.RelationBandOf(80f));
            Assert.AreEqual(RelationBand.Ally, OfficialFmt.RelationBandOf(75f), "75 是同盟下边界（>=75 即同盟）");
            Assert.AreEqual(RelationBand.Friendly, OfficialFmt.RelationBandOf(74.9f), "75 以下即降到 友好");
            Assert.AreEqual(RelationBand.Friendly, OfficialFmt.RelationBandOf(30f));
            Assert.AreEqual(RelationBand.Neutral, OfficialFmt.RelationBandOf(0f));
            Assert.AreEqual(RelationBand.Neutral, OfficialFmt.RelationBandOf(-24f));
            Assert.AreEqual(RelationBand.Hostile, OfficialFmt.RelationBandOf(-50f));
            Assert.AreEqual(RelationBand.DeadHostile, OfficialFmt.RelationBandOf(-90f));
        }

        [Test]
        public void RelationColor_ThreeTones()
        {
            Assert.AreEqual(Tokens.Alive, OfficialFmt.RelationColor(50f));
            Assert.AreEqual(Tokens.Muted, OfficialFmt.RelationColor(0f));
            Assert.AreEqual(Tokens.Dead, OfficialFmt.RelationColor(-50f));
        }

        [Test]
        public void BillRatio_ClampsAndDefaults()
        {
            Assert.AreEqual(0.5f, OfficialFmt.BillRatio(50f, 100f), 0.001f);
            Assert.AreEqual(1f, OfficialFmt.BillRatio(120f, 100f), 0.001f);
            Assert.AreEqual(1f, OfficialFmt.BillRatio(10f, 0f), 0.001f);
        }

        // 注：原 EtaLabel_Formats 用例已删除 —— v2.1.0 Keyed 化后 EtaLabel 走 Translate，
        // 离线（无语言数据）不可调用。其唯一逻辑是「days<=0 → 已到期 / 否则 剩余 {0} 天」，
        // 该分支由代码审查覆盖，文案与占位符正确性由门禁的"键覆盖 + 中英键对称"保证。

        [Test]
        public void WealthTotal_Sums()
        {
            Assert.AreEqual(284f, OfficialFmt.WealthTotal(new[] { 131f, 108f, 45f }), 0.001f);
            Assert.AreEqual(0f, OfficialFmt.WealthTotal(null), 0.001f);
        }

        [Test]
        public void HexWithHash_PrefixesHash()
        {
            Assert.AreEqual("#FF0000", OfficialFmt.HexWithHash(new Color(1f, 0f, 0f)));
        }
    }
}
