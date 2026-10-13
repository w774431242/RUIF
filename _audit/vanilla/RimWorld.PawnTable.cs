using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020034AA RID: 13482
	public class PawnTable
	{
		// Token: 0x1700318E RID: 12686
		// (get) Token: 0x06012FA5 RID: 77733 RVA: 0x0059EBDC File Offset: 0x0059CDDC
		public List<PawnColumnDef> Columns
		{
			get
			{
				this.columns.Clear();
				foreach (PawnColumnDef pawnColumnDef in this.def.columns)
				{
					if (pawnColumnDef.Worker.VisibleCurrently)
					{
						this.columns.Add(pawnColumnDef);
					}
				}
				return this.columns;
			}
		}

		// Token: 0x1700318F RID: 12687
		// (get) Token: 0x06012FA6 RID: 77734 RVA: 0x0059EC58 File Offset: 0x0059CE58
		public PawnColumnDef SortingBy
		{
			get
			{
				return this.sortByColumn;
			}
		}

		// Token: 0x17003190 RID: 12688
		// (get) Token: 0x06012FA7 RID: 77735 RVA: 0x0059EC60 File Offset: 0x0059CE60
		public bool SortingDescending
		{
			get
			{
				return this.SortingBy != null && this.sortDescending;
			}
		}

		// Token: 0x17003191 RID: 12689
		// (get) Token: 0x06012FA8 RID: 77736 RVA: 0x0059EC72 File Offset: 0x0059CE72
		public Vector2 Size
		{
			get
			{
				this.RecacheIfDirty();
				return this.cachedSize;
			}
		}

		// Token: 0x17003192 RID: 12690
		// (get) Token: 0x06012FA9 RID: 77737 RVA: 0x0059EC80 File Offset: 0x0059CE80
		public float HeightNoScrollbar
		{
			get
			{
				this.RecacheIfDirty();
				return this.cachedHeightNoScrollbar;
			}
		}

		// Token: 0x17003193 RID: 12691
		// (get) Token: 0x06012FAA RID: 77738 RVA: 0x0059EC8E File Offset: 0x0059CE8E
		public float HeaderHeight
		{
			get
			{
				this.RecacheIfDirty();
				return this.cachedHeaderHeight;
			}
		}

		// Token: 0x17003194 RID: 12692
		// (get) Token: 0x06012FAB RID: 77739 RVA: 0x0059EC9C File Offset: 0x0059CE9C
		public List<Pawn> PawnsListForReading
		{
			get
			{
				this.RecacheIfDirty();
				return this.cachedPawns;
			}
		}

		// Token: 0x06012FAC RID: 77740 RVA: 0x0059ECAC File Offset: 0x0059CEAC
		public PawnTable(PawnTableDef def, Func<IEnumerable<Pawn>> pawnsGetter, int uiWidth, int uiHeight)
		{
			this.def = def;
			this.pawnsGetter = pawnsGetter;
			this.SetMinMaxSize(def.minWidth, uiWidth, 0, uiHeight);
			this.SetDirty();
		}

		// Token: 0x06012FAD RID: 77741 RVA: 0x0059ED30 File Offset: 0x0059CF30
		public void PawnTableOnGUI(Vector2 position)
		{
			if (Event.current.type == EventType.Layout)
			{
				return;
			}
			this.RecacheIfDirty();
			float num = this.cachedSize.x - 16f;
			List<PawnColumnDef> list = this.Columns;
			int num2 = 0;
			for (int i = 0; i < list.Count; i++)
			{
				int num3;
				if (i == list.Count - 1)
				{
					num3 = (int)(num - (float)num2);
				}
				else
				{
					num3 = (int)this.cachedColumnWidths[i];
				}
				Rect rect = new Rect((float)((int)position.x + num2), (float)((int)position.y), (float)num3, (float)((int)this.cachedHeaderHeight));
				list[i].Worker.DoHeader(rect, this);
				num2 += num3;
			}
			Rect rect2 = new Rect((float)((int)position.x), (float)((int)position.y + (int)this.cachedHeaderHeight), (float)((int)this.cachedSize.x), (float)((int)this.cachedSize.y - (int)this.cachedHeaderHeight));
			Rect rect3 = new Rect(0f, 0f, rect2.width - 16f, (float)((int)this.cachedHeightNoScrollbar - (int)this.cachedHeaderHeight));
			Widgets.BeginScrollView(rect2, ref this.scrollPosition, rect3, true);
			num2 = 0;
			int num4;
			for (int j = 0; j < list.Count; j++)
			{
				num4 = 0;
				PawnColumnDef pawnColumnDef = list[j];
				int num5;
				if (j == list.Count - 1)
				{
					num5 = (int)(num - (float)num2);
				}
				else
				{
					num5 = (int)this.cachedColumnWidths[j];
				}
				for (int k = 0; k < this.cachedPawns.Count; k++)
				{
					GUI.color = PawnTable.BorderColor;
					Widgets.DrawLineHorizontal((float)num2, (float)num4, (float)num5);
					GUI.color = Color.white;
					Rect rect4 = new Rect((float)num2, (float)num4, (float)num5, (float)((int)this.cachedRowHeights[k]));
					Pawn pawn = this.cachedPawns[k];
					bool flag = false;
					if (pawnColumnDef.groupable)
					{
						int num6 = k;
						int num7 = k + 1;
						while (num7 < this.cachedPawns.Count && list[j].Worker.CanGroupWith(this.cachedPawns[k], this.cachedPawns[num7]))
						{
							rect4.yMax += (float)((int)this.cachedRowHeights[num7]);
							num6 = num7;
							flag = true;
							num7++;
						}
						k = num6;
					}
					if ((float)num4 - this.scrollPosition.y + (float)((int)this.cachedRowHeights[k]) >= 0f && (float)num4 - this.scrollPosition.y <= rect2.height)
					{
						list[j].Worker.DoCell(rect4, pawn, this);
						if (pawnColumnDef.groupable && flag)
						{
							GUI.color = PawnTable.BorderColor;
							Widgets.DrawLineVertical(rect4.xMin, rect4.yMin, rect4.height);
							Widgets.DrawLineVertical(rect4.xMax, rect4.yMin, rect4.height);
							GUI.color = Color.white;
						}
					}
					GUI.color = Color.white;
					num4 += (int)rect4.height;
				}
				num2 += num5;
			}
			num4 = 0;
			for (int l = 0; l < this.cachedPawns.Count; l++)
			{
				Rect rect5 = new Rect(0f, (float)num4, rect3.width, (float)((int)this.cachedRowHeights[l]));
				if (Find.Selector.IsSelected(this.cachedPawns[l]))
				{
					Widgets.DrawHighlight(rect5, 0.6f);
				}
				if (Mouse.IsOver(rect5))
				{
					Widgets.DrawHighlight(rect5);
					this.cachedLookTargets[l].Highlight(true, this.cachedPawns[l].IsColonist, false);
				}
				if (this.cachedPawns[l].Downed)
				{
					GUI.color = new Color(1f, 0f, 0f, 0.5f);
					Widgets.DrawLineHorizontal(0f, rect5.center.y, rect3.width);
					GUI.color = Color.white;
				}
				num4 += (int)this.cachedRowHeights[l];
			}
			Widgets.EndScrollView();
		}

		// Token: 0x06012FAE RID: 77742 RVA: 0x0059F183 File Offset: 0x0059D383
		public void SetDirty()
		{
			this.dirty = true;
		}

		// Token: 0x06012FAF RID: 77743 RVA: 0x0059F18C File Offset: 0x0059D38C
		public void SetMinMaxSize(int minTableWidth, int maxTableWidth, int minTableHeight, int maxTableHeight)
		{
			this.minTableWidth = minTableWidth;
			this.maxTableWidth = maxTableWidth;
			this.minTableHeight = minTableHeight;
			this.maxTableHeight = maxTableHeight;
			this.hasFixedSize = false;
			this.SetDirty();
		}

		// Token: 0x06012FB0 RID: 77744 RVA: 0x0059F1B8 File Offset: 0x0059D3B8
		public void SetFixedSize(Vector2 size)
		{
			this.fixedSize = size;
			this.hasFixedSize = true;
			this.SetDirty();
		}

		// Token: 0x06012FB1 RID: 77745 RVA: 0x0059F1CE File Offset: 0x0059D3CE
		public void SortBy(PawnColumnDef column, bool descending)
		{
			this.sortByColumn = column;
			this.sortDescending = descending;
			this.SetDirty();
		}

		// Token: 0x06012FB2 RID: 77746 RVA: 0x0059F1E4 File Offset: 0x0059D3E4
		private void RecacheIfDirty()
		{
			if (!this.dirty)
			{
				return;
			}
			this.dirty = false;
			this.RecacheColumns();
			this.RecachePawns();
			this.RecacheRowHeights();
			this.cachedHeaderHeight = this.CalculateHeaderHeight();
			this.cachedHeightNoScrollbar = this.CalculateTotalRequiredHeight();
			this.RecacheSize();
			this.RecacheColumnWidths();
			this.RecacheLookTargets();
		}

		// Token: 0x06012FB3 RID: 77747 RVA: 0x0059F240 File Offset: 0x0059D440
		private void RecacheColumns()
		{
			foreach (PawnColumnDef pawnColumnDef in this.def.columns)
			{
				pawnColumnDef.Worker.Recache();
			}
		}

		// Token: 0x06012FB4 RID: 77748 RVA: 0x0059F29C File Offset: 0x0059D49C
		private void RecachePawns()
		{
			this.cachedPawns.Clear();
			this.cachedPawns.AddRange(this.pawnsGetter());
			this.cachedPawns = this.LabelSortFunction(this.cachedPawns).ToList<Pawn>();
			if (this.sortByColumn != null)
			{
				if (this.sortDescending)
				{
					this.cachedPawns.SortStable<Pawn>(new Func<Pawn, Pawn, int>(this.sortByColumn.Worker.Compare));
				}
				else
				{
					this.cachedPawns.SortStable<Pawn>((Pawn a, Pawn b) => this.sortByColumn.Worker.Compare(b, a));
				}
			}
			this.cachedPawns = this.PrimarySortFunction(this.cachedPawns).ToList<Pawn>();
		}

		// Token: 0x06012FB5 RID: 77749 RVA: 0x0059F343 File Offset: 0x0059D543
		protected virtual IEnumerable<Pawn> LabelSortFunction(IEnumerable<Pawn> input)
		{
			return input.OrderBy<Pawn, string>((Pawn p) => p.Label);
		}

		// Token: 0x06012FB6 RID: 77750 RVA: 0x00009BBB File Offset: 0x00007DBB
		protected virtual IEnumerable<Pawn> PrimarySortFunction(IEnumerable<Pawn> input)
		{
			return input;
		}

		// Token: 0x06012FB7 RID: 77751 RVA: 0x0059F36C File Offset: 0x0059D56C
		private void RecacheColumnWidths()
		{
			float num = this.cachedSize.x - 16f;
			float num2 = 0f;
			this.RecacheColumnWidths_StartWithMinWidths(out num2);
			if (num2 == num)
			{
				return;
			}
			if (num2 > num)
			{
				this.SubtractProportionally(num2 - num, num2);
				return;
			}
			bool flag;
			this.RecacheColumnWidths_DistributeUntilOptimal(num, ref num2, out flag);
			if (flag)
			{
				return;
			}
			this.RecacheColumnWidths_DistributeAboveOptimal(num, ref num2);
		}

		// Token: 0x06012FB8 RID: 77752 RVA: 0x0059F3C8 File Offset: 0x0059D5C8
		private void RecacheColumnWidths_StartWithMinWidths(out float minWidthsSum)
		{
			minWidthsSum = 0f;
			this.cachedColumnWidths.Clear();
			List<PawnColumnDef> list = this.Columns;
			for (int i = 0; i < list.Count; i++)
			{
				float minWidth = this.GetMinWidth(list[i]);
				this.cachedColumnWidths.Add(minWidth);
				minWidthsSum += minWidth;
			}
		}

		// Token: 0x06012FB9 RID: 77753 RVA: 0x0059F420 File Offset: 0x0059D620
		private void RecacheColumnWidths_DistributeUntilOptimal(float totalAvailableSpaceForColumns, ref float usedWidth, out bool noMoreFreeSpace)
		{
			this.columnAtOptimalWidth.Clear();
			List<PawnColumnDef> list = this.Columns;
			for (int i = 0; i < list.Count; i++)
			{
				this.columnAtOptimalWidth.Add(this.cachedColumnWidths[i] >= this.GetOptimalWidth(list[i]));
			}
			int num = 0;
			for (;;)
			{
				num++;
				if (num >= 10000)
				{
					break;
				}
				float num2 = float.MinValue;
				for (int j = 0; j < list.Count; j++)
				{
					if (!this.columnAtOptimalWidth[j])
					{
						num2 = Mathf.Max(num2, (float)list[j].widthPriority);
					}
				}
				float num3 = 0f;
				for (int k = 0; k < this.cachedColumnWidths.Count; k++)
				{
					if (!this.columnAtOptimalWidth[k] && (float)list[k].widthPriority == num2)
					{
						num3 += this.GetOptimalWidth(list[k]);
					}
				}
				float num4 = totalAvailableSpaceForColumns - usedWidth;
				bool flag = false;
				bool flag2 = false;
				for (int l = 0; l < this.cachedColumnWidths.Count; l++)
				{
					if (!this.columnAtOptimalWidth[l])
					{
						if ((float)list[l].widthPriority != num2)
						{
							flag = true;
						}
						else
						{
							float num5 = num4 * this.GetOptimalWidth(list[l]) / num3;
							float num6 = this.GetOptimalWidth(list[l]) - this.cachedColumnWidths[l];
							if (num5 >= num6)
							{
								num5 = num6;
								this.columnAtOptimalWidth[l] = true;
								flag2 = true;
							}
							else
							{
								flag = true;
							}
							if (num5 > 0f)
							{
								List<float> list2 = this.cachedColumnWidths;
								int num7 = l;
								list2[num7] += num5;
								usedWidth += num5;
							}
						}
					}
				}
				if (usedWidth >= totalAvailableSpaceForColumns - 0.1f)
				{
					goto Block_13;
				}
				if (!flag || !flag2)
				{
					goto IL_01F3;
				}
			}
			Log.Error("Too many iterations.");
			goto IL_01F3;
			Block_13:
			noMoreFreeSpace = true;
			IL_01F3:
			noMoreFreeSpace = false;
		}

		// Token: 0x06012FBA RID: 77754 RVA: 0x0059F624 File Offset: 0x0059D824
		private void RecacheColumnWidths_DistributeAboveOptimal(float totalAvailableSpaceForColumns, ref float usedWidth)
		{
			this.columnAtMaxWidth.Clear();
			List<PawnColumnDef> list = this.Columns;
			for (int i = 0; i < list.Count; i++)
			{
				this.columnAtMaxWidth.Add(this.cachedColumnWidths[i] >= this.GetMaxWidth(list[i]));
			}
			int num = 0;
			for (;;)
			{
				num++;
				if (num >= 10000)
				{
					break;
				}
				float num2 = 0f;
				for (int j = 0; j < list.Count; j++)
				{
					if (!this.columnAtMaxWidth[j])
					{
						num2 += Mathf.Max(this.GetOptimalWidth(list[j]), 1f);
					}
				}
				float num3 = totalAvailableSpaceForColumns - usedWidth;
				bool flag = false;
				for (int k = 0; k < list.Count; k++)
				{
					if (!this.columnAtMaxWidth[k])
					{
						float num4 = num3 * Mathf.Max(this.GetOptimalWidth(list[k]), 1f) / num2;
						float num5 = this.GetMaxWidth(list[k]) - this.cachedColumnWidths[k];
						if (num4 >= num5)
						{
							num4 = num5;
							this.columnAtMaxWidth[k] = true;
						}
						else
						{
							flag = true;
						}
						if (num4 > 0f)
						{
							List<float> list2 = this.cachedColumnWidths;
							int num6 = k;
							list2[num6] += num4;
							usedWidth += num4;
						}
					}
				}
				if (usedWidth >= totalAvailableSpaceForColumns - 0.1f)
				{
					return;
				}
				if (!flag)
				{
					goto Block_10;
				}
			}
			Log.Error("Too many iterations.");
			return;
			Block_10:
			this.DistributeRemainingWidthProportionallyAboveMax(totalAvailableSpaceForColumns - usedWidth);
		}

		// Token: 0x06012FBB RID: 77755 RVA: 0x0059F7B8 File Offset: 0x0059D9B8
		private void RecacheRowHeights()
		{
			this.cachedRowHeights.Clear();
			for (int i = 0; i < this.cachedPawns.Count; i++)
			{
				this.cachedRowHeights.Add(this.CalculateRowHeight(this.cachedPawns[i]));
			}
		}

		// Token: 0x06012FBC RID: 77756 RVA: 0x0059F804 File Offset: 0x0059DA04
		private void RecacheSize()
		{
			if (this.hasFixedSize)
			{
				this.cachedSize = this.fixedSize;
				return;
			}
			float num = 0f;
			List<PawnColumnDef> list = this.Columns;
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].ignoreWhenCalculatingOptimalTableSize)
				{
					num += this.GetOptimalWidth(list[i]);
				}
			}
			float num2 = Mathf.Clamp(num + 16f, (float)this.minTableWidth, (float)this.maxTableWidth);
			float num3 = Mathf.Clamp(this.cachedHeightNoScrollbar, (float)this.minTableHeight, (float)this.maxTableHeight);
			num2 = Mathf.Min(num2, (float)UI.screenWidth);
			num3 = Mathf.Min(num3, (float)UI.screenHeight);
			this.cachedSize = new Vector2(num2, num3);
		}

		// Token: 0x06012FBD RID: 77757 RVA: 0x0059F8C4 File Offset: 0x0059DAC4
		private void RecacheLookTargets()
		{
			this.cachedLookTargets.Clear();
			this.cachedLookTargets.AddRange(this.cachedPawns.Select<Pawn, LookTargets>((Pawn p) => new LookTargets(p)));
		}

		// Token: 0x06012FBE RID: 77758 RVA: 0x0059F914 File Offset: 0x0059DB14
		private void SubtractProportionally(float toSubtract, float totalUsedWidth)
		{
			for (int i = 0; i < this.cachedColumnWidths.Count; i++)
			{
				List<float> list = this.cachedColumnWidths;
				int num = i;
				list[num] -= toSubtract * this.cachedColumnWidths[i] / totalUsedWidth;
			}
		}

		// Token: 0x06012FBF RID: 77759 RVA: 0x0059F960 File Offset: 0x0059DB60
		private void DistributeRemainingWidthProportionallyAboveMax(float toDistribute)
		{
			float num = 0f;
			List<PawnColumnDef> list = this.Columns;
			for (int i = 0; i < list.Count; i++)
			{
				num += Mathf.Max(this.GetOptimalWidth(list[i]), 1f);
			}
			for (int j = 0; j < list.Count; j++)
			{
				List<float> list2 = this.cachedColumnWidths;
				int num2 = j;
				list2[num2] += toDistribute * Mathf.Max(this.GetOptimalWidth(list[j]), 1f) / num;
			}
		}

		// Token: 0x06012FC0 RID: 77760 RVA: 0x0059F9EF File Offset: 0x0059DBEF
		private float GetOptimalWidth(PawnColumnDef column)
		{
			return Mathf.Max((float)column.Worker.GetOptimalWidth(this), 0f);
		}

		// Token: 0x06012FC1 RID: 77761 RVA: 0x0059FA08 File Offset: 0x0059DC08
		private float GetMinWidth(PawnColumnDef column)
		{
			return Mathf.Max((float)column.Worker.GetMinWidth(this), 0f);
		}

		// Token: 0x06012FC2 RID: 77762 RVA: 0x0059FA21 File Offset: 0x0059DC21
		private float GetMaxWidth(PawnColumnDef column)
		{
			return Mathf.Max((float)column.Worker.GetMaxWidth(this), 0f);
		}

		// Token: 0x06012FC3 RID: 77763 RVA: 0x0059FA3C File Offset: 0x0059DC3C
		private float CalculateRowHeight(Pawn pawn)
		{
			float num = 0f;
			List<PawnColumnDef> list = this.Columns;
			for (int i = 0; i < list.Count; i++)
			{
				num = Mathf.Max(num, (float)list[i].Worker.GetMinCellHeight(pawn));
			}
			return num;
		}

		// Token: 0x06012FC4 RID: 77764 RVA: 0x0059FA84 File Offset: 0x0059DC84
		private float CalculateHeaderHeight()
		{
			float num = 0f;
			List<PawnColumnDef> list = this.Columns;
			for (int i = 0; i < list.Count; i++)
			{
				num = Mathf.Max(num, (float)list[i].Worker.GetMinHeaderHeight(this));
			}
			return num;
		}

		// Token: 0x06012FC5 RID: 77765 RVA: 0x0059FACC File Offset: 0x0059DCCC
		private float CalculateTotalRequiredHeight()
		{
			float num = this.CalculateHeaderHeight();
			for (int i = 0; i < this.cachedPawns.Count; i++)
			{
				num += this.CalculateRowHeight(this.cachedPawns[i]);
			}
			return num;
		}

		// Token: 0x0400C4B0 RID: 50352
		private static readonly Color BorderColor = new Color(1f, 1f, 1f, 0.2f);

		// Token: 0x0400C4B1 RID: 50353
		private PawnTableDef def;

		// Token: 0x0400C4B2 RID: 50354
		private Func<IEnumerable<Pawn>> pawnsGetter;

		// Token: 0x0400C4B3 RID: 50355
		private int minTableWidth;

		// Token: 0x0400C4B4 RID: 50356
		private int maxTableWidth;

		// Token: 0x0400C4B5 RID: 50357
		private int minTableHeight;

		// Token: 0x0400C4B6 RID: 50358
		private int maxTableHeight;

		// Token: 0x0400C4B7 RID: 50359
		private Vector2 fixedSize;

		// Token: 0x0400C4B8 RID: 50360
		private bool hasFixedSize;

		// Token: 0x0400C4B9 RID: 50361
		private bool dirty;

		// Token: 0x0400C4BA RID: 50362
		private List<bool> columnAtMaxWidth = new List<bool>();

		// Token: 0x0400C4BB RID: 50363
		private List<bool> columnAtOptimalWidth = new List<bool>();

		// Token: 0x0400C4BC RID: 50364
		private Vector2 scrollPosition;

		// Token: 0x0400C4BD RID: 50365
		private PawnColumnDef sortByColumn;

		// Token: 0x0400C4BE RID: 50366
		private bool sortDescending;

		// Token: 0x0400C4BF RID: 50367
		private Vector2 cachedSize;

		// Token: 0x0400C4C0 RID: 50368
		private List<Pawn> cachedPawns = new List<Pawn>();

		// Token: 0x0400C4C1 RID: 50369
		private List<float> cachedColumnWidths = new List<float>();

		// Token: 0x0400C4C2 RID: 50370
		private List<float> cachedRowHeights = new List<float>();

		// Token: 0x0400C4C3 RID: 50371
		private List<LookTargets> cachedLookTargets = new List<LookTargets>();

		// Token: 0x0400C4C4 RID: 50372
		private List<PawnColumnDef> columns = new List<PawnColumnDef>();

		// Token: 0x0400C4C5 RID: 50373
		private float cachedHeaderHeight;

		// Token: 0x0400C4C6 RID: 50374
		private float cachedHeightNoScrollbar;
	}
}
