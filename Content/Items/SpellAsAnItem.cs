using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using GSMP.DataStructures;
using Terraria.ID;

namespace GSMP.Content.Items
{
    public class SpellAsAnItem : ModItem
    {
        internal Spell spell;
        public override string Texture => "GSMP/Assets/SpellBookGreen";
        public SpellAsAnItem()
        {
            Spell S1 = new Spell("Slave"); // Follow spell
            Spell S2 = new Spell("Master"); // Main spell

            Spell[,] DefaultFormation = { 
                { S1, S1, S1 },
                { S1, S2, S1 },
                { S1, S1, S1 },
            };
            spell.formation = DefaultFormation;
            spell.formationRotate = 0;
            spell.usesFormation = true;
            spell.Type = "Debug";
            int[] DefaultProjStats = { 0, 0, 1, 0, 60, 1, 0, 0 };
            spell.projStats = DefaultProjStats;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WaterBolt);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Spell"] = spell;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Spell")) spell = tag.Get<Spell>("Spell"); 
        }
    }
}
