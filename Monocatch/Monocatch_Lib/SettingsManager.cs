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

            public static class Movement
            {
                public static class Ground
                {

                    public const float GlideFrictionXForce = 150.0f;
                    public const float MovementXForce = 150.0f;
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
                    public const float PostJumpBonusHorizontalMovementForce = 200.0f;
                    public const float PostJumpBonusHorizontalMovementTopSpeed = 145.0f;
                    public static TimeSpan PostJumpBonusTimeSpan = TimeSpan.FromSeconds(1.5);
                    public const float PostJumpBonusDecayForce = 100.0f;
                }

                public static TimeSpan BadCollisionRecovery = TimeSpan.FromSeconds(1);
            }
        }

        public static class BadProjectileSettings
        {
            public const float Mass = 1.0f;
        }

        public static class WorldSettings
        {
            public static Vector2 GravityForce = new Vector2(0.0f, 100.0f);
            public const float FloorLocationYAsFractionOfPlayAreaHeight = .75f;
        }

        public static class WallSettings
        {
            public const float Mass = 1.0f;
            public const float WallWidthAsFractionOfPlayAreaWidth = .01f;
        }

        public static class ProjectileManagerSettings
        {
            public const float DespawnHeightAsFractionOfPlayAreaHeight = 1.05f;
            public const float SpawnHeightAsFractionOfPlayAreaHeight = .015f;
        }

        public static class MainMenuSettings
        {
            public const float ButtonWidthAsFractionOfPlayAreaWidth = .25f;
            public const float ButtonHeightAsFractionOfPlayAreaHeight = .075f;
        }

        public static class PostSessionStatsSettings
        {
            public const float ButtonWidthAsFractionOfPlayAreaWidth = .30f;
            public const float ButtonHeightAsFractionOfPlayAreaHeight = .075f;
        }

        public static class Patterns
        {
            public static class Default
            {
                public const float MaxXSpeedOnSpawn = 100.0f;
                public static TimeSpan SpawningInterval = TimeSpan.FromSeconds(2);
                public const int NumEasyProjectiles = 25;
                public const int NumMediumProjectiles = 15;
                public const int NumHardProjectiles = 10;
                public const int TotalNumGoodProjectiles = NumEasyProjectiles + NumMediumProjectiles + NumHardProjectiles;
                public static Vector2 EasyVerticalVelocityOnSpawn = new Vector2(0.0f, 0.0f);
                public static float MediumVelocityHorizontalMultiplier = 2.0f;
                public static Vector2 MediumVerticalVelocityOnSpawn = new Vector2(0.0f, 10.0f);
                public static float HardVelocityHorizontalMultiplier = 3.0f;
                public static Vector2 HardVerticalVelocityOnSpawn = new Vector2(0.0f, 20.0f);
            }

            public class Bad
            {
                public const int TotalNumBadProjectiles = 25;
                public const float MaxXSpeedOnSpawn = Default.MaxXSpeedOnSpawn;
                public static TimeSpan SpawningInterval = TimeSpan.FromSeconds(Default.TotalNumGoodProjectiles * Default.SpawningInterval.TotalSeconds / TotalNumBadProjectiles);
                public static float SpawningYSpeed = 5.0f;
            }
        }

        public static class Projectiles
        {
            public static class Easy
            {
                public const int Radius = 8;
                public static Color FillColor = Color.White;
            }

            public static class Medium
            {
                public const int Radius = 8;
                public static Color FillColor = Color.Blue;
            }

            public static class Hard
            {
                public const int Radius = 8;
                public static Color FillColor = Color.Gold;
            }

            public static class Bad
            {
                public const int Radius = 8;
                public static Color FillColor = Color.OrangeRed;
            }
        }
    }
}
