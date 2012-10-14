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
        Camera camera;

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

        Texture2D keyTexture; // ключ
        Texture2D cardTexture; // пластиковая карта
        Texture2D goldTexture; // куча золота
        
        Texture2D tableWithCompSystemLU;  // стол с управлением камерами. LU - стол слева и сверху. Стул внизу, справа
        Texture2D tableWithCompSystemUR;
        Texture2D tableWithCompSystemRD; // DL - стол Снизу и справа. Стул сверху и слева
        Texture2D tableWithCompSystemDL;

        Texture2D tableWithCompU; // стол с компьюетером. Стол вверху, стул внизу
        Texture2D tableWithCompR;
        Texture2D tableWithCompD;
        Texture2D tableWithCompL;



        //объявляем текустуры охранников
        Texture2D guardIdleTexture;
        Texture2D guardRunTexture;

        SpriteFont font;

        
        //длина уровня в пикселях
        int lenghtX;
        int lenghtY;

        //получаем двину уровня в пикселях
        public int GetLenghtX
        {
            get { return lenghtX; }
            set { lenghtX = value; }
        }
        public int GetLenghtY
        {
            get { return lenghtY; }
            set { lenghtY = value; }
        }

        int screenWidth = 300; // длина и высота экрана
        int screenHeight = 300;

        public int GetScreenWidth
        {
            get { return screenWidth; }
        }
        public int GetScreenHeight
        {
            get { return screenHeight; }
        }
        
        //размер 1 ячейки уровня
        int size = 30;
        public int Size
        {
            get { return size; }
        }

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
        List<Object> objs; // объекты на уровне
        List<Guards> guards; // список охранников
        

        // карта уровня
        // так же используется для алгоритма поиска пути к игроку
        byte[,] levelMap;

        // сложность уровня
        Complexity complexity;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = screenWidth; //ширина и высота экрана
            this.graphics.PreferredBackBufferHeight = screenHeight;

            this.Components.Add(new FPSCounter(this));  // добавили игровой компонент - fps счетчик

            this.camera = new Camera();

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

            //загружаем текстуры объектов на уровне
            keyTexture = Content.Load<Texture2D>("Textures/objects/key");
            cardTexture = Content.Load<Texture2D>("Textures/objects/card");
            goldTexture = Content.Load<Texture2D>("Textures/objects/money");

            tableWithCompSystemLU = Content.Load<Texture2D>("Textures/objects/spLU");
            tableWithCompSystemUR = Content.Load<Texture2D>("Textures/objects/spUR");
            tableWithCompSystemRD = Content.Load<Texture2D>("Textures/objects/spRD");
            tableWithCompSystemDL = Content.Load<Texture2D>("Textures/objects/spDL");

            tableWithCompU = Content.Load<Texture2D>("Textures/objects/tableU");
            tableWithCompR = Content.Load<Texture2D>("Textures/objects/tableR");
            tableWithCompD = Content.Load<Texture2D>("Textures/objects/tableD");
            tableWithCompL = Content.Load<Texture2D>("Textures/objects/tableL");

            //загружаем текстуры для охранников
            guardIdleTexture = Content.Load<Texture2D>("players/player");
            guardRunTexture = Content.Load<Texture2D>("players/player_run");

            // инициализируем нового игрока
            Rectangle plaerPosition = new Rectangle(130, 130, sizePeople, sizePeople);
            player = new Player(Content.Load<Texture2D>("players/player"), Content.Load<Texture2D>("players/player_run"), plaerPosition, this, camera);

            font = Content.Load<SpriteFont>("myFont1");

            //сразу создаем первый уровень
            CreateLevel(4);

            // TODO: use this.Content to load your game content here
            // TODO: возможно, стоит написать универсальный загрузчик ресурсов. Сейчас код кажется громоздким и неудобным / не гибким
            //       interface ILoaderResources { 
            //                                    Load(string wayToLoad, type SpriteFont / Texture2D by default, ref or out whereLoad);
            //                                    GetResources(string name); 
            //                                   }
            //                                    **перегрузить метод Load для загрузки массива объектов
            //                                    **перегрузить метод Load для загрузки целой папки с ресурсами
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
            // координаты верхнего леового и нижнего правого угла игрока.
            int minx = rect.Left / size; // size - размер клетки
            int miny = rect.Top / size;
            int maxx = rect.Right / size;
            int maxy = rect.Bottom / size;

            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                    if (levelMap[i, j] == 1)
                        return true;
            }

            return false;
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
            }
            else if (state.IsKeyDown(Keys.Right))
            {
                player.Run(PlayerMove.Right);
            }
            else if (state.IsKeyDown(Keys.Up))
            {
                player.Run(PlayerMove.Up);
            }
            else if (state.IsKeyDown(Keys.Down))
            {
                player.Run(PlayerMove.Down);
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
            // отрисовываем объекты
            foreach (Object obj in objs)
            {
                obj.Draw(spriteBatch);
            }

            try
            {
                spriteBatch.DrawString(font, guards[0].X.ToString(), new Vector2(10, 0), Color.Red);
                spriteBatch.DrawString(font, guards[0].Y.ToString(), new Vector2(10, 20), Color.Red);
                spriteBatch.DrawString(font, lenghtX.ToString(), new Vector2(10, 40), Color.Red); // распечатка длины уровня по X
                spriteBatch.DrawString(font, lenghtY.ToString(), new Vector2(10, 60), Color.Red); // распечатка длины уровня по Y 
            }
            catch {
                // TODO: необходимо как-то обрабатывать исключения!
            }

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
            objs = new List<Object>();
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

            string[] str = {};

            foreach (string line in lines) //считали каждый символ в каждой строке
            {
                str = line.Split(' ');

                foreach (string s in str)
                {
                    
                    //s.Equals("0", StringComparison.OrdinalIgnoreCase) - ф-ция сравнения строки s и "0"

                    //добавили стену, соответвующую данному символу в файле
                    Rectangle Rect = new Rectangle(x, y, size, size);
                    if (s.Equals("0", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallEmpty, this, this.camera);
                        blocks.Add(block);
                    }
                    if (s.Equals("1", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallGoriz, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("2", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallVert, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("3", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallDownRight, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("4", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallUpRight, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("5", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallLeftDown, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("6", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallLeftUp, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("7", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wall4sides, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("8", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallURD, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("9", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallRDL, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("10", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallDLU, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("11", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, wallLUR, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }


                    //двери
                    if (s.Equals("20", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, doorHoriz, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("21", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, doorVertic, this, this.camera);
                        blocks.Add(block);

                        // добавим стену в карту
                        levelMap[indexI, indexJ] = 1;
                    }
                    if (s.Equals("22", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, doorHorizOpen, this, this.camera);
                        blocks.Add(block);
                    }
                    if (s.Equals("23", StringComparison.OrdinalIgnoreCase))
                    {
                        Block block = new Block(Rect, doorVerticOpen, this, this.camera);
                        blocks.Add(block);
                    }

                    if (s.Equals("30", StringComparison.OrdinalIgnoreCase))
                    { //буква "о"
                        //пол
                        Block block = new Block(Rect, wallEmpty, this, this.camera);
                        blocks.Add(block);

                        // инициализируем нового охранника
                        Rectangle RectGuard = new Rectangle(x + sizePeople / 4, y + sizePeople / 4, sizePeople, sizePeople);
                        Guards guard = new Guards(guardIdleTexture, guardRunTexture, RectGuard, this, player, this.camera);
                        guards.Add(guard);
                        guard.Run(PlayerMove.Left);
                    }

                    // ключ и пластиковая карта
                    if (s.Equals("40", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, keyTexture, this, this.camera);
                        objs.Add(obj);
                    }
                    if (s.Equals("41", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, cardTexture, this, this.camera);
                        objs.Add(obj);
                    }

                    // золото
                    if (s.Equals("50", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, goldTexture, this, this.camera);
                        objs.Add(obj);
                    }

                    // стол системы управления камерами
                    if (s.Equals("60", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, tableWithCompSystemLU, this, this.camera);
                        objs.Add(obj);
                    }
                    if (s.Equals("61", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, tableWithCompSystemUR, this, this.camera);
                        objs.Add(obj);
                    }
                    if (s.Equals("62", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, tableWithCompSystemRD, this, this.camera);
                        objs.Add(obj);
                    }
                    if (s.Equals("63", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, tableWithCompSystemDL, this, this.camera);
                        objs.Add(obj);
                    }

                    // стол с компьютером
                    if (s.Equals("70", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, tableWithCompU, this, this.camera);
                        objs.Add(obj);
                    }
                    if (s.Equals("71", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, tableWithCompR, this, this.camera);
                        objs.Add(obj);
                    }
                    if (s.Equals("72", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, tableWithCompD, this, this.camera);
                        objs.Add(obj);
                    }
                    if (s.Equals("73", StringComparison.OrdinalIgnoreCase))
                    {
                        Object obj = new Object(Rect, tableWithCompL, this, this.camera);
                        objs.Add(obj);
                    }

                    

                    x += size;

                    indexI++;
                }

                x = 0;
                y += size;

                indexI = 0;
                indexJ++;
            
            }

            lenghtX = size * str.Length; // длина уровня в пикселях
            lenghtY = y;

            Guards.SetLevelMap(levelMap, lenghtX / size, lenghtY / size);
        }

    }
}