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
            _lastBadCollisionTime = null;
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
            UpdateThisTickIntendedAction(MovementAction.Glide);
        }

        public void IntendBothAction()
        {
            UpdateThisTickIntendedAction(MovementAction.Jump);
        }

        public void IntendLeftAction()
        {
            UpdateThisTickIntendedAction(MovementAction.Left);
        }

        public void IntendRightAction()
        {
            UpdateThisTickIntendedAction(MovementAction.Right);
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

            if (iGameTime.TotalGameTime - _lastGroundJumpTime >= SettingsManager.PlayerSettings.Movement.Airborne.CollisionJumpTimeProximity &&
                (!_lastAirborneJumpTime.HasValue || iGameTime.TotalGameTime - _lastAirborneJumpTime.Value >= SettingsManager.PlayerSettings.Movement.Airborne.CollisionJumpTimeProximity))
            {
                if (_bufferedAirborneJumpActionTime.HasValue && iGameTime.TotalGameTime - _bufferedAirborneJumpActionTime.Value <= SettingsManager.PlayerSettings.Movement.Airborne.CollisionJumpTimeProximity)
                {
                    Owner.SetActorVelocity(GetAirborneJumpVelocity());
                    _lastAirborneJumpTime = iGameTime.TotalGameTime;
                    _bufferedAirborneJumpActionTime = null;
                }
            }
        }

        public void SignalBadCollision(GameTime iGameTime)
        {
            _lastBadCollisionTime = iGameTime.TotalGameTime;
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
        private TimeSpan? _lastBadCollisionTime;


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

        private void UpdateThisTickIntendedAction(MovementAction iThisTickAction)
        {
            _previousTickIntendedMovementAction = _thisTickIntendedMovementAction;
            _thisTickIntendedMovementAction = iThisTickAction;
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
                    if (iGameTime.TotalGameTime - _lastLandTime > SettingsManager.PlayerSettings.Movement.Ground.RecoveryTime)
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
                Owner.AddForce(GetStoppingForce(currentVelocity, SettingsManager.PlayerSettings.Movement.Ground.GlideFrictionXForce));
            }
        }

        private void HandleHorizontalMovement(Vector2 iDirectionVector, GameTime iGameTime)
        {
            // If recent bad collision, ignore horizontal movement
            if (_lastBadCollisionTime.HasValue && iGameTime.TotalGameTime - _lastBadCollisionTime.Value < SettingsManager.PlayerSettings.Movement.BadCollisionRecovery)
                return;

            iDirectionVector.Normalize();
            Debug.Assert(iDirectionVector.Y == 0.0f, "Assuming perfectly horizontal vector");

            var currentVelocity = Owner.GetActorVelocity();
            var mass = Owner.GetActorMass();

            switch (_physicalState)
            {
                case PhysicalState.Grounded:
                case PhysicalState.LandRecovery:
                    Owner.AddForce(GetMovementForce(
                        currentVelocity, 
                        SettingsManager.PlayerSettings.Movement.Ground.TopXSpeed, 
                        iDirectionVector * SettingsManager.PlayerSettings.Movement.Ground.MovementXForce, 
                        mass, 
                        iGameTime));
                    break;
                case PhysicalState.Airborne:
                    if (!_lastAirborneJumpTime.HasValue || iGameTime.TotalGameTime - _lastAirborneJumpTime.Value > SettingsManager.PlayerSettings.Movement.Airborne.PostJumpBonusTimeSpan)
                    {
                        if (Math.Abs(currentVelocity.X) <= SettingsManager.PlayerSettings.Movement.Ground.TopXSpeed)
                        {
                            Owner.AddForce(GetMovementForce(
                                currentVelocity, 
                                SettingsManager.PlayerSettings.Movement.Ground.TopXSpeed,
                                iDirectionVector * SettingsManager.PlayerSettings.Movement.Ground.MovementXForce, 
                                mass, 
                                iGameTime));
                        }
                        else
                        {
                            var areInputDirectionsAntiparallel = Vector2.Dot(iDirectionVector, currentVelocity) < 0;
                            if (areInputDirectionsAntiparallel)
                            {
                                Owner.AddForce(iDirectionVector * SettingsManager.PlayerSettings.Movement.Ground.MovementXForce);
                            }
                            else
                            {
                                Owner.AddForce(-1.0f * iDirectionVector * SettingsManager.PlayerSettings.Movement.Airborne.PostJumpBonusDecayForce);
                            }
                        }
                    }
                    else
                    {
                        Owner.AddForce(GetMovementForce(
                            currentVelocity,
                            SettingsManager.PlayerSettings.Movement.Airborne.PostJumpBonusHorizontalMovementTopSpeed,
                            iDirectionVector * SettingsManager.PlayerSettings.Movement.Airborne.PostJumpBonusHorizontalMovementForce,
                            mass,
                            iGameTime));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleJump(bool iIsUniqueJumpPressInstance, GameTime iGameTime)
        {
            // If recent bad collision, ignore jumping
            if (_lastBadCollisionTime.HasValue && iGameTime.TotalGameTime - _lastBadCollisionTime.Value < SettingsManager.PlayerSettings.Movement.BadCollisionRecovery)
                return;

            switch (_physicalState)
            {
                case PhysicalState.Grounded:
                    if (!iIsUniqueJumpPressInstance)
                    {
                        if (_groundJumpWindupBegin.HasValue)
                        {
                            if (iGameTime.TotalGameTime - _groundJumpWindupBegin.Value >= SettingsManager.PlayerSettings.Movement.Ground.JumpWindupTime)
                            {
                                Owner.SetActorVelocity(new Vector2(Owner.GetActorVelocity().X, -1.0f * SettingsManager.PlayerSettings.Movement.Ground.JumpYVelocity));
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
                        if (iGameTime.TotalGameTime - _lastGroundJumpTime >= SettingsManager.PlayerSettings.Movement.Airborne.CollisionJumpTimeProximity &&
                            (!_lastAirborneJumpTime.HasValue || iGameTime.TotalGameTime - _lastAirborneJumpTime.Value >= SettingsManager.PlayerSettings.Movement.Airborne.CollisionJumpTimeProximity))
                        {
                            if (_lastJumpableCollisionTime.HasValue && iGameTime.TotalGameTime - _lastJumpableCollisionTime.Value <= SettingsManager.PlayerSettings.Movement.Airborne.CollisionJumpTimeProximity)
                            {
                                //Owner.AddForce(GetJumpForce(iGameTime) * SettingsManager.PlayerSettings.ForceOfCollisionJumpsAsPercentageOfGroundJumpForce);
                                Owner.SetActorVelocity(GetAirborneJumpVelocity());
                                _lastAirborneJumpTime = iGameTime.TotalGameTime;
                                _bufferedAirborneJumpActionTime = null;
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

        private Vector2 GetAirborneJumpVelocity()
        {
            float GetXSpeed(float iCurrentXSpeed) =>
                iCurrentXSpeed * SettingsManager.PlayerSettings.Movement.Airborne.PostJumpBonusHorizontalMovementTopSpeed / SettingsManager.PlayerSettings.Movement.Ground.TopXSpeed;

            float GetYSpeed(float iCurrentYSpeed)
            {
                var yVelocityCeiling = -1.0f * SettingsManager.PlayerSettings.Movement.Airborne.JumpYVelocityMinimum;
                var yVelocityFloor = -1.0f * SettingsManager.PlayerSettings.Movement.Ground.JumpYVelocity;

                if (iCurrentYSpeed > yVelocityCeiling)
                    return yVelocityCeiling;
                if (iCurrentYSpeed < yVelocityFloor)
                    return Owner.GetActorVelocity().Y;

                // f(x) = ax^2 + bx + c polynomial scaling between max and min exit Y velocities where:
                // f(0) = ceiling
                // f(floor) = floor
                // f'(floor) = 0
                //
                // Solving these equations gives:
                // a = ceiling / (floor^2) - 1 / floor
                // b = 2 - (2 ceiling) / floor
                // c = ceiling
                var aCoefficient = (yVelocityCeiling / yVelocityFloor / yVelocityFloor) - (1.0f / yVelocityFloor);
                var bCoefficient = 2 - (2 * yVelocityCeiling / yVelocityFloor);
                var cCoefficient = yVelocityCeiling;

                var newYVelocity = aCoefficient * aCoefficient * iCurrentYSpeed + bCoefficient * iCurrentYSpeed + cCoefficient;

                return newYVelocity;
            }

            var currentXVelocity = Owner.GetActorVelocity().X;
            var currentYVelocity = Owner.GetActorVelocity().Y;

            return new Vector2(GetXSpeed(currentXVelocity), GetYSpeed(currentYVelocity));
        }

        // What happens if current velocity is greater than top speed?
        private static Vector2 GetMovementForce(Vector2 iCurrentVelocity, float iTopHorizontalSpeed, Vector2 iMovementForce, float iMass, GameTime iGameTime)
        {
            if (Math.Abs(iCurrentVelocity.X) < iTopHorizontalSpeed)
            {
                var movementForceDirection = iMovementForce;
                movementForceDirection.Normalize();
                var forceToTopSpeedThisTick = (iTopHorizontalSpeed * movementForceDirection - iCurrentVelocity) * iMass / (float)iGameTime.ElapsedGameTime.TotalSeconds;

                return (Math.Abs(iMovementForce.Length()) < Math.Abs(forceToTopSpeedThisTick.Length())) ?
                    iMovementForce :
                    forceToTopSpeedThisTick;
            }
            else
            {
                var potentialFinalVelocity = iMovementForce * (float)iGameTime.ElapsedGameTime.TotalSeconds / iMass + iCurrentVelocity;
                var potentialSpeed = potentialFinalVelocity.Length();

                return potentialSpeed < Math.Abs(iCurrentVelocity.X) ? iMovementForce : Vector2.Zero;
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

        #endregion
    }
}