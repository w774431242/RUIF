using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Verse.Sound;

namespace Verse
{
	// Token: 0x02000CD6 RID: 3286
	public static class Find
	{
		// Token: 0x17000CEE RID: 3310
		// (get) Token: 0x060053ED RID: 21485 RVA: 0x001B075E File Offset: 0x001AE95E
		public static Root Root
		{
			get
			{
				return Current.Root;
			}
		}

		// Token: 0x17000CEF RID: 3311
		// (get) Token: 0x060053EE RID: 21486 RVA: 0x001B0765 File Offset: 0x001AE965
		public static SoundRoot SoundRoot
		{
			get
			{
				return Current.Root.soundRoot;
			}
		}

		// Token: 0x17000CF0 RID: 3312
		// (get) Token: 0x060053EF RID: 21487 RVA: 0x001B0771 File Offset: 0x001AE971
		public static UIRoot UIRoot
		{
			get
			{
				if (!(Current.Root != null))
				{
					return null;
				}
				return Current.Root.uiRoot;
			}
		}

		// Token: 0x17000CF1 RID: 3313
		// (get) Token: 0x060053F0 RID: 21488 RVA: 0x001B078C File Offset: 0x001AE98C
		public static MusicManagerEntry MusicManagerEntry
		{
			get
			{
				return ((Root_Entry)Current.Root).musicManagerEntry;
			}
		}

		// Token: 0x17000CF2 RID: 3314
		// (get) Token: 0x060053F1 RID: 21489 RVA: 0x001B079D File Offset: 0x001AE99D
		public static MusicManagerPlay MusicManagerPlay
		{
			get
			{
				return ((Root_Play)Current.Root).musicManagerPlay;
			}
		}

		// Token: 0x17000CF3 RID: 3315
		// (get) Token: 0x060053F2 RID: 21490 RVA: 0x001B07AE File Offset: 0x001AE9AE
		public static LanguageWorker ActiveLanguageWorker
		{
			get
			{
				return LanguageDatabase.activeLanguage.Worker;
			}
		}

		// Token: 0x17000CF4 RID: 3316
		// (get) Token: 0x060053F3 RID: 21491 RVA: 0x001B07BA File Offset: 0x001AE9BA
		public static Camera Camera
		{
			get
			{
				return Current.Camera;
			}
		}

		// Token: 0x17000CF5 RID: 3317
		// (get) Token: 0x060053F4 RID: 21492 RVA: 0x001B07C1 File Offset: 0x001AE9C1
		public static CameraDriver CameraDriver
		{
			get
			{
				return Current.CameraDriver;
			}
		}

		// Token: 0x17000CF6 RID: 3318
		// (get) Token: 0x060053F5 RID: 21493 RVA: 0x001B07C8 File Offset: 0x001AE9C8
		public static ColorCorrectionCurves CameraColor
		{
			get
			{
				return Current.ColorCorrectionCurves;
			}
		}

		// Token: 0x17000CF7 RID: 3319
		// (get) Token: 0x060053F6 RID: 21494 RVA: 0x001B07CF File Offset: 0x001AE9CF
		public static Camera PawnCacheCamera
		{
			get
			{
				return PawnCacheCameraManager.PawnCacheCamera;
			}
		}

		// Token: 0x17000CF8 RID: 3320
		// (get) Token: 0x060053F7 RID: 21495 RVA: 0x001B07D6 File Offset: 0x001AE9D6
		public static PawnCacheRenderer PawnCacheRenderer
		{
			get
			{
				return PawnCacheCameraManager.PawnCacheRenderer;
			}
		}

		// Token: 0x17000CF9 RID: 3321
		// (get) Token: 0x060053F8 RID: 21496 RVA: 0x001B07DD File Offset: 0x001AE9DD
		public static Camera WorldCamera
		{
			get
			{
				return WorldCameraManager.WorldCamera;
			}
		}

		// Token: 0x17000CFA RID: 3322
		// (get) Token: 0x060053F9 RID: 21497 RVA: 0x001B07E4 File Offset: 0x001AE9E4
		public static WorldCameraDriver WorldCameraDriver
		{
			get
			{
				return WorldCameraManager.WorldCameraDriver;
			}
		}

		// Token: 0x17000CFB RID: 3323
		// (get) Token: 0x060053FA RID: 21498 RVA: 0x001B07EB File Offset: 0x001AE9EB
		public static WindowStack WindowStack
		{
			get
			{
				UIRoot uiroot = Find.UIRoot;
				if (uiroot == null)
				{
					return null;
				}
				return uiroot.windows;
			}
		}

		// Token: 0x17000CFC RID: 3324
		// (get) Token: 0x060053FB RID: 21499 RVA: 0x001B07FD File Offset: 0x001AE9FD
		public static ScreenshotModeHandler ScreenshotModeHandler
		{
			get
			{
				UIRoot uiroot = Find.UIRoot;
				if (uiroot == null)
				{
					return null;
				}
				return uiroot.screenshotMode;
			}
		}

		// Token: 0x17000CFD RID: 3325
		// (get) Token: 0x060053FC RID: 21500 RVA: 0x001B080F File Offset: 0x001AEA0F
		public static MainButtonsRoot MainButtonsRoot
		{
			get
			{
				return ((UIRoot_Play)Find.UIRoot).mainButtonsRoot;
			}
		}

		// Token: 0x17000CFE RID: 3326
		// (get) Token: 0x060053FD RID: 21501 RVA: 0x001B0820 File Offset: 0x001AEA20
		public static MainTabsRoot MainTabsRoot
		{
			get
			{
				return Find.MainButtonsRoot.tabs;
			}
		}

		// Token: 0x17000CFF RID: 3327
		// (get) Token: 0x060053FE RID: 21502 RVA: 0x001B082C File Offset: 0x001AEA2C
		public static MapInterface MapUI
		{
			get
			{
				return ((UIRoot_Play)Find.UIRoot).mapUI;
			}
		}

		// Token: 0x17000D00 RID: 3328
		// (get) Token: 0x060053FF RID: 21503 RVA: 0x001B083D File Offset: 0x001AEA3D
		public static AlertsReadout Alerts
		{
			get
			{
				return ((UIRoot_Play)Find.UIRoot).alerts;
			}
		}

		// Token: 0x17000D01 RID: 3329
		// (get) Token: 0x06005400 RID: 21504 RVA: 0x001B084E File Offset: 0x001AEA4E
		public static Selector Selector
		{
			get
			{
				return Find.MapUI.selector;
			}
		}

		// Token: 0x17000D02 RID: 3330
		// (get) Token: 0x06005401 RID: 21505 RVA: 0x001B085A File Offset: 0x001AEA5A
		public static Targeter Targeter
		{
			get
			{
				return Find.MapUI.targeter;
			}
		}

		// Token: 0x17000D03 RID: 3331
		// (get) Token: 0x06005402 RID: 21506 RVA: 0x001B0866 File Offset: 0x001AEA66
		public static ColonistBar ColonistBar
		{
			get
			{
				return Find.MapUI.colonistBar;
			}
		}

		// Token: 0x17000D04 RID: 3332
		// (get) Token: 0x06005403 RID: 21507 RVA: 0x001B0872 File Offset: 0x001AEA72
		public static DesignatorManager DesignatorManager
		{
			get
			{
				return Find.MapUI.designatorManager;
			}
		}

		// Token: 0x17000D05 RID: 3333
		// (get) Token: 0x06005404 RID: 21508 RVA: 0x001B087E File Offset: 0x001AEA7E
		public static ReverseDesignatorDatabase ReverseDesignatorDatabase
		{
			get
			{
				return Find.MapUI.reverseDesignatorDatabase;
			}
		}

		// Token: 0x17000D06 RID: 3334
		// (get) Token: 0x06005405 RID: 21509 RVA: 0x001B088A File Offset: 0x001AEA8A
		public static GameInitData GameInitData
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.InitData;
			}
		}

		// Token: 0x17000D07 RID: 3335
		// (get) Token: 0x06005406 RID: 21510 RVA: 0x001B089C File Offset: 0x001AEA9C
		public static GameInfo GameInfo
		{
			get
			{
				return Current.Game.Info;
			}
		}

		// Token: 0x17000D08 RID: 3336
		// (get) Token: 0x06005407 RID: 21511 RVA: 0x001B08A8 File Offset: 0x001AEAA8
		public static Scenario Scenario
		{
			get
			{
				if (Current.Game != null && Current.Game.Scenario != null)
				{
					return Current.Game.Scenario;
				}
				if (ScenarioMaker.GeneratingScenario != null)
				{
					return ScenarioMaker.GeneratingScenario;
				}
				if (Find.UIRoot != null)
				{
					Page_ScenarioEditor page_ScenarioEditor = Find.WindowStack.WindowOfType<Page_ScenarioEditor>();
					if (page_ScenarioEditor != null)
					{
						return page_ScenarioEditor.EditingScenario;
					}
				}
				return null;
			}
		}

		// Token: 0x17000D09 RID: 3337
		// (get) Token: 0x06005408 RID: 21512 RVA: 0x001B08FD File Offset: 0x001AEAFD
		public static World World
		{
			get
			{
				if (Current.Game == null || Current.Game.World == null)
				{
					return Current.CreatingWorld;
				}
				return Current.Game.World;
			}
		}

		// Token: 0x17000D0A RID: 3338
		// (get) Token: 0x06005409 RID: 21513 RVA: 0x001B0922 File Offset: 0x001AEB22
		public static List<Map> Maps
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.Maps;
			}
		}

		// Token: 0x17000D0B RID: 3339
		// (get) Token: 0x0600540A RID: 21514 RVA: 0x001B0934 File Offset: 0x001AEB34
		public static Map CurrentMap
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.CurrentMap;
			}
		}

		// Token: 0x17000D0C RID: 3340
		// (get) Token: 0x0600540B RID: 21515 RVA: 0x001B0946 File Offset: 0x001AEB46
		public static Map AnyPlayerHomeMap
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.AnyPlayerHomeMap;
			}
		}

		// Token: 0x17000D0D RID: 3341
		// (get) Token: 0x0600540C RID: 21516 RVA: 0x001B0958 File Offset: 0x001AEB58
		public static Map RandomPlayerHomeMap
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.RandomPlayerHomeMap;
			}
		}

		// Token: 0x17000D0E RID: 3342
		// (get) Token: 0x0600540D RID: 21517 RVA: 0x001B096A File Offset: 0x001AEB6A
		public static Map RandomRootSurfacePlayerHomeMap
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.RandomRootSurfacePlayerHomeMap;
			}
		}

		// Token: 0x17000D0F RID: 3343
		// (get) Token: 0x0600540E RID: 21518 RVA: 0x001B096A File Offset: 0x001AEB6A
		public static Map RandomSurfacePlayerHomeMap
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.RandomRootSurfacePlayerHomeMap;
			}
		}

		// Token: 0x17000D10 RID: 3344
		// (get) Token: 0x0600540F RID: 21519 RVA: 0x001B097C File Offset: 0x001AEB7C
		public static StoryWatcher StoryWatcher
		{
			get
			{
				return Current.Game.storyWatcher;
			}
		}

		// Token: 0x17000D11 RID: 3345
		// (get) Token: 0x06005410 RID: 21520 RVA: 0x001B0988 File Offset: 0x001AEB88
		public static ResearchManager ResearchManager
		{
			get
			{
				return Current.Game.researchManager;
			}
		}

		// Token: 0x17000D12 RID: 3346
		// (get) Token: 0x06005411 RID: 21521 RVA: 0x001B0994 File Offset: 0x001AEB94
		public static AnalysisManager AnalysisManager
		{
			get
			{
				return Current.Game.analysisManager;
			}
		}

		// Token: 0x17000D13 RID: 3347
		// (get) Token: 0x06005412 RID: 21522 RVA: 0x001B09A0 File Offset: 0x001AEBA0
		public static Storyteller Storyteller
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.storyteller;
			}
		}

		// Token: 0x17000D14 RID: 3348
		// (get) Token: 0x06005413 RID: 21523 RVA: 0x001B09B2 File Offset: 0x001AEBB2
		public static GameEnder GameEnder
		{
			get
			{
				return Current.Game.gameEnder;
			}
		}

		// Token: 0x17000D15 RID: 3349
		// (get) Token: 0x06005414 RID: 21524 RVA: 0x001B09BE File Offset: 0x001AEBBE
		public static LetterStack LetterStack
		{
			get
			{
				return Current.Game.letterStack;
			}
		}

		// Token: 0x17000D16 RID: 3350
		// (get) Token: 0x06005415 RID: 21525 RVA: 0x001B09CA File Offset: 0x001AEBCA
		public static Archive Archive
		{
			get
			{
				History history = Find.History;
				if (history == null)
				{
					return null;
				}
				return history.archive;
			}
		}

		// Token: 0x17000D17 RID: 3351
		// (get) Token: 0x06005416 RID: 21526 RVA: 0x001B09DC File Offset: 0x001AEBDC
		public static PlaySettings PlaySettings
		{
			get
			{
				return Current.Game.playSettings;
			}
		}

		// Token: 0x17000D18 RID: 3352
		// (get) Token: 0x06005417 RID: 21527 RVA: 0x001B09E8 File Offset: 0x001AEBE8
		public static History History
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.history;
			}
		}

		// Token: 0x17000D19 RID: 3353
		// (get) Token: 0x06005418 RID: 21528 RVA: 0x001B09FA File Offset: 0x001AEBFA
		public static TaleManager TaleManager
		{
			get
			{
				return Current.Game.taleManager;
			}
		}

		// Token: 0x17000D1A RID: 3354
		// (get) Token: 0x06005419 RID: 21529 RVA: 0x001B0A06 File Offset: 0x001AEC06
		public static PlayLog PlayLog
		{
			get
			{
				return Current.Game.playLog;
			}
		}

		// Token: 0x17000D1B RID: 3355
		// (get) Token: 0x0600541A RID: 21530 RVA: 0x001B0A12 File Offset: 0x001AEC12
		public static BattleLog BattleLog
		{
			get
			{
				return Current.Game.battleLog;
			}
		}

		// Token: 0x17000D1C RID: 3356
		// (get) Token: 0x0600541B RID: 21531 RVA: 0x001B0A1E File Offset: 0x001AEC1E
		public static TickManager TickManager
		{
			get
			{
				return Current.Game.tickManager;
			}
		}

		// Token: 0x17000D1D RID: 3357
		// (get) Token: 0x0600541C RID: 21532 RVA: 0x001B0A2A File Offset: 0x001AEC2A
		public static Tutor Tutor
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.tutor;
			}
		}

		// Token: 0x17000D1E RID: 3358
		// (get) Token: 0x0600541D RID: 21533 RVA: 0x001B0A3C File Offset: 0x001AEC3C
		public static TutorialState TutorialState
		{
			get
			{
				return Current.Game.tutor.tutorialState;
			}
		}

		// Token: 0x17000D1F RID: 3359
		// (get) Token: 0x0600541E RID: 21534 RVA: 0x001B0A4D File Offset: 0x001AEC4D
		public static ActiveLessonHandler ActiveLesson
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.tutor.activeLesson;
			}
		}

		// Token: 0x17000D20 RID: 3360
		// (get) Token: 0x0600541F RID: 21535 RVA: 0x001B0A64 File Offset: 0x001AEC64
		public static Autosaver Autosaver
		{
			get
			{
				return Current.Game.autosaver;
			}
		}

		// Token: 0x17000D21 RID: 3361
		// (get) Token: 0x06005420 RID: 21536 RVA: 0x001B0A70 File Offset: 0x001AEC70
		public static DateNotifier DateNotifier
		{
			get
			{
				return Current.Game.dateNotifier;
			}
		}

		// Token: 0x17000D22 RID: 3362
		// (get) Token: 0x06005421 RID: 21537 RVA: 0x001B0A7C File Offset: 0x001AEC7C
		public static SignalManager SignalManager
		{
			get
			{
				return Current.Game.signalManager;
			}
		}

		// Token: 0x17000D23 RID: 3363
		// (get) Token: 0x06005422 RID: 21538 RVA: 0x001B0A88 File Offset: 0x001AEC88
		public static UniqueIDsManager UniqueIDsManager
		{
			get
			{
				Game game = Current.Game;
				if (game == null)
				{
					return null;
				}
				return game.uniqueIDsManager;
			}
		}

		// Token: 0x17000D24 RID: 3364
		// (get) Token: 0x06005423 RID: 21539 RVA: 0x001B0A9A File Offset: 0x001AEC9A
		public static QuestManager QuestManager
		{
			get
			{
				return Current.Game.questManager;
			}
		}

		// Token: 0x17000D25 RID: 3365
		// (get) Token: 0x06005424 RID: 21540 RVA: 0x001B0AA6 File Offset: 0x001AECA6
		public static RelationshipRecords RelationshipRecords
		{
			get
			{
				return Current.Game.relationshipRecords;
			}
		}

		// Token: 0x17000D26 RID: 3366
		// (get) Token: 0x06005425 RID: 21541 RVA: 0x001B0AB2 File Offset: 0x001AECB2
		public static TransportShipManager TransportShipManager
		{
			get
			{
				return Current.Game.transportShipManager;
			}
		}

		// Token: 0x17000D27 RID: 3367
		// (get) Token: 0x06005426 RID: 21542 RVA: 0x001B0ABE File Offset: 0x001AECBE
		public static GameComponent_Bossgroup BossgroupManager
		{
			get
			{
				return Current.Game.GetComponent<GameComponent_Bossgroup>();
			}
		}

		// Token: 0x17000D28 RID: 3368
		// (get) Token: 0x06005427 RID: 21543 RVA: 0x001B0ACA File Offset: 0x001AECCA
		public static GameComponent_Anomaly Anomaly
		{
			get
			{
				GameComponent_Anomaly gameComponent_Anomaly;
				if ((gameComponent_Anomaly = Find.cachedAnomaly) == null)
				{
					gameComponent_Anomaly = (Find.cachedAnomaly = Current.Game.GetComponent<GameComponent_Anomaly>());
				}
				return gameComponent_Anomaly;
			}
		}

		// Token: 0x17000D29 RID: 3369
		// (get) Token: 0x06005428 RID: 21544 RVA: 0x001B0AE5 File Offset: 0x001AECE5
		public static StudyManager StudyManager
		{
			get
			{
				return Current.Game.studyManager;
			}
		}

		// Token: 0x17000D2A RID: 3370
		// (get) Token: 0x06005429 RID: 21545 RVA: 0x001B0AF1 File Offset: 0x001AECF1
		public static CustomXenogermDatabase CustomXenogermDatabase
		{
			get
			{
				return Current.Game.customXenogermDatabase;
			}
		}

		// Token: 0x17000D2B RID: 3371
		// (get) Token: 0x0600542A RID: 21546 RVA: 0x001B0AFD File Offset: 0x001AECFD
		public static GameComponent_PsychicRitualManager PsychicRitualManager
		{
			get
			{
				return Current.Game.GetComponent<GameComponent_PsychicRitualManager>();
			}
		}

		// Token: 0x17000D2C RID: 3372
		// (get) Token: 0x0600542B RID: 21547 RVA: 0x001B0B09 File Offset: 0x001AED09
		public static GameComponent_PawnDuplicator PawnDuplicator
		{
			get
			{
				return Current.Game.GetComponent<GameComponent_PawnDuplicator>();
			}
		}

		// Token: 0x17000D2D RID: 3373
		// (get) Token: 0x0600542C RID: 21548 RVA: 0x001B0B15 File Offset: 0x001AED15
		public static HiddenItemsManager HiddenItemsManager
		{
			get
			{
				return Current.Game.hiddenItemsManager;
			}
		}

		// Token: 0x17000D2E RID: 3374
		// (get) Token: 0x0600542D RID: 21549 RVA: 0x001B0B21 File Offset: 0x001AED21
		public static EntityCodex EntityCodex
		{
			get
			{
				return Current.Game.entityCodex;
			}
		}

		// Token: 0x17000D2F RID: 3375
		// (get) Token: 0x0600542E RID: 21550 RVA: 0x001B0B2D File Offset: 0x001AED2D
		public static Gravship CurrentGravship
		{
			get
			{
				return Current.Game.Gravship;
			}
		}

		// Token: 0x17000D30 RID: 3376
		// (get) Token: 0x0600542F RID: 21551 RVA: 0x001B0B39 File Offset: 0x001AED39
		public static FactionManager FactionManager
		{
			get
			{
				return Find.World.factionManager;
			}
		}

		// Token: 0x17000D31 RID: 3377
		// (get) Token: 0x06005430 RID: 21552 RVA: 0x001B0B45 File Offset: 0x001AED45
		public static IdeoManager IdeoManager
		{
			get
			{
				return Find.World.ideoManager;
			}
		}

		// Token: 0x17000D32 RID: 3378
		// (get) Token: 0x06005431 RID: 21553 RVA: 0x001B0B51 File Offset: 0x001AED51
		public static WorldPawns WorldPawns
		{
			get
			{
				return Find.World.worldPawns;
			}
		}

		// Token: 0x17000D33 RID: 3379
		// (get) Token: 0x06005432 RID: 21554 RVA: 0x001B0B5D File Offset: 0x001AED5D
		public static WorldObjectsHolder WorldObjects
		{
			get
			{
				return Find.World.worldObjects;
			}
		}

		// Token: 0x17000D34 RID: 3380
		// (get) Token: 0x06005433 RID: 21555 RVA: 0x001B0B69 File Offset: 0x001AED69
		public static WorldGrid WorldGrid
		{
			get
			{
				return Find.World.grid;
			}
		}

		// Token: 0x17000D35 RID: 3381
		// (get) Token: 0x06005434 RID: 21556 RVA: 0x001B0B75 File Offset: 0x001AED75
		public static WorldDebugDrawer WorldDebugDrawer
		{
			get
			{
				return Find.World.debugDrawer;
			}
		}

		// Token: 0x17000D36 RID: 3382
		// (get) Token: 0x06005435 RID: 21557 RVA: 0x001B0B81 File Offset: 0x001AED81
		public static WorldPathGrid WorldPathGrid
		{
			get
			{
				return Find.World.pathGrid;
			}
		}

		// Token: 0x17000D37 RID: 3383
		// (get) Token: 0x06005436 RID: 21558 RVA: 0x001B0B8D File Offset: 0x001AED8D
		public static WorldDynamicDrawManager WorldDynamicDrawManager
		{
			get
			{
				return Find.World.dynamicDrawManager;
			}
		}

		// Token: 0x17000D38 RID: 3384
		// (get) Token: 0x06005437 RID: 21559 RVA: 0x001B0B99 File Offset: 0x001AED99
		public static WorldPathPool WorldPathPool
		{
			get
			{
				return Find.World.pathPool;
			}
		}

		// Token: 0x17000D39 RID: 3385
		// (get) Token: 0x06005438 RID: 21560 RVA: 0x001B0BA5 File Offset: 0x001AEDA5
		public static WorldReachability WorldReachability
		{
			get
			{
				return Find.World.reachability;
			}
		}

		// Token: 0x17000D3A RID: 3386
		// (get) Token: 0x06005439 RID: 21561 RVA: 0x001B0BB1 File Offset: 0x001AEDB1
		public static WorldFeatures WorldFeatures
		{
			get
			{
				return Find.World.features;
			}
		}

		// Token: 0x17000D3B RID: 3387
		// (get) Token: 0x0600543A RID: 21562 RVA: 0x001B0BBD File Offset: 0x001AEDBD
		public static WorldComponent_GravshipController GravshipController
		{
			get
			{
				return Find.World.GetComponent<WorldComponent_GravshipController>();
			}
		}

		// Token: 0x17000D3C RID: 3388
		// (get) Token: 0x0600543B RID: 21563 RVA: 0x001B0BC9 File Offset: 0x001AEDC9
		public static WorldInterface WorldInterface
		{
			get
			{
				return Find.World.UI;
			}
		}

		// Token: 0x17000D3D RID: 3389
		// (get) Token: 0x0600543C RID: 21564 RVA: 0x001B0BD5 File Offset: 0x001AEDD5
		public static WorldSelector WorldSelector
		{
			get
			{
				return Find.WorldInterface.selector;
			}
		}

		// Token: 0x17000D3E RID: 3390
		// (get) Token: 0x0600543D RID: 21565 RVA: 0x001B0BE1 File Offset: 0x001AEDE1
		public static WorldTargeter WorldTargeter
		{
			get
			{
				return Find.WorldInterface.targeter;
			}
		}

		// Token: 0x17000D3F RID: 3391
		// (get) Token: 0x0600543E RID: 21566 RVA: 0x001B0BED File Offset: 0x001AEDED
		public static WorldRoutePlanner WorldRoutePlanner
		{
			get
			{
				return Find.WorldInterface.routePlanner;
			}
		}

		// Token: 0x17000D40 RID: 3392
		// (get) Token: 0x0600543F RID: 21567 RVA: 0x001B0BF9 File Offset: 0x001AEDF9
		public static TilePicker TilePicker
		{
			get
			{
				return Find.WorldInterface.tilePicker;
			}
		}

		// Token: 0x17000D41 RID: 3393
		// (get) Token: 0x06005440 RID: 21568 RVA: 0x001B0C05 File Offset: 0x001AEE05
		public static HistoryEventsManager HistoryEventsManager
		{
			get
			{
				return Find.History.historyEventsManager;
			}
		}

		// Token: 0x17000D42 RID: 3394
		// (get) Token: 0x06005441 RID: 21569 RVA: 0x001B0C11 File Offset: 0x001AEE11
		public static GoodwillSituationManager GoodwillSituationManager
		{
			get
			{
				return Find.FactionManager.goodwillSituationManager;
			}
		}

		// Token: 0x06005442 RID: 21570 RVA: 0x001B0C1D File Offset: 0x001AEE1D
		public static void ClearCache()
		{
			Find.cachedAnomaly = null;
		}

		// Token: 0x04003CAB RID: 15531
		private static GameComponent_Anomaly cachedAnomaly;
	}
}
