using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200023F RID: 575
	public class UnderworldShader : ChromaShader
	{
		// Token: 0x06001CA1 RID: 7329 RVA: 0x004CC46C File Offset: 0x004CA66C
		public UnderworldShader(Color backColor, Color frontColor, float speed)
		{
			this._backColor = backColor.ToVector4();
			this._frontColor = frontColor.ToVector4();
			this._speed = speed;
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x004CC498 File Offset: 0x004CA698
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._backColor, this._frontColor, (float)Math.Sin((double)(time * this._speed + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x004CC4FC File Offset: 0x004CA6FC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				float dynamicNoise = NoiseHelper.GetDynamicNoise(fragment.GetCanvasPositionOfIndex(i) * 0.5f, time * this._speed / 3f);
				Vector4 color = Vector4.Lerp(this._backColor, this._frontColor, dynamicNoise);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x0400433A RID: 17210
		private readonly Vector4 _backColor;

		// Token: 0x0400433B RID: 17211
		private readonly Vector4 _frontColor;

		// Token: 0x0400433C RID: 17212
		private readonly float _speed;
	}
}
