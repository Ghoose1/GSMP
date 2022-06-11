using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GSMP.Content.Tiles
{
	public class IndicatorBlue : ModWall
	{
		public override string Texture => "GSMP/Assets/Wall_165";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallLight[Type] = true;
			Main.tileLighted[Type] = true;

			AddMapEntry(new Color(0, 150, 0));
		}
	}
	public class IndicatorWhite : ModWall
	{
		public override string Texture => "GSMP/Assets/Wall_155";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallLight[Type] = true;

			AddMapEntry(new Color(150, 150, 150));
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            base.ModifyLight(i, j, ref r, ref g, ref b);
        }
    }
}
