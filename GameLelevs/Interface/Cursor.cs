using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace GameLevels
{
    class Cursor
    {
        private Vector2 pos;
        private Texture2D texMouse;
        private Texture2D texPoint;
        private MouseState mouseState;
        public Cursor(Texture2D tex_mouse, Texture2D tex_point)
        {
            this.pos = Vector2.Zero;
            this.texMouse = tex_mouse;
            this.texPoint = tex_point;
        }
        public void Update()
        {
            mouseState = Mouse.GetState();
            this.pos.X = mouseState.X;
            this.pos.Y = mouseState.Y;
        }
        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(this.texMouse, this.pos, Color.White);
        }

        public void DrawPointer(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texPoint, this.pos, Color.White);
        }
    }
}
