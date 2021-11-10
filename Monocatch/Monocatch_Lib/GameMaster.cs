using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocatch_Lib.Actors;
using Monocatch_Lib.Collision;
using Monocatch_Lib.Screens;

namespace Monocatch_Lib
{
    public class GameMaster : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameTime _gameTime;


        private ScreenId _currentScreenId;
        private readonly Dictionary<ScreenId, IScreen> _idToScreenDictionary;

        public GameMaster()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            _idToScreenDictionary = new Dictionary<ScreenId, IScreen>();
            foreach (var enumValue in Enum.GetValues(typeof(ScreenId)).Cast<ScreenId>())
            {
                _idToScreenDictionary.Add(enumValue, null);
            }
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var windowHeight = Window.ClientBounds.Height;
            var windowWidth = Window.ClientBounds.Width;

            var chosenWidth = (int)(windowHeight / SettingsManager.GameMasterSettings.TargetScreenAspectRatio);
            var chosenHeight = windowHeight;

            var topLeftGamePlayAreaX = (int)((windowWidth / 2.0f) - (chosenWidth / 2.0f));
            var topLeftGamePlayAreaY = 0;
            var gamePlayArea = new Rectangle(topLeftGamePlayAreaX, topLeftGamePlayAreaY, chosenWidth, chosenHeight);

            GraphicsHelper.RegisterContentManager(Content);
            GraphicsHelper.RegisterGraphicsDevice(GraphicsDevice);
            GraphicsHelper.RegisterSpriteBatch(_spriteBatch);
            GraphicsHelper.RegisterGamePlayArea(gamePlayArea);

            _currentScreenId = ScreenId.MainMenu;
            _idToScreenDictionary[_currentScreenId] = new MainMenuScreen(OnPlayGame, OnExitGame);
            _idToScreenDictionary[_currentScreenId].OnNavigateTo();
        }

        protected override void Update(GameTime gameTime)
        {
            _gameTime = gameTime;

            _idToScreenDictionary[_currentScreenId].Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _idToScreenDictionary[_currentScreenId].Draw();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void OnPlayGame()
        {
            var gameStart = _gameTime.TotalGameTime;
            var gamePlayInstance = new GamePlayInstance(gameStart);

            _currentScreenId = ScreenId.GamePlay;
            _idToScreenDictionary[_currentScreenId] = new GamePlayScreen(gamePlayInstance, OnExitGame);
            _idToScreenDictionary[_currentScreenId].OnNavigateTo();
        }

        private void OnExitGame()
        {
            Exit();
        }

        private enum ScreenId
        {
            MainMenu,
            GamePlay
        }
    }
}
