﻿using System;
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
        public List<Laser> lasers; // лазеры на уровне
        public List<Cameras> cameras; // камеры на уровне
        public List<SysControl> sysControls; // камеры на уровне


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

        /// <summary>
        /// Принимаем объекты
        /// </summary>
        /// <param name="guards">Охранники</param>
        /// <param name="objs">Объекты</param>
        /// <param name="lasers">Лазеры</param>
        /// <param name="cameras">Камеры слежения</param>
        /// <param name="sysControls">Системы управления камерами</param>
        public Shadow(List<Guards> guards, List<Object> objs, List<Laser> lasers, List<Cameras> cameras, List<SysControl> sysControls)
        {
            this.guards = new List<Guards>();
            this.guards = guards;
            this.objs = new List<Object>();
            this.objs = objs;
            this.lasers = new List<Laser>();
            this.lasers = lasers;
            this.cameras = new List<Cameras>();
            this.cameras = cameras;
            this.sysControls = new List<SysControl>();
            this.sysControls = sysControls;
            

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
            foreach (Laser laser in lasers)
            {
                laser.isVisible = false;
            }
            foreach (Cameras camera in cameras)
            {
                camera.isVisible = false;
            }
            foreach (SysControl sysControl in sysControls)
            {
                sysControl.isVisible = false;
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
            foreach (Laser laser in lasers)
            {
                laser.isVisible = true;
            }
            foreach (Cameras camera in cameras)
            {
                camera.isVisible = true;
            }
            foreach (SysControl sysControl in sysControls)
            {
                sysControl.isVisible = true;
            }

        }

        /// <summary>
        /// Показываем все объекты в комнате
        /// </summary>
        /// <param name="room">Номер комнаты</param>
        public void ShowInRoom(int room)
        {

            if (room != 0)
            {
                foreach (Object obj in objs)
                {
                    if (LevelLoader.levelMapRooms[obj.Rect.X / LevelLoader.Size, obj.Rect.Y / LevelLoader.Size] == room)
                    {
                        obj.isVisible = true;
                    }
                }
                foreach (Laser laser in lasers)
                {
                    if (LevelLoader.levelMapRooms[laser.Rect.X / LevelLoader.Size, laser.Rect.Y / LevelLoader.Size] == room)
                    {
                        laser.isVisible = true;
                    }
                }
                foreach (Guards guard in guards)
                {
                    if (LevelLoader.levelMapRooms[guard.Rect.X / LevelLoader.Size, guard.Rect.Y / LevelLoader.Size] == room)
                    {
                        guard.isVisible = true;
                    }
                }
                foreach (Cameras camera in cameras)
                {
                    if (LevelLoader.levelMapRooms[camera.posX, camera.posY] == room)
                    {
                        camera.isVisible = true;
                    }
                }
                foreach (SysControl sysControl in sysControls)
                {
                    if (LevelLoader.levelMapRooms[sysControl.posX, sysControl.posY] == room)
                    {
                        sysControl.isVisible = true;
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
