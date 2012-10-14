using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLevels
{
    class Object
    {
        public Rectangle Rect { get; set; }

        Texture2D texture;
        Game1 game;
        Camera camera;

        /*
         * @param {Rectangle} Rect - прямоугольник стены
         * @param {Texture2D} Texture - текстура стены
        */
        public Object(Rectangle Rect, Texture2D texture, Game1 game, Camera camera)
        {
            this.Rect = Rect;
            this.texture = texture;
            this.game = game;
            this.camera = camera;
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            Rectangle screenRect = camera.GetScreenRect(Rect);  // рисуем только то, что помещается на экране
            spriteBatch.Draw(texture, screenRect, Color.White);
        }
    }
}
