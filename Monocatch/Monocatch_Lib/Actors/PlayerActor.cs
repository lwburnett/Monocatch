using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocatch_Lib.Actors.Components;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib.Actors
{
    public class PlayerActor : ActorBase
    {
        public PlayerActor(Vector2 iPosition, int iPlayerWidth, int iPlayerHeight, Color iFillColor) : 
            base(iPosition, Vector2.Zero, SettingsManager.PlayerSettings.Mass)
        {
            Debug.Assert(iPlayerWidth > 0);
            Debug.Assert(iPlayerHeight > 0);
            Debug.Assert(iFillColor != Color.Transparent);

            var colorData = new Color[iPlayerWidth * iPlayerHeight];
            
            for (var ii = 0; ii < colorData.Length; ii++)
            {
                colorData[ii] = iFillColor;
            }

            _texture = GraphicsHelper.CreateTexture(colorData, iPlayerWidth, iPlayerHeight);
            _texture.SetData(colorData);
        }

        private readonly Texture2D _texture;

        protected override Texture2D GetTexture() => _texture;

        public override void OnCollision(ActorBase iOtherActor, GameTime iGameTime)
        {
            void SignalJumpableCollisionToMovementComponent()
            {
                var movementComponent = GetComponentByType<PlayerMovementComponent>();
                Debug.Assert(movementComponent != null);
                movementComponent.SignalJumpableCollision(iGameTime);
            }

            if (iOtherActor is WallActor)
            {
                SignalJumpableCollisionToMovementComponent();
                SetActorVelocity(new Vector2(-1.00f * GetActorVelocity().X, GetActorVelocity().Y));
            }
            else if (iOtherActor is BasicProjectileActor)
            {
                SignalJumpableCollisionToMovementComponent();
            }

            base.OnCollision(iOtherActor, iGameTime);
        }
    }
}