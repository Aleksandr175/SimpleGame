using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLevels
{
    class FPSCounter : DrawableGameComponent
    {
        public int FPS;

        int frames;
        double seconds; 

        public FPSCounter(Game game) : base(game) 
        {
        }

        public override void Update(GameTime gameTime)
        {
            seconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (seconds >= 1)
            {
                FPS = frames;
                seconds = 0;
                frames = 0;
                Game.Window.Title = "Коллекционер,  Fps: " + FPS.ToString();  // вывод в заголовке кол-ва кадров в секунду
            }
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            frames++;
            base.Draw(gameTime);
        }

    }
}
