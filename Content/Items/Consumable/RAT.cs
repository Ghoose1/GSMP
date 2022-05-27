using Terraria;
using Terraria.ModLoader;
using GSMP.Content.Buffs;
using Terraria.ID;

namespace GSMP.Content.Items.Consumable
{
    public class RAT : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("RAT");
            Tooltip.SetDefault("Rapid Antigen Test");
        }
        public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.consumable = true;
            Item.maxStack = 99;
            Item.width = 10;
            Item.height = 10;
            Item.useStyle = ItemUseStyleID.EatFood;
        }
        //public override bool? UseItem(Player player)
        //{
            
        //    return null;
        //}
        public override void UseAnimation(Player player)
        {

            if (player.HasBuff(ModContent.BuffType<Covid20>()))
                Main.NewText(player.name + " is infected");
            else
                Main.NewText(player.name + " isn't infected");
        }
    }
}
