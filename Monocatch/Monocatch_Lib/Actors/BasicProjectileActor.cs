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
            base(iPosition, iVelocity, 1.0f, iGame)
        {
            Debug.Assert(iRadius > 0);
            Debug.Assert(iProjectileColor != Color.Transparent);
            Debug.Assert(iGame != null);

            _radius = iRadius;
            _color = iProjectileColor;
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
                        _color : Color.Transparent;
                }
            }

            _texture = new Texture2D(_game.GraphicsDevice, diameter, diameter);
            _texture.SetData(colorData);
            SetCollider(new CircleCollider(new Vector2(iPosition.X + _radius,iPosition.Y + _radius), _radius));
        }

        private readonly Texture2D _texture;
        private readonly int _radius;
        private readonly Color _color;
        private readonly GameMaster _game;

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