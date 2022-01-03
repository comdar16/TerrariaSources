using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000252 RID: 594
	public class SlimeRainShader : ChromaShader
	{
		// Token: 0x06001CDE RID: 7390 RVA: 0x004CE060 File Offset: 0x004CC260
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		}, IsTransparent = true)]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 vector = new Vector4(0f, 0f, 0f, 0.75f);
			for (int i = 0; i < fragment.Count; i++)
			{
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				float num = (NoiseHelper.GetStaticNoise(gridPositionOfIndex.X) * 7f + time * 0.4f) % 7f - canvasPositionOfIndex.Y;
				Vector4 vector2 = vector;
				if (num > 0f)
				{
					float amount = Math.Max(0f, 1.2f - num);
					if (num < 0.4f)
					{
						amount = num / 0.4f;
					}
					int num2 = (gridPositionOfIndex.X % SlimeRainShader._colors.Length + SlimeRainShader._colors.Length) % SlimeRainShader._colors.Length;
					vector2 = Vector4.Lerp(vector2, SlimeRainShader._colors[num2], amount);
				}
				fragment.SetColor(i, vector2);
			}
		}

		// Token: 0x04004371 RID: 17265
		private static readonly Vector4[] _colors = new Vector4[]
		{
			Color.Blue.ToVector4(),
			Color.Green.ToVector4(),
			Color.Purple.ToVector4()
		};
	}
}
