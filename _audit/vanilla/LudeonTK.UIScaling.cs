using System;
using UnityEngine;
using Verse;

namespace LudeonTK
{
	// Token: 0x020000B1 RID: 177
	public static class UIScaling
	{
		// Token: 0x060003C6 RID: 966 RVA: 0x00016570 File Offset: 0x00014770
		public static Rect AdjustRectToUIScaling(Rect rect)
		{
			Rect rect2 = rect;
			rect2.xMin = UIScaling.AdjustCoordToUIScalingFloor(rect.xMin);
			rect2.yMin = UIScaling.AdjustCoordToUIScalingFloor(rect.yMin);
			rect2.xMax = UIScaling.AdjustCoordToUIScalingCeil(rect.xMax);
			rect2.yMax = UIScaling.AdjustCoordToUIScalingCeil(rect.yMax);
			return rect2;
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x000165CC File Offset: 0x000147CC
		public static float AdjustCoordToUIScalingFloor(float coord)
		{
			double num = (double)(Prefs.UIScale * coord);
			float num2 = (float)(num - Math.Floor(num)) / Prefs.UIScale;
			return coord - num2;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x000165F4 File Offset: 0x000147F4
		public static float AdjustCoordToUIScalingCeil(float coord)
		{
			double num = (double)(Prefs.UIScale * coord);
			float num2 = (float)(num - Math.Ceiling(num)) / Prefs.UIScale;
			return coord - num2;
		}
	}
}
