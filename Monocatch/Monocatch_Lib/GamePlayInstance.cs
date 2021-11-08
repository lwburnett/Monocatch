using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocatch_Lib.Actors;
using Monocatch_Lib.Actors.Components;

namespace Monocatch_Lib
{
    public class GamePlayInstance
    {
        public GamePlayInstance(TimeSpan iGameStart, GameMaster iGameMaster)
        {
            _game = iGameMaster;
            _gameStart = iGameStart;
        }

        private readonly TimeSpan _gameStart;

        public void LoadLevel()
        {
            var gamePlayArea = _game.GamePlayArea;

            const int wallWidth = 8;
            var playAreaLeft = gamePlayArea.X + wallWidth;
            var playAreaRight = gamePlayArea.X + gamePlayArea.Width - wallWidth;
            _projectileManager = new ProjectileManager(16, gamePlayArea.Height + 50, playAreaLeft, gamePlayArea.X + gamePlayArea.Width - 8, _game);
            _leftWall = new WallActor(new Point(gamePlayArea.X, 0), new Point(playAreaLeft, gamePlayArea.Height), Color.LightSlateGray, _game);
            _rightWall = new WallActor(new Point(playAreaRight, 0), new Point(playAreaRight + wallWidth, gamePlayArea.Height), Color.LightSlateGray, _game);

            LoadPlayer();
        }

        public void Update(GameTime iGameTime)
        {
            HandleInput();

            var adjustedGameTime = new GameTime(
                iGameTime.TotalGameTime - _gameStart, 
                iGameTime.ElapsedGameTime,
                iGameTime.IsRunningSlowly);

            _player.Update(adjustedGameTime);
            _leftWall.Update(adjustedGameTime);
            _rightWall.Update(adjustedGameTime);
            _projectileManager.Update(adjustedGameTime);
        }

        public void Draw()
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
        private readonly GameMaster _game;


        private void LoadPlayer()
        {
            var playArea = _game.GamePlayArea;

            var playerWidth = (int)(playArea.Width / 10.0f);
            var playerHeight = (int)(playArea.Height / 64.0f);
            var playerTopLeftX = (int)(playArea.X + playArea.Width / 2.0f - playerWidth / 2.0f);
            var playerTopLeftY = (int)(playArea.Y + playArea.Height * .75f - playerHeight / 2.0f);
            _player = new PlayerActor(
                new Vector2(playerTopLeftX, playerTopLeftY),
                playerWidth,
                playerHeight,
                Color.LightCoral,
                _game);

            var playerMovementComponent = new PlayerMovementComponent(_game);
            _player.RegisterComponent(playerMovementComponent);
        }

        // Using this in place of a player controller because of this game's simplicity
        private void HandleInput()
        {
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