using System;
using Microsoft.Xna.Framework;

namespace Monocatch_Lib
{
    public static class SettingsManager
    {
        public static class GameMasterSettings
        {
            public const float TargetScreenAspectRatio = 1.778f;
        }

        public static class PlayerSettings
        {
            public const float Mass = 1.0f;

            public const float GlideFrictionForce = 50.0f;
            public const float MovementForce = 85.0f;
            public const float TopHorizontalSpeed = 100.0f;
            // ReSharper disable once InconsistentNaming
            public static TimeSpan RecoveryTime = TimeSpan.FromSeconds(1);
            public const float GroundJumpVelocity = 100.0f;

            public const float WidthAsFractionOfPlayAreaWidth = .1f;
            public const float HeightAsFractionOfPlayAreaHeight = .016f;
            public const float SpawnHeightAsFractionOfPlayAreaHeight = .75f;
            public static TimeSpan GroundJumpWindupTime = TimeSpan.FromMilliseconds(100);
            public static TimeSpan CollisionJumpTimeProximity = TimeSpan.FromMilliseconds(100);
            public static float ForceOfCollisionJumpsAsPercentageOfGroundJumpForce = .8f;
        }

        public static class BasicProjectileSettings
        {
            public const float Mass = 1.0f;
        }

        public static class WorldSettings
        {
            public static Vector2 GravityForce = new Vector2(0.0f, 100.0f);
        }

        public static class WallSettings
        {
            public const float Mass = 1.0f;
            public const float WallWidthAsFractionOfPlayAreaWidth = .01f;
        }

        public static class ProjectileManagerSettings
        {
            public const float MaxXSpeedOnSpawn = 100.0f;
            public static TimeSpan SpawningInterval = TimeSpan.FromSeconds(2);
            public const float DespawnHeightAsFractionOfPlayAreaHeight = 1.05f;
            public const float SpawnHeightAsFractionOfPlayAreaHeight = .015f;
        }

        public static class MainMenuSettings
        {
            public const float ButtonWidthAsFractionOfPlayAreaWidth = .25f;
            public const float ButtonHeightAsFractionOfPlayAreaHeight = .075f;
        }
    }
}
