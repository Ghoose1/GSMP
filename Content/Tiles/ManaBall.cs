using GSMP.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using GSMP.Content.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using System;
using GraphicsLib.Primitives;
using Terraria.ObjectData;

namespace GSMP.Content.Items.Placeable
{
    public class ManaBallItem : ModItem // ManaBall means like crystal ball not a ball of mana thing
    {
        public override string Texture => "GSMP/Assets/ManaJarItem";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.CrystalBall);
            Item.createTile = ModContent.TileType<ManaBall>();
        }
    }
}

namespace GSMP.Content.Tiles
{
    public class ManaBall : ModTile
    {
        public override string Texture => "GSMP/Assets/ManaBall";

        public override void PlaceInWorld(int i, int j, Item item) // Placing Tile entity and assigning parameters
        {
            TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<ManaStorageEntity>());
            //Main.NewText("Test1");
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity existing))
            {
                //Main.NewText("Test2");
                if (existing is ManaStorageEntity ModTE)
                {
                    //Main.NewText("Test3");
                    ModTE.MaxMana = 10000;
                    ModTE.TransferRate = 10;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileWaterDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            //TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.addTile(Type);

            //ItemDrop = ModContent.ItemType<Items.Placeable.ManaBallItem>();

            AddMapEntry(new Color(100, 100, 255));
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            // Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
            // Therefore we register the top-left of the tile as a "special point"
            // This allows us to draw things in SpecialDraw
            if (drawData.tileFrameX % 36 == 0 && drawData.tileFrameY % 36 == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // This is lighting-mode specific, always include this if you draw tiles manually
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
            {
                offScreen = Vector2.Zero;
            }

            // Take the tile, check if it actually exists
            Tile tile = Main.tile[i, j];
            if (tile == null || !tile.HasTile)
            {
                return;
            }

            int X = i - (tile.TileFrameX / 18);
            int Y = j - (tile.TileFrameY / 18) + 1;

            Texture2D Balltexture = ModContent.Request<Texture2D>("GSMP/Assets/ManaBall").Value;

            Vector2 worldPos = new Point(i, j).ToWorldCoordinates(24f, 64f);

            //Color color = Lighting.GetColor(p.X, p.Y);
            int num = (int)Math.Floor((float)(ManaTEutils.Mana(X, Y) / (ManaTEutils.MaxMana(X, Y) / 254)));
            Color color = new Color(255 - num, 255 - num, 255);

            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(12f, -48f);

            // Draw the main texture
            spriteBatch.Draw(Balltexture, drawPos, new Rectangle(36, 0, 32, 32)/*Balltexture.Frame()*/, color, 0f, Balltexture.Frame().Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        public override bool RightClick(int i, int j) // Debug stuff
        {
            if (Main.keyState.PressingShift())
            {
                Main.NewText(ManaTEutils.DebugTriggerTransferMana(i, j).ToString());
            }
            else
            {
                Main.NewText("Transfer Rate: " + ManaTEutils.TransferRate(i, j));
                Main.NewText("Connections To:");
                for (int k = 0; k < ManaTEutils.ConnectionsTo(i, j).Length; k++)
                    Main.NewText(k.ToString() + " | X: " + ManaTEutils.ConnectionsTo(i, j)[k].X.ToString() + " | Y: " + ManaTEutils.ConnectionsTo(i, j)[k].Y.ToString());

                Main.NewText("Connections From:");
                for (int k = 0; k < ManaTEutils.ConnectionsFrom(i, j).Length; k++)
                    Main.NewText(k.ToString() + " | X: " + ManaTEutils.ConnectionsFrom(i, j)[k].X.ToString() + " | Y: " + ManaTEutils.ConnectionsFrom(i, j)[k].Y.ToString());
            }

            return false;
        }

        public override void MouseOverFar(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int X = i - (tile.TileFrameX / 18);
            int Y = j - (tile.TileFrameY / 18) + 1;
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeable.ManaBallItem>();
            player.cursorItemIconText = "  Mana: " + ManaTEutils.Mana(X, Y).ToString() + " / " + ManaTEutils.MaxMana(X, Y).ToString();
        }

        //public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        //{
        //    Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/ManaBall").Value;

        //    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange); // These two lines are from some example tile, i dont entirely understand them.
        //    Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

        //    // Draw Base
        //    //spriteBatch.Draw(texture, Pos, new Rectangle(0, 0, 16, 16), Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            

        //    //for (int k = 0; k < ManaTEutils.ConnectionsTo(i, j).Length; k++) // For all the connections the tile has, draw a line to the connected node
        //    //{
        //    //    Vector2 vector2 = new Vector2(ManaTEutils.ConnectionsTo(i, j)[k].X * 16, ManaTEutils.ConnectionsTo(i, j)[k].Y * 16);
        //    //    Vector2 vector1 = new Vector2(i * 16, j * 16);

        //    //    Vector2[] points = new Vector2[] { vector1, vector2 };

        //    //    PrimitiveDrawing.DrawLineStrip(points, Color.Blue, Color.White);
        //    //}

        //    return true; // Stop vanilla draw code from running
        //}

        //public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        //{
        //    Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/ManaBall").Value;

        //    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange); // These two lines are from some example tile, i dont entirely understand them.
        //    Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

        //    // Draw Ball
        //    //Color color = new Color(0, 0, (int)Math.Floor((float)(ManaTEutils.Mana(i, j) / (ManaTEutils.MaxMana(i, j) / 254))));
        //    Color color = Color.White;
        //    spriteBatch.Draw(texture, Pos, new Rectangle(18, 0, 18, 18), color /*Lighting.GetColor(i, j)*/, 0f, default, 1f, SpriteEffects.None, 0f);
        //}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                modEntity.Kill(i, j);
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<Items.Placeable.ManaBallItem>());
        }
    }
}

