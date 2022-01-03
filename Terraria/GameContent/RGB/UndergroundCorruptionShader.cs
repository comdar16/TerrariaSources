using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000259 RID: 601
	public class UndergroundCorruptionShader : ChromaShader
	{
		// Token: 0x06001CF8 RID: 7416 RVA: 0x004CEB60 File Offset: 0x004CCD60
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 value = Vector4.Lerp(this._flameColor, this._flameTipColor, 0.25f);
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._corruptionColor, value, (float)Math.Sin((double)(time + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x004CEBD0 File Offset: 0x004CCDD0
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
				float num = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.3f + new Vector2(12.5f, time * 0.05f), time * 0.1f);
				num = Math.Max(0f, 1f - num * num * 4f * (1.2f - canvasPositionOfIndex.Y)) * canvasPositionOfIndex.Y;
				num = MathHelper.Clamp(num, 0f, 1f);
				Vector4 value = Vector4.Lerp(this._flameColor, this._flameTipColor, num);
				Vector4 color = Vector4.Lerp(this._corruptionColor, value, num);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x04004386 RID: 17286
		private readonly Vector4 _corruptionColor = new Vector4(Color.Purple.ToVector3() * 0.2f, 1f);

		// Token: 0x04004387 RID: 17287
		private readonly Vector4 _flameColor = Color.Green.ToVector4();

		// Token: 0x04004388 RID: 17288
		private readonly Vector4 _flameTipColor = Color.Yellow.ToVector4();
	}
}
