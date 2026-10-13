using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using RimWorld;
using UnityEngine;
using Verse.Grammar;
using Verse.Steam;

namespace Verse
{
	// Token: 0x02000125 RID: 293
	public static class GenText
	{
		// Token: 0x060008C2 RID: 2242 RVA: 0x00029297 File Offset: 0x00027497
		public static string Possessive(this Pawn p)
		{
			if (p.gender == Gender.Female)
			{
				return "Proher".Translate();
			}
			return "Prohis".Translate();
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x000292C1 File Offset: 0x000274C1
		public static string PossessiveCap(this Pawn p)
		{
			if (p.gender == Gender.Female)
			{
				return "ProherCap".Translate();
			}
			return "ProhisCap".Translate();
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x000292EB File Offset: 0x000274EB
		public static string ProObj(this Pawn p)
		{
			if (p.gender == Gender.Female)
			{
				return "ProherObj".Translate();
			}
			return "ProhimObj".Translate();
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x00029315 File Offset: 0x00027515
		public static string ProObjCap(this Pawn p)
		{
			if (p.gender == Gender.Female)
			{
				return "ProherObjCap".Translate();
			}
			return "ProhimObjCap".Translate();
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x0002933F File Offset: 0x0002753F
		public static string ProSubj(this Pawn p)
		{
			if (p.gender == Gender.Female)
			{
				return "Proshe".Translate();
			}
			return "Prohe".Translate();
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00029369 File Offset: 0x00027569
		public static string ProSubjCap(this Pawn p)
		{
			if (p.gender == Gender.Female)
			{
				return "ProsheCap".Translate();
			}
			return "ProheCap".Translate();
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x00029394 File Offset: 0x00027594
		public static string AdjustedFor(this string text, Pawn p, string pawnSymbol = "PAWN", bool addRelationInfoSymbol = true)
		{
			GrammarRequest grammarRequest = default(GrammarRequest);
			grammarRequest.Includes.Add(RulePackDefOf.DynamicWrapper);
			grammarRequest.Rules.Add(new Rule_String("RULE", text));
			grammarRequest.Rules.AddRange(GrammarUtility.RulesForPawn(pawnSymbol, p, null, addRelationInfoSymbol, true));
			return GrammarResolver.Resolve("r_root", grammarRequest, null, false, null, null, null, true);
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x000293F8 File Offset: 0x000275F8
		public static string AdjustedForKeys(this string text, List<string> outErrors = null, bool resolveKeys = true)
		{
			if (outErrors != null)
			{
				outErrors.Clear();
			}
			if (text.NullOrEmpty())
			{
				return text;
			}
			int num = 0;
			for (;;)
			{
				num++;
				if (num > 500000)
				{
					break;
				}
				int num2 = text.IndexOf("{Key:");
				if (num2 < 0)
				{
					return text;
				}
				int num3 = num2;
				while (text[num3] != '}')
				{
					num3++;
					if (num3 >= text.Length)
					{
						goto Block_6;
					}
				}
				string text2 = text.Substring(num2 + 5, num3 - (num2 + 5));
				KeyBindingDef namedSilentFail = DefDatabase<KeyBindingDef>.GetNamedSilentFail(text2);
				string text3 = text.Substring(0, num2);
				if (namedSilentFail != null)
				{
					if (resolveKeys)
					{
						if (SteamDeck.IsSteamDeckInNonKeyboardMode)
						{
							text3 += SteamDeck.GetKeyBindingLabel(namedSilentFail);
						}
						else
						{
							text3 += namedSilentFail.MainKeyLabel;
						}
					}
					else
					{
						text3 += "placeholder";
					}
				}
				else
				{
					text3 += "error";
					if (outErrors != null)
					{
						string text4 = "Could not find key '" + text2 + "'";
						string text5 = BackCompatibility.BackCompatibleDefName(typeof(KeyBindingDef), text2, false, null);
						if (text5 != text2)
						{
							text4 = text4 + " (hint: it was renamed to '" + text5 + "')";
						}
						outErrors.Add(text4);
					}
				}
				text3 += text.Substring(num3 + 1);
				text = text3;
			}
			Log.Error("Too many iterations.");
			if (outErrors != null)
			{
				outErrors.Add("The parsed string caused an infinite loop");
				return text;
			}
			return text;
			Block_6:
			if (outErrors != null)
			{
				outErrors.Add("Mismatched braces");
			}
			return text;
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00029565 File Offset: 0x00027765
		public static string LabelIndefinite(this Pawn pawn)
		{
			if (pawn.Name != null && !pawn.Name.Numerical)
			{
				return pawn.LabelShort;
			}
			return pawn.KindLabelIndefinite();
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x00029589 File Offset: 0x00027789
		public static string LabelDefinite(this Pawn pawn)
		{
			if (pawn.Name != null && !pawn.Name.Numerical)
			{
				return pawn.LabelShort;
			}
			return pawn.KindLabelDefinite();
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x000295B0 File Offset: 0x000277B0
		public static string KindLabelIndefinite(this Pawn pawn)
		{
			string kindLabel = pawn.KindLabel;
			Gender gender = LanguageDatabase.activeLanguage.ResolveGender(kindLabel, null, pawn.gender);
			return Find.ActiveLanguageWorker.WithIndefiniteArticlePostProcessed(kindLabel, gender, false, false);
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x000295E8 File Offset: 0x000277E8
		public static string KindLabelDefinite(this Pawn pawn)
		{
			string kindLabel = pawn.KindLabel;
			Gender gender = LanguageDatabase.activeLanguage.ResolveGender(kindLabel, null, pawn.gender);
			return Find.ActiveLanguageWorker.WithDefiniteArticlePostProcessed(kindLabel, gender, false, false);
		}

		// Token: 0x060008CE RID: 2254 RVA: 0x00029620 File Offset: 0x00027820
		public static string RandomSeedString()
		{
			return GrammarResolver.Resolve("r_seed", new GrammarRequest
			{
				Includes = { RulePackDefOf.SeedGenerator }
			}, null, false, null, null, null, true).ToLower();
		}

		// Token: 0x060008CF RID: 2255 RVA: 0x0002965C File Offset: 0x0002785C
		public static string Shorten(this string s)
		{
			if (s.NullOrEmpty() || s.Length <= 4)
			{
				return s;
			}
			s = s.Trim();
			string[] array = s.Split(' ', StringSplitOptions.None);
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0)
				{
					text += " ";
				}
				if (array[i].Length > 2)
				{
					text = text + array[i].Substring(0, 1) + array[i].Substring(1, array[i].Length - 2).WithoutVowels() + array[i].Substring(array[i].Length - 1, 1);
				}
			}
			return text;
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x000296FC File Offset: 0x000278FC
		private static string WithoutVowels(this string s)
		{
			string vowels = "aeiouy";
			return new string(s.Where<char>((char c) => !vowels.Contains(c)).ToArray<char>());
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x00029738 File Offset: 0x00027938
		public static string MarchingEllipsis(float offset = 0f)
		{
			switch (Mathf.FloorToInt(Time.realtimeSinceStartup + offset) % 3)
			{
			case 0:
				return ".";
			case 1:
				return "..";
			case 2:
				return "...";
			default:
				throw new Exception();
			}
		}

		// Token: 0x060008D2 RID: 2258 RVA: 0x0002977F File Offset: 0x0002797F
		public static void SetTextSizeToFit(string text, Rect r)
		{
			Text.Font = GameFont.Small;
			if (Text.CalcHeight(text, r.width) > r.height)
			{
				Text.Font = GameFont.Tiny;
			}
		}

		// Token: 0x060008D3 RID: 2259 RVA: 0x000297A3 File Offset: 0x000279A3
		public static string TrimEndNewlines(this string s)
		{
			return s.TrimEnd(new char[] { '\r', '\n' });
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x000297BB File Offset: 0x000279BB
		public static string Indented(this string s, string indentation = "    ")
		{
			if (s.NullOrEmpty())
			{
				return s;
			}
			return indentation + s.Replace("\r", "").Replace("\n", "\n" + indentation);
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x000297F4 File Offset: 0x000279F4
		public static string ReplaceFirst(this string source, string key, string replacement)
		{
			int num = source.IndexOf(key);
			if (num < 0)
			{
				return source;
			}
			return source.Substring(0, num) + replacement + source.Substring(num + key.Length);
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x0002982C File Offset: 0x00027A2C
		public static int StableStringHash(string str)
		{
			if (str == null)
			{
				return 0;
			}
			int num = 23;
			int length = str.Length;
			for (int i = 0; i < length; i++)
			{
				num = num * 31 + (int)str[i];
			}
			return num;
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x00029864 File Offset: 0x00027A64
		public static string StringFromEnumerable(IEnumerable source)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in source)
			{
				stringBuilder.AppendLine("• " + obj.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x000298D0 File Offset: 0x00027AD0
		public static IEnumerable<string> LinesFromString(string text)
		{
			string[] array = new string[] { "\r\n", "\n" };
			string[] array2 = text.Split(array, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array2.Length; i++)
			{
				string text2 = array2[i].Trim();
				if (!text2.StartsWith("//"))
				{
					text2 = text2.Split(new string[] { "//" }, StringSplitOptions.None)[0];
					if (text2.Length != 0)
					{
						yield return text2;
					}
				}
			}
			array2 = null;
			yield break;
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x000298E0 File Offset: 0x00027AE0
		public static string GetInvalidFilenameCharacters()
		{
			return new string(Path.GetInvalidFileNameChars()) + "/\\{}<>:*|!@#$%^&*?";
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x000298F6 File Offset: 0x00027AF6
		public static bool IsValidFilename(string str)
		{
			return str.Length <= 40 && !new Regex("[" + Regex.Escape(GenText.GetInvalidFilenameCharacters()) + "]").IsMatch(str);
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x0002992B File Offset: 0x00027B2B
		public static string SanitizeFilename(string str)
		{
			return string.Join("_", str.Split(GenText.GetInvalidFilenameCharacters().ToArray<char>(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x0002994F File Offset: 0x00027B4F
		public static bool NullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x00029957 File Offset: 0x00027B57
		public static string SplitCamelCase(string Str)
		{
			return Regex.Replace(Str, "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1");
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x0002996C File Offset: 0x00027B6C
		public static string CapitalizedNoSpaces(string s)
		{
			string[] array = s.Split(' ', StringSplitOptions.None);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in array)
			{
				if (text.Length > 0)
				{
					stringBuilder.Append(char.ToUpper(text[0]));
				}
				if (text.Length > 1)
				{
					stringBuilder.Append(text.Substring(1));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x000299D4 File Offset: 0x00027BD4
		public static string RemoveNonAlphanumeric(string s)
		{
			GenText.tmpSb.Length = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (char.IsLetterOrDigit(s[i]))
				{
					GenText.tmpSb.Append(s[i]);
				}
			}
			return GenText.tmpSb.ToString();
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x00029A27 File Offset: 0x00027C27
		public static bool EqualsIgnoreCase(this string A, string B)
		{
			return string.Compare(A, B, true) == 0;
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x00029A34 File Offset: 0x00027C34
		public static string WithoutByteOrderMark(this string str)
		{
			return str.Trim().Trim(new char[] { '\ufeff' });
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x00029A50 File Offset: 0x00027C50
		public static bool NamesOverlap(string A, string B)
		{
			A = A.ToLower();
			B = B.ToLower();
			string[] array = A.Split(' ', StringSplitOptions.None);
			string[] array2 = B.Split(' ', StringSplitOptions.None);
			foreach (string text in array)
			{
				if (TitleCaseHelper.IsUppercaseTitleWord(text) && array2.Contains(text))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x00029AA8 File Offset: 0x00027CA8
		public static int FirstLetterBetweenTags(this string str)
		{
			int num = 0;
			if (str[num] == '<' && str.IndexOf('>') > num && num < str.Length - 1 && str[num + 1] != '/')
			{
				num = str.IndexOf('>') + 1;
			}
			return num;
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x00029AF4 File Offset: 0x00027CF4
		public static string CapitalizeFirst(this string str)
		{
			if (str.NullOrEmpty())
			{
				return str;
			}
			if (char.IsUpper(str[0]))
			{
				return str;
			}
			if (str.Length == 1)
			{
				return str.ToUpper();
			}
			int num = str.FirstLetterBetweenTags();
			if (num == 0)
			{
				return char.ToUpper(str[num]).ToString() + str.Substring(num + 1);
			}
			return str.Substring(0, num) + char.ToUpper(str[num]).ToString() + str.Substring(num + 1);
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x00029B82 File Offset: 0x00027D82
		public static string EndWithPeriod(this string str)
		{
			if (str.NullOrEmpty())
			{
				return str;
			}
			if (str[str.Length - 1] == '.')
			{
				return str;
			}
			return str + ".";
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x00029BAD File Offset: 0x00027DAD
		public static string CapitalizeFirst(this string str, Def possibleDef)
		{
			if (possibleDef != null && str == possibleDef.label)
			{
				return possibleDef.LabelCap;
			}
			return str.CapitalizeFirst();
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x00029BD4 File Offset: 0x00027DD4
		public static string UncapitalizeFirst(this string str)
		{
			if (str.NullOrEmpty())
			{
				return str;
			}
			if (char.IsLower(str[0]))
			{
				return str;
			}
			if (str.Length == 1)
			{
				return str.ToLower();
			}
			int num = str.FirstLetterBetweenTags();
			if (num == 0)
			{
				return char.ToLower(str[num]).ToString() + str.Substring(num + 1);
			}
			return str.Substring(0, num) + char.ToLower(str[num]).ToString() + str.Substring(num + 1);
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x00029C64 File Offset: 0x00027E64
		public static string ToNewsCase(string str)
		{
			string[] array = str.Split(' ', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.Length >= 2)
				{
					if (i == 0)
					{
						array[i] = text[0].ToString().ToUpper() + text.Substring(1);
					}
					else
					{
						array[i] = text.ToLower();
					}
				}
			}
			return string.Join(" ", array);
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x00029CD2 File Offset: 0x00027ED2
		public static string ToTitleCaseSmart(string str)
		{
			if (str.NullOrEmpty())
			{
				return str;
			}
			return GenText.CapitalizeFirstLetterAfterSeparator(GenText.CapitalizeFirstLetterAfterSeparator(GenText.CapitalizeFirstLetterAfterSeparator(str.MergeMultipleSpaces(false).Trim(), " "), "-"), " '");
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x00029D08 File Offset: 0x00027F08
		private static string CapitalizeFirstLetterAfterSeparator(string str, string separator)
		{
			GenText.tmpSeparators[0] = separator;
			string[] array = str.Split(GenText.tmpSeparators, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if ((i == 0 || i == array.Length - 1 || TitleCaseHelper.IsUppercaseTitleWord(text)) && !text.NullOrEmpty())
				{
					int num = text.FirstLetterBetweenTags();
					string text2 = ((num == 0) ? text[num].ToString().ToUpper() : (text.Substring(0, num) + char.ToUpper(text[num]).ToString()));
					string text3 = text.Substring(num + 1);
					array[i] = text2 + text3;
				}
			}
			return string.Join(separator, array);
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x00029DC0 File Offset: 0x00027FC0
		public static string CapitalizeSentences(string input, bool capitalizeFirstSentence = true)
		{
			if (input.NullOrEmpty())
			{
				return input;
			}
			if (input.Length != 1)
			{
				bool flag = capitalizeFirstSentence;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				GenText.tmpSbForCapitalizedSentences.Length = 0;
				for (int i = 0; i < input.Length; i++)
				{
					if (flag && char.IsLetterOrDigit(input[i]) && !flag2 && !flag3 && !flag4)
					{
						GenText.tmpSbForCapitalizedSentences.Append(char.ToUpper(input[i]));
						flag = false;
					}
					else
					{
						GenText.tmpSbForCapitalizedSentences.Append(input[i]);
					}
					if (input[i] == '\r' || input[i] == '\n' || (input[i] == '.' && i < input.Length - 1 && input[i + 1] == ' ') || input[i] == '!' || input[i] == '?' || input[i] == ':')
					{
						flag = true;
					}
					else if (input[i] == '<' && i < input.Length - 1 && input[i + 1] != '/')
					{
						flag2 = true;
					}
					else if (flag2 && input[i] == '>')
					{
						flag2 = false;
					}
					else if (input[i] == '(' && i < input.Length - 1 && input[i + 1] == '*')
					{
						flag4 = true;
					}
					else if (flag4 && input[i] == ')')
					{
						flag4 = false;
					}
					else if (input[i] == '{')
					{
						flag3 = true;
						flag = false;
					}
					else if (flag3 && input[i] == '}')
					{
						flag3 = false;
						flag = false;
					}
				}
				return GenText.tmpSbForCapitalizedSentences.ToString();
			}
			if (capitalizeFirstSentence)
			{
				return input.ToUpper();
			}
			return input;
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x00029F76 File Offset: 0x00028176
		public static string CapitalizeAsTitle(string str)
		{
			return Find.ActiveLanguageWorker.ToTitleCase(str);
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x00029F84 File Offset: 0x00028184
		public static string ToCommaList(this IEnumerable<string> items, bool useAnd = false, bool emptyIfNone = false)
		{
			if (items == null)
			{
				return "";
			}
			string text;
			string text2;
			int num;
			StringBuilder stringBuilder = GenText.ToCommaListInner(items, out text, out text2, out num);
			if (num == 0)
			{
				return emptyIfNone ? "" : "NoneLower".Translate();
			}
			if (num == 1)
			{
				return text2;
			}
			if (!useAnd)
			{
				stringBuilder.Append(text2);
				return stringBuilder.ToString();
			}
			if (num == 2)
			{
				return "ToCommaListAnd".Translate(text, text2).Resolve();
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			return "ToCommaListAnd".Translate(stringBuilder.ToString(), text2).Resolve();
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x0002A03C File Offset: 0x0002823C
		public static string ToCommaListOr(this IEnumerable<string> items, bool emptyIfNone = false)
		{
			if (items == null)
			{
				return "";
			}
			string text;
			string text2;
			int num;
			StringBuilder stringBuilder = GenText.ToCommaListInner(items, out text, out text2, out num);
			if (num == 0)
			{
				return emptyIfNone ? "" : "NoneLower".Translate();
			}
			if (num == 1)
			{
				return text2;
			}
			if (num == 2)
			{
				return "ToCommaListOr".Translate(text, text2).Resolve();
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			return "ToCommaListOr".Translate(stringBuilder.ToString(), text2).Resolve();
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x0002A0E0 File Offset: 0x000282E0
		private static StringBuilder ToCommaListInner(IEnumerable<string> items, out string first, out string last, out int count)
		{
			first = null;
			last = null;
			count = 0;
			StringBuilder stringBuilder = new StringBuilder();
			IList<string> list = items as IList<string>;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					string text = list[i];
					if (!text.NullOrEmpty())
					{
						if (first == null)
						{
							first = text;
						}
						if (last != null)
						{
							stringBuilder.Append(last + ", ");
						}
						last = text;
						count++;
					}
				}
			}
			else
			{
				foreach (string text2 in items)
				{
					if (!text2.NullOrEmpty())
					{
						if (first == null)
						{
							first = text2;
						}
						if (last != null)
						{
							stringBuilder.Append(last + ", ");
						}
						last = text2;
						count++;
					}
				}
			}
			return stringBuilder;
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x0002A1C0 File Offset: 0x000283C0
		public static TaggedString ToClauseSequence(this List<string> entries)
		{
			switch (entries.Count)
			{
			case 0:
				return "None".Translate() + ".";
			case 1:
				return entries[0] + ".";
			case 2:
				return "ClauseSequence2".Translate(entries[0], entries[1]);
			case 3:
				return "ClauseSequence3".Translate(entries[0], entries[1], entries[2]);
			case 4:
				return "ClauseSequence4".Translate(entries[0], entries[1], entries[2], entries[3]);
			case 5:
				return "ClauseSequence5".Translate(entries[0], entries[1], entries[2], entries[3], entries[4]);
			case 6:
				return "ClauseSequence6".Translate(entries[0], entries[1], entries[2], entries[3], entries[4], entries[5]);
			case 7:
				return "ClauseSequence7".Translate(entries[0], entries[1], entries[2], entries[3], entries[4], entries[5], entries[6]);
			case 8:
				return "ClauseSequence8".Translate(entries[0], entries[1], entries[2], entries[3], entries[4], entries[5], entries[6], entries[7]);
			default:
				return entries.ToCommaList(true, false);
			}
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x0002A430 File Offset: 0x00028630
		public static string ToLineList(this IList<string> entries, string prefix = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < entries.Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append("\n");
				}
				if (prefix != null)
				{
					stringBuilder.Append(prefix);
				}
				stringBuilder.Append(entries[i]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x0002A484 File Offset: 0x00028684
		public static string ToLineList(this IList<string> entries, Color color, string prefix = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < entries.Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append("\n");
				}
				if (prefix != null)
				{
					stringBuilder.Append(prefix);
				}
				stringBuilder.Append(entries[i].Colorize(color));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x0002A4DC File Offset: 0x000286DC
		public static string ToLineList(this IEnumerable<string> entries, string prefix = null, bool capitalizeItems = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string text in entries)
			{
				if (!flag)
				{
					stringBuilder.Append("\n");
				}
				if (prefix != null)
				{
					stringBuilder.Append(prefix);
				}
				stringBuilder.Append(capitalizeItems ? text.CapitalizeFirst() : text);
				flag = false;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x0002A55C File Offset: 0x0002875C
		public static string ToSpaceList(IEnumerable<string> entries)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string text in entries)
			{
				if (!flag)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(text);
				flag = false;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x0002A5C4 File Offset: 0x000287C4
		public static string ToStringList(this IEnumerable<string> items, string seperator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			string text2 = null;
			IList<string> list = items as IList<string>;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					string text3 = list[i];
					if (!text3.NullOrEmpty())
					{
						if (text == null)
						{
							text = text3;
						}
						if (text2 != null)
						{
							stringBuilder.Append(text2 + seperator);
						}
						text2 = text3;
					}
				}
			}
			else
			{
				foreach (string text4 in items)
				{
					if (text != null)
					{
						stringBuilder.Append(seperator);
					}
					stringBuilder.Append(text4);
					if (text == null)
					{
						text = text4;
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x0002A684 File Offset: 0x00028884
		public static string ToCamelCase(string str)
		{
			string text = "";
			foreach (string text2 in str.Split(' ', StringSplitOptions.None))
			{
				if (text.Length == 0)
				{
					text += text2.UncapitalizeFirst();
				}
				else
				{
					text += text2.CapitalizeFirst();
				}
			}
			return text;
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x0002A6D8 File Offset: 0x000288D8
		public static string Truncate(this string str, float width, Dictionary<string, string> cache = null)
		{
			string text;
			if (cache != null && cache.TryGetValue(str, out text))
			{
				return text;
			}
			if (Text.CalcSize(str).x <= width)
			{
				if (cache != null)
				{
					cache.Add(str, str);
				}
				return str;
			}
			text = str;
			do
			{
				text = text.Substring(0, text.Length - 1);
			}
			while (text.Length > 0 && Text.CalcSize(text + "...").x > width);
			text += "...";
			if (cache != null)
			{
				cache.Add(str, text);
			}
			return text;
		}

		// Token: 0x060008F8 RID: 2296 RVA: 0x0002A75C File Offset: 0x0002895C
		public static TaggedString Truncate(this TaggedString str, float width, Dictionary<string, TaggedString> cache = null)
		{
			TaggedString taggedString;
			if (cache != null && cache.TryGetValue(str.RawText, out taggedString))
			{
				return taggedString;
			}
			if (Text.CalcSize(str.RawText.StripTags()).x < width)
			{
				if (cache != null)
				{
					cache.Add(str.RawText, str);
				}
				return str;
			}
			taggedString = str;
			do
			{
				taggedString = taggedString.RawText.Substring(0, taggedString.RawText.Length - 1);
			}
			while (taggedString.RawText.StripTags().Length > 0 && Text.CalcSize(GenText.AddEllipses(taggedString.RawText.StripTags())).x > width);
			taggedString = GenText.AddEllipses(taggedString);
			if (cache != null)
			{
				cache.Add(str.RawText, str);
			}
			return taggedString;
		}

		// Token: 0x060008F9 RID: 2297 RVA: 0x0002A824 File Offset: 0x00028A24
		public static string TruncateHeight(this string str, float width, float height, Dictionary<string, string> cache = null)
		{
			string text;
			if (cache != null && cache.TryGetValue(str, out text))
			{
				return text;
			}
			if (Text.CalcHeight(str, width) <= height)
			{
				if (cache != null)
				{
					cache.Add(str, str);
				}
				return str;
			}
			text = str;
			do
			{
				text = text.Substring(0, text.Length - 1);
			}
			while (text.Length > 0 && Text.CalcHeight(GenText.AddEllipses(text), width) > height);
			text = GenText.AddEllipses(text);
			if (cache != null)
			{
				cache.Add(str, text);
			}
			return text;
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x0002A898 File Offset: 0x00028A98
		public static TaggedString TruncateHeight(this TaggedString str, float width, float height, Dictionary<string, TaggedString> cache = null)
		{
			TaggedString taggedString;
			if (cache != null && cache.TryGetValue(str.RawText, out taggedString))
			{
				return taggedString;
			}
			if (Text.CalcHeight(str.RawText.StripTags(), width) <= height)
			{
				if (cache != null)
				{
					cache.Add(str.RawText, str);
				}
				return str;
			}
			taggedString = str;
			do
			{
				taggedString = taggedString.RawText.Substring(0, taggedString.RawText.Length - 1);
			}
			while (taggedString.RawText.StripTags().Length > 0 && Text.CalcHeight(GenText.AddEllipses(taggedString.RawText.StripTags()), width) > height);
			taggedString = GenText.AddEllipses(taggedString);
			if (cache != null)
			{
				cache.Add(str.RawText, taggedString);
			}
			return taggedString;
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x0002A957 File Offset: 0x00028B57
		public static string AddEllipses(string s)
		{
			if (s.Length > 0 && s[s.Length - 1] == '.')
			{
				return s + " ...";
			}
			return s + "...";
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x0002A98C File Offset: 0x00028B8C
		public static string Flatten(this string str)
		{
			if (str.NullOrEmpty())
			{
				return str;
			}
			if (str.Contains("\n"))
			{
				str = str.Replace("\n", " ");
			}
			if (str.Contains("\r"))
			{
				str = str.Replace("\r", "");
			}
			str = str.MergeMultipleSpaces(false);
			return str.Trim(new char[] { ' ', '\n', '\r', '\t' });
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x0002AA04 File Offset: 0x00028C04
		public static string MergeMultipleSpaces(this string str, bool leaveMultipleSpacesAtLineBeginning = true)
		{
			if (str.NullOrEmpty())
			{
				return str;
			}
			if (!str.Contains("  "))
			{
				return str;
			}
			bool flag = true;
			GenText.tmpStringBuilder.Length = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == '\r' || str[i] == '\n')
				{
					flag = true;
				}
				if ((leaveMultipleSpacesAtLineBeginning && flag) || str[i] != ' ' || i == 0 || str[i - 1] != ' ')
				{
					GenText.tmpStringBuilder.Append(str[i]);
				}
				if (!char.IsWhiteSpace(str[i]))
				{
					flag = false;
				}
			}
			return GenText.tmpStringBuilder.ToString();
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x0002AAAC File Offset: 0x00028CAC
		public static string TrimmedToLength(this string str, int length)
		{
			if (str == null || str.Length <= length)
			{
				return str;
			}
			return str.Substring(0, length);
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x0002AAC4 File Offset: 0x00028CC4
		public static bool ContainsEmptyLines(string str)
		{
			return str.NullOrEmpty() || (str[0] == '\n' || str[0] == '\r') || (str[str.Length - 1] == '\n' || str[str.Length - 1] == '\r') || (str.Contains("\n\n") || str.Contains("\r\n\r\n") || str.Contains("\r\r"));
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x0002AB44 File Offset: 0x00028D44
		public static string ToStringByStyle(this float f, ToStringStyle style, ToStringNumberSense numberSense = ToStringNumberSense.Absolute)
		{
			if (style == ToStringStyle.Temperature && numberSense == ToStringNumberSense.Offset)
			{
				style = ToStringStyle.TemperatureOffset;
			}
			if (numberSense == ToStringNumberSense.Factor)
			{
				if (f >= 10f)
				{
					style = ToStringStyle.FloatMaxTwo;
				}
				else
				{
					style = ToStringStyle.PercentZero;
				}
			}
			string text;
			switch (style)
			{
			case ToStringStyle.Integer:
				text = Mathf.RoundToInt(f).ToString();
				break;
			case ToStringStyle.FloatOne:
				text = f.ToString("F1");
				break;
			case ToStringStyle.FloatTwo:
				text = f.ToString("F2");
				break;
			case ToStringStyle.FloatThree:
				text = f.ToString("F3");
				break;
			case ToStringStyle.FloatMaxOne:
				text = f.ToString("0.#");
				break;
			case ToStringStyle.FloatMaxTwo:
				text = f.ToString("0.##");
				break;
			case ToStringStyle.FloatMaxThree:
				text = f.ToString("0.###");
				break;
			case ToStringStyle.FloatTwoOrThree:
				text = f.ToString((f == 0f || Mathf.Abs(f) >= 0.01f) ? "F2" : "F3");
				break;
			case ToStringStyle.PercentZero:
				text = f.ToStringPercent();
				break;
			case ToStringStyle.PercentOne:
				text = f.ToStringPercent("F1");
				break;
			case ToStringStyle.PercentTwo:
				text = f.ToStringPercent("F2");
				break;
			case ToStringStyle.Temperature:
				text = f.ToStringTemperature("F1");
				break;
			case ToStringStyle.TemperatureOffset:
				text = f.ToStringTemperatureOffset("F1");
				break;
			case ToStringStyle.WorkAmount:
				text = f.ToStringWorkAmount();
				break;
			case ToStringStyle.Money:
				text = f.ToStringMoney(null);
				break;
			default:
				Log.Error("Unknown ToStringStyle " + style.ToString());
				text = f.ToString();
				break;
			}
			if (numberSense == ToStringNumberSense.Offset)
			{
				if (f >= 0f)
				{
					text = "+" + text;
				}
			}
			else if (numberSense == ToStringNumberSense.Factor)
			{
				text = "x" + text;
			}
			return text;
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x0002AD08 File Offset: 0x00028F08
		public static string ToStringDecimalIfSmall(this float f)
		{
			if (Mathf.Abs(f) < 1f)
			{
				return Math.Round((double)f, 2).ToString("0.##");
			}
			if (Mathf.Abs(f) < 10f)
			{
				return Math.Round((double)f, 1).ToString("0.#");
			}
			return Mathf.RoundToInt(f).ToStringCached();
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x0002AD66 File Offset: 0x00028F66
		public static string ToStringPercent(this float f)
		{
			return (f * 100f).ToStringDecimalIfSmall() + "%";
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x0002AD80 File Offset: 0x00028F80
		public static string ToStringPercent(this float f, string format)
		{
			return ((f + 1E-05f) * 100f).ToString(format) + "%";
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x0002ADB0 File Offset: 0x00028FB0
		public static string ToStringPercentSigned(this float f)
		{
			return ((Mathf.Sign(f) > 0f) ? "+" : ((Mathf.Sign(f) < 0f) ? "-" : "")) + (Mathf.Abs(f) * 100f).ToStringDecimalIfSmall() + "%";
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0002AE08 File Offset: 0x00029008
		public static string ToStringPercentSigned(this float f, string format)
		{
			return ((Mathf.Sign(f) > 0f) ? "+" : ((Mathf.Sign(f) < 0f) ? "-" : "")) + ((Mathf.Abs(f) + 1E-05f) * 100f).ToString(format) + "%";
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0002AE68 File Offset: 0x00029068
		public static string ToStringMoney(this float f, string format = null)
		{
			if (format == null)
			{
				if (f >= 10f || f == 0f)
				{
					format = "F0";
				}
				else
				{
					format = "F2";
				}
			}
			return "MoneyFormat".Translate(f.ToString(format));
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0002AEB4 File Offset: 0x000290B4
		public static string ToStringMoneyOffset(this float f, string format = null)
		{
			string text = Mathf.Abs(f).ToStringMoney(format);
			if (f > 0f && text != "$0")
			{
				return "+" + text;
			}
			if (f < 0f)
			{
				return "-" + text;
			}
			return text;
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x0002AF04 File Offset: 0x00029104
		public static string ToStringWithSign(this int i)
		{
			return i.ToString("+#;-#;0");
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x0002AF12 File Offset: 0x00029112
		public static string ToStringWithSign(this float f, string format = "0.##")
		{
			if (f > 0f)
			{
				return "+" + f.ToString(format);
			}
			return f.ToString(format);
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x0002AF38 File Offset: 0x00029138
		public static string ToStringKilobytes(this int bytes, string format = "F2")
		{
			return ((float)bytes / 1024f).ToString(format) + "Kb";
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x0002AF60 File Offset: 0x00029160
		public static string ToStringYesNo(this bool b)
		{
			return b ? "Yes".Translate() : "No".Translate();
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x0002AF80 File Offset: 0x00029180
		public static string ToStringLongitude(this float longitude)
		{
			bool flag = longitude < 0f;
			if (flag)
			{
				longitude = -longitude;
			}
			return longitude.ToString("F2") + "°" + (flag ? "W" : "E");
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x0002AFC4 File Offset: 0x000291C4
		public static string ToStringLatitude(this float latitude)
		{
			bool flag = latitude < 0f;
			if (flag)
			{
				latitude = -latitude;
			}
			return latitude.ToString("F2") + "°" + (flag ? "S" : "N");
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0002B008 File Offset: 0x00029208
		public static string ToStringMass(this float mass)
		{
			if (mass == 0f)
			{
				return "0 g";
			}
			float num = Mathf.Abs(mass);
			if (num >= 100f)
			{
				return mass.ToString("F0") + " kg";
			}
			if (num >= 10f)
			{
				return mass.ToString("0.#") + " kg";
			}
			if (num >= 0.1f)
			{
				return mass.ToString("0.##") + " kg";
			}
			float num2 = mass * 1000f;
			if (num >= 0.01f)
			{
				return num2.ToString("F0") + " g";
			}
			if (num >= 0.001f)
			{
				return num2.ToString("0.#") + " g";
			}
			return num2.ToString("0.##") + " g";
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x0002B0E4 File Offset: 0x000292E4
		public static string ToStringMassOffset(this float mass)
		{
			string text = mass.ToStringMass();
			if (mass > 0f)
			{
				return "+" + text;
			}
			return text;
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x0002B10D File Offset: 0x0002930D
		public static string ToStringSign(this float val)
		{
			if (val >= 0f)
			{
				return "+";
			}
			return "";
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x0002B124 File Offset: 0x00029324
		public static string ToStringEnsureThreshold(this float value, float threshold, int decimalPlaces)
		{
			if (value > threshold && Math.Round((double)value, decimalPlaces) <= Math.Round((double)threshold, decimalPlaces))
			{
				return (value + 1f / Mathf.Pow(10f, (float)decimalPlaces)).ToString("F" + decimalPlaces.ToString());
			}
			return value.ToString("F" + decimalPlaces.ToString());
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x0002B18D File Offset: 0x0002938D
		public static string ToStringTemperature(this float celsiusTemp, string format = "F1")
		{
			celsiusTemp = GenTemperature.CelsiusTo(celsiusTemp, Prefs.TemperatureMode);
			return celsiusTemp.ToStringTemperatureRaw(format);
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x0002B1A3 File Offset: 0x000293A3
		public static string ToStringTemperatureOffset(this float celsiusTemp, string format = "F1")
		{
			celsiusTemp = GenTemperature.CelsiusToOffset(celsiusTemp, Prefs.TemperatureMode);
			return celsiusTemp.ToStringTemperatureRaw(format);
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x0002B1BC File Offset: 0x000293BC
		public static string ToStringTemperatureRaw(this float temp, string format = "F1")
		{
			switch (Prefs.TemperatureMode)
			{
			case TemperatureDisplayMode.Celsius:
				return temp.ToString(format) + "C";
			case TemperatureDisplayMode.Fahrenheit:
				return temp.ToString(format) + "F";
			case TemperatureDisplayMode.Kelvin:
				return temp.ToString(format) + "K";
			default:
				throw new InvalidOperationException();
			}
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x0002B224 File Offset: 0x00029424
		public static string ToStringTwoDigits(this Vector2 v)
		{
			return string.Concat(new string[]
			{
				"(",
				v.x.ToString("F2"),
				", ",
				v.y.ToString("F2"),
				")"
			});
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x0002B27C File Offset: 0x0002947C
		public static string ToStringWorkAmount(this float workAmount)
		{
			return Mathf.CeilToInt(workAmount / 60f).ToString();
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x0002B2A0 File Offset: 0x000294A0
		public static string ToStringBytes(this int b, string format = "F2")
		{
			return ((float)b / 8f / 1024f).ToString(format) + "kb";
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x0002B2D0 File Offset: 0x000294D0
		public static string ToStringBytes(this uint b, string format = "F2")
		{
			return (b / 8f / 1024f).ToString(format) + "kb";
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x0002B300 File Offset: 0x00029500
		public static string ToStringBytes(this long b, string format = "F2")
		{
			return ((float)b / 8f / 1024f).ToString(format) + "kb";
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x0002B330 File Offset: 0x00029530
		public static string ToStringBytes(this ulong b, string format = "F2")
		{
			return (b / 8f / 1024f).ToString(format) + "kb";
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x0002B360 File Offset: 0x00029560
		public static string ToStringReadable(this KeyCode k)
		{
			if (k <= KeyCode.BackQuote)
			{
				switch (k)
				{
				case KeyCode.Backspace:
					return "Bksp";
				case KeyCode.Tab:
				case (KeyCode)10:
				case (KeyCode)11:
				case (KeyCode)14:
				case (KeyCode)15:
				case (KeyCode)16:
				case (KeyCode)17:
				case (KeyCode)18:
				case KeyCode.Pause:
				case (KeyCode)20:
				case (KeyCode)21:
				case (KeyCode)22:
				case (KeyCode)23:
				case (KeyCode)24:
				case (KeyCode)25:
				case (KeyCode)26:
				case (KeyCode)28:
				case (KeyCode)29:
				case (KeyCode)30:
				case (KeyCode)31:
				case KeyCode.Space:
				case KeyCode.Percent:
				case KeyCode.Equals:
					break;
				case KeyCode.Clear:
					return "Clr";
				case KeyCode.Return:
					return "Ent";
				case KeyCode.Escape:
					return "Esc";
				case KeyCode.Exclaim:
					return "!";
				case KeyCode.DoubleQuote:
					return "\"";
				case KeyCode.Hash:
					return "#";
				case KeyCode.Dollar:
					return "$";
				case KeyCode.Ampersand:
					return "&";
				case KeyCode.Quote:
					return "'";
				case KeyCode.LeftParen:
					return "(";
				case KeyCode.RightParen:
					return ")";
				case KeyCode.Asterisk:
					return "*";
				case KeyCode.Plus:
					return "+";
				case KeyCode.Comma:
					return ",";
				case KeyCode.Minus:
					return "-";
				case KeyCode.Period:
					return ".";
				case KeyCode.Slash:
					return "/";
				case KeyCode.Alpha0:
					return "0";
				case KeyCode.Alpha1:
					return "1";
				case KeyCode.Alpha2:
					return "2";
				case KeyCode.Alpha3:
					return "3";
				case KeyCode.Alpha4:
					return "4";
				case KeyCode.Alpha5:
					return "5";
				case KeyCode.Alpha6:
					return "6";
				case KeyCode.Alpha7:
					return "7";
				case KeyCode.Alpha8:
					return "8";
				case KeyCode.Alpha9:
					return "9";
				case KeyCode.Colon:
					return ":";
				case KeyCode.Semicolon:
					return ";";
				case KeyCode.Less:
					return "<";
				case KeyCode.Greater:
					return ">";
				case KeyCode.Question:
					return "?";
				case KeyCode.At:
					return "@";
				default:
					switch (k)
					{
					case KeyCode.LeftBracket:
						return "[";
					case KeyCode.Backslash:
						return "\\";
					case KeyCode.RightBracket:
						return "]";
					case KeyCode.Caret:
						return "^";
					case KeyCode.Underscore:
						return "_";
					case KeyCode.BackQuote:
						return "`";
					}
					break;
				}
			}
			else
			{
				if (k == KeyCode.Delete)
				{
					return "Del";
				}
				switch (k)
				{
				case KeyCode.Keypad0:
					return "Kp0";
				case KeyCode.Keypad1:
					return "Kp1";
				case KeyCode.Keypad2:
					return "Kp2";
				case KeyCode.Keypad3:
					return "Kp3";
				case KeyCode.Keypad4:
					return "Kp4";
				case KeyCode.Keypad5:
					return "Kp5";
				case KeyCode.Keypad6:
					return "Kp6";
				case KeyCode.Keypad7:
					return "Kp7";
				case KeyCode.Keypad8:
					return "Kp8";
				case KeyCode.Keypad9:
					return "Kp9";
				case KeyCode.KeypadPeriod:
					return "Kp.";
				case KeyCode.KeypadDivide:
					return "Kp/";
				case KeyCode.KeypadMultiply:
					return "Kp*";
				case KeyCode.KeypadMinus:
					return "Kp-";
				case KeyCode.KeypadPlus:
					return "Kp+";
				case KeyCode.KeypadEnter:
					return "KpEnt";
				case KeyCode.KeypadEquals:
					return "Kp=";
				case KeyCode.UpArrow:
					return "Up";
				case KeyCode.DownArrow:
					return "Down";
				case KeyCode.RightArrow:
					return "Right";
				case KeyCode.LeftArrow:
					return "Left";
				case KeyCode.Insert:
					return "Ins";
				case KeyCode.Home:
					return "Home";
				case KeyCode.End:
					return "End";
				case KeyCode.PageUp:
					return "PgUp";
				case KeyCode.PageDown:
					return "PgDn";
				case KeyCode.Numlock:
					return "NumL";
				case KeyCode.CapsLock:
					return "CapL";
				case KeyCode.ScrollLock:
					return "ScrL";
				case KeyCode.RightShift:
					return "RShf";
				case KeyCode.LeftShift:
					return "LShf";
				case KeyCode.RightControl:
					return "RCtrl";
				case KeyCode.LeftControl:
					return "LCtrl";
				case KeyCode.RightAlt:
					return "RAlt";
				case KeyCode.LeftAlt:
					return "LAlt";
				case KeyCode.RightMeta:
					return "Appl";
				case KeyCode.LeftMeta:
					return "Cmd";
				case KeyCode.LeftWindows:
					return "Win";
				case KeyCode.RightWindows:
					return "Win";
				case KeyCode.AltGr:
					return "AltGr";
				case KeyCode.Help:
					return "Help";
				case KeyCode.Print:
					return "Prnt";
				case KeyCode.SysReq:
					return "SysReq";
				case KeyCode.Break:
					return "Brk";
				case KeyCode.Menu:
					return "Menu";
				}
			}
			return k.ToString();
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x0002B7B1 File Offset: 0x000299B1
		public static void AppendWithComma(this StringBuilder sb, string text)
		{
			sb.AppendWithSeparator(text, ", ");
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x0002B7BF File Offset: 0x000299BF
		public static StringBuilder AppendInNewLine(this StringBuilder sb, string text)
		{
			sb.AppendWithSeparator(text, "\n");
			return sb;
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x0002B7CE File Offset: 0x000299CE
		public static void AppendWithSeparator(this StringBuilder sb, string text, string separator)
		{
			if (text.NullOrEmpty())
			{
				return;
			}
			if (sb.Length > 0)
			{
				sb.Append(separator);
			}
			sb.Append(text);
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x0002B7F4 File Offset: 0x000299F4
		public static string WordWrapAt(this string text, float length)
		{
			Text.Font = GameFont.Medium;
			if (text.GetWidthCached() < length)
			{
				return text;
			}
			IEnumerable<Pair<char, int>> enumerable = from p in text.Select<char, Pair<char, int>>((char c, int idx) => new Pair<char, int>(c, idx))
				where p.First == ' '
				select p;
			if (!enumerable.Any<Pair<char, int>>())
			{
				return text;
			}
			Pair<char, int> pair = enumerable.MinBy<Pair<char, int>, float>((Pair<char, int> p) => Mathf.Abs(text.Substring(0, p.Second).GetWidthCached() - text.Substring(p.Second + 1).GetWidthCached()));
			return text.Substring(0, pair.Second) + "\n" + text.Substring(pair.Second + 1);
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x0002B8CC File Offset: 0x00029ACC
		public static string EventTypeToStringCached(EventType eventType)
		{
			if (GenText.eventTypesCached == null)
			{
				int num = 0;
				foreach (object obj in Enum.GetValues(typeof(EventType)))
				{
					num = Mathf.Max(num, (int)obj);
				}
				GenText.eventTypesCached = new string[num + 1];
				foreach (object obj2 in Enum.GetValues(typeof(EventType)))
				{
					GenText.eventTypesCached[(int)obj2] = obj2.ToString();
				}
			}
			if (eventType >= EventType.MouseDown && eventType < (EventType)GenText.eventTypesCached.Length)
			{
				return GenText.eventTypesCached[(int)eventType];
			}
			return "Unknown";
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x0002B9C0 File Offset: 0x00029BC0
		public static string FieldsToString<T>(T obj)
		{
			if (obj == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (FieldInfo fieldInfo in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(fieldInfo.Name);
				stringBuilder.Append("=");
				object value = fieldInfo.GetValue(obj);
				if (value == null)
				{
					stringBuilder.Append("null");
				}
				else
				{
					stringBuilder.Append(value.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x0002BA68 File Offset: 0x00029C68
		public static bool TryGetSeparatedValues(string str, char separator, out string[] output)
		{
			if (str.NullOrEmpty() || !str.Contains(separator))
			{
				output = null;
				return false;
			}
			GenText.separatorArrayTmp[0] = separator;
			output = str.Split(GenText.separatorArrayTmp, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < output.Length; i++)
			{
				output[i] = output[i].Trim();
			}
			return true;
		}

		// Token: 0x04000496 RID: 1174
		private const int SaveNameMaxLength = 40;

		// Token: 0x04000497 RID: 1175
		private const char DegreeSymbol = '°';

		// Token: 0x04000498 RID: 1176
		private static StringBuilder tmpSb = new StringBuilder();

		// Token: 0x04000499 RID: 1177
		private static string[] tmpSeparators = new string[1];

		// Token: 0x0400049A RID: 1178
		private static StringBuilder tmpSbForCapitalizedSentences = new StringBuilder();

		// Token: 0x0400049B RID: 1179
		private static StringBuilder tmpStringBuilder = new StringBuilder();

		// Token: 0x0400049C RID: 1180
		private static string[] eventTypesCached;

		// Token: 0x0400049D RID: 1181
		private static readonly char[] separatorArrayTmp = new char[] { ' ' };
	}
}
