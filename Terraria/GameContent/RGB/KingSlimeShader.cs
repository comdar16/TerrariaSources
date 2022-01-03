using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000235 RID: 565
	public class KingSlimeShader : ChromaShader
	{
		// Token: 0x06001C82 RID: 7298 RVA: 0x004CB06D File Offset: 0x004C926D
		public KingSlimeShader(Color slimeColor, Color debrisColor)
		{
			this._slimeColor = slimeColor.ToVector4();
			this._debrisColor = debrisColor.ToVector4();
		}

		// Token: 0x06001C83 RID: 7299 RVA: 0x004CB090 File Offset: 0x004C9290
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

		// Token: 0x06001C84 RID: 7300 RVA: 0x004CB0F8 File Offset: 0x004C92F8
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

		// Token: 0x04004319 RID: 17177
		private readonly Vector4 _slimeColor;

		// Token: 0x0400431A RID: 17178
		private readonly Vector4 _debrisColor;
	}
}
