using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200022D RID: 557
	public class DeathShader : ChromaShader
	{
		// Token: 0x06001C6B RID: 7275 RVA: 0x004CA222 File Offset: 0x004C8422
		public DeathShader(Color primaryColor, Color secondaryColor)
		{
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x004CA244 File Offset: 0x004C8444
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High,
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			time *= 3f;
			float amount = 0f;
			float num = time % 12.566371f;
			if (num < 3.14159274f)
			{
				amount = (float)Math.Sin((double)num);
			}
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector4 color = Vector4.Lerp(this._primaryColor, this._secondaryColor, amount);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x04004307 RID: 17159
		private readonly Vector4 _primaryColor;

		// Token: 0x04004308 RID: 17160
		private readonly Vector4 _secondaryColor;
	}
}
