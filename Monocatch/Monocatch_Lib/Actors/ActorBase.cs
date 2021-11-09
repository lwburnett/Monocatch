using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocatch_Lib.Actors.Components;
using Monocatch_Lib.Collision;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Monocatch_Lib.Actors
{
    public abstract class ActorBase
    {
        protected ActorBase(Vector2 iPosition, Vector2 iVelocity, float iMass)
        {
            _position = iPosition;
            _velocity = iVelocity;
            _mass = iMass;
            _thisFrameForce = new FrameForce();
            _components = new List<ActorComponentBase>();
        }

        public virtual void Update(GameTime iGameTime)
        {
            vUpdate(iGameTime);
            foreach (var component in _components.Where(c => c.UpdateBeforePosition))
                component.Update(iGameTime);

            var elapsedTimeSeconds = (float)iGameTime.ElapsedGameTime.TotalSeconds;

            var positionFinalX = (_thisFrameForce.SummedForce.X * elapsedTimeSeconds * elapsedTimeSeconds / _mass) + (_velocity.X * elapsedTimeSeconds) + _position.X;
            var positionFinalY = (_thisFrameForce.SummedForce.Y * elapsedTimeSeconds * elapsedTimeSeconds / _mass) + (_velocity.Y * elapsedTimeSeconds) + _position.Y;

            var velocityFinalX = (_thisFrameForce.SummedForce.X * elapsedTimeSeconds / _mass) + _velocity.X;
            var velocityFinalY = (_thisFrameForce.SummedForce.Y * elapsedTimeSeconds / _mass) + _velocity.Y;

            _position = new Vector2(positionFinalX, positionFinalY);
            _velocity = new Vector2(velocityFinalX, velocityFinalY);

            foreach (var component in _components.Where(c => !c.UpdateBeforePosition))
                component.Update(iGameTime);

            _thisFrameForce.Reset();
        }

        public void Draw()
        {
            GraphicsHelper.DrawTexture(GetTexture(), _position);

            // Draw debug collider textures
            var collisionComponent = GetComponentByType<CollisionComponent>();
            if (collisionComponent != null)
                collisionComponent.GetCollider().DrawDebug();
        }

        public void RegisterComponent(ActorComponentBase ioComponent)
        {
            ioComponent.RegisterOwner(this);
            _components.Add(ioComponent);
        }

        public T GetComponentByType<T>() where T : ActorComponentBase
        {
            return _components.FirstOrDefault(c => c is T) as T;
        }

        public void AddForce(Vector2 iForce)
        {
            _thisFrameForce.Add(iForce);
        }
        
        public virtual void OnCollision(ActorBase iOtherActor, GameTime iGameTime) { }

        public Vector2 GetActorPosition() => _position;

        public void SetActorPosition(Vector2 iNewPosition)
        {
            _position = iNewPosition;
        }

        public Vector2 GetActorVelocity() => _velocity;

        public void SetActorVelocity(Vector2 iNewVelocity)
        {
            _velocity = iNewVelocity;
        }

        public float GetActorMass() => _mass;

        private Vector2 _position;
        private Vector2 _velocity;
        private readonly float _mass;
        private readonly FrameForce _thisFrameForce;
        private readonly List<ActorComponentBase> _components;

        protected abstract Texture2D GetTexture();

        // ReSharper disable once InconsistentNaming
        protected virtual void vUpdate(GameTime iGameTime) {}

        #region FrameForce Class

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

        #endregion
    }
}