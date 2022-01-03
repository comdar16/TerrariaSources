using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200023A RID: 570
	public class PlanteraShader : ChromaShader
	{
		// Token: 0x06001C91 RID: 7313 RVA: 0x004CBA39 File Offset: 0x004C9C39
		public PlanteraShader(Color bulbColor, Color vineColor, Color backgroundColor)
		{
			this._bulbColor = bulbColor.ToVector4();
			this._vineColor = vineColor.ToVector4();
			this._backgroundColor = backgroundColor.ToVector4();
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x004CBA68 File Offset: 0x004C9C68
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._bulbColor, this._vineColor, (float)Math.Sin((double)(time * 2f + canvasPositionOfIndex.X * 10f)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x004CBAD0 File Offset: 0x004C9CD0
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
				canvasPositionOfIndex.X -= 1.8f;
				if (canvasPositionOfIndex.X < 0f)
				{
					canvasPositionOfIndex.X *= -1f;
					gridPositionOfIndex.Y += 101;
				}
				float num = NoiseHelper.GetStaticNoise(gridPositionOfIndex.Y);
				num = (num * 5f + time * 0.4f) % 5f;
				float num2 = 1f;
				if (num > 1f)
				{
					num2 = 1f - MathHelper.Clamp((num - 0.4f - 1f) / 0.4f, 0f, 1f);
					num = 1f;
				}
				float num3 = num - canvasPositionOfIndex.X / 5f;
				Vector4 color = this._backgroundColor;
				if (num3 > 0f)
				{
					float num4 = 1f;
					if (num3 < 0.2f)
					{
						num4 = num3 / 0.2f;
					}
					if ((gridPositionOfIndex.X + 7 * gridPositionOfIndex.Y) % 5 == 0)
					{
						color = Vector4.Lerp(this._backgroundColor, this._bulbColor, num4 * num2);
					}
					else
					{
						color = Vector4.Lerp(this._backgroundColor, this._vineColor, num4 * num2);
					}
				}
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x04004328 RID: 17192
		private readonly Vector4 _bulbColor;

		// Token: 0x04004329 RID: 17193
		private readonly Vector4 _vineColor;

		// Token: 0x0400432A RID: 17194
		private readonly Vector4 _backgroundColor;
	}
}
