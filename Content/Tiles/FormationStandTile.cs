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
				XSize = XSize == 3 ? 1 : XSize + 1;
				YSize = YSize == 3 ? 1 : YSize + 1;
				XSize2 = 3 + (2 * XSize);
				YSize2 = 3 + (2 * YSize);
				X = i - (tile.TileFrameX / 18) + 1 - XSize;

				for (int k = 1; k < XSize2 - 1; k++)
                {
					for (int l = 1; l < YSize2 - 1; l++)
					{
						WorldGen.KillWall(X + k, Y - l);
						WorldGen.PlaceWall(X + k, Y - l, ModContent.WallType<IndicatorWhite>());
                    }
                }
				for (int k = 0; k < XSize2; k++)
				{
					WorldGen.KillWall(X + k, Y);
					WorldGen.PlaceWall(X + k, Y, ModContent.WallType<IndicatorBlue>());
					WorldGen.KillWall(X + k, Y - YSize2 + 1);
					WorldGen.PlaceWall(X + k, Y - YSize2 + 1, ModContent.WallType<IndicatorBlue>());
				}
				for (int l = 0; l < YSize2; l++)
                {
					WorldGen.KillWall(X, Y - l);
					WorldGen.PlaceWall(X, Y - l, ModContent.WallType<IndicatorBlue>());
					WorldGen.KillWall(X + XSize2 - 1, Y - l);
					WorldGen.PlaceWall(X + XSize2 - 1, Y - l, ModContent.WallType<IndicatorBlue>());
				}

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
	}
}
