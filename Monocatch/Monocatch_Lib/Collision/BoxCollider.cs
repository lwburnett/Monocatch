using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch_Lib.Collision
{
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
}