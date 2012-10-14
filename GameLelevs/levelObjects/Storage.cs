using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace GameLevels.levelObjects
{
    class Storage
    {
        // словарь для текстур
        private Dictionary<String, Texture2D> textures = new Dictionary<String, Texture2D>();

        // словарь для шрифтов
        private Dictionary<String, SpriteFont> fonts = new Dictionary<String, SpriteFont>();

        /// <summary>
        /// Добавляет текстуру в хранилице
        /// К сожалению, нельзя вызыать из этого класса метод Content.Load<T>
        /// </summary>
        /// <param name="name">Название текстуры</param>
        /// <param name="texture">Сама текстура</param>
        public void PushTexture2D(String name, Texture2D texture)
        {
            textures.Add(name, texture);
        }

        /// <summary>
        /// Возвращает текстуру по названию
        /// </summary>
        /// <param name="name">Имя текстуры</param>
        /// <returns>Возвращает текстуру или исключение, что она не найдена</returns>
        public Texture2D Pull2DTexture(String name)
        {
            Texture2D texture;
            textures.TryGetValue(name, out texture);

            return texture;
        }

        /// <summary>
        /// Добавляет текстуру в хранилице
        /// К сожалению, нельзя вызыать из этого класса метод Content.Load<T>
        /// </summary>
        /// <param name="name">Название текстуры</param>
        /// <param name="texture">Шрифт</param>
        public void PushFont(String name, SpriteFont font)
        {
            fonts.Add(name, font);
        }

        /// <summary>
        /// Возвращает текстуру по названию
        /// </summary>
        /// <param name="name">Имя текстуры</param>
        /// <returns>Возвращает текстуру или исключение, что она не найдена</returns>
        public SpriteFont PullFont(String name)
        {
            SpriteFont font;
            fonts.TryGetValue(name, out font);

            return font;
        }

    }
}
