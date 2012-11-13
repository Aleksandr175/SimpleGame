using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLevels.levelObjects;
using Enumerations;

namespace GameLevels
{
    class Laser : BaseObject, ILaser
    {
        private bool isActive = true; // включен ли лазер?

        public bool IsActive
        {
            get { return isActive; }
        }

        Texture2D textureInactive; // текстура выключенного лазера
        

        private static float intervalActivity = 2.0f; // интервал работы лазера. Каждые 2 сек. лазер включается и выключается.
        private float timer; // время до следующего отключения, вкл. лазера
        public LevelObject typeLaser; // тип лазера (гориз/вертик).
        
        public Laser(Rectangle rect, Texture2D texture, Texture2D textureInactive, Game1 game, Camera camera) 
            : base(rect, texture, game, camera) {
                timer = intervalActivity;
                intervalActivity += 0.3f;
                
            this.textureInactive = textureInactive;
        }

        public void Update(GameTime gameTime)
        {
            if (timer > 0)
            {
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (timer <= 0)
            {
                timer = intervalActivity;
                isActive = !isActive;
            }
        }


        

        /// <summary>
        /// Открытый метод, для отрисовки объекта
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D currentTexture = textureInactive;

            if (this.isActive)
            {
                currentTexture = texture; 
            }
            else
            {
                currentTexture = textureInactive;
            }
            

            Rectangle screenRect = camera.GetScreenRect(Rect);  // рисуем только то, что помещается на экране
            spriteBatch.Draw(currentTexture, screenRect, Color.White);
        }

    }
}
