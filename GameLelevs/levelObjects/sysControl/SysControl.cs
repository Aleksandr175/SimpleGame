using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLevels.levelObjects;
using Enumerations;

namespace GameLevels.levelObjects
{
    class SysControl : BaseObject
    {
        private bool isActive = true;

        public int posX;
        public int posY;

        public bool IsActive
        {
            get { return isActive; }
        }


        /// <summary>
        /// Конструктор системы управления камерами
        /// </summary>
        /// <param name="rect">Прямоугольник</param>
        /// <param name="texture">Текстура лазера</param>
        /// <param name="textureInactive">Текстура выключенного лазера</param>
        /// <param name="typeLaser">Тип лазера</param>
        /// <param name="game">ссылка на игру</param>
        /// <param name="camera">ссылка на камеру</param>
        public SysControl(Rectangle Rect, Texture2D texture, Game1 game, Camera camera)
            : base(Rect, texture, game, camera)
        {
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
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


        /// <summary>
        /// Отключаем определенную камеру слежения
        /// </summary>
        public void TurnOffCamera()
        {

        }

    }
}
