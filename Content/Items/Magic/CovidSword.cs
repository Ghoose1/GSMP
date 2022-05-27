using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using GSMP.Content.Buffs;

namespace GSMP.Content.Items.Magic
{
    public class CovidSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Spreads the infection");
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(24);
            Item.damage = 1;
            Item.autoReuse = true;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Covid20>(), 1200);
        }
        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Covid20>(), 600);
        }
        
    }
    public class CovidBall : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WaterBolt);
            Projectile.friendly = false;
            Projectile.damage = 1;
        }
        public override void AI()
        {
            Projectile.aiStyle = ProjectileID.WaterBolt;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Covid20>(), 600);
        }
    }
}
