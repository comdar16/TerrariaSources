using Newtonsoft.Json.Linq;

namespace Terraria.IO
{
	public class GameConfiguration
	{
		// Token: 0x06001540 RID: 5440 RVA: 0x0047500F File Offset: 0x0047320F
		public GameConfiguration(JObject configurationRoot)
		{
			this._root = configurationRoot;
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x0047501E File Offset: 0x0047321E
		public T Get<T>(string entry)
		{
			return this._root[entry].ToObject<T>();
		}

		// Token: 0x0400124D RID: 4685
		private readonly JObject _root;
	}
}
