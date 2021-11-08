using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib.Actors
{
    public class WallActor : ActorBase
    {
        public WallActor(Point iTopLeft, Point iBottomRight, Color iFillColor, GameMaster iGame) : 
            base(iTopLeft.ToVector2(), Vector2.Zero, SettingsManager.WallSettings.Mass, true, iGame)
        {
            Debug.Assert(iTopLeft.X < iBottomRight.X);
            Debug.Assert(iTopLeft.Y < iBottomRight.Y);
            Debug.Assert(iFillColor != Color.Transparent);
            Debug.Assert(iGame != null);

            var width = iBottomRight.X - iTopLeft.X;
            var height = iBottomRight.Y - iTopLeft.Y;

            var colorData = new Color[width * height];
            for (var ii = 0; ii < colorData.Length; ii++)
            {
                colorData[ii] = iFillColor;
            }

            _texture = new Texture2D(iGame.GraphicsDevice, width, height);
            _texture.SetData(colorData);
            SetCollider(new BoxCollider(iTopLeft.ToVector2(), iBottomRight.ToVector2()));
        }

        private readonly Texture2D _texture;
        protected sealed override Texture2D GetTexture() => _texture;
    }
}