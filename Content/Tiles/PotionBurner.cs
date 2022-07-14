using GSMP.Content.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace GSMP.Content.Tiles
{
    public class PotionBurner : ModTile
    {
        public override string Texture => "GSMP/Assets/CandleTile";

        public override void PlaceInWorld(int i, int j, Item item)
        {
            TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<PotionBurnerTE>());
            PotionBurnerTE TE = modEntity(i, j);
            TE.MaxMana = 5000;
        }

        public override bool Drop(int i, int j)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 8, 8), ModContent.ItemType<PotionBurnerItem>());
            PotionBurnerTE TE = modEntity(i, j);
            if (TE != null)
            {
                if (TE.itemtype != 0)
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 8, 8), TE.itemtype);
                TE.Kill(i, j);
            }
            return false;
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.addTile(Type);
        }

        public override void MouseOverFar(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeable.ManaJarItem>();
            player.cursorItemIconText = "  Mana: " + ManaTEutils.Mana(i, j).ToString() + " / " + ManaTEutils.MaxMana(i, j).ToString();
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Item item = player.HeldItem;
            PotionBurnerTE TE = modEntity(i, j);
            if (TE != null)
            {
                if (TE.itemtype != 0)
                {
                    Item.NewItem(new EntitySource_TileInteraction(player, i, j), new Rectangle(i * 16, j * 16, 8, 8), TE.itemtype);
                    TE.itemtype = 0;
                    TE.buff = 0;
                }

                if (IsPotion(item))
                {
                    TE.itemtype = item.type;
                    TE.buff = item.buffType;

                    Color col = TileEntities.ManaTEutils.PotionColor(item.type);

                    TE.color = col; // Store color in TE, not saved

                    if (item.stack > 1)
                        item.stack--;
                    else item.TurnToAir();

                    return true;
                }

                TE.itemtype = 0;
                TE.buff = 0;
            }
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            PotionBurnerTE TE = modEntity(i, j);
            if (TE != null && TE.buff != 0)
            {
                Texture2D FlameTexture = ModContent.Request<Texture2D>("GSMP/Content/Tiles/FlameTexture").Value;

                Tile tile = Main.tile[i, j];
                int width = 18;
                int offsetY = 0;
                int height = 22;
                short frameX = tile.TileFrameX;
                short frameY = tile.TileFrameY;

                ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

                float shakeX = Utils.RandomInt(ref randSeed, -2, 2) * 0.15f;
                float shakeY = Utils.RandomInt(ref randSeed, -2, 1) * 0.35f;

                TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY);

                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Color color = GetColor(i, j);

                spriteBatch.Draw(FlameTexture,
                                 new Vector2(i * 16 - (int)Main.screenPosition.X + shakeX - 1, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY - 4) + zero,
                                 new Rectangle(0, 0, 18, 22),
                                 color,
                                 0f,
                                 default,
                                 1.15f,
                                 SpriteEffects.None,
                                 1f);

            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (GetType(i, j) != 0)
            {
                Color c = GetColor(i, j);
                r = c.R / 255;
                g = c.G / 255;
                b = c.B / 255;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/CandleTile").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y - 4) + zero;

            spriteBatch.Draw(texture, Pos, new Rectangle(0, 0, 16, 20), Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = GetType(i, j) == 0 ? ItemID.WaterCandle : GetType(i, j);
        }

        internal int timer; // This can be the same for every tile

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            PotionBurnerTE PotTE = modEntity(i, j);
            ManaStorageEntity ManaTE = ManaTEutils.modEntity(i, j);
            if (player.active && !player.dead && !player.ghost && PotTE != null && ManaTE != null && !PotTE.inactiveWithFlame && ManaTE.StoredMana != 0 && timer == 30)
            {
                timer = 0;
                player.AddBuff(PotTE.buff, 30);
                ManaTE.StoredMana--;
            }
            else timer++;
        }

        #region TE methods
        public static PotionBurnerTE modEntity(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                return modEntity;
            else return null;
        }

        public static int GetBuff(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                return modEntity.buff;
            else return 0;
        }

        public static int GetType(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                return modEntity.itemtype;
            else return 0;
        }

        public static Color GetColor(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                return modEntity.color;
            else return Color.White;
        }
        #endregion

        public static bool IsPotion(Item item)
        {
            bool flag = item != null && item.active && item.consumable && item.buffType != 0 && item.buffTime != 0 && !item.IsAir;
            return flag;
        }
    }

    public class PotionBurnerItem : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.WaterCandle}";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WaterCandle);
            Item.flame = false;
            Item.createTile = ModContent.TileType<PotionBurner>();
        }
    }
}

namespace GSMP.Content.TileEntities
{
    public class PotionBurnerTE : ManaStorageEntity
    {
        public int itemtype;
        public int buff;
        public Color color;
        public bool inactiveWithFlame;

        public override bool IsTileValidForEntity(int x, int y) => Main.tile[x, y].TileType == ModContent.TileType<Tiles.PotionBurner>();

        public override void SaveData(TagCompound tag)
        {
            if (itemtype != 0) tag.Add("itemtype", itemtype);
            if (buff != 0) tag.Add("buff", buff);

            if (MaxMana != 1) tag.Add("MaxMana", MaxMana);

            if (StoredMana != 0) tag.Add("StoredMana", StoredMana);
            tag.Add("ConnectionsFrom", ConnectionsFrom);
        }

        public override void LoadData(TagCompound tag)
        {
            itemtype = 0;
            if (tag.ContainsKey("itemtype"))
                itemtype = tag.Get<int>("itemtype");

            color = ManaTEutils.PotionColor(itemtype);
            color.A = 75;

            buff = 0;
            if (tag.ContainsKey("buff"))
                buff = tag.Get<int>("buff");

            MaxMana = 1;
            if (tag.ContainsKey("MaxMana"))
                MaxMana = tag.Get<int>("MaxMana");

            StoredMana = 0;
            if (tag.ContainsKey("StoredMana"))
                StoredMana = tag.Get<int>("StoredMana");

            ConnectionsFrom = new List<Vector2>();
            if (tag.ContainsKey("ConnectionsFrom"))
                ConnectionsFrom = tag.Get<List<Vector2>>("ConnectionsFrom");
        }
    }
}
