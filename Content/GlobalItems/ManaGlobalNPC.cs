using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GSMP.Content.GlobalItems
{
    public class ManaGlobalNPC : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return TileEntities.ManaTEutils.CanNPCHaveMana(entity);
        }

        public override bool InstancePerEntity => true;

        public int Mana;
        public int MaxMana;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            //if (TileEntities.NPCMana.SpecialManaNPCs.Contains(npc.type))
            MaxMana = TileEntities.NPCMana.ManaByType(npc.type);
            Mana = MaxMana;
        }

        public override void OnKill(NPC npc)
        {
            Main.NewText("Test");
        }
    }
}
