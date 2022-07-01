using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GSMP.Content.TileEntities
{
    public class ManaTEutils
    {
        // This list should contain every Tile in magic system
        public static readonly List<int> ValidTiles = new List<int> {
            ModContent.TileType<Tiles.ManaJar>(),
            ModContent.TileType<Tiles.ManaBall>(),
            ModContent.TileType<Tiles.CelestialMagnet>(),
            ModContent.TileType<Tiles.ManaExtractorCandle>(),
        };

        public static readonly List<int> ManaItems = new List<int> {
            ModContent.ItemType<Items.Magic.ManaStar>(),
            ItemID.Star,
            ItemID.SoulCake,
            ItemID.SugarPlum,
        };

        public static bool CanNPCHaveMana(NPC npc)
        {
            return !npc.boss
                   && npc != null
                   && !NPCID.Sets.CountsAsCritter[npc.type]
                   && !npc.SpawnedFromStatue
                   && !NPCID.Sets.ProjectileNPC[npc.type]
                   && !NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type]
                   && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
        }

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
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                modEntity.ConnectionsFrom.Add(pos);
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
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                return modEntity.ConnectionsFrom.ToArray();
            return new Vector2[] { Vector2.Zero };
        }
    }
}
