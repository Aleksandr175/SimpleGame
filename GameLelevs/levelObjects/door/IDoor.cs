using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Enumerations;

namespace GameLevels.levelObjects.door
{
    /// <summary>
    /// Интерфейс для объектов типа "Дверь"
    /// </summary>
    interface IDoor
    {
        /// <summary>
        /// Просматривает закрыта ли дверь
        /// </summary>
        /// <returns>Закрыта ли дверь</returns>
        bool IsClosed();
        
        /// <summary>
        /// Возвращает цвет двери
        /// </summary>
        /// <returns>Цвет двери</returns>
        EColor GetColor();

        /// <summary>
        /// Открывает дверь, если это возможно
        /// </summary>
        /// <returns>Удалось ли открыть дверь</returns>
        bool Open(Texture2D openDoor, Key key = null);

        /// <summary>
        /// Возвращает ориентацию двери
        /// </summary>
        /// <returns>Ориентация двери</returns>
        DoorOrientation GetOrientation();

        /// <summary>
        /// Возвращает индекс I
        /// </summary>
        /// <returns>Индекс I</returns>
        int GetIndexI();

        /// <summary>
        /// Возвращает индекс J
        /// </summary>
        /// <returns>Индекс J</returns>
        int GetIndexJ();
    }
}
