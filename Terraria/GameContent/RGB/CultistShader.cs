using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200022B RID: 555
	public class CultistShader : ChromaShader
	{
		// Token: 0x06001C67 RID: 7271 RVA: 0x004C9DD8 File Offset: 0x004C7FD8
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High,
			EffectDetailLevel.Low
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			time *= 2f;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 vector = this._backgroundColor;
				float num = time * 0.5f + canvasPositionOfIndex.X + canvasPositionOfIndex.Y;
				float num2 = (float)Math.Cos((double)num) * 2f + 2f;
				num2 = MathHelper.Clamp(num2, 0f, 1f);
				num = (num + 3.14159274f) % 18.849556f;
				Vector4 value;
				if (num < 6.28318548f)
				{
					float num3 = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * 0.3f + new Vector2(12.5f, time * 0.2f));
					num3 = Math.Max(0f, 1f - num3 * num3 * 4f * num3);
					num3 = MathHelper.Clamp(num3, 0f, 1f);
					value = Vector4.Lerp(this._fireDarkColor, this._fireBrightColor, num3);
				}
				else if (num < 12.566371f)
				{
					float num4 = NoiseHelper.GetDynamicNoise(new Vector2((canvasPositionOfIndex.X + canvasPositionOfIndex.Y) * 0.2f, 0f), time / 5f);
					num4 = Math.Max(0f, 1f - num4 * 1.5f);
					value = Vector4.Lerp(this._iceDarkColor, this._iceBrightColor, num4);
				}
				else
				{
					float num5 = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.15f, time * 0.05f);
					num5 = (float)Math.Sin((double)(num5 * 15f)) * 0.5f + 0.5f;
					num5 = Math.Max(0f, 1f - 5f * num5);
					value = Vector4.Lerp(this._lightningDarkColor, this._lightningBrightColor, num5);
				}
				vector = Vector4.Lerp(vector, value, num2);
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x040042FE RID: 17150
		private readonly Vector4 _lightningDarkColor = new Color(23, 11, 23).ToVector4();

		// Token: 0x040042FF RID: 17151
		private readonly Vector4 _lightningBrightColor = new Color(249, 140, 255).ToVector4();

		// Token: 0x04004300 RID: 17152
		private readonly Vector4 _fireDarkColor = Color.Red.ToVector4();

		// Token: 0x04004301 RID: 17153
		private readonly Vector4 _fireBrightColor = new Color(255, 196, 0).ToVector4();

		// Token: 0x04004302 RID: 17154
		private readonly Vector4 _iceDarkColor = new Color(4, 4, 148).ToVector4();

		// Token: 0x04004303 RID: 17155
		private readonly Vector4 _iceBrightColor = new Color(208, 233, 255).ToVector4();

		// Token: 0x04004304 RID: 17156
		private readonly Vector4 _backgroundColor = Color.Black.ToVector4();
	}
}
