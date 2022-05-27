using System;
using GSMP.Content.Items.Placeable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GSMP.Content.Items.Placeable
{
    public class CandleItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("candle thing idk");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 16;
            Item.value = 750; 
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Candle>(); 
        }
    }
}