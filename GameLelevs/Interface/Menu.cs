using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GameLevels
{
    class Menu
    {
        public List<Button> Items { get; set; }
        int screenWidth;

        public Menu(int screenWidth)
        {
            this.screenWidth = screenWidth;
            Items = new List<Button>();
        }

        public void Update()
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].ButtonClick())
                    Items[i].OnClick();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            int y = 100;
            foreach (Button item in Items)
            {
                item.position = new Vector2(screenWidth / 2 - item.tex.Width / 2, y);
                item.Draw(spriteBatch);
                y += item.tex.Height + 10;
            }
            spriteBatch.End();
        }

    }
}
