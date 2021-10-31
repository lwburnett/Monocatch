using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public class BasicProjectile
    {
        public BasicProjectile()
        {
            Texture = GameMaster.GetOrCreateInstance().Content.Load<Texture2D>("BasicProjectile");
        }
        
        public Texture2D Texture { get; }
    }
}