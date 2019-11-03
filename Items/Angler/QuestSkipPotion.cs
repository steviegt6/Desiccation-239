using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Desiccation.Items.Angler
{
    public class QuestSkipPotion : ModItem
    {

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Very useful for those grinding times");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 30;
            item.maxStack = 30;
            item.rare = 2;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.value = 200;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
            Main.NewText("<Angler> Wait! Come back! I have more things for you to do.", 0, 128, 255);
            Main.AnglerQuestSwap();
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(7);
            }
            return true;
        }

        public override void AddRecipes()
        {
            if (NPC.downedBoss3)
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddIngredient(ItemID.CratePotion, 3);
                recipe.AddIngredient(ItemID.SonarPotion, 2);
                recipe.AddIngredient(ItemID.FishingPotion, 5);
                recipe.AddTile(TileID.Bottles);
                recipe.SetResult(this);
                recipe.AddRecipe();
            }
        }
    }
}