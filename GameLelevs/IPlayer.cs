using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enumeration;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameLevels.IPlayer
{
    interface IPlayer
    {
        void Init();
        void Run(PlayerMove move);
        void Stop();
        void Draw(SpriteBatch spriteBatch);
        void Update(GameTime gameTime);
    }
}
