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

namespace GSMP.Content.Items.Placeable
{
    public class ManaJarItem : ModItem // Man Ajar
    {
        public override string Texture => "GSMP/Assets/ManaJarItem";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Bottle);
            Item.createTile = ModContent.TileType<ManaJar>();
        }
    }
}

namespace GSMP.Content.Tiles
{
    public class ManaJar : ModTile
    {
        public override string Texture => "GSMP/Assets/ManaJar";

        public override void PlaceInWorld(int i, int j, Item item) // Placing Tile entity and assigning parameters
        {
            TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<ManaStorageEntity>());
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity existing))
            {
                if (existing is ManaStorageEntity ModTE)
                {
                    ModTE.MaxMana = 2000;
                    ModTE.TransferRate = 2;
                }
            }
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileWaterDeath[Type] = true;

            ItemDrop = ModContent.ItemType<Items.Placeable.ManaJarItem>();

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            AddMapEntry(new Color(100, 100, 255));
        }

        public override bool RightClick(int i, int j) // Debug stuff
        {
            if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<Items.Magic.ManaTransferer>())
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
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeable.ManaJarItem>();
            player.cursorItemIconText = "  Mana: " + ManaTEutils.Mana(i, j).ToString() + " / " + ManaTEutils.MaxMana(i, j).ToString();
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/ManaJar").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange); // These two lines are from some example tile, i dont entirely understand them.
            Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            int frameX = (int)Math.Floor((float)(ManaTEutils.Mana(i, j) / (ManaTEutils.MaxMana(i, j) / 5))); // Making the sprite change

            spriteBatch.Draw( texture, Pos, new Rectangle(frameX * 18, 0, 16, 16), Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            for (int k = 0; k < ManaTEutils.ConnectionsTo(i, j).Length; k++) // For all the connections the tile has, draw a line to the connected node
            {
                Vector2 vector2 = new Vector2(ManaTEutils.ConnectionsTo(i, j)[k].X * 16, ManaTEutils.ConnectionsTo(i, j)[k].Y * 16);
                Vector2 vector1 = new Vector2(i * 16, j * 16);

                Vector2[] points = new Vector2[] { vector1, vector2 };

                PrimitiveDrawing.DrawLineStrip(points, Color.Blue, Color.White);
            }

            return false; // Stop vanilla draw code from running
        } 

        public override bool Drop(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
            {
                ManaIntSource source = new ManaIntSource(i, j, ManaTEutils.Mana(i, j));
                int item = Item.NewItem(source, new Rectangle(i * 16, j * 16, 0, 0), ModContent.ItemType<Items.Magic.ManaStar>());
                if (Main.item[item].ModItem is Items.Magic.ManaStar star)
                {
                    star.Mana = modEntity.StoredMana;
                }
                modEntity.Kill(i, j);
            }
            return false;
        }
    }

    public class ManaIntSource : EntitySource_TileBreak
    {
        public int mana;
        public ManaIntSource(int X, int Y, int mana_, string context = null) : base(X, Y)
        {
            mana = mana_;
        }
    }
}
