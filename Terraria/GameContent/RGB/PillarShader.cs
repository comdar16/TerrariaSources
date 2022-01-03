using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000238 RID: 568
	public class PillarShader : ChromaShader
	{
		// Token: 0x06001C8B RID: 7307 RVA: 0x004CB6C0 File Offset: 0x004C98C0
		public PillarShader(Color primaryColor, Color secondaryColor)
		{
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001C8C RID: 7308 RVA: 0x004CB6E4 File Offset: 0x004C98E4
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._primaryColor, this._secondaryColor, (float)Math.Sin((double)(time * 2.5f + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x004CB748 File Offset: 0x004C9948
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector2 value = new Vector2(1.5f, 0.5f);
			time *= 4f;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 vector = fragment.GetCanvasPositionOfIndex(i) - value;
				float num = vector.Length() * 2f;
				float num2 = (float)Math.Atan2((double)vector.Y, (double)vector.X);
				float amount = (float)Math.Sin((double)(num * 4f - time - num2)) * 0.5f + 0.5f;
				Vector4 color = Vector4.Lerp(this._primaryColor, this._secondaryColor, amount);
				if (num < 1f)
				{
					float num3 = num / 1f;
					num3 *= num3 * num3;
					float amount2 = (float)Math.Sin((double)(4f - time - num2)) * 0.5f + 0.5f;
					color = Vector4.Lerp(this._primaryColor, this._secondaryColor, amount2) * num3;
				}
				color.W = 1f;
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x04004322 RID: 17186
		private readonly Vector4 _primaryColor;

		// Token: 0x04004323 RID: 17187
		private readonly Vector4 _secondaryColor;
	}
}
