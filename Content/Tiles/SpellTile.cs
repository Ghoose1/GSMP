using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GSMP.Content.Items;
using GSMP.DataStructures;
using GSMP;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework.Graphics;

namespace GSMP.Content.Tiles
{
    public class SpellTile : ModTile
    {
        public override string Texture => "GSMP/Assets/spellTile";
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Item item = player.HeldItem;

            return true;
        }
        public override void SetStaticDefaults()
		{
			ItemDrop = ModContent.ItemType<SpellItem>();
            Main.tileFrameImportant[Type] = true;
            AddMapEntry(new Color(200, 200, 200));
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            //Main.NewText("Block Placed");
            if (item.ModItem is SpellItem spellItem) 
            {
                //Main.NewText("Item Is Valid");
                TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<SpellTileEntity>());
                if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity existing))
                {
                    //Main.NewText("B");
                    if (existing is SpellTileEntity existingAsT)
                    {
                        //Main.NewText("C");
                        existingAsT.spell = spellItem.spell;
                    }
                }
            }
        }

        public override bool Drop(int i, int j)
        {
            //Main.NewText("1");
            //if (ModTileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity existing) 
            //    && existing is SpellTile existingAsT) { }
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is SpellTileEntity modEntity)
            {
                //Main.NewText("2");
                
                SpellItemSource source = new SpellItemSource(i * 16, j * 16, modEntity.spell);
                int item = Item.NewItem(source, new Rectangle(i * 16, j * 16, 0, 0), ModContent.ItemType<SpellItem>());
                if (Main.item[item].ModItem is SpellItem spellItem)
                {
                    spellItem.spell = modEntity.spell;
                }
                modEntity.Kill(i, j);
                
            }
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/Projectile Images/" + 
                CustomTexture.GetString(spell(i, j).textureID)).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("GSMP/Assets/spellTileglow").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;
            Pos.Y -= (texture.Height - 16) / 2;
            Pos.X -= (texture.Width - 16) / 2;
            Color color = new Color(spell(i, j).R, spell(i, j).G, spell(i, j).B);

            spriteBatch.Draw(
                texture,
                Pos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                color, 0f, default, 1f, SpriteEffects.None, 0f);

            if (!spell(i, j).isFormationSlave)
                spriteBatch.Draw(
                    glowTexture,
                    new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                    new Rectangle(0, 0, 16, 16),
                    Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            return false;
        }

        public Spell spell(int i, int j)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is SpellTileEntity modEntity)
            {
                return modEntity.spell;
            }
            return new Spell("Error Spell");
        }

        
    }

    public class SpellTileEntity : ModTileEntity
    {
        public Spell spell;
        //public int debugTimer;

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Main.tile[x, y].TileType == ModContent.TileType<SpellTile>() && Main.tile[x, y].HasTile;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int placedEntity = Place(i - 1, j - 1);
            //debugTimer = 0;
            return placedEntity;
        }

        public override void OnKill() => Main.NewText("TE killed");

        public override void SaveData(TagCompound tag) => tag["Spell"] = spell;
        public override void LoadData(TagCompound tag) => spell = tag.Get<Spell>("Spell");
    }

    public class SpellItemSource : EntitySource_TileBreak
    {
        public Spell spell;
        public SpellItemSource(int X, int Y, Spell spell_, string context = null) : base(X, Y)
        {
            spell = spell_;
        }
    }
}
