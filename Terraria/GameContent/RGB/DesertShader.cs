using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000246 RID: 582
	public class DesertShader : ChromaShader
	{
		// Token: 0x06001CB8 RID: 7352 RVA: 0x004CCD48 File Offset: 0x004CAF48
		public DesertShader(Color baseColor, Color sandColor)
		{
			this._baseColor = baseColor.ToVector4();
			this._sandColor = sandColor.ToVector4();
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x004CCD6C File Offset: 0x004CAF6C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low,
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				fragment.GetGridPositionOfIndex(i);
				canvasPositionOfIndex.Y += (float)Math.Sin((double)(canvasPositionOfIndex.X * 2f + time * 2f)) * 0.2f;
				float staticNoise = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * new Vector2(0.1f, 0.5f));
				Vector4 color = Vector4.Lerp(this._baseColor, this._sandColor, staticNoise * staticNoise);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x0400434A RID: 17226
		private readonly Vector4 _baseColor;

		// Token: 0x0400434B RID: 17227
		private readonly Vector4 _sandColor;
	}
}
