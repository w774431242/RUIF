using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngineInternal;

namespace UnityEngine
{
	// Token: 0x020001F2 RID: 498
	[NativeHeader("Runtime/Math/FloatConversion.h")]
	[NativeHeader("Runtime/Math/PerlinNoise.h")]
	[Il2CppEagerStaticClassConstruction]
	[NativeHeader("Runtime/Utilities/BitUtility.h")]
	[NativeHeader("Runtime/Math/ColorSpaceConversion.h")]
	public struct Mathf
	{
		// Token: 0x06001608 RID: 5640
		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ClosestPowerOfTwo(int value);

		// Token: 0x06001609 RID: 5641
		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsPowerOfTwo(int value);

		// Token: 0x0600160A RID: 5642
		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int NextPowerOfTwo(int value);

		// Token: 0x0600160B RID: 5643
		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GammaToLinearSpace(float value);

		// Token: 0x0600160C RID: 5644
		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float LinearToGammaSpace(float value);

		// Token: 0x0600160D RID: 5645 RVA: 0x00023034 File Offset: 0x00021234
		[FreeFunction(IsThreadSafe = true)]
		public static Color CorrelatedColorTemperatureToRGB(float kelvin)
		{
			Color color;
			Mathf.CorrelatedColorTemperatureToRGB_Injected(kelvin, out color);
			return color;
		}

		// Token: 0x0600160E RID: 5646
		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ushort FloatToHalf(float val);

		// Token: 0x0600160F RID: 5647
		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float HalfToFloat(ushort val);

		// Token: 0x06001610 RID: 5648
		[FreeFunction("PerlinNoise::NoiseNormalized", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float PerlinNoise(float x, float y);

		// Token: 0x06001611 RID: 5649
		[FreeFunction("PerlinNoise::NoiseNormalized", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float PerlinNoise1D(float x);

		// Token: 0x06001612 RID: 5650 RVA: 0x0002304C File Offset: 0x0002124C
		public static float Sin(float f)
		{
			return (float)Math.Sin((double)f);
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x00023068 File Offset: 0x00021268
		public static float Cos(float f)
		{
			return (float)Math.Cos((double)f);
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x00023084 File Offset: 0x00021284
		public static float Tan(float f)
		{
			return (float)Math.Tan((double)f);
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x000230A0 File Offset: 0x000212A0
		public static float Asin(float f)
		{
			return (float)Math.Asin((double)f);
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x000230BC File Offset: 0x000212BC
		public static float Acos(float f)
		{
			return (float)Math.Acos((double)f);
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x000230D8 File Offset: 0x000212D8
		public static float Atan(float f)
		{
			return (float)Math.Atan((double)f);
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x000230F4 File Offset: 0x000212F4
		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2((double)y, (double)x);
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x00023110 File Offset: 0x00021310
		public static float Sqrt(float f)
		{
			return (float)Math.Sqrt((double)f);
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x0002312C File Offset: 0x0002132C
		public static float Abs(float f)
		{
			return Math.Abs(f);
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x00023144 File Offset: 0x00021344
		public static int Abs(int value)
		{
			return Math.Abs(value);
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x0002315C File Offset: 0x0002135C
		public static float Min(float a, float b)
		{
			return (a < b) ? a : b;
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x00023178 File Offset: 0x00021378
		public static float Min(params float[] values)
		{
			int num = values.Length;
			bool flag = num == 0;
			float num2;
			if (flag)
			{
				num2 = 0f;
			}
			else
			{
				float num3 = values[0];
				for (int i = 1; i < num; i++)
				{
					bool flag2 = values[i] < num3;
					if (flag2)
					{
						num3 = values[i];
					}
				}
				num2 = num3;
			}
			return num2;
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x000231D0 File Offset: 0x000213D0
		public static int Min(int a, int b)
		{
			return (a < b) ? a : b;
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x000231EC File Offset: 0x000213EC
		public static int Min(params int[] values)
		{
			int num = values.Length;
			bool flag = num == 0;
			int num2;
			if (flag)
			{
				num2 = 0;
			}
			else
			{
				int num3 = values[0];
				for (int i = 1; i < num; i++)
				{
					bool flag2 = values[i] < num3;
					if (flag2)
					{
						num3 = values[i];
					}
				}
				num2 = num3;
			}
			return num2;
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x00023240 File Offset: 0x00021440
		public static float Max(float a, float b)
		{
			return (a > b) ? a : b;
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x0002325C File Offset: 0x0002145C
		public static float Max(params float[] values)
		{
			int num = values.Length;
			bool flag = num == 0;
			float num2;
			if (flag)
			{
				num2 = 0f;
			}
			else
			{
				float num3 = values[0];
				for (int i = 1; i < num; i++)
				{
					bool flag2 = values[i] > num3;
					if (flag2)
					{
						num3 = values[i];
					}
				}
				num2 = num3;
			}
			return num2;
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x000232B4 File Offset: 0x000214B4
		public static int Max(int a, int b)
		{
			return (a > b) ? a : b;
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x000232D0 File Offset: 0x000214D0
		public static int Max(params int[] values)
		{
			int num = values.Length;
			bool flag = num == 0;
			int num2;
			if (flag)
			{
				num2 = 0;
			}
			else
			{
				int num3 = values[0];
				for (int i = 1; i < num; i++)
				{
					bool flag2 = values[i] > num3;
					if (flag2)
					{
						num3 = values[i];
					}
				}
				num2 = num3;
			}
			return num2;
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x00023324 File Offset: 0x00021524
		public static float Pow(float f, float p)
		{
			return (float)Math.Pow((double)f, (double)p);
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x00023340 File Offset: 0x00021540
		public static float Exp(float power)
		{
			return (float)Math.Exp((double)power);
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x0002335C File Offset: 0x0002155C
		public static float Log(float f, float p)
		{
			return (float)Math.Log((double)f, (double)p);
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x00023378 File Offset: 0x00021578
		public static float Log(float f)
		{
			return (float)Math.Log((double)f);
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x00023394 File Offset: 0x00021594
		public static float Log10(float f)
		{
			return (float)Math.Log10((double)f);
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x000233B0 File Offset: 0x000215B0
		public static float Ceil(float f)
		{
			return (float)Math.Ceiling((double)f);
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x000233CC File Offset: 0x000215CC
		public static float Floor(float f)
		{
			return (float)Math.Floor((double)f);
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x000233E8 File Offset: 0x000215E8
		public static float Round(float f)
		{
			return (float)Math.Round((double)f);
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x00023404 File Offset: 0x00021604
		public static int CeilToInt(float f)
		{
			return (int)Math.Ceiling((double)f);
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x00023420 File Offset: 0x00021620
		public static int FloorToInt(float f)
		{
			return (int)Math.Floor((double)f);
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x0002343C File Offset: 0x0002163C
		public static int RoundToInt(float f)
		{
			return (int)Math.Round((double)f);
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x00023458 File Offset: 0x00021658
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sign(float f)
		{
			return (f >= 0f) ? 1f : (-1f);
		}

		// Token: 0x06001630 RID: 5680 RVA: 0x00023480 File Offset: 0x00021680
		public static float Clamp(float value, float min, float max)
		{
			bool flag = value < min;
			if (flag)
			{
				value = min;
			}
			else
			{
				bool flag2 = value > max;
				if (flag2)
				{
					value = max;
				}
			}
			return value;
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x000234AC File Offset: 0x000216AC
		public static int Clamp(int value, int min, int max)
		{
			bool flag = value < min;
			if (flag)
			{
				value = min;
			}
			else
			{
				bool flag2 = value > max;
				if (flag2)
				{
					value = max;
				}
			}
			return value;
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x000234D8 File Offset: 0x000216D8
		public static float Clamp01(float value)
		{
			bool flag = value < 0f;
			float num;
			if (flag)
			{
				num = 0f;
			}
			else
			{
				bool flag2 = value > 1f;
				if (flag2)
				{
					num = 1f;
				}
				else
				{
					num = value;
				}
			}
			return num;
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x00023514 File Offset: 0x00021714
		public static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * Mathf.Clamp01(t);
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x00023534 File Offset: 0x00021734
		public static float LerpUnclamped(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x00023550 File Offset: 0x00021750
		public static float LerpAngle(float a, float b, float t)
		{
			float num = Mathf.Repeat(b - a, 360f);
			bool flag = num > 180f;
			if (flag)
			{
				num -= 360f;
			}
			return a + num * Mathf.Clamp01(t);
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x00023590 File Offset: 0x00021790
		public static float MoveTowards(float current, float target, float maxDelta)
		{
			bool flag = Mathf.Abs(target - current) <= maxDelta;
			float num;
			if (flag)
			{
				num = target;
			}
			else
			{
				num = current + Mathf.Sign(target - current) * maxDelta;
			}
			return num;
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x000235C4 File Offset: 0x000217C4
		public static float MoveTowardsAngle(float current, float target, float maxDelta)
		{
			float num = Mathf.DeltaAngle(current, target);
			bool flag = -maxDelta < num && num < maxDelta;
			float num2;
			if (flag)
			{
				num2 = target;
			}
			else
			{
				target = current + num;
				num2 = Mathf.MoveTowards(current, target, maxDelta);
			}
			return num2;
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x00023600 File Offset: 0x00021800
		public static float SmoothStep(float from, float to, float t)
		{
			t = Mathf.Clamp01(t);
			t = -2f * t * t * t + 3f * t * t;
			return to * t + from * (1f - t);
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x00023640 File Offset: 0x00021840
		public static float Gamma(float value, float absmax, float gamma)
		{
			bool flag = value < 0f;
			float num = Mathf.Abs(value);
			bool flag2 = num > absmax;
			float num2;
			if (flag2)
			{
				num2 = (flag ? (-num) : num);
			}
			else
			{
				float num3 = Mathf.Pow(num / absmax, gamma) * absmax;
				num2 = (flag ? (-num3) : num3);
			}
			return num2;
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x0002368C File Offset: 0x0002188C
		public static bool Approximately(float a, float b)
		{
			return Mathf.Abs(b - a) < Mathf.Max(1E-06f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8f);
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x000236D0 File Offset: 0x000218D0
		[ExcludeFromDocs]
		public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed)
		{
			float deltaTime = Time.deltaTime;
			return Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		// Token: 0x0600163C RID: 5692 RVA: 0x000236F4 File Offset: 0x000218F4
		[ExcludeFromDocs]
		public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime)
		{
			float deltaTime = Time.deltaTime;
			float positiveInfinity = float.PositiveInfinity;
			return Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x00023720 File Offset: 0x00021920
		public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
		{
			smoothTime = Mathf.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			float num4 = current - target;
			float num5 = target;
			float num6 = maxSpeed * smoothTime;
			num4 = Mathf.Clamp(num4, -num6, num6);
			target = current - num4;
			float num7 = (currentVelocity + num * num4) * deltaTime;
			currentVelocity = (currentVelocity - num * num7) * num3;
			float num8 = target + (num4 + num7) * num3;
			bool flag = num5 - current > 0f == num8 > num5;
			if (flag)
			{
				num8 = num5;
				currentVelocity = (num8 - num5) / deltaTime;
			}
			return num8;
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x000237DC File Offset: 0x000219DC
		[ExcludeFromDocs]
		public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed)
		{
			float deltaTime = Time.deltaTime;
			return Mathf.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x00023800 File Offset: 0x00021A00
		[ExcludeFromDocs]
		public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime)
		{
			float deltaTime = Time.deltaTime;
			float positiveInfinity = float.PositiveInfinity;
			return Mathf.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x0002382C File Offset: 0x00021A2C
		public static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
		{
			target = current + Mathf.DeltaAngle(current, target);
			return Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x00023858 File Offset: 0x00021A58
		public static float Repeat(float t, float length)
		{
			return Mathf.Clamp(t - Mathf.Floor(t / length) * length, 0f, length);
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x00023884 File Offset: 0x00021A84
		public static float PingPong(float t, float length)
		{
			t = Mathf.Repeat(t, length * 2f);
			return length - Mathf.Abs(t - length);
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x000238B0 File Offset: 0x00021AB0
		public static float InverseLerp(float a, float b, float value)
		{
			bool flag = a != b;
			float num;
			if (flag)
			{
				num = Mathf.Clamp01((value - a) / (b - a));
			}
			else
			{
				num = 0f;
			}
			return num;
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x000238E4 File Offset: 0x00021AE4
		public static float DeltaAngle(float current, float target)
		{
			float num = Mathf.Repeat(target - current, 360f);
			bool flag = num > 180f;
			if (flag)
			{
				num -= 360f;
			}
			return num;
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x0002391C File Offset: 0x00021B1C
		internal static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
		{
			float num = p2.x - p1.x;
			float num2 = p2.y - p1.y;
			float num3 = p4.x - p3.x;
			float num4 = p4.y - p3.y;
			float num5 = num * num4 - num2 * num3;
			bool flag = num5 == 0f;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				float num6 = p3.x - p1.x;
				float num7 = p3.y - p1.y;
				float num8 = (num6 * num4 - num7 * num3) / num5;
				result.x = p1.x + num8 * num;
				result.y = p1.y + num8 * num2;
				flag2 = true;
			}
			return flag2;
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x000239D8 File Offset: 0x00021BD8
		internal static bool LineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 result)
		{
			float num = p2.x - p1.x;
			float num2 = p2.y - p1.y;
			float num3 = p4.x - p3.x;
			float num4 = p4.y - p3.y;
			float num5 = num * num4 - num2 * num3;
			bool flag = num5 == 0f;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				float num6 = p3.x - p1.x;
				float num7 = p3.y - p1.y;
				float num8 = (num6 * num4 - num7 * num3) / num5;
				bool flag3 = num8 < 0f || num8 > 1f;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					float num9 = (num6 * num2 - num7 * num) / num5;
					bool flag4 = num9 < 0f || num9 > 1f;
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						result.x = p1.x + num8 * num;
						result.y = p1.y + num8 * num2;
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x00023AE8 File Offset: 0x00021CE8
		internal static long RandomToLong(Random r)
		{
			byte[] array = new byte[8];
			r.NextBytes(array);
			return (long)(BitConverter.ToUInt64(array, 0) & 9223372036854775807UL);
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x00023B1C File Offset: 0x00021D1C
		internal static float ClampToFloat(double value)
		{
			bool flag = double.IsPositiveInfinity(value);
			float num;
			if (flag)
			{
				num = float.PositiveInfinity;
			}
			else
			{
				bool flag2 = double.IsNegativeInfinity(value);
				if (flag2)
				{
					num = float.NegativeInfinity;
				}
				else
				{
					bool flag3 = value < -3.4028234663852886E+38;
					if (flag3)
					{
						num = float.MinValue;
					}
					else
					{
						bool flag4 = value > 3.4028234663852886E+38;
						if (flag4)
						{
							num = float.MaxValue;
						}
						else
						{
							num = (float)value;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x00023B88 File Offset: 0x00021D88
		internal static int ClampToInt(long value)
		{
			bool flag = value < -2147483648L;
			int num;
			if (flag)
			{
				num = int.MinValue;
			}
			else
			{
				bool flag2 = value > 2147483647L;
				if (flag2)
				{
					num = int.MaxValue;
				}
				else
				{
					num = (int)value;
				}
			}
			return num;
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x00023BC8 File Offset: 0x00021DC8
		internal static uint ClampToUInt(long value)
		{
			bool flag = value < 0L;
			uint num;
			if (flag)
			{
				num = 0U;
			}
			else
			{
				bool flag2 = value > (long)((ulong)(-1));
				if (flag2)
				{
					num = uint.MaxValue;
				}
				else
				{
					num = (uint)value;
				}
			}
			return num;
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x00023BF8 File Offset: 0x00021DF8
		internal static float RoundToMultipleOf(float value, float roundingValue)
		{
			bool flag = roundingValue == 0f;
			float num;
			if (flag)
			{
				num = value;
			}
			else
			{
				num = Mathf.Round(value / roundingValue) * roundingValue;
			}
			return num;
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x00023C24 File Offset: 0x00021E24
		internal static float GetClosestPowerOfTen(float positiveNumber)
		{
			bool flag = positiveNumber <= 0f;
			float num;
			if (flag)
			{
				num = 1f;
			}
			else
			{
				num = Mathf.Pow(10f, (float)Mathf.RoundToInt(Mathf.Log10(positiveNumber)));
			}
			return num;
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x00023C64 File Offset: 0x00021E64
		internal static int GetNumberOfDecimalsForMinimumDifference(float minDifference)
		{
			return Mathf.Clamp(-Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(minDifference))), 0, 15);
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x00023C90 File Offset: 0x00021E90
		internal static int GetNumberOfDecimalsForMinimumDifference(double minDifference)
		{
			return (int)Math.Max(0.0, -Math.Floor(Math.Log10(Math.Abs(minDifference))));
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x00023CC4 File Offset: 0x00021EC4
		internal static float RoundBasedOnMinimumDifference(float valueToRound, float minDifference)
		{
			bool flag = minDifference == 0f;
			float num;
			if (flag)
			{
				num = Mathf.DiscardLeastSignificantDecimal(valueToRound);
			}
			else
			{
				num = (float)Math.Round((double)valueToRound, Mathf.GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
			}
			return num;
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x00023CFC File Offset: 0x00021EFC
		internal static double RoundBasedOnMinimumDifference(double valueToRound, double minDifference)
		{
			bool flag = minDifference == 0.0;
			double num;
			if (flag)
			{
				num = Mathf.DiscardLeastSignificantDecimal(valueToRound);
			}
			else
			{
				num = Math.Round(valueToRound, Mathf.GetNumberOfDecimalsForMinimumDifference(minDifference), MidpointRounding.AwayFromZero);
			}
			return num;
		}

		// Token: 0x06001651 RID: 5713 RVA: 0x00023D34 File Offset: 0x00021F34
		internal static float DiscardLeastSignificantDecimal(float v)
		{
			int num = Mathf.Clamp((int)(5f - Mathf.Log10(Mathf.Abs(v))), 0, 15);
			return (float)Math.Round((double)v, num, MidpointRounding.AwayFromZero);
		}

		// Token: 0x06001652 RID: 5714 RVA: 0x00023D6C File Offset: 0x00021F6C
		internal static double DiscardLeastSignificantDecimal(double v)
		{
			int num = Math.Max(0, (int)(5.0 - Math.Log10(Math.Abs(v))));
			double num2;
			try
			{
				num2 = Math.Round(v, num);
			}
			catch (ArgumentOutOfRangeException)
			{
				num2 = 0.0;
			}
			return num2;
		}

		// Token: 0x06001654 RID: 5716
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CorrelatedColorTemperatureToRGB_Injected(float kelvin, out Color ret);

		// Token: 0x04000810 RID: 2064
		public const float PI = 3.1415927f;

		// Token: 0x04000811 RID: 2065
		public const float Infinity = float.PositiveInfinity;

		// Token: 0x04000812 RID: 2066
		public const float NegativeInfinity = float.NegativeInfinity;

		// Token: 0x04000813 RID: 2067
		public const float Deg2Rad = 0.017453292f;

		// Token: 0x04000814 RID: 2068
		public const float Rad2Deg = 57.29578f;

		// Token: 0x04000815 RID: 2069
		internal const int kMaxDecimals = 15;

		// Token: 0x04000816 RID: 2070
		public static readonly float Epsilon = (MathfInternal.IsFlushToZeroEnabled ? MathfInternal.FloatMinNormal : MathfInternal.FloatMinDenormal);
	}
}
