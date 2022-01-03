using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000249 RID: 585
	public class HallowSurfaceShader : ChromaShader
	{
		// Token: 0x06001CC1 RID: 7361 RVA: 0x004CD3DB File Offset: 0x004CB5DB
		public override void Update(float elapsedTime)
		{
			this._lightColor = Main.ColorOfTheSkies.ToVector4() * 0.75f + Vector4.One * 0.25f;
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x004CD40C File Offset: 0x004CB60C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._skyColor, this._groundColor, (float)Math.Sin((double)(time + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CC3 RID: 7363 RVA: 0x004CD468 File Offset: 0x004CB668
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 vector = this._skyColor * this._lightColor;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				float num = NoiseHelper.GetDynamicNoise(gridPositionOfIndex.X, gridPositionOfIndex.Y, time / 20f);
				num = Math.Max(0f, 1f - num * 5f);
				Vector4 vector2 = vector;
				if ((gridPositionOfIndex.X * 100 + gridPositionOfIndex.Y) % 2 == 0)
				{
					vector2 = Vector4.Lerp(vector2, this._yellowFlowerColor, num);
				}
				else
				{
					vector2 = Vector4.Lerp(vector2, this._pinkFlowerColor, num);
				}
				float num2 = (float)Math.Sin((double)canvasPositionOfIndex.X) * 0.3f + 0.7f;
				if (canvasPositionOfIndex.Y > num2)
				{
					vector2 = this._groundColor;
				}
				fragment.SetColor(i, vector2);
			}
		}

		// Token: 0x04004359 RID: 17241
		private readonly Vector4 _skyColor = new Color(150, 220, 220).ToVector4();

		// Token: 0x0400435A RID: 17242
		private readonly Vector4 _groundColor = new Vector4(1f, 0.2f, 0.25f, 1f);

		// Token: 0x0400435B RID: 17243
		private readonly Vector4 _pinkFlowerColor = new Vector4(1f, 0.2f, 0.25f, 1f);

		// Token: 0x0400435C RID: 17244
		private readonly Vector4 _yellowFlowerColor = new Vector4(1f, 1f, 0f, 1f);

		// Token: 0x0400435D RID: 17245
		private Vector4 _lightColor;
	}
}
