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

        internal int timer;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            void CreateAura(int x, int y, int dist, int num1)
            {
                Vector2 NextVector2CircularEdge(float a, float startRotation = 0f, float rotationRange = (float)Math.PI * 2f)
                {
                    return (startRotation + rotationRange * a).ToRotationVector2() * new Vector2(a, a);
                }

                Vector2 Center = new Vector2(x, y);

                Vector2 vector2 = Center + NextVector2CircularEdge(dist, num1 * ((float)Math.PI) / 180);
                Vector2 offset = vector2 - Main.LocalPlayer.Center;
                //if (Math.Abs(offset.X) > Main.screenWidth * 0.6f || Math.Abs(offset.Y) > Main.screenHeight * 0.6f)
                Dust dust = Main.dust[Dust.NewDust(vector2, 0, 0, ModContent.DustType<RitualDust>(), 0, 0, 0, Color.Pink)];
                dust.velocity.Y *= 0.02f;
                dust.velocity.X *= 0.02f;
                dust.noGravity = true;
            }

            int num = 3;

            for (int e = 0; e < 4; e++)
            {
                for (int a = 1; a <= num; a++)
                    CreateAura(i * 16 + 4, j * 16 + 4, 120, timer * 2 + (360 * a / num));
                timer += 1;
            }

            //if (TileEntities.TEutils.TryModEntity(i, j, out ModTileEntity entity) && entity is TileEntities.RitualCoreTE ModEntity && !ModEntity.projBool)
            //{
            //    Projectiles.AuraProjSpawnSource source = new Projectiles.AuraProjSpawnSource(i * 16, j * 16, 20, true, 1);
            //    Projectile.NewProjectile(source, new Vector2(i * 16, j * 16), Vector2.Zero, ModContent.ProjectileType<Projectiles.CoolAuraProj>(), 0, 0);
            //    ModEntity.projBool = true;
            //}
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            timer = 0;
            TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<TileEntities.RitualCoreTE>());
            if (TileEntities.TEutils.TryModEntity(i, j, out ModTileEntity entity) && entity is TileEntities.RitualCoreTE modEntity)
            {
                modEntity.projBool = false;
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

    public class RitualDust : ModDust
    {
        public override string Texture => null;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;

            // Since the vanilla dust texture has all the dust in 1 file, we'll need to do some math.
            // If you want to use a vanilla dust texture, you can copy and paste it, changing the desiredVanillaDustTexture
            int desiredVanillaDustTexture = 173;
            int frameX = desiredVanillaDustTexture * 10 % 1000;
            int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
            dust.frame = new Rectangle(frameX, frameY, 8, 8);
            dust.alpha = 150;

            //dust.velocity = Vector2.Zero;
        }

        public override bool Update(Dust dust)
        { // Calls every frame the dust is active
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.15f;
            dust.scale *= 0.992f;

            float light = dust.scale;

            Lighting.AddLight(dust.position, dust.color.R, dust.color.G, dust.color.B);

            if (dust.scale < 0.4f)
            {
                dust.active = false;
            }

            return false; // Return false to prevent vanilla behavior.
        }
    }
}
