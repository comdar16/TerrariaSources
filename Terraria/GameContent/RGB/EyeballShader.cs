using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;
using Terraria.Utilities;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000248 RID: 584
	public class EyeballShader : ChromaShader
	{
		// Token: 0x06001CBC RID: 7356 RVA: 0x004CCF30 File Offset: 0x004CB130
		public EyeballShader(bool isSpawning)
		{
			this._isSpawning = isSpawning;
		}

		// Token: 0x06001CBD RID: 7357 RVA: 0x004CCF84 File Offset: 0x004CB184
		public override void Update(float elapsedTime)
		{
			this.UpdateEyelid(elapsedTime);
			bool flag = this._timeUntilPupilMove <= 0f;
			this._pupilOffset = (this._targetOffset + this._pupilOffset) * 0.5f;
			this._timeUntilPupilMove -= elapsedTime;
			if (flag)
			{
				float num = (float)this._random.NextDouble() * 6.28318548f;
				float scaleFactor;
				if (this._isSpawning)
				{
					this._timeUntilPupilMove = (float)this._random.NextDouble() * 0.4f + 0.3f;
					scaleFactor = (float)this._random.NextDouble() * 0.7f;
				}
				else
				{
					this._timeUntilPupilMove = (float)this._random.NextDouble() * 0.4f + 0.6f;
					scaleFactor = (float)this._random.NextDouble() * 0.3f;
				}
				this._targetOffset = new Vector2((float)Math.Cos((double)num), (float)Math.Sin((double)num)) * scaleFactor;
			}
		}

		// Token: 0x06001CBE RID: 7358 RVA: 0x004CD07C File Offset: 0x004CB27C
		private void UpdateEyelid(float elapsedTime)
		{
			float num = 0.5f;
			float num2 = 6f;
			if (this._isSpawning)
			{
				if (NPC.MoonLordCountdown >= 3590)
				{
					this._eyelidStateTime = 0f;
					this._eyelidState = EyeballShader.EyelidState.Closed;
				}
				num = (float)NPC.MoonLordCountdown / 3600f * 10f + 0.5f;
				num2 = 2f;
			}
			this._eyelidStateTime += elapsedTime;
			switch (this._eyelidState)
			{
			case EyeballShader.EyelidState.Closed:
				this._eyelidProgress = 0f;
				if (this._eyelidStateTime > num)
				{
					this._eyelidStateTime = 0f;
					this._eyelidState = EyeballShader.EyelidState.Opening;
					return;
				}
				break;
			case EyeballShader.EyelidState.Opening:
				this._eyelidProgress = this._eyelidStateTime / 0.4f;
				if (this._eyelidStateTime > 0.4f)
				{
					this._eyelidStateTime = 0f;
					this._eyelidState = EyeballShader.EyelidState.Open;
					return;
				}
				break;
			case EyeballShader.EyelidState.Open:
				this._eyelidProgress = 1f;
				if (this._eyelidStateTime > num2)
				{
					this._eyelidStateTime = 0f;
					this._eyelidState = EyeballShader.EyelidState.Closing;
					return;
				}
				break;
			case EyeballShader.EyelidState.Closing:
				this._eyelidProgress = 1f - this._eyelidStateTime / 0.4f;
				if (this._eyelidStateTime > 0.4f)
				{
					this._eyelidStateTime = 0f;
					this._eyelidState = EyeballShader.EyelidState.Closed;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06001CBF RID: 7359 RVA: 0x004CD1C0 File Offset: 0x004CB3C0
		[RgbProcessor(new EffectDetailLevel[]
		{
			EffectDetailLevel.High
		})]
		private void ProcessHighDetail(RgbDevice device, Fragment fragment, EffectDetailLevel quality, float time)
		{
			Vector2 vector = new Vector2(1.5f, 0.5f);
			Vector2 value = vector + this._pupilOffset;
			for (int i = 0; i < fragment.Count; i++)
			{
				Vector2 canvasPositionOfIndex = fragment.GetCanvasPositionOfIndex(i);
				Vector2 vector2 = canvasPositionOfIndex - vector;
				Vector4 vector3 = Vector4.One;
				float num = (value - canvasPositionOfIndex).Length();
				for (int j = 1; j < EyeballShader.Rings.Length; j++)
				{
					EyeballShader.Ring ring = EyeballShader.Rings[j];
					EyeballShader.Ring ring2 = EyeballShader.Rings[j - 1];
					if (num < ring.Distance)
					{
						vector3 = Vector4.Lerp(ring2.Color, ring.Color, (num - ring2.Distance) / (ring.Distance - ring2.Distance));
						break;
					}
				}
				float num2 = (float)Math.Sqrt((double)(1f - 0.4f * vector2.Y * vector2.Y)) * 5f;
				float num3 = Math.Abs(vector2.X) - num2 * (1.1f * this._eyelidProgress - 0.1f);
				if (num3 > 0f)
				{
					vector3 = Vector4.Lerp(vector3, this._eyelidColor, Math.Min(1f, num3 * 10f));
				}
				fragment.SetColor(i, vector3);
			}
		}

		// Token: 0x0400434F RID: 17231
		private static readonly EyeballShader.Ring[] Rings = new EyeballShader.Ring[]
		{
			new EyeballShader.Ring(Color.Black.ToVector4(), 0f),
			new EyeballShader.Ring(Color.Black.ToVector4(), 0.4f),
			new EyeballShader.Ring(new Color(17, 220, 237).ToVector4(), 0.5f),
			new EyeballShader.Ring(new Color(17, 120, 237).ToVector4(), 0.6f),
			new EyeballShader.Ring(Vector4.One, 0.65f)
		};

		// Token: 0x04004350 RID: 17232
		private readonly Vector4 _eyelidColor = new Color(108, 110, 75).ToVector4();

		// Token: 0x04004351 RID: 17233
		private float _eyelidProgress;

		// Token: 0x04004352 RID: 17234
		private Vector2 _pupilOffset = Vector2.Zero;

		// Token: 0x04004353 RID: 17235
		private Vector2 _targetOffset = Vector2.Zero;

		// Token: 0x04004354 RID: 17236
		private readonly UnifiedRandom _random = new UnifiedRandom();

		// Token: 0x04004355 RID: 17237
		private float _timeUntilPupilMove;

		// Token: 0x04004356 RID: 17238
		private float _eyelidStateTime;

		// Token: 0x04004357 RID: 17239
		private readonly bool _isSpawning;

		// Token: 0x04004358 RID: 17240
		private EyeballShader.EyelidState _eyelidState;

		// Token: 0x020005C8 RID: 1480
		private struct Ring
		{
			// Token: 0x06002FB4 RID: 12212 RVA: 0x005AC051 File Offset: 0x005AA251
			public Ring(Vector4 color, float distance)
			{
				this.Color = color;
				this.Distance = distance;
			}

			// Token: 0x04005A48 RID: 23112
			public readonly Vector4 Color;

			// Token: 0x04005A49 RID: 23113
			public readonly float Distance;
		}

		// Token: 0x020005C9 RID: 1481
		private enum EyelidState
		{
			// Token: 0x04005A4B RID: 23115
			Closed,
			// Token: 0x04005A4C RID: 23116
			Opening,
			// Token: 0x04005A4D RID: 23117
			Open,
			// Token: 0x04005A4E RID: 23118
			Closing
		}
	}
}
