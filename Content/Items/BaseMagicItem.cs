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

        // // Formation variables:
        //private int rotate;
        //public S[,] CustomFormation;
        public Spell Castspell;

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
        //public int[] projStats = new int[8];

        public BaseMagicItem() { // The default stats so that when a item is created it actually works
            int[] DefaultItemStats = { 100, 100, 0, 0, 5, 5, 5, 30, 30};
            itemStats = DefaultItemStats;
            Castspell = new Spell("Item", CustomTextureID.Ball, false, true);
            //Castspell.usesFormation = true;
            //Spell[,] DefaultFormation = new Spell[,] { { new Spell("null") } };
            //Castspell.formation = DefaultFormation;
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
            Item.channel = item.Castspell.projStats[2] == 3;

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
            
            tag["Spell"] = Castspell;
            tag["Stats"] = itemStats.ToList();
            //tag["Formation"] = CustomFormation;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Spell"))
                Castspell = tag.Get<Spell>("Spell");

            if (tag.ContainsKey("Stats"))
                itemStats = tag.Get<List<int>>("Stats").ToArray();

            UpdateStats(this);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WaterBolt);
            Item.maxStack = 1;
        }

        public override bool AltFunctionUse(Player player)
        {
            Castspell.formationRotate = Castspell.formationRotate == 1 ? -1 : Castspell.formationRotate + 1;
            Main.NewText(Castspell.formationRotate < 1 ? Castspell.formationRotate == 0 ? "Rotation inactive" : "Rotation AntiClockwise" : "Rotation Clockwise");
            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is for the projectile's position relative to the main projectile, this can stay for better efficency
            //int[] relativePosition = { 0, 0};  // x position, y position relative t the initialising proj, not needed here
            SpellEntitySource StatSource = new SpellEntitySource(player, Castspell);
            Projectile.NewProjectile(StatSource, player.position, velocity, 
                ModContent.ProjectileType<BaseMagicProjectile>(), damage, knockback, player.whoAmI, 0);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltipLine;
            Spell[,] formation = Castspell.formation;
            tooltipLine = new TooltipLine(Mod, "GSMP", "usesFormation: " + Castspell.usesFormation.ToString());
            tooltips.Add(tooltipLine);
            tooltipLine = new TooltipLine(Mod, "GSMP", "isFormationSlave: " + Castspell.isFormationSlave.ToString());
            tooltips.Add(tooltipLine);
            for (int j = 0; j < formation.GetLength(0); j++)
            {
                string line = "";
                for (int i = 0; i < formation.GetLength(1); i++)
                    line = line + (formation[j, i].Type != "Blank" ? (formation[j, i].isFormationSlave ? 1 : 2) : 0) + " ";
                tooltipLine = new TooltipLine(Mod, "GSMP", j.ToString() + ": " + line);
                tooltips.Add(tooltipLine);
            }

            for (int j = 0; j < formation.GetLength(0); j++)
            {
                for (int i = 0; i < formation.GetLength(1); i++)
                    if (formation[j, i].usesFormation)
                    {
                        tooltipLine = new TooltipLine(Mod, "GSMP", "AHHHHH");
                        tooltips.Add(tooltipLine);
                    }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                //.AddIngredient(ItemID.DirtBlock, 1)
                .Register();
        }
    }
    
}
