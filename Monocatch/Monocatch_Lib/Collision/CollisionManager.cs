using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Monocatch_Lib.Actors;
using Monocatch_Lib.Actors.Components;

namespace Monocatch_Lib.Collision
{
    public class CollisionManager
    {
        public CollisionManager()
        {
            _collidableActors = new List<ActorBase>();
        }

        public void Register(ActorBase iActor)
        {
            _collidableActors.Add(iActor);
        }

        public void Unregister(ActorBase iActor)
        {
            var success = _collidableActors.Remove(iActor);
            Debug.Assert(success, "Actor to be removed is not part of collision network. Strange.");
        }

        public void Update(GameTime iGameTime)
        {
            for (var ii = 0; ii < _collidableActors.Count; ii++)
            {
                var actor1 = _collidableActors[ii];

                for (var jj = ii + 1; jj < _collidableActors.Count; jj++)
                {
                    var actor2 = _collidableActors[jj];

                    if (actor1 == actor2)
                        continue;

                    var collider1 = actor1.GetComponentByType<CollisionComponent>()?.GetCollider();
                    var collider2 = actor2.GetComponentByType<CollisionComponent>()?.GetCollider();

                    Debug.Assert(collider1 != null);
                    Debug.Assert(collider2 != null);

                    var areColliding = false;
                    if (collider1 is BoxCollider boxColliderP11 && collider2 is BoxCollider boxColliderP12)
                    {
                        areColliding = DoCollisionDetection(boxColliderP11, boxColliderP12);
                    }
                    else if (collider1 is CircleCollider circleColliderP21 && collider2 is CircleCollider circleColliderP22)
                    {
                        areColliding = DoCollisionDetection(circleColliderP21, circleColliderP22);
                    }
                    else if (collider1 is BoxCollider boxColliderP31 && collider2 is CircleCollider circleColliderP31)
                    {
                        areColliding = DoCollisionDetection(boxColliderP31, circleColliderP31);
                    }
                    else if (collider1 is CircleCollider circleColliderP41 && collider2 is BoxCollider boxColliderP42)
                    {
                        areColliding = DoCollisionDetection(boxColliderP42, circleColliderP41);
                    }
                    else
                    {
                        Debug.Fail("Unknown collision geometry configuration.");
                    }

                    if (!areColliding) 
                        continue;

                    actor1.OnCollision(actor2, iGameTime);
                    actor2.OnCollision(actor1, iGameTime);
                }
            }
        }

        private readonly List<ActorBase> _collidableActors;

        private static bool DoCollisionDetection(BoxCollider iBoxCollider1, BoxCollider iBoxCollider2)
        {
            return iBoxCollider1.Left < iBoxCollider2.Right &&
                   iBoxCollider1.Right > iBoxCollider2.Left &&
                   iBoxCollider1.Top < iBoxCollider2.Bottom &&
                   iBoxCollider1.Bottom > iBoxCollider2.Top;

        }

        private static bool DoCollisionDetection(CircleCollider iCircleCollider1, CircleCollider iCircleCollider2)
        {
            var centerPoint1 = iCircleCollider1.Center;
            var centerPoint2 = iCircleCollider2.Center;

            var distance = (centerPoint1 - centerPoint2).Length();

            return Math.Abs(distance) < iCircleCollider1.Radius + iCircleCollider2.Radius;
        }

        private static bool DoCollisionDetection(BoxCollider iBoxCollider, CircleCollider iCircleCollider)
        {
            var circleLeft = iCircleCollider.Center.X - iCircleCollider.Radius;
            var circleTop = iCircleCollider.Center.Y - iCircleCollider.Radius;
            var circleRight = circleLeft + iCircleCollider.Radius * 2;
            var circleBottom = circleTop + iCircleCollider.Radius * 2;

            return iBoxCollider.Left < circleRight &&
                   iBoxCollider.Right > circleLeft &&
                   iBoxCollider.Top < circleBottom &&
                   iBoxCollider.Bottom > circleTop;
        }
    }
}