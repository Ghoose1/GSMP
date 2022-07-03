using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using GSMP.Content.Items.Magic;
using Terraria.Audio;
using System;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace GSMP.Content.Items.Magic
{

    public class telekinesis : ModItem
    {
        //public string SomePrivateField
        //{
        //    get
        //    {
        //        return somePrivateField;
        //    }
        //    set
        //    {
        //        TileToplace = value;
        //    }
        //}
        public int TileToPlace;
        public Tile tile;
        public override string Texture => "GSMP/Assets/SpellBook";
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Allows the manipulation of basic materials");
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, position, Color.Blue);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Item.position, Color.Blue);
            return false;
        }

        public override void SetDefaults()
        {
            Item.channel = true;
            Item.knockBack = 5f;
            Item.useStyle = 1;
            Item.shoot = ModContent.ProjectileType<DirtProj>();
            Item.width = 32;
            Item.height = 28;
            Item.UseSound = SoundID.Item8; 
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 5);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            WorldGen.KillTile((int)(((float)Main.mouseX + Main.screenPosition.X) / 16f),
                              (int)(((float)Main.mouseY + Main.screenPosition.Y) / 16f),
                              false,
                              false,
                              true);
        }
        public override bool CanUseItem(Player player)
        {
            int X = (int)(((float)Main.mouseX + Main.screenPosition.X) / 16f);
            int Y = (int)(((float)Main.mouseY + Main.screenPosition.Y) / 16f);
            Tile tile = Main.tile[X, Y];
            TileToPlace = tile.TileType;
            if (tile.HasTile && Main.tileSolid[tile.TileType]) // Main.tileSolid[Type] = true;
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //public override bool CanShoot(Player player)
        //{
        //    int X = (int)(((float)Main.mouseX + Main.screenPosition.X) / 16f);
        //    int Y = (int)(((float)Main.mouseY + Main.screenPosition.Y) / 16f);
        //    Tile tile = Main.tile[X, Y];
        //    TileToPlace = tile.TileType;
        //    if (tile.HasTile && Main.tileSolid[tile.TileType]) // Main.tileSolid[Type] = true;
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float ai = TileToPlace;
            //int projectileID = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, TileID.Dirt);//, 255, ai0, 0);
            //Projectile projectile = Main.projectile[projectileID];
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, ai);
            return false; //Projectile.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DirtBlock)
                .AddIngredient(ItemID.Book)
                .Register();
        }
    }
    public class DirtProj : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("cool projectile");
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            //Projectile.aiStyle = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
        }
        public void ProjLight()
        {
            if (!(Projectile.light > 0f))
            {
                return;
            }
            float num;
            num = Projectile.light;
            float num2;
            num2 = Projectile.light;
            float num3;
            //num3 = Projectile.light;
            num *= 0.1f;
            num2 *= 0.4f;
            num3 = 1f;
            Lighting.AddLight((int)((Projectile.position.X + (float)(Projectile.width / 2)) / 16f), (int)((Projectile.position.Y + (float)(Projectile.height / 2)) / 16f), num, num2, num3);
        }
        public override void AI() //aistyle == 10, aitype == 17
        {
            bool flag3;
            flag3 = ((Vector2)(Projectile.velocity)).Length() > 0.1f && Vector2.Dot(Projectile.oldVelocity.SafeNormalize(Vector2.Zero), Projectile.velocity.SafeNormalize(Vector2.Zero)) < 0.2f;
            if (Projectile.soundDelay == 0 && Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) > 2f)
            {
                Projectile.soundDelay = 10;
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }

            int num9;
            num9 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 16, 0f, 0f, 100, Color.White, 2f);
            Dust obj;
            obj = Main.dust[num9];
            obj.velocity *= 0.3f;
            Main.dust[num9].position.X = Projectile.position.X + (float)(Projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
            Main.dust[num9].position.Y = Projectile.position.Y + (float)(Projectile.height / 2) + (float)Main.rand.Next(-4, 5);
            Main.dust[num9].noGravity = true;
            //Dust obj2;
            //obj2 = Main.dust[num9];
            //obj2.velocity += Main.rand.NextVector2Circular(2f, 2f);

            if (flag3)
            {
                int num10;
                num10 = Main.rand.Next(2, 5);
                for (int i = 0; i < num10; i++)
                {
                    Dust dust5;
                    dust5 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 15, 0f, 0f, 100, Color.White, 1.5f);
                    dust5.velocity *= 0.3f;
                    dust5.position = Projectile.Center;
                    dust5.noGravity = true;
                    dust5.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust5.fadeIn = 2.2f;
                }
            }
            //Vector2 val4;
            // Rectangle hitbox;
            Projectile.tileCollide = true;
            Projectile.localAI[1] = 0f;
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 0f)
            {
                Projectile.tileCollide = false;
                if (Main.player[Projectile.owner].channel)
                {
                    Projectile.localAI[1] = -1f;
                    float num5;
                    num5 = 12f;
                    Vector2 vector = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num6;
                    num6 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
                    float num7;
                    num7 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
                    if (Main.player[Projectile.owner].gravDir == -1f)
                    {
                        num7 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector.Y;
                    }
                    float num8;
                    num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                    num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                    if (num8 > num5)
                    {
                        num8 = num5 / num8;
                        num6 *= num8;
                        num7 *= num8;
                        if (num6 != Projectile.velocity.X || num7 != Projectile.velocity.Y)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = num6;
                        Projectile.velocity.Y = num7;
                    }
                    else
                    {
                        if (num6 != Projectile.velocity.X || num7 != Projectile.velocity.Y)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = num6;
                        Projectile.velocity.Y = num7;
                    }
                }
                else
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.ai[0] == 1f)
            {
                Projectile.velocity.Y += 0.41f;
            }
            else if (Projectile.ai[0] == 2f)
            {
                Projectile.velocity.Y += 0.2f;
                if ((double)Projectile.velocity.X < -0.04)
                {
                    Projectile.velocity.X += 0.04f;
                }
                else if ((double)Projectile.velocity.X > 0.04)
                {
                    Projectile.velocity.X -= 0.04f;
                }
                else
                {
                    Projectile.velocity.X = 0f;
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = (int)(Projectile.position.X / 16f); i <= (int)((Projectile.position.X + (float)Projectile.width) / 16f); i++)
                {
                    for (int j = (int)(Projectile.position.Y / 16f); j <= (int)((Projectile.position.Y + (float)Projectile.height) / 16f); j++)
                    {
                        if (!WorldGen.InWorld(i, j))
                        {
                            continue;
                        }
                        Tile tile;
                        tile = Main.tile[i, j];
                        if (tile != null && tile.HasTile)
                        {
                            if ((tile.TileType >= 185 && tile.TileType <= 187) || tile.TileType == 165 || tile.TileType == 12 || tile.TileType == 105 || tile.TileType == 178)
                            {
                                WorldGen.KillTile(i, j);
                            }
                            //else if (tile.topSlope() && !TileID.Sets.Platforms[tile.type])
                            //{
                            //    tile.slope(0);
                            //}
                        }
                    }
                }
            }
            int width2;
            width2 = Projectile.width;
            int height2;
            height2 = Projectile.height;
            Projectile.Resize(128, 128);
            //Projectile.maxPenetrate = -1;
            //Projectile.penetrate = -1;
            //Projectile.Damage();
            Projectile.Resize(width2, height2);
            //Projectile.scale += 0.1f;
            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }
        }
        public override void Kill(int timeLeft)
        {
            if (!Projectile.active)
            {
                return;
            }
            Main.projectileIdentity[Projectile.owner, Projectile.identity] = -1;
            int num;
            num = Projectile.timeLeft;
            Projectile.timeLeft = 0;
            if (!ProjectileLoader.PreKill(Projectile, num))
            {
                Projectile.active = false;
                return;
            }
            //Vector2 center2;

            if (!Main.projPet[Projectile.type])
            {
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                for (int num636 = 0; num636 < 5; num636++)
                {
                    Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 0);
                }
            }

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.netMode != 0)
                {
                    NetMessage.SendData(29, -1, -1, null, Projectile.identity, Projectile.owner);
                }
            }

            int num976;
            num976 = -1;
            int num981;
            num981 = (int)(Projectile.position.X + (float)(Projectile.width / 2)) / 16;
            int num982;
            num982 = (int)(Projectile.position.Y + (float)(Projectile.width / 2)) / 16;
            int num983;
            num983 = (int)Projectile.ai[1];
            
            Tile tile = Main.tile[num981, num982];
            if (tile.HasTile && tile.IsHalfBlock && Projectile.velocity.Y > 0f && Math.Abs(Projectile.velocity.Y) > Math.Abs(Projectile.velocity.X))
            {
                num982--;
            }
            if (!tile.HasTile && num983 >= 0)
            {
                bool flag4;
                flag4 = false;
                bool flag5;
                flag5 = false;
                if (num982 < Main.maxTilesY - 2)
                {
                    Tile tile2;
                    tile2 = Main.tile[num981, num982 + 1];
                    if (tile2 != null && tile2.HasTile)
                    {
                        if (tile2.HasTile && tile2.TileType == 314)
                        {
                            flag5 = true;
                        }
                        if (tile2.HasTile && WorldGen.BlockBelowMakesSandFall(num981, num982))
                        {
                            flag5 = true;
                        }
                    }
                    
                }
                if (!flag5)
                {
                    flag4 = WorldGen.PlaceTile(num981, num982, num983, mute: false, forced: true);
                }
                if (!flag5 && tile.HasTile && tile.TileType == num983)
                {
                    Tile tile3 = Main.tile[num981, num982 + 1];
                    if (tile3.IsHalfBlock) //|| tile3.slope( != 0)
                    {
                        WorldGen.SlopeTile(num981, num982 + 1);
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(17, -1, -1, null, 14, num981, num982 + 1);
                        }
                    }
                    if (Main.netMode != 0)
                    {
                        NetMessage.SendData(17, -1, -1, null, 1, num981, num982, num983);
                    }
                }
                //    else if (!flag4 && num984 > 0)
                //    {
                //        num976 = Item.NewItem((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, num984);
                //    }
                //}
                //else if (num984 > 0)
                //{
                //    num976 = Item.NewItem((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, num984);
                //}
            }
            if (Main.netMode == 1 && num976 >= 0)
            {
                NetMessage.SendData(21, -1, -1, null, num976, 1f);
            }
        }
    }
}