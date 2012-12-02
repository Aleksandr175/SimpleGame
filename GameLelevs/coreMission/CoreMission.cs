using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XMLContent;

namespace GameLevels.coreMission
{
    class CoreMission : ICoreMission
    {
        public List<BaseTarget> targets;
        public List<Rectangle> keyRects;

        public CoreMission()
        {
            targets = new List<BaseTarget>();
            keyRects = new List<Rectangle>();
        }

        /// <summary>
        /// Проверяет может ли игрок перейти на новый уровень
        /// </summary>
        /// <returns>может ли перйти</returns>
        public bool GoNextLevel()
        {
            // проверим все главные цели
            foreach (BaseTarget target in targets)
            {
                if (!target.IsDone && target.IsMain)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Добавим все текущие цели
        /// </summary>
        /// <param name="loader">Загрузчик XML миссий</param>
        public void ParseLoader(XMLCoreMissionLoader loader)
        {

            targets.Clear();
            keyRects.Clear();

            // порйдемся по каждой цели и добавим её

            foreach (XMLExpressionTarger target in loader.expressions)
            {
                ExpressionTarger expTarget = new ExpressionTarger(target.name, target.description, target.isMain, target.expression, target.rightValue);
                targets.Add(expTarget);

                Rectangle rect = new Rectangle((int)target.point.X, (int)target.point.Y, 40, 40);
                keyRects.Add(rect);
            }

            foreach (XMLTimeTarget target in loader.times)
            {
                TimeTarget timeTarget = new TimeTarget(target.name, target.description, target.isMain, target.runtime);
                targets.Add(timeTarget);

                Rectangle rect = new Rectangle((int)target.point.X, (int)target.point.Y, 40, 40);
                keyRects.Add(rect);
            }

            foreach (XMLCollectTarget target in loader.collects)
            {
                CollectTarget colTarget = new CollectTarget(target.name, target.description, target.isMain, target.count);
                targets.Add(colTarget);

                Rectangle rect = new Rectangle((int)target.point.X, (int)target.point.Y, 40, 40);
                keyRects.Add(rect);
            }

        }
    }
}
