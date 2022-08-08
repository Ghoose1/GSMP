using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using GSMP.Content.Tiles;

namespace GSMP.Content.TileEntities
{
    public class TEutils
    {
        public static readonly List<int> Manastorage = new List<int> {
            ModContent.TileType<ManaJar>(),
            ModContent.TileType<ManaBall>(),
            ModContent.TileType<PotionBurner>(),
        };

        public static readonly List<int> ManaInput = new List<int>
        {
            ModContent.TileType<CelestialMagnet>(),
            ModContent.TileType<ManaExtractorCandle>(),
        };

        public static readonly List<int> ManaOutput = new List<int> {
            ModContent.TileType<PotionBurner>(),
        };

        public static readonly List<int> RitualCores = new List<int>
        {
            //ModContent.TileType<PeaceRitualTile>(),
        };

        public static readonly List<int> ManaItems = new List<int> {
            ModContent.ItemType<Items.Magic.ManaStar>(),
            ItemID.Star,
            ItemID.SoulCake,
            ItemID.SugarPlum,
        };

        public static bool IsConnectionValid(int type) => Manastorage.Contains(type) || ManaInput.Contains(type) || ManaOutput.Contains(type);

        public static  bool CanNPCHaveMana(NPC npc)
        {
            return !(npc.boss
                   || npc == null
                   || !npc.active
                   || NPCID.Sets.CountsAsCritter[npc.type]
                   || npc.SpawnedFromStatue
                   || NPCID.Sets.ProjectileNPC[npc.type]
                   || NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type]
                   || NPCID.Sets.ShouldBeCountedAsBoss[npc.type]);
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

        public static ModTileEntity modEntity(int i, int j) // The only good method
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ModTileEntity modEntity)
                return modEntity;
            else return null;
        }

        public static bool TryModEntity(int i, int j, out ModTileEntity entity)
        {
            entity = modEntity(i, j);
            if (entity != null) return true;
            else return false;
        }

        public static bool TryManaEntity(int i, int j, out ManaStorageEntity TE) // The REAL only good method
        {
            ModTileEntity entity = modEntity(i, j);
            TE = null;
            if (entity != null && entity is ManaStorageEntity ManaEntity)
            {
                TE = ManaEntity;
                return true;
            }
            else return false;
        }

        public static int Mana(int i, int j)
        {
            if (TryManaEntity(i ,j, out ManaStorageEntity TE))
                return TE.StoredMana;
            else return 0;
        }

        public static int MaxMana(int i, int j)
        {
            if (TryManaEntity(i, j, out ManaStorageEntity TE))
                return TE.MaxMana;
            else return 1;
        }

        public static Vector2[] ConnectionsTo(int i, int j)
        {
            if (TryManaEntity(i, j, out ManaStorageEntity TE))
                return TE.ConnectionsTo.ToArray();
            return new Vector2[] { Vector2.Zero };
        }

        public static Vector2[] ConnectionsFrom(int i, int j)
        {
            if (TryManaEntity(i, j, out ManaStorageEntity TE))
                return TE.ConnectionsFrom.ToArray();
            return new Vector2[] { Vector2.Zero };
        }
    }
}
