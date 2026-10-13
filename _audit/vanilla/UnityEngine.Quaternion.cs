using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	// Token: 0x020001F1 RID: 497
	[NativeType(Header = "Runtime/Math/Quaternion.h")]
	[UsedByNativeCode]
	[Il2CppEagerStaticClassConstruction]
	[NativeHeader("Runtime/Math/MathScripting.h")]
	public struct Quaternion : IEquatable<Quaternion>, IFormattable
	{
		// Token: 0x060015C3 RID: 5571 RVA: 0x0002254C File Offset: 0x0002074C
		[FreeFunction("FromToQuaternionSafe", IsThreadSafe = true)]
		public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
		{
			Quaternion quaternion;
			Quaternion.FromToRotation_Injected(ref fromDirection, ref toDirection, out quaternion);
			return quaternion;
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x00022568 File Offset: 0x00020768
		[FreeFunction(IsThreadSafe = true)]
		public static Quaternion Inverse(Quaternion rotation)
		{
			Quaternion quaternion;
			Quaternion.Inverse_Injected(ref rotation, out quaternion);
			return quaternion;
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x00022580 File Offset: 0x00020780
		[FreeFunction("QuaternionScripting::Slerp", IsThreadSafe = true)]
		public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
		{
			Quaternion quaternion;
			Quaternion.Slerp_Injected(ref a, ref b, t, out quaternion);
			return quaternion;
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x0002259C File Offset: 0x0002079C
		[FreeFunction("QuaternionScripting::SlerpUnclamped", IsThreadSafe = true)]
		public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, float t)
		{
			Quaternion quaternion;
			Quaternion.SlerpUnclamped_Injected(ref a, ref b, t, out quaternion);
			return quaternion;
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x000225B8 File Offset: 0x000207B8
		[FreeFunction("QuaternionScripting::Lerp", IsThreadSafe = true)]
		public static Quaternion Lerp(Quaternion a, Quaternion b, float t)
		{
			Quaternion quaternion;
			Quaternion.Lerp_Injected(ref a, ref b, t, out quaternion);
			return quaternion;
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x000225D4 File Offset: 0x000207D4
		[FreeFunction("QuaternionScripting::LerpUnclamped", IsThreadSafe = true)]
		public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, float t)
		{
			Quaternion quaternion;
			Quaternion.LerpUnclamped_Injected(ref a, ref b, t, out quaternion);
			return quaternion;
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x000225F0 File Offset: 0x000207F0
		[FreeFunction("EulerToQuaternion", IsThreadSafe = true)]
		private static Quaternion Internal_FromEulerRad(Vector3 euler)
		{
			Quaternion quaternion;
			Quaternion.Internal_FromEulerRad_Injected(ref euler, out quaternion);
			return quaternion;
		}

		// Token: 0x060015CA RID: 5578 RVA: 0x00022608 File Offset: 0x00020808
		[FreeFunction("QuaternionScripting::ToEuler", IsThreadSafe = true)]
		private static Vector3 Internal_ToEulerRad(Quaternion rotation)
		{
			Vector3 vector;
			Quaternion.Internal_ToEulerRad_Injected(ref rotation, out vector);
			return vector;
		}

		// Token: 0x060015CB RID: 5579 RVA: 0x0002261F File Offset: 0x0002081F
		[FreeFunction("QuaternionScripting::ToAxisAngle", IsThreadSafe = true)]
		private static void Internal_ToAxisAngleRad(Quaternion q, out Vector3 axis, out float angle)
		{
			Quaternion.Internal_ToAxisAngleRad_Injected(ref q, out axis, out angle);
		}

		// Token: 0x060015CC RID: 5580 RVA: 0x0002262C File Offset: 0x0002082C
		[FreeFunction("QuaternionScripting::AngleAxis", IsThreadSafe = true)]
		public static Quaternion AngleAxis(float angle, Vector3 axis)
		{
			Quaternion quaternion;
			Quaternion.AngleAxis_Injected(angle, ref axis, out quaternion);
			return quaternion;
		}

		// Token: 0x060015CD RID: 5581 RVA: 0x00022644 File Offset: 0x00020844
		[FreeFunction("QuaternionScripting::LookRotation", IsThreadSafe = true)]
		public static Quaternion LookRotation(Vector3 forward, [DefaultValue("Vector3.up")] Vector3 upwards)
		{
			Quaternion quaternion;
			Quaternion.LookRotation_Injected(ref forward, ref upwards, out quaternion);
			return quaternion;
		}

		// Token: 0x060015CE RID: 5582 RVA: 0x00022660 File Offset: 0x00020860
		[ExcludeFromDocs]
		public static Quaternion LookRotation(Vector3 forward)
		{
			return Quaternion.LookRotation(forward, Vector3.up);
		}

		// Token: 0x1700046A RID: 1130
		public float this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				float num;
				switch (index)
				{
				case 0:
					num = this.x;
					break;
				case 1:
					num = this.y;
					break;
				case 2:
					num = this.z;
					break;
				case 3:
					num = this.w;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Quaternion index!");
				}
				return num;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				case 2:
					this.z = value;
					break;
				case 3:
					this.w = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Quaternion index!");
				}
			}
		}

		// Token: 0x060015D1 RID: 5585 RVA: 0x00022735 File Offset: 0x00020935
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Quaternion(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		// Token: 0x060015D2 RID: 5586 RVA: 0x00022735 File Offset: 0x00020935
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(float newX, float newY, float newZ, float newW)
		{
			this.x = newX;
			this.y = newY;
			this.z = newZ;
			this.w = newW;
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x060015D3 RID: 5587 RVA: 0x00022758 File Offset: 0x00020958
		public static Quaternion identity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Quaternion.identityQuaternion;
			}
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x00022770 File Offset: 0x00020970
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
		{
			return new Quaternion(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x00022864 File Offset: 0x00020A64
		public static Vector3 operator *(Quaternion rotation, Vector3 point)
		{
			float num = rotation.x * 2f;
			float num2 = rotation.y * 2f;
			float num3 = rotation.z * 2f;
			float num4 = rotation.x * num;
			float num5 = rotation.y * num2;
			float num6 = rotation.z * num3;
			float num7 = rotation.x * num2;
			float num8 = rotation.x * num3;
			float num9 = rotation.y * num3;
			float num10 = rotation.w * num;
			float num11 = rotation.w * num2;
			float num12 = rotation.w * num3;
			Vector3 vector;
			vector.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
			vector.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
			vector.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
			return vector;
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x00022994 File Offset: 0x00020B94
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsEqualUsingDot(float dot)
		{
			return dot > 0.999999f;
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x000229B0 File Offset: 0x00020BB0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Quaternion lhs, Quaternion rhs)
		{
			return Quaternion.IsEqualUsingDot(Quaternion.Dot(lhs, rhs));
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x000229D0 File Offset: 0x00020BD0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Quaternion lhs, Quaternion rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x060015D9 RID: 5593 RVA: 0x000229EC File Offset: 0x00020BEC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Quaternion a, Quaternion b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		// Token: 0x060015DA RID: 5594 RVA: 0x00022A38 File Offset: 0x00020C38
		[ExcludeFromDocs]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetLookRotation(Vector3 view)
		{
			Vector3 up = Vector3.up;
			this.SetLookRotation(view, up);
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x00022A55 File Offset: 0x00020C55
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetLookRotation(Vector3 view, [DefaultValue("Vector3.up")] Vector3 up)
		{
			this = Quaternion.LookRotation(view, up);
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x00022A68 File Offset: 0x00020C68
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Angle(Quaternion a, Quaternion b)
		{
			float num = Mathf.Min(Mathf.Abs(Quaternion.Dot(a, b)), 1f);
			return Quaternion.IsEqualUsingDot(num) ? 0f : (Mathf.Acos(num) * 2f * 57.29578f);
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x00022AB4 File Offset: 0x00020CB4
		private static Vector3 Internal_MakePositive(Vector3 euler)
		{
			float num = -0.005729578f;
			float num2 = 360f + num;
			bool flag = euler.x < num;
			if (flag)
			{
				euler.x += 360f;
			}
			else
			{
				bool flag2 = euler.x > num2;
				if (flag2)
				{
					euler.x -= 360f;
				}
			}
			bool flag3 = euler.y < num;
			if (flag3)
			{
				euler.y += 360f;
			}
			else
			{
				bool flag4 = euler.y > num2;
				if (flag4)
				{
					euler.y -= 360f;
				}
			}
			bool flag5 = euler.z < num;
			if (flag5)
			{
				euler.z += 360f;
			}
			else
			{
				bool flag6 = euler.z > num2;
				if (flag6)
				{
					euler.z -= 360f;
				}
			}
			return euler;
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x060015DE RID: 5598 RVA: 0x00022B94 File Offset: 0x00020D94
		// (set) Token: 0x060015DF RID: 5599 RVA: 0x00022BC0 File Offset: 0x00020DC0
		public Vector3 eulerAngles
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Quaternion.Internal_MakePositive(Quaternion.Internal_ToEulerRad(this) * 57.29578f);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this = Quaternion.Internal_FromEulerRad(value * 0.017453292f);
			}
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x00022BDC File Offset: 0x00020DDC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Euler(float x, float y, float z)
		{
			return Quaternion.Internal_FromEulerRad(new Vector3(x, y, z) * 0.017453292f);
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x00022C08 File Offset: 0x00020E08
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Euler(Vector3 euler)
		{
			return Quaternion.Internal_FromEulerRad(euler * 0.017453292f);
		}

		// Token: 0x060015E2 RID: 5602 RVA: 0x00022C2A File Offset: 0x00020E2A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ToAngleAxis(out float angle, out Vector3 axis)
		{
			Quaternion.Internal_ToAxisAngleRad(this, out axis, out angle);
			angle *= 57.29578f;
		}

		// Token: 0x060015E3 RID: 5603 RVA: 0x00022C45 File Offset: 0x00020E45
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
		{
			this = Quaternion.FromToRotation(fromDirection, toDirection);
		}

		// Token: 0x060015E4 RID: 5604 RVA: 0x00022C58 File Offset: 0x00020E58
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
		{
			float num = Quaternion.Angle(from, to);
			bool flag = num == 0f;
			Quaternion quaternion;
			if (flag)
			{
				quaternion = to;
			}
			else
			{
				quaternion = Quaternion.SlerpUnclamped(from, to, Mathf.Min(1f, maxDegreesDelta / num));
			}
			return quaternion;
		}

		// Token: 0x060015E5 RID: 5605 RVA: 0x00022C98 File Offset: 0x00020E98
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Normalize(Quaternion q)
		{
			float num = Mathf.Sqrt(Quaternion.Dot(q, q));
			bool flag = num < Mathf.Epsilon;
			Quaternion quaternion;
			if (flag)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				quaternion = new Quaternion(q.x / num, q.y / num, q.z / num, q.w / num);
			}
			return quaternion;
		}

		// Token: 0x060015E6 RID: 5606 RVA: 0x00022CF0 File Offset: 0x00020EF0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalize()
		{
			this = Quaternion.Normalize(this);
		}

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x060015E7 RID: 5607 RVA: 0x00022D04 File Offset: 0x00020F04
		public Quaternion normalized
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Quaternion.Normalize(this);
			}
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x00022D24 File Offset: 0x00020F24
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ (this.y.GetHashCode() << 2) ^ (this.z.GetHashCode() >> 2) ^ (this.w.GetHashCode() >> 1);
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x00022D6C File Offset: 0x00020F6C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object other)
		{
			bool flag = !(other is Quaternion);
			return !flag && this.Equals((Quaternion)other);
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x00022DA0 File Offset: 0x00020FA0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Quaternion other)
		{
			return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x00022E00 File Offset: 0x00021000
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return this.ToString(null, null);
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x00022E1C File Offset: 0x0002101C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x00022E38 File Offset: 0x00021038
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			bool flag = string.IsNullOrEmpty(format);
			if (flag)
			{
				format = "F5";
			}
			bool flag2 = formatProvider == null;
			if (flag2)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return UnityString.Format("({0}, {1}, {2}, {3})", new object[]
			{
				this.x.ToString(format, formatProvider),
				this.y.ToString(format, formatProvider),
				this.z.ToString(format, formatProvider),
				this.w.ToString(format, formatProvider)
			});
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x00022EC0 File Offset: 0x000210C0
		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion EulerRotation(float x, float y, float z)
		{
			return Quaternion.Internal_FromEulerRad(new Vector3(x, y, z));
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x00022EE0 File Offset: 0x000210E0
		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion EulerRotation(Vector3 euler)
		{
			return Quaternion.Internal_FromEulerRad(euler);
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x00022EF8 File Offset: 0x000210F8
		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetEulerRotation(float x, float y, float z)
		{
			this = Quaternion.Internal_FromEulerRad(new Vector3(x, y, z));
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x00022F0E File Offset: 0x0002110E
		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetEulerRotation(Vector3 euler)
		{
			this = Quaternion.Internal_FromEulerRad(euler);
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x00022F20 File Offset: 0x00021120
		[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 ToEuler()
		{
			return Quaternion.Internal_ToEulerRad(this);
		}

		// Token: 0x060015F3 RID: 5619 RVA: 0x00022F40 File Offset: 0x00021140
		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion EulerAngles(float x, float y, float z)
		{
			return Quaternion.Internal_FromEulerRad(new Vector3(x, y, z));
		}

		// Token: 0x060015F4 RID: 5620 RVA: 0x00022F60 File Offset: 0x00021160
		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion EulerAngles(Vector3 euler)
		{
			return Quaternion.Internal_FromEulerRad(euler);
		}

		// Token: 0x060015F5 RID: 5621 RVA: 0x00022F78 File Offset: 0x00021178
		[Obsolete("Use Quaternion.ToAngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ToAxisAngle(out Vector3 axis, out float angle)
		{
			Quaternion.Internal_ToAxisAngleRad(this, out axis, out angle);
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x00022F89 File Offset: 0x00021189
		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetEulerAngles(float x, float y, float z)
		{
			this.SetEulerRotation(new Vector3(x, y, z));
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x00022F9B File Offset: 0x0002119B
		[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetEulerAngles(Vector3 euler)
		{
			this = Quaternion.EulerRotation(euler);
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x00022FAC File Offset: 0x000211AC
		[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 ToEulerAngles(Quaternion rotation)
		{
			return Quaternion.Internal_ToEulerRad(rotation);
		}

		// Token: 0x060015F9 RID: 5625 RVA: 0x00022FC4 File Offset: 0x000211C4
		[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 ToEulerAngles()
		{
			return Quaternion.Internal_ToEulerRad(this);
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x00022FE1 File Offset: 0x000211E1
		[Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetAxisAngle(Vector3 axis, float angle)
		{
			this = Quaternion.AxisAngle(axis, angle);
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x00022FF4 File Offset: 0x000211F4
		[Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion AxisAngle(Vector3 axis, float angle)
		{
			return Quaternion.AngleAxis(57.29578f * angle, axis);
		}

		// Token: 0x060015FD RID: 5629
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FromToRotation_Injected(ref Vector3 fromDirection, ref Vector3 toDirection, out Quaternion ret);

		// Token: 0x060015FE RID: 5630
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Inverse_Injected(ref Quaternion rotation, out Quaternion ret);

		// Token: 0x060015FF RID: 5631
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Slerp_Injected(ref Quaternion a, ref Quaternion b, float t, out Quaternion ret);

		// Token: 0x06001600 RID: 5632
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SlerpUnclamped_Injected(ref Quaternion a, ref Quaternion b, float t, out Quaternion ret);

		// Token: 0x06001601 RID: 5633
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Lerp_Injected(ref Quaternion a, ref Quaternion b, float t, out Quaternion ret);

		// Token: 0x06001602 RID: 5634
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LerpUnclamped_Injected(ref Quaternion a, ref Quaternion b, float t, out Quaternion ret);

		// Token: 0x06001603 RID: 5635
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_FromEulerRad_Injected(ref Vector3 euler, out Quaternion ret);

		// Token: 0x06001604 RID: 5636
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ToEulerRad_Injected(ref Quaternion rotation, out Vector3 ret);

		// Token: 0x06001605 RID: 5637
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ToAxisAngleRad_Injected(ref Quaternion q, out Vector3 axis, out float angle);

		// Token: 0x06001606 RID: 5638
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AngleAxis_Injected(float angle, ref Vector3 axis, out Quaternion ret);

		// Token: 0x06001607 RID: 5639
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LookRotation_Injected(ref Vector3 forward, [DefaultValue("Vector3.up")] ref Vector3 upwards, out Quaternion ret);

		// Token: 0x0400080A RID: 2058
		public float x;

		// Token: 0x0400080B RID: 2059
		public float y;

		// Token: 0x0400080C RID: 2060
		public float z;

		// Token: 0x0400080D RID: 2061
		public float w;

		// Token: 0x0400080E RID: 2062
		private static readonly Quaternion identityQuaternion = new Quaternion(0f, 0f, 0f, 1f);

		// Token: 0x0400080F RID: 2063
		public const float kEpsilon = 1E-06f;
	}
}
