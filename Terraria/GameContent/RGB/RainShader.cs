using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000250 RID: 592
	public class RainShader : ChromaShader
	{
		// Token: 0x06001CD9 RID: 7385 RVA: 0x004CDE8C File Offset: 0x004CC08C
		public override void Update(float elapsedTime)
		{
			this._inBloodMoon = Main.bloodMoon;
		}

		// Token: 0x06001CDA RID: 7386 RVA: 0x004CDE9C File Offset: 0x004CC09C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		}, IsTransparent = true)]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 value = ((!_inBloodMoon) ? new Vector4(0f, 0f, 1f, 1f) : new Vector4(1f, 0f, 0f, 1f));
			Vector4 vector = new Vector4(0f, 0f, 0f, 0.75f);
			for (int i = 0; i < fragment.Count; i++)
			{
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				float num = (NoiseHelper.GetStaticNoise(gridPositionOfIndex.X) * 10f + time) % 10f - canvasPositionOfIndex.Y;
				Vector4 vector2 = vector;
				if (num > 0f)
				{
					float amount = Math.Max(0f, 1.2f - num);
					if (num < 0.2f)
					{
						amount = num * 5f;
					}
					vector2 = Vector4.Lerp(vector2, value, amount);
				}
				fragment.SetColor(i, vector2);
			}
		}

		// Token: 0x0400436E RID: 17262
		private bool _inBloodMoon;
	}
}
