using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace GSMP
{
	public class GSMPConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		//[Header("$Mods.ExampleMod.Config.ItemHeader")]
		//[Label("$Mods.ExampleMod.Config.ExampleWingsToggle.Label")] 
		//[Tooltip("$Mods.ExampleMod.Config.ExampleWingsToggle.Tooltip")] 
		[DefaultValue(300)] 
		public int RitualCheckTimer;
	}
}