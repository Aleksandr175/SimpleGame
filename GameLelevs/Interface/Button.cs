using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace GameLevels
{
    class Button
    {
        public Vector2 position;
        public Texture2D tex;
        public event EventHandler Click;
        Color color;
        SpriteFont font;
        Vector2 scale;
        string name;
        public Button() { }
        public Button(Texture2D tex, SpriteFont font, string name)
        {
            this.tex = tex;
            this.color = Color.White;
            this.font = font;
            this.name = name;
            this.scale = new Vector2(1, 1);
        }
        public Button(Vector2 pos, Texture2D tex, SpriteFont font, string name)
        {
            this.position = pos;
            this.tex = tex;
            this.color = Color.White;
            this.font = font;
            this.name = name;
            this.scale = new Vector2(1, 1);
        }
        public Button(Vector2 pos, Texture2D tex, SpriteFont font, Vector2 scale, string name)
        {
            this.position = pos;
            this.tex = tex;
            this.color = Color.White;
            this.font = font;
            this.name = name;
            this.scale = scale;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, position, color);
            Vector2 size = font.MeasureString(name)*scale;
            spriteBatch.DrawString(font, name, position, Color.White, 0, new Vector2((-tex.Width / 2 + size.X / 2)/scale.X, (-tex.Height / 2 + size.Y / 2)/scale.Y), scale, SpriteEffects.None, 0);
        }

        public void OnClick()
        {
            if (Click != null)
                Click(this, null);
        }
        /// <summary>
        /// Проверка на нахождение мыши на кнопке
        /// </summary>
        /// <param name="mouseState">состояние мыши</param>
        public bool Hover(MouseState mouseState)
        {
            MouseState mouse = Mouse.GetState();
            if ((mouseState.X > this.position.X) && (mouseState.X < this.position.X + this.tex.Width)
                    && (mouseState.Y > this.position.Y) && (mouseState.Y < this.position.Y + this.tex.Height))
            {
                color = Color.White;
                return true;
            }
            else
            {
                color = new Color(255, 255, 255, 190);
                return false;
            }
        }
        /// <summary>
        /// Проверка на нажатие левой кнопкой мыши на кнопку
        /// </summary>
        /// <param name="mouseState">состояние мыши</param>
        /// <param name="oldState">предыдущее состояние мыши</param>
        public bool ButtonClick(MouseState mouseState, MouseState oldState)
        {
            if (Hover(mouseState) && (mouseState.LeftButton == ButtonState.Pressed) && oldState.LeftButton == ButtonState.Released)

                return true;
            else
                return false;

        }
    }
}
