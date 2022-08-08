using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using GSMP.DataStructures;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;

namespace GSMP.Content.TileEntities
{
    /*
     * Rituals are going to work more like this so they are more complex:
     * instead of just checking for tiles and stuff around them, rituals will have a radius depending on their tier
     * this radius will be made up of 'rings' in which different things can be required
     * 
     * 1st ring will generally have things that control the type and specifications of the ritual
     * 2nd has resources for the ritual (e.g mana, life etc)
     * 3rd ring can have objects capable of boosting the effects of the ritual
     * 4th I will think of more
     * 
     * More rings will allow for more things to be included in the ritual that wouldn't be with lower tier rituals.
     * 
     * In addition, rituals will not just be a tile for each that checks for specific things, instead there will be 'stands' or bases for rituals
     * when you add certain items or artifacts to these bases, they become like the ritual cores that are currently a thing, and start trying to fufil a specifc ritual.
     * 
    */
    public class RitualCoreTE : ModTileEntity
    {
        public int radius;

        public bool active;

        internal int timer;
        internal float drainbuffer;



        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return TEutils.RitualCores.Contains(tile.TileType) && tile.HasTile;
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("radius", radius);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("radius")) radius = tag.Get<int>("radius");
        }
    }
}