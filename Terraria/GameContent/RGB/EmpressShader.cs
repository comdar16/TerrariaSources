using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200022F RID: 559
	public class EmpressShader : ChromaShader
	{
		// Token: 0x06001C70 RID: 7280 RVA: 0x004CA3AC File Offset: 0x004C85AC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		}, IsTransparent = false)]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			float num = time * 2f;
			for (int i = 0; i < fragment.Count; i++)
			{
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				float staticNoise = NoiseHelper.GetStaticNoise(gridPositionOfIndex.X);
				float num2 = MathHelper.Max(0f, (float)Math.Cos((double)((staticNoise + num) * 6.28318548f * 0.2f)));
				Vector4 vector = Color.Lerp(Color.Black, Color.Indigo, 0.5f).ToVector4();
				float amount = Math.Max(0f, (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * 2f + canvasPositionOfIndex.X * 1f)));
				amount = 0f;
				vector = Vector4.Lerp(vector, new Vector4(1f, 0.1f, 0.1f, 1f), amount);
				float num3 = (num2 + canvasPositionOfIndex.X + canvasPositionOfIndex.Y) % 1f;
				if (num3 > 0f)
				{
					int num4 = (gridPositionOfIndex.X + gridPositionOfIndex.Y) % EmpressShader._colors.Length;
					if (num4 < 0)
					{
						num4 += EmpressShader._colors.Length;
					}
					Vector4 value = Main.hslToRgb(((canvasPositionOfIndex.X + canvasPositionOfIndex.Y) * 0.15f + time * 0.1f) % 1f, 1f, 0.5f, byte.MaxValue).ToVector4();
					vector = Vector4.Lerp(vector, value, num3);
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x004CA52C File Offset: 0x004C872C
		private static void RedsVersion(Fragment fragment, float time)
		{
			time *= 3f;
			for (int i = 0; i < fragment.Count; i++)
			{
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				float num = (NoiseHelper.GetStaticNoise(gridPositionOfIndex.X) * 7f + time * 0.4f) % 7f - canvasPositionOfIndex.Y;
				Vector4 vector = default(Vector4);
				if (num > 0f)
				{
					float amount = Math.Max(0f, 1.4f - num);
					if (num < 0.4f)
					{
						amount = num / 0.4f;
					}
					int num2 = (gridPositionOfIndex.X + EmpressShader._colors.Length + (int)(time / 6f)) % EmpressShader._colors.Length;
					vector = Vector4.Lerp(vector, EmpressShader._colors[num2], amount);
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x0400430B RID: 17163
		private static readonly Vector4[] _colors = new Vector4[]
		{
			new Vector4(1f, 0.1f, 0.1f, 1f),
			new Vector4(1f, 0.5f, 0.1f, 1f),
			new Vector4(1f, 1f, 0.1f, 1f),
			new Vector4(0.5f, 1f, 0.1f, 1f),
			new Vector4(0.1f, 1f, 0.1f, 1f),
			new Vector4(0.1f, 1f, 0.5f, 1f),
			new Vector4(0.1f, 1f, 1f, 1f),
			new Vector4(0.1f, 0.5f, 1f, 1f),
			new Vector4(0.1f, 0.1f, 1f, 1f),
			new Vector4(0.5f, 0.1f, 1f, 1f),
			new Vector4(1f, 0.1f, 1f, 1f),
			new Vector4(1f, 0.1f, 0.5f, 1f)
		};
	}
}
