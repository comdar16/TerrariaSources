using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200024B RID: 587
	public class JungleShader : ChromaShader
	{
		// Token: 0x06001CC7 RID: 7367 RVA: 0x004CD710 File Offset: 0x004CB910
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				float dynamicNoise = NoiseHelper.GetDynamicNoise(fragment.GetCanvasPositionOfIndex(i) * 0.3f, time / 5f);
				Vector4 color = Vector4.Lerp(this._backgroundColor, this._sporeColor, dynamicNoise);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CC8 RID: 7368 RVA: 0x004CD768 File Offset: 0x004CB968
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			bool flag = device.Type == RgbDeviceType.Keyboard || device.Type == RgbDeviceType.Virtual;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				float num = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.3f, time / 5f);
				num = Math.Max(0f, 1f - num * 2.5f);
				Vector4 vector = Vector4.Lerp(this._backgroundColor, this._sporeColor, num);
				if (flag)
				{
					float num2 = NoiseHelper.GetDynamicNoise(gridPositionOfIndex.X, gridPositionOfIndex.Y, time / 100f);
					num2 = Math.Max(0f, 1f - num2 * 20f);
					vector = Vector4.Lerp(vector, this._flowerColors[((gridPositionOfIndex.Y * 47 + gridPositionOfIndex.X) % 5 + 5) % 5], num2);
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004361 RID: 17249
		private readonly Vector4 _backgroundColor = new Color(40, 80, 0).ToVector4();

		// Token: 0x04004362 RID: 17250
		private readonly Vector4 _sporeColor = new Color(255, 255, 0).ToVector4();

		// Token: 0x04004363 RID: 17251
		private readonly Vector4[] _flowerColors = new Vector4[]
		{
			Color.Yellow.ToVector4(),
			Color.Pink.ToVector4(),
			Color.Purple.ToVector4(),
			Color.Red.ToVector4(),
			Color.Blue.ToVector4()
		};
	}
}
