using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using GSMP.DataStructures;
using Terraria.ID;
using System.Collections.Generic;
using GSMP.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

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

        public override void SaveData(TagCompound tag) => tag["Spell"] = spell;

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("Spell")) spell = tag.Get<Spell>("Spell");
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D spellTexture = ModContent.Request<Texture2D>("GSMP/Assets/Projectile Images/" +
                CustomTexture.GetString(spell.textureID)).Value;
            Texture2D texture = ModContent.Request<Texture2D>("GSMP/Assets/SpellBookGray").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            Color color = new Color(spell.R, spell.G, spell.B);

            scale = scale - texture.Width / (float)Math.Pow(frame.Width, 2);

            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            Vector2 spellPos = new Vector2(position.X + ((26.173f - spellTexture.Width/4) * scale), position.Y - ((spellTexture.Height - 34)/2) * scale);
            spellPos = new Vector2(position.X + (18) *scale, position.Y + 19 * scale);
            spriteBatch.Draw(spellTexture, spellPos,
                new Rectangle(0, 0, spellTexture.Width, spellTexture.Height),
                color, 0.75f, new Vector2(spellTexture.Width / 2, spellTexture.Height / 2), scale, SpriteEffects.None, 1f);


            //spriteBatch.Draw(
            //    spellTexture,
            //    position,  // + zero,
            //    new Rectangle(0, 0, texture.Width, texture.Height),
            //    color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).Register();
        }
    }

    public class TexureCMD : ModCommand
    {
        public override string Command => "t";
        public override CommandType Type => CommandType.Chat;
        public override string Description => "/t <TextureID> <R> <G> <B>";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (caller.Player.HeldItem.ModItem is SpellItem item)
            {
                item.spell.textureID = CustomTexture.GetID(args[0]);
                if (args.Length == 4) 
                {
                    item.spell.R = int.TryParse(args[1], out int r) ? r : 100;
                    item.spell.G = int.TryParse(args[2], out int g) ? g : 100;
                    item.spell.B = int.TryParse(args[3], out int b) ? b : 100;
                }
            }
        }
    }
}
