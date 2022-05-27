using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using GSMP.Content.Buffs;

namespace GSMP.Content.Items.Consumable
{
    public class CovidPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Gives you Covid-20");
        }
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 1);
			Item.buffType = ModContent.BuffType<Buffs.Covid20>(); // Specify an existing buff to be applied when used.
			Item.buffTime = 5400; 
		}
   //     public override void OnConsumeItem(Player player)
   //     {
			//CurseFunction.Add(1);
   //     }
        public override bool CanUseItem(Player player)
        {
			return !player.HasBuff(ModContent.BuffType<Vaccinated>());
        }
    }
}
