using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Actors.Components
{
    public abstract class ActorComponentBase
    {
        protected ActorComponentBase(bool iUpdateBeforePosition)
        {
            UpdateBeforePosition = iUpdateBeforePosition;
        }

        public bool UpdateBeforePosition { get; }

        public void RegisterOwner(ActorBase iOwner)
        {
            Owner = iOwner;
            OnOwnerRegistered();
        }
        protected ActorBase Owner;

        public virtual void Update(GameTime iGameTime)
        {
            Debug.Assert(Owner != null);
        }

        protected virtual void OnOwnerRegistered()
        {
        }
    }
}