using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GSMP.Content.TileEntities
{
    public class ManaMagnetEntity : ManaStorageEntity
    {
        public override void Update()
        {
            // Making sure linked tiles are active and removing the link if not
            for (int k = 0; k < ConnectionsTo.Count; k++)
            {
                Tile tile = Main.tile[(int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y];
                if (tile == null || !tile.HasTile || !ManaTEutils.IsConnectionValid(tile.TileType))
                    ConnectionsTo.RemoveAt(k);
            }

            foreach (Item item in Main.item)
            {
                if (ManaTEutils.ManaItems.Contains(item.type) && item.position.DistanceSQ(new Vector2(Position.X * 16 + 12, Position.Y * 16 + 12)) < 6144)
                {
                    if (ConnectionsTo.Count > 0)
                    {
                        if (item.type == ItemID.Star || item.type == ItemID.SugarPlum || item.type == ItemID.SoulCake)
                            RotationNum = TransferMana(100, ConnectionsTo, RotationNum);

                        else if (item.type == ModContent.ItemType<Items.Magic.ManaStar>() && item.ModItem is Items.Magic.ManaStar star)
                            RotationNum = TransferMana(star.Mana, ConnectionsTo, RotationNum);
                    }
                    item.TurnToAir();
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (ConnectionsTo != new List<Vector2>()) tag.Add("ConnectionsTo", ConnectionsTo);
        }

        public override void LoadData(TagCompound tag)
        {
            ConnectionsTo = new List<Vector2>();
            if (tag.ContainsKey("ConnectionsTo"))
                ConnectionsTo = tag.Get<List<Vector2>>("ConnectionsTo");
        }
    }
}
