using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Monocatch_Lib
{
    public abstract class ActorComponentBase
    {
        public void RegisterOwner(ActorBase iOwner)
        {
            Owner = iOwner;
        }
        protected ActorBase Owner;

        public void Update(GameTime iGameTime)
        {
            Debug.Assert(Owner != null);

            vUpdate(iGameTime);
        }
        protected abstract void vUpdate(GameTime iGameTime);
    }
}