using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch_Lib.Collision
{
    public abstract class ColliderBase
    {
        public abstract void SetPosition(Vector2 iNewPosition);

        public abstract void DrawDebug(Action<Texture2D, Vector2> iDrawAction, Game iGame);
    }
}