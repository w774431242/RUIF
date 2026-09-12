using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>数据展示组件纯计算单测。</summary>
    [TestFixture]
    public class DataAtomsTests
    {
        [Test]
        public void Bar01_DividesAndClamps()
        {
            Assert.AreEqual(0.5f, DataFmt.Bar01(50f, 100f), 0.001f);
            Assert.AreEqual(1f, DataFmt.Bar01(500f, 100f), 0.001f);
            Assert.AreEqual(0f, DataFmt.Bar01(50f, 0f), 0.001f);
        }

        [Test]
        public void SkillColor_FiveTiers()
        {
            Assert.AreEqual(Tokens.Dim, DataFmt.SkillColor(2f));
            Assert.AreEqual(Tokens.Muted, DataFmt.SkillColor(6f));
            Assert.AreEqual(Tokens.Alive, DataFmt.SkillColor(10f));
            Assert.AreEqual(Tokens.Info, DataFmt.SkillColor(14f));
            Assert.AreEqual(Tokens.Accent, DataFmt.SkillColor(18f));
        }

        [Test]
        public void RowRect_AdvancesByHeight()
        {
            var list = new Rect(0, 0, 100, 100);
            Rect r2 = DataFmt.RowRect(list, 2, 20f, 5f);
            Assert.AreEqual(50f, r2.y, 0.001f);
            Assert.AreEqual(20f, r2.height, 0.001f);
        }

        [Test]
        public void HeatLevel_ZeroIsEmpty()
        {
            Assert.AreEqual(0, DataFmt.HeatLevel(0f, 4));
            Assert.AreEqual(1, DataFmt.HeatLevel(0.1f, 4));
            Assert.AreEqual(4, DataFmt.HeatLevel(1f, 4));
            Assert.AreEqual(2, DataFmt.HeatLevel(0.4f, 4));
        }

        [Test]
        public void RankMedal_TopThreeThenNumber()
        {
            Assert.AreEqual("①", DataFmt.RankMedal(0));
            Assert.AreEqual("②", DataFmt.RankMedal(1));
            Assert.AreEqual("③", DataFmt.RankMedal(2));
            Assert.AreEqual("4", DataFmt.RankMedal(3));
        }
    }
}
