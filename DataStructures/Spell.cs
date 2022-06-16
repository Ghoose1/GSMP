using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GSMP;
using System.Collections.Generic;
using System.Linq;

namespace GSMP.DataStructures {
	public struct Spell {
        // Is this really the only data i need?
		public int[] projStats;
        public string Type;

        // Formation Stats, in order of accessing
        public bool usesFormation;
		public Spell[,] formation;
		public int formationRotate; // 1 = Clockwise, 0 = No Rotation, -1 = AntiClockwise

        public Spell(string Type_ = "Blank")
        {
            int[] DefaultProjStats = { 0, 0, 1, 0, 60, 1, 0, 0 };
            projStats = DefaultProjStats;
            Type = Type_;   

            usesFormation = false;
            formation = null;
            formationRotate = 0;
        }
	}
	
	class FormationSerializer : TagSerializer<Spell[,], TagCompound> // TagCompound saving Spell[,] for Formations
    {
        public override TagCompound Serialize(Spell[,] value) // Saves the 2D array as a bunch of 1D arrays for each row
        {
            TagCompound tag = new TagCompound();

            tag["Length"] = value.GetLength(0);
            for (int j = 0; j < value.GetLength(0); j++)
            {
                Spell[] temp = new Spell[value.GetLength(1)];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = value[j, i];
                }
                tag["Row " + j.ToString()] = temp.ToList(); // ToList here beacuse they are easier to read and edit in the files
            }
            return tag;
        }

        public override Spell[,] Deserialize(TagCompound tag) // Does the opposite fo serialise, translates 1D arrays into a 2D array
        {
            Spell[,] temp = { { new Spell() } };

            if (tag.ContainsKey("Length") && tag.ContainsKey("Row 0"))
            {
                temp = new Spell[tag.Get<int>("Length"), tag.Get<List<Spell>>("Row 0").Count]; 
                int j = 0;

                while (true)
                {
                    if (tag.ContainsKey("Row " + j.ToString()))
                    {
                        Spell[] array = tag.Get<List<Spell>>("Row " + j.ToString()).ToArray();

                        for (int i = 0; i < temp.GetLength(1); i++)
                        {
                            temp[j, i] = array[i];
                        }
                        j++;
                    }
                    else break;
                }
            }

            return temp;
        }
    }

    //class SpellArraySerializer : TagSerializer<Spell[], TagCompound> // lmao 3 serializers why am i doing this
    //{
    //    public override TagCompound Serialize(Spell[] array)
    //    {
    //        TagCompound tag = new TagCompound();

    //        tag["Length"] = array.Length;
    //        for (int a = 0; a < array.Length; a++)
    //        {
    //            tag[a.ToString()] = array[a];
    //        }

    //        return tag;
    //    }

    //    public override Spell[] Deserialize(TagCompound tag)
    //    {
    //        Spell[] array = { new Spell() };

    //        if (tag.ContainsKey("Length") && tag.ContainsKey("0"))
    //        {
    //            int a = 0;
    //            while (true)
    //            {
    //                if (tag.ContainsKey(a.ToString())) array[a] = tag.Get<Spell>(a.ToString());
    //                else break;
    //                a++;
    //            }
    //        }

    //        return array;
    //    }
    //}

    class SpellSerializer : TagSerializer<Spell, TagCompound> { // Saving spells for use in BaseMagicItem
		public override TagCompound Serialize(Spell spell) {
			TagCompound tag = new TagCompound();

            tag["Type"] = spell.Type;
			tag["projStats"] = spell.projStats.ToList();
			
			tag["usesFormation"] = spell.usesFormation;
			if (spell.usesFormation) tag["formation"] = spell.formation;
			tag["formationRotate"] = spell.formationRotate;
			
			return tag;
		}
		
		public override Spell Deserialize(TagCompound tag) {
			Spell spell = new Spell();

            if (tag.ContainsKey("Type")) spell.Type = tag.Get<string>("Type");
			if (tag.ContainsKey("projStats")) spell.projStats = tag.Get<List<int>>("projStats").ToArray();
			
			if (tag.ContainsKey("usesFormation")) spell.usesFormation = tag.Get<bool>("usesFormation");
			if (tag.ContainsKey("formation") && tag.Get<bool>("usesFormation")) spell.formation = tag.Get<Spell[,]>("formation");
			if (tag.ContainsKey("formationRotate")) spell.formationRotate = tag.Get<int>("formationRotate");
			
			return spell;
		}
	}
}