using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x0200024F RID: 591
	public class MoonShader : ChromaShader
	{
		// Token: 0x06001CD3 RID: 7379 RVA: 0x004CDBBC File Offset: 0x004CBDBC
		public MoonShader(Color skyColor, Color moonRingColor, Color moonCoreColor) : this(skyColor, moonRingColor, moonCoreColor, Color.White)
		{
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x004CDBCC File Offset: 0x004CBDCC
		public MoonShader(Color skyColor, Color moonColor) : this(skyColor, moonColor, moonColor)
		{
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x004CDBD7 File Offset: 0x004CBDD7
		public MoonShader(Color skyColor, Color moonRingColor, Color moonCoreColor, Color cloudColor)
		{
			this._skyColor = skyColor.ToVector4();
			this._moonRingColor = moonRingColor.ToVector4();
			this._moonCoreColor = moonCoreColor.ToVector4();
			this._cloudColor = cloudColor.ToVector4();
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x004CDC13 File Offset: 0x004CBE13
		public override void Update(float elapsedTime)
		{
			if (Main.dayTime)
			{
				this._progress = (float)(Main.time / 54000.0);
				return;
			}
			this._progress = (float)(Main.time / 32400.0);
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x004CDC4C File Offset: 0x004CBE4C
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.Low
		})]
		private void ProcessLowDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			for (int i = 0; i < fragment.Count; i++)
			{
				float num = NoiseHelper.GetDynamicNoise(fragment.GetCanvasPositionOfIndex(i) * new Vector2(0.1f, 0.5f) + new Vector2(time * 0.02f, 0f), time / 40f);
				num = (float)Math.Sqrt((double)Math.Max(0f, 1f - 2f * num));
				Vector4 color = Vector4.Lerp(this._skyColor, this._cloudColor, num * 0.1f);
				fragment.SetColor(i, color);
			}
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x004CDCF0 File Offset: 0x004CBEF0
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
			Vector2 value = new Vector2(2f, 0.5f);
			Vector2 value2 = new Vector2(2.5f, 1f);
			float num = this._progress * 3.14159274f + 3.14159274f;
			Vector2 value3 = new Vector2((float)Math.Cos((double)num), (float)Math.Sin((double)num)) * value2 + value;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				float num2 = NoiseHelper.GetDynamicNoise(canvasPositionOfIndex * new Vector2(0.1f, 0.5f) + new Vector2(time * 0.02f, 0f), time / 40f);
				num2 = (float)Math.Sqrt((double)Math.Max(0f, 1f - 2f * num2));
				float num3 = (canvasPositionOfIndex - value3).Length();
				Vector4 vector = Vector4.Lerp(this._skyColor, this._cloudColor, num2 * 0.15f);
				if (num3 < 0.8f)
				{
					vector = Vector4.Lerp(this._moonRingColor, this._moonCoreColor, Math.Min(0.1f, 0.8f - num3) / 0.1f);
				}
				else if (num3 < 1f)
				{
					vector = Vector4.Lerp(vector, this._moonRingColor, Math.Min(0.2f, 1f - num3) / 0.2f);
				}
				fragment.SetColor(i, vector);
			}
		}

		// Token: 0x04004369 RID: 17257
		private readonly Vector4 _moonCoreColor;

		// Token: 0x0400436A RID: 17258
		private readonly Vector4 _moonRingColor;

		// Token: 0x0400436B RID: 17259
		private readonly Vector4 _skyColor;

		// Token: 0x0400436C RID: 17260
		private readonly Vector4 _cloudColor;

		// Token: 0x0400436D RID: 17261
		private float _progress;
	}
}
