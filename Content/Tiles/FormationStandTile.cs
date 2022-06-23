using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using GSMP.DataStructures;
using Terraria.ObjectData;
using GSMP.Content.Items;
using Microsoft.Xna.Framework.Graphics;
using GSMP.Content.Buffs;
using GSMP;

namespace GSMP.Content.Tiles
{
    public class FormationStandTile : ModTile
    {
		public override string Texture => "GSMP/Assets/FormationStand";

		internal int YSize;
		internal int XSize;

        public override void SetStaticDefaults()
		{
			// Properties
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			//TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4); 
			TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
			TileObjectData.addTile(Type);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Formation Stand");
			AddMapEntry(new Color(200, 200, 200), name);
			
			XSize = 1;
			YSize = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 32, ModContent.ItemType<Items.Placeable.FormationStandItem>());
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int X = i - (tile.TileFrameX / 18) + 1 - XSize;
			int Y = j - (tile.TileFrameY / 18) - 1;
			int XSize2 = 3 + (2 * XSize);
			int YSize2 = 3 + (2 * YSize);

			if (player.HeldItem.ModItem is BaseMagicItem item)
			{
				// Y-4 [ ] [ ] [1] [ ] [ ]
				// Y-3 [ ] [ ] [1] [ ] [ ]
				// Y-2 [1] [1] [2] [1] [1]
				// Y-1 [ ] [ ] [1] [ ] [ ]
				// Y   [ ] [ ] [1] [ ] [ ]
				//     X   X+1 X+2 X+3 X+4

				Spell[,] Formation = new Spell[YSize2, XSize2];

				int searchX;
				int searchY;
				bool originFound = false;

				for (searchX = 0; searchX < XSize2; searchX++)
                {
					for (searchY = 0; searchY < YSize2; searchY++)
                    {
						Tile tile_ = Main.tile[X + searchX, Y - searchY];
						if (tile_.HasTile && ModContent.GetModTile(tile_.TileType) is SpellTile spellTile)
						{
							Spell spell = SpellTile.spell(X + searchX, Y - searchY);
							if (!spell.isFormationSlave && !(spell.Type == "Error Spell"))
							{
								//Main.NewText("2");
								originFound = true;
								//spell.usesFormation = false;
								item.Castspell = spell;
								//Formation[YSize2 - 1 - searchY, searchX] = spell;
							}
							//else Main.NewText("1");
						}
						//else Main.NewText("0");

					}
                }

				//Main.NewText(originFound.ToString());
				if (originFound)
				{
					for (searchX = 0; searchX < XSize2; searchX++)
					{
						for (searchY = 0; searchY < YSize2; searchY++)
						{
							Tile tile_ = Main.tile[X + searchX, Y - searchY];
							if (tile_.HasTile && tile_.TileType == ModContent.TileType<SpellTile>())// && ModContent.GetModTile(tile_.TileType) == SpellTile spellTile)
							{
								ModTile modTile = ModContent.GetModTile(tile_.TileType);
								Spell spell = SpellTile.spell(X + searchX, Y - searchY);
								spell.usesFormation = false; //paranoia
								Formation[YSize2 - 1 - searchY, searchX] = spell;
							}
							else Formation[YSize2 - 1 - searchY, searchX] = new Spell("Blank");
						}
					}
					item.Castspell.projStats[2] = 1;

					item.Castspell.usesFormation = true;
					item.Castspell.formation = Formation;
				}
				else Main.NewText("No Origin Tile");
			}
			else
            {
				if ((tile.TileFrameY / 18) == 0)
                {
                    switch ((tile.TileFrameX / 18))
                    {
						case 0:
							XSize++;
							XSize2 += 2;
							Main.NewText("X size: " + XSize2.ToString());
							break;
						case 1:
							if (XSize > 1)
							{
								XSize--;
								XSize2 -= 2;
								Main.NewText("X size: " + XSize2.ToString());
							}
							break;
						case 2:
							XSize = 1;
							XSize2 = 5;
							YSize = 1;
							YSize2 = 5;
							Main.NewText("X size: " + XSize2.ToString());
							Main.NewText("Y size: " + YSize2.ToString());
							break;
						case 3:
							YSize++;
							YSize2 += 2;
							Main.NewText("Y size: " + YSize2.ToString());
							break;
						case 4:
							if (YSize > 1)
							{
								YSize--;
								YSize2 -= 2;
								Main.NewText("Y size: " + YSize2.ToString());
							}
							break;
						default:
                            break;
                    }
				}
			}
			return true;
		}

        public override void NearbyEffects(int i, int j, bool closer)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int X = i - (tile.TileFrameX / 18) + 1 - XSize;
			int Y = j - (tile.TileFrameY / 18) - 1;
			int XSize2 = 3 + (2 * XSize);
			int YSize2 = 3 + (2 * YSize);
			Vector2 tilepos = new Vector2(X * 16, Y * 16);
			CreateAura((i + 2 - (tile.TileFrameX / 18)) * 16 + 4,
						   (j + 2 - (tile.TileFrameY / 18)) * 16 - 64 - 16 * (XSize2 > YSize2 ? XSize2 / 2 : YSize2 / 2),
						   128 + 16 * (XSize2 > YSize2 ? XSize2 : YSize2));

			if (player.DistanceSQ(
					new Vector2((i + 2 - (tile.TileFrameX / 18)) * 16 + 4,
					(j + 2 - (tile.TileFrameY / 18)) * 16 - 64 - 16 * (XSize2 > YSize2 ? XSize2 / 2 : YSize2 / 2)))
					< 65535 + 256 * Math.Pow(XSize2 > YSize2 ? XSize2 : YSize2, 2)) // Player is in range to bother rendering the rectangle
			{

				if (player.DistanceSQ(
					new Vector2((i + 2 - (tile.TileFrameX / 18)) * 16 + 4,
					(j + 2 - (tile.TileFrameY / 18)) * 16 - 64 - 16 * (XSize2 > YSize2 ? XSize2 / 2 : YSize2 / 2)))
					< 32768 + 256 * Math.Pow(XSize2 > YSize2 ? XSize2 : YSize2, 2))
				{
					player.AddBuff(ModContent.BuffType<PlaceableMagic>(), 10);
					Vector2 pos;
					int dust;
					int X2 = (X * 16) + 4; // Middle of blocks instead of top-right corners
					int Y2 = (Y * 16) + 4;

					for (int k = 0; k < XSize2 * 16; k += 16)
					{
						if (Main.rand.NextBool(10))
						{
							// Bottom Row
							pos = new Vector2(X2 + k - 6, Y2 + 12);
							dust = Dust.NewDust(pos, 18, 0, 173, 0, 0, 0);
							Main.dust[dust].velocity.Y *= 0.1f;
							Main.dust[dust].velocity.X *= 0.1f;

							// Top Row
							pos = new Vector2(X2 + k - 6, Y2 + 12 - (YSize2 * 16));
							dust = Dust.NewDust(pos, 18, 0, 173, 0, 0, 0);
							Main.dust[dust].velocity.Y *= 0.1f;
							Main.dust[dust].velocity.X *= 0.1f;
						}
					}

					for (int l = 0; l < YSize2 * 16; l += 16)
					{
						if (Main.rand.NextBool(10))
						{
							// Left 
							pos = new Vector2(X2 - 8, Y2 - l);
							dust = Dust.NewDust(pos, 0, 18, 173, 0, 0, 0);
							Main.dust[dust].velocity.Y *= 0.1f;
							Main.dust[dust].velocity.X *= 0.1f;

							// Right
							pos = new Vector2(X2 + XSize2 * 16 - 8, Y2 - l);
							dust = Dust.NewDust(pos, 0, 18, 173, 0, 0, 0);
							Main.dust[dust].velocity.Y *= 0.1f;
							Main.dust[dust].velocity.X *= 0.1f;
						}
					}
				}
			}
		}

        public void CreateAura(int x, int y, int dist)
        {
			Vector2 Center = new Vector2(x, y);

			int dustMax = dist / 500 * 10;
			if (dustMax < 10) dustMax = 10;
			else if (dustMax > 40) dustMax = 40;

			for (int i = 0; i < dustMax; i++)
			{
				Vector2 vector2 = Center + Main.rand.NextVector2CircularEdge(dist, dist);
				Vector2 offset = vector2 - Main.LocalPlayer.Center;
				if (Math.Abs(offset.X) > Main.screenWidth * 0.6f || Math.Abs(offset.Y) > Main.screenHeight * 0.6f) continue;
				Dust dust = Main.dust[Dust.NewDust(vector2, 0, 0, 173, 0, 0, 0)];
				dust.velocity.Y *= 0.1f;
				dust.velocity.X *= 0.1f;

				dust.noGravity = true;
			}
		}
    }
}
