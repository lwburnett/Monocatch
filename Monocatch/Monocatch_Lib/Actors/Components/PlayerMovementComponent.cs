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
        private const float CGlideFrictionForce = 50.0f;
        private const float CMovementForce = 50.0f;
        private const float CTopHorizontalSpeed = 100.0f;
        // ReSharper disable once InconsistentNaming
        private readonly TimeSpan CRecoveryTime = TimeSpan.FromSeconds(1);
        private const float CJumpVelocity = 100.0f;

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
                        Owner.AddForce(Game.GravityForce);
                    }
                    break;
                case PhysicalState.LandRecovery:
                    if (iGameTime.TotalGameTime - _lastLandTime > CRecoveryTime)
                        _physicalState = PhysicalState.Grounded;
                    break;
                default:
                    Debug.Fail($"Unknown value of enum {nameof(PhysicalState)}: {_physicalState}");
                    break;
            }
        }

        private void HandleGlide()
        {
            var currentVelocity = Owner.GetActorVelocity();
            Owner.AddForce(GetStoppingForce(currentVelocity, CGlideFrictionForce));
        }

        private void HandleHorizontalMovement(Vector2 iDirectionVector, GameTime iGameTime)
        {
            iDirectionVector.Normalize();
            Debug.Assert(iDirectionVector.Y == 0.0f, "Assuming perfectly horizontal vector");

            var currentVelocity = Owner.GetActorVelocity();
            var mass = Owner.GetActorMass();

            Owner.AddForce(GetMovementForce(currentVelocity, iDirectionVector * CMovementForce, mass, iGameTime));
        }

        private void HandleJump(GameTime iGameTime)
        {
            var currentVelocity = Owner.GetActorVelocity();
            var mass = Owner.GetActorMass();

            switch (_physicalState)
            {
                case PhysicalState.Grounded:
                    // Remember positive Y is down
                    var forceToGetDesiredVelocity = 
                        (currentVelocity.Y - CJumpVelocity) * mass / (float)iGameTime.ElapsedGameTime.TotalSeconds;
                    Owner.AddForce(new Vector2(0, forceToGetDesiredVelocity));
                    _physicalState = PhysicalState.Airborne;
                    break;
                case PhysicalState.Airborne:
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
            if (iCurrentVelocity.Length() < CTopHorizontalSpeed)
            {
                var movementForceDirection = iMovementForce;
                movementForceDirection.Normalize();
                var forceToTopSpeedThisTick = (CTopHorizontalSpeed * movementForceDirection - iCurrentVelocity) * iMass / (float)iGameTime.ElapsedGameTime.TotalSeconds;

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