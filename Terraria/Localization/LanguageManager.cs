using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using CsvHelper;
using Newtonsoft.Json;
using ReLogic.Content;
using ReLogic.Content.Sources;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.Initializers;
using Terraria.Utilities;
namespace Terraria.Localization
{
	public class LanguageManager
	{
		public static LanguageManager Instance = new LanguageManager();

		private readonly Dictionary<string, LocalizedText> _localizedTexts = new Dictionary<string, LocalizedText>();

		private readonly Dictionary<string, List<string>> _categoryGroupedKeys = new Dictionary<string, List<string>>();

		private GameCulture _fallbackCulture = GameCulture.DefaultCulture;

		public GameCulture ActiveCulture { get; private set; }

		public event LanguageChangeCallback OnLanguageChanging;

		public event LanguageChangeCallback OnLanguageChanged;

		private LanguageManager()
		{
			_localizedTexts[""] = LocalizedText.Empty;
		}

		public int GetCategorySize(string name)
		{
			if (_categoryGroupedKeys.ContainsKey(name))
			{
				return _categoryGroupedKeys[name].Count;
			}
			return 0;
		}

		public void SetLanguage(int legacyId)
		{
			GameCulture language = GameCulture.FromLegacyId(legacyId);
			SetLanguage(language);
		}

		public void SetLanguage(string cultureName)
		{
			GameCulture language = GameCulture.FromName(cultureName);
			SetLanguage(language);
		}

		public int EstimateWordCount()
		{
			int num = 0;
			foreach (string key in _localizedTexts.Keys)
			{
				string textValue = GetTextValue(key);
				textValue.Replace(",", "").Replace(".", "").Replace("\"", "")
					.Trim();
				string[] array = textValue.Split(' ');
				string[] array2 = textValue.Split(' ');
				if (array.Length == array2.Length)
				{
					string[] array3 = array;
					foreach (string text in array3)
					{
						if (!string.IsNullOrWhiteSpace(text) && text.Length >= 1)
						{
							num++;
						}
					}
					continue;
				}
				return num;
			}
			return num;
		}

		private void SetAllTextValuesToKeys()
		{
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				localizedText.Value.SetValue(localizedText.Key);
			}
		}
		
		private string[] GetLanguageFilesForCulture(GameCulture culture)
		{
			Assembly.GetExecutingAssembly();
			//string[] aboba = new string[7];
			string[] abc = Array.FindAll(typeof(Program).Assembly.GetManifestResourceNames(), (string element) => element.StartsWith("Terraria.Localization.Content." + culture.CultureInfo.Name) && element.EndsWith(".json"));
			//abc[abc.Length] = "Terraria.Localization.Content.ru-RU.json";
			//aboba = abc;
			//string []abc1 = abc;
			//if (culture.CultureInfo.Name == "ru-RU")
			//{
			//	Array.Resize(ref aboba, aboba.Length + 1);
			//	aboba[aboba.Length - 1]  =  "Terraria.Localization.Content.ru-RU.json";
			//	return aboba;
			//}
			//return Array.FindAll(typeof(Program).Assembly.GetManifestResourceNames(), (string element) => element.StartsWith("Terraria.Localization.Content." + culture.CultureInfo.Name) && element.EndsWith(".json"));

			return abc;
		}

		public void SetLanguage(GameCulture culture)
		{
			if (ActiveCulture != culture)
			{
				if (culture != _fallbackCulture && ActiveCulture != _fallbackCulture)
				{
					SetAllTextValuesToKeys();
					LoadLanguage(_fallbackCulture);
				}
				LoadLanguage(culture);
				ActiveCulture = culture;
				Thread.CurrentThread.CurrentCulture = culture.CultureInfo;
				Thread.CurrentThread.CurrentUICulture = culture.CultureInfo;
				if (this.OnLanguageChanged != null)
				{
					this.OnLanguageChanged(this);
				}
				_ = FontAssets.DeathText;
			}
		}

		private void LoadLanguage(GameCulture culture, bool processCopyCommands = true)
		{
			LoadFilesForCulture(culture);
			if (this.OnLanguageChanging != null)
			{
				this.OnLanguageChanging(this);
			}
			if (processCopyCommands)
			{
				ProcessCopyCommandsInTexts();
			}
			ChatInitializer.PrepareAliases();
		}

		private void LoadFilesForCulture(GameCulture culture)
		{
			string[] languageFilesForCulture = GetLanguageFilesForCulture(culture);
			foreach (string text in languageFilesForCulture)
			{
				try
				{
					string text2 = Utils.ReadEmbeddedResource(text);
					if (text2 == null || text2.Length < 2)
					{
						throw new FormatException();
					}
					LoadLanguageFromFileTextJson(text2, canCreateCategories: true);
				}
				catch (Exception)
				{
					if (Debugger.IsAttached)
					{
						Debugger.Break();
					}
					Console.WriteLine("Failed to load language file: " + text);
					return;
				}
			}
		}

		private void ProcessCopyCommandsInTexts()
		{
			Regex regex = new Regex("{\\$(\\w+\\.\\w+)}", RegexOptions.Compiled);
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				LocalizedText value = localizedText.Value;
				for (int i = 0; i < 100; i++)
				{
					string text = regex.Replace(value.Value, (Match match) => GetTextValue(match.Groups[1].ToString()));
					if (text == value.Value)
					{
						break;
					}
					value.SetValue(text);
				}
			}
		}

		public void UseSources(List<IContentSource> sourcesFromLowestToHighest)
		{
			string name = ActiveCulture.Name;
			string text = ("Localization" + Path.DirectorySeparatorChar + name).ToLower();
			LoadLanguage(ActiveCulture, processCopyCommands: false);
			foreach (IContentSource item in sourcesFromLowestToHighest)
			{
				foreach (string item2 in item.GetAllAssetsStartingWith(text))
				{
					string extension = item.GetExtension(item2);
					if (!(extension == ".json") && !(extension == ".csv"))
					{
						continue;
					}
					using (Stream stream = item.OpenStream(item2))
					{
						using (StreamReader streamReader = new StreamReader(stream))
						{
							string fileText = streamReader.ReadToEnd();
							if (extension == ".json")
							{
								LoadLanguageFromFileTextJson(fileText, canCreateCategories: false);
							}
							if (extension == ".csv")
							{
								LoadLanguageFromFileTextCsv(fileText);
							}
						}
					}
				}
			}
			ProcessCopyCommandsInTexts();
			ChatInitializer.PrepareAliases();
		}

		public void LoadLanguageFromFileTextCsv(string fileText)
		{
			using (TextReader textReader = new StringReader(fileText))
			{
				using (CsvReader csvReader = new CsvReader(textReader))
				{
					csvReader.Configuration.HasHeaderRecord = true;
					if (csvReader.ReadHeader())
					{
						string[] fieldHeaders = csvReader.FieldHeaders;
						int num = -1;
						int num2 = -1;
						for (int i = 0; i < fieldHeaders.Length; i++)
						{
							string a = fieldHeaders[i].ToLower();
							if (a == "translation")
							{
								num2 = i;
							}
							if (a == "key")
							{
								num = i;
							}
						}
						if (num != -1 && num2 != -1)
						{
							int num3 = Math.Max(num, num2) + 1;
							while (csvReader.Read())
							{
								string[] currentRecord = csvReader.CurrentRecord;
								if (currentRecord.Length >= num3)
								{
									string text = currentRecord[num];
									string value = currentRecord[num2];
									if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(value) && this._localizedTexts.ContainsKey(text))
									{
										this._localizedTexts[text].SetValue(value);
									}
								}
							}
						}
					}
				}
			}
		}

		public void LoadLanguageFromFileTextJson(string fileText, bool canCreateCategories)
		{
			foreach (KeyValuePair<string, Dictionary<string, string>> item in JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(fileText))
			{
				_ = item.Key;
				foreach (KeyValuePair<string, string> item2 in item.Value)
				{
					string key = item.Key + "." + item2.Key;
					if (_localizedTexts.ContainsKey(key))
					{
						_localizedTexts[key].SetValue(item2.Value);
					}
					else if (canCreateCategories)
					{
						_localizedTexts.Add(key, new LocalizedText(key, item2.Value));
						if (!_categoryGroupedKeys.ContainsKey(item.Key))
						{
							_categoryGroupedKeys.Add(item.Key, new List<string>());
						}
						_categoryGroupedKeys[item.Key].Add(item2.Key);
					}
				}
			}
		}

		[Conditional("DEBUG")]
		private void ValidateAllCharactersContainedInFont(DynamicSpriteFont font)
		{
			if (font == null)
			{
				return;
			}
			string text = "";
			foreach (LocalizedText value2 in _localizedTexts.Values)
			{
				string value = value2.Value;
				for (int i = 0; i < value.Length; i++)
				{
					char c = value[i];
					if (!font.IsCharacterSupported(c))
					{
						string[] obj = new string[7]
						{
							text,
							value2.Key,
							", ",
							c.ToString(),
							", ",
							null,
							null
						};
						int num = c;
						obj[5] = num.ToString();
						obj[6] = "\n";
						text = string.Concat(obj);
					}
				}
			}
		}

		public LocalizedText[] FindAll(Regex regex)
		{
			int num = 0;
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				if (regex.IsMatch(localizedText.Key))
				{
					num++;
				}
			}
			LocalizedText[] array = new LocalizedText[num];
			int num2 = 0;
			foreach (KeyValuePair<string, LocalizedText> localizedText2 in _localizedTexts)
			{
				if (regex.IsMatch(localizedText2.Key))
				{
					array[num2] = localizedText2.Value;
					num2++;
				}
			}
			return array;
		}

		public LocalizedText[] FindAll(LanguageSearchFilter filter)
		{
			LinkedList<LocalizedText> linkedList = new LinkedList<LocalizedText>();
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				if (filter(localizedText.Key, localizedText.Value))
				{
					linkedList.AddLast(localizedText.Value);
				}
			}
			return linkedList.ToArray();
		}

		public LocalizedText SelectRandom(LanguageSearchFilter filter, UnifiedRandom random = null)
		{
			int num = 0;
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				if (filter(localizedText.Key, localizedText.Value))
				{
					num++;
				}
			}
			int num2 = (random ?? Main.rand).Next(num);
			foreach (KeyValuePair<string, LocalizedText> localizedText2 in _localizedTexts)
			{
				if (filter(localizedText2.Key, localizedText2.Value) && --num == num2)
				{
					return localizedText2.Value;
				}
			}
			return LocalizedText.Empty;
		}

		public LocalizedText RandomFromCategory(string categoryName, UnifiedRandom random = null)
		{
			if (!_categoryGroupedKeys.ContainsKey(categoryName))
			{
				return new LocalizedText(categoryName + ".RANDOM", categoryName + ".RANDOM");
			}
			List<string> list = _categoryGroupedKeys[categoryName];
			return GetText(categoryName + "." + list[(random ?? Main.rand).Next(list.Count)]);
		}

		public LocalizedText IndexedFromCategory(string categoryName, int index)
		{
			if (!_categoryGroupedKeys.ContainsKey(categoryName))
			{
				return new LocalizedText(categoryName + ".INDEXED", categoryName + ".INDEXED");
			}
			List<string> list = _categoryGroupedKeys[categoryName];
			int index2 = index % list.Count;
			return GetText(categoryName + "." + list[index2]);
		}

		public bool Exists(string key)
		{
			return _localizedTexts.ContainsKey(key);
		}

		public LocalizedText GetText(string key)
		{
			if (!_localizedTexts.ContainsKey(key))
			{
				return new LocalizedText(key, key);
			}
			return _localizedTexts[key];
		}

		public string GetTextValue(string key)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Value;
			}
			return key;
		}

		public string GetTextValue(string key, object arg0)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Format(arg0);
			}
			return key;
		}

		public string GetTextValue(string key, object arg0, object arg1)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Format(arg0, arg1);
			}
			return key;
		}

		public string GetTextValue(string key, object arg0, object arg1, object arg2)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Format(arg0, arg1, arg2);
			}
			return key;
		}

		public string GetTextValue(string key, params object[] args)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Format(args);
			}
			return key;
		}

		public void SetFallbackCulture(GameCulture culture)
		{
			_fallbackCulture = culture;
		}
	}
}
