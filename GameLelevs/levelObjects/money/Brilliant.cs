using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLevels.levelObjects.money
{
    class Brilliant : BaseObject, IMoney
    {
        public Brilliant(Rectangle rect, Texture2D texture, Game1 game, Camera camera, int cost)
            : base(rect, texture, game, camera)
        {
        }
    }
}
