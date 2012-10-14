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

namespace GameLevels
{
    class Guards : IMan.IMan
    {
        public Rectangle Rect { get; set; }

        Rectangle position;
        Camera camera;

        int oldPosGuardX; // старая позиция охранника. Для сравнения - "перешел ли охранник на новую клетку?"
        int oldPosGuardY;

        bool waySet = false; // задан ли охраннику путь к цели ?
        int step; // номер шага клетки, куда нужно идти

        int targetX = 10; //координаты цели, к кот. идет охранник
        int targetY = 8;
        int currentStepPatrol = 0; // текущий шаг для патрулирования

        int speed;
        PlayerMove direction; //направление охранника
        bool isRunning; //бежит или нет?

        private bool alarm = false; // есть ли тревога ? True - включена

        // текстура для вывода охранника в состоянии спокойствия
        private Texture2D idlTexture;

        // текстура для вывода охранника в состоянии бега по вертикали
        private Texture2D runTextureVert;

        // текстура вывода охранника в состоянии бега по горизонтали
        private Texture2D runTextureGoriz;

        // для анимации
        private FrameInfo frameInfo;

        // ссылка на экран
        private Game1 game;
        private Player player;

        List<List<int>> wayToTarget = new List<List<int>>(); // путь к цели
        List<List<int>> wayToPatrol = new List<List<int>>(); // траектория для патрулирования
        


        //карта уроня для алгоритма поиска пути (получаем из кл. Game1.cs)
        private static byte[,] levelMap;
        private static int levelWidth; //длина уровня (клеток)
        private static int levelHeight; // высота уровня (клеток)


        //запоминаем карту уровня
        public static void SetLevelMap(byte[,] tempLevelMap, int n, int m) 
        {
            levelWidth = n;
            levelHeight = m;

            levelMap = new byte[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    levelMap[i, j] = tempLevelMap[i, j];
                }
            }

        }


        // для алгоритма мин. пути
        private int x; // координаты охранников
        private int y;
	    private enum Propety  {Finish, Start = 253, FreeWay, Wall};

        public int X
	    {
		    get { return x; }
	    }
        public int Y
	    {
		    get { return y; }
	    }
        
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="playerTexture">Текстура охранника</param>
        /// <param name="width">Ширина кадра</param>
        /// <param name="height">Высота кадра</param>
        public Guards(Texture2D idlTexture, Texture2D runTextureVert, Texture2D runTextureGoriz, Rectangle position, Game1 game, Player player, Camera camera)
        {
            Init();
            this.idlTexture = idlTexture;
            this.runTextureGoriz = runTextureGoriz;
            this.runTextureVert = runTextureVert;

            this.game = game;
            this.camera = camera;
            this.player = player;

            this.step = 1;
            
            //присваиваем положение охранника
            x = position.X / game.Size;
            y = position.Y / game.Size;
            
            
            this.oldPosGuardX = x / game.Size;
            this.oldPosGuardY = y / game.Size;

            frameInfo.height = frameInfo.width = runTextureVert.Height;

            // вычислим сколько кадров в анимации
            frameInfo.count = this.runTextureVert.Width / frameInfo.width;

            this.position = position;


            //массив для патрулирования
            // временный. Потом будет считываться из файла
            wayToPatrol.Add(new List<int>());// добавляем новую строку под координату
            wayToPatrol[0].Add(3); // первым указывается координата X
            wayToPatrol[0].Add(3); // второе - координата Y
            wayToPatrol.Add(new List<int>());// добавляем новую строку под координату
            wayToPatrol[1].Add(3);
            wayToPatrol[1].Add(5);
            wayToPatrol.Add(new List<int>());// добавляем новую строку под координату
            wayToPatrol[2].Add(5);
            wayToPatrol[2].Add(5);
            wayToPatrol.Add(new List<int>());// добавляем новую строку под координату
            wayToPatrol[3].Add(7);
            wayToPatrol[3].Add(3);
            targetX = wayToPatrol[0][0];
            targetY = wayToPatrol[0][1];

            //targetX = wayToPatrol[1][0]; // 0 - x
            //targetY = wayToPatrol[1][1]; // 1 - y

        }

        /// <summary>
        /// Перегруженный конструктор класса
        /// </summary>
        /// <param name="sb">SB</param>
        /// <param name="playerTexture">Текстура игрока</param>
        /// <param name="frameSettings">Информация о фрейме</param>
        public Guards(Texture2D idlTexture, Texture2D runTextureVert, Texture2D runTextureGoriz, FrameInfo frameInfo, Game1 game)
        {
            Init();
            this.idlTexture = idlTexture;
            this.runTextureGoriz = runTextureGoriz;
            this.runTextureVert = runTextureVert;

            this.frameInfo = frameInfo;

            this.game = game;
        }

        /// <summary>
        /// Функция инициализирует значениями по умолчанию поля класса
        /// </summary>
        public void Init()
        {
            this.speed = 1;

            frameInfo.height = 0;
            frameInfo.width = 0;
            frameInfo.timeForFrame = 70;
        }

        /// <summary>
        /// Функция устанавливает флаг в состояние бега
        /// </summary>>
        public void Run(PlayerMove move)
        {
            this.isRunning = true;
            this.direction = move;
        }
        public void Run()
        {
            this.isRunning = true;
        }

        /// <summary>
        /// Функция устанавливает флаг в состояние спокойствия
        /// </summary>>
        public void Stop()
        {
            this.isRunning = false;
            frameInfo.current = 0;
            frameInfo.timeElapsed = 0;
        }



        




        /// <summary>
        /// Функция отрисовывает охранника
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            Rectangle sourceRect = new Rectangle(frameInfo.width * frameInfo.current, 0, frameInfo.width, frameInfo.height);
            Rectangle screenRect = camera.GetScreenRect(this.position);  // рисуем только то, что помещается на экране. При передвижении камеры - охраники правильно отображаются. Не едут за камерой.

            if (this.isRunning)
            {
                //spriteBatch.Draw(runTexture, screenRect, sourceRect, Color.White);

                SpriteEffects currentEffect = new SpriteEffects();
                Texture2D currentTexture;

                // TODO: оптимизировать код!
                switch (this.direction)
                {
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

                spriteBatch.Draw(currentTexture, screenRect, sourceRect, Color.White, 0, Vector2.Zero, currentEffect, 0);

            }
            else
            {
                spriteBatch.Draw(idlTexture, screenRect, Color.White);
            }

            spriteBatch.End();
            
        }

        /// <summary>
        /// Обновление положения охранника
        /// </summary>
        public void Update(GameTime gameTime)
        {
            //изменяем точку, в кот. идет охранник, если игрок сместился на другую клетку и включена тревога
            if (player.changedPos && alarm)
            {
                player.changedPos = false;
                this.targetX = player.NewPosX;
                this.targetY = player.NewPosY;
                this.Run();
            }


            if (alarm == false)
            {
                //this.Patrol();
            }
            
            if (this.isRunning) 
            {
                // изменим кадр анимации
                frameInfo.timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (frameInfo.timeElapsed > frameInfo.timeForFrame) 
                {
                    frameInfo.timeElapsed = 0;
                    frameInfo.current = (frameInfo.current + 1) % frameInfo.count;
                }

                // передвижение охранника
                int offset = this.speed * gameTime.ElapsedGameTime.Milliseconds / 10;
                
                // новое положение охранника
                Rectangle newPosition = this.position;


                // смотрим, сместился ли на клетку охранник
                x = newPosition.X / game.Size;
                y = newPosition.Y / game.Size;


                //текущая клетка охранника
                int nowPosGuardX = newPosition.X / game.Size;
                int nowPosGuardY = newPosition.Y / game.Size;

                // следующая клетка, в кот. должен бежать охранник
                int nextX = 0;
                int nextY = 0;


                if (!this.waySet)
                {
                    this.waySet = true;
                    wayToTarget = this.Way(levelMap, levelWidth, levelHeight, this.targetX, this.targetY); // прокладываем путь к цели
                }
                
                if (wayToTarget != null)
                {
                    try
                    {
                        nextX = this.wayToTarget[1][0]; // координаты след. клетки
                        nextY = this.wayToTarget[1][1];
                    }
                    catch { 
                        // Если обрабатываешь все исключения, то нет необходимости указывать его тип
                        // Если нужен только тип, но объект исключения не нужен, то указывается в скобках просто: catch(Exception)
                        step = 1;
                    }

                    // в зависимости от положения клетки, в кот. должен бегать охранник изменяем направление движения
                    if (nextY < nowPosGuardY)
                    {
                        this.direction = PlayerMove.Up;
                    }
                    else if (nextY > nowPosGuardY)
                    {
                        this.direction = PlayerMove.Down;
                    }
                    if (nextX > nowPosGuardX)
                    {
                        this.direction = PlayerMove.Right;
                    }
                    else if (nextX < nowPosGuardX)
                    {
                        this.direction = PlayerMove.Left;
                    }

                }
                

                if (Math.Abs(this.oldPosGuardX * game.Size + game.Size / 2 - (newPosition.X + game.SizePeople / 2)) >= game.Size)
                {
                    if (nowPosGuardX != this.oldPosGuardX)
                    {
                        this.oldPosGuardX = nowPosGuardX;
                        step++;
                        this.waySet = false;
                    }

                    if (this.oldPosGuardX == this.targetX && this.oldPosGuardY == this.targetY) 
                    {
                        this.CheckStop();
                    }
                }
                if (Math.Abs(this.oldPosGuardY * game.Size + game.Size / 2 - (newPosition.Y + game.SizePeople / 2)) >= game.Size)
                {
                    if (nowPosGuardY != this.oldPosGuardY)
                    {
                        this.oldPosGuardY = nowPosGuardY;
                        step++;
                        this.waySet = false;
                    }
                    if (this.oldPosGuardX == this.targetX && this.oldPosGuardY == this.targetY) 
                    {
                        this.CheckStop();
                    }
                }
                


                
                
                // смотрим, в каком направлении движемся 
                switch (this.direction)
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
            
                // перемещение охранника
                if (newPosition.Left > 0 && newPosition.Right < game.GetLenghtX)
                {
                    this.position = newPosition;
                }
                
            }
        }


        
        /// <summary>
        /// Ф-ция проверяет, стоит ли останавливаться охраннику.
        /// Если тревога работает - то охранник продолжает преследовать игрока
        /// </summary>
        private void CheckStop() 
        {
            if (alarm == false)
            {
                currentStepPatrol++;
                this.Patrol();
            }
            else
            {
                this.Stop();
            }
            
        }





        /// <summary>
        /// Ф-ция для патрулирования местности
        /// </summary>
        private void Patrol()
        {
            if (currentStepPatrol > 3)
            {
                currentStepPatrol = 0;
            }
            targetX = wayToPatrol[currentStepPatrol][0];
            targetY = wayToPatrol[currentStepPatrol][1];
        }






 

        /// <summary>
        /// Алгоритм поиска минимального пути
        /// </summary>
        /// <param name="arr">Массив с картой уровня</param>
        /// <param name="N">Размер массива (ширина)</param>
        /// <param name="M">Размер массива (высота)</param>
        /// <param name="x_f">Координата конечной точки Х</param>
        /// <param name="y_f">Координата конечной точки У</param>
        /// <returns>Список точек пути до конечной точки</returns>
        public List<List<int>> Way(byte[,] arr, int N, int M, int x_f, int y_f)
        {
            int[,] workarr = new int[N + 1, M + 1];
            int k;
            int max_k = N * M; // максимальное число итераций
            int i, j;
            int twice;
            // заполняем рабочий массив
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    if (arr[i, j] == 0 )
                        workarr[i, j] = (int)Propety.FreeWay; // проходимо
                    else
                        workarr[i, j] = (int)Propety.Wall; // не проходимо
                    if (i == x && j == y)
                        workarr[i, j] = (int)Propety.Start; // старт
                    if (i == x_f && j == y_f)
                        workarr[i, j] = (int)Propety.Finish; // финиш                        
                }
            }

            bool f_way = false; // есть ли путь
            bool f_way_iter = true; // путь на данной итерации
            // Распространение волны 
            k = 0;
            // начинаем поиск.. сначала ищем финиш
            while (!f_way && f_way_iter && k < max_k)
            {
                f_way_iter = false; // считаем что на данной итерации нет пути
                for (i = 1; i < N - 1; i++)
                {
                    for (j = 1; j < M - 1; j++)
                    {
                        if (workarr[i, j] == k)
                        {
                            twice = 0;
                            f_way_iter = true; // на данной итерации есть путь
                            do
                            {
                                for (int p = -1; p < 2; p = p + 2)
                                {
                                    if (workarr[i + p * (-twice + 1), j + p * twice] == (int)Propety.Start) // дошли до старта => выходим из поиска
                                    {
                                        twice = 2;
                                        i = N;
                                        j = M;
                                        f_way = true; // путь полностью найден
                                        break;
                                    }
                                    if (workarr[i + p * (-twice + 1), j + p * twice] == (int)Propety.FreeWay)
                                        workarr[i + p * (-twice + 1), j + p * twice] = k + 1;
                                }
                                twice++;
                            }
                            while (twice < 2);

                        }
                    }
                }
                k++;
            }

            if (!f_way)
                return null; // Нет пути
            // заполняем массив коордианатами
            List<List<int>> ArrWay = new List<List<int>>();
            ArrWay.Add(new List<int>());
            int r = 0;
            //стартовая координата
            ArrWay[r].Add(x);
            ArrWay[r].Add(y);
            i = x;
            j = y;

            int min = workarr[i, j];
            int i_m = i;
            int j_m = j;
            while (workarr[i, j] != (int)Propety.Finish) // прокладываем маршрут пока не дойдем до финиша
            {
                twice = 0;
                // поиск минимального элемента из [i+1,j]; [i-1,j]; [i,j+1]; [i,j-1]
                do
                {
                    for (int p = -1; p < 2; p = p + 2)
                    {
                        if (workarr[i + p * (-twice + 1), j + p * twice] < min)
                        {
                            min = workarr[i + p * (-twice + 1), j + p * twice];
                            i_m = i + p * (-twice + 1);
                            j_m = j + p * twice;
                        }
                    }
                    twice++;
                }
                while (twice <= 1);
                i = i_m;
                j = j_m;
                r++;
                ArrWay.Add(new List<int>());// добавляем новую строку под координату
                ArrWay[r].Add(i);
                ArrWay[r].Add(j);
            }
            return ArrWay;
        }



    }
}
