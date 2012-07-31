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

        int scrollX;
        int scrollY;
        int lenghtX;
        int lenghtY;
        
        int size = 30;
        int speedCamera = 1;

        int width;
        int height;

        int currentLvl;
        int maxLvl = 4;
        KeyboardState oldState;
        
        List<Block> blocks;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            width = this.graphics.PreferredBackBufferWidth = 500; //ширина и высота экрана
            height = this.graphics.PreferredBackBufferHeight = 500;

            this.Components.Add(new FPSCounter(this));  // добавили игровой компонент - fps счетчик

            // отключили ограничение fps счетчика
            IsFixedTimeStep = false; 
            graphics.SynchronizeWithVerticalRetrace = false;
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

            doorHoriz = Content.Load<Texture2D>("Textures/lvl/doors/door_horiz");
            doorVertic = Content.Load<Texture2D>("Textures/lvl/doors/door_vertic");
            doorHorizOpen = Content.Load<Texture2D>("Textures/lvl/doors/door_horiz_open");
            doorVerticOpen = Content.Load<Texture2D>("Textures/lvl/doors/door_vertic_open");

            // инициализируем нового игрока
            player = new Player(spriteBatch, Content.Load<Texture2D>("players/player"), 20, 20);

            CreateLevel(1);

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



        public Rectangle GetScreenRect(Rectangle rect)
        {
            Rectangle r = rect;
            r.Offset(-scrollX, -scrollY);
            return r;
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

            
            if (state.IsKeyDown(Keys.Left))
            {
                Scroll(-speedCamera, 0);
            }
            if (state.IsKeyDown(Keys.Right))
            {
                Scroll(speedCamera, 0);
            }
            if (state.IsKeyDown(Keys.Up))
            {
                Scroll(0, -speedCamera);
            }
            if (state.IsKeyDown(Keys.Down))
            {
                Scroll(0, speedCamera);
            }

            player.Update(gameTime);


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

            foreach (Block block in blocks)
            {
                block.Draw(spriteBatch);
            }
            spriteBatch.End();

            player.Draw(new Rectangle(0, 0, 20, 20));

            base.Draw(gameTime);
        }


        void CreateLevel(int lvl)
        {
            blocks = new List<Block>();
            
            string lvl_name = "content/lvls/lvl" + Convert.ToString(lvl) + ".txt";
            string[] lines = File.ReadAllLines(lvl_name);

            
            int x = 0;
            int y = 0;
            foreach (string line in lines)
            {
                foreach (char c in line)
                {
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
                    }
                    if (c == '2')
                    {
                        Block block = new Block(Rect, wallVert, this);
                        blocks.Add(block);
                    }
                    if (c == '3')
                    {
                        Block block = new Block(Rect, wallDownRight, this);
                        blocks.Add(block);
                    }
                    if (c == '4')
                    {
                        Block block = new Block(Rect, wallUpRight, this);
                        blocks.Add(block);
                    }
                    if (c == '5')
                    {
                        Block block = new Block(Rect, wallLeftDown, this);
                        blocks.Add(block);
                    }
                    if (c == '6')
                    {
                        Block block = new Block(Rect, wallLeftUp, this);
                        blocks.Add(block);
                    }
                    if (c == '7')
                    {
                        Block block = new Block(Rect, wall4sides, this);
                        blocks.Add(block);
                    }
                    if (c == 'h')
                    {
                        Block block = new Block(Rect, wallURD, this);
                        blocks.Add(block);
                    }
                    if (c == 'i')
                    {
                        Block block = new Block(Rect, wallRDL, this);
                        blocks.Add(block);
                    }
                    if (c == 'j')
                    {
                        Block block = new Block(Rect, wallDLU, this);
                        blocks.Add(block);
                    }
                    if (c == 'k')
                    {
                        Block block = new Block(Rect, wallLUR, this);
                        blocks.Add(block);
                    }


                    //двери
                    if (c == 'r')
                    {
                        Block block = new Block(Rect, doorHoriz, this);
                        blocks.Add(block);
                    }
                    if (c == 's')
                    {
                        Block block = new Block(Rect, doorVertic, this);
                        blocks.Add(block);
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




                    x += size;
                }

                x = 0;
                y += size;
            
            }

            lenghtX = size * lines[0].Length; // длина уровня в пикселях
            lenghtY = y;

        }

    }
}