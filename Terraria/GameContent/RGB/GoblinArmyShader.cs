using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000233 RID: 563
	public class GoblinArmyShader : ChromaShader
	{
		// Token: 0x06001C7C RID: 7292 RVA: 0x004CAC88 File Offset: 0x004C8E88
		public GoblinArmyShader(Color primaryColor, Color secondaryColor)
		{
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001C7D RID: 7293 RVA: 0x004CACAC File Offset: 0x004C8EAC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			time *= 0.5f;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				canvasPositionOfIndex.Y = 1f;
				float num = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * 0.3f + new Vector2(12.5f, time * 0.2f));
				num = Math.Max(0f, 1f - num * num * 4f * num);
				num = MathHelper.Clamp(num, 0f, 1f);
				Vector4 vector = Vector4.Lerp(this._primaryColor, this._secondaryColor, num);
				vector = Vector4.Lerp(vector, Vector4.One, num * num);
				Vector4 color = Vector4.Lerp(new Vector4(0f, 0f, 0f, 1f), vector, num);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C7E RID: 7294 RVA: 0x004CAD90 File Offset: 0x004C8F90
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				float num = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * 0.3f + new Vector2(12.5f, time * 0.2f));
				num = Math.Max(0f, 1f - num * num * 4f * num * (1.2f - canvasPositionOfIndex.Y)) * canvasPositionOfIndex.Y * canvasPositionOfIndex.Y;
				num = MathHelper.Clamp(num, 0f, 1f);
				Vector4 vector = Vector4.Lerp(this._primaryColor, this._secondaryColor, num);
				vector = Vector4.Lerp(vector, Vector4.One, num * num * num);
				Vector4 color = Vector4.Lerp(new Vector4(0f, 0f, 0f, 1f), vector, num);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x04004314 RID: 17172
		private readonly Vector4 _primaryColor;

		// Token: 0x04004315 RID: 17173
		private readonly Vector4 _secondaryColor;
	}
}
