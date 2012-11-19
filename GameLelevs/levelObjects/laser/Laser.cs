﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLevels.levelObjects;
using Enumerations;

namespace GameLevels
{
    class Laser : BaseObject, ILaser
    {
        private bool isActive = true; // включен ли лазер?
        
        int speed; // скорость движения лазера
        int currentStepPatrol = -1; // текущий шаг для патрулирования (-1 - лазер не будет двигаться)
        int targetX;
        int targetY;

        int oldPosGuardX; // старая позиция лазера. Для сравнения - "перешел ли лазер на новую клетку?"
        int oldPosGuardY;


        public bool IsActive
        {
            get { return isActive; }
        }


        Texture2D textureInactive; // текстура выключенного лазера
        List<List<int>> wayToPatrol = new List<List<int>>(); // траектория для патрулирования
        

        private static float intervalActivity = 2.0f; // интервал работы лазера. Каждые 2 сек. лазер включается и выключается.
        private float timer; // время до следующего отключения, вкл. лазера
        public LevelObject typeLaser; // тип лазера (гориз/вертик).
        
        /// <summary>
        /// Конструктор лазера
        /// </summary>
        /// <param name="rect">Прямоугольник</param>
        /// <param name="texture">Текстура лазера</param>
        /// <param name="textureInactive">Текстура выключенного лазера</param>
        /// <param name="typeLaser">Тип лазера</param>
        /// <param name="game">ссылка на игру</param>
        /// <param name="camera">ссылка на камеру</param>
        public Laser(Rectangle rect, Texture2D texture, Texture2D textureInactive, LevelObject typeLaser, Game1 game, Camera camera) 
            : base(rect, texture, game, camera) {
                timer = intervalActivity;
                intervalActivity += 0.3f;

            this.Rect = rect;
            this.textureInactive = textureInactive;
            this.typeLaser = typeLaser;

            this.speed = 1;
            


            //массив для патрулирования
            // временный. Потом будет считываться из файла
            wayToPatrol.Add(new List<int>());// добавляем новую строку под координату
            wayToPatrol[0].Add(11); // первым указывается координата X
            wayToPatrol[0].Add(3); // второе - координата Y
            wayToPatrol.Add(new List<int>());// добавляем новую строку под координату
            wayToPatrol[1].Add(8);
            wayToPatrol[1].Add(3);


            //проверяем на тип лазера. Будет ли он двигаться или нет.
            if (this.typeLaser == LevelObject.LaserHorizMoving || this.typeLaser == LevelObject.LaserVerticMoving)
            {
                this.currentStepPatrol = 0;
                this.Patrol(); // сразу задаем первую точку, куда двигаться лазеру
            }



        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // недвижущиеся лазеры
            if (!isLaserMoving())
            {
                if (timer > 0)
                {
                    timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (timer <= 0)
                {
                    timer = intervalActivity;
                    isActive = !isActive;
                }
            }
            else
            {
                // движущиеся лазеры

                int offset = 0;
                Rectangle newPosition;

                oldPosGuardX = this.Rect.X / LevelLoader.Size;
                oldPosGuardY = this.Rect.Y / LevelLoader.Size;



                int currentPositionX = this.oldPosGuardX * LevelLoader.Size + LevelLoader.Size / 2;
                int positionTargetX = targetX * LevelLoader.Size + LevelLoader.SizePeople / 2;

                // проверка достижения точки патрулирования по X
                if (Math.Abs(currentPositionX - (positionTargetX)) <= LevelLoader.Size / 4)
                {
                    currentStepPatrol++;
                    Patrol();
                }




                // передвижение лазера
                offset = this.speed * gameTime.ElapsedGameTime.Milliseconds / 15;

                // новое положение охранника
                newPosition = this.Rect;
                //смещение
                if (this.oldPosGuardX * LevelLoader.Size + LevelLoader.Size / 2 > targetX * LevelLoader.Size + LevelLoader.SizePeople / 2)
                {
                    newPosition.Offset(-offset, 0);
                }
                if (this.oldPosGuardX * LevelLoader.Size + LevelLoader.Size / 2 < targetX * LevelLoader.Size + LevelLoader.SizePeople / 2)
                {
                    newPosition.Offset(offset, 0);
                }

                //                newPosition.Offset(offset, 0);
                //сохраняем положение
                this.Rect = newPosition;

            }
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
        /// Функция патрулирования лазера по уровню.
        /// Задает следующую точку для движения лазера
        /// </summary>
        private void Patrol()
        {
            if (currentStepPatrol != -1)
            {
                if (currentStepPatrol > 1)
                {
                    currentStepPatrol = 0;
                }
                targetX = wayToPatrol[currentStepPatrol][0];
                targetY = wayToPatrol[currentStepPatrol][1];
            }
        }

        /// <summary>
        /// Лазер может двигаться?
        /// </summary>
        /// <returns>Может ли двигаться лазер? (true)</returns>
        private bool isLaserMoving() 
        {
            if (currentStepPatrol == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
