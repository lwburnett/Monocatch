﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch_Lib.Actors
{
    public class BadProjectileActor : ProjectileActorBase
    {
        public BadProjectileActor(int iRadius, Color iProjectileColor, Vector2 iPosition, Vector2 iVelocity) : 
            base(iPosition, iVelocity, SettingsManager.BadProjectileSettings.Mass)
        {
            Debug.Assert(iRadius > 0);
            Debug.Assert(iProjectileColor != Color.Transparent);

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

            _texture = GraphicsHelper.CreateTexture(colorData, diameter, diameter);
            _texture.SetData(colorData);
        }

        private readonly Texture2D _texture;

        protected sealed override Texture2D GetTexture() => _texture;



        public override void OnCollision(ActorBase iOtherActor, GameTime iGameTime)
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

            base.OnCollision(iOtherActor, iGameTime);
        }
    }
}