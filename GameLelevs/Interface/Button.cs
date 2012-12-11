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
        MouseState mouseState;
        public event EventHandler Click;

        public Button(Texture2D tex)
        {
            this.tex = tex;
        }
        public Button(Vector2 pos, Texture2D tex)
        {
            this.position = pos;
            this.tex = tex;
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

        public bool ButtonClick()
        {
            mouseState = Mouse.GetState();
            if ((mouseState.X > this.position.X) && (mouseState.X < this.position.X + this.tex.Width)
                    && (mouseState.Y > this.position.Y) && (mouseState.Y < this.position.Y + this.tex.Height) && (mouseState.LeftButton == ButtonState.Pressed))
                return true;
            else
                return false;

        }
    }
}
