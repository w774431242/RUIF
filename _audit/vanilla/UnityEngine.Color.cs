using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	// Token: 0x020001E7 RID: 487
	[NativeHeader("Runtime/Math/Color.h")]
	[NativeClass("ColorRGBAf")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	public struct Color : IEquatable<Color>, IFormattable
	{
		// Token: 0x060014E7 RID: 5351 RVA: 0x0001E92B File Offset: 0x0001CB2B
		public Color(float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x0001E94B File Offset: 0x0001CB4B
		public Color(float r, float g, float b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 1f;
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x0001E970 File Offset: 0x0001CB70
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return this.ToString(null, null);
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x0001E98C File Offset: 0x0001CB8C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x0001E9A8 File Offset: 0x0001CBA8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format, IFormatProvider formatProvider)
		{
			bool flag = string.IsNullOrEmpty(format);
			if (flag)
			{
				format = "F3";
			}
			bool flag2 = formatProvider == null;
			if (flag2)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return UnityString.Format("RGBA({0}, {1}, {2}, {3})", new object[]
			{
				this.r.ToString(format, formatProvider),
				this.g.ToString(format, formatProvider),
				this.b.ToString(format, formatProvider),
				this.a.ToString(format, formatProvider)
			});
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x0001EA30 File Offset: 0x0001CC30
		public override int GetHashCode()
		{
			return this.GetHashCode();
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x0001EA5C File Offset: 0x0001CC5C
		public override bool Equals(object other)
		{
			bool flag = !(other is Color);
			return !flag && this.Equals((Color)other);
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x0001EA90 File Offset: 0x0001CC90
		public bool Equals(Color other)
		{
			return this.r.Equals(other.r) && this.g.Equals(other.g) && this.b.Equals(other.b) && this.a.Equals(other.a);
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x0001EAF0 File Offset: 0x0001CCF0
		public static Color operator +(Color a, Color b)
		{
			return new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x0001EB3C File Offset: 0x0001CD3C
		public static Color operator -(Color a, Color b)
		{
			return new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x0001EB88 File Offset: 0x0001CD88
		public static Color operator *(Color a, Color b)
		{
			return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x0001EBD4 File Offset: 0x0001CDD4
		public static Color operator *(Color a, float b)
		{
			return new Color(a.r * b, a.g * b, a.b * b, a.a * b);
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x0001EC0C File Offset: 0x0001CE0C
		public static Color operator *(float b, Color a)
		{
			return new Color(a.r * b, a.g * b, a.b * b, a.a * b);
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x0001EC44 File Offset: 0x0001CE44
		public static Color operator /(Color a, float b)
		{
			return new Color(a.r / b, a.g / b, a.b / b, a.a / b);
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x0001EC7C File Offset: 0x0001CE7C
		public static bool operator ==(Color lhs, Color rhs)
		{
			return lhs == rhs;
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x0001ECA0 File Offset: 0x0001CEA0
		public static bool operator !=(Color lhs, Color rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x0001ECBC File Offset: 0x0001CEBC
		public static Color Lerp(Color a, Color b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x0001ED34 File Offset: 0x0001CF34
		public static Color LerpUnclamped(Color a, Color b, float t)
		{
			return new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x0001EDA4 File Offset: 0x0001CFA4
		internal Color RGBMultiplied(float multiplier)
		{
			return new Color(this.r * multiplier, this.g * multiplier, this.b * multiplier, this.a);
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x0001EDDC File Offset: 0x0001CFDC
		internal Color AlphaMultiplied(float multiplier)
		{
			return new Color(this.r, this.g, this.b, this.a * multiplier);
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x0001EE10 File Offset: 0x0001D010
		internal Color RGBMultiplied(Color multiplier)
		{
			return new Color(this.r * multiplier.r, this.g * multiplier.g, this.b * multiplier.b, this.a);
		}

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x060014FC RID: 5372 RVA: 0x0001EE54 File Offset: 0x0001D054
		public static Color red
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0f, 0f, 1f);
			}
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x060014FD RID: 5373 RVA: 0x0001EE80 File Offset: 0x0001D080
		public static Color green
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 1f, 0f, 1f);
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x060014FE RID: 5374 RVA: 0x0001EEAC File Offset: 0x0001D0AC
		public static Color blue
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0f, 1f, 1f);
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x060014FF RID: 5375 RVA: 0x0001EED8 File Offset: 0x0001D0D8
		public static Color white
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 1f, 1f, 1f);
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06001500 RID: 5376 RVA: 0x0001EF04 File Offset: 0x0001D104
		public static Color black
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0f, 0f, 1f);
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06001501 RID: 5377 RVA: 0x0001EF30 File Offset: 0x0001D130
		public static Color yellow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0.92156863f, 0.015686275f, 1f);
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06001502 RID: 5378 RVA: 0x0001EF5C File Offset: 0x0001D15C
		public static Color cyan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 1f, 1f, 1f);
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06001503 RID: 5379 RVA: 0x0001EF88 File Offset: 0x0001D188
		public static Color magenta
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(1f, 0f, 1f, 1f);
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06001504 RID: 5380 RVA: 0x0001EFB4 File Offset: 0x0001D1B4
		public static Color gray
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5f, 0.5f, 0.5f, 1f);
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06001505 RID: 5381 RVA: 0x0001EFE0 File Offset: 0x0001D1E0
		public static Color grey
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0.5f, 0.5f, 0.5f, 1f);
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06001506 RID: 5382 RVA: 0x0001F00C File Offset: 0x0001D20C
		public static Color clear
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Color(0f, 0f, 0f, 0f);
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06001507 RID: 5383 RVA: 0x0001F038 File Offset: 0x0001D238
		public float grayscale
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return 0.299f * this.r + 0.587f * this.g + 0.114f * this.b;
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06001508 RID: 5384 RVA: 0x0001F070 File Offset: 0x0001D270
		public Color linear
		{
			get
			{
				return new Color(Mathf.GammaToLinearSpace(this.r), Mathf.GammaToLinearSpace(this.g), Mathf.GammaToLinearSpace(this.b), this.a);
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06001509 RID: 5385 RVA: 0x0001F0B0 File Offset: 0x0001D2B0
		public Color gamma
		{
			get
			{
				return new Color(Mathf.LinearToGammaSpace(this.r), Mathf.LinearToGammaSpace(this.g), Mathf.LinearToGammaSpace(this.b), this.a);
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x0600150A RID: 5386 RVA: 0x0001F0F0 File Offset: 0x0001D2F0
		public float maxColorComponent
		{
			get
			{
				return Mathf.Max(Mathf.Max(this.r, this.g), this.b);
			}
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x0001F120 File Offset: 0x0001D320
		public static implicit operator Vector4(Color c)
		{
			return new Vector4(c.r, c.g, c.b, c.a);
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x0001F150 File Offset: 0x0001D350
		public static implicit operator Color(Vector4 v)
		{
			return new Color(v.x, v.y, v.z, v.w);
		}

		// Token: 0x1700044A RID: 1098
		public float this[int index]
		{
			get
			{
				float num;
				switch (index)
				{
				case 0:
					num = this.r;
					break;
				case 1:
					num = this.g;
					break;
				case 2:
					num = this.b;
					break;
				case 3:
					num = this.a;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Color index(" + index.ToString() + ")!");
				}
				return num;
			}
			set
			{
				switch (index)
				{
				case 0:
					this.r = value;
					break;
				case 1:
					this.g = value;
					break;
				case 2:
					this.b = value;
					break;
				case 3:
					this.a = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Color index(" + index.ToString() + ")!");
				}
			}
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x0001F258 File Offset: 0x0001D458
		public static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
		{
			bool flag = rgbColor.b > rgbColor.g && rgbColor.b > rgbColor.r;
			if (flag)
			{
				Color.RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
			}
			else
			{
				bool flag2 = rgbColor.g > rgbColor.r;
				if (flag2)
				{
					Color.RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
				}
				else
				{
					Color.RGBToHSVHelper(0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
				}
			}
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x0001F300 File Offset: 0x0001D500
		private static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
		{
			V = dominantcolor;
			bool flag = V != 0f;
			if (flag)
			{
				bool flag2 = colorone > colortwo;
				float num;
				if (flag2)
				{
					num = colortwo;
				}
				else
				{
					num = colorone;
				}
				float num2 = V - num;
				bool flag3 = num2 != 0f;
				if (flag3)
				{
					S = num2 / V;
					H = offset + (colorone - colortwo) / num2;
				}
				else
				{
					S = 0f;
					H = offset + (colorone - colortwo);
				}
				H /= 6f;
				bool flag4 = H < 0f;
				if (flag4)
				{
					H += 1f;
				}
			}
			else
			{
				S = 0f;
				H = 0f;
			}
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x0001F3B4 File Offset: 0x0001D5B4
		public static Color HSVToRGB(float H, float S, float V)
		{
			return Color.HSVToRGB(H, S, V, true);
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x0001F3D0 File Offset: 0x0001D5D0
		public static Color HSVToRGB(float H, float S, float V, bool hdr)
		{
			Color white = Color.white;
			bool flag = S == 0f;
			if (flag)
			{
				white.r = V;
				white.g = V;
				white.b = V;
			}
			else
			{
				bool flag2 = V == 0f;
				if (flag2)
				{
					white.r = 0f;
					white.g = 0f;
					white.b = 0f;
				}
				else
				{
					white.r = 0f;
					white.g = 0f;
					white.b = 0f;
					float num = H * 6f;
					int num2 = (int)Mathf.Floor(num);
					float num3 = num - (float)num2;
					float num4 = V * (1f - S);
					float num5 = V * (1f - S * num3);
					float num6 = V * (1f - S * (1f - num3));
					switch (num2)
					{
					case -1:
						white.r = V;
						white.g = num4;
						white.b = num5;
						break;
					case 0:
						white.r = V;
						white.g = num6;
						white.b = num4;
						break;
					case 1:
						white.r = num5;
						white.g = V;
						white.b = num4;
						break;
					case 2:
						white.r = num4;
						white.g = V;
						white.b = num6;
						break;
					case 3:
						white.r = num4;
						white.g = num5;
						white.b = V;
						break;
					case 4:
						white.r = num6;
						white.g = num4;
						white.b = V;
						break;
					case 5:
						white.r = V;
						white.g = num4;
						white.b = num5;
						break;
					case 6:
						white.r = V;
						white.g = num6;
						white.b = num4;
						break;
					}
					bool flag3 = !hdr;
					if (flag3)
					{
						white.r = Mathf.Clamp(white.r, 0f, 1f);
						white.g = Mathf.Clamp(white.g, 0f, 1f);
						white.b = Mathf.Clamp(white.b, 0f, 1f);
					}
				}
			}
			return white;
		}

		// Token: 0x040007D1 RID: 2001
		public float r;

		// Token: 0x040007D2 RID: 2002
		public float g;

		// Token: 0x040007D3 RID: 2003
		public float b;

		// Token: 0x040007D4 RID: 2004
		public float a;
	}
}
