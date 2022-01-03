using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200023E RID: 574
	public class TwinsShader : ChromaShader
	{
		// Token: 0x06001C9D RID: 7325 RVA: 0x004CC05C File Offset: 0x004CA25C
		public TwinsShader(Color eyeColor, Color veinColor, Color laserColor, Color mouthColor, Color flameColor, Color backgroundColor)
		{
			this._eyeColor = eyeColor.ToVector4();
			this._veinColor = veinColor.ToVector4();
			this._laserColor = laserColor.ToVector4();
			this._mouthColor = mouthColor.ToVector4();
			this._flameColor = flameColor.ToVector4();
			this._backgroundColor = backgroundColor.ToVector4();
		}

		// Token: 0x06001C9E RID: 7326 RVA: 0x004CC0C0 File Offset: 0x004CA2C0
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
				Vector4 vector = Vector4.Lerp(this._veinColor, this._eyeColor, (float)Math.Sin((double)(time + canvasPositionOfIndex.X * 4f)) * 0.5f + 0.5f);
				float num = NoiseHelper.GetDynamicNoise(gridPositionOfIndex.X, gridPositionOfIndex.Y, time / 25f);
				num = Math.Max(0f, 1f - num * 5f);
				vector = Vector4.Lerp(vector, TwinsShader._irisColors[((gridPositionOfIndex.Y * 47 + gridPositionOfIndex.X) % TwinsShader._irisColors.Length + TwinsShader._irisColors.Length) % TwinsShader._irisColors.Length], num);
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x06001C9F RID: 7327 RVA: 0x004CC1A0 File Offset: 0x004CA3A0
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
			bool flag = true;
			float num = time * 0.1f % 2f;
			if (num > 1f)
			{
				num = 2f - num;
				flag = false;
			}
			Vector2 vector = new Vector2(num * 7f - 3.5f, 0f) + fragment.CanvasCenter;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Point gridPositionOfIndex = fragment.GetGridPositionOfIndex(i);
				Vector4 vector2 = this._backgroundColor;
				Vector2 vector3 = canvasPositionOfIndex - vector;
				float num2 = vector3.Length();
				if (num2 < 0.5f)
				{
					float amount = 1f - MathHelper.Clamp((num2 - 0.5f + 0.2f) / 0.2f, 0f, 1f);
					float num3 = MathHelper.Clamp((vector3.X + 0.5f - 0.2f) / 0.6f, 0f, 1f);
					if (flag)
					{
						num3 = 1f - num3;
					}
					Vector4 value = Vector4.Lerp(this._eyeColor, this._veinColor, num3);
					float value2 = (float)Math.Atan2((double)vector3.Y, (double)vector3.X);
					if (!flag && 3.14159274f - Math.Abs(value2) < 0.6f)
					{
						value = this._mouthColor;
					}
					vector2 = Vector4.Lerp(vector2, value, amount);
				}
				if (flag && gridPositionOfIndex.Y == 3 && canvasPositionOfIndex.X > vector.X)
				{
					float value3 = 1f - Math.Abs(canvasPositionOfIndex.X - vector.X * 2f - 0.5f) / 0.5f;
					vector2 = Vector4.Lerp(vector2, this._laserColor, MathHelper.Clamp(value3, 0f, 1f));
				}
				else if (!flag)
				{
					Vector2 vector4 = canvasPositionOfIndex - (vector - new Vector2(1.2f, 0f));
					vector4.Y *= 3.5f;
					float num4 = vector4.Length();
					if (num4 < 0.7f)
					{
						float num5 = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex, time);
						num5 = num5 * num5 * num5;
						num5 *= 1f - MathHelper.Clamp((num4 - 0.7f + 0.3f) / 0.3f, 0f, 1f);
						vector2 = Vector4.Lerp(vector2, this._flameColor, num5);
					}
				}
				fragment.SetColor(i, vector2);
			}
		}

		// Token: 0x04004333 RID: 17203
		private readonly Vector4 _eyeColor;

		// Token: 0x04004334 RID: 17204
		private readonly Vector4 _veinColor;

		// Token: 0x04004335 RID: 17205
		private readonly Vector4 _laserColor;

		// Token: 0x04004336 RID: 17206
		private readonly Vector4 _mouthColor;

		// Token: 0x04004337 RID: 17207
		private readonly Vector4 _flameColor;

		// Token: 0x04004338 RID: 17208
		private readonly Vector4 _backgroundColor;

		// Token: 0x04004339 RID: 17209
		private static readonly Vector4[] _irisColors = new Vector4[]
		{
			Color.Green.ToVector4(),
			Color.Blue.ToVector4()
		};
	}
}
