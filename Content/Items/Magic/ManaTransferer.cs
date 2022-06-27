using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using GSMP.Content.Tiles;
using Microsoft.Xna.Framework;
using GraphicsLib.Primitives;
using Terraria.ID;
using Terraria.Graphics;

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

            Tile tile = Main.tile[(int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16];
            if (tile.HasTile && tile.TileType == ModContent.TileType<ManaJar>())
            {
                Main.NewText("A");
                if (!flag1)
                {
                    Main.NewText("B");
                    X1 = (int)Main.MouseWorld.X / 16;
                    Y1 = (int)Main.MouseWorld.Y / 16;
                    //Main.NewText("X1: " + X1.ToString() + " Y1: " + Y1.ToString());
                    flag1 = true;
                }
                else
                {
                    Main.NewText("B2");
                    //Main.NewText("X1: " + X1.ToString() + " Y1: " + Y1.ToString());
                    //Main.NewText("MX: " + (Main.MouseWorld.X / 16).ToString() + " MY: " + (Main.MouseWorld.Y / 16).ToString());
                    if ((int)Main.MouseWorld.X / 16 != X1 || (int)Main.MouseWorld.Y / 16 != Y1)
                    {
                        Main.NewText("C");
                        int X2 = (int)Main.MouseWorld.X / 16;
                        int Y2 = (int)Main.MouseWorld.Y / 16;
                        Tile tile1 = Main.tile[X1, Y1];
                        if (tile1.HasTile && tile.TileType == ModContent.TileType<ManaJar>())
                        {
                            Main.NewText("D");
                            Point16 point1 = new Point16(X1, Y1);
                            Point16 point2 = new Point16(X2, Y2);
                            ManaJar.AddConnection(X2, Y2, point1);
                            ManaJar.AddConnection(X1, Y1, point2);
                            flag1 = false;
                        }
                        else
                        {
                            Main.NewText("D2");
                            X1 = (int)Main.MouseWorld.X / 16;
                            Y1 = (int)Main.MouseWorld.Y / 16;
                            flag1 = true;
                        }
                    }
                }
            }

            return false;
        }

        public override bool CanUseItem(Player player)
        {
            Tile tile = Main.tile[(int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16];

            int TileMana = 0;
            int TileManaMax = 0;
            if (tile.TileType == ModContent.TileType<ManaJar>())
            {
                TileMana = ManaJar.Mana((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
                TileManaMax = ManaJar.MaxMana((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            }

            return tile.TileType == ModContent.TileType<ManaJar>() && TileMana < TileManaMax - Item.mana;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Tile tile = Main.tile[(int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16];

                if (tile.TileType == ModContent.TileType<ManaJar>())
                {
                    ManaJar.Mana((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, Item.mana);
                }
            }

            return true;
        }
    }

    public class Testthing : ModItem
    {
        public override string Texture => "GSMP/Assets/SpellBookOrange";
        public override void SetDefaults()
        {
            Item.CloneDefaults(165);
            Item.shoot = ModContent.ProjectileType<FuckProj>();
        }
    }

    public class FuckProj : ModProjectile
    {
        public override string Texture => "GSMP/Assets/Projectile Images/Ball";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WaterBolt);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2[] array = new Vector2[] { Projectile.position, new Vector2(Projectile.position.X + 16, Projectile.position.Y + 16) };
            PrimitiveDrawing.DrawLineList(array, Color.White);
            return true;
        }

        public override void AI()
        {
            AIType = 27;
        }
    }
}
