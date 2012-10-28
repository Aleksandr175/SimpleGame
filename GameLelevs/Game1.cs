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
        LevelLoader levelLoader;
        Shadow shadow;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // хранилище данных
        Storage storage;


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
        
        

        //информация об уровнях
        int currentLvl;
        int maxLvl = 6;
        KeyboardState oldState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = screenWidth; //ширина и высота экрана
            this.graphics.PreferredBackBufferHeight = screenHeight;

            this.Components.Add(new FPSCounter(this));  // добавили игровой компонент - fps счетчик

            this.camera = new Camera();
            storage = new Storage();

            shadow = new Shadow();

            
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
            
            // выбираем все возможные номера уровней
            storage.GetLevelNumbers();

            // инициализируем нового игрока
            Rectangle plaerPosition = new Rectangle(120, 120, LevelLoader.SizePeople, LevelLoader.SizePeople);
            player = new Player(storage.Pull2DTexture("player"), storage.Pull2DTexture("player_run"), storage.Pull2DTexture("player_run_goriz"), plaerPosition, this, camera, levelLoader);


            this.levelLoader = new LevelLoader(this, player, storage, camera);


            maxLvl = storage.GetMaxLevelNumber();
            //сразу создаем уровень
            levelLoader.CreateLevel(7);
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
                    if (levelLoader.levelMap[i, j] == 1)
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


            if (state.IsKeyDown(Keys.D))
            {
                debugMode = !debugMode;
            }

            // вкл, выкл. тревогу
            if (state.IsKeyDown(Keys.A))
            {
                foreach (Guards guard in levelLoader.guards)
                {
                    guard.Alarm = !guard.Alarm;
                }
            }

            // вкл, выкл. туман войны
            if (state.IsKeyDown(Keys.S))
            {
                shadow.isShadow = !shadow.isShadow;
                shadow.HideOrShowAll(levelLoader.guards, levelLoader.objs); // отображает или скрывание сущности на экране
            }

            //смена уровня по нажатию на пробел
            if (state.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space)) {
                if (oldState != state)
                {
                    currentLvl++;
                    if (currentLvl > maxLvl)
                    {
                        currentLvl = 1;
                    }
                    levelLoader.CreateLevel(currentLvl);
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
            foreach (Guards guard in levelLoader.guards)
            {
                guard.Update(gameTime);
            }

            int i = 0;
            while (i < levelLoader.objs.Count) {
                if (levelLoader.objs[i].Rect.Intersects(player.Position))
                {
                    levelLoader.blocks.Add(new Block(levelLoader.objs[i].Rect, storage.Pull2DTexture("empty"), this, this.camera));
                    levelLoader.objs.RemoveAt(i);
                }
                else i++;
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

            // отрисовываем объекты
            foreach (Object obj in levelLoader.objs)
            {
                if (obj.visible)
                {
                    obj.Draw(spriteBatch);
                }
            }
            // отрисовываем стены
            foreach (Block block in levelLoader.blocks)
            {
                block.Draw(spriteBatch);
            }
            

            try
            {
                if (debugMode)
                {
                    spriteBatch.DrawString(storage.PullFont("font"), "PosGuard[0].X = " + levelLoader.guards[0].X.ToString(), new Vector2(10, 0), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "PosGuard[0].Y = " + levelLoader.guards[0].Y.ToString(), new Vector2(10, 20), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "LevelLenght = " + LevelLoader.GetLenghtX.ToString(), new Vector2(10, 40), Color.Orange); // распечатка длины уровня по X
                    spriteBatch.DrawString(storage.PullFont("font"), "LevelHeight = " + LevelLoader.GetLenghtY.ToString(), new Vector2(10, 60), Color.Orange); // распечатка длины уровня по Y 
                    spriteBatch.DrawString(storage.PullFont("font"), "LvlMapGuard(0, 0) = " + Guards.GetLevelMap(0, 0).ToString(), new Vector2(10, 80), Color.Orange); // распечатка уровня игры для охранника
                    spriteBatch.DrawString(storage.PullFont("font"), "LvlMapGuard(0, 1) = " + Guards.GetLevelMap(0, 1).ToString(), new Vector2(10, 100), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "LvlMapGuard(0, 2) = " + Guards.GetLevelMap(0, 2).ToString(), new Vector2(10, 120), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "LvlMapGuard(1, 0) = " + Guards.GetLevelMap(1, 0).ToString(), new Vector2(10, 140), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "LvlMapGuard(1, 1) = " + Guards.GetLevelMap(1, 1).ToString(), new Vector2(10, 160), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "NextStepGuardX - " + levelLoader.guards[0].NextX.ToString(), new Vector2(10, 250), Color.Orange);
                    spriteBatch.DrawString(storage.PullFont("font"), "NextStepGuardY - " + levelLoader.guards[0].NextY.ToString(), new Vector2(10, 270), Color.Orange); // распечатка клетки для следующего хода охранника

                    spriteBatch.DrawString(storage.PullFont("font"), "MyPosX - " + player.NewPosX.ToString(), new Vector2(10, 330), Color.Orange); // распечатка клетки для следующего хода охранника
                    spriteBatch.DrawString(storage.PullFont("font"), "MyPosY - " + player.NewPosY.ToString(), new Vector2(10, 350), Color.Orange); // распечатка клетки для следующего хода охранника
                    spriteBatch.DrawString(storage.PullFont("font"), "Alarm - " + levelLoader.guards[0].Alarm, new Vector2(10, 380), Color.Orange); // тревога
                    spriteBatch.DrawString(storage.PullFont("font"), "ShadowOfWar - " + shadow.isShadow, new Vector2(10, 400), Color.Orange); // тревога

                    // подсказки управления
                    spriteBatch.DrawString(storage.PullFont("font"), "S - shadow", new Vector2(400, 40), Color.LimeGreen); 
                    spriteBatch.DrawString(storage.PullFont("font"), "Arrows - control", new Vector2(400, 60), Color.LimeGreen); 
                    spriteBatch.DrawString(storage.PullFont("font"), "Space - chage level", new Vector2(400, 80), Color.LimeGreen);

                }
                else
                {
                    spriteBatch.DrawString(storage.PullFont("font"), "D - debug", new Vector2(400, 20), Color.LimeGreen); // тревога
                }

                
            }
            catch {
                // TODO: необходимо как-то обрабатывать исключения!
            }

            spriteBatch.End();

            // отрисовываем охранников
            foreach (Guards guard in levelLoader.guards)
            {
                if (guard.visible)
                {
                    guard.Draw(spriteBatch);
                }
            }

            // отрисовываем положение игрока
            player.Draw(spriteBatch);
            
            
            base.Draw(gameTime);
        }





        

        

    }
}