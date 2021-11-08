using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Actors.Components
{
    public abstract class ActorComponentBase
    {
        protected ActorComponentBase(GameMaster iGameMaster)
        {
            Game = iGameMaster;
        }

        public void RegisterOwner(ActorBase iOwner)
        {
            Owner = iOwner;
            OnOwnerRegistered();
        }
        protected ActorBase Owner;

        public void Update(GameTime iGameTime)
        {
            Debug.Assert(Owner != null);

            vUpdate(iGameTime);
        }
        protected abstract void vUpdate(GameTime iGameTime);

        protected virtual void OnOwnerRegistered() {}

        protected GameMaster Game { get; }
    }
}