using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GraphicsLib.Primitives;

namespace GSMP.Content.Items
{
    public class LineTest : ModItem
    {
        public override string Texture => "GSMP/Assets/SpellBookOrange";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WaterBolt);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<LineTestProj>(), damage, knockback, Main.myPlayer, 1f);
            return false;
        }
    }

    public class LineTestProj : ModProjectile
    {
        public override string Texture => "GSMP/Assets/Projectile Images/Ball";
        internal Projectile Partner;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WaterBolt);
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 1f)
            {
                LineTestSource source1 = new LineTestSource(Main.player[Projectile.owner], this);
                Projectile.NewProjectile(source1, Projectile.position, Vector2.Zero, ModContent.ProjectileType<LineTestProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f);
            }
            else if (source is LineTestSource source1)
                Partner = source1.proj.Projectile;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] != 1f && Partner != null)
            {
                Vector2[] points = new Vector2[] { Projectile.position, Partner.position };
                PrimitiveDrawing.DrawLineList(points, Color.Blue);
            }
            return true;
        }

        public override void AI()
        {
            AIType = 27;
        }
    }

    public class LineTestSource : EntitySource_Parent
    {
        public LineTestProj proj;
        public LineTestSource(Entity entity, LineTestProj proj_ = null, string context = null) : base(entity, context)
        {
            proj = proj_;
        }
    }
}
