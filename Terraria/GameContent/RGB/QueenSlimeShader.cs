using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200023C RID: 572
	public class QueenSlimeShader : ChromaShader
	{
		// Token: 0x06001C97 RID: 7319 RVA: 0x004CBD34 File Offset: 0x004C9F34
		public QueenSlimeShader(Color slimeColor, Color debrisColor)
		{
			this._slimeColor = slimeColor.ToVector4();
			this._debrisColor = debrisColor.ToVector4();
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x004CBD58 File Offset: 0x004C9F58
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				float num = NoiseHelper.GetDynamicNoise(fragment.GetCanvasPositionOfIndex(i), time * 0.25f);
				num = Math.Max(0f, 1f - num * 2f);
				Vector4 color = Vector4.Lerp(this._slimeColor, this._debrisColor, num);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001C99 RID: 7321 RVA: 0x004CBDC0 File Offset: 0x004C9FC0
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			new Vector2(1.6f, 0.5f);
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector4 vector = this._slimeColor;
				float num = NoiseHelper.GetStaticNoise(canvasPositionOfIndex * 0.3f + new Vector2(0f, time * 0.1f));
				num = Math.Max(0f, 1f - num * 3f);
				num = (float)Math.Sqrt((double)num);
				vector = Vector4.Lerp(vector, this._debrisColor, num);
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x0400432D RID: 17197
		private readonly Vector4 _slimeColor;

		// Token: 0x0400432E RID: 17198
		private readonly Vector4 _debrisColor;
	}
}
