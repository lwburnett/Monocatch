using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Actors
{
    public class EasyProjectileActor : GoodProjectileActorBase
    {
        public EasyProjectileActor(Vector2 iPosition, Vector2 iVelocity) : 
            base(iPosition, iVelocity, SettingsManager.Projectiles.Easy.Radius, SettingsManager.Projectiles.Easy.FillColor)
        {
        }
    }
}