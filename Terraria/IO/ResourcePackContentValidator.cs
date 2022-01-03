using System.IO;
using ReLogic.Content;
using Terraria.GameContent;

namespace Terraria.IO
{
	public class ResourcePackContentValidator
	{
		// Token: 0x060014BF RID: 5311 RVA: 0x00473188 File Offset: 0x00471388
		public void ValidateResourePack(ResourcePack pack)
		{
			if ((AssetReaderCollection)Main.instance.Services.GetService(typeof(AssetReaderCollection)) == null)
			{
				return;
			}
			pack.GetContentSource().GetAllAssetsStartingWith("Images" + Path.DirectorySeparatorChar.ToString());
			VanillaContentValidator.Instance.GetValidImageFilePaths();
		}
	}
}
