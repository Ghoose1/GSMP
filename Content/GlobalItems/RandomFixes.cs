using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GSMP.Content.GlobalItems
{
	public class ReaverSharkFix : GlobalItem
	{
		public override bool AppliesToEntity(Item item, bool lateInstatiation)
		{
			return item.type == ItemID.ReaverShark;
		}

		public override void SetDefaults(Item item)
		{
			item.pick = 100;
		}
	}
	public class Stacking : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation)
        {
			return item.maxStack > 1;
		}
        public override void SetDefaults(Item item)
        {
			item.maxStack =  69420;
        }
    }
}