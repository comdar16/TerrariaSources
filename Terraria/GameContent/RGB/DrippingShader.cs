using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200025A RID: 602
	public class DrippingShader : ChromaShader
	{
		// Token: 0x06001CFB RID: 7419 RVA: 0x004CED00 File Offset: 0x004CCF00
		public DrippingShader(Color baseColor, Color liquidColor, float viscosity = 1f)
		{
			this._baseColor = baseColor.ToVector4();
			this._liquidColor = liquidColor.ToVector4();
			this._viscosity = viscosity;
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x004CED2C File Offset: 0x004CCF2C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._baseColor, this._liquidColor, (float)Math.Sin((double)(time * 0.5f + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x004CED90 File Offset: 0x004CCF90
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
				float num = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * new Vector2(0.7f * this._viscosity, 0.075f) + new Vector2(0f, time * -0.1f * this._viscosity));
				num = Math.Max(0f, 1f - (canvasPositionOfIndex.Y * 4.5f + 0.5f) * num);
				Vector4 vector = this._baseColor;
				vector = Vector4.Lerp(vector, this._liquidColor, num);
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004389 RID: 17289
		private readonly Vector4 _baseColor;

		// Token: 0x0400438A RID: 17290
		private readonly Vector4 _liquidColor;

		// Token: 0x0400438B RID: 17291
		private readonly float _viscosity;
	}
}
