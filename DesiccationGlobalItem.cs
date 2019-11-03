using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Desiccation.Items
{
    public class DesiccationGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.sentry)
            {
                TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.mod == "Terraria");
                if (tt != null)
                {
                    string[] splitText = tt.text.Split(' ');
                    string damageValue = splitText.First();
                    string damageWord = splitText.Last();
                    tt.text = damageValue + " sentry " + damageWord;
                }
            }
        }
    }
}