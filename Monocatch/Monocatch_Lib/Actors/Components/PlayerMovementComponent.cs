using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Monocatch_Lib.Actors.Components
{
    public class PlayerMovementComponent : ActorComponentBase
    {
        public PlayerMovementComponent(GameMaster iGameMaster) : base(iGameMaster)
        {
            _thisTickIntendedMovementAction = MovementAction.Glide;
            _physicalState = PhysicalState.Grounded;
            _floorLocation = 0.0f;
            _lastLandTime = TimeSpan.MinValue;
            _jumpWindupBegin = null;
        }

        protected sealed override void vUpdate(GameTime iGameTime)
        {
            UpdatePhysicalState(iGameTime);

            switch (_thisTickIntendedMovementAction)
            {
                case MovementAction.Glide:
                    HandleGlide();
                    break;
                case MovementAction.Left:
                    HandleHorizontalMovement(new Vector2(-1.0f, 0), iGameTime);
                    break;
                case MovementAction.Right:
                    HandleHorizontalMovement(new Vector2(1.0f, 0), iGameTime);
                    break;
                case MovementAction.Jump:
                    HandleJump(iGameTime);
                    break;
                default:
                    Debug.Fail($"Unknown value of enum {nameof(MovementAction)}: {_thisTickIntendedMovementAction}");
                    break;
            }
        }

        protected override void OnOwnerRegistered()
        {
            _floorLocation = Owner.GetActorPosition().Y;

            base.OnOwnerRegistered();
        }

        public void IntendNoneAction()
        {
            _thisTickIntendedMovementAction = MovementAction.Glide;
        }

        public void IntendBothAction()
        {
            _thisTickIntendedMovementAction = MovementAction.Jump;
        }

        public void IntendLeftAction()
        {
            _thisTickIntendedMovementAction = MovementAction.Left;
        }

        public void IntendRightAction()
        {
            _thisTickIntendedMovementAction = MovementAction.Right;
        }

        private MovementAction _thisTickIntendedMovementAction;
        private PhysicalState _physicalState;
        private float _floorLocation;
        private TimeSpan _lastLandTime;
        private TimeSpan? _jumpWindupBegin;

        private enum MovementAction
        {
            Glide,
            Left,
            Right,
            Jump
        }

        private enum PhysicalState
        {
            Grounded,
            Airborne,
            LandRecovery
        }

        private void UpdatePhysicalState(GameTime iGameTime)
        {
            switch (_physicalState)
            {
                case PhysicalState.Grounded:
                    break;
                case PhysicalState.Airborne:
                    if (Owner.GetActorPosition().Y > _floorLocation)
                    {
                        Owner.SetActorPosition(new Vector2(Owner.GetActorPosition().X, _floorLocation));
                        Owner.SetActorVelocity(new Vector2(Owner.GetActorVelocity().X, 0.0f));
                        _physicalState = PhysicalState.LandRecovery;
                        _lastLandTime = iGameTime.TotalGameTime;
                    }
                    else
                    {
                        Owner.AddForce(SettingsManager.WorldSettings.GravityForce);
                    }
                    break;
                case PhysicalState.LandRecovery:
                    if (iGameTime.TotalGameTime - _lastLandTime > SettingsManager.PlayerSettings.RecoveryTime)
                        _physicalState = PhysicalState.Grounded;
                    break;
                default:
                    Debug.Fail($"Unknown value of enum {nameof(PhysicalState)}: {_physicalState}");
                    break;
            }
        }

        private void HandleGlide()
        {
            if (_physicalState != PhysicalState.Airborne)
            {
                var currentVelocity = Owner.GetActorVelocity();
                Owner.AddForce(GetStoppingForce(currentVelocity, SettingsManager.PlayerSettings.GlideFrictionForce));
            }
        }

        private void HandleHorizontalMovement(Vector2 iDirectionVector, GameTime iGameTime)
        {
            iDirectionVector.Normalize();
            Debug.Assert(iDirectionVector.Y == 0.0f, "Assuming perfectly horizontal vector");

            var currentVelocity = Owner.GetActorVelocity();
            var mass = Owner.GetActorMass();

            Owner.AddForce(GetMovementForce(currentVelocity, iDirectionVector * SettingsManager.PlayerSettings.MovementForce, mass, iGameTime));
        }

        private void HandleJump(GameTime iGameTime)
        {
            var currentVelocity = Owner.GetActorVelocity();
            var mass = Owner.GetActorMass();

            switch (_physicalState)
            {
                case PhysicalState.Grounded:
                    if (_jumpWindupBegin.HasValue)
                    {
                        if (iGameTime.TotalGameTime - _jumpWindupBegin.Value >= SettingsManager.PlayerSettings.JumpWindupTime)
                        {
                            // Remember positive Y is down
                            var forceToGetDesiredVelocity = (currentVelocity.Y - SettingsManager.PlayerSettings.JumpVelocity) * mass / (float)iGameTime.ElapsedGameTime.TotalSeconds;
                            Owner.AddForce(new Vector2(0, forceToGetDesiredVelocity));
                            _physicalState = PhysicalState.Airborne;
                        }
                    }
                    else
                    {
                        _jumpWindupBegin = iGameTime.TotalGameTime;
                    }
                    break;
                case PhysicalState.Airborne:
                case PhysicalState.LandRecovery:
                    _jumpWindupBegin = null;
                    break;
                default:
                    Debug.Fail($"Unknown value of enum {nameof(PhysicalState)}: {_physicalState}");
                    break;
            }
        }

        // What happens if current velocity is greater than top speed?
        private static Vector2 GetMovementForce(Vector2 iCurrentVelocity, Vector2 iMovementForce, float iMass, GameTime iGameTime)
        {
            if (iCurrentVelocity.Length() < SettingsManager.PlayerSettings.TopHorizontalSpeed)
            {
                var movementForceDirection = iMovementForce;
                movementForceDirection.Normalize();
                var forceToTopSpeedThisTick = (SettingsManager.PlayerSettings.TopHorizontalSpeed * movementForceDirection - iCurrentVelocity) * iMass / (float)iGameTime.ElapsedGameTime.TotalSeconds;

                return (Math.Abs(iMovementForce.Length()) < Math.Abs(forceToTopSpeedThisTick.Length())) ?
                    iMovementForce :
                    forceToTopSpeedThisTick;
            }
            else
            {
                var potentialFinalVelocity = iMovementForce * (float)iGameTime.ElapsedGameTime.TotalSeconds / iMass + iCurrentVelocity;
                var potentialSpeed = potentialFinalVelocity.Length();

                return potentialSpeed < iCurrentVelocity.Length() ? iMovementForce : Vector2.Zero;
            }
        }

        private static Vector2 GetStoppingForce(Vector2 iCurrentVelocity, float iStoppingForceMag)
        {
            if (Math.Abs(iCurrentVelocity.X) < .0001)
                return Vector2.Zero;

            var directionUnitVector = new Vector2(-1 * iCurrentVelocity.X, 0);
            directionUnitVector.Normalize();
            return directionUnitVector * iStoppingForceMag;
        }
    }
}