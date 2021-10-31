using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public class WallActor : ActorBase
    {
        public WallActor(Point iTopLeft, Point iBottomRight, Color iFillColor, GameMaster iGame)
        {
            Debug.Assert(iTopLeft.X < iBottomRight.X);
            Debug.Assert(iTopLeft.Y < iBottomRight.Y);
            Debug.Assert(iFillColor != Color.Transparent);
            Debug.Assert(iGame != null);

            _topLeft = iTopLeft;
            _bottomRight = iBottomRight;
            _game = iGame;

            var width = _bottomRight.X - _topLeft.X;
            var height = _bottomRight.Y - _topLeft.Y;

            var colorData = new Color[width * height];
            for (var ii = 0; ii < colorData.Length; ii++)
            {
                colorData[ii] = iFillColor;
            }

            _texture = new Texture2D(_game.GraphicsDevice, width, height);
            _texture.SetData(colorData);
        }

        private readonly Texture2D _texture;
        private readonly Point _topLeft;
        private readonly Point _bottomRight;
        private readonly GameMaster _game;

        public override void Draw(Action<Texture2D, Vector2> iDrawAction)
        {
            iDrawAction(_texture, new Vector2(_topLeft.X, _topLeft.Y));
        }
    }
}