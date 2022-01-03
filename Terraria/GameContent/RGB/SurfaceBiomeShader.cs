using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000253 RID: 595
	public class SurfaceBiomeShader : ChromaShader
	{
		// Token: 0x06001CE1 RID: 7393 RVA: 0x004CE1A0 File Offset: 0x004CC3A0
		public SurfaceBiomeShader(Color primaryColor, Color secondaryColor)
		{
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x004CE1C4 File Offset: 0x004CC3C4
		public override void Update(float elapsedTime)
		{
			this._surfaceColor = Main.ColorOfTheSkies.ToVector4() * 0.75f + Vector4.One * 0.25f;
			if (Main.dayTime)
			{
				float num = (float)(Main.time / 54000.0);
				if (num < 0.25f)
				{
					this._starVisibility = 1f - num / 0.25f;
					return;
				}
				if (num > 0.75f)
				{
					this._starVisibility = (num - 0.75f) / 0.25f;
					return;
				}
			}
			else
			{
				this._starVisibility = 1f;
			}
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x004CE25C File Offset: 0x004CC45C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 value = this._primaryColor * this._surfaceColor;
			Vector4 value2 = this._secondaryColor * this._surfaceColor;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(value, value2, (float)Math.Sin((double)(time * 0.5f + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x004CE2DC File Offset: 0x004CC4DC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 value = this._primaryColor * this._surfaceColor;
			Vector4 value2 = this._secondaryColor * this._surfaceColor;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				float amount = (float)Math.Sin((double)(canvasPositionOfIndex.X * 1.5f + canvasPositionOfIndex.Y + time)) * 0.5f + 0.5f;
				Vector4 vector = Vector4.Lerp(value, value2, amount);
				float num = NoiseHelper.GetDynamicNoise(gridPositionOfIndex.X, gridPositionOfIndex.Y, time / 60f);
				num = Math.Max(0f, 1f - num * 20f);
				num *= 1f - this._surfaceColor.X;
				vector = Vector4.Max(vector, new Vector4(num * this._starVisibility));
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004372 RID: 17266
		private readonly Vector4 _primaryColor;

		// Token: 0x04004373 RID: 17267
		private readonly Vector4 _secondaryColor;

		// Token: 0x04004374 RID: 17268
		private Vector4 _surfaceColor;

		// Token: 0x04004375 RID: 17269
		private float _starVisibility;
	}
}
