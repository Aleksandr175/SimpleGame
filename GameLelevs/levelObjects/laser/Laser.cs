using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLevels.levelObjects;

namespace GameLevels
{
    class Laser : BaseObject, ILaser
    {
        public Laser(Rectangle rect, Texture2D texture, Game1 game, Camera camera) 
            : base(rect, texture, game, camera) {
        }
    }
}
