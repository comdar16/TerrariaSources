using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200022A RID: 554
	public class BrainShader : ChromaShader
	{
		// Token: 0x06001C64 RID: 7268 RVA: 0x004C9C79 File Offset: 0x004C7E79
		public BrainShader(Color brainColor, Color veinColor)
		{
			this._brainColor = brainColor.ToVector4();
			this._veinColor = veinColor.ToVector4();
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x004C9C9C File Offset: 0x004C7E9C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 color = Vector4.Lerp(this._brainColor, this._veinColor, Math.Max(0f, (float)Math.Sin((double)(time * 3f))));
			for (int i = 0; i < fragment.Count; i++)
			{
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x004C9CF0 File Offset: 0x004C7EF0
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			new Vector2(1.6f, 0.5f);
			Vector4 value = Vector4.Lerp(this._brainColor, this._veinColor, Math.Max(0f, (float)Math.Sin((double)(time * 3f))) * 0.5f + 0.5f);
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 vector = this._brainColor;
				float num = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.15f + new Vector2(time * 0.002f), time * 0.03f);
				num = (float)Math.Sin((double)(num * 10f)) * 0.5f + 0.5f;
				num = Math.Max(0f, 1f - 5f * num);
				vector = Vector4.Lerp(vector, value, num);
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x040042FC RID: 17148
		private readonly Vector4 _brainColor;

		// Token: 0x040042FD RID: 17149
		private readonly Vector4 _veinColor;
	}
}
