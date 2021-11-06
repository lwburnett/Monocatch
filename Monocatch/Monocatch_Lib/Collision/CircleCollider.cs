using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch_Lib.Collision
{
    public class CircleCollider : ColliderBase
    {
        public CircleCollider(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Vector2 Center { get; private set; }
        public float Radius { get; }
        public override void SetPosition(Vector2 iNewPosition)
        {
            Center = new Vector2(iNewPosition.X + Radius, iNewPosition.Y + Radius);
        }

        public override void DrawDebug(Action<Texture2D, Vector2> iDrawAction, Game iGame)
        {
            var diameter = 2 * Radius;

            var colorData = new Color[(int)(diameter * diameter)];

            for (var xx = 0; xx < diameter; xx++)
            {
                for (var yy = 0; yy < diameter; yy++)
                {
                    var thisIndex = xx * (int)diameter + yy;
                    var distanceFromCenter = new Vector2(xx - Radius, yy - Radius);

                    const int redOutlineWidth = 2;
                    if (Math.Abs(distanceFromCenter.Length()) < Radius &&
                        Math.Abs(distanceFromCenter.Length()) > Radius - redOutlineWidth)
                        colorData[thisIndex] = Color.Red;
                    else
                        colorData[thisIndex] = Color.Transparent;
                }
            }

            var texture = new Texture2D(iGame.GraphicsDevice, (int)diameter, (int)diameter);
            texture.SetData(colorData);

            iDrawAction(texture, new Vector2(Center.X - Radius, Center.Y - Radius));
        }
    }
}