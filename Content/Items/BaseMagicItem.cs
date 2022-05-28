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
        public int[] stats = new int[8];
        public BaseMagicItem() { for (int i = 0; i < stats.Length; i++) stats[i] = 10; }
        public override void SaveData(TagCompound tag) => tag["Stats"] = stats.ToList();
        public override void LoadData(TagCompound tag) => stats = tag.Get<List<int>>("Stats").ToArray();
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WaterBolt);
            Item.maxStack = 1;
            //Item.shoot = ModContent.ProjectileType<CustomProjectile>();
        }
        public override bool AltFunctionUse(Player player)
        {
            for (int i = 0; i < stats.Length; i++) stats[i] = 12;
            return true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < stats.Length; i++)
            {
                TooltipLine Line = new TooltipLine(Mod, "GitGud", stats[i].ToString());
                tooltips.Add(Line);
            }
        }
    }
}
