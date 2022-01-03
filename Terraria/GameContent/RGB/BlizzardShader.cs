using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000242 RID: 578
	public class BlizzardShader : ChromaShader
	{
		// Token: 0x06001CAA RID: 7338 RVA: 0x004CC7BC File Offset: 0x004CA9BC
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
				float staticNoise = NoiseHelper.GetStaticNoise(fragment.GetCanvasPositionOfIndex(i) * new Vector2(0.2f, 0.4f) + new Vector2(time * 0.35f, time * -0.35f));
				Vector4 color = Vector4.Lerp(this._backColor, this._frontColor, staticNoise * staticNoise);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x04004342 RID: 17218
		private readonly Vector4 _backColor = new Vector4(0.1f, 0.1f, 0.3f, 1f);

		// Token: 0x04004343 RID: 17219
		private readonly Vector4 _frontColor = new Vector4(1f, 1f, 1f, 1f);
	}
}
