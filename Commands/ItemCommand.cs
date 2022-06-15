using Terraria;
using Terraria.ModLoader;
using GSMP.Content.Items;
/* Temporarily removed
namespace GSMP.Commands
{
    public class ItemCommand : ModCommand
    {
		public override CommandType Type => CommandType.Chat;
		public override string Command => "c";
		public override string Usage => "/c <damage> <crit> <autoswing (1 = true)> <knockback> <mana> <shootSpeed> <useStyle> <useTime> <useAnimation>";
		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (caller.Player.HeldItem.ModItem is BaseMagicItem item)
			{
				int[] args2 = new int[args.Length];
				for (int i = 0; i < args.Length; i++) args2[i] = int.Parse(args[i]);
				item.itemStats = args2;
				item.UpdateStats(item);
			}
			else Main.NewText("Invalid Item");
		}
	}
	public class ProjCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "v";
		public override string Usage => "/v <lifeSteal (0-100)> <manaSteal (0-100)> <CustomAIType> <maxPenetrate> <timeLeft> <ignoreWater (1 = true)> <tileCollide (1 = true)> <formation>";
		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (caller.Player.HeldItem.ModItem is BaseMagicItem item)
			{
				int[] args2 = new int[args.Length];
				for (int i = 0; i < args.Length; i++) args2[i] = int.Parse(args[i]); 
				item.projStats = args2;
				item.UpdateStats(item);
			}
			else Main.NewText("Invalid Item");
		}
	}
}
*/
