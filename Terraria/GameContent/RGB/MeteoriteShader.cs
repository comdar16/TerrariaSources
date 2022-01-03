using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200024E RID: 590
	public class MeteoriteShader : ChromaShader
	{
		// Token: 0x06001CD0 RID: 7376 RVA: 0x004CDA34 File Offset: 0x004CBC34
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._baseColor, this._secondaryColor, (float)Math.Sin((double)(time + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CD1 RID: 7377 RVA: 0x004CDA90 File Offset: 0x004CBC90
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector4 vector = this._baseColor;
				float dynamicNoise = NoiseHelper.GetDynamicNoise(gridPositionOfIndex.X, gridPositionOfIndex.Y, time / 10f);
				vector = Vector4.Lerp(vector, this._secondaryColor, dynamicNoise * dynamicNoise);
				float num = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.5f + new Vector2(0f, time * 0.05f), time / 20f);
				num = Math.Max(0f, 1f - num * 2f);
				vector = Vector4.Lerp(vector, this._glowColor, (float)Math.Sqrt((double)num) * 0.75f);
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004366 RID: 17254
		private readonly Vector4 _baseColor = new Color(39, 15, 26).ToVector4();

		// Token: 0x04004367 RID: 17255
		private readonly Vector4 _secondaryColor = new Color(69, 50, 43).ToVector4();

		// Token: 0x04004368 RID: 17256
		private readonly Vector4 _glowColor = Color.DarkOrange.ToVector4();
	}
}
