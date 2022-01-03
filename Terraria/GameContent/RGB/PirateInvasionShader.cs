using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000239 RID: 569
	public class PirateInvasionShader : ChromaShader
	{
		// Token: 0x06001C8E RID: 7310 RVA: 0x004CB85C File Offset: 0x004C9A5C
		public PirateInvasionShader(Color cannonBallColor, Color splashColor, Color waterColor, Color backgroundColor)
		{
			this._cannonBallColor = cannonBallColor.ToVector4();
			this._splashColor = splashColor.ToVector4();
			this._waterColor = waterColor.ToVector4();
			this._backgroundColor = backgroundColor.ToVector4();
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x004CB898 File Offset: 0x004C9A98
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._waterColor, this._cannonBallColor, (float)Math.Sin((double)(time * 0.5f + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x004CB8FC File Offset: 0x004C9AFC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				gridPositionOfIndex.X /= 2;
				float num = (NoiseHelper.GetStaticNoise(gridPositionOfIndex.X) * 40f + time * 1f) % 40f;
				float amount = 0f;
				float num2 = num - canvasPositionOfIndex.Y / 1.2f;
				if (num > 1f)
				{
					float num3 = 1f - canvasPositionOfIndex.Y / 1.2f;
					amount = (1f - Math.Min(1f, num2 - num3)) * (1f - Math.Min(1f, num3 / 1f));
				}
				Vector4 vector = this._backgroundColor;
				if (num2 > 0f)
				{
					float amount2 = Math.Max(0f, 1.2f - num2 * 4f);
					if (num2 < 0.1f)
					{
						amount2 = num2 / 0.1f;
					}
					vector = Vector4.Lerp(vector, this._cannonBallColor, amount2);
					vector = Vector4.Lerp(vector, this._splashColor, amount);
				}
				if (canvasPositionOfIndex.Y > 0.8f)
				{
					vector = this._waterColor;
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004324 RID: 17188
		private readonly Vector4 _cannonBallColor;

		// Token: 0x04004325 RID: 17189
		private readonly Vector4 _splashColor;

		// Token: 0x04004326 RID: 17190
		private readonly Vector4 _waterColor;

		// Token: 0x04004327 RID: 17191
		private readonly Vector4 _backgroundColor;
	}
}
