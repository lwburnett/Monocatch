using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public class PlayerActor : ActorBase
    {
        public PlayerActor(Point iPosition, int iPlayerWidth, int iPlayerHeight, Color iFillColor, GameMaster iGame) : base(iPosition.ToVector2(), Vector2.Zero, 1.0f, iGame)
        {
            Debug.Assert(iPlayerWidth > 0);
            Debug.Assert(iPlayerHeight > 0);
            Debug.Assert(iFillColor != Color.Transparent);
            Debug.Assert(iGame != null);

            var colorData = new Color[iPlayerWidth * iPlayerHeight];
            
            for (var ii = 0; ii < colorData.Length; ii++)
            {
                colorData[ii] = iFillColor;
            }

            _texture = new Texture2D(iGame.GraphicsDevice, iPlayerWidth, iPlayerHeight);
            _texture.SetData(colorData);
        }

        private readonly Texture2D _texture;

        protected override Texture2D GetTexture() => _texture;
    }
}