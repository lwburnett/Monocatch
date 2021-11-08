using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Actors
{
    public abstract class ProjectileActorBase : ActorBase
    {
        protected ProjectileActorBase(Vector2 iPosition, Vector2 iVelocity, float iMass, GameMaster iGame) : 
            base(iPosition, iVelocity, iMass, true, iGame)
        {
            IsCaught = false;
        }

        public bool IsCaught { get; protected set; }
        
        protected sealed override void vUpdate(GameTime gameTime)
        {
            AddForce(SettingsManager.WorldSettings.GravityForce);
        }
    }
}