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

		internal int Size;

        public override void SetStaticDefaults()
		{
			// Properties
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			//TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.IsValidSpawnPoint[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4); // this style already takes care of direction for us
			//TileObjectData.newTile.CoordinateHeights = new[] { 16, 18, 32, 34 };
			TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
			TileObjectData.addTile(Type);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Formation Stand");
			AddMapEntry(new Color(200, 200, 200), name);
			Size = 1;
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return true;
		}

		public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
		{
			// Because beds have special smart interaction, this splits up the left and right side into the necessary 2x2 sections
			width = 2; // Default to the Width defined for TileObjectData.newTile
			height = 2; // Default to the Height defined for TileObjectData.newTile
						//extraY = 0; // Depends on how you set up frameHeight and CoordinateHeights and CoordinatePaddingFix.Y
		}

		//public override void NumDust(int i, int j, bool fail, ref int num)
		//{
		//	num = 1;
		//}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 32, ModContent.ItemType<Items.Placeable.FormationStandItem>());
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int X = i - (tile.TileFrameX / 18) + 1 - Size;
			int Y = j - (tile.TileFrameY / 18) - 1;
			int Size2 = 3 + (2 * Size);

			if (player.HeldItem.ModItem is BaseMagicItem item)
			{
				// Y-4 [ ] [ ] [1] [ ] [ ]
				// Y-3 [ ] [ ] [1] [ ] [ ]
				// Y-2 [1] [1] [2] [1] [1]
				// Y-1 [ ] [ ] [1] [ ] [ ]
				// Y   [ ] [ ] [1] [ ] [ ]
				//     X   X+1 X+2 X+3 X+4

				int[,] Formation = new int[Size2, Size2];

				int searchX;
				int searchY;
				bool originFound = false;

				for (searchX = 0; searchX < Size2; searchX++)
                {
					for (searchY = 0; searchY < Size2; searchY++)
                    {
						if (Main.tile[X + searchX, Y - searchY].TileType == TileID.Stone)
						{
							originFound = true;
							Formation[Size2 - 1 - searchY, searchX] = 2;
						} 
                    }
                }

				if (originFound)
				{
					for (searchX = 0; searchX < Size2; searchX++)
					{
						for (searchY = 0; searchY < Size2; searchY++)
						{
							if (Main.tile[X + searchX, Y - searchY].HasTile && Formation[Size2 - 1 - searchY, searchX] != 2) Formation[Size2 - 1 - searchY, searchX] = 1; // MultiType will require this to be changed
						}
					}

					item.CustomFormation = Formation;
				}
				else Main.NewText("No Origin Tile");
			}
			else
            {
				Size = Size == 3 ? 1 : Size + 1;
				Size2 = 3 + (2 * Size);
				X = i - (tile.TileFrameX / 18) + 1 - Size;

				for (int k = 1; k < Size2 - 1; k++)
                {
					for (int l = 1; l < Size2 - 1; l++)
					{
						WorldGen.KillWall(X + k, Y - l);
						WorldGen.PlaceWall(X + k, Y - l, ModContent.WallType<IndicatorWhite>());
                    }
                }
				for (int k = 0; k < Size2; k++)
				{
					WorldGen.KillWall(X + k, Y);
					WorldGen.PlaceWall(X + k, Y, ModContent.WallType<IndicatorBlue>());
					WorldGen.KillWall(X + k, Y - Size2 + 1);
					WorldGen.PlaceWall(X + k, Y - Size2 + 1, ModContent.WallType<IndicatorBlue>());

					WorldGen.KillWall(X, Y - k);
					WorldGen.PlaceWall(X, Y - k, ModContent.WallType<IndicatorBlue>());
					WorldGen.KillWall(X + Size2 - 1, Y - k);
					WorldGen.PlaceWall(X + Size2 - 1, Y - k, ModContent.WallType<IndicatorBlue>());
				}

				Main.NewText("size " + Size2.ToString());

				//WorldGen.PlaceTile(i - 5, j - 5, TileID.DiamondGemspark);
				//WorldGen.PlaceWall(X, Y, WallID.DiamondGemspark);
				//WorldGen.PlaceWall(X + 3 + Size, Y, WallID.DiamondGemspark);
				//WorldGen.PlaceWall(X, Y - 2 - (2 * Size), WallID.DiamondGemspark);
				//WorldGen.PlaceWall(X + 3 + Size, Y - 2 - (2 * Size), WallID.DiamondGemspark);
				//item.UpdateStats(item);
			}

			return true;
		}

		//public override void MouseOver(int i, int j)
		//{
		//	Player player = Main.LocalPlayer;

		//	if (!Player.IsHoveringOverABottomSideOfABed(i, j))
		//	{
		//		if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
		//		{ // Match condition in RightClick. Interaction should only show if clicking it does something
		//			player.noThrow = 2;
		//			player.cursorItemIconEnabled = true;
		//			player.cursorItemIconID = ItemID.SleepingIcon;
		//		}
		//	}
		//	else
		//	{
		//		player.noThrow = 2;
		//		player.cursorItemIconEnabled = true;
		//		player.cursorItemIconID = ModContent.ItemType<Items.Placeable.FormationStandItem>();
		//	}
		//}
	}
}
