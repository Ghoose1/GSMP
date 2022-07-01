using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace GSMP.Content.Buffs
{
    public class ManaRegenNerf : ModBuff
    {
        public override string Texture => $"Terraria/Images/Buff_{BuffID.ManaSickness}";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drained");
            Description.SetDefault("Your Mana is being stolen from you");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //player.manaRegenDelay = 60;
            //if (player.velocity == Vector2.Zero)
            // Vanilla mana regen when moving
            //player.manaRegen = (int)(((player.statManaMax / 7) + 1f) * ((player.statMana / player.statManaMax) * 0.8f + 0.2f) * 1.15f);
            //player.UpdateManaRegen();
            //Main.NewText(player.manaRegen);
        }
    }
}
