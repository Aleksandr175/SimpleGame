using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enumerations;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLevels.IMan
{
    interface IMan
    {
        void Init();
        void Run(PlayerMove move);
        void Stop();
        void Draw(SpriteBatch spriteBatch);
        void Update(GameTime gameTime);
    }
}
