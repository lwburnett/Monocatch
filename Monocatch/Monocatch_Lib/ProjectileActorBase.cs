using Microsoft.Xna.Framework;

namespace Monocatch_Lib
{
    public abstract class ProjectileActorBase : ActorBase
    {
        protected ProjectileActorBase(Vector2 iPosition, Vector2 iVelocity, float iMass, GameMaster iGame) : 
            base(iPosition, iVelocity, iMass, true, iGame)
        {
            IsCaught = false;
        }

        public bool IsCaught { get; protected set; }
        
        private const float _cGravityMag = 100f;
        
        protected sealed override void vUpdate(GameTime gameTime)
        {
            AddForce(new Vector2(0.0f, _cGravityMag));
        }
    }
}