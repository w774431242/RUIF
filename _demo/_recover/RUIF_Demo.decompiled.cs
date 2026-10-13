using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]
[assembly: AssemblyVersion("0.0.0.0")]
namespace RUIF.Demo;

internal sealed class BenchDiagnostics : IBench
{
	private static readonly string[] TokenNames = new string[16]
	{
		"Window", "Panel", "PanelRaised", "Card", "CardHover", "Border", "BorderSoft", "BorderHair", "Text", "Muted",
		"Dim", "Accent", "Info", "Alive", "Dead", "Warn"
	};

	private static readonly Color[] TokenColors = (Color[])(object)new Color[16]
	{
		Tokens.Window,
		Tokens.Panel,
		Tokens.PanelRaised,
		Tokens.Card,
		Tokens.CardHover,
		Tokens.Border,
		Tokens.BorderSoft,
		Tokens.BorderHair,
		Tokens.Text,
		Tokens.Muted,
		Tokens.Dim,
		Tokens.Accent,
		Tokens.Info,
		Tokens.Alive,
		Tokens.Dead,
		Tokens.Warn
	};

	private static readonly Type[] Probes = new Type[17]
	{
		typeof(Px),
		typeof(TexFmt),
		typeof(TexCache),
		typeof(TexAtoms),
		typeof(ReorderFmt),
		typeof(ReorderAtoms),
		typeof(HiAtoms),
		typeof(Draw),
		typeof(Layout),
		typeof(Atoms),
		typeof(Tokens),
		typeof(ReorderableWidget),
		typeof(PawnTable),
		typeof(PawnColumnWorker),
		typeof(MainButtonWorker),
		typeof(UIHighlighter),
		typeof(UIScaling)
	};

	private const float FramePadV = 20f;

	private static readonly int AssertRows = (Probes.Length + 2) / 3;

	private static float AssertSectionHeight => SecH(AssertRows + 1, Measure.RowHeight((GameFont)0));

	public bool Scrollable => true;

	private static float SecH(int rows, float rowH)
	{
		return 20f + (float)rows * rowH;
	}

	public float Height(float width)
	{
		return 600f;
	}

	public void Render(Rect area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		VStack val = Layout.Column(area, 16f);
		VStack val2 = Layout.Column(Atoms.SectionFrame(((VStack)(ref val)).Take(SecH(3, 18f)), "程序集与运行时"), 3f);
		Assembly assembly = typeof(Draw).Assembly;
		Type[] types = assembly.GetTypes();
		int num = 0;
		for (int i = 0; i < types.Length; i++)
		{
			if (types[i].IsPublic)
			{
				num++;
			}
		}
		Line(((VStack)(ref val2)).Take(18f), "RUIF 程序集", assembly.GetName().Name + "\u3000Api 1.1.0\u3000程序集版本 " + assembly.GetName().Version?.ToString() + "\u3000类型 " + types.Length + "（public " + num + "）");
		Line(((VStack)(ref val2)).Take(18f), "UI 缩放", "Prefs.UIScale = " + Prefs.UIScale.ToString("0.##") + "（像素对齐的必要性正来自非整数缩放）");
		Line(((VStack)(ref val2)).Take(18f), "贴图缓存", "TexCache 上限 " + 128 + " 条（按插入顺序淘汰；Clear() 可整体释放）");
		VStack val3 = Layout.Column(Atoms.SectionFrame(((VStack)(ref val)).Take(AssertSectionHeight), "编译期类型断言（" + Probes.Length + " 个）"), 2f);
		Draw.EllipsisLabel(((VStack)(ref val3)).Take(18f), "构建通过 ⇒ 以下外层类型均可被消费方直接引用：", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		for (int j = 0; j < Probes.Length; j += 3)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int k = j; k < j + 3 && k < Probes.Length; k++)
			{
				stringBuilder.Append(Probes[k].FullName).Append("   ");
			}
			Draw.EllipsisLabel(((VStack)(ref val3)).Take(18f), stringBuilder.ToString(), (GameFont)0, Tokens.Info, (TextAnchor)3);
		}
		Rect val4 = Atoms.SectionFrame(((VStack)(ref val)).Take(150f), "像素对齐对照（Px）");
		float num2 = ((Rect)(ref val4)).y + 6f;
		float num3 = ((Rect)(ref val4)).x + 6f;
		for (int l = 0; l < 3; l++)
		{
			float num4 = 0.5f + (float)l * 0.5f;
			Draw.LineH(num3 + num4, num2 + (float)l * 22f, 220f, Tokens.Accent);
			Draw.Label(new Rect(num3, num2 + (float)l * 22f - 8f, 200f, 16f), "raw + " + num4.ToString("0.0") + "px", (GameFont)0, Tokens.Muted, (TextAnchor)3);
			Rect val5 = Px.HLine(num3 + num4, num2 + (float)l * 22f + 8f, 220f);
			Draw.Box(val5, Tokens.Alive);
			Draw.Label(new Rect(num3 + 232f, num2 + (float)l * 22f - 6f, 260f, 16f), "raw y=" + (num2 + (float)l * 22f).ToString("0.0") + " → Px y=" + ((Rect)(ref val5)).y.ToString("0.0") + "  h=" + ((Rect)(ref val5)).height.ToString("0.0"), (GameFont)0, Tokens.Info, (TextAnchor)3);
		}
		Draw.Label(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).yMax - 16f, ((Rect)(ref val4)).width, 16f), "上：直接给浮点坐标（非整数缩放下发虚）\u3000下：Px.HLine 吸附后（干净 1px）", (GameFont)0, Tokens.Dim, (TextAnchor)3);
		Rect val6 = Atoms.SectionFrame(((VStack)(ref val)).Take(84f), "设计令牌 16 色");
		Rect val7 = default(Rect);
		((Rect)(ref val7))..ctor(((Rect)(ref val6)).x, ((Rect)(ref val6)).y + 2f, ((Rect)(ref val6)).width, 24f);
		HStack val8 = Layout.Row(val7, 4f);
		for (int m = 0; m < TokenColors.Length; m++)
		{
			Rect val9 = ((HStack)(ref val8)).Take(56f);
			Draw.Box(val9, TokenColors[m]);
			Draw.Border(val9, Tokens.BorderHair);
		}
		Draw.Label(new Rect(((Rect)(ref val6)).x, ((Rect)(ref val7)).yMax + 2f, ((Rect)(ref val6)).width, 16f), string.Join(" · ", TokenNames), (GameFont)0, Tokens.Dim, (TextAnchor)3);
	}

	private static void Line(Rect r, string k, string v)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		HStack val = Layout.Row(r, 12f);
		Draw.EllipsisLabel(((HStack)(ref val)).Take(112f), k, (GameFont)0, Tokens.Muted, (TextAnchor)3);
		Draw.EllipsisLabel(((HStack)(ref val)).Rest, v, (GameFont)0, Tokens.Text, (TextAnchor)3);
	}
}
internal sealed class BenchTextures : IBench
{
	private const int HeatRows = 12;

	private const int HeatCols = 44;

	private const int SectorSize = 128;

	private float[,] heat;

	private int heatVersion;

	private int sectorVersion;

	private float clock;

	public bool Scrollable => true;

	public float Height(float width)
	{
		return 880f;
	}

	public void Render(Rect area)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		clock += Time.deltaTime;
		if (heat == null)
		{
			Rebuild();
		}
		VStack val = Layout.Column(area, 16f);
		Rect val2 = Atoms.SectionFrame(((VStack)(ref val)).Take(118f), "真渐变 vs 分段（A/B 对照）");
		HStack val3 = Layout.Row(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, ((Rect)(ref val2)).width, 40f), 16f);
		Rect val4 = ((HStack)(ref val3)).Take((((Rect)(ref val2)).width - 16f) * 0.5f);
		Rect rest = ((HStack)(ref val3)).Rest;
		VisualAtoms.GradientBar(val4, Tokens.Accent, Tokens.Info, 16);
		Draw.Border(val4, Tokens.BorderHair);
		Draw.Label(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).yMax + 2f, ((Rect)(ref val4)).width, 16f), "VisualAtoms.GradientBar（16 段折线）", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		TexAtoms.GradientRect(rest, Tokens.Accent, Tokens.Info, true);
		Draw.Border(rest, Tokens.BorderHair);
		Draw.Label(new Rect(((Rect)(ref rest)).x, ((Rect)(ref rest)).yMax + 2f, ((Rect)(ref rest)).width, 16f), "TexAtoms.GradientRect（贴图，1 draw call）", (GameFont)0, Tokens.Alive, (TextAnchor)3);
		Draw.Label(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).yMax - 16f, ((Rect)(ref val2)).width, 16f), "放大到全宽看台阶：分段版可见 16 段色阶，贴图版连续", (GameFont)0, Tokens.Dim, (TextAnchor)3);
		Rect val5 = Atoms.SectionFrame(((VStack)(ref val)).Take(106f), "真渐隐遮罩（FadeRight）");
		Rect val6 = default(Rect);
		((Rect)(ref val6))..ctor(((Rect)(ref val5)).x, ((Rect)(ref val5)).y, ((Rect)(ref val5)).width, 26f);
		Draw.Box(val6, Tokens.PanelRaised);
		Draw.Label(new Rect(((Rect)(ref val6)).x + 4f, ((Rect)(ref val6)).y, ((Rect)(ref val6)).width - 8f, ((Rect)(ref val6)).height), "殖民地在第 12 象遭遇持续三天的有毒尘埃，所有外出作业暂停，储藏区的草药开始腐烂", (GameFont)1, Tokens.Text, (TextAnchor)3);
		TexAtoms.FadeRight(val6, Tokens.PanelRaised, 90f);
		Draw.Label(new Rect(((Rect)(ref val5)).x, ((Rect)(ref val6)).yMax + 2f, ((Rect)(ref val5)).width, 16f), "渐隐带 RGB 必须等于背后真实底色（此处 Tokens.PanelRaised），否则会出现接缝", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		Rect val7 = Atoms.SectionFrame(((VStack)(ref val)).Take(192f), "热力格网（TexAtoms.Heat · 单次 draw call）");
		Rect val8 = default(Rect);
		((Rect)(ref val8))..ctor(((Rect)(ref val7)).x, ((Rect)(ref val7)).y, ((Rect)(ref val7)).width, 144f);
		TexAtoms.Heat(val8, heatVersion, heat, 0f, 1f, true);
		Draw.Border(val8, Tokens.BorderHair);
		HStack val9 = Layout.Row(new Rect(((Rect)(ref val7)).x, ((Rect)(ref val8)).yMax + 4f, ((Rect)(ref val7)).width, 22f), 12f);
		if (Widgets.ButtonText(((HStack)(ref val9)).Take(120f), "重算数据（版本 +1）", true, true, true, (TextAnchor?)null))
		{
			Rebuild();
		}
		Draw.Label(((HStack)(ref val9)).Rest, "版本 " + heatVersion + " · " + 12 + "×" + 44 + " = " + 528 + " 格，仅 1 次 GUI.DrawTexture（改版本号才重算像素）", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		Rect val10 = Atoms.SectionFrame(((VStack)(ref val)).Take(222f), "扇环（TexAtoms.Sector）· 饼图与环图同一原语");
		float num = 150f;
		Rect val11 = default(Rect);
		((Rect)(ref val11))..ctor(((Rect)(ref val10)).x, ((Rect)(ref val10)).y, num, num);
		Rect val12 = default(Rect);
		((Rect)(ref val12))..ctor(((Rect)(ref val11)).xMax + 40f, ((Rect)(ref val11)).y, num, num);
		Rect val13 = default(Rect);
		((Rect)(ref val13))..ctor(((Rect)(ref val12)).xMax + 30f, ((Rect)(ref val11)).y + 20f, Mathf.Max(60f, ((Rect)(ref val10)).xMax - ((Rect)(ref val12)).xMax - 34f), 16f);
		float[] array = new float[3] { 0.42f, 0.31f, 0.27f };
		Color[] array2 = (Color[])(object)new Color[3]
		{
			Tokens.Accent,
			Tokens.Info,
			Tokens.Alive
		};
		string[] array3 = new string[3] { "建造", "采矿", "手工" };
		float num2 = 0f;
		VStack val14 = Layout.Column(val13, 4f);
		for (int i = 0; i < array.Length; i++)
		{
			float num3 = array[i] * 360f;
			int num4 = 1000 + i * 7 + sectorVersion;
			long num5 = TexCache.KeyVersioned(81, num4, 128, 128);
			TexAtoms.Sector(val11, num5, num2, num3, 0f, array2[i], 128);
			long num6 = TexCache.KeyVersioned(82, num4, 128, 128);
			TexAtoms.Sector(val12, num6, num2, num3, 0.55f, array2[i], 128);
			num2 += num3;
			Rect val15 = ((VStack)(ref val14)).Take(18f);
			HStack val16 = Layout.Row(val15, 6f);
			Draw.Box(((HStack)(ref val16)).Take(12f), array2[i]);
			Draw.Label(((HStack)(ref val16)).Rest, array3[i] + " " + Mathf.RoundToInt(array[i] * 100f) + "%", (GameFont)0, Tokens.Text, (TextAnchor)3);
		}
		HStack val17 = Layout.Row(new Rect(((Rect)(ref val10)).x, ((Rect)(ref val11)).yMax + 2f, ((Rect)(ref val10)).width, 22f), 12f);
		if (Widgets.ButtonText(((HStack)(ref val17)).Take(110f), "换一组角度", true, true, true, (TextAnchor?)null))
		{
			sectorVersion++;
		}
		Draw.Label(((HStack)(ref val17)).Rest, "缓存键把量化角度编进 version ⇒ 角度变了才重建贴图", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		Draw.Label(new Rect(((Rect)(ref val11)).x, ((Rect)(ref val11)).yMax + 2f, num, 16f), "饼  inner=0", (GameFont)0, Tokens.Dim, (TextAnchor)3);
		Draw.Label(new Rect(((Rect)(ref val12)).x, ((Rect)(ref val12)).yMax + 2f, num, 16f), "环  inner=0.55", (GameFont)0, Tokens.Dim, (TextAnchor)3);
	}

	private void Rebuild()
	{
		heat = new float[12, 44];
		for (int i = 0; i < 12; i++)
		{
			for (int j = 0; j < 44; j++)
			{
				float num = 0.5f + 0.5f * Mathf.Sin((float)j * 0.28f + (float)i * 0.5f + clock * 0.2f);
				heat[i, j] = Mathf.Clamp01(num * 0.75f + Random.value * 0.25f);
			}
		}
		heatVersion++;
	}
}
internal sealed class BenchHighlight : IBench
{
	private const string Tag = "RUIF.Demo.Highlight";

	private int markedFrame = -1;

	public bool Scrollable => true;

	public float Height(float width)
	{
		return 300f;
	}

	public void Render(Rect area)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		if (markedFrame < 0)
		{
			HiAtoms.Tag("RUIF.Demo.Highlight");
			markedFrame = Time.frameCount;
		}
		VStack val = Layout.Column(area, 16f);
		Rect val2 = Atoms.SectionFrame(((VStack)(ref val)).Take(196f), "脉冲高亮（HiAtoms · 桥 Verse.UIHighlighter）");
		VStack val3 = Layout.Column(val2, 6f);
		Draw.Label(((VStack)(ref val3)).Take(18f), "原版协议是两帧协作：本帧 Tag 登记 → 下一帧 Opportunity 才绘制。", (GameFont)0, Tokens.Text, (TextAnchor)3);
		Draw.Label(((VStack)(ref val3)).Take(18f), "单独调 Opportunity 不会有任何效果（判定条件是 frameCount == 登记帧 + 1）。", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		HStack val4 = Layout.Row(((VStack)(ref val3)).Take(26f), 12f);
		if (Widgets.ButtonText(((HStack)(ref val4)).Take(150f), "重新登记高亮", true, true, true, (TextAnchor?)null))
		{
			HiAtoms.Tag("RUIF.Demo.Highlight");
			markedFrame = Time.frameCount;
		}
		Draw.Label(((HStack)(ref val4)).Rest, "当前：登记帧 " + markedFrame + " · 现在 " + Time.frameCount, (GameFont)0, (Time.frameCount == markedFrame + 1) ? Tokens.Alive : Tokens.Dim, (TextAnchor)3);
		Rect val5 = ((VStack)(ref val3)).Take(56f);
		((Rect)(ref val5))..ctor(((Rect)(ref val5)).x, ((Rect)(ref val5)).y, Mathf.Min(320f, ((Rect)(ref val5)).width), 48f);
		Draw.Box(val5, Tokens.PanelRaised);
		Draw.Border(val5, Tokens.BorderSoft);
		Draw.Label(val5, "\u3000这里是一个「新功能」目标控件", (GameFont)1, Tokens.Text, (TextAnchor)3);
		HiAtoms.Opportunity(val5, "RUIF.Demo.Highlight");
	}
}
internal sealed class BenchAtoms42 : IBench
{
	private sealed class Atom
	{
		public string No;

		public string Name;

		public string Api;

		public string Desc = "";

		public string Note = "";

		public string Badge = "ok";

		public int DescLines = 1;

		public float BodyH = 26f;

		public float MinW = 60f;

		public float MinH = LineTiny;

		public bool Interactive;

		public Action<Rect> Paint;
	}

	private sealed class Family
	{
		public string Title;

		public string Sub;

		public readonly List<Atom> Items = new List<Atom>();

		public void Add(Atom a)
		{
			Items.Add(a);
		}
	}

	private const float CardGap = 14f;

	private const float PadX = 12f;

	private const float DescTop = 9f;

	private const float DemoPad = 11f;

	private const float DemoMT = 10f;

	private const float DemoMB = 12f;

	private const float NoteGap = 7f;

	private const float Border = 2f;

	private static readonly float LineTiny = Measure.RowHeight((GameFont)0);

	private static readonly float LineSmall = Measure.RowHeight((GameFont)1);

	private static readonly float CtrlTiny = Measure.ControlHeight((GameFont)0);

	private static readonly float HeadH = 9f + LineSmall + 2f + LineTiny;

	private const float CardUnit = 414f;

	private static readonly Color DemoBg = new Color(Tokens.Window.r * 0.55f + Tokens.Panel.r * 0.45f, Tokens.Window.g * 0.55f + Tokens.Panel.g * 0.45f, Tokens.Window.b * 0.55f + Tokens.Panel.b * 0.45f);

	private readonly List<Family> families = new List<Family>();

	private readonly List<Atom> cards = new List<Atom>();

	private readonly HashSet<string> failed = new HashSet<string>();

	internal static readonly string[] TableColumns = new string[4] { "名字", "心情", "工时", "击杀" };

	private readonly SortState[] sortStates = (SortState[])(object)new SortState[TableColumns.Length];

	private float t;

	private int numMode;

	private float typeProgress;

	private float swatchSel;

	private bool pinA;

	private bool pinB;

	private bool pinC;

	private bool eyeOn;

	private bool iconOn;

	private bool caretOpen;

	private bool moreBelow = true;

	private RowDensity density = (RowDensity)1;

	private float resizeX = 0.42f;

	private float zoom = 0.86f;

	private float wipe;

	private float flash;

	private bool gripOn;

	private float spinP;

	private bool spinDone;

	private int tsMode;

	private int glyphIdx;

	private int aspectIdx = 1;

	private float gradHover = -1f;

	private int alertLv = 2;

	private float planW = -1f;

	private int planCols;

	private float planTotal;

	private const float SecH = 26f;

	internal static readonly string[] WorkTypes = new string[20]
	{
		"建造", "烹饪", "制作", "医生", "灭火", "种植", "驯兽", "搬运", "狩猎", "采矿",
		"研究", "锻造", "缝制", "监管", "清洁", "艺术", "割除", "基本", "就医", "休养"
	};

	internal static readonly string[] Names = new string[6] { "蓝 雪牛", "赤 眼镜蛇", "青 大地懒", "素 野猪", "赫 松鼠", "翠 羊驼" };

	private readonly float[] cellMood = new float[6] { 62f, 38f, 88f, 51f, 74f, 45f };

	private readonly int[] cellKill = new int[6] { 3, 17, 0, 9, 5, 12 };

	private readonly int[] cellWork = new int[6] { 18400, 42100, 9700, 26300, 33900, 15200 };

	private int budgetSpent = 72;

	private int ovIdx;

	private int ruleIdx;

	private string shQuery = "建造";

	private bool insetRaised;

	private int researchIdx;

	private int anchorIdx = 4;

	private int cornerIdx;

	private readonly bool[] secOpen = new bool[3] { true, false, true };

	private float thinScroll;

	private float frozenScroll;

	internal static readonly string[] EventWords = new string[12]
	{
		"袭击", "寒流", "虫灾", "贸易商队", "太阳耀斑", "极光", "日蚀", "枯萎病", "猎杀人类", "心灵低语",
		"陨石撞击", "家养动物加入"
	};

	internal static readonly string[] Research = new string[8] { "电力", "蓄电池", "水栽培", "空调", "枪械制作", "锻造", "岩石切割", "酿酒" };

	private int segIdx;

	private bool tglOn;

	private int stepVal = 12;

	private float sliderVal = 0.62f;

	private bool keyArmed;

	private readonly bool[] tagPicked = new bool[8] { true, false, false, true, false, false, false, false };

	private float dsLo = 25f;

	private float dsHi = 70f;

	private Color cpColor = new Color(0.35f, 0.71f, 0.89f);

	private float cpH = 0.573f;

	private float cpS = 0.61f;

	private float cpV = 0.89f;

	private int cycIdx;

	private string editText = "迫降乡";

	private string editBuffer = "迫降乡";

	private bool editing;

	private readonly int[] pgVals = new int[12]
	{
		3, 2, 1, 1, 2, 4, 0, 1, 1, 1,
		3, 2
	};

	private readonly int[] ttStates = new int[24];

	private float ticksVal = 9f;

	private readonly float[] batchVals = new float[3] { 21f, 64f, 78f };

	private readonly SliderCfg[] batchCfg = (SliderCfg[])(object)new SliderCfg[3]
	{
		new SliderCfg
		{
			label = "温度上限",
			min = -10f,
			max = 40f,
			unit = "°C"
		},
		new SliderCfg
		{
			label = "亮度",
			min = 0f,
			max = 100f,
			unit = "%"
		},
		new SliderCfg
		{
			label = "音量",
			min = 0f,
			max = 100f,
			unit = "%"
		}
	};

	private static readonly string[] SkillRanks = new string[21]
	{
		"一窍不通", "不明就里", "初学乍练", "一知半解", "初窥门径", "恍然大悟", "略有小成", "渐入佳境", "得心应手", "出类拔萃",
		"游刃有馀", "良工巧匠", "别具匠心", "炉火纯青", "一代大师", "登峰造极", "蜚声内外", "一代宗师", "蜚声寰宇", "举世无双",
		"点石成金"
	};

	private int keyDown;

	private int badgeSlot;

	private int avatarSel = -1;

	private int qualitySel = -1;

	private bool skelLoaded;

	private string sfQuery = "";

	private string sfBuffer = "";

	private float sfLast;

	private bool pcSelected;

	private bool sbOneCol;

	private static readonly string[] SkillNames = new string[5] { "射击", "近战", "建造", "采矿", "烹饪" };

	private static readonly float[] SkillBase = new float[5] { 14f, 6f, 18f, 9f, 11f };

	private bool npFew;

	private static readonly string[] NeedNames = new string[4] { "心情", "饮食", "休息", "娱乐" };

	private static readonly float[] NeedValues = new float[4] { 0.82f, 0.64f, 0.91f, 0.23f };

	private static readonly string[] NeedNamesFew = new string[2] { "心情", "娱乐" };

	private static readonly float[] NeedValuesFew = new float[2] { 0.82f, 0.23f };

	public bool Scrollable => true;

	public float Height(float width)
	{
		Plan(width);
		return planTotal + 2f;
	}

	private void Plan(float width)
	{
		if (Mathf.Abs(planW - width) < 0.5f)
		{
			return;
		}
		planW = width;
		planCols = Mathf.Clamp(Mathf.FloorToInt((width + 14f) / 414f), 1, 3);
		float cardW = (width - 14f * (float)(planCols - 1)) / (float)planCols;
		float num = 0f;
		for (int i = 0; i < families.Count; i++)
		{
			Family family = families[i];
			num += 26f + LineTiny + (string.IsNullOrEmpty(family.Sub) ? 0f : LineTiny) + 6f;
			int num2 = Mathf.CeilToInt((float)family.Items.Count / (float)planCols);
			for (int j = 0; j < num2; j++)
			{
				float num3 = 0f;
				for (int k = j * planCols; k < Mathf.Min((j + 1) * planCols, family.Items.Count); k++)
				{
					num3 = Mathf.Max(num3, CardH(family.Items[k], cardW));
				}
				num += num3 + 14f;
			}
			num += 10f;
		}
		planTotal = num;
	}

	private static float CardH(Atom a, float cardW)
	{
		float num = (string.IsNullOrEmpty(a.Desc) ? 0f : (9f + (float)a.DescLines * LineTiny));
		float num2 = (string.IsNullOrEmpty(a.Note) ? 0f : (7f + LineTiny));
		return 2f + HeadH + num + 10f + (22f + a.BodyH + num2) + 12f;
	}

	public BenchAtoms42()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		Build();
		for (int i = 0; i < families.Count; i++)
		{
			for (int j = 0; j < families[i].Items.Count; j++)
			{
				cards.Add(families[i].Items[j]);
			}
		}
	}

	public void Render(Rect area)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		t += Time.deltaTime;
		typeProgress = Mathf.Repeat(typeProgress + Time.deltaTime * 0.22f, 1.35f);
		wipe = Mathf.Repeat(wipe + Time.deltaTime * 0.16f, 1.05f);
		flash = Mathf.Repeat(flash + Time.deltaTime * 0.9f, 1f);
		if (!spinDone)
		{
			spinP = Mathf.Min(100f, spinP + Time.deltaTime * 7f);
			if (spinP >= 100f)
			{
				spinDone = true;
			}
		}
		Plan(((Rect)(ref area)).width);
		float num = (((Rect)(ref area)).width - 14f * (float)(planCols - 1)) / (float)planCols;
		float num2 = ((Rect)(ref area)).y;
		Rect card = default(Rect);
		for (int i = 0; i < families.Count; i++)
		{
			Family family = families[i];
			Draw.Label(new Rect(((Rect)(ref area)).x, num2, ((Rect)(ref area)).width - 120f, 26f), family.Title, (GameFont)1, Tokens.Accent, (TextAnchor)3);
			Draw.EllipsisLabel(new Rect(((Rect)(ref area)).xMax - 120f, num2, 120f, 26f), ((family.Items.Count < 10) ? "0" : "") + family.Items[0].No + "–" + family.Items[family.Items.Count - 1].No, (GameFont)0, Tokens.Dim, (TextAnchor)5);
			num2 += 26f;
			Draw.LineH(((Rect)(ref area)).x, num2, ((Rect)(ref area)).width, Tokens.Border);
			num2 += LineTiny;
			if (!string.IsNullOrEmpty(family.Sub))
			{
				Draw.EllipsisLabel(new Rect(((Rect)(ref area)).x, num2, ((Rect)(ref area)).width, LineTiny), family.Sub, (GameFont)0, Tokens.Muted, (TextAnchor)3);
				num2 += LineTiny;
			}
			num2 += 6f;
			int num3 = Mathf.CeilToInt((float)family.Items.Count / (float)planCols);
			for (int j = 0; j < num3; j++)
			{
				int num4 = j * planCols;
				int num5 = Mathf.Min(num4 + planCols, family.Items.Count);
				float num6 = 0f;
				for (int k = num4; k < num5; k++)
				{
					num6 = Mathf.Max(num6, CardH(family.Items[k], num));
				}
				for (int l = num4; l < num5; l++)
				{
					((Rect)(ref card))..ctor(((Rect)(ref area)).x + (float)(l - num4) * (num + 14f), num2, num, CardH(family.Items[l], num));
					PaintCard(card, family.Items[l]);
				}
				num2 += num6 + 14f;
			}
			num2 += 10f;
		}
	}

	private void PaintCard(Rect card, Atom a)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Mouse.IsOver(card);
		Draw.Box(card, flag ? Tokens.CardHover : Tokens.Panel);
		Draw.Border(card, flag ? Tokens.Accent : Tokens.Border);
		Rect val = default(Rect);
		((Rect)(ref val))..ctor(((Rect)(ref card)).x + 1f, ((Rect)(ref card)).y + 1f, ((Rect)(ref card)).width - 2f, HeadH);
		Draw.Box(val, Tokens.PanelRaised);
		Draw.LineH(((Rect)(ref val)).x, ((Rect)(ref val)).yMax - 1f, ((Rect)(ref val)).width, Tokens.Border);
		Rect val2 = default(Rect);
		((Rect)(ref val2))..ctor(((Rect)(ref val)).x + 12f, ((Rect)(ref val)).y + 9f, 26f, LineTiny + 1f);
		Draw.Box(val2, Tokens.Card);
		Draw.Border(val2, failed.Contains(a.No) ? Tokens.Dead : Tokens.BorderSoft);
		Draw.Label(val2, a.No, (GameFont)0, failed.Contains(a.No) ? Tokens.Dead : Tokens.Dim, (TextAnchor)4);
		Rect val3 = default(Rect);
		((Rect)(ref val3))..ctor(((Rect)(ref val)).xMax - 12f - 34f, ((Rect)(ref val)).y + 9f, 34f, LineTiny + 1f);
		Draw.Box(val3, (a.Badge == "ok") ? Draw.WithAlpha(Tokens.Alive, 0.14f) : Draw.WithAlpha(Tokens.Warn, 0.14f));
		Draw.Border(val3, (a.Badge == "ok") ? Tokens.Alive : Tokens.Warn);
		Draw.Label(val3, (a.Badge == "ok") ? "✅" : "⚠\ufe0f", (GameFont)0, (a.Badge == "ok") ? Tokens.Alive : Tokens.Warn, (TextAnchor)4);
		Draw.EllipsisLabel(new Rect(((Rect)(ref val2)).xMax + 5f, ((Rect)(ref val)).y + 9f, ((Rect)(ref val3)).x - ((Rect)(ref val2)).xMax - 10f, LineSmall), a.Name, (GameFont)1, Tokens.Text, (TextAnchor)3);
		Draw.EllipsisLabel(new Rect(((Rect)(ref val)).x + 12f, ((Rect)(ref val)).y + 9f + LineSmall + 2f, ((Rect)(ref val)).width - 24f, LineTiny), a.Api, (GameFont)0, Tokens.Info, (TextAnchor)3);
		float num = ((Rect)(ref val)).yMax;
		if (!string.IsNullOrEmpty(a.Desc))
		{
			Draw.EllipsisLabel(new Rect(((Rect)(ref card)).x + 12f, num + 9f, ((Rect)(ref card)).width - 24f, (float)a.DescLines * LineTiny), a.Desc, (GameFont)0, Tokens.Muted, (TextAnchor)0);
			num += 9f + (float)a.DescLines * LineTiny;
		}
		float num2 = (string.IsNullOrEmpty(a.Note) ? 0f : (7f + LineTiny));
		Rect val4 = default(Rect);
		((Rect)(ref val4))..ctor(((Rect)(ref card)).x + 12f, num + 10f, ((Rect)(ref card)).width - 24f, 22f + a.BodyH + num2);
		Draw.Box(val4, DemoBg);
		Draw.Border(val4, (a.Interactive && Mouse.IsOver(val4)) ? Tokens.Accent : Tokens.BorderSoft);
		Rect val5 = default(Rect);
		((Rect)(ref val5))..ctor(((Rect)(ref val4)).x + 11f, ((Rect)(ref val4)).y + 11f, ((Rect)(ref val4)).width - 22f, a.BodyH);
		if (((Rect)(ref val5)).width > 6f)
		{
			if (((Rect)(ref val5)).width < a.MinW || ((Rect)(ref val5)).height < a.MinH)
			{
				PaintTooSmall(val5, a);
			}
			else if (failed.Contains(a.No))
			{
				PaintFailed(val5, a);
			}
			else
			{
				try
				{
					if (a.Paint != null)
					{
						a.Paint(val5);
					}
				}
				catch (Exception ex)
				{
					failed.Add(a.No);
					Log.ErrorOnce("[RUIF_Demo] 原子 " + a.No + " " + a.Name + " 的演示抛异常，已隔离（其余卡片不受影响）：\n" + ex, ("ruif.atom." + a.No).GetHashCode());
					PaintFailed(val5, a);
				}
			}
		}
		if (num2 > 0f)
		{
			Draw.EllipsisLabel(new Rect(((Rect)(ref val5)).x, ((Rect)(ref val5)).yMax + 7f, ((Rect)(ref val5)).width, LineTiny), a.Note, (GameFont)0, Tokens.Dim, (TextAnchor)3);
		}
	}

	private static void PaintFailed(Rect body, Atom a)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Draw.Border(body, Tokens.Dead);
		Draw.EllipsisLabel(body, "⚠ " + a.No + " 演示异常 · 已隔离 · 详见日志", (GameFont)0, Tokens.Dead, (TextAnchor)4);
	}

	private static void PaintTooSmall(Rect body, Atom a)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Draw.Border(body, Tokens.BorderSoft);
		Draw.EllipsisLabel(body, "— 空间不足 · " + a.No + " 需 ≥ " + Mathf.RoundToInt(a.MinW) + "×" + Mathf.RoundToInt(a.MinH), (GameFont)0, Tokens.Dim, (TextAnchor)4);
	}

	private static void KBar(Rect rect, float p01, Color? fill = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Draw.Box(rect, Tokens.BorderHair);
		Draw.Border(rect, Tokens.BorderSoft);
		float num = Mathf.Max(0f, (((Rect)(ref rect)).width - 2f) * Mathf.Clamp01(p01));
		if (num > 0.5f)
		{
			Draw.Box(new Rect(((Rect)(ref rect)).x + 1f, ((Rect)(ref rect)).y + 1f, num, ((Rect)(ref rect)).height - 2f), (Color)(((??)fill) ?? Tokens.Accent));
		}
	}

	private void Build()
	{
		BuildText();
		BuildGeo();
		BuildStatus();
		BuildVisual();
		BuildTable();
		BuildPanel();
		BuildInteract();
		BuildProgress();
		BuildMisc();
		BuildExt();
		BuildBase1();
		BuildDInput();
		BuildDData();
	}

	private void BuildText()
	{
		Family family = new Family
		{
			Title = "A0 · 原子层 —— 文本原语",
			Sub = "6 件 · TextFmt / TextAtoms（官方依据：Verse GenText / GenDate / Text.CalcSize）"
		};
		family.Add(new Atom
		{
			No = "01",
			Name = "FormattedNumber",
			Api = "TextFmt.FormatNumber(double) → string",
			Interactive = true,
			Desc = "千分位与单位自适应缩写（12 480 → 12.5k）。官方依据：Verse GenText 工具族。",
			Note = "库存实时增长（钢铁 / 白银 / 零部件）· 切换格式档看同一数值的三种排法",
			BodyH = 26f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				HStack val6 = Layout.Row(r, 6f);
				HStack val7 = Layout.Row(((HStack)(ref val6)).Take(((Rect)(ref r)).width - 150f), 6f);
				double[] array2 = new double[4] { 12480.0, 284312.0, 469.0, 1400000.0 };
				for (int i = 0; i < array2.Length; i++)
				{
					Rect rest3 = ((HStack)(ref val7)).Rest;
					Rect val8 = ((HStack)(ref val7)).Take(Mathf.Min(((Rect)(ref rest3)).width, 88f));
					Draw.Box(val8, Tokens.Card);
					Draw.Border(val8, Tokens.BorderSoft);
					string text = ((numMode == 2) ? ((long)array2[i]).ToString() : ((numMode == 1) ? ((long)array2[i]).ToString("N0") : TextFmt.FormatNumber(array2[i])));
					Draw.EllipsisLabel(val8, text, (GameFont)1, Tokens.Accent, (TextAnchor)4);
				}
				InputAtoms.Segmented(Isset(((HStack)(ref val6)).Rest, 0f, CtrlTiny), new string[3] { "缩写", "千分位", "原值" }, ref numMode);
			}
		});
		family.Add(new Atom
		{
			No = "02",
			Name = "TimeStamp",
			Api = "TextAtoms.DateFull(long, Vector3) + NavFmt.RelativeTime(float)",
			Desc = "刻 → 游戏内日期时间 / 相对时间（「3 天之前」）。全框架统一时间语义。",
			Note = "随时钟推进——相对档会从「刚刚」变成「1 小时之前」",
			BodyH = CtrlTiny + 5f + LineTiny,
			Paint = delegate(Rect r)
			{
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_016c: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				long num3 = ((Find.TickManager != null) ? Find.TickManager.TicksGame : 0);
				VStack val4 = Layout.Column(r, 5f);
				InputAtoms.Segmented(((VStack)(ref val4)).Take(CtrlTiny), new string[3] { "绝对", "相对", "混合" }, ref tsMode);
				Rect rest2 = ((VStack)(ref val4)).Rest;
				HStack val5 = Layout.Row(((VStack)(ref val4)).Take(((Rect)(ref rest2)).height), 10f);
				if (tsMode == 0)
				{
					Draw.EllipsisLabel(((HStack)(ref val5)).Take(200f), TextAtoms.DateFull(num3, Vector3.zero), (GameFont)1, Tokens.Text, (TextAnchor)3);
					Draw.EllipsisLabel(((HStack)(ref val5)).Rest, "1 天 = 60000 tick（官方口径）", (GameFont)0, Tokens.Dim, (TextAnchor)3);
				}
				else if (tsMode == 1)
				{
					Draw.EllipsisLabel(((HStack)(ref val5)).Take(78f), NavFmt.RelativeTime(42f), (GameFont)0, Tokens.Info, (TextAnchor)3);
					Draw.EllipsisLabel(((HStack)(ref val5)).Take(78f), NavFmt.RelativeTime(3700f), (GameFont)0, Tokens.Info, (TextAnchor)3);
					Draw.EllipsisLabel(((HStack)(ref val5)).Rest, NavFmt.RelativeTime(260000f), (GameFont)0, Tokens.Muted, (TextAnchor)3);
				}
				else
				{
					Draw.EllipsisLabel(((HStack)(ref val5)).Take(200f), TextAtoms.DateFull(num3, Vector3.zero), (GameFont)1, Tokens.Text, (TextAnchor)3);
					Draw.EllipsisLabel(((HStack)(ref val5)).Rest, NavFmt.RelativeTime(42f), (GameFont)0, Tokens.Info, (TextAnchor)3);
				}
			}
		});
		family.Add(new Atom
		{
			No = "03",
			Name = "UnitLabel",
			Api = "TextAtoms.UnitLabel(Rect, string, string, GameFont, Color?)",
			Desc = "数值 + 单位对齐排版（数值 Medium 右对齐、单位 Tiny 灰）。KPI 卡与电力条的内嵌原子。",
			Note = "三行共用同一右边界——单位列宽由最长单位撑开，不是各排各的",
			BodyH = 92f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				VStack val3 = Layout.Column(r, 2f);
				TextAtoms.UnitLabel(((VStack)(ref val3)).Take(Measure.RowHeight(Tokens.FontValue)), "12 480", "件", Tokens.FontValue, (Color?)Tokens.Accent);
				TextAtoms.UnitLabel(((VStack)(ref val3)).Take(Measure.RowHeight(Tokens.FontValue)), "469", "W", Tokens.FontValue, (Color?)Tokens.Info);
				TextAtoms.UnitLabel(((VStack)(ref val3)).Take(Measure.RowHeight(Tokens.FontValue)), "86", "%", Tokens.FontValue, (Color?)Tokens.Alive);
			}
		});
		family.Add(new Atom
		{
			No = "04",
			Name = "RichTextLine 内联混排",
			Api = "TextAtoms.RichTextLine(Rect, RichSpan[], TextAnchor, GameFont) → float",
			Desc = "单行混排：变色词 / 加粗词 / 内嵌小图标。超越标签能力的样式按 span 分段拼接绘制。",
			Note = "RichSpan(text, Color?, GameFont?) · 返回已消费高度",
			BodyH = 62f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_0151: Unknown result type (might be due to invalid IL or missing references)
				//IL_0156: Unknown result type (might be due to invalid IL or missing references)
				//IL_0162: Unknown result type (might be due to invalid IL or missing references)
				//IL_0175: Unknown result type (might be due to invalid IL or missing references)
				//IL_017a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				TextAtoms.RichTextLine(Isset(r, 0f, 30f), (RichSpan[])(object)new RichSpan[5]
				{
					new RichSpan("蓝 雪牛 ", (Color?)Tokens.Accent, (GameFont?)null),
					new RichSpan("完成了 ", (Color?)Tokens.Text, (GameFont?)null),
					new RichSpan("大师级", (Color?)Tokens.Warn, (GameFont?)Tokens.FontValue),
					new RichSpan(" 突击步枪 ", (Color?)Tokens.Text, (GameFont?)null),
					new RichSpan("声望 +2", (Color?)Tokens.Alive, (GameFont?)null)
				}, (TextAnchor)3, Tokens.FontBody);
				TextAtoms.RichTextLine(Isset(r, 31f, 30f), (RichSpan[])(object)new RichSpan[4]
				{
					new RichSpan("赤 眼镜蛇 ", (Color?)Tokens.Accent, (GameFont?)null),
					new RichSpan("在裁缝台上做出了 ", (Color?)Tokens.Text, (GameFont?)null),
					new RichSpan("传奇级", (Color?)Tokens.Warn, (GameFont?)Tokens.FontValue),
					new RichSpan(" 布制大衣", (Color?)Tokens.Text, (GameFont?)null)
				}, (TextAnchor)3, Tokens.FontBody);
			}
		});
		family.Add(new Atom
		{
			No = "05",
			Name = "Typewriter 逐字浮现",
			Api = "TextAtoms.Typewriter(Rect, string, float, GameFont, Color, TextAnchor)",
			Desc = "按进度逐字显示——事件叙事与信件的开场仪式感。点击即显全文（跳过）。",
			Note = "进度随真实时间推进 · 跳过 / 重播（交互回补）",
			BodyH = 54f + CtrlTiny + 4f,
			Paint = delegate(Rect r)
			{
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0146: Unknown result type (might be due to invalid IL or missing references)
				string[] array = new string[2] { "殖民地第一天，没有墙，也没有床，只有一堆迫降残骸和四张惊恐的脸。", "医疗帐篷里油灯昏黄，医生用最后一株草药保住了病人的腿。" };
				VStack val = Layout.Column(r, 4f);
				int num = Mathf.FloorToInt(typeProgress);
				float num2 = typeProgress - (float)num;
				TextAtoms.Typewriter(((VStack)(ref val)).Take(54f), array[num % array.Length], Mathf.Clamp01(num2), Tokens.FontBody, Tokens.Text, (TextAnchor)0);
				Rect rest = ((VStack)(ref val)).Rest;
				HStack val2 = Layout.Row(((VStack)(ref val)).Take(((Rect)(ref rest)).height), 6f);
				if (Widgets.ButtonText(((HStack)(ref val2)).Take(64f), "跳过", true, true, true, (TextAnchor?)null))
				{
					typeProgress = 1.34f;
				}
				if (Widgets.ButtonText(((HStack)(ref val2)).Take(64f), "重播", true, true, true, (TextAnchor?)null))
				{
					typeProgress = 0f;
				}
				Draw.EllipsisLabel(((HStack)(ref val2)).Rest, "第 " + (num + 1) + " 段 · " + Mathf.RoundToInt(Mathf.Clamp01(num2) * 100f) + "%", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "06",
			Name = "SubtitleLine 主注双行",
			Api = "TextAtoms.SubtitleLine(Rect, string main, string sub)",
			Desc = "主文案 + 来源/翻译小注，两行同宽基线对齐（本地化场景：译文 + 原文对照）。",
			Note = "职称档位与英文原文对照 · 悬停看原文由调用方实现",
			BodyH = 78f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				TextAtoms.SubtitleLine(Isset(r, 0f, 38f), "总工程师 · 部落工匠", "职称档位 T4 · Chief Engineer");
				TextAtoms.SubtitleLine(Isset(r, 39f, 38f), "首席医师 · 医者", "职称档位 T3 · Chief Medical Officer");
			}
		});
		families.Add(family);
	}

	private void BuildGeo()
	{
		Family family = new Family
		{
			Title = "A0 · 原子层 —— 几何与装饰",
			Sub = "6 件 · Atoms / Shapes（零贴图实现）"
		};
		family.Add(new Atom
		{
			No = "07",
			Name = "SectionFrame 组框",
			Api = "Atoms.SectionFrame(Rect, string) → Rect（内容区）",
			Desc = "带标题的 fieldset 组框：标题嵌于上边线断口处。返回内容区矩形，调用方直接在返回区排版。",
			Note = "返回值即内容区（已含内边距）——不要再套一层 Inset",
			BodyH = 86f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				FontScope val5 = Draw.Font(Tokens.FontLabel);
				Rect val6;
				try
				{
					val6 = Atoms.SectionFrame(r, "采集规则 · 储存区");
				}
				finally
				{
					((IDisposable)(FontScope)(ref val5)).Dispose();
				}
				VStack val7 = Layout.Column(val6, 2f);
				Draw.EllipsisLabel(((VStack)(ref val7)).Take(LineTiny), "优先储存\u3000钢铁 / 零部件", (GameFont)0, Tokens.Text, (TextAnchor)3);
				Draw.EllipsisLabel(((VStack)(ref val7)).Take(LineTiny), "允许腐烂\u3000否", (GameFont)0, Tokens.Text, (TextAnchor)3);
				Draw.EllipsisLabel(((VStack)(ref val7)).Take(LineTiny), "优先级\u3000\u3000重要", (GameFont)0, Tokens.Warn, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "08",
			Name = "CornerFrame HUD 角框",
			Api = "Shapes.CornerFrame(Rect, Color, float armLen, float thickness)",
			Desc = "只画四个 L 角（各两段线）——指挥台 / 选中态 / 镜头框气质，比全边框更「军事 HUD」。",
			Note = "规格对齐预览 .corner{56px} + i{14px} · 悬停张开 / 点击切换目标（交互回补）",
			BodyH = 56f,
			Paint = delegate(Rect r)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				string[] array2 = new string[4] { "走廊防线\u3000HP 480 / 480", "储藏区 A\u3000库存 12 480", "太阳能阵列\u30001 100 W", "手术台\u3000医疗 91%" };
				bool flag = Mouse.IsOver(r);
				Shapes.CornerFrame(r, flag ? Tokens.Info : Tokens.Accent, flag ? 20f : 14f, 2f);
				Draw.EllipsisLabel(r, array2[Mathf.Clamp(cornerIdx, 0, array2.Length - 1)], (GameFont)0, flag ? Tokens.Text : Tokens.Muted, (TextAnchor)4);
				if (Widgets.ButtonInvisible(r, true))
				{
					cornerIdx = (cornerIdx + 1) % array2.Length;
				}
			}
		});
		family.Add(new Atom
		{
			No = "09",
			Name = "GradientRule 渐变分隔线",
			Api = "Shapes.GradientRule(Rect, Color, int segments)",
			Desc = "accent → 透明的渐隐分隔线。零贴图实现：8~12 段 alpha 递减折线拼接，纯原生。",
			Note = "预览 .gradrule{height:2px}；段数由调用方给定",
			BodyH = 30f,
			Paint = delegate(Rect r)
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				Shapes.GradientRule(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).center.y - 1f, ((Rect)(ref r)).width, 2f), Tokens.Accent, 12);
			}
		});
		family.Add(new Atom
		{
			No = "10",
			Name = "RuleWithLabel 带字分割线",
			Api = "Shapes.RuleWithLabel(Rect, string, Color, float gap)",
			Desc = "线中断字（── 历史记录 ──），长页面语义分段。线宽由 label 反推，两侧自动等长。",
			Note = "两段共用一个分段名列表 · 换一段（交互回补）",
			BodyH = 50f + CtrlTiny + 4f,
			Paint = delegate(Rect r)
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				string[] array = new string[5] { "历 史 记 录", "建 造 清 单", "战 斗 履 历", "研 究 队 列", "贸 易 记 录" };
				FontScope val3 = Draw.Font(Tokens.FontLabel);
				try
				{
					Shapes.RuleWithLabel(Isset(r, 2f, 18f), array[ruleIdx], Tokens.BorderSoft, 9f);
					Shapes.RuleWithLabel(Isset(r, 26f, 18f), "战 斗 履 历", Tokens.Accent, 9f);
				}
				finally
				{
					((IDisposable)(FontScope)(ref val3)).Dispose();
				}
				HStack val4 = Layout.Row(Isset(r, 50f, CtrlTiny), 8f);
				if (Widgets.ButtonText(((HStack)(ref val4)).Take(74f), "换一段", true, true, true, (TextAnchor?)null))
				{
					ruleIdx = (ruleIdx + 1) % array.Length;
				}
				Draw.EllipsisLabel(((HStack)(ref val4)).Rest, "换字 → 断口宽随之变化", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "11",
			Name = "Watermark 水印",
			Api = "Shapes.Watermark(Rect, string mark, float alpha)",
			Desc = "低透明度底纹徽记（空页面 / 封面页气质件）。绘制在内容之下，随主题令牌变色。",
			Note = "预览 alpha .06~.10 · 内容浮于水印之上",
			BodyH = 52f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				Shapes.InsetPanel(r);
				Shapes.Watermark(r, "PC", 0.1f);
				Draw.EllipsisLabel(r, "内容浮于水印之上", (GameFont)0, Tokens.Muted, (TextAnchor)4);
			}
		});
		family.Add(new Atom
		{
			No = "12",
			Name = "InsetPanel 凹陷面板",
			Api = "Shapes.InsetPanel(Rect)",
			Desc = "上暗下亮双描边拟物内嵌（输入区 / 列表容器常用），与 Panel（凸起）形成层级对偶。",
			Note = "描边方向即层级语义：凹陷 = 可输入，凸起 = 可点击 · 点框体切换（交互回补）",
			BodyH = 52f + CtrlTiny + 4f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				VStack val = Layout.Column(r, 4f);
				float lineTiny = LineTiny;
				Rect rest = ((VStack)(ref val)).Rest;
				Rect val2 = ((VStack)(ref val)).Take(Mathf.Max(lineTiny, ((Rect)(ref rest)).height - CtrlTiny - 4f));
				if (Widgets.ButtonInvisible(val2, true))
				{
					insetRaised = !insetRaised;
				}
				if (insetRaised)
				{
					Draw.Box(val2, Tokens.PanelRaised);
					Draw.Border(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, ((Rect)(ref val2)).width, 1f), Tokens.Card);
					Draw.Border(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).yMax - 1f, ((Rect)(ref val2)).width, 1f), Tokens.Border);
				}
				else
				{
					Shapes.InsetPanel(val2);
				}
				HoverFrame(val2);
				Draw.EllipsisLabel(val2, insetRaised ? "凸起 = 可点击" : "凹陷 = 可输入", (GameFont)0, Tokens.Text, (TextAnchor)4);
				Draw.EllipsisLabel(((VStack)(ref val)).Rest, insetRaised ? "当前：凸起" : "当前：凹陷", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		families.Add(family);
	}

	private static Rect Isset(Rect r, float dy, float h)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + dy, ((Rect)(ref r)).width, Mathf.Min(h, ((Rect)(ref r)).height - dy));
	}

	private static string Pick(params string[] a)
	{
		return a[Random.Range(0, a.Length)];
	}

	private void BuildTable()
	{
		Family family = new Family
		{
			Title = "A0+ · 表格原子族",
			Sub = "5 件 · TableAtoms / TableFmt（Numbers / Work Tab 实证）"
		};
		family.Add(new Atom
		{
			No = "24",
			Name = "SortHeaderCell 排序表头单元",
			Api = "TableAtoms.HeaderCell(Rect, string, SortState) → bool · TableFmt.NextSort",
			Interactive = true,
			Badge = "ok",
			Desc = "点表头循环排序（无 → 升 → 降 → 无），三角指示。返回 true 表示该次点击命中——这是真排序，下面这张表会跟着重排。",
			Note = "列宽 34% / 22% / 22% / 22%（对齐预览 .ttab 四列）· 数据每 6 秒刷新一次且保持当前排序",
			BodyH = 152f,
			Paint = delegate(Rect r)
			{
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Expected I4, but got Unknown
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_020b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0210: Unknown result type (might be due to invalid IL or missing references)
				//IL_021f: Unknown result type (might be due to invalid IL or missing references)
				//IL_022d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0246: Unknown result type (might be due to invalid IL or missing references)
				//IL_026c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0293: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
				float[] array3 = new float[4] { 0.34f, 0.22f, 0.22f, 0.22f };
				HStack val13 = Layout.Row(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), 0f);
				for (int n = 0; n < TableColumns.Length; n++)
				{
					Rect val14 = ((HStack)(ref val13)).Take(((Rect)(ref r)).width * array3[n]);
					if (TableAtoms.HeaderCell(val14, TableColumns[n], sortStates[n]))
					{
						sortStates[n] = (SortState)(int)TableFmt.NextSort(sortStates[n]);
					}
				}
				int num3 = -1;
				int dir = 0;
				for (int num4 = 0; num4 < sortStates.Length; num4++)
				{
					if (sortStates[num4])
					{
						num3 = num4;
						dir = (((int)sortStates[num4] == 1) ? 1 : (-1));
					}
				}
				int[] array4 = new int[6] { 0, 1, 2, 3, 4, 5 };
				if (num3 > 0)
				{
					float[] key = num3 switch
					{
						2 => Array.ConvertAll(cellWork, (Converter<int, float>)((int v) => v)), 
						1 => cellMood, 
						_ => Array.ConvertAll(cellKill, (Converter<int, float>)((int v) => v)), 
					};
					Array.Sort(array4, (int a, int b) => key[a].CompareTo(key[b]) * dir);
				}
				VStack val15 = Layout.Column(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 22f, ((Rect)(ref r)).width, ((Rect)(ref r)).height - 22f), 0f);
				foreach (int num6 in array4)
				{
					HStack val16 = Layout.Row(((VStack)(ref val15)).Take(21f), 0f);
					Draw.EllipsisLabel(((HStack)(ref val16)).Take(((Rect)(ref r)).width * array3[0]), Names[num6], (GameFont)0, Tokens.Text, (TextAnchor)3);
					TableAtoms.ValueCell(((HStack)(ref val16)).Take(((Rect)(ref r)).width * array3[1]), cellMood[num6], 100f);
					TableAtoms.ValueCell(((HStack)(ref val16)).Take(((Rect)(ref r)).width * array3[2]), (float)cellWork[num6], 45000f);
					TableAtoms.ValueCell(((HStack)(ref val16)).Take(((Rect)(ref r)).width * array3[3]), (float)cellKill[num6], 20f);
					Draw.LineH(((Rect)(ref r)).x, ((VStack)(ref val15)).Y - 1f, ((Rect)(ref r)).width, Tokens.BorderHair);
				}
			}
		});
		family.Add(new Atom
		{
			No = "25",
			Name = "ValueCell 阈值着色单元",
			Api = "TableAtoms.ValueCell(Rect, float value, float reference)",
			Desc = "数值按阈值映射底色（红 → 黄 → 绿三段），表格秒读异常行——Numbers 的核心可读性技巧。",
			Note = "值 / 参照值 → 三段映射：低 = red，中 = warn，高 = alive",
			BodyH = 24f,
			Paint = delegate(Rect r)
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				float[] array2 = new float[6] { 3f, 9f, 17f, 12f, 5f, 19f };
				HStack val11 = Layout.Row(r, 4f);
				for (int m = 0; m < array2.Length; m++)
				{
					Rect rest3 = ((HStack)(ref val11)).Rest;
					Rect val12 = ((HStack)(ref val11)).Take(Mathf.Min(58f, ((Rect)(ref rest3)).width));
					if (((Rect)(ref val12)).width < 20f)
					{
						break;
					}
					TableAtoms.ValueCell(val12, array2[m], 20f);
				}
			}
		});
		family.Add(new Atom
		{
			No = "26",
			Name = "ColumnResize 列宽手柄",
			Api = "TableAtoms.ResizeHandle(Rect area, float x) → bool · TableFmt.ResizeWidth",
			Interactive = true,
			Desc = "列间竖向手柄，按住横向拖动调列宽（宽度存列配置）。MouseDrag 事件 + clamp。",
			Note = "按住竖手柄横向拖 · 预览 .colres{width:8px}（基准表 §1.4）",
			BodyH = 40f,
			Paint = delegate(Rect r)
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Invalid comparison between Unknown and I4
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				Rect val10 = default(Rect);
				((Rect)(ref val10))..ctor(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 34f);
				Draw.Box(val10, Tokens.Card);
				Draw.Border(val10, Tokens.BorderSoft);
				int num2 = Mathf.RoundToInt(((Rect)(ref val10)).width * resizeX);
				Draw.EllipsisLabel(new Rect(((Rect)(ref val10)).x + 8f, ((Rect)(ref val10)).y, (float)num2, ((Rect)(ref val10)).height), "名字列", (GameFont)0, Tokens.Text, (TextAnchor)3);
				Draw.EllipsisLabel(new Rect(((Rect)(ref val10)).x + (float)num2 + 12f, ((Rect)(ref val10)).y, ((Rect)(ref val10)).width - (float)num2 - 14f, ((Rect)(ref val10)).height), "数值列 " + Mathf.RoundToInt(resizeX * 100f) + "%", (GameFont)0, Tokens.Muted, (TextAnchor)3);
				if (TableAtoms.ResizeHandle(val10, resizeX) && Event.current != null && (int)Event.current.type == 3)
				{
					resizeX = Mathf.Clamp(TableFmt.ResizeWidth(resizeX + Event.current.delta.x / Mathf.Max(1f, ((Rect)(ref val10)).width), 0.15f, 0.85f, 0f), 0.15f, 0.85f);
				}
			}
		});
		family.Add(new Atom
		{
			No = "27",
			Name = "FrozenColumn 冻结首列",
			Api = "TableAtoms.FrozenSplit(Rect, float frozenWidth, out Rect frozen, out Rect scrollArea)",
			Desc = "横向滚动时首列（姓名 / 图标）钉住不动：裁剪双区同 y 对齐绘制，滚动偏移只作用于右区。",
			Note = "预览 .frozen{64px} · .fc{border-right:2px solid border} · 右区可横向滚动（交互回补）",
			BodyH = 68f + CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0149: Unknown result type (might be due to invalid IL or missing references)
				//IL_014a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_0191: Unknown result type (might be due to invalid IL or missing references)
				//IL_019f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
				VStack val4 = Layout.Column(r, 4f);
				Rect val5 = default(Rect);
				Rect val6 = default(Rect);
				TableAtoms.FrozenSplit(((VStack)(ref val4)).Take(64f), 118f, ref val5, ref val6);
				Draw.Box(val6, Tokens.Panel);
				string[] array = new string[8] { "心情", "工时", "击杀", "建造", "采矿", "手工", "种植", "驯兽" };
				for (int j = 0; j < 3; j++)
				{
					HStack val7 = Layout.Row(new Rect(((Rect)(ref val6)).x + 4f - frozenScroll, ((Rect)(ref val6)).y + 4f + (float)j * 20f, 560f, 18f), 14f);
					for (int k = 0; k < array.Length; k++)
					{
						Rect val8 = ((HStack)(ref val7)).Take(58f);
						Draw.EllipsisLabel(val8, array[k] + " " + (int)(cellMood[j] + (float)(k * 3)), (GameFont)0, Tokens.Muted, (TextAnchor)3);
					}
				}
				Draw.Box(val5, Tokens.PanelRaised);
				Draw.Border(val5, Tokens.BorderSoft);
				for (int l = 0; l < 3; l++)
				{
					Draw.EllipsisLabel(new Rect(((Rect)(ref val5)).x + 6f, ((Rect)(ref val5)).y + 4f + (float)l * 20f, ((Rect)(ref val5)).width - 12f, 18f), Names[l], (GameFont)0, Tokens.Text, (TextAnchor)3);
				}
				Rect rest2 = ((VStack)(ref val4)).Rest;
				HStack val9 = Layout.Row(((VStack)(ref val4)).Take(((Rect)(ref rest2)).height), 8f);
				Widgets.HorizontalSlider(((HStack)(ref val9)).Rest, ref frozenScroll, new FloatRange(0f, 140f), (string)null, -1f);
			}
		});
		family.Add(new Atom
		{
			No = "28",
			Name = "DensitySwitch 密度三档",
			Api = "TableAtoms.DensitySwitch(Rect, ref RowDensity) → bool · TableFmt.RowHeight",
			Interactive = true,
			Desc = "紧凑 / 标准 / 舒适三档行高全局切换（22 / 28 / 34f），大表格按屏幅自选——Work Tab 缩放实证。",
			Note = "返回 true 表示档位被切换；行高由 TableFmt.RowHeight 派生，不写死数字",
			BodyH = 124f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				VStack val = Layout.Column(r, 6f);
				if (TableAtoms.DensitySwitch(((VStack)(ref val)).Take(26f), ref density))
				{
				}
				float num = TableFmt.RowHeight(density);
				VStack val2 = Layout.Column(((VStack)(ref val)).Take(90f), 0f);
				for (int i = 0; i < 6; i++)
				{
					Rect rest = ((VStack)(ref val2)).Rest;
					if (!(((Rect)(ref rest)).height > num * 0.5f))
					{
						break;
					}
					HStack val3 = Layout.Row(((VStack)(ref val2)).Take(num), 0f);
					Draw.EllipsisLabel(((HStack)(ref val3)).Take(((Rect)(ref r)).width * 0.6f), WorkTypes[i], (GameFont)0, Tokens.Text, (TextAnchor)3);
					Draw.EllipsisLabel(((HStack)(ref val3)).Rest, i + 1 + " 号位", (GameFont)0, Tokens.Dim, (TextAnchor)5);
					Draw.LineH(((Rect)(ref r)).x, ((VStack)(ref val2)).Y - 1f, ((Rect)(ref r)).width, Tokens.BorderHair);
				}
			}
		});
		families.Add(family);
	}

	private void BuildPanel()
	{
		Family family = new Family
		{
			Title = "A0+ · 面板原子族",
			Sub = "4 件 · PanelAtoms（RimHUD 实证）"
		};
		family.Add(new Atom
		{
			No = "29",
			Name = "StatRow 图标统计行",
			Api = "PanelAtoms.StatRow(Rect, string label, string value, float? bar01, Color? barColor)",
			Desc = "HUD 面板的行原子：图标 + 标签 + 数值（可带微型条）。RimHUD 每个信息块都是它的排列组合。",
			Note = "数值随时钟真实衰减 / 回补，低值转 warn，归零转 dead",
			BodyH = 62f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				VStack val6 = Layout.Column(r, 1f);
				float num = Mathf.Repeat(t * 0.06f, 1f);
				PanelAtoms.StatRow(((VStack)(ref val6)).Take(20f), "心情", Mathf.RoundToInt(62f + 8f * Mathf.Sin(t * 0.7f)) + "%", (float?)(0.62f + 0.08f * Mathf.Sin(t * 0.7f)), (Color?)Tokens.Accent);
				PanelAtoms.StatRow(((VStack)(ref val6)).Take(20f), "健康", "96%", (float?)0.96f, (Color?)Tokens.Alive);
				PanelAtoms.StatRow(((VStack)(ref val6)).Take(20f), "疲劳", Mathf.RoundToInt(40f + num * 50f) + "%", (float?)(0.4f + num * 0.5f), (Color?)Tokens.Warn);
			}
		});
		family.Add(new Atom
		{
			No = "30",
			Name = "MiniProfileChip 迷你档案条",
			Api = "PanelAtoms.MiniProfileChip(Rect, string initial, string name, float mood01, float health01)",
			Desc = "单行高密档案：微头像 + 名 + 双 micro 条（心情 / 健康）——事件流与队列行内嵌 pawn 信息。",
			Note = "两条 micro 条分别走心情 / 健康色阶；数据随帧漂移",
			BodyH = 76f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				VStack val5 = Layout.Column(r, 4f);
				PanelAtoms.MiniProfileChip(((VStack)(ref val5)).Take(36f), "蓝", Names[0], 0.72f + 0.04f * Mathf.Sin(t * 0.9f), 0.94f);
				PanelAtoms.MiniProfileChip(((VStack)(ref val5)).Take(36f), "赤", Names[1], 0.38f + 0.05f * Mathf.Sin(t * 1.3f), 0.81f);
			}
		});
		family.Add(new Atom
		{
			No = "31",
			Name = "FlashOverlay 警示闪边",
			Api = "PanelAtoms.FlashOverlay(Rect, float t, Color?, float thickness)",
			Desc = "致命病症 / 临近崩溃时整块边框脉动（accent ↔ dead 插值 + sin 呼吸），比静态红框更难被忽视。",
			Note = "预览 km-blink{box-shadow 扩散} ⇒ 引擎侧降级为边框色相位脉动（基准表 §1.5）",
			BodyH = CtrlTiny + 5f + 44f,
			Paint = delegate(Rect r)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_014c: Unknown result type (might be due to invalid IL or missing references)
				string[] array4 = new string[4] { "正常", "低", "中", "高" };
				VStack val3 = Layout.Column(r, 5f);
				InputAtoms.Segmented(((VStack)(ref val3)).Take(CtrlTiny), array4, ref alertLv);
				Rect rest = ((VStack)(ref val3)).Rest;
				Rect val4 = ((VStack)(ref val3)).Take(((Rect)(ref rest)).height);
				if (!(((Rect)(ref val4)).height < LineTiny))
				{
					Draw.Box(val4, Tokens.Card);
					if (alertLv == 0)
					{
						Draw.Border(val4, Tokens.BorderSoft);
						Draw.Box(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).y + 1f, 3f, ((Rect)(ref val4)).height - 2f), Tokens.Alive);
						Draw.EllipsisLabel(val4, "\u3000正常 · 无告警", (GameFont)0, Tokens.Muted, (TextAnchor)3);
					}
					else
					{
						PanelAtoms.FlashOverlay(val4, flash, (Color?)Tokens.Dead, (alertLv >= 3) ? 2f : 1f);
						Draw.EllipsisLabel(val4, "\u3000崩溃风险（" + array4[alertLv] + "）· 心情 " + (38 - alertLv * 10), (GameFont)0, Tokens.Dead, (TextAnchor)3);
					}
				}
			}
		});
		family.Add(new Atom
		{
			No = "32",
			Name = "SectionBar 面板分段条",
			Api = "PanelAtoms.SectionBar(Rect, string title, string count, bool expanded)",
			Desc = "HUD 面板的薄分段头（可折叠）：高 20f，标题居左 + 计数 + 折叠箭头，比 SectionTitle 更密。",
			Note = "计数取官方警报 / 想法 / 历史记录器词表 · 点条折叠（交互回补）",
			BodyH = 118f,
			Paint = delegate(Rect r)
			{
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				string[] array = new string[3] { "警报", "想法", "历史记录器" };
				string[] array2 = new string[3] { "11 项", "15 项", "11 项" };
				string[] array3 = new string[3] { "食物不足 / 药品不足 / 崩溃风险（高）/ 衣衫褴褛 …", "很高的期望 / 合理的期望 / 大麻亢奋 / 宿醉（轻微）…", "殖民者心情 / 财富（物品）/ 威胁点数 / 人口适应 …" };
				VStack val = Layout.Column(r, 3f);
				for (int i = 0; i < array.Length; i++)
				{
					Rect val2 = ((VStack)(ref val)).Take(19f);
					PanelAtoms.SectionBar(val2, array[i], array2[i], secOpen[i]);
					HoverFrame(val2);
					if (Widgets.ButtonInvisible(val2, true))
					{
						secOpen[i] = !secOpen[i];
					}
					if (secOpen[i])
					{
						Draw.EllipsisLabel(((VStack)(ref val)).Take(LineTiny), array3[i], (GameFont)0, Tokens.Dim, (TextAnchor)3);
					}
				}
			}
		});
		families.Add(family);
	}

	private void BuildInteract()
	{
		Family family = new Family
		{
			Title = "A0+ · 交互原子族",
			Sub = "4 件 · InteractAtoms（Mod Manager / DragList 实证）"
		};
		family.Add(new Atom
		{
			No = "33",
			Name = "GripHandle 拖拽把手",
			Api = "InteractAtoms.GripHandle(Rect, int rows, int cols, Color?) → bool",
			Interactive = true,
			Desc = "行首把手（可拖行视觉提示 + 悬停变 accent），拖拽重排的专用命中区——与行点击分离，防误拖。",
			Note = "点把手切换高亮态；点行体不触发（命中区分离是防误拖的关键）",
			BodyH = 176f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				VStack val9 = Layout.Column(r, 3f);
				for (int l = 0; l < 8; l++)
				{
					HStack val10 = Layout.Row(((VStack)(ref val9)).Take(19f), 6f);
					Rect val11 = ((HStack)(ref val10)).Take(16f);
					if (InteractAtoms.GripHandle(val11, 3, 2, (gripOn && l == 0) ? new Color?(Tokens.Accent) : null) && l == 0)
					{
						gripOn = !gripOn;
					}
					Draw.EllipsisLabel(((HStack)(ref val10)).Take(20f), (l + 1).ToString("00"), (GameFont)0, Tokens.Dim, (TextAnchor)3);
					Draw.EllipsisLabel(((HStack)(ref val10)).Rest, WorkTypes[l], (GameFont)0, Tokens.Text, (TextAnchor)3);
					Draw.LineH(((Rect)(ref r)).x, ((VStack)(ref val9)).Y - 1f, ((Rect)(ref r)).width, Tokens.BorderHair);
				}
			}
		});
		family.Add(new Atom
		{
			No = "34",
			Name = "EyeToggle 可见性切换",
			Api = "InteractAtoms.EyeToggle(Rect, bool visible, Color?) → bool",
			Interactive = true,
			Desc = "切换行 / 列可见性（隐藏态半透明 + 斜杠），列配置、图层显隐、面板模块开关通用。",
			Note = "返回新状态由调用方回写（组件自身零状态）· 预览 .iconbtn{28×28}",
			BodyH = 132f,
			Paint = delegate(Rect r)
			{
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				string[] array3 = new string[6] { "角色", "健康", "需求", "装备", "社交", "日志" };
				VStack val6 = Layout.Column(r, 3f);
				for (int k = 0; k < array3.Length; k++)
				{
					HStack val7 = Layout.Row(((VStack)(ref val6)).Take(19f), 7f);
					Rect val8 = ((HStack)(ref val7)).Take(24f);
					bool flag2 = ((k == 0) ? eyeOn : (k != 2));
					if (InteractAtoms.EyeToggle(val8, flag2, (Color?)null) && k == 0)
					{
						eyeOn = !eyeOn;
					}
					Draw.EllipsisLabel(((HStack)(ref val7)).Rest, array3[k] + " 页签" + (flag2 ? "" : "（已隐藏）"), (GameFont)0, flag2 ? Tokens.Text : Tokens.Dim, (TextAnchor)3);
				}
			}
		});
		family.Add(new Atom
		{
			No = "35",
			Name = "SearchHighlight 搜索命中高亮",
			Api = "InteractAtoms.SearchHighlight(Rect, string text, string query, GameFont, Color?)",
			Desc = "搜索关键词在结果行内分段高亮（命中段 accent 底）。分段测量绘制，CJK 安全。",
			Note = "⚠ 契约：关键词为空时整行普通绘制 · 真输入框（交互回补）",
			BodyH = CtrlTiny + 2f + 6f * LineTiny + 10f,
			Paint = delegate(Rect r)
			{
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				string[] array2 = new string[6] { "建造 12 · 建造师 · 部落工匠", "采矿 9 · 矿工 · 在井下待了半辈子", "手工 7 · 铁匠 · 敲钢铁的手很稳", "射击 5 · 猎手 · 只用反曲弓", "种植 4 · 农夫 · 会看云识天气", "医疗 1 · 医者 · 抄过三本旧医书" };
				HStack val3 = Layout.Row(r, 6f);
				Rect val4 = ((HStack)(ref val3)).Take(58f);
				shQuery = Widgets.TextField(((HStack)(ref val3)).Rest, shQuery);
				if (Widgets.ButtonText(val4, "清空", true, true, true, (TextAnchor?)null))
				{
					shQuery = "";
				}
				VStack val5 = Layout.Column(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + CtrlTiny + 2f, ((Rect)(ref r)).width, Mathf.Max(0f, ((Rect)(ref r)).height - CtrlTiny - 2f)), 2f);
				for (int j = 0; j < array2.Length; j++)
				{
					InteractAtoms.SearchHighlight(((VStack)(ref val5)).Take(LineTiny), array2[j], shQuery, Tokens.FontBody, (Color?)null);
				}
			}
		});
		family.Add(new Atom
		{
			No = "36",
			Name = "ToolbarIconButton 图标工具钮",
			Api = "InteractAtoms.ToolbarIcon(Rect, string glyph, bool active) → bool",
			Interactive = true,
			Desc = "方形单图标钮（激活态 accent 底 + 2px 内描边），悬停 Tooltip、长按可弹出备选。",
			Note = "按钮本体即 28×28（预览 .iconbtn）· 首钮可切换激活态",
			BodyH = 30f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				HStack val = Layout.Row(r, 5f);
				string[] array = new string[5] { "E", "M", "D", "L", "Z" };
				for (int i = 0; i < array.Length; i++)
				{
					Rect val2 = ((HStack)(ref val)).Take(28f);
					if (((Rect)(ref val2)).width < 20f)
					{
						break;
					}
					bool flag = iconOn && i == 0;
					if (InteractAtoms.ToolbarIcon(val2, array[i], flag) && i == 0)
					{
						iconOn = !iconOn;
					}
				}
			}
		});
		families.Add(family);
	}

	private void BuildProgress()
	{
		Family family = new Family
		{
			Title = "A0+ · 进度与容量",
			Sub = "3 件 · ProgressAtoms"
		};
		family.Add(new Atom
		{
			No = "37",
			Name = "FillWipe 斜向进度擦除",
			Api = "ProgressAtoms.FillWipe(Rect, float progress01, Color?, int rows, float slant)",
			Desc = "节点 / 卡片上的斜向填充推进（分段行绘制），比水平条更有「施工中」叙事感。",
			Note = "⚠ 契约：组件自铺 BorderSoft 满底 + 分段实心条 —— 调用方不得再叠底、不得与 label 叠压 · 可跳过本项",
			BodyH = 52f + CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_0143: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_0161: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
				VStack val6 = Layout.Column(r, 4f);
				Rect val7 = ((VStack)(ref val6)).Take(48f);
				((Rect)(ref val7)).width = Mathf.Min(300f, ((Rect)(ref val7)).width);
				Draw.Box(val7, Tokens.Card);
				Draw.Border(val7, Tokens.BorderSoft);
				Draw.Box(new Rect(((Rect)(ref val7)).x, ((Rect)(ref val7)).y + 1f, 3f, ((Rect)(ref val7)).height - 2f), Tokens.Accent);
				ProgressAtoms.FillWipe(new Rect(((Rect)(ref val7)).x + 4f, ((Rect)(ref val7)).y + 1f, ((Rect)(ref val7)).width - 5f, ((Rect)(ref val7)).height - 2f), Mathf.Clamp01(wipe), (Color?)Tokens.Accent, 10, 0.35f);
				Draw.EllipsisLabel(new Rect(((Rect)(ref val7)).x + 11f, ((Rect)(ref val7)).y, ((Rect)(ref val7)).width - 22f, ((Rect)(ref val7)).height), Research[researchIdx] + " · 研究点 " + Mathf.RoundToInt(Mathf.Clamp01(wipe) * 100f) + "%", (GameFont)0, Tokens.Text, (TextAnchor)3);
				Rect rest2 = ((VStack)(ref val6)).Rest;
				HStack val8 = Layout.Row(((VStack)(ref val6)).Take(((Rect)(ref rest2)).height), 6f);
				if (Widgets.ButtonText(((HStack)(ref val8)).Take(82f), "跳过本项", true, true, true, (TextAnchor?)null))
				{
					researchIdx = (researchIdx + 1) % Research.Length;
				}
				Draw.EllipsisLabel(((HStack)(ref val8)).Rest, "研究队列 · 官方 ResearchProjectDef", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "38",
			Name = "CapacityBar 容量叠印条",
			Api = "ProgressAtoms.CapacityBar(Rect, float used, float capacity, Color?)",
			Desc = "容量占用叠印（搬运 / 仓储 / 背包）：接近满载变黄，超载时右端出现红色溢出竖块。",
			Note = "⚠ 契约：默认色 = ratio>0.85 ? Warn : Alive（非 Accent）；超载溢出 = 4 个红竖块（非虚线）⇒ 传 null 让组件自表达",
			BodyH = 66f,
			Paint = delegate(Rect r)
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				string[] array = new string[3] { "背包", "仓库", "冷藏柜" };
				float[] array2 = new float[3] { 780f, 1000f, 1180f };
				VStack val3 = Layout.Column(r, 4f);
				for (int j = 0; j < array.Length; j++)
				{
					HStack val4 = Layout.Row(((VStack)(ref val3)).Take(18f), 8f);
					Draw.EllipsisLabel(((HStack)(ref val4)).Take(44f), array[j], (GameFont)0, Tokens.Muted, (TextAnchor)3);
					Rect rest = ((HStack)(ref val4)).Rest;
					Rect val5 = ((HStack)(ref val4)).Take(((Rect)(ref rest)).width - 46f);
					ProgressAtoms.CapacityBar(val5, array2[j], 1000f, (Color?)null);
					Draw.EllipsisLabel(((HStack)(ref val4)).Rest, Mathf.RoundToInt(array2[j] / 10f) + "%", (GameFont)0, (j == 2) ? Tokens.Dead : Tokens.Muted, (TextAnchor)5);
				}
			}
		});
		family.Add(new Atom
		{
			No = "39",
			Name = "ZoomIndicator 缩放指示",
			Api = "ProgressAtoms.ZoomIndicator(Rect, float zoom, float resetValue) → bool",
			Interactive = true,
			Desc = "图谱 / 画布角落的缩放读数 + 一键复位；平移缩放工作区的标配伴侣。",
			Note = "⚠ 契约：组件内部自带 Layout.Row 排 [标签34][读数50][按钮64] ⇒ 调用方只需给 ≥174 宽",
			BodyH = 100f,
			Paint = delegate(Rect r)
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0145: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_0134: Unknown result type (might be due to invalid IL or missing references)
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Unknown result type (might be due to invalid IL or missing references)
				if (ProgressAtoms.ZoomIndicator(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, Mathf.Min(200f, ((Rect)(ref r)).width), 26f), zoom, 0.86f))
				{
					zoom = 0.86f;
				}
				zoom = Mathf.Clamp(0.86f + 0.34f * Mathf.Sin(t * 0.6f), 0.2f, 3f);
				Rect val = default(Rect);
				((Rect)(ref val))..ctor(((Rect)(ref r)).x, ((Rect)(ref r)).y + 30f, Mathf.Min(268f, ((Rect)(ref r)).width), 66f);
				Draw.Box(val, Tokens.Window);
				Draw.Border(val, Tokens.BorderSoft);
				for (int i = 0; i < 8; i++)
				{
					int num = i % 4;
					int num2 = i / 4;
					Rect val2 = Atoms.Anchor(val, (Anchor9)((num2 != 0) ? 6 : 0), new Vector2(52f, 22f), 6f);
					((Rect)(ref val2))..ctor(((Rect)(ref val2)).x + (float)num * 58f, ((Rect)(ref val2)).y + (float)num2 * 26f, 52f, 22f);
					Draw.Box(val2, Draw.WithAlpha((i == 0 || i == 5) ? Tokens.Accent : Tokens.PanelRaised, 0.5f));
					Draw.Border(val2, (i == 0) ? Tokens.Accent : Tokens.BorderSoft);
				}
			}
		});
		families.Add(family);
	}

	private void BuildMisc()
	{
		Family family = new Family
		{
			Title = "A0+ · 杂项原子",
			Sub = "3 件 · MiscAtoms"
		};
		family.Add(new Atom
		{
			No = "40",
			Name = "PointCostBadge 点数徽章",
			Api = "MiscAtoms.PointCostBadge(Rect, int budget, int spent)",
			Desc = "预算点数徽章（剩余 / 用尽 / 超支三态自动换色），角色编辑器式配额展示；预算管理类界面通用。",
			Note = "⚠ 契约：参数序是 (budget, spent)；宽高即徽章本体 ⇒ 按 .pill 取「文本宽 + 18」",
			BodyH = 68f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_014e: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				VStack val6 = Layout.Column(r, 6f);
				HStack val7 = Layout.Row(((VStack)(ref val6)).Take(24f), 6f);
				float num4 = Mathf.Min(((Rect)(ref r)).width, Text.CalcSize("剩余 128 点").x + 18f);
				Rect val8 = ((HStack)(ref val7)).Take(num4);
				if (Widgets.ButtonText(((HStack)(ref val7)).Take(26f), "−", true, true, budgetSpent > 0, (TextAnchor?)null))
				{
					budgetSpent = Mathf.Max(0, budgetSpent - 16);
				}
				if (Widgets.ButtonText(((HStack)(ref val7)).Take(26f), "+", true, true, budgetSpent < 240, (TextAnchor?)null))
				{
					budgetSpent = Mathf.Min(240, budgetSpent + 16);
				}
				MiscAtoms.PointCostBadge(new Rect(((Rect)(ref val8)).x, ((Rect)(ref val8)).y, Mathf.Min(num4, ((Rect)(ref val8)).width), ((Rect)(ref val8)).height), 200, budgetSpent);
				Rect val9 = ((VStack)(ref val6)).Take(11f);
				ProgressAtoms.CapacityBar(val9, (float)budgetSpent, 200f, (Color?)null);
				Draw.EllipsisLabel(((VStack)(ref val6)).Take(LineTiny), "预算 " + 200 + " · 已分配 " + budgetSpent, (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "41",
			Name = "BubbleTag 窗口层气泡标签",
			Api = "MiscAtoms.BubbleTag(Rect bubble, string text, Color? color)",
			Desc = "带下尾巴的悬浮小气泡（需求 / 想法快显）。气泡文案取官方 ThoughtDef：宿醉、很高的期望、大麻亢奋……",
			Note = "⚠ 契约：rect 仅为主体，尾巴画在 yMax 下方 8px ⇒ 高度只给一行（22）",
			BodyH = 60f,
			Paint = delegate(Rect r)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				string[] array = new string[4] { "宿醉（轻微）", "很高的期望", "大麻亢奋", "仙馐果暖身" };
				VStack val4 = Layout.Column(r, 26f);
				string text = array[Mathf.FloorToInt(t * 0.3f) % array.Length];
				float num3 = Mathf.Min(((Rect)(ref r)).width, Text.CalcSize(text).x + 18f);
				Rect val5 = ((VStack)(ref val4)).Take(22f);
				MiscAtoms.BubbleTag(new Rect(((Rect)(ref val5)).x, ((Rect)(ref val5)).y, Mathf.Min(num3, ((Rect)(ref val5)).width), ((Rect)(ref val5)).height), text, (Color?)Tokens.Card);
				MiscAtoms.BubbleTag(new Rect(((Rect)(ref val5)).x + num3 + 12f, ((Rect)(ref val5)).y, Mathf.Min(num3, Mathf.Max(0f, ((Rect)(ref val5)).width - num3 - 12f)), ((Rect)(ref val5)).height), "合理的期望", (Color?)null);
			}
		});
		family.Add(new Atom
		{
			No = "42",
			Name = "ThinScrollBar 细滚动条",
			Api = "MiscAtoms.ThinScrollBar(Rect viewRect, float contentH, float scrollY, float width)",
			Desc = "自绘细滚动条（细轨 + 圆角滑块），替代原版宽灰条——深色主题下违和感根除。",
			Note = "⚠ 契约：条画在 viewRect.xMax + 1f（rect 外侧）；contentH ≤ height 时完全不画 ⇒ 传视口 rect",
			BodyH = 76f,
			Paint = delegate(Rect r)
			{
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Invalid comparison between Unknown and I4
				//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0180: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
				Rect val = default(Rect);
				((Rect)(ref val))..ctor(((Rect)(ref r)).x, ((Rect)(ref r)).y, Mathf.Max(40f, ((Rect)(ref r)).width - 12f), 76f);
				float num = Mathf.Max(1f, 220f - ((Rect)(ref val)).height);
				Draw.Box(val, Tokens.Panel);
				Draw.Border(val, Tokens.BorderSoft);
				Rect val2 = default(Rect);
				((Rect)(ref val2))..ctor(((Rect)(ref val)).xMax + 1f, ((Rect)(ref val)).y, 8f, ((Rect)(ref val)).height);
				if (Mouse.IsOver(val2) && Event.current.isMouse && Event.current.button == 0 && ((int)Event.current.type == 0 || (int)Event.current.type == 3))
				{
					thinScroll = Mathf.Clamp01((Event.current.mousePosition.y - ((Rect)(ref val)).y) / Mathf.Max(1f, ((Rect)(ref val)).height)) * num;
				}
				HoverFrame(val2);
				VStack val3 = Layout.Column(new Rect(((Rect)(ref val)).x + 4f, ((Rect)(ref val)).y + 3f, ((Rect)(ref val)).width - 8f, ((Rect)(ref val)).height - 6f), 2f);
				int num2 = Mathf.Clamp(Mathf.FloorToInt(thinScroll / LineTiny), 0, EventWords.Length - 1);
				for (int i = num2; i < EventWords.Length; i++)
				{
					Rect rest = ((VStack)(ref val3)).Rest;
					if (!(((Rect)(ref rest)).height >= LineTiny))
					{
						break;
					}
					Draw.EllipsisLabel(((VStack)(ref val3)).Take(LineTiny), EventWords[i] + "\u3000" + (120 + i * 37), (GameFont)0, Tokens.Muted, (TextAnchor)3);
				}
				MiscAtoms.ThinScrollBar(val, 220f, thinScroll, 8f);
			}
		});
		families.Add(family);
	}

	private void BuildStatus()
	{
		Family family = new Family
		{
			Title = "A0 · 原子层 —— 状态原子",
			Sub = "5 件 · StatusAtoms（alpha/scale 插值，无 CSS 过渡）"
		};
		family.Add(new Atom
		{
			No = "13",
			Name = "PulseDot 呼吸状态点",
			Api = "StatusAtoms.PulseDot(Rect, Color, float t, float minAlpha)",
			Desc = "绿在线 / 黄忙碌 / 红告急，alpha 与 scale 双插值脉动。任意锚点的存活指示。",
			Note = "三个点共用同一条动画但错相位——同相位会像灯泡，错相位才像呼吸",
			BodyH = 24f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				HStack val19 = Layout.Row(r, 20f);
				string[] array4 = new string[3] { "在线", "忙碌", "告急" };
				Color[] array5 = (Color[])(object)new Color[3]
				{
					Tokens.Alive,
					Tokens.Warn,
					Tokens.Dead
				};
				for (int l = 0; l < array4.Length; l++)
				{
					Rect val20 = ((HStack)(ref val19)).Take(17f + Measure.Width(array4[l], (GameFont)0));
					if (((Rect)(ref val20)).width < 10f)
					{
						break;
					}
					HStack val21 = Layout.Row(val20, 7f);
					StatusAtoms.PulseDot(((HStack)(ref val21)).Take(10f), array5[l], t + (float)l * 0.55f, 0.45f);
					Draw.EllipsisLabel(((HStack)(ref val21)).Rest, array4[l], (GameFont)0, Tokens.Muted, (TextAnchor)3);
				}
			}
		});
		family.Add(new Atom
		{
			No = "14",
			Name = "StatusGlyph 状态图形",
			Api = "StatusAtoms.Glyph(Rect, StatusKind)",
			Desc = "五态语义图形（色随语义令牌），比图标字库更可控——自绘两笔线条。",
			Note = "规格对齐预览 .glyph{17×17;font-size:12px}（基准表 §1.4）· 点图形切换读数",
			BodyH = 44f,
			Paint = delegate(Rect r)
			{
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				StatusKind[] array = new StatusKind[5];
				RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				StatusKind[] array2 = (StatusKind[])(object)array;
				string[] array3 = new string[5] { "正常", "信息", "警告", "失效", "暂停" };
				HStack val16 = Layout.Row(r, 14f);
				for (int k = 0; k < array2.Length; k++)
				{
					Rect val17 = ((HStack)(ref val16)).Take(23f + Measure.Width(array3[k], (GameFont)0));
					if (((Rect)(ref val17)).width < 17f)
					{
						break;
					}
					HStack val18 = Layout.Row(val17, 6f);
					StatusAtoms.Glyph(((HStack)(ref val18)).Take(17f), array2[k]);
					Draw.EllipsisLabel(((HStack)(ref val18)).Rest, array3[k], (GameFont)0, Tokens.Dim, (TextAnchor)3);
					HoverFrame(val17);
					if (Widgets.ButtonInvisible(new Rect(((Rect)(ref val17)).x, ((Rect)(ref val17)).y, ((Rect)(ref val17)).width, ((Rect)(ref val17)).height), true))
					{
						glyphIdx = k;
					}
				}
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 20f, ((Rect)(ref r)).width, LineTiny), "当前读数：" + array3[Mathf.Clamp(glyphIdx, 0, 4)], (GameFont)0, Tokens.Accent, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "15",
			Name = "LoadingSpinner 旋转加载",
			Api = "StatusAtoms.Spinner(Rect, float t, Color?, int segments, float radius, float size)",
			Desc = "自绘分段旋转。另有 DrawTextureRotated 贴图重载，走 GUIUtility.RotateAroundPivot 捷径。",
			Note = "进度累积到 100% 自动停转并转 ✓ · 可重播（交互回补）",
			BodyH = 41f + CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				HStack val12 = Layout.Row(r, 10f);
				Rect val13 = ((HStack)(ref val12)).Take(22f);
				if (!spinDone)
				{
					StatusAtoms.Spinner(val13, t, (Color?)Tokens.Accent, 8, 8f, 3f);
				}
				else
				{
					Draw.Box(val13, Draw.WithAlpha(Tokens.Alive, 0.16f));
					Draw.Label(val13, "✓", (GameFont)1, Tokens.Alive, (TextAnchor)4);
				}
				VStack val14 = Layout.Column(((HStack)(ref val12)).Rest, 4f);
				KBar(((VStack)(ref val14)).Take(11f), spinP / 100f, spinDone ? Tokens.Alive : Tokens.Accent);
				Rect rest3 = ((VStack)(ref val14)).Rest;
				HStack val15 = Layout.Row(((VStack)(ref val14)).Take(((Rect)(ref rest3)).height), 6f);
				if (Widgets.ButtonText(((HStack)(ref val15)).Take(64f), "重播", true, true, true, (TextAnchor?)null))
				{
					spinP = 0f;
					spinDone = false;
				}
				Draw.EllipsisLabel(((HStack)(ref val15)).Rest, spinDone ? "归档完成 · 事件日志已写入" : "正在归档 事件日志…", (GameFont)0, Tokens.Muted, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "16",
			Name = "ScrollFadeHint 滚动渐隐提示",
			Api = "StatusAtoms.ScrollFadeHint(Rect viewport, bool moreBelow)",
			Interactive = true,
			Desc = "视口底部渐隐 + ▾ 提示「下面还有」。渐隐遮罩需渐变贴图，▾ 与判定为纯原生。",
			Note = "滚到底部自动消失（moreBelow=false 时不绘制）",
			BodyH = 82f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Unknown result type (might be due to invalid IL or missing references)
				//IL_019c: Unknown result type (might be due to invalid IL or missing references)
				HStack val8 = Layout.Row(r, 10f);
				Rect rest2 = ((HStack)(ref val8)).Rest;
				Rect val9 = ((HStack)(ref val8)).Take(Mathf.Min(210f, ((Rect)(ref rest2)).width - 90f));
				Draw.Box(val9, Tokens.Panel);
				VStack val10 = Layout.Column(new Rect(((Rect)(ref val9)).x + 4f, ((Rect)(ref val9)).y + 3f, ((Rect)(ref val9)).width - 8f, ((Rect)(ref val9)).height - 6f), 2f);
				for (int j = 0; j < 6; j++)
				{
					rest2 = ((VStack)(ref val10)).Rest;
					if (!(((Rect)(ref rest2)).height >= LineTiny))
					{
						break;
					}
					Draw.EllipsisLabel(((VStack)(ref val10)).Take(LineTiny), "09:4" + j + " · " + Pick("袭击", "寒流", "虫灾", "贸易商队"), (GameFont)0, Tokens.Muted, (TextAnchor)3);
				}
				StatusAtoms.ScrollFadeHint(val9, moreBelow);
				VStack val11 = Layout.Column(((HStack)(ref val8)).Rest, 4f);
				if (Widgets.ButtonText(((VStack)(ref val11)).Take(CtrlTiny), moreBelow ? "切到已到底" : "切到还有内容", true, true, true, (TextAnchor?)null))
				{
					moreBelow = !moreBelow;
				}
				Draw.EllipsisLabel(((VStack)(ref val11)).Take(LineTiny), moreBelow ? "moreBelow = true" : "moreBelow = false", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "17",
			Name = "ExpandCaret 展开箭头",
			Api = "StatusAtoms.Caret(Rect, bool expanded, Color?) / Caret(Rect, float progress01, Color?)",
			Interactive = true,
			Desc = "▸ / ▾ 旋转插值——CollapsibleTree / SectionFrame / 折叠组的共享原子。",
			Note = "bool 重载（插值旋转）+ float 重载（0–1 进度）；工坊实证 20 项工作类型",
			BodyH = 58f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_014a: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				VStack val = Layout.Column(r, 4f);
				Rect val2 = ((VStack)(ref val)).Take(24f);
				HStack val3 = Layout.Row(val2, 7f);
				Rect val4 = ((HStack)(ref val3)).Take(16f);
				StatusAtoms.Caret(val4, caretOpen, (Color?)null);
				if (Widgets.ButtonInvisible(val2, true))
				{
					caretOpen = !caretOpen;
				}
				Draw.EllipsisLabel(((HStack)(ref val3)).Rest, "工作优先级（" + WorkTypes.Length + " 项）", (GameFont)0, Tokens.Text, (TextAnchor)3);
				Rect val5 = ((VStack)(ref val)).Take(30f);
				if (caretOpen)
				{
					HStack val6 = Layout.Row(val5, 4f);
					for (int i = 0; i < WorkTypes.Length; i++)
					{
						Rect rest = ((HStack)(ref val6)).Rest;
						if (!(((Rect)(ref rest)).width > 30f))
						{
							break;
						}
						float num = Text.CalcSize(WorkTypes[i]).x + 10f;
						rest = ((HStack)(ref val6)).Rest;
						Rect val7 = ((HStack)(ref val6)).Take(Mathf.Min(num, ((Rect)(ref rest)).width));
						Draw.Box(val7, Tokens.Card);
						Draw.Border(val7, Tokens.BorderSoft);
						Draw.Label(val7, WorkTypes[i], (GameFont)0, Tokens.Muted, (TextAnchor)4);
					}
				}
			}
		});
		families.Add(family);
	}

	private void BuildVisual()
	{
		Family family = new Family
		{
			Title = "A0 · 原子层 —— 布局与视觉原子",
			Sub = "6 件 · Atoms / VisualAtoms（比例 / 锚定 / 取色 / 图例 / 截断 / 角标）"
		};
		family.Add(new Atom
		{
			No = "18",
			Name = "AspectBox 等比容器",
			Api = "Atoms.AspectBox(Rect outer, float ratio) → Rect",
			Desc = "锁定宽高比，内容等比适配居中——头像 / 缩略图 / 贴图卡排版的根治件，杜绝拉伸变形。",
			Note = "原版 DrawTextureFitted 的等价几何：切比例时容器重排，内容不拉伸",
			BodyH = CtrlTiny + 5f + 56f,
			Paint = delegate(Rect r)
			{
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				float[] array8 = new float[4] { 1.7777778f, 1f, 1.3333334f, 0.5625f };
				string[] array9 = new string[4] { "16:9", "1:1", "4:3", "9:16" };
				VStack val9 = Layout.Column(r, 5f);
				InputAtoms.Segmented(((VStack)(ref val9)).Take(CtrlTiny), array9, ref aspectIdx);
				Rect rest3 = ((VStack)(ref val9)).Rest;
				HStack val10 = Layout.Row(((VStack)(ref val9)).Take(((Rect)(ref rest3)).height), 10f);
				Rect val11 = Atoms.AspectBox(((HStack)(ref val10)).Take(104f), array8[aspectIdx]);
				Draw.Box(val11, Tokens.PanelRaised);
				Draw.Border(val11, Tokens.BorderSoft);
				Draw.Label(val11, array9[aspectIdx], (GameFont)0, Tokens.Accent, (TextAnchor)4);
				Draw.EllipsisLabel(((HStack)(ref val10)).Rest, "切档 → 容器重排，内容不拉伸（原版 DrawTextureFitted 的等价几何）", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "19",
			Name = "AnchorFrame 九宫锚定",
			Api = "Atoms.Anchor(Rect parent, Anchor9, Vector2 size, float margin) → Rect",
			Desc = "声明式定位原子：九宫九锚（贴边 / 居中 / 拉伸）+ 边距。L4 布局的定位地基。",
			Note = "点舞台任意位置吸附最近锚 · 规格对齐 .anchorstage{74px} + .tgt{26×18}",
			BodyH = 74f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				Draw.Border(r, Tokens.BorderSoft);
				Anchor9[] array6 = new Anchor9[9];
				RuntimeHelpers.InitializeArray(array6, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				Anchor9[] array7 = (Anchor9[])(object)array6;
				for (int k = 0; k < array7.Length; k++)
				{
					Rect val8 = Atoms.Anchor(r, array7[k], new Vector2(26f, 18f), 4f);
					Draw.Box(val8, (k == anchorIdx) ? Tokens.Accent : Tokens.PanelRaised);
					Draw.Border(val8, Tokens.BorderSoft);
					Draw.Label(val8, ((object)(Anchor9)(ref array7[k])).ToString().Substring(0, 2), (GameFont)0, (k == anchorIdx) ? Tokens.Window : Tokens.Dim, (TextAnchor)4);
				}
				if (Widgets.ButtonInvisible(r, true))
				{
					Vector2 mousePosition = Event.current.mousePosition;
					int num = ((!(mousePosition.x < ((Rect)(ref r)).x + ((Rect)(ref r)).width / 3f)) ? ((!(mousePosition.x > ((Rect)(ref r)).x + ((Rect)(ref r)).width * 2f / 3f)) ? 1 : 2) : 0);
					int num2 = ((!(mousePosition.y < ((Rect)(ref r)).y + ((Rect)(ref r)).height / 3f)) ? ((!(mousePosition.y > ((Rect)(ref r)).y + ((Rect)(ref r)).height * 2f / 3f)) ? 1 : 2) : 0);
					anchorIdx = num2 * 3 + num;
				}
			}
		});
		family.Add(new Atom
		{
			No = "20",
			Name = "Swatch 色卡",
			Api = "VisualAtoms.Swatch(Rect, Color, bool selected)",
			Interactive = true,
			Desc = "取色回显的色块原子（选中态 FocusRing）。官方依据：Verse GenColor 派生对比边框色。",
			Note = "点色块切换选中态 · 规格对齐 .swatch{28×22}（基准表 §1.4）",
			BodyH = 24f,
			Paint = delegate(Rect r)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
				Color[] array4 = (Color[])(object)new Color[5]
				{
					Tokens.Warn,
					Tokens.Info,
					Tokens.Accent,
					Tokens.Alive,
					Tokens.Dead
				};
				string[] array5 = new string[5] { "金", "蓝", "青", "绿", "红" };
				HStack val6 = Layout.Row(r, 6f);
				for (int j = 0; j < array4.Length; j++)
				{
					Rect val7 = ((HStack)(ref val6)).Take(28f);
					bool flag2 = Mathf.Approximately(swatchSel, (float)j);
					VisualAtoms.Swatch(val7, array4[j], flag2);
					if (Widgets.ButtonInvisible(val7, true))
					{
						swatchSel = j;
					}
				}
				Draw.EllipsisLabel(((HStack)(ref val6)).Rest, "当前：" + array5[Mathf.Clamp(Mathf.RoundToInt(swatchSel), 0, 4)], (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "21",
			Name = "GradientBar 渐变图例条",
			Api = "VisualAtoms.GradientBar(Rect, Color lo, Color hi, int segments)",
			Desc = "渐变强度图例（HeatCalendar / 双端滑条的共伴），分段绘制即可平滑，零贴图。",
			Note = "预览 .gradbar{height:11px}，两端标注 0 与 100 件/日",
			BodyH = 32f,
			Paint = delegate(Rect r)
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0137: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
				Rect val5 = default(Rect);
				((Rect)(ref val5))..ctor(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 11f);
				VisualAtoms.GradientBar(val5, Tokens.Accent, Tokens.Alive, 16);
				bool flag = Mouse.IsOver(val5);
				gradHover = (flag ? Mathf.Clamp01((Event.current.mousePosition.x - ((Rect)(ref val5)).x) / Mathf.Max(1f, ((Rect)(ref val5)).width)) : (-1f));
				if (gradHover >= 0f)
				{
					Draw.Box(new Rect(((Rect)(ref val5)).x + gradHover * ((Rect)(ref val5)).width - 1f, ((Rect)(ref val5)).y - 2f, 2f, 15f), Tokens.Text);
				}
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 14f, 60f, LineTiny), "0", (GameFont)0, Tokens.Dim, (TextAnchor)3);
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).xMax - 90f, ((Rect)(ref r)).y + 14f, 90f, LineTiny), "100 件/日", (GameFont)0, Tokens.Dim, (TextAnchor)5);
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x + 62f, ((Rect)(ref r)).y + 14f, Mathf.Max(0f, ((Rect)(ref r)).width - 156f), LineTiny), (gradHover >= 0f) ? ("读数 " + Mathf.RoundToInt(gradHover * 100f) + " 件/日") : "悬停读数", (GameFont)0, (gradHover >= 0f) ? Tokens.Text : Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "22",
			Name = "OverflowFade 渐隐截断",
			Api = "VisualAtoms.OverflowFade(Rect, string, Color bgColor, GameFont, Color?, float fadeWidth, int segments)",
			Desc = "文字右缘渐隐（比「…」优雅的截断暗示）。需横向渐变遮罩贴图，TruncateToWidth 兜底。",
			Note = "bgColor 必须是真实底色，否则渐隐带出现接缝 · 可换一条（长短交替）",
			BodyH = LineSmall + 2f + CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				string[] array3 = new string[2] { "殖民地在第 12 象遭遇持续三天的有毒尘埃，储藏区的草药开始腐烂", "寒流来袭，作物冻伤。" };
				VStack val3 = Layout.Column(r, 2f);
				VisualAtoms.OverflowFade(((VStack)(ref val3)).Take(LineSmall), array3[ovIdx], DemoBg, Tokens.FontBody, (Color?)Tokens.Text, 40f, 8);
				Rect rest2 = ((VStack)(ref val3)).Rest;
				HStack val4 = Layout.Row(((VStack)(ref val3)).Take(((Rect)(ref rest2)).height), 6f);
				if (Widgets.ButtonText(((HStack)(ref val4)).Take(72f), "换一条", true, true, true, (TextAnchor?)null))
				{
					ovIdx = (ovIdx + 1) % array3.Length;
				}
				Draw.EllipsisLabel(((HStack)(ref val4)).Rest, (ovIdx == 0) ? "长文本 → 右缘渐隐" : "短文本 → 放得下，不渐隐", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "23",
			Name = "PinBadge 钉选角标",
			Api = "VisualAtoms.PinBadge(Rect host, bool pinned, Color?, float size, float margin)",
			Interactive = true,
			Desc = "钉选 / 置顶状态角标（AnchorFrame 右上定位的示范用户）。",
			Note = "点卡片切换钉选；钉住的面板在真实场景里会跨页签常驻",
			BodyH = 30f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				HStack val = Layout.Row(r, 8f);
				bool[] array = new bool[3] { pinA, pinB, pinC };
				string[] array2 = new string[3] { "殖民者档案", "事件日志", "研究队列" };
				for (int i = 0; i < array2.Length; i++)
				{
					Rect rest = ((HStack)(ref val)).Rest;
					Rect val2 = ((HStack)(ref val)).Take(Mathf.Min(132f, ((Rect)(ref rest)).width));
					if (((Rect)(ref val2)).width < 40f)
					{
						break;
					}
					Draw.Box(val2, Tokens.PanelRaised);
					Draw.Border(val2, Tokens.BorderSoft);
					Draw.EllipsisLabel(new Rect(((Rect)(ref val2)).x + 6f, ((Rect)(ref val2)).y, ((Rect)(ref val2)).width - 22f, ((Rect)(ref val2)).height), array2[i], (GameFont)0, Tokens.Text, (TextAnchor)3);
					VisualAtoms.PinBadge(val2, array[i], (Color?)null, 12f, 2f);
					if (Widgets.ButtonInvisible(val2, true))
					{
						switch (i)
						{
						case 0:
							pinA = !pinA;
							break;
						case 1:
							pinB = !pinB;
							break;
						default:
							pinC = !pinC;
							break;
						}
					}
				}
			}
		});
		families.Add(family);
	}

	private static void HoverFrame(Rect r)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(r))
		{
			Draw.Border(r, Tokens.Accent);
		}
	}

	private void BuildExt()
	{
		Family family = new Family
		{
			Title = "扩展原子族 · 输入",
			Sub = "14 件 · InputAtoms / InputExAtoms / InputPlusAtoms / InputCandAtoms（深挖补录，编号 43–56）"
		};
		family.Add(new Atom
		{
			No = "43",
			Name = "Segmented 分段选择",
			Api = "InputAtoms.Segmented(Rect, string[], ref int) → bool",
			Interactive = true,
			Desc = "互斥分段选择 —— 取代原版缺失的 RadioButtonGroup。自绘 PanelRaised 底 + 段间分隔线，返回是否变化。",
			Note = "⚠ 与验证台本地 helper 真重复（报告 D1）：本件已改走框架实现并删除本地版本",
			BodyH = CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				string[] array2 = new string[3] { "全部", "只看异常", "只看空闲" };
				InputAtoms.Segmented(Isset(r, 0f, CtrlTiny), array2, ref segIdx);
				HoverFrame(Isset(r, 0f, CtrlTiny));
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + CtrlTiny + 2f, ((Rect)(ref r)).width, LineTiny), "index = " + segIdx + " · 与游戏内验证台同号 43", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "44",
			Name = "ToggleSwitch 开关",
			Api = "InputAtoms.ToggleSwitch(Rect, ref bool) → bool",
			Interactive = true,
			MinW = 40f,
			Desc = "Track 固定 36×18 垂直居中；rect 宽 > 40 时右侧自动补 On/Off 文字。命中区是**整个 rect**（不只 track）。",
			Note = "⚠ 与 34 号 EyeToggle 部分重叠（报告 D2）：按语义选——通断用本件，显隐用 EyeToggle",
			BodyH = CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				HStack val14 = Layout.Row(r, 8f);
				Rect rest7 = ((HStack)(ref val14)).Rest;
				Rect val15 = ((HStack)(ref val14)).Take(Mathf.Min(130f, ((Rect)(ref rest7)).width));
				if (InputAtoms.ToggleSwitch(val15, ref tglOn))
				{
				}
				HoverFrame(val15);
				rest7 = ((HStack)(ref val14)).Rest;
				Rect val16 = ((HStack)(ref val14)).Take(Mathf.Min(130f, ((Rect)(ref rest7)).width));
				if (((Rect)(ref val16)).width > 40f)
				{
					bool flag = false;
					InputAtoms.ToggleSwitch(val16, ref flag);
					Draw.Box(val16, Draw.WithAlpha(Tokens.Window, 0.62f));
					Draw.Border(val16, Tokens.BorderSoft);
					Draw.Label(val16, "禁用", (GameFont)0, Tokens.Dim, (TextAnchor)5);
				}
				Draw.EllipsisLabel(((HStack)(ref val14)).Rest, "左＝可用 · 次＝禁用态（压暗表达）", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "45",
			Name = "Stepper 数值步进",
			Api = "InputAtoms.Stepper(Rect, ref int, int min, int max, int step) → bool",
			Interactive = true,
			MinW = 60f,
			Desc = "−/＋ 步进，内置 min/max 夹取（越界由 InputFmt.StepValue 处理）。左钮 26 宽、右钮绝对定位在 xMax−26。",
			Note = "⚠ 与未落地的 NumericField 重叠（报告 D3）：点击步进 vs 键入，同一值的两条输入通道",
			BodyH = CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				HStack val12 = Layout.Row(r, 8f);
				Rect rest6 = ((HStack)(ref val12)).Rest;
				Rect val13 = ((HStack)(ref val12)).Take(Mathf.Min(150f, ((Rect)(ref rest6)).width));
				if (InputAtoms.Stepper(val13, ref stepVal, 0, 20, 1))
				{
				}
				HoverFrame(val13);
				Draw.EllipsisLabel(((HStack)(ref val12)).Rest, "0–20 · 技能等级口径 · 现 " + stepVal, (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "46",
			Name = "SliderWithValue 滑条+数值",
			Api = "InputAtoms.SliderWithValue(Rect, ref float, float min, float max, float roundTo, Func<float,string>) → bool",
			Interactive = true,
			Desc = "走官方 Widgets.HorizontalSlider（**ref 语义**）。数值标签画在 rect **上方 18px**，变化时下方 2px 处画指示线。",
			Note = "⚠ 外扩绘制（报告 §4）：调用方必须为 rect 外预留「上 18 / 下 4」—— BodyH 已含",
			BodyH = 18f + CtrlTiny + 6f,
			Paint = delegate(Rect r)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				Rect val9 = default(Rect);
				((Rect)(ref val9))..ctor(((Rect)(ref r)).x, ((Rect)(ref r)).y + 18f, ((Rect)(ref r)).width, CtrlTiny);
				HStack val10 = Layout.Row(val9, 8f);
				Rect rest5 = ((HStack)(ref val10)).Rest;
				Rect val11 = ((HStack)(ref val10)).Take(Mathf.Min(260f, ((Rect)(ref rest5)).width));
				InputAtoms.SliderWithValue(val11, ref sliderVal, 0f, 1f, 0.01f, (Func<float, string>)((float v) => Mathf.RoundToInt(v * 100f) + "%"));
				HoverFrame(val11);
				Draw.EllipsisLabel(((HStack)(ref val10)).Rest, "值由 ref 回写", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "47",
			Name = "HotkeyBind 键位绑定",
			Api = "InputAtoms.HotkeyBind(Rect, string label, string currentKey) → bool",
			Interactive = true,
			MinW = 254f,
			Desc = "「动作 + 当前键位 + 改键」三格。内部固定总宽 254（标签 90 + 键框 84 + 按钮 64 + 间距）。",
			Note = "⚠ 内部自排固定宽 254（报告 §4）：窗口窄于该值会被裁切，需自行降级",
			BodyH = CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				HStack val7 = Layout.Row(r, 8f);
				Rect rest4 = ((HStack)(ref val7)).Rest;
				Rect val8 = ((HStack)(ref val7)).Take(Mathf.Min(254f, ((Rect)(ref rest4)).width));
				if (InputAtoms.HotkeyBind(val8, "快速存档", keyArmed ? "F5（监听中）" : "F5"))
				{
					keyArmed = !keyArmed;
				}
				HoverFrame(val8);
				Draw.EllipsisLabel(((HStack)(ref val7)).Rest, "⚠ 捕获需官方 KeyPrefs 流程，本件只做显示 + 回调", (GameFont)0, Tokens.Warn, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "48",
			Name = "TagPicker 多选标签",
			Api = "InputAtoms.TagPicker(Rect, string[] all, bool[] picked, float gap) → bool",
			Interactive = true,
			Desc = "Chips 多选，选中打 ✓；`picked` 由调用方持有。**不折行** —— Row 会自动裁剪，超出即静默少画。",
			Note = "与 DataAtoms.TraitChips 不重叠：那是「显示」，本件是「选择」",
			BodyH = CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				InputAtoms.TagPicker(Isset(r, 0f, CtrlTiny), new string[4] { "勤劳", "嗜血", "乐天派", "夜猫子" }, tagPicked, 6f);
				HoverFrame(Isset(r, 0f, CtrlTiny));
				int num4 = 0;
				for (int j = 0; j < tagPicked.Length; j++)
				{
					if (tagPicked[j])
					{
						num4++;
					}
				}
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + CtrlTiny + 2f, ((Rect)(ref r)).width, LineTiny), "已选 " + num4 + " 项 · 宽度不足时静默少画（需自行降级，报告 §3）", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "49",
			Name = "DoubleSlider 双端范围条",
			Api = "InputExAtoms.DoubleSlider(Rect, ref float lo, ref float hi, float min, float max, float roundTo) → bool",
			Interactive = true,
			MinW = 140f,
			Desc = "范围条（高固定 12）+ Lower/Upper 两行滑条（各 18）。对齐原版 FloatRange / IntRange / QualityRange。",
			Note = "最小高 54、实用最小宽 140（宽不足时滑条只剩 10px）",
			BodyH = 58f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				InputExAtoms.DoubleSlider(Isset(r, 0f, 54f), ref dsLo, ref dsHi, 0f, 100f, 1f);
				HoverFrame(Isset(r, 0f, 54f));
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 56f, ((Rect)(ref r)).width, LineTiny), "区间 " + Mathf.RoundToInt(dsLo) + " – " + Mathf.RoundToInt(dsHi), (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "50",
			Name = "ColorPicker HSV 取色器",
			Api = "InputExAtoms.ColorPicker(Rect, ref Color, ref float h, ref float s, ref float v) → bool",
			Interactive = true,
			Desc = "预览色块（30 宽）+ #HEX + H/S/V 三条滑条。**配套 InputExFmt.HsvColor / RgbToHsv 为手写实现**，不依赖 Color.HSVToRGB。",
			Note = "最小高 83（20 预览 + 三条 18 + 间隙）",
			BodyH = 88f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				InputExAtoms.ColorPicker(Isset(r, 0f, 83f), ref cpColor, ref cpH, ref cpS, ref cpV);
				HoverFrame(Isset(r, 0f, 83f));
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 85f, ((Rect)(ref r)).width, LineTiny), "H " + cpH.ToString("0.00") + " · S " + cpS.ToString("0.00") + " · V " + cpV.ToString("0.00"), (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "51",
			Name = "CycleButton 循环枚举钮",
			Api = "InputExAtoms.CycleButton(Rect, string label, string[] values, ref int index) → bool",
			Interactive = true,
			MinW = 190f,
			Desc = "单击循环到下一个枚举值 —— 占地最小、点击最少的状态切换件。**悬停时边框转 accent**（本件自带 hover 反馈）。",
			Note = "⚠ 与 43 号 Segmented 部分重叠（报告 D5）：选项 ≤4 且空间足用 43，空间紧张用本件",
			BodyH = CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				string[] array = new string[4] { "任意", "工作", "娱乐", "休息" };
				HStack val5 = Layout.Row(r, 8f);
				Rect rest3 = ((HStack)(ref val5)).Rest;
				Rect val6 = ((HStack)(ref val5)).Take(Mathf.Min(190f, ((Rect)(ref rest3)).width));
				InputExAtoms.CycleButton(val6, "作息", array, ref cycIdx);
				Draw.EllipsisLabel(((HStack)(ref val5)).Rest, "index = " + cycIdx + " → " + array[cycIdx], (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "52",
			Name = "EditableLabel 行内编辑",
			Api = "InputExAtoms.EditableLabel(Rect, ref string text, ref bool editing, ref string buffer) → bool",
			Interactive = true,
			Desc = "就地编辑：editing 为真时画 TextField（占 Rest），否则画文本 + 提交按钮。**editing / buffer 全由调用方持有**。",
			Note = "与 SearchField 不同：本件是就地改名，不是防抖搜索",
			BodyH = CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				HStack val3 = Layout.Row(r, 8f);
				Rect rest2 = ((HStack)(ref val3)).Rest;
				Rect val4 = ((HStack)(ref val3)).Take(Mathf.Min(240f, ((Rect)(ref rest2)).width));
				if (InputExAtoms.EditableLabel(val4, ref editText, ref editing, ref editBuffer))
				{
					editing = false;
				}
				HoverFrame(val4);
				Draw.EllipsisLabel(((HStack)(ref val3)).Rest, "editing = " + editing + " · text = " + editText, (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "53",
			Name = "PriorityGrid 优先级网格",
			Api = "InputExAtoms.PriorityGrid(Rect, string[] rowLabels, string[] colLabels, int[] values, out int hitIndex) → bool",
			Interactive = true,
			Desc = "列头（高 16）+ 行标签 + 每格一个整数值，点格子递进。工作优先级 / 技能分配 / 政策矩阵通用。",
			Note = "最小高 = 18 + rows×rowH；与 TimetableMatrix / PaintStrokeGrid 构成「矩阵族」（报告 D7/D8）",
			BodyH = 80f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				int num3 = default(int);
				InputExAtoms.PriorityGrid(Isset(r, 0f, 78f), new string[3] { "建造", "采矿", "手工" }, new string[4] { "P1", "P2", "P3", "P4" }, pgVals, ref num3);
				HoverFrame(Isset(r, 0f, 78f));
			}
		});
		family.Add(new Atom
		{
			No = "54",
			Name = "TimetableMatrix 时间表矩阵",
			Api = "InputExAtoms.TimetableMatrix(Rect, string[] rowLabels, int[] states, out int hitIndex) → bool",
			Interactive = true,
			Desc = "作息表：行 = 时间轴，列 = 24 时，点格子循环状态（官方 TimeAssignmentDef 四态；状态 2 用更低透明度）。",
			Note = "最小高 = rows×rowH；行标签由 rowLabels 提供",
			BodyH = 48f,
			Paint = delegate(Rect r)
			{
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				if (ttStates[8] == 0 && ttStates[20] == 0)
				{
					for (int i = 0; i < 24; i++)
					{
						ttStates[i] = ((i >= 8 && i < 18) ? 1 : ((i >= 20 || i < 6) ? 3 : 0));
					}
				}
				int num2 = default(int);
				InputExAtoms.TimetableMatrix(Isset(r, 0f, 44f), new string[1] { "作息" }, ttStates, ref num2);
				HoverFrame(Isset(r, 0f, 44f));
			}
		});
		family.Add(new Atom
		{
			No = "55",
			Name = "SliderTicks 刻度吸附滑条",
			Api = "InputPlusAtoms.SliderTicks(Rect, ref float, float[] ticks, float min, float max) → bool",
			Interactive = true,
			MinW = 178f,
			Desc = "拖动后吸附到最近刻度 —— 技能等级 / 品质档位 / 离散参数的专用滑条。",
			Note = "⚠ 与 46 号部分重叠（报告 D4）：本件是 SliderWithValue 的超集（多 ticks），连续量用 46、离散档用本件。宽需 ≥ 178",
			BodyH = CtrlTiny,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				HStack val = Layout.Row(r, 8f);
				Rect rest = ((HStack)(ref val)).Rest;
				Rect val2 = ((HStack)(ref val)).Take(Mathf.Min(280f, ((Rect)(ref rest)).width));
				InputPlusAtoms.SliderTicks(val2, ref ticksVal, new float[5] { 0f, 5f, 10f, 15f, 20f }, 0f, 20f);
				HoverFrame(val2);
				int num = Mathf.Clamp(Mathf.RoundToInt(ticksVal), 0, 20);
				Draw.EllipsisLabel(((HStack)(ref val)).Rest, SkillRanks[num], (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "56",
			Name = "BatchSliders 批量滑条",
			Api = "InputCandAtoms.BatchSliders(Rect, SliderCfg[] cfg, float[] values)",
			Interactive = true,
			MinW = 200f,
			Desc = "一组同规格滑条，由 SliderCfg{label,min,max,unit} 驱动 —— 设置页与批量参数调整的紧凑形态。返回 void（值就地写入 values）。",
			Note = "⚠ 本质是**容器/组合件**（报告 D9），不计入原子口径统计",
			BodyH = 72f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				InputCandAtoms.BatchSliders(Isset(r, 0f, 72f), batchCfg, batchVals);
				HoverFrame(Isset(r, 0f, 72f));
			}
		});
		families.Add(family);
	}

	private void BuildBase1()
	{
		Family family = new Family
		{
			Title = "宝典 · A1/A2 展示件",
			Sub = "6 件 · 组合范式（RUIF 无内置件，用已有原子组装）· 编号 57–62"
		};
		family.Add(new Atom
		{
			No = "57",
			Name = "StatChip 状态芯片",
			Api = "组合范式 · StatChip(icon, label, value, trend)",
			Desc = "图标 + 标签 + 大数值 + 趋势箭头的紧凑指标件，仪表盘原子单元。",
			Note = "基座：Box + Border + 单元矩形五段排版｜图标位用汉字（IMGUI 字库对 emoji 不可靠，接图标族后可替换）",
			BodyH = 34f,
			MinW = 210f,
			MinH = 34f,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				HStack row2 = Layout.Row(r, 8f);
				StatChip(row2, "人", "人口", "12", "▲2", Tokens.Alive);
				StatChip(row2, "战", "威胁", "3.4", "▼0.6", Tokens.Dead);
				StatChip(row2, "物", "物资", "982", "—", Tokens.Dim);
			}
		});
		family.Add(new Atom
		{
			No = "58",
			Name = "DeltaBadge 涨跌徽章",
			Api = "组合范式 · DeltaBadge(float delta01, string unit)",
			Desc = "独立趋势徽章：正绿负红零灰（Trend 语义驱动），可附加环比说明。",
			Note = "点徽章选中（选中转 accent 边，状态归调用方）｜三态并排，便于一次看全语义色",
			BodyH = 54f,
			MinW = 190f,
			MinH = 54f,
			Interactive = true,
			Paint = delegate(Rect r)
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				HStack val14 = Layout.Row(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 34f), 8f);
				float[] array6 = new float[3] { 0.18f, -0.07f, 0f };
				string[] array7 = new string[3] { "% 环比", "% 环比", "持平" };
				for (int k = 0; k < array6.Length; k++)
				{
					Rect rest = ((HStack)(ref val14)).Rest;
					float width = ((Rect)(ref rest)).width;
					if (width < 44f)
					{
						break;
					}
					Rect val15 = ((HStack)(ref val14)).Take(Mathf.Min(112f, width));
					DeltaBadge(val15, array6[k], array7[k], badgeSlot == k);
					if (Widgets.ButtonInvisible(val15, true))
					{
						badgeSlot = k;
					}
					HoverFrame(val15);
				}
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 36f, ((Rect)(ref r)).width, LineTiny), "点任一徽章选中 · 语义色随正负零（涨绿 / 跌红 / 持平灰）", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "59",
			Name = "AvatarPawn 殖民者头像环",
			Api = "组合范式 · AvatarPawn(Texture portrait, int tier)",
			Desc = "肖像 + 职称档位色环（对齐职称档位色）+ 状态角标。环与角标为原生绘制，肖像由消费方提供。",
			Note = "宝典原文：肖像贴图由消费 Mod 提供｜本件无贴图，用首字代肖像位；色环 3 档对齐宝典演示（正高 / 副高 / 阵亡）",
			BodyH = 62f,
			MinW = 200f,
			MinH = 62f,
			Interactive = true,
			Paint = delegate(Rect r)
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0167: Unknown result type (might be due to invalid IL or missing references)
				//IL_0172: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				HStack val11 = Layout.Row(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 42f), 14f);
				string[] array3 = new string[3] { "张", "李", "王" };
				Color[] array4 = (Color[])(object)new Color[3]
				{
					Tokens.Alive,
					Tokens.Info,
					Tokens.Dead
				};
				string[] array5 = new string[3] { "正高", "副高", "阵亡" };
				for (int j = 0; j < array3.Length; j++)
				{
					Rect val12 = ((HStack)(ref val11)).Take(46f);
					if (((Rect)(ref val12)).width < 30f)
					{
						break;
					}
					VStack val13 = Layout.Column(val12, 2f);
					Avatar(((VStack)(ref val13)).Take(34f), array3[j], array4[j], avatarSel == j);
					Draw.EllipsisLabel(((VStack)(ref val13)).Rest, array5[j], (GameFont)0, Tokens.Dim, (TextAnchor)4);
					if (Widgets.ButtonInvisible(val12, true))
					{
						avatarSel = j;
					}
					HoverFrame(val12);
				}
				Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).yMax - LineTiny, ((Rect)(ref r)).width, LineTiny), "点选切换 · 环色 = 职称档位（金环正高 / 蓝环副高 / 红环阵亡）", (GameFont)0, Tokens.Muted, (TextAnchor)3);
			}
		});
		family.Add(new Atom
		{
			No = "60",
			Name = "QualityStar 品质星阶",
			Api = "组合范式 · QualityStar(QualityCategory q)",
			Desc = "对齐原版 QualityCategory 全档（Awful→Legendary），色值取品质语义色，跨主题可辨。",
			Note = "⚠ C 类（预览缺失）：宝典预览列 6 行，原版 QualityCategory 实为 7 档（另有 Poor 较差）——本件按原版 7 档落地",
			BodyH = 84f,
			MinW = 240f,
			MinH = 84f,
			Interactive = true,
			Paint = delegate(Rect r)
			{
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				QualityCategory[] array = new QualityCategory[7];
				RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				QualityCategory[] array2 = (QualityCategory[])(object)array;
				float num2 = Mathf.Max(LineTiny + 2f, (((Rect)(ref r)).height - 3f) / 4f);
				float num3 = (((Rect)(ref r)).width - 8f) * 0.5f;
				Rect val10 = default(Rect);
				for (int i = 0; i < array2.Length; i++)
				{
					((Rect)(ref val10))..ctor(((Rect)(ref r)).x + (float)(i % 2) * (num3 + 8f), ((Rect)(ref r)).y + (float)(i / 2) * (num2 + 1f), num3, num2);
					QualityRow(val10, array2[i], qualitySel == i);
					if (Widgets.ButtonInvisible(val10, true))
					{
						qualitySel = i;
					}
					HoverFrame(val10);
				}
			}
		});
		family.Add(new Atom
		{
			No = "61",
			Name = "Skeleton 骨架屏",
			Api = "组合范式 · Skeleton(SkeletonLayout)",
			Desc = "数据未就绪时的呼吸占位（透明度 0.5~1.0 正弦插值，IMGUI 每帧重绘天然支持）。替代「空白等待」。",
			Note = "点一下切「加载完成」：同一矩形内换成真实内容 —— 空态与就绪态共用一次布局",
			BodyH = 96f,
			MinW = 190f,
			MinH = 96f,
			Interactive = true,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0148: Unknown result type (might be due to invalid IL or missing references)
				//IL_014d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0160: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0177: Unknown result type (might be due to invalid IL or missing references)
				//IL_017c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0209: Unknown result type (might be due to invalid IL or missing references)
				//IL_020e: Unknown result type (might be due to invalid IL or missing references)
				//IL_021a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0245: Unknown result type (might be due to invalid IL or missing references)
				//IL_0250: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				if (Widgets.ButtonInvisible(r, true))
				{
					skelLoaded = !skelLoaded;
				}
				HoverFrame(r);
				if (skelLoaded)
				{
					VStack val3 = Layout.Column(r, 4f);
					HStack val4 = Layout.Row(((VStack)(ref val3)).Take(LineSmall + 6f), 8f);
					Avatar(((HStack)(ref val4)).Take(28f), "蓝", Tokens.Accent, selected: false);
					Draw.EllipsisLabel(((HStack)(ref val4)).Rest, Names[0], (GameFont)1, Tokens.Text, (TextAnchor)3);
					Draw.EllipsisLabel(((VStack)(ref val3)).Take(LineTiny), "心情 72% · 健康 94% · 无病症", (GameFont)0, Tokens.Muted, (TextAnchor)3);
					Shapes.GradientRule(((VStack)(ref val3)).Take(LineTiny), Tokens.Accent, 10);
					Draw.EllipsisLabel(((VStack)(ref val3)).Take(LineTiny), "数据已就绪（再点回到骨架态）", (GameFont)0, Tokens.Alive, (TextAnchor)3);
				}
				else
				{
					float num = Mathf.Lerp(0.5f, 1f, (Mathf.Sin(t * 2.2f) + 1f) * 0.5f);
					VStack val5 = Layout.Column(r, 4f);
					HStack val6 = Layout.Row(((VStack)(ref val5)).Take(34f), 9f);
					Draw.Box(((HStack)(ref val6)).Take(34f), Draw.WithAlpha(Tokens.BorderSoft, num));
					VStack val7 = Layout.Column(((HStack)(ref val6)).Rest, 6f);
					Rect val8 = ((VStack)(ref val7)).Take(9f);
					Draw.Box(new Rect(((Rect)(ref val8)).x, ((Rect)(ref val8)).y, ((Rect)(ref val8)).width * 0.6f, ((Rect)(ref val8)).height), Draw.WithAlpha(Tokens.BorderSoft, num));
					Rect val9 = ((VStack)(ref val7)).Take(7f);
					Draw.Box(new Rect(((Rect)(ref val9)).x, ((Rect)(ref val9)).y, ((Rect)(ref val9)).width * 0.85f, ((Rect)(ref val9)).height), Draw.WithAlpha(Tokens.BorderSoft, num * 0.7f));
					Draw.Box(((VStack)(ref val5)).Take(8f), Draw.WithAlpha(Tokens.BorderSoft, num * 0.8f));
					Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).yMax - LineTiny, ((Rect)(ref r)).width, LineTiny), "加载中… 骨架占位（点一下切到就绪态）", (GameFont)0, Tokens.Dim, (TextAnchor)3);
				}
			}
		});
		family.Add(new Atom
		{
			No = "62",
			Name = "KeyCap 键位帽",
			Api = "组合范式 · KeyCap(string key)",
			Desc = "键位显示件（立体下边框拟物）。快捷键提示、命令面板、设置键位列表通用。",
			Note = "立体感 = 下边 2px 暗边；按下时暗边收 1px 且文字下移 1px —— 拟物件才有的按压反馈",
			BodyH = 50f,
			MinW = 230f,
			MinH = 50f,
			Interactive = true,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				VStack val = Layout.Column(r, 2f);
				Rect val2 = ((VStack)(ref val)).Take(26f);
				HStack row = Layout.Row(val2, 6f);
				KeyCap(row, "Ctrl", keyDown == 0);
				Draw.EllipsisLabel(((HStack)(ref row)).Take(10f), "+", (GameFont)0, Tokens.Dim, (TextAnchor)4);
				KeyCap(row, "K", keyDown == 1);
				Draw.EllipsisLabel(((HStack)(ref row)).Rest, "打开命令面板", (GameFont)0, Tokens.Muted, (TextAnchor)3);
				KeyCapBox(new Rect(((Rect)(ref r)).xMax - 40f, ((Rect)(ref r)).y, 40f, 26f), "Esc", keyDown == 2);
				if (Widgets.ButtonInvisible(r, true))
				{
					keyDown = (keyDown + 1) % 3;
				}
				HoverFrame(r);
				Draw.EllipsisLabel(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).yMax + 2f, ((Rect)(ref val2)).width, LineTiny), "点任意处循环按下状态（Ctrl → K → Esc）", (GameFont)0, Tokens.Dim, (TextAnchor)3);
			}
		});
		families.Add(family);
	}

	private static void StatChip(HStack row, string icon, string label, string value, string trend, Color trendColor)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		float num = 12f + Measure.Width(icon, (GameFont)0) + 5f + Measure.Width(label, (GameFont)0) + 7f + Measure.Width(value, (GameFont)1) + 6f + Measure.Width(trend, (GameFont)0) + 8f;
		Rect rest = ((HStack)(ref row)).Rest;
		if (!(((Rect)(ref rest)).width < 34f))
		{
			Rect val = ((HStack)(ref row)).Take(num);
			Draw.Box(val, Tokens.Card);
			Draw.Border(val, Tokens.BorderSoft);
			HStack val2 = Layout.Row(Layout.Inset(val, 5f, 0f), 0f);
			Draw.EllipsisLabel(((HStack)(ref val2)).Take(Measure.Width(icon, (GameFont)0) + 5f), icon, (GameFont)0, Tokens.Muted, (TextAnchor)3);
			Draw.EllipsisLabel(((HStack)(ref val2)).Take(Measure.Width(label, (GameFont)0) + 7f), label, (GameFont)0, Tokens.Muted, (TextAnchor)3);
			Draw.Label(((HStack)(ref val2)).Take(Measure.Width(value, (GameFont)1) + 6f), value, (GameFont)1, Tokens.Text, (TextAnchor)3);
			Draw.Label(((HStack)(ref val2)).Rest, trend, (GameFont)0, trendColor, (TextAnchor)3);
		}
	}

	private static void DeltaBadge(Rect r, float delta01, string unit, bool selected)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		Color val = ((delta01 > 0.001f) ? Tokens.Alive : ((delta01 < -0.001f) ? Tokens.Dead : Tokens.Muted));
		Draw.Box(r, Draw.WithAlpha(val, 0.15f));
		Draw.Border(r, selected ? Tokens.Accent : val);
		string text = ((delta01 > 0.001f) ? ("▲ +" + Mathf.RoundToInt(delta01 * 100f) + "%") : ((delta01 < -0.001f) ? ("▼ −" + Mathf.RoundToInt((0f - delta01) * 100f) + "%") : ("— " + unit)));
		Draw.EllipsisLabel(r, text, (GameFont)1, val, (TextAnchor)4);
	}

	private static void Avatar(Rect r, string initial, Color ring, bool selected)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		Draw.Box(r, Draw.WithAlpha(ring, 0.18f));
		Draw.Border(r, Tokens.Card);
		Draw.Border(new Rect(((Rect)(ref r)).x - 1f, ((Rect)(ref r)).y - 1f, ((Rect)(ref r)).width + 2f, ((Rect)(ref r)).height + 2f), ring);
		if (selected)
		{
			Draw.Border(new Rect(((Rect)(ref r)).x - 2f, ((Rect)(ref r)).y - 2f, ((Rect)(ref r)).width + 4f, ((Rect)(ref r)).height + 4f), Tokens.Accent);
		}
		Draw.Label(r, initial, (GameFont)1, Tokens.Text, (TextAnchor)4);
	}

	private static void QualityRow(Rect r, QualityCategory q, bool selected)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		int num = QualitySteps(q);
		Color val = QualityTint(q);
		Draw.Box(r, selected ? Draw.WithAlpha(val, 0.14f) : Tokens.Card);
		Draw.Border(r, selected ? Tokens.Accent : Tokens.BorderSoft);
		HStack val2 = Layout.Row(Layout.Inset(r, 5f, 0f), 6f);
		string text = new string('★', num) + new string('☆', 6 - num);
		float num2 = Measure.Width(text, (GameFont)0) + 3f;
		Rect rest = ((HStack)(ref val2)).Rest;
		if (((Rect)(ref rest)).width >= num2)
		{
			Draw.Label(((HStack)(ref val2)).Take(num2), text, (GameFont)0, val, (TextAnchor)3);
			Draw.EllipsisLabel(((HStack)(ref val2)).Rest, QualityLabel(q), (GameFont)0, val, (TextAnchor)3);
		}
		else
		{
			Draw.EllipsisLabel(((HStack)(ref val2)).Rest, QualityLabel(q), (GameFont)0, val, (TextAnchor)3);
		}
	}

	private static int QualitySteps(QualityCategory q)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected I4, but got Unknown
		return (int)q switch
		{
			0 => 1, 
			1 => 2, 
			2 => 3, 
			3 => 4, 
			4 => 5, 
			5 => 6, 
			_ => 6, 
		};
	}

	private static string QualityLabel(QualityCategory q)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected I4, but got Unknown
		return (int)q switch
		{
			0 => "粗劣", 
			1 => "较差", 
			2 => "普通", 
			3 => "良好", 
			4 => "优秀", 
			5 => "杰作", 
			_ => "传奇", 
		};
	}

	private static Color QualityTint(QualityCategory q)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected I4, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		return (Color)((int)q switch
		{
			0 => Tokens.Dim, 
			1 => Tokens.Muted, 
			2 => Tokens.Muted, 
			3 => Tokens.Alive, 
			4 => Tokens.Info, 
			5 => Tokens.Accent, 
			_ => Tokens.Accent, 
		});
	}

	private static void KeyCap(HStack row, string key, bool down)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Max(30f, Measure.Width(key, (GameFont)0) + 14f);
		Rect rest = ((HStack)(ref row)).Rest;
		if (!(((Rect)(ref rest)).width < 14f))
		{
			KeyCapBox(((HStack)(ref row)).Take(num), key, down);
		}
	}

	private static void KeyCapBox(Rect cap, string key, bool down)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		int num = (down ? 1 : 2);
		Draw.Box(new Rect(((Rect)(ref cap)).x, ((Rect)(ref cap)).y + (float)num, ((Rect)(ref cap)).width, ((Rect)(ref cap)).height - (float)num), Tokens.Border);
		Draw.Box(cap, Tokens.Card);
		Draw.Border(new Rect(((Rect)(ref cap)).x, ((Rect)(ref cap)).y, ((Rect)(ref cap)).width, ((Rect)(ref cap)).height - (float)num), Tokens.BorderSoft);
		Draw.EllipsisLabel(new Rect(((Rect)(ref cap)).x, ((Rect)(ref cap)).y + (down ? 1f : 0f), ((Rect)(ref cap)).width, ((Rect)(ref cap)).height - (float)num), key, (GameFont)0, down ? Tokens.Accent : Tokens.Text, (TextAnchor)4);
	}

	private void BuildDInput()
	{
		Family family = new Family
		{
			Title = "宝典 · d-input 输入",
			Sub = "1 件 · InputAtoms.SearchField（防抖搜索框）· 编号 63"
		};
		family.Add(new Atom
		{
			No = "63",
			Name = "SearchField 搜索框",
			Api = "InputAtoms.SearchField(Rect, ref query, ref buffer, placeholder, ref lastInputTime, delay) → bool",
			Desc = "包装 Widgets.TextField：buffer 即时回显，query 防抖 200ms 后才更新；返回 query 是否刚更新。",
			Note = "⚠ 宝典 sig 写 SearchField(Rect, ref query, bool clearable)，RUIF 实现无 clearable、不画清空钮 ⇒ 清空钮由调用方绘制（本件已补）｜placeholder 由原子自画，勿重复",
			BodyH = 68f,
			MinW = 220f,
			MinH = 68f,
			Interactive = true,
			Paint = delegate(Rect r)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_0109: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01df: Unknown result type (might be due to invalid IL or missing references)
				//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_02af: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0321: Unknown result type (might be due to invalid IL or missing references)
				//IL_031a: Unknown result type (might be due to invalid IL or missing references)
				//IL_033b: Unknown result type (might be due to invalid IL or missing references)
				VStack val = Layout.Column(r, 2f);
				Rect val2 = ((VStack)(ref val)).Take(CtrlTiny);
				HStack val3 = Layout.Row(val2, 4f);
				float num = 24f;
				Rect rest = ((HStack)(ref val3)).Rest;
				if (((Rect)(ref rest)).width < num + 44f)
				{
					Draw.EllipsisLabel(val2, "空间不足（D6）", (GameFont)0, Tokens.Dead, (TextAnchor)3);
				}
				else
				{
					rest = ((HStack)(ref val3)).Rest;
					Rect val4 = ((HStack)(ref val3)).Take(((Rect)(ref rest)).width - num - 4f);
					Rect val5 = default(Rect);
					((Rect)(ref val5))..ctor(((Rect)(ref val4)).xMax + 4f, ((Rect)(ref val4)).y, num, ((Rect)(ref val4)).height);
					InputAtoms.SearchField(val4, ref sfQuery, ref sfBuffer, "搜索殖民者…", ref sfLast, 0.2f);
					bool flag = !string.IsNullOrEmpty(sfBuffer);
					bool flag2 = flag && Mouse.IsOver(val5);
					Color val6 = ((!flag) ? Tokens.Dim : (flag2 ? Tokens.Accent : Tokens.Muted));
					Draw.Box(val5, Tokens.Card);
					Draw.Border(val5, val6);
					Draw.Label(val5, "×", (GameFont)1, val6, (TextAnchor)4);
					if (flag && Widgets.ButtonInvisible(val5, true))
					{
						sfBuffer = "";
						sfQuery = "";
					}
					Draw.EllipsisLabel(((VStack)(ref val)).Take(LineTiny), "实时 buffer：" + ((sfBuffer.Length == 0) ? "（空）" : ("「" + sfBuffer + "」")) + "\u3000防抖 query：" + ((sfQuery.Length == 0) ? "（空）" : ("「" + sfQuery + "」")), (GameFont)0, (sfBuffer != sfQuery) ? Tokens.Warn : Tokens.Dim, (TextAnchor)3);
					int num2 = ((sfQuery.Length == 0) ? Names.Length : 0);
					string text = "";
					for (int i = 0; i < Names.Length; i++)
					{
						if (sfQuery.Length != 0 && Names[i].IndexOf(sfQuery, StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2++;
							if (text.Length < 30)
							{
								text = text + ((text.Length > 0) ? " · " : "") + Names[i];
							}
						}
					}
					Rect val7 = ((VStack)(ref val)).Take(LineTiny);
					Draw.EllipsisLabel(val7, "命中 " + num2 + " / " + Names.Length + ((num2 > 0 && sfQuery.Length > 0) ? ("：" + text) : ""), (GameFont)0, (num2 > 0) ? Tokens.Alive : Tokens.Dead, (TextAnchor)3);
					if (sfQuery.Length > 0 && Widgets.ButtonInvisible(val7, true))
					{
						sfBuffer = "";
						sfQuery = "";
					}
				}
			}
		});
		families.Add(family);
	}

	private void BuildDData()
	{
		Family family = new Family
		{
			Title = "宝典 · d-data 数据展示",
			Sub = "3 件 · DataAtoms.PawnCard / SkillBars / NeedsPanel（真名落地）· 编号 64–66"
		};
		family.Add(new Atom
		{
			No = "64",
			Name = "PawnCard 殖民者卡",
			Api = "DataAtoms.PawnCard(Rect, initial, name, title, mood01, health01, status) → void",
			Desc = "头像环 + 名字职称 + 心情/健康微型条 + 状态胶囊一体卡。档案馆与列表页的 hero 组件。",
			Note = "组件**自画底与边框**（Box+Border）⇒ 调用方不得再套底｜内部：头像 ≤34、状态胶囊定宽 52、双条各定宽 60 ⇒ rect 高 ≥ 62、宽 ≥ 190｜数值为宝典预览样例（同源）",
			BodyH = 84f,
			MinW = 200f,
			MinH = 84f,
			Interactive = true,
			Paint = delegate(Rect r)
			{
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0156: Unknown result type (might be due to invalid IL or missing references)
				Rect val3 = default(Rect);
				((Rect)(ref val3))..ctor(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 62f);
				if (((Rect)(ref val3)).width < 190f)
				{
					Draw.EllipsisLabel(val3, "空间不足（D6）", (GameFont)0, Tokens.Dead, (TextAnchor)3);
				}
				else
				{
					float num5 = 0.82f + 0.05f * Mathf.Sin(t * 0.9f);
					float num6 = Mathf.Clamp01(0.94f + 0.04f * Mathf.Sin(t * 1.3f));
					DataAtoms.PawnCard(val3, "张", "张三", "正高 · 总工程师", num5, num6, pcSelected ? "已选中" : "在线");
					if (Widgets.ButtonInvisible(val3, true))
					{
						pcSelected = !pcSelected;
					}
					HoverFrame(val3);
					Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 64f, ((Rect)(ref r)).width, LineTiny), "心情 " + Mathf.RoundToInt(num5 * 100f) + " · 健康 " + Mathf.RoundToInt(num6 * 100f) + "｜点卡切换状态胶囊", (GameFont)0, Tokens.Dim, (TextAnchor)3);
				}
			}
		});
		family.Add(new Atom
		{
			No = "65",
			Name = "SkillBars 技能条组",
			Api = "DataAtoms.SkillBars(Rect, string[] names, float[] levels, maxLevel = 20f, columns = 2) → void",
			Desc = "技能条组：深度 0~20 语义色阶，行内叠印等级数字。",
			Note = "⚠ 宝典 desc 声称「10/20 刻度线 + 燃烧标识 + 无兴趣置灰」，**实现里没有**（只有条 + 数值）——登记为 A 类差异｜点条切 columns 1↔2，演示列数自适应｜数据为宝典预览样例（5 项，columns=2 时第 6 格自然留空）",
			BodyH = 82f,
			MinW = 230f,
			MinH = 82f,
			Interactive = true,
			Paint = delegate(Rect r)
			{
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				int num3 = (sbOneCol ? 1 : 2);
				float num4 = Mathf.Ceil((float)SkillNames.Length / (float)num3) * (LineTiny + 2f);
				Rect val2 = default(Rect);
				((Rect)(ref val2))..ctor(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, num4);
				if (((Rect)(ref val2)).width < 120f)
				{
					Draw.EllipsisLabel(val2, "空间不足（D6）", (GameFont)0, Tokens.Dead, (TextAnchor)3);
				}
				else
				{
					DataAtoms.SkillBars(val2, SkillNames, SkillBase, 20f, num3);
					if (Widgets.ButtonInvisible(val2, true))
					{
						sbOneCol = !sbOneCol;
					}
					HoverFrame(val2);
					Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref val2)).yMax + 2f, ((Rect)(ref r)).width, LineTiny), "columns = " + num3 + " · maxLevel = 20｜点条切换列数", (GameFont)0, Tokens.Dim, (TextAnchor)3);
				}
			}
		});
		family.Add(new Atom
		{
			No = "66",
			Name = "NeedsPanel 需求面板",
			Api = "DataAtoms.NeedsPanel(Rect, string[] names, float[] values) → void",
			Desc = "需求条组（心情/饮食/休息/娱乐）：名称 + 条 + 百分比，条色走阈值色带。",
			Note = "⚠ 宝典 desc 声称「低于阈值自动闪红 + 心情修正浮动提示」，**实现是 TableFmt.BandColor 阈值色带 + 数值叠印**（无闪烁、无浮层）——登记为 A 类差异｜点面板切「全 4 项 / 前 2 项」，演示项数自适应｜数据为宝典预览样例",
			BodyH = 108f,
			MinW = 160f,
			MinH = 108f,
			Interactive = true,
			Paint = delegate(Rect r)
			{
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				int num = (npFew ? 2 : NeedNames.Length);
				float num2 = (float)num * (LineTiny + 3f);
				Rect val = default(Rect);
				((Rect)(ref val))..ctor(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, num2);
				if (((Rect)(ref val)).width < 150f)
				{
					Draw.EllipsisLabel(val, "空间不足（D6）", (GameFont)0, Tokens.Dead, (TextAnchor)3);
				}
				else
				{
					string[] array = (npFew ? NeedNamesFew : NeedNames);
					float[] array2 = (npFew ? NeedValuesFew : NeedValues);
					DataAtoms.NeedsPanel(val, array, array2);
					if (Widgets.ButtonInvisible(val, true))
					{
						npFew = !npFew;
					}
					HoverFrame(val);
					Draw.EllipsisLabel(new Rect(((Rect)(ref r)).x, ((Rect)(ref val)).yMax + 2f, ((Rect)(ref r)).width, LineTiny), "项数 " + num + " / 4（低值自动走告警色带）｜点面板切换项数", (GameFont)0, Tokens.Dim, (TextAnchor)3);
				}
			}
		});
		families.Add(family);
	}
}
internal sealed class BenchReorder : IBench
{
	private readonly List<string> items = new List<string> { "建造", "烹饪", "制作", "医生", "灭火", "种植", "驯兽", "搬运" };

	private const float RowH = 26f;

	private int groupId;

	private string lastMsg = "（尚未拖动；按住任意行向下/上拖）";

	private int lastFrom = -1;

	private int lastTo = -1;

	public bool Scrollable => false;

	public float Height(float width)
	{
		return (float)items.Count * 26f + 300f;
	}

	public void Render(Rect area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		VStack val = Layout.Column(area, 16f);
		VStack val2 = Layout.Column(Atoms.SectionFrame(((VStack)(ref val)).Take(114f), "列表拖拽重排（ReorderAtoms · 桥 Verse.ReorderableWidget）"), 3f);
		Draw.EllipsisLabel(((VStack)(ref val2)).Take(18f), "原版白送：5px 起拖阈值 · 起拖/放下音效 · 2px 白色插入线 · 被拖行高亮。", (GameFont)0, Tokens.Text, (TextAnchor)3);
		Draw.EllipsisLabel(((VStack)(ref val2)).Take(18f), "组 id 由本面持有（ref int）—— 非 Repaint 帧 NewGroup 返回 -1，直接用它登记会导致拖拽永远起不来。", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		Draw.EllipsisLabel(((VStack)(ref val2)).Take(18f), "行数变化会触发原版引擎的 StopDragging()（reorderables.Count 变化时自动中断）。", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		Rect val3 = Atoms.SectionFrame(((VStack)(ref val)).Take((float)items.Count * 26f + 60f), "工作类型（拖动重排）");
		Rect val4 = default(Rect);
		((Rect)(ref val4))..ctor(((Rect)(ref val3)).x, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width, (float)items.Count * 26f);
		ReorderAtoms.List(val4, items.Count, 26f, ref groupId, new ReorderRowDrawer(DrawRow), (Action<int, int>)OnReorder, true);
		VStack val5 = Layout.Column(Atoms.SectionFrame(((VStack)(ref val)).Take(98f), "回调语义回显（ReorderFmt）"), 3f);
		Draw.EllipsisLabel(((VStack)(ref val5)).Take(18f), lastMsg, (GameFont)0, Tokens.Text, (TextAnchor)3);
		Draw.EllipsisLabel(((VStack)(ref val5)).Take(18f), (lastFrom < 0) ? "LandingIndex / WillChange：等待首次拖动" : ("from=" + lastFrom + " insert=" + lastTo + "\u3000LandingIndex=" + ReorderFmt.LandingIndex(lastFrom, lastTo) + "\u3000WillChange=" + ReorderFmt.WillChange(lastFrom, lastTo) + "（insert == from 或 from+1 时原版不回调，就是插回原位）"), (GameFont)0, Tokens.Info, (TextAnchor)3);
		Draw.Label(((VStack)(ref val5)).Take(18f), "顺序：" + string.Join(" › ", items.ToArray()), (GameFont)0, Tokens.Alive, (TextAnchor)3);
	}

	private void DrawRow(int index, Rect row, bool dragging)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		Draw.Box(row, dragging ? Tokens.CardHover : Tokens.Card);
		Draw.Border(row, dragging ? Tokens.Accent : Tokens.BorderSoft);
		HStack val = Layout.Row(row, 8f);
		ReorderAtoms.Grip(((HStack)(ref val)).Take(26f), dragging);
		Draw.Label(((HStack)(ref val)).Take(34f), (index + 1).ToString("00"), (GameFont)0, dragging ? Tokens.Accent : Tokens.Dim, (TextAnchor)3);
		Draw.Label(((HStack)(ref val)).Rest, items[index], (GameFont)1, Tokens.Text, (TextAnchor)3);
		if (dragging)
		{
			Draw.Label(new Rect(((Rect)(ref row)).xMax - 110f, ((Rect)(ref row)).y, 104f, ((Rect)(ref row)).height), "正在拖动", (GameFont)0, Tokens.Accent, (TextAnchor)5);
		}
	}

	private void OnReorder(int from, int to)
	{
		lastFrom = from;
		lastTo = to;
		int num = ReorderFmt.LandingIndex(from, to);
		ReorderFmt.Move<string>((IList<string>)items, from, to);
		lastMsg = "回调 Move(from=" + from + ", insert=" + to + ") → 落到下标 " + num + "\u3000（原版就给了 insert 语义，不做 -1 修正会整体偏一位）";
	}
}
internal sealed class BenchGallery : IBench
{
	private readonly GalleryExtra extra = new GalleryExtra();

	private float t;

	public bool Scrollable => true;

	public float Height(float width)
	{
		return 1240f;
	}

	public void Render(Rect area)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		t += Time.deltaTime;
		VStack val = Layout.Column(area, 16f);
		VStack val2 = Layout.Column(Atoms.SectionFrame(((VStack)(ref val)).Take(122f), "文本原子 TextAtoms"), 4f);
		HStack val3 = Layout.Row(((VStack)(ref val2)).Take(22f), 24f);
		TextAtoms.UnitLabel(((HStack)(ref val3)).Take(150f), "12 480", "件", (GameFont)1, (Color?)Tokens.Accent);
		TextAtoms.UnitLabel(((HStack)(ref val3)).Take(150f), "469", "W", (GameFont)1, (Color?)Tokens.Info);
		TextAtoms.UnitLabel(((HStack)(ref val3)).Take(150f), "86", "%", (GameFont)1, (Color?)Tokens.Alive);
		TextAtoms.SubtitleLine(((VStack)(ref val2)).Take(34f), "总工程师 · 部落工匠", "职称档位 T4 · 背景：部落猎手");
		TextAtoms.Typewriter(((VStack)(ref val2)).Take(26f), "第一块太阳能板接通的瞬间，电池指示灯亮了。", 26f, (GameFont)1, Tokens.Muted, (TextAnchor)3);
		VStack val4 = Layout.Column(Atoms.SectionFrame(((VStack)(ref val)).Take(140f), "状态原子 StatusAtoms"), 8f);
		HStack val5 = Layout.Row(((VStack)(ref val4)).Take(20f), 24f);
		StatusAtoms.PulseDot(((HStack)(ref val5)).Take(14f), Tokens.Alive, t, 0.45f);
		StatusAtoms.PulseDot(((HStack)(ref val5)).Take(14f), Tokens.Warn, t + 0.55f, 0.45f);
		StatusAtoms.PulseDot(((HStack)(ref val5)).Take(14f), Tokens.Dead, t + 1.1f, 0.45f);
		Draw.Label(((HStack)(ref val5)).Take(200f), "PulseDot ×3（错相位呼吸）", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		StatusAtoms.Spinner(((HStack)(ref val5)).Take(24f), t, (Color?)Tokens.Accent, 8, 8f, 3f);
		Draw.Label(((HStack)(ref val5)).Rest, "Spinner", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		HStack val6 = Layout.Row(((VStack)(ref val4)).Take(22f), 16f);
		StatusKind[] array = new StatusKind[5];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		StatusKind[] array2 = (StatusKind[])(object)array;
		for (int i = 0; i < array2.Length; i++)
		{
			Rect val7 = ((HStack)(ref val6)).Take(20f);
			StatusAtoms.Glyph(val7, array2[i]);
			Draw.Label(new Rect(((Rect)(ref val7)).x + 20f, ((Rect)(ref val7)).y, 44f, ((Rect)(ref val7)).height), ((object)(StatusKind)(ref array2[i])).ToString(), (GameFont)0, Tokens.Dim, (TextAnchor)3);
		}
		HStack val8 = Layout.Row(((VStack)(ref val4)).Take(20f), 24f);
		StatusAtoms.Caret(((HStack)(ref val8)).Take(18f), true, (Color?)null);
		Draw.Label(((HStack)(ref val8)).Take(80f), "Caret 展开", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		StatusAtoms.Caret(((HStack)(ref val8)).Take(18f), false, (Color?)null);
		Draw.Label(((HStack)(ref val8)).Take(80f), "Caret 折叠", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		Rect rest = ((HStack)(ref val8)).Rest;
		StatusAtoms.ScrollFadeHint(new Rect(((Rect)(ref rest)).x, ((Rect)(ref rest)).y, 120f, 20f), true);
		Draw.Label(new Rect(((Rect)(ref rest)).x + 122f, ((Rect)(ref rest)).y, 140f, 20f), "ScrollFadeHint", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		VStack val9 = Layout.Column(Atoms.SectionFrame(((VStack)(ref val)).Take(196f), "进度与容量 ProgressAtoms"), 8f);
		float num = Mathf.Repeat(t * 0.18f, 1.06f);
		ProgressAtoms.FillWipe(((VStack)(ref val9)).Take(42f), Mathf.Clamp01(num), (Color?)Tokens.Accent, 10, 0.35f);
		HStack val10 = Layout.Row(((VStack)(ref val9)).Take(18f), 16f);
		ProgressAtoms.CapacityBar(((HStack)(ref val10)).Take(170f), 780f, 1000f, (Color?)Tokens.Info);
		ProgressAtoms.CapacityBar(((HStack)(ref val10)).Take(170f), 1180f, 1000f, (Color?)Tokens.Dead);
		Draw.Label(((HStack)(ref val10)).Rest, "CapacityBar 正常 / 超载", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		HStack val11 = Layout.Row(((VStack)(ref val9)).Take(64f), 16f);
		float num2 = Mathf.Repeat(t * 0.22f, 1f);
		ProgressAtoms.ProgressRing(((HStack)(ref val11)).Take(60f), num2, (Color?)Tokens.BorderSoft, (Color?)Tokens.Alive, (string)null, 360f, 0.22f);
		ProgressAtoms.ProgressRing(((HStack)(ref val11)).Take(60f), 0.72f, (Color?)null, (Color?)Tokens.Accent, "72%", 360f, 0.22f);
		ProgressAtoms.ZoomIndicator(((HStack)(ref val11)).Rest, 0.86f, 1f);
		VStack val12 = Layout.Column(Atoms.SectionFrame(((VStack)(ref val)).Take(160f), "布局原子 Atoms / Layout"), 8f);
		HStack val13 = Layout.Row(((VStack)(ref val12)).Take(56f), 16f);
		Rect val14 = Atoms.AspectBox(((HStack)(ref val13)).Take(96f), 1.7777778f);
		Draw.Box(val14, Tokens.PanelRaised);
		Draw.Border(val14, Tokens.BorderSoft);
		Draw.Label(val14, "16:9", (GameFont)0, Tokens.Dim, (TextAnchor)4);
		Rect rest2 = ((HStack)(ref val13)).Rest;
		((Rect)(ref rest2))..ctor(((Rect)(ref rest2)).x, ((Rect)(ref rest2)).y, Mathf.Min(320f, ((Rect)(ref rest2)).width), ((Rect)(ref rest2)).height);
		Draw.Border(rest2, Tokens.BorderHair);
		Anchor9[] array3 = new Anchor9[4];
		RuntimeHelpers.InitializeArray(array3, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		Anchor9[] array4 = (Anchor9[])(object)array3;
		for (int j = 0; j < array4.Length; j++)
		{
			Rect val15 = Atoms.Anchor(rest2, array4[j], new Vector2(34f, 16f), 4f);
			Draw.Box(val15, Tokens.Accent);
			Draw.Label(val15, ((object)(Anchor9)(ref array4[j])).ToString().Substring(0, 2), (GameFont)0, Tokens.Window, (TextAnchor)4);
		}
		Draw.Label(new Rect(((Rect)(ref rest2)).x, ((Rect)(ref rest2)).yMax + 2f, ((Rect)(ref rest2)).width, 16f), "Atoms.Anchor 四角锚定", (GameFont)0, Tokens.Dim, (TextAnchor)3);
		HStack val16 = Layout.Row(((VStack)(ref val12)).Take(30f), 0f);
		Draw.Label(((HStack)(ref val16)).Rest, "Layout.Grid / Row / Column / Split / Inset 见各验证面布局本身（本台全部由它们搭出）", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		extra.DataSection(((VStack)(ref val)).Take(132f));
		extra.PanelGeoSection(((VStack)(ref val)).Take(168f));
		extra.NavMiscSection(((VStack)(ref val)).Take(150f));
	}
}
internal sealed class BenchPawnTable : IBench
{
	private static readonly string[] DefNames = new string[4] { "Assign", "Work", "Restrict", "Animals" };

	private readonly PawnTableHost host = new PawnTableHost();

	private string current = "Assign";

	public bool Scrollable => false;

	public float Height(float width)
	{
		return 340f;
	}

	public void Render(Rect area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		VStack val = Layout.Column(area, 12f);
		Rect val2 = ((VStack)(ref val)).Take(30f);
		HStack val3 = Layout.Row(val2, 6f);
		Draw.Label(((HStack)(ref val3)).Take(80f), "PawnTableDef", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		for (int i = 0; i < DefNames.Length; i++)
		{
			string text = DefNames[i];
			bool flag = text == current;
			if (Widgets.ButtonText(((HStack)(ref val3)).Take(88f), (flag ? "● " : "") + text, true, true, true, (TextAnchor?)null) && !flag)
			{
				current = text;
				host.Reset();
			}
		}
		if (Widgets.ButtonText(((HStack)(ref val3)).Take(60f), "刷新", true, true, true, (TextAnchor?)null))
		{
			host.MarkDirty();
		}
		Draw.EllipsisLabel(((HStack)(ref val3)).Rest, "列集合 / 表头三态排序 / 行高 / 内部滚动 —— 全部由原版 PawnTable 提供，RUIF 只做宿主与框。", (GameFont)0, Tokens.Dim, (TextAnchor)3);
		Rect rest = ((VStack)(ref val)).Rest;
		if (!(((Rect)(ref rest)).height < 80f))
		{
			PawnTableAtoms.PawnTableFrame(rest, host, current, (Func<IEnumerable<Pawn>>)null, true);
		}
	}
}
internal sealed class GalleryExtra
{
	private readonly string[] skills = new string[6] { "射击", "格斗", "建造", "采矿", "手工", "艺术" };

	private readonly float[] skillVals = new float[6] { 0.35f, 0.2f, 0.85f, 0.6f, 0.45f, 0.7f };

	private readonly string[] needs = new string[4] { "心情", "饮食", "休息", "娱乐" };

	private readonly float[] needVals = new float[4] { 0.62f, 0.78f, 0.41f, 0.88f };

	private int tabIdx = 1;

	private float tabScroll;

	private int page = 2;

	private readonly int[] pageSizes = new int[4] { 10, 25, 50, 100 };

	private int dot = 1;

	public const float DataH = 132f;

	public const float PanelGeoH = 168f;

	public const float NavMiscH = 150f;

	public void DataSection(Rect area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		Rect val = Atoms.SectionFrame(area, "数据展示 DataAtoms（技能条 / 需求面板）");
		HStack val2 = Layout.Row(val, 24f);
		Rect val3 = ((HStack)(ref val2)).Take((((Rect)(ref val)).width - 24f) * 0.55f);
		Rect rest = ((HStack)(ref val2)).Rest;
		Draw.Label(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width, 16f), "SkillBars（0–20 级色阶）", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		DataAtoms.SkillBars(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y + 18f, ((Rect)(ref val3)).width, ((Rect)(ref val3)).height - 18f), skills, skillVals, 20f, 12);
		Draw.Label(new Rect(((Rect)(ref rest)).x, ((Rect)(ref rest)).y, ((Rect)(ref rest)).width, 16f), "NeedsPanel（需求条）", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		DataAtoms.NeedsPanel(new Rect(((Rect)(ref rest)).x, ((Rect)(ref rest)).y + 18f, ((Rect)(ref rest)).width, ((Rect)(ref rest)).height - 18f), needs, needVals);
	}

	public void PanelGeoSection(Rect area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		Rect val = Atoms.SectionFrame(area, "面板与几何 PanelAtoms / Shapes");
		VStack val2 = Layout.Column(val, 6f);
		HStack val3 = Layout.Row(((VStack)(ref val2)).Take(20f), 16f);
		PanelAtoms.StatRow(((HStack)(ref val3)).Take(150f), "心情", "62%", (float?)0.62f, (Color?)Tokens.Accent);
		PanelAtoms.StatRow(((HStack)(ref val3)).Take(150f), "健康", "96%", (float?)0.96f, (Color?)Tokens.Alive);
		PanelAtoms.StatRow(((HStack)(ref val3)).Take(150f), "疲劳", "78%", (float?)0.78f, (Color?)Tokens.Warn);
		HStack val4 = Layout.Row(((VStack)(ref val2)).Take(52f), 16f);
		Rect val5 = ((HStack)(ref val4)).Take(170f);
		Shapes.CornerFrame(val5, Tokens.Accent, 14f, 2f);
		Draw.Label(val5, "CornerFrame", (GameFont)0, Tokens.Muted, (TextAnchor)4);
		Rect val6 = ((HStack)(ref val4)).Take(170f);
		Shapes.InsetPanel(val6);
		Shapes.Watermark(val6, "PC", 0.1f);
		Draw.Label(val6, "InsetPanel + Watermark", (GameFont)0, Tokens.Muted, (TextAnchor)4);
		Rect rest = ((HStack)(ref val4)).Rest;
		Shapes.RuleWithLabel(new Rect(((Rect)(ref rest)).x, ((Rect)(ref rest)).y, ((Rect)(ref rest)).width, 16f), "战斗履历", Tokens.Border, 1f);
		Draw.Label(new Rect(((Rect)(ref rest)).x, ((Rect)(ref rest)).y + 18f, ((Rect)(ref rest)).width, 16f), "RuleWithLabel", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		HStack val7 = Layout.Row(((VStack)(ref val2)).Take(20f), 16f);
		PanelAtoms.MiniProfileChip(((HStack)(ref val7)).Take(220f), "蓝 雪牛", "建造 12 · 出类拔萃", 0.72f, 0.94f);
		PanelAtoms.MiniProfileChip(((HStack)(ref val7)).Take(220f), "赤 眼镜蛇", "射击 15 · 登峰造极", 0.38f, 0.81f);
	}

	public void NavMiscSection(Rect area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		Rect val = Atoms.SectionFrame(area, "导航与杂项 NavAtoms / MiscAtoms");
		VStack val2 = Layout.Column(val, 7f);
		Rect val3 = ((VStack)(ref val2)).Take(28f);
		NavAtoms.TabBar(val3, new string[4] { "总览", "技能", "需求", "社交" }, ref tabIdx, ref tabScroll);
		HStack val4 = Layout.Row(((VStack)(ref val2)).Take(20f), 16f);
		NavAtoms.Breadcrumb(((HStack)(ref val4)).Take(280f), new string[4] { "殖民地", "迫降乡", "殖民者", "蓝 雪牛" }, (Action<int>)null);
		NavAtoms.AnchorDots(((HStack)(ref val4)).Take(120f), new string[4] { "一", "二", "三", "四" }, ref dot);
		HStack val5 = Layout.Row(((VStack)(ref val2)).Take(22f), 16f);
		NavAtoms.Pager(((HStack)(ref val5)).Take(240f), 12, page, ref page, pageSizes);
		MiscAtoms.PointCostBadge(((HStack)(ref val5)).Take(120f), 72, 200);
		Rect rest = ((VStack)(ref val2)).Rest;
		if (((Rect)(ref rest)).height > 24f)
		{
			MiscAtoms.BubbleTag(new Rect(((Rect)(ref rest)).x, ((Rect)(ref rest)).y, 260f, 24f), "宿醉（轻微）", (Color?)Tokens.Warn);
		}
	}
}
public class DemoButtonWorker : MainButtonWorker
{
	public override float ButtonBarPercent => 0f;

	public override void Activate()
	{
		DemoWindow demoWindow = Find.WindowStack.WindowOfType<DemoWindow>();
		if (demoWindow != null)
		{
			Find.WindowStack.TryRemove((Window)(object)demoWindow, false);
		}
		else
		{
			Find.WindowStack.Add((Window)(object)new DemoWindow());
		}
	}
}
public class DemoMod : Mod
{
	public DemoMod(ModContentPack content)
		: base(content)
	{
	}

	public override string SettingsCategory()
	{
		return "RUIF 组件验证台";
	}

	public override void DoSettingsWindowContents(Rect inRect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		VStack val = Layout.Column(inRect, 16f);
		Rect val2 = ((VStack)(ref val)).Take(46f);
		Draw.Label(val2, "RUIF · 组件验证台", (GameFont)2, Tokens.Text, (TextAnchor)3);
		Draw.Label(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).yMax - 14f, ((Rect)(ref val2)).width, 16f), "开发期实机验证工具（不随框架发布）", (GameFont)0, Tokens.Dim, (TextAnchor)3);
		Rect val3 = ((VStack)(ref val)).Take(96f);
		VStack val4 = Layout.Column(Atoms.SectionFrame(val3, "打开方式"), 3f);
		Draw.Label(((VStack)(ref val4)).Take(18f), "① 进游戏后：底栏「组件验证台」按钮，或按 F9", (GameFont)0, Tokens.Text, (TextAnchor)3);
		Draw.Label(((VStack)(ref val4)).Take(18f), "② 任意时刻（含标题界面）：本页下方按钮", (GameFont)0, Tokens.Text, (TextAnchor)3);
		Draw.Label(((VStack)(ref val4)).Take(18f), "底栏与 F9 由 UIRoot_Play 提供，标题界面是 UIRoot_Entry —— 那里不画底栏，属原版结构。", (GameFont)0, Tokens.Dim, (TextAnchor)3);
		Rect val5 = ((VStack)(ref val)).Take(34f);
		HStack val6 = Layout.Row(val5, 12f);
		if (Widgets.ButtonText(((HStack)(ref val6)).Take(180f), "打开组件验证台", true, true, true, (TextAnchor?)null))
		{
			DemoWindow demoWindow = Find.WindowStack.WindowOfType<DemoWindow>();
			if (demoWindow != null)
			{
				Find.WindowStack.TryRemove((Window)(object)demoWindow, false);
			}
			else
			{
				Find.WindowStack.Add((Window)(object)new DemoWindow());
			}
		}
		Draw.Label(((HStack)(ref val6)).Rest, "验证台窗口可在标题界面直接打开（WindowStack 在 Entry 场景同样可用）", (GameFont)0, Tokens.Muted, (TextAnchor)3);
		Rect val7 = ((VStack)(ref val)).Take(72f);
		Rect val8 = Atoms.SectionFrame(val7, "六个验证面");
		VStack val9 = Layout.Column(val8, 3f);
		Draw.EllipsisLabel(((VStack)(ref val9)).Take(18f), "组件画廊 · 贴图管线 · 拖拽重排 · 脉冲高亮 · 殖民者表 · 诊断", (GameFont)0, Tokens.Info, (TextAnchor)3);
		Draw.EllipsisLabel(((VStack)(ref val9)).Take(18f), "改动 RUIF 后重跑：dotnet build Source/RUIF.csproj && pwsh _scripts/sync_ruif.ps1", (GameFont)0, Tokens.Muted, (TextAnchor)3);
	}
}
public class DemoWindow : Window
{
	private static readonly string[] TabLabels = new string[1] { "42 原子" };

	private readonly List<TabRecord> tabs = new List<TabRecord>();

	private readonly BenchDiagnostics diag = new BenchDiagnostics();

	private readonly BenchTextures tex = new BenchTextures();

	private readonly BenchReorder reorder = new BenchReorder();

	private readonly BenchGallery gallery = new BenchGallery();

	private readonly BenchHighlight hilt = new BenchHighlight();

	private readonly BenchPawnTable pawnTable = new BenchPawnTable();

	private readonly BenchAtoms42 atoms42 = new BenchAtoms42();

	private int tab;

	private Vector2 scroll;

	private readonly HashSet<string> reported = new HashSet<string>();

	private string faulted;

	public override Vector2 InitialSize => new Vector2(1040f, 720f);

	public DemoWindow()
		: base((IWindowDrawing)null)
	{
		base.doCloseX = true;
		base.doCloseButton = false;
		base.resizeable = true;
		base.draggable = true;
		base.absorbInputAroundWindow = true;
		base.closeOnCancel = true;
		base.onlyOneOfTypeAllowed = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		HStack val = Layout.Row(inRect, 12f);
		Rect val2 = ((HStack)(ref val)).Take(((Rect)(ref inRect)).width - 160f);
		Draw.Label(val2, "RUIF · 组件验证台", (GameFont)2, Tokens.Text, (TextAnchor)3);
		Draw.Label(((HStack)(ref val)).Rest, "底栏按钮 / F9", (GameFont)0, Tokens.Dim, (TextAnchor)5);
		float num3;
		if (TabLabels.Length > 1)
		{
			Rect val3 = default(Rect);
			((Rect)(ref val3))..ctor(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y + 30f, ((Rect)(ref inRect)).width, 32f);
			Widgets.DrawMenuSection(val3);
			if (tabs.Count == 0)
			{
				for (int i = 0; i < TabLabels.Length; i++)
				{
					int idx = i;
					tabs.Add(new TabRecord(TabLabels[i], (Action)delegate
					{
						tab = idx;
					}, (Func<bool>)(() => tab == idx)));
				}
			}
			float num = ((Rect)(ref val3)).x + 2f;
			Text.Font = (GameFont)1;
			Rect val4 = default(Rect);
			for (int j = 0; j < tabs.Count; j++)
			{
				float num2 = Text.CalcSize(TabLabels[j]).x + 26f;
				((Rect)(ref val4))..ctor(num, ((Rect)(ref val3)).y, num2, ((Rect)(ref val3)).height);
				tabs[j].Draw(val4);
				if (Widgets.ButtonInvisible(val4, true) && tabs[j].clickedAction != null)
				{
					tabs[j].clickedAction();
				}
				num += num2;
			}
			num3 = ((Rect)(ref val3)).yMax + 10f;
		}
		else
		{
			num3 = ((Rect)(ref inRect)).y + 34f;
		}
		Rect val5 = default(Rect);
		((Rect)(ref val5))..ctor(((Rect)(ref inRect)).x, num3, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height - (num3 - ((Rect)(ref inRect)).y) - 4f);
		Draw.Box(val5, Tokens.Window);
		Draw.Border(val5, Tokens.BorderSoft);
		Rect val6 = Layout.Inset(val5, 12f, 12f);
		IBench bench = Current();
		float num4 = 0f;
		try
		{
			num4 = bench.Height(((Rect)(ref val6)).width);
		}
		catch (Exception e)
		{
			ReportOnce(bench, e);
		}
		if (bench.Scrollable)
		{
			Rect area = Layout.BeginScroll(val6, num4, ref scroll, true);
			try
			{
				if (((Rect)(ref area)).width > 0f)
				{
					bench.Render(area);
				}
			}
			catch (Exception e2)
			{
				ReportOnce(bench, e2);
			}
			finally
			{
				Layout.EndScroll();
			}
		}
		else
		{
			try
			{
				bench.Render(val6);
			}
			catch (Exception e3)
			{
				ReportOnce(bench, e3);
			}
		}
		Draw.Label(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).yMax - 2f, ((Rect)(ref inRect)).width, 18f), "内容高 " + Mathf.RoundToInt(num4) + "px · 滚动 " + Mathf.RoundToInt(scroll.y) + "px" + ((faulted == null) ? "" : ("\u3000⚠ " + faulted + " 已隔离（本次会话内不再重绘，详见日志）")), (GameFont)0, (faulted == null) ? Tokens.Dim : Tokens.Dead, (TextAnchor)6);
	}

	private void ReportOnce(IBench bench, Exception e)
	{
		string text = (faulted = bench.GetType().Name);
		if (reported.Add(text))
		{
			Log.ErrorOnce("[RUIF_Demo] 验证面 " + text + " 抛异常，已隔离（其余页签不受影响）：\n" + e, text.GetHashCode());
		}
	}

	private IBench Current()
	{
		return tab switch
		{
			1 => gallery, 
			2 => tex, 
			3 => reorder, 
			4 => hilt, 
			5 => pawnTable, 
			6 => diag, 
			_ => atoms42, 
		};
	}
}
public interface IBench
{
	bool Scrollable { get; }

	float Height(float width);

	void Render(Rect area);
}
