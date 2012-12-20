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
        public Texture2D texLight, texDark;
        MouseState mouseState;
        public event EventHandler Click;

        public Button(Texture2D texLight, Texture2D texDark)
        {
            this.texLight = texLight;
            this.texDark = texDark;
            this.tex = texDark;
        }
        public Button(Vector2 pos, Texture2D texLight, Texture2D texDark)
        {
            this.position = pos;
            this.texLight = texLight;
            this.texDark = texDark;
            this.tex = texDark;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, position, Color.White);
        }

        public void OnClick()
        {
            if (Click != null)
                Click(this, null);
        }

        public bool Hover()
        {
            mouseState = Mouse.GetState();
            if ((mouseState.X > this.position.X) && (mouseState.X < this.position.X + this.tex.Width)
                    && (mouseState.Y > this.position.Y) && (mouseState.Y < this.position.Y + this.tex.Height))
            {
                tex = texLight;
                return true;
            }
            else
            {
                tex = texDark;
                return false;
            }
        }

        public bool ButtonClick()
        {

            if (Hover() && (mouseState.LeftButton == ButtonState.Pressed))
                return true;
            else
                return false;

        }
    }
}
