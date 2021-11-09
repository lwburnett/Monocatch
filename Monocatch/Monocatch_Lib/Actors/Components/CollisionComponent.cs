using Microsoft.Xna.Framework;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib.Actors.Components
{
    public class CollisionComponent : ActorComponentBase
    {
        public CollisionComponent(ColliderBase iCollider) : base(false)
        {
            _collider = iCollider;
            _transformFromOwner = Vector2.Zero;
        }

        public ColliderBase GetCollider() => _collider;

        public override void Update(GameTime iGameTime)
        {
            _collider.SetPosition(Owner.GetActorPosition() + _transformFromOwner);

            base.Update(iGameTime);
        }

        protected override void OnOwnerRegistered()
        {
            _transformFromOwner = _collider.GetPosition() - Owner.GetActorPosition();

            base.OnOwnerRegistered();
        }

        private readonly ColliderBase _collider;
        private Vector2 _transformFromOwner;
    }
}