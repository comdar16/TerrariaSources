using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200023D RID: 573
	public class SkullShader : ChromaShader
	{
		// Token: 0x06001C9A RID: 7322 RVA: 0x004CBE5C File Offset: 0x004CA05C
		public SkullShader(Color skullColor, Color bloodDark, Color bloodLight)
		{
			this._skullColor = skullColor.ToVector4();
			this._bloodDark = bloodDark.ToVector4();
			this._bloodLight = bloodLight.ToVector4();
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x004CBEAC File Offset: 0x004CA0AC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._skullColor, this._bloodLight, (float)Math.Sin((double)(time * 2f + canvasPositionOfIndex.X * 2f)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C9C RID: 7324 RVA: 0x004CBF14 File Offset: 0x004CA114
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector4 value = _backgroundColor;
				float num = (NoiseHelper.GetStaticNoise(gridPositionOfIndex.X) * 10f + time * 0.75f) % 10f + canvasPositionOfIndex.Y - 1f;
				if (num > 0f)
				{
					float amount = Math.Max(0f, 1.2f - num);
					if (num < 0.2f)
					{
						amount = num * 5f;
					}
					value = Vector4.Lerp(value, _skullColor, amount);
				}
				float staticNoise = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * 0.5f + new Vector2(12.5f, time * 0.2f));
				staticNoise = Math.Max(0f, 1f - staticNoise * staticNoise * 4f * staticNoise * (1f - canvasPositionOfIndex.Y * canvasPositionOfIndex.Y)) * canvasPositionOfIndex.Y * canvasPositionOfIndex.Y;
				staticNoise = MathHelper.Clamp(staticNoise, 0f, 1f);
				Vector4 value2 = Vector4.Lerp(_bloodDark, _bloodLight, staticNoise);
				value = Vector4.Lerp(value, value2, staticNoise);
				fragment.SetColor(i, value);
			}
		}

		// Token: 0x0400432F RID: 17199
		private readonly Vector4 _skullColor;

		// Token: 0x04004330 RID: 17200
		private readonly Vector4 _bloodDark;

		// Token: 0x04004331 RID: 17201
		private readonly Vector4 _bloodLight;

		// Token: 0x04004332 RID: 17202
		private readonly Vector4 _backgroundColor = Color.Black.ToVector4();
	}
}
