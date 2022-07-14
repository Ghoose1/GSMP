using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GSMP.Content.Tiles
{
    public class PeaceRC : ModTile
    {
        public override string Texture => "GSMP/Assets/ManaExtractorTile";

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<TileEntities.RitualCoreTE>());
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is TileEntities.RitualCoreTE modEntity)
            {
                modEntity.radius = 3;
                modEntity.objects = new List<KeyValuePair<DataStructures.RitualObject, int>> {
                    new KeyValuePair<DataStructures.RitualObject, int>(new DataStructures.RitualObject(3, TileID.Sunflower), 2),
                    new KeyValuePair<DataStructures.RitualObject, int>(new DataStructures.RitualObject(3, TileID.Campfire), 1),
                    new KeyValuePair<DataStructures.RitualObject, int>(new DataStructures.RitualObject(2, BuffID.Regeneration), 2),
                    new KeyValuePair<DataStructures.RitualObject, int>(new DataStructures.RitualObject(2, BuffID.Calm), 2),
                };
            }
        }

        public override bool Drop(int i, int j)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Rectangle(i * 16, j * 16, 8, 8), ModContent.ItemType<Items.Placeable.PeaceRCItem>());
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is TileEntities.RitualCoreTE modEntity)
                modEntity.Kill(i, j);
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            bool effectActive()
            {
                if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is TileEntities.RitualCoreTE modEntity)
                    return modEntity.active;
                return false;
            }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 Pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            Color color = effectActive() ? Color.Red : Lighting.GetColor(i, j);

            spriteBatch.Draw(texture, Pos, new Rectangle(0, 0, 16, 16), color, 0f, default, 1f, SpriteEffects.None, 0f);

            return false; // Stop vanilla draw code from running
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is TileEntities.RitualCoreTE modEntity)
            {

            }
        }
    }
}

namespace GSMP.Content.Items.Placeable
{
    public class PeaceRCItem : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Snowball}";
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Glass);
            Item.createTile = ModContent.TileType<Tiles.PeaceRC>();
        }
    }
}
