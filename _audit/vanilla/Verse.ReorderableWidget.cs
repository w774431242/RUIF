using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse
{
	// Token: 0x02000C0E RID: 3086
	public static class ReorderableWidget
	{
		// Token: 0x17000C22 RID: 3106
		// (get) Token: 0x06004EC9 RID: 20169 RVA: 0x00190A90 File Offset: 0x0018EC90
		public static bool Dragging
		{
			get
			{
				return ReorderableWidget.dragBegun;
			}
		}

		// Token: 0x17000C23 RID: 3107
		// (get) Token: 0x06004ECA RID: 20170 RVA: 0x00190A97 File Offset: 0x0018EC97
		public static int GetDraggedIndex
		{
			get
			{
				return ReorderableWidget.GetIndexWithinGroup(ReorderableWidget.draggingReorderable);
			}
		}

		// Token: 0x17000C24 RID: 3108
		// (get) Token: 0x06004ECB RID: 20171 RVA: 0x00190AA3 File Offset: 0x0018ECA3
		public static int GetDraggedFromGroupID
		{
			get
			{
				return ReorderableWidget.reorderables[ReorderableWidget.draggingReorderable].groupID;
			}
		}

		// Token: 0x06004ECC RID: 20172 RVA: 0x00190ABC File Offset: 0x0018ECBC
		public static void ReorderableWidgetOnGUI_BeforeWindowStack()
		{
			if (ReorderableWidget.dragBegun && ReorderableWidget.draggingReorderable >= 0 && ReorderableWidget.draggingReorderable < ReorderableWidget.reorderables.Count)
			{
				int groupID = ReorderableWidget.reorderables[ReorderableWidget.draggingReorderable].groupID;
				if (groupID >= 0 && groupID < ReorderableWidget.groups.Count && ReorderableWidget.groups[groupID].extraDraggedItemOnGUI != null)
				{
					ReorderableWidget.groups[groupID].extraDraggedItemOnGUI(ReorderableWidget.GetIndexWithinGroup(ReorderableWidget.draggingReorderable), ReorderableWidget.dragStartPos);
				}
			}
		}

		// Token: 0x06004ECD RID: 20173 RVA: 0x00190B48 File Offset: 0x0018ED48
		public static void ReorderableWidgetOnGUI_AfterWindowStack()
		{
			if (Event.current.rawType == EventType.MouseUp)
			{
				ReorderableWidget.released = true;
			}
			if (Event.current.type == EventType.Repaint)
			{
				if (ReorderableWidget.clicked)
				{
					ReorderableWidget.StopDragging();
					for (int i = 0; i < ReorderableWidget.reorderables.Count; i++)
					{
						if (ReorderableWidget.reorderables[i].groupID == ReorderableWidget.groupClicked && ReorderableWidget.reorderables[i].rect == ReorderableWidget.clickedInRect)
						{
							ReorderableWidget.draggingReorderable = i;
							ReorderableWidget.dragStartPos = Event.current.mousePosition;
							break;
						}
					}
					ReorderableWidget.clicked = false;
				}
				if (ReorderableWidget.draggingReorderable >= ReorderableWidget.reorderables.Count)
				{
					ReorderableWidget.StopDragging();
				}
				if (ReorderableWidget.reorderables.Count != ReorderableWidget.lastFrameReorderableCount)
				{
					ReorderableWidget.StopDragging();
				}
				ReorderableWidget.lastInsertNear = ReorderableWidget.CurrentInsertNear(out ReorderableWidget.lastInsertNearLeft);
				ReorderableWidget.hoveredGroup = -1;
				for (int j = 0; j < ReorderableWidget.groups.Count; j++)
				{
					if (ReorderableWidget.groups[j].absRect.Contains(Event.current.mousePosition))
					{
						ReorderableWidget.hoveredGroup = j;
						if (ReorderableWidget.lastInsertNear >= 0 && ReorderableWidget.AreInMultiGroup(j, ReorderableWidget.reorderables[ReorderableWidget.lastInsertNear].groupID) && ReorderableWidget.reorderables[ReorderableWidget.lastInsertNear].groupID != j)
						{
							ReorderableWidget.lastInsertNear = ReorderableWidget.FindLastReorderableIndexWithinGroup(j);
							ReorderableWidget.lastInsertNearLeft = ReorderableWidget.lastInsertNear < 0;
						}
					}
				}
				if (ReorderableWidget.released)
				{
					ReorderableWidget.released = false;
					if (ReorderableWidget.dragBegun && ReorderableWidget.draggingReorderable >= 0)
					{
						int indexWithinGroup = ReorderableWidget.GetIndexWithinGroup(ReorderableWidget.draggingReorderable);
						int groupID = ReorderableWidget.reorderables[ReorderableWidget.draggingReorderable].groupID;
						int num;
						if (ReorderableWidget.lastInsertNear == ReorderableWidget.draggingReorderable)
						{
							num = indexWithinGroup;
						}
						else if (ReorderableWidget.lastInsertNearLeft)
						{
							num = ReorderableWidget.GetIndexWithinGroup(ReorderableWidget.lastInsertNear);
						}
						else
						{
							num = ReorderableWidget.GetIndexWithinGroup(ReorderableWidget.lastInsertNear) + 1;
						}
						int num2 = -1;
						if (ReorderableWidget.lastInsertNear >= 0)
						{
							num2 = ReorderableWidget.reorderables[ReorderableWidget.lastInsertNear].groupID;
						}
						if (ReorderableWidget.AreInMultiGroup(groupID, ReorderableWidget.hoveredGroup) && ReorderableWidget.hoveredGroup >= 0 && ReorderableWidget.hoveredGroup != num2)
						{
							num2 = ReorderableWidget.hoveredGroup;
							num = ReorderableWidget.GetIndexWithinGroup(ReorderableWidget.FindLastReorderableIndexWithinGroup(num2)) + 1;
						}
						if (ReorderableWidget.AreInMultiGroup(groupID, num2))
						{
							ReorderableWidget.GetMultiGroupByGroupID(groupID).Value.reorderedAction(indexWithinGroup, groupID, num, num2);
							SoundDefOf.DropElement.PlayOneShotOnCamera(null);
						}
						else if (num >= 0 && num != indexWithinGroup && num != indexWithinGroup + 1)
						{
							SoundDefOf.DropElement.PlayOneShotOnCamera(null);
							try
							{
								ReorderableWidget.groups[ReorderableWidget.reorderables[ReorderableWidget.draggingReorderable].groupID].reorderedAction(indexWithinGroup, num);
							}
							catch (Exception ex)
							{
								string[] array = new string[6];
								array[0] = "Could not reorder elements (from ";
								array[1] = indexWithinGroup.ToString();
								array[2] = " to ";
								array[3] = num.ToString();
								array[4] = "): ";
								int num3 = 5;
								Exception ex2 = ex;
								array[num3] = ((ex2 != null) ? ex2.ToString() : null);
								Log.Error(string.Concat(array));
							}
						}
					}
					ReorderableWidget.StopDragging();
				}
				ReorderableWidget.lastFrameReorderableCount = ReorderableWidget.reorderables.Count;
				ReorderableWidget.multiGroups.Clear();
				ReorderableWidget.groups.Clear();
				ReorderableWidget.reorderables.Clear();
			}
		}

		// Token: 0x06004ECE RID: 20174 RVA: 0x00190EB4 File Offset: 0x0018F0B4
		public static int NewGroup(Action<int, int> reorderedAction, ReorderableDirection direction, Rect rect, float drawLineExactlyBetween_space = -1f, Action<int, Vector2> extraDraggedItemOnGUI = null, bool playSoundOnStartReorder = true)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return -1;
			}
			int count = ReorderableWidget.groups.Count;
			ReorderableWidget.ReorderableGroup reorderableGroup = default(ReorderableWidget.ReorderableGroup);
			reorderableGroup.reorderedAction = reorderedAction;
			reorderableGroup.direction = direction;
			reorderableGroup.absRect = new Rect(UI.GUIToScreenPoint(Vector2.zero), rect.size);
			reorderableGroup.drawLineExactlyBetween_space = drawLineExactlyBetween_space;
			reorderableGroup.extraDraggedItemOnGUI = extraDraggedItemOnGUI;
			reorderableGroup.playSoundOnStartReorder = playSoundOnStartReorder;
			ReorderableWidget.groups.Add(reorderableGroup);
			if (ReorderableWidget.draggingReorderable >= 0 && ReorderableWidget.hoveredGroup == count && ReorderableWidget.lastInsertNear == -1)
			{
				ReorderableWidget.DrawLine(count, new Rect(0f, 0f, rect.width, rect.height));
			}
			return count;
		}

		// Token: 0x06004ECF RID: 20175 RVA: 0x00190F74 File Offset: 0x0018F174
		public static int NewMultiGroup(List<int> includedGroups, Action<int, int, int, int> reorderedAction)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return -1;
			}
			ReorderableWidget.ReorderableMultiGroup reorderableMultiGroup = default(ReorderableWidget.ReorderableMultiGroup);
			reorderableMultiGroup.includedGroups = includedGroups;
			reorderableMultiGroup.reorderedAction = reorderedAction;
			ReorderableWidget.multiGroups.Add(reorderableMultiGroup);
			return ReorderableWidget.multiGroups.Count - 1;
		}

		// Token: 0x06004ED0 RID: 20176 RVA: 0x00190FC0 File Offset: 0x0018F1C0
		public static bool Reorderable(int groupID, Rect rect, bool useRightButton = false, bool highlightDragged = true)
		{
			if (Event.current.type == EventType.Repaint)
			{
				ReorderableWidget.ReorderableInstance reorderableInstance = default(ReorderableWidget.ReorderableInstance);
				reorderableInstance.groupID = groupID;
				reorderableInstance.rect = rect;
				reorderableInstance.absRect = new Rect(UI.GUIToScreenPoint(rect.position), rect.size);
				ReorderableWidget.reorderables.Add(reorderableInstance);
				int num = ReorderableWidget.reorderables.Count - 1;
				if (ReorderableWidget.draggingReorderable != -1 && (ReorderableWidget.dragBegun || (Vector2.Distance(ReorderableWidget.clickedAt, Event.current.mousePosition) > 5f && ReorderableWidget.groupClicked == groupID)))
				{
					if (!ReorderableWidget.dragBegun)
					{
						if (groupID >= 0 && groupID < ReorderableWidget.groups.Count && ReorderableWidget.groups[groupID].playSoundOnStartReorder)
						{
							SoundDefOf.DragElement.PlayOneShotOnCamera(null);
						}
						ReorderableWidget.dragBegun = true;
					}
					if (highlightDragged && ReorderableWidget.draggingReorderable == num)
					{
						GUI.color = ReorderableWidget.HighlightColor;
						Widgets.DrawHighlight(rect);
						GUI.color = Color.white;
					}
					if (ReorderableWidget.lastInsertNear == num && groupID >= 0 && groupID < ReorderableWidget.groups.Count)
					{
						Rect rect2 = ReorderableWidget.reorderables[ReorderableWidget.lastInsertNear].rect;
						ReorderableWidget.DrawLine(groupID, rect2);
					}
				}
				return ReorderableWidget.draggingReorderable == num && ReorderableWidget.dragBegun;
			}
			if (Event.current.rawType == EventType.MouseUp)
			{
				ReorderableWidget.released = true;
			}
			if (Event.current.type == EventType.MouseDown && ((useRightButton && Event.current.button == 1) || (!useRightButton && Event.current.button == 0)) && Mouse.IsOver(rect))
			{
				ReorderableWidget.clicked = true;
				ReorderableWidget.clickedAt = Event.current.mousePosition;
				ReorderableWidget.groupClicked = groupID;
				ReorderableWidget.clickedInRect = rect;
			}
			return false;
		}

		// Token: 0x06004ED1 RID: 20177 RVA: 0x00191174 File Offset: 0x0018F374
		private static int CurrentInsertNear(out bool toTheLeft)
		{
			toTheLeft = false;
			if (ReorderableWidget.draggingReorderable < 0)
			{
				return -1;
			}
			int groupID = ReorderableWidget.reorderables[ReorderableWidget.draggingReorderable].groupID;
			if (groupID < 0 || groupID >= ReorderableWidget.groups.Count)
			{
				Log.ErrorOnce("Reorderable used invalid group.", 1968375560);
				return -1;
			}
			int num = -1;
			for (int i = 0; i < ReorderableWidget.reorderables.Count; i++)
			{
				ReorderableWidget.ReorderableInstance reorderableInstance = ReorderableWidget.reorderables[i];
				if ((reorderableInstance.groupID == groupID || ReorderableWidget.AreInMultiGroup(reorderableInstance.groupID, groupID)) && (num == -1 || Event.current.mousePosition.DistanceToRect(reorderableInstance.absRect) < Event.current.mousePosition.DistanceToRect(ReorderableWidget.reorderables[num].absRect)))
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				ReorderableWidget.ReorderableInstance reorderableInstance2 = ReorderableWidget.reorderables[num];
				if (ReorderableWidget.groups[reorderableInstance2.groupID].direction == ReorderableDirection.Horizontal)
				{
					toTheLeft = Event.current.mousePosition.x < reorderableInstance2.absRect.center.x;
				}
				else
				{
					toTheLeft = Event.current.mousePosition.y < reorderableInstance2.absRect.center.y;
				}
			}
			return num;
		}

		// Token: 0x06004ED2 RID: 20178 RVA: 0x001912B0 File Offset: 0x0018F4B0
		private static void DrawLine(int groupID, Rect r)
		{
			ReorderableWidget.ReorderableGroup reorderableGroup = ReorderableWidget.groups[groupID];
			if (reorderableGroup.DrawLineExactlyBetween)
			{
				if (reorderableGroup.direction == ReorderableDirection.Horizontal)
				{
					r.xMin -= reorderableGroup.drawLineExactlyBetween_space / 2f;
					r.xMax += reorderableGroup.drawLineExactlyBetween_space / 2f;
				}
				else
				{
					r.yMin -= reorderableGroup.drawLineExactlyBetween_space / 2f;
					r.yMax += reorderableGroup.drawLineExactlyBetween_space / 2f;
				}
			}
			GUI.color = ReorderableWidget.LineColor;
			if (reorderableGroup.direction == ReorderableDirection.Horizontal)
			{
				if (ReorderableWidget.lastInsertNearLeft)
				{
					Widgets.DrawLine(r.position, new Vector2(r.x, r.yMax), ReorderableWidget.LineColor, 2f);
				}
				else
				{
					Widgets.DrawLine(new Vector2(r.xMax, r.y), new Vector2(r.xMax, r.yMax), ReorderableWidget.LineColor, 2f);
				}
			}
			else if (ReorderableWidget.lastInsertNearLeft)
			{
				Widgets.DrawLine(r.position, new Vector2(r.xMax, r.y), ReorderableWidget.LineColor, 2f);
			}
			else
			{
				Widgets.DrawLine(new Vector2(r.x, r.yMax), new Vector2(r.xMax, r.yMax), ReorderableWidget.LineColor, 2f);
			}
			GUI.color = Color.white;
		}

		// Token: 0x06004ED3 RID: 20179 RVA: 0x00191434 File Offset: 0x0018F634
		private static int GetIndexWithinGroup(int index)
		{
			if (index < 0 || index >= ReorderableWidget.reorderables.Count)
			{
				return -1;
			}
			int num = -1;
			for (int i = 0; i <= index; i++)
			{
				if (ReorderableWidget.reorderables[i].groupID == ReorderableWidget.reorderables[index].groupID)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06004ED4 RID: 20180 RVA: 0x0019148C File Offset: 0x0018F68C
		private static int FindLastReorderableIndexWithinGroup(int groupID)
		{
			if (groupID < 0 || groupID >= ReorderableWidget.groups.Count)
			{
				return -1;
			}
			int num = -1;
			for (int i = 0; i < ReorderableWidget.reorderables.Count; i++)
			{
				if (ReorderableWidget.reorderables[i].groupID == groupID)
				{
					num = i;
				}
			}
			return num;
		}

		// Token: 0x06004ED5 RID: 20181 RVA: 0x001914DC File Offset: 0x0018F6DC
		private static ReorderableWidget.ReorderableMultiGroup? GetMultiGroupByGroupID(int groupID)
		{
			foreach (ReorderableWidget.ReorderableMultiGroup reorderableMultiGroup in ReorderableWidget.multiGroups)
			{
				if (reorderableMultiGroup.includedGroups.Contains(groupID))
				{
					return new ReorderableWidget.ReorderableMultiGroup?(reorderableMultiGroup);
				}
			}
			return null;
		}

		// Token: 0x06004ED6 RID: 20182 RVA: 0x0019154C File Offset: 0x0018F74C
		private static bool AreInMultiGroup(int groupA, int groupB)
		{
			ReorderableWidget.ReorderableMultiGroup? multiGroupByGroupID = ReorderableWidget.GetMultiGroupByGroupID(groupA);
			return multiGroupByGroupID != null && groupA != groupB && multiGroupByGroupID.Value.includedGroups.Contains(groupB);
		}

		// Token: 0x06004ED7 RID: 20183 RVA: 0x00191581 File Offset: 0x0018F781
		private static void StopDragging()
		{
			ReorderableWidget.draggingReorderable = -1;
			ReorderableWidget.dragStartPos = default(Vector2);
			ReorderableWidget.lastInsertNear = -1;
			ReorderableWidget.dragBegun = false;
		}

		// Token: 0x0400381D RID: 14365
		private static List<ReorderableWidget.ReorderableGroup> groups = new List<ReorderableWidget.ReorderableGroup>();

		// Token: 0x0400381E RID: 14366
		private static List<ReorderableWidget.ReorderableMultiGroup> multiGroups = new List<ReorderableWidget.ReorderableMultiGroup>();

		// Token: 0x0400381F RID: 14367
		private static List<ReorderableWidget.ReorderableInstance> reorderables = new List<ReorderableWidget.ReorderableInstance>();

		// Token: 0x04003820 RID: 14368
		private static int draggingReorderable = -1;

		// Token: 0x04003821 RID: 14369
		private static Vector2 dragStartPos;

		// Token: 0x04003822 RID: 14370
		private static bool clicked;

		// Token: 0x04003823 RID: 14371
		private static bool released;

		// Token: 0x04003824 RID: 14372
		private static bool dragBegun;

		// Token: 0x04003825 RID: 14373
		private static Vector2 clickedAt;

		// Token: 0x04003826 RID: 14374
		private static int groupClicked;

		// Token: 0x04003827 RID: 14375
		private static Rect clickedInRect;

		// Token: 0x04003828 RID: 14376
		private static int lastInsertNear = -1;

		// Token: 0x04003829 RID: 14377
		private static int hoveredGroup = -1;

		// Token: 0x0400382A RID: 14378
		private static bool lastInsertNearLeft;

		// Token: 0x0400382B RID: 14379
		private static int lastFrameReorderableCount = -1;

		// Token: 0x0400382C RID: 14380
		private const float MinMouseMoveToHighlightReorderable = 5f;

		// Token: 0x0400382D RID: 14381
		private static readonly Color LineColor = new Color(1f, 1f, 1f, 0.6f);

		// Token: 0x0400382E RID: 14382
		private static readonly Color HighlightColor = new Color(1f, 1f, 1f, 0.3f);

		// Token: 0x0400382F RID: 14383
		private const float LineWidth = 2f;

		// Token: 0x02000C0F RID: 3087
		private struct ReorderableGroup
		{
			// Token: 0x17000C25 RID: 3109
			// (get) Token: 0x06004ED9 RID: 20185 RVA: 0x0019161F File Offset: 0x0018F81F
			public bool DrawLineExactlyBetween
			{
				get
				{
					return this.drawLineExactlyBetween_space > 0f;
				}
			}

			// Token: 0x04003830 RID: 14384
			public Action<int, int> reorderedAction;

			// Token: 0x04003831 RID: 14385
			public ReorderableDirection direction;

			// Token: 0x04003832 RID: 14386
			public float drawLineExactlyBetween_space;

			// Token: 0x04003833 RID: 14387
			public Action<int, Vector2> extraDraggedItemOnGUI;

			// Token: 0x04003834 RID: 14388
			public Rect absRect;

			// Token: 0x04003835 RID: 14389
			public bool playSoundOnStartReorder;
		}

		// Token: 0x02000C10 RID: 3088
		private struct ReorderableMultiGroup
		{
			// Token: 0x04003836 RID: 14390
			public Action<int, int, int, int> reorderedAction;

			// Token: 0x04003837 RID: 14391
			public List<int> includedGroups;
		}

		// Token: 0x02000C11 RID: 3089
		private struct ReorderableInstance
		{
			// Token: 0x04003838 RID: 14392
			public int groupID;

			// Token: 0x04003839 RID: 14393
			public Rect rect;

			// Token: 0x0400383A RID: 14394
			public Rect absRect;
		}
	}
}
