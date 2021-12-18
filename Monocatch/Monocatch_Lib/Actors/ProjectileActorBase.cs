using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Actors
{
    public abstract class ProjectileActorBase : ActorBase
    {
        protected ProjectileActorBase(Vector2 iPosition, Vector2 iVelocity, float iMass) : 
            base(iPosition, iVelocity, iMass)
        {
            IsCaught = false;
        }

        public bool IsCaught { get; protected set; }
        
        protected sealed override void vUpdate(GameTime gameTime)
        {
            AddForce(SettingsManager.WorldSettings.GravityForce);
        }

        public override void OnCollision(ActorBase iOtherActor, GameTime iGameTime)
        {
            if (iOtherActor is PlayerActor)
            {
                IsCaught = true;
            }
            else if (iOtherActor is WallActor)
            {
                var currentVelocity = GetActorVelocity();
                SetActorVelocity(new Vector2(-1.0f * currentVelocity.X, currentVelocity.Y));
            }

            base.OnCollision(iOtherActor, iGameTime);
        }
    }
}