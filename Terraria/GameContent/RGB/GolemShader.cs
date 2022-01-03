using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000234 RID: 564
	public class GolemShader : ChromaShader
	{
		// Token: 0x06001C7F RID: 7295 RVA: 0x004CAE7B File Offset: 0x004C907B
		public GolemShader(Color glowColor, Color coreColor, Color backgroundColor)
		{
			this._glowColor = glowColor.ToVector4();
			this._coreColor = coreColor.ToVector4();
			this._backgroundColor = backgroundColor.ToVector4();
		}

		// Token: 0x06001C80 RID: 7296 RVA: 0x004CAEAC File Offset: 0x004C90AC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector4 value = Vector4.Lerp(this._backgroundColor, this._coreColor, Math.Max(0f, (float)Math.Sin((double)(time * 0.5f))));
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(value, this._glowColor, Math.Max(0f, (float)Math.Sin((double)(canvasPositionOfIndex.X * 2f + time + 101f))));
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C81 RID: 7297 RVA: 0x004CAF38 File Offset: 0x004C9138
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			float num = 0.5f + (float)Math.Sin(time * 3f) * 0.1f;
			Vector2 vector = new Vector2(1.6f, 0.5f);
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector4 vector2 = _backgroundColor;
				float num2 = (NoiseHelper.GetStaticNoise(gridPositionOfIndex.Y) * 10f + time * 2f) % 10f - Math.Abs(canvasPositionOfIndex.X - vector.X);
				if (num2 > 0f)
				{
					float amount = Math.Max(0f, 1.2f - num2);
					if (num2 < 0.2f)
					{
						amount = num2 * 5f;
					}
					vector2 = Vector4.Lerp(vector2, _glowColor, amount);
				}
				float num3 = (canvasPositionOfIndex - vector).Length();
				if (num3 < num)
				{
					float amount2 = 1f - MathHelper.Clamp((num3 - num + 0.1f) / 0.1f, 0f, 1f);
					vector2 = Vector4.Lerp(vector2, _coreColor, amount2);
				}
				fragment.SetColor(i, vector2);
			}
		}

		// Token: 0x04004316 RID: 17174
		private readonly Vector4 _glowColor;

		// Token: 0x04004317 RID: 17175
		private readonly Vector4 _coreColor;

		// Token: 0x04004318 RID: 17176
		private readonly Vector4 _backgroundColor;
	}
}
