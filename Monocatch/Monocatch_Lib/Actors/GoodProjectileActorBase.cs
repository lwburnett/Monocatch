using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch_Lib.Actors
{
    public class GoodProjectileActorBase : ProjectileActorBase
    {
        public GoodProjectileActorBase(Vector2 iPosition, Vector2 iVelocity, int iRadius, Color iColor) : base(iPosition, iVelocity, 1.0f)
        {
            var diameter = 2 * iRadius;

            var colorData = new Color[diameter * diameter];

            for (var xx = 0; xx < diameter; xx++)
            {
                for (var yy = 0; yy < diameter; yy++)
                {
                    var thisIndex = xx * diameter + yy;
                    var distanceFromCenter = new Vector2(xx - iRadius, yy - iRadius);

                    colorData[thisIndex] = Math.Abs(distanceFromCenter.Length()) < iRadius ?
                        iColor : Color.Transparent;
                }
            }

            _texture = GraphicsHelper.CreateTexture(colorData, diameter, diameter);
            _texture.SetData(colorData);
        }

        private readonly Texture2D _texture;

        protected sealed override Texture2D GetTexture() => _texture;
    }
}