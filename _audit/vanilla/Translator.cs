using System;
using System.Collections.Generic;
using System.Text;

namespace Verse
{
	// Token: 0x020003A4 RID: 932
	public static class Translator
	{
		// Token: 0x060017DB RID: 6107 RVA: 0x0007946F File Offset: 0x0007766F
		public static bool CanTranslate(this string key)
		{
			return LanguageDatabase.activeLanguage.HaveTextForKey(key, false);
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x00079480 File Offset: 0x00077680
		public static TaggedString TranslateWithBackup(this string key, TaggedString backupKey)
		{
			TaggedString taggedString;
			if (key.TryTranslate(out taggedString))
			{
				return taggedString;
			}
			if (backupKey.TryTranslate(out taggedString))
			{
				return taggedString;
			}
			return key.Translate();
		}

		// Token: 0x060017DD RID: 6109 RVA: 0x000794B0 File Offset: 0x000776B0
		public static bool TryTranslate(this string key, out TaggedString result)
		{
			if (key.NullOrEmpty())
			{
				result = key;
				return false;
			}
			if (LanguageDatabase.activeLanguage == null)
			{
				Log.Error("No active language! Cannot translate from key " + key + ".");
				result = key;
				return true;
			}
			if (LanguageDatabase.activeLanguage.TryGetTextFromKey(key, out result))
			{
				return true;
			}
			result = key;
			return false;
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x0007951A File Offset: 0x0007771A
		public static string TranslateSimple(this string key)
		{
			return key.Translate();
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x00079528 File Offset: 0x00077728
		public static TaggedString Translate(this string key)
		{
			TaggedString taggedString;
			if (key.TryTranslate(out taggedString))
			{
				return taggedString;
			}
			LanguageDatabase.defaultLanguage.TryGetTextFromKey(key, out taggedString);
			if (Prefs.DevMode)
			{
				taggedString = Translator.PseudoTranslated(taggedString);
			}
			return taggedString;
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x00079568 File Offset: 0x00077768
		[Obsolete("Use TranslatorFormattedStringExtensions")]
		public static string Translate(this string key, params object[] args)
		{
			if (key.NullOrEmpty())
			{
				return key;
			}
			if (LanguageDatabase.activeLanguage == null)
			{
				Log.Error("No active language! Cannot translate from key " + key + ".");
				return key;
			}
			TaggedString taggedString;
			if (!LanguageDatabase.activeLanguage.TryGetTextFromKey(key, out taggedString))
			{
				LanguageDatabase.defaultLanguage.TryGetTextFromKey(key, out taggedString);
				if (Prefs.DevMode)
				{
					taggedString = Translator.PseudoTranslated(taggedString);
				}
			}
			string text = taggedString;
			try
			{
				text = string.Format(taggedString, args);
			}
			catch (Exception ex)
			{
				string text2 = "Exception translating '" + taggedString + "': ";
				Exception ex2 = ex;
				Log.ErrorOnce(text2 + ((ex2 != null) ? ex2.ToString() : null), Gen.HashCombineInt(key.GetHashCode(), 394878901));
			}
			return text;
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x00079640 File Offset: 0x00077840
		public static bool TryGetTranslatedStringsForFile(string fileName, out List<string> stringList)
		{
			if (!LanguageDatabase.activeLanguage.TryGetStringsFromFile(fileName, out stringList) && !LanguageDatabase.defaultLanguage.TryGetStringsFromFile(fileName, out stringList))
			{
				Log.Error("No string files for " + fileName + ".");
				return false;
			}
			return true;
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x00079678 File Offset: 0x00077878
		private static string PseudoTranslated(string original)
		{
			if (original == null)
			{
				return null;
			}
			if (!Prefs.DevMode)
			{
				return original;
			}
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in original)
			{
				if (c == '{')
				{
					flag = true;
					stringBuilder.Append(c);
				}
				else if (c == '}')
				{
					flag = false;
					stringBuilder.Append(c);
				}
				else if (!flag)
				{
					string text;
					switch (c)
					{
					case 'a':
						text = "à";
						break;
					case 'b':
						text = "þ";
						break;
					case 'c':
						text = "ç";
						break;
					case 'd':
						text = "ð";
						break;
					case 'e':
						text = "è";
						break;
					case 'f':
						text = "Ƒ";
						break;
					case 'g':
						text = "ğ";
						break;
					case 'h':
						text = "ĥ";
						break;
					case 'i':
						text = "ì";
						break;
					case 'j':
						text = "ĵ";
						break;
					case 'k':
						text = "к";
						break;
					case 'l':
						text = "ſ";
						break;
					case 'm':
						text = "ṁ";
						break;
					case 'n':
						text = "ƞ";
						break;
					case 'o':
						text = "ò";
						break;
					case 'p':
						text = "ṗ";
						break;
					case 'q':
						text = "q";
						break;
					case 'r':
						text = "ṟ";
						break;
					case 's':
						text = "ș";
						break;
					case 't':
						text = "ṭ";
						break;
					case 'u':
						text = "ù";
						break;
					case 'v':
						text = "ṽ";
						break;
					case 'w':
						text = "ẅ";
						break;
					case 'x':
						text = "ẋ";
						break;
					case 'y':
						text = "ý";
						break;
					case 'z':
						text = "ž";
						break;
					default:
						text = c.ToString() ?? "";
						break;
					}
					stringBuilder.Append(text);
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
