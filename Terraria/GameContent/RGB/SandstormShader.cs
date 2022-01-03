using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000251 RID: 593
	public class SandstormShader : ChromaShader
	{
		// Token: 0x06001CDC RID: 7388 RVA: 0x004CDF94 File Offset: 0x004CC194
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low,
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			if (quality == EffectDetailLevel.Low)
			{
				time *= 0.25f;
			}
			for (int i = 0; i < fragment.Count; i++)
			{
				float staticNoise = NoiseHelper.GetStaticNoise(fragment.GetCanvasPositionOfIndex(i) * 0.3f + new Vector2(time, -time) * 0.5f);
				Vector4 color = Vector4.Lerp(this._backColor, this._frontColor, staticNoise);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x0400436F RID: 17263
		private readonly Vector4 _backColor = new Vector4(0.2f, 0f, 0f, 1f);

		// Token: 0x04004370 RID: 17264
		private readonly Vector4 _frontColor = new Vector4(1f, 0.5f, 0f, 1f);
	}
}
