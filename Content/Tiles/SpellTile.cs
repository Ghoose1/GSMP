using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GSMP.Content.Items;

namespace GSMP.Content.Tiles
{
    public class SpellTile : ModTile
    {
        public override string Texture => "GSMP/Assets/Projectile Images/IceBolt";

        public override void SetStaticDefaults()
		{
			Main.tileBlockLight[Type] = true;
			ItemDrop = ModContent.ItemType<SpellItem>();
			AddMapEntry(new Color(200, 200, 200));
		}

        public override bool Drop(int i, int j)
        {

            return false;
        }
    }
}
