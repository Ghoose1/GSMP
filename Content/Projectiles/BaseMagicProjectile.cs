using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GSMP;
using System.Collections.Generic;
using System.Linq;

namespace GSMP.Content.Projectiles
{
    public class BaseMagicProjectile : ModProjectile
    {
        public int[] stats = new int[8];
        internal int timer; // Rotation Timer
        internal int[] vars; // Formation configuration array
        internal int Xoffset;
        internal int Yoffset;
        internal bool locked;
        internal float baseVelo; 
        internal int CustomAIStyle;
        internal int[,] formTemplate;
        internal double diagDist;
        internal float parentVelocity;

        internal int fancySpawningTimer;
        internal int fancySpawningX;
        internal int fancySpawningY;
        internal int requiredProjs;
        internal bool follow;
        internal bool allLocked;
        internal int amountUnLocked;

        public override string Texture => "GSMP/Assets/Projectile Images/IceBolt";
        public BaseMagicProjectile ParentProjectile;

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is MagicProjEntitySource Source)
            {
                vars = Source.FormStats;
                stats = Source.Stats;
                formTemplate = Source.Form;
                CustomAIStyle = stats[2];
                Projectile.maxPenetrate = stats[3];
                Projectile.timeLeft = stats[4];
                Projectile.ignoreWater = stats[5] == 1;
                Projectile.tileCollide = stats[6] == 1;
                if (CustomAIStyle == 1 || CustomAIStyle == 2) // Projectile is using a formation 
                {
                    // When projectile is a main projectile, it initalises the other projectiles in the formation
                    if (Projectile.ai[0] == 0)
                    {
                        ParentProjectile = null;
                        Projectile.tileCollide = false;

                        // find Parent Proj point in formation
                        for (int i = 0; i < Source.Form.GetLength(1); i++)
                        {
                            for (int j = 0; j < Source.Form.GetLength(0); j++)
                            {
                                if (Source.Form[j, i] == 2)
                                {
                                    Xoffset = i;
                                    Yoffset = j;
                                    break;
                                }
                            }
                        }

                        // Create a bunch of projectiles for the rest of the formation
                        for (int x = 0; x < Source.Form.GetLength(1); x++)
                        {
                            for (int y = 0; y < Source.Form.GetLength(0); y++)
                            {
                                if (Source.Form[y, x] == 1)
                                {
                                    // x position, y position, rotate? (1 = true)
                                    int[] passStats = { x - Xoffset, y - Yoffset, vars[2] };
                                    // player (required), BMI stats, This Projectile, Formation Stats, Formation
                                    var SpawnSource = new MagicProjEntitySource(Main.player[Projectile.owner], Source.Stats, this, passStats, Source.Form);
                                    Projectile.NewProjectile(SpawnSource, Main.player[Projectile.owner].position,
                                        Projectile.velocity, ModContent.ProjectileType<BaseMagicProjectile>(),
                                        Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Onspawn stuff for non-main projectiles
                        baseVelo = 1;
                        locked = false;
                        Projectile.tileCollide = false;
                        ParentProjectile = Source.proj;

                        parentVelocity = (float)Math.Sqrt(Math.Pow(Main.projectile[ParentProjectile.Projectile.whoAmI].velocity.X, 2) + Math.Pow(Main.projectile[ParentProjectile.Projectile.whoAmI].velocity.Y, 2));
                        diagDist = Math.Sqrt(Math.Pow(-vars[0], 2) + Math.Pow(-vars[1], 2));
                    }
                    #region old code
                    // uhhh the code for onspawn stuff with the orbiting formation is somewhere here
                    //
                    //if (Formation == 1)
                    //{
                    //    if (Projectile.ai[0] == 0f)
                    //    {
                    //        ParentProjectile = null;
                    //        Projectile.tileCollide = true;
                    //        //form[0] = Projectile;
                    //        var SpawnSource = new TestProjEntitySource(Main.player[Projectile.owner], this, 100, 0, 1, Source.var4);
                    //        Projectile.NewProjectile(SpawnSource, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TestFormationProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 1);
                    //        Projectile.NewProjectile(SpawnSource, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TestFormationProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 0);
                    //    }
                    //    else
                    //    {
                    //        var1 = Source.var1;
                    //        Projectile.tileCollide = false;
                    //        ParentProjectile = Source.proj.Projectile;
                    //        //form[(int)Projectile.ai[0]] = Projectile;
                    //        if (Projectile.ai[0] < 3)
                    //        {
                    //            var SpawnSource = new TestProjEntitySource(Main.player[Projectile.owner], this, 50 / (int)Projectile.ai[0], 0, 1, Source.var4);
                    //            Projectile.NewProjectile(SpawnSource, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TestFormationProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1, 1);
                    //            Projectile.NewProjectile(SpawnSource, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TestFormationProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1, 0);
                    //        }
                    //    }
                    //}
                    //after adding the formation to the arrays, make another case for it then go to the test item to change stuff
                    //switch (Formation)
                    //{
                    //    case 2:
                    //        FormationSpawnHandeler(funny, Source, Formation);
                    //        break;
                    //    case 3:
                    //        FormationSpawnHandeler(funnybig, Source, Formation);
                    //        break;
                    //    case 4:
                    //        FormationSpawnHandeler(penis, Source, Formation);
                    //        break;
                    //    case 5:
                    //        FormationSpawnHandeler(linetest, Source, Formation);
                    //        break;
                    //    case 6:
                    //        FormationSpawnHandeler(hehe, Source, Formation);
                    //        break;
                    //    //case 7:
                    //    //    FormationSpawnHandeler(name, Source, Formation);
                    //    //    break;
                    //    default:
                    //        break;
                    //}
                    #endregion

                }
                if (CustomAIStyle == 3) // main projectile follows cursor until released and other projectiles spanw at player.
                {
                    fancySpawningX = 0;
                    fancySpawningY = 0;
                    fancySpawningTimer = 0;
                    ParentProjectile = null;
                    Projectile.tileCollide = false;
                    requiredProjs = SumOfForm(Source.Form)[1]; // Currently we only care about projectile type 1 (default)
                    amountUnLocked = requiredProjs;
                    if (Projectile.owner == Main.myPlayer) 
                    {
                        if (Projectile.ai[0] == 0)
                        {
                            if (Main.player[Projectile.owner].channel) follow = true;
                            ParentProjectile = null;

                            // find Parent Proj point in formation
                            for (int i = 0; i < Source.Form.GetLength(1); i++)
                            {
                                for (int j = 0; j < Source.Form.GetLength(0); j++)
                                {
                                    if (Source.Form[j, i] == 2)
                                    {
                                        Xoffset = i;
                                        Yoffset = j;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Onspawn stuff for non-main projectiles
                            baseVelo = 1;
                            locked = false;
                            Projectile.tileCollide = false;
                            ParentProjectile = Source.proj;

                            parentVelocity = (float)Math.Sqrt(Math.Pow(Main.projectile[ParentProjectile.Projectile.whoAmI].velocity.X, 2) + Math.Pow(Main.projectile[ParentProjectile.Projectile.whoAmI].velocity.Y, 2));
                            diagDist = Math.Sqrt(Math.Pow(-vars[0], 2) + Math.Pow(-vars[1], 2));
                        }
                    }
                }
            }
        }

        public override void AI()
        {
            if (stats[2] == 1 || stats[2] == 2)
            {
                if (Projectile.ai[0] != 0)
                {
                    if (timer < 360) timer += 4 * vars[2];
                    else timer = 0;
                    //checks if last proj is dead or not, if alive does formation stuff. 
                    if (Main.projectile[ParentProjectile.Projectile.whoAmI].active)
                    {
                        // This doesn't nessissarily need to happen but probobly saves processing large numbers
                        //if (timer == 360) timer = 0;

                        // Getting the amount the projectile should be offset on x and y axis. 
                        // Basically, if this didnt happen the whole formation would spawn to the bottom right of the main projectile
                        int Xoff = 0 - vars[0];
                        int Yoff = 0 - vars[1];

                        // Vector1 is where the projectile would be in relation to the Main projectile if there was no rotation
                        Vector2 vector1 = new Vector2(Main.projectile[ParentProjectile.Projectile.whoAmI].Center.X + (Xoff * 20),
                                                      Main.projectile[ParentProjectile.Projectile.whoAmI].Center.Y + (Yoff * 20));

                        // Angle between were the projectile would be with no rotation and main projectile position
                        float angle = (float)(vector1.AngleTo(Main.projectile[ParentProjectile.Projectile.whoAmI].Center) * (180f / Math.PI)) - 180;

                        // This is used in some stuff with the rotation code to ensure the projectiles are the correct distance from the main
                        //double diagDist = Math.Sqrt(Math.Pow(Xoff, 2) + Math.Pow(Yoff, 2));

                        // I still dont understand why this works, but it does. 
                        // Basically it uses angle and timer to find the angle it should be at in relation to the main projectile, then other variables to determine the distance as sin and cos functions have a max output of 1 and min output of -1
                        // Idk if that makes sence
                        Vector2 vector2 = new Vector2((float)(Main.projectile[ParentProjectile.Projectile.whoAmI].Center.X + Math.Cos((angle - 180 + timer) * (Math.PI / 180f)) * 20 * diagDist),
                                                      (float)(Main.projectile[ParentProjectile.Projectile.whoAmI].Center.Y + Math.Sin((angle - 180 + timer) * (Math.PI / 180f)) * 20 * diagDist));
                        // This basically makes the projectile home into the position where it is meant to be at a speed such that it will do it instantly but not do weird homing stuff once it is there.
                        //float parentvelocity = (float)Math.Sqrt(Math.Pow(Main.projectile[ParentProjectile.Projectile.whoAmI].velocity.X, 2) + Math.Pow(Main.projectile[ParentProjectile.Projectile.whoAmI].velocity.Y, 2));
                        if (stats[2] == 2)
                        {
                            if (!locked)
                            {
                                baseVelo = baseVelo >= Projectile.Center.Distance(vector2) ? Projectile.Center.Distance(vector2) : baseVelo * 1.1f;//  (float)Math.Pow(baseVelo, 1.1);
                                if (Projectile.Center.Distance(vector2) < 20) locked = true;
                                else
                                {
                                    Projectile.velocity = (vector2 - Projectile.Center).SafeNormalize(Vector2.Zero) * (parentVelocity + baseVelo / 2); //Projectile.Center.Distance(vector2)); // (float)Math.Sqrt(Math.Pow(Xoff, 2) + Math.Pow(Yoff, 2));
                                }
                            }
                            else Projectile.velocity = (vector2 - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.Center.Distance(vector2);
                        }
                        else Projectile.velocity = (vector2 - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.Center.Distance(vector2);

                        //  Projectile.velocity = (vector2 - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.Center.Distance(vector2);
                    }
                    else
                    {
                        //currently a projectile will just die if its parent is dead
                        Projectile.Kill();
                    }
                }
                else
                {
                    // whatever AI the main projectile should have. e.g. homing on cursor
                }
            }
            else if (stats[2] == 3)
            {
                Player player = Main.player[Projectile.owner];

                
                //Projectile.velocity.X *= 0.1f;
                    
                //Projectile.velocity.Y *= 0.1f;

                if (Projectile.ai[0] == 0)
                {
                    if (requiredProjs > 0) fancySpawningTimer += 4;
                    if (player.channel && follow)
                    {
                        Projectile.timeLeft = stats[4];
                        Vector2 pointPoisition;
                        pointPoisition = Main.MouseWorld;
                        player.LimitPointToPlayerReachableArea(ref pointPoisition);
                        Projectile.velocity = (pointPoisition - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.Center.Distance(pointPoisition);
                        if (fancySpawningTimer % 4 == 0 && requiredProjs > 0)
                        {
                            doFancySpawning();
                            Main.NewText(requiredProjs.ToString());

                        }
                        allLocked = amountUnLocked == 0;
                    }
                    else
                    {
                        // Code borrowed from exampleMod 1.4 

                        follow = false;
                        float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target
                        float projSpeed = 5f; // The speed at which the projectile moves towards the target

                        // Trying to find NPC closest to the projectile
                        NPC closestNPC = FindClosestNPC(maxDetectRadius);
                        if (closestNPC == null)
                            return;

                        // If found, change the velocity of the projectile and turn it in the direction of the target
                        // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                        Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                }
                else
                {
                    requiredProjs = ParentProjectile.requiredProjs;
                    if (timer < 360 && requiredProjs < 1 && ParentProjectile.allLocked) timer += 4 * vars[2];
                    else timer = 0;

                    if (Main.projectile[ParentProjectile.Projectile.whoAmI].active)
                    {

                        Vector2 vector1 = new Vector2(Main.projectile[ParentProjectile.Projectile.whoAmI].Center.X + (vars[0] * -20),
                                                      Main.projectile[ParentProjectile.Projectile.whoAmI].Center.Y + (vars[1] * -20));

                        float angle = (float)(vector1.AngleTo(Main.projectile[ParentProjectile.Projectile.whoAmI].Center) * (180f / Math.PI)) - 180;

                        Vector2 vector2 = new Vector2((float)(Main.projectile[ParentProjectile.Projectile.whoAmI].Center.X + Math.Cos((angle - 180 + timer) * (Math.PI / 180f)) * 20 * diagDist),
                                                      (float)(Main.projectile[ParentProjectile.Projectile.whoAmI].Center.Y + Math.Sin((angle - 180 + timer) * (Math.PI / 180f)) * 20 * diagDist));
                        
                        //float parentvelocity = (float)Math.Sqrt(Math.Pow(Main.projectile[ParentProjectile.Projectile.whoAmI].velocity.X, 2) + Math.Pow(Main.projectile[ParentProjectile.Projectile.whoAmI].velocity.Y, 2));
                        
                        if (!locked)
                        {
                            baseVelo = baseVelo >= Projectile.Center.Distance(vector2) ? Projectile.Center.Distance(vector2) : baseVelo * 1.1f; //(float)Math.Pow(baseVelo, 1.1);
                            if (Projectile.Center.Distance(vector2) < 20)
                            {
                                locked = true;
                                ParentProjectile.amountUnLocked--;
                            }
                            else
                            {
                                Projectile.velocity = (vector2 - Projectile.Center).SafeNormalize(Vector2.Zero) * (baseVelo / 2); //Projectile.Center.Distance(vector2)); // (float)Math.Sqrt(Math.Pow(Xoff, 2) + Math.Pow(Yoff, 2));
                            }
                        }
                        else Projectile.velocity = (vector2 - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.Center.Distance(vector2);
                    }
                    else
                    {
                        
                    }
                }
            }
            else Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }

        internal void doFancySpawning() // this is for spawning projectile with CustomAIStyle 3
        {
            if (formTemplate[fancySpawningY, fancySpawningX] == 1) // == projectile type
            {
                int[] passStats = { fancySpawningX - Xoffset, fancySpawningY - Yoffset, vars[2] };
                var SpawnSource = new MagicProjEntitySource(Main.player[Projectile.owner], stats, this, passStats, formTemplate);
                Projectile.NewProjectile(SpawnSource, Main.player[Projectile.owner].position,
                    Projectile.velocity, ModContent.ProjectileType<BaseMagicProjectile>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 0);

                requiredProjs--;
                // 1, 1, 1, 0, 1, 
                // 0, 0, 1, 0, 1, 
                // 1, 1, 2, 1, 1, 
                // 1, 0, 1, 0, 0, 
                // 1, 0, 1, 1, 1, 

                // 6  5  4  5  6 
                // 5  3  2  3  5 
                // 4  2  1  2  4
                // 5  3  2  3  5 
                // 6  5  4  5  6 
                
                if (fancySpawningX < formTemplate.GetLength(1)-1) fancySpawningX++;
                else
                {
                    fancySpawningX = 0;
                    if (fancySpawningY < formTemplate.GetLength(0)-1) fancySpawningY++;
                }
            }
            else
            {
                if (fancySpawningX < formTemplate.GetLength(1)-1)
                {
                    fancySpawningX++;
                    doFancySpawning();
                }
                else
                {
                    fancySpawningX = 0;
                    if (fancySpawningY < formTemplate.GetLength(0)-1)
                    {
                        fancySpawningY++;
                        doFancySpawning();
                    }
                }
            }
        }
        
        internal int[] SumOfForm(int[,] Array)
        {
            List<int> totalList = new List<int> { 0 };
            int[] total = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // all the types of projectiles, i couldnt get this to work with lists
            for (int i = 0; i < Array.GetLength(1); i++)
            {
                for (int j = 0; j < Array.GetLength(0); j++)
                {
                    if (Array[j, i] != 0 && Array[j, i] != 2) // 0 is no proj, 2 is main proj, >2 is differnet projs.
                    {
                        total[Array[j, i]] += 1;
                    }
                }
            }
            return total;
        }

        // Code Borrowed from exampleMod 1.4

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
    public class MagicProjEntitySource : EntitySource_Parent
    {
        public int[] Stats;
        public BaseMagicProjectile proj;
        public int[] FormStats;
        public int[,] Form;
        public MagicProjEntitySource(Entity entity, int[] Stats2, BaseMagicProjectile proj2, int[] FormStats2, int[,] Form2, string context = null) : base(entity, context)
        {
            Stats = Stats2;
            proj = proj2;
            FormStats = FormStats2;
            Form = Form2;
        }
    }
}
