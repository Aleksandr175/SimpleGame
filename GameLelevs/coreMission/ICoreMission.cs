using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLevels.coreMission
{
    interface ICoreMission
    {
        /// <summary>
        /// Задание основной цели миссии
        /// </summary>
        void SetMainMissionTarget();

        /// <summary>
        /// Задание второстепенных целей
        /// </summary>
        void SetSecondaryMissionTarget();

        /// <summary>
        /// Задание ключевой точки
        /// </summary>
        /// <param name="point">Точка</param>
        /// <param name="effect">Эффект</param>
        /// <returns>Удалось ли установить точку</returns>
        bool SetKeyPoint(Point point, Effect effect);
    }
}
