using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using GSMP.Content.UI;
using Terraria.UI;
using System;
using Terraria.DataStructures;

namespace GSMP.Content.Tiles
{
    public class Candle : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.addTile(Type);

			AddMapEntry(Color.SkyBlue, Language.GetText("MapObject.Torch")); 
		}

        public override void HitWire(int i, int j)
        {
			Tile tile = Main.tile[i, j];
			short frameAdjustment = (short)(tile.TileFrameX > 0 ? -18 : 18);
			NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None);
		}

        public override bool Drop(int i, int j)
		{
			Tile t = Main.tile[i, j];
			int style = t.TileFrameX / 18;
			if (style == 0) 
			{
				Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<Items.Placeable.CandleItem>());
			}

			return base.Drop(i, j);
		}
		public int CheckAdjacent(int i, int j)
        {
			int num = 0;
			int[] adjacent =
			{
				Main.tile[i + 1, j].TileType,
				Main.tile[i + 1, j + 1].TileType,
				Main.tile[i + 1, j - 1].TileType,
				Main.tile[i, j + 1].TileType,
				Main.tile[i, j - 1].TileType,
				Main.tile[i - 1, j].TileType,
				Main.tile[i - 1, j + 1].TileType,
				Main.tile[i - 1, j - 1].TileType
			};

			for (int k = 0; k < adjacent.Length; k++)
				if (adjacent[k] == ModContent.TileType<Candle>())
					num++;
			return num;
		}
        public override void NearbyEffects(int i, int j, bool closer)
        {
			Player player = Main.LocalPlayer;
			player.AddBuff(BuffID.Regeneration, 1);
			//if (closer)
			//{
			//	Player player = Main.LocalPlayer;
			//	player.AddBuff(BuffID.Regeneration, 1);
			//}
			//if (closer && CheckAdjacent(i, j) > 0)
			//{
			//}
		}
	}
}