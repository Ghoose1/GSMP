using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GSMP.Content.Items;

namespace GSMP
{
    public class GSMP : Mod
    {

        public override void Load()
        {
            //TagSerializer(new ArraySerializer());
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