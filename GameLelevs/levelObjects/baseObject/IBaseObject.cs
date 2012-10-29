using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameLevels.levelObjects
{
    /// <summary>
    /// Базовый интерфейс всех объектов
    /// </summary>
    /// <!-- по хорошему он мог бы быть базовым и для игрока, охранников? -->
    interface IBaseObject
    {
        /// <summary>
        /// Открытый метод, для отрисовки объекта
        /// </summary>
        /// <param name="spriteBatch"></param>
        void Draw(SpriteBatch spriteBatch);
    }
}
