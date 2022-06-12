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
using Terraria.Localization;
using Terraria.ObjectData;
using GSMP.Content.Items;
using Microsoft.Xna.Framework.Graphics;

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

				int[,] Formation = new int[YSize2, XSize2];

				int searchX;
				int searchY;
				bool originFound = false;

				for (searchX = 0; searchX < XSize2; searchX++)
                {
					for (searchY = 0; searchY < YSize2; searchY++)
                    {
						if (Main.tile[X + searchX, Y - searchY].TileType == TileID.Stone)
						{
							originFound = true;
							Formation[YSize2 - 1 - searchY, searchX] = 2;
						} 
                    }
                }

				if (originFound)
				{
					for (searchX = 0; searchX < XSize2; searchX++)
					{
						for (searchY = 0; searchY < YSize2; searchY++)
						{
							if (Main.tile[X + searchX, Y - searchY].HasTile && Formation[YSize2 - 1 - searchY, searchX] != 2) Formation[YSize2 - 1 - searchY, searchX] = 1; // MultiType will require this to be changed
						}
					}

					item.CustomFormation = Formation;
				}
				else Main.NewText("No Origin Tile");
			}
			else
            {
				//Main.NewText("i: " + (tile.TileFrameX / 18).ToString());
				//Main.NewText("j: " + (tile.TileFrameY / 18).ToString());
				//int i2 = i - (i - (tile.TileFrameX / 18));
				if ((tile.TileFrameY / 18) == 0)
                {
					if ((tile.TileFrameX / 18) == 0) XSize++;
					if ((tile.TileFrameX / 18) == 1 && XSize > 1) XSize--;
					if ((tile.TileFrameX / 18) == 3) YSize++;
					if ((tile.TileFrameX / 18) == 4 && YSize > 1) YSize--;
				}

				XSize2 = 3 + (2 * XSize);
				YSize2 = 3 + (2 * YSize);
				X = i - (tile.TileFrameX / 18) + 1 - XSize;

				//for (int k = 1; k < XSize2 - 1; k++)
    //            {
				//	for (int l = 1; l < YSize2 - 1; l++)
				//	{
				//		WorldGen.KillWall(X + k, Y - l);
				//		WorldGen.PlaceWall(X + k, Y - l, ModContent.WallType<IndicatorWhite>());
    //                }
    //            }
				//for (int k = 0; k < XSize2; k++)
				//{
				//	WorldGen.KillWall(X + k, Y);
				//	WorldGen.PlaceWall(X + k, Y, ModContent.WallType<IndicatorBlue>());
				//	WorldGen.KillWall(X + k, Y - YSize2 + 1);
				//	WorldGen.PlaceWall(X + k, Y - YSize2 + 1, ModContent.WallType<IndicatorBlue>());
				//}
				//for (int l = 0; l < YSize2; l++)
    //            {
				//	WorldGen.KillWall(X, Y - l);
				//	WorldGen.PlaceWall(X, Y - l, ModContent.WallType<IndicatorBlue>());
				//	WorldGen.KillWall(X + XSize2 - 1, Y - l);
				//	WorldGen.PlaceWall(X + XSize2 - 1, Y - l, ModContent.WallType<IndicatorBlue>());
				//}

				Main.NewText("X size: " + XSize2.ToString());

				Main.NewText("Y size: " + YSize2.ToString());

				//WorldGen.PlaceTile(i - 5, j - 5, TileID.DiamondGemspark);
				//WorldGen.PlaceWall(X, Y, WallID.DiamondGemspark);
				//WorldGen.PlaceWall(X + 3 + Size, Y, WallID.DiamondGemspark);
				//WorldGen.PlaceWall(X, Y - 2 - (2 * Size), WallID.DiamondGemspark);
				//WorldGen.PlaceWall(X + 3 + Size, Y - 2 - (2 * Size), WallID.DiamondGemspark);
				//item.UpdateStats(item);
			}

			return true;
		}

        public override void NearbyEffects(int i, int j, bool closer)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int X = i - (tile.TileFrameX / 18) + 2 - XSize;
			int Y = j - (tile.TileFrameY / 18) - 1;
			int XSize2 = 3 + (2 * XSize);
			int YSize2 = 3 + (2 * YSize);

			Vector2 tilepos = new Vector2(X * 16, Y * 16);
			if (closer && player.Distance(tilepos) < 1280)
			{
				Vector2 pos;
				int dust; // = Dust.NewDust(tilepos, 4, 4, 173, 0, 0, 0);
				//            for (int k = 1; k < XSize2 - 1; k++)
				//{
				//	for (int l = 1; l < YSize2 - 1; l++)
				//	{
				//		int dust = Dust.NewDust(tilepos, 4, 4, 173, 0, 0, 0);
				//		Main.dust[dust].position.Y = (Y - l) * 16;
				//		Main.dust[dust].position.X = (X + k) * 16;
				//	}
				//            }

				for (int k = 0; k < XSize2; k++)
                {
					if (Main.rand.Next(6) == 0)
					{
						pos = new Vector2((X + k) * 16 - 16 - 4, (Y) * 16 + 16 - 4);
						dust = Dust.NewDust(pos, 16, 0, 173, 0, 0, 0);
						//Main.dust[dust].position.Y = (Y) * 16;
						//Main.dust[dust].position.X = (X + k) * 16;
						Main.dust[dust].velocity.Y *= 0.1f;
						Main.dust[dust].velocity.X *= 0.1f;

						pos = new Vector2((X + k) * 16 - 16 - 4, (Y - YSize2 + 1) * 16 - 4);
						dust = Dust.NewDust(pos, 16, 0, 173, 0, 0, 0);
						//Main.dust[dust].position.Y = (Y - YSize2 + 1) * 16;
						//Main.dust[dust].position.X = (X + k) * 16;
						Main.dust[dust].velocity.Y *= 0.1f;
						Main.dust[dust].velocity.X *= 0.1f;
					}
                }
                for (int l = 0; l < YSize2; l++)
				{
					if (Main.rand.Next(6) == 0)
					{
						pos = new Vector2((X) * 16 - 16 - 4, (Y - l) * 16);
						dust = Dust.NewDust(pos, 0, 16, 173, 0, 0, 0);
						//Main.dust[dust].position.Y = (Y - l) * 16;
						//Main.dust[dust].position.X = (X) * 16;
						Main.dust[dust].velocity.Y *= 0.1f;
						Main.dust[dust].velocity.X *= 0.1f;

						pos = new Vector2((X + XSize2 - 1) * 16 - 4, (Y - l) * 16);
						dust = Dust.NewDust(pos, 0, 16, 173, 0, 0, 0);
						//Main.dust[dust].position.Y = (Y - l) * 16;
						//Main.dust[dust].position.X = (X + XSize2 - 1) * 16;
						Main.dust[dust].velocity.Y *= 0.1f;
						Main.dust[dust].velocity.X *= 0.1f;
					}
                }

            }
		}

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            base.DrawEffects(i, j, spriteBatch, ref drawData);
        }
    }

	public class IndicatorDust : ModDust
    {
        public override string Texture => "GSMP/Assets/IndicatorDust";

        public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true; // Makes the dust have no gravity.
			dust.noLight = false; // Makes the dust emit no light.
								  //dust.scale *= 1.5f; // Multiplies the dust's initial scale by 1.5.
			
			dust.velocity *= 0.2f;
		}

		public override bool Update(Dust dust)
		{
			//dust.position += dust.velocity;
			//dust.rotation += dust.velocity.X * 0.15f;
			//dust.scale *= 0.99f;

			//float light = 0.35f * dust.scale;

			//Lighting.AddLight(dust.position, light, light, light);

			//if (dust.scale < 0.5f)
			//{
			//	dust.active = false;
			//}

			return false; 
		}
	}
}
