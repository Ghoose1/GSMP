using GSMP.DataStructures;
using System.Collections.Generic;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;

namespace GSMP
{
    public class SpellPlayer : ModPlayer
    {
        public List<Spell> StoredSpells;
        public bool canPlaceSpells;

        public override void Initialize()
        {
            StoredSpells = new List<Spell>();
        }

        public override void ResetEffects()
        {
            canPlaceSpells = false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["SpellList"] = StoredSpells;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("SpellList")) StoredSpells = tag.Get<List<Spell>>("SpellList");
        }
    }

    public class SpellCommand : ModCommand // This is just for testing adding spells to the player's inventory
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "a";

        public override string Usage => "/a <add, remove, list> <type to add>";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            switch (args[0])
            {
                case "add":
                case "Add":
                    {
                        SpellPlayer player = caller.Player.GetModPlayer<SpellPlayer>();
                        Spell spell = new Spell(args[1]);
                        player.StoredSpells.Add(spell);
                        break;
                    }
                case "remove":
                case "Remove":
                    {
                        SpellPlayer player = caller.Player.GetModPlayer<SpellPlayer>();
                        player.StoredSpells.Clear();
                        break;
                    }
                case "list":
                case "List":
                    {
                        SpellPlayer player = caller.Player.GetModPlayer<SpellPlayer>();
                        for (int a = 0; a < player.StoredSpells.Count; a++)
                            Main.NewText(player.StoredSpells[a].Type);
                        break;
                    }
                default:
                    {
                        Main.NewText("Invalid Command", Color.Red);
                        break;
                    }
            }
        }
    }
}
