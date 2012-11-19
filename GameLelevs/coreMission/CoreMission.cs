using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLevels.coreMission
{
    class CoreMission : ICoreMission
    {
        /// <summary>
        /// Задание основной цели миссии
        /// </summary>
        public void SetMainMissionTarget() { 
        }

        /// <summary>
        /// Задание второстепенных целей
        /// </summary>
        public void SetSecondaryMissionTarget() {
        }

        /// <summary>
        /// Задание ключевой точки
        /// </summary>
        /// <param name="point">Точка</param>
        /// <param name="effect">Эффект</param>
        /// <returns>Удалось ли установить точку</returns>
        public bool SetKeyPoint(Vector2 point, Effect effect) {
            return true;
        }
    }
}
