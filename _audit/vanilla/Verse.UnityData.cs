using System;
using System.Linq;
using System.Threading;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000132 RID: 306
	public static class UnityData
	{
		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000977 RID: 2423 RVA: 0x0002D389 File Offset: 0x0002B589
		public static bool IsInMainThread
		{
			get
			{
				return UnityData.mainThreadId == Thread.CurrentThread.ManagedThreadId;
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000978 RID: 2424 RVA: 0x0002D39C File Offset: 0x0002B59C
		public static bool Is32BitBuild
		{
			get
			{
				return IntPtr.Size == 4;
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000979 RID: 2425 RVA: 0x0002D3A6 File Offset: 0x0002B5A6
		public static bool Is64BitBuild
		{
			get
			{
				return IntPtr.Size == 8;
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x0600097A RID: 2426 RVA: 0x0002D3B0 File Offset: 0x0002B5B0
		public static int MaxJobWorkerThreadCount
		{
			get
			{
				return UnityData.maximumJobWorkerThreads;
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x0600097B RID: 2427 RVA: 0x0002D3B7 File Offset: 0x0002B5B7
		public static int MaxJobWorkerCount
		{
			get
			{
				return UnityData.maximumJobWorkerCount;
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x0600097C RID: 2428 RVA: 0x0002D3BE File Offset: 0x0002B5BE
		public static bool ComputeShadersSupported
		{
			get
			{
				return UnityData.computeShadersSupported;
			}
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0002D3C5 File Offset: 0x0002B5C5
		public static int GetIdealBatchCount(int items)
		{
			return Mathf.Max(items / UnityData.maximumJobWorkerThreads, 4);
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600097E RID: 2430 RVA: 0x0002D3D4 File Offset: 0x0002B5D4
		// (remove) Token: 0x0600097F RID: 2431 RVA: 0x0002D408 File Offset: 0x0002B608
		public static event Action DisposeStatic;

		// Token: 0x06000980 RID: 2432 RVA: 0x0002D43B File Offset: 0x0002B63B
		public static void DisposeStaticResources()
		{
			Action disposeStatic = UnityData.DisposeStatic;
			if (disposeStatic != null)
			{
				disposeStatic();
			}
			UnityData.DisposeStatic = null;
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0002D453 File Offset: 0x0002B653
		static UnityData()
		{
			if (!UnityData.initialized && !UnityDataInitializer.initializing)
			{
				Log.Warning("Used UnityData before it's initialized.");
			}
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x0002D470 File Offset: 0x0002B670
		public static void CopyUnityData()
		{
			UnityData.mainThreadId = Thread.CurrentThread.ManagedThreadId;
			UnityData.isEditor = Application.isEditor;
			UnityData.dataPath = Application.dataPath;
			UnityData.platform = Application.platform;
			UnityData.persistentDataPath = Application.persistentDataPath;
			UnityData.maximumJobWorkerThreads = JobsUtility.ThreadIndexCount + 1;
			UnityData.maximumJobWorkerCount = JobsUtility.JobWorkerCount;
			UnityData.computeShadersSupported = SystemInfo.supportsComputeShaders && SystemInfo.graphicsMemorySize > 5000 && !UnityData.IsIntegratedGraphicsCard() && !Environment.GetCommandLineArgs().Contains("-disable-compute-shaders");
			UnityData.initialized = true;
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0002D504 File Offset: 0x0002B704
		private static bool IsIntegratedGraphicsCard()
		{
			return SystemInfo.graphicsDeviceVendorID == 32902 || SystemInfo.graphicsDeviceVendorID == 4203 || UnityData.IsAmdIntegratedGraphics();
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0002D528 File Offset: 0x0002B728
		private static bool IsAmdIntegratedGraphics()
		{
			if (SystemInfo.graphicsDeviceVendorID != 4098)
			{
				return false;
			}
			string text = SystemInfo.graphicsDeviceName.ToLowerInvariant();
			int num = text.IndexOf("m graphics", StringComparison.InvariantCulture);
			if (num > 1 && char.IsDigit(text[num - 1]))
			{
				return true;
			}
			if (text.Contains("vega"))
			{
				return true;
			}
			if (text.Contains("integrated") || text.Contains("apu"))
			{
				return true;
			}
			if (text.Contains("ryzen"))
			{
				return true;
			}
			string text2 = text.ToLowerInvariant().Trim();
			return text2 == "amd radeon(tm) graphics" || text2 == "amd radeon graphics";
		}

		// Token: 0x040004B5 RID: 1205
		private static bool initialized;

		// Token: 0x040004B6 RID: 1206
		public static bool isEditor;

		// Token: 0x040004B7 RID: 1207
		public static string dataPath;

		// Token: 0x040004B8 RID: 1208
		public static RuntimePlatform platform;

		// Token: 0x040004B9 RID: 1209
		public static string persistentDataPath;

		// Token: 0x040004BA RID: 1210
		private static int mainThreadId;

		// Token: 0x040004BB RID: 1211
		private static int maximumJobWorkerThreads;

		// Token: 0x040004BC RID: 1212
		private static int maximumJobWorkerCount;

		// Token: 0x040004BD RID: 1213
		private static bool computeShadersSupported;
	}
}
