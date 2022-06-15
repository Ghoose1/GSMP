using GSMP.DataStructures;
using System.Collections.Generic;
using Terraria.ModLoader;
using GSMP.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;
using System.Linq;

namespace GSMP
{
    public class SpellPlayer : ModPlayer
    {
        //public Spell[] StoredSpells;
        public bool canPlaceSpells;
        public Spell TestSave;

        //public SpellPlayer() 
        //{
        //    TestSave = new Spell("SavePls");
        //}

        public override void ResetEffects()
        {
            canPlaceSpells = false;
            TestSave = new Spell("Debug");
        }

        public override void SaveData(TagCompound tag)
        {
            //tag["SpellList"] = StoredSpells.ToList();
            tag["Test"] = TestSave;
        }

        public override void LoadData(TagCompound tag)
        {
            //if (tag.ContainsKey("SpellList")) StoredSpells = tag.Get<List<Spell>>("SpellList").ToArray();
            if (tag.ContainsKey("Test")) TestSave = tag.Get<Spell>("Test");
        }
    }
}
