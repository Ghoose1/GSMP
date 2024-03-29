﻿using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using GSMP.Content.Tiles;
using Microsoft.Xna.Framework;
using GraphicsLib.Primitives;
using Terraria.ID;
using Terraria.Graphics;
using GSMP.Content.TileEntities;
using Microsoft.Xna.Framework.Graphics;

namespace GSMP.Content.Items.Magic
{
    public class ManaTransferer : ModItem
    {
        public override string Texture => "GSMP/Assets/SpellBook";
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

        //public override void UpdateInventory(Player player)
        //{
        //    if (player.HeldItem.type != ModContent.ItemType<ManaTransferer>())
        //    {
        //        flag1 = false;
        //    }
        //}

        //public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        //{
        //    Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        //    spriteBatch.Draw(texture, position, Color.Pink);
        //    return false;
        //}

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Item.position, Color.Pink);
            return false;
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

            if (tile.HasTile && ManaTEutils.IsConnectionValid(tile.TileType))
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
                        if (tile1.HasTile && ManaTEutils.IsConnectionValid(tile.TileType))
                        {
                            Main.NewText("Connection Created");

                            Main.NewText("A");
                            if (TileEntity.ByPosition.TryGetValue(new Point16(X2, Y2), out TileEntity entity))
                            {
                                ManaStorageEntity TE = entity as ManaStorageEntity;
                                TE.ConnectionsFrom.Add(new Vector2(X1, Y1));
                                //if (entity is PotionBurnerTE PotTE)
                                //    PotTE.ConnectionsFrom.Add(new Vector2(X1, Y1));
                            }
                            //ManaStorageEntity TE1 = ManaTEutils.modEntity(X2, Y2);

                            ManaStorageEntity TE2 = ManaTEutils.modEntity(X1, Y1);
                            TE2.ConnectionsTo.Add(new Vector2(X2, Y2));
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

        public override void HoldItem(Player player)
        {
            Vector2[] points = new Vector2[] { Main.MouseWorld.ToWorldCoordinates(), player.position.ToWorldCoordinates()/*new Vector2(X1 * 16, Y1 * 16)*/ };
            PrimitiveDrawing.DrawLineStrip(points, Color.Blue, Color.White);
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
            if (ManaTEutils.IsConnectionValid(tile.TileType))
            {
                TileMana = ManaTEutils.Mana(X, Y);
                TileManaMax = ManaTEutils.MaxMana(X, Y);
            }

            return ManaTEutils.IsConnectionValid(tile.TileType) && TileMana <= TileManaMax - Item.mana;
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

                if (ManaTEutils.IsConnectionValid(tile.TileType))
                {
                    ManaStorageEntity TE = ManaTEutils.modEntity(i, j);
                    TE.StoredMana += Item.mana;
                }
            }

            return true;
        }
    }
}
