using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace GSMP.Content.Tiles
{
    public class PotionBurner : ModTile
    {
        public override string Texture => "GSMP/Assets/CandleTile";

        public override void PlaceInWorld(int i, int j, Item item) => TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<PotionBurnerTE>());

        public override bool Drop(int i, int j)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 8, 8), ModContent.ItemType<PotionBurnerItem>());
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
            {
                if (modEntity.itemtype != 0)
                    Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 8, 8), modEntity.itemtype);
                modEntity.Kill(i, j);
            }
            return false;
        }

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFlame[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Item item = player.HeldItem;

            if (GetType(i, j) != 0) Item.NewItem(new EntitySource_TileInteraction(player, i, j), new Rectangle(i * 16, j * 16, 8, 8), GetType(i, j));

            if (IsPotion(item))
            {
                SetType(i, j, item.type);
                SetBuff(i, j, item.buffType);

                Color col = TileEntities.ManaTEutils.PotionColor(item.type);
                col.A = 75;
                SetColor(i, j, col); // Store color in TE, not saved

                if (item.stack > 1)
                    item.stack--;
                else item.TurnToAir();

                return true;
            }

            SetType(i, j, 0);
            SetBuff(i, j, 0);
            return false;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (GetType(i, j) != 0)
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

                Vector2 zero = Main.drawToScreen ? Vector2.Zero  : new Vector2(Main.offScreenRange, Main.offScreenRange);

                spriteBatch.Draw(FlameTexture,
                                 new Vector2(i * 16 - (int)Main.screenPosition.X + shakeX - 1, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY - 4) + zero,
                                 new Rectangle(0, 0, 18, 22),
                                 GetColor(i, j),
                                 0f,
                                 default,
                                 1.15f,
                                 SpriteEffects.None,
                                 1f);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX == 0)
            {
                r = 0f;
                g = 0.35f;
                b = 0.8f;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/CandleTile").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange); // These two lines are from some example tile, i dont entirely understand them.
            Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y - 4) + zero;
            Color color = GetColor(i, j);
            color.A = 255;
            spriteBatch.Draw(texture, Pos, new Rectangle(0, 14, 14, 8), color, 0f, new Vector2(0, -14), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, Pos, new Rectangle(GetType(i, j) != 0 ? 0 : 18, 0, 16, 20), color, 0f, default, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = GetType(i, j) == 0 ? ItemID.WaterCandle : GetType(i, j);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            if (player.active && !player.dead && !player.ghost)
            {
                int buff = GetBuff(i, j);
                player.AddBuff(buff, 30);
            }
        }

        #region TE methods
        public static int GetBuff(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                return modEntity.buff;
            else return 0;
        }

        public static void SetBuff(int i, int j, int value)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                modEntity.buff = value;
        }

        public static int GetType(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                return modEntity.itemtype;
            else return 0;
        }

        public static void SetType(int i, int j, int type)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                modEntity.itemtype = type;
        }

        public static Color GetColor(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                return modEntity.color;
            else return Color.White;
        }

        public static void SetColor(int i, int j, Color color)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is PotionBurnerTE modEntity)
                modEntity.color = color;
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

    public class PotionBurnerTE : ModTileEntity
    {
        public int itemtype;
        public int buff;
        public Color color;

        public override bool IsTileValidForEntity(int x, int y) => Main.tile[x, y].TileType == ModContent.TileType<PotionBurner>();

        public override void SaveData(TagCompound tag)
        {
            if (itemtype != 0) tag.Add("itemtype", itemtype);
            if (buff != 0) tag.Add("buff", buff);
        }

        public override void LoadData(TagCompound tag)
        {
            itemtype = 0;
            if (tag.ContainsKey("itemtype"))
                itemtype = tag.Get<int>("itemtype");

            color = TileEntities.ManaTEutils.PotionColor(itemtype);
            color.A = 75;

            buff = 0;
            if (tag.ContainsKey("buff"))
                buff = tag.Get<int>("buff");
        }
    }
}
