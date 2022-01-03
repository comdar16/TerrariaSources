using System;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;

namespace Terraria.GameContent.RGB
{
	// Token: 0x02000254 RID: 596
	public static class CommonConditions
	{
		// Token: 0x04004376 RID: 17270
		public static readonly ChromaCondition InMenu = new CommonConditions.SimpleCondition((Player player) => Main.gameMenu && !Main.drunkWorld);

		// Token: 0x04004377 RID: 17271
		public static readonly ChromaCondition DrunkMenu = new CommonConditions.SimpleCondition((Player player) => Main.gameMenu && Main.drunkWorld);

		// Token: 0x020005CA RID: 1482
		public abstract class ConditionBase : ChromaCondition
		{
			// Token: 0x1700039B RID: 923
			// (get) Token: 0x06002FB5 RID: 12213 RVA: 0x0031BD0D File Offset: 0x00319F0D
			protected Player CurrentPlayer
			{
				get
				{
					return Main.player[Main.myPlayer];
				}
			}
		}

		// Token: 0x020005CB RID: 1483
		private class SimpleCondition : CommonConditions.ConditionBase
		{
			// Token: 0x06002FB7 RID: 12215 RVA: 0x005AC069 File Offset: 0x005AA269
			public SimpleCondition(Func<Player, bool> condition)
			{
				this._condition = condition;
			}

			// Token: 0x06002FB8 RID: 12216 RVA: 0x005AC078 File Offset: 0x005AA278
			public override bool IsActive()
			{
				return this._condition(base.CurrentPlayer);
			}

			// Token: 0x04005A4F RID: 23119
			private Func<Player, bool> _condition;
		}

		// Token: 0x020005CC RID: 1484
		public static class SurfaceBiome
		{
			// Token: 0x04005A50 RID: 23120
			public static readonly ChromaCondition Ocean = new CommonConditions.SimpleCondition((Player player) => player.ZoneBeach && player.ZoneOverworldHeight);

			// Token: 0x04005A51 RID: 23121
			public static readonly ChromaCondition Desert = new CommonConditions.SimpleCondition((Player player) => player.ZoneDesert && !player.ZoneBeach && player.ZoneOverworldHeight);

			// Token: 0x04005A52 RID: 23122
			public static readonly ChromaCondition Jungle = new CommonConditions.SimpleCondition((Player player) => player.ZoneJungle && player.ZoneOverworldHeight);

			// Token: 0x04005A53 RID: 23123
			public static readonly ChromaCondition Snow = new CommonConditions.SimpleCondition((Player player) => player.ZoneSnow && player.ZoneOverworldHeight);

			// Token: 0x04005A54 RID: 23124
			public static readonly ChromaCondition Mushroom = new CommonConditions.SimpleCondition((Player player) => player.ZoneGlowshroom && player.ZoneOverworldHeight);

			// Token: 0x04005A55 RID: 23125
			public static readonly ChromaCondition Corruption = new CommonConditions.SimpleCondition((Player player) => player.ZoneCorrupt && player.ZoneOverworldHeight);

			// Token: 0x04005A56 RID: 23126
			public static readonly ChromaCondition Hallow = new CommonConditions.SimpleCondition((Player player) => player.ZoneHallow && player.ZoneOverworldHeight);

			// Token: 0x04005A57 RID: 23127
			public static readonly ChromaCondition Crimson = new CommonConditions.SimpleCondition((Player player) => player.ZoneCrimson && player.ZoneOverworldHeight);
		}

		// Token: 0x020005CD RID: 1485
		public static class MiscBiome
		{
			// Token: 0x04005A58 RID: 23128
			public static readonly ChromaCondition Meteorite = new CommonConditions.SimpleCondition((Player player) => player.ZoneMeteor);
		}

		// Token: 0x020005CE RID: 1486
		public static class UndergroundBiome
		{
			// Token: 0x06002FBB RID: 12219 RVA: 0x005AC188 File Offset: 0x005AA388
			private static bool InTemple(Player player)
			{
				int num = (int)(player.position.X + (float)(player.width / 2)) / 16;
				int num2 = (int)(player.position.Y + (float)(player.height / 2)) / 16;
				return WorldGen.InWorld(num, num2, 0) && Main.tile[num, num2] != null && Main.tile[num, num2].wall == 87;
			}

			// Token: 0x06002FBC RID: 12220 RVA: 0x005AC1F8 File Offset: 0x005AA3F8
			private static bool InIce(Player player)
			{
				return player.ZoneSnow && !player.ZoneOverworldHeight;
			}

			// Token: 0x06002FBD RID: 12221 RVA: 0x005AC20D File Offset: 0x005AA40D
			private static bool InDesert(Player player)
			{
				return player.ZoneDesert && !player.ZoneOverworldHeight;
			}

			// Token: 0x04005A59 RID: 23129
			public static readonly ChromaCondition Hive = new CommonConditions.SimpleCondition((Player player) => player.ZoneHive);

			// Token: 0x04005A5A RID: 23130
			public static readonly ChromaCondition Jungle = new CommonConditions.SimpleCondition((Player player) => player.ZoneJungle && !player.ZoneOverworldHeight);

			// Token: 0x04005A5B RID: 23131
			public static readonly ChromaCondition Mushroom = new CommonConditions.SimpleCondition((Player player) => player.ZoneGlowshroom && !player.ZoneOverworldHeight);

			// Token: 0x04005A5C RID: 23132
			public static readonly ChromaCondition Ice = new CommonConditions.SimpleCondition(new Func<Player, bool>(CommonConditions.UndergroundBiome.InIce));

			// Token: 0x04005A5D RID: 23133
			public static readonly ChromaCondition HallowIce = new CommonConditions.SimpleCondition((Player player) => CommonConditions.UndergroundBiome.InIce(player) && player.ZoneHallow);

			// Token: 0x04005A5E RID: 23134
			public static readonly ChromaCondition CrimsonIce = new CommonConditions.SimpleCondition((Player player) => CommonConditions.UndergroundBiome.InIce(player) && player.ZoneCrimson);

			// Token: 0x04005A5F RID: 23135
			public static readonly ChromaCondition CorruptIce = new CommonConditions.SimpleCondition((Player player) => CommonConditions.UndergroundBiome.InIce(player) && player.ZoneCorrupt);

			// Token: 0x04005A60 RID: 23136
			public static readonly ChromaCondition Hallow = new CommonConditions.SimpleCondition((Player player) => player.ZoneHallow && !player.ZoneOverworldHeight);

			// Token: 0x04005A61 RID: 23137
			public static readonly ChromaCondition Crimson = new CommonConditions.SimpleCondition((Player player) => player.ZoneCrimson && !player.ZoneOverworldHeight);

			// Token: 0x04005A62 RID: 23138
			public static readonly ChromaCondition Corrupt = new CommonConditions.SimpleCondition((Player player) => player.ZoneCorrupt && !player.ZoneOverworldHeight);

			// Token: 0x04005A63 RID: 23139
			public static readonly ChromaCondition Desert = new CommonConditions.SimpleCondition(new Func<Player, bool>(CommonConditions.UndergroundBiome.InDesert));

			// Token: 0x04005A64 RID: 23140
			public static readonly ChromaCondition HallowDesert = new CommonConditions.SimpleCondition((Player player) => CommonConditions.UndergroundBiome.InDesert(player) && player.ZoneHallow);

			// Token: 0x04005A65 RID: 23141
			public static readonly ChromaCondition CrimsonDesert = new CommonConditions.SimpleCondition((Player player) => CommonConditions.UndergroundBiome.InDesert(player) && player.ZoneCrimson);

			// Token: 0x04005A66 RID: 23142
			public static readonly ChromaCondition CorruptDesert = new CommonConditions.SimpleCondition((Player player) => CommonConditions.UndergroundBiome.InDesert(player) && player.ZoneCorrupt);

			// Token: 0x04005A67 RID: 23143
			public static readonly ChromaCondition Temple = new CommonConditions.SimpleCondition(new Func<Player, bool>(CommonConditions.UndergroundBiome.InTemple));

			// Token: 0x04005A68 RID: 23144
			public static readonly ChromaCondition Dungeon = new CommonConditions.SimpleCondition((Player player) => player.ZoneDungeon);

			// Token: 0x04005A69 RID: 23145
			public static readonly ChromaCondition Marble = new CommonConditions.SimpleCondition((Player player) => player.ZoneMarble);

			// Token: 0x04005A6A RID: 23146
			public static readonly ChromaCondition Granite = new CommonConditions.SimpleCondition((Player player) => player.ZoneGranite);

			// Token: 0x04005A6B RID: 23147
			public static readonly ChromaCondition GemCave = new CommonConditions.SimpleCondition((Player player) => player.ZoneGemCave);
		}

		// Token: 0x020005CF RID: 1487
		public static class Boss
		{
			// Token: 0x04005A6C RID: 23148
			public static int HighestTierBossOrEvent;

			// Token: 0x04005A6D RID: 23149
			public static readonly ChromaCondition EaterOfWorlds = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 13);

			// Token: 0x04005A6E RID: 23150
			public static readonly ChromaCondition Destroyer = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 134);

			// Token: 0x04005A6F RID: 23151
			public static readonly ChromaCondition KingSlime = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 50);

			// Token: 0x04005A70 RID: 23152
			public static readonly ChromaCondition QueenSlime = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 657);

			// Token: 0x04005A71 RID: 23153
			public static readonly ChromaCondition BrainOfCthulhu = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 266);

			// Token: 0x04005A72 RID: 23154
			public static readonly ChromaCondition DukeFishron = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 370);

			// Token: 0x04005A73 RID: 23155
			public static readonly ChromaCondition QueenBee = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 222);

			// Token: 0x04005A74 RID: 23156
			public static readonly ChromaCondition Plantera = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 262);

			// Token: 0x04005A75 RID: 23157
			public static readonly ChromaCondition Empress = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 636);

			// Token: 0x04005A76 RID: 23158
			public static readonly ChromaCondition EyeOfCthulhu = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 4);

			// Token: 0x04005A77 RID: 23159
			public static readonly ChromaCondition TheTwins = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 126);

			// Token: 0x04005A78 RID: 23160
			public static readonly ChromaCondition MoonLord = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 398);

			// Token: 0x04005A79 RID: 23161
			public static readonly ChromaCondition WallOfFlesh = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 113);

			// Token: 0x04005A7A RID: 23162
			public static readonly ChromaCondition Golem = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 245);

			// Token: 0x04005A7B RID: 23163
			public static readonly ChromaCondition Cultist = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 439);

			// Token: 0x04005A7C RID: 23164
			public static readonly ChromaCondition Skeletron = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 35);

			// Token: 0x04005A7D RID: 23165
			public static readonly ChromaCondition SkeletronPrime = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == 127);
		}

		// Token: 0x020005D0 RID: 1488
		public static class Weather
		{
			// Token: 0x04005A7E RID: 23166
			public static readonly ChromaCondition Rain = new CommonConditions.SimpleCondition((Player player) => player.ZoneRain && !player.ZoneSnow && !player.ZoneSandstorm);

			// Token: 0x04005A7F RID: 23167
			public static readonly ChromaCondition Sandstorm = new CommonConditions.SimpleCondition((Player player) => player.ZoneSandstorm);

			// Token: 0x04005A80 RID: 23168
			public static readonly ChromaCondition Blizzard = new CommonConditions.SimpleCondition((Player player) => player.ZoneSnow && player.ZoneRain);

			// Token: 0x04005A81 RID: 23169
			public static readonly ChromaCondition SlimeRain = new CommonConditions.SimpleCondition((Player player) => Main.slimeRain && player.ZoneOverworldHeight);
		}

		// Token: 0x020005D1 RID: 1489
		public static class Depth
		{
			// Token: 0x06002FC1 RID: 12225 RVA: 0x005AC654 File Offset: 0x005AA854
			private static bool IsPlayerInFrontOfDirtWall(Player player)
			{
				Point point = player.Center.ToTileCoordinates();
				if (!WorldGen.InWorld(point.X, point.Y, 0))
				{
					return false;
				}
				if (Main.tile[point.X, point.Y] == null)
				{
					return false;
				}
				ushort wall = Main.tile[point.X, point.Y].wall;
				if (wall <= 61)
				{
					if (wall <= 16)
					{
						if (wall != 2 && wall != 16)
						{
							return false;
						}
					}
					else if (wall - 54 > 5 && wall != 61)
					{
						return false;
					}
				}
				else if (wall <= 185)
				{
					if (wall - 170 > 1 && wall != 185)
					{
						return false;
					}
				}
				else if (wall - 196 > 3 && wall - 212 > 3)
				{
					return false;
				}
				return true;
			}

			// Token: 0x04005A82 RID: 23170
			public static readonly ChromaCondition Sky = new CommonConditions.SimpleCondition((Player player) => (double)(player.position.Y / 16f) < Main.worldSurface * 0.44999998807907104);

			// Token: 0x04005A83 RID: 23171
			public static readonly ChromaCondition Surface = new CommonConditions.SimpleCondition((Player player) => player.ZoneOverworldHeight && (double)(player.position.Y / 16f) >= Main.worldSurface * 0.44999998807907104 && !CommonConditions.Depth.IsPlayerInFrontOfDirtWall(player));

			// Token: 0x04005A84 RID: 23172
			public static readonly ChromaCondition Vines = new CommonConditions.SimpleCondition((Player player) => player.ZoneOverworldHeight && (double)(player.position.Y / 16f) >= Main.worldSurface * 0.44999998807907104 && CommonConditions.Depth.IsPlayerInFrontOfDirtWall(player));

			// Token: 0x04005A85 RID: 23173
			public static readonly ChromaCondition Underground = new CommonConditions.SimpleCondition((Player player) => player.ZoneDirtLayerHeight);

			// Token: 0x04005A86 RID: 23174
			public static readonly ChromaCondition Caverns = new CommonConditions.SimpleCondition((Player player) => player.ZoneRockLayerHeight && player.position.ToTileCoordinates().Y <= Main.maxTilesY - 400);

			// Token: 0x04005A87 RID: 23175
			public static readonly ChromaCondition Magma = new CommonConditions.SimpleCondition((Player player) => player.ZoneRockLayerHeight && player.position.ToTileCoordinates().Y > Main.maxTilesY - 400);

			// Token: 0x04005A88 RID: 23176
			public static readonly ChromaCondition Underworld = new CommonConditions.SimpleCondition((Player player) => player.ZoneUnderworldHeight);
		}

		// Token: 0x020005D2 RID: 1490
		public static class Events
		{
			// Token: 0x04005A89 RID: 23177
			public static readonly ChromaCondition BloodMoon = new CommonConditions.SimpleCondition((Player player) => Main.bloodMoon && !Main.snowMoon && !Main.pumpkinMoon);

			// Token: 0x04005A8A RID: 23178
			public static readonly ChromaCondition FrostMoon = new CommonConditions.SimpleCondition((Player player) => Main.snowMoon);

			// Token: 0x04005A8B RID: 23179
			public static readonly ChromaCondition PumpkinMoon = new CommonConditions.SimpleCondition((Player player) => Main.pumpkinMoon);

			// Token: 0x04005A8C RID: 23180
			public static readonly ChromaCondition SolarEclipse = new CommonConditions.SimpleCondition((Player player) => Main.eclipse);

			// Token: 0x04005A8D RID: 23181
			public static readonly ChromaCondition SolarPillar = new CommonConditions.SimpleCondition((Player player) => player.ZoneTowerSolar);

			// Token: 0x04005A8E RID: 23182
			public static readonly ChromaCondition NebulaPillar = new CommonConditions.SimpleCondition((Player player) => player.ZoneTowerNebula);

			// Token: 0x04005A8F RID: 23183
			public static readonly ChromaCondition VortexPillar = new CommonConditions.SimpleCondition((Player player) => player.ZoneTowerVortex);

			// Token: 0x04005A90 RID: 23184
			public static readonly ChromaCondition StardustPillar = new CommonConditions.SimpleCondition((Player player) => player.ZoneTowerStardust);

			// Token: 0x04005A91 RID: 23185
			public static readonly ChromaCondition PirateInvasion = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == -3);

			// Token: 0x04005A92 RID: 23186
			public static readonly ChromaCondition DD2Event = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == -6);

			// Token: 0x04005A93 RID: 23187
			public static readonly ChromaCondition FrostLegion = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == -2);

			// Token: 0x04005A94 RID: 23188
			public static readonly ChromaCondition MartianMadness = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == -4);

			// Token: 0x04005A95 RID: 23189
			public static readonly ChromaCondition GoblinArmy = new CommonConditions.SimpleCondition((Player player) => CommonConditions.Boss.HighestTierBossOrEvent == -1);
		}

		// Token: 0x020005D3 RID: 1491
		public static class Alert
		{
			// Token: 0x04005A96 RID: 23190
			public static readonly ChromaCondition MoonlordComing = new CommonConditions.SimpleCondition((Player player) => NPC.MoonLordCountdown > 0);

			// Token: 0x04005A97 RID: 23191
			public static readonly ChromaCondition Drowning = new CommonConditions.SimpleCondition((Player player) => player.breath != player.breathMax);

			// Token: 0x04005A98 RID: 23192
			public static readonly ChromaCondition Keybinds = new CommonConditions.SimpleCondition((Player player) => Main.InGameUI.CurrentState == Main.ManageControlsMenu || Main.MenuUI.CurrentState == Main.ManageControlsMenu);

			// Token: 0x04005A99 RID: 23193
			public static readonly ChromaCondition LavaIndicator = new CommonConditions.SimpleCondition((Player player) => player.lavaWet);
		}

		// Token: 0x020005D4 RID: 1492
		public static class CriticalAlert
		{
			// Token: 0x04005A9A RID: 23194
			public static readonly ChromaCondition LowLife = new CommonConditions.SimpleCondition((Player player) => Main.ChromaPainter.PotionAlert);

			// Token: 0x04005A9B RID: 23195
			public static readonly ChromaCondition Death = new CommonConditions.SimpleCondition((Player player) => player.dead);
		}
	}
}
