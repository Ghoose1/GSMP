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
using GraphicsLib.Primitives;
using Terraria.Graphics.Effects;

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

        #region Stat things
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

        public static void AddConnection(int i, int j, Vector2 pos)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                modEntity.StoredConnections.Add(pos);
        }

        public static Vector2[] StoredConnections(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)
                return modEntity.StoredConnections.ToArray();
            else return new Vector2[] { Vector2.Zero };
        }
        #endregion

        public override void PlaceInWorld(int i, int j, Item item) // Placing Tile entity and assigning parameters
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

        public override bool RightClick(int i, int j) // Debug stuff
        {
            Main.NewText("Mana: " + Mana(i, j).ToString() + " / " + MaxMana(i, j).ToString());
            for (int k = 0; k < StoredConnections(i, j).Length; k++)
                Main.NewText(k.ToString() + " | X: " + StoredConnections(i, j)[k].X.ToString() + " | Y: " + StoredConnections(i, j)[k].Y.ToString());
             
            return false;
        }

        /*public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/ManaJar").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange); // These two lines are from some example tile, i dont entirely understand them.
            Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero; 

            int frameX = (int)Math.Floor((float)(Mana(i, j) / (MaxMana(i, j) / 6))); // Making the sprite change

            spriteBatch.Draw( // Drawing the actual sprite
                texture,
                Pos,
                new Rectangle(frameX * 18, 0, 16, 16),
                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            for (int k = 0; k < StoredConnections(i, j).Length; k++) // For all the connections the tile has, draw a line to the connected node
            {
                Vector2 vector1 = new Vector2(i * 16, j * 16); // this tile pos
                Vector2 vector2 = new Vector2(StoredConnections(i, j)[k].X * 16, StoredConnections(i, j)[k].Y * 16); // other tile pos
                PrimitiveDrawing.DrawLineList(new Vector2[] { vector1, vector2 }, Color.White);
            }

            return false; // Stop vanilla draw code from running
        }*/

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/ManaJar").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange); // These two lines are from some example tile, i dont entirely understand them.
            Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            int frameX = (int)Math.Floor((float)(Mana(i, j) / (MaxMana(i, j) / 6))); // Making the sprite change

            spriteBatch.Draw( texture, Pos, new Rectangle(frameX * 18, 0, 16, 16), Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            for (int k = 0; k < StoredConnections(i, j).Length; k++) // For all the connections the tile has, draw a line to the connected node
            {
                Vector2 vector2 = new Vector2(StoredConnections(i, j)[k].X * 16, StoredConnections(i, j)[k].Y * 16) + zero;
                Vector2 vector1 = new Vector2(i * 16, j * 16) + zero;

                Vector2[] points = new Vector2[] { vector1, vector2 };

                if (vector2.X > vector1.X || (vector2.X < vector1.X && vector2.Y > vector1.Y)) 
                    PrimitiveDrawing.DrawLineList(points, Color.White);                
            }

            return false; // Stop vanilla draw code from running
        }

        //public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        //{
        //    //ApparentlyThisHelps();
        //    //spriteBatch.End();
        //    if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is ManaStorageEntity modEntity)


        //    for (int k = 0; k < StoredConnections(i, j).Length; k++) // For all the connections the tile has, draw a line to the connected node
        //    {
        //        Vector2 vector1 = new Vector2(i * 16, j * 16); // this tile pos
        //        Vector2 vector2 = new Vector2(StoredConnections(i, j)[k].X * 16, StoredConnections(i, j)[k].Y * 16); // other tile pos
        //        PrimitiveDrawing.DrawLineList(new Vector2[] { vector1, vector2 }, Color.Red);
        //    }
        //    // spriteBatch.Begin();
        //}

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
        public List<Vector2> StoredConnections = new List<Vector2>();

        public override bool IsTileValidForEntity(int x, int y)
        {
            return ValidTiles.Contains(Main.tile[x, y].TileType);
        }

        //public override void PostGlobalUpdate()
        //{
        //    PrimitiveDrawing.DrawFilledRectangle(new Vector2(Position.X, Position.Y), new Vector2(Position.X + 15, Position.Y + 15), Color.White, Color.Blue, Color.Red, Color.Green);
        //}

        public override void Update()
        {
            for (int k = 0; k < StoredConnections.Count; k++)
            {
                Tile tile = Main.tile[(int)StoredConnections[k].X, (int)StoredConnections[k].Y];
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
            StoredMana = 0;
            if (tag.ContainsKey("StoredMana")) 
                StoredMana = tag.Get<int>("StoredMana");

            StoredConnections = new List<Vector2>();
            if (tag.ContainsKey("StoredConnections"))
            {
                StoredConnections = tag.Get<List<Vector2>>("StoredConnections");
            }

            MaxMana = 1;
            if (tag.ContainsKey("MaxMana"))
                MaxMana = tag.Get<int>("MaxMana");
        }
    }
}
