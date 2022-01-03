using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000255 RID: 597
	public class DrowningShader : ChromaShader
	{
		// Token: 0x06001CE6 RID: 7398 RVA: 0x004CE410 File Offset: 0x004CC610
		public override void Update(float elapsedTime)
		{
			Player player = Main.player[Main.myPlayer];
			this._breath = (float)(player.breath * player.breathCDMax - player.breathCD) / (float)(player.breathMax * player.breathCDMax);
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x004CE454 File Offset: 0x004CC654
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		}, IsTransparent = true)]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = new Vector4(0f, 0f, 1f, 1f - this._breath);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x004CE4A4 File Offset: 0x004CC6A4
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		}, IsTransparent = true)]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			float num = this._breath * 1.2f - 0.1f;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 zero = Vector4.Zero;
				if (canvasPositionOfIndex.Y > num)
				{
					zero = new Vector4(0f, 0f, 1f, MathHelper.Clamp((canvasPositionOfIndex.Y - num) * 5f, 0f, 1f));
				}
				fragment.SetColor(i, zero);
			}
		}

		// Token: 0x04004378 RID: 17272
		private float _breath = 1f;
	}
}
