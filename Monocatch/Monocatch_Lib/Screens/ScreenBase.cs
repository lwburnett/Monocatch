using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Screens
{
    public abstract class ScreenBase
    {
        protected ScreenBase(GameMaster iGameMaster)
        {
            Game = iGameMaster;
        }

        public abstract void OnNavigateTo();

        public abstract void Update(GameTime iGameTime);

        public abstract void Draw();

        protected GameMaster Game;
    }
}