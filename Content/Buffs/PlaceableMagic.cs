using Terraria;
using Terraria.ModLoader;

namespace GSMP.Content.Buffs
{
    public class PlaceableMagic : ModBuff
    {
        public override string Texture => "GSMP/Assets/MagicBuff";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magical Aura");
            Description.SetDefault("Mana can exist in a physical form nearby");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<SpellPlayer>().canPlaceSpells = true;
        }
    }
}
