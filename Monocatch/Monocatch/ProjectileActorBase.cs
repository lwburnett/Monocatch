using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public abstract class ProjectileActorBase
    {
        protected ProjectileActorBase(GameMaster iGame)
        {
            _game = iGame;
            _position = Vector2.Zero;
            _velocity = Vector2.Zero;
        }


        private readonly GameMaster _game;
        private const float _cGravityMag = 100f;

        private const float _cMass = 1.0f;
        protected Vector2 _position;
        protected Vector2 _velocity;

        public abstract void Draw(Action<Texture2D, Vector2> iDrawAction);

        public void Update(GameTime gameTime)
        {
            var elapsedTimeSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var positionFinalX = _velocity.X * elapsedTimeSeconds + _position.X;
            var positionFinalY = (_cGravityMag * elapsedTimeSeconds * elapsedTimeSeconds / _cMass) + (_velocity.Y * elapsedTimeSeconds) + _position.Y;

            var velocityFinalY = (_cGravityMag * elapsedTimeSeconds / _cMass) + _velocity.Y;

            _position = new Vector2(positionFinalX, positionFinalY);
            _velocity = new Vector2(_velocity.X, velocityFinalY);
        }
    }
}