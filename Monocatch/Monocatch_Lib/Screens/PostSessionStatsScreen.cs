using System;
using Microsoft.Xna.Framework;
using Monocatch_Lib.Ui;

namespace Monocatch_Lib.Screens
{
    public class PostSessionStatsScreen : IScreen
    {
        public PostSessionStatsScreen(Action iPlayAgainAction, Action iMainMenuAction)
        {
            _onPlayAgainCallback = iPlayAgainAction;
            _onMainMenuCallback = iMainMenuAction;
        }

        public void OnNavigateTo()
        {
            var gamePlayAreaWidth = GraphicsHelper.GamePlayArea.Width;
            var gamePlayAreaHeight = GraphicsHelper.GamePlayArea.Height;

            var buttonWidth = (int)(gamePlayAreaWidth * SettingsManager.PostSessionStatsSettings.ButtonWidthAsFractionOfPlayAreaWidth);
            var buttonHeight = (int)(gamePlayAreaHeight * SettingsManager.PostSessionStatsSettings.ButtonHeightAsFractionOfPlayAreaHeight);

            var playButtonTopLeftX = GraphicsHelper.GamePlayArea.X + (gamePlayAreaWidth - buttonWidth) / 2;
            var playButtonTopLeftY = GraphicsHelper.GamePlayArea.Y + (gamePlayAreaHeight - 2 * buttonWidth) / 2;

            var exitButtonTopLeftX = GraphicsHelper.GamePlayArea.X + (gamePlayAreaWidth - buttonWidth) / 2;
            var exitButtonTopLeftY = GraphicsHelper.GamePlayArea.Y + (gamePlayAreaHeight + 2 * buttonHeight) / 2;

            _playAgainButton = new UiTextButton(new Point(playButtonTopLeftX, playButtonTopLeftY), buttonWidth, buttonHeight, "Play Again", OnPlayAgainClicked);
            _mainMenuButton = new UiTextButton(new Point(exitButtonTopLeftX, exitButtonTopLeftY), buttonWidth, buttonHeight, "Main Menu", OnMainMenuClicked);

            _scoreBanner = new UiFloatingText(new Point(playButtonTopLeftX, playButtonTopLeftY - 50), "Finish", Color.Aqua);
        }

        public void Update(GameTime iGameTime)
        {
            _scoreBanner.Update(iGameTime);
            _playAgainButton.Update(iGameTime);
            _mainMenuButton.Update(iGameTime);
        }

        public void Draw()
        {
            _scoreBanner.Draw();
            _playAgainButton.Draw();
            _mainMenuButton.Draw();
        }

        private IUiElement _scoreBanner;
        private IUiElement _playAgainButton;
        private IUiElement _mainMenuButton;
        private readonly Action _onPlayAgainCallback;
        private readonly Action _onMainMenuCallback;

        private void OnPlayAgainClicked()
        {
            _onPlayAgainCallback();
        }

        private void OnMainMenuClicked()
        {
            _onMainMenuCallback();
        }
    }
}