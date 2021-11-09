using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Screens
{
    public interface IScreen
    {
        void OnNavigateTo();

        void Update(GameTime iGameTime);

        void Draw();
    }
}