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
        private Texture2D texture;
        private MouseState mouseState;
        public Cursor(Texture2D tex)
        {
            this.pos = Vector2.Zero;
            this.texture = tex;
        }
        public void Update()
        {
            mouseState = Mouse.GetState();
            this.pos.X = mouseState.X;
            this.pos.Y = mouseState.Y;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.pos, Color.White);
        }
    }
}
