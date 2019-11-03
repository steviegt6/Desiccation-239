using Terraria.ModLoader;

namespace Desiccation.Projectiles.Miner
{
    public class MinerProjectile : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.type = 52;
            projectile.aiStyle = 3;
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.maxPenetrate = -1;
            projectile.melee = true;
        }
    }
}