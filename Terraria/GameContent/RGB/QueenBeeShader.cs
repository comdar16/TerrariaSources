using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200023B RID: 571
	public class QueenBeeShader : ChromaShader
	{
		// Token: 0x06001C94 RID: 7316 RVA: 0x004CBC29 File Offset: 0x004C9E29
		public QueenBeeShader(Color primaryColor, Color secondaryColor)
		{
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x004CBC4C File Offset: 0x004C9E4C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._primaryColor, this._secondaryColor, (float)Math.Sin((double)(time * 2f + canvasPositionOfIndex.X * 10f)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x004CBCB4 File Offset: 0x004C9EB4
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			time *= 0.5f;
			for (int i = 0; i < fragment.Count; i++)
			{
				float amount = MathHelper.Clamp((float)Math.Sin((double)fragment.GetCanvasPositionOfIndex(i).X * 5.0 - (double)(4f * time)) * 1.5f, 0f, 1f);
				Vector4 color = Vector4.Lerp(this._primaryColor, this._secondaryColor, amount);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x0400432B RID: 17195
		private readonly Vector4 _primaryColor;

		// Token: 0x0400432C RID: 17196
		private readonly Vector4 _secondaryColor;
	}
}
