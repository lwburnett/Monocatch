using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Collision
{
    public abstract class ColliderBase
    {
        public abstract void SetPosition(Vector2 iNewPosition);

        public abstract Vector2 GetPosition();

        public abstract void DrawDebug();
    }
}