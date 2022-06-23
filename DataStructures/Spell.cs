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
		public int textureID;
		//public Color color;
		public int R;
		public int G;
		public int B;

		// Formation Stats, in order of accessing
		public bool isFormationSlave; // So as to not have infinite loops of formations
		public bool usesFormation;
		public Spell[,] formation;
		public int formationRotate; // 1 = Clockwise, 0 = No Rotation, -1 = AntiClockwise

		public Spell(string Type_ = "Blank", int textureID_ = 0, bool isFormationSlave_ = false, bool usesFormation_ = false, int R_ = 100, int G_ = 100, int B_ = 100)
		{
			int[] DefaultProjStats = { 0, 0, 1, 0, 60, 1, 0, 0 };
			projStats = DefaultProjStats;
			Type = Type_;
			textureID = textureID_;
			
			isFormationSlave = isFormationSlave_;
			usesFormation = usesFormation_;
			if (usesFormation) formation = new Spell[,] { { new Spell("null") } };
			else formation = null;
			formationRotate = 0;
			R = R_;
			G = G_;
			B = B_;
		}
	}

	public class CustomTextureID
    {
		public const short Default = 3;
		public const short Arrow = 1;
		public const short Sword = 2;
		public const short Ball = 3;
		public const short Star = 4;
		public const short Knife = 5;
	}

	public class CustomTexture
    {
		public static string GetString(int ID)
        {
            return ID switch
            {
                //0 => "Default",
                1 => "Arrow",
                2 => "Sword",
				3 => "Ball",
                4 => "Star",
				5 => "Knife",
                _ => "Ball",
            };
        }

		public static int GetID(string Name)
		{
			return Name switch
			{
				"Arrow" => 1,
				"Sword" => 2,
				"Ball" => 3,
				"Star" => 4,
				"Knife" => 5,
				_ => 3,
			};
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

	class SpellSerializer : TagSerializer<Spell, TagCompound> { // Saving spells for use in BaseMagicItem
		public override TagCompound Serialize(Spell spell) {
			TagCompound tag = new TagCompound();

			tag["Type"] = spell.Type;
			tag["textureID"] = spell.textureID;
			tag["R"] = spell.R;
			tag["G"] = spell.G;
			tag["B"] = spell.B;
			tag["projStats"] = spell.projStats.ToList();

			tag["isFormationSlave"] = spell.isFormationSlave;
			tag["usesFormation"] = spell.usesFormation;
			// These could be both true, so this makes sure that there isnt infinite formation saving
			if (spell.usesFormation && !spell.isFormationSlave && spell.Type != "Default") tag["formation"] = spell.formation;
			tag["formationRotate"] = spell.formationRotate;
			
			return tag;
		}
		
		public override Spell Deserialize(TagCompound tag) {
			Spell spell = new Spell();

			if (tag.ContainsKey("Type")) spell.Type = tag.Get<string>("Type");
			if (tag.ContainsKey("textureID")) spell.textureID = tag.Get<int>("textureID");
			if (tag.ContainsKey("R")) spell.R = tag.Get<int>("R");
			if (tag.ContainsKey("G")) spell.G = tag.Get<int>("G");
			if (tag.ContainsKey("B")) spell.B = tag.Get<int>("B");
			if (tag.ContainsKey("projStats")) spell.projStats = tag.Get<List<int>>("projStats").ToArray();

			if (tag.ContainsKey("isFormationSlave")) spell.isFormationSlave = tag.Get<bool>("isFormationSlave");
			if (tag.ContainsKey("usesFormation")) spell.usesFormation = tag.Get<bool>("usesFormation");
			if (tag.ContainsKey("formation") && tag.Get<bool>("usesFormation")) spell.formation = tag.Get<Spell[,]>("formation");
			if (tag.ContainsKey("formationRotate")) spell.formationRotate = tag.Get<int>("formationRotate");
			
			return spell;
		}
	}
}