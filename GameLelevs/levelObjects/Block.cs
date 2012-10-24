using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLevels.levelObjects;

namespace GameLevels
{
    //класс, отвечающий за отрисовку стен игрового уровня
    class Block : BaseObject, IBlock
    {

        public Block(Rectangle rect, Texture2D texture, Game1 game, Camera camera) 
            : base(rect, texture, game, camera) {
        }
    }
}
