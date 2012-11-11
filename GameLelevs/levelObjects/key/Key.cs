using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLevels.levelObjects
{
    class Key : BaseObject, IKey
    {

        /// <summary>
        /// цвет ключа
        /// </summary>
        private EColor color;

        private bool isVisible = false;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="rect">Область, где должен быть расположен объект</param>
        /// <param name="texture">Текстура</param>
        /// <param name="game">Ссылка на игру</param>
        /// <param name="camera">Ссылка на камеру</param>
        public Key(Rectangle rect, Texture2D texture, Game1 game, Camera camera, EColor color)
            : base(rect, texture, game, camera) {

                this.color = color;
        }

        /// <summary>
        /// Возвращает цвет ключа
        /// </summary>
        /// <returns></returns>
        public EColor GetColor() {
            return this.color;
        }
    }
}
