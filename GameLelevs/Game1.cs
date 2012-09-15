using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using Enumeration;

namespace GameLevels
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Player player;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D wallGoriz;
        Texture2D wallVert;
        Texture2D wallDownRight; //угловые стены
        Texture2D wallUpRight;
        Texture2D wallLeftDown;
        Texture2D wallLeftUp;
        Texture2D wall4sides; // стены в 4 стороны
        Texture2D wallURD;  // стены в 3 стороны U - top, R - right, L - left, D - down
        Texture2D wallRDL;
        Texture2D wallDLU;
        Texture2D wallLUR;

        Texture2D wallEmpty;

        Texture2D doorHoriz;  // двери
        Texture2D doorVertic;
        Texture2D doorHorizOpen;  // двери открытые
        Texture2D doorVerticOpen;

        //объявляем текустуры охранников
        Texture2D guardIdleTexture;
        Texture2D guardRunTexture;

        SpriteFont font;

        //экранные координаты - смещение камеры относительно начала мировых координат 
        int scrollX;
        int scrollY;

        //длина уровня в пикселях
        int lenghtX;
        int lenghtY;
        
        //размер 1 ячейки уровня
        int size = 30;
        public int Size
        {
            get { return size; }
        }
        // скорость перемещения камеры
        int speedCamera = 1;

        //размер экрана
        int width;
        int height;

        int sizePeople = 20; //размер изображения игрока и охранников
        public int SizePeople
        {
            get { return sizePeople; }
        }

        //информация об уровнях
        int currentLvl;
        int maxLvl = 4;
        KeyboardState oldState;

        List<Block> blocks; // объекты стен и дверей
        List<Guards> guards; // список охранников
        

        // карта уровня
        // так же используется для алгоритма поиска пути к игроку
        byte[,] levelMap;

        // сложность уровня
        Complexity complexity;

        public int Width 
        {
            get { return width;}
        }

        public int Height
        {
            get { return height; }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            width = this.graphics.PreferredBackBufferWidth = 500; //ширина и высота экрана
            height = this.graphics.PreferredBackBufferHeight = 500;

            this.Components.Add(new FPSCounter(this));  // добавили игровой компонент - fps счетчик

            // отключили ограничение fps счетчика
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // загружаем текстуры стен
            wallGoriz = Content.Load<Texture2D>("Textures/lvl/wall_goriz");
            wallVert = Content.Load<Texture2D>("Textures/lvl/wall_vert");
            wallDownRight = Content.Load<Texture2D>("Textures/lvl/wall_down_right");
            wallUpRight = Content.Load<Texture2D>("Textures/lvl/wall_up_right");
            wallLeftDown = Content.Load<Texture2D>("Textures/lvl/wall_left_down");
            wallLeftUp = Content.Load<Texture2D>("Textures/lvl/wall_left_up");
            wall4sides = Content.Load<Texture2D>("Textures/lvl/wall_4sides");
            wallEmpty = Content.Load<Texture2D>("Textures/lvl/empty");
            wallURD = Content.Load<Texture2D>("Textures/lvl/wall_urd");
            wallRDL = Content.Load<Texture2D>("Textures/lvl/wall_rdl");
            wallDLU = Content.Load<Texture2D>("Textures/lvl/wall_dlu");
            wallLUR = Content.Load<Texture2D>("Textures/lvl/wall_lur");

            // загружаем текстуры дверей
            doorHoriz = Content.Load<Texture2D>("Textures/lvl/doors/door_horiz");
            doorVertic = Content.Load<Texture2D>("Textures/lvl/doors/door_vertic");
            doorHorizOpen = Content.Load<Texture2D>("Textures/lvl/doors/door_horiz_open");
            doorVerticOpen = Content.Load<Texture2D>("Textures/lvl/doors/door_vertic_open");

            //загружаем текстуры для охранников
            guardIdleTexture = Content.Load<Texture2D>("players/player");
            guardRunTexture = Content.Load<Texture2D>("players/player_run");

            // инициализируем нового игрока
            Rectangle plaerPosition = new Rectangle(50, 50, sizePeople, sizePeople);
            player = new Player(Content.Load<Texture2D>("players/player"), Content.Load<Texture2D>("players/player_run"), plaerPosition, this);

            font = Content.Load<SpriteFont>("myFont1");

            //сразу создаем первый уровень
            CreateLevel(3);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /*
         * Ф-ция для получения прямоугольника текущего положения экрана
         */
        public Rectangle GetScreenRect(Rectangle rect)
        {
            Rectangle r = rect;
            r.Offset(-scrollX, -scrollY);
            return r;
        }

        /// <summary>
        /// Функция смотрит, нет ли пересечения игрока с объектами на уровне
        /// </summary>
        /// <param name="rect">Текщее полодение игрока</param>
        /// <returns>Bool - пересечение</returns>
        public bool CollidesWithLevel(Rectangle rect) 
        {
            bool collides = false;

            switch (complexity) 
            {
                case Complexity.Low :
                    collides = CollidesLow(rect);
                    break;
                case Complexity.Medium:
                    collides = CollidesHigh(rect);
                    break;
                case Complexity.High:
                    collides = CollidesHigh(rect);
                    break;
                default: break;
            }

            return collides;
        }

        /// <summary>
        /// Выборка простого взаимодействия с уровнем
        /// </summary>
        /// <param name="rect">Текщее полодение игрока</param>
        /// <returns>Bool - пересечение</returns>
        private bool CollidesLow(Rectangle rect)
        {
            // для каждого блока на уровне
            foreach (Block block in blocks)
            {
                // смотрим, есть ли пересечение
                if (block.Rect.Intersects(rect))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Выборка сложного взаимодействия с уровнем
        /// </summary>
        /// <param name="rect">Текщее полодение игрока</param>
        /// <returns>Bool - пересечение</returns>
        private bool CollidesHigh(Rectangle rect)
        {
            int minx = rect.Left / 20;
            int miny = rect.Top / 20;
            int maxx = rect.Right / 20;
            int maxy = rect.Bottom / 20;

            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                    if (levelMap[i, j] == 1)
                        return true;
            }

            return false;
        }
        //смещает камеру относительно начала уровня
        public void Scroll(int dx, int dy) {
            if (scrollX + dx > 0 && scrollX + dx < lenghtX - width) 
            {
                scrollX += dx;
            }
            if (scrollY + dy > 0 && scrollY + dy < lenghtY - height) 
            {
                scrollY += dy;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState state = Keyboard.GetState();
            
            //смена уровня по нажатию на пробел
            if (state.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space)) {
                if (oldState != state)
                {
                    currentLvl++;
                    if (currentLvl > maxLvl)
                    {
                        currentLvl = 1;
                    }
                    CreateLevel(currentLvl);
                }
            }
            oldState = state;

            
            // перемещение камеры
            if (state.IsKeyDown(Keys.Left))
            {
                player.Run(PlayerMove.Left);
                Scroll(-speedCamera, 0);
            }
            else if (state.IsKeyDown(Keys.Right))
            {
                player.Run(PlayerMove.Right);
                Scroll(speedCamera, 0);
            }
            else if (state.IsKeyDown(Keys.Up))
            {
                player.Run(PlayerMove.Up);
                Scroll(0, -speedCamera);
            }
            else if (state.IsKeyDown(Keys.Down))
            {
                player.Run(PlayerMove.Down);
                Scroll(0, speedCamera);
            }
            else 
                player.Stop();

            player.Update(gameTime);

            // обновляем охранников
            foreach (Guards guard in guards)
            {
                guard.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            // отрисовываем стены
            foreach (Block block in blocks)
            {
                block.Draw(spriteBatch);
            }

            try
            {
                spriteBatch.DrawString(font, guards[0].X.ToString(), new Vector2(10, 0), Color.Red);
                spriteBatch.DrawString(font, guards[0].Y.ToString(), new Vector2(10, 20), Color.Red);
            }
            catch (Exception e) { }

            spriteBatch.End();

            // отрисовываем охранников
            foreach (Guards guard in guards)
            {
                guard.Draw(spriteBatch);
            }

            // отрисовываем положение игрока
            player.Draw(spriteBatch);
            
            
            base.Draw(gameTime);
        }





        /*
         * Ф-ция создания уровня.
         * Считывает данные из файла.
         * Файл д.б. с названием "lvlX.txt";
         * 
         * @param {int} lvl - номер уровня
         * 
         */
        void CreateLevel(int lvl)
        {
            blocks = new List<Block>();
            guards = new List<Guards>();
            
            string lvl_name = "content/lvls/lvl" + Convert.ToString(lvl) + ".txt";
            string[] lines = File.ReadAllLines(lvl_name); //получили массив строк


            // проверим уровень сложности
            if (lvl == 4)
                complexity = Complexity.High;
            else
                complexity = Complexity.Low;

            // выделим память для карты уровня
            levelMap = new byte[lines[0].Length, lines.Length];

            // индексы для заполнения карты
            int indexI = 0;
            int indexJ = 0;
            
            int x = 0;
            int y = 0;

            
            foreach (string line in lines) //считали каждый символ в каждой строке
            {
                foreach (char c in line)
                {
                    //добавили стену, соответвующую данному символу в файле
                    Rectangle Rect = new Rectangle(x, y, size, size);
                    if (c == '0')
                    {
                        Block block = new Block(Rect, wallEmpty, this);
                        blocks.Add(block);
                    }
                    if (c == '1')
                    {
                        Block block = new Block(Rect, wallGoriz, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == '2')
                    {
                        Block block = new Block(Rect, wallVert, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == '3')
                    {
                        Block block = new Block(Rect, wallDownRight, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == '4')
                    {
                        Block block = new Block(Rect, wallUpRight, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == '5')
                    {
                        Block block = new Block(Rect, wallLeftDown, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == '6')
                    {
                        Block block = new Block(Rect, wallLeftUp, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == '7')
                    {
                        Block block = new Block(Rect, wall4sides, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == 'h')
                    {
                        Block block = new Block(Rect, wallURD, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == 'i')
                    {
                        Block block = new Block(Rect, wallRDL, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == 'j')
                    {
                        Block block = new Block(Rect, wallDLU, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == 'k')
                    {
                        Block block = new Block(Rect, wallLUR, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }


                    //двери
                    if (c == 'r')
                    {
                        Block block = new Block(Rect, doorHoriz, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == 's')
                    {
                        Block block = new Block(Rect, doorVertic, this);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (c == 't')
                    {
                        Block block = new Block(Rect, doorHorizOpen, this);
                        blocks.Add(block);
                    }
                    if (c == 'u')
                    {
                        Block block = new Block(Rect, doorVerticOpen, this);
                        blocks.Add(block);
                    }

                    if (c == 'o') { //буква "о"
                        //пол
                        Block block = new Block(Rect, wallEmpty, this);
                        blocks.Add(block);

                        // инициализируем нового охранника
                        Rectangle RectGuard = new Rectangle(x + sizePeople / 4, y + sizePeople / 4, sizePeople, sizePeople);
                        Guards guard = new Guards(guardIdleTexture, guardRunTexture, RectGuard, this);
                        guards.Add(guard);
                        guard.Run(PlayerMove.Left);
                    }

                    x += size;

                    indexI++;
                }

                x = 0;
                y += size;

                indexI = 0;
                indexJ++;
            
            }

            lenghtX = size * lines[0].Length; // длина уровня в пикселях
            lenghtY = y;

            Guards.SetLevelMap(levelMap, lenghtX / size, lenghtY / size);
        }

    }
}