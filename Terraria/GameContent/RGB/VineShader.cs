using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200025D RID: 605
	public class VineShader : ChromaShader
	{
		// Token: 0x06001D04 RID: 7428 RVA: 0x004CF1E8 File Offset: 0x004CD3E8
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				fragment.GetCanvasPositionOfIndex(i);
				fragment.SetColor(i, this._backgroundColor);
			}
		}

		// Token: 0x06001D05 RID: 7429 RVA: 0x004CF21C File Offset: 0x004CD41C
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
				float staticNoise = NoiseHelper.GetStaticNoise(gridPositionOfIndex.X);
				staticNoise = (staticNoise * 10f + time * 0.4f) % 10f;
				float num = 1f;
				if (staticNoise > 1f)
				{
					num = 1f - MathHelper.Clamp((staticNoise - 0.4f - 1f) / 0.4f, 0f, 1f);
					staticNoise = 1f;
				}
				float num2 = staticNoise - canvasPositionOfIndex.Y / 1f;
				Vector4 vector = _backgroundColor;
				if (num2 > 0f)
				{
					float num3 = 1f;
					if (num2 < 0.2f)
					{
						num3 = num2 / 0.2f;
					}
					vector = Vector4.Lerp(_backgroundColor, _vineColor, num3 * num);
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004392 RID: 17298
		private readonly Vector4 _backgroundColor = new Color(46, 17, 6).ToVector4();

		// Token: 0x04004393 RID: 17299
		private readonly Vector4 _vineColor = Color.Green.ToVector4();
	}
}
