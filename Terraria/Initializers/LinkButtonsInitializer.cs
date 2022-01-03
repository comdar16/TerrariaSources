using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;

namespace Terraria.Initializers
{
	public class LinkButtonsInitializer
	{
		public static void Load()
		{
			List<TitleLinkButton> titleLinks = Main.TitleLinks;
			titleLinks.Add(LinkButtonsInitializer.MakeSimpleButton("TitleLinks.Discord", "https://discord.gg/terraria", 0));
			titleLinks.Add(LinkButtonsInitializer.MakeSimpleButton("TitleLinks.Instagram", "https://www.instagram.com/terraria_logic/", 1));
			titleLinks.Add(LinkButtonsInitializer.MakeSimpleButton("TitleLinks.Reddit", "https://www.reddit.com/r/Terraria/", 2));
			titleLinks.Add(LinkButtonsInitializer.MakeSimpleButton("TitleLinks.Twitter", "https://twitter.com/Terraria_Logic", 3));
			titleLinks.Add(LinkButtonsInitializer.MakeSimpleButton("TitleLinks.Forums", "https://forums.terraria.org/index.php", 4));
			titleLinks.Add(LinkButtonsInitializer.MakeSimpleButton("TitleLinks.Merch", "https://terraria.org/store", 5));
		}

		private static TitleLinkButton MakeSimpleButton(string textKey, string linkUrl, int horizontalFrameIndex)
		{
			Asset<Texture2D> asset = Main.Assets.Request<Texture2D>("Images/UI/TitleLinkButtons", AssetRequestMode.ImmediateLoad);
			Rectangle value = asset.Frame(7, 2, horizontalFrameIndex, 0, 0, 0);
			Rectangle value2 = asset.Frame(7, 2, horizontalFrameIndex, 1, 0, 0);
			value.Width--;
			value.Height--;
			value2.Width--;
			value2.Height--;
			return new TitleLinkButton
			{
				TooltipTextKey = textKey,
				LinkUrl = linkUrl,
				FrameWehnSelected = new Rectangle?(value2),
				FrameWhenNotSelected = new Rectangle?(value),
				Image = asset
			};
		}
	}
}
