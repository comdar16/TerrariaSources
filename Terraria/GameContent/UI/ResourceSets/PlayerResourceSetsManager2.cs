using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.IO;

namespace Terraria.GameContent.UI.ResourceSets
{
	public class PlayerResourceSetsManager2 : SelectionHolder<IPlayerResourcesDisplaySet>
	{
		protected override void Configuration_Save(Preferences obj)
		{
			obj.Put("PlayerResourcesSet", ActiveSelectionConfigKey);
		}

		protected override void Configuration_OnLoad(Preferences obj)
		{
			ActiveSelectionConfigKey = Main.Configuration.Get("PlayerResourcesSet", "New");
		}

		protected override void PopulateOptionsAndLoadContent(AssetRequestMode mode)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			Options["New"] = new FancyClassicPlayerResourcesDisplaySet("New", "New", "FancyClassic", mode);
			Options["Default"] = new ClassicPlayerResourcesDisplaySet("Default", "Default");
			Options["HorizontalBars"] = new HorizontalBarsPlayerReosurcesDisplaySet("HorizontalBars", "HorizontalBars", "HorizontalBars", mode);
		}

		public void TryToHoverOverResources()
		{
			ActiveSelection.TryToHover();
		}

		public void Draw()
		{
			ActiveSelection.Draw();
		}
	}
}
