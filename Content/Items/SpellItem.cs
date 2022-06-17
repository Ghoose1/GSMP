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
            spell = new Spell("Default", false);
            spell.formationRotate = 0;
            spell.usesFormation = true;
            int[] DefaultProjStats = { 0, 0, 1, 0, 60, 1, 0, 0 };
            spell.projStats = DefaultProjStats;

            Spell S1 = new Spell("Slave", true); // Follow spell
            Spell S2 = spell; // Main spell

            Spell[,] DefaultFormation = { // This is how formations will now be
                { S1, S1, S1 },
                { S1, S2, S1 },
                { S1, S1, S1 },
            };
            spell.formation = DefaultFormation;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DirtBlock);
            Item.createTile = ModContent.TileType<Tiles.SpellTile>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<SpellPlayer>().canPlaceSpells;
        }

        public override bool AltFunctionUse(Player player)
        {
            if (player.GetModPlayer<SpellPlayer>().debug) Main.NewText(spell.Type);
            return player.GetModPlayer<SpellPlayer>().debug;
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
