using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XMLContent;

namespace GameLevels.coreMission
{
    interface ICoreMission
    {
        /// <summary>
        /// Добавим все текущие цели
        /// </summary>
        /// <param name="loader">Загрузчик XML миссий</param>
        void ParseLoader(XMLCoreMissionLoader loader);

        /// <summary>
        /// Проверяет может ли игрок перейти на новый уровень
        /// </summary>
        /// <returns>может ли перйти</returns>
        bool GoNextLevel();
    }
}
