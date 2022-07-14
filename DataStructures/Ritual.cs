using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace GSMP.DataStructures
{
    public struct RitualObject
    {
        // Kind || data
        // 0 || null || null
        // 1 || ManaStorage || type, stored mana, maxmana 
        // 2 || Candle || buff (Type), stored mana
        // 3 || Tile || Type
        // ect
        public int kind;
        //public float[] data; // Maybe i could just have a bunch of parameters for the different data things that can be encountered?
        public int type;
        public int storedMana;
        public int maxMana;

        /// <summary>
        /// <para>null || 0 || null</para>
        /// <para>ManaStorage || 1 || type, stored mana, maxmana</para>
        /// <para>Candle || 2 || buff (Type), stored mana</para>
        /// <para>Tile || 3 || Type</para>
        /// </summary>
        /// <param name="kind_"></param>
        /// <param name="type_"></param>
        /// <param name="storedMana_"></param>
        /// <param name="maxMana_"></param>
        public RitualObject(int kind_ = 0, int type_ = 0, int storedMana_ = 0, int maxMana_ = 0)
        {
            kind = kind_;
            type = type_;
            storedMana = storedMana_;
            maxMana = maxMana_;
        }
    }

    public class RitualObjectSerializer : TagSerializer<RitualObject, TagCompound>
    {
        public override TagCompound Serialize(RitualObject obj)
        {
            TagCompound tag = new();

            if (obj.kind != 0) tag.Add("kind", obj.kind);
            if (obj.type != 0) tag.Add("type", obj.type);
            if (obj.storedMana != 0) tag.Add("storedMana", obj.storedMana);
            if (obj.maxMana != 0) tag.Add("maxMana", obj.maxMana);

            return tag;
        }

        public override RitualObject Deserialize(TagCompound tag)
        {
            RitualObject obj = new RitualObject();

            if (tag.ContainsKey("kind")) obj.kind = tag.Get<int>("kind");
            if (tag.ContainsKey("type")) obj.type = tag.Get<int>("type");
            if (tag.ContainsKey("storedMana")) obj.storedMana = tag.Get<int>("storedMana");
            if (tag.ContainsKey("maxMana")) obj.maxMana = tag.Get<int>("maxMana");

            return obj;
        }
    }

    public class KeyPairSerializer : TagSerializer<KeyValuePair<RitualObject, int>, TagCompound>
    {
        public override TagCompound Serialize(KeyValuePair<RitualObject, int> value)
        {
            TagCompound tag = new();

            tag.Add("Key", value.Key);
            tag.Add("Value", value.Value);

            return tag;
        }

        public override KeyValuePair<RitualObject, int> Deserialize(TagCompound tag)
        {
            KeyValuePair<RitualObject, int> value = new KeyValuePair<RitualObject, int>(new RitualObject(), 0);

            if (tag.ContainsKey("Key") && tag.ContainsKey("Value"))
                value = new KeyValuePair<RitualObject, int>(tag.Get<RitualObject>("Key"), tag.Get<int>("Value"));

            return value;
        }
    }
}
