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

        private static int levelLenghtX; // длина уровня
        private static int levelLenghtY;

        public static int LevelLenghtX
        {
            set { Shadow.levelLenghtX = value; }
        }
        public static int LevelLenghtY
        {
            set { Shadow.levelLenghtY = value; }
        }

        public bool isShadow = false; // включен ли туман войны?

        public Shadow(List<Guards> guards, List<Object> objs)
        {
            this.guards = new List<Guards>();
            this.guards = guards;
            this.objs = new List<Object>();
            this.objs = objs;
        }

        /// <summary>
        /// Скрываем все объекты на уровне
        /// </summary>
        private void HideAll() 
        {
            foreach (Guards guard in guards)
            {
                guard.isVisible = false;
            }
            foreach (Object obj in objs)
            {
                obj.isVisible = false;
            }
        }

        /// <summary>
        /// Показываем все объекты на уровне
        /// </summary>
        private void ShowAll()
        {
            foreach (Guards guard in guards)
            {
                guard.isVisible = true;
            }
            foreach (Object obj in objs)
            {
                obj.isVisible = true;
            }
        }

        /// <summary>
        /// Показываем все объекты в комнате
        /// </summary>
        /// <param name="room">Номер комнаты</param>
        public void ShowInRoom(int room)
        {
/*            foreach (Guards guard in guards)
            {
                guard.visible = true;
            }*/
            if (room != 0)
            {
                foreach (Object obj in objs)
                {
                    if (LevelLoader.levelMapRooms[obj.Rect.X / LevelLoader.Size, obj.Rect.Y / LevelLoader.Size] == room)
                    {
                        obj.isVisible = true;
                    }
                }
                foreach (Guards guard in guards)
                {
                    if (LevelLoader.levelMapRooms[guard.Rect.X / LevelLoader.Size, guard.Rect.Y / LevelLoader.Size] == room)
                    {
                        guard.isVisible = true;
                    }
                }

            }
        }

        /// <summary>
        /// Функция в зависимости от наличия тумана войны, 
        /// вызывает либо скрытие всех объектов и охранников,
        /// либо наоборот вывод изображений на экран
        /// </summary>
        /// <param name="guards">Список экземпляров охранников</param>
        /// <param name="objs">Список экземпляров объектов</param>
        public void HideOrShowAll()
        {
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
