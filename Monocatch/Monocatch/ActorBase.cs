using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Monocatch
{
    public abstract class ActorBase
    {
        protected ActorBase(Vector2 iPosition, Vector2 iVelocity, float iMass, bool iIsCollidable, GameMaster iGame)
        {
            _position = iPosition;
            _velocity = iVelocity;
            _mass = iMass;
            _thisFrameForce = new FrameForce();
            _components = new List<ActorComponentBase>();
            _collider = null;
            _isCollidable = iIsCollidable;
            _game = iGame;

            if (_isCollidable)
                iGame.RegisterCollidableActor(this);
        }

        public virtual void Update(GameTime iGameTime)
        {
            vUpdate(iGameTime);
            foreach (var component in _components) 
                component.Update(iGameTime);

            var elapsedTimeSeconds = (float)iGameTime.ElapsedGameTime.TotalSeconds;

            var positionFinalX = (_thisFrameForce.SummedForce.X * elapsedTimeSeconds * elapsedTimeSeconds / _mass) + (_velocity.X * elapsedTimeSeconds) + _position.X;
            var positionFinalY = (_thisFrameForce.SummedForce.Y * elapsedTimeSeconds * elapsedTimeSeconds / _mass) + (_velocity.Y * elapsedTimeSeconds) + _position.Y;

            var velocityFinalX = (_thisFrameForce.SummedForce.X * elapsedTimeSeconds / _mass) + _velocity.X;
            var velocityFinalY = (_thisFrameForce.SummedForce.Y * elapsedTimeSeconds / _mass) + _velocity.Y;

            _position = new Vector2(positionFinalX, positionFinalY);
            _velocity = new Vector2(velocityFinalX, velocityFinalY);

            if (_isCollidable)
                _collider.SetPosition(_position);

            _thisFrameForce.Reset();
        }

        public void Draw(Action<Texture2D, Vector2> iDrawAction)
        {
            iDrawAction(GetTexture(), _position);

            //_collider.DrawDebug(iDrawAction, _game);
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
        
        public virtual void OnCollision(ActorBase actor2) { }

        public Vector2 GetActorPosition() => _position;

        public Vector2 GetActorVelocity() => _velocity;

        protected void SetActorVelocity(Vector2 iNewVelocity)
        {
            _velocity = iNewVelocity;
        }

        public float GetActorMass() => _mass;

        public ColliderBase GetCollider() => _collider;

        protected void SetCollider(ColliderBase iCollider)
        {
            _collider = iCollider;
        }

        private Vector2 _position;
        private Vector2 _velocity;
        private readonly float _mass;
        private readonly FrameForce _thisFrameForce;
        private readonly List<ActorComponentBase> _components;
        private ColliderBase _collider;
        private readonly bool _isCollidable;
        private readonly GameMaster _game;

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