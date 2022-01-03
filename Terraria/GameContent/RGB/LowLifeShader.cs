using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200024D RID: 589
	public class LowLifeShader : ChromaShader
	{
		// Token: 0x06001CCD RID: 7373 RVA: 0x004CD9AC File Offset: 0x004CBBAC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low,
			EffectDetailLevel.High
		}, IsTransparent = true)]
		private void ProcessAnyDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			float scaleFactor = (float)Math.Cos((double)(time * 3.14159274f)) * 0.3f + 0.7f;
			Vector4 color = LowLifeShader._baseColor * scaleFactor;
			color.W = LowLifeShader._baseColor.W;
			for (int i = 0; i < fragment.Count; i++)
			{
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x04004365 RID: 17253
		private static Vector4 _baseColor = new Color(40, 0, 8, 255).ToVector4();
	}
}
