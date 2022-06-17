using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using GSMP.Content.Items;
using GSMP.DataStructures;

namespace GSMP.Content.Tiles
{
    public class SpellTile : ModTile
    {
        public override string Texture => "GSMP/Assets/ExampleBlock";
        internal Spell spell;

        public override void SetStaticDefaults()
		{
			Main.tileBlockLight[Type] = true;
			ItemDrop = ModContent.ItemType<SpellItem>();
			AddMapEntry(new Color(200, 200, 200));
		}

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (item.ModItem is SpellItem spellItem)
                spell = spellItem.spell;
        }

        public override bool Drop(int i, int j)
        {
            SpellItemSource source = new SpellItemSource(i * 16, j * 16, spell);
            int item = Item.NewItem(source, new Rectangle(i * 16, j * 16, 0, 0), ModContent.ItemType<SpellItem>());
            if (Main.item[item].ModItem is SpellItem spellItem)
                spellItem.spell = spell;
            return false;
        }
    }

    public class SpellItemSource : EntitySource_TileBreak
    {
        public Spell spell;
        public SpellItemSource(int X, int Y, Spell spell_, string context = null) : base(X, Y)
        {
            spell = spell_;
        }
    }
}
