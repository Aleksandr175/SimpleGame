using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLevels.levelObjects.money
{
    class Rubin : BaseObject, IMoney
    {

        public Rubin(Rectangle rect, Texture2D texture, Game1 game, Camera camera, int cost)
            : base(rect, texture, game, camera)
        {
        }
    }
}
