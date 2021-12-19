using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib.Actors
{
    public abstract class ProjectilePatternBase
    {
        protected ProjectilePatternBase(int iBottomBound, int iNumberOfProjectilesToSpawn, CollisionManager iCollisionManager)
        {
            Debug.Assert(iBottomBound > 0);
            Debug.Assert(iNumberOfProjectilesToSpawn > 0);

            _activeProjectiles = new List<ProjectileActorBase>();
            _numGoodProjectilesToSpawn = iNumberOfProjectilesToSpawn;
            _collisionManager = iCollisionManager;
            _bottomBound = iBottomBound;
        }

        public bool IsSpawning()
        {
            return _numGoodProjectilesSpawnedSoFar < _numGoodProjectilesToSpawn;
        }

        public bool IsCompletelyFinished()
        {
            return !_activeProjectiles.Any();
        }

        public void Update(GameTime iGameTime)
        {
            if (_numGoodProjectilesSpawnedSoFar < _numGoodProjectilesToSpawn)
                SpawnNewProjectilesIfNeeded(iGameTime);

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

        protected abstract void SpawnNewProjectilesIfNeeded(GameTime iGameTime);

        protected void SpawnProjectile(ProjectileActorBase iProjectileActor)
        {
            switch (iProjectileActor)
            {
                case EasyProjectileActor _:
                case MediumProjectileActor _:
                case HardProjectileActor _:
                    _numGoodProjectilesSpawnedSoFar++;
                    _activeProjectiles.Add(iProjectileActor);
                    _collisionManager.Register(iProjectileActor);
                    break;
                case BadProjectileActor _:
                    _activeProjectiles.Add(iProjectileActor);
                    _collisionManager.Register(iProjectileActor);
                    break;
                default:
                    Debug.Fail($"Unknown derived type of {nameof(ProjectileActorBase)}");
                    throw new ArgumentOutOfRangeException(nameof(iProjectileActor));
            }
        }

        private readonly List<ProjectileActorBase> _activeProjectiles;
        private readonly int _numGoodProjectilesToSpawn;
        private readonly CollisionManager _collisionManager;

        private int _numGoodProjectilesSpawnedSoFar;
        private readonly int _bottomBound;
    }
}