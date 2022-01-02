using System;
using Microsoft.Xna.Framework;

namespace Monocatch_Lib.Ui
{
    public interface IUiElement
    {
        void Update(GameTime iGameTime);
        void Draw();
    }
}