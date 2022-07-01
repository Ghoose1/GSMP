using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GSMP.Content.TileEntities
{
    public class ManaExtractorEntity : ManaStorageEntity
    {
        public override void Update()
        {
            for (int k = 0; k < ConnectionsTo.Count; k++)
            {
                Tile tile = Main.tile[(int)ConnectionsTo[k].X, (int)ConnectionsTo[k].Y];
                if (tile == null || !tile.HasTile || !ManaTEutils.ValidTiles.Contains(tile.TileType))
                    ConnectionsTo.RemoveAt(k);
            }

            // Todo: if multiple are near eachother, make them all contribute to one aura to avoid stacking being op
            // Stealing mana from entities
            int Mana = 0;

            foreach (NPC npc in Main.npc)
            {
                if (npc.active 
                    && ManaTEutils.CanNPCHaveMana(npc) 
                    && npc.position.DistanceSQ(new Vector2(Position.X * 16 + 12, Position.Y * 16 + 12)) < 6144 
                    && npc.GetGlobalNPC<GlobalItems.ManaGlobalNPC>().Mana > 0)
                {
                    npc.GetGlobalNPC<GlobalItems.ManaGlobalNPC>().Mana--;
                    Mana++;
                }
            }

            foreach (Player player in Main.player)
            {
                if (player.active
                    && player.position.DistanceSQ(new Vector2(Position.X * 16 + 12, Position.Y * 16 + 12)) < 6144
                    && player.statMana > 0)
                {
                    player.statMana--; // Maybe add resistance to this? if (Main.rand.NextBool(restance)) ?
                    player.manaRegenDelay = 40;
                    Mana++;
                }
            }

            if (ConnectionsTo.Count > 0)
            {
                TransferMana(Mana, ConnectionsTo);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (ConnectionsTo != new List<Vector2>()) tag.Add("ConnectionsTo", ConnectionsTo);
        }

        public override void LoadData(TagCompound tag)
        {
            ConnectionsTo = new List<Vector2>();
            if (tag.ContainsKey("ConnectionsTo"))
                ConnectionsTo = tag.Get<List<Vector2>>("ConnectionsTo");
        }
    }
}