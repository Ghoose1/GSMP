using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using GSMP.DataStructures;
using Terraria.ID;

namespace GSMP.Content.Items
{
    public class SpellItem : ModItem
    {
        internal Spell spell;
        public override string Texture => "GSMP/Assets/SpellBookGreen";
        public SpellItem()
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
            Item.CloneDefaults(ItemID.DirtBlock);
            Item.createTile = ModContent.TileType<Tiles.SpellTile>();
        }

        //public override void OnCreate(ItemCreationContext context)
        //{
        //    Player player = Main.player[Main.myPlayer];
        //    player.GetModPlayer<SpellPlayer>().StoredSpells.Add(spell); // Adding this to the player's spell 'inventory'
        //}

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
