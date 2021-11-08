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
        
        private ScreenId _currentScreenId;
        private readonly Dictionary<ScreenId, ScreenBase> _idToScreenDictionary;

        private CollisionManager _collisionManager;

        public GameMaster()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            _idToScreenDictionary = new Dictionary<ScreenId, ScreenBase>();
            foreach (var enumValue in Enum.GetValues(typeof(ScreenId)).Cast<ScreenId>())
            {
                _idToScreenDictionary.Add(enumValue, null);
            }
        }

        public void RegisterCollidableActor(ActorBase iActor)
        {
            _collisionManager.Register(iActor);
        }

        public void UnregisterCollidableActor(ActorBase iActor)
        {
            _collisionManager.Unregister(iActor);
        }

        public void DrawTexture(Texture2D iTexture, Vector2 iPosition)
        {
            _spriteBatch.Draw(iTexture, iPosition, Color.White);
        }

        public void DrawString(SpriteFont iSpriteFont, string iString, Vector2 iPosition, Color iFontColor, float iFontScaling = 24.0f)
        {
            _spriteBatch.DrawString(iSpriteFont, iString, iPosition, iFontColor, 0.0f, Vector2.Zero, iFontScaling, SpriteEffects.None, 0.0f);
        }

        public Rectangle GamePlayArea { get; private set; }

        protected override void Initialize()
        {
            _collisionManager = new CollisionManager();

            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var windowHeight = Window.ClientBounds.Height;
            var windowWidth = Window.ClientBounds.Width;

            var chosenWidth = (int)(windowHeight * 9.0f / 16.0f);
            var chosenHeight = windowHeight;

            var topLeftGamePlayAreaX = (int)((windowWidth / 2.0f) - (chosenWidth / 2.0f));
            var topLeftGamePlayAreaY = 0;
            GamePlayArea = new Rectangle(topLeftGamePlayAreaX, topLeftGamePlayAreaY, chosenWidth, chosenHeight);

            _currentScreenId = ScreenId.MainMenu;
            _idToScreenDictionary[_currentScreenId] = new MainMenuScreen(OnPlayGame, OnExitGame, this);
            _idToScreenDictionary[_currentScreenId].OnNavigateTo();
        }

        protected override void Update(GameTime gameTime)
        {
            _idToScreenDictionary[_currentScreenId].Update(gameTime);
            _collisionManager.Update(gameTime);

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
            _currentScreenId = ScreenId.GamePlay;
            _idToScreenDictionary[_currentScreenId] = new GamePlayScreen(OnExitGame, this);
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
