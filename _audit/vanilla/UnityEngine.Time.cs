using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	// Token: 0x0200028C RID: 652
	[NativeHeader("Runtime/Input/TimeManager.h")]
	[StaticAccessor("GetTimeManager()", StaticAccessorType.Dot)]
	public class Time
	{
		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06001BA1 RID: 7073
		[NativeProperty("CurTime")]
		public static extern float time
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06001BA2 RID: 7074
		[NativeProperty("CurTime")]
		public static extern double timeAsDouble
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06001BA3 RID: 7075
		[NativeProperty("TimeSinceSceneLoad")]
		public static extern float timeSinceLevelLoad
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06001BA4 RID: 7076
		[NativeProperty("TimeSinceSceneLoad")]
		public static extern double timeSinceLevelLoadAsDouble
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06001BA5 RID: 7077
		public static extern float deltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06001BA6 RID: 7078
		public static extern float fixedTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06001BA7 RID: 7079
		[NativeProperty("FixedTime")]
		public static extern double fixedTimeAsDouble
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x06001BA8 RID: 7080
		public static extern float unscaledTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x06001BA9 RID: 7081
		[NativeProperty("UnscaledTime")]
		public static extern double unscaledTimeAsDouble
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06001BAA RID: 7082
		public static extern float fixedUnscaledTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06001BAB RID: 7083
		[NativeProperty("FixedUnscaledTime")]
		public static extern double fixedUnscaledTimeAsDouble
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06001BAC RID: 7084
		public static extern float unscaledDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06001BAD RID: 7085
		public static extern float fixedUnscaledDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06001BAE RID: 7086
		// (set) Token: 0x06001BAF RID: 7087
		public static extern float fixedDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06001BB0 RID: 7088
		// (set) Token: 0x06001BB1 RID: 7089
		public static extern float maximumDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06001BB2 RID: 7090
		public static extern float smoothDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06001BB3 RID: 7091
		// (set) Token: 0x06001BB4 RID: 7092
		public static extern float maximumParticleDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06001BB5 RID: 7093
		// (set) Token: 0x06001BB6 RID: 7094
		public static extern float timeScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x06001BB7 RID: 7095
		public static extern int frameCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06001BB8 RID: 7096
		[NativeProperty("RenderFrameCount")]
		public static extern int renderedFrameCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06001BB9 RID: 7097
		[NativeProperty("Realtime")]
		public static extern float realtimeSinceStartup
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06001BBA RID: 7098
		[NativeProperty("Realtime")]
		public static extern double realtimeSinceStartupAsDouble
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06001BBB RID: 7099
		// (set) Token: 0x06001BBC RID: 7100
		public static extern float captureDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06001BBD RID: 7101 RVA: 0x0002DFD4 File Offset: 0x0002C1D4
		// (set) Token: 0x06001BBE RID: 7102 RVA: 0x0002E006 File Offset: 0x0002C206
		public static int captureFramerate
		{
			get
			{
				return (Time.captureDeltaTime == 0f) ? 0 : ((int)Mathf.Round(1f / Time.captureDeltaTime));
			}
			set
			{
				Time.captureDeltaTime = ((value == 0) ? 0f : (1f / (float)value));
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06001BBF RID: 7103
		public static extern bool inFixedTimeStep
		{
			[NativeName("IsUsingFixedTimeStep")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
