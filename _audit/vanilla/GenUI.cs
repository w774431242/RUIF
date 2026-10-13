using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000BF6 RID: 3062
	[StaticConstructorOnStartup]
	public static class GenUI
	{
		// Token: 0x06004E48 RID: 20040 RVA: 0x0018DFF8 File Offset: 0x0018C1F8
		public static void SetLabelAlign(TextAnchor a)
		{
			Text.Anchor = a;
		}

		// Token: 0x06004E49 RID: 20041 RVA: 0x0018E000 File Offset: 0x0018C200
		public static void ResetLabelAlign()
		{
			Text.Anchor = TextAnchor.UpperLeft;
		}

		// Token: 0x06004E4A RID: 20042 RVA: 0x0018E008 File Offset: 0x0018C208
		public static float BackgroundDarkAlphaForText()
		{
			if (Find.CurrentMap == null)
			{
				return 0f;
			}
			float num = GenCelestial.CurCelestialSunGlow(Find.CurrentMap);
			float num2 = ((Find.CurrentMap.Biome == BiomeDefOf.IceSheet) ? 1f : Mathf.Clamp01(Find.CurrentMap.snowGrid.TotalDepth / 1000f));
			return num * num2 * 0.41f;
		}

		// Token: 0x06004E4B RID: 20043 RVA: 0x0018E068 File Offset: 0x0018C268
		public static void DrawTextWinterShadow(Rect rect)
		{
			float num = GenUI.BackgroundDarkAlphaForText();
			if (num > 0.001f)
			{
				GUI.color = new Color(1f, 1f, 1f, num);
				GUI.DrawTexture(rect, GenUI.UnderShadowTex);
				GUI.color = Color.white;
			}
		}

		// Token: 0x06004E4C RID: 20044 RVA: 0x0018E0B4 File Offset: 0x0018C2B4
		public static void DrawTextureWithMaterial(Rect rect, Texture texture, Material material, Rect texCoords = default(Rect))
		{
			if (texCoords == default(Rect))
			{
				if (material == null)
				{
					GUI.DrawTexture(rect, texture);
					return;
				}
				if (Event.current.type == EventType.Repaint)
				{
					Color color = (material.shader.SupportsMaskTex() ? GUI.color : new Color(GUI.color.r * 0.5f, GUI.color.g * 0.5f, GUI.color.b * 0.5f, GUI.color.a));
					Graphics.DrawTexture(rect, texture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, color, material);
					return;
				}
			}
			else
			{
				if (material == null)
				{
					GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
					return;
				}
				if (Event.current.type == EventType.Repaint)
				{
					Color color2 = (material.shader.SupportsMaskTex() ? GUI.color : new Color(GUI.color.r * 0.5f, GUI.color.g * 0.5f, GUI.color.b * 0.5f, GUI.color.a));
					Graphics.DrawTexture(rect, texture, texCoords, 0, 0, 0, 0, color2, material);
				}
			}
		}

		// Token: 0x06004E4D RID: 20045 RVA: 0x0018E1F4 File Offset: 0x0018C3F4
		public static void DrawTexturePartWithMaterial(Rect rect, Texture texture, Material material, Rect texCoords)
		{
			if (texCoords == default(Rect))
			{
				if (material == null)
				{
					GUI.DrawTexture(rect, texture);
					return;
				}
				if (Event.current.type == EventType.Repaint)
				{
					Color color = (material.shader.SupportsMaskTex() ? GUI.color : (GUI.color * 0.5f));
					Graphics.DrawTexture(rect, texture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, color, material);
					return;
				}
			}
			else
			{
				texCoords.y = 1f - texCoords.y - texCoords.height;
				if (material == null)
				{
					GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
					return;
				}
				if (Event.current.type == EventType.Repaint)
				{
					Color color2 = (material.shader.SupportsMaskTex() ? GUI.color : (GUI.color * 0.5f));
					Graphics.DrawTexture(rect, texture, texCoords, 0, 0, 0, 0, color2, material);
				}
			}
		}

		// Token: 0x06004E4E RID: 20046 RVA: 0x0018E2EC File Offset: 0x0018C4EC
		public static float IconDrawScale(ThingDef tDef)
		{
			float num = tDef.uiIconScale;
			if (tDef.uiIconPath.NullOrEmpty() && tDef.graphicData != null)
			{
				num *= Mathf.Min(tDef.graphicData.drawSize.x / (float)tDef.Size.x, tDef.graphicData.drawSize.y / (float)tDef.Size.z);
			}
			return num;
		}

		// Token: 0x06004E4F RID: 20047 RVA: 0x0018E358 File Offset: 0x0018C558
		public static void ErrorDialog(string message)
		{
			if (Find.WindowStack != null)
			{
				Find.WindowStack.Add(new Dialog_MessageBox(message, null, null, null, null, null, false, null, null, WindowLayer.Dialog));
			}
		}

		// Token: 0x06004E50 RID: 20048 RVA: 0x0018E38C File Offset: 0x0018C58C
		public static void DrawFlash(float centerX, float centerY, float size, float alpha, Color color)
		{
			Rect rect = new Rect(centerX - size / 2f, centerY - size / 2f, size, size);
			Color color2 = color;
			color2.a = alpha;
			GUI.color = color2;
			GUI.DrawTexture(rect, GenUI.UIFlash);
			GUI.color = Color.white;
		}

		// Token: 0x06004E51 RID: 20049 RVA: 0x0018E3D8 File Offset: 0x0018C5D8
		public static Vector2 GetSizeCached(this string s)
		{
			if (GenUI.labelWidthCache.Count > 2000 || (Time.frameCount % 40000 == 0 && GenUI.labelWidthCache.Count > 100))
			{
				GenUI.labelWidthCache.Clear();
			}
			s = s.StripTags();
			Vector2 vector;
			if (!GenUI.labelWidthCache.TryGetValue(s, out vector))
			{
				vector = Text.CalcSize(s);
				GenUI.labelWidthCache.Add(s, vector);
			}
			return vector;
		}

		// Token: 0x06004E52 RID: 20050 RVA: 0x0018E446 File Offset: 0x0018C646
		public static float GetWidthCached(this string s)
		{
			return s.GetSizeCached().x;
		}

		// Token: 0x06004E53 RID: 20051 RVA: 0x0018E453 File Offset: 0x0018C653
		public static float GetHeightCached(this string s)
		{
			return s.GetSizeCached().y;
		}

		// Token: 0x06004E54 RID: 20052 RVA: 0x0018E460 File Offset: 0x0018C660
		public static void ClearLabelWidthCache()
		{
			GenUI.labelWidthCache.Clear();
		}

		// Token: 0x06004E55 RID: 20053 RVA: 0x0018E46C File Offset: 0x0018C66C
		public static Rect Rounded(this Rect r)
		{
			return new Rect((float)((int)r.x), (float)((int)r.y), (float)((int)r.width), (float)((int)r.height));
		}

		// Token: 0x06004E56 RID: 20054 RVA: 0x0018E497 File Offset: 0x0018C697
		public static Rect RoundedCeil(this Rect r)
		{
			return new Rect((float)Mathf.CeilToInt(r.x), (float)Mathf.CeilToInt(r.y), (float)Mathf.CeilToInt(r.width), (float)Mathf.CeilToInt(r.height));
		}

		// Token: 0x06004E57 RID: 20055 RVA: 0x0018E4D2 File Offset: 0x0018C6D2
		public static Vector2 Rounded(this Vector2 v)
		{
			return new Vector2((float)((int)v.x), (float)((int)v.y));
		}

		// Token: 0x06004E58 RID: 20056 RVA: 0x0018E4EC File Offset: 0x0018C6EC
		public static float DistFromRect(Rect r, Vector2 p)
		{
			float num = Mathf.Abs(p.x - r.center.x) - r.width / 2f;
			if (num < 0f)
			{
				num = 0f;
			}
			float num2 = Mathf.Abs(p.y - r.center.y) - r.height / 2f;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			return Mathf.Sqrt(num * num + num2 * num2);
		}

		// Token: 0x06004E59 RID: 20057 RVA: 0x0018E570 File Offset: 0x0018C770
		public static void DrawMouseAttachment(Texture iconTex, string text = "", float angle = 0f, Vector2 offset = default(Vector2), Rect? customRect = null, Color? textColor = null, bool drawTextBackground = false, Color textBgColor = default(Color), Color? iconColor = null, Action<Rect> postDrawAction = null)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			float num = mousePosition.y + 12f;
			if (drawTextBackground && text != "")
			{
				Rect value;
				if (customRect != null)
				{
					value = customRect.Value;
				}
				else
				{
					float num2 = Mathf.Min(Text.CalcSize(text).x, 200f);
					float num3 = Text.CalcHeight(text, 200f);
					float num4 = ((iconTex != null) ? 42f : 0f);
					value = new Rect(mousePosition.x + 12f - 4f, num + num4, num2 + 8f, num3);
				}
				Widgets.DrawBoxSolid(value, textBgColor);
			}
			if (iconTex != null)
			{
				Rect mouseRect;
				if (customRect != null)
				{
					mouseRect = customRect.Value;
				}
				else
				{
					mouseRect = new Rect(mousePosition.x + 8f, num + 8f, 32f, 32f);
				}
				Find.WindowStack.ImmediateWindow(34003428, mouseRect, WindowLayer.Super, delegate
				{
					Rect rect = mouseRect.AtZero();
					rect.position += new Vector2(offset.x * rect.size.x, offset.y * rect.size.y);
					GUI.color = iconColor ?? Color.white;
					Widgets.DrawTextureRotated(rect, iconTex, angle, null);
					GUI.color = Color.white;
					Action<Rect> postDrawAction2 = postDrawAction;
					if (postDrawAction2 == null)
					{
						return;
					}
					postDrawAction2(rect);
				}, false, false, 0f, null, false);
				num += mouseRect.height + 10f;
			}
			if (text != "")
			{
				Rect textRect = new Rect(mousePosition.x + 12f, num, 200f, 9999f);
				Find.WindowStack.ImmediateWindow(34003429, textRect, WindowLayer.Super, delegate
				{
					GameFont font = Text.Font;
					Text.Font = GameFont.Small;
					GUI.color = textColor ?? Color.white;
					Widgets.Label(textRect.AtZero(), text);
					GUI.color = Color.white;
					Text.Font = font;
				}, false, false, 0f, null, false);
			}
		}

		// Token: 0x06004E5A RID: 20058 RVA: 0x0018E78C File Offset: 0x0018C98C
		public static void DrawMouseAttachment(Texture2D icon)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			Rect mouseRect = new Rect(mousePosition.x + 8f, mousePosition.y + 8f, 32f, 32f);
			Find.WindowStack.ImmediateWindow(34003428, mouseRect, WindowLayer.Super, delegate
			{
				GUI.DrawTexture(mouseRect.AtZero(), icon);
			}, false, false, 0f, null, false);
		}

		// Token: 0x06004E5B RID: 20059 RVA: 0x0018E80C File Offset: 0x0018CA0C
		public static void RenderMouseoverBracket()
		{
			Vector3 vector = UI.MouseCell().ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
			Graphics.DrawMesh(MeshPool.plane10, vector, Quaternion.identity, GenUI.MouseoverBracketMaterial, 0);
		}

		// Token: 0x06004E5C RID: 20060 RVA: 0x0018E840 File Offset: 0x0018CA40
		public static void DrawStatusLevel(Need status, Rect rect)
		{
			Widgets.BeginGroup(rect);
			Widgets.Label(new Rect(0f, 2f, rect.width, 25f), status.LabelCap);
			Rect rect2 = new Rect(100f, 3f, GenUI.PieceBarSize.x, GenUI.PieceBarSize.y);
			Widgets.FillableBar(rect2, status.CurLevelPercentage);
			Widgets.FillableBarChangeArrows(rect2, status.GUIChangeArrow);
			Widgets.EndGroup();
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, status.GetTipString());
			}
			if (Mouse.IsOver(rect))
			{
				GUI.DrawTexture(rect, TexUI.HighlightTex);
			}
		}

		// Token: 0x06004E5D RID: 20061 RVA: 0x0018E8E5 File Offset: 0x0018CAE5
		public static IEnumerable<LocalTargetInfo> TargetsAtMouse(TargetingParameters clickParams, bool thingsOnly = false, ITargetingSource source = null)
		{
			return GenUI.TargetsAt(UI.MouseMapPosition(), clickParams, thingsOnly, source);
		}

		// Token: 0x06004E5E RID: 20062 RVA: 0x0018E8F4 File Offset: 0x0018CAF4
		public static IEnumerable<LocalTargetInfo> TargetsAt(Vector3 clickPos, TargetingParameters clickParams, bool thingsOnly = false, ITargetingSource source = null)
		{
			List<Thing> clickableList = GenUI.ThingsUnderMouse(clickPos, 0.8f, clickParams, source);
			Thing caster = ((source != null) ? source.Caster : null);
			int num;
			for (int i = 0; i < clickableList.Count; i = num + 1)
			{
				Pawn pawn = clickableList[i] as Pawn;
				if (pawn == null || !pawn.IsPsychologicallyInvisible() || caster == null || caster.Faction == pawn.Faction)
				{
					yield return clickableList[i];
				}
				num = i;
			}
			if (!thingsOnly)
			{
				IntVec3 intVec = UI.MouseCell();
				if (intVec.InBounds(Find.CurrentMap, clickParams.mapBoundsContractedBy) && clickParams.CanTarget(new TargetInfo(intVec, Find.CurrentMap, false), source))
				{
					yield return intVec;
				}
			}
			yield break;
		}

		// Token: 0x06004E5F RID: 20063 RVA: 0x0018E91C File Offset: 0x0018CB1C
		public static List<Thing> ThingsUnderMouse(Vector3 clickPos, float pawnWideClickRadius, TargetingParameters clickParams, ITargetingSource source = null)
		{
			IntVec3 intVec = IntVec3.FromVector3(clickPos);
			List<Thing> list = new List<Thing>();
			IReadOnlyList<Pawn> allPawnsSpawned = Find.CurrentMap.mapPawns.AllPawnsSpawned;
			for (int i = 0; i < allPawnsSpawned.Count; i++)
			{
				Pawn pawn = allPawnsSpawned[i];
				if ((pawn.DrawPos - clickPos).MagnitudeHorizontal() < 0.4f && clickParams.CanTarget(pawn, source))
				{
					list.Add(pawn);
					list.AddRange(ContainingSelectionUtility.SelectableContainedThings(pawn));
				}
			}
			list.Sort(new Comparison<Thing>(GenUI.CompareThingsByDistanceToMousePointer));
			GenUI.cellThings.Clear();
			foreach (Thing thing4 in Find.CurrentMap.thingGrid.ThingsAt(intVec))
			{
				if (!list.Contains(thing4) && clickParams.CanTarget(thing4, source))
				{
					GenUI.cellThings.Add(thing4);
					GenUI.cellThings.AddRange(ContainingSelectionUtility.SelectableContainedThings(thing4));
				}
			}
			IntVec3[] adjacentCells = GenAdj.AdjacentCells;
			for (int j = 0; j < adjacentCells.Length; j++)
			{
				IntVec3 intVec2 = adjacentCells[j] + intVec;
				if (intVec2.InBounds(Find.CurrentMap) && intVec2.GetItemCount(Find.CurrentMap) > 1)
				{
					foreach (Thing thing2 in Find.CurrentMap.thingGrid.ThingsAt(intVec2))
					{
						if (thing2.def.category == ThingCategory.Item && (thing2.TrueCenter() - UI.MouseMapPosition()).MagnitudeHorizontalSquared() <= 0.25f && !list.Contains(thing2) && clickParams.CanTarget(thing2, source))
						{
							GenUI.cellThings.Add(thing2);
						}
					}
				}
			}
			List<Thing> list2 = Find.CurrentMap.listerThings.ThingsInGroup(ThingRequestGroup.WithCustomRectForSelector);
			for (int k = 0; k < list2.Count; k++)
			{
				Thing thing3 = list2[k];
				if (thing3.CustomRectForSelector != null && thing3.CustomRectForSelector.Value.Contains(intVec) && !list.Contains(thing3) && clickParams.CanTarget(thing3, source))
				{
					GenUI.cellThings.Add(thing3);
				}
			}
			GenUI.cellThings.Sort(new Comparison<Thing>(GenUI.CompareThingsByDrawAltitudeOrDistToItem));
			list.AddRange(GenUI.cellThings);
			GenUI.cellThings.Clear();
			IReadOnlyList<Pawn> allPawnsSpawned2 = Find.CurrentMap.mapPawns.AllPawnsSpawned;
			for (int l = 0; l < allPawnsSpawned2.Count; l++)
			{
				Pawn pawn2 = allPawnsSpawned2[l];
				if ((pawn2.DrawPos - clickPos).MagnitudeHorizontal() < pawnWideClickRadius && clickParams.CanTarget(pawn2, source))
				{
					GenUI.cellThings.Add(pawn2);
				}
			}
			GenUI.cellThings.Sort(new Comparison<Thing>(GenUI.CompareThingsByDistanceToMousePointer));
			for (int m = 0; m < GenUI.cellThings.Count; m++)
			{
				if (!list.Contains(GenUI.cellThings[m]))
				{
					list.Add(GenUI.cellThings[m]);
					list.AddRange(ContainingSelectionUtility.SelectableContainedThings(GenUI.cellThings[m]));
				}
			}
			list.RemoveAll((Thing thing) => !clickParams.CanTarget(thing, source));
			list.RemoveAll(delegate(Thing thing)
			{
				Pawn pawn3 = thing as Pawn;
				return pawn3 != null && pawn3.IsHiddenFromPlayer();
			});
			GenUI.cellThings.Clear();
			return list;
		}

		// Token: 0x06004E60 RID: 20064 RVA: 0x0018ED38 File Offset: 0x0018CF38
		private static int CompareThingsByDistanceToMousePointer(Thing a, Thing b)
		{
			Vector3 vector = UI.MouseMapPosition();
			float num = (a.DrawPosHeld.Value - vector).MagnitudeHorizontalSquared();
			float num2 = (b.DrawPosHeld.Value - vector).MagnitudeHorizontalSquared();
			if (num < num2)
			{
				return -1;
			}
			if (num == num2)
			{
				return b.Spawned.CompareTo(a.Spawned);
			}
			return 1;
		}

		// Token: 0x06004E61 RID: 20065 RVA: 0x0018EDA0 File Offset: 0x0018CFA0
		private static int CompareThingsByDrawAltitudeOrDistToItem(Thing A, Thing B)
		{
			if (A.def.category == ThingCategory.Item && B.def.category == ThingCategory.Item)
			{
				return (A.TrueCenter() - UI.MouseMapPosition()).MagnitudeHorizontalSquared().CompareTo((B.TrueCenter() - UI.MouseMapPosition()).MagnitudeHorizontalSquared());
			}
			Thing spawnedParentOrMe = A.SpawnedParentOrMe;
			Thing spawnedParentOrMe2 = B.SpawnedParentOrMe;
			if (spawnedParentOrMe.def.Altitude != spawnedParentOrMe2.def.Altitude)
			{
				return spawnedParentOrMe2.def.Altitude.CompareTo(spawnedParentOrMe.def.Altitude);
			}
			return B.Spawned.CompareTo(A.Spawned);
		}

		// Token: 0x06004E62 RID: 20066 RVA: 0x0018EE55 File Offset: 0x0018D055
		public static int CurrentAdjustmentMultiplier()
		{
			if (KeyBindingDefOf.ModifierIncrement_10x.IsDownEvent && KeyBindingDefOf.ModifierIncrement_100x.IsDownEvent)
			{
				return 1000;
			}
			if (KeyBindingDefOf.ModifierIncrement_100x.IsDownEvent)
			{
				return 100;
			}
			if (KeyBindingDefOf.ModifierIncrement_10x.IsDownEvent)
			{
				return 10;
			}
			return 1;
		}

		// Token: 0x06004E63 RID: 20067 RVA: 0x0018EE94 File Offset: 0x0018D094
		public static Rect GetInnerRect(this Rect rect)
		{
			return rect.ContractedBy(17f);
		}

		// Token: 0x06004E64 RID: 20068 RVA: 0x0018EEA1 File Offset: 0x0018D0A1
		public static Rect ExpandedBy(this Rect rect, float margin)
		{
			return new Rect(rect.x - margin, rect.y - margin, rect.width + margin * 2f, rect.height + margin * 2f);
		}

		// Token: 0x06004E65 RID: 20069 RVA: 0x0018EED8 File Offset: 0x0018D0D8
		public static Rect ExpandedBy(this Rect rect, float marginX, float marginY)
		{
			return new Rect(rect.x - marginX, rect.y - marginY, rect.width + marginX * 2f, rect.height + marginY * 2f);
		}

		// Token: 0x06004E66 RID: 20070 RVA: 0x0018EF0F File Offset: 0x0018D10F
		public static Rect ContractedBy(this Rect rect, float margin)
		{
			return new Rect(rect.x + margin, rect.y + margin, rect.width - margin * 2f, rect.height - margin * 2f);
		}

		// Token: 0x06004E67 RID: 20071 RVA: 0x0018EF46 File Offset: 0x0018D146
		public static Rect ContractedBy(this Rect rect, float marginX, float marginY)
		{
			return new Rect(rect.x + marginX, rect.y + marginY, rect.width - marginX * 2f, rect.height - marginY * 2f);
		}

		// Token: 0x06004E68 RID: 20072 RVA: 0x0018EF80 File Offset: 0x0018D180
		public static Rect ScaledBy(this Rect rect, float scale)
		{
			rect.x -= rect.width * (scale - 1f) / 2f;
			rect.y -= rect.height * (scale - 1f) / 2f;
			rect.width *= scale;
			rect.height *= scale;
			return rect;
		}

		// Token: 0x06004E69 RID: 20073 RVA: 0x0018EFF2 File Offset: 0x0018D1F2
		public static Rect CenteredOnXIn(this Rect rect, Rect otherRect)
		{
			return new Rect(otherRect.x + (otherRect.width - rect.width) / 2f, rect.y, rect.width, rect.height);
		}

		// Token: 0x06004E6A RID: 20074 RVA: 0x0018F02B File Offset: 0x0018D22B
		public static Rect CenteredOnYIn(this Rect rect, Rect otherRect)
		{
			return new Rect(rect.x, otherRect.y + (otherRect.height - rect.height) / 2f, rect.width, rect.height);
		}

		// Token: 0x06004E6B RID: 20075 RVA: 0x0018F064 File Offset: 0x0018D264
		public static Rect AtZero(this Rect rect)
		{
			return new Rect(0f, 0f, rect.width, rect.height);
		}

		// Token: 0x06004E6C RID: 20076 RVA: 0x0018F084 File Offset: 0x0018D284
		public static Rect Union(this Rect a, Rect b)
		{
			return new Rect
			{
				min = Vector2.Min(a.min, b.min),
				max = Vector2.Max(a.max, b.max)
			};
		}

		// Token: 0x06004E6D RID: 20077 RVA: 0x0018F0CE File Offset: 0x0018D2CE
		public static void AbsorbClicksInRect(Rect r)
		{
			if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
			{
				Event.current.Use();
			}
		}

		// Token: 0x06004E6E RID: 20078 RVA: 0x0018F0F9 File Offset: 0x0018D2F9
		public static Rect LeftHalf(this Rect rect)
		{
			return new Rect(rect.x, rect.y, rect.width / 2f, rect.height);
		}

		// Token: 0x06004E6F RID: 20079 RVA: 0x0018F122 File Offset: 0x0018D322
		public static Rect LeftPart(this Rect rect, float pct)
		{
			return new Rect(rect.x, rect.y, rect.width * pct, rect.height);
		}

		// Token: 0x06004E70 RID: 20080 RVA: 0x0018F147 File Offset: 0x0018D347
		public static Rect LeftPartPixels(this Rect rect, float width)
		{
			return new Rect(rect.x, rect.y, width, rect.height);
		}

		// Token: 0x06004E71 RID: 20081 RVA: 0x0018F164 File Offset: 0x0018D364
		public static Rect RightHalf(this Rect rect)
		{
			return new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height);
		}

		// Token: 0x06004E72 RID: 20082 RVA: 0x0018F19B File Offset: 0x0018D39B
		public static Rect RightPart(this Rect rect, float pct)
		{
			return new Rect(rect.x + rect.width * (1f - pct), rect.y, rect.width * pct, rect.height);
		}

		// Token: 0x06004E73 RID: 20083 RVA: 0x0018F1D0 File Offset: 0x0018D3D0
		public static Rect RightPartPixels(this Rect rect, float width)
		{
			return new Rect(rect.x + rect.width - width, rect.y, width, rect.height);
		}

		// Token: 0x06004E74 RID: 20084 RVA: 0x0018F1F7 File Offset: 0x0018D3F7
		public static Rect TopHalf(this Rect rect)
		{
			return new Rect(rect.x, rect.y, rect.width, rect.height / 2f);
		}

		// Token: 0x06004E75 RID: 20085 RVA: 0x0018F220 File Offset: 0x0018D420
		public static Rect TopPart(this Rect rect, float pct)
		{
			return new Rect(rect.x, rect.y, rect.width, rect.height * pct);
		}

		// Token: 0x06004E76 RID: 20086 RVA: 0x0018F245 File Offset: 0x0018D445
		public static Rect TopPartPixels(this Rect rect, float height)
		{
			return new Rect(rect.x, rect.y, rect.width, height);
		}

		// Token: 0x06004E77 RID: 20087 RVA: 0x0018F262 File Offset: 0x0018D462
		public static Rect BottomHalf(this Rect rect)
		{
			return new Rect(rect.x, rect.y + rect.height / 2f, rect.width, rect.height / 2f);
		}

		// Token: 0x06004E78 RID: 20088 RVA: 0x0018F299 File Offset: 0x0018D499
		public static Rect BottomPart(this Rect rect, float pct)
		{
			return new Rect(rect.x, rect.y + rect.height * (1f - pct), rect.width, rect.height * pct);
		}

		// Token: 0x06004E79 RID: 20089 RVA: 0x0018F2CE File Offset: 0x0018D4CE
		public static Rect BottomPartPixels(this Rect rect, float height)
		{
			return new Rect(rect.x, rect.y + rect.height - height, rect.width, height);
		}

		// Token: 0x06004E7A RID: 20090 RVA: 0x0018F2F8 File Offset: 0x0018D4F8
		public static Rect MiddlePart(this Rect rect, float pctWidth, float pctHeight)
		{
			return new Rect(rect.x + rect.width / 2f - rect.width * pctWidth / 2f, rect.y + rect.height / 2f - rect.height * pctHeight / 2f, rect.width * pctWidth, rect.height * pctHeight);
		}

		// Token: 0x06004E7B RID: 20091 RVA: 0x0018F368 File Offset: 0x0018D568
		public static Rect MiddlePartPixels(this Rect rect, float width, float height)
		{
			return new Rect(rect.x + rect.width / 2f - width / 2f, rect.y + rect.height / 2f - height / 2f, width, height);
		}

		// Token: 0x06004E7C RID: 20092 RVA: 0x0018F3B6 File Offset: 0x0018D5B6
		public static IEnumerable<Vector2> Corners(this Rect rect)
		{
			yield return new Vector2(rect.x, rect.y);
			yield return new Vector2(rect.x, rect.yMax);
			yield return new Vector2(rect.xMax, rect.yMax);
			yield return new Vector2(rect.xMax, rect.y);
			yield break;
		}

		// Token: 0x06004E7D RID: 20093 RVA: 0x0018F3C8 File Offset: 0x0018D5C8
		public static bool SplitHorizontallyWithMargin(this Rect rect, out Rect top, out Rect bottom, out float overflow, float compressibleMargin = 0f, float? topHeight = null, float? bottomHeight = null)
		{
			if (topHeight != null == (bottomHeight != null))
			{
				throw new ArgumentException("Exactly one null height and one non-null height must be provided.");
			}
			overflow = Mathf.Max(0f, (topHeight ?? bottomHeight.GetValueOrDefault()) - rect.height);
			float num = Mathf.Clamp(topHeight ?? (rect.height - bottomHeight.Value - compressibleMargin), 0f, rect.height);
			float num2 = Mathf.Clamp(bottomHeight ?? (rect.height - topHeight.Value - compressibleMargin), 0f, rect.height);
			top = new Rect(rect.x, rect.y, rect.width, num);
			bottom = new Rect(rect.x, rect.yMax - num2, rect.width, num2);
			return overflow == 0f;
		}

		// Token: 0x06004E7E RID: 20094 RVA: 0x0018F4E0 File Offset: 0x0018D6E0
		public static void SplitVerticallyWithMargin(this Rect rect, out Rect left, out Rect right, float margin)
		{
			float num = rect.width / 2f;
			left = new Rect(rect.x, rect.y, num - margin / 2f, rect.height);
			right = new Rect(left.xMax + margin, rect.y, num - margin / 2f, rect.height);
		}

		// Token: 0x06004E7F RID: 20095 RVA: 0x0018F550 File Offset: 0x0018D750
		public static bool SplitVerticallyWithMargin(this Rect rect, out Rect left, out Rect right, out float overflow, float compressibleMargin = 0f, float? leftWidth = null, float? rightWidth = null)
		{
			if (leftWidth != null == (rightWidth != null))
			{
				throw new ArgumentException("Exactly one null width and one non-null width must be provided.");
			}
			overflow = Mathf.Max(0f, (leftWidth ?? rightWidth.GetValueOrDefault()) - rect.width);
			float num = Mathf.Clamp(leftWidth ?? (rect.width - rightWidth.Value - compressibleMargin), 0f, rect.width);
			float num2 = Mathf.Clamp(rightWidth ?? (rect.width - leftWidth.Value - compressibleMargin), 0f, rect.width);
			left = new Rect(rect.x, rect.y, num, rect.height);
			right = new Rect(rect.xMax - num2, rect.y, num2, rect.height);
			return overflow == 0f;
		}

		// Token: 0x06004E80 RID: 20096 RVA: 0x0018F668 File Offset: 0x0018D868
		public static void SplitHorizontally(this Rect rect, float topHeight, out Rect top, out Rect bottom)
		{
			float num;
			rect.SplitHorizontallyWithMargin(out top, out bottom, out num, 0f, new float?(topHeight), null);
		}

		// Token: 0x06004E81 RID: 20097 RVA: 0x0018F694 File Offset: 0x0018D894
		public static void SplitVertically(this Rect rect, float leftWidth, out Rect left, out Rect right)
		{
			float num;
			rect.SplitVerticallyWithMargin(out left, out right, out num, 0f, new float?(leftWidth), null);
		}

		// Token: 0x06004E82 RID: 20098 RVA: 0x0018F6C0 File Offset: 0x0018D8C0
		public static Color LerpColor(List<Pair<float, Color>> colors, float value)
		{
			if (colors.Count == 0)
			{
				return Color.white;
			}
			int i = 0;
			while (i < colors.Count)
			{
				if (value < colors[i].First)
				{
					if (i == 0)
					{
						return colors[i].Second;
					}
					return Color.Lerp(colors[i - 1].Second, colors[i].Second, Mathf.InverseLerp(colors[i - 1].First, colors[i].First, value));
				}
				else
				{
					i++;
				}
			}
			return colors.Last<Pair<float, Color>>().Second;
		}

		// Token: 0x06004E83 RID: 20099 RVA: 0x0018F76C File Offset: 0x0018D96C
		public static Vector2 GetMouseAttachedWindowPos(float width, float height)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			float num;
			if (mousePosition.y + 14f + height < (float)UI.screenHeight)
			{
				num = mousePosition.y + 14f;
			}
			else if (mousePosition.y - 5f - height >= 0f)
			{
				num = mousePosition.y - 5f - height;
			}
			else
			{
				num = (float)UI.screenHeight - (14f + height);
			}
			float num2;
			if (mousePosition.x + 16f + width < (float)UI.screenWidth)
			{
				num2 = mousePosition.x + 16f;
			}
			else
			{
				num2 = mousePosition.x - 4f - width;
			}
			return new Vector2(num2, num);
		}

		// Token: 0x06004E84 RID: 20100 RVA: 0x0018F828 File Offset: 0x0018DA28
		public static float GetCenteredButtonPos(int buttonIndex, int buttonsCount, float totalWidth, float buttonWidth, float pad = 10f)
		{
			float num = (float)buttonsCount * buttonWidth + (float)(buttonsCount - 1) * pad;
			return Mathf.Floor((totalWidth - num) / 2f + (float)buttonIndex * (buttonWidth + pad));
		}

		// Token: 0x06004E85 RID: 20101 RVA: 0x0018F858 File Offset: 0x0018DA58
		public static void DrawArrowPointingAt(Rect rect)
		{
			Vector2 vector = new Vector2((float)UI.screenWidth, (float)UI.screenHeight) / 2f;
			float num = Mathf.Atan2(rect.center.x - vector.x, vector.y - rect.center.y) * 57.29578f;
			Vector2 vector2 = new Bounds(rect.center, rect.size).ClosestPoint(vector);
			Rect rect2 = new Rect(vector2 + Vector2.left * (float)GenUI.ArrowTex.width * 0.5f, new Vector2((float)GenUI.ArrowTex.width, (float)GenUI.ArrowTex.height));
			Matrix4x4 matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.identity;
			Vector2 vector3 = GUIUtility.GUIToScreenPoint(vector2);
			GUI.matrix = matrix;
			UI.RotateAroundPivot(num, vector3);
			GUI.DrawTexture(rect2, GenUI.ArrowTex);
			GUI.matrix = matrix;
			UnityGUIBugsFixer.Notify_GUIMatrixChanged();
		}

		// Token: 0x06004E86 RID: 20102 RVA: 0x0018F968 File Offset: 0x0018DB68
		public static void DrawArrowPointingAtWorldspace(Vector3 worldspace, Camera camera)
		{
			Vector3 vector = camera.WorldToScreenPoint(worldspace) / Prefs.UIScale;
			GenUI.DrawArrowPointingAt(new Rect(new Vector2(vector.x, (float)UI.screenHeight - vector.y) + new Vector2(-2f, 2f), new Vector2(4f, 4f)));
		}

		// Token: 0x06004E87 RID: 20103 RVA: 0x0018F9CC File Offset: 0x0018DBCC
		public static Rect DrawElementStack<T>(Rect rect, float rowHeight, List<T> elements, GenUI.StackElementDrawer<T> drawer, GenUI.StackElementWidthGetter<T> widthGetter, float rowMargin = 4f, float elementMargin = 5f, bool allowOrderOptimization = true)
		{
			GenUI.tmpRects.Clear();
			GenUI.tmpRects2.Clear();
			for (int i = 0; i < elements.Count; i++)
			{
				GenUI.tmpRects.Add(new GenUI.StackedElementRect(new Rect(0f, 0f, widthGetter(elements[i]), rowHeight), i));
			}
			int num = Mathf.FloorToInt(rect.height / rowHeight);
			List<GenUI.StackedElementRect> list = GenUI.tmpRects;
			float num3;
			float num2;
			if (allowOrderOptimization)
			{
				num2 = (num3 = 0f);
				while (num2 < (float)num)
				{
					GenUI.StackedElementRect stackedElementRect = default(GenUI.StackedElementRect);
					int num4 = -1;
					for (int j = 0; j < list.Count; j++)
					{
						GenUI.StackedElementRect stackedElementRect2 = list[j];
						if (num4 == -1 || (stackedElementRect.rect.width < stackedElementRect2.rect.width && stackedElementRect2.rect.width < rect.width - num3))
						{
							num4 = j;
							stackedElementRect = stackedElementRect2;
						}
					}
					if (num4 == -1)
					{
						if (num3 == 0f)
						{
							break;
						}
						num3 = 0f;
						num2 += 1f;
					}
					else
					{
						num3 += stackedElementRect.rect.width + elementMargin;
						GenUI.tmpRects2.Add(stackedElementRect);
					}
					list.RemoveAt(num4);
					if (list.Count <= 0)
					{
						break;
					}
				}
				list = GenUI.tmpRects2;
			}
			num2 = (num3 = 0f);
			while (list.Count > 0)
			{
				GenUI.StackedElementRect stackedElementRect3 = list[0];
				if (num3 + stackedElementRect3.rect.width > rect.width)
				{
					num3 = 0f;
					num2 += rowHeight + rowMargin;
				}
				if (drawer != null)
				{
					drawer(new Rect(rect.x + num3, rect.y + num2, stackedElementRect3.rect.width, stackedElementRect3.rect.height), elements[stackedElementRect3.elementIndex]);
				}
				num3 += stackedElementRect3.rect.width + elementMargin;
				list.RemoveAt(0);
			}
			return new Rect(rect.x, rect.y, rect.width, num2 + rowHeight);
		}

		// Token: 0x06004E88 RID: 20104 RVA: 0x0018FBE0 File Offset: 0x0018DDE0
		public static Rect DrawElementStackVertical<T>(Rect rect, float rowHeight, List<T> elements, GenUI.StackElementDrawer<T> drawer, GenUI.StackElementWidthGetter<T> widthGetter, float elementMargin = 5f, bool highlightOdd = false)
		{
			GenUI.tmpRects.Clear();
			for (int i = 0; i < elements.Count; i++)
			{
				GenUI.tmpRects.Add(new GenUI.StackedElementRect(new Rect(0f, 0f, widthGetter(elements[i]), rowHeight), i));
			}
			int num = Mathf.FloorToInt(rect.height / rowHeight);
			GenUI.spacingCache.Reset(num);
			int num2 = 0;
			float num3 = 0f;
			float num4 = 0f;
			for (int j = 0; j < GenUI.tmpRects.Count; j++)
			{
				GenUI.StackedElementRect stackedElementRect = GenUI.tmpRects[j];
				if (num4 + stackedElementRect.rect.height > rect.height)
				{
					num4 = 0f;
					num2 = 0;
				}
				Rect rect2 = new Rect(rect.x + GenUI.spacingCache.GetSpaceFor(num2), rect.y + num4, stackedElementRect.rect.width, stackedElementRect.rect.height);
				if (highlightOdd && j % 2 == 1)
				{
					Widgets.DrawLightHighlight(rect2);
				}
				drawer(rect2, elements[stackedElementRect.elementIndex]);
				num4 += stackedElementRect.rect.height + elementMargin;
				GenUI.spacingCache.AddSpace(num2, stackedElementRect.rect.width + elementMargin);
				num3 = Mathf.Max(num3, GenUI.spacingCache.GetSpaceFor(num2));
				num2++;
			}
			return new Rect(rect.x, rect.y, num3, num4 + rowHeight);
		}

		// Token: 0x040037C3 RID: 14275
		public const float Pad = 10f;

		// Token: 0x040037C4 RID: 14276
		public const float GapTiny = 4f;

		// Token: 0x040037C5 RID: 14277
		public const float GapSmall = 10f;

		// Token: 0x040037C6 RID: 14278
		public const float Gap = 17f;

		// Token: 0x040037C7 RID: 14279
		public const float GapWide = 26f;

		// Token: 0x040037C8 RID: 14280
		public const float GapLabel = 22f;

		// Token: 0x040037C9 RID: 14281
		public const float ListSpacing = 28f;

		// Token: 0x040037CA RID: 14282
		public const float MouseAttachIconSize = 32f;

		// Token: 0x040037CB RID: 14283
		public const float MouseAttachIconOffset = 8f;

		// Token: 0x040037CC RID: 14284
		public const float ScrollBarWidth = 16f;

		// Token: 0x040037CD RID: 14285
		public const float HorizontalSliderHeight = 10f;

		// Token: 0x040037CE RID: 14286
		public static readonly Vector2 TradeableDrawSize = new Vector2(150f, 45f);

		// Token: 0x040037CF RID: 14287
		public static readonly Color MouseoverColor = new Color(0.3f, 0.7f, 0.9f);

		// Token: 0x040037D0 RID: 14288
		public static readonly Color SubtleMouseoverColor = new Color(0.7f, 0.7f, 0.7f);

		// Token: 0x040037D1 RID: 14289
		public static readonly Color FillableBar_Green = new Color(0.40392157f, 0.7019608f, 0.28627452f);

		// Token: 0x040037D2 RID: 14290
		public static readonly Color FillableBar_Empty = new Color(0.03f, 0.035f, 0.05f);

		// Token: 0x040037D3 RID: 14291
		public static readonly Vector2 MaxWinSize = new Vector2(1010f, 754f);

		// Token: 0x040037D4 RID: 14292
		public const float SmallIconSize = 24f;

		// Token: 0x040037D5 RID: 14293
		public const int RootGUIDepth = 50;

		// Token: 0x040037D6 RID: 14294
		private const float MouseIconSize = 32f;

		// Token: 0x040037D7 RID: 14295
		private const float MouseIconOffset = 12f;

		// Token: 0x040037D8 RID: 14296
		private static readonly Material MouseoverBracketMaterial = MaterialPool.MatFrom("UI/Overlays/MouseoverBracketTex", ShaderDatabase.MetaOverlay);

		// Token: 0x040037D9 RID: 14297
		private static readonly Texture2D UnderShadowTex = ContentFinder<Texture2D>.Get("UI/Misc/ScreenCornerShadow", true);

		// Token: 0x040037DA RID: 14298
		private static readonly Texture2D UIFlash = ContentFinder<Texture2D>.Get("UI/Misc/Flash", true);

		// Token: 0x040037DB RID: 14299
		private static Dictionary<string, Vector2> labelWidthCache = new Dictionary<string, Vector2>();

		// Token: 0x040037DC RID: 14300
		private static readonly Vector2 PieceBarSize = new Vector2(100f, 17f);

		// Token: 0x040037DD RID: 14301
		public const float PawnDirectClickRadius = 0.4f;

		// Token: 0x040037DE RID: 14302
		private static List<Thing> cellThings = new List<Thing>(32);

		// Token: 0x040037DF RID: 14303
		private static readonly Texture2D ArrowTex = ContentFinder<Texture2D>.Get("UI/Overlays/Arrow", true);

		// Token: 0x040037E0 RID: 14304
		private static List<GenUI.StackedElementRect> tmpRects = new List<GenUI.StackedElementRect>();

		// Token: 0x040037E1 RID: 14305
		private static List<GenUI.StackedElementRect> tmpRects2 = new List<GenUI.StackedElementRect>();

		// Token: 0x040037E2 RID: 14306
		public const float ElementStackDefaultElementMargin = 5f;

		// Token: 0x040037E3 RID: 14307
		public const float ElementStackDefaultRowMargin = 4f;

		// Token: 0x040037E4 RID: 14308
		private static GenUI.SpacingCache spacingCache;

		// Token: 0x02000BF7 RID: 3063
		private struct StackedElementRect
		{
			// Token: 0x06004E8A RID: 20106 RVA: 0x0018FE87 File Offset: 0x0018E087
			public StackedElementRect(Rect rect, int elementIndex)
			{
				this.rect = rect;
				this.elementIndex = elementIndex;
			}

			// Token: 0x040037E5 RID: 14309
			public Rect rect;

			// Token: 0x040037E6 RID: 14310
			public int elementIndex;
		}

		// Token: 0x02000BF8 RID: 3064
		public class AnonymousStackElement
		{
			// Token: 0x040037E7 RID: 14311
			public Action<Rect> drawer;

			// Token: 0x040037E8 RID: 14312
			public float width;
		}

		// Token: 0x02000BF9 RID: 3065
		private struct SpacingCache
		{
			// Token: 0x06004E8C RID: 20108 RVA: 0x0018FE98 File Offset: 0x0018E098
			public void Reset(int elem = 16)
			{
				if (this.spaces == null || this.maxElements != elem)
				{
					this.maxElements = elem;
					this.spaces = new float[this.maxElements];
					return;
				}
				for (int i = 0; i < this.maxElements; i++)
				{
					this.spaces[i] = 0f;
				}
			}

			// Token: 0x06004E8D RID: 20109 RVA: 0x0018FEED File Offset: 0x0018E0ED
			public float GetSpaceFor(int elem)
			{
				if (this.spaces == null || this.maxElements < 1)
				{
					this.Reset(16);
				}
				if (elem >= 0 && elem < this.maxElements)
				{
					return this.spaces[elem];
				}
				return 0f;
			}

			// Token: 0x06004E8E RID: 20110 RVA: 0x0018FF23 File Offset: 0x0018E123
			public void AddSpace(int elem, float space)
			{
				if (this.spaces == null || this.maxElements < 1)
				{
					this.Reset(16);
				}
				if (elem >= 0 && elem < this.maxElements)
				{
					this.spaces[elem] += space;
				}
			}

			// Token: 0x040037E9 RID: 14313
			private int maxElements;

			// Token: 0x040037EA RID: 14314
			private float[] spaces;
		}

		// Token: 0x02000BFA RID: 3066
		// (Invoke) Token: 0x06004E90 RID: 20112
		public delegate void StackElementDrawer<T>(Rect rect, T element);

		// Token: 0x02000BFB RID: 3067
		// (Invoke) Token: 0x06004E94 RID: 20116
		public delegate float StackElementWidthGetter<T>(T element);
	}
}
