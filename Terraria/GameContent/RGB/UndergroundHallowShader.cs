using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200025B RID: 603
	public class UndergroundHallowShader : ChromaShader
	{
		// Token: 0x06001CFE RID: 7422 RVA: 0x004CEE48 File Offset: 0x004CD048
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._pinkCrystalColor, this._blueCrystalColor, (float)Math.Sin((double)(time * 2f + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CFF RID: 7423 RVA: 0x004CEEAC File Offset: 0x004CD0AC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				fragment.GetGridPositionOfIndex(i);
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 vector = this._baseColor;
				float num = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.4f, time * 0.05f);
				num = Math.Max(0f, 1f - 2.5f * num);
				float num2 = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.4f + new Vector2(0.05f, 0f), time * 0.05f);
				num2 = Math.Max(0f, 1f - 2.5f * num2);
				if (num > num2)
				{
					vector = Vector4.Lerp(vector, this._pinkCrystalColor, num);
				}
				else
				{
					vector = Vector4.Lerp(vector, this._blueCrystalColor, num2);
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x0400438C RID: 17292
		private readonly Vector4 _baseColor = new Color(0.05f, 0.05f, 0.05f).ToVector4();

		// Token: 0x0400438D RID: 17293
		private readonly Vector4 _pinkCrystalColor = Color.HotPink.ToVector4();

		// Token: 0x0400438E RID: 17294
		private readonly Vector4 _blueCrystalColor = Color.DeepSkyBlue.ToVector4();
	}
}
