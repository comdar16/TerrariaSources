using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000231 RID: 561
	public class FrostLegionShader : ChromaShader
	{
		// Token: 0x06001C77 RID: 7287 RVA: 0x004CA981 File Offset: 0x004C8B81
		public FrostLegionShader(Color primaryColor, Color secondaryColor)
		{
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001C78 RID: 7288 RVA: 0x004CA9A4 File Offset: 0x004C8BA4
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
				float staticNoise = NoiseHelper.GetStaticNoise(fragment.GetGridPositionOfIndex(i).X / 2);
				float num = (canvasPositionOfIndex.Y + canvasPositionOfIndex.X / 2f - staticNoise + time) % 2f;
				if (num < 0f)
				{
					num += 2f;
				}
				if (num < 0.2f)
				{
					num = 1f - num / 0.2f;
				}
				float amount = num / 2f;
				Vector4 color = Vector4.Lerp(this._primaryColor, this._secondaryColor, amount);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x0400430F RID: 17167
		private readonly Vector4 _primaryColor;

		// Token: 0x04004310 RID: 17168
		private readonly Vector4 _secondaryColor;
	}
}
