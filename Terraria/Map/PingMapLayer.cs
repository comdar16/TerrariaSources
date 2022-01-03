using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI;

namespace Terraria.Map
{
	public class PingMapLayer : IMapLayer
	{
		// Token: 0x06001493 RID: 5267 RVA: 0x00469814 File Offset: 0x00467A14
		public void Draw(ref MapOverlayDrawContext context, ref string text)
		{
			SpriteFrame spriteFrame = new SpriteFrame(1, 5);
			DateTime now = DateTime.Now;
			foreach (SlotVector<PingMapLayer.Ping>.ItemPair itemPair in ((IEnumerable<SlotVector<PingMapLayer.Ping>.ItemPair>)this._pings))
			{
				PingMapLayer.Ping value = itemPair.Value;
				double totalSeconds = (now - value.Time).TotalSeconds;
				int num = (int)(totalSeconds * 10.0);
				spriteFrame.CurrentRow = (byte)(num % (int)spriteFrame.RowCount);
				context.Draw(TextureAssets.MapPing.Value, value.Position, spriteFrame, Alignment.Center);
				if (totalSeconds > 15.0)
				{
					this._pings.Remove(itemPair.Id);
				}
			}
		}

		// Token: 0x06001494 RID: 5268 RVA: 0x004698E4 File Offset: 0x00467AE4
		public void Add(Vector2 position)
		{
			if (this._pings.Count == this._pings.Capacity)
			{
				return;
			}
			this._pings.Add(new PingMapLayer.Ping(position));
		}

		// Token: 0x040011D7 RID: 4567
		private const double PING_DURATION_IN_SECONDS = 15.0;

		// Token: 0x040011D8 RID: 4568
		private const double PING_FRAME_RATE = 10.0;

		// Token: 0x040011D9 RID: 4569
		private readonly SlotVector<PingMapLayer.Ping> _pings = new SlotVector<PingMapLayer.Ping>(100);

		// Token: 0x02000512 RID: 1298
		private struct Ping
		{
			// Token: 0x06002D83 RID: 11651 RVA: 0x0058D0AE File Offset: 0x0058B2AE
			public Ping(Vector2 position)
			{
				this.Position = position;
				this.Time = DateTime.Now;
			}

			// Token: 0x040052E1 RID: 21217
			public readonly Vector2 Position;

			// Token: 0x040052E2 RID: 21218
			public readonly DateTime Time;
		}
	}
}
