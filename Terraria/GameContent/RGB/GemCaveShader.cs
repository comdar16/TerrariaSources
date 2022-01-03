using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000232 RID: 562
	public class GemCaveShader : ChromaShader
	{
		// Token: 0x06001C79 RID: 7289 RVA: 0x004CAA50 File Offset: 0x004C8C50
		public GemCaveShader(Color primaryColor, Color secondaryColor)
		{
			this._primaryColor = primaryColor.ToVector4();
			this._secondaryColor = secondaryColor.ToVector4();
		}

		// Token: 0x06001C7A RID: 7290 RVA: 0x004CAA74 File Offset: 0x004C8C74
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			time *= 0.25f;
			float num = time % 1f;
			bool flag = time % 2f > 1f;
			Vector4 vector = flag ? this._secondaryColor : this._primaryColor;
			Vector4 value = flag ? this._primaryColor : this._secondaryColor;
			num *= 1.2f;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				float num2 = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * 0.5f + new Vector2(0f, time * 0.5f));
				Vector4 vector2 = vector;
				num2 += num;
				if (num2 > 0.999f)
				{
					float amount = MathHelper.Clamp((num2 - 0.999f) / 0.2f, 0f, 1f);
					vector2 = Vector4.Lerp(vector2, value, amount);
				}
				float num3 = NoiseHelper.GetDynamicNoise(gridPositionOfIndex.X, gridPositionOfIndex.Y, time / 100f);
				num3 = Math.Max(0f, 1f - num3 * 20f);
				vector2 = Vector4.Lerp(vector2, GemCaveShader._gemColors[((gridPositionOfIndex.Y * 47 + gridPositionOfIndex.X) % GemCaveShader._gemColors.Length + GemCaveShader._gemColors.Length) % GemCaveShader._gemColors.Length], num3);
				fragment.SetColor(i, vector2);
				fragment.SetColor(i, vector2);
			}
		}

		// Token: 0x04004311 RID: 17169
		private readonly Vector4 _primaryColor;

		// Token: 0x04004312 RID: 17170
		private readonly Vector4 _secondaryColor;

		// Token: 0x04004313 RID: 17171
		private static readonly Vector4[] _gemColors = new Vector4[]
		{
			Color.White.ToVector4(),
			Color.Yellow.ToVector4(),
			Color.Orange.ToVector4(),
			Color.Red.ToVector4(),
			Color.Green.ToVector4(),
			Color.Blue.ToVector4(),
			Color.Purple.ToVector4()
		};
	}
}
