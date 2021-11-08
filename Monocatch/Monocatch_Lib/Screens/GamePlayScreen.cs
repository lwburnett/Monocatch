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

            var gamePlayArea = Game.GamePlayArea;

            const int wallWidth = 8;
            var playAreaLeft = gamePlayArea.X + wallWidth;
            var playAreaRight = gamePlayArea.X + gamePlayArea.Width - wallWidth;
            _projectileManager = new ProjectileManager(16, gamePlayArea.Height + 50, playAreaLeft, gamePlayArea.X + gamePlayArea.Width - 8, Game);
            _leftWall = new WallActor(new Point(gamePlayArea.X, 0), new Point(playAreaLeft, gamePlayArea.Height), Color.LightSlateGray, Game);
            _rightWall = new WallActor(new Point(playAreaRight, 0), new Point(playAreaRight + wallWidth, gamePlayArea.Height), Color.LightSlateGray, Game);

            LoadPlayer();
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


        private void LoadPlayer()
        {
            var playArea = Game.GamePlayArea;

            var playerWidth = (int)(playArea.Width / 10.0f);
            var playerHeight = (int)(playArea.Height / 64.0f);
            var playerTopLeftX = (int)(playArea.X + playArea.Width / 2.0f - playerWidth / 2.0f);
            var playerTopLeftY = (int)(playArea.Y + playArea.Height * .75f - playerHeight / 2.0f);
            _player = new PlayerActor(
                new Vector2(playerTopLeftX, playerTopLeftY),
                playerWidth,
                playerHeight,
                Color.LightCoral,
                Game);

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