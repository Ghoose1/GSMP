using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GSMP.GlobalItems
{
	public class MagicMagnet : GlobalItem
	{
		public override bool AppliesToEntity(Item item, bool lateInstatiation)
		{
			return item.type == ItemID.CelestialMagnet;
		}

		public override void SetDefaults(Item item)
		{
			item.useStyle = ItemUseStyleID.Swing;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
			item.DefaultToPlaceableTile(ModContent.TileType<Content.Tiles.CelestialMagnet>());
		}
	}

	public class TechMagnet : GlobalItem
    {
		public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
			return item.type == ItemID.TreasureMagnet;
        }

		public override void SetDefaults(Item item)
        {
			item.useStyle = ItemUseStyleID.Swing;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
			item.DefaultToPlaceableTile(ModContent.TileType<Content.Tiles.ItemMagnet>());
		}
    }
}