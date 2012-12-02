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
        Random random;
        public int posX;
        public int posY;
        private bool isVisibleExample = false; // показываем на экране пример?

        public bool IsVisibleExample
        {
            set { isVisibleExample = value; }
        }

        private string generatedMathExample = ""; // сгенерированный пример для взлома компа

        public string GetGeneratedMathExample()
        {
            if (isVisibleExample)
            {
                return generatedMathExample;
            }
            return "";
        }


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
        /// Генерируем математический пример для взлома компа
        /// </summary>
        public void generateMathEmample()
        {
            random = new Random();
            int tempVar1 = random.Next(1, 6);
            int tempVar2 = random.Next(1, 5);
            int tempAction;
            string example = Convert.ToString(tempVar1 + " ");
            
            //действие
            tempAction = random.Next(1, 3);
            if (tempAction == 1)
            {
                example += Convert.ToString("+ ");
            }
            else
            {
                while(tempVar1 - tempVar2 < 0)  
                {
                    tempVar2 = random.Next(1, 5);
                }
                example += Convert.ToString("- ");
            }

            example += Convert.ToString(tempVar2 + " = ?");

            this.generatedMathExample =  example;
        }


        /// <summary>
        /// Изменяем параметр видимости для математического примера
        /// </summary>
        public void changeVisibleExample()
        {
            isVisibleExample = !isVisibleExample;
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
