using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Terraria.DataStructures;

namespace GSMP.Content.Items.Magic
{
    public class ManaStar : ModItem
    {
        public override string Texture => "GSMP/Assets/Star";

        public int Mana;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is Tiles.ManaIntSource manaSource)
                Mana = manaSource.mana;
        }

        public override bool OnPickup(Player player)
        {
            player.ManaEffect(Mana);
            player.statMana += Mana; 
            return false;
        }
    }
}
