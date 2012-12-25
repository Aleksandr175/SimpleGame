using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.IO;

namespace GameLevels
{
    class Menu
    {
        public List<Button> Items { get; set; }
        int screenWidth;
        public int page;
        int numberOfButtons;
        Texture2D open, close;
        public Button next, previous, back;
        public SpriteFont font;
        Cursor cursor;
        public Menu(int screenWidth)
        {
            this.screenWidth = screenWidth;
            Items = new List<Button>();
            page = 0;
            numberOfButtons = 0;
        }

        public void Update()
        {
            cursor.Update();
            for (int i = page * numberOfButtons; (i < (page + 1) * numberOfButtons) && (i < Items.Count); i++)
                if (Items[i].ButtonClick(cursor.State, cursor.OldState))
                    Items[i].OnClick();
            if (next != null)
                if (next.ButtonClick(cursor.State, cursor.OldState))
                    next.OnClick();
            if (previous != null)
                if (previous.ButtonClick(cursor.State, cursor.OldState))
                    previous.OnClick();
            if (back != null)
                if (back.ButtonClick(cursor.State, cursor.OldState))
                    back.OnClick();

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //выводится не больше 6 пунктов
            if (Items.Count < 8)
                numberOfButtons = Items.Count;
            else
                numberOfButtons = 8;
            spriteBatch.Begin();

            if (Items.Count < 8)
                //рисовать главное меню
                DrawMainMenu(spriteBatch);
            else
                //рисовать меню выбора уровня
                DrawLvlMenu(spriteBatch);

            if (Hover())
                cursor.DrawPointer(spriteBatch);
            else
                cursor.Draw(spriteBatch);
            spriteBatch.End();
        }
        public void DrawMainMenu(SpriteBatch spriteBatch)
        {
            float posX;
            int y = 100;
            posX = screenWidth / 2 - Items[0].tex.Width / 2;
            foreach (Button item in Items)
            {
                item.position = new Vector2(posX, y);
                item.Draw(spriteBatch);
                y += item.tex.Height + 10;
            }
            spriteBatch.DrawString(font, "Меню", new Vector2((screenWidth / 2 - font.MeasureString("Меню").X / 2), 20), Color.White);

        }

        public void DrawLvlMenu(SpriteBatch spriteBatch)
        {

            float posX;
            int y = 100;
            posX = screenWidth / 4 - Items[0].tex.Width / 2;
            for (int i = page * (numberOfButtons); (i < (page + 1) * numberOfButtons) && (i < Items.Count); i++)
            {
                Button item = Items[i];
                if (i == 4 + page * numberOfButtons)
                {
                    y = 100;
                    posX = (screenWidth / 2) + (screenWidth / 4) - item.tex.Width / 2;
                }
 
                item.position = new Vector2(posX, y);
                item.Draw(spriteBatch);
                if (IsLvlFinished(i-1))
                    spriteBatch.Draw(open, new Vector2(posX + item.tex.Width - open.Width, y), Color.White);
                else
                    spriteBatch.Draw(close, new Vector2(posX + item.tex.Width - close.Width, y), Color.White);
                y += item.tex.Height + 10;

            }
            previous.position = new Vector2((screenWidth / 4) - Items[0].tex.Width / 2, 400);
            previous.Draw(spriteBatch);
            next.position = new Vector2((screenWidth / 2) + (screenWidth / 4) - Items[0].tex.Width / 2, 400);
            next.Draw(spriteBatch);
            back.position = new Vector2((screenWidth / 2) - Items[0].tex.Width / 2, 480);
            back.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Выберите уровень:", new Vector2((screenWidth / 2 - font.MeasureString("Выберите уровень:").X / 2), 20), Color.White);

        }
        
        public bool Hover()
        {
            for (int i = page * numberOfButtons; (i < (page + 1) * numberOfButtons) && (i < Items.Count); i++)
                if (Items[i].Hover(cursor.State))
                    return true;
            if (next != null)
            {
                if (next.Hover(cursor.State))
                    return true;
            }
            if (previous != null)
            {
                if (previous.Hover(cursor.State))
                    return true;
            }
            if (back != null)
            {
                if (back.Hover(cursor.State))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Проверка пройден ли уровень
        /// </summary>
        /// <param name="number">номер уровня</param>
        public bool IsLvlFinished(int number)
        {
            string[] info = File.ReadAllLines("content/lvl_info.txt");
            if (number == -1)
                return true;
            if (info[number].EndsWith("1"))
                return true;
            return false;
        }
        //текстуры замочка и шрифт
        public void LoadTextures(Texture2D open, Texture2D close, SpriteFont font)
        {
            this.open = open;
            this.close = close;
            this.font = font;
        }
        public void LoadCursor(Cursor cursor)
        {
            this.cursor = cursor;
        }

    }
}
