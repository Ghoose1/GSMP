using GSMP.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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

        public static int Mana(int i, int j, int Transfer)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
            {
                modEntity.StoredMana += Transfer;
                return modEntity.StoredMana;
            }
            else return 0;
        }

        public static int Mana(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                return modEntity.StoredMana;
            else return 0;
        }

        public static int MaxMana(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                return modEntity.MaxMana;
            else return 1;
        }

        public static void AddConnection(int i, int j, Point16 point)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                modEntity.StoredConnections.Add(point);
        }

        public static Point16[] StoredConnections(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                return modEntity.StoredConnections.ToArray();
            else return new Point16[] { Point16.Zero };
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<ManaStorageEntity>());
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity existing))
                if (existing is ManaStorageEntity existingAsT)
                    existingAsT.MaxMana = 2000;
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

        public override bool RightClick(int i, int j)
        {
            Main.NewText("Mana: " + Mana(i, j).ToString() + " / " + MaxMana(i, j).ToString());
            for (int k = 0; k < StoredConnections(i, j).Length; k++)
                Main.NewText(k.ToString() + " | X: " + StoredConnections(i, j)[k].X.ToString() + " | Y: " + StoredConnections(i, j)[k].Y.ToString());
             
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            void Dustline(int X2, int Y2)
            {
                for (int k = 0; k < X2 - i; k++)
                {
                    if (true) //Main.rand.NextBool(10))
                    {
                        Vector2 pos = new Vector2((k + i) * 16, j * 16);
                        Vector2 offset = new Vector2((k + i) * 16, j * 16) - Main.LocalPlayer.Center;
                        if (Math.Abs(offset.X) > Main.screenWidth * 0.6f || Math.Abs(offset.Y) > Main.screenHeight * 0.6f) continue;
                        Dust dust = Main.dust[Dust.NewDust(pos, 0, 0, 173, 0, 0, 0)];
                        dust.velocity.Y *= 0.1f;
                        dust.velocity.X *= 0.1f;

                        dust.noGravity = true;
                    }
                }

                for (int k = 0; k < Y2 - i; k++)
                {
                    if (true) //Main.rand.NextBool(10))
                    {
                        Vector2 pos = new Vector2(i * 16, (k + j) * 16);
                        Vector2 offset = new Vector2(i * 16, (k + j) * 16) - Main.LocalPlayer.Center;
                        if (Math.Abs(offset.X) > Main.screenWidth * 0.6f || Math.Abs(offset.Y) > Main.screenHeight * 0.6f) continue;
                        Dust dust = Main.dust[Dust.NewDust(pos, 0, 0, 173, 0, 0, 0)];
                        dust.velocity.Y *= 0.1f;
                        dust.velocity.X *= 0.1f;

                        dust.noGravity = true;
                    }
                }
            }
            void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color colorStart, Color colorEnd, float width)
            {
                float num;
                num = Vector2.Distance(start, end);
                Vector2 vector;
                vector = (end - start) / num;
                Vector2 value;
                value = start;
                Vector2 screenPosition;
                screenPosition = Main.screenPosition;
                float rotation;
                rotation = vector.ToRotation();
                float scale;
                scale = width / 16f;
                for (float num2 = 0f; num2 <= num; num2 += width)
                {
                    float amount;
                    amount = num2 / num;
                    spriteBatch.Draw(ModContent.Request<Texture2D>("GSMP/Assets/Projectile Images/Ball").Value, value - screenPosition, null, Color.Lerp(colorStart, colorEnd, amount), rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    value = start + num2 * vector;
                }
            }

            Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/ManaJar").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;
            int frameX = (int)Math.Floor((float)(Mana(i, j) / (MaxMana(i, j) / 6)));

            spriteBatch.Draw(
                texture,
                Pos,
                new Rectangle(frameX * 18, 0, 16, 16),
                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            for (int k = 0; k < StoredConnections(i, j).Length; k++)
            {
                Vector2 vector1 = new Vector2((i + 13) * 16, (j + 13) * 16);
                Vector2 vector2 = new Vector2((StoredConnections(i, j)[k].X + 13) * 16, (StoredConnections(i, j)[k].Y + 13) * 16);
                if (vector1.AngleTo(vector2) < 180) 
                    DrawLine(spriteBatch,
                        vector1,
                        vector2,
                        Color.White,
                        Color.Black, 
                        16);
            }

            return false;
        }

        public override bool Drop(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                modEntity.Kill(i, j);
            return false;
        }
    }

    public class ManaStorageEntity : ModTileEntity
    {
        public List<int> ValidTiles = new List<int> { ModContent.TileType<ManaJar>() };
        public int StoredMana;
        public int MaxMana;
        public List<Point16> StoredConnections = new List<Point16>();

        public override bool IsTileValidForEntity(int x, int y)
        {
            return ValidTiles.Contains(Main.tile[x, y].TileType);
        }

        public override void Update()
        {
            for (int k = 0; k < StoredConnections.Count; k++)
            {
                Tile tile = Main.tile[StoredConnections[k].X, StoredConnections[k].Y];
                if (!tile.HasTile || tile.TileType != ModContent.TileType<ManaJar>())
                    StoredConnections.RemoveAt(k);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("StoredMana", StoredMana);
            tag.Add("StoredConnections", StoredConnections);
            tag.Add("MaxMana", MaxMana);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("StoredMana")) 
                StoredMana = tag.Get<int>("StoredMana");
            else StoredMana = 0;

            if (tag.ContainsKey("StoredConnections"))
                StoredConnections = tag.Get<List<Point16>>("StoredConnections");
            else StoredConnections = new List<Point16>();

            if (tag.ContainsKey("MaxMana"))
                MaxMana = tag.Get<int>("MaxMana");
            else MaxMana = 1;
        }
    }
}
