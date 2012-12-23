using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

enum EColor { Red, Blue, Green };

namespace GameLevels.levelObjects
{
    /// <summary>
    /// Класс карта
    /// </summary>
    class Card : BaseObject, ICard
    {
        /// <summary>
        /// Цвет карты
        /// </summary>
        private EColor color;

        public int targetDoorX; // координаты двери-цели
        public int targetDoorY;

        public Card(Rectangle rect, Texture2D texture, Game1 game, Camera camera)
            : base(rect, texture, game, camera) {
        }
    }
}
