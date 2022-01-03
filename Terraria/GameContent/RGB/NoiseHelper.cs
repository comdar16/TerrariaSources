using System;
using Microsoft.Xna.Framework;
using Terraria.Utilities;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000256 RID: 598
	public static class NoiseHelper
	{
		// Token: 0x06001CEA RID: 7402 RVA: 0x004CE53C File Offset: 0x004CC73C
		private static float[] CreateStaticNoise(int length)
		{
			UnifiedRandom r = new UnifiedRandom(1);
			float[] array = new float[length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = r.NextFloat();
			}
			return array;
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x004CE570 File Offset: 0x004CC770
		public static float GetDynamicNoise(int index, float currentTime)
		{
			float num = NoiseHelper.StaticNoise[index & 1023];
			float num2 = currentTime % 1f;
			return Math.Abs(Math.Abs(num - num2) - 0.5f) * 2f;
		}

		// Token: 0x06001CEC RID: 7404 RVA: 0x004CE5AA File Offset: 0x004CC7AA
		public static float GetStaticNoise(int index)
		{
			return NoiseHelper.StaticNoise[index & 1023];
		}

		// Token: 0x06001CED RID: 7405 RVA: 0x004CE5B9 File Offset: 0x004CC7B9
		public static float GetDynamicNoise(int x, int y, float currentTime)
		{
			return NoiseHelper.GetDynamicNoiseInternal(x, y, currentTime % 1f);
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x004CE5C9 File Offset: 0x004CC7C9
		private static float GetDynamicNoiseInternal(int x, int y, float wrappedTime)
		{
			x &= 31;
			y &= 31;
			return Math.Abs(Math.Abs(NoiseHelper.StaticNoise[y * 32 + x] - wrappedTime) - 0.5f) * 2f;
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x004CE5FB File Offset: 0x004CC7FB
		public static float GetStaticNoise(int x, int y)
		{
			x &= 31;
			y &= 31;
			return NoiseHelper.StaticNoise[y * 32 + x];
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x004CE618 File Offset: 0x004CC818
		public static float GetDynamicNoise(Vector2 position, float currentTime)
		{
			position *= 10f;
			currentTime %= 1f;
			Vector2 vector = new Vector2((float)Math.Floor((double)position.X), (float)Math.Floor((double)position.Y));
			Point point = new Point((int)vector.X, (int)vector.Y);
			Vector2 vector2 = new Vector2(position.X - vector.X, position.Y - vector.Y);
			float value = MathHelper.Lerp(NoiseHelper.GetDynamicNoiseInternal(point.X, point.Y, currentTime), NoiseHelper.GetDynamicNoiseInternal(point.X, point.Y + 1, currentTime), vector2.Y);
			float value2 = MathHelper.Lerp(NoiseHelper.GetDynamicNoiseInternal(point.X + 1, point.Y, currentTime), NoiseHelper.GetDynamicNoiseInternal(point.X + 1, point.Y + 1, currentTime), vector2.Y);
			return MathHelper.Lerp(value, value2, vector2.X);
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x004CE708 File Offset: 0x004CC908
		public static float GetStaticNoise(Vector2 position)
		{
			position *= 10f;
			Vector2 vector = new Vector2((float)Math.Floor((double)position.X), (float)Math.Floor((double)position.Y));
			Point point = new Point((int)vector.X, (int)vector.Y);
			Vector2 vector2 = new Vector2(position.X - vector.X, position.Y - vector.Y);
			float value = MathHelper.Lerp(NoiseHelper.GetStaticNoise(point.X, point.Y), NoiseHelper.GetStaticNoise(point.X, point.Y + 1), vector2.Y);
			float value2 = MathHelper.Lerp(NoiseHelper.GetStaticNoise(point.X + 1, point.Y), NoiseHelper.GetStaticNoise(point.X + 1, point.Y + 1), vector2.Y);
			return MathHelper.Lerp(value, value2, vector2.X);
		}

		// Token: 0x04004379 RID: 17273
		private const int RANDOM_SEED = 1;

		// Token: 0x0400437A RID: 17274
		private const int NOISE_2D_SIZE = 32;

		// Token: 0x0400437B RID: 17275
		private const int NOISE_2D_SIZE_MASK = 31;

		// Token: 0x0400437C RID: 17276
		private const int NOISE_SIZE_MASK = 1023;

		// Token: 0x0400437D RID: 17277
		private static readonly float[] StaticNoise = NoiseHelper.CreateStaticNoise(1024);
	}
}
