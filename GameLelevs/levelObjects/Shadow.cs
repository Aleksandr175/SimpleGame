using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLevels.levelObjects
{
    /// <summary>
    /// Класс тумана войны. 
    /// Скрывает или открывает объекты на уровне, комнате.
    /// </summary>
    class Shadow
    {

        public List<Guards> guards; // список охранников
        public List<Object> objs; // объекты на уровне


        public bool isShadow = false; // включен ли туман войны?

        public Shadow()
        {
        }

        /// <summary>
        /// Скрываем все объекты на уровне
        /// </summary>
        private void HideAll() 
        {
            foreach (Guards guard in guards)
            {
                guard.visible = false;
            }
            foreach (Object obj in objs)
            {
                obj.visible = false;
            }
        }

        /// <summary>
        /// Показываем все объекты на уровне
        /// </summary>
        private void ShowAll()
        {
            foreach (Guards guard in guards)
            {
                guard.visible = true;
            }
            foreach (Object obj in objs)
            {
                obj.visible = true;
            }
        }

        /// <summary>
        /// Показываем все объекты в комнате
        /// </summary>
        public void ShowInRoom()
        {

        }

        /// <summary>
        /// Функция в зависимости от наличия тумана войны, 
        /// вызывает либо скрытие всех объектов и охранников,
        /// либо наоборот вывод изображений на экран
        /// </summary>
        /// <param name="guards">Список экземпляров охранников</param>
        /// <param name="objs">Список экземпляров объектов</param>
        public void HideOrShowAll(List<Guards> guards, List<Object> objs)
        {
            this.guards = new List<Guards>();
            this.guards = guards;
            this.objs = new List<Object>();
            this.objs = objs;

            if (this.isShadow)
            {
                HideAll();
            }
            else
            {
                ShowAll();
            }
        }




    }
}
