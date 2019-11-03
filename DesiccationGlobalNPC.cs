using Desiccation.Items.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Desiccation.NPCs
{
    public class DesiccationGlobalNPC : GlobalNPC
    {
        public override void NPCLoot(NPC npc)
        {
            if (!npc.boss)
            {
                if (Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].ZoneBeach)
                {
                    if (Main.rand.Next(99) == 0)
                    { //1/100
                        Item.NewItem(npc.getRect(), ItemType<SeaHeart>());
                    }
                }
            }
            if (npc.type == NPCID.Zombie)
            {
                if (Main.rand.Next(9) == 0)
                { //1/10
                    Item.NewItem(npc.getRect(), ItemID.Leather);
                }
            }
        }
    }
}
