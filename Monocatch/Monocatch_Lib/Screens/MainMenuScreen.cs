using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocatch_Lib.Ui;

namespace Monocatch_Lib.Screens
{
    public class MainMenuScreen : ScreenBase
    {
        public MainMenuScreen(Action iOnPlayCallback, Action iOnExitCallback, GameMaster iGameMaster) : base(iGameMaster)
        {
            _onPlayCallback = iOnPlayCallback;
            _onExitCallback = iOnExitCallback;
        }

        public override void OnNavigateTo()
        {
            Game.IsMouseVisible = true;

            var gamePlayAreaWidth = Game.GamePlayArea.Width;
            var gamePlayAreaHeight = Game.GamePlayArea.Height;

            var buttonWidth = (int)(gamePlayAreaWidth * SettingsManager.MainMenuSettings.ButtonWidthAsFractionOfPlayAreaWidth);
            var buttonHeight = (int)(gamePlayAreaHeight * SettingsManager.MainMenuSettings.ButtonHeightAsFractionOfPlayAreaHeight);

            var playButtonTopLeftX = Game.GamePlayArea.X + (gamePlayAreaWidth - buttonWidth) / 2;
            var playButtonTopLeftY = Game.GamePlayArea.Y + (gamePlayAreaHeight - 2 * buttonWidth) / 2;

            var exitButtonTopLeftX = Game.GamePlayArea.X + (gamePlayAreaWidth - buttonWidth) / 2;
            var exitButtonTopLeftY = Game.GamePlayArea.Y + (gamePlayAreaHeight + 2 * buttonHeight) / 2;

            _playButton = new UiTextButton(new Point(playButtonTopLeftX, playButtonTopLeftY), buttonWidth, buttonHeight, "Play", OnPlayClicked, Game);
            _exitButton = new UiTextButton(new Point(exitButtonTopLeftX, exitButtonTopLeftY), buttonWidth, buttonHeight, "Exit", OnExitClicked, Game);
        }

        public override void Update(GameTime iGameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _onExitCallback();

            if (_playButton.IsOverlappingWithMouse(Mouse.GetState().Position))
            {
                _exitButton.Reset();

                _playButton.OnOverlap();

                if (Mouse.GetState().LeftButton == ButtonState.Pressed) 
                    _playButton.OnPressed();
                else if (Mouse.GetState().LeftButton == ButtonState.Released)
                    _playButton.OnReleased();
            }
            else if (_exitButton.IsOverlappingWithMouse(Mouse.GetState().Position))
            {
                _playButton.Reset();

                _exitButton.OnOverlap();

                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    _exitButton.OnPressed();
                else if (Mouse.GetState().LeftButton == ButtonState.Released)
                    _exitButton.OnReleased();
            }
            else
            {
                if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    _playButton.Reset();
                    _exitButton.Reset();
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    _playButton.OnNotOverlap();
                    _exitButton.OnNotOverlap();
                }
            }
        }

        public override void Draw()
        {
            _playButton.Draw();
            _exitButton.Draw();
        }

        private UiTextButton _playButton;
        private UiTextButton _exitButton;
        private readonly Action _onPlayCallback;
        private readonly Action _onExitCallback;

        private void OnPlayClicked()
        {
            _onPlayCallback();
        }

        private void OnExitClicked()
        {
            _onExitCallback();
        }
    }
}