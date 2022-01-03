using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using ReLogic.Content.Sources;
using ReLogic.Utilities;
using Terraria.GameContent;

namespace Terraria.IO
{
	public class ResourcePack
	{
		public enum BrandingType
		{
			None,
			SteamWorkshop
		}

		public readonly string FullPath;

		public readonly string FileName;

		private readonly IServiceProvider _services;

		public readonly bool IsCompressed;

		public readonly BrandingType Branding;

		private readonly ZipFile _zipFile;

		private Texture2D _icon;

		private IContentSource _contentSource;

		private bool _needsReload = true;

		private const string ICON_FILE_NAME = "icon.png";

		private const string PACK_FILE_NAME = "pack.json";

		public Texture2D Icon
		{
			get
			{
				if (_icon == null)
				{
					_icon = CreateIcon();
				}
				return _icon;
			}
		}

		public string Name { get; private set; }

		public string Description { get; private set; }

		public string Author { get; private set; }

		public ResourcePackVersion Version { get; private set; }

		public bool IsEnabled { get; set; }

		public int SortingOrder { get; set; }

		public ResourcePack(IServiceProvider services, string path, BrandingType branding = BrandingType.None)
		{
			if (File.Exists(path))
			{
				IsCompressed = true;
			}
			else if (!Directory.Exists(path))
			{
				throw new FileNotFoundException("Unable to find file or folder for resource pack at: " + path);
			}
			FileName = Path.GetFileName(path);
			_services = services;
			FullPath = path;
			Branding = branding;
			if (IsCompressed)
			{
				_zipFile = ZipFile.Read(path);
			}
			LoadManifest();
		}

		public void Refresh()
		{
			_needsReload = true;
		}

		public IContentSource GetContentSource()
		{
			if (this._needsReload)
			{
				if (this.IsCompressed)
				{
					this._contentSource = new ZipContentSource(this.FullPath, "Content");
				}
				else
				{
					this._contentSource = new FileSystemContentSource(Path.Combine(this.FullPath, "Content"));
				}
				this._contentSource.ContentValidator = VanillaContentValidator.Instance;
				this._needsReload = false;
			}
			return this._contentSource;
		}

		private Texture2D CreateIcon()
		{
			if (!HasFile("icon.png"))
			{
				return XnaExtensions.Get<IAssetRepository>(_services).Request<Texture2D>("Images/UI/DefaultResourcePackIcon", (AssetRequestMode)1).Value;
			}
			using (Stream stream = OpenStream("icon.png"))
			{
				return XnaExtensions.Get<AssetReaderCollection>(_services).Read<Texture2D>(stream, ".png");
			}
		}

		private void LoadManifest()
		{
			if (!this.HasFile("pack.json"))
			{
				throw new FileNotFoundException(string.Format("Resource Pack at \"{0}\" must contain a {1} file.", this.FullPath, "pack.json"));
			}
			JObject jobject;
			using (Stream stream = this.OpenStream("pack.json"))
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					jobject = JObject.Parse(streamReader.ReadToEnd());
				}
			}
			this.Name = jobject["Name"].Value<string>();
			this.Description = jobject["Description"].Value<string>();
			this.Author = jobject["Author"].Value<string>();
			this.Version = jobject["Version"].ToObject<ResourcePackVersion>();
		}

		private Stream OpenStream(string fileName)
		{
			if (!this.IsCompressed)
			{
				return File.OpenRead(Path.Combine(this.FullPath, fileName));
			}
			ZipEntry zipEntry = this._zipFile[fileName];
			MemoryStream memoryStream = new MemoryStream((int)zipEntry.UncompressedSize);
			zipEntry.Extract(memoryStream);
			memoryStream.Position = 0L;
			return memoryStream;
		}

		private bool HasFile(string fileName)
		{
			if (!IsCompressed)
			{
				return File.Exists(Path.Combine(FullPath, fileName));
			}
			return _zipFile.ContainsEntry(fileName);
		}
	}
}
