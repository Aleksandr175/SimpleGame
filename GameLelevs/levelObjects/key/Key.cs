using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

enum EType { Simple, Hard };

namespace GameLevels.levelObjects
{
    class Key : BaseObject, IKey
    {

        /// <summary>
        /// тип ключа
        /// </summary>
        private EType type;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="rect">Область, где должен быть расположен объект</param>
        /// <param name="texture">Текстура</param>
        /// <param name="game">Ссылка на игру</param>
        /// <param name="camera">Ссылка на камеру</param>
        public Key(Rectangle rect, Texture2D texture, Game1 game, Camera camera)
            : base(rect, texture, game, camera) {
        }
    }
}
