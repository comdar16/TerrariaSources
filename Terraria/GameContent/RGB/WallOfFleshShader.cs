using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000240 RID: 576
	public class WallOfFleshShader : ChromaShader
	{
		// Token: 0x06001CA4 RID: 7332 RVA: 0x004CC55B File Offset: 0x004CA75B
		public WallOfFleshShader(Color primaryColor, Color secondaryColor)
		{
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x004CC580 File Offset: 0x004CA780
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High,
			EffectDetailLevel.Low
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 vector = this._secondaryColor;
				float num = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * 0.3f, time / 5f);
				num = Math.Max(0f, 1f - num * 2f);
				vector = Vector4.Lerp(vector, this._primaryColor, (float)Math.Sqrt((double)num) * 0.75f);
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x0400433D RID: 17213
		private readonly Vector4 _primaryColor;

		// Token: 0x0400433E RID: 17214
		private readonly Vector4 _secondaryColor;
	}
}
