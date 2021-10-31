using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Monocatch
{
    public class GameMaster : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private ProjectileActorBase _projectile;
        private WallActor _leftWall;
        private WallActor _rightWall;


        public GameMaster()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
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

            var topLeftPlayAreaX = (int)((windowWidth / 2.0f) - (chosenWidth / 2.0f));

            _projectile = new BasicProjectileActor(16, Color.White, new Vector2(topLeftPlayAreaX, 16), new Vector2(20,0), this);
            _leftWall = new WallActor(new Point(topLeftPlayAreaX, 0), new Point(topLeftPlayAreaX + 8, windowWidth), Color.LightSlateGray, this);
            _rightWall = new WallActor(new Point(topLeftPlayAreaX + chosenWidth - 8, 0), new Point(topLeftPlayAreaX + chosenWidth, windowWidth), Color.LightSlateGray, this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _projectile.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            void DrawAction(Texture2D tx, Vector2 vec) => _spriteBatch.Draw(tx, vec, Color.White);

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _projectile.Draw(DrawAction);
            _leftWall.Draw(DrawAction);
            _rightWall.Draw(DrawAction);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
