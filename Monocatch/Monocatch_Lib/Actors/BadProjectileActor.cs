﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch_Lib.Actors
{
    public class BadProjectileActor : ProjectileActorBase
    {
        public BadProjectileActor(Vector2 iPosition, Vector2 iVelocity) : 
            base(iPosition, iVelocity, SettingsManager.BadProjectileSettings.Mass)
        {
            var radius = SettingsManager.Projectiles.Bad.Radius;
            var color = SettingsManager.Projectiles.Bad.FillColor;

            var diameter = 2 * radius;

            var colorData = new Color[diameter * diameter];

            for (var xx = 0; xx < diameter; xx++)
            {
                for (var yy = 0; yy < diameter; yy++)
                {
                    var thisIndex = xx * diameter + yy;
                    var distanceFromCenter = new Vector2(xx - radius, yy - radius);

                    colorData[thisIndex] = Math.Abs(distanceFromCenter.Length()) < radius ?
                        color : Color.Transparent;
                }
            }

            _texture = GraphicsHelper.CreateTexture(colorData, diameter, diameter);
            _texture.SetData(colorData);
        }

        private readonly Texture2D _texture;

        protected sealed override Texture2D GetTexture() => _texture;
    }
}