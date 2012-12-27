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
using GameLevels.levelObjects.door;
using XMLContent;
using System.Text;

namespace GameLevels
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Player player;
        Camera camera;
        LevelLoader levelLoader;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // хранилище данных
        Storage storage;

        XMLCoreMissionLoader xmlCoreMissionLoader;

        private List<string> toDraw;

        bool debugMode = false; // режим отладки. При нем включается вывод информации о объектах. Горячая клавиша D.
        bool areYouCanGetAnswer = false; // можно ли ответить на пример
        bool isShowAdvice = false; //  показывается ли сейчас подсказка перед уровнем?

        public static int screenWidth = 600; // длина и высота экрана
        public static int screenHeight = 600;

        public int GetScreenWidth
        {
            get { return screenWidth; }
        }
        public int GetScreenHeight
        {
            get { return screenHeight; }
        }

        private float timerAdvice = 0; // таймер подсказки
        private float durationAdvice = 3000; // длительность

        double someValue;

        //информация об уровнях
        int currentLvl;
        int maxLvl;
        KeyboardState oldState;

        //текстура инвентаря
        private Texture2D inventory;
        //текстура для кнопки
        private Texture2D buttonTexture;
        //шрифт в меню
        private SpriteFont menuFont;
        //кнопка меню в игре
        Button menuButton;
        Cursor cursor;
        //главное меню
        Menu menu;
        //меню выбора уровня
        Menu menuLvl;
        GameState gameState = GameState.Menu;


        // подсказка перед уровнем
        Advice advice;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = screenWidth; //ширина и высота экрана
            this.graphics.PreferredBackBufferHeight = screenHeight;

            this.Components.Add(new FPSCounter(this));  // добавили игровой компонент - fps счетчик

            this.camera = new Camera();
            storage = new Storage();
            toDraw = new List<string>();
            
            // включили ограничение fps счетчика
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
            menu = new Menu(screenWidth);
            menuLvl = new Menu(screenWidth);
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
            storage.LoadTexture2DFolder("Content/advices");
            storage.PushFont("font", Content.Load<SpriteFont>("myFont1"));
            storage.PushFont("menufont", Content.Load<SpriteFont>("menufont"));

            // выбираем все возможные номера уровней
            storage.GetLevelNumbers();

            xmlCoreMissionLoader = Content.Load<XMLCoreMissionLoader>("lvls/tasks/mission_description1");
            foreach (XMLExpressionTarger st in xmlCoreMissionLoader.expressions) {
                toDraw.Add("Key point: " + st.point);
                toDraw.Add("Name: " + st.name + ", EXP: " + st.expression);
            }

            // инициализируем нового игрока
            Rectangle plaerPosition = new Rectangle(120, 120, LevelLoader.SizePeople, LevelLoader.SizePeople);
            player = new Player(storage.Pull2DTexture("player"), storage.Pull2DTexture("player_run"), storage.Pull2DTexture("player_run_goriz"), plaerPosition, this, camera);

            this.levelLoader = new LevelLoader(this, player, storage, camera);
            
            player.setLinkLevelLoader(levelLoader); // передадим игроку ссылку на загрузчик уровней

            maxLvl = storage.GetMaxLevelNumber();

            // стоит проверять существование уровня
            /*if (storage.IsExist(1))
            {
                levelLoader.CreateLevel(1);
                currentLvl = 1;
            }
            else
                levelLoader.CreateLevel(maxLvl);*/
            
            menuFont = storage.PullFont("menufont");
            inventory = storage.Pull2DTexture("inventory");
            menuButton = new Button(new Vector2(screenWidth - 40, 0), storage.Pull2DTexture("menu_active"), storage.PullFont("menufont"), "меню");
            menuButton.Click += new EventHandler(menuButton_Click);
            cursor = new Cursor(storage.Pull2DTexture("cursor"), storage.Pull2DTexture("pointer"));

            buttonTexture = storage.Pull2DTexture("button");
            LoadMenu();
            LoadLvlMenu();

        }

        /// <summary>
        /// Загрузка главного меню
        /// </summary>
        public void LoadMenu()
        {
            menu.font = menuFont;
            menu.LoadCursor(cursor);
            Button exitGame = new Button(buttonTexture, menuFont, "Выход");
            Button newGame = new Button(buttonTexture, menuFont, "Новая игра");
            Button chooseGame = new Button(buttonTexture, menuFont, "Выбрать уровень");
            exitGame.Click += new EventHandler(exitGame_Click);
            newGame.Click += new EventHandler(newGame_Click);
            chooseGame.Click += new EventHandler(chooseGame_Click);
            menu.Items.Add(newGame);
            menu.Items.Add(chooseGame);
            menu.Items.Add(exitGame);
        }
        /// <summary>
        /// Загрузка меню выбора уровня
        /// </summary>
        public void LoadLvlMenu()
        {
            menuLvl.LoadTextures(storage.Pull2DTexture("open"), storage.Pull2DTexture("close"), menuFont);
            menuLvl.LoadCursor(cursor);
            for (int i = 0; i < 10; i++)
            {
                Button button = new Button(buttonTexture, menuFont, "Уровень " + (i + 1));
                int num = i;
                button.Click += delegate(object sender, EventArgs e) { button_Click(sender, e, num); };
                menuLvl.Items.Add(button);
            }
            Button next = new Button(buttonTexture, menuFont, "вперед");
            Button previous = new Button(buttonTexture, menuFont, "назад");
            Button back = new Button(buttonTexture, menuFont, "в меню");
            back.Click += new EventHandler(back_Click);
            previous.Click += new EventHandler(previous_Click);
            next.Click += new EventHandler(next_Click);
            menuLvl.next = next;
            menuLvl.previous = previous;
            menuLvl.back = back;
        }
        /// <summary>
        /// Обработка нажатия на "новая игра"
        /// </summary>
        void newGame_Click(object sender, EventArgs e)
        {
            Button resumeGame = new Button(buttonTexture, storage.PullFont("menufont"), "Продолжить");
            Button retryGame = new Button(buttonTexture, storage.PullFont("menufont"), "Начать заново");
            resumeGame.Click += new EventHandler(resumeGame_Click);
            retryGame.Click += new EventHandler(retryGame_Click);
            menu.Items.RemoveAt(0);
            menu.Items.Insert(0, resumeGame);
            menu.Items.Insert(1, retryGame);
            gameState = GameState.Advice;
            currentLvl = 1;
            PrintAdvice(currentLvl);
        }
        /// <summary>
        /// Обработка нажатия на "выход"
        /// </summary>
        void exitGame_Click(object sender, EventArgs e)
        {
            this.Exit();
        }
        /// <summary>
        /// Обработка нажатия на "продолжить"
        /// </summary>
        void resumeGame_Click(object sender, EventArgs e)
        {
            gameState = GameState.Game;
        }
        /// <summary>
        /// Обработка нажатия на "начать заново"
        /// </summary>
        void retryGame_Click(object sender, EventArgs e)
        {
            levelLoader.CreateLevel(currentLvl);
            player.ClearBackpack();
            gameState = GameState.Game;
        }
        /// <summary>
        /// Обработка нажатия на "выбрать уровень"
        /// </summary>
        void chooseGame_Click(object sender, EventArgs e)
        {
            gameState = GameState.LvlMenu;
        }
        /// <summary>
        /// Обработка нажатия на "меню"
        /// </summary>
        void menuButton_Click(object sender, EventArgs e)
        {
            gameState = GameState.Menu;
        }
        /// <summary>
        /// Обработка нажатия на "вперед"
        /// </summary>
        void next_Click(object sender, EventArgs e)
        {
            menuLvl.page++;
        }
        /// <summary>
        /// Обработка нажатия на "назад"
        /// </summary>
        void previous_Click(object sender, EventArgs e)
        {
            if (menuLvl.page > 0)
                menuLvl.page--;
        }
        /// <summary>
        /// Обработка нажатия на "в меню"
        /// </summary>
        void back_Click(object sender, EventArgs e)
        {
            gameState = GameState.Menu;
        }
        /// <summary>
        /// Обработка нажатия на "уровень"
        /// </summary>
        void button_Click(object sender, EventArgs e, int num)
        {
            if (menuLvl.IsLvlFinished(num-1))
            {
                currentLvl = num+1;
                levelLoader.CreateLevel(currentLvl);
                gameState = GameState.Game;
            }
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

            switch (levelLoader.complexity) 
            {
                case Complexity.Low :
                    collides = CollidesLow(rect);
                    break;
                case Complexity.Medium:
                    collides = CollidesHigh(rect);
                    break;
                case Complexity.High:
                    collides = CollidesHigh(rect);
                    CollidesLasers(rect);
                    CollidesCameras(rect);
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
            foreach (Block block in levelLoader.blocks)
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
            int minx = (rect.Left + LevelLoader.SizePeople / 2 - 1) / LevelLoader.Size; // size - размер клетки
            int miny = (rect.Top + LevelLoader.SizePeople / 2 - 1) / LevelLoader.Size;
            int maxx = (rect.Right - LevelLoader.SizePeople / 2 + 1) / LevelLoader.Size;
            int maxy = (rect.Bottom - LevelLoader.SizePeople / 2 + 1) / LevelLoader.Size;

            /*int currentI = (rect.Left + LevelLoader.SizePeople / 2)  / LevelLoader.Size;
            int currentJ = (rect.Top + LevelLoader.SizePeople / 2)  / LevelLoader.Size;


            if (levelLoader.levelMap[currentI, currentJ] >= 20 && levelLoader.levelMap[currentI, currentJ] <= 23)
            {
                minx = (rect.Left) / LevelLoader.Size; // size - размер клетки
                miny = (rect.Top) / LevelLoader.Size;
                maxx = (rect.Right) / LevelLoader.Size;
                maxy = (rect.Bottom) / LevelLoader.Size;
            }*/


            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                    if (levelLoader.levelMap[i, j] == LevelObject.Wall)
                        return true;
            }

            return false;
        }


        /// <summary>
        /// Пересечения с лазерами
        /// </summary>
        /// <param name="rect">прямоугольник игрока</param>
        void CollidesLasers(Rectangle rect)
        {
            int centerX = rect.Left + LevelLoader.SizePeople / 2;
            int centerY = rect.Top + LevelLoader.SizePeople / 2;

            foreach (Laser laser in levelLoader.lasers)
            {
                if (laser.IsActive)
                {
                    if (laser.typeLaser == LevelObject.LaserVertic || laser.typeLaser == LevelObject.LaserVerticMoving)
                    {
                        if (Math.Abs(laser.Rect.X + 20 - centerX) < 10 && Math.Abs(laser.Rect.Y + 30 - centerY) < 30)
                        {
                            Guards.generalAlarm = true;
                            changeAlarmGuards();
                        }
                    }
                    if (laser.typeLaser == LevelObject.LaserHoriz || laser.typeLaser == LevelObject.LaserHorizMoving)
                    {
                        if (Math.Abs(laser.Rect.X + 30 - centerX) < 30 && Math.Abs(laser.Rect.Y + 20 - centerY) < 10)
                        {
                            Guards.generalAlarm = true;
                            changeAlarmGuards();
                        }
                    }
                }
            }

        }


        /// <summary>
        /// пересечение с камерами слежения
        /// </summary>
        /// <param name="rect">прямоугольник игрока</param>
        void CollidesCameras(Rectangle rect)
        {
            int centerX = rect.Left + LevelLoader.SizePeople / 2;
            int centerY = rect.Top + LevelLoader.SizePeople / 2;

            foreach (Cameras camera in levelLoader.cameras)
            {
                if (camera.IsActive)
                {
                    if (camera.typeCamera == LevelObject.CameraUL)
                    {
                        if (centerX >= camera.Rect.X && centerY >= camera.Rect.Y)
                        {
                            double radiusX = Math.Pow(Math.Abs(camera.Rect.X - centerX), 2);
                            double radiusY = Math.Pow(Math.Abs(camera.Rect.Y - centerY), 2);

                            if (Math.Sqrt(radiusX + radiusY) <= 100)
                            {
                                
                                Guards.generalAlarm = true;
                                changeAlarmGuards();
                                this.someValue++;
                            }
                        }
                    }
                    if (camera.typeCamera == LevelObject.CameraUR)
                    {
                        if (centerX <= camera.Rect.X + 90 && centerY >= camera.Rect.Y)
                        {
                            double radiusX = Math.Pow(Math.Abs(camera.Rect.X + 90 - centerX), 2);
                            double radiusY = Math.Pow(Math.Abs(camera.Rect.Y - centerY), 2);

                            if (Math.Sqrt(radiusX + radiusY) <= 100)
                            {
                                Guards.generalAlarm = true;
                                changeAlarmGuards();
                                this.someValue++;
                            }
                        }
                    }
                }
            }

        }
        

        /// <summary>
        /// Изменяем встревоженность каждого охранника в зависимости от общей тревоги
        /// </summary>
        private void changeAlarmGuards() 
        {
            if (Guards.generalAlarm)
            {
                foreach (Guards guard in levelLoader.guards)
                {
                    guard.Alarm = true;
                }
            }
            else
            {
                foreach (Guards guard in levelLoader.guards)
                {
                    guard.Alarm = false;
                }
            }
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.Game)
                UpdateGame(gameTime);
            else if (gameState == GameState.Menu)
                menu.Update();
            else if (gameState == GameState.Advice)
                UpdateGameAdvice(gameTime);
            else
                menuLvl.Update();
            
            base.Update(gameTime);
        }


        /// <summary>
        /// Функция отрисовки подсказки перед уровнем. После истечения времени - загружается новый уровень и начинается стандартный цикл
        /// </summary>
        /// <param name="gameTime">Время</param>
        private void UpdateGameAdvice(GameTime gameTime)
        {
            timerAdvice += gameTime.ElapsedGameTime.Milliseconds;
            if (timerAdvice > durationAdvice)
            {
                timerAdvice = 0;
                gameState = GameState.Game;
                levelLoader.CreateLevel(currentLvl);  // загружаем лвл
            }
        }

        private void UpdateGame(GameTime gameTime)
        {
            cursor.Update();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState state = Keyboard.GetState();

            //нажатие на меню
            if (menuButton.ButtonClick(cursor.State, cursor.OldState))
                menuButton.OnClick();

            //изменить видимость игрока по кнопке V
            if (state.IsKeyDown(Keys.V) && oldState.IsKeyUp(Keys.V))
            {
                if (!player.IsVisible())
                    player.SetVisible();
                else
                    player.SetInvisible();
            }

            if (state.IsKeyDown(Keys.D))
            {
                debugMode = !debugMode;
            }

            // вкл, выкл. тревогу
            if (state.IsKeyDown(Keys.A))
            {
                if (player.IsVisible())
                {
                    if (Guards.generalAlarm) // анализируем всеобщую тревогу
                    {
                        foreach (Guards guard in levelLoader.guards)
                        {
                            guard.Alarm = false;
                        }
                        Guards.generalAlarm = false;
                    }
                    else
                    {
                        foreach (Guards guard in levelLoader.guards)
                        {
                            guard.Alarm = true;
                        }
                        Guards.generalAlarm = true;
                    }
                }
            }

            // вкл, выкл. камеры
            if (state.IsKeyDown(Keys.C))
            {
                foreach (Cameras camera in levelLoader.cameras)
                {
                    camera.changeActiveCamera();
                }
            }


            foreach (SysControl sysControl in levelLoader.sysControls)
            {
                double radiusX = Math.Pow((player.Position.Center.X - sysControl.Rect.Center.X), 2);
                double radiusY = Math.Pow((player.Position.Center.Y - sysControl.Rect.Center.Y), 2);

                if (Math.Sqrt(radiusX + radiusY) <= 30 && sysControl.isSysControlHacked == false)
                {
                    
                    // взаимодействие с ПУК
                    if (state.IsKeyDown(Keys.F))
                    {

                        sysControl.IsVisibleExample = true;
                        areYouCanGetAnswer = true;
                        
                    }
                    if (areYouCanGetAnswer)
                    {
                        // вводим число и делаем проверку правильности
                        KeyboardState state2 = Keyboard.GetState();
                        int answerNumber = -1;

                        if (state2.IsKeyDown(Keys.D0)) { answerNumber = 0; }
                        if (state2.IsKeyDown(Keys.D1)) { answerNumber = 1; }
                        if (state2.IsKeyDown(Keys.D2)) { answerNumber = 2; }
                        if (state2.IsKeyDown(Keys.D3)) { answerNumber = 3; }
                        if (state2.IsKeyDown(Keys.D4)) { answerNumber = 4; }
                        if (state2.IsKeyDown(Keys.D5)) { answerNumber = 5; }
                        if (state2.IsKeyDown(Keys.D6)) { answerNumber = 6; }
                        if (state2.IsKeyDown(Keys.D7)) { answerNumber = 7; }
                        if (state2.IsKeyDown(Keys.D8)) { answerNumber = 8; }
                        if (state2.IsKeyDown(Keys.D9)) { answerNumber = 9; }
                        
                        if (answerNumber == sysControl.RightAnswer) 
                        {
                            // если правильно - отключаем камеры
                            foreach (Cameras camera in levelLoader.cameras)
                            {
                                if (sysControl.targetCameraX == camera.posX && sysControl.targetCameraY == camera.posY)
                                {
                                    camera.IsActive = false;
                                    sysControl.isSysControlHacked = true; // взломано
                                }
                            }
                        }
                        
            
                        
                    }
                }
                else
                {
                    //все ПУК-и, от которых мы далеко - отключают примеры
                    sysControl.IsVisibleExample = false;
                }
                //
                //camera.changeActiveCamera();
            }

            //смена уровня по нажатию на пробел
            if (state.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
            {
                SaveLvlInfo();
                if (oldState != state)
                {
                    currentLvl++;
                    if (currentLvl > maxLvl)
                    {
                        currentLvl = 1;
                    }

                    PrintAdvice(currentLvl);
                    //levelLoader.CreateLevel(currentLvl);
                    player.ClearBackpack();
                    toDraw.Clear();
                }
            }
            oldState = state;

            

            // открываем дверь
            if (state.IsKeyDown(Keys.E))
            {
                int j = 0;
                while (j < levelLoader.doors.Count)
                {
                    // если игрок пересекается с каким-либо "интерактивным" объектом на уровне
                    if (levelLoader.doors[j].Rect.Intersects(player.Position))
                    {
                        Door door = (Door)levelLoader.doors[j];

                        if (door.IsClosed())
                        {
                            // анализируем рюкзак
                            foreach (BaseObject obj in player.backpack)
                            {
                                // смотрим, подходят ли наши ключи к двери  
                                if (obj is Key || obj is Card) {
                                    if (obj.targetDoorX == door.posX && obj.targetDoorY == door.posY)
                                    {
                                        // открываем дверь, присваиваем нужную текстуру открытой двери
                                        Texture2D openDoor = door.GetOrientation() == DoorOrientation.Horiz ? storage.Pull2DTexture("door_horiz_open") : storage.Pull2DTexture("door_vertic_open");
                                        if (door.GetOrientation() == DoorOrientation.HorizWood) 
                                        { 
                                            openDoor = storage.Pull2DTexture("wood_door_horiz_open");
                                        }
                                        else {
                                            if (door.GetOrientation() == DoorOrientation.VertWood)
                                            { 
                                                openDoor = storage.Pull2DTexture("wood_door_vertic_open");
                                            }
                                        }

                                        if (door.Open(openDoor))
                                        {
                                            toDraw.Add("The door was open");
                                            levelLoader.levelMap[door.GetIndexI(), door.GetIndexJ()] = LevelObject.Empty;
                                        }
                                    }
                                }
                            }


                            
                        }

                    }

                    j++;
                }
            }




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
            foreach (Guards guard in levelLoader.guards)
            {
                guard.Update(gameTime);
            }
            // обновляем лазеры
            foreach (Laser laser in levelLoader.lasers)
            {
                laser.Update(gameTime);
            }

            // обновляем лазеры
            foreach (SysControl sysControl in levelLoader.sysControls)
            {
                sysControl.Update(gameTime);
            }


            int i = 0;
            while (i < levelLoader.interactionSubjects.Count)
            {
                // если игрок пересекается с каким-либо "интерактивным" объектом на уровне
                if (levelLoader.interactionSubjects[i].Rect.Intersects(player.Position))
                {
                    // если удалось добавить объект в рюкзак
                    if (player.AddItem(levelLoader.interactionSubjects[i]))
                    {

                        levelLoader.blocks.Add(new Block(levelLoader.interactionSubjects[i].Rect, storage.Pull2DTexture("empty"), this, this.camera));

                        // если объект это карта
                        if (levelLoader.interactionSubjects[i] is Card)
                            toDraw.Add("Add a map to a list of items!");

                        if (levelLoader.interactionSubjects[i] is Key)
                            toDraw.Add("Add a key to a list of items!");

                        if (levelLoader.interactionSubjects[i] is Money)
                            toDraw.Add("Add coin worth " + ((Money)levelLoader.interactionSubjects[i]).Cost);

                        levelLoader.interactionSubjects.RemoveAt(i);
                    }
                    else
                        toDraw.Add("Too much items in backpack");
                }
                else i++;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (gameState == GameState.Game)
                DrawGame();
            else if (gameState == GameState.Menu)
                menu.Draw(spriteBatch);
            else if (gameState == GameState.Advice)
                DrawAdvice();
            else
                menuLvl.Draw(spriteBatch);

            
            base.Draw(gameTime);
        }

        private void DrawAdvice()
        {
            spriteBatch.Begin();
            advice.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawGame()
        {
            // TODO: Add your drawing code here
            spriteBatch.Begin();


            // отрисовываем стены
            foreach (Block block in levelLoader.blocks)
            {
                block.Draw(spriteBatch);
            }
            foreach (Laser laser in levelLoader.lasers)
            {
                if (laser.isVisible)
                {
                    laser.Draw(spriteBatch);
                }
            }

            foreach (BaseObject bo in levelLoader.interactionSubjects)
                bo.Draw(spriteBatch);

            foreach (BaseObject bo in levelLoader.doors)
                bo.Draw(spriteBatch);

            try
            {
                if (debugMode)
                {
                    // подсказки управления
                    spriteBatch.DrawString(storage.PullFont("font"), "S - shadow", new Vector2(400, 40), Color.LimeGreen);
                    spriteBatch.DrawString(storage.PullFont("font"), "Arrows - control", new Vector2(400, 60), Color.LimeGreen);
                    spriteBatch.DrawString(storage.PullFont("font"), "Space - chage level", new Vector2(400, 80), Color.LimeGreen);
                    spriteBatch.DrawString(storage.PullFont("font"), "C - turn off, on cameras", new Vector2(400, 100), Color.LimeGreen);

                    // инфа о уровне
                    spriteBatch.DrawString(storage.PullFont("font"), "LevelLenght = " + LevelLoader.GetLenghtX.ToString(), new Vector2(10, 40), Color.Orange); // распечатка длины уровня по X
                    spriteBatch.DrawString(storage.PullFont("font"), "LevelHeight = " + LevelLoader.GetLenghtY.ToString(), new Vector2(10, 60), Color.Orange); // распечатка длины уровня по Y 

                    // все для игрока
                    spriteBatch.DrawString(storage.PullFont("font"), "CurrentRoom - " + player.room, new Vector2(10, 440), Color.Orange); // комната
                    spriteBatch.DrawString(storage.PullFont("font"), "MyPosX - " + player.NewPosX.ToString(), new Vector2(10, 330), Color.Orange); // распечатка клетки для следующего хода охранника
                    spriteBatch.DrawString(storage.PullFont("font"), "MyPosY - " + player.NewPosY.ToString(), new Vector2(10, 350), Color.Orange); // распечатка клетки для следующего хода охранника
                    spriteBatch.DrawString(storage.PullFont("font"), "CamerasIsActive - " + levelLoader.cameras[0].IsActive, new Vector2(10, 420), Color.Orange); // тревога

                    spriteBatch.DrawString(storage.PullFont("font"), "SomeValue - " + this.someValue, new Vector2(10, 500), Color.Yellow);

                    // все для охранника 0
                    spriteBatch.DrawString(storage.PullFont("font"), "PosGuard[0].X = " + levelLoader.guards[0].X.ToString(), new Vector2(10, 0), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "PosGuard[0].Y = " + levelLoader.guards[0].Y.ToString(), new Vector2(10, 20), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "NextStepGuardX - " + levelLoader.guards[0].NextX.ToString(), new Vector2(10, 250), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "NextStepGuardY - " + levelLoader.guards[0].NextY.ToString(), new Vector2(10, 270), Color.Orange); // распечатка клетки для следующего хода охранника
                    spriteBatch.DrawString(storage.PullFont("font"), "Alarm - " + levelLoader.guards[0].Alarm, new Vector2(10, 380), Color.Orange); // тревога

                    spriteBatch.DrawString(storage.PullFont("font"), "LaserX - " + Convert.ToInt32(levelLoader.lasers[0].Rect.X + LevelLoader.Size / 2), new Vector2(10, 460), Color.Orange); // координата Х лазера
                    spriteBatch.DrawString(storage.PullFont("font"), "LaserY - " + Convert.ToInt32(levelLoader.lasers[0].Rect.Y + LevelLoader.Size / 2), new Vector2(10, 480), Color.Orange);




                }
                else
                {
                    spriteBatch.DrawString(storage.PullFont("font"), "D - debug", new Vector2(400, 20), Color.LimeGreen); // тревога

                    int xCoord = 40;
                    foreach (string str in toDraw)
                    {
                        spriteBatch.DrawString(storage.PullFont("font"), str, new Vector2(300, xCoord), Color.Red);
                        xCoord += 20;
                    }


                    spriteBatch.DrawString(storage.PullFont("font"), levelLoader.sysControls[0].GetGeneratedMathExample(), new Vector2(120, 120), Color.Red); // печать примера на экране


                }


            }
            catch
            {
                // TODO: необходимо как-то обрабатывать исключения!
            }

            

            // отрисовываем объекты
            foreach (Object obj in levelLoader.objs)
            {
                if (obj.isVisible)
                {
                    obj.Draw(spriteBatch);
                }
            }
            // отрисовываем объекты
            foreach (SysControl sysControl in levelLoader.sysControls)
            {
                if (sysControl.isVisible)
                {
                    sysControl.Draw(spriteBatch);
                }
            }

            //рисуется кнопка "меню"
            menuButton.Draw(spriteBatch);
            //рисуется инвентарь
            spriteBatch.Draw(inventory, new Rectangle(screenWidth - inventory.Width, inventory.Width, inventory.Width, inventory.Height), Color.White);
            //рисуется курсор
            if (menuButton.Hover(cursor.State))
                cursor.DrawPointer(spriteBatch);
            else
                cursor.Draw(spriteBatch);
            spriteBatch.End();


            // отрисовываем охранников
            foreach (Guards guard in levelLoader.guards)
            {
                if (guard.isVisible)
                {
                    guard.Draw(spriteBatch);
                }
            }


            // отрисовываем положение игрока
            player.Draw(spriteBatch);

            spriteBatch.Begin();
            // отрисовываем камеры
            foreach (Cameras camera in levelLoader.cameras)
            {
                if (camera.isVisible)
                {
                    camera.Draw(spriteBatch);
                }
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Сохраняет информацию о пройденных уровнях
        /// </summary>
        public void SaveLvlInfo()
        {
            string[] info = File.ReadAllLines("content/lvl_info.txt");
            info[currentLvl - 1] = currentLvl + " " + "1";
            File.WriteAllLines("content/lvl_info.txt", info);
        }


        public void PrintAdvice(int currentLvl)
        {
            gameState = GameState.Advice;
            Texture2D imgAdvice = storage.Pull2DTexture("advice" + currentLvl);
            //Texture2D imgAdvice = storage.Pull2DTexture("advice");
            advice = new Advice(imgAdvice);
        }
        

        

    }
}