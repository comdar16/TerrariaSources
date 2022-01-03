using System;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000245 RID: 581
	internal class DebugKeyboard : RgbDevice
	{
		// Token: 0x06001CB4 RID: 7348 RVA: 0x004CCC2E File Offset: 0x004CAE2E
		private DebugKeyboard(Fragment fragment) : base(RgbDeviceVendor.Virtual, RgbDeviceType.Virtual, fragment, new DeviceColorProfile())
		{
		}

		// Token: 0x06001CB5 RID: 7349 RVA: 0x004CCC40 File Offset: 0x004CAE40
		public static DebugKeyboard Create()
		{
			int num = 400;
			int num2 = 100;
			Point[] array = new Point[num * num2];
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					array[i * num + j] = new Point(j / 10, i / 10);
				}
			}
			Vector2[] array2 = new Vector2[num * num2];
			for (int k = 0; k < num2; k++)
			{
				for (int l = 0; l < num; l++)
				{
					array2[k * num + l] = new Vector2((float)l / (float)num2, (float)k / (float)num2);
				}
			}
			return new DebugKeyboard(Fragment.FromCustom(array, array2));
		}

		// Token: 0x06001CB6 RID: 7350 RVA: 0x000392B0 File Offset: 0x000374B0
		public override void Present()
		{
		}

		// Token: 0x06001CB7 RID: 7351 RVA: 0x004CCCF0 File Offset: 0x004CAEF0
		public override void DebugDraw(IDebugDrawer drawer, Vector2 position, float scale)
		{
			for (int i = 0; i < base.LedCount; i++)
			{
				Vector2 ledCanvasPosition = base.GetLedCanvasPosition(i);
				drawer.DrawSquare(new Vector4(ledCanvasPosition * scale + position, scale / 100f, scale / 100f), new Color(base.GetUnprocessedLedColor(i)));
			}
		}
	}
}
