using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace GSMP.Content.Projectiles
{
    public class TestItem : ModItem
    {
        public override string Texture => "GSMP/Assets/SpellBookGreen";
        private int Mode;
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.DirtBlock, 1)
                .Register();
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WaterBolt);
            Item.maxStack = 1;
            Mode = 1;
            //Item.shoot = ModContent.ProjectileType<TestFormationProjectile>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var SpawnSource = new TestProjEntitySource(player, null, 0, 0, Mode);
            Projectile.NewProjectile(SpawnSource, position, velocity, ModContent.ProjectileType<TestFormationProjectile>(), damage, knockback, player.whoAmI, 0f);
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            if (Mode != 5) Mode++;
            else Mode = 1;
            Main.NewText("Mode: " + Mode.ToString());
            return false;
        }
    }
    public class TestFormationProjectile : ModProjectile
    {
        public override string Texture => "GSMP/Assets/Projectile Images/IceBolt";
        public Projectile ParentProjectile;
        /// <summary>
        /// Formations: 
        /// 1 - orbiting
        /// 2 - Use Formation shape
        /// 3 - use Formation shape (spinning)
        /// </summary>
        public int Formation;
        public int FormationShape;
        #region formation shapes
        private readonly int[,] T =
        {
            { 1, 1, 1 },
            { 0, 2, 0 },
            { 0, 1, 0 }
        };
        private readonly int[,] L =
        {
            { 1, 0 },
            { 2, 0 },
            { 1, 1 }
        };
        private readonly int[,] funny =
        {
            { 1, 1, 1, 0, 1, },
            { 0, 0, 1, 0, 1, },
            { 1, 1, 2, 1, 1, },
            { 1, 0, 1, 0, 0, },
            { 1, 0, 1, 1, 1, },
        };
        private readonly int[,] funnybig =
        {
            { 1, 0, 0, 1, 1, 1, 1, },
            { 1, 0, 0, 1, 0, 0, 0, },
            { 1, 0, 0, 1, 0, 0, 0, },
            { 1, 1, 1, 2, 1, 1, 1, },
            { 0, 0, 0, 1, 0, 0, 1, },
            { 0, 0, 0, 1, 0, 0, 1, },
            { 1, 1, 1, 1, 0, 0, 1, },
        };
        private readonly int[,] penis =
        {
            { 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, },
            { 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 2, 1, 0, 0, 0, 1, 0, 1, 0, 1, },
            { 1, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, },
            { 1, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, },
            { 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 1, },
        };
        private readonly int[,] linetest =
        {
            { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, }
        };
        #endregion
        //public Projectile[] form = new Projectile[8];
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.maxPenetrate = 10;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is TestProjEntitySource Source)
            {
                Formation = Source.var3;
                switch (Formation)
                {
                    case 1:
                        if (Projectile.ai[0] == 0f)
                        {
                            ParentProjectile = null;
                            Projectile.tileCollide = true;
                            //form[0] = Projectile;
                            var SpawnSource = new TestProjEntitySource(Main.player[Projectile.owner], this, 100, 0, 1);
                            Projectile.NewProjectile(SpawnSource, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TestFormationProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 1);
                            Projectile.NewProjectile(SpawnSource, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TestFormationProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 0);
                        }
                        else
                        {
                            var1 = Source.var1;
                            Projectile.tileCollide = false;
                            ParentProjectile = Source.proj.Projectile;
                            //form[(int)Projectile.ai[0]] = Projectile;
                            if (Projectile.ai[0] < 3)
                            {
                                var SpawnSource = new TestProjEntitySource(Main.player[Projectile.owner], this, 50 / (int)Projectile.ai[0], 0, 1);
                                Projectile.NewProjectile(SpawnSource, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TestFormationProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1, 1);
                                Projectile.NewProjectile(SpawnSource, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TestFormationProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1, 0);
                            }
                        }
                        break;
                    case 2:
                        FormationSpawnHandeler(funny, Source);
                        break;
                    case 3:
                        FormationSpawnHandeler(funnybig, Source);
                        break;
                    case 4:
                        FormationSpawnHandeler(penis, Source);
                        break;
                    case 5:
                        FormationSpawnHandeler(linetest, Source);
                        break;
                    default:
                        break;
                }
            }
        }
        //public void addThing(Projectile proj, int pos)
        //{
        //    form[pos] = proj;
        //}
        //public override void Kill(int timeLeft)
        //{
        //    for (int i = 1; i < form.Length; i++)
        //    {
        //        Main.projectile[form[i].whoAmI].Kill();
        //    }
        //}
        internal int timer;
        internal int var1;
        internal int var2;
        internal int var3;
        internal int Xoffset;
        internal int Yoffset;
        public void FormationSpawnHandeler(int[,] template, TestProjEntitySource Source)
        {
            if (Projectile.ai[0] == 0)
            {
                ParentProjectile = null;
                Projectile.tileCollide = false;
                // find Parent Proj point
                for (int i = 0; i < template.GetLength(1); i++)
                {
                    for ( int j = 0; j < template.GetLength(0); j++)
                    {
                        if (template[j, i] == 2)
                        {
                            Xoffset = -i;
                            Yoffset = -j;
                            break;
                        }
                    }
                }
                //// i coulnt get Math.floor to work so i did this instead
                //if (template.GetLength(1) % 2 != 0) Xoffset = -(int)(template.GetLength(1) / 2 + 0.5);
                //else Xoffset = -(int)(template.GetLength(1) / 2);
                //if (template.GetLength(0) % 2 != 0) Yoffset = -(int)(template.GetLength(0) / 2 + 0.5);
                //else Yoffset = -(int)(template.GetLength(0) / 2);
                //form[0] = Projectile;
                for (int x = 0; x < template.GetLength(1); x++)
                {
                    for (int y = 0; y < template.GetLength(0); y++)
                    {
                        if (template[y, x] == 1)
                        {
                            var SpawnSource = new TestProjEntitySource(Main.player[Projectile.owner], this, x + Xoffset, y + Yoffset, 2);
                            Projectile.NewProjectile(SpawnSource, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TestFormationProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 0);
                        }
                    }
                }
            }
            else
            {
                var1 = Source.var1;
                var2 = Source.var2;
                Projectile.tileCollide = false;
                ParentProjectile = Source.proj.Projectile;
                //form[(int)Projectile.ai[0]] = Projectile;
            }
        }
        public override void AI()
        {
            if (Formation == 1)
            {
                timer += 4;
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
                if (Projectile.ai[0] == 0)
                {
                    int num9;
                    num9 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 16, 0f, 0f, 100, Color.White, 2f);
                    Dust obj;
                    obj = Main.dust[num9];
                    obj.velocity *= 0.3f;
                    Main.dust[num9].position.X = Projectile.position.X + (float)(Projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
                    Main.dust[num9].position.Y = Projectile.position.Y + (float)(Projectile.height / 2) + (float)Main.rand.Next(-4, 5);
                    Main.dust[num9].noGravity = true;
                }
                else
                {
                    //checks if last proj is dead or not, if alive does formation stuff. 
                    if (Main.projectile[ParentProjectile.whoAmI].active)
                    {
                        if (timer == 360) timer = 0;
                        int rev = Projectile.ai[1] == 1f ? 1 : -1;
                        int speedIncrease = (int)Projectile.ai[0];
                        Vector2 vector2 = new Vector2((float)(Main.projectile[ParentProjectile.whoAmI].Center.X + Math.Sin(timer * speedIncrease * Math.PI / 180) * var1 * rev),
                                                      (float)(Main.projectile[ParentProjectile.whoAmI].Center.Y + Math.Cos(timer * speedIncrease * Math.PI / 180) * var1 * rev));
                        Projectile.velocity = (vector2 - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.Center.Distance(vector2);
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    else
                    {
                        //currently a projectile will just die if it hits a tile if its parent is dead
                        Projectile.tileCollide = true; // Projectile.Kill();
                    }
                }
            }
            else if (Formation == 2)
            {
                //timer += 4;
                //Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
                if (Projectile.ai[0] != 0)
                {
                    //checks if last proj is dead or not, if alive does formation stuff. 
                    if (Main.projectile[ParentProjectile.whoAmI].active)
                    {
                        if (timer == 360) timer = 0;
                        int Xdist = -var1;
                        int Ydist = -var2;
                        //double angle = Math.Atan((Ydist / Xdist)) * (180 / Math.PI);
                        Vector2 vector1 = new Vector2(Main.projectile[ParentProjectile.whoAmI].position.X + Xdist*20, 
                                                      Main.projectile[ParentProjectile.whoAmI].position.Y + Ydist*20);
                        int angle = (int)(vector1.AngleTo(Main.projectile[ParentProjectile.whoAmI].position) * (180/Math.PI)) - 180;
                        //if (timer == 240) Main.NewText(angle.ToString());
                        Vector2 vector2 = new Vector2(Main.projectile[ParentProjectile.whoAmI].position.X + (float)(Math.Cos((angle - 180 + timer) * (Math.PI / 180)) * 20 * Math.Sqrt(Math.Pow(Xdist, 2) + Math.Pow(Ydist, 2))),
                                                      Main.projectile[ParentProjectile.whoAmI].position.Y + (float)(Math.Sin((angle - 180 + timer) * (Math.PI / 180)) * 20 * Math.Sqrt(Math.Pow(Xdist, 2) + Math.Pow(Ydist, 2))));
                        //vector1 = new Vector2((float)(vector1.X * Math.Cos((timer+(int)angle) * (Math.PI / 180))), (float)(vector1.Y * Math.Sin((timer+(int)angle) * (Math.PI / 180))));
                        Projectile.velocity = (vector2 - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.Center.Distance(vector2);
                        //Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    else
                    {
                        //currently a projectile will just die if its parent is dead
                        Projectile.Kill();
                    }
                }
                else
                {
                    int num9;
                    num9 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 16, 0f, 0f, 100, Color.White, 2f);
                    Dust obj;
                    obj = Main.dust[num9];
                    obj.velocity *= 0.3f;
                    Main.dust[num9].position.X = Projectile.position.X + (float)(Projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
                    Main.dust[num9].position.Y = Projectile.position.Y + (float)(Projectile.height / 2) + (float)Main.rand.Next(-4, 5);
                    Main.dust[num9].noGravity = true;
                }
            }
        }
    }
    public class TestProjEntitySource : EntitySource_Parent
    {
        public TestFormationProjectile proj;
        public int var1;
        public int var2;
        public int var3;
        public TestProjEntitySource(Entity entity, TestFormationProjectile proj2, int var1b, int var2b, int var3b, string context = null) : base(entity, context)
        {
            var1 = var1b;
            var2 = var2b;
            var3 = var3b;
            proj = proj2;
        }
    }
}
