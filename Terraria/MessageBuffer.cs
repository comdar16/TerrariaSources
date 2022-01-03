using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.GameContent.Golf;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Net;
using Terraria.Testing;
using Terraria.UI;

namespace Terraria
{
	public class MessageBuffer
	{
		public const int readBufferMax = 131070;

		public const int writeBufferMax = 131070;

		public bool broadcast;

		public byte[] readBuffer = new byte[131070];

		public byte[] writeBuffer = new byte[131070];

		public bool writeLocked;

		public int messageLength;

		public int totalData;

		public int whoAmI;

		public int spamCount;

		public int maxSpam;

		public bool checkBytes;

		public MemoryStream readerStream;

		public MemoryStream writerStream;

		public BinaryReader reader;

		public BinaryWriter writer;

		public PacketHistory History = new PacketHistory();

		public static event TileChangeReceivedEvent OnTileChangeReceived;

		public void Reset()
		{
			Array.Clear(readBuffer, 0, readBuffer.Length);
			Array.Clear(writeBuffer, 0, writeBuffer.Length);
			writeLocked = false;
			messageLength = 0;
			totalData = 0;
			spamCount = 0;
			broadcast = false;
			checkBytes = false;
			ResetReader();
			ResetWriter();
		}

		public void ResetReader()
		{
			if (readerStream != null)
			{
				readerStream.Close();
			}
			readerStream = new MemoryStream(readBuffer);
			reader = new BinaryReader(readerStream);
		}

		public void ResetWriter()
		{
			if (writerStream != null)
			{
				writerStream.Close();
			}
			writerStream = new MemoryStream(writeBuffer);
			writer = new BinaryWriter(writerStream);
		}

		public void GetData(int start, int length, out int messageType)
		{
			if (whoAmI < 256)
			{
				Netplay.Clients[whoAmI].TimeOutTimer = 0;
			}
			else
			{
				Netplay.Connection.TimeOutTimer = 0;
			}
			byte b = 0;
			int num = 0;
			num = start + 1;
			b = (byte)(messageType = readBuffer[start]);
			if (b >= 143)
			{
				return;
			}
			Main.ActiveNetDiagnosticsUI.CountReadMessage(b, length);
			if (Main.netMode == 1 && Netplay.Connection.StatusMax > 0)
			{
				Netplay.Connection.StatusCount++;
			}
			if (Main.verboseNetplay)
			{
				for (int i = start; i < start + length; i++)
				{
				}
				for (int j = start; j < start + length; j++)
				{
					_ = readBuffer[j];
				}
			}
			if (Main.netMode == 2 && b != 38 && Netplay.Clients[whoAmI].State == -1)
			{
				NetMessage.TrySendData(2, whoAmI, -1, Lang.mp[1].ToNetworkText());
				return;
			}
			if (Main.netMode == 2)
			{
				if (Netplay.Clients[whoAmI].State < 10 && b > 12 && b != 93 && b != 16 && b != 42 && b != 50 && b != 38 && b != 68)
				{
					NetMessage.BootPlayer(whoAmI, Lang.mp[2].ToNetworkText());
				}
				if (Netplay.Clients[whoAmI].State == 0 && b != 1)
				{
					NetMessage.BootPlayer(whoAmI, Lang.mp[2].ToNetworkText());
				}
			}
			if (reader == null)
			{
				ResetReader();
			}
			reader.BaseStream.Position = num;
			NPCSpawnParams spawnparams;
			switch (b)
			{
			case 1:
				if (Main.netMode != 2)
				{
					break;
				}
				if (Main.dedServ && Netplay.IsBanned(Netplay.Clients[whoAmI].Socket.GetRemoteAddress()))
				{
					NetMessage.TrySendData(2, whoAmI, -1, Lang.mp[3].ToNetworkText());
				}
				else
				{
					if (Netplay.Clients[whoAmI].State != 0)
					{
						break;
					}
					if (reader.ReadString() == "Terraria" + 244)
					{
						if (string.IsNullOrEmpty(Netplay.ServerPassword))
						{
							Netplay.Clients[whoAmI].State = 1;
							NetMessage.TrySendData(3, whoAmI);
						}
						else
						{
							Netplay.Clients[whoAmI].State = -1;
							NetMessage.TrySendData(37, whoAmI);
						}
					}
					else
					{
						NetMessage.TrySendData(2, whoAmI, -1, Lang.mp[4].ToNetworkText());
					}
				}
				break;
			case 2:
				if (Main.netMode == 1)
				{
					Netplay.Disconnect = true;
					Main.statusText = NetworkText.Deserialize(reader).ToString();
				}
				break;
			case 3:
				if (Main.netMode == 1)
				{
					if (Netplay.Connection.State == 1)
					{
						Netplay.Connection.State = 2;
					}
					int num237 = reader.ReadByte();
					bool value9 = reader.ReadBoolean();
					Netplay.Connection.ServerSpecialFlags[2] = value9;
					if (num237 != Main.myPlayer)
					{
						Main.player[num237] = Main.ActivePlayerFileData.Player;
						Main.player[Main.myPlayer] = new Player();
					}
					Main.player[num237].whoAmI = num237;
					Main.myPlayer = num237;
					Player player14 = Main.player[num237];
					NetMessage.TrySendData(4, -1, -1, null, num237);
					NetMessage.TrySendData(68, -1, -1, null, num237);
					NetMessage.TrySendData(16, -1, -1, null, num237);
					NetMessage.TrySendData(42, -1, -1, null, num237);
					NetMessage.TrySendData(50, -1, -1, null, num237);
					for (int num238 = 0; num238 < 59; num238++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num237, num238, (int)player14.inventory[num238].prefix);
					}
					for (int num239 = 0; num239 < player14.armor.Length; num239++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num237, 59 + num239, (int)player14.armor[num239].prefix);
					}
					for (int num240 = 0; num240 < player14.dye.Length; num240++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num237, 79 + num240, (int)player14.dye[num240].prefix);
					}
					for (int num241 = 0; num241 < player14.miscEquips.Length; num241++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num237, 89 + num241, (int)player14.miscEquips[num241].prefix);
					}
					for (int num242 = 0; num242 < player14.miscDyes.Length; num242++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num237, 94 + num242, (int)player14.miscDyes[num242].prefix);
					}
					for (int num243 = 0; num243 < player14.bank.item.Length; num243++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num237, 99 + num243, (int)player14.bank.item[num243].prefix);
					}
					for (int num244 = 0; num244 < player14.bank2.item.Length; num244++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num237, 139 + num244, (int)player14.bank2.item[num244].prefix);
					}
					NetMessage.TrySendData(5, -1, -1, null, num237, 179f, (int)player14.trashItem.prefix);
					for (int num245 = 0; num245 < player14.bank3.item.Length; num245++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num237, 180 + num245, (int)player14.bank3.item[num245].prefix);
					}
					for (int num246 = 0; num246 < player14.bank4.item.Length; num246++)
					{
						NetMessage.TrySendData(5, -1, -1, null, num237, 220 + num246, (int)player14.bank4.item[num246].prefix);
					}
					NetMessage.TrySendData(6);
					if (Netplay.Connection.State == 2)
					{
						Netplay.Connection.State = 3;
					}
				}
				break;
			case 4:
			{
				int num70 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num70 = whoAmI;
				}
				if (num70 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					break;
				}
				Player player7 = Main.player[num70];
				player7.whoAmI = num70;
				player7.skinVariant = reader.ReadByte();
				player7.skinVariant = (int)MathHelper.Clamp(player7.skinVariant, 0f, 11f);
				player7.hair = reader.ReadByte();
				if (player7.hair >= 165)
				{
					player7.hair = 0;
				}
				player7.name = reader.ReadString().Trim().Trim();
				player7.hairDye = reader.ReadByte();
				BitsByte bitsByte6 = reader.ReadByte();
				for (int num71 = 0; num71 < 8; num71++)
				{
					player7.hideVisibleAccessory[num71] = bitsByte6[num71];
				}
				bitsByte6 = reader.ReadByte();
				for (int num72 = 0; num72 < 2; num72++)
				{
					player7.hideVisibleAccessory[num72 + 8] = bitsByte6[num72];
				}
				player7.hideMisc = reader.ReadByte();
				player7.hairColor = reader.ReadRGB();
				player7.skinColor = reader.ReadRGB();
				player7.eyeColor = reader.ReadRGB();
				player7.shirtColor = reader.ReadRGB();
				player7.underShirtColor = reader.ReadRGB();
				player7.pantsColor = reader.ReadRGB();
				player7.shoeColor = reader.ReadRGB();
				BitsByte bitsByte7 = reader.ReadByte();
				player7.difficulty = 0;
				if (bitsByte7[0])
				{
					player7.difficulty = 1;
				}
				if (bitsByte7[1])
				{
					player7.difficulty = 2;
				}
				if (bitsByte7[3])
				{
					player7.difficulty = 3;
				}
				if (player7.difficulty > 3)
				{
					player7.difficulty = 3;
				}
				player7.extraAccessory = bitsByte7[2];
				BitsByte bitsByte8 = reader.ReadByte();
				player7.UsingBiomeTorches = bitsByte8[0];
				player7.happyFunTorchTime = bitsByte8[1];
				player7.unlockedBiomeTorches = bitsByte8[2];
				if (Main.netMode != 2)
				{
					break;
				}
				bool flag6 = false;
				if (Netplay.Clients[whoAmI].State < 10)
				{
					for (int num73 = 0; num73 < 255; num73++)
					{
						if (num73 != num70 && player7.name == Main.player[num73].name && Netplay.Clients[num73].IsActive)
						{
							flag6 = true;
						}
					}
				}
				if (flag6)
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey(Lang.mp[5].Key, player7.name));
				}
				else if (player7.name.Length > Player.nameLen)
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey("Net.NameTooLong"));
				}
				else if (player7.name == "")
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey("Net.EmptyName"));
				}
				else if (player7.difficulty == 3 && !Main.GameModeInfo.IsJourneyMode)
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey("Net.PlayerIsCreativeAndWorldIsNotCreative"));
				}
				else if (player7.difficulty != 3 && Main.GameModeInfo.IsJourneyMode)
				{
					NetMessage.TrySendData(2, whoAmI, -1, NetworkText.FromKey("Net.PlayerIsNotCreativeAndWorldIsCreative"));
				}
				else
				{
					Netplay.Clients[whoAmI].Name = player7.name;
					Netplay.Clients[whoAmI].Name = player7.name;
					NetMessage.TrySendData(4, -1, whoAmI, null, num70);
				}
				break;
			}
			case 5:
			{
				int num34 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num34 = whoAmI;
				}
				if (num34 == Main.myPlayer && !Main.ServerSideCharacter && !Main.player[num34].HasLockedInventory())
				{
					break;
				}
				Player player2 = Main.player[num34];
				lock (player2)
				{
					int num35 = reader.ReadInt16();
					int stack2 = reader.ReadInt16();
					int num36 = reader.ReadByte();
					int type4 = reader.ReadInt16();
					Item[] array = null;
					Item[] array2 = null;
					int num37 = 0;
					bool flag2 = false;
					if (num35 >= 220)
					{
						num37 = num35 - 220;
						array = player2.bank4.item;
						array2 = Main.clientPlayer.bank4.item;
					}
					else if (num35 >= 180)
					{
						num37 = num35 - 180;
						array = player2.bank3.item;
						array2 = Main.clientPlayer.bank3.item;
					}
					else if (num35 >= 179)
					{
						flag2 = true;
					}
					else if (num35 >= 139)
					{
						num37 = num35 - 139;
						array = player2.bank2.item;
						array2 = Main.clientPlayer.bank2.item;
					}
					else if (num35 >= 99)
					{
						num37 = num35 - 99;
						array = player2.bank.item;
						array2 = Main.clientPlayer.bank.item;
					}
					else if (num35 >= 94)
					{
						num37 = num35 - 94;
						array = player2.miscDyes;
						array2 = Main.clientPlayer.miscDyes;
					}
					else if (num35 >= 89)
					{
						num37 = num35 - 89;
						array = player2.miscEquips;
						array2 = Main.clientPlayer.miscEquips;
					}
					else if (num35 >= 79)
					{
						num37 = num35 - 79;
						array = player2.dye;
						array2 = Main.clientPlayer.dye;
					}
					else if (num35 >= 59)
					{
						num37 = num35 - 59;
						array = player2.armor;
						array2 = Main.clientPlayer.armor;
					}
					else
					{
						num37 = num35;
						array = player2.inventory;
						array2 = Main.clientPlayer.inventory;
					}
					if (flag2)
					{
						player2.trashItem = new Item();
						player2.trashItem.netDefaults(type4);
						player2.trashItem.stack = stack2;
						player2.trashItem.Prefix(num36);
						if (num34 == Main.myPlayer && !Main.ServerSideCharacter)
						{
							Main.clientPlayer.trashItem = player2.trashItem.Clone();
						}
					}
					else if (num35 <= 58)
					{
						int type5 = array[num37].type;
						int stack3 = array[num37].stack;
						array[num37] = new Item();
						array[num37].netDefaults(type4);
						array[num37].stack = stack2;
						array[num37].Prefix(num36);
						if (num34 == Main.myPlayer && !Main.ServerSideCharacter)
						{
							array2[num37] = array[num37].Clone();
						}
						if (num34 == Main.myPlayer && num37 == 58)
						{
							Main.mouseItem = array[num37].Clone();
						}
						if (num34 == Main.myPlayer && Main.netMode == 1)
						{
							Main.player[num34].inventoryChestStack[num35] = false;
							if (array[num37].stack != stack3 || array[num37].type != type5)
							{
								Recipe.FindRecipes(canDelayCheck: true);
								SoundEngine.PlaySound(7);
							}
						}
					}
					else
					{
						array[num37] = new Item();
						array[num37].netDefaults(type4);
						array[num37].stack = stack2;
						array[num37].Prefix(num36);
						if (num34 == Main.myPlayer && !Main.ServerSideCharacter)
						{
							array2[num37] = array[num37].Clone();
						}
					}
					if (Main.netMode == 2 && num34 == whoAmI && num35 <= 58 + player2.armor.Length + player2.dye.Length + player2.miscEquips.Length + player2.miscDyes.Length)
					{
						NetMessage.TrySendData(5, -1, whoAmI, null, num34, num35, num36);
					}
				}
				break;
			}
			case 6:
				if (Main.netMode == 2)
				{
					if (Netplay.Clients[whoAmI].State == 1)
					{
						Netplay.Clients[whoAmI].State = 2;
					}
					NetMessage.TrySendData(7, whoAmI);
					Main.SyncAnInvasion(whoAmI);
				}
				break;
			case 7:
				if (Main.netMode == 1)
				{
					Main.time = reader.ReadInt32();
					BitsByte bitsByte20 = reader.ReadByte();
					Main.dayTime = bitsByte20[0];
					Main.bloodMoon = bitsByte20[1];
					Main.eclipse = bitsByte20[2];
					Main.moonPhase = reader.ReadByte();
					Main.maxTilesX = reader.ReadInt16();
					Main.maxTilesY = reader.ReadInt16();
					Main.spawnTileX = reader.ReadInt16();
					Main.spawnTileY = reader.ReadInt16();
					Main.worldSurface = reader.ReadInt16();
					Main.rockLayer = reader.ReadInt16();
					Main.worldID = reader.ReadInt32();
					Main.worldName = reader.ReadString();
					Main.GameMode = reader.ReadByte();
					Main.ActiveWorldFileData.UniqueId = new Guid(reader.ReadBytes(16));
					Main.ActiveWorldFileData.WorldGeneratorVersion = reader.ReadUInt64();
					Main.moonType = reader.ReadByte();
					WorldGen.setBG(0, reader.ReadByte());
					WorldGen.setBG(10, reader.ReadByte());
					WorldGen.setBG(11, reader.ReadByte());
					WorldGen.setBG(12, reader.ReadByte());
					WorldGen.setBG(1, reader.ReadByte());
					WorldGen.setBG(2, reader.ReadByte());
					WorldGen.setBG(3, reader.ReadByte());
					WorldGen.setBG(4, reader.ReadByte());
					WorldGen.setBG(5, reader.ReadByte());
					WorldGen.setBG(6, reader.ReadByte());
					WorldGen.setBG(7, reader.ReadByte());
					WorldGen.setBG(8, reader.ReadByte());
					WorldGen.setBG(9, reader.ReadByte());
					Main.iceBackStyle = reader.ReadByte();
					Main.jungleBackStyle = reader.ReadByte();
					Main.hellBackStyle = reader.ReadByte();
					Main.windSpeedTarget = reader.ReadSingle();
					Main.numClouds = reader.ReadByte();
					for (int num261 = 0; num261 < 3; num261++)
					{
						Main.treeX[num261] = reader.ReadInt32();
					}
					for (int num262 = 0; num262 < 4; num262++)
					{
						Main.treeStyle[num262] = reader.ReadByte();
					}
					for (int num263 = 0; num263 < 3; num263++)
					{
						Main.caveBackX[num263] = reader.ReadInt32();
					}
					for (int num264 = 0; num264 < 4; num264++)
					{
						Main.caveBackStyle[num264] = reader.ReadByte();
					}
					WorldGen.TreeTops.SyncReceive(reader);
					WorldGen.BackgroundsCache.UpdateCache();
					Main.maxRaining = reader.ReadSingle();
					Main.raining = Main.maxRaining > 0f;
					BitsByte bitsByte21 = reader.ReadByte();
					WorldGen.shadowOrbSmashed = bitsByte21[0];
					NPC.downedBoss1 = bitsByte21[1];
					NPC.downedBoss2 = bitsByte21[2];
					NPC.downedBoss3 = bitsByte21[3];
					Main.hardMode = bitsByte21[4];
					NPC.downedClown = bitsByte21[5];
					Main.ServerSideCharacter = bitsByte21[6];
					NPC.downedPlantBoss = bitsByte21[7];
					if (Main.ServerSideCharacter)
					{
						Main.ActivePlayerFileData.MarkAsServerSide();
					}
					BitsByte bitsByte22 = reader.ReadByte();
					NPC.downedMechBoss1 = bitsByte22[0];
					NPC.downedMechBoss2 = bitsByte22[1];
					NPC.downedMechBoss3 = bitsByte22[2];
					NPC.downedMechBossAny = bitsByte22[3];
					Main.cloudBGActive = (bitsByte22[4] ? 1 : 0);
					WorldGen.crimson = bitsByte22[5];
					Main.pumpkinMoon = bitsByte22[6];
					Main.snowMoon = bitsByte22[7];
					BitsByte bitsByte23 = reader.ReadByte();
					Main.fastForwardTime = bitsByte23[1];
					Main.UpdateTimeRate();
					bool num265 = bitsByte23[2];
					NPC.downedSlimeKing = bitsByte23[3];
					NPC.downedQueenBee = bitsByte23[4];
					NPC.downedFishron = bitsByte23[5];
					NPC.downedMartians = bitsByte23[6];
					NPC.downedAncientCultist = bitsByte23[7];
					BitsByte bitsByte24 = reader.ReadByte();
					NPC.downedMoonlord = bitsByte24[0];
					NPC.downedHalloweenKing = bitsByte24[1];
					NPC.downedHalloweenTree = bitsByte24[2];
					NPC.downedChristmasIceQueen = bitsByte24[3];
					NPC.downedChristmasSantank = bitsByte24[4];
					NPC.downedChristmasTree = bitsByte24[5];
					NPC.downedGolemBoss = bitsByte24[6];
					BirthdayParty.ManualParty = bitsByte24[7];
					BitsByte bitsByte25 = reader.ReadByte();
					NPC.downedPirates = bitsByte25[0];
					NPC.downedFrost = bitsByte25[1];
					NPC.downedGoblins = bitsByte25[2];
					Sandstorm.Happening = bitsByte25[3];
					DD2Event.Ongoing = bitsByte25[4];
					DD2Event.DownedInvasionT1 = bitsByte25[5];
					DD2Event.DownedInvasionT2 = bitsByte25[6];
					DD2Event.DownedInvasionT3 = bitsByte25[7];
					BitsByte bitsByte26 = reader.ReadByte();
					NPC.combatBookWasUsed = bitsByte26[0];
					LanternNight.ManualLanterns = bitsByte26[1];
					NPC.downedTowerSolar = bitsByte26[2];
					NPC.downedTowerVortex = bitsByte26[3];
					NPC.downedTowerNebula = bitsByte26[4];
					NPC.downedTowerStardust = bitsByte26[5];
					Main.forceHalloweenForToday = bitsByte26[6];
					Main.forceXMasForToday = bitsByte26[7];
					BitsByte bitsByte27 = reader.ReadByte();
					NPC.boughtCat = bitsByte27[0];
					NPC.boughtDog = bitsByte27[1];
					NPC.boughtBunny = bitsByte27[2];
					NPC.freeCake = bitsByte27[3];
					Main.drunkWorld = bitsByte27[4];
					NPC.downedEmpressOfLight = bitsByte27[5];
					NPC.downedQueenSlime = bitsByte27[6];
					Main.getGoodWorld = bitsByte27[7];
					BitsByte bitsByte28 = reader.ReadByte();
					Main.tenthAnniversaryWorld = bitsByte28[0];
					Main.dontStarveWorld = bitsByte28[1];
					NPC.downedDeerclops = bitsByte28[2];
					Main.notTheBeesWorld = bitsByte28[3];
					WorldGen.SavedOreTiers.Copper = reader.ReadInt16();
					WorldGen.SavedOreTiers.Iron = reader.ReadInt16();
					WorldGen.SavedOreTiers.Silver = reader.ReadInt16();
					WorldGen.SavedOreTiers.Gold = reader.ReadInt16();
					WorldGen.SavedOreTiers.Cobalt = reader.ReadInt16();
					WorldGen.SavedOreTiers.Mythril = reader.ReadInt16();
					WorldGen.SavedOreTiers.Adamantite = reader.ReadInt16();
					if (num265)
					{
						Main.StartSlimeRain();
					}
					else
					{
						Main.StopSlimeRain();
					}
					Main.invasionType = reader.ReadSByte();
					Main.LobbyId = reader.ReadUInt64();
					Sandstorm.IntendedSeverity = reader.ReadSingle();
					if (Netplay.Connection.State == 3)
					{
						Main.windSpeedCurrent = Main.windSpeedTarget;
						Netplay.Connection.State = 4;
					}
					Main.checkHalloween();
					Main.checkXMas();
				}
				break;
			case 8:
			{
				if (Main.netMode != 2)
				{
					break;
				}
				int num109 = reader.ReadInt32();
				int num110 = reader.ReadInt32();
				bool flag7 = true;
				if (num109 == -1 || num110 == -1)
				{
					flag7 = false;
				}
				else if (num109 < 10 || num109 > Main.maxTilesX - 10)
				{
					flag7 = false;
				}
				else if (num110 < 10 || num110 > Main.maxTilesY - 10)
				{
					flag7 = false;
				}
				int num111 = Netplay.GetSectionX(Main.spawnTileX) - 2;
				int num112 = Netplay.GetSectionY(Main.spawnTileY) - 1;
				int num113 = num111 + 5;
				int num114 = num112 + 3;
				if (num111 < 0)
				{
					num111 = 0;
				}
				if (num113 >= Main.maxSectionsX)
				{
					num113 = Main.maxSectionsX - 1;
				}
				if (num112 < 0)
				{
					num112 = 0;
				}
				if (num114 >= Main.maxSectionsY)
				{
					num114 = Main.maxSectionsY - 1;
				}
				int num115 = (num113 - num111) * (num114 - num112);
				List<Point> list = new List<Point>();
				for (int num116 = num111; num116 < num113; num116++)
				{
					for (int num117 = num112; num117 < num114; num117++)
					{
						list.Add(new Point(num116, num117));
					}
				}
				int num118 = -1;
				int num119 = -1;
				if (flag7)
				{
					num109 = Netplay.GetSectionX(num109) - 2;
					num110 = Netplay.GetSectionY(num110) - 1;
					num118 = num109 + 5;
					num119 = num110 + 3;
					if (num109 < 0)
					{
						num109 = 0;
					}
					if (num118 >= Main.maxSectionsX)
					{
						num118 = Main.maxSectionsX - 1;
					}
					if (num110 < 0)
					{
						num110 = 0;
					}
					if (num119 >= Main.maxSectionsY)
					{
						num119 = Main.maxSectionsY - 1;
					}
					for (int num120 = num109; num120 <= num118; num120++)
					{
						for (int num121 = num110; num121 <= num119; num121++)
						{
							if (num120 < num111 || num120 >= num113 || num121 < num112 || num121 >= num114)
							{
								list.Add(new Point(num120, num121));
								num115++;
							}
						}
					}
				}
				int num122 = 1;
				PortalHelper.SyncPortalsOnPlayerJoin(whoAmI, 1, list, out var portals, out var portalCenters);
				num115 += portals.Count;
				if (Netplay.Clients[whoAmI].State == 2)
				{
					Netplay.Clients[whoAmI].State = 3;
				}
				NetMessage.TrySendData(9, whoAmI, -1, Lang.inter[44].ToNetworkText(), num115);
				Netplay.Clients[whoAmI].StatusText2 = Language.GetTextValue("Net.IsReceivingTileData");
				Netplay.Clients[whoAmI].StatusMax += num115;
				for (int num123 = num111; num123 < num113; num123++)
				{
					for (int num124 = num112; num124 < num114; num124++)
					{
						NetMessage.SendSection(whoAmI, num123, num124);
					}
				}
				NetMessage.TrySendData(11, whoAmI, -1, null, num111, num112, num113 - 1, num114 - 1);
				if (flag7)
				{
					for (int num125 = num109; num125 <= num118; num125++)
					{
						for (int num126 = num110; num126 <= num119; num126++)
						{
							NetMessage.SendSection(whoAmI, num125, num126, skipSent: true);
						}
					}
					NetMessage.TrySendData(11, whoAmI, -1, null, num109, num110, num118, num119);
				}
				for (int num127 = 0; num127 < portals.Count; num127++)
				{
					NetMessage.SendSection(whoAmI, portals[num127].X, portals[num127].Y, skipSent: true);
				}
				for (int num128 = 0; num128 < portalCenters.Count; num128++)
				{
					NetMessage.TrySendData(11, whoAmI, -1, null, portalCenters[num128].X - num122, portalCenters[num128].Y - num122, portalCenters[num128].X + num122 + 1, portalCenters[num128].Y + num122 + 1);
				}
				for (int num129 = 0; num129 < 400; num129++)
				{
					if (Main.item[num129].active)
					{
						NetMessage.TrySendData(21, whoAmI, -1, null, num129);
						NetMessage.TrySendData(22, whoAmI, -1, null, num129);
					}
				}
				for (int num130 = 0; num130 < 200; num130++)
				{
					if (Main.npc[num130].active)
					{
						NetMessage.TrySendData(23, whoAmI, -1, null, num130);
					}
				}
				for (int num131 = 0; num131 < 1000; num131++)
				{
					if (Main.projectile[num131].active && (Main.projPet[Main.projectile[num131].type] || Main.projectile[num131].netImportant))
					{
						NetMessage.TrySendData(27, whoAmI, -1, null, num131);
					}
				}
				for (int num132 = 0; num132 < 289; num132++)
				{
					NetMessage.TrySendData(83, whoAmI, -1, null, num132);
				}
				NetMessage.TrySendData(49, whoAmI);
				NetMessage.TrySendData(57, whoAmI);
				NetMessage.TrySendData(7, whoAmI);
				NetMessage.TrySendData(103, -1, -1, null, NPC.MoonLordCountdown);
				NetMessage.TrySendData(101, whoAmI);
				NetMessage.TrySendData(136, whoAmI);
				Main.BestiaryTracker.OnPlayerJoining(whoAmI);
				CreativePowerManager.Instance.SyncThingsToJoiningPlayer(whoAmI);
				Main.PylonSystem.OnPlayerJoining(whoAmI);
				break;
			}
			case 9:
				if (Main.netMode == 1)
				{
					Netplay.Connection.StatusMax += reader.ReadInt32();
					Netplay.Connection.StatusText = NetworkText.Deserialize(reader).ToString();
					BitsByte bitsByte9 = reader.ReadByte();
					BitsByte serverSpecialFlags = Netplay.Connection.ServerSpecialFlags;
					serverSpecialFlags[0] = bitsByte9[0];
					serverSpecialFlags[1] = bitsByte9[1];
					Netplay.Connection.ServerSpecialFlags = serverSpecialFlags;
				}
				break;
			case 10:
				if (Main.netMode == 1)
				{
					NetMessage.DecompressTileBlock(readBuffer, num, length);
				}
				break;
			case 11:
				if (Main.netMode == 1)
				{
					WorldGen.SectionTileFrame(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
				}
				break;
			case 12:
			{
				int num235 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num235 = whoAmI;
				}
				Player player13 = Main.player[num235];
				player13.SpawnX = reader.ReadInt16();
				player13.SpawnY = reader.ReadInt16();
				player13.respawnTimer = reader.ReadInt32();
				if (player13.respawnTimer > 0)
				{
					player13.dead = true;
				}
				PlayerSpawnContext playerSpawnContext = (PlayerSpawnContext)reader.ReadByte();
				player13.Spawn(playerSpawnContext);
				if (num235 == Main.myPlayer && Main.netMode != 2)
				{
					Main.ActivePlayerFileData.StartPlayTimer();
					Player.Hooks.EnterWorld(Main.myPlayer);
				}
				if (Main.netMode != 2 || Netplay.Clients[whoAmI].State < 3)
				{
					break;
				}
				if (Netplay.Clients[whoAmI].State == 3)
				{
					Netplay.Clients[whoAmI].State = 10;
					NetMessage.buffer[whoAmI].broadcast = true;
					NetMessage.SyncConnectedPlayer(whoAmI);
					bool flag14 = NetMessage.DoesPlayerSlotCountAsAHost(whoAmI);
					Main.countsAsHostForGameplay[whoAmI] = flag14;
					if (NetMessage.DoesPlayerSlotCountAsAHost(whoAmI))
					{
						NetMessage.TrySendData(139, whoAmI, -1, null, whoAmI, flag14.ToInt());
					}
					NetMessage.TrySendData(12, -1, whoAmI, null, whoAmI, (int)(byte)playerSpawnContext);
					NetMessage.TrySendData(74, whoAmI, -1, NetworkText.FromLiteral(Main.player[whoAmI].name), Main.anglerQuest);
					NetMessage.TrySendData(129, whoAmI);
					NetMessage.greetPlayer(whoAmI);
					if (Main.player[num235].unlockedBiomeTorches)
					{
						NPC nPC5 = new NPC();
						spawnparams = default(NPCSpawnParams);
						nPC5.SetDefaults(664, spawnparams);
						Main.BestiaryTracker.Kills.RegisterKill(nPC5);
					}
				}
				else
				{
					NetMessage.TrySendData(12, -1, whoAmI, null, whoAmI, (int)(byte)playerSpawnContext);
				}
				break;
			}
			case 13:
			{
				int num213 = reader.ReadByte();
				if (num213 != Main.myPlayer || Main.ServerSideCharacter)
				{
					if (Main.netMode == 2)
					{
						num213 = whoAmI;
					}
					Player player11 = Main.player[num213];
					BitsByte bitsByte14 = reader.ReadByte();
					BitsByte bitsByte15 = reader.ReadByte();
					BitsByte bitsByte16 = reader.ReadByte();
					BitsByte bitsByte17 = reader.ReadByte();
					player11.controlUp = bitsByte14[0];
					player11.controlDown = bitsByte14[1];
					player11.controlLeft = bitsByte14[2];
					player11.controlRight = bitsByte14[3];
					player11.controlJump = bitsByte14[4];
					player11.controlUseItem = bitsByte14[5];
					player11.direction = (bitsByte14[6] ? 1 : (-1));
					if (bitsByte15[0])
					{
						player11.pulley = true;
						player11.pulleyDir = (byte)((!bitsByte15[1]) ? 1u : 2u);
					}
					else
					{
						player11.pulley = false;
					}
					player11.vortexStealthActive = bitsByte15[3];
					player11.gravDir = (bitsByte15[4] ? 1 : (-1));
					player11.TryTogglingShield(bitsByte15[5]);
					player11.ghost = bitsByte15[6];
					player11.selectedItem = reader.ReadByte();
					player11.position = reader.ReadVector2();
					if (bitsByte15[2])
					{
						player11.velocity = reader.ReadVector2();
					}
					else
					{
						player11.velocity = Vector2.Zero;
					}
					if (bitsByte16[6])
					{
						player11.PotionOfReturnOriginalUsePosition = reader.ReadVector2();
						player11.PotionOfReturnHomePosition = reader.ReadVector2();
					}
					else
					{
						player11.PotionOfReturnOriginalUsePosition = null;
						player11.PotionOfReturnHomePosition = null;
					}
					player11.tryKeepingHoveringUp = bitsByte16[0];
					player11.IsVoidVaultEnabled = bitsByte16[1];
					player11.sitting.isSitting = bitsByte16[2];
					player11.downedDD2EventAnyDifficulty = bitsByte16[3];
					player11.isPettingAnimal = bitsByte16[4];
					player11.isTheAnimalBeingPetSmall = bitsByte16[5];
					player11.tryKeepingHoveringDown = bitsByte16[7];
					player11.sleeping.SetIsSleepingAndAdjustPlayerRotation(player11, bitsByte17[0]);
					if (Main.netMode == 2 && Netplay.Clients[whoAmI].State == 10)
					{
						NetMessage.TrySendData(13, -1, whoAmI, null, num213);
					}
				}
				break;
			}
			case 14:
			{
				int num81 = reader.ReadByte();
				int num82 = reader.ReadByte();
				if (Main.netMode != 1)
				{
					break;
				}
				bool active = Main.player[num81].active;
				if (num82 == 1)
				{
					if (!Main.player[num81].active)
					{
						Main.player[num81] = new Player();
					}
					Main.player[num81].active = true;
				}
				else
				{
					Main.player[num81].active = false;
				}
				if (active != Main.player[num81].active)
				{
					if (Main.player[num81].active)
					{
						Player.Hooks.PlayerConnect(num81);
					}
					else
					{
						Player.Hooks.PlayerDisconnect(num81);
					}
				}
				break;
			}
			case 16:
			{
				int num227 = reader.ReadByte();
				if (num227 != Main.myPlayer || Main.ServerSideCharacter)
				{
					if (Main.netMode == 2)
					{
						num227 = whoAmI;
					}
					Player player12 = Main.player[num227];
					player12.statLife = reader.ReadInt16();
					player12.statLifeMax = reader.ReadInt16();
					if (player12.statLifeMax < 100)
					{
						player12.statLifeMax = 100;
					}
					player12.dead = player12.statLife <= 0;
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(16, -1, whoAmI, null, num227);
					}
				}
				break;
			}
			case 17:
			{
				byte b14 = reader.ReadByte();
				int num184 = reader.ReadInt16();
				int num185 = reader.ReadInt16();
				short num186 = reader.ReadInt16();
				int num187 = reader.ReadByte();
				bool flag10 = num186 == 1;
				if (!WorldGen.InWorld(num184, num185, 3))
				{
					break;
				}
				if (Main.tile[num184, num185] == null)
				{
					Main.tile[num184, num185] = new Tile();
				}
				if (Main.netMode == 2)
				{
					if (!flag10)
					{
						if (b14 == 0 || b14 == 2 || b14 == 4)
						{
							Netplay.Clients[whoAmI].SpamDeleteBlock += 1f;
						}
						if (b14 == 1 || b14 == 3)
						{
							Netplay.Clients[whoAmI].SpamAddBlock += 1f;
						}
					}
					if (!Netplay.Clients[whoAmI].TileSections[Netplay.GetSectionX(num184), Netplay.GetSectionY(num185)])
					{
						flag10 = true;
					}
				}
				if (b14 == 0)
				{
					WorldGen.KillTile(num184, num185, flag10);
					if (Main.netMode == 1 && !flag10)
					{
						HitTile.ClearAllTilesAtThisLocation(num184, num185);
					}
				}
				if (b14 == 1)
				{
					WorldGen.PlaceTile(num184, num185, num186, mute: false, forced: true, -1, num187);
				}
				if (b14 == 2)
				{
					WorldGen.KillWall(num184, num185, flag10);
				}
				if (b14 == 3)
				{
					WorldGen.PlaceWall(num184, num185, num186);
				}
				if (b14 == 4)
				{
					WorldGen.KillTile(num184, num185, flag10, effectOnly: false, noItem: true);
				}
				if (b14 == 5)
				{
					WorldGen.PlaceWire(num184, num185);
				}
				if (b14 == 6)
				{
					WorldGen.KillWire(num184, num185);
				}
				if (b14 == 7)
				{
					WorldGen.PoundTile(num184, num185);
				}
				if (b14 == 8)
				{
					WorldGen.PlaceActuator(num184, num185);
				}
				if (b14 == 9)
				{
					WorldGen.KillActuator(num184, num185);
				}
				if (b14 == 10)
				{
					WorldGen.PlaceWire2(num184, num185);
				}
				if (b14 == 11)
				{
					WorldGen.KillWire2(num184, num185);
				}
				if (b14 == 12)
				{
					WorldGen.PlaceWire3(num184, num185);
				}
				if (b14 == 13)
				{
					WorldGen.KillWire3(num184, num185);
				}
				if (b14 == 14)
				{
					WorldGen.SlopeTile(num184, num185, num186);
				}
				if (b14 == 15)
				{
					Minecart.FrameTrack(num184, num185, pound: true);
				}
				if (b14 == 16)
				{
					WorldGen.PlaceWire4(num184, num185);
				}
				if (b14 == 17)
				{
					WorldGen.KillWire4(num184, num185);
				}
				switch (b14)
				{
				case 18:
					Wiring.SetCurrentUser(whoAmI);
					Wiring.PokeLogicGate(num184, num185);
					Wiring.SetCurrentUser();
					return;
				case 19:
					Wiring.SetCurrentUser(whoAmI);
					Wiring.Actuate(num184, num185);
					Wiring.SetCurrentUser();
					return;
				case 20:
					if (WorldGen.InWorld(num184, num185, 2))
					{
						int type12 = Main.tile[num184, num185].type;
						WorldGen.KillTile(num184, num185, flag10);
						num186 = (short)((Main.tile[num184, num185].active() && Main.tile[num184, num185].type == type12) ? 1 : 0);
						if (Main.netMode == 2)
						{
							NetMessage.TrySendData(17, -1, -1, null, b14, num184, num185, num186, num187);
						}
					}
					return;
				case 21:
					WorldGen.ReplaceTile(num184, num185, (ushort)num186, num187);
					break;
				}
				if (b14 == 22)
				{
					WorldGen.ReplaceWall(num184, num185, (ushort)num186);
				}
				if (b14 == 23)
				{
					WorldGen.SlopeTile(num184, num185, num186);
					WorldGen.PoundTile(num184, num185);
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(17, -1, whoAmI, null, b14, num184, num185, num186, num187);
					if ((b14 == 1 || b14 == 21) && TileID.Sets.Falling[num186])
					{
						NetMessage.SendTileSquare(-1, num184, num185);
					}
				}
				break;
			}
			case 18:
				if (Main.netMode == 1)
				{
					Main.dayTime = reader.ReadByte() == 1;
					Main.time = reader.ReadInt32();
					Main.sunModY = reader.ReadInt16();
					Main.moonModY = reader.ReadInt16();
				}
				break;
			case 19:
			{
				byte b4 = reader.ReadByte();
				int num15 = reader.ReadInt16();
				int num16 = reader.ReadInt16();
				if (WorldGen.InWorld(num15, num16, 3))
				{
					int num17 = ((reader.ReadByte() != 0) ? 1 : (-1));
					switch (b4)
					{
					case 0:
						WorldGen.OpenDoor(num15, num16, num17);
						break;
					case 1:
						WorldGen.CloseDoor(num15, num16, forced: true);
						break;
					case 2:
						WorldGen.ShiftTrapdoor(num15, num16, num17 == 1, 1);
						break;
					case 3:
						WorldGen.ShiftTrapdoor(num15, num16, num17 == 1, 0);
						break;
					case 4:
						WorldGen.ShiftTallGate(num15, num16, closing: false, forced: true);
						break;
					case 5:
						WorldGen.ShiftTallGate(num15, num16, closing: true, forced: true);
						break;
					}
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(19, -1, whoAmI, null, b4, num15, num16, (num17 == 1) ? 1 : 0);
					}
				}
				break;
			}
			case 20:
			{
				int num56 = reader.ReadInt16();
				int num57 = reader.ReadInt16();
				ushort num58 = reader.ReadByte();
				ushort num59 = reader.ReadByte();
				byte b8 = reader.ReadByte();
				if (!WorldGen.InWorld(num56, num57, 3))
				{
					break;
				}
				TileChangeType type7 = TileChangeType.None;
				if (Enum.IsDefined(typeof(TileChangeType), b8))
				{
					type7 = (TileChangeType)b8;
				}
				if (MessageBuffer.OnTileChangeReceived != null)
				{
					MessageBuffer.OnTileChangeReceived(num56, num57, Math.Max(num58, num59), type7);
				}
				BitsByte bitsByte4 = (byte)0;
				BitsByte bitsByte5 = (byte)0;
				Tile tile4 = null;
				for (int num60 = num56; num60 < num56 + num58; num60++)
				{
					for (int num61 = num57; num61 < num57 + num59; num61++)
					{
						if (Main.tile[num60, num61] == null)
						{
							Main.tile[num60, num61] = new Tile();
						}
						tile4 = Main.tile[num60, num61];
						bool flag4 = tile4.active();
						bitsByte4 = reader.ReadByte();
						bitsByte5 = reader.ReadByte();
						tile4.active(bitsByte4[0]);
						tile4.wall = (byte)(bitsByte4[2] ? 1u : 0u);
						bool flag5 = bitsByte4[3];
						if (Main.netMode != 2)
						{
							tile4.liquid = (byte)(flag5 ? 1u : 0u);
						}
						tile4.wire(bitsByte4[4]);
						tile4.halfBrick(bitsByte4[5]);
						tile4.actuator(bitsByte4[6]);
						tile4.inActive(bitsByte4[7]);
						tile4.wire2(bitsByte5[0]);
						tile4.wire3(bitsByte5[1]);
						if (bitsByte5[2])
						{
							tile4.color(reader.ReadByte());
						}
						if (bitsByte5[3])
						{
							tile4.wallColor(reader.ReadByte());
						}
						if (tile4.active())
						{
							int type8 = tile4.type;
							tile4.type = reader.ReadUInt16();
							if (Main.tileFrameImportant[tile4.type])
							{
								tile4.frameX = reader.ReadInt16();
								tile4.frameY = reader.ReadInt16();
							}
							else if (!flag4 || tile4.type != type8)
							{
								tile4.frameX = -1;
								tile4.frameY = -1;
							}
							byte b9 = 0;
							if (bitsByte5[4])
							{
								b9 = (byte)(b9 + 1);
							}
							if (bitsByte5[5])
							{
								b9 = (byte)(b9 + 2);
							}
							if (bitsByte5[6])
							{
								b9 = (byte)(b9 + 4);
							}
							tile4.slope(b9);
						}
						tile4.wire4(bitsByte5[7]);
						if (tile4.wall > 0)
						{
							tile4.wall = reader.ReadUInt16();
						}
						if (flag5)
						{
							tile4.liquid = reader.ReadByte();
							tile4.liquidType(reader.ReadByte());
						}
					}
				}
				WorldGen.RangeFrame(num56, num57, num56 + num58, num57 + num59);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(b, -1, whoAmI, null, num56, num57, (int)num58, (int)num59, b8);
				}
				break;
			}
			case 21:
			case 90:
			{
				int num162 = reader.ReadInt16();
				Vector2 position2 = reader.ReadVector2();
				Vector2 velocity4 = reader.ReadVector2();
				int stack6 = reader.ReadInt16();
				int pre2 = reader.ReadByte();
				int num163 = reader.ReadByte();
				int num164 = reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num164 == 0)
					{
						Main.item[num162].active = false;
						break;
					}
					int num165 = num162;
					Item item2 = Main.item[num165];
					ItemSyncPersistentStats itemSyncPersistentStats = default(ItemSyncPersistentStats);
					itemSyncPersistentStats.CopyFrom(item2);
					bool newAndShiny = (item2.newAndShiny || item2.netID != num164) && ItemSlot.Options.HighlightNewItems && (num164 < 0 || num164 >= 5125 || !ItemID.Sets.NeverAppearsAsNewInInventory[num164]);
					item2.netDefaults(num164);
					item2.newAndShiny = newAndShiny;
					item2.Prefix(pre2);
					item2.stack = stack6;
					item2.position = position2;
					item2.velocity = velocity4;
					item2.active = true;
					if (b == 90)
					{
						item2.instanced = true;
						item2.playerIndexTheItemIsReservedFor = Main.myPlayer;
						item2.keepTime = 600;
					}
					item2.wet = Collision.WetCollision(item2.position, item2.width, item2.height);
					itemSyncPersistentStats.PasteInto(item2);
				}
				else
				{
					if (Main.timeItemSlotCannotBeReusedFor[num162] > 0)
					{
						break;
					}
					if (num164 == 0)
					{
						if (num162 < 400)
						{
							Main.item[num162].active = false;
							NetMessage.TrySendData(21, -1, -1, null, num162);
						}
						break;
					}
					bool flag9 = false;
					if (num162 == 400)
					{
						flag9 = true;
					}
					if (flag9)
					{
						Item item3 = new Item();
						item3.netDefaults(num164);
						num162 = Item.NewItem((int)position2.X, (int)position2.Y, item3.width, item3.height, item3.type, stack6, noBroadcast: true);
					}
					Item obj5 = Main.item[num162];
					obj5.netDefaults(num164);
					obj5.Prefix(pre2);
					obj5.stack = stack6;
					obj5.position = position2;
					obj5.velocity = velocity4;
					obj5.active = true;
					obj5.playerIndexTheItemIsReservedFor = Main.myPlayer;
					if (flag9)
					{
						NetMessage.TrySendData(21, -1, -1, null, num162);
						if (num163 == 0)
						{
							Main.item[num162].ownIgnore = whoAmI;
							Main.item[num162].ownTime = 100;
						}
						Main.item[num162].FindOwner(num162);
					}
					else
					{
						NetMessage.TrySendData(21, -1, whoAmI, null, num162);
					}
				}
				break;
			}
			case 22:
			{
				int num160 = reader.ReadInt16();
				int num161 = reader.ReadByte();
				if (Main.netMode != 2 || Main.item[num160].playerIndexTheItemIsReservedFor == whoAmI)
				{
					Main.item[num160].playerIndexTheItemIsReservedFor = num161;
					if (num161 == Main.myPlayer)
					{
						Main.item[num160].keepTime = 15;
					}
					else
					{
						Main.item[num160].keepTime = 0;
					}
					if (Main.netMode == 2)
					{
						Main.item[num160].playerIndexTheItemIsReservedFor = 255;
						Main.item[num160].keepTime = 15;
						NetMessage.TrySendData(22, -1, -1, null, num160);
					}
				}
				break;
			}
			case 23:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num144 = reader.ReadInt16();
				Vector2 vector5 = reader.ReadVector2();
				Vector2 velocity3 = reader.ReadVector2();
				int num145 = reader.ReadUInt16();
				if (num145 == 65535)
				{
					num145 = 0;
				}
				BitsByte bitsByte10 = reader.ReadByte();
				BitsByte bitsByte11 = reader.ReadByte();
				float[] array3 = new float[NPC.maxAI];
				for (int num146 = 0; num146 < NPC.maxAI; num146++)
				{
					if (bitsByte10[num146 + 2])
					{
						array3[num146] = reader.ReadSingle();
					}
					else
					{
						array3[num146] = 0f;
					}
				}
				int num147 = reader.ReadInt16();
				int? playerCountForMultiplayerDifficultyOverride = 1;
				if (bitsByte11[0])
				{
					playerCountForMultiplayerDifficultyOverride = reader.ReadByte();
				}
				float value5 = 1f;
				if (bitsByte11[2])
				{
					value5 = reader.ReadSingle();
				}
				int num148 = 0;
				if (!bitsByte10[7])
				{
					switch (reader.ReadByte())
					{
					case 2:
						num148 = reader.ReadInt16();
						break;
					case 4:
						num148 = reader.ReadInt32();
						break;
					default:
						num148 = reader.ReadSByte();
						break;
					}
				}
				int num149 = -1;
				NPC nPC4 = Main.npc[num144];
				if (nPC4.active && Main.multiplayerNPCSmoothingRange > 0 && Vector2.DistanceSquared(nPC4.position, vector5) < 640000f)
				{
					nPC4.netOffset += nPC4.position - vector5;
				}
				if (!nPC4.active || nPC4.netID != num147)
				{
					nPC4.netOffset *= 0f;
					if (nPC4.active)
					{
						num149 = nPC4.type;
					}
					nPC4.active = true;
					spawnparams = new NPCSpawnParams
					{
						playerCountForMultiplayerDifficultyOverride = playerCountForMultiplayerDifficultyOverride,
						strengthMultiplierOverride = value5
					};
					nPC4.SetDefaults(num147, spawnparams);
				}
				nPC4.position = vector5;
				nPC4.velocity = velocity3;
				nPC4.target = num145;
				nPC4.direction = (bitsByte10[0] ? 1 : (-1));
				nPC4.directionY = (bitsByte10[1] ? 1 : (-1));
				nPC4.spriteDirection = (bitsByte10[6] ? 1 : (-1));
				if (bitsByte10[7])
				{
					num148 = (nPC4.life = nPC4.lifeMax);
				}
				else
				{
					nPC4.life = num148;
				}
				if (num148 <= 0)
				{
					nPC4.active = false;
				}
				nPC4.SpawnedFromStatue = bitsByte11[0];
				if (nPC4.SpawnedFromStatue)
				{
					nPC4.value = 0f;
				}
				for (int num150 = 0; num150 < NPC.maxAI; num150++)
				{
					nPC4.ai[num150] = array3[num150];
				}
				if (num149 > -1 && num149 != nPC4.type)
				{
					nPC4.TransformVisuals(num149, nPC4.type);
				}
				if (num147 == 262)
				{
					NPC.plantBoss = num144;
				}
				if (num147 == 245)
				{
					NPC.golemBoss = num144;
				}
				if (nPC4.type >= 0 && nPC4.type < 670 && Main.npcCatchable[nPC4.type])
				{
					nPC4.releaseOwner = reader.ReadByte();
				}
				break;
			}
			case 24:
			{
				int num68 = reader.ReadInt16();
				int num69 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num69 = whoAmI;
				}
				Player player6 = Main.player[num69];
				Main.npc[num68].StrikeNPC(player6.inventory[player6.selectedItem].damage, player6.inventory[player6.selectedItem].knockBack, player6.direction);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(24, -1, whoAmI, null, num68, num69);
					NetMessage.TrySendData(23, -1, -1, null, num68);
				}
				break;
			}
			case 27:
			{
				int num168 = reader.ReadInt16();
				Vector2 position3 = reader.ReadVector2();
				Vector2 velocity5 = reader.ReadVector2();
				int num169 = reader.ReadByte();
				int num170 = reader.ReadInt16();
				BitsByte bitsByte12 = reader.ReadByte();
				float[] array4 = new float[Projectile.maxAI];
				for (int num171 = 0; num171 < Projectile.maxAI; num171++)
				{
					if (bitsByte12[num171])
					{
						array4[num171] = reader.ReadSingle();
					}
					else
					{
						array4[num171] = 0f;
					}
				}
				int bannerIdToRespondTo = (bitsByte12[3] ? reader.ReadUInt16() : 0);
				int damage2 = (bitsByte12[4] ? reader.ReadInt16() : 0);
				float knockBack2 = (bitsByte12[5] ? reader.ReadSingle() : 0f);
				int originalDamage = (bitsByte12[6] ? reader.ReadInt16() : 0);
				int num172 = (bitsByte12[7] ? reader.ReadInt16() : (-1));
				if (num172 >= 1000)
				{
					num172 = -1;
				}
				if (Main.netMode == 2)
				{
					if (num170 == 949)
					{
						num169 = 255;
					}
					else
					{
						num169 = whoAmI;
						if (Main.projHostile[num170])
						{
							break;
						}
					}
				}
				int num173 = 1000;
				for (int num174 = 0; num174 < 1000; num174++)
				{
					if (Main.projectile[num174].owner == num169 && Main.projectile[num174].identity == num168 && Main.projectile[num174].active)
					{
						num173 = num174;
						break;
					}
				}
				if (num173 == 1000)
				{
					for (int num175 = 0; num175 < 1000; num175++)
					{
						if (!Main.projectile[num175].active)
						{
							num173 = num175;
							break;
						}
					}
				}
				if (num173 == 1000)
				{
					num173 = Projectile.FindOldestProjectile();
				}
				Projectile projectile = Main.projectile[num173];
				if (!projectile.active || projectile.type != num170)
				{
					projectile.SetDefaults(num170);
					if (Main.netMode == 2)
					{
						Netplay.Clients[whoAmI].SpamProjectile += 1f;
					}
				}
				projectile.identity = num168;
				projectile.position = position3;
				projectile.velocity = velocity5;
				projectile.type = num170;
				projectile.damage = damage2;
				projectile.bannerIdToRespondTo = bannerIdToRespondTo;
				projectile.originalDamage = originalDamage;
				projectile.knockBack = knockBack2;
				projectile.owner = num169;
				for (int num176 = 0; num176 < Projectile.maxAI; num176++)
				{
					projectile.ai[num176] = array4[num176];
				}
				if (num172 >= 0)
				{
					projectile.projUUID = num172;
					Main.projectileIdentity[num169, num172] = num173;
				}
				projectile.ProjectileFixDesperation();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(27, -1, whoAmI, null, num173);
				}
				break;
			}
			case 28:
			{
				int num2 = reader.ReadInt16();
				int num3 = reader.ReadInt16();
				float num4 = reader.ReadSingle();
				int num5 = reader.ReadByte() - 1;
				byte b2 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					if (num3 < 0)
					{
						num3 = 0;
					}
					Main.npc[num2].PlayerInteraction(whoAmI);
				}
				if (num3 >= 0)
				{
					Main.npc[num2].StrikeNPC(num3, num4, num5, b2 == 1, noEffect: false, fromNet: true);
				}
				else
				{
					Main.npc[num2].life = 0;
					Main.npc[num2].HitEffect();
					Main.npc[num2].active = false;
				}
				if (Main.netMode != 2)
				{
					break;
				}
				NetMessage.TrySendData(28, -1, whoAmI, null, num2, num3, num4, num5, b2);
				if (Main.npc[num2].life <= 0)
				{
					NetMessage.TrySendData(23, -1, -1, null, num2);
				}
				else
				{
					Main.npc[num2].netUpdate = true;
				}
				if (Main.npc[num2].realLife >= 0)
				{
					if (Main.npc[Main.npc[num2].realLife].life <= 0)
					{
						NetMessage.TrySendData(23, -1, -1, null, Main.npc[num2].realLife);
					}
					else
					{
						Main.npc[Main.npc[num2].realLife].netUpdate = true;
					}
				}
				break;
			}
			case 29:
			{
				int num210 = reader.ReadInt16();
				int num211 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num211 = whoAmI;
				}
				for (int num212 = 0; num212 < 1000; num212++)
				{
					if (Main.projectile[num212].owner == num211 && Main.projectile[num212].identity == num210 && Main.projectile[num212].active)
					{
						Main.projectile[num212].Kill();
						break;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(29, -1, whoAmI, null, num210, num211);
				}
				break;
			}
			case 30:
			{
				int num217 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num217 = whoAmI;
				}
				bool flag12 = reader.ReadBoolean();
				Main.player[num217].hostile = flag12;
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(30, -1, whoAmI, null, num217);
					LocalizedText obj8 = (flag12 ? Lang.mp[11] : Lang.mp[12]);
					ChatHelper.BroadcastChatMessage(color: Main.teamColor[Main.player[num217].team], text: NetworkText.FromKey(obj8.Key, Main.player[num217].name));
				}
				break;
			}
			case 31:
			{
				if (Main.netMode != 2)
				{
					break;
				}
				int num64 = reader.ReadInt16();
				int num65 = reader.ReadInt16();
				int num66 = Chest.FindChest(num64, num65);
				if (num66 > -1 && Chest.UsingChest(num66) == -1)
				{
					for (int num67 = 0; num67 < 40; num67++)
					{
						NetMessage.TrySendData(32, whoAmI, -1, null, num66, num67);
					}
					NetMessage.TrySendData(33, whoAmI, -1, null, num66);
					Main.player[whoAmI].chest = num66;
					if (Main.myPlayer == whoAmI)
					{
						Main.recBigList = false;
					}
					NetMessage.TrySendData(80, -1, whoAmI, null, whoAmI, num66);
					if (Main.netMode == 2 && WorldGen.IsChestRigged(num64, num65))
					{
						Wiring.SetCurrentUser(whoAmI);
						Wiring.HitSwitch(num64, num65);
						Wiring.SetCurrentUser();
						NetMessage.TrySendData(59, -1, whoAmI, null, num64, num65);
					}
				}
				break;
			}
			case 32:
			{
				int num254 = reader.ReadInt16();
				int num255 = reader.ReadByte();
				int stack8 = reader.ReadInt16();
				int pre3 = reader.ReadByte();
				int type17 = reader.ReadInt16();
				if (num254 >= 0 && num254 < 8000)
				{
					if (Main.chest[num254] == null)
					{
						Main.chest[num254] = new Chest();
					}
					if (Main.chest[num254].item[num255] == null)
					{
						Main.chest[num254].item[num255] = new Item();
					}
					Main.chest[num254].item[num255].netDefaults(type17);
					Main.chest[num254].item[num255].Prefix(pre3);
					Main.chest[num254].item[num255].stack = stack8;
					Recipe.FindRecipes(canDelayCheck: true);
				}
				break;
			}
			case 33:
			{
				int num77 = reader.ReadInt16();
				int num78 = reader.ReadInt16();
				int num79 = reader.ReadInt16();
				int num80 = reader.ReadByte();
				string name = string.Empty;
				if (num80 != 0)
				{
					if (num80 <= 20)
					{
						name = reader.ReadString();
					}
					else if (num80 != 255)
					{
						num80 = 0;
					}
				}
				if (Main.netMode == 1)
				{
					Player player8 = Main.player[Main.myPlayer];
					if (player8.chest == -1)
					{
						Main.playerInventory = true;
						SoundEngine.PlaySound(10);
					}
					else if (player8.chest != num77 && num77 != -1)
					{
						Main.playerInventory = true;
						SoundEngine.PlaySound(12);
						Main.recBigList = false;
					}
					else if (player8.chest != -1 && num77 == -1)
					{
						SoundEngine.PlaySound(11);
						Main.recBigList = false;
					}
					player8.chest = num77;
					player8.chestX = num78;
					player8.chestY = num79;
					Recipe.FindRecipes(canDelayCheck: true);
					if (Main.tile[num78, num79].frameX >= 36 && Main.tile[num78, num79].frameX < 72)
					{
						AchievementsHelper.HandleSpecialEvent(Main.player[Main.myPlayer], 16);
					}
				}
				else
				{
					if (num80 != 0)
					{
						int chest = Main.player[whoAmI].chest;
						Chest chest2 = Main.chest[chest];
						chest2.name = name;
						NetMessage.TrySendData(69, -1, whoAmI, null, chest, chest2.x, chest2.y);
					}
					Main.player[whoAmI].chest = num77;
					Recipe.FindRecipes(canDelayCheck: true);
					NetMessage.TrySendData(80, -1, whoAmI, null, whoAmI, num77);
				}
				break;
			}
			case 34:
			{
				byte b5 = reader.ReadByte();
				int num24 = reader.ReadInt16();
				int num25 = reader.ReadInt16();
				int num26 = reader.ReadInt16();
				int num27 = reader.ReadInt16();
				if (Main.netMode == 2)
				{
					num27 = 0;
				}
				if (Main.netMode == 2)
				{
					switch (b5)
					{
					case 0:
					{
						int num30 = WorldGen.PlaceChest(num24, num25, 21, notNearOtherChests: false, num26);
						if (num30 == -1)
						{
							NetMessage.TrySendData(34, whoAmI, -1, null, b5, num24, num25, num26, num30);
							Item.NewItem(num24 * 16, num25 * 16, 32, 32, Chest.chestItemSpawn[num26], 1, noBroadcast: true);
						}
						else
						{
							NetMessage.TrySendData(34, -1, -1, null, b5, num24, num25, num26, num30);
						}
						break;
					}
					case 1:
						if (Main.tile[num24, num25].type == 21)
						{
							Tile tile = Main.tile[num24, num25];
							if (tile.frameX % 36 != 0)
							{
								num24--;
							}
							if (tile.frameY % 36 != 0)
							{
								num25--;
							}
							int number = Chest.FindChest(num24, num25);
							WorldGen.KillTile(num24, num25);
							if (!tile.active())
							{
								NetMessage.TrySendData(34, -1, -1, null, b5, num24, num25, 0f, number);
							}
							break;
						}
						goto default;
					default:
						switch (b5)
						{
						case 2:
						{
							int num28 = WorldGen.PlaceChest(num24, num25, 88, notNearOtherChests: false, num26);
							if (num28 == -1)
							{
								NetMessage.TrySendData(34, whoAmI, -1, null, b5, num24, num25, num26, num28);
								Item.NewItem(num24 * 16, num25 * 16, 32, 32, Chest.dresserItemSpawn[num26], 1, noBroadcast: true);
							}
							else
							{
								NetMessage.TrySendData(34, -1, -1, null, b5, num24, num25, num26, num28);
							}
							break;
						}
						case 3:
							if (Main.tile[num24, num25].type == 88)
							{
								Tile tile2 = Main.tile[num24, num25];
								num24 -= tile2.frameX % 54 / 18;
								if (tile2.frameY % 36 != 0)
								{
									num25--;
								}
								int number2 = Chest.FindChest(num24, num25);
								WorldGen.KillTile(num24, num25);
								if (!tile2.active())
								{
									NetMessage.TrySendData(34, -1, -1, null, b5, num24, num25, 0f, number2);
								}
								break;
							}
							goto default;
						default:
							switch (b5)
							{
							case 4:
							{
								int num29 = WorldGen.PlaceChest(num24, num25, 467, notNearOtherChests: false, num26);
								if (num29 == -1)
								{
									NetMessage.TrySendData(34, whoAmI, -1, null, b5, num24, num25, num26, num29);
									Item.NewItem(num24 * 16, num25 * 16, 32, 32, Chest.chestItemSpawn2[num26], 1, noBroadcast: true);
								}
								else
								{
									NetMessage.TrySendData(34, -1, -1, null, b5, num24, num25, num26, num29);
								}
								break;
							}
							case 5:
								if (Main.tile[num24, num25].type == 467)
								{
									Tile tile3 = Main.tile[num24, num25];
									if (tile3.frameX % 36 != 0)
									{
										num24--;
									}
									if (tile3.frameY % 36 != 0)
									{
										num25--;
									}
									int number3 = Chest.FindChest(num24, num25);
									WorldGen.KillTile(num24, num25);
									if (!tile3.active())
									{
										NetMessage.TrySendData(34, -1, -1, null, b5, num24, num25, 0f, number3);
									}
								}
								break;
							}
							break;
						}
						break;
					}
					break;
				}
				switch (b5)
				{
				case 0:
					if (num27 == -1)
					{
						WorldGen.KillTile(num24, num25);
						break;
					}
					SoundEngine.PlaySound(0, num24 * 16, num25 * 16);
					WorldGen.PlaceChestDirect(num24, num25, 21, num26, num27);
					break;
				case 2:
					if (num27 == -1)
					{
						WorldGen.KillTile(num24, num25);
						break;
					}
					SoundEngine.PlaySound(0, num24 * 16, num25 * 16);
					WorldGen.PlaceDresserDirect(num24, num25, 88, num26, num27);
					break;
				case 4:
					if (num27 == -1)
					{
						WorldGen.KillTile(num24, num25);
						break;
					}
					SoundEngine.PlaySound(0, num24 * 16, num25 * 16);
					WorldGen.PlaceChestDirect(num24, num25, 467, num26, num27);
					break;
				default:
					Chest.DestroyChestDirect(num24, num25, num27);
					WorldGen.KillTile(num24, num25);
					break;
				}
				break;
			}
			case 35:
			{
				int num250 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num250 = whoAmI;
				}
				int num251 = reader.ReadInt16();
				if (num250 != Main.myPlayer || Main.ServerSideCharacter)
				{
					Main.player[num250].HealEffect(num251);
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(35, -1, whoAmI, null, num250, num251);
				}
				break;
			}
			case 36:
			{
				int num234 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num234 = whoAmI;
				}
				Player obj9 = Main.player[num234];
				obj9.zone1 = reader.ReadByte();
				obj9.zone2 = reader.ReadByte();
				obj9.zone3 = reader.ReadByte();
				obj9.zone4 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(36, -1, whoAmI, null, num234);
				}
				break;
			}
			case 37:
				if (Main.netMode == 1)
				{
					if (Main.autoPass)
					{
						NetMessage.TrySendData(38);
						Main.autoPass = false;
					}
					else
					{
						Netplay.ServerPassword = "";
						Main.menuMode = 31;
					}
				}
				break;
			case 38:
				if (Main.netMode == 2)
				{
					if (reader.ReadString() == Netplay.ServerPassword)
					{
						Netplay.Clients[whoAmI].State = 1;
						NetMessage.TrySendData(3, whoAmI);
					}
					else
					{
						NetMessage.TrySendData(2, whoAmI, -1, Lang.mp[1].ToNetworkText());
					}
				}
				break;
			case 39:
				if (Main.netMode == 1)
				{
					int num188 = reader.ReadInt16();
					Main.item[num188].playerIndexTheItemIsReservedFor = 255;
					NetMessage.TrySendData(22, -1, -1, null, num188);
				}
				break;
			case 40:
			{
				int num180 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num180 = whoAmI;
				}
				int npcIndex = reader.ReadInt16();
				Main.player[num180].SetTalkNPC(npcIndex, fromNet: true);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(40, -1, whoAmI, null, num180);
				}
				break;
			}
			case 41:
			{
				int num104 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num104 = whoAmI;
				}
				Player player9 = Main.player[num104];
				float itemRotation = reader.ReadSingle();
				int itemAnimation = reader.ReadInt16();
				player9.itemRotation = itemRotation;
				player9.itemAnimation = itemAnimation;
				player9.channel = player9.inventory[player9.selectedItem].channel;
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(41, -1, whoAmI, null, num104);
				}
				break;
			}
			case 42:
			{
				int num76 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num76 = whoAmI;
				}
				else if (Main.myPlayer == num76 && !Main.ServerSideCharacter)
				{
					break;
				}
				int statMana = reader.ReadInt16();
				int statManaMax = reader.ReadInt16();
				Main.player[num76].statMana = statMana;
				Main.player[num76].statManaMax = statManaMax;
				break;
			}
			case 43:
			{
				int num19 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num19 = whoAmI;
				}
				int num20 = reader.ReadInt16();
				if (num19 != Main.myPlayer)
				{
					Main.player[num19].ManaEffect(num20);
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(43, -1, whoAmI, null, num19, num20);
				}
				break;
			}
			case 45:
			{
				int num247 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num247 = whoAmI;
				}
				int num248 = reader.ReadByte();
				Player player15 = Main.player[num247];
				int team = player15.team;
				player15.team = num248;
				Color color4 = Main.teamColor[num248];
				if (Main.netMode != 2)
				{
					break;
				}
				NetMessage.TrySendData(45, -1, whoAmI, null, num247);
				LocalizedText localizedText = Lang.mp[13 + num248];
				if (num248 == 5)
				{
					localizedText = Lang.mp[22];
				}
				for (int num249 = 0; num249 < 255; num249++)
				{
					if (num249 == whoAmI || (team > 0 && Main.player[num249].team == team) || (num248 > 0 && Main.player[num249].team == num248))
					{
						ChatHelper.SendChatMessageToClient(NetworkText.FromKey(localizedText.Key, player15.name), color4, num249);
					}
				}
				break;
			}
			case 46:
				if (Main.netMode == 2)
				{
					short i3 = reader.ReadInt16();
					int j3 = reader.ReadInt16();
					int num236 = Sign.ReadSign(i3, j3);
					if (num236 >= 0)
					{
						NetMessage.TrySendData(47, whoAmI, -1, null, num236, whoAmI);
					}
				}
				break;
			case 47:
			{
				int num196 = reader.ReadInt16();
				int x12 = reader.ReadInt16();
				int y11 = reader.ReadInt16();
				string text3 = reader.ReadString();
				int num197 = reader.ReadByte();
				BitsByte bitsByte13 = reader.ReadByte();
				if (num196 >= 0 && num196 < 1000)
				{
					string text4 = null;
					if (Main.sign[num196] != null)
					{
						text4 = Main.sign[num196].text;
					}
					Main.sign[num196] = new Sign();
					Main.sign[num196].x = x12;
					Main.sign[num196].y = y11;
					Sign.TextSign(num196, text3);
					if (Main.netMode == 2 && text4 != text3)
					{
						num197 = whoAmI;
						NetMessage.TrySendData(47, -1, whoAmI, null, num196, num197);
					}
					if (Main.netMode == 1 && num197 == Main.myPlayer && Main.sign[num196] != null && !bitsByte13[0])
					{
						Main.playerInventory = false;
						Main.player[Main.myPlayer].SetTalkNPC(-1, fromNet: true);
						Main.npcChatCornerItem = 0;
						Main.editSign = false;
						SoundEngine.PlaySound(10);
						Main.player[Main.myPlayer].sign = num196;
						Main.npcChatText = Main.sign[num196].text;
					}
				}
				break;
			}
			case 48:
			{
				int num87 = reader.ReadInt16();
				int num88 = reader.ReadInt16();
				byte liquid = reader.ReadByte();
				byte liquidType = reader.ReadByte();
				if (Main.netMode == 2 && Netplay.SpamCheck)
				{
					int num89 = whoAmI;
					int num90 = (int)(Main.player[num89].position.X + (float)(Main.player[num89].width / 2));
					int num91 = (int)(Main.player[num89].position.Y + (float)(Main.player[num89].height / 2));
					int num92 = 10;
					int num93 = num90 - num92;
					int num94 = num90 + num92;
					int num95 = num91 - num92;
					int num96 = num91 + num92;
					if (num87 < num93 || num87 > num94 || num88 < num95 || num88 > num96)
					{
						Netplay.Clients[whoAmI].SpamWater += 1f;
					}
				}
				if (Main.tile[num87, num88] == null)
				{
					Main.tile[num87, num88] = new Tile();
				}
				lock (Main.tile[num87, num88])
				{
					Main.tile[num87, num88].liquid = liquid;
					Main.tile[num87, num88].liquidType(liquidType);
					if (Main.netMode == 2)
					{
						WorldGen.SquareTileFrame(num87, num88);
					}
				}
				break;
			}
			case 49:
				if (Netplay.Connection.State == 6)
				{
					Netplay.Connection.State = 10;
					Main.ActivePlayerFileData.StartPlayTimer();
					Player.Hooks.EnterWorld(Main.myPlayer);
					Main.player[Main.myPlayer].Spawn(PlayerSpawnContext.SpawningIntoWorld);
				}
				break;
			case 50:
			{
				int num62 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num62 = whoAmI;
				}
				else if (num62 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					break;
				}
				Player player5 = Main.player[num62];
				for (int num63 = 0; num63 < 22; num63++)
				{
					player5.buffType[num63] = reader.ReadUInt16();
					if (player5.buffType[num63] > 0)
					{
						player5.buffTime[num63] = 60;
					}
					else
					{
						player5.buffTime[num63] = 0;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(50, -1, whoAmI, null, num62);
				}
				break;
			}
			case 51:
			{
				byte b6 = reader.ReadByte();
				byte b7 = reader.ReadByte();
				switch (b7)
				{
				case 1:
					NPC.SpawnSkeletron();
					break;
				case 2:
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(51, -1, whoAmI, null, b6, (int)b7);
					}
					else
					{
						SoundEngine.PlaySound(SoundID.Item1, (int)Main.player[b6].position.X, (int)Main.player[b6].position.Y);
					}
					break;
				case 3:
					if (Main.netMode == 2)
					{
						Main.Sundialing();
					}
					break;
				case 4:
					Main.npc[b6].BigMimicSpawnSmoke();
					break;
				case 5:
					if (Main.netMode == 2)
					{
						NPC nPC = new NPC();
						spawnparams = default(NPCSpawnParams);
						nPC.SetDefaults(664, spawnparams);
						Main.BestiaryTracker.Kills.RegisterKill(nPC);
					}
					break;
				}
				break;
			}
			case 52:
			{
				int num8 = reader.ReadByte();
				int num9 = reader.ReadInt16();
				int num10 = reader.ReadInt16();
				if (num8 == 1)
				{
					Chest.Unlock(num9, num10);
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(52, -1, whoAmI, null, 0, num8, num9, num10);
						NetMessage.SendTileSquare(-1, num9, num10, 2);
					}
				}
				if (num8 == 2)
				{
					WorldGen.UnlockDoor(num9, num10);
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(52, -1, whoAmI, null, 0, num8, num9, num10);
						NetMessage.SendTileSquare(-1, num9, num10, 2);
					}
				}
				break;
			}
			case 53:
			{
				int num7 = reader.ReadInt16();
				int type = reader.ReadUInt16();
				int time = reader.ReadInt16();
				Main.npc[num7].AddBuff(type, time, quiet: true);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(54, -1, -1, null, num7);
				}
				break;
			}
			case 54:
				if (Main.netMode == 1)
				{
					int num252 = reader.ReadInt16();
					NPC nPC6 = Main.npc[num252];
					for (int num253 = 0; num253 < 5; num253++)
					{
						nPC6.buffType[num253] = reader.ReadUInt16();
						nPC6.buffTime[num253] = reader.ReadInt16();
					}
				}
				break;
			case 55:
			{
				int num231 = reader.ReadByte();
				int num232 = reader.ReadUInt16();
				int num233 = reader.ReadInt32();
				if (Main.netMode != 2 || num231 == whoAmI || Main.pvpBuff[num232])
				{
					if (Main.netMode == 1 && num231 == Main.myPlayer)
					{
						Main.player[num231].AddBuff(num232, num233);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.TrySendData(55, num231, -1, null, num231, num232, num233);
					}
				}
				break;
			}
			case 56:
			{
				int num223 = reader.ReadInt16();
				if (num223 >= 0 && num223 < 200)
				{
					if (Main.netMode == 1)
					{
						string givenName = reader.ReadString();
						Main.npc[num223].GivenName = givenName;
						int townNpcVariationIndex = reader.ReadInt32();
						Main.npc[num223].townNpcVariationIndex = townNpcVariationIndex;
					}
					else if (Main.netMode == 2)
					{
						NetMessage.TrySendData(56, whoAmI, -1, null, num223);
					}
				}
				break;
			}
			case 57:
				if (Main.netMode == 1)
				{
					WorldGen.tGood = reader.ReadByte();
					WorldGen.tEvil = reader.ReadByte();
					WorldGen.tBlood = reader.ReadByte();
				}
				break;
			case 58:
			{
				int num208 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num208 = whoAmI;
				}
				float num209 = reader.ReadSingle();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(58, -1, whoAmI, null, whoAmI, num209);
					break;
				}
				Player player10 = Main.player[num208];
				int type14 = player10.inventory[player10.selectedItem].type;
				switch (type14)
				{
				case 4057:
				case 4372:
				case 4715:
					player10.PlayGuitarChord(num209);
					break;
				case 4673:
					player10.PlayDrums(num209);
					break;
				default:
				{
					Main.musicPitch = num209;
					LegacySoundStyle type15 = SoundID.Item26;
					if (type14 == 507)
					{
						type15 = SoundID.Item35;
					}
					if (type14 == 1305)
					{
						type15 = SoundID.Item47;
					}
					SoundEngine.PlaySound(type15, player10.position);
					break;
				}
				}
				break;
			}
			case 59:
			{
				int num194 = reader.ReadInt16();
				int num195 = reader.ReadInt16();
				Wiring.SetCurrentUser(whoAmI);
				Wiring.HitSwitch(num194, num195);
				Wiring.SetCurrentUser();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(59, -1, whoAmI, null, num194, num195);
				}
				break;
			}
			case 60:
			{
				int num157 = reader.ReadInt16();
				int num158 = reader.ReadInt16();
				int num159 = reader.ReadInt16();
				byte b13 = reader.ReadByte();
				if (num157 >= 200)
				{
					NetMessage.BootPlayer(whoAmI, NetworkText.FromKey("Net.CheatingInvalid"));
				}
				else if (Main.netMode == 1)
				{
					Main.npc[num157].homeless = b13 == 1;
					Main.npc[num157].homeTileX = num158;
					Main.npc[num157].homeTileY = num159;
					switch (b13)
					{
					case 1:
						WorldGen.TownManager.KickOut(Main.npc[num157].type);
						break;
					case 2:
						WorldGen.TownManager.SetRoom(Main.npc[num157].type, num158, num159);
						break;
					}
				}
				else if (b13 == 1)
				{
					WorldGen.kickOut(num157);
				}
				else
				{
					WorldGen.moveRoom(num158, num159, num157);
				}
				break;
			}
			case 61:
			{
				int plr = reader.ReadInt16();
				int num206 = reader.ReadInt16();
				if (Main.netMode != 2)
				{
					break;
				}
				if (num206 >= 0 && num206 < 670 && NPCID.Sets.MPAllowedEnemies[num206])
				{
					if (!NPC.AnyNPCs(num206))
					{
						NPC.SpawnOnPlayer(plr, num206);
					}
				}
				else if (num206 == -4)
				{
					if (!Main.dayTime && !DD2Event.Ongoing)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[31].Key), new Color(50, 255, 130));
						Main.startPumpkinMoon();
						NetMessage.TrySendData(7);
						NetMessage.TrySendData(78, -1, -1, null, 0, 1f, 2f, 1f);
					}
				}
				else if (num206 == -5)
				{
					if (!Main.dayTime && !DD2Event.Ongoing)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[34].Key), new Color(50, 255, 130));
						Main.startSnowMoon();
						NetMessage.TrySendData(7);
						NetMessage.TrySendData(78, -1, -1, null, 0, 1f, 1f, 1f);
					}
				}
				else if (num206 == -6)
				{
					if (Main.dayTime && !Main.eclipse)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[20].Key), new Color(50, 255, 130));
						Main.eclipse = true;
						NetMessage.TrySendData(7);
					}
				}
				else if (num206 == -7)
				{
					Main.invasionDelay = 0;
					Main.StartInvasion(4);
					NetMessage.TrySendData(7);
					NetMessage.TrySendData(78, -1, -1, null, 0, 1f, Main.invasionType + 3);
				}
				else if (num206 == -8)
				{
					if (NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
					{
						WorldGen.StartImpendingDoom();
						NetMessage.TrySendData(7);
					}
				}
				else if (num206 == -10)
				{
					if (!Main.dayTime && !Main.bloodMoon)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[8].Key), new Color(50, 255, 130));
						Main.bloodMoon = true;
						if (Main.GetMoonPhase() == MoonPhase.Empty)
						{
							Main.moonPhase = 5;
						}
						AchievementsHelper.NotifyProgressionEvent(4);
						NetMessage.TrySendData(7);
					}
				}
				else if (num206 == -11)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.CombatBookUsed"), new Color(50, 255, 130));
					NPC.combatBookWasUsed = true;
					NetMessage.TrySendData(7);
				}
				else if (num206 == -12)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.LicenseCatUsed"), new Color(50, 255, 130));
					NPC.boughtCat = true;
					NetMessage.TrySendData(7);
				}
				else if (num206 == -13)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.LicenseDogUsed"), new Color(50, 255, 130));
					NPC.boughtDog = true;
					NetMessage.TrySendData(7);
				}
				else if (num206 == -14)
				{
					ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.LicenseBunnyUsed"), new Color(50, 255, 130));
					NPC.boughtBunny = true;
					NetMessage.TrySendData(7);
				}
				else if (num206 < 0)
				{
					int num207 = 1;
					if (num206 > -5)
					{
						num207 = -num206;
					}
					if (num207 > 0 && Main.invasionType == 0)
					{
						Main.invasionDelay = 0;
						Main.StartInvasion(num207);
					}
					NetMessage.TrySendData(78, -1, -1, null, 0, 1f, Main.invasionType + 3);
				}
				break;
			}
			case 62:
			{
				int num155 = reader.ReadByte();
				int num156 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num155 = whoAmI;
				}
				if (num156 == 1)
				{
					Main.player[num155].NinjaDodge();
				}
				if (num156 == 2)
				{
					Main.player[num155].ShadowDodge();
				}
				if (num156 == 4)
				{
					Main.player[num155].BrainOfConfusionDodge();
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(62, -1, whoAmI, null, num155, num156);
				}
				break;
			}
			case 63:
			{
				int num151 = reader.ReadInt16();
				int num152 = reader.ReadInt16();
				byte b12 = reader.ReadByte();
				WorldGen.paintTile(num151, num152, b12);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(63, -1, whoAmI, null, num151, num152, (int)b12);
				}
				break;
			}
			case 64:
			{
				int num142 = reader.ReadInt16();
				int num143 = reader.ReadInt16();
				byte b11 = reader.ReadByte();
				WorldGen.paintWall(num142, num143, b11);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(64, -1, whoAmI, null, num142, num143, (int)b11);
				}
				break;
			}
			case 65:
			{
				BitsByte bitsByte3 = reader.ReadByte();
				int num41 = reader.ReadInt16();
				if (Main.netMode == 2)
				{
					num41 = whoAmI;
				}
				Vector2 vector = reader.ReadVector2();
				int num42 = 0;
				num42 = reader.ReadByte();
				int num43 = 0;
				if (bitsByte3[0])
				{
					num43++;
				}
				if (bitsByte3[1])
				{
					num43 += 2;
				}
				bool flag3 = false;
				if (bitsByte3[2])
				{
					flag3 = true;
				}
				int num44 = 0;
				if (bitsByte3[3])
				{
					num44 = reader.ReadInt32();
				}
				if (flag3)
				{
					vector = Main.player[num41].position;
				}
				switch (num43)
				{
				case 0:
					Main.player[num41].Teleport(vector, num42, num44);
					break;
				case 1:
					Main.npc[num41].Teleport(vector, num42, num44);
					break;
				case 2:
				{
					Main.player[num41].Teleport(vector, num42, num44);
					if (Main.netMode != 2)
					{
						break;
					}
					RemoteClient.CheckSection(whoAmI, vector);
					NetMessage.TrySendData(65, -1, -1, null, 0, num41, vector.X, vector.Y, num42, flag3.ToInt(), num44);
					int num45 = -1;
					float num46 = 9999f;
					for (int m = 0; m < 255; m++)
					{
						if (Main.player[m].active && m != whoAmI)
						{
							Vector2 vector2 = Main.player[m].position - Main.player[whoAmI].position;
							if (vector2.Length() < num46)
							{
								num46 = vector2.Length();
								num45 = m;
							}
						}
					}
					if (num45 >= 0)
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Game.HasTeleportedTo", Main.player[whoAmI].name, Main.player[num45].name), new Color(250, 250, 0));
					}
					break;
				}
				}
				if (Main.netMode == 2 && num43 == 0)
				{
					NetMessage.TrySendData(65, -1, whoAmI, null, num43, num41, vector.X, vector.Y, num42, flag3.ToInt(), num44);
				}
				break;
			}
			case 66:
			{
				int num22 = reader.ReadByte();
				int num23 = reader.ReadInt16();
				if (num23 > 0)
				{
					Player player = Main.player[num22];
					player.statLife += num23;
					if (player.statLife > player.statLifeMax2)
					{
						player.statLife = player.statLifeMax2;
					}
					player.HealEffect(num23, broadcast: false);
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(66, -1, whoAmI, null, num22, num23);
					}
				}
				break;
			}
			case 68:
				reader.ReadString();
				break;
			case 69:
			{
				int num228 = reader.ReadInt16();
				int num229 = reader.ReadInt16();
				int num230 = reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num228 >= 0 && num228 < 8000)
					{
						Chest chest3 = Main.chest[num228];
						if (chest3 == null)
						{
							chest3 = new Chest();
							chest3.x = num229;
							chest3.y = num230;
							Main.chest[num228] = chest3;
						}
						else if (chest3.x != num229 || chest3.y != num230)
						{
							break;
						}
						chest3.name = reader.ReadString();
					}
				}
				else
				{
					if (num228 < -1 || num228 >= 8000)
					{
						break;
					}
					if (num228 == -1)
					{
						num228 = Chest.FindChest(num229, num230);
						if (num228 == -1)
						{
							break;
						}
					}
					Chest chest4 = Main.chest[num228];
					if (chest4.x == num229 && chest4.y == num230)
					{
						NetMessage.TrySendData(69, whoAmI, -1, null, num228, num229, num230);
					}
				}
				break;
			}
			case 70:
				if (Main.netMode == 2)
				{
					int num219 = reader.ReadInt16();
					int who = reader.ReadByte();
					if (Main.netMode == 2)
					{
						who = whoAmI;
					}
					if (num219 < 200 && num219 >= 0)
					{
						NPC.CatchNPC(num219, who);
					}
				}
				break;
			case 71:
				if (Main.netMode == 2)
				{
					int x14 = reader.ReadInt32();
					int y13 = reader.ReadInt32();
					int type16 = reader.ReadInt16();
					byte style3 = reader.ReadByte();
					NPC.ReleaseNPC(x14, y13, type16, style3, whoAmI);
				}
				break;
			case 72:
				if (Main.netMode == 1)
				{
					for (int num214 = 0; num214 < 40; num214++)
					{
						Main.travelShop[num214] = reader.ReadInt16();
					}
				}
				break;
			case 73:
				switch (reader.ReadByte())
				{
				case 0:
					Main.player[whoAmI].TeleportationPotion();
					break;
				case 1:
					Main.player[whoAmI].MagicConch();
					break;
				case 2:
					Main.player[whoAmI].DemonConch();
					break;
				}
				break;
			case 74:
				if (Main.netMode == 1)
				{
					Main.anglerQuest = reader.ReadByte();
					Main.anglerQuestFinished = reader.ReadBoolean();
				}
				break;
			case 75:
				if (Main.netMode == 2)
				{
					string name2 = Main.player[whoAmI].name;
					if (!Main.anglerWhoFinishedToday.Contains(name2))
					{
						Main.anglerWhoFinishedToday.Add(name2);
					}
				}
				break;
			case 76:
			{
				int num193 = reader.ReadByte();
				if (num193 != Main.myPlayer || Main.ServerSideCharacter)
				{
					if (Main.netMode == 2)
					{
						num193 = whoAmI;
					}
					Player obj7 = Main.player[num193];
					obj7.anglerQuestsFinished = reader.ReadInt32();
					obj7.golferScoreAccumulated = reader.ReadInt32();
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(76, -1, whoAmI, null, num193);
					}
				}
				break;
			}
			case 77:
			{
				short type13 = reader.ReadInt16();
				ushort tileType = reader.ReadUInt16();
				short x11 = reader.ReadInt16();
				short y10 = reader.ReadInt16();
				Animation.NewTemporaryAnimation(type13, tileType, x11, y10);
				break;
			}
			case 78:
				if (Main.netMode == 1)
				{
					Main.ReportInvasionProgress(reader.ReadInt32(), reader.ReadInt32(), reader.ReadSByte(), reader.ReadSByte());
				}
				break;
			case 79:
			{
				int x9 = reader.ReadInt16();
				int y8 = reader.ReadInt16();
				short type11 = reader.ReadInt16();
				int style2 = reader.ReadInt16();
				int num154 = reader.ReadByte();
				int random = reader.ReadSByte();
				int direction = (reader.ReadBoolean() ? 1 : (-1));
				if (Main.netMode == 2)
				{
					Netplay.Clients[whoAmI].SpamAddBlock += 1f;
					if (!WorldGen.InWorld(x9, y8, 10) || !Netplay.Clients[whoAmI].TileSections[Netplay.GetSectionX(x9), Netplay.GetSectionY(y8)])
					{
						break;
					}
				}
				WorldGen.PlaceObject(x9, y8, type11, mute: false, style2, num154, random, direction);
				if (Main.netMode == 2)
				{
					NetMessage.SendObjectPlacment(whoAmI, x9, y8, type11, style2, num154, random, direction);
				}
				break;
			}
			case 80:
				if (Main.netMode == 1)
				{
					int num136 = reader.ReadByte();
					int num137 = reader.ReadInt16();
					if (num137 >= -3 && num137 < 8000)
					{
						Main.player[num136].chest = num137;
						Recipe.FindRecipes(canDelayCheck: true);
					}
				}
				break;
			case 81:
				if (Main.netMode == 1)
				{
					int x8 = (int)reader.ReadSingle();
					int y7 = (int)reader.ReadSingle();
					CombatText.NewText(color: reader.ReadRGB(), amount: reader.ReadInt32(), location: new Rectangle(x8, y7, 0, 0));
				}
				break;
			case 119:
				if (Main.netMode == 1)
				{
					int x7 = (int)reader.ReadSingle();
					int y6 = (int)reader.ReadSingle();
					CombatText.NewText(color: reader.ReadRGB(), text: NetworkText.Deserialize(reader).ToString(), location: new Rectangle(x7, y6, 0, 0));
				}
				break;
			case 82:
				NetManager.Instance.Read(reader, whoAmI, length);
				break;
			case 83:
				if (Main.netMode == 1)
				{
					int num102 = reader.ReadInt16();
					int num103 = reader.ReadInt32();
					if (num102 >= 0 && num102 < 289)
					{
						NPC.killCount[num102] = num103;
					}
				}
				break;
			case 84:
			{
				int num83 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num83 = whoAmI;
				}
				float stealth = reader.ReadSingle();
				Main.player[num83].stealth = stealth;
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(84, -1, whoAmI, null, num83);
				}
				break;
			}
			case 85:
			{
				int num75 = whoAmI;
				byte b10 = reader.ReadByte();
				if (Main.netMode == 2 && num75 < 255 && b10 < 58)
				{
					Chest.ServerPlaceItem(whoAmI, b10);
				}
				break;
			}
			case 86:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num47 = reader.ReadInt32();
				if (!reader.ReadBoolean())
				{
					if (TileEntity.ByID.TryGetValue(num47, out var value2))
					{
						TileEntity.ByID.Remove(num47);
						TileEntity.ByPosition.Remove(value2.Position);
					}
				}
				else
				{
					TileEntity tileEntity = TileEntity.Read(reader, networkSend: true);
					tileEntity.ID = num47;
					TileEntity.ByID[tileEntity.ID] = tileEntity;
					TileEntity.ByPosition[tileEntity.Position] = tileEntity;
				}
				break;
			}
			case 87:
				if (Main.netMode == 2)
				{
					int x4 = reader.ReadInt16();
					int y4 = reader.ReadInt16();
					int type3 = reader.ReadByte();
					if (WorldGen.InWorld(x4, y4) && !TileEntity.ByPosition.ContainsKey(new Point16(x4, y4)))
					{
						TileEntity.PlaceEntityNet(x4, y4, type3);
					}
				}
				break;
			case 88:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num218 = reader.ReadInt16();
				if (num218 < 0 || num218 > 400)
				{
					break;
				}
				Item item4 = Main.item[num218];
				BitsByte bitsByte18 = reader.ReadByte();
				if (bitsByte18[0])
				{
					item4.color.PackedValue = reader.ReadUInt32();
				}
				if (bitsByte18[1])
				{
					item4.damage = reader.ReadUInt16();
				}
				if (bitsByte18[2])
				{
					item4.knockBack = reader.ReadSingle();
				}
				if (bitsByte18[3])
				{
					item4.useAnimation = reader.ReadUInt16();
				}
				if (bitsByte18[4])
				{
					item4.useTime = reader.ReadUInt16();
				}
				if (bitsByte18[5])
				{
					item4.shoot = reader.ReadInt16();
				}
				if (bitsByte18[6])
				{
					item4.shootSpeed = reader.ReadSingle();
				}
				if (bitsByte18[7])
				{
					bitsByte18 = reader.ReadByte();
					if (bitsByte18[0])
					{
						item4.width = reader.ReadInt16();
					}
					if (bitsByte18[1])
					{
						item4.height = reader.ReadInt16();
					}
					if (bitsByte18[2])
					{
						item4.scale = reader.ReadSingle();
					}
					if (bitsByte18[3])
					{
						item4.ammo = reader.ReadInt16();
					}
					if (bitsByte18[4])
					{
						item4.useAmmo = reader.ReadInt16();
					}
					if (bitsByte18[5])
					{
						item4.notAmmo = reader.ReadBoolean();
					}
				}
				break;
			}
			case 89:
				if (Main.netMode == 2)
				{
					short x13 = reader.ReadInt16();
					int y12 = reader.ReadInt16();
					int netid3 = reader.ReadInt16();
					int prefix3 = reader.ReadByte();
					int stack7 = reader.ReadInt16();
					TEItemFrame.TryPlacing(x13, y12, netid3, prefix3, stack7);
				}
				break;
			case 91:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num201 = reader.ReadInt32();
				int num202 = reader.ReadByte();
				if (num202 == 255)
				{
					if (EmoteBubble.byID.ContainsKey(num201))
					{
						EmoteBubble.byID.Remove(num201);
					}
					break;
				}
				int num203 = reader.ReadUInt16();
				int num204 = reader.ReadUInt16();
				int num205 = reader.ReadByte();
				int metadata = 0;
				if (num205 < 0)
				{
					metadata = reader.ReadInt16();
				}
				WorldUIAnchor worldUIAnchor = EmoteBubble.DeserializeNetAnchor(num202, num203);
				if (num202 == 1)
				{
					Main.player[num203].emoteTime = 360;
				}
				lock (EmoteBubble.byID)
				{
					if (!EmoteBubble.byID.ContainsKey(num201))
					{
						EmoteBubble.byID[num201] = new EmoteBubble(num205, worldUIAnchor, num204);
					}
					else
					{
						EmoteBubble.byID[num201].lifeTime = num204;
						EmoteBubble.byID[num201].lifeTimeStart = num204;
						EmoteBubble.byID[num201].emote = num205;
						EmoteBubble.byID[num201].anchor = worldUIAnchor;
					}
					EmoteBubble.byID[num201].ID = num201;
					EmoteBubble.byID[num201].metadata = metadata;
					EmoteBubble.OnBubbleChange(num201);
				}
				break;
			}
			case 92:
			{
				int num189 = reader.ReadInt16();
				int num190 = reader.ReadInt32();
				float num191 = reader.ReadSingle();
				float num192 = reader.ReadSingle();
				if (num189 >= 0 && num189 <= 200)
				{
					if (Main.netMode == 1)
					{
						Main.npc[num189].moneyPing(new Vector2(num191, num192));
						Main.npc[num189].extraValue = num190;
					}
					else
					{
						Main.npc[num189].extraValue += num190;
						NetMessage.TrySendData(92, -1, -1, null, num189, Main.npc[num189].extraValue, num191, num192);
					}
				}
				break;
			}
			case 95:
			{
				ushort num181 = reader.ReadUInt16();
				int num182 = reader.ReadByte();
				if (Main.netMode != 2)
				{
					break;
				}
				for (int num183 = 0; num183 < 1000; num183++)
				{
					if (Main.projectile[num183].owner == num181 && Main.projectile[num183].active && Main.projectile[num183].type == 602 && Main.projectile[num183].ai[1] == (float)num182)
					{
						Main.projectile[num183].Kill();
						NetMessage.TrySendData(29, -1, -1, null, Main.projectile[num183].identity, (int)num181);
						break;
					}
				}
				break;
			}
			case 96:
			{
				int num177 = reader.ReadByte();
				Player obj6 = Main.player[num177];
				int num178 = reader.ReadInt16();
				Vector2 newPos2 = reader.ReadVector2();
				Vector2 velocity6 = reader.ReadVector2();
				int num179 = (obj6.lastPortalColorIndex = num178 + ((num178 % 2 == 0) ? 1 : (-1)));
				obj6.Teleport(newPos2, 4, num178);
				obj6.velocity = velocity6;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(96, -1, -1, null, num177, newPos2.X, newPos2.Y, num178);
				}
				break;
			}
			case 97:
				if (Main.netMode == 1)
				{
					AchievementsHelper.NotifyNPCKilledDirect(Main.player[Main.myPlayer], reader.ReadInt16());
				}
				break;
			case 98:
				if (Main.netMode == 1)
				{
					AchievementsHelper.NotifyProgressionEvent(reader.ReadInt16());
				}
				break;
			case 99:
			{
				int num153 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num153 = whoAmI;
				}
				Main.player[num153].MinionRestTargetPoint = reader.ReadVector2();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(99, -1, whoAmI, null, num153);
				}
				break;
			}
			case 115:
			{
				int num141 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num141 = whoAmI;
				}
				Main.player[num141].MinionAttackTargetNPC = reader.ReadInt16();
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(115, -1, whoAmI, null, num141);
				}
				break;
			}
			case 100:
			{
				int num133 = reader.ReadUInt16();
				NPC obj4 = Main.npc[num133];
				int num134 = reader.ReadInt16();
				Vector2 newPos = reader.ReadVector2();
				Vector2 velocity2 = reader.ReadVector2();
				int num135 = (obj4.lastPortalColorIndex = num134 + ((num134 % 2 == 0) ? 1 : (-1)));
				obj4.Teleport(newPos, 4, num134);
				obj4.velocity = velocity2;
				obj4.netOffset *= 0f;
				break;
			}
			case 101:
				if (Main.netMode != 2)
				{
					NPC.ShieldStrengthTowerSolar = reader.ReadUInt16();
					NPC.ShieldStrengthTowerVortex = reader.ReadUInt16();
					NPC.ShieldStrengthTowerNebula = reader.ReadUInt16();
					NPC.ShieldStrengthTowerStardust = reader.ReadUInt16();
					if (NPC.ShieldStrengthTowerSolar < 0)
					{
						NPC.ShieldStrengthTowerSolar = 0;
					}
					if (NPC.ShieldStrengthTowerVortex < 0)
					{
						NPC.ShieldStrengthTowerVortex = 0;
					}
					if (NPC.ShieldStrengthTowerNebula < 0)
					{
						NPC.ShieldStrengthTowerNebula = 0;
					}
					if (NPC.ShieldStrengthTowerStardust < 0)
					{
						NPC.ShieldStrengthTowerStardust = 0;
					}
					if (NPC.ShieldStrengthTowerSolar > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerSolar = NPC.LunarShieldPowerExpert;
					}
					if (NPC.ShieldStrengthTowerVortex > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerVortex = NPC.LunarShieldPowerExpert;
					}
					if (NPC.ShieldStrengthTowerNebula > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerNebula = NPC.LunarShieldPowerExpert;
					}
					if (NPC.ShieldStrengthTowerStardust > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerStardust = NPC.LunarShieldPowerExpert;
					}
				}
				break;
			case 102:
			{
				int num48 = reader.ReadByte();
				ushort num49 = reader.ReadUInt16();
				Vector2 other = reader.ReadVector2();
				if (Main.netMode == 2)
				{
					num48 = whoAmI;
					NetMessage.TrySendData(102, -1, -1, null, num48, (int)num49, other.X, other.Y);
					break;
				}
				Player player3 = Main.player[num48];
				for (int n = 0; n < 255; n++)
				{
					Player player4 = Main.player[n];
					if (!player4.active || player4.dead || (player3.team != 0 && player3.team != player4.team) || !(player4.Distance(other) < 700f))
					{
						continue;
					}
					Vector2 value3 = player3.Center - player4.Center;
					Vector2 vector3 = Vector2.Normalize(value3);
					if (!vector3.HasNaNs())
					{
						int type6 = 90;
						float num50 = 0f;
						float num51 = (float)Math.PI / 15f;
						Vector2 spinningpoint = new Vector2(0f, -8f);
						Vector2 vector4 = new Vector2(-3f);
						float num52 = 0f;
						float num53 = 0.005f;
						switch (num49)
						{
						case 179:
							type6 = 86;
							break;
						case 173:
							type6 = 90;
							break;
						case 176:
							type6 = 88;
							break;
						}
						for (int num54 = 0; (float)num54 < value3.Length() / 6f; num54++)
						{
							Vector2 position = player4.Center + 6f * (float)num54 * vector3 + spinningpoint.RotatedBy(num50) + vector4;
							num50 += num51;
							int num55 = Dust.NewDust(position, 6, 6, type6, 0f, 0f, 100, default(Color), 1.5f);
							Main.dust[num55].noGravity = true;
							Main.dust[num55].velocity = Vector2.Zero;
							num52 = (Main.dust[num55].fadeIn = num52 + num53);
							Main.dust[num55].velocity += vector3 * 1.5f;
						}
					}
					player4.NebulaLevelup(num49);
				}
				break;
			}
			case 103:
				if (Main.netMode == 1)
				{
					NPC.MoonLordCountdown = reader.ReadInt32();
				}
				break;
			case 104:
				if (Main.netMode == 1 && Main.npcShop > 0)
				{
					Item[] item = Main.instance.shop[Main.npcShop].item;
					int num33 = reader.ReadByte();
					int type2 = reader.ReadInt16();
					int stack = reader.ReadInt16();
					int pre = reader.ReadByte();
					int value = reader.ReadInt32();
					BitsByte bitsByte = reader.ReadByte();
					if (num33 < item.Length)
					{
						item[num33] = new Item();
						item[num33].netDefaults(type2);
						item[num33].stack = stack;
						item[num33].Prefix(pre);
						item[num33].value = value;
						item[num33].buyOnce = bitsByte[0];
					}
				}
				break;
			case 105:
				if (Main.netMode != 1)
				{
					short i2 = reader.ReadInt16();
					int j2 = reader.ReadInt16();
					bool on = reader.ReadBoolean();
					WorldGen.ToggleGemLock(i2, j2, on);
				}
				break;
			case 106:
				if (Main.netMode == 1)
				{
					HalfVector2 halfVector = default(HalfVector2);
					halfVector.PackedValue = reader.ReadUInt32();
					Utils.PoofOfSmoke(halfVector.ToVector2());
				}
				break;
			case 107:
				if (Main.netMode == 1)
				{
					Color c = reader.ReadRGB();
					string text = NetworkText.Deserialize(reader).ToString();
					int widthLimit = reader.ReadInt16();
					Main.NewTextMultiline(text, force: false, c, widthLimit);
				}
				break;
			case 108:
				if (Main.netMode == 1)
				{
					int damage = reader.ReadInt16();
					float knockBack = reader.ReadSingle();
					int x3 = reader.ReadInt16();
					int y3 = reader.ReadInt16();
					int angle = reader.ReadInt16();
					int ammo = reader.ReadInt16();
					int num14 = reader.ReadByte();
					if (num14 == Main.myPlayer)
					{
						WorldGen.ShootFromCannon(x3, y3, angle, ammo, damage, knockBack, num14, fromWire: true);
					}
				}
				break;
			case 109:
				if (Main.netMode == 2)
				{
					short x = reader.ReadInt16();
					int y = reader.ReadInt16();
					int x2 = reader.ReadInt16();
					int y2 = reader.ReadInt16();
					byte toolMode = reader.ReadByte();
					int num11 = whoAmI;
					WiresUI.Settings.MultiToolMode toolMode2 = WiresUI.Settings.ToolMode;
					WiresUI.Settings.ToolMode = (WiresUI.Settings.MultiToolMode)toolMode;
					Wiring.MassWireOperation(new Point(x, y), new Point(x2, y2), Main.player[num11]);
					WiresUI.Settings.ToolMode = toolMode2;
				}
				break;
			case 110:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int type18 = reader.ReadInt16();
				int num266 = reader.ReadInt16();
				int num267 = reader.ReadByte();
				if (num267 == Main.myPlayer)
				{
					Player player16 = Main.player[num267];
					for (int num268 = 0; num268 < num266; num268++)
					{
						player16.ConsumeItem(type18);
					}
					player16.wireOperationsCooldown = 0;
				}
				break;
			}
			case 111:
				if (Main.netMode == 2)
				{
					BirthdayParty.ToggleManualParty();
				}
				break;
			case 112:
			{
				int num256 = reader.ReadByte();
				int num257 = reader.ReadInt32();
				int num258 = reader.ReadInt32();
				int num259 = reader.ReadByte();
				int num260 = reader.ReadInt16();
				switch (num256)
				{
				case 1:
					if (Main.netMode == 1)
					{
						WorldGen.TreeGrowFX(num257, num258, num259, num260);
					}
					if (Main.netMode == 2)
					{
						NetMessage.TrySendData(b, -1, -1, null, num256, num257, num258, num259, num260);
					}
					break;
				case 2:
					NPC.FairyEffects(new Vector2(num257, num258), num260);
					break;
				}
				break;
			}
			case 113:
			{
				int x15 = reader.ReadInt16();
				int y14 = reader.ReadInt16();
				if (Main.netMode == 2 && !Main.snowMoon && !Main.pumpkinMoon)
				{
					if (DD2Event.WouldFailSpawningHere(x15, y14))
					{
						DD2Event.FailureMessage(whoAmI);
					}
					DD2Event.SummonCrystal(x15, y14);
				}
				break;
			}
			case 114:
				if (Main.netMode == 1)
				{
					DD2Event.WipeEntities();
				}
				break;
			case 116:
				if (Main.netMode == 1)
				{
					DD2Event.TimeLeftBetweenWaves = reader.ReadInt32();
				}
				break;
			case 117:
			{
				int num224 = reader.ReadByte();
				if (Main.netMode != 2 || whoAmI == num224 || (Main.player[num224].hostile && Main.player[whoAmI].hostile))
				{
					PlayerDeathReason playerDeathReason2 = PlayerDeathReason.FromReader(reader);
					int damage3 = reader.ReadInt16();
					int num225 = reader.ReadByte() - 1;
					BitsByte bitsByte19 = reader.ReadByte();
					bool flag13 = bitsByte19[0];
					bool pvp2 = bitsByte19[1];
					int num226 = reader.ReadSByte();
					Main.player[num224].Hurt(playerDeathReason2, damage3, num225, pvp2, quiet: true, flag13, num226);
					if (Main.netMode == 2)
					{
						NetMessage.SendPlayerHurt(num224, playerDeathReason2, damage3, num225, flag13, pvp2, num226, -1, whoAmI);
					}
				}
				break;
			}
			case 118:
			{
				int num220 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num220 = whoAmI;
				}
				PlayerDeathReason playerDeathReason = PlayerDeathReason.FromReader(reader);
				int num221 = reader.ReadInt16();
				int num222 = reader.ReadByte() - 1;
				bool pvp = ((BitsByte)reader.ReadByte())[0];
				Main.player[num220].KillMe(playerDeathReason, num221, num222, pvp);
				if (Main.netMode == 2)
				{
					NetMessage.SendPlayerDeath(num220, playerDeathReason, num221, num222, pvp, -1, whoAmI);
				}
				break;
			}
			case 120:
			{
				int num215 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num215 = whoAmI;
				}
				int num216 = reader.ReadByte();
				if (num216 >= 0 && num216 < 151 && Main.netMode == 2)
				{
					EmoteBubble.NewBubble(num216, new WorldUIAnchor(Main.player[num215]), 360);
					EmoteBubble.CheckForNPCsToReactToEmoteBubble(num216, Main.player[num215]);
				}
				break;
			}
			case 121:
			{
				int num198 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num198 = whoAmI;
				}
				int num199 = reader.ReadInt32();
				int num200 = reader.ReadByte();
				bool flag11 = false;
				if (num200 >= 8)
				{
					flag11 = true;
					num200 -= 8;
				}
				if (!TileEntity.ByID.TryGetValue(num199, out var value8))
				{
					reader.ReadInt32();
					reader.ReadByte();
					break;
				}
				if (num200 >= 8)
				{
					value8 = null;
				}
				TEDisplayDoll tEDisplayDoll = value8 as TEDisplayDoll;
				if (tEDisplayDoll != null)
				{
					tEDisplayDoll.ReadItem(num200, reader, flag11);
				}
				else
				{
					reader.ReadInt32();
					reader.ReadByte();
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(b, -1, num198, null, num198, num199, num200, flag11.ToInt());
				}
				break;
			}
			case 122:
			{
				int num166 = reader.ReadInt32();
				int num167 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num167 = whoAmI;
				}
				if (Main.netMode == 2)
				{
					if (num166 == -1)
					{
						Main.player[num167].tileEntityAnchor.Clear();
						NetMessage.TrySendData(b, -1, -1, null, num166, num167);
						break;
					}
					if (!TileEntity.IsOccupied(num166, out var _) && TileEntity.ByID.TryGetValue(num166, out var value6))
					{
						Main.player[num167].tileEntityAnchor.Set(num166, value6.Position.X, value6.Position.Y);
						NetMessage.TrySendData(b, -1, -1, null, num166, num167);
					}
				}
				if (Main.netMode == 1)
				{
					TileEntity value7;
					if (num166 == -1)
					{
						Main.player[num167].tileEntityAnchor.Clear();
					}
					else if (TileEntity.ByID.TryGetValue(num166, out value7))
					{
						TileEntity.SetInteractionAnchor(Main.player[num167], value7.Position.X, value7.Position.Y, num166);
					}
				}
				break;
			}
			case 123:
				if (Main.netMode == 2)
				{
					short x10 = reader.ReadInt16();
					int y9 = reader.ReadInt16();
					int netid2 = reader.ReadInt16();
					int prefix2 = reader.ReadByte();
					int stack5 = reader.ReadInt16();
					TEWeaponsRack.TryPlacing(x10, y9, netid2, prefix2, stack5);
				}
				break;
			case 124:
			{
				int num138 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num138 = whoAmI;
				}
				int num139 = reader.ReadInt32();
				int num140 = reader.ReadByte();
				bool flag8 = false;
				if (num140 >= 2)
				{
					flag8 = true;
					num140 -= 2;
				}
				if (!TileEntity.ByID.TryGetValue(num139, out var value4))
				{
					reader.ReadInt32();
					reader.ReadByte();
					break;
				}
				if (num140 >= 2)
				{
					value4 = null;
				}
				TEHatRack tEHatRack = value4 as TEHatRack;
				if (tEHatRack != null)
				{
					tEHatRack.ReadItem(num140, reader, flag8);
				}
				else
				{
					reader.ReadInt32();
					reader.ReadByte();
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(b, -1, num138, null, num138, num139, num140, flag8.ToInt());
				}
				break;
			}
			case 125:
			{
				int num105 = reader.ReadByte();
				int num106 = reader.ReadInt16();
				int num107 = reader.ReadInt16();
				int num108 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num105 = whoAmI;
				}
				if (Main.netMode == 1)
				{
					Main.player[Main.myPlayer].GetOtherPlayersPickTile(num106, num107, num108);
				}
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(125, -1, num105, null, num105, num106, num107, num108);
				}
				break;
			}
			case 126:
				if (Main.netMode == 1)
				{
					NPC.RevengeManager.AddMarkerFromReader(reader);
				}
				break;
			case 127:
			{
				int markerUniqueID = reader.ReadInt32();
				if (Main.netMode == 1)
				{
					NPC.RevengeManager.DestroyMarker(markerUniqueID);
				}
				break;
			}
			case 128:
			{
				int num97 = reader.ReadByte();
				int num98 = reader.ReadUInt16();
				int num99 = reader.ReadUInt16();
				int num100 = reader.ReadUInt16();
				int num101 = reader.ReadUInt16();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(128, -1, num97, null, num97, num100, num101, 0f, num98, num99);
				}
				else
				{
					GolfHelper.ContactListener.PutBallInCup_TextAndEffects(new Point(num98, num99), num97, num100, num101);
				}
				break;
			}
			case 129:
				if (Main.netMode == 1)
				{
					Main.FixUIScale();
					Main.TrySetPreparationState(Main.WorldPreparationState.ProcessingData);
				}
				break;
			case 130:
				if (Main.netMode == 2)
				{
					ushort num84 = reader.ReadUInt16();
					int num85 = reader.ReadUInt16();
					int type9 = reader.ReadInt16();
					int x6 = num84 * 16;
					num85 *= 16;
					NPC nPC3 = new NPC();
					spawnparams = default(NPCSpawnParams);
					nPC3.SetDefaults(type9, spawnparams);
					int type10 = nPC3.type;
					int netID = nPC3.netID;
					int num86 = NPC.NewNPC(x6, num85, type9);
					if (netID != type10)
					{
						NPC obj3 = Main.npc[num86];
						spawnparams = default(NPCSpawnParams);
						obj3.SetDefaults(netID, spawnparams);
						NetMessage.TrySendData(23, -1, -1, null, num86);
					}
				}
				break;
			case 131:
				if (Main.netMode == 1)
				{
					int num74 = reader.ReadUInt16();
					NPC nPC2 = null;
					nPC2 = ((num74 >= 200) ? new NPC() : Main.npc[num74]);
					if (reader.ReadByte() == 1)
					{
						int time2 = reader.ReadInt32();
						int fromWho = reader.ReadInt16();
						nPC2.GetImmuneTime(fromWho, time2);
					}
				}
				break;
			case 132:
				if (Main.netMode == 1)
				{
					Point point = reader.ReadVector2().ToPoint();
					ushort key = reader.ReadUInt16();
					LegacySoundStyle legacySoundStyle = SoundID.SoundByIndex[key];
					BitsByte bitsByte2 = reader.ReadByte();
					int num38 = -1;
					float num39 = 1f;
					float num40 = 0f;
					SoundEngine.PlaySound(Style: (!bitsByte2[0]) ? legacySoundStyle.Style : reader.ReadInt32(), volumeScale: (!bitsByte2[1]) ? legacySoundStyle.Volume : MathHelper.Clamp(reader.ReadSingle(), 0f, 1f), pitchOffset: (!bitsByte2[2]) ? legacySoundStyle.GetRandomPitch() : MathHelper.Clamp(reader.ReadSingle(), -1f, 1f), type: legacySoundStyle.SoundId, x: point.X, y: point.Y);
				}
				break;
			case 133:
				if (Main.netMode == 2)
				{
					short x5 = reader.ReadInt16();
					int y5 = reader.ReadInt16();
					int netid = reader.ReadInt16();
					int prefix = reader.ReadByte();
					int stack4 = reader.ReadInt16();
					TEFoodPlatter.TryPlacing(x5, y5, netid, prefix, stack4);
				}
				break;
			case 134:
			{
				int num32 = reader.ReadByte();
				int ladyBugLuckTimeLeft = reader.ReadInt32();
				float torchLuck = reader.ReadSingle();
				byte luckPotion = reader.ReadByte();
				bool hasGardenGnomeNearby = reader.ReadBoolean();
				if (Main.netMode == 2)
				{
					num32 = whoAmI;
				}
				Player obj2 = Main.player[num32];
				obj2.ladyBugLuckTimeLeft = ladyBugLuckTimeLeft;
				obj2.torchLuck = torchLuck;
				obj2.luckPotion = luckPotion;
				obj2.HasGardenGnomeNearby = hasGardenGnomeNearby;
				obj2.RecalculateLuck();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(134, -1, num32, null, num32);
				}
				break;
			}
			case 135:
			{
				int num31 = reader.ReadByte();
				if (Main.netMode == 1)
				{
					Main.player[num31].immuneAlpha = 255;
				}
				break;
			}
			case 136:
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = 0; l < 3; l++)
					{
						NPC.cavernMonsterType[k, l] = reader.ReadUInt16();
					}
				}
				break;
			}
			case 137:
				if (Main.netMode == 2)
				{
					int num21 = reader.ReadInt16();
					int buffTypeToRemove = reader.ReadUInt16();
					if (num21 >= 0 && num21 < 200)
					{
						Main.npc[num21].RequestBuffRemoval(buffTypeToRemove);
					}
				}
				break;
			case 139:
				if (Main.netMode != 2)
				{
					int num18 = reader.ReadByte();
					bool flag = reader.ReadBoolean();
					Main.countsAsHostForGameplay[num18] = flag;
				}
				break;
			case 140:
				if (Main.netMode == 1)
				{
					reader.ReadByte();
					CreditsRollEvent.SetRemainingTimeDirect(reader.ReadInt32());
				}
				break;
			case 141:
			{
				LucyAxeMessage.MessageSource messageSource = (LucyAxeMessage.MessageSource)reader.ReadByte();
				byte b3 = reader.ReadByte();
				Vector2 velocity = reader.ReadVector2();
				int num12 = reader.ReadInt32();
				int num13 = reader.ReadInt32();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(141, -1, whoAmI, null, (int)messageSource, (int)b3, velocity.X, velocity.Y, num12, num13);
				}
				else
				{
					LucyAxeMessage.CreateFromNet(messageSource, b3, new Vector2(num12, num13), velocity);
				}
				break;
			}
			case 142:
			{
				int num6 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num6 = whoAmI;
				}
				Player obj = Main.player[num6];
				obj.piggyBankProjTracker.TryReading(reader);
				obj.voidLensChest.TryReading(reader);
				if (Main.netMode == 2)
				{
					NetMessage.TrySendData(142, -1, whoAmI, null, num6);
				}
				break;
			}
			default:
				if (Netplay.Clients[whoAmI].State == 0)
				{
					NetMessage.BootPlayer(whoAmI, Lang.mp[2].ToNetworkText());
				}
				break;
			case 15:
			case 25:
			case 26:
			case 44:
			case 67:
			case 93:
				break;
			}
		}
	}
}
