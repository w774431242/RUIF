using System;
using UnityEngine;
using Verse.Steam;

namespace Verse
{
	// Token: 0x02000BE3 RID: 3043
	public static class Text
	{
		// Token: 0x17000C0E RID: 3086
		// (get) Token: 0x06004DDB RID: 19931 RVA: 0x0018BCCE File Offset: 0x00189ECE
		public static bool TinyFontSupported
		{
			get
			{
				return (LongEventHandler.AnyEventNowOrWaiting || LanguageDatabase.activeLanguage.info.canBeTiny) && !Prefs.DisableTinyText && !SteamDeck.IsSteamDeck;
			}
		}

		// Token: 0x17000C0F RID: 3087
		// (get) Token: 0x06004DDC RID: 19932 RVA: 0x0018BCFD File Offset: 0x00189EFD
		// (set) Token: 0x06004DDD RID: 19933 RVA: 0x0018BD04 File Offset: 0x00189F04
		public static GameFont Font
		{
			get
			{
				return Text.fontInt;
			}
			set
			{
				if (value == GameFont.Tiny && !Text.TinyFontSupported)
				{
					Text.fontInt = GameFont.Small;
					return;
				}
				Text.fontInt = value;
			}
		}

		// Token: 0x17000C10 RID: 3088
		// (get) Token: 0x06004DDE RID: 19934 RVA: 0x0018BD1D File Offset: 0x00189F1D
		// (set) Token: 0x06004DDF RID: 19935 RVA: 0x0018BD24 File Offset: 0x00189F24
		public static TextAnchor Anchor
		{
			get
			{
				return Text.anchorInt;
			}
			set
			{
				Text.anchorInt = value;
			}
		}

		// Token: 0x17000C11 RID: 3089
		// (get) Token: 0x06004DE0 RID: 19936 RVA: 0x0018BD2C File Offset: 0x00189F2C
		// (set) Token: 0x06004DE1 RID: 19937 RVA: 0x0018BD33 File Offset: 0x00189F33
		public static bool WordWrap
		{
			get
			{
				return Text.wordWrapInt;
			}
			set
			{
				Text.wordWrapInt = value;
			}
		}

		// Token: 0x17000C12 RID: 3090
		// (get) Token: 0x06004DE2 RID: 19938 RVA: 0x0018BD3B File Offset: 0x00189F3B
		public static float LineHeight
		{
			get
			{
				return Text.lineHeights[(int)Text.Font];
			}
		}

		// Token: 0x17000C13 RID: 3091
		// (get) Token: 0x06004DE3 RID: 19939 RVA: 0x0018BD48 File Offset: 0x00189F48
		public static float SpaceBetweenLines
		{
			get
			{
				return Text.spaceBetweenLines[(int)Text.Font];
			}
		}

		// Token: 0x17000C14 RID: 3092
		// (get) Token: 0x06004DE4 RID: 19940 RVA: 0x0018BD58 File Offset: 0x00189F58
		public static GUIStyle CurFontStyle
		{
			get
			{
				GUIStyle guistyle;
				switch (Text.fontInt)
				{
				case GameFont.Tiny:
					guistyle = Text.fontStyles[0];
					break;
				case GameFont.Small:
					guistyle = Text.fontStyles[1];
					break;
				case GameFont.Medium:
					guistyle = Text.fontStyles[2];
					break;
				default:
					throw new NotImplementedException();
				}
				guistyle.alignment = Text.anchorInt;
				guistyle.wordWrap = Text.wordWrapInt;
				return guistyle;
			}
		}

		// Token: 0x17000C15 RID: 3093
		// (get) Token: 0x06004DE5 RID: 19941 RVA: 0x0018BDBC File Offset: 0x00189FBC
		public static GUIStyle CurTextFieldStyle
		{
			get
			{
				switch (Text.fontInt)
				{
				case GameFont.Tiny:
					return Text.textFieldStyles[0];
				case GameFont.Small:
					return Text.textFieldStyles[1];
				case GameFont.Medium:
					return Text.textFieldStyles[2];
				default:
					throw new NotImplementedException();
				}
			}
		}

		// Token: 0x17000C16 RID: 3094
		// (get) Token: 0x06004DE6 RID: 19942 RVA: 0x0018BE00 File Offset: 0x0018A000
		public static GUIStyle CurTextAreaStyle
		{
			get
			{
				switch (Text.fontInt)
				{
				case GameFont.Tiny:
					return Text.textAreaStyles[0];
				case GameFont.Small:
					return Text.textAreaStyles[1];
				case GameFont.Medium:
					return Text.textAreaStyles[2];
				default:
					throw new NotImplementedException();
				}
			}
		}

		// Token: 0x17000C17 RID: 3095
		// (get) Token: 0x06004DE7 RID: 19943 RVA: 0x0018BE44 File Offset: 0x0018A044
		public static GUIStyle CurTextAreaReadOnlyStyle
		{
			get
			{
				switch (Text.fontInt)
				{
				case GameFont.Tiny:
					return Text.textAreaReadOnlyStyles[0];
				case GameFont.Small:
					return Text.textAreaReadOnlyStyles[1];
				case GameFont.Medium:
					return Text.textAreaReadOnlyStyles[2];
				default:
					throw new NotImplementedException();
				}
			}
		}

		// Token: 0x06004DE8 RID: 19944 RVA: 0x0018BE88 File Offset: 0x0018A088
		static Text()
		{
			Text.fonts[0] = (Font)Resources.Load("Fonts/Calibri_tiny");
			Text.fonts[1] = (Font)Resources.Load("Fonts/Arial_small");
			Text.fonts[2] = (Font)Resources.Load("Fonts/Arial_medium");
			Text.fontStyles[0] = new GUIStyle(GUI.skin.label);
			Text.fontStyles[0].font = Text.fonts[0];
			Text.fontStyles[1] = new GUIStyle(GUI.skin.label);
			Text.fontStyles[1].font = Text.fonts[1];
			Text.fontStyles[1].contentOffset = new Vector2(0f, -1f);
			Text.fontStyles[2] = new GUIStyle(GUI.skin.label);
			Text.fontStyles[2].font = Text.fonts[2];
			for (int i = 0; i < Text.textFieldStyles.Length; i++)
			{
				Text.textFieldStyles[i] = new GUIStyle(GUI.skin.textField);
				Text.textFieldStyles[i].alignment = TextAnchor.MiddleLeft;
			}
			Text.textFieldStyles[0].font = Text.fonts[0];
			Text.textFieldStyles[1].font = Text.fonts[1];
			Text.textFieldStyles[2].font = Text.fonts[2];
			for (int j = 0; j < Text.textAreaStyles.Length; j++)
			{
				Text.textAreaStyles[j] = new GUIStyle(Text.textFieldStyles[j]);
				Text.textAreaStyles[j].alignment = TextAnchor.UpperLeft;
				Text.textAreaStyles[j].wordWrap = true;
			}
			for (int k = 0; k < Text.textAreaReadOnlyStyles.Length; k++)
			{
				Text.textAreaReadOnlyStyles[k] = new GUIStyle(Text.textAreaStyles[k]);
				GUIStyle guistyle = Text.textAreaReadOnlyStyles[k];
				guistyle.normal.background = null;
				guistyle.active.background = null;
				guistyle.onHover.background = null;
				guistyle.hover.background = null;
				guistyle.onFocused.background = null;
				guistyle.focused.background = null;
			}
			GUI.skin.settings.doubleClickSelectsWord = true;
			int num = 0;
			foreach (object obj in Enum.GetValues(typeof(GameFont)))
			{
				Text.Font = (GameFont)obj;
				Text.lineHeights[num] = Text.CalcHeight("W", 999f);
				Text.spaceBetweenLines[num] = Text.CalcHeight("W\nW", 999f) - Text.CalcHeight("W", 999f) * 2f;
				num++;
			}
			Text.Font = GameFont.Small;
		}

		// Token: 0x06004DE9 RID: 19945 RVA: 0x0018C1B0 File Offset: 0x0018A3B0
		public static float LineHeightOf(GameFont font)
		{
			return Text.lineHeights[(int)font];
		}

		// Token: 0x06004DEA RID: 19946 RVA: 0x0018C1B9 File Offset: 0x0018A3B9
		public static float CalcHeight(string text, float width)
		{
			Text.tmpTextGUIContent.text = text.StripTags();
			return Text.CurFontStyle.CalcHeight(Text.tmpTextGUIContent, width);
		}

		// Token: 0x06004DEB RID: 19947 RVA: 0x0018C1DB File Offset: 0x0018A3DB
		public static Vector2 CalcSize(string text)
		{
			Text.tmpTextGUIContent.text = text.StripTags();
			return Text.CurFontStyle.CalcSize(Text.tmpTextGUIContent);
		}

		// Token: 0x06004DEC RID: 19948 RVA: 0x0018C1FC File Offset: 0x0018A3FC
		public static string ClampTextWithEllipsis(Rect rect, string text)
		{
			if (text.Length <= 4)
			{
				return text;
			}
			Vector2 vector = Text.CalcSize(text);
			if (vector.x <= rect.width - 13f)
			{
				return text;
			}
			while (vector.x > rect.width - 13f)
			{
				if (text.Length == 0)
				{
					return "";
				}
				text = text.Substring(0, text.Length - 1);
				vector = Text.CalcSize(text + "...");
			}
			return text + "...";
		}

		// Token: 0x06004DED RID: 19949 RVA: 0x0018C284 File Offset: 0x0018A484
		public static void StartOfOnGUI()
		{
			if (!Text.WordWrap)
			{
				Log.ErrorOnce("Word wrap was false at end of frame.", 764362);
				Text.WordWrap = true;
			}
			if (Text.Anchor != TextAnchor.UpperLeft)
			{
				Log.ErrorOnce("Alignment was " + Text.Anchor.ToString() + " at end of frame.", 15558);
				Text.Anchor = TextAnchor.UpperLeft;
			}
			Text.Font = GameFont.Small;
		}

		// Token: 0x0400376E RID: 14190
		private static GameFont fontInt = GameFont.Small;

		// Token: 0x0400376F RID: 14191
		private static TextAnchor anchorInt = TextAnchor.UpperLeft;

		// Token: 0x04003770 RID: 14192
		private static bool wordWrapInt = true;

		// Token: 0x04003771 RID: 14193
		private static Font[] fonts = new Font[3];

		// Token: 0x04003772 RID: 14194
		public static readonly GUIStyle[] fontStyles = new GUIStyle[3];

		// Token: 0x04003773 RID: 14195
		public static readonly GUIStyle[] textFieldStyles = new GUIStyle[3];

		// Token: 0x04003774 RID: 14196
		public static readonly GUIStyle[] textAreaStyles = new GUIStyle[3];

		// Token: 0x04003775 RID: 14197
		public static readonly GUIStyle[] textAreaReadOnlyStyles = new GUIStyle[3];

		// Token: 0x04003776 RID: 14198
		private static readonly float[] lineHeights = new float[3];

		// Token: 0x04003777 RID: 14199
		private static readonly float[] spaceBetweenLines = new float[3];

		// Token: 0x04003778 RID: 14200
		private static GUIContent tmpTextGUIContent = new GUIContent();

		// Token: 0x04003779 RID: 14201
		private const int NumFonts = 3;

		// Token: 0x0400377A RID: 14202
		public const float SmallFontHeight = 22f;
	}
}
