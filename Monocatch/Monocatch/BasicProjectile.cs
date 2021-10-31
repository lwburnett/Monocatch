using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public class BasicProjectile : ProjectileActorBase
    {
        public BasicProjectile(int iRadius, Color iProjectileColor, Vector2 iPosition, Vector2 iVelocity, GameMaster iGame)
        {
            Debug.Assert(iRadius > 0);
            Debug.Assert(iProjectileColor != Color.Transparent);
            Debug.Assert(iGame != null);

            _radius = iRadius;
            _Color = iProjectileColor;
            _position = iPosition;
            _velocity = iVelocity;
            _game = iGame;

            var diameter = 2 * iRadius;

            var colorData = new Color[diameter * diameter];
            
            for (var xx = 0; xx < diameter; xx++)
            {
                for (var yy = 0; yy < diameter; yy++)
                {
                    var thisIndex = xx * diameter + yy;
                    var distanceFromCenter = new Vector2(xx - _radius, yy - _radius);

                    colorData[thisIndex] = Math.Abs(distanceFromCenter.Length()) < _radius ?
                        _Color : Color.Transparent;
                }
            }

            _texture = new Texture2D(_game.GraphicsDevice, diameter, diameter);
            _texture.SetData(colorData);
        }

        private readonly Texture2D _texture;
        private readonly int _radius;
        private readonly Color _Color;
        private readonly GameMaster _game;

        private Vector2 _position;
        private Vector2 _velocity;

        public override void Draw(Action<Texture2D, Vector2> iDrawAction)
        {
            iDrawAction(_texture, _position);
        }
    }
}