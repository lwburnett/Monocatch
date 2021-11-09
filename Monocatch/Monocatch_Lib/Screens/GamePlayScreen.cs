using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Monocatch_Lib.Screens
{
    public class GamePlayScreen : IScreen
    {
        public GamePlayScreen(GamePlayInstance iGamePlayInstance, Action iOnExitCallback)
        {
            _gamePlayInstance = iGamePlayInstance;
            _onExitCallback = iOnExitCallback;
        }

        public void OnNavigateTo()
        {
            _gamePlayInstance.LoadLevel();
        }

        public void Update(GameTime iGameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _onExitCallback();

            _gamePlayInstance.Update(iGameTime);
        }

        public void Draw()
        {
            _gamePlayInstance.Draw();
        }

        private readonly Action _onExitCallback;
        private readonly GamePlayInstance _gamePlayInstance;
    }
}