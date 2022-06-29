using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using GSMP.Content.Tiles;
using Microsoft.Xna.Framework;
using GraphicsLib.Primitives;
using Terraria.ID;
using Terraria.Graphics;
using GSMP.Content.TileEntities;

namespace GSMP.Content.Items.Magic
{
    public class ManaTransferer : ModItem
    {
        public override string Texture => "GSMP/Assets/SpellBookPink";
        internal bool flag1;
        internal int X1;
        internal int Y1;

        public override void SetDefaults()
        {
            //Item.CloneDefaults(ItemID.WaterBolt);
            flag1 = false;
            Item.useStyle = 5;
            Item.mana = 1;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.autoReuse = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.type != ModContent.ItemType<ManaTransferer>())
            {
                flag1 = false;
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            if (Main.keyState.PressingShift())
            {
                if (Item.mana != 10) Item.mana++;
                else Item.mana = 1;

                Main.NewText(Item.mana.ToString());
            }
            int i = (int)Main.MouseWorld.X / 16;
            int j = (int)Main.MouseWorld.Y / 16;
            Tile tile = Main.tile[i, j];
            int X = i - (tile.TileFrameX / 18);
            int Y = j - (tile.TileFrameY / 18);

            if (tile.HasTile && ManaTEutils.ValidTiles.Contains(tile.TileType))
            {
                //Main.NewText("A");
                if (!flag1)
                {
                    Main.NewText("Tile Selected");
                    X1 = X;
                    Y1 = Y;
                    //Main.NewText("X1: " + X1.ToString() + " Y1: " + Y1.ToString());
                    flag1 = true;
                }
                else
                {
                    //Main.NewText("B2");
                    //Main.NewText("X1: " + X1.ToString() + " Y1: " + Y1.ToString());
                    //Main.NewText("MX: " + (Main.MouseWorld.X / 16).ToString() + " MY: " + (Main.MouseWorld.Y / 16).ToString());
                    if (X != X1 || Y != Y1)
                    {
                        //Main.NewText("C");
                        int X2 = X;
                        int Y2 = Y;
                        Tile tile1 = Main.tile[X1, Y1];
                        if (tile1.HasTile && ManaTEutils.ValidTiles.Contains(tile1.TileType))
                        {
                            Main.NewText("Connection Created");
                            Vector2 point1 = new Vector2(X1, Y1);
                            Vector2 point2 = new Vector2(X2, Y2);
                            ManaTEutils.ConnectionsFrom(X2, Y2, point1);
                            ManaTEutils.ConnectionsTo(X1, Y1, point2);
                            flag1 = false;
                        }
                        else
                        {
                            //Main.NewText("D2");
                            X1 = X;
                            Y1 = Y;
                            flag1 = true;
                        }
                    }
                }
            }
            else flag1 = false;

            return false;
        }

        public override bool CanUseItem(Player player)
        {
            //Tile tile = Main.tile[(int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16];
            int i = (int)Main.MouseWorld.X / 16;
            int j = (int)Main.MouseWorld.Y / 16;
            Tile tile = Main.tile[i, j];
            int X = i - (tile.TileFrameX / 18);
            int Y = j - (tile.TileFrameY / 18);

            int TileMana = 0;
            int TileManaMax = 0;
            if (ManaTEutils.ValidTiles.Contains(tile.TileType))
            {
                TileMana = ManaTEutils.Mana(X, Y);
                TileManaMax = ManaTEutils.MaxMana(X, Y);
            }

            return ManaTEutils.ValidTiles.Contains(tile.TileType) && TileMana <= TileManaMax - Item.mana;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int i = (int)Main.MouseWorld.X / 16;
                int j = (int)Main.MouseWorld.Y / 16;
                Tile tile = Main.tile[i, j];
                int X = i - (tile.TileFrameX / 18);
                int Y = j - (tile.TileFrameY / 18);

                if (ManaTEutils.ValidTiles.Contains(tile.TileType))
                {
                    ManaTEutils.Mana(X, Y, Item.mana);
                }
            }

            return true;
        }
    }
}
