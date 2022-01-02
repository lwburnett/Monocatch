using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocatch_Lib.Ui
{
    public class UiFloatingText : IUiElement
    {
        public UiFloatingText(Point iTopLeft, string iText) :
            this(iTopLeft, iText, Color.Black)
        {
        }

        public UiFloatingText(Point iTopLeft, string iText, Color iTextColor)
        {
            _topLeft = iTopLeft;
            _text = iText;
            _textColor = iTextColor;

            _textFont = GraphicsHelper.LoadContent<SpriteFont>("PrototypeFont");
        }

        public void Update(GameTime iGameTime)
        {
        }

        public void Draw()
        {
            GraphicsHelper.DrawString(_textFont, _text, _topLeft.ToVector2(), _textColor);
        }

        private Point _topLeft;
        private readonly string _text;
        private readonly SpriteFont _textFont;
        private readonly Color _textColor;
    }
}