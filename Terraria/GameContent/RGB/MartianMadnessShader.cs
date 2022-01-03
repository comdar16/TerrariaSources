using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000237 RID: 567
	public class MartianMadnessShader : ChromaShader
	{
		// Token: 0x06001C88 RID: 7304 RVA: 0x004CB37E File Offset: 0x004C957E
		public MartianMadnessShader(Color metalColor, Color glassColor, Color beamColor, Color backgroundColor)
		{
			this._metalColor = metalColor.ToVector4();
			this._glassColor = glassColor.ToVector4();
			this._beamColor = beamColor.ToVector4();
			this._backgroundColor = backgroundColor.ToVector4();
		}

		// Token: 0x06001C89 RID: 7305 RVA: 0x004CB3BC File Offset: 0x004C95BC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				float amount = (float)Math.Sin((double)(time * 2f + canvasPositionOfIndex.X * 5f)) * 0.5f + 0.5f;
				int num = (gridPositionOfIndex.X + gridPositionOfIndex.Y) % 2;
				if (num < 0)
				{
					num += 2;
				}
				Vector4 color = (num == 1) ? Vector4.Lerp(this._glassColor, this._beamColor, amount) : this._metalColor;
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C8A RID: 7306 RVA: 0x004CB460 File Offset: 0x004C9660
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			if (device.Type != RgbDeviceType.Keyboard && device.Type != RgbDeviceType.Virtual)
			{
				this.ProcessLowDetail(device, fragment, quality, time);
				return;
			}
			float num = time * 0.5f % 6.28318548f;
			if (num > 3.14159274f)
			{
				num = 6.28318548f - num;
			}
			Vector2 vector = new Vector2(1.7f + (float)Math.Cos((double)num) * 2f, -0.5f + (float)Math.Sin((double)num) * 1.1f);
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 vector2 = this._backgroundColor;
				float num2 = Math.Abs(vector.X - canvasPositionOfIndex.X);
				if (canvasPositionOfIndex.Y > vector.Y && num2 < 0.2f)
				{
					float num3 = 1f - MathHelper.Clamp((num2 - 0.2f + 0.2f) / 0.2f, 0f, 1f);
					float num4 = Math.Abs((num - 1.57079637f) / 1.57079637f);
					num4 = Math.Max(0f, 1f - num4 * 3f);
					vector2 = Vector4.Lerp(vector2, this._beamColor, num3 * num4);
				}
				Vector2 vector3 = vector - canvasPositionOfIndex;
				vector3.X /= 1f;
				vector3.Y /= 0.2f;
				float num5 = vector3.Length();
				if (num5 < 1f)
				{
					float amount = 1f - MathHelper.Clamp((num5 - 1f + 0.2f) / 0.2f, 0f, 1f);
					vector2 = Vector4.Lerp(vector2, this._metalColor, amount);
				}
				Vector2 vector4 = vector - canvasPositionOfIndex + new Vector2(0f, -0.1f);
				vector4.X /= 0.3f;
				vector4.Y /= 0.3f;
				if (vector4.Y < 0f)
				{
					vector4.Y *= 2f;
				}
				float num6 = vector4.Length();
				if (num6 < 1f)
				{
					float amount2 = 1f - MathHelper.Clamp((num6 - 1f + 0.2f) / 0.2f, 0f, 1f);
					vector2 = Vector4.Lerp(vector2, this._glassColor, amount2);
				}
				fragment.SetColor(i, vector2);
			}
		}

		// Token: 0x0400431E RID: 17182
		private readonly Vector4 _metalColor;

		// Token: 0x0400431F RID: 17183
		private readonly Vector4 _glassColor;

		// Token: 0x04004320 RID: 17184
		private readonly Vector4 _beamColor;

		// Token: 0x04004321 RID: 17185
		private readonly Vector4 _backgroundColor;
	}
}
