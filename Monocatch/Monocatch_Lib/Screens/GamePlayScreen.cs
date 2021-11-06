using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocatch_Lib.Actors;
using Monocatch_Lib.Actors.Components;

namespace Monocatch_Lib.Screens
{
    public class GamePlayScreen : ScreenBase
    {
        public GamePlayScreen(Action iOnExitCallback, GameMaster iGameMaster) : base(iGameMaster)
        {
            _onExitCallback = iOnExitCallback;
        }

        public override void OnNavigateTo()
        {
            Game.IsMouseVisible = false;

            var windowHeight = Game.Window.ClientBounds.Height;
            var windowWidth = Game.Window.ClientBounds.Width;

            var chosenWidth = (int)(windowHeight * 9.0f / 16.0f);

            var topLeftScreenAreaX = (int)((windowWidth / 2.0f) - (chosenWidth / 2.0f));

            const int wallWidth = 8;
            var playAreaLeft = topLeftScreenAreaX + wallWidth;
            var playAreaRight = topLeftScreenAreaX + chosenWidth - wallWidth;
            _projectileManager = new ProjectileManager(16, windowHeight + 50, playAreaLeft, topLeftScreenAreaX + chosenWidth - 8, Game);
            _leftWall = new WallActor(new Point(topLeftScreenAreaX, 0), new Point(playAreaLeft, windowHeight), Color.LightSlateGray, Game);
            _rightWall = new WallActor(new Point(playAreaRight, 0), new Point(playAreaRight + wallWidth, windowHeight), Color.LightSlateGray, Game);

            LoadPlayer(chosenWidth, windowHeight, windowWidth, Game);
        }

        public override void Update(GameTime iGameTime)
        {
            HandleInput();

            _player.Update(iGameTime);
            _leftWall.Update(iGameTime);
            _rightWall.Update(iGameTime);
            _projectileManager.Update(iGameTime);
        }

        public override void Draw()
        {
            _projectileManager.Draw();
            _leftWall.Draw();
            _rightWall.Draw();
            _player.Draw();
        }


        private ActorBase _player;
        private ActorBase _leftWall;
        private ActorBase _rightWall;
        private ProjectileManager _projectileManager;
        private readonly Action _onExitCallback;


        private void LoadPlayer(int chosenWidth, int windowHeight, int windowWidth, GameMaster iGameMaster)
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
                iGameMaster);

            var playerMovementComponent = new PlayerMovementComponent();
            _player.RegisterComponent(playerMovementComponent);
        }



        // Using this in place of a player controller because of this game's simplicity
        private void HandleInput()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _onExitCallback();

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
    }
}