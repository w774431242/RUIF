using System;
using System.Collections.Generic;
using System.IO;
using RimWorld.IO;
using Steamworks;
using UnityEngine;
using Verse.Steam;

namespace Verse
{
	// Token: 0x02000377 RID: 887
	public static class LanguageDatabase
	{
		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x060016BB RID: 5819 RVA: 0x0007105B File Offset: 0x0006F25B
		public static IEnumerable<LoadedLanguage> AllLoadedLanguages
		{
			get
			{
				return LanguageDatabase.languages;
			}
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x00071062 File Offset: 0x0006F262
		public static void SelectLanguage(LoadedLanguage lang)
		{
			Prefs.LangFolderName = lang.folderName;
			LongEventHandler.QueueLongEvent(delegate
			{
				PlayDataLoader.ClearAllPlayData();
				PlayDataLoader.LoadAllPlayData(false);
			}, "LoadingLongEvent", true, null, true, false, null);
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x0007109D File Offset: 0x0006F29D
		public static void Clear()
		{
			LanguageDatabase.languages.Clear();
			LanguageDatabase.activeLanguage = null;
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x000710B0 File Offset: 0x0006F2B0
		public static void InitAllMetadata()
		{
			foreach (ModContentPack modContentPack in LoadedModManager.RunningMods)
			{
				HashSet<string> hashSet = new HashSet<string>();
				foreach (string text in modContentPack.foldersToLoadDescendingOrder)
				{
					string text2 = Path.Combine(text, "Languages");
					if (new DirectoryInfo(text2).Exists)
					{
						foreach (VirtualDirectory virtualDirectory in AbstractFilesystem.GetDirectories(text2, "*", SearchOption.TopDirectoryOnly, false))
						{
							if (!virtualDirectory.FullPath.StartsWith(text2))
							{
								Log.Error("Failed to get a relative path for a file: " + virtualDirectory.FullPath + ", located in " + text2);
							}
							else
							{
								string text3 = virtualDirectory.FullPath.Substring(text2.Length);
								if (!hashSet.Contains(text3))
								{
									LanguageDatabase.InitLanguageMetadataFrom(virtualDirectory);
									hashSet.Add(text3);
								}
							}
						}
					}
				}
			}
			LanguageDatabase.languages.SortBy<LoadedLanguage, string>((LoadedLanguage l) => l.folderName);
			LanguageDatabase.defaultLanguage = LanguageDatabase.languages.FirstOrDefault<LoadedLanguage>((LoadedLanguage la) => la.folderName == LanguageDatabase.DefaultLangFolderName);
			LanguageDatabase.activeLanguage = LanguageDatabase.languages.FirstOrDefault<LoadedLanguage>((LoadedLanguage la) => la.folderName == Prefs.LangFolderName);
			if (LanguageDatabase.activeLanguage == null)
			{
				Prefs.LangFolderName = LanguageDatabase.DefaultLangFolderName;
				LanguageDatabase.activeLanguage = LanguageDatabase.languages.FirstOrDefault<LoadedLanguage>((LoadedLanguage la) => la.folderName == Prefs.LangFolderName);
			}
			if (LanguageDatabase.activeLanguage == null || LanguageDatabase.defaultLanguage == null)
			{
				Log.Error("No default language found!");
				LanguageDatabase.defaultLanguage = LanguageDatabase.languages[0];
				LanguageDatabase.activeLanguage = LanguageDatabase.languages[0];
			}
			LanguageDatabase.activeLanguage.LoadMetadata();
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x00071324 File Offset: 0x0006F524
		private static LoadedLanguage InitLanguageMetadataFrom(VirtualDirectory langDir)
		{
			LoadedLanguage loadedLanguage = LanguageDatabase.languages.FirstOrDefault<LoadedLanguage>((LoadedLanguage lib) => lib.folderName == langDir.Name || lib.LegacyFolderName == langDir.Name);
			if (loadedLanguage == null)
			{
				loadedLanguage = new LoadedLanguage(langDir.Name);
				LanguageDatabase.languages.Add(loadedLanguage);
			}
			if (loadedLanguage != null)
			{
				loadedLanguage.InitMetadata(langDir);
			}
			return loadedLanguage;
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x00071384 File Offset: 0x0006F584
		public static string SystemLanguageFolderName()
		{
			if (SteamManager.Initialized)
			{
				string text = SteamApps.GetCurrentGameLanguage().CapitalizeFirst();
				if (LanguageDatabase.SupportedAutoSelectLanguages.Contains(text))
				{
					return text;
				}
			}
			string text2 = Application.systemLanguage.ToString();
			if (LanguageDatabase.SupportedAutoSelectLanguages.Contains(text2))
			{
				return text2;
			}
			return LanguageDatabase.DefaultLangFolderName;
		}

		// Token: 0x040014C4 RID: 5316
		private static List<LoadedLanguage> languages = new List<LoadedLanguage>();

		// Token: 0x040014C5 RID: 5317
		public static LoadedLanguage activeLanguage;

		// Token: 0x040014C6 RID: 5318
		public static LoadedLanguage defaultLanguage;

		// Token: 0x040014C7 RID: 5319
		public static readonly string DefaultLangFolderName = "English";

		// Token: 0x040014C8 RID: 5320
		private static readonly List<string> SupportedAutoSelectLanguages = new List<string>
		{
			"Arabic", "ChineseSimplified", "ChineseTraditional", "Czech", "Danish", "Dutch", "English", "Estonian", "Finnish", "French",
			"German", "Hungarian", "Italian", "Japanese", "Korean", "Norwegian", "Polish", "Portuguese", "PortugueseBrazilian", "Romanian",
			"Russian", "Slovak", "Spanish", "SpanishLatin", "Swedish", "Turkish", "Ukrainian"
		};
	}
}
