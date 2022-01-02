using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocatch_Lib.Actors.Components;

namespace Monocatch_Lib.Screens
{
    public class GamePlayScreen : IScreen
    {
        public GamePlayScreen(TimeSpan iGameTime, Action iOnExitCallback)
        {
            _gamePlayInstance = new GamePlayInstance(iGameTime, OnGamePlaySessionFinished);
            _onExitCallback = iOnExitCallback;
            _subScreen = SubScreen.GamePlay;
        }

        public void OnNavigateTo()
        {
            _gamePlayInstance.LoadLevel();
        }

        public void Update(GameTime iGameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _onExitCallback();

            switch (_subScreen)
            {
                case SubScreen.GamePlay:
                    _gamePlayInstance.Update(iGameTime);
                    break;
                case SubScreen.PostSessionStats:
                    _postSessionStatsScreen.Update(iGameTime);
                    break;
                default:
                    Debug.Fail($"Unknown value of enum {nameof(SubScreen)}: {_subScreen}");
                    break;
            }
        }

        public void Draw()
        {
            _gamePlayInstance.Draw();

            switch (_subScreen)
            {
                case SubScreen.GamePlay:
                    break;
                case SubScreen.PostSessionStats:
                    _postSessionStatsScreen.Draw();
                    break;
                default:
                    Debug.Fail($"Unknown value of enum {nameof(SubScreen)}: {_subScreen}");
                    break;
            }
        }

        private readonly Action _onExitCallback;
        private readonly GamePlayInstance _gamePlayInstance;
        private SubScreen _subScreen;
        private IScreen _postSessionStatsScreen;

        private void OnGamePlaySessionFinished()
        {
            _subScreen = SubScreen.PostSessionStats;
            _postSessionStatsScreen = new PostSessionStatsScreen(OnPlayAgain, OnMainMenu);
            _postSessionStatsScreen.OnNavigateTo();
        }

        private void OnMainMenu()
        {
            throw new NotImplementedException();
        }

        private void OnPlayAgain()
        {
            throw new NotImplementedException();
        }

        private enum SubScreen
        {
            GamePlay,
            PostSessionStats
        }
    }
}