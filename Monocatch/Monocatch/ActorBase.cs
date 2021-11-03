using System;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Monocatch
{
    public abstract class ActorBase
    {
        protected ActorBase(Vector2 iPosition, Vector2 iVelocity, float iMass, GameMaster iGame)
        {
            _position = iPosition;
            _velocity = iVelocity;
            _mass = iMass;
            _thisFrameForce = new FrameForce();
        }

        public virtual void Update(GameTime iGameTime)
        {
            vUpdate(iGameTime);

            var elapsedTimeSeconds = (float)iGameTime.ElapsedGameTime.TotalSeconds;

            var positionFinalX = (_thisFrameForce.SummedForce.X * elapsedTimeSeconds * elapsedTimeSeconds / _mass) + (_velocity.X * elapsedTimeSeconds) + _position.X;
            var positionFinalY = (_thisFrameForce.SummedForce.Y * elapsedTimeSeconds * elapsedTimeSeconds / _mass) + (_velocity.Y * elapsedTimeSeconds) + _position.Y;

            var velocityFinalX = (_thisFrameForce.SummedForce.X * elapsedTimeSeconds / _mass) + _velocity.X;
            var velocityFinalY = (_thisFrameForce.SummedForce.Y * elapsedTimeSeconds / _mass) + _velocity.Y;

            _position = new Vector2(positionFinalX, positionFinalY);
            _velocity = new Vector2(velocityFinalX, velocityFinalY);

            _thisFrameForce.Reset();
        }

        public void Draw(Action<Texture2D, Vector2> iDrawAction)
        {
            iDrawAction(GetTexture(), _position);
        }

        private Vector2 _position;
        private Vector2 _velocity;
        private float _mass;
        private FrameForce _thisFrameForce;

        protected abstract Texture2D GetTexture();
        protected virtual void vUpdate(GameTime iGameTime) {}

        protected void AddForce(Vector2 iForce)
        {
            _thisFrameForce.Add(iForce);
        }

        private class FrameForce
        {
            public FrameForce()
            {
                SummedForce = Vector2.Zero;
            }

            public void Add(Vector2 iForce)
            {
                SummedForce += iForce;
            }

            public void Reset()
            {
                SummedForce = Vector2.Zero;
            }

            public Vector2 SummedForce { get; private set; }
        }
    }
}