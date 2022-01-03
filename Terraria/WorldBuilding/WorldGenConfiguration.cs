using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria.IO;

namespace Terraria.WorldBuilding
{
	public class WorldGenConfiguration : GameConfiguration
	{
		// Token: 0x06000F8F RID: 3983 RVA: 0x0044E9AC File Offset: 0x0044CBAC
		public WorldGenConfiguration(JObject configurationRoot) : base(configurationRoot)
		{
			this._biomeRoot = (((JObject)configurationRoot["Biomes"]) ?? new JObject());
			this._passRoot = (((JObject)configurationRoot["Passes"]) ?? new JObject());
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x0044E9FE File Offset: 0x0044CBFE
		public T CreateBiome<T>() where T : MicroBiome, new()
		{
			return this.CreateBiome<T>(typeof(T).Name);
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x0044EA18 File Offset: 0x0044CC18
		public T CreateBiome<T>(string name) where T : MicroBiome, new()
		{
			JToken jtoken;
			if (this._biomeRoot.TryGetValue(name, out jtoken))
			{
				return jtoken.ToObject<T>();
			}
			return Activator.CreateInstance<T>();
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x0044EA44 File Offset: 0x0044CC44
		public GameConfiguration GetPassConfiguration(string name)
		{
			JToken jtoken;
			if (this._passRoot.TryGetValue(name, out jtoken))
			{
				return new GameConfiguration((JObject)jtoken);
			}
			return new GameConfiguration(new JObject());
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x0044EA78 File Offset: 0x0044CC78
		public static WorldGenConfiguration FromEmbeddedPath(string path)
		{
			WorldGenConfiguration result;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
			{
				using (StreamReader streamReader = new StreamReader(manifestResourceStream))
				{
					result = new WorldGenConfiguration(JsonConvert.DeserializeObject<JObject>(streamReader.ReadToEnd()));
				}
			}
			return result;
		}

		// Token: 0x04000EB1 RID: 3761
		private readonly JObject _biomeRoot;

		// Token: 0x04000EB2 RID: 3762
		private readonly JObject _passRoot;
	}
}
