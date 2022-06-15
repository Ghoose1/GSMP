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
        public List<Spell> StoredSpells;
        public bool canPlaceSpells;

        public override void ResetEffects()
        {
            canPlaceSpells = false;
        }

        public override void SaveData(TagCompound tag)
        {
            StoredSpells.Add(new Spell("Debug"));
            tag["Spells"] = StoredSpells.ToArray();
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Spells")) StoredSpells = tag.Get<Spell[]>("Spells").ToList();
        }
    }
}
