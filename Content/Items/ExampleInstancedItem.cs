using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Terraria.DataStructures;

namespace GSMP.Content.Items
{
	public class CustomInstancedItem : ModItem
	{
		public override string Texture => "GSMP/Assets/SpellBookGray";
		public override string Name => "CustomStatItem";
		public int[] stats;
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			Tooltip.SetDefault("maybe it works this time");
		}
        public override void SetDefaults()
        {
			Item.CloneDefaults(ItemID.WaterBolt);
			Item.maxStack = 1;
			Item.shoot = ModContent.ProjectileType<CustomProjectile>();
			UpdateStats();
		}
        public override ModItem Clone(Item item)
		{
			CustomInstancedItem clone = (CustomInstancedItem)base.Clone(item);
			clone.stats = (int[])stats.Clone();
			return clone;
		}
		public override void OnCreate(ItemCreationContext context)
		{
			stats = new int[8];
			for (int i = 0; i < 8; i++)
			{
				stats[i] = 12;
			}
			Main.NewText("test1");
			//Item.shoot = ModContent.ProjectileType<CustomProjectile>();
			//UpdateStats();
		}
        public override bool AltFunctionUse(Player player)
        {
			player.QuickSpawnItem(new EntitySource_Loot(player), Item.type, 1);
            return base.AltFunctionUse(player);
        }
        /// <summary>
        /// Stats:
        /// 0 - Base Item |
        /// 1 - Damage |
        /// 2 - Class (1-4) |
        /// 3 - Mana Cost |
        /// 4 - Use Style |
        /// 5 - Melee |
        /// 6 - Projectile  |
        /// 7 - Projectile Damage (unused) |
        /// </summary>
        public void UpdateStats()
        {
            //Main.NewText("UpdateStats called");
            //for (int i = 0; i < stats.Length; i++) Main.NewText("Stat " + i + " is " + stats[i]);
            if (stats[0] != 0) Item.CloneDefaults(stats[0]);
            Item.damage = stats[1] != 0 ? stats[1] : Main.item[stats[0]].damage;
            if (stats[2] != 0)
            {
                switch (stats[2])
                {
                    case 1:
                        Item.DamageType = DamageClass.Melee;
                        break;
                    case 2:
                        Item.DamageType = DamageClass.Ranged;
                        break;
                    case 3:
                        Item.DamageType = DamageClass.Magic;
                        break;
                    case 4:
                        Item.DamageType = DamageClass.Summon;
                        break;
                }
            }
            Item.mana = stats[3] != 0 ? stats[3] : Main.item[stats[0]].mana;
            Item.useStyle = stats[4] != 0 ? stats[4] : Main.item[stats[0]].useStyle;
            Item.noMelee = stats[5] == 1;
            Item.shoot = ModContent.ProjectileType<CustomProjectile>();
            //else if (stats[0] != 0) stats[6] = Main.item[stats[0]].shoot;
        }
		public override void SaveData(TagCompound tag) => tag["Stats"] = stats.ToList();
		public override void LoadData(TagCompound tag)
		{
			stats = tag.Get<List<int>>("Stats").ToArray();
			UpdateStats();
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			int[] projstats = new int[2];
			projstats[0] = stats[6];
			var StatSource = new MagicProjEntitySource(player, projstats);
			Projectile.NewProjectile(StatSource, position, velocity, ModContent.ProjectileType<CustomProjectile>(), damage, knockback, player.whoAmI);
			//for (int i = 0; i < stats.Length; i++) FloatStats[i] = (float)stats[i];
			//Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			return false;
        }
        //public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit) => Main.NewText(stats[0]);
		public void ChangeItemStats(Player player, int[] Values)
        {
			if (player.whoAmI == Main.myPlayer)
			{
				for (int i = 0; i < Values.Length; i++) stats[i] = Values[i];
				//stats[0] = Values;
				UpdateStats();
            }
        }
    }

	public class CustomProjectile : ModProjectile
	{
		public override string Texture => "GSMP/Content/Items/Magic/CovidBall";
		public int[] ProjStats = new int[2];
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Main.NewText("GG | " + ProjStats[0].ToString());
		}
		public override void OnSpawn(IEntitySource source)
		{
			if (source is MagicProjEntitySource Source)
			{
				ProjStats = Source.Stats;
				Projectile proj = Main.projectile[ProjStats[0]];
				ModProjectile ModProj = ModContent.GetModProjectile(ProjStats[0]);
				//Projectile.CloneDefaults(proj.type);
				Projectile.aiStyle = proj.aiStyle;
				//AIType = ModProj.AIType;
				//Main.NewText("OnSpawn Called | " + ProjStats[0].ToString() + " | " + proj.type.ToString() + " | ");// + ModProj.AIType.ToString());
				//AIType = ProjStats[0];
				//UpdateStats();
				//Projectile.CloneDefaults(ProjStats[0]);
				//FirstNumber = importantNumberSource.importantNumber[0];
				//SecondNumber = importantNumberSource.importantNumber[1];
			}
		}
		public override void SetDefaults()
		{
			//Projectile proj = Main.projectile[ProjStats[0]];
			//Projectile.CloneDefaults(12);
		}
		public void UpdateStats()
        {
			//this method is probobly useless since a projectile wouldnt be changed while it is active (or would it?)\
			Projectile proj = Main.projectile[ProjStats[0]];
			Projectile.CloneDefaults(proj.type);
			Projectile.aiStyle = proj.aiStyle;
			//AIType = ModContent.GetModProjectile(ProjStats[0]).AIType;
		}
    }

	public class StatItemCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "c";
		public override string Usage => "/c <Base Item> <Damage> <Class (1-4)> <Mana Cost> <use style> <melee (0 or 1)> <Projectile>";
        public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (caller.Player.HeldItem.ModItem is CustomInstancedItem item)
			{
				int[] args2 = new int[args.Length];
				for (int i = 0; i < args.Length; i++) args2[i] = int.Parse(args[i]);
				item.ChangeItemStats(caller.Player, args2);
			}
			else Main.NewText("Invalid Item");
		}
	}

	//public class MagicProjPlayer : ModPlayer
	//{
	//	public override void PreUpdate()
	//	{
	//		if (Main.GameUpdateCount % 60 == 0 && Main.myPlayer == Player.whoAmI)
	//		{
	//			int[] stat = new int[2];
	//			stat[0] = 1;
	//			stat[1] = 3;
	//			//int importantNumber = 42;
	//			var source = new MagicProjEntitySource(Player, stat);
	//			Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<TestProjectile>(), 0, 0, Main.myPlayer);
	//		}
	//	}
	//}

	public class MagicProjEntitySource : EntitySource_Parent
	{
		public int[] Stats;
		public MagicProjEntitySource(Entity entity, int[] Stats2, string context = null) : base(entity, context)
		{
			Stats = Stats2;
		}
	}
}