using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Enumerations;

namespace GameLevels.levelObjects.door
{
    /// <summary>
    ///  Класс "дверь"
    /// </summary>
    class Door : BaseObject, IDoor
    {
        /// <summary>
        /// Цвет двери
        /// </summary>
        private EColor color;

        /// <summary>
        /// Отвечает за "закрытость" двери
        /// </summary>
        private bool isClosed;

        /// <summary>
        /// Ориентация двери
        /// </summary>
        private DoorOrientation orientation;

        private int indexI;
        private int indexJ;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="rect">Координаты текстуры</param>
        /// <param name="texture">Текстура</param>
        /// <param name="game">Ссылка на игру</param>
        /// <param name="camera">Ссылка на камеру</param>
        /// <param name="orientation">Ориентация стены</param>
        /// <param name="indexI">Индекс в карте уровня</param>
        /// <param name="indexJ">Индекс в карте уровня</param>
        /// <param name="isCLosed">Закрыта ли дверь</param>
        public Door(Rectangle rect, Texture2D texture, Game1 game, Camera camera, DoorOrientation orientation, bool isCLosed = false, int indexI = -1, int indexJ = -1)
            : base(rect, texture, game, camera) {

                this.orientation = orientation;
                this.isClosed = isCLosed;

                this.indexI = indexI;
                this.indexJ = indexJ;
        }

        /// <summary>
        /// Просматривает закрыта ли дверь
        /// </summary>
        /// <returns>Закрыта ли дверь</returns>
        public bool IsClosed() {
            return this.isClosed;
        }

        /// <summary>
        /// Возвращает цвет двери
        /// </summary>
        /// <returns>Цвет двери</returns>
        public EColor GetColor() {
            return this.color;
        }

        /// <summary>
        /// Открывает дверь, если это возможно
        /// </summary>
        /// <returns>Удалось ли открыть дверь</returns>
        public bool Open(Texture2D openDoor, Key key = null)
        {
            if (!this.isClosed)
                return false;

            // TODO: продумать логику открывания двери
            this.texture = openDoor;
            this.isClosed = false;

            return true;
        }

        /// <summary>
        /// Возвращает ориентацию двери
        /// </summary>
        /// <returns>Ориентация двери</returns>
        public DoorOrientation GetOrientation() {
            return this.orientation;
        }

        /// <summary>
        /// Возвращает индекс I
        /// </summary>
        /// <returns>Индекс I</returns>
        public int GetIndexI() {
            return this.indexI;
        }

        /// <summary>
        /// Возвращает индекс J
        /// </summary>
        /// <returns>Индекс J</returns>
        public int GetIndexJ() {
            return this.indexJ;
        }
    }
}
