using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using GSMP.DataStructures;
using Terraria.ID;
using System.Collections.Generic;
using GSMP.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GSMP.Content.Items
{
    public class SpellItem : ModItem
    {
        internal Spell spell;
        public override string Texture => "GSMP/Assets/SpellBookGreen";
        public SpellItem()
        {
            spell = new Spell("Default");
            spell.formationRotate = 0;
            int[] DefaultProjStats = { 0, 0, 1, 0, 600, 1, 0, 0 };
            spell.projStats = DefaultProjStats;
            //spell.color = Color.SkyBlue;
            spell.textureID = 0;

            //Spell S1 = new Spell("Slave", true); // Follow spell
            //Spell S2 = spell; // Main spell

            //Spell[,] DefaultFormation = { // This is how formations will now be
            //    { S1, S1, S1 },
            //    { S1, S2, S1 },
            //    { S1, S1, S1 },
            //};
            //spell.formation = DefaultFormation;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.DirtBlock);
            Item.maxStack = 1;
            Item.createTile = ModContent.TileType<SpellTile>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<SpellPlayer>().canPlaceSpells;
        }

        public override bool AltFunctionUse(Player player)
        {
            //if (player.GetModPlayer<SpellPlayer>().debug) Main.NewText(spell.Type);
            spell.isFormationSlave = !spell.isFormationSlave;

            return false; //player.GetModPlayer<SpellPlayer>().debug;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltipLine = new TooltipLine(Mod, "GSMP", "Type " + spell.Type);
            tooltips.Add(tooltipLine);
            tooltipLine = new TooltipLine(Mod, "GSMP", "usesFormation: " + spell.usesFormation.ToString());
            tooltips.Add(tooltipLine);
            tooltipLine = new TooltipLine(Mod, "GSMP", "IsSlave " + spell.isFormationSlave); 
            tooltips.Add(tooltipLine);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Spell"] = spell;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Spell")) spell = tag.Get<Spell>("Spell");
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).Register();
        }
    }

    public class TexureCMD : ModCommand
    {
        public override string Command => "tex";
        public override CommandType Type => CommandType.Chat;
        public override string Description => "/tex <TextureID> <R> <G> <B>";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (caller.Player.HeldItem.ModItem is SpellItem item)
            {
                item.spell.textureID = CustomTexture.GetID(args[0]);
                if (args.Length == 4) 
                    item.spell.color = new Color(int.TryParse(args[1], out int r) ? r : 100, int.TryParse(args[2], out int g) ? g : 100, int.TryParse(args[3], out int b) ? b : 100);
            }
        }
    }
}
