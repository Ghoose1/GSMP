using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using GSMP.Content.Buffs;

namespace GSMP.Content.Items.Consumable
{
    public class Vaccine : ModItem
    {
        public override void SetStaticDefaults()
        {
			Tooltip.SetDefault("Gives you immunity to Covid-20");
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
			Item.buffType = ModContent.BuffType<Vaccinated>();
			Item.buffTime = 3600;
		}
    }
	public class Vaccinated : ModBuff
    {
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Vaccinated");
			Description.SetDefault("You are vaccinated against Covid-20");
			Main.buffNoSave[Type] = false;
			Main.buffNoTimeDisplay[Type] = true;
			//Main.debuff[Type] = true;
			Main.persistentBuff[Type] = true;
		}
	}
}
