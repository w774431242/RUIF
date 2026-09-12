using NUnit.Framework;
using UnityEngine;
using RUIF;

namespace RUIF.Tests
{
    /// <summary>C2 数据扩展组件纯计算单测。</summary>
    [TestFixture]
    public class DataExAtomsTests
    {
        [Test]
        public void LinkMid_FormsRightAngle()
        {
            Vector2 mid = DataExFmt.LinkMid(new Vector2(0f, 10f), new Vector2(50f, 30f));
            Assert.AreEqual(50f, mid.x, 0.001f);
            Assert.AreEqual(10f, mid.y, 0.001f);
        }

        [Test]
        public void NodeRect_OffsetsByColRow()
        {
            var canvas = new Rect(0, 0, 300, 200);
            Rect r = DataExFmt.NodeRect(canvas, 2, 1, 80f, 30f, 10f);
            Assert.AreEqual(180f, r.x, 0.001f);      // 2 × (80+10)
            Assert.AreEqual(40f, r.y, 0.001f);       // 1 × (30+10)
            Assert.AreEqual(80f, r.width, 0.001f);
        }

        [Test]
        public void DockZone_DetectsEdges()
        {
            var screen = new Rect(0, 0, 1000, 600);
            Assert.AreEqual(1, DataExFmt.DockZone(screen, 24f, new Vector2(10f, 300f)));
            Assert.AreEqual(2, DataExFmt.DockZone(screen, 24f, new Vector2(990f, 300f)));
            Assert.AreEqual(3, DataExFmt.DockZone(screen, 24f, new Vector2(500f, 10f)));
            Assert.AreEqual(4, DataExFmt.DockZone(screen, 24f, new Vector2(500f, 590f)));
            Assert.AreEqual(0, DataExFmt.DockZone(screen, 24f, new Vector2(500f, 300f)));
        }

        [Test]
        public void PresetIndex_FindsByName()
        {
            var names = new[] { "全员施工", "战时体制" };
            Assert.AreEqual(1, DataExFmt.PresetIndex(names, "战时体制"));
            Assert.AreEqual(-1, DataExFmt.PresetIndex(names, "不存在"));
            Assert.AreEqual(-1, DataExFmt.PresetIndex(null, "x"));
        }

        [Test]
        public void Sum_And_Share()
        {
            Assert.AreEqual(60f, DataExFmt.Sum(new[] { 10f, 20f, 30f }), 0.001f);
            Assert.AreEqual(0f, DataExFmt.Sum(null), 0.001f);
            Assert.AreEqual(0.25f, DataExFmt.Share(25f, 100f), 0.001f);
            Assert.AreEqual(0f, DataExFmt.Share(25f, 0f), 0.001f);
            Assert.AreEqual(1f, DataExFmt.Ratio(5f, 5f), 0.001f);
        }

        [Test]
        public void ShareRatios_WritesBuffer()
        {
            var values = new[] { 50f, 30f, 20f };
            var ratios = new float[3];
            DataExFmt.ShareRatios(values, ratios);
            Assert.AreEqual(0.5f, ratios[0], 0.001f);
            Assert.AreEqual(0.3f, ratios[1], 0.001f);
            Assert.AreEqual(0.2f, ratios[2], 0.001f);
            Assert.AreEqual(1f, ratios[0] + ratios[1] + ratios[2], 0.001f);
        }

        [Test]
        public void NetBalance_And_Compare()
        {
            Assert.AreEqual(290f, DataExFmt.NetBalance(1420f, 1130f), 0.001f);
            Assert.AreEqual(-50f, DataExFmt.NetBalance(100f, 150f), 0.001f);
            Assert.AreEqual(1, DataExFmt.CompareWinner(10f, 5f));
            Assert.AreEqual(-1, DataExFmt.CompareWinner(3f, 8f));
            Assert.AreEqual(0, DataExFmt.CompareWinner(7f, 7f));
        }

        [Test]
        public void Tones_MapToTokens()
        {
            Assert.AreEqual(Tokens.Dead, DataExFmt.RelationTone(-10f));
            Assert.AreEqual(Tokens.Alive, DataExFmt.RelationTone(50f));
            Assert.AreEqual(Tokens.Dim, DataExFmt.TrainingTone(0));
            Assert.AreEqual(Tokens.Accent, DataExFmt.TrainingTone(5));
            Assert.AreEqual(Tokens.Accent, DataExFmt.WeatherTone(0));
            Assert.AreEqual(Tokens.Warn, DataExFmt.WeatherTone(4));
        }
    }
}
