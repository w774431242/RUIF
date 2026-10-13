using System;
using System.Text;
using RimWorld;
using RimWorld.Planet;

namespace Verse
{
	// Token: 0x02000367 RID: 871
	public struct NamedArgument
	{
		// Token: 0x06001661 RID: 5729 RVA: 0x0006DC79 File Offset: 0x0006BE79
		public NamedArgument(object arg, string label)
		{
			this.arg = arg;
			this.label = label;
		}

		// Token: 0x06001662 RID: 5730 RVA: 0x0006DC89 File Offset: 0x0006BE89
		public static implicit operator NamedArgument(int value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001663 RID: 5731 RVA: 0x0006DC97 File Offset: 0x0006BE97
		public static implicit operator NamedArgument(char value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001664 RID: 5732 RVA: 0x0006DCA5 File Offset: 0x0006BEA5
		public static implicit operator NamedArgument(float value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001665 RID: 5733 RVA: 0x0006DCB3 File Offset: 0x0006BEB3
		public static implicit operator NamedArgument(double value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001666 RID: 5734 RVA: 0x0006DCC1 File Offset: 0x0006BEC1
		public static implicit operator NamedArgument(long value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001667 RID: 5735 RVA: 0x0006DCCF File Offset: 0x0006BECF
		public static implicit operator NamedArgument(string value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x0006DCD8 File Offset: 0x0006BED8
		public static implicit operator NamedArgument(uint value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x0006DCE6 File Offset: 0x0006BEE6
		public static implicit operator NamedArgument(byte value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x0006DCF4 File Offset: 0x0006BEF4
		public static implicit operator NamedArgument(ulong value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x0006DCCF File Offset: 0x0006BECF
		public static implicit operator NamedArgument(StringBuilder value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x0006DCCF File Offset: 0x0006BECF
		public static implicit operator NamedArgument(Thing value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x0600166D RID: 5741 RVA: 0x0006DCCF File Offset: 0x0006BECF
		public static implicit operator NamedArgument(Def value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x0006DCCF File Offset: 0x0006BECF
		public static implicit operator NamedArgument(WorldObject value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x0006DCCF File Offset: 0x0006BECF
		public static implicit operator NamedArgument(Faction value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001670 RID: 5744 RVA: 0x0006DD02 File Offset: 0x0006BF02
		public static implicit operator NamedArgument(IntVec3 value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x0006DD10 File Offset: 0x0006BF10
		public static implicit operator NamedArgument(LocalTargetInfo value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001672 RID: 5746 RVA: 0x0006DD1E File Offset: 0x0006BF1E
		public static implicit operator NamedArgument(TargetInfo value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001673 RID: 5747 RVA: 0x0006DD2C File Offset: 0x0006BF2C
		public static implicit operator NamedArgument(GlobalTargetInfo value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001674 RID: 5748 RVA: 0x0006DCCF File Offset: 0x0006BECF
		public static implicit operator NamedArgument(Map value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x0006DD3A File Offset: 0x0006BF3A
		public static implicit operator NamedArgument(TaggedString value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x0006DCCF File Offset: 0x0006BECF
		public static implicit operator NamedArgument(Ideo value)
		{
			return new NamedArgument(value, null);
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x0006DD48 File Offset: 0x0006BF48
		public override string ToString()
		{
			return (this.label.NullOrEmpty() ? "unnamed" : this.label) + "->" + this.arg.ToStringSafe<object>();
		}

		// Token: 0x0400148D RID: 5261
		public object arg;

		// Token: 0x0400148E RID: 5262
		public string label;
	}
}
