using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public class BasicProjectile : ActorBase
    {
        public BasicProjectile(int iRadius, Color iProjectileColor, GameMaster iGame)
        {
            Debug.Assert(iRadius > 0);
            Debug.Assert(iProjectileColor != Color.Transparent);
            Debug.Assert(iGame != null);

            _radius = iRadius;
            _Color = iProjectileColor;
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

            Texture = new Texture2D(_game.GraphicsDevice, diameter, diameter);
            Texture.SetData(colorData);
        }
        
        public Texture2D Texture { get; }

        private readonly int _radius;
        private readonly Color _Color;
        private readonly GameMaster _game;

        public override void Draw(Action<Texture2D, Vector2> iDrawAction)
        {
            iDrawAction(Texture, Vector2.Zero);
        }
    }
}