using System.Collections.Generic;
using Terraria.ID;

namespace GSMP.Content.TileEntities
{
    public static class NPCMana
    {
        public static List<int> SpecialManaNPCs = new List<int>
        {
            NPCID.Tim,
            NPCID.RuneWizard,
            NPCID.DarkCaster,
        };

        public static int ManaByType(int type)
        {
            return type switch
            {
                NPCID.Tim => 1000,
                NPCID.RuneWizard => 5000,
                NPCID.DarkCaster => 500,
                _ => 20,
            };
        }
    }
}
