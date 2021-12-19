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

            LoadPatterns();
        }

        public void Update(GameTime iGameTime)
        {
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

        private void LoadPatterns()
        {
            var defaultPattern = new DefaultProjectilePattern(_startingHeight, _bottomBound, _leftXBound, _rightXBound, _collisionManager);
            _activePatterns.Add(defaultPattern);

            var badPattern = new DefaultBadProjectilePattern(_startingHeight, _bottomBound, _leftXBound, _rightXBound, _collisionManager);
            _activePatterns.Add(badPattern);
        }
    }
}