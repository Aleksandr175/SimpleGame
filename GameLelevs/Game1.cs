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
using Enumerations;
using GameLevels.levelObjects;

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

        // хранилище данных
        Storage storage;
        
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

        int screenWidth = 900; // длина и высота экрана
        int screenHeight = 600;

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
        int maxLvl = 5;
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
            storage = new Storage();

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

            // настройка хранилища
            storage.TexturesLoader = Content.Load<Texture2D>;
            storage.FontLoader = Content.Load<SpriteFont>;

            // загрузка всех текстур из заданной папки
            // !!! пока, обязательно указывать CONTENT/
            storage.LoadTexture2DFolder("Content/Textures");
            storage.LoadTexture2DFolder("Content/players");
            storage.PushFont("font", Content.Load<SpriteFont>("myFont1"));

            // инициализируем нового игрока
            Rectangle plaerPosition = new Rectangle(120, 120, sizePeople, sizePeople);
            player = new Player(storage.Pull2DTexture("player"), storage.Pull2DTexture("player_run"), storage.Pull2DTexture("player_run_goriz"), plaerPosition, this, camera);

            //сразу создаем первый уровень
            CreateLevel(5);

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

            
            // перемещение игрока
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
                spriteBatch.DrawString(storage.PullFont("font"), guards[0].X.ToString(), new Vector2(10, 0), Color.Red);
                spriteBatch.DrawString(storage.PullFont("font"), guards[0].Y.ToString(), new Vector2(10, 20), Color.Red);
                spriteBatch.DrawString(storage.PullFont("font"), lenghtX.ToString(), new Vector2(10, 40), Color.Red); // распечатка длины уровня по X
                spriteBatch.DrawString(storage.PullFont("font"), lenghtY.ToString(), new Vector2(10, 60), Color.Red); // распечатка длины уровня по Y 
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





        /// <summary>
        /// Ф-ция создания уровня.
        /// Считывает данные из файла.
        /// Файл д.б. с названием "lvlX.txt";
        /// </summary>
        /// <param name="lvl">номер уровня</param>
        void CreateLevel(int lvl)
        {
            blocks = new List<Block>();
            objs = new List<Object>();
            guards = new List<Guards>();
            
            string lvl_name = "content/lvls/lvl" + Convert.ToString(lvl) + ".txt";
            string[] lines = File.ReadAllLines(lvl_name); //получили массив строк

            // проверим уровень сложности
            //if (lvl == 4)
                complexity = Complexity.High;
            //else
            //    complexity = Complexity.Low;

            // индексы для заполнения карты
            int indexI = 0;
            int indexJ = 0;
            
            int x = 0;
            int y = 0;

            string[] str = {};

            int tempIndex = 0;
            int[] sizeFile = new int[2];

            

            // тестовый режим...
            // ЕСЛИ УРОВЕНЬ ИЗ РЕДАКТОРА, ТО...
            //приспособим загрузку уровней, сделанных в редакторе
            if (lvl == 5)
            {

                lines = File.ReadAllLines(lvl_name); //получили массив строк                
                //считываем размеры массива с уровнем (0 значение - строки, 1 - колонки)
                foreach (string line in lines)
                {
                    str = line.Split(' ');
                    foreach (string s in str)
                    {
                        sizeFile[tempIndex] = Convert.ToInt32(s);
                        tempIndex++;
                        if (tempIndex == 2) { break; }
                    }
                    break;
                }

                // выделим память для карты уровня
                levelMap = new byte[sizeFile[0] + 1, sizeFile[1] + 1];


                tempIndex = 0;
                //считывание уровня из файла
                foreach (string line in lines)
                {

                    if (tempIndex == 0) { tempIndex++;  continue; } // пропускаем первую строку с данными размера уровня

                    str = line.Split(' ');
                    foreach (string s in str)
                    {
                        levelMap[indexI, indexJ] = 0;

                        if (s.Equals("0", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 0;
                        }
                        if (s.Equals("1", StringComparison.OrdinalIgnoreCase))
                        {
                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }



                        //двери
                        if (s.Equals("20", StringComparison.OrdinalIgnoreCase))
                        {
                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 20;
                        }
                        if (s.Equals("21", StringComparison.OrdinalIgnoreCase))
                        {
                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 21;
                        }
                        if (s.Equals("22", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 22;
                        }
                        if (s.Equals("23", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 23;
                        }

                        if (s.Equals("30", StringComparison.OrdinalIgnoreCase))
                        { //буква "о"
                            levelMap[indexI, indexJ] = 30;
                        }

                        // ключ и пластиковая карта
                        if (s.Equals("40", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 40;
                        }
                        if (s.Equals("41", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 41;
                        }

                        // золото
                        if (s.Equals("50", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 50;
                        }

                        // стол системы управления камерами
                        if (s.Equals("60", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 60;
                        }
                        if (s.Equals("61", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 61;
                        }
                        if (s.Equals("62", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 62;
                        }
                        if (s.Equals("63", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 63;
                        }

                        // стол с компьютером
                        if (s.Equals("70", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 70;
                        }
                        if (s.Equals("71", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 71;
                        }
                        if (s.Equals("72", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 72;
                        }
                        if (s.Equals("73", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMap[indexI, indexJ] = 73;
                        }


                        indexI++;

                    }
                    indexI = 0;
                    indexJ++;

                }


                // преобразование значений объектов (стен) на уровне
                for (int i = 0; i <= sizeFile[0]; i++)
                {
                    for (int j = 0; j <= sizeFile[1]; j++)
                    {

                    }
                }



                // создание объектов на уровне
                for (int i = 0; i <= sizeFile[0]; i++)
                {
                    for (int j = 0; j <= sizeFile[1]; j++)
                    {
                        Rectangle Rect = new Rectangle(i * size, j * size, size, size);
                        if (levelMap[i, j] == 0)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), this, this.camera);
                            blocks.Add(block);
                        } 
                        if (levelMap[i, j] == 1)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_goriz"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 2)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_vertic"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 3)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_down_right"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 4)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_up_right"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 5)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_down"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 6)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_up"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 7)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_4sides"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 8)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_urd"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 9)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_rdl"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 10)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_dlu"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 11)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_lur"), this, this.camera);
                            blocks.Add(block);
                        }


                        //двери
                        if (levelMap[i, j] == 20)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 21)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 22)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz_open"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 23)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic_open"), this, this.camera);
                            blocks.Add(block);
                        }

                        if (levelMap[i, j] == 30)
                        { //буква "о"
                            //пол
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), this, this.camera);
                            blocks.Add(block);

                            // инициализируем нового охранника
                            Rectangle RectGuard = new Rectangle(x + sizePeople / 4, y + sizePeople / 4, sizePeople, sizePeople);
                            Guards guard = new Guards(storage.Pull2DTexture("player"), storage.Pull2DTexture("player_run"), storage.Pull2DTexture("player_run_goriz"), RectGuard, this, player, this.camera);
                            guards.Add(guard);
                            guard.Run(PlayerMove.Left);
                        }

                        // ключ и пластиковая карта
                        if (levelMap[i, j] == 40)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("key"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 41)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("card"), this, this.camera);
                            objs.Add(obj);
                        }

                        // золото
                        if (levelMap[i, j] == 50)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("money"), this, this.camera);
                            objs.Add(obj);
                        }

                        // стол системы управления камерами
                        if (levelMap[i, j] == 60)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spLU"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 61)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spUR"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 62)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spRD"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 63)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spDL"), this, this.camera);
                            objs.Add(obj);
                        }

                        // стол с компьютером
                        if (levelMap[i, j] == 70)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableU"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 71)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableR"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 72)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableD"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 73)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableL"), this, this.camera);
                            objs.Add(obj);
                        }
                    }
                }
                // конец создание объектов на уровне

                lenghtX = size * sizeFile[1]; // длина уровня в пикселях
                lenghtY = size * sizeFile[0];
                

            }




            // ЕСЛИ УРОВЕНЬ НЕ ИЗ РЕДАКТОРА, ТО ОБЫЧНАЯ ЗАГРУЗКА
            else
            {
                // выделим память для карты уровня
                levelMap = new byte[lines[0].Length, lines.Length];


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
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (s.Equals("1", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_goriz"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("2", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_vert"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("3", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_down_right"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("4", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_up_right"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("5", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_down"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("6", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_up"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("7", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_4sides"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("8", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_urd"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("9", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_rdl"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("10", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_dlu"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("11", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_lur"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }


                        //двери
                        if (s.Equals("20", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("21", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic"), this, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("22", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz_open"), this, this.camera);
                            blocks.Add(block);
                        }
                        if (s.Equals("23", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic_open"), this, this.camera);
                            blocks.Add(block);
                        }

                        if (s.Equals("30", StringComparison.OrdinalIgnoreCase))
                        { //буква "о"
                            //пол
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), this, this.camera);
                            blocks.Add(block);

                            // инициализируем нового охранника
                            Rectangle RectGuard = new Rectangle(x + sizePeople / 4, y + sizePeople / 4, sizePeople, sizePeople);
                            Guards guard = new Guards(storage.Pull2DTexture("player"), storage.Pull2DTexture("player_run"), storage.Pull2DTexture("player_run_goriz"), RectGuard, this, player, this.camera);
                            guards.Add(guard);
                            guard.Run(PlayerMove.Left);
                        }

                        // ключ и пластиковая карта
                        if (s.Equals("40", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("key"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("41", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("card"), this, this.camera);
                            objs.Add(obj);
                        }

                        // золото
                        if (s.Equals("50", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("money"), this, this.camera);
                            objs.Add(obj);
                        }

                        // стол системы управления камерами
                        if (s.Equals("60", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spLU"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("61", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spUR"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("62", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spRD"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("63", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spDL"), this, this.camera);
                            objs.Add(obj);
                        }

                        // стол с компьютером
                        if (s.Equals("70", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableU"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("71", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableR"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("72", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableD"), this, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("73", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableL"), this, this.camera);
                            objs.Add(obj);
                        }



                        x += size;

                        indexI++;
                    }

                    x = 0;
                    y += size;

                    indexI = 0;
                    indexJ++;

                } //end foreach

                lenghtX = size * str.Length; // длина уровня в пикселях
                lenghtY = y;

                Guards.SetLevelMap(levelMap, lenghtX / size, lenghtY / size);
            }


        }

    }
}