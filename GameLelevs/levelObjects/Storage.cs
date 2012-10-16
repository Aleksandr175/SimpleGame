using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

// шаблоный делегат для загрузки чего либо
public delegate T ContentLoad<T>(string path);

namespace GameLevels.levelObjects
{
    class Storage
    {
        // фактически это указатель на функцию Content.Load<Texture2D> для загрузки текстур
        public ContentLoad<Texture2D> TexturesLoader;

        // фактически это указатель на функцию Content.Load<SpriteFont> для загрузки шрифтов
        public ContentLoad<SpriteFont> FontLoader;

        // словарь для текстур
        private Dictionary<String, Texture2D> textures = new Dictionary<String, Texture2D>();

        // словарь для шрифтов
        private Dictionary<String, SpriteFont> fonts = new Dictionary<String, SpriteFont>();
                
        /// <summary>
        /// Добавляет текстуру в хранилице
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

        /// <summary>
        /// <para>Рекурсивный метод, загружающий в словарь все текстуры из указанной папки</para>
        /// <para>Создает в словаре запись с указанным именем и текстурой</para>
        /// </summary>
        /// <param name="activeDir">Путь к папке</param>
        public void LoadTexture2DFolder(string activeDir) { 

            // если папки не существует то вызывать исключение
            if (!Directory.Exists(activeDir))
                return;

            // вспомогательные переменные для 
            string fileName;
            string filePath;
            string newActiveDir = activeDir.Remove(0, 8);

            // найдем информацию о папке
            DirectoryInfo di = new DirectoryInfo(activeDir);
            
            // загрузим по одному файлу
            FileInfo[] fi = di.GetFiles();

            // единственный недостаток - если текстуры называются одинаково, то они не загрузятся
            foreach (FileInfo file in fi) {
                // проверка, чтобы не загружать файлы .xnb
                if(String.Equals(file.Name.Substring((file.Name.LastIndexOf('.')) + 1, 3), "xnb")) {
                    fileName = file.Name.Remove(file.Name.IndexOf('.'));
                    filePath = Path.Combine(newActiveDir, fileName);
                    PushTexture2D(fileName, TexturesLoader(filePath));
                }
            }
            // выберем все папки
            DirectoryInfo[] directories = di.GetDirectories();

            // обойдем каждую папку
            foreach (DirectoryInfo dir in directories) {
                LoadTexture2DFolder(Path.Combine(activeDir, dir.Name));
            }
        }

    }
}
