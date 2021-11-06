using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Monocatch_Lib.Actors.Components
{
    public class PlayerMovementComponent : ActorComponentBase
    {
        protected sealed override void vUpdate(GameTime iGameTime)
        {
            var currentVelocity = Owner.GetActorVelocity();
            var mass = Owner.GetActorMass();

            switch (_movementAction)
            {
                case MovementAction.Glide:
                    Owner.AddForce(GetStoppingForce(currentVelocity, CGlideFrictionForce));
                    break;
                case MovementAction.Left:
                    Owner.AddForce(GetMovementForce(currentVelocity, new Vector2(-CMovementForce, 0.0f), mass, iGameTime));
                    break;
                case MovementAction.Right:
                    Owner.AddForce(GetMovementForce(currentVelocity, new Vector2(CMovementForce, 0.0f), mass, iGameTime));
                    break;
                case MovementAction.Halt:
                    Owner.AddForce(GetStoppingForce(currentVelocity, CHaltForce));
                    break;
                default:
                    Debug.Fail($"Unknown value of enum {nameof(MovementAction)}: {_movementAction}");
                    break;
            }
        }

        public void IntendNoneAction()
        {
            _movementAction = MovementAction.Glide;
        }

        public void IntendBothAction()
        {
            _movementAction = MovementAction.Halt;
        }

        public void IntendLeftAction()
        {
            _movementAction = MovementAction.Left;
        }

        public void IntendRightAction()
        {
            _movementAction = MovementAction.Right;
        }

        private MovementAction _movementAction;
        private const float CGlideFrictionForce = 50.0f;
        private const float CHaltForce = 100.0f;
        private const float CMovementForce = 50.0f;
        private const float CTopHorizontalSpeed = 100.0f;

        private enum MovementAction
        {
            Glide,
            Left,
            Right,
            Halt
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