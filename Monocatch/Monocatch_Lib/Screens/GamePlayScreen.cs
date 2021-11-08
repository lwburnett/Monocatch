using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Monocatch_Lib.Screens
{
    public class GamePlayScreen : ScreenBase
    {
        public GamePlayScreen(GamePlayInstance iGamePlayInstance, Action iOnExitCallback, GameMaster iGameMaster) : base(iGameMaster)
        {
            _gamePlayInstance = iGamePlayInstance;
            _onExitCallback = iOnExitCallback;
        }

        public override void OnNavigateTo()
        {
            Game.IsMouseVisible = false;
            _gamePlayInstance.LoadLevel();
        }

        public override void Update(GameTime iGameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _onExitCallback();

            _gamePlayInstance.Update(iGameTime);
        }

        public override void Draw()
        {
            _gamePlayInstance.Draw();
        }

        private readonly Action _onExitCallback;
        private readonly GamePlayInstance _gamePlayInstance;
    }
}