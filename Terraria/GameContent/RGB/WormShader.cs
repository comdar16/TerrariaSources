using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000241 RID: 577
	public class WormShader : ChromaShader
	{
		// Token: 0x06001CA6 RID: 7334 RVA: 0x004CA605 File Offset: 0x004C8805
		public WormShader()
		{
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x004CC5FF File Offset: 0x004CA7FF
		public WormShader(Color skinColor, Color eyeColor, Color innerEyeColor)
		{
			this._skinColor = skinColor.ToVector4();
			this._eyeColor = eyeColor.ToVector4();
			this._innerEyeColor = innerEyeColor.ToVector4();
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x004CC630 File Offset: 0x004CA830
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				float amount = Math.Max(0f, (float)Math.Sin((double)(time * -3f + canvasPositionOfIndex.X)));
				Vector4 color = Vector4.Lerp(this._skinColor, this._eyeColor, amount);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x004CC694 File Offset: 0x004CA894
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			time *= 0.25f;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				canvasPositionOfIndex.X -= time * 1.5f;
				canvasPositionOfIndex.X %= 2f;
				if (canvasPositionOfIndex.X < 0f)
				{
					canvasPositionOfIndex.X += 2f;
				}
				float num = (canvasPositionOfIndex - new Vector2(0.5f)).Length();
				Vector4 vector = this._skinColor;
				if (num < 0.5f)
				{
					float num2 = MathHelper.Clamp((num - 0.5f + 0.2f) / 0.2f, 0f, 1f);
					vector = Vector4.Lerp(vector, this._eyeColor, 1f - num2);
					if (num < 0.4f)
					{
						num2 = MathHelper.Clamp((num - 0.4f + 0.2f) / 0.2f, 0f, 1f);
						vector = Vector4.Lerp(vector, this._innerEyeColor, 1f - num2);
					}
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x0400433F RID: 17215
		private readonly Vector4 _skinColor;

		// Token: 0x04004340 RID: 17216
		private readonly Vector4 _eyeColor;

		// Token: 0x04004341 RID: 17217
		private readonly Vector4 _innerEyeColor;
	}
}
