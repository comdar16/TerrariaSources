using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200022C RID: 556
	public class DD2Shader : ChromaShader
	{
		// Token: 0x06001C69 RID: 7273 RVA: 0x004CA08E File Offset: 0x004C828E
		public DD2Shader(Color darkGlowColor, Color lightGlowColor)
		{
			this._darkGlowColor = darkGlowColor.ToVector4();
			this._lightGlowColor = lightGlowColor.ToVector4();
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x004CA0B0 File Offset: 0x004C82B0
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low,
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector2 canvasCenter = fragment.CanvasCenter;
			if (quality == EffectDetailLevel.Low)
			{
				canvasCenter = new Vector2(1.7f, 0.5f);
			}
			time *= 0.5f;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 vector = new Vector4(0f, 0f, 0f, 1f);
				float num = (canvasPositionOfIndex - canvasCenter).Length();
				float num2 = num * num * 0.75f;
				float num3 = (num - time) % 1f;
				if (num3 < 0f)
				{
					num3 += 1f;
				}
				if (num3 > 0.8f)
				{
					num3 *= 1f - (num3 - 1f + 0.2f) / 0.2f;
				}
				else
				{
					num3 /= 0.8f;
				}
				Vector4 value = Vector4.Lerp(this._darkGlowColor, this._lightGlowColor, num3 * num3);
				num3 *= MathHelper.Clamp(1f - num2, 0f, 1f) * 0.75f + 0.25f;
				vector = Vector4.Lerp(vector, value, num3);
				if (num < 0.5f)
				{
					float amount = 1f - MathHelper.Clamp((num - 0.5f + 0.4f) / 0.4f, 0f, 1f);
					vector = Vector4.Lerp(vector, this._lightGlowColor, amount);
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004305 RID: 17157
		private readonly Vector4 _darkGlowColor;

		// Token: 0x04004306 RID: 17158
		private readonly Vector4 _lightGlowColor;
	}
}
