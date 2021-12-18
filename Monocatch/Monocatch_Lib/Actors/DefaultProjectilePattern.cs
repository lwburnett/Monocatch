using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocatch_Lib.Actors.Components;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib.Actors
{
    public class DefaultProjectilePattern : ProjectilePatternBase
    {
        public DefaultProjectilePattern(int iStartingHeight, int iBottomBound, int iLeftBound, int iRightBound, CollisionManager iCollisionManager) :
            base(iBottomBound, SettingsManager.Patterns.Default.TotalNumGoodProjectiles, iCollisionManager)
        {
            _startingHeight = iStartingHeight;
            _leftXBound = iLeftBound;
            _rightXBound = iRightBound;
            _maxSpawningXSpeed = SettingsManager.Patterns.Default.MaxXSpeedOnSpawn;
            _lastSpawn = TimeSpan.Zero;
            _spawnIntervalSeconds = SettingsManager.Patterns.Default.SpawningInterval;
            _rng = new Random();
            _projectileList = new List<ProjectileType>();
            _projectileIndex = 0;

            LoadProjectileDistribution();
        }

        protected override void SpawnNewProjectilesIfNeeded(GameTime iGameTime)
        {
            var timePassedSinceLastSpawn = iGameTime.TotalGameTime - _lastSpawn;
            if (timePassedSinceLastSpawn > _spawnIntervalSeconds)
            {
                var newProjectile = GetNewProjectile(_projectileList[_projectileIndex]);
                SpawnProjectile(newProjectile);
                _lastSpawn = iGameTime.TotalGameTime;
                _projectileIndex++;
            }
        }

        private readonly int _startingHeight;
        private readonly int _leftXBound;
        private readonly int _rightXBound;
        private readonly float _maxSpawningXSpeed;
        private TimeSpan _lastSpawn;
        private readonly TimeSpan _spawnIntervalSeconds;
        private readonly Random _rng;
        private readonly List<ProjectileType> _projectileList;
        private int _projectileIndex;

        // Algorithm inspired by http://keyj.emphy.de/balanced-shuffle/
        private enum ProjectileType
        {
            Easy,
            Medium,
            Hard
        }

        private enum TrimStrategy
        {
            Front,
            Back
        }

        private void LoadProjectileDistribution()
        {
            // ----- Fill algorithm -----
            const int numEasy = SettingsManager.Patterns.Default.NumEasyProjectiles;
            const int numMedium = SettingsManager.Patterns.Default.NumMediumProjectiles;
            const int numHard = SettingsManager.Patterns.Default.NumHardProjectiles;

            // Find size to fill into
            var fillSize = Math.Max(Math.Max(numEasy, numMedium), numHard);

            var filledEasyList = GetFilledList(numEasy, fillSize, ProjectileType.Easy, TrimStrategy.Front).ToList();
            var filledMediumList = GetFilledList(numMedium, fillSize, ProjectileType.Medium, TrimStrategy.Back).ToList();
            var filledHardList = GetFilledList(numHard, fillSize, ProjectileType.Hard, TrimStrategy.Back).ToList();
            
            for (var ii = 0; ii < fillSize; ii++)
            {
                var thisIterationOptions = new List<ProjectileType>();

                if (filledEasyList[ii] != null)
                    thisIterationOptions.Add(filledEasyList[ii].Value);
                if (filledMediumList[ii] != null)
                    thisIterationOptions.Add(filledMediumList[ii].Value);
                if (filledHardList[ii] != null)
                    thisIterationOptions.Add(filledHardList[ii].Value);

                var originalNumOptions = thisIterationOptions.Count;
                for (var jj = 0; jj < originalNumOptions; jj++)
                {
                    var indexChoice = _rng.Next(0, thisIterationOptions.Count);
                    _projectileList.Add(thisIterationOptions[indexChoice]);
                    thisIterationOptions.RemoveAt(indexChoice);
                }
            }
        }

        private IEnumerable<ProjectileType?> GetFilledList(int iNumProjectiles, int iFilledSize, ProjectileType iType, TrimStrategy iTrimStrategy)
        {
            var deviation = .25;

            ProjectileType? minorityValue;
            ProjectileType? majorityValue;
            int numSegments;

            var numNull = iFilledSize - iNumProjectiles;
            if (iNumProjectiles >= numNull)
            {
                minorityValue = null;
                majorityValue = iType;
                numSegments = numNull;
            }
            else
            {
                minorityValue = iType;
                majorityValue = null;
                numSegments = iNumProjectiles;
            }

            if (numSegments == 0)
                return Enumerable.Repeat((ProjectileType?)iType, iFilledSize);

            var defaultSegmentLength = Math.Max(Math.Ceiling((double)iFilledSize / numSegments), 1);

            var smallestSegmentLength = Math.Max((int)(defaultSegmentLength * (1 - deviation)), 1);
            var largestSegmentLength = (int)Math.Ceiling(defaultSegmentLength * (1 + deviation));

            // Make segments
            var filledList = new List<ProjectileType?>();
            for (var ii = 0; ii < numSegments; ii++)
            {
                var thisSegmentLength = _rng.Next(smallestSegmentLength, largestSegmentLength + 1);

                filledList.Add(minorityValue);

                for (var jj = 0; jj < thisSegmentLength - 1; jj++) 
                    filledList.Add(majorityValue);
            }

            if (filledList.Count < iFilledSize)
            {
                filledList.AddRange(Enumerable.Repeat(majorityValue, iFilledSize - filledList.Count));
            }

            // Variable size offset
            var numMajorityPaddingEnd = 0;
            for (var ii = 0; ii < filledList.Count; ii++)
            {
                if (filledList[filledList.Count - 1 - ii] == majorityValue)
                    numMajorityPaddingEnd++;
                else
                    break;
            }
            

            var offset = numMajorityPaddingEnd > 0 ? _rng.Next(0, numMajorityPaddingEnd / 2) : 0;

            for (var ii = 0; ii < offset; ii++)
            {
                filledList.RemoveAt(filledList.Count - 1);
                filledList.Insert(0, majorityValue);
            }

            // Trim down to size based on trim strategy
            if (filledList.Count > iFilledSize)
            {
                if (iTrimStrategy == TrimStrategy.Front)
                {
                    var ii = 0;
                    while (filledList.Count > iFilledSize)
                    {
                        if (filledList[ii] == majorityValue)
                            filledList.RemoveAt(ii);
                        else
                            ii++;
                    }
                }
                else
                {
                    var ii = filledList.Count - 1;
                    while (filledList.Count > iFilledSize)
                    {
                        if (filledList[ii] == majorityValue)
                            filledList.RemoveAt(ii);
                        ii--;
                    }
                }
            }

            return filledList;
        }

        private ProjectileActorBase GetNewProjectile(ProjectileType iProjectileType)
        {
            const int projectileRadius = 8;
            var spawningPositionX = _rng.Next(_leftXBound + projectileRadius, _rightXBound - projectileRadius - projectileRadius - projectileRadius);

            var spawningSpeedX = (int)((_maxSpawningXSpeed) * (-1.0f + 2.0f * _rng.NextDouble()));

            var position = new Vector2(spawningPositionX, _startingHeight);
            var velocity = new Vector2(spawningSpeedX, 0);

            ProjectileActorBase actor;

            switch (iProjectileType)
            {
                case ProjectileType.Easy:
                    actor = new EasyProjectileActor(position, velocity);
                    break;
                case ProjectileType.Medium:
                    actor = new MediumProjectileActor(position, velocity);
                    break;
                case ProjectileType.Hard:
                    actor = new HardProjectileActor(position, velocity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(iProjectileType), iProjectileType, null);
            }

            var collider = new CircleCollider(actor.GetActorPosition() + new Vector2(projectileRadius, projectileRadius), projectileRadius);
            actor.RegisterComponent(new CollisionComponent(collider));

            return actor;
        }
    }
}