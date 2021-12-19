using System;
using Microsoft.Xna.Framework;
using Monocatch_Lib.Actors.Components;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib.Actors
{
    internal class DefaultBadProjectilePattern : ProjectilePatternBase
    {
        private readonly int _startingHeight;
        private readonly int _leftXBound;
        private readonly int _rightXBound;
        private readonly float _maxSpawningXSpeed;
        private TimeSpan _lastSpawn;
        private readonly TimeSpan _spawnIntervalSeconds;
        private readonly Random _rng;
        private readonly float _spawningYSpeed;

        public DefaultBadProjectilePattern(int iStartingHeight, int iBottomBound, int iLeftXBound, int iRightXBound, CollisionManager iCollisionManager) : 
            base(iBottomBound, SettingsManager.Patterns.Bad.TotalNumBadProjectiles, iCollisionManager)
        {
            _startingHeight = iStartingHeight;
            _leftXBound = iLeftXBound;
            _rightXBound = iRightXBound;
            _maxSpawningXSpeed = SettingsManager.Patterns.Bad.MaxXSpeedOnSpawn;
            _spawningYSpeed = SettingsManager.Patterns.Bad.SpawningYSpeed;
            _lastSpawn = TimeSpan.Zero;
            _spawnIntervalSeconds = SettingsManager.Patterns.Bad.SpawningInterval;
            _rng = new Random();
        }

        protected override void SpawnNewProjectilesIfNeeded(GameTime iGameTime)
        {
            var timePassedSinceLastSpawn = iGameTime.TotalGameTime - _lastSpawn;
            if (timePassedSinceLastSpawn > _spawnIntervalSeconds)
            {
                var newProjectile = GetNewProjectile();
                SpawnProjectile(newProjectile);
                _lastSpawn = iGameTime.TotalGameTime;
            }
        }

        private ProjectileActorBase GetNewProjectile()
        {
            const int projectileRadius = 8;
            var spawningPositionX = _rng.Next(_leftXBound + projectileRadius, _rightXBound - projectileRadius - projectileRadius - projectileRadius);

            var spawningSpeedX = (int)((_maxSpawningXSpeed) * (-1.0f + 2.0f * _rng.NextDouble()));

            var position = new Vector2(spawningPositionX, _startingHeight);
            var velocity = new Vector2(spawningSpeedX, _spawningYSpeed);

            var actor = new BadProjectileActor(position, velocity);

            var collider = new CircleCollider(actor.GetActorPosition() + new Vector2(projectileRadius, projectileRadius), projectileRadius);
            actor.RegisterComponent(new CollisionComponent(collider));

            return actor;
        }
    }
}