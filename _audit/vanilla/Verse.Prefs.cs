using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LudeonTK;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000D52 RID: 3410
	public static class Prefs
	{
		// Token: 0x17000D85 RID: 3461
		// (get) Token: 0x060056C0 RID: 22208 RVA: 0x001BFBAB File Offset: 0x001BDDAB
		// (set) Token: 0x060056C1 RID: 22209 RVA: 0x001BFBB7 File Offset: 0x001BDDB7
		public static float VolumeMaster
		{
			get
			{
				return Prefs.data.volumeMaster;
			}
			set
			{
				if (Prefs.data.volumeMaster == value)
				{
					return;
				}
				Prefs.data.volumeMaster = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D86 RID: 3462
		// (get) Token: 0x060056C2 RID: 22210 RVA: 0x001BFBD7 File Offset: 0x001BDDD7
		// (set) Token: 0x060056C3 RID: 22211 RVA: 0x001BFBE3 File Offset: 0x001BDDE3
		public static float VolumeGame
		{
			get
			{
				return Prefs.data.volumeGame;
			}
			set
			{
				if (Prefs.data.volumeGame == value)
				{
					return;
				}
				Prefs.data.volumeGame = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D87 RID: 3463
		// (get) Token: 0x060056C4 RID: 22212 RVA: 0x001BFC03 File Offset: 0x001BDE03
		// (set) Token: 0x060056C5 RID: 22213 RVA: 0x001BFC0F File Offset: 0x001BDE0F
		public static float VolumeMusic
		{
			get
			{
				return Prefs.data.volumeMusic;
			}
			set
			{
				if (Prefs.data.volumeMusic == value)
				{
					return;
				}
				Prefs.data.volumeMusic = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D88 RID: 3464
		// (get) Token: 0x060056C6 RID: 22214 RVA: 0x001BFC2F File Offset: 0x001BDE2F
		// (set) Token: 0x060056C7 RID: 22215 RVA: 0x001BFC3B File Offset: 0x001BDE3B
		public static float VolumeAmbient
		{
			get
			{
				return Prefs.data.volumeAmbient;
			}
			set
			{
				if (Prefs.data.volumeAmbient == value)
				{
					return;
				}
				Prefs.data.volumeAmbient = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D89 RID: 3465
		// (get) Token: 0x060056C8 RID: 22216 RVA: 0x001BFC5B File Offset: 0x001BDE5B
		// (set) Token: 0x060056C9 RID: 22217 RVA: 0x001BFC67 File Offset: 0x001BDE67
		public static float VolumeUI
		{
			get
			{
				return Prefs.data.volumeUI;
			}
			set
			{
				if (Prefs.data.volumeUI == value)
				{
					return;
				}
				Prefs.data.volumeUI = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D8A RID: 3466
		// (get) Token: 0x060056CA RID: 22218 RVA: 0x001BFC87 File Offset: 0x001BDE87
		// (set) Token: 0x060056CB RID: 22219 RVA: 0x001BFC93 File Offset: 0x001BDE93
		public static bool AdaptiveTrainingEnabled
		{
			get
			{
				return Prefs.data.adaptiveTrainingEnabled;
			}
			set
			{
				if (Prefs.data.adaptiveTrainingEnabled == value)
				{
					return;
				}
				Prefs.data.adaptiveTrainingEnabled = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D8B RID: 3467
		// (get) Token: 0x060056CC RID: 22220 RVA: 0x001BFCB3 File Offset: 0x001BDEB3
		// (set) Token: 0x060056CD RID: 22221 RVA: 0x001BFCBF File Offset: 0x001BDEBF
		public static bool SteamDeckKeyboardMode
		{
			get
			{
				return Prefs.data.steamDeckKeyboardMode;
			}
			set
			{
				if (Prefs.data.steamDeckKeyboardMode == value)
				{
					return;
				}
				Prefs.data.steamDeckKeyboardMode = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D8C RID: 3468
		// (get) Token: 0x060056CE RID: 22222 RVA: 0x001BFCDF File Offset: 0x001BDEDF
		// (set) Token: 0x060056CF RID: 22223 RVA: 0x001BFCEB File Offset: 0x001BDEEB
		public static bool EdgeScreenScroll
		{
			get
			{
				return Prefs.data.edgeScreenScroll;
			}
			set
			{
				if (Prefs.data.edgeScreenScroll == value)
				{
					return;
				}
				Prefs.data.edgeScreenScroll = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D8D RID: 3469
		// (get) Token: 0x060056D0 RID: 22224 RVA: 0x001BFD0B File Offset: 0x001BDF0B
		// (set) Token: 0x060056D1 RID: 22225 RVA: 0x001BFD17 File Offset: 0x001BDF17
		public static bool RememberDrawStlyes
		{
			get
			{
				return Prefs.data.rememberDrawStyles;
			}
			set
			{
				if (Prefs.data.rememberDrawStyles == value)
				{
					return;
				}
				Prefs.data.rememberDrawStyles = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D8E RID: 3470
		// (get) Token: 0x060056D2 RID: 22226 RVA: 0x001BFD37 File Offset: 0x001BDF37
		// (set) Token: 0x060056D3 RID: 22227 RVA: 0x001BFD43 File Offset: 0x001BDF43
		public static bool ZoomSwitchWorldLayer
		{
			get
			{
				return Prefs.data.zoomSwitchWorldLayer;
			}
			set
			{
				if (Prefs.data.zoomSwitchWorldLayer == value)
				{
					return;
				}
				Prefs.data.zoomSwitchWorldLayer = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D8F RID: 3471
		// (get) Token: 0x060056D4 RID: 22228 RVA: 0x001BFD63 File Offset: 0x001BDF63
		// (set) Token: 0x060056D5 RID: 22229 RVA: 0x001BFD6F File Offset: 0x001BDF6F
		public static bool ZoomToMouse
		{
			get
			{
				return Prefs.data.zoomToMouse;
			}
			set
			{
				if (Prefs.data.zoomToMouse == value)
				{
					return;
				}
				Prefs.data.zoomToMouse = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D90 RID: 3472
		// (get) Token: 0x060056D6 RID: 22230 RVA: 0x001BFD8F File Offset: 0x001BDF8F
		// (set) Token: 0x060056D7 RID: 22231 RVA: 0x001BFD9B File Offset: 0x001BDF9B
		public static float ScreenShakeIntensity
		{
			get
			{
				return Prefs.data.screenShakeIntensity;
			}
			set
			{
				if (Prefs.data.screenShakeIntensity == value)
				{
					return;
				}
				Prefs.data.screenShakeIntensity = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D91 RID: 3473
		// (get) Token: 0x060056D8 RID: 22232 RVA: 0x001BFDBB File Offset: 0x001BDFBB
		// (set) Token: 0x060056D9 RID: 22233 RVA: 0x001BFDC7 File Offset: 0x001BDFC7
		public static bool RunInBackground
		{
			get
			{
				return Prefs.data.runInBackground;
			}
			set
			{
				if (Prefs.data.runInBackground == value)
				{
					return;
				}
				Prefs.data.runInBackground = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D92 RID: 3474
		// (get) Token: 0x060056DA RID: 22234 RVA: 0x001BFDE7 File Offset: 0x001BDFE7
		// (set) Token: 0x060056DB RID: 22235 RVA: 0x001BFDF3 File Offset: 0x001BDFF3
		public static TemperatureDisplayMode TemperatureMode
		{
			get
			{
				return Prefs.data.temperatureMode;
			}
			set
			{
				if (Prefs.data.temperatureMode == value)
				{
					return;
				}
				Prefs.data.temperatureMode = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D93 RID: 3475
		// (get) Token: 0x060056DC RID: 22236 RVA: 0x001BFE13 File Offset: 0x001BE013
		// (set) Token: 0x060056DD RID: 22237 RVA: 0x001BFE1F File Offset: 0x001BE01F
		public static float AutosaveIntervalDays
		{
			get
			{
				return Prefs.data.autosaveIntervalDays;
			}
			set
			{
				if (Prefs.data.autosaveIntervalDays == value)
				{
					return;
				}
				Prefs.data.autosaveIntervalDays = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D94 RID: 3476
		// (get) Token: 0x060056DE RID: 22238 RVA: 0x001BFE3F File Offset: 0x001BE03F
		// (set) Token: 0x060056DF RID: 22239 RVA: 0x001BFE4B File Offset: 0x001BE04B
		public static bool CustomCursorEnabled
		{
			get
			{
				return Prefs.data.customCursorEnabled;
			}
			set
			{
				if (Prefs.data.customCursorEnabled == value)
				{
					return;
				}
				Prefs.data.customCursorEnabled = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D95 RID: 3477
		// (get) Token: 0x060056E0 RID: 22240 RVA: 0x001BFE6B File Offset: 0x001BE06B
		// (set) Token: 0x060056E1 RID: 22241 RVA: 0x001BFE77 File Offset: 0x001BE077
		public static DotHighlightDisplayMode DotHighlightDisplayMode
		{
			get
			{
				return Prefs.data.dotHighlightDisplayMode;
			}
			set
			{
				if (Prefs.data.dotHighlightDisplayMode == value)
				{
					return;
				}
				Prefs.data.dotHighlightDisplayMode = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D96 RID: 3478
		// (get) Token: 0x060056E2 RID: 22242 RVA: 0x001BFE97 File Offset: 0x001BE097
		// (set) Token: 0x060056E3 RID: 22243 RVA: 0x001BFEA3 File Offset: 0x001BE0A3
		public static HighlightStyleMode HighlightStyleMode
		{
			get
			{
				return Prefs.data.highlightStyleMode;
			}
			set
			{
				if (Prefs.data.highlightStyleMode == value)
				{
					return;
				}
				Prefs.data.highlightStyleMode = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D97 RID: 3479
		// (get) Token: 0x060056E4 RID: 22244 RVA: 0x001BFEC3 File Offset: 0x001BE0C3
		// (set) Token: 0x060056E5 RID: 22245 RVA: 0x001BFECF File Offset: 0x001BE0CF
		public static AnimalNameDisplayMode AnimalNameMode
		{
			get
			{
				return Prefs.data.animalNameMode;
			}
			set
			{
				if (Prefs.data.animalNameMode == value)
				{
					return;
				}
				Prefs.data.animalNameMode = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D98 RID: 3480
		// (get) Token: 0x060056E6 RID: 22246 RVA: 0x001BFEEF File Offset: 0x001BE0EF
		// (set) Token: 0x060056E7 RID: 22247 RVA: 0x001BFEFB File Offset: 0x001BE0FB
		public static MechNameDisplayMode MechNameMode
		{
			get
			{
				return Prefs.data.mechNameMode;
			}
			set
			{
				if (Prefs.data.mechNameMode == value)
				{
					return;
				}
				Prefs.data.mechNameMode = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D99 RID: 3481
		// (get) Token: 0x060056E8 RID: 22248 RVA: 0x001BFF1B File Offset: 0x001BE11B
		// (set) Token: 0x060056E9 RID: 22249 RVA: 0x001BFF27 File Offset: 0x001BE127
		public static ShowWeaponsUnderPortraitMode ShowWeaponsUnderPortraitMode
		{
			get
			{
				return Prefs.data.showWeaponsUnderPortraitMode;
			}
			set
			{
				if (Prefs.data.showWeaponsUnderPortraitMode == value)
				{
					return;
				}
				Prefs.data.showWeaponsUnderPortraitMode = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D9A RID: 3482
		// (get) Token: 0x060056EA RID: 22250 RVA: 0x001BFF47 File Offset: 0x001BE147
		// (set) Token: 0x060056EB RID: 22251 RVA: 0x001BFF53 File Offset: 0x001BE153
		public static bool VisibleMood
		{
			get
			{
				return Prefs.data.visibleMood;
			}
			set
			{
				if (Prefs.data.visibleMood == value)
				{
					return;
				}
				Prefs.data.visibleMood = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D9B RID: 3483
		// (get) Token: 0x060056EC RID: 22252 RVA: 0x001BFF73 File Offset: 0x001BE173
		// (set) Token: 0x060056ED RID: 22253 RVA: 0x001BFF88 File Offset: 0x001BE188
		public static bool DevMode
		{
			get
			{
				return Prefs.data == null || Prefs.data.devMode;
			}
			set
			{
				if (Prefs.data.devMode == value)
				{
					return;
				}
				Prefs.data.devMode = value;
				if (!Prefs.data.devMode)
				{
					Prefs.data.logVerbose = false;
					Prefs.data.resetModsConfigOnCrash = true;
					DebugSettings.godMode = false;
					Find.WindowStack.TryRemove(typeof(Dialog_DevPalette), true);
				}
				Prefs.Apply();
			}
		}

		// Token: 0x17000D9C RID: 3484
		// (get) Token: 0x060056EE RID: 22254 RVA: 0x001BFFF1 File Offset: 0x001BE1F1
		// (set) Token: 0x060056EF RID: 22255 RVA: 0x001C0006 File Offset: 0x001BE206
		public static bool ResetModsConfigOnCrash
		{
			get
			{
				return Prefs.data == null || Prefs.data.resetModsConfigOnCrash;
			}
			set
			{
				if (Prefs.data.resetModsConfigOnCrash == value)
				{
					return;
				}
				Prefs.data.resetModsConfigOnCrash = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D9D RID: 3485
		// (get) Token: 0x060056F0 RID: 22256 RVA: 0x001C0026 File Offset: 0x001BE226
		// (set) Token: 0x060056F1 RID: 22257 RVA: 0x001C0032 File Offset: 0x001BE232
		public static List<string> PreferredNames
		{
			get
			{
				return Prefs.data.preferredNames;
			}
			set
			{
				if (Prefs.data.preferredNames == value)
				{
					return;
				}
				Prefs.data.preferredNames = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D9E RID: 3486
		// (get) Token: 0x060056F2 RID: 22258 RVA: 0x001C0052 File Offset: 0x001BE252
		// (set) Token: 0x060056F3 RID: 22259 RVA: 0x001C005E File Offset: 0x001BE25E
		public static string LangFolderName
		{
			get
			{
				return Prefs.data.langFolderName;
			}
			set
			{
				if (Prefs.data.langFolderName == value)
				{
					return;
				}
				Prefs.data.langFolderName = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000D9F RID: 3487
		// (get) Token: 0x060056F4 RID: 22260 RVA: 0x001C0083 File Offset: 0x001BE283
		// (set) Token: 0x060056F5 RID: 22261 RVA: 0x001C008F File Offset: 0x001BE28F
		public static bool LogVerbose
		{
			get
			{
				return Prefs.data.logVerbose;
			}
			set
			{
				if (Prefs.data.logVerbose == value)
				{
					return;
				}
				Prefs.data.logVerbose = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000DA0 RID: 3488
		// (get) Token: 0x060056F6 RID: 22262 RVA: 0x001C00AF File Offset: 0x001BE2AF
		// (set) Token: 0x060056F7 RID: 22263 RVA: 0x001C00BB File Offset: 0x001BE2BB
		public static bool DisableQuickStartCryptoSickness
		{
			get
			{
				return Prefs.data.disableQuickStartCryptoSickness;
			}
			set
			{
				if (Prefs.data.disableQuickStartCryptoSickness == value)
				{
					return;
				}
				Prefs.data.disableQuickStartCryptoSickness = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000DA1 RID: 3489
		// (get) Token: 0x060056F8 RID: 22264 RVA: 0x001C00DB File Offset: 0x001BE2DB
		// (set) Token: 0x060056F9 RID: 22265 RVA: 0x001C00E7 File Offset: 0x001BE2E7
		public static bool StartDevPaletteOn
		{
			get
			{
				return Prefs.data.quickStartDevPaletteOn;
			}
			set
			{
				if (Prefs.data.quickStartDevPaletteOn == value)
				{
					return;
				}
				Prefs.data.quickStartDevPaletteOn = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000DA2 RID: 3490
		// (get) Token: 0x060056FA RID: 22266 RVA: 0x001C0107 File Offset: 0x001BE307
		// (set) Token: 0x060056FB RID: 22267 RVA: 0x001C0113 File Offset: 0x001BE313
		public static bool PauseOnLoad
		{
			get
			{
				return Prefs.data.pauseOnLoad;
			}
			set
			{
				Prefs.data.pauseOnLoad = value;
			}
		}

		// Token: 0x17000DA3 RID: 3491
		// (get) Token: 0x060056FC RID: 22268 RVA: 0x001C0120 File Offset: 0x001BE320
		// (set) Token: 0x060056FD RID: 22269 RVA: 0x001C012C File Offset: 0x001BE32C
		public static AutomaticPauseMode AutomaticPauseMode
		{
			get
			{
				return Prefs.data.automaticPauseMode;
			}
			set
			{
				Prefs.data.automaticPauseMode = value;
			}
		}

		// Token: 0x17000DA4 RID: 3492
		// (get) Token: 0x060056FE RID: 22270 RVA: 0x001C0139 File Offset: 0x001BE339
		// (set) Token: 0x060056FF RID: 22271 RVA: 0x001C0145 File Offset: 0x001BE345
		public static bool ShowRealtimeClock
		{
			get
			{
				return Prefs.data.showRealtimeClock;
			}
			set
			{
				Prefs.data.showRealtimeClock = value;
			}
		}

		// Token: 0x17000DA5 RID: 3493
		// (get) Token: 0x06005700 RID: 22272 RVA: 0x001C0152 File Offset: 0x001BE352
		// (set) Token: 0x06005701 RID: 22273 RVA: 0x001C015E File Offset: 0x001BE35E
		public static bool TwelveHourClockMode
		{
			get
			{
				return Prefs.data.twelveHourClock;
			}
			set
			{
				Prefs.data.twelveHourClock = value;
			}
		}

		// Token: 0x17000DA6 RID: 3494
		// (get) Token: 0x06005702 RID: 22274 RVA: 0x001C016B File Offset: 0x001BE36B
		// (set) Token: 0x06005703 RID: 22275 RVA: 0x001C0177 File Offset: 0x001BE377
		public static bool DisableTinyText
		{
			get
			{
				return Prefs.data.disableTinyText;
			}
			set
			{
				Prefs.data.disableTinyText = value;
			}
		}

		// Token: 0x17000DA7 RID: 3495
		// (get) Token: 0x06005704 RID: 22276 RVA: 0x001C0184 File Offset: 0x001BE384
		// (set) Token: 0x06005705 RID: 22277 RVA: 0x001C0190 File Offset: 0x001BE390
		public static bool TestMapSizes
		{
			get
			{
				return Prefs.data.testMapSizes;
			}
			set
			{
				Prefs.data.testMapSizes = value;
			}
		}

		// Token: 0x17000DA8 RID: 3496
		// (get) Token: 0x06005706 RID: 22278 RVA: 0x001C019D File Offset: 0x001BE39D
		// (set) Token: 0x06005707 RID: 22279 RVA: 0x001C01A9 File Offset: 0x001BE3A9
		public static int MaxNumberOfPlayerSettlements
		{
			get
			{
				return Prefs.data.maxNumberOfPlayerSettlements;
			}
			set
			{
				Prefs.data.maxNumberOfPlayerSettlements = value;
			}
		}

		// Token: 0x17000DA9 RID: 3497
		// (get) Token: 0x06005708 RID: 22280 RVA: 0x001C01B6 File Offset: 0x001BE3B6
		// (set) Token: 0x06005709 RID: 22281 RVA: 0x001C01C2 File Offset: 0x001BE3C2
		public static bool PlantWindSway
		{
			get
			{
				return Prefs.data.plantWindSway;
			}
			set
			{
				Prefs.data.plantWindSway = value;
			}
		}

		// Token: 0x17000DAA RID: 3498
		// (get) Token: 0x0600570A RID: 22282 RVA: 0x001C01CF File Offset: 0x001BE3CF
		// (set) Token: 0x0600570B RID: 22283 RVA: 0x001C01DB File Offset: 0x001BE3DB
		public static bool TextureCompression
		{
			get
			{
				return Prefs.data.textureCompression;
			}
			set
			{
				Prefs.data.textureCompression = value;
			}
		}

		// Token: 0x17000DAB RID: 3499
		// (get) Token: 0x0600570C RID: 22284 RVA: 0x001C01E8 File Offset: 0x001BE3E8
		// (set) Token: 0x0600570D RID: 22285 RVA: 0x001C01F4 File Offset: 0x001BE3F4
		public static bool ResourceReadoutCategorized
		{
			get
			{
				return Prefs.data.resourceReadoutCategorized;
			}
			set
			{
				if (value == Prefs.data.resourceReadoutCategorized)
				{
					return;
				}
				Prefs.data.resourceReadoutCategorized = value;
				Prefs.Save();
			}
		}

		// Token: 0x17000DAC RID: 3500
		// (get) Token: 0x0600570E RID: 22286 RVA: 0x001C0214 File Offset: 0x001BE414
		// (set) Token: 0x0600570F RID: 22287 RVA: 0x001C0220 File Offset: 0x001BE420
		public static float UIScale
		{
			get
			{
				return Prefs.data.uiScale;
			}
			set
			{
				Prefs.data.uiScale = value;
			}
		}

		// Token: 0x17000DAD RID: 3501
		// (get) Token: 0x06005710 RID: 22288 RVA: 0x001C022D File Offset: 0x001BE42D
		// (set) Token: 0x06005711 RID: 22289 RVA: 0x001C0239 File Offset: 0x001BE439
		public static int ScreenWidth
		{
			get
			{
				return Prefs.data.screenWidth;
			}
			set
			{
				Prefs.data.screenWidth = value;
			}
		}

		// Token: 0x17000DAE RID: 3502
		// (get) Token: 0x06005712 RID: 22290 RVA: 0x001C0246 File Offset: 0x001BE446
		// (set) Token: 0x06005713 RID: 22291 RVA: 0x001C0252 File Offset: 0x001BE452
		public static int ScreenHeight
		{
			get
			{
				return Prefs.data.screenHeight;
			}
			set
			{
				Prefs.data.screenHeight = value;
			}
		}

		// Token: 0x17000DAF RID: 3503
		// (get) Token: 0x06005714 RID: 22292 RVA: 0x001C025F File Offset: 0x001BE45F
		// (set) Token: 0x06005715 RID: 22293 RVA: 0x001C026B File Offset: 0x001BE46B
		public static bool FullScreen
		{
			get
			{
				return Prefs.data.fullscreen;
			}
			set
			{
				Prefs.data.fullscreen = value;
			}
		}

		// Token: 0x17000DB0 RID: 3504
		// (get) Token: 0x06005716 RID: 22294 RVA: 0x001C0278 File Offset: 0x001BE478
		// (set) Token: 0x06005717 RID: 22295 RVA: 0x001C0284 File Offset: 0x001BE484
		public static bool HatsOnlyOnMap
		{
			get
			{
				return Prefs.data.hatsOnlyOnMap;
			}
			set
			{
				if (Prefs.data.hatsOnlyOnMap == value)
				{
					return;
				}
				Prefs.data.hatsOnlyOnMap = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000DB1 RID: 3505
		// (get) Token: 0x06005718 RID: 22296 RVA: 0x001C02A4 File Offset: 0x001BE4A4
		// (set) Token: 0x06005719 RID: 22297 RVA: 0x001C02B0 File Offset: 0x001BE4B0
		public static float MapDragSensitivity
		{
			get
			{
				return Prefs.data.mapDragSensitivity;
			}
			set
			{
				Prefs.data.mapDragSensitivity = value;
			}
		}

		// Token: 0x17000DB2 RID: 3506
		// (get) Token: 0x0600571A RID: 22298 RVA: 0x001C02C0 File Offset: 0x001BE4C0
		// (set) Token: 0x0600571B RID: 22299 RVA: 0x001C0307 File Offset: 0x001BE507
		public static ExpansionDef BackgroundImageExpansion
		{
			get
			{
				if (Prefs.data.backgroundExpansionId != null)
				{
					ExpansionDef expansionWithIdentifier = ModLister.GetExpansionWithIdentifier(Prefs.data.backgroundExpansionId);
					if (expansionWithIdentifier != null && expansionWithIdentifier.Status != ExpansionStatus.NotInstalled)
					{
						return expansionWithIdentifier;
					}
				}
				ExpansionDef lastInstalledExpansion = ModsConfig.LastInstalledExpansion;
				if (lastInstalledExpansion != null)
				{
					return lastInstalledExpansion;
				}
				return ExpansionDefOf.Core;
			}
			set
			{
				Prefs.data.backgroundExpansionId = ((value != null) ? value.linkedMod : null);
				((UI_BackgroundMain)UIMenuBackgroundManager.background).overrideBGImage = ((value != null) ? value.BackgroundImage : null);
			}
		}

		// Token: 0x17000DB3 RID: 3507
		// (get) Token: 0x0600571C RID: 22300 RVA: 0x001C033A File Offset: 0x001BE53A
		// (set) Token: 0x0600571D RID: 22301 RVA: 0x001C0346 File Offset: 0x001BE546
		public static bool RandomBackgroundImage
		{
			get
			{
				return Prefs.data.randomBackground;
			}
			set
			{
				Prefs.data.randomBackground = value;
			}
		}

		// Token: 0x17000DB4 RID: 3508
		// (get) Token: 0x0600571E RID: 22302 RVA: 0x001C0353 File Offset: 0x001BE553
		// (set) Token: 0x0600571F RID: 22303 RVA: 0x001C035F File Offset: 0x001BE55F
		public static List<string> DebugActionsPalette
		{
			get
			{
				return Prefs.data.debugActionPalette;
			}
			set
			{
				if (Prefs.data.debugActionPalette != value)
				{
					Prefs.data.debugActionPalette = value;
					Prefs.Save();
				}
			}
		}

		// Token: 0x17000DB5 RID: 3509
		// (get) Token: 0x06005720 RID: 22304 RVA: 0x001C037E File Offset: 0x001BE57E
		// (set) Token: 0x06005721 RID: 22305 RVA: 0x001C038A File Offset: 0x001BE58A
		public static Vector2 DevPalettePosition
		{
			get
			{
				return Prefs.data.devPalettePosition;
			}
			set
			{
				if (Prefs.data.devPalettePosition != value)
				{
					Prefs.data.devPalettePosition = value;
					Prefs.Save();
				}
			}
		}

		// Token: 0x17000DB6 RID: 3510
		// (get) Token: 0x06005722 RID: 22306 RVA: 0x001C03AE File Offset: 0x001BE5AE
		// (set) Token: 0x06005723 RID: 22307 RVA: 0x001C03BA File Offset: 0x001BE5BA
		public static bool SmoothCameraJumps
		{
			get
			{
				return Prefs.data.smoothCameraJumps;
			}
			set
			{
				if (Prefs.data.smoothCameraJumps == value)
				{
					return;
				}
				Prefs.data.smoothCameraJumps = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000DB7 RID: 3511
		// (get) Token: 0x06005724 RID: 22308 RVA: 0x001C03DA File Offset: 0x001BE5DA
		// (set) Token: 0x06005725 RID: 22309 RVA: 0x001C03E6 File Offset: 0x001BE5E6
		public static bool GravshipCutscenes
		{
			get
			{
				return Prefs.data.gravshipCutscenes;
			}
			set
			{
				if (Prefs.data.gravshipCutscenes == value)
				{
					return;
				}
				Prefs.data.gravshipCutscenes = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000DB8 RID: 3512
		// (get) Token: 0x06005726 RID: 22310 RVA: 0x001C0406 File Offset: 0x001BE606
		// (set) Token: 0x06005727 RID: 22311 RVA: 0x001C0418 File Offset: 0x001BE618
		public static bool OpenLogOnWarnings
		{
			get
			{
				PrefsData prefsData = Prefs.data;
				return prefsData != null && prefsData.openLogOnWarnings;
			}
			set
			{
				if (Prefs.data.openLogOnWarnings == value)
				{
					return;
				}
				Prefs.data.openLogOnWarnings = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000DB9 RID: 3513
		// (get) Token: 0x06005728 RID: 22312 RVA: 0x001C0438 File Offset: 0x001BE638
		// (set) Token: 0x06005729 RID: 22313 RVA: 0x001C0444 File Offset: 0x001BE644
		public static bool CloseLogWindowOnEscape
		{
			get
			{
				return Prefs.data.closeLogWindowOnEscape;
			}
			set
			{
				if (Prefs.data.closeLogWindowOnEscape == value)
				{
					return;
				}
				Prefs.data.closeLogWindowOnEscape = value;
				Prefs.Apply();
			}
		}

		// Token: 0x17000DBA RID: 3514
		// (get) Token: 0x0600572A RID: 22314 RVA: 0x001C0464 File Offset: 0x001BE664
		// (set) Token: 0x0600572B RID: 22315 RVA: 0x001C0470 File Offset: 0x001BE670
		public static int AutosavesCount
		{
			get
			{
				return Prefs.data.autosavesCount;
			}
			set
			{
				if (Prefs.data.autosavesCount == value)
				{
					return;
				}
				Prefs.data.autosavesCount = value;
				Prefs.Apply();
			}
		}

		// Token: 0x0600572C RID: 22316 RVA: 0x001C0490 File Offset: 0x001BE690
		public static void Init()
		{
			bool flag = !new FileInfo(GenFilePaths.PrefsFilePath).Exists;
			Prefs.data = new PrefsData();
			Prefs.data = DirectXmlLoader.ItemFromXmlFile<PrefsData>(GenFilePaths.PrefsFilePath, true);
			BackCompatibility.PrefsDataPostLoad(Prefs.data);
			if (flag)
			{
				Prefs.data.langFolderName = LanguageDatabase.SystemLanguageFolderName();
				Prefs.data.uiScale = ResolutionUtility.GetRecommendedUIScale(Prefs.data.screenWidth, Prefs.data.screenHeight);
			}
			if (DevModePermanentlyDisabledUtility.Disabled)
			{
				Prefs.DevMode = false;
			}
			Prefs.Apply();
		}

		// Token: 0x0600572D RID: 22317 RVA: 0x001C051C File Offset: 0x001BE71C
		public static void Save()
		{
			try
			{
				XDocument xdocument = new XDocument();
				XElement xelement = DirectXmlSaver.XElementFromObject(Prefs.data, typeof(PrefsData));
				xdocument.Add(xelement);
				xdocument.Save(GenFilePaths.PrefsFilePath);
			}
			catch (Exception ex)
			{
				GenUI.ErrorDialog("ProblemSavingFile".Translate(GenFilePaths.PrefsFilePath, ex.ToString()));
				string text = "Exception saving prefs: ";
				Exception ex2 = ex;
				Log.Error(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x0600572E RID: 22318 RVA: 0x001C05B0 File Offset: 0x001BE7B0
		public static void Apply()
		{
			Prefs.data.Apply();
		}

		// Token: 0x0600572F RID: 22319 RVA: 0x001C05BC File Offset: 0x001BE7BC
		public static void Notify_NewExpansion()
		{
			Prefs.data.backgroundExpansionId = null;
		}

		// Token: 0x06005730 RID: 22320 RVA: 0x001C05CC File Offset: 0x001BE7CC
		public static NameTriple RandomPreferredName()
		{
			string text;
			if (Prefs.PreferredNames.Where<string>((string name) => !name.NullOrEmpty()).TryRandomElement<string>(out text))
			{
				return NameTriple.FromString(text, false);
			}
			return null;
		}

		// Token: 0x04003F45 RID: 16197
		private static PrefsData data;
	}
}
