using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch
{
    public class ProjectileManager
    {
        public ProjectileManager(int iStartingHeight, int iBottomBound, int iLeftBound, int iRightBound, GameMaster iGame)
        {
            _startingHeight = iStartingHeight;
            _bottomBound = iBottomBound;
            _leftXBound = iLeftBound;
            _rightXBound = iRightBound;
            _maxSpawningXSpeed = 10.0f;
            _game = iGame;
            _lastSpawn = TimeSpan.Zero;
            _spawnIntervalSeconds = TimeSpan.FromSeconds(2);
            _rng = new Random();
            _activeProjectiles = new List<ProjectileActorBase>();
        }

        public void Update(GameTime iGameTime)
        {
            var timePassedSinceLastSpawn = iGameTime.TotalGameTime - _lastSpawn;
            if (timePassedSinceLastSpawn > _spawnIntervalSeconds)
            {
                var newProjectile = GetNewProjectile();
                _activeProjectiles.Add(newProjectile);
                _lastSpawn = iGameTime.TotalGameTime;
            }

            var ii = 0;
            while (ii < _activeProjectiles.Count)
            {
                var thisProjectile = _activeProjectiles[ii];

                if (thisProjectile.GetActorPosition().Y > _bottomBound)
                    _activeProjectiles.RemoveAt(ii);
                else
                {
                    thisProjectile.Update(iGameTime);
                    ii++;
                }
            }
        }

        public void Draw(Action<Texture2D, Vector2> iDrawAction)
        {
            _activeProjectiles.ForEach(p => p.Draw(iDrawAction));
        }

        private readonly int _startingHeight;
        private readonly int _bottomBound;
        private readonly int _leftXBound;
        private readonly int _rightXBound;
        private readonly float _maxSpawningXSpeed;
        private readonly GameMaster _game;
        private TimeSpan _lastSpawn;
        private readonly TimeSpan _spawnIntervalSeconds;
        private readonly Random _rng;

        private readonly List<ProjectileActorBase> _activeProjectiles;

        private ProjectileActorBase GetNewProjectile()
        {
            const int projectileRadius = 8;
            var spawningPositionX = _rng.Next(_leftXBound + projectileRadius, _rightXBound - projectileRadius);

            var spawningSpeedX = (int)((_maxSpawningXSpeed) * (-1.0f + 2.0f * _rng.NextDouble()));

            return new BasicProjectileActor(
                projectileRadius,
                Color.White,
                new Vector2(spawningPositionX, _startingHeight),
                new Vector2(spawningSpeedX, 0),
                _game);
        }
    }
}