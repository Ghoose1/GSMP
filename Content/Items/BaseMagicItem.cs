using GSMP.Content.Projectiles;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GSMP.DataStructures;

namespace GSMP.Content.Items
{
    public class BaseMagicItem : ModItem
    {
        public override string Texture => "GSMP/Assets/SpellBookRed";
        public override string Name => "Custom Magic Item";

        // Formation variables:
        private int rotate;
        public int[,] CustomFormation;

        // Custom Stat arrays
        /// <summary>
        /// 0 - damage |
        /// 1 - crit |
        /// 2 - autoswing (1 = true) |
        /// 3 - knockBack |
        /// 4 - mana |
        /// 5 - shootSpeed |
        /// 6 - useStyle |
        /// 7 - useTime |
        /// 8 - useAnimation |
        /// </summary>
        public int[] itemStats = new int[9];
        /// <summary>
        /// 0 - lifeSteal (Unused) |
        /// 1 - manaSteal (Unused) |
        /// 2 - CustomAIType |
        /// 3 - maxPenetrate |
        /// 4 - timeLeft |
        /// 5 - ignoreWater (1 = true) |
        /// 6 - tileCollide (1 = true) |
        /// 7 - formation number (Depreciated) |
        /// </summary>
        public int[] projStats = new int[8];

        public BaseMagicItem() { // The default stats so that when a item is created it actually works
            int[] DefaultItemStats = { 100, 100, 0, 0, 5, 5, 5, 30, 30};
            itemStats = DefaultItemStats;
            int[] DefaultProjStats = { 0, 0, 1, 0, 60, 1, 0, 0 };
            projStats = DefaultProjStats;
            int[,] DefaultFormation =
            {
                { 1, 1, 1, 1, 1, },
                { 1, 1, 1, 1, 1, },
                { 1, 1, 2, 1, 1, },
                { 1, 1, 1, 1, 1, },
                { 1, 1, 1, 1, 1, },
            };
            CustomFormation = DefaultFormation;
        }

        /// <summary>
        /// itemStats:
        /// 0 - damage |
        /// 1 - crit |
        /// 2 - autoswing (1 = true) |
        /// 3 - knockBack |
        /// 4 - mana |
        /// 5 - shootSpeed |
        /// 6 - useStyle |
        /// 7 - useTime |
        /// 8 - useAnimation | 
        /// projStats:
        /// 0 - lifeSteal |
        /// 1 - manaSteal |
        /// 2 - CustomAIType |
        /// 3 - maxPenetrate |
        /// 4 - timeLeft |
        /// 5 - ignoreWater (1 = true) |
        /// 6 - tileCollide (1 = true) |
        /// 7 - formation number |
        /// </summary>
        /// <param name="item"></param>
        public void UpdateStats(BaseMagicItem item)
        {
            Item.channel = item.projStats[2] == 3;

            Item.damage = item.itemStats[0];
            Item.crit = item.itemStats[1];
            Item.autoReuse = item.itemStats[2] == 1;
            Item.knockBack = item.itemStats[3];
            Item.mana = item.itemStats[4];
            Item.shootSpeed = item.itemStats[5];
            Item.useStyle = item.itemStats[6];
            Item.useTime = item.itemStats[7];
            Item.useAnimation = item.itemStats[8];
        }

        public override void SaveData(TagCompound tag)
        {
            
            tag["ProjStats"] = projStats.ToList();
            tag["Stats"] = itemStats.ToList();
            tag["Formation"] = CustomFormation;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("ProjStats"))
                projStats = tag.Get<List<int>>("ProjStats").ToArray();

            if (tag.ContainsKey("Stats"))
                itemStats = tag.Get<List<int>>("Stats").ToArray();

            if (tag.ContainsKey("Formation"))
                CustomFormation = tag.Get<int[,]>("Formation");

            UpdateStats(this);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WaterBolt);
            Item.maxStack = 1;
        }

        public override bool AltFunctionUse(Player player)
        {
            rotate = rotate == 1 ? -1 : rotate + 1;
            Main.NewText(rotate < 1 ? rotate == 0 ? "Rotation inactive" : "Rotation AntiClockwise" : "Rotation Clockwise");
            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int[] passStats = { 0, 0, rotate };
            var StatSource = new MagicProjEntitySource(player, projStats, null, passStats, CustomFormation); // AllFormations[projStats[7]]);
            Projectile.NewProjectile(StatSource, player.position, velocity, ModContent.ProjectileType<BaseMagicProjectile>(), damage, knockback, player.whoAmI, 0);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.DirtBlock, 1)
                .Register();
        }
    }
    
}
