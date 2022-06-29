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
        public List<Vector2> ConnectionsTo = new(); // Links between these entities 
        internal int AntiLagTimer;

        public override bool IsTileValidForEntity(int x, int y)
        {
            return ManaTEutils.ValidTiles.Contains(Main.tile[x, y].TileType) && Main.tile[x, y].HasTile;
        }

        public override void Update() 
        {
            // Making sure linked tiles are active and removing the link if not
            for (int k = 0; k < ConnectionsTo.Count; k++) 
            {
                Tile tile = Main.tile[(int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y];
                if (tile == null || !tile.HasTile || !ManaTEutils.ValidTiles.Contains(tile.TileType))
                    ConnectionsTo.RemoveAt(k);
            }

            foreach (Item item in Main.item)
            {
                if (ManaTEutils.ManaItems.Contains(item.type) && item.position.DistanceSQ(new Vector2(Position.X * 16 + 12, Position.Y * 16 + 12)) < 6144)
                {
                    if (ConnectionsTo.Count > 0)
                    {
                        // Assinging this here so that I dont need to get the values every time TransferMana() is Called
                        int[] maxes = new int[ConnectionsTo.Count];
                        for (int k = 0; k < ConnectionsTo.Count; k++)
                            maxes[k] = ManaTEutils.MaxMana((int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y);

                        if (item.type == ItemID.Star || item.type == ItemID.SugarPlum || item.type == ItemID.SoulCake)
                            TransferMana(100, maxes);

                        else if (item.type == ModContent.ItemType<Items.Magic.ManaStar>() && item.ModItem is Items.Magic.ManaStar star)
                            TransferMana(star.Mana, maxes);
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

        public void TransferMana(int num, int[] maxes, int[] previous = null, int[] changes = null) // This took wayyy too long to make but hopefully is worth it
        {
            int[] current; 

            // This would indicate that this is the first time TransferMana() is called, so setting correct values for changes[] and current[]
            if (previous == null) 
            {
                // Making the changes array blank, cus no changes have been made yet.
                changes = new int[ConnectionsTo.Count];

                // Assigning the starting mana values to current
                current = new int[ConnectionsTo.Count];
                for (int k = 0; k < ConnectionsTo.Count; k++)
                    current[k] = ManaTEutils.Mana((int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y);
            }
            else current = previous;

            // Default amount to try to add
            int average = (int)Math.Floor(num / (float)ConnectionsTo.Count);

            // Gets distributed separately to the bulk of the mana
            int remainder = num % ConnectionsTo.Count; 

            for (int k = 0; k < current.Length; k++) // modifying the current and change values
            {
                if (remainder > 0 && current[k] < maxes[k]) // remainder will always be less than amount of things
                {
                    changes[k]++;
                    current[k]++;
                    remainder--;
                    num--;
                }
                if (current[k] + average <= maxes[k]) // if it can fit the value just add the average / target value
                {
                    changes[k] += average;
                    current[k] += average;
                    num -= average;
                }
                else // if it cant, change the current value to its max value etc.
                {
                    changes[k] += maxes[k] - current[k];
                    num -= maxes[k] - current[k];
                    current[k] = maxes[k];
                }
            }

            if (num == 0 || current == previous) // num being zero indicates that mana has been distributed, current == prev indicates that it did not do anything
            {
                for (int k = 0; k < current.Length; k++)
                {
                    // Applying all the calculated changes
                    ManaTEutils.Mana((int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y, changes[k]);
                }
                return;
            }
            else TransferMana(num, current, changes); // Re-running the code with updated changes and mana arrays
        }
    }
}
