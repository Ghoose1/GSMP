using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;

namespace GSMP.Content.TileEntities
{
    public class RitualCoreTE : ModTileEntity
    {
        public int radius;
        public int MaxInclude;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return ManaTEutils.RitualCores.Contains(tile.TileType) && tile.HasTile;
        }

        // TODO: This TE finds other ritual TEs in a radius around it
        //       If it finds the required ones for a certin ritual it will trigger that ritual and apply the appropriate effects to the tiles.
        //       This can range from giving a special buff and depleting mana to things like summoning bosses and large changes to the world.
    }
}