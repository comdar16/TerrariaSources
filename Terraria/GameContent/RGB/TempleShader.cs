using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000258 RID: 600
	public class TempleShader : ChromaShader
	{
		// Token: 0x06001CF6 RID: 7414 RVA: 0x004CEA70 File Offset: 0x004CCC70
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
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector4 vector = _backgroundColor;
				float num = (NoiseHelper.GetStaticNoise(gridPositionOfIndex.Y * 7) * 10f + time) % 10f - (canvasPositionOfIndex.X + 2f);
				if (num > 0f)
				{
					float amount = Math.Max(0f, 1.2f - num);
					if (num < 0.2f)
					{
						amount = num * 5f;
					}
					vector = Vector4.Lerp(vector, _glowColor, amount);
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004384 RID: 17284
		private readonly Vector4 _backgroundColor = new Vector4(0.05f, 0.025f, 0f, 1f);

		// Token: 0x04004385 RID: 17285
		private readonly Vector4 _glowColor = Color.Orange.ToVector4();
	}
}
