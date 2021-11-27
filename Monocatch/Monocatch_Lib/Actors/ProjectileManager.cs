using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
            _activePatterns = new List<ProjectilePatternBase>();
            _collisionManager = iCollisionManager;
            _patternExpirationTime = null;
        }

        public void Update(GameTime iGameTime)
        {
            var isAtLeastOneSpawningPattern = false;
            var ii = 0;
            while (ii < _activePatterns.Count)
            {
                var thisPattern = _activePatterns[ii];

                if (thisPattern.IsCompletelyFinished())
                {
                    _activePatterns.RemoveAt(ii);
                    continue;
                }

                if (thisPattern.IsSpawning())
                    isAtLeastOneSpawningPattern = true;

                ii++;
            }

            if (!isAtLeastOneSpawningPattern)
            {
                if (!_patternExpirationTime.HasValue)
                    _activePatterns.Add(new DefaultProjectilePattern(_startingHeight, _bottomBound, _leftXBound, _rightXBound, 10, _collisionManager));
                else if (iGameTime.TotalGameTime - _patternExpirationTime.Value > SettingsManager.ProjectileManagerSettings.SpawningInterval)
                {
                    _activePatterns.Add(new DefaultProjectilePattern(_startingHeight, _bottomBound, _leftXBound, _rightXBound, 10, _collisionManager));
                    _patternExpirationTime = null;
                }

            }

            _activePatterns.ForEach(p => p.Update(iGameTime));
        }

        public void Draw()
        {
            _activePatterns.ForEach(p => p.Draw());
        }

        private readonly int _startingHeight;
        private readonly int _bottomBound;
        private readonly int _leftXBound;
        private readonly int _rightXBound;
        
        private readonly CollisionManager _collisionManager;
        private readonly List<ProjectilePatternBase> _activePatterns;
        private TimeSpan? _patternExpirationTime;
    }
}