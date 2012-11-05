using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Structs;

// подключим XNA фреймворк
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Enumerations;
using GameLevels.levelObjects;

namespace GameLevels
{
    /// <summary>
    /// Класс описывающий основные функции игрока
    ///  - отрисовка
    ///  - передвижение
    ///  - взаимодействие
    /// </summary>
    class Player : IMan.IMan
    {
        // текстура для вывода игрока в состоянии спокойствия
        private Texture2D idlTexture;

        // текстура для вывода игрока в состоянии бега по вертикали
        private Texture2D runTextureVert;

        // текстура вывода игрока в состоянии бега по горизонтали
        private Texture2D runTextureGoriz;

        // для анимации
        private FrameInfo frameInfo;

        // информация об игроке
        private PlayerInfo playerInfo;

        // рюкзак игрока
        private List<BaseObject> backpack;

        // максимальный размер рюкзака
        private const int maxSizeBackpack = 6;

        // ссылка на экран
        private Game1 game;
        // ссылка на камеру
        private Camera camera;

        private Shadow shadow;

        private LevelLoader levelLoader;

        // старое положение игрока
        private int oldPosX;
        private int oldPosY;

        // новое положение игрока
        private int newPosX;
        private int newPosY;

        public int room;  // в какой комнате игрок?

        public int NewPosX
        {
            get { return newPosX; }
        }
        public int NewPosY
        {
            get { return newPosY; }
        }

        // позиция игрока изменилась ? Заново направляем охранников на игрока
        public bool changedPos = false;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="playerTexture">Текстура игрока</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        public Player(Texture2D idlTexture, Texture2D runTextureVert, Texture2D runTextureGoriz, Rectangle position, Game1 game, Camera camera)
        {
            Init();
            this.idlTexture = idlTexture;
            this.runTextureVert = runTextureVert;
            this.runTextureGoriz = runTextureGoriz;

            frameInfo.height = frameInfo.width = runTextureVert.Height;

            // вычислим сколько кадров в анимации
            frameInfo.count = this.runTextureVert.Width / frameInfo.width;

            playerInfo.position = position;

            this.game = game;
            this.camera = camera;
            
            this.backpack = new List<BaseObject>();
        }

        /// <summary>
        /// Передаем ссылку на загрузчик уровней
        /// </summary>
        /// <param name="levelLoader"></param>
        public void setLinkLevelLoader(LevelLoader levelLoader)
        {
            this.levelLoader = levelLoader;
        }

        /// <summary>
        /// Передаем ссылку на туман войны
        /// </summary>
        /// <param name="shadow"></param>
        public void setShadow(Shadow shadow)
        {
            this.shadow = shadow;
        }
        

        /// <summary>
        /// Перегруженный конструктор класса
        /// </summary>
        /// <param name="sb">SB</param>
        /// <param name="playerTexture">Текстура игрока</param>
        /// <param name="frameSettings">Информация о фрейме</param>
        public Player(Texture2D idlTexture, Texture2D runTextureVert, Texture2D runTextureGoriz, FrameInfo frameInfo, Game1 game)
        {
            Init();
            this.idlTexture = idlTexture;
            this.runTextureVert = runTextureVert;
            this.runTextureGoriz = runTextureGoriz;

            this.frameInfo = frameInfo;

            this.game = game;
        }

        /// <summary>
        /// Функция инициализирует значениями по умолчанию поля класса
        /// </summary>
        public void Init()
        {
            playerInfo.speed = 2;
            playerInfo.isVisible = true;

            frameInfo.height = 0;
            frameInfo.width = 0;
            frameInfo.timeForFrame = 70;
        }

        /// <summary>
        /// Функция устанавливает флаг в состояние бега
        /// </summary>>
        public void Run(PlayerMove move) 
        {
            playerInfo.isRunning = true;
            playerInfo.direction = move;
        }

        /// <summary>
        /// Функция устанавливает флаг в состояние спокойствия
        /// </summary>>
        public void Stop()
        {
            playerInfo.isRunning = false;
            frameInfo.current = 0;
            frameInfo.timeElapsed = 0;
        }

        /// <summary>
        /// Функция отрисовывает игрока
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            Rectangle screenRect = camera.GetScreenRect(playerInfo.position); // экранные координаты. При движении камеры, игрок отрисовывается на месте, а не едет вместе с камерой

            Rectangle sourceRect = new Rectangle(frameInfo.width * frameInfo.current, 0, frameInfo.width, frameInfo.height);

            Color color;

            if (playerInfo.isVisible)
            {
                color = Color.White;
            }
            else
            {
                color = new Color(255, 255, 255, 150);
            }

            if (playerInfo.isRunning)
            {
                SpriteEffects currentEffect = new SpriteEffects();
                Texture2D currentTexture;

                // TODO: оптимизировать код!
                switch (playerInfo.direction) {
                    case PlayerMove.Left:
                        currentEffect = SpriteEffects.FlipHorizontally;
                        currentTexture = runTextureGoriz;
                        break;
                    case PlayerMove.Right:
                        currentEffect = SpriteEffects.None;
                        currentTexture = runTextureGoriz;
                        break;
                    case PlayerMove.Up:
                        currentEffect = SpriteEffects.FlipVertically;
                        currentTexture = runTextureVert;
                        break;
                    default: 
                        currentEffect = SpriteEffects.None;
                        currentTexture = runTextureVert;
                        break;
                }
                
                spriteBatch.Draw(currentTexture, screenRect, sourceRect, color, 0, Vector2.Zero, currentEffect, 0);
            }
            else
            {
                spriteBatch.Draw(idlTexture, screenRect, color);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Обновление положения игрока
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (playerInfo.isRunning) 
            {
                // изменим кадр анимации
                frameInfo.timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (frameInfo.timeElapsed > frameInfo.timeForFrame) 
                {
                    frameInfo.timeElapsed = 0;
                    frameInfo.current = (frameInfo.current + 1) % frameInfo.count;
                }

                // передвижение игрока
                int offset = playerInfo.speed;//playerInfo.speed * gameTime.ElapsedGameTime.Milliseconds / 10;

                // новое положение игрока
                Rectangle newPosition = playerInfo.position;

                // считаем старую клетку игрока. Считается от центра игрока.
                oldPosX = (playerInfo.position.X + (LevelLoader.SizePeople / 2)) / LevelLoader.Size;
                oldPosY = (playerInfo.position.Y + (LevelLoader.SizePeople / 2)) / LevelLoader.Size;
                room = LevelLoader.levelMapRooms[oldPosX, oldPosY]; // текущая комната

                // смотрим, в каком направлении движемся 
                switch (playerInfo.direction)
                {
                    case PlayerMove.Up:
                        newPosition.Offset(0, -offset);
                        break;

                    case PlayerMove.Left:
                        newPosition.Offset(-offset, 0);
                        break;

                    case PlayerMove.Right:
                        newPosition.Offset(offset, 0);
                        break;

                    case PlayerMove.Down:
                        newPosition.Offset(0, offset);
                        break;

                    default: break;
                }

                if (newPosition.Left > 0 && newPosition.Right < LevelLoader.GetLenghtX && !game.CollidesWithLevel(newPosition))
                {
                    // перемещаем игрока на новую позицию
                    playerInfo.position = newPosition;
                    //считаем новую клетку игрока. Считается от центра игрока
                    newPosX = (playerInfo.position.X + (LevelLoader.SizePeople / 2)) / LevelLoader.Size;
                    newPosY = (playerInfo.position.Y + (LevelLoader.SizePeople / 2)) / LevelLoader.Size;
                    //изменилась ли клетка, в кот. находился игрок?
                    if (newPosX != oldPosX || newPosY != oldPosY)
                    {
                        changedPos = true;
                        if (room != LevelLoader.levelMapRooms[newPosX, newPosY]) {
                            room = LevelLoader.levelMapRooms[newPosX, newPosY];
                            shadow.ShowInRoom(room); // показать объекты в комнате
                        }
                    }


                    // движение камеры за игроком. Камера не выходит за пределы экрана
                    // движение камеры по X (сравнение с границами уровня)
                    if (playerInfo.position.X > game.GetScreenWidth / 2 - LevelLoader.Size / 2 && playerInfo.position.X + game.GetScreenWidth / 2 + LevelLoader.Size / 2 < LevelLoader.GetLenghtX)
                    {
                        camera.SetCameraPosition(playerInfo.position.X - game.GetScreenWidth / 2 + LevelLoader.Size / 2, camera.ScrollY);
                    }
                    // движение камеры по Y (сравнение с границами уровня)
                    if (playerInfo.position.Y > game.GetScreenHeight / 2 - LevelLoader.Size / 2 && playerInfo.position.Y + game.GetScreenHeight / 2 + LevelLoader.Size / 2 < LevelLoader.GetLenghtY)
                    {
                        camera.SetCameraPosition(camera.ScrollX, playerInfo.position.Y - game.GetScreenHeight / 2 + LevelLoader.Size / 2);
                    }

                }
            }
        }

        /// <summary>
        /// Возвращает позицию игрока
        /// </summary>
        public Rectangle Position {
            get {
                return playerInfo.position;
            }
            set {
                playerInfo.position = value;
            }
        }

        /// <summary>
        /// Добавляет предмет в рюкзак
        /// </summary>
        /// <param name="obj">Предмет</param>
        /// <returns>Успешность операции</returns>
        public bool AddItem(BaseObject obj) {
            if (this.backpack.Count >= maxSizeBackpack)
                return false;

            backpack.Add(obj);

            return true;
        }

        /// <summary>
        /// Делает игрока видимым
        /// </summary>
        public void SetVisible()
        {
            playerInfo.isVisible = true;
        }

        /// <summary>
        /// Делает игрока невидимым
        /// </summary>
        public void SetInvisible()
        {
            playerInfo.isVisible = false;
        }

        /// <summary>
        /// Проверяет видимость игрока
        /// </summary>
        /// <returns>видимость игрока</returns>
        public bool IsVisible()
        {
            return playerInfo.isVisible;
        }
    }
}
