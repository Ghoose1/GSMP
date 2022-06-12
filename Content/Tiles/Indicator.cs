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
			Main.dust[Type] = null;

			AddMapEntry(new Color(0, 150, 0));
		}

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }
    }
	public class IndicatorWhite : ModWall
	{
		public override string Texture => "GSMP/Assets/Wall_155";

		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			Main.wallLight[Type] = true;
			Main.dust[Type] = null;

			AddMapEntry(new Color(150, 150, 150));
		}

		public override bool CreateDust(int i, int j, ref int type)
		{
			return false;
		}
	}
}
