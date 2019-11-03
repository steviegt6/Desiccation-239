/*using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Desiccation.DiscordRP
{
	class Testplayer : ModPlayer
	{
		public override void OnEnterWorld(Player someone)
		{
			string wName = Main.worldName;
			bool expert = Main.expertMode;
			string wDiff = (expert) ? "Expert" : "Normal";
            RPControl.presence.details = string.Format("{0} | {1}", wDiff, wName);
            Desiccation.UpdaterLoad();
			RPUtility.dead = false;
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			RPUtility.dead = true;
			RPUtility.Update();
		}

		public override void OnRespawn(Player player)
		{
			RPUtility.dead = false;
		}
	}
}*/