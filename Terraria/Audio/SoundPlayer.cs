using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;

namespace Terraria.Audio
{
	public class SoundPlayer
	{
		private readonly SlotVector<ActiveSound> _trackedSounds = new SlotVector<ActiveSound>(4096);

		public SlotId Play(SoundStyle style, Vector2 position)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (Main.dedServ || style == null || !style.IsTrackable)
			{
				return SlotId.Invalid;
			}
			if (Vector2.DistanceSquared(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), position) > 1E+08f)
			{
				return SlotId.Invalid;
			}
			ActiveSound activeSound = new ActiveSound(style, position);
			return _trackedSounds.Add(activeSound);
		}

		public void Reload()
		{
			StopAll();
		}

		public SlotId Play(SoundStyle style)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (Main.dedServ || style == null || !style.IsTrackable)
			{
				return SlotId.Invalid;
			}
			ActiveSound activeSound = new ActiveSound(style);
			return _trackedSounds.Add(activeSound);
		}

		public ActiveSound GetActiveSound(SlotId id)
		{
			if (!this._trackedSounds.Has(id))
			{
				return null;
			}
			return this._trackedSounds[id];
		}

		public void PauseAll()
		{
			foreach (SlotVector<ActiveSound>.ItemPair itemPair in ((IEnumerable<SlotVector<ActiveSound>.ItemPair>)this._trackedSounds))
			{
				itemPair.Value.Pause();
			}
		}

		public void ResumeAll()
		{
			foreach (SlotVector<ActiveSound>.ItemPair itemPair in ((IEnumerable<SlotVector<ActiveSound>.ItemPair>)this._trackedSounds))
			{
				itemPair.Value.Resume();
			}
		}

		public void StopAll()
		{
			foreach (SlotVector<ActiveSound>.ItemPair itemPair in ((IEnumerable<SlotVector<ActiveSound>.ItemPair>)this._trackedSounds))
			{
				itemPair.Value.Stop();
			}
			this._trackedSounds.Clear();
		}

		public void Update()
		{
			foreach (SlotVector<ActiveSound>.ItemPair itemPair in ((IEnumerable<SlotVector<ActiveSound>.ItemPair>)this._trackedSounds))
			{
				try
				{
					itemPair.Value.Update();
					if (!itemPair.Value.IsPlaying)
					{
						this._trackedSounds.Remove(itemPair.Id);
					}
				}
				catch
				{
					this._trackedSounds.Remove(itemPair.Id);
				}
			}
		}

		public ActiveSound FindActiveSound(SoundStyle style)
		{
			foreach (SlotVector<ActiveSound>.ItemPair itemPair in ((IEnumerable<SlotVector<ActiveSound>.ItemPair>)this._trackedSounds))
			{
				if (itemPair.Value.Style == style)
				{
					return itemPair.Value;
				}
			}
			return null;
		}
	}
}
