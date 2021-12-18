using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Actors
{
    public class HardProjectileActor : GoodProjectileActorBase
    {
        public HardProjectileActor(Vector2 iPosition, Vector2 iVelocity) : 
            base(iPosition, iVelocity, SettingsManager.Projectiles.Hard.Radius, SettingsManager.Projectiles.Hard.FillColor)
        {
        }
    }
}