using Terraria;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using GraphicsLib.Primitives;
using Terraria.ID;

namespace GSMP.Content.TileEntities
{
    public class ManaStorageEntity : ModTileEntity
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

            if (ConnectionsTo.Count >= 1)
                TransferMana();
        }

        public override void SaveData(TagCompound tag)
        {
            if (MaxMana != 1) tag.Add("MaxMana", MaxMana);
            if (TransferRate != 1) tag.Add("TransferRate", TransferRate);

            if (StoredMana != 0) tag.Add("StoredMana", StoredMana);
            tag.Add("ConnectionsTo", ConnectionsTo);
            tag.Add("ConnectionsFrom", ConnectionsFrom);
        }

        public override void LoadData(TagCompound tag)
        {
            MaxMana = 1;
            if (tag.ContainsKey("MaxMana"))
                MaxMana = tag.Get<int>("MaxMana");

            TransferRate = 1;
            if (tag.ContainsKey("TransferRate"))
                TransferRate = tag.Get<int>("TransferRate");



            StoredMana = 0;
            if (tag.ContainsKey("StoredMana")) 
                StoredMana = tag.Get<int>("StoredMana");

            ConnectionsTo = new List<Vector2>();
            if (tag.ContainsKey("ConnectionsTo"))
                ConnectionsTo = tag.Get<List<Vector2>>("ConnectionsTo");

            ConnectionsFrom = new List<Vector2>();
            if (tag.ContainsKey("ConnectionsFrom"))
                ConnectionsFrom = tag.Get<List<Vector2>>("ConnectionsFrom");
        }

        public void TransferMana()
        {
            int amount = (int)Math.Ceiling(((float)TransferRate / (float)ConnectionsTo.Count));
            if (amount * ConnectionsTo.Count > StoredMana) return;
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

    public class ManaTEutils
    {
        // This list should contain every Tile in magic system
        public static readonly List<int> ValidTiles = new List<int> { 
            ModContent.TileType<Tiles.ManaJar>(),
            ModContent.TileType<Tiles.ManaBall>(),
            ModContent.TileType<Tiles.CelestialMagnet>(),
        };

        public static readonly List<int> ManaItems = new List<int> {
            ModContent.ItemType<Items.Magic.ManaStar>(),
            ItemID.Star,
            ItemID.SoulCake,
            ItemID.SugarPlum,
        };

        public static int Mana(int i, int j, int Transfer)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
            {
                modEntity.StoredMana += Transfer;
                return modEntity.StoredMana;
            }
            else return 0;
        }

        public static int Mana(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                return modEntity.StoredMana;
            else return 0;
        }

        public static int MaxMana(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                return modEntity.MaxMana;
            else return 1;
        }

        public static int TransferRate(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                return modEntity.TransferRate;
            else return 1;
        }

        public static void ConnectionsTo(int i, int j, Vector2 pos)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity))
            {
                if (entity is ManaStorageEntity modEntity)
                    modEntity.ConnectionsTo.Add(pos);
                else if (entity is ManaMagnetEntity magEntity)
                    magEntity.ConnectionsTo.Add(pos);
            }
        }

        public static void ConnectionsFrom(int i, int j, Vector2 pos)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity))
            {
                if (entity is ManaStorageEntity modEntity)
                    modEntity.ConnectionsFrom.Add(pos);
                else if (entity is ManaMagnetEntity magEntity)
                    magEntity.ConnectionsFrom.Add(pos);
            }
        }

        public static Vector2[] ConnectionsTo(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity))
            {
                if (entity is ManaStorageEntity modEntity)
                    return modEntity.ConnectionsTo.ToArray();
                else if (entity is ManaMagnetEntity magEntity)
                    return magEntity.ConnectionsTo.ToArray();
            }
            return new Vector2[] { Vector2.Zero };
        }

        public static Vector2[] ConnectionsFrom(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity))
            {
                if (entity is ManaStorageEntity modEntity)
                    return modEntity.ConnectionsFrom.ToArray();
                else if (entity is ManaMagnetEntity magEntity)
                    return magEntity.ConnectionsFrom.ToArray();
            }
            return new Vector2[] { Vector2.Zero };
        }

        public static int DebugTriggerTransferMana(int i, int j)
        {
            int mana1 = 0;
            int mana2 = 0;
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
            {
                mana1 = modEntity.StoredMana;
                modEntity.TransferMana();
                mana2 = modEntity.StoredMana;
            }
            return mana1 - mana2;
        }
    }
}