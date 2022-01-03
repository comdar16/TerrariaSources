using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000236 RID: 566
	public class LavaIndicatorShader : ChromaShader
	{
		// Token: 0x06001C85 RID: 7301 RVA: 0x004CB192 File Offset: 0x004C9392
		public LavaIndicatorShader(Color backgroundColor, Color primaryColor, Color secondaryColor)
		{
			this._backgroundColor = backgroundColor.ToVector4();
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001C86 RID: 7302 RVA: 0x004CB1C4 File Offset: 0x004C93C4
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				float num = NoiseHelper.GetStaticNoise(fragment.GetCanvasPositionOfIndex(i) * 0.3f + new Vector2(12.5f, time * 0.2f));
				num = Math.Max(0f, 1f - num * num * 4f * num);
				num = MathHelper.Clamp(num, 0f, 1f);
				Vector4 color = Vector4.Lerp(this._primaryColor, this._secondaryColor, num);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C87 RID: 7303 RVA: 0x004CB25C File Offset: 0x004C945C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 vector = this._backgroundColor;
				float dynamicNoise = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.2f, time * 0.5f);
				float num = 0.4f;
				num += dynamicNoise * 0.4f;
				float num2 = 1.1f - canvasPositionOfIndex.Y;
				if (num2 < num)
				{
					float num3 = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * 0.3f + new Vector2(12.5f, time * 0.2f));
					num3 = Math.Max(0f, 1f - num3 * num3 * 4f * num3);
					num3 = MathHelper.Clamp(num3, 0f, 1f);
					Vector4 value = Vector4.Lerp(this._primaryColor, this._secondaryColor, num3);
					float amount = 1f - MathHelper.Clamp((num2 - num + 0.2f) / 0.2f, 0f, 1f);
					vector = Vector4.Lerp(vector, value, amount);
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x0400431B RID: 17179
		private readonly Vector4 _backgroundColor;

		// Token: 0x0400431C RID: 17180
		private readonly Vector4 _primaryColor;

		// Token: 0x0400431D RID: 17181
		private readonly Vector4 _secondaryColor;
	}
}
