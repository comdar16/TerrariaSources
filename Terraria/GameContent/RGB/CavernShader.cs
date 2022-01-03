using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000243 RID: 579
	public class CavernShader : ChromaShader
	{
		// Token: 0x06001CAC RID: 7340 RVA: 0x004CC891 File Offset: 0x004CAA91
		public CavernShader(Color backColor, Color frontColor, float speed)
		{
			this._backColor = backColor.ToVector4();
			this._frontColor = frontColor.ToVector4();
			this._speed = speed;
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x004CC8BC File Offset: 0x004CAABC
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 color = Vector4.Lerp(this._backColor, this._frontColor, (float)Math.Sin((double)(time * this._speed + canvasPositionOfIndex.X)) * 0.5f + 0.5f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x004CC920 File Offset: 0x004CAB20
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			time *= this._speed * 0.5f;
			float num = time % 1f;
			bool flag = time % 2f > 1f;
			Vector4 vector = flag ? this._frontColor : this._backColor;
			Vector4 value = flag ? this._backColor : this._frontColor;
			num *= 1.2f;
			for (int i = 0; i < fragment.Count; i++)
			{
				float num2 = NoiseHelper.GetStaticNoise(fragment.GetCanvasPositionOfIndex(i) * 0.5f + new Vector2(0f, time * 0.5f));
				Vector4 vector2 = vector;
				num2 += num;
				if (num2 > 0.999f)
				{
					float amount = MathHelper.Clamp((num2 - 0.999f) / 0.2f, 0f, 1f);
					vector2 = Vector4.Lerp(vector2, value, amount);
				}
				fragment.SetColor(i, vector2);
			}
		}

		// Token: 0x04004344 RID: 17220
		private readonly Vector4 _backColor;

		// Token: 0x04004345 RID: 17221
		private readonly Vector4 _frontColor;

		// Token: 0x04004346 RID: 17222
		private readonly float _speed;
	}
}
