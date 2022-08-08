using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using GSMP.Content.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GraphicsLib.Primitives;
using Terraria.DataStructures;
using System;

namespace GSMP.Content.Tiles
{
    public class ManaExtractorCandle : ModTile
    {
        public override string Texture => "GSMP/Assets/ManaExtractorTile";

        public override void SetStaticDefaults()
        {
            ItemDrop = ModContent.ItemType<Items.Placeable.ManaExtractorCandleItem>();
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            AddMapEntry(new Color(200, 200, 200));
        }

        public override void PlaceInWorld(int i, int j, Item item) => TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<ManaExtractorEntity>());

        public override bool RightClick(int i, int j) // Debug stuff
        {
            if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<Items.Magic.ManaTransferer>())
            {
                Main.NewText("Connections To:");
                for (int k = 0; k < TEutils.ConnectionsTo(i, j).Length; k++)
                    Main.NewText(k.ToString() + " | X: " + TEutils.ConnectionsTo(i, j)[k].X.ToString() + " | Y: " + TEutils.ConnectionsTo(i, j)[k].Y.ToString());
            }

            return false;
        }

        public override void MouseOverFar(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeable.ManaExtractorCandleItem>();
        }

        public override bool Drop(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaExtractorEntity modEntity)
                modEntity.Kill(i, j);
            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            void CreateAura(int x, int y, int dist)
            {
                Vector2 Center = new Vector2(x, y);

                int dustMax = dist / 500 * 10;
                if (dustMax < 10) dustMax = 10;
                else if (dustMax > 80) dustMax = 80;

                for (int i = 0; i < dustMax; i++)
                {
                    Vector2 vector2 = Center + Main.rand.NextVector2CircularEdge(dist, dist);
                    Vector2 offset = vector2 - Main.LocalPlayer.Center;
                    if (Math.Abs(offset.X) > Main.screenWidth * 0.6f || Math.Abs(offset.Y) > Main.screenHeight * 0.6f) continue;
                    Dust dust = Main.dust[Dust.NewDust(vector2, 0, 0, DustID.BlueCrystalShard, 0, 0, 0)];
                    dust.velocity.Y *= 0.1f;
                    dust.velocity.X *= 0.1f;

                    dust.noGravity = true;
                }
            }

            Tile tile = Main.tile[i, j];
            int X = i - (tile.TileFrameX / 18);
            int Y = j - (tile.TileFrameY / 18);
            CreateAura(X * 16 + 4, Y * 16 + 4, 64);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            for (int k = 0; k < TEutils.ConnectionsTo(i, j).Length; k++) // For all the connections the tile has, draw a line to the connected node
            {
                Vector2 vector2 = new Vector2(TEutils.ConnectionsTo(i, j)[k].X * 16, TEutils.ConnectionsTo(i, j)[k].Y * 16);
                Vector2 vector1 = new Vector2(i * 16, j * 16);

                Vector2[] points = new Vector2[] { vector1, vector2 };

                PrimitiveDrawing.DrawLineStrip(points, Color.Blue, Color.White);
            }

            return true;
        }
    }
}

namespace GSMP.Content.Items.Placeable
{
    public class ManaExtractorCandleItem : ModItem
    {
        public override string Texture => "GSMP/Assets/ManaExtractorItem";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Extractor Orb"); 
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Bottle);
            Item.createTile = ModContent.TileType<Tiles.ManaExtractorCandle>();
        }
    }
}
