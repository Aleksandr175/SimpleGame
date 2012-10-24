using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLevels.levelObjects;

namespace GameLevels
{
    class Object : BaseObject, ICard
    {
        public Object(Rectangle rect, Texture2D texture, Game1 game, Camera camera)
            : base(rect, texture, game, camera) {
        }
    }
}
