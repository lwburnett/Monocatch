using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public class BasicProjectile
    {
        public BasicProjectile(int iRadius, Color iProjectileColor)
        {
            Debug.Assert(iRadius > 0);
            Debug.Assert(iProjectileColor != Color.Transparent);

            _radius = iRadius;
            _Color = iProjectileColor;

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

            Texture = new Texture2D(GameMaster.GetOrCreateInstance().GraphicsDevice, diameter, diameter);
            Texture.SetData(colorData);
        }
        
        public Texture2D Texture { get; }

        private readonly int _radius;
        private readonly Color _Color;
    }
}