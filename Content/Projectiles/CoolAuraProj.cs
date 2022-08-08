using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;

namespace GSMP.Content.Projectiles
{
    public class CoolAuraProj : ModProjectile
    {
        public override string Texture => "GSMP/Assets/Projectile Images/Ball";
        public int X;
        public int Y;
        public int dist;
        public int num;

        internal float rad;

        public override void OnSpawn(IEntitySource spawnSource)
        {
            if (spawnSource is AuraProjSpawnSource source)
            {
                X = source.X;
                Y = source.Y;
                dist = source.dist;
                num = source.num;

                rad = 0f;

                if (source.Master && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (num == 0)
                    {

                    }
                    else
                    {
                        for (int e = 0; e < num - 1; e++)
                        {
                            AuraProjSpawnSource NewSource = source;
                            NewSource.Master = false;
                            Projectile.NewProjectile(NewSource, new Vector2(X, Y), Projectile.velocity, ModContent.ProjectileType<CoolAuraProj>(), 0, 0);
                        }
                    }
                }
            }
        }

        public override void AI()
        {
            Projectile.position.X = X + (float)(Math.Cos(rad) * dist);
            Projectile.position.Y = Y + (float)(Math.Sin(rad) * dist);
            rad += (float)Math.PI / 90f;
        }
    }

    public class AuraProjSpawnSource : AEntitySource_Tile
    {
        public int X;
        public int Y;
        public int dist;
        public int num;

        public bool Master;

        public AuraProjSpawnSource(int X_, int Y_, int dist_, bool Master_ = false, int num_ = 0, string context = null) : base(X_, Y_, context)
        {
            X = X_;
            Y = Y_;
            dist = dist_;
            Master = Master_;
            num = num_;
        }
    }
}
