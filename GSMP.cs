using Terraria.ModLoader;
using Terraria.UI;
using GSMP.Content.UI;
using Terraria;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GSMP
{
    public class GSMP : Mod
    {
        internal PracticeBar PracticeBar;

        private UserInterface _PracticeBar;

        public override void Load()
        {
            PracticeBar = new PracticeBar();
            PracticeBar.Activate();
            _PracticeBar = new UserInterface();
            _PracticeBar.SetState(PracticeBar);
        }
        public static void Log(object message)
        {
            ModContent.GetInstance<GSMP>().Logger.Info(message);
        }

        // note:
  //      if (Main.netMode == 1)
		//{
		//	for (int l = 10; l< 50; l++)
		//	{
		//		if (this.inventory[l].type > 0 && this.inventory[l].stack > 0 && !this.inventory[l].favorited && !this.inventory[l].IsACoin)
  //              {
  //                  NetMessage.SendData(5, -1, -1, null, base.whoAmI, l, this.inventory[l].prefix);
  //                  NetMessage.SendData(85, -1, -1, null, l);
  //                  this.inventoryChestStack[l] = true;
  //              }
  //          }
  //      }
    }
}