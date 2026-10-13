using System;
using LudeonTK;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000C13 RID: 3091
	[StaticConstructorOnStartup]
	public class TabRecord
	{
		// Token: 0x17000C26 RID: 3110
		// (get) Token: 0x06004EDD RID: 20189 RVA: 0x0019162E File Offset: 0x0018F82E
		public bool Selected
		{
			get
			{
				if (this.selectedGetter == null)
				{
					return this.selected;
				}
				return this.selectedGetter();
			}
		}

		// Token: 0x17000C27 RID: 3111
		// (get) Token: 0x06004EDE RID: 20190 RVA: 0x00002C42 File Offset: 0x00000E42
		public virtual string TutorTag
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06004EDF RID: 20191 RVA: 0x0019164A File Offset: 0x0018F84A
		public TabRecord(string label, Action clickedAction, bool selected)
		{
			this.label = label;
			this.clickedAction = clickedAction;
			this.selected = selected;
		}

		// Token: 0x06004EE0 RID: 20192 RVA: 0x00191672 File Offset: 0x0018F872
		public TabRecord(string label, Action clickedAction, Func<bool> selected)
		{
			this.label = label;
			this.clickedAction = clickedAction;
			this.selectedGetter = selected;
		}

		// Token: 0x06004EE1 RID: 20193 RVA: 0x00002C42 File Offset: 0x00000E42
		public virtual string GetTip()
		{
			return null;
		}

		// Token: 0x06004EE2 RID: 20194 RVA: 0x0019169C File Offset: 0x0018F89C
		public void Draw(Rect rect)
		{
			Rect rect2 = new Rect(rect);
			rect2.width = 30f;
			Rect rect3 = new Rect(rect);
			rect3.width = 30f;
			rect3.x = rect.x + rect.width - 30f;
			Rect rect4 = new Rect(0.53125f, 0f, 0.46875f, 1f);
			Rect rect5 = new Rect(rect);
			rect5.x += rect2.width;
			rect5.width -= 60f;
			rect5.xMin = UIScaling.AdjustCoordToUIScalingFloor(rect5.xMin);
			rect5.xMax = UIScaling.AdjustCoordToUIScalingCeil(rect5.xMax);
			Rect rect6 = new Rect(30f, 0f, 4f, (float)TabRecord.TabAtlas.height).ToUVRect(new Vector2((float)TabRecord.TabAtlas.width, (float)TabRecord.TabAtlas.height));
			Widgets.DrawTexturePart(rect2, new Rect(0f, 0f, 0.46875f, 1f), TabRecord.TabAtlas);
			Widgets.DrawTexturePart(rect5, rect6, TabRecord.TabAtlas);
			Widgets.DrawTexturePart(rect3, rect4, TabRecord.TabAtlas);
			GUI.color = this.labelColor ?? Color.white;
			Rect rect7 = rect;
			rect7.width -= 10f;
			if (Mouse.IsOver(rect7))
			{
				GUI.color = Color.yellow;
				rect7.x += 2f;
				rect7.y -= 2f;
			}
			if (!this.TutorTag.NullOrEmpty())
			{
				UIHighlighter.HighlightOpportunity(rect7, this.TutorTag);
			}
			Text.WordWrap = false;
			Widgets.Label(rect, this.label);
			Text.WordWrap = true;
			GUI.color = Color.white;
			if (!this.Selected)
			{
				Rect rect8 = new Rect(rect);
				rect8.y += rect.height;
				rect8.y -= 1f;
				rect8.height = 1f;
				Rect rect9 = new Rect(0.5f, 0.01f, 0.01f, 0.01f);
				Widgets.DrawTexturePart(rect8, rect9, TabRecord.TabAtlas);
			}
		}

		// Token: 0x0400383B RID: 14395
		public string label = "Tab";

		// Token: 0x0400383C RID: 14396
		public Action clickedAction;

		// Token: 0x0400383D RID: 14397
		public bool selected;

		// Token: 0x0400383E RID: 14398
		public Func<bool> selectedGetter;

		// Token: 0x0400383F RID: 14399
		public Color? labelColor;

		// Token: 0x04003840 RID: 14400
		private const float TabEndWidth = 30f;

		// Token: 0x04003841 RID: 14401
		private const float TabMiddleGraphicWidth = 4f;

		// Token: 0x04003842 RID: 14402
		private static readonly Texture2D TabAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/TabAtlas", true);
	}
}
