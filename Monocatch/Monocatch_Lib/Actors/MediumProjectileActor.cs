using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Actors
{
    public class MediumProjectileActor : GoodProjectileActorBase
    {
        public MediumProjectileActor(Vector2 iPosition, Vector2 iVelocity) : 
            base(iPosition, iVelocity, SettingsManager.Projectiles.Medium.Radius, SettingsManager.Projectiles.Medium.FillColor)
        {
        }
    }
}