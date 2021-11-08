using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib.Actors
{
    public class BasicProjectileActor : ProjectileActorBase
    {
        public BasicProjectileActor(int iRadius, Color iProjectileColor, Vector2 iPosition, Vector2 iVelocity, GameMaster iGame) : 
            base(iPosition, iVelocity, SettingsManager.BasicProjectileSettings.Mass, iGame)
        {
            Debug.Assert(iRadius > 0);
            Debug.Assert(iProjectileColor != Color.Transparent);
            Debug.Assert(iGame != null);

            var diameter = 2 * iRadius;

            var colorData = new Color[diameter * diameter];
            
            for (var xx = 0; xx < diameter; xx++)
            {
                for (var yy = 0; yy < diameter; yy++)
                {
                    var thisIndex = xx * diameter + yy;
                    var distanceFromCenter = new Vector2(xx - iRadius, yy - iRadius);

                    colorData[thisIndex] = Math.Abs(distanceFromCenter.Length()) < iRadius ?
                        iProjectileColor : Color.Transparent;
                }
            }

            _texture = new Texture2D(iGame.GraphicsDevice, diameter, diameter);
            _texture.SetData(colorData);
            SetCollider(new CircleCollider(new Vector2(iPosition.X + iRadius, iPosition.Y + iRadius), iRadius));
        }

        private readonly Texture2D _texture;

        protected sealed override Texture2D GetTexture() => _texture;

        public override void OnCollision(ActorBase iOtherActor)
        {
            if (iOtherActor is PlayerActor)
            {
                IsCaught = true;
            }
            else if (iOtherActor is WallActor)
            {
                var currentVelocity = GetActorVelocity();
                SetActorVelocity(new Vector2(-1.0f * currentVelocity.X, currentVelocity.Y));
            }

            base.OnCollision(iOtherActor);
        }
    }
}