using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLevels
{
    //класс, отвечающий за отрисовку стен игрового уровня
    class Block
    {
        public Rectangle Rect { get; set; }

        Texture2D texture;
        Game1 game;

        /*
         * @param {Rectangle} Rect - прямоугольник стены
         * @param {Texture2D} Texture - текстура стены
        */
        public Block(Rectangle Rect, Texture2D texture, Game1 game)
        {
            this.Rect = Rect;
            this.texture = texture;
            this.game = game;
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            Rectangle screenRect = game.GetScreenRect(Rect);  // рисуем только то, что помещается на экране
            spriteBatch.Draw(texture, screenRect, Color.White);
        }
    }
}
