using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Monocatch
{
    public class GameMaster : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private ActorBase _leftWall;
        private ActorBase _rightWall;
        private ProjectileManager _projectileManager;
        private CollisionManager _collisionManager;
        private ActorBase _player;

        public GameMaster()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void RegisterCollidableActor(ActorBase iActor)
        {
            _collisionManager.Register(iActor);
        }

        public void UnregisterCollidableActor(ActorBase iActor)
        {
            _collisionManager.Unregister(iActor);
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
            _collisionManager = new CollisionManager();

            var windowHeight = Window.ClientBounds.Height;
            var windowWidth = Window.ClientBounds.Width;

            var chosenWidth = (int)(windowHeight * 9.0f / 16.0f);

            var topLeftScreenAreaX = (int)((windowWidth / 2.0f) - (chosenWidth / 2.0f));

            const int wallWidth = 8;
            var playAreaLeft = topLeftScreenAreaX + wallWidth;
            var playAreaRight = topLeftScreenAreaX + chosenWidth - wallWidth;
            _projectileManager = new ProjectileManager(16, windowHeight + 50, playAreaLeft, topLeftScreenAreaX + chosenWidth - 8, this);
            _leftWall = new WallActor(new Point(topLeftScreenAreaX, 0), new Point(playAreaLeft, windowHeight), Color.LightSlateGray, this);
            _rightWall = new WallActor(new Point(playAreaRight, 0), new Point(playAreaRight + wallWidth, windowHeight), Color.LightSlateGray, this);

            LoadPlayer(chosenWidth, windowHeight, windowWidth);
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            _collisionManager.Update(gameTime);
            _projectileManager.Update(gameTime);
            _player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            void DrawAction(Texture2D tx, Vector2 vec) => _spriteBatch.Draw(tx, vec, Color.White);

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _leftWall.Draw(DrawAction);
            _rightWall.Draw(DrawAction);
            _player.Draw(DrawAction);
            _projectileManager.Draw(DrawAction);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // Using this in place of a player controller because of this game's simplicity
        private void HandleInput()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var playerMovementComponent = _player.GetComponentByType<PlayerMovementComponent>();

            Debug.Assert(playerMovementComponent != null);
            
            var isLeftDown = Keyboard.GetState().IsKeyDown(Keys.Left);
            var isRightDown = Keyboard.GetState().IsKeyDown(Keys.Right);

            if (!isLeftDown && !isRightDown)
            {
                playerMovementComponent.IntendNoneAction();
                return;
            }

            if (isLeftDown && isRightDown)
            {
                playerMovementComponent.IntendBothAction();
                return;
            }

            if (isLeftDown)
            {
                playerMovementComponent.IntendLeftAction();
                return;
            }

            playerMovementComponent.IntendRightAction();
        }

        private void LoadPlayer(int chosenWidth, int windowHeight, int windowWidth)
        {
            var playerWidth = (int)(chosenWidth / 10.0f);
            var playerHeight = (int)(windowHeight / 64.0f);
            var playerTopLeftX = (int)(windowWidth / 2.0f - playerWidth / 2.0f);
            var playerTopLeftY = (int)(windowHeight * .75f - playerHeight / 2.0f);
            _player = new PlayerActor(
                new Vector2(playerTopLeftX, playerTopLeftY),
                playerWidth,
                playerHeight,
                Color.LightCoral,
                this);

            var playerMovementComponent = new PlayerMovementComponent();
            _player.RegisterComponent(playerMovementComponent);
        }
    }
}
