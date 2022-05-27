using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using GSMP.Content.Items.Consumable;
using GSMP.Content.Buffs;
using Terraria.DataStructures;
using System.Collections.Generic;

namespace GSMP.Content.Buffs
{
    public class CurseFunction
    {
        static int AmountInfected;
        public static int HowManyInfected(string Curse)
        {
            return AmountInfected;
        }
        public static void Add(int amount, string Curse)
        {
            AmountInfected += amount;
        }
        public static void Remove(int amount, string Curse)
        {
            AmountInfected -= amount;
        }
    }
    public class Covid20 : ModBuff //Covid20 code is : Covid20:1:true:true:false:false:600 
                                   // name : regen change : spread by melee : spread by magic : spread by other : spread by tiles : is permanent? : time to give when spreading
    {
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Covid-20");
            Description.SetDefault("the sequel");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<Cursedplayer>().HasCovid = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.color = Microsoft.Xna.Framework.Color.Blue;
            npc.GetGlobalNPC<CursedNPC>().HasCovid = true;
        }
    }
    public class Cursedplayer : ModPlayer
    {
        public bool HasCovid;
        public bool HasVaccine;
        public static string GiveNameOfInfected(Player player)
        {
            if (player.HasBuff(ModContent.BuffType<Covid20>()))
                return player.name;
            else
                return null;
        }
        public override void ResetEffects()
        {
            //CurseFunction.Remove(1);
            HasCovid = false;
        }
        public override void UpdateLifeRegen()
        {
            if (HasCovid)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 1;
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            //This is only for testing convenience so i can hit enemy with sword
            if (HasCovid && item.DamageType == DamageClass.Magic)
            {
                target.AddBuff(ModContent.BuffType<Covid20>(), 600);
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.DamageType == DamageClass.Magic)
                target.AddBuff(ModContent.BuffType<Covid20>(), 600);
        }


        //public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        //{
        //    if (proj.DamageType == DamageClass.Magic)
        //    {
        //        if (!target.HasBuff<Vaccinated>() && HasCovid)
        //            target.AddBuff(ModContent.BuffType<Covid20>(), 600);
        //    }
        //}
        //public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        //{
        //    base.OnHitByProjectile(proj, damage, crit);
        //}
    }
    public class CursedNPC : GlobalNPC
    {
        public bool HasCovid;
        public override bool InstancePerEntity => true;
        //public override bool CloneNewInstances => true;
        internal Color normalcolor;
        
        public override void SetDefaults(NPC npc)
        {
            HasCovid = npc.HasBuff(ModContent.BuffType<Covid20>());
            normalcolor = npc.color;
        }
        public override void ResetEffects(NPC npc)
        {
            //CurseFunction.Remove(1);
            HasCovid = false;
        }
        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            if (HasCovid && !target.HasBuff(ModContent.BuffType<Vaccinated>())) {
                target.AddBuff(ModContent.BuffType<Covid20>(), 600);
            }
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff(ModContent.BuffType<Covid20>()))
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
                npc.lifeRegen -= 1;
            }
            else
                npc.color = normalcolor;
        }
        
    }
    //public class FlamingProj : GlobalProjectile
    //{
    //    public override bool InstancePerEntity => true;
    //    public override bool CloneNewInstances => true;
    //    public bool Flaming;
    //    public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
    //    {
    //        //checking for on fire buff
    //        Flaming = Main.npc[projectile.owner].HasBuff(BuffID.OnFire);  // ModContent.BuffType<Covid20>());
    //        //if it is on fire, give the target the on fire debuff
    //        if (Flaming)
    //        {
    //            target.AddBuff(BuffID.OnFire, 600);     //ModContent.BuffType<Covid20>(), 600);
    //        }
    //        else
    //            target.AddBuff(BuffID.Wet, 600);     //ModContent.BuffType<Vaccinated>(), 600);
    //    }
    //    public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
    //    {
    //        if (Main.player[projectile.owner] != null)
    //            if (Main.player[projectile.owner].HasBuff<Covid20>())
    //                target.AddBuff(ModContent.BuffType<Covid20>(), 600);
    //        if (Main.npc[projectile.owner] != null)
    //            if (Main.npc[projectile.owner].HasBuff<Covid20>())
    //                target.AddBuff(ModContent.BuffType<Covid20>(), 600);
    //    }
    //}
}
