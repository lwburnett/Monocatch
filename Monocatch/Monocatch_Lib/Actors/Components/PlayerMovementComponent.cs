using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Monocatch_Lib.Actors.Components
{
    public class PlayerMovementComponent : ActorComponentBase
    {
        public PlayerMovementComponent() : base(true)
        {
            _thisTickIntendedMovementAction = MovementAction.Glide;
            _previousTickIntendedMovementAction = MovementAction.Glide;
            _physicalState = PhysicalState.Grounded;
            _floorLocation = 0.0f;
            _lastLandTime = TimeSpan.MinValue;
            _groundJumpWindupBegin = null;
            _lastGroundJumpTime = null;
            _lastJumpableCollisionTime = null;
            _bufferedAirborneJumpActionTime = null;
            _lastAirborneJumpTime = null;
        }

        #region ActorComponentBase

        public sealed override void Update(GameTime iGameTime)
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
                    var isUniqueJumpPressInstance = _previousTickIntendedMovementAction != MovementAction.Jump;
                    HandleJump(isUniqueJumpPressInstance, iGameTime);
                    break;
                default:
                    Debug.Fail($"Unknown value of enum {nameof(MovementAction)}: {_thisTickIntendedMovementAction}");
                    break;
            }

            base.Update(iGameTime);
        }

        protected override void OnOwnerRegistered()
        {
            _floorLocation = Owner.GetActorPosition().Y;

            base.OnOwnerRegistered();
        }

        #endregion

        public void IntendNoneAction()
        {
            _previousTickIntendedMovementAction = _thisTickIntendedMovementAction;
            _thisTickIntendedMovementAction = MovementAction.Glide;
        }

        public void IntendBothAction(GameTime iGameTime)
        {
            _previousTickIntendedMovementAction = _thisTickIntendedMovementAction;
            _thisTickIntendedMovementAction = MovementAction.Jump;
        }

        public void IntendLeftAction()
        {
            _previousTickIntendedMovementAction = _thisTickIntendedMovementAction;
            _thisTickIntendedMovementAction = MovementAction.Left;
        }

        public void IntendRightAction()
        {
            _previousTickIntendedMovementAction = _thisTickIntendedMovementAction;
            _thisTickIntendedMovementAction = MovementAction.Right;
        }

        public void SignalJumpableCollision(GameTime iGameTime)
        {
            _lastJumpableCollisionTime = iGameTime.TotalGameTime;

            // if ((!_lastAirborneJumpTime.HasValue || iGameTime.TotalGameTime - _lastAirborneJumpTime.Value > SettingsManager.PlayerSettings.CollisionJumpTimeProximity) &&
            //     (_bufferedAirborneJumpActionTime.HasValue && _lastJumpableCollisionTime - _bufferedAirborneJumpActionTime.Value > SettingsManager.PlayerSettings.CollisionJumpTimeProximity))
            // {
            //     Owner.AddForce(GetJumpForce(iGameTime));
            //     _lastAirborneJumpTime = iGameTime.TotalGameTime;
            // }

            if (iGameTime.TotalGameTime - _lastGroundJumpTime >= SettingsManager.PlayerSettings.CollisionJumpTimeProximity &&
                (!_lastAirborneJumpTime.HasValue || iGameTime.TotalGameTime - _lastAirborneJumpTime.Value >= SettingsManager.PlayerSettings.CollisionJumpTimeProximity))
            {
                if (_bufferedAirborneJumpActionTime.HasValue && iGameTime.TotalGameTime - _bufferedAirborneJumpActionTime.Value <= SettingsManager.PlayerSettings.CollisionJumpTimeProximity)
                {
                    Owner.AddForce(GetJumpForce(iGameTime) * .5f);
                    _lastAirborneJumpTime = iGameTime.TotalGameTime;
                    _bufferedAirborneJumpActionTime = null;
                }
            }
        }

        #region Implementation

        private MovementAction _thisTickIntendedMovementAction;
        private MovementAction _previousTickIntendedMovementAction;
        private PhysicalState _physicalState;
        private float _floorLocation;
        private TimeSpan _lastLandTime;
        private TimeSpan? _groundJumpWindupBegin;
        private TimeSpan? _lastGroundJumpTime;
        private TimeSpan? _lastJumpableCollisionTime;
        private TimeSpan? _bufferedAirborneJumpActionTime;
        private TimeSpan? _lastAirborneJumpTime;

        #region Helper Objects

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

        #endregion

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
                        _lastJumpableCollisionTime = null;
                        _bufferedAirborneJumpActionTime = null;
                        _lastAirborneJumpTime = null;
                        _lastGroundJumpTime = null;
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

        private void HandleJump(bool iIsUniqueJumpPressInstance, GameTime iGameTime)
        {
            switch (_physicalState)
            {
                case PhysicalState.Grounded:
                    if (!iIsUniqueJumpPressInstance)
                    {
                        if (_groundJumpWindupBegin.HasValue)
                        {
                            if (iGameTime.TotalGameTime - _groundJumpWindupBegin.Value >= SettingsManager.PlayerSettings.GroundJumpWindupTime)
                            {
                                Owner.AddForce(GetJumpForce(iGameTime));
                                _groundJumpWindupBegin = null;
                                _physicalState = PhysicalState.Airborne;
                                _lastGroundJumpTime = iGameTime.TotalGameTime;
                            }
                        }
                        else
                            HandleGlide();
                    }
                    else
                    {
                        _groundJumpWindupBegin = iGameTime.TotalGameTime;
                    }
                    break;
                case PhysicalState.Airborne:
                    if (iIsUniqueJumpPressInstance)
                    {
                        if (iGameTime.TotalGameTime - _lastGroundJumpTime >= SettingsManager.PlayerSettings.CollisionJumpTimeProximity &&
                            (!_lastAirborneJumpTime.HasValue || iGameTime.TotalGameTime - _lastAirborneJumpTime.Value >= SettingsManager.PlayerSettings.CollisionJumpTimeProximity))
                        {
                            if (_lastJumpableCollisionTime.HasValue && iGameTime.TotalGameTime - _lastJumpableCollisionTime.Value <= SettingsManager.PlayerSettings.CollisionJumpTimeProximity)
                            {
                                _lastAirborneJumpTime = iGameTime.TotalGameTime;
                                _bufferedAirborneJumpActionTime = null;
                                Owner.AddForce(GetJumpForce(iGameTime) * .5f);
                            }
                            else
                            {
                                _bufferedAirborneJumpActionTime = iGameTime.TotalGameTime;
                            }
                        }
                    }
                    break;
                case PhysicalState.LandRecovery:
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

        // Remember positive Y is down
        private Vector2 GetJumpForce(GameTime iGameTime)
        {
            var currentVelocity = Owner.GetActorVelocity();
            var mass = Owner.GetActorMass();

            return new Vector2(0, (currentVelocity.Y - SettingsManager.PlayerSettings.JumpVelocity) * mass / (float)iGameTime.ElapsedGameTime.TotalSeconds);
        }

        #endregion
    }
}