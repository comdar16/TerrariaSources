using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000244 RID: 580
	public class CorruptSurfaceShader : ChromaShader
	{
		// Token: 0x06001CAF RID: 7343 RVA: 0x004CCA0C File Offset: 0x004CAC0C
		public CorruptSurfaceShader(Color color)
		{
			this._baseColor = color.ToVector4();
			this._skyColor = Vector4.Lerp(this._baseColor, Color.DeepSkyBlue.ToVector4(), 0.5f);
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x004CCA4F File Offset: 0x004CAC4F
		public CorruptSurfaceShader(Color vineColor, Color skyColor)
		{
			this._baseColor = vineColor.ToVector4();
			this._skyColor = skyColor.ToVector4();
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x004CCA71 File Offset: 0x004CAC71
		public override void Update(float elapsedTime)
		{
			this._lightColor = Main.ColorOfTheSkies.ToVector4() * 0.75f + Vector4.One * 0.25f;
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x004CCAA4 File Offset: 0x004CACA4
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 value = this._skyColor * this._lightColor;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._baseColor, value, (float)Math.Sin((double)(time * 0.5f + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x004CCB14 File Offset: 0x004CAD14
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 vector = _skyColor * _lightColor;
			for (int i = 0; i < fragment.Count; i++)
			{
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				float staticNoise = NoiseHelper.GetStaticNoise(gridPositionOfIndex.X);
				staticNoise = (staticNoise * 10f + time * 0.4f) % 10f;
				float num = 1f;
				if (staticNoise > 1f)
				{
					num = MathHelper.Clamp(1f - (staticNoise - 1.4f), 0f, 1f);
					staticNoise = 1f;
				}
				float num2 = (float)Math.Sin(canvasPositionOfIndex.X) * 0.3f + 0.7f;
				float num3 = staticNoise - (1f - canvasPositionOfIndex.Y);
				Vector4 vector2 = vector;
				if (num3 > 0f)
				{
					float num4 = 1f;
					if (num3 < 0.2f)
					{
						num4 = num3 * 5f;
					}
					vector2 = Vector4.Lerp(vector2, _baseColor, num4 * num);
				}
				if (canvasPositionOfIndex.Y > num2)
				{
					vector2 = _baseColor;
				}
				fragment.SetColor(i, vector2);
			}
		}

		// Token: 0x04004347 RID: 17223
		private readonly Vector4 _baseColor;

		// Token: 0x04004348 RID: 17224
		private readonly Vector4 _skyColor;

		// Token: 0x04004349 RID: 17225
		private Vector4 _lightColor;
	}
}
