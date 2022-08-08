using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ID;
using System.Collections.Generic;
using GSMP.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace GSMP.Content.Tiles
{
    public class RitualCoreItem : ModItem
    {
        public override string Texture => "GSMP/Assets/ManaExtractorItem";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Glass);
            Item.createTile = ModContent.TileType<RitualCoreTile>();
        }
    }

    public class RitualCoreTile : ModTile
    {
        public override string Texture => "GSMP/Assets/ManaExtractorTile";
        public override bool IsLoadingEnabled(Mod mod) => true;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
        }

        internal int num;

        public override bool RightClick(int i, int j)
        {
            //Vector2 NextVector2Unit(float startRotation = 0f, float rotationRange = (float)Math.PI * 2f)
            //{
            //    return (startRotation + rotationRange * Main.rand.NextFloat()).ToRotationVector2();
            //}
            Vector2 NextVector2CircularEdge(float a, float circleHalfWidth, float circleHalfHeight, float startRotation = 0f, float rotationRange = (float)Math.PI * 2f)
            {
                return (startRotation + rotationRange * a).ToRotationVector2() * new Vector2(circleHalfWidth, circleHalfHeight);
            }

            int radius = 5;
            //for (int a = 0; a < 360; a++)
            //{
            //    int X = (int)(i + Math.Cos(a * (180 / Math.PI)) * 5);
            //    int Y = (int)(j - Math.Sin(a * (180 / Math.PI)) * 5);
            //    Vector2 vec = NextVector2CircularEdge(a / 360f, 5, 5);
            //    X = i + (int)vec.X;
            //    Y = j + (int)vec.Y;
            //    WorldGen.PlaceTile(X, Y, TileID.Dirt);
            //}

            Main.NewText(num);

            Vector2 vec = NextVector2CircularEdge(num / 60f, 15, 15);
            int X = i + (int)vec.X;
            int Y = j + (int)vec.Y;
            WorldGen.PlaceTile(X, Y, TileID.Dirt);
            num++;

            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            void CreateAura(int x, int y, int dist)
            {
                Vector2 Center = new Vector2(x, y);

                int dustMax = dist / 500 * 100;
                if (dustMax < 10) dustMax = 10;
                else if (dustMax > 40) dustMax = 40;

                for (int i = 0; i < dustMax; i++)
                {
                    Vector2 vector2 = Center + Main.rand.NextVector2CircularEdge(dist, dist);
                    Vector2 offset = vector2 - Main.LocalPlayer.Center;
                    if (Math.Abs(offset.X) > Main.screenWidth * 0.6f || Math.Abs(offset.Y) > Main.screenHeight * 0.6f) continue;
                    Dust dust = Main.dust[Dust.NewDust(vector2, 0, 0, DustID.PinkTorch, 0, 0, 0)];
                    dust.velocity.Y *= 0.1f;
                    dust.velocity.X *= 0.1f;
                    dust.noGravity = true;
                }
            }

            for (int e = 0; e < 6; e++)  CreateAura(i * 16 + 4, j * 16 + 4, 120);
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            num = 0;
            TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<TileEntities.RitualCoreTE>());
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is TileEntities.RitualCoreTE modEntity)
            {
                
            }
        }

        public override bool Drop(int i, int j)
        {
            //Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 8, 8), ModContent.ItemType<Items.Magic.Ritual_Cores.PeaceRitual>());
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is TileEntities.RitualCoreTE modEntity)
                modEntity.Kill(i, j);
            return true;
        }
    }
}
