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
        // This list should contain every mana Tile in magic system
        public static readonly List<int> Manastorage = new List<int> {
            ModContent.TileType<Tiles.ManaJar>(),
            ModContent.TileType<Tiles.ManaBall>(),
        };

        public static readonly List<int> ManaInput = new List<int>
        {
            ModContent.TileType<Tiles.CelestialMagnet>(),
            ModContent.TileType<Tiles.ManaExtractorCandle>(),
        };

        public static readonly List<int> ManaOutput = new List<int> {
            ModContent.TileType<Tiles.PotionBurner>(),
        };

        public static bool IsConnectionValid(int type) => Manastorage.Contains(type) || ManaInput.Contains(type) || ManaOutput.Contains(type);

        public static readonly List<int> RitualCores = new List<int>
        {
            ModContent.TileType<Tiles.PeaceRC>(),
        };

        public static readonly List<int> ManaItems = new List<int> {
            ModContent.ItemType<Items.Magic.ManaStar>(),
            ItemID.Star,
            ItemID.SoulCake,
            ItemID.SugarPlum,
        };

        public static  bool CanNPCHaveMana(NPC npc)
        {
            return !npc.boss
                   && npc != null
                   && !NPCID.Sets.CountsAsCritter[npc.type]
                   && !npc.SpawnedFromStatue
                   && !NPCID.Sets.ProjectileNPC[npc.type]
                   && !NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type]
                   && !NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
        }

        public static Color PotionColor(int type)
        {
            return type switch
            {
                ItemID.AmmoReservationPotion => Color.Orange,
                ItemID.ArcheryPotion => Color.Orange,
                ItemID.BattlePotion => Color.MediumPurple,
                ItemID.BuilderPotion => Color.Brown,
                ItemID.CalmingPotion => Color.CadetBlue,
                ItemID.CratePotion => Color.SandyBrown,
                ItemID.TrapsightPotion => Color.OrangeRed,
                ItemID.EndurancePotion => Color.Gray,
                ItemID.FeatherfallPotion => Color.LightBlue,
                ItemID.FishingPotion => Color.DarkSeaGreen,
                ItemID.FlipperPotion => Color.SkyBlue,
                ItemID.GillsPotion => Color.Blue,
                ItemID.GravitationPotion => Color.Purple,
                ItemID.LuckPotionGreater => Color.DeepPink,
                ItemID.HeartreachPotion => Color.HotPink,
                ItemID.HunterPotion => Color.Orange,
                ItemID.InfernoPotion => Color.Orange,
                ItemID.InvisibilityPotion => Color.LightBlue,
                ItemID.IronskinPotion => Color.Yellow,
                ItemID.LuckPotionLesser => Color.WhiteSmoke,
                ItemID.LifeforcePotion => Color.PaleVioletRed,
                ItemID.LuckPotion => Color.SlateBlue,
                ItemID.MagicPowerPotion => Color.Purple,
                ItemID.ManaRegenerationPotion => Color.Pink,
                ItemID.MiningPotion => Color.MediumSlateBlue,
                ItemID.NightOwlPotion => Color.GreenYellow,
                ItemID.ObsidianSkinPotion => Color.Black,
                ItemID.RagePotion => Color.OrangeRed,
                ItemID.RegenerationPotion => Color.Red,
                ItemID.ShinePotion => Color.Yellow,
                ItemID.SonarPotion => Color.LawnGreen,
                ItemID.SpelunkerPotion => Color.Yellow,
                ItemID.SummoningPotion => Color.LimeGreen,
                ItemID.SwiftnessPotion => Color.GreenYellow,
                ItemID.ThornsPotion => Color.LimeGreen,
                ItemID.TitanPotion => Color.LightSeaGreen,
                ItemID.WarmthPotion => Color.Orange,
                ItemID.WaterWalkingPotion => Color.Blue,
                ItemID.WrathPotion => Color.DarkRed,
                _ => Color.White
            };
        }

        public static ManaStorageEntity modEntity(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                return modEntity;
            else return null;
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
