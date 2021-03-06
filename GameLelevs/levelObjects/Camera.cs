﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLevels
{
    class Camera
    {
        Game1 game;
        LevelLoader levelLoader;
        public Rectangle Rect { get; set; }
        
        void camera(Game1 game, LevelLoader levelLoader)
        {
            this.game = game;
            this.levelLoader = levelLoader;
        }
        //экранные координаты - смещение камеры относительно начала мировых координат 
        int scrollX;
        int scrollY;

        public int ScrollX
        {
            get { return scrollX; }
            set { scrollX = value; }
        }
        public int ScrollY
        {
            get { return scrollY; }
            set { scrollY = value; }
        }



        // скорость перемещения камеры
        int speedCamera = 2;

        public int GetSpeedCamera
        {
            get { return speedCamera; }
        }



        /*
         * Ф-ция для получения прямоугольника текущего положения экрана
         */
        public Rectangle GetScreenRect(Rectangle rect)
        {
            Rectangle r = rect;
            r.Offset(-scrollX, -scrollY);
            return r;
        }


        //смещает камеру относительно начала уровня
        public void Scroll(int dx, int dy)
        {
            // TODO: сделать, чтобы просмотр камеры не уходил, если есть еще много места для просмотра 
            //                                                    и при столкновении игрока со стенами
            if (scrollX + dx > 0 && scrollX + dx < LevelLoader.GetLenghtX - game.GetScreenWidth)
            {
                scrollX += dx;
            }
            if (scrollY + dy > 0 && scrollY + dy < LevelLoader.GetLenghtY - game.GetScreenHeight)
            {
                scrollY += dy;
            }
        }



        //ф-ция для позиционирования камеры в определенной точке
        public void SetCameraPosition(int x, int y)
        {
            scrollX = x;
            scrollY = y;
        }

    }
}
