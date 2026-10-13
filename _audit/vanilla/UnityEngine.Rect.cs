using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	// Token: 0x02000130 RID: 304
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeClass("Rectf", "template<typename T> class RectT; typedef RectT<float> Rectf;")]
	[NativeHeader("Runtime/Math/Rect.h")]
	public struct Rect : IEquatable<Rect>, IFormattable
	{
		// Token: 0x060007D4 RID: 2004 RVA: 0x0000C793 File Offset: 0x0000A993
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rect(float x, float y, float width, float height)
		{
			this.m_XMin = x;
			this.m_YMin = y;
			this.m_Width = width;
			this.m_Height = height;
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x0000C7B3 File Offset: 0x0000A9B3
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rect(Vector2 position, Vector2 size)
		{
			this.m_XMin = position.x;
			this.m_YMin = position.y;
			this.m_Width = size.x;
			this.m_Height = size.y;
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0000C7E6 File Offset: 0x0000A9E6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rect(Rect source)
		{
			this.m_XMin = source.m_XMin;
			this.m_YMin = source.m_YMin;
			this.m_Width = source.m_Width;
			this.m_Height = source.m_Height;
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060007D7 RID: 2007 RVA: 0x0000C819 File Offset: 0x0000AA19
		public static Rect zero
		{
			get
			{
				return new Rect(0f, 0f, 0f, 0f);
			}
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0000C834 File Offset: 0x0000AA34
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Rect MinMaxRect(float xmin, float ymin, float xmax, float ymax)
		{
			return new Rect(xmin, ymin, xmax - xmin, ymax - ymin);
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0000C793 File Offset: 0x0000A993
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(float x, float y, float width, float height)
		{
			this.m_XMin = x;
			this.m_YMin = y;
			this.m_Width = width;
			this.m_Height = height;
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060007DA RID: 2010 RVA: 0x0000C854 File Offset: 0x0000AA54
		// (set) Token: 0x060007DB RID: 2011 RVA: 0x0000C86C File Offset: 0x0000AA6C
		public float x
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_XMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_XMin = value;
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060007DC RID: 2012 RVA: 0x0000C878 File Offset: 0x0000AA78
		// (set) Token: 0x060007DD RID: 2013 RVA: 0x0000C890 File Offset: 0x0000AA90
		public float y
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_YMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_YMin = value;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060007DE RID: 2014 RVA: 0x0000C89C File Offset: 0x0000AA9C
		// (set) Token: 0x060007DF RID: 2015 RVA: 0x0000C8BF File Offset: 0x0000AABF
		public Vector2 position
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Vector2(this.m_XMin, this.m_YMin);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_XMin = value.x;
				this.m_YMin = value.y;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060007E0 RID: 2016 RVA: 0x0000C8DC File Offset: 0x0000AADC
		// (set) Token: 0x060007E1 RID: 2017 RVA: 0x0000C919 File Offset: 0x0000AB19
		public Vector2 center
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Vector2(this.x + this.m_Width / 2f, this.y + this.m_Height / 2f);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_XMin = value.x - this.m_Width / 2f;
				this.m_YMin = value.y - this.m_Height / 2f;
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060007E2 RID: 2018 RVA: 0x0000C950 File Offset: 0x0000AB50
		// (set) Token: 0x060007E3 RID: 2019 RVA: 0x0000C973 File Offset: 0x0000AB73
		public Vector2 min
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Vector2(this.xMin, this.yMin);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.xMin = value.x;
				this.yMin = value.y;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060007E4 RID: 2020 RVA: 0x0000C990 File Offset: 0x0000AB90
		// (set) Token: 0x060007E5 RID: 2021 RVA: 0x0000C9B3 File Offset: 0x0000ABB3
		public Vector2 max
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Vector2(this.xMax, this.yMax);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.xMax = value.x;
				this.yMax = value.y;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060007E6 RID: 2022 RVA: 0x0000C9D0 File Offset: 0x0000ABD0
		// (set) Token: 0x060007E7 RID: 2023 RVA: 0x0000C9E8 File Offset: 0x0000ABE8
		public float width
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Width;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Width = value;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060007E8 RID: 2024 RVA: 0x0000C9F4 File Offset: 0x0000ABF4
		// (set) Token: 0x060007E9 RID: 2025 RVA: 0x0000CA0C File Offset: 0x0000AC0C
		public float height
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Height;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Height = value;
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060007EA RID: 2026 RVA: 0x0000CA18 File Offset: 0x0000AC18
		// (set) Token: 0x060007EB RID: 2027 RVA: 0x0000CA3B File Offset: 0x0000AC3B
		public Vector2 size
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Vector2(this.m_Width, this.m_Height);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Width = value.x;
				this.m_Height = value.y;
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060007EC RID: 2028 RVA: 0x0000CA58 File Offset: 0x0000AC58
		// (set) Token: 0x060007ED RID: 2029 RVA: 0x0000CA70 File Offset: 0x0000AC70
		public float xMin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_XMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				float xMax = this.xMax;
				this.m_XMin = value;
				this.m_Width = xMax - this.m_XMin;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060007EE RID: 2030 RVA: 0x0000CA9C File Offset: 0x0000AC9C
		// (set) Token: 0x060007EF RID: 2031 RVA: 0x0000CAB4 File Offset: 0x0000ACB4
		public float yMin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_YMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				float yMax = this.yMax;
				this.m_YMin = value;
				this.m_Height = yMax - this.m_YMin;
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060007F0 RID: 2032 RVA: 0x0000CAE0 File Offset: 0x0000ACE0
		// (set) Token: 0x060007F1 RID: 2033 RVA: 0x0000CAFF File Offset: 0x0000ACFF
		public float xMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Width + this.m_XMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Width = value - this.m_XMin;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060007F2 RID: 2034 RVA: 0x0000CB10 File Offset: 0x0000AD10
		// (set) Token: 0x060007F3 RID: 2035 RVA: 0x0000CB2F File Offset: 0x0000AD2F
		public float yMax
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Height + this.m_YMin;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this.m_Height = value - this.m_YMin;
			}
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0000CB40 File Offset: 0x0000AD40
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Vector2 point)
		{
			return point.x >= this.xMin && point.x < this.xMax && point.y >= this.yMin && point.y < this.yMax;
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0000CB90 File Offset: 0x0000AD90
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Vector3 point)
		{
			return point.x >= this.xMin && point.x < this.xMax && point.y >= this.yMin && point.y < this.yMax;
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0000CBE0 File Offset: 0x0000ADE0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(Vector3 point, bool allowInverse)
		{
			bool flag = !allowInverse;
			bool flag2;
			if (flag)
			{
				flag2 = this.Contains(point);
			}
			else
			{
				bool flag3 = (this.width < 0f && point.x <= this.xMin && point.x > this.xMax) || (this.width >= 0f && point.x >= this.xMin && point.x < this.xMax);
				bool flag4 = (this.height < 0f && point.y <= this.yMin && point.y > this.yMax) || (this.height >= 0f && point.y >= this.yMin && point.y < this.yMax);
				flag2 = flag3 && flag4;
			}
			return flag2;
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x0000CCC0 File Offset: 0x0000AEC0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Rect OrderMinMax(Rect rect)
		{
			bool flag = rect.xMin > rect.xMax;
			if (flag)
			{
				float xMin = rect.xMin;
				rect.xMin = rect.xMax;
				rect.xMax = xMin;
			}
			bool flag2 = rect.yMin > rect.yMax;
			if (flag2)
			{
				float yMin = rect.yMin;
				rect.yMin = rect.yMax;
				rect.yMax = yMin;
			}
			return rect;
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x0000CD44 File Offset: 0x0000AF44
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Overlaps(Rect other)
		{
			return other.xMax > this.xMin && other.xMin < this.xMax && other.yMax > this.yMin && other.yMin < this.yMax;
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0000CD98 File Offset: 0x0000AF98
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Overlaps(Rect other, bool allowInverse)
		{
			Rect rect = this;
			if (allowInverse)
			{
				rect = Rect.OrderMinMax(rect);
				other = Rect.OrderMinMax(other);
			}
			return rect.Overlaps(other);
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x0000CDD0 File Offset: 0x0000AFD0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 NormalizedToPoint(Rect rectangle, Vector2 normalizedRectCoordinates)
		{
			return new Vector2(Mathf.Lerp(rectangle.x, rectangle.xMax, normalizedRectCoordinates.x), Mathf.Lerp(rectangle.y, rectangle.yMax, normalizedRectCoordinates.y));
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x0000CE1C File Offset: 0x0000B01C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 PointToNormalized(Rect rectangle, Vector2 point)
		{
			return new Vector2(Mathf.InverseLerp(rectangle.x, rectangle.xMax, point.x), Mathf.InverseLerp(rectangle.y, rectangle.yMax, point.y));
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x0000CE68 File Offset: 0x0000B068
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Rect lhs, Rect rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x0000CE84 File Offset: 0x0000B084
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Rect lhs, Rect rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x0000CEDC File Offset: 0x0000B0DC
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ (this.width.GetHashCode() << 2) ^ (this.y.GetHashCode() >> 2) ^ (this.height.GetHashCode() >> 1);
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x0000CF30 File Offset: 0x0000B130
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object other)
		{
			bool flag = !(other is Rect);
			return !flag && this.Equals((Rect)other);
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x0000CF64 File Offset: 0x0000B164
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(Rect other)
		{
			return this.x.Equals(other.x) && this.y.Equals(other.y) && this.width.Equals(other.width) && this.height.Equals(other.height);
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x0000CFD4 File Offset: 0x0000B1D4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return this.ToString(null, null);
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x0000CFF0 File Offset: 0x0000B1F0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x0000D00C File Offset: 0x0000B20C
		public string ToString(string format, IFormatProvider formatProvider)
		{
			bool flag = string.IsNullOrEmpty(format);
			if (flag)
			{
				format = "F2";
			}
			bool flag2 = formatProvider == null;
			if (flag2)
			{
				formatProvider = CultureInfo.InvariantCulture.NumberFormat;
			}
			return UnityString.Format("(x:{0}, y:{1}, width:{2}, height:{3})", new object[]
			{
				this.x.ToString(format, formatProvider),
				this.y.ToString(format, formatProvider),
				this.width.ToString(format, formatProvider),
				this.height.ToString(format, formatProvider)
			});
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000804 RID: 2052 RVA: 0x0000D0A0 File Offset: 0x0000B2A0
		[Obsolete("use xMin")]
		public float left
		{
			get
			{
				return this.m_XMin;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000805 RID: 2053 RVA: 0x0000D0B8 File Offset: 0x0000B2B8
		[Obsolete("use xMax")]
		public float right
		{
			get
			{
				return this.m_XMin + this.m_Width;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000806 RID: 2054 RVA: 0x0000D0D8 File Offset: 0x0000B2D8
		[Obsolete("use yMin")]
		public float top
		{
			get
			{
				return this.m_YMin;
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000807 RID: 2055 RVA: 0x0000D0F0 File Offset: 0x0000B2F0
		[Obsolete("use yMax")]
		public float bottom
		{
			get
			{
				return this.m_YMin + this.m_Height;
			}
		}

		// Token: 0x040003EC RID: 1004
		[NativeName("x")]
		private float m_XMin;

		// Token: 0x040003ED RID: 1005
		[NativeName("y")]
		private float m_YMin;

		// Token: 0x040003EE RID: 1006
		[NativeName("width")]
		private float m_Width;

		// Token: 0x040003EF RID: 1007
		[NativeName("height")]
		private float m_Height;
	}
}
