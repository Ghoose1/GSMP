using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace GSMP.Content.Items
{
    public class BaseMagicItem : ModItem
    {
        public override string Texture => "GSMP/Assets/SpellBookRed";
        public override string Name => "Custom Magic Item";
        public int[] itemStats = new int[8];
        public int[] projStats = new int[8];
        public BaseMagicItem() { for (int i = 0; i < itemStats.Length; i++) itemStats[i] = 10; }
        /// <summary>
        /// itemStats:
        /// 0 - damage |
        /// 1 - crit |
        /// 2 - channel (1 = true) |
        /// 3 - knockBack |
        /// 4 - mana |
        /// 5 - shootSpeed |
        /// 6 - useStyle |
        /// 7 - useTime |
        /// projStats:
        /// 0 - lifeSteal |
        /// 1 - manaSteal |
        /// 2 - CustomAIType |
        /// 3 - maxPenetrate |
        /// 4 - timeLeft |
        /// 5 - ignoreWater (1 = true) |
        /// 6 - tileCollide (1 = true) |
        /// 7 - hostile (1 = true) |
        /// </summary>
        /// <param name="item"></param>
        public void UpdateStats(BaseMagicItem item)
        {
            Item.damage = item.itemStats[0];
            Item.crit = item.itemStats[1];
            Item.channel = item.itemStats[2] == 1;
            Item.knockBack = item.itemStats[3];
            Item.mana = item.itemStats[4];
            Item.shootSpeed = item.itemStats[5];
            Item.useStyle = item.itemStats[6];
            Item.useTime = item.itemStats[7];
        }
        public override void SaveData(TagCompound tag) => tag["Stats"] = itemStats.ToList();
        public override void LoadData(TagCompound tag)
        {
            itemStats = tag.Get<List<int>>("Stats").ToArray();
            UpdateStats(this);
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WaterBolt);
            Item.maxStack = 1;
            Item.shoot = ModContent.ProjectileType<BaseMagicProjectile>();
        }
        public override bool AltFunctionUse(Player player)
        {
            for (int i = 0; i < itemStats.Length; i++) itemStats[i] = 12;
            UpdateStats(this);
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < itemStats.Length; i++)
            {
                TooltipLine Line = new TooltipLine(Mod, "Git Gud", i.ToString() + ": " + itemStats[i].ToString());
                tooltips.Add(Line);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var StatSource = new MagicProjEntitySource(player, projStats);
            Projectile.NewProjectile(StatSource, position, velocity, ModContent.ProjectileType<BaseMagicProjectile>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.DirtBlock, 1)
                .Register();
        }
    }
    /// <summary>
    /// Projectile Stats:
    /// 0 - lifeSteal |
    /// 1 - manaSteal |
    /// 2 - CustomAIType |
    /// 3 - maxPenetrate |
    /// 4 - timeLeft |
    /// 5 - ignoreWater (1 = true) |
    /// 6 - tileCollide (1 = true) |
    /// 7 - hostile (1 = true) |
    /// </summary>
    public class BaseMagicProjectile : ModProjectile
    {
        public int[] stats = new int[8];
        public override string Texture => "GSMP/Assets/Projectile Images/Knife";
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.maxPenetrate = stats[3];
            Projectile.timeLeft = stats[4];
            Projectile.ignoreWater = stats[5] == 1;
            Projectile.tileCollide = stats[6] == 1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }
    }
    public class MagicProjEntitySource : EntitySource_Parent
    {
        public int[] Stats;
        public MagicProjEntitySource(Entity entity, int[] Stats2, string context = null) : base(entity, context)
        {
            Stats = Stats2;
        }
    }
}
