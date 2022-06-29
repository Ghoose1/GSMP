using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using Terraria.ID;

namespace GSMP.Content.TileEntities
{
    public class ManaMagnetEntity : ModTileEntity
    {

        public int MaxMana; // MaxMana and TransferRate are set by the tile creating the entity and not modified after
        public int TransferRate; // Mana per tick from this

        public int StoredMana;
        public List<Vector2> ConnectionsTo = new(); // Links between these entities 
        public List<Vector2> ConnectionsFrom = new();

        public override bool IsTileValidForEntity(int x, int y)
        {
            return ManaTEutils.ValidTiles.Contains(Main.tile[x, y].TileType) && Main.tile[x, y].HasTile;
        }

        public override void Update() // Used to check if Tiles this tile is connected to are still active and to transfer mana to linked Entities
        {
            for (int k = 0; k < ConnectionsTo.Count; k++)
            {
                Tile tile = Main.tile[(int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y];
                if (tile == null || !tile.HasTile || !ManaTEutils.ValidTiles.Contains(tile.TileType))
                    ConnectionsTo.RemoveAt(k);
            }

            for (int k = 0; k < ConnectionsFrom.Count; k++)
            {
                Tile tile = Main.tile[(int)ConnectionsFrom[k].X, (int)ConnectionsFrom[k].Y];
                if (tile == null || !tile.HasTile || !ManaTEutils.ValidTiles.Contains(tile.TileType))
                    ConnectionsFrom.RemoveAt(k);
            }

            foreach (Item item in Main.item)
            {
                if (ManaTEutils.ManaItems.Contains(item.type))
                {
                    //Main.NewText($"Distance: {item.position.DistanceSQ(new Vector2(Position.X * 16, Position.Y * 16))}");
                    //Main.NewText($"Item X: {item.position.X} Y: {item.position.Y} | This X: {Position.X * 16} Y: {Position.Y * 16} | Distance: {item.position.DistanceSQ(new Vector2(Position.X * 16, Position.Y * 16))}");
                    if (item.position.DistanceSQ(new Vector2(Position.X * 16 + 12, Position.Y * 16 + 12)) < 6144)
                    {
                        Main.NewText("Test2");
                        if (item.type == ItemID.Star || item.type == ItemID.SugarPlum || item.type == ItemID.SoulCake)
                            TransferMana(100);
                        else if (item.type == ModContent.ItemType<Items.Magic.ManaStar>())
                            if (item.ModItem is Items.Magic.ManaStar star)
                            {
                                Main.NewText("test3");
                                TransferMana(star.Mana);
                            }
                        item.TurnToAir();
                    }
                }
            }
        }

        public void TransferMana(int num)
        {
            int amount = (int)Math.Floor(num / (float)ConnectionsTo.Count);
            if (amount * ConnectionsTo.Count > num) return;
            //Main.NewText(amount.ToString() + " | " + (TransferRate / ConnectionsTo.Count).ToString());

            for (int k = 0; k < ConnectionsTo.Count; k++)
            {
                if (!(ManaTEutils.MaxMana((int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y) < ManaTEutils.Mana((int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y) + amount))
                {
                    ManaTEutils.Mana((int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y, amount);
                    StoredMana -= amount;
                }
            }

        }
    }
}
