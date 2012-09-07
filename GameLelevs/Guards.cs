using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Structs;

// подключим XNA фреймворк
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Enumeration;

namespace GameLevels
{
    class Guards
    {
        public Rectangle Rect { get; set; }

        // текстура для вывода охранника в состоянии спокойствия
        private Texture2D idlTexture;

        // текстура для вывода охранника в состоянии бега
        private Texture2D runTexture;

        // для анимации
        private FrameInfo frameInfo;

        // информация об охраннике
        private GuardInfo guardInfo;

        // ссылка на экран
        private Game1 game;
        
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="playerTexture">Текстура охранника</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        public Guards(Texture2D idlTexture, Texture2D runTexture, Rectangle position, Game1 game)
        {
            Init();
            this.idlTexture = idlTexture;
            this.runTexture = runTexture;

            frameInfo.height = frameInfo.width = runTexture.Height;

            // вычислим сколько кадров в анимации
            frameInfo.count = this.runTexture.Width / frameInfo.width;

            guardInfo.position = position;

            this.game = game;
        }

        /// <summary>
        /// Перегруженный конструктор класса
        /// </summary>
        /// <param name="sb">SB</param>
        /// <param name="playerTexture">Текстура игрока</param>
        /// <param name="frameSettings">Информация о фрейме</param>
        public Guards(Texture2D idlTexture, Texture2D runTexture, FrameInfo frameInfo, Game1 game)
        {
            Init();
            this.idlTexture = idlTexture;
            this.runTexture = runTexture;

            this.frameInfo = frameInfo;

            this.game = game;
        }

        /// <summary>
        /// Функция инициализирует значениями по умолчанию поля класса
        /// </summary>
        public void Init()
        {
            guardInfo.speed = 15;

            frameInfo.height = 0;
            frameInfo.width = 0;
            frameInfo.timeForFrame = 5;
        }

        /// <summary>
        /// Функция устанавливает флаг в состояние бега
        /// </summary>>
        public void Run(PlayerMove move) 
        {
            guardInfo.isRunning = true;
            guardInfo.direction = move;
        }

        /// <summary>
        /// Функция устанавливает флаг в состояние спокойствия
        /// </summary>>
        public void Stop()
        {
            guardInfo.isRunning = false;
            frameInfo.current = 0;
            frameInfo.timeElapsed = 0;
        }

        /// <summary>
        /// Функция отрисовывает игрока
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            
            if (guardInfo.isRunning)
            {
                Rectangle source = new Rectangle(frameInfo.width * frameInfo.current, 0, frameInfo.width, frameInfo.height);
                spriteBatch.Draw(runTexture, guardInfo.position, source, Color.White);
            }
            else 
            {
                spriteBatch.Draw(idlTexture, guardInfo.position, Color.White);
            }

            
        }

        /// <summary>
        /// Обновление положения охранника
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (guardInfo.isRunning) 
            {
                // изменим кадр анимации
                frameInfo.timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (frameInfo.timeElapsed > frameInfo.timeForFrame) 
                {
                    frameInfo.timeElapsed = 0;
                    frameInfo.current = (frameInfo.current + 1) % frameInfo.count;
                }

                // передвижение игрока
                int offset = guardInfo.speed * gameTime.ElapsedGameTime.Milliseconds / 10;

                // новое положение игрока
                Rectangle newPosition = guardInfo.position;

                // смотрим, в каком направлении движемся 
                switch (guardInfo.direction)
                {
                    /*case GuardMove.Up:
                        newPosition.Offset(0, -offset);
                        break;

                    case GuardMove.Left:
                        newPosition.Offset(-offset, 0);
                        break;

                    case GuardMove.Right:
                        newPosition.Offset(offset, 0);
                        break;

                    case GuardMove.Down:
                        newPosition.Offset(0, offset);
                        break;
                    */
                    default: break;
                }

                if (newPosition.Left > 0 && newPosition.Right < game.Width && !game.CollidesWithLevel(newPosition))
                    guardInfo.position = newPosition;
            }
        }

    }
}
