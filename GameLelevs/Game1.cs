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
        Shadow shadow;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // хранилище данных
        Storage storage;

        XMLCoreMissionLoader xmlCoreMissionLoader;

        private List<string> toDraw;

        bool debugMode = false; // режим отладки. При нем включается вывод информации о объектах. Горячая клавиша D.
        
        
        int screenWidth = 600; // длина и высота экрана
        int screenHeight = 600;

        public int GetScreenWidth
        {
            get { return screenWidth; }
        }
        public int GetScreenHeight
        {
            get { return screenHeight; }
        }


        double someValue;

        //информация об уровнях
        int currentLvl;
        int maxLvl;
        KeyboardState oldState;

        //текстура инвентаря
        private Texture2D inventory;
        //кнопка меню в игре
        Button menuButton;
        Cursor cursor;
        Menu menu;
        GameState gameState = GameState.Menu;

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
            if (storage.IsExist(1))
            {
                levelLoader.CreateLevel(1);
                currentLvl = 1;
            }
            else
                levelLoader.CreateLevel(maxLvl);

            shadow = new Shadow(levelLoader.guards, levelLoader.objs, levelLoader.lasers, levelLoader.cameras, levelLoader.sysControls);
            Shadow.LevelLenghtX = LevelLoader.GetLenghtX / LevelLoader.Size;
            Shadow.LevelLenghtY = LevelLoader.GetLenghtY / LevelLoader.Size;
            shadow.ShowInRoom(LevelLoader.levelMapRooms[player.Position.X / LevelLoader.Size, player.Position.Y / LevelLoader.Size]);
            player.setShadow(shadow); // передадим игроку ссылку на на туман войны

            inventory = storage.Pull2DTexture("inventory");
            menuButton = new Button(new Vector2(screenWidth - 40, 0), storage.Pull2DTexture("menu"));
            cursor = new Cursor(storage.Pull2DTexture("cursor"));
            LoadMenu();

        }
        /// <summary>
        /// Добавляет кнопки в меню
        /// </summary>
        public void LoadMenu()
        {
            Button exitGame = new Button(storage.Pull2DTexture("exitbutton"));
            Button newGame = new Button(storage.Pull2DTexture("newgamebutton"));
            Button chooseGame = new Button(storage.Pull2DTexture("chooselevel"));
            exitGame.Click += new EventHandler(exitGame_Click);
            newGame.Click += new EventHandler(newGame_Click);
            menu.Items.Add(newGame);
            menu.Items.Add(chooseGame);
            menu.Items.Add(exitGame);
        }

        void newGame_Click(object sender, EventArgs e)
        {
            Button resumeGame = new Button(storage.Pull2DTexture("resumebutton"));
            Button retryGame = new Button(storage.Pull2DTexture("retrybutton"));
            resumeGame.Click += new EventHandler(resumeGame_Click);
            retryGame.Click += new EventHandler(retryGame_Click);
            menu.Items.RemoveAt(0);
            menu.Items.Insert(0, resumeGame);
            menu.Items.Insert(1, retryGame);
            gameState = GameState.Game;
        }

        void exitGame_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        void resumeGame_Click(object sender, EventArgs e)
        {
            gameState = GameState.Game;
        }

        void retryGame_Click(object sender, EventArgs e)
        {
            levelLoader.CreateLevel(currentLvl);
            shadow = new Shadow(levelLoader.guards, levelLoader.objs, levelLoader.lasers, levelLoader.cameras, levelLoader.sysControls);
            Shadow.LevelLenghtX = LevelLoader.GetLenghtX / LevelLoader.Size;
            Shadow.LevelLenghtY = LevelLoader.GetLenghtY / LevelLoader.Size;
            shadow.ShowInRoom(LevelLoader.levelMapRooms[player.Position.X / LevelLoader.Size, player.Position.Y / LevelLoader.Size]);
            player.setShadow(shadow);
            player.ClearBackpack();
            gameState = GameState.Game;
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
            else
                menu.Update();
            cursor.Update();
            base.Update(gameTime);
        }

        private void UpdateGame(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState state = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (menuButton.ButtonClick())
                gameState = GameState.Menu;

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

                if (Math.Sqrt(radiusX + radiusY) <= 30)
                {
                    // взаимодействие с ПУК
                    if (state.IsKeyDown(Keys.F))
                    {
                        sysControl.IsVisibleExample = true;
                        foreach (Cameras camera in levelLoader.cameras)
                        {
                            camera.IsActive = false;
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

            // вкл, выкл. туман войны
            if (state.IsKeyDown(Keys.S))
            {
                shadow.isShadow = !shadow.isShadow;
                shadow.HideOrShowAll(); // отображает или скрывание сущности на экране
            }

            //смена уровня по нажатию на пробел
            if (state.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
            {
                if (oldState != state)
                {
                    currentLvl++;
                    if (currentLvl > maxLvl)
                    {
                        currentLvl = 1;
                    }
                    levelLoader.CreateLevel(currentLvl);
                    shadow = new Shadow(levelLoader.guards, levelLoader.objs, levelLoader.lasers, levelLoader.cameras, levelLoader.sysControls);
                    Shadow.LevelLenghtX = LevelLoader.GetLenghtX / LevelLoader.Size;
                    Shadow.LevelLenghtY = LevelLoader.GetLenghtY / LevelLoader.Size;
                    shadow.ShowInRoom(LevelLoader.levelMapRooms[player.Position.X / LevelLoader.Size, player.Position.Y / LevelLoader.Size]);
                    player.setShadow(shadow); // передадим игроку ссылку на на туман войны
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

                            Texture2D openDoor = door.GetOrientation() == DoorOrientation.Horiz ? storage.Pull2DTexture("door_horiz_open") : storage.Pull2DTexture("door_vertic_open");

                            if (door.Open(openDoor))
                            {
                                toDraw.Add("The door was open");
                                levelLoader.levelMap[door.GetIndexI(), door.GetIndexJ()] = LevelObject.Empty;
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
            else
                menu.Draw(spriteBatch);
            spriteBatch.Begin();
            cursor.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
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
                    spriteBatch.DrawString(storage.PullFont("font"), "ShadowOfWar - " + shadow.isShadow, new Vector2(10, 400), Color.Orange); // тревога
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

                }


            }
            catch
            {
                // TODO: необходимо как-то обрабатывать исключения!
            }

            //spriteBatch.DrawString(storage.PullFont("font"), levelLoader.sysControls[0].GetGeneratedMathExample(), new Vector2(120, 120), Color.Red);


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





        

        

    }
}