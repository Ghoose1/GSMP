using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace GSMP.Content.Projectiles
{
    public class TeleProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("cool projectile");
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(17);
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            //AIType = ProjectileID.DirtBall;
        }

        //public override void AI()
        //{
        //    AIType = 17;
        //}
    }
}
