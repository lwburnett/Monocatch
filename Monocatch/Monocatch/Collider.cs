using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public abstract class ColliderBase
    {
        public abstract void SetPosition(Vector2 iNewPosition);

        public abstract void DrawDebug(Action<Texture2D, Vector2> iDrawAction, Game iGame);
    }

    public class BoxCollider : ColliderBase
    {
        public BoxCollider(Vector2 iTopLeft, Vector2 iBottomRight)
        {
            Left = iTopLeft.X;
            Top = iTopLeft.Y;
            Right = iBottomRight.X;
            Bottom = iBottomRight.Y;
            Width = Right - Left;
            Height = Bottom - Top;
            
            Debug.Assert(Left < Right);
            Debug.Assert(Top < Bottom);
            Debug.Assert(Width > 0);
            Debug.Assert(Height > 0);
        }

        public BoxCollider(int left, int top, int right, int bottom, int width, int height)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            Width = width;
            Height = height;

            Debug.Assert(Left < Right);
            Debug.Assert(Top < Bottom);
            Debug.Assert(Width > 0);
            Debug.Assert(Height > 0);
        }

        public float Left { get; private set; }
        public float Top { get; private set; }
        public float Right { get; private set; }
        public float Bottom { get; private set; }
        public float Width { get; }
        public float Height { get; }

        public override void SetPosition(Vector2 iNewPosition)
        {
            Left = iNewPosition.X;
            Top = iNewPosition.Y;
            Right = iNewPosition.X + Width;
            Bottom = iNewPosition.Y + Height;
        }

        public override void DrawDebug(Action<Texture2D, Vector2> iDrawAction, Game iGame)
        {
            var colorData = new Color[(int)Width * (int)Height];
            for (var xx = 0; xx < (int)Height; xx++)
            {
                for (var yy = 0; yy < (int)Width; yy++)
                {
                    const int redOutlineWidth = 2;

                    if (xx < redOutlineWidth|| yy < redOutlineWidth || yy > (int)Width - redOutlineWidth - 1 || xx > (int)Height - redOutlineWidth - 1)
                        colorData[(xx * ((int)Width)) + yy] = Color.Red;
                }
            }

            var texture = new Texture2D(iGame.GraphicsDevice, (int)Width, (int)Height);
            texture.SetData(colorData);

            iDrawAction(texture, new Vector2(Left, Top));
        }
    }

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