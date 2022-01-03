using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200022E RID: 558
	public class DukeFishronShader : ChromaShader
	{
		// Token: 0x06001C6D RID: 7277 RVA: 0x004CA2A7 File Offset: 0x004C84A7
		public DukeFishronShader(Color primaryColor, Color secondaryColor)
		{
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x004CA2CC File Offset: 0x004C84CC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._primaryColor, this._secondaryColor, Math.Max(0f, (float)Math.Sin((double)(time * 2f + canvasPositionOfIndex.X))));
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x004CA32C File Offset: 0x004C852C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				float dynamicNoise = NoiseHelper.GetDynamicNoise(fragment.GetGridPositionOfIndex(i).Y, time);
				float val = (float)Math.Sin(canvasPositionOfIndex.X + 2f * time + dynamicNoise) - 0.2f;
				val = Math.Max(0f, val);
				Vector4 vector = Vector4.Lerp(_primaryColor, _secondaryColor, val);
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004309 RID: 17161
		private readonly Vector4 _primaryColor;

		// Token: 0x0400430A RID: 17162
		private readonly Vector4 _secondaryColor;
	}
}
