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
    class Cameras : BaseObject
    {
        private bool isActive = true; // включена ли камера?

        public int posX;
        public int posY;

        public bool IsActive
        {
            get { return isActive; }
        }

        Texture2D textureInactive; // текстура выключенного лазера

        /// <summary>
        /// Конструктор лазера
        /// </summary>
        /// <param name="rect">Прямоугольник</param>
        /// <param name="texture">Текстура лазера</param>
        /// <param name="textureInactive">Текстура выключенного лазера</param>
        /// <param name="typeLaser">Тип лазера</param>
        /// <param name="game">ссылка на игру</param>
        /// <param name="camera">ссылка на камеру</param>
        public Cameras(Rectangle Rect, Texture2D texture, Texture2D textureInactive, Game1 game, Camera camera) 
            : base(Rect, texture, game, camera) {

            this.textureInactive = textureInactive;

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
            Texture2D currentTexture = textureInactive;

            if (this.isActive)
            {
                currentTexture = texture; 
            }
            else
            {
                currentTexture = textureInactive;
            }
            
            Rectangle screenRect = camera.GetScreenRect(Rect);  // рисуем только то, что помещается на экране
            spriteBatch.Draw(currentTexture, screenRect, Color.White);

        }


        /// <summary>
        /// Переключение состояния камеры
        /// </summary>
        public void changeActiveCamera()
        {
            this.isActive = !this.isActive;
        }



    }
}
