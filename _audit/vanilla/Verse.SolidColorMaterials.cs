using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000D4A RID: 3402
	public static class SolidColorMaterials
	{
		// Token: 0x17000D7E RID: 3454
		// (get) Token: 0x06005683 RID: 22147 RVA: 0x001BEEFE File Offset: 0x001BD0FE
		public static int SimpleColorMatCount
		{
			get
			{
				return SolidColorMaterials.simpleColorMats.Count + SolidColorMaterials.simpleColorAndVertexColorMats.Count;
			}
		}

		// Token: 0x06005684 RID: 22148 RVA: 0x001BEF18 File Offset: 0x001BD118
		public static Material SimpleSolidColorMaterial(Color col, bool careAboutVertexColors = false)
		{
			col = col;
			Material material;
			if (careAboutVertexColors)
			{
				if (!SolidColorMaterials.simpleColorAndVertexColorMats.TryGetValue(col, out material))
				{
					material = SolidColorMaterials.NewSolidColorMaterial(col, ShaderDatabase.VertexColor);
					SolidColorMaterials.simpleColorAndVertexColorMats.Add(col, material);
				}
			}
			else if (!SolidColorMaterials.simpleColorMats.TryGetValue(col, out material))
			{
				material = SolidColorMaterials.NewSolidColorMaterial(col, ShaderDatabase.SolidColor);
				SolidColorMaterials.simpleColorMats.Add(col, material);
			}
			return material;
		}

		// Token: 0x06005685 RID: 22149 RVA: 0x001BEF88 File Offset: 0x001BD188
		public static Material NewSolidColorMaterial(Color col, Shader shader)
		{
			if (!UnityData.IsInMainThread)
			{
				Log.Error("Tried to create a material from a different thread.");
				return null;
			}
			Material material = MaterialAllocator.Create(shader);
			material.color = col;
			string text = "SolidColorMat-";
			string name = shader.name;
			string text2 = "-";
			Color color = col;
			material.name = text + name + text2 + color.ToString();
			return material;
		}

		// Token: 0x06005686 RID: 22150 RVA: 0x001BEFDF File Offset: 0x001BD1DF
		public static Texture2D NewSolidColorTexture(float r, float g, float b, float a)
		{
			return SolidColorMaterials.NewSolidColorTexture(new Color(r, g, b, a));
		}

		// Token: 0x06005687 RID: 22151 RVA: 0x001BEFF0 File Offset: 0x001BD1F0
		public static Texture2D NewSolidColorTexture(Color color)
		{
			if (!UnityData.IsInMainThread)
			{
				Log.Error("Tried to create a texture from a different thread.");
				return null;
			}
			Texture2D texture2D = new Texture2D(2, 2);
			string text = "SolidColorTex-";
			Color color2 = color;
			texture2D.name = text + color2.ToString();
			texture2D.SetPixel(0, 0, color);
			texture2D.SetPixel(1, 0, color);
			texture2D.SetPixel(0, 1, color);
			texture2D.SetPixel(1, 1, color);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x04003F35 RID: 16181
		private static Dictionary<Color, Material> simpleColorMats = new Dictionary<Color, Material>();

		// Token: 0x04003F36 RID: 16182
		private static Dictionary<Color, Material> simpleColorAndVertexColorMats = new Dictionary<Color, Material>();
	}
}
