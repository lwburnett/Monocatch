using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public abstract class ActorBase
    {
        public abstract void Draw(Action<Texture2D, Vector2> iDrawAction);
    }
}