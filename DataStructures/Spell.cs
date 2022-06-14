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
		public int[] projStats;
		
		// Formation Stats, in order of accessing
		public bool usesFormation;
		public int[,] formation;
		public int formationRotate; // 1 = Clockwise, 0 = No Rotation, -1 = AntiClockwise
	}
	
	public struct SpellMap { // Unused currently cus i dont know how to do that kinda of stuff, I'll ask Yo
		public Spell[] Array;
	}
	
	class ArraySerializer : TagSerializer<int[,], TagCompound> // TagCompound saving int[,] for Formations
    {
        public override TagCompound Serialize(int[,] value)
        {
            TagCompound tag = new TagCompound();

            tag["num"] = value.GetLength(0);
            for (int j = 0; j < value.GetLength(0); j++)
            {
                int[] temp = new int[value.GetLength(1)];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = value[j, i];
                }
                tag["Row " + j.ToString()] = temp.ToList();
            }
            return tag;
        }

        public override int[,] Deserialize(TagCompound tag) 
        {
            int[,] temp = { { 2 } };

            if (tag.ContainsKey("num") && tag.ContainsKey("Row 0"))
            {
                temp = new int[tag.Get<int>("num"), tag.Get<List<int>>("Row 0").ToArray().Length]; // Doesnt assume sqaure
                int j = 0;

                while (true)
                {
                    if (tag.ContainsKey("Row " + j.ToString()))
                    {
                        int[] array = tag.Get<List<int>>("Row " + j.ToString()).ToArray();

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

	class SpellSerializer : TagSerializer<Spell, TagCompound> {
		public override TagCompound Serialize(Spell spell) {
			TagCompound tag = new TagCompound();
			
			tag["projStats"] = spell.projStats.ToList();
			
			tag["usesFormation"] = spell.usesFormation;
			tag["formation"] = spell.formation;
			tag["formationRotate"] = spell.formationRotate;
			
			return tag;
		}
		
		public override Spell Deserialize(TagCompound tag) {
			Spell spell = new Spell();
			
			if (tag.ContainsKey("projStats")) spell.projStats = tag.Get<List<int>>("projStats").ToArray();
			
			if (tag.ContainsKey("usesFormation")) spell.usesFormation = tag.Get<bool>("usesFormation");
			if (tag.ContainsKey("formation")) spell.formation = tag.Get<int[,]>("formation");
			if (tag.ContainsKey("formationRotate")) spell.formationRotate = tag.Get<int>("formationRotate");
			
			return spell;
		}
	}
}