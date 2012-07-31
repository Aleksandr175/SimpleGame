using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Structs;

// подключим XNA фреймворк
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

// TODO: посмотреть правилльное документирование кода

namespace GameLevels
{
    /// <summary>
    /// Класс описывающий основные функции игрока
    ///  - отрисовка
    ///  - передвижение
    ///  - взаимодействие
    /// </summary>
    class Player
    {
        private SpriteBatch spriteBatch;

        // текстура для вывода игрока
        private Texture2D playerTexture;

        // для анимации
        private FrameInfo frameInfo;

        // информация об игроке
        private PlayerInfo playerInfo;

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Player()
        {
            playerInfo.speed = 0;
            playerInfo.isRunning = false;

            frameInfo.height = 0;
            frameInfo.width = 0;
            frameInfo.timeForFrame = 100;
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="sb">SB</param>
        /// <param name="playerTexture">Текстура игрока</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        public Player(SpriteBatch sb, Texture2D playerTexture, int width, int height)
            : base()
        {

            this.spriteBatch = sb;
            this.playerTexture = playerTexture;

            frameInfo.width = width;
            frameInfo.height = height;

            // вычислим сколько кадров в анимации
            frameInfo.count = playerTexture.Width / frameInfo.width;
        }

        /// <summary>
        /// Перегруженный конструктор класса
        /// </summary>
        /// <param name="sb">SB</param>
        /// <param name="playerTexture">Текстура игрока</param>
        /// <param name="frameSettings">Информация о фрейме</param>
        public Player(SpriteBatch sb, Texture2D playerTexture, FrameInfo frameInfo)
            : base()
        {

            this.spriteBatch = sb;
            this.playerTexture = playerTexture;

            this.frameInfo = frameInfo;
        }

        /// <summary>
        /// Функция отрисовывает игрока
        /// </summary>
        public void Draw(Rectangle rect)
        {
            spriteBatch.Begin();

            if (playerInfo.isRunning)
            {
                Rectangle source = new Rectangle(frameInfo.width * frameInfo.current, 0, frameInfo.width, frameInfo.height);
                spriteBatch.Draw(playerTexture, rect, source, Color.White);
            }
            else 
            {
                spriteBatch.Draw(playerTexture, rect, Color.White);
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Обновление положения игрока
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (playerInfo.isRunning) 
            {
                frameInfo.timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (frameInfo.timeElapsed > frameInfo.timeForFrame) 
                {
                    frameInfo.timeElapsed = 0;
                    frameInfo.current = (frameInfo.current + 1) % frameInfo.count;
                }
            }
        }

    }
}
