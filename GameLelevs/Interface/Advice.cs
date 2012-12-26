using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLevels
{
    class Advice
    {
        public Rectangle Rect { get; set; }


        Texture2D imgAdvice;

        public Advice(Texture2D imgAdvice)
        {
            this.Rect = new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight);
            this.imgAdvice = imgAdvice;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(imgAdvice, Rect, Color.White);
        }




    }
}
