using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLevels.levelObjects
{
    class Money : BaseObject, IMoney
    {
        // достоинство монеты
        private int cost;

        public Money(Rectangle rect, Texture2D texture, Game1 game, Camera camera, int cost)
            : base(rect, texture, game, camera) {

                this.cost = cost;
        }

        // свойство для установки / возврата значения достоинства монеты
        public int Cost {
            set {
                this.cost = value < 0 ? 0 : value;
            }
            get {
                return this.cost;
            }
        }
    }
}
