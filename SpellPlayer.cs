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
        public List<int> TestList;

        public override void Initialize()
        {
            StoredSpells = new List<Spell>();
        }
        //public Spell TestSave;

        //public SpellPlayer() 
        //{
        //    TestSave = new Spell("SavePls");
        //}

        public override void ResetEffects()
        {
            canPlaceSpells = false;

            //TestSave = new Spell("Debug");
            //StoredSpells[0] = TestSave;
        }

        public override void SaveData(TagCompound tag)
        {
            //Spell[] SaveThing = StoredSpells.ToArray();
            tag["SpellList"] = StoredSpells;  // WHy tf deos this not work
            //tag["Test"] = TestSave;
        }

        public override void LoadData(TagCompound tag)
        {
            //if (tag.ContainsKey("SpellList")) StoredSpells = tag.Get<List<Spell>>("SpellList");
            if (tag.ContainsKey("SpellList")) StoredSpells = tag.Get<List<Spell>>("SpellList");
        }
    }

    public class SpellCommand : ModCommand // This was just for testing adding spells to the player's inventory
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
