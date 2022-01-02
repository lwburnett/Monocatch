using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocatch_Lib.Actors;
using Monocatch_Lib.Actors.Components;
using Monocatch_Lib.Collision;

namespace Monocatch_Lib
{
    public class GamePlayInstance
    {
        public GamePlayInstance(TimeSpan iGameStart, Action iOnGamePlaySessionFinishedCallback)
        {
            _gameStart = iGameStart;
            _playSessionHasFinished = false;
            _onGamePlaySessionFinishedCallback = iOnGamePlaySessionFinishedCallback;
        }

        private readonly TimeSpan _gameStart;

        public void LoadLevel()
        {
            _collisionManager = new CollisionManager();

            var gamePlayArea = GraphicsHelper.GamePlayArea;

            var wallWidth = (int)(SettingsManager.WallSettings.WallWidthAsFractionOfPlayAreaWidth * gamePlayArea.Width);
            var playAreaLeft = gamePlayArea.X + wallWidth;
            var playAreaRight = gamePlayArea.X + gamePlayArea.Width - wallWidth;
            var bottomBound = (int)(gamePlayArea.Height * SettingsManager.ProjectileManagerSettings.DespawnHeightAsFractionOfPlayAreaHeight);
            var spawningHeight = (int)(gamePlayArea.Height * SettingsManager.ProjectileManagerSettings.SpawnHeightAsFractionOfPlayAreaHeight);
            _projectileManager = new ProjectileManager(spawningHeight, bottomBound, playAreaLeft, gamePlayArea.X + gamePlayArea.Width - wallWidth, _collisionManager, OnProjectilesDoneSpawning);

            var leftWallTopLeft = new Point(gamePlayArea.X, 0);
            var leftWallBottomRight = new Point(playAreaLeft, gamePlayArea.Height);
            _leftWall = new WallActor(leftWallTopLeft, leftWallBottomRight, Color.LightSlateGray);
            _leftWall.RegisterComponent(new CollisionComponent(new BoxCollider(leftWallTopLeft.ToVector2(), leftWallBottomRight.ToVector2())));

            var rightWallTopLeft = new Point(playAreaRight, 0);
            var rightWallBottomRight = new Point(playAreaRight + wallWidth, gamePlayArea.Height);
            _rightWall = new WallActor(rightWallTopLeft, rightWallBottomRight, Color.LightSlateGray);
            _rightWall.RegisterComponent(new CollisionComponent(new BoxCollider(rightWallTopLeft.ToVector2(), rightWallBottomRight.ToVector2())));

            var bottomWallTopLeft = new Point(gamePlayArea.X + wallWidth, (int)(SettingsManager.WorldSettings.FloorLocationYAsFractionOfPlayAreaHeight * gamePlayArea.Height));
            var bottomWallBottomRight = new Point(rightWallTopLeft.X, bottomWallTopLeft.Y + wallWidth);
            _bottomWall = new WallActor(bottomWallTopLeft, bottomWallBottomRight, Color.LightSlateGray);

            LoadPlayer(bottomWallTopLeft.Y);

            _collisionManager.Register(_leftWall);
            _collisionManager.Register(_rightWall);
            _collisionManager.Register(_player);
        }

        public void Update(GameTime iGameTime)
        {
            if (_playSessionHasFinished) 
                return;

            HandleInput();

            var adjustedGameTime = new GameTime(
                iGameTime.TotalGameTime - _gameStart,
                iGameTime.ElapsedGameTime,
                iGameTime.IsRunningSlowly);

            _player.Update(adjustedGameTime);
            _leftWall.Update(adjustedGameTime);
            _rightWall.Update(adjustedGameTime);
            _projectileManager.Update(adjustedGameTime);
            _collisionManager.Update(adjustedGameTime);
        }

        public void Draw()
        {
            _projectileManager.Draw();
            _leftWall.Draw();
            _rightWall.Draw();
            _bottomWall.Draw();
            _player.Draw();
        }

        private ActorBase _player;
        private ActorBase _leftWall;
        private ActorBase _rightWall;
        private ActorBase _bottomWall;
        private ProjectileManager _projectileManager;
        private CollisionManager _collisionManager;
        private bool _playSessionHasFinished;
        private readonly Action _onGamePlaySessionFinishedCallback;

        private void LoadPlayer(int iBottomY)
        {
            var playArea = GraphicsHelper.GamePlayArea;

            var playerWidth = (int)(playArea.Width * SettingsManager.PlayerSettings.WidthAsFractionOfPlayAreaWidth);
            var playerHeight = (int)(playArea.Height * SettingsManager.PlayerSettings.HeightAsFractionOfPlayAreaHeight);
            var playerTopLeftX = (int)(playArea.X + playArea.Width / 2.0f - playerWidth / 2.0f);
            var playerTopLeftY = iBottomY - playerHeight;
            _player = new PlayerActor(
                new Vector2(playerTopLeftX, playerTopLeftY),
                playerWidth,
                playerHeight,
                Color.LightCoral);

            var playerMovementComponent = new PlayerMovementComponent();
            _player.RegisterComponent(playerMovementComponent);

            var playerTopLeft = new Vector2(playerTopLeftX, playerTopLeftY);
            var playerBottomRight = playerTopLeft + new Vector2(playerWidth, playerHeight);
            _player.RegisterComponent(new CollisionComponent(new BoxCollider(playerTopLeft, playerBottomRight)));
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

        private void OnProjectilesDoneSpawning()
        {
            _playSessionHasFinished = true;
            _onGamePlaySessionFinishedCallback();
        }
    }
}