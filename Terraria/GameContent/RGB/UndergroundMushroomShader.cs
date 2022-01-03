using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200025C RID: 604
	public class UndergroundMushroomShader : ChromaShader
	{
		// Token: 0x06001D01 RID: 7425 RVA: 0x004CEFE4 File Offset: 0x004CD1E4
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._edgeGlowColor, this._sporeColor, (float)Math.Sin((double)(time * 0.5f + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x004CF048 File Offset: 0x004CD248
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 value = _baseColor;
				float num = ((NoiseHelper.GetStaticNoise(gridPositionOfIndex.X) * 10f + time * 0.2f) % 10f - (1f - canvasPositionOfIndex.Y)) * 2f;
				if (num > 0f)
				{
					float amount = Math.Max(0f, 1.5f - num);
					if (num < 0.5f)
					{
						amount = num * 2f;
					}
					value = Vector4.Lerp(value, _sporeColor, amount);
				}
				float staticNoise = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * 0.3f + new Vector2(0f, time * 0.1f));
				staticNoise = Math.Max(0f, 1f - staticNoise * (1f + (1f - canvasPositionOfIndex.Y) * 4f));
				staticNoise *= Math.Max(0f, (canvasPositionOfIndex.Y - 0.3f) / 0.7f);
				value = Vector4.Lerp(value, _edgeGlowColor, staticNoise);
				fragment.SetColor(i, value);
			}
		}


		// Token: 0x0400438F RID: 17295
		private readonly Vector4 _baseColor = new Color(10, 10, 10).ToVector4();

		// Token: 0x04004390 RID: 17296
		private readonly Vector4 _edgeGlowColor = new Color(0, 0, 255).ToVector4();

		// Token: 0x04004391 RID: 17297
		private readonly Vector4 _sporeColor = new Color(255, 230, 150).ToVector4();
	}
}
