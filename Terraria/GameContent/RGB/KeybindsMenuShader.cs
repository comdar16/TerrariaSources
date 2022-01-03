using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200024C RID: 588
	internal class KeybindsMenuShader : ChromaShader
	{
		// Token: 0x06001CCA RID: 7370 RVA: 0x004CD920 File Offset: 0x004CBB20
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low,
			EffectDetailLevel.High
		}, IsTransparent = true)]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			float scaleFactor = (float)Math.Cos((double)(time * 1.57079637f)) * 0.2f + 0.8f;
			Vector4 color = KeybindsMenuShader._baseColor * scaleFactor;
			color.W = KeybindsMenuShader._baseColor.W;
			for (int i = 0; i < fragment.Count; i++)
			{
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x04004364 RID: 17252
		private static Vector4 _baseColor = new Color(20, 20, 20, 245).ToVector4();
	}
}
