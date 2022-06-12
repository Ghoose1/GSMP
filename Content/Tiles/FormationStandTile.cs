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
				if ((tile.TileFrameY / 18) == 0)
                {
					if ((tile.TileFrameX / 18) == 0)
					{
						XSize++;
						XSize2 += 2;
						Main.NewText("X size: " + XSize2.ToString());
					}
					if ((tile.TileFrameX / 18) == 1 && XSize > 1)
					{
						XSize--;
						XSize2 -= 2;
						Main.NewText("X size: " + XSize2.ToString());
					}
					if ((tile.TileFrameX / 18) == 3)
					{
						YSize++;
						YSize2 += 2;
						Main.NewText("Y size: " + YSize2.ToString());
					}
					if ((tile.TileFrameX / 18) == 4 && YSize > 1)
					{
						YSize--;
						YSize2 -= 2;
						Main.NewText("Y size: " + YSize2.ToString());
					}
					if ((tile.TileFrameX / 18) == 2)
                    {
						XSize = 1;
						XSize2 = 5;
						YSize = 1;
						YSize2 = 5;
						Main.NewText("X size: " + XSize2.ToString());
						Main.NewText("Y size: " + YSize2.ToString());
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

			if (player.Distance(
					new Vector2((i + 2 - (tile.TileFrameX / 18)) * 16 + 4,
					(j + 2 - (tile.TileFrameY / 18)) * 16 - 64 - 16 * (XSize2 > YSize2 ? XSize2 / 2 : YSize2 / 2)))
					< 256 + 16 * (XSize2 > YSize2 ? XSize2 : YSize2))
			{
				for (int angle = 0; angle <= 360; angle++)
					CreateAura((i + 2 - (tile.TileFrameX / 18)) * 16 + 4,
							   (j + 2 - (tile.TileFrameY / 18)) * 16 - 64 - 16 * (XSize2 > YSize2 ? XSize2 / 2 : YSize2 / 2),
							   128 + 16 * (XSize2 > YSize2 ? XSize2 : YSize2), angle);

				if (player.Distance(
					new Vector2((i + 2 - (tile.TileFrameX / 18)) * 16 + 4,
					(j + 2 - (tile.TileFrameY / 18)) * 16 - 64 - 16 * (XSize2 > YSize2 ? XSize2 / 2 : YSize2 / 2)))
					< 128 + 16 * (XSize2 > YSize2 ? XSize2 : YSize2))
				{
					Vector2 pos;
					int dust;
					int X2 = (X * 16) + 4; // Middle of blocks instead of top-right corners
					int Y2 = (Y * 16) + 4;

					for (int k = 0; k < XSize2 * 16; k += 16)
					{
						if (Main.rand.Next(6) == 0)
						{
							// Bottom Row
							pos = new Vector2(X2 + k - 6, Y2 + 12);
							dust = Dust.NewDust(pos, 16, 0, 173, 0, 0, 0);
							Main.dust[dust].velocity.Y *= 0.1f;
							Main.dust[dust].velocity.X *= 0.1f;

							// Top Row
							pos = new Vector2(X2 + k - 6, Y2 + 12 - (YSize2 * 16));
							dust = Dust.NewDust(pos, 16, 0, 173, 0, 0, 0);
							Main.dust[dust].velocity.Y *= 0.1f;
							Main.dust[dust].velocity.X *= 0.1f;
						}
					}

					for (int l = 0; l < YSize2 * 16; l += 16)
					{
						if (Main.rand.Next(6) == 0)
						{
							// Left 
							pos = new Vector2(X2 - 8, Y2 - l);
							dust = Dust.NewDust(pos, 0, 16, 173, 0, 0, 0);
							Main.dust[dust].velocity.Y *= 0.1f;
							Main.dust[dust].velocity.X *= 0.1f;

							// Right
							pos = new Vector2(X2 + XSize2 * 16 - 8, Y2 - l);
							dust = Dust.NewDust(pos, 0, 16, 173, 0, 0, 0);
							Main.dust[dust].velocity.Y *= 0.1f;
							Main.dust[dust].velocity.X *= 0.1f;
						}
					}
				}
			}
		}

        public void CreateAura(int x, int y, int dist, int angle)
        {
			Vector2 Center = new Vector2(x, y);
			if (Main.rand.Next(100) == 0)
			{
				Vector2 vector2 = new Vector2((float)(Center.X + Math.Cos(angle * (Math.PI / 180f)) * dist),
												(float)(Center.Y + Math.Sin(angle * (Math.PI / 180f)) * dist));
				int dust = Dust.NewDust(vector2, 0, 0, 173, 0, 0, 0);
				Main.dust[dust].velocity.Y *= 0.1f;
				Main.dust[dust].velocity.X *= 0.1f;
			}
		}
    }
}
