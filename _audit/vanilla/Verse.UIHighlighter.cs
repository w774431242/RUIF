using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000BEC RID: 3052
	[StaticConstructorOnStartup]
	public static class UIHighlighter
	{
		// Token: 0x06004E1F RID: 19999 RVA: 0x0018D148 File Offset: 0x0018B348
		public static void HighlightTag(string tag)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			if (tag.NullOrEmpty())
			{
				return;
			}
			for (int i = 0; i < UIHighlighter.liveTags.Count; i++)
			{
				if (UIHighlighter.liveTags[i].First == tag && UIHighlighter.liveTags[i].Second == Time.frameCount)
				{
					return;
				}
			}
			UIHighlighter.liveTags.Add(new Pair<string, int>(tag, Time.frameCount));
		}

		// Token: 0x06004E20 RID: 20000 RVA: 0x0018D1CC File Offset: 0x0018B3CC
		public static void HighlightOpportunity(Rect rect, string tag)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			for (int i = 0; i < UIHighlighter.liveTags.Count; i++)
			{
				Pair<string, int> pair = UIHighlighter.liveTags[i];
				if (tag == pair.First && Time.frameCount == pair.Second + 1)
				{
					Widgets.DrawTextHighlight(rect, 0f, new Color?(Color.white.ToTransparent(Pulser.PulseBrightness(1.2f, 0.7f))));
				}
			}
		}

		// Token: 0x06004E21 RID: 20001 RVA: 0x0018D250 File Offset: 0x0018B450
		public static void UIHighlighterUpdate()
		{
			UIHighlighter.liveTags.RemoveAll((Pair<string, int> pair) => Time.frameCount > pair.Second + 1);
		}

		// Token: 0x04003798 RID: 14232
		private static List<Pair<string, int>> liveTags = new List<Pair<string, int>>();

		// Token: 0x04003799 RID: 14233
		private const float PulseFrequency = 1.2f;

		// Token: 0x0400379A RID: 14234
		private const float PulseAmplitude = 0.7f;
	}
}
