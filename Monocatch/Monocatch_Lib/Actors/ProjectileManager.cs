using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocatch_Lib.Actors.Components;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib.Actors
{
    public class ProjectileManager
    {
        public ProjectileManager(int iStartingHeight, int iBottomBound, int iLeftBound, int iRightBound, CollisionManager iCollisionManager)
        {
            _startingHeight = iStartingHeight;
            _bottomBound = iBottomBound;
            _leftXBound = iLeftBound;
            _rightXBound = iRightBound;
            _maxSpawningXSpeed = SettingsManager.ProjectileManagerSettings.MaxXSpeedOnSpawn;
            _lastSpawn = TimeSpan.Zero;
            _spawnIntervalSeconds = SettingsManager.ProjectileManagerSettings.SpawningInterval;
            _rng = new Random();
            _activeProjectiles = new List<ProjectileActorBase>();
            _collisionManager = iCollisionManager;
        }

        public void Update(GameTime iGameTime)
        {
            var timePassedSinceLastSpawn = iGameTime.TotalGameTime - _lastSpawn;
            if (timePassedSinceLastSpawn > _spawnIntervalSeconds)
            {
                var newProjectile = GetNewProjectile();
                _activeProjectiles.Add(newProjectile);
                _lastSpawn = iGameTime.TotalGameTime;
                _collisionManager.Register(newProjectile);
            }

            var ii = 0;
            while (ii < _activeProjectiles.Count)
            {
                var thisProjectile = _activeProjectiles[ii];

                if (thisProjectile.IsCaught || thisProjectile.GetActorPosition().Y > _bottomBound)
                {
                    _activeProjectiles.RemoveAt(ii);
                    _collisionManager.Unregister(thisProjectile);
                }
                else
                {
                    thisProjectile.Update(iGameTime);
                    ii++;
                }
            }
        }

        public void Draw()
        {
            _activeProjectiles.ForEach(p => p.Draw());
        }

        private readonly int _startingHeight;
        private readonly int _bottomBound;
        private readonly int _leftXBound;
        private readonly int _rightXBound;
        private readonly float _maxSpawningXSpeed;
        private TimeSpan _lastSpawn;
        private readonly TimeSpan _spawnIntervalSeconds;
        private readonly Random _rng;

        private readonly List<ProjectileActorBase> _activeProjectiles;
        private readonly CollisionManager _collisionManager;

        private ProjectileActorBase GetNewProjectile()
        {
            const int projectileRadius = 8;
            var spawningPositionX = _rng.Next(_leftXBound + projectileRadius, _rightXBound - projectileRadius - projectileRadius - projectileRadius);

            var spawningSpeedX = (int)((_maxSpawningXSpeed) * (-1.0f + 2.0f * _rng.NextDouble()));

            ProjectileActorBase actor;
            if (_rng.Next() % 10 != 0)
                actor = new BasicProjectileActor(
                    projectileRadius,
                    Color.White,
                    new Vector2(spawningPositionX, _startingHeight),
                    new Vector2(spawningSpeedX, 0));
            else
                actor = new BadProjectileActor(
                    projectileRadius,
                    Color.OrangeRed,
                    new Vector2(spawningPositionX, _startingHeight),
                    new Vector2(spawningSpeedX, 0));

            var collider = new CircleCollider(actor.GetActorPosition() + new Vector2(projectileRadius, projectileRadius), projectileRadius);
            actor.RegisterComponent(new CollisionComponent(collider));

            return actor;
        }
    }
}