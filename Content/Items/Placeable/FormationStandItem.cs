using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace GSMP.Content.Items.Placeable
{
	public class FormationStandItem : ModItem
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.DD2ElderCrystalStand}";
        public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Idfk lol");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.maxStack = 1;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 2000;
			Item.createTile = ModContent.TileType<Tiles.FormationStandTile>();
		}

		public override void AddRecipes()
		{
			CreateRecipe(1)
				//.AddIngredient(ItemID.DirtBlock, 1)
				.Register();
		}
	}
}
