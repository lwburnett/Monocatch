using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch_Lib.Ui
{
    public class UiTextButton
    {
        public UiTextButton(Point iTopLeft, int iWidth, int iHeight, string iText, Action iOnClickedCallback, GameMaster iGame)
        {
            _topLeft = iTopLeft;
            _width = iWidth;
            _height = iHeight;
            _text = iText;
            _onClickedCallback = iOnClickedCallback;
            _game = iGame;

            var dataSize = _width * _height;
            var colorData1 = new Color[dataSize];
            var colorData2 = new Color[dataSize];
            var colorData3 = new Color[dataSize];
            for (var ii = 0; ii < dataSize; ii++)
            {
                colorData1[ii] = Color.LightGray;
                colorData2[ii] = Color.DarkGray;
                colorData3[ii] = Color.Gray;
            }

            _defaultTexture = new Texture2D(iGame.GraphicsDevice, _width, _height);
            _defaultTexture.SetData(colorData1);

            _overLapTexture = new Texture2D(iGame.GraphicsDevice, _width, _height);
            _overLapTexture.SetData(colorData2);

            _pressedTexture = new Texture2D(iGame.GraphicsDevice, _width, _height);
            _pressedTexture.SetData(colorData3);

            _textFont = iGame.Content.Load<SpriteFont>("PrototypeFont");

            Reset();
        }

        public bool IsOverlappingWithMouse(Point iPosition)
        {
            return iPosition.X > _topLeft.X &&
                   iPosition.X < _topLeft.X + _width &&
                   iPosition.Y > _topLeft.Y &&
                   iPosition.Y < _topLeft.Y + _height;
        }

        public void OnPressed()
        {
            _isPressed = true;
        }

        public void OnReleased()
        {
            if (_isPressed && _isOverlapped)
                _onClickedCallback();

            _isPressed = false;
        }

        public void Reset()
        {
            _isPressed = false;
            _isOverlapped = false;
        }

        public void OnOverlap()
        {
            _isOverlapped = true;
        }

        public void OnNotOverlap()
        {
            _isOverlapped = false;
        }

        public void Draw()
        {
            if (!_isOverlapped && !_isPressed)
                _game.DrawTexture(_defaultTexture, _topLeft.ToVector2());
            else if (_isOverlapped && !_isPressed)
                _game.DrawTexture(_overLapTexture, _topLeft.ToVector2());
            else
                _game.DrawTexture(_pressedTexture, _topLeft.ToVector2());

            _game.DrawString(_textFont, _text, new Vector2(_topLeft.X + _width / 2.0f, _topLeft.Y + _height / 2.0f), Color.Black);
        }

        private Point _topLeft;
        private readonly int _width;
        private readonly int _height;
        private readonly string _text;
        private readonly Texture2D _defaultTexture;
        private readonly Texture2D _overLapTexture;
        private readonly Texture2D _pressedTexture;
        private readonly SpriteFont _textFont;
        private readonly GameMaster _game;

        private bool _isPressed;
        private bool _isOverlapped;

        private readonly Action _onClickedCallback;
    }
}