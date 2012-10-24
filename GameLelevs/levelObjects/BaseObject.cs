using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLevels.levelObjects
{
    class BaseObject : IBaseObject
    {
        public Rectangle Rect { get; set; }

        private Texture2D texture;
        private Game1 game;
        private Camera camera;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="Rect">прямоугольник объекта</param>
        /// <param name="texture">текстура объекта</param>
        /// <param name="game">ссылка на игру</param>
        /// <param name="camera">ссылка на камеру</param>
        public BaseObject(Rectangle Rect, Texture2D texture, Game1 game, Camera camera)
        {
            this.Rect = Rect;
            this.texture = texture;
            this.game = game;
            this.camera = camera;
        }

        /// <summary>
        /// Открытый метод, для отрисовки объекта
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch) 
        {
            Rectangle screenRect = camera.GetScreenRect(Rect);  // рисуем только то, что помещается на экране
            spriteBatch.Draw(texture, screenRect, Color.White);
        }
    }
}
