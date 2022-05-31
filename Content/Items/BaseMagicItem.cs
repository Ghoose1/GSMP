using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;

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
        /// 0 - damage
        /// 1 - crit
        /// 2 - channel (1 = true)
        /// 3 - knockBack
        /// 4 - mana
        /// 5 - shootSpeed
        /// 6 - useStyle
        /// 7 - useTime
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
                TooltipLine Line = new TooltipLine(Mod, "GitGud ", itemStats[i].ToString());
                tooltips.Add(Line);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.DirtBlock, 1)
                .Register();
        }
    }
    public class BaseMagicProjectile : ModProjectile
    {
        public override string Texture => "GSMP/Assets/Projectile Images/Knife";
    }
}
