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
        public GamePlayInstance(TimeSpan iGameStart)
        {
            _gameStart = iGameStart;
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
            _projectileManager = new ProjectileManager(spawningHeight, bottomBound, playAreaLeft, gamePlayArea.X + gamePlayArea.Width - wallWidth, _collisionManager);

            var leftWallTopLeft = new Point(gamePlayArea.X, 0);
            var leftWallBottomRight = new Point(playAreaLeft, gamePlayArea.Height);
            _leftWall = new WallActor(leftWallTopLeft, leftWallBottomRight, Color.LightSlateGray);
            _leftWall.RegisterComponent(new CollisionComponent(new BoxCollider(leftWallTopLeft.ToVector2(), leftWallBottomRight.ToVector2())));

            var rightWallTopLeft = new Point(playAreaRight, 0);
            var rightWallBottomRight = new Point(playAreaRight + wallWidth, gamePlayArea.Height);
            _rightWall = new WallActor(rightWallTopLeft, rightWallBottomRight, Color.LightSlateGray);
            _rightWall.RegisterComponent(new CollisionComponent(new BoxCollider(rightWallTopLeft.ToVector2(), rightWallBottomRight.ToVector2())));

            LoadPlayer();

            _collisionManager.Register(_leftWall);
            _collisionManager.Register(_rightWall);
            _collisionManager.Register(_player);
        }

        public void Update(GameTime iGameTime)
        {
            HandleInput(iGameTime);

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
            _player.Draw();
        }

        private ActorBase _player;
        private ActorBase _leftWall;
        private ActorBase _rightWall;
        private ProjectileManager _projectileManager;
        private CollisionManager _collisionManager;

        private void LoadPlayer()
        {
            var playArea = GraphicsHelper.GamePlayArea;

            var playerWidth = (int)(playArea.Width * SettingsManager.PlayerSettings.Movement.WidthAsFractionOfPlayAreaWidth);
            var playerHeight = (int)(playArea.Height * SettingsManager.PlayerSettings.Movement.HeightAsFractionOfPlayAreaHeight);
            var playerTopLeftX = (int)(playArea.X + playArea.Width / 2.0f - playerWidth / 2.0f);
            var playerTopLeftY = (int)(playArea.Y + playArea.Height * SettingsManager.PlayerSettings.Movement.SpawnHeightAsFractionOfPlayAreaHeight - playerHeight / 2.0f);
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
        private void HandleInput(GameTime iGameTime)
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