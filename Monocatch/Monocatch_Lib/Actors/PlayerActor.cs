using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib.Actors
{
    public class PlayerActor : ActorBase
    {
        public PlayerActor(Vector2 iPosition, int iPlayerWidth, int iPlayerHeight, Color iFillColor, GameMaster iGame) : 
            base(iPosition, Vector2.Zero, 1.0f, true, iGame)
        {
            Debug.Assert(iPlayerWidth > 0);
            Debug.Assert(iPlayerHeight > 0);
            Debug.Assert(iFillColor != Color.Transparent);
            Debug.Assert(iGame != null);

            var colorData = new Color[iPlayerWidth * iPlayerHeight];
            
            for (var ii = 0; ii < colorData.Length; ii++)
            {
                colorData[ii] = iFillColor;
            }

            _texture = new Texture2D(iGame.GraphicsDevice, iPlayerWidth, iPlayerHeight);
            _texture.SetData(colorData);
            SetCollider(new BoxCollider(iPosition, new Vector2(iPosition.X + iPlayerWidth, iPosition.Y + iPlayerHeight)));
        }

        private readonly Texture2D _texture;

        protected override Texture2D GetTexture() => _texture;

        public override void OnCollision(ActorBase iOtherActor)
        {
            if (iOtherActor is WallActor)
            {
                SetActorVelocity(new Vector2(-1.05f * GetActorVelocity().X, GetActorVelocity().Y));
            }

            base.OnCollision(iOtherActor);
        }
    }
}