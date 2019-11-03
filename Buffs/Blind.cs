using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Desiccation.Buffs
{
    public class Blind : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Blind Debuff");
            Description.SetDefault("This Debuff will slow down Enemys.");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed -= 2;
        }

    }
}