using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	// Token: 0x020001CE RID: 462
	[ExcludeFromPreset]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/Texture2D.h")]
	[NativeHeader("Runtime/Graphics/GeneratedTextures.h")]
	public sealed class Texture2D : Texture
	{
		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x0600125E RID: 4702
		public extern TextureFormat format
		{
			[NativeName("GetTextureFormat")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x0600125F RID: 4703
		// (set) Token: 0x06001260 RID: 4704
		public extern bool ignoreMipmapLimit
		{
			[NativeName("IgnoreMipmapLimit")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetIgnoreMipmapLimitAndReload")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06001261 RID: 4705
		public extern string mipmapLimitGroup
		{
			[NativeName("GetMipmapLimitGroupName")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06001262 RID: 4706
		public extern int activeMipmapLimit
		{
			[NativeName("GetMipmapLimit")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06001263 RID: 4707
		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D whiteTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06001264 RID: 4708
		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D blackTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06001265 RID: 4709
		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D redTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06001266 RID: 4710
		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D grayTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06001267 RID: 4711
		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D linearGrayTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06001268 RID: 4712
		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D normalTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x06001269 RID: 4713
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Compress(bool highQuality);

		// Token: 0x0600126A RID: 4714
		[FreeFunction("Texture2DScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl([Writable] Texture2D mono, int w, int h, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex, string mipmapLimitGroupName);

		// Token: 0x0600126B RID: 4715 RVA: 0x00019670 File Offset: 0x00017870
		private static void Internal_Create([Writable] Texture2D mono, int w, int h, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex, string mipmapLimitGroupName)
		{
			bool flag = !Texture2D.Internal_CreateImpl(mono, w, h, mipCount, format, colorSpace, flags, nativeTex, mipmapLimitGroupName);
			if (flag)
			{
				throw new UnityException("Failed to create texture because of invalid parameters.");
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x0600126C RID: 4716
		public override extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x0600126D RID: 4717
		[NativeName("VTOnly")]
		[NativeConditional("ENABLE_VIRTUALTEXTURING && UNITY_EDITOR")]
		public extern bool vtOnly
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x0600126E RID: 4718
		[NativeName("Apply")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable);

		// Token: 0x0600126F RID: 4719
		[NativeName("Reinitialize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool ReinitializeImpl(int width, int height);

		// Token: 0x06001270 RID: 4720 RVA: 0x000196A3 File Offset: 0x000178A3
		[NativeName("SetPixel")]
		private void SetPixelImpl(int image, int mip, int x, int y, Color color)
		{
			this.SetPixelImpl_Injected(image, mip, x, y, ref color);
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x000196B4 File Offset: 0x000178B4
		[NativeName("GetPixel")]
		private Color GetPixelImpl(int image, int mip, int x, int y)
		{
			Color color;
			this.GetPixelImpl_Injected(image, mip, x, y, out color);
			return color;
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x000196D0 File Offset: 0x000178D0
		[NativeName("GetPixelBilinear")]
		private Color GetPixelBilinearImpl(int image, int mip, float u, float v)
		{
			Color color;
			this.GetPixelBilinearImpl_Injected(image, mip, u, v, out color);
			return color;
		}

		// Token: 0x06001273 RID: 4723
		[FreeFunction(Name = "Texture2DScripting::ReinitializeWithFormat", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool ReinitializeWithFormatImpl(int width, int height, GraphicsFormat format, bool hasMipMap);

		// Token: 0x06001274 RID: 4724
		[FreeFunction(Name = "Texture2DScripting::ReinitializeWithTextureFormat", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool ReinitializeWithTextureFormatImpl(int width, int height, TextureFormat textureFormat, bool hasMipMap);

		// Token: 0x06001275 RID: 4725 RVA: 0x000196EB File Offset: 0x000178EB
		[FreeFunction(Name = "Texture2DScripting::ReadPixels", HasExplicitThis = true)]
		private void ReadPixelsImpl(Rect source, int destX, int destY, bool recalculateMipMaps)
		{
			this.ReadPixelsImpl_Injected(ref source, destX, destY, recalculateMipMaps);
		}

		// Token: 0x06001276 RID: 4726
		[FreeFunction(Name = "Texture2DScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPixelsImpl(int x, int y, int w, int h, Color[] pixel, int miplevel, int frame);

		// Token: 0x06001277 RID: 4727
		[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool LoadRawTextureDataImpl(IntPtr data, ulong size);

		// Token: 0x06001278 RID: 4728
		[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool LoadRawTextureDataImplArray(byte[] data);

		// Token: 0x06001279 RID: 4729
		[FreeFunction(Name = "Texture2DScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool SetPixelDataImplArray(Array data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0);

		// Token: 0x0600127A RID: 4730
		[FreeFunction(Name = "Texture2DScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool SetPixelDataImpl(IntPtr data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0);

		// Token: 0x0600127B RID: 4731
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetWritableImageData(int frame);

		// Token: 0x0600127C RID: 4732
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ulong GetRawImageDataSize();

		// Token: 0x0600127D RID: 4733
		[FreeFunction("Texture2DScripting::GenerateAtlas")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateAtlasImpl(Vector2[] sizes, int padding, int atlasSize, [Out] Rect[] rect);

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x0600127E RID: 4734
		internal extern bool isPreProcessed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x0600127F RID: 4735
		public extern bool streamingMipmaps
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06001280 RID: 4736
		public extern int streamingMipmapsPriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06001281 RID: 4737
		// (set) Token: 0x06001282 RID: 4738
		public extern int requestedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetRequestedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(Name = "GetTextureStreamingManager().SetRequestedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06001283 RID: 4739
		// (set) Token: 0x06001284 RID: 4740
		public extern int minimumMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetMinimumMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(Name = "GetTextureStreamingManager().SetMinimumMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06001285 RID: 4741
		// (set) Token: 0x06001286 RID: 4742
		internal extern bool loadAllMips
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadAllMips", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(Name = "GetTextureStreamingManager().SetLoadAllMips", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06001287 RID: 4743
		public extern int calculatedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetCalculatedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06001288 RID: 4744
		public extern int desiredMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetDesiredMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06001289 RID: 4745
		public extern int loadingMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadingMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x0600128A RID: 4746
		public extern int loadedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		// Token: 0x0600128B RID: 4747
		[FreeFunction(Name = "GetTextureStreamingManager().ClearRequestedMipmapLevel", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearRequestedMipmapLevel();

		// Token: 0x0600128C RID: 4748
		[FreeFunction(Name = "GetTextureStreamingManager().IsRequestedMipmapLevelLoaded", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsRequestedMipmapLevelLoaded();

		// Token: 0x0600128D RID: 4749
		[FreeFunction(Name = "GetTextureStreamingManager().ClearMinimumMipmapLevel", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearMinimumMipmapLevel();

		// Token: 0x0600128E RID: 4750
		[FreeFunction("Texture2DScripting::UpdateExternalTexture", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateExternalTexture(IntPtr nativeTex);

		// Token: 0x0600128F RID: 4751
		[FreeFunction("Texture2DScripting::SetAllPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetAllPixels32([Unmarshalled] Color32[] colors, int miplevel);

		// Token: 0x06001290 RID: 4752
		[FreeFunction("Texture2DScripting::SetBlockOfPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBlockOfPixels32(int x, int y, int blockWidth, int blockHeight, [Unmarshalled] Color32[] colors, int miplevel);

		// Token: 0x06001291 RID: 4753
		[FreeFunction("Texture2DScripting::GetRawTextureData", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern byte[] GetRawTextureData();

		// Token: 0x06001292 RID: 4754
		[FreeFunction("Texture2DScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel);

		// Token: 0x06001293 RID: 4755 RVA: 0x000196FC File Offset: 0x000178FC
		[ExcludeFromDocs]
		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
		{
			return this.GetPixels(x, y, blockWidth, blockHeight, 0);
		}

		// Token: 0x06001294 RID: 4756
		[FreeFunction("Texture2DScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32([DefaultValue("0")] int miplevel);

		// Token: 0x06001295 RID: 4757 RVA: 0x0001971C File Offset: 0x0001791C
		[ExcludeFromDocs]
		public Color32[] GetPixels32()
		{
			return this.GetPixels32(0);
		}

		// Token: 0x06001296 RID: 4758
		[FreeFunction("Texture2DScripting::PackTextures", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize, bool makeNoLongerReadable);

		// Token: 0x06001297 RID: 4759 RVA: 0x00019738 File Offset: 0x00017938
		public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize)
		{
			return this.PackTextures(textures, padding, maximumAtlasSize, false);
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x00019754 File Offset: 0x00017954
		public Rect[] PackTextures(Texture2D[] textures, int padding)
		{
			return this.PackTextures(textures, padding, 2048);
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x00019774 File Offset: 0x00017974
		internal bool ValidateFormat(TextureFormat format, int width, int height)
		{
			bool flag = base.ValidateFormat(format);
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = TextureFormat.PVRTC_RGB2 <= format && format <= TextureFormat.PVRTC_RGBA4;
				bool flag4 = flag3 && (width != height || !Mathf.IsPowerOfTwo(width));
				if (flag4)
				{
					throw new UnityException(string.Format("'{0}' demands texture to be square and have power-of-two dimensions", format.ToString()));
				}
			}
			return flag;
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x000197E0 File Offset: 0x000179E0
		internal bool ValidateFormat(GraphicsFormat format, int width, int height)
		{
			bool flag = base.ValidateFormat(format, FormatUsage.Sample);
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = GraphicsFormatUtility.IsPVRTCFormat(format);
				bool flag4 = flag3 && (width != height || !Mathf.IsPowerOfTwo(width));
				if (flag4)
				{
					throw new UnityException(string.Format("'{0}' demands texture to be square and have power-of-two dimensions", format.ToString()));
				}
			}
			return flag;
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x00019844 File Offset: 0x00017A44
		internal Texture2D(int width, int height, GraphicsFormat format, TextureCreationFlags flags, int mipCount, IntPtr nativeTex, string mipmapLimitGroupName)
		{
			bool flag = this.ValidateFormat(format, width, height);
			if (flag)
			{
				Texture2D.Internal_Create(this, width, height, mipCount, format, base.GetTextureColorSpace(format), flags, nativeTex, mipmapLimitGroupName);
			}
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x0001987F File Offset: 0x00017A7F
		[ExcludeFromDocs]
		public Texture2D(int width, int height, DefaultFormat format, TextureCreationFlags flags)
			: this(width, height, SystemInfo.GetGraphicsFormat(format), flags)
		{
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x00019893 File Offset: 0x00017A93
		[ExcludeFromDocs]
		public Texture2D(int width, int height, DefaultFormat format, int mipCount, TextureCreationFlags flags)
			: this(width, height, SystemInfo.GetGraphicsFormat(format), flags, mipCount, IntPtr.Zero, null)
		{
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x000198AF File Offset: 0x00017AAF
		[ExcludeFromDocs]
		public Texture2D(int width, int height, DefaultFormat format, int mipCount, string mipmapLimitGroupName, TextureCreationFlags flags)
			: this(width, height, SystemInfo.GetGraphicsFormat(format), flags, mipCount, IntPtr.Zero, mipmapLimitGroupName)
		{
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x000198CC File Offset: 0x00017ACC
		[ExcludeFromDocs]
		public Texture2D(int width, int height, GraphicsFormat format, TextureCreationFlags flags)
			: this(width, height, format, flags, Texture.GenerateAllMips, IntPtr.Zero, null)
		{
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x000198E6 File Offset: 0x00017AE6
		[ExcludeFromDocs]
		public Texture2D(int width, int height, GraphicsFormat format, int mipCount, TextureCreationFlags flags)
			: this(width, height, format, flags, mipCount, IntPtr.Zero, null)
		{
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x000198FD File Offset: 0x00017AFD
		[ExcludeFromDocs]
		public Texture2D(int width, int height, GraphicsFormat format, int mipCount, string mipmapLimitGroupName, TextureCreationFlags flags)
			: this(width, height, format, flags, mipCount, IntPtr.Zero, mipmapLimitGroupName)
		{
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x00019918 File Offset: 0x00017B18
		internal Texture2D(int width, int height, TextureFormat textureFormat, int mipCount, bool linear, IntPtr nativeTex, bool createUninitialized, bool ignoreMipmapLimit, string mipmapLimitGroupName)
		{
			bool flag = !this.ValidateFormat(textureFormat, width, height);
			if (!flag)
			{
				GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(textureFormat, !linear);
				TextureCreationFlags textureCreationFlags = ((mipCount != 1) ? TextureCreationFlags.MipChain : TextureCreationFlags.None);
				bool flag2 = GraphicsFormatUtility.IsCrunchFormat(textureFormat);
				if (flag2)
				{
					textureCreationFlags |= TextureCreationFlags.Crunch;
				}
				if (createUninitialized)
				{
					textureCreationFlags |= TextureCreationFlags.DontInitializePixels | TextureCreationFlags.DontUploadUponCreate;
				}
				if (ignoreMipmapLimit)
				{
					textureCreationFlags |= TextureCreationFlags.IgnoreMipmapLimit;
				}
				Texture2D.Internal_Create(this, width, height, mipCount, graphicsFormat, base.GetTextureColorSpace(linear), textureCreationFlags, nativeTex, mipmapLimitGroupName);
			}
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x000199A0 File Offset: 0x00017BA0
		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear)
			: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, false, false, null)
		{
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x000199C4 File Offset: 0x00017BC4
		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
			: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, createUninitialized, false, null)
		{
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x000199EC File Offset: 0x00017BEC
		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized, [DefaultValue("false")] bool ignoreMipmapLimit, [DefaultValue("null")] string mipmapLimitGroupName)
			: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, createUninitialized, ignoreMipmapLimit, mipmapLimitGroupName)
		{
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x00019A14 File Offset: 0x00017C14
		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("true")] bool mipChain, [DefaultValue("false")] bool linear)
			: this(width, height, textureFormat, mipChain ? Texture.GenerateAllMips : 1, linear, IntPtr.Zero, false, false, null)
		{
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x00019A44 File Offset: 0x00017C44
		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("true")] bool mipChain, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
			: this(width, height, textureFormat, mipChain ? Texture.GenerateAllMips : 1, linear, IntPtr.Zero, createUninitialized, false, null)
		{
		}

		// Token: 0x060012A8 RID: 4776 RVA: 0x00019A74 File Offset: 0x00017C74
		[ExcludeFromDocs]
		public Texture2D(int width, int height, TextureFormat textureFormat, bool mipChain)
			: this(width, height, textureFormat, mipChain ? Texture.GenerateAllMips : 1, false, IntPtr.Zero, false, false, null)
		{
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x00019AA4 File Offset: 0x00017CA4
		[ExcludeFromDocs]
		public Texture2D(int width, int height)
			: this(width, height, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero, false, false, null)
		{
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x00019ACC File Offset: 0x00017CCC
		public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipChain, bool linear, IntPtr nativeTex)
		{
			bool flag = nativeTex == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentException("nativeTex can not be null");
			}
			return new Texture2D(width, height, format, mipChain ? (-1) : 1, linear, nativeTex, false, false, null);
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x00019B10 File Offset: 0x00017D10
		[ExcludeFromDocs]
		public void SetPixel(int x, int y, Color color)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl(0, 0, x, y, color);
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x00019B40 File Offset: 0x00017D40
		public void SetPixel(int x, int y, Color color, [DefaultValue("0")] int mipLevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl(0, mipLevel, x, y, color);
		}

		// Token: 0x060012AD RID: 4781 RVA: 0x00019B70 File Offset: 0x00017D70
		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelsImpl(x, y, blockWidth, blockHeight, colors, miplevel, 0);
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x00019BA4 File Offset: 0x00017DA4
		[ExcludeFromDocs]
		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
		{
			this.SetPixels(x, y, blockWidth, blockHeight, colors, 0);
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x00019BB8 File Offset: 0x00017DB8
		public void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel)
		{
			int num = this.width >> miplevel;
			bool flag = num < 1;
			if (flag)
			{
				num = 1;
			}
			int num2 = this.height >> miplevel;
			bool flag2 = num2 < 1;
			if (flag2)
			{
				num2 = 1;
			}
			this.SetPixels(0, 0, num, num2, colors, miplevel);
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x00019BFF File Offset: 0x00017DFF
		[ExcludeFromDocs]
		public void SetPixels(Color[] colors)
		{
			this.SetPixels(0, 0, this.width, this.height, colors, 0);
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x00019C1C File Offset: 0x00017E1C
		[ExcludeFromDocs]
		public Color GetPixel(int x, int y)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl(0, 0, x, y);
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x00019C50 File Offset: 0x00017E50
		public Color GetPixel(int x, int y, [DefaultValue("0")] int mipLevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl(0, mipLevel, x, y);
		}

		// Token: 0x060012B3 RID: 4787 RVA: 0x00019C84 File Offset: 0x00017E84
		[ExcludeFromDocs]
		public Color GetPixelBilinear(float u, float v)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelBilinearImpl(0, 0, u, v);
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x00019CB8 File Offset: 0x00017EB8
		public Color GetPixelBilinear(float u, float v, [DefaultValue("0")] int mipLevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelBilinearImpl(0, mipLevel, u, v);
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x00019CEC File Offset: 0x00017EEC
		public void LoadRawTextureData(IntPtr data, int size)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = data == IntPtr.Zero || size == 0;
			if (flag2)
			{
				Debug.LogError("No texture data provided to LoadRawTextureData", this);
			}
			else
			{
				bool flag3 = !this.LoadRawTextureDataImpl(data, (ulong)((long)size));
				if (flag3)
				{
					throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
				}
			}
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x00019D54 File Offset: 0x00017F54
		public void LoadRawTextureData(byte[] data)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = data == null || data.Length == 0;
			if (flag2)
			{
				Debug.LogError("No texture data provided to LoadRawTextureData", this);
			}
			else
			{
				bool flag3 = !this.LoadRawTextureDataImplArray(data);
				if (flag3)
				{
					throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
				}
			}
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x00019DB0 File Offset: 0x00017FB0
		public void LoadRawTextureData<T>(NativeArray<T> data) where T : struct
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = !data.IsCreated || data.Length == 0;
			if (flag2)
			{
				throw new UnityException("No texture data provided to LoadRawTextureData");
			}
			bool flag3 = !this.LoadRawTextureDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), (ulong)((long)data.Length * (long)UnsafeUtility.SizeOf<T>()));
			if (flag3)
			{
				throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
			}
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x00019E2C File Offset: 0x0001802C
		public void SetPixelData<T>(T[] data, int mipLevel, [DefaultValue("0")] int sourceDataStartIndex = 0)
		{
			bool flag = sourceDataStartIndex < 0;
			if (flag)
			{
				throw new UnityException("SetPixelData: sourceDataStartIndex cannot be less than 0.");
			}
			bool flag2 = !this.isReadable;
			if (flag2)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag3 = data == null || data.Length == 0;
			if (flag3)
			{
				throw new UnityException("No texture data provided to SetPixelData.");
			}
			this.SetPixelDataImplArray(data, mipLevel, Marshal.SizeOf<T>(data[0]), data.Length, sourceDataStartIndex);
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x00019E98 File Offset: 0x00018098
		public void SetPixelData<T>(NativeArray<T> data, int mipLevel, [DefaultValue("0")] int sourceDataStartIndex = 0) where T : struct
		{
			bool flag = sourceDataStartIndex < 0;
			if (flag)
			{
				throw new UnityException("SetPixelData: sourceDataStartIndex cannot be less than 0.");
			}
			bool flag2 = !this.isReadable;
			if (flag2)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag3 = !data.IsCreated || data.Length == 0;
			if (flag3)
			{
				throw new UnityException("No texture data provided to SetPixelData.");
			}
			this.SetPixelDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), mipLevel, UnsafeUtility.SizeOf<T>(), data.Length, sourceDataStartIndex);
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x00019F14 File Offset: 0x00018114
		public unsafe NativeArray<T> GetPixelData<T>(int mipLevel) where T : struct
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = mipLevel < 0 || mipLevel >= base.mipmapCount;
			if (flag2)
			{
				throw new ArgumentException("The passed in miplevel " + mipLevel.ToString() + " is invalid. It needs to be in the range 0 and " + (base.mipmapCount - 1).ToString());
			}
			bool flag3 = this.GetWritableImageData(0).ToInt64() == 0L;
			if (flag3)
			{
				throw new UnityException("Texture '" + base.name + "' has no data.");
			}
			ulong pixelDataOffset = base.GetPixelDataOffset(mipLevel, 0);
			ulong pixelDataSize = base.GetPixelDataSize(mipLevel, 0);
			int num = UnsafeUtility.SizeOf<T>();
			ulong num2 = pixelDataSize / (ulong)((long)num);
			bool flag4 = num2 > 2147483647UL;
			if (flag4)
			{
				throw base.CreateNativeArrayLengthOverflowException();
			}
			IntPtr intPtr = new IntPtr((long)this.GetWritableImageData(0) + (long)pixelDataOffset);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num2, Allocator.None);
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x0001A018 File Offset: 0x00018218
		public unsafe NativeArray<T> GetRawTextureData<T>() where T : struct
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			int num = UnsafeUtility.SizeOf<T>();
			ulong num2 = this.GetRawImageDataSize() / (ulong)((long)num);
			bool flag2 = num2 > 2147483647UL;
			if (flag2)
			{
				throw base.CreateNativeArrayLengthOverflowException();
			}
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.GetWritableImageData(0), (int)num2, Allocator.None);
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x0001A080 File Offset: 0x00018280
		public void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.ApplyImpl(updateMipmaps, makeNoLongerReadable);
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x0001A0AC File Offset: 0x000182AC
		[ExcludeFromDocs]
		public void Apply(bool updateMipmaps)
		{
			this.Apply(updateMipmaps, false);
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x0001A0B8 File Offset: 0x000182B8
		[ExcludeFromDocs]
		public void Apply()
		{
			this.Apply(true, false);
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x0001A0C4 File Offset: 0x000182C4
		public bool Reinitialize(int width, int height)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.ReinitializeImpl(width, height);
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x0001A0F4 File Offset: 0x000182F4
		public bool Reinitialize(int width, int height, TextureFormat format, bool hasMipMap)
		{
			return this.ReinitializeWithTextureFormatImpl(width, height, format, hasMipMap);
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x0001A114 File Offset: 0x00018314
		public bool Reinitialize(int width, int height, GraphicsFormat format, bool hasMipMap)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.ReinitializeWithFormatImpl(width, height, format, hasMipMap);
		}

		// Token: 0x060012C2 RID: 4802 RVA: 0x0001A148 File Offset: 0x00018348
		[Obsolete("Texture2D.Resize(int, int) has been deprecated because it actually reinitializes the texture. Use Texture2D.Reinitialize(int, int) instead (UnityUpgradable) -> Reinitialize([*] System.Int32, [*] System.Int32)", false)]
		public bool Resize(int width, int height)
		{
			return this.Reinitialize(width, height);
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x0001A164 File Offset: 0x00018364
		[Obsolete("Texture2D.Resize(int, int, TextureFormat, bool) has been deprecated because it actually reinitializes the texture. Use Texture2D.Reinitialize(int, int, TextureFormat, bool) instead (UnityUpgradable) -> Reinitialize([*] System.Int32, [*] System.Int32, UnityEngine.TextureFormat, [*] System.Boolean)", false)]
		public bool Resize(int width, int height, TextureFormat format, bool hasMipMap)
		{
			return this.Reinitialize(width, height, format, hasMipMap);
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x0001A184 File Offset: 0x00018384
		[Obsolete("Texture2D.Resize(int, int, GraphicsFormat, bool) has been deprecated because it actually reinitializes the texture. Use Texture2D.Reinitialize(int, int, GraphicsFormat, bool) instead (UnityUpgradable) -> Reinitialize([*] System.Int32, [*] System.Int32, UnityEngine.Experimental.Rendering.GraphicsFormat, [*] System.Boolean)", false)]
		public bool Resize(int width, int height, GraphicsFormat format, bool hasMipMap)
		{
			return this.Reinitialize(width, height, format, hasMipMap);
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x0001A1A4 File Offset: 0x000183A4
		public void ReadPixels(Rect source, int destX, int destY, [DefaultValue("true")] bool recalculateMipMaps)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.ReadPixelsImpl(source, destX, destY, recalculateMipMaps);
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x0001A1D3 File Offset: 0x000183D3
		[ExcludeFromDocs]
		public void ReadPixels(Rect source, int destX, int destY)
		{
			this.ReadPixels(source, destX, destY, true);
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x0001A1E4 File Offset: 0x000183E4
		public static bool GenerateAtlas(Vector2[] sizes, int padding, int atlasSize, List<Rect> results)
		{
			bool flag = sizes == null;
			if (flag)
			{
				throw new ArgumentException("sizes array can not be null");
			}
			bool flag2 = results == null;
			if (flag2)
			{
				throw new ArgumentException("results list cannot be null");
			}
			bool flag3 = padding < 0;
			if (flag3)
			{
				throw new ArgumentException("padding can not be negative");
			}
			bool flag4 = atlasSize <= 0;
			if (flag4)
			{
				throw new ArgumentException("atlas size must be positive");
			}
			results.Clear();
			bool flag5 = sizes.Length == 0;
			bool flag6;
			if (flag5)
			{
				flag6 = true;
			}
			else
			{
				NoAllocHelpers.EnsureListElemCount<Rect>(results, sizes.Length);
				Texture2D.GenerateAtlasImpl(sizes, padding, atlasSize, NoAllocHelpers.ExtractArrayFromListT<Rect>(results));
				flag6 = results.Count != 0;
			}
			return flag6;
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x0001A280 File Offset: 0x00018480
		public void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel)
		{
			this.SetAllPixels32(colors, miplevel);
		}

		// Token: 0x060012C9 RID: 4809 RVA: 0x0001A28C File Offset: 0x0001848C
		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors)
		{
			this.SetPixels32(colors, 0);
		}

		// Token: 0x060012CA RID: 4810 RVA: 0x0001A298 File Offset: 0x00018498
		public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, [DefaultValue("0")] int miplevel)
		{
			this.SetBlockOfPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
		}

		// Token: 0x060012CB RID: 4811 RVA: 0x0001A2AB File Offset: 0x000184AB
		[ExcludeFromDocs]
		public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors)
		{
			this.SetPixels32(x, y, blockWidth, blockHeight, colors, 0);
		}

		// Token: 0x060012CC RID: 4812 RVA: 0x0001A2C0 File Offset: 0x000184C0
		public Color[] GetPixels([DefaultValue("0")] int miplevel)
		{
			int num = this.width >> miplevel;
			bool flag = num < 1;
			if (flag)
			{
				num = 1;
			}
			int num2 = this.height >> miplevel;
			bool flag2 = num2 < 1;
			if (flag2)
			{
				num2 = 1;
			}
			return this.GetPixels(0, 0, num, num2, miplevel);
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x0001A30C File Offset: 0x0001850C
		[ExcludeFromDocs]
		public Color[] GetPixels()
		{
			return this.GetPixels(0);
		}

		// Token: 0x060012CE RID: 4814
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPixelImpl_Injected(int image, int mip, int x, int y, ref Color color);

		// Token: 0x060012CF RID: 4815
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPixelImpl_Injected(int image, int mip, int x, int y, out Color ret);

		// Token: 0x060012D0 RID: 4816
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPixelBilinearImpl_Injected(int image, int mip, float u, float v, out Color ret);

		// Token: 0x060012D1 RID: 4817
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ReadPixelsImpl_Injected(ref Rect source, int destX, int destY, bool recalculateMipMaps);

		// Token: 0x04000654 RID: 1620
		internal const int streamingMipmapsPriorityMin = -128;

		// Token: 0x04000655 RID: 1621
		internal const int streamingMipmapsPriorityMax = 127;

		// Token: 0x020001CF RID: 463
		[Flags]
		public enum EXRFlags
		{
			// Token: 0x04000657 RID: 1623
			None = 0,
			// Token: 0x04000658 RID: 1624
			OutputAsFloat = 1,
			// Token: 0x04000659 RID: 1625
			CompressZIP = 2,
			// Token: 0x0400065A RID: 1626
			CompressRLE = 4,
			// Token: 0x0400065B RID: 1627
			CompressPIZ = 8
		}
	}
}
