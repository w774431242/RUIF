using System;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000C06 RID: 3078
	public static class Mouse
	{
		// Token: 0x17000C20 RID: 3104
		// (get) Token: 0x06004EB6 RID: 20150 RVA: 0x00190684 File Offset: 0x0018E884
		public static bool IsInputBlockedNow
		{
			get
			{
				WindowStack windowStack = Find.WindowStack;
				return (Widgets.mouseOverScrollViewStack.Count > 0 && !Widgets.mouseOverScrollViewStack.Peek()) || windowStack.MouseObscuredNow || !windowStack.CurrentWindowGetsInput;
			}
		}

		// Token: 0x06004EB7 RID: 20151 RVA: 0x001906C7 File Offset: 0x0018E8C7
		public static bool IsOver(Rect rect)
		{
			return rect.Contains(Event.current.mousePosition) && !Mouse.IsInputBlockedNow;
		}
	}
}
