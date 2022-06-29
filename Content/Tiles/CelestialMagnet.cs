using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using GSMP.Content.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using GraphicsLib.Primitives;
using System;

namespace GSMP.Content.Tiles
{
    public class CelestialMagnet : ModTile
    {

        public override void PlaceInWorld(int i, int j, Item item) // Placing Tile entity and assigning parameters
        {
            //Tile tile = Main.tile[i, j];
            //int X = i - (tile.TileFrameX / 18);
            //int Y = j - (tile.TileFrameY / 18);
            Tile tile = Framing.GetTileSafely(i, j);

            Point16 coord = new Point16(i, j);
            Point16 frame = new Point16(tile.TileFrameX / 18, tile.TileFrameY / 18);

            Point16 p = coord - frame;

            TileEntity.PlaceEntityNet(p.X, p.Y, ModContent.TileEntityType<ManaMagnetEntity>());
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileWaterDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(100, 100, 255));
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            void CreateAura(int x, int y, int dist)
            {
                Vector2 Center = new Vector2(x, y);

                int dustMax = dist / 500 * 10;
                if (dustMax < 10) dustMax = 10;
                else if (dustMax > 40) dustMax = 40;

                for (int i = 0; i < dustMax; i++)
                {
                    Vector2 vector2 = Center + Main.rand.NextVector2CircularEdge(dist, dist);
                    Vector2 offset = vector2 - Main.LocalPlayer.Center;
                    if (Math.Abs(offset.X) > Main.screenWidth * 0.6f || Math.Abs(offset.Y) > Main.screenHeight * 0.6f) continue;
                    Dust dust = Main.dust[Dust.NewDust(vector2, 0, 0, DustID.WaterCandle, 0, 0, 0)];
                    dust.velocity.Y *= 0.1f;
                    dust.velocity.X *= 0.1f;

                    dust.noGravity = true;
                }
            }

            Tile tile = Main.tile[i, j];
            int X = i - (tile.TileFrameX / 18);
            int Y = j - (tile.TileFrameY / 18);
            CreateAura(X * 16 + 12, Y * 16 + 12, 64);
        }

        public override bool RightClick(int i, int j) // Debug stuff
        {
            if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<Items.Magic.ManaTransferer>())
            {
                Tile tile = Main.tile[i, j];
                i = i - (tile.TileFrameX / 18);
                j = j - (tile.TileFrameY / 18);
                Main.NewText("Connections To:");
                for (int k = 0; k < ManaTEutils.ConnectionsTo(i, j).Length; k++)
                    Main.NewText(k.ToString() + " | X: " + ManaTEutils.ConnectionsTo(i, j)[k].X.ToString() + " | Y: " + ManaTEutils.ConnectionsTo(i, j)[k].Y.ToString());

                Main.NewText("Connections From:");
                for (int k = 0; k < ManaTEutils.ConnectionsFrom(i, j).Length; k++)
                    Main.NewText(k.ToString() + " | X: " + ManaTEutils.ConnectionsFrom(i, j)[k].X.ToString() + " | Y: " + ManaTEutils.ConnectionsFrom(i, j)[k].Y.ToString());
            }

            return false;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                modEntity.Kill(i, j);
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ItemID.CelestialMagnet);
        }
    }
}