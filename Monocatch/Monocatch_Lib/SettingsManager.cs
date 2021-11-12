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
            public const float WidthAsFractionOfPlayAreaWidth = .1f;
            public const float HeightAsFractionOfPlayAreaHeight = .016f;
            public const float SpawnHeightAsFractionOfPlayAreaHeight = .75f;

            public static class Movement
            {
                public static class Ground
                {

                    public const float GlideFrictionXForce = 50.0f;
                    public const float MovementXForce = 100.0f;
                    public const float TopXSpeed = 100.0f;
                    // ReSharper disable once InconsistentNaming
                    public static TimeSpan RecoveryTime = TimeSpan.FromSeconds(0);
                    public const float JumpYVelocity = 150.0f;
                    public static TimeSpan JumpWindupTime = TimeSpan.FromMilliseconds(100);
                }

                public static class Airborne
                {
                    public const float JumpYVelocityMinimum = Ground.JumpYVelocity * .75f;
                    public static TimeSpan CollisionJumpTimeProximity = TimeSpan.FromMilliseconds(100);
                    public const float PostJumpBonusHorizontalMovementForce = 125.0f;
                    public const float PostJumpBonusHorizontalMovementTopSpeed = 125.0f;
                    public static TimeSpan PostJumpBonusTimeSpan = TimeSpan.FromSeconds(1.5);
                    public const float PostJumpBonusDecayForce = 100.0f;
                }
            }
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
