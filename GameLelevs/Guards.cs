using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Structs;

// подключим XNA фреймворк
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Enumeration;

namespace GameLevels
{
    class Guards
    {
        public Rectangle Rect { get; set; }
        
        Rectangle position;
        int speed;
        char direction; //направление охранника
        bool isRunning; //бежит или нет?

        // текстура для вывода охранника в состоянии спокойствия
        private Texture2D idlTexture;

        // текстура для вывода охранника в состоянии бега
        private Texture2D runTexture;

        // для анимации
        private FrameInfo frameInfo;

        // информация об охраннике
        private GuardInfo guardInfo;

        // ссылка на экран
        private Game1 game;

        //карта уроня для алгоритма поиска пути (получаем из кл. Game1.cs)
        private static byte[,] levelMap;

        //запоминаем карту уровня
        public void SetLevelMap(byte[,] tempLevelMap, int n, int m) 
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    levelMap[i, j] = tempLevelMap[i, j];
                }
            }
 
        }


        private int x; // координаты охранников
        private int y;
	    private enum Propety  {Finish, Start = 24, FreeWay, Wall};

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
        public Guards(Texture2D idlTexture, Texture2D runTexture, Rectangle position, Game1 game)
        {
            Init();
            this.idlTexture = idlTexture;
            this.runTexture = runTexture;

            //присваиваем положение охранника
            x = position.X;
            y = position.Y;


            frameInfo.height = frameInfo.width = runTexture.Height;

            // вычислим сколько кадров в анимации
            frameInfo.count = this.runTexture.Width / frameInfo.width;

            this.position = position;
            
            this.game = game;
        }

        /// <summary>
        /// Перегруженный конструктор класса
        /// </summary>
        /// <param name="sb">SB</param>
        /// <param name="playerTexture">Текстура игрока</param>
        /// <param name="frameSettings">Информация о фрейме</param>
        public Guards(Texture2D idlTexture, Texture2D runTexture, FrameInfo frameInfo, Game1 game)
        {
            Init();
            this.idlTexture = idlTexture;
            this.runTexture = runTexture;

            this.frameInfo = frameInfo;

            this.game = game;
        }

        /// <summary>
        /// Функция инициализирует значениями по умолчанию поля класса
        /// </summary>
        public void Init()
        {
            this.speed = 15;

            frameInfo.height = 0;
            frameInfo.width = 0;
            frameInfo.timeForFrame = 5;
        }

        /// <summary>
        /// Функция устанавливает флаг в состояние бега
        /// </summary>>
        public void Run(char direction) 
        {
            this.isRunning = true;
            this.direction = direction;
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

            if (this.isRunning)
            {
                Rectangle source = new Rectangle(frameInfo.width * frameInfo.current, 0, frameInfo.width, frameInfo.height);
                spriteBatch.Draw(runTexture, this.position, source, Color.White);
            }
            else 
            {
                spriteBatch.Draw(idlTexture, this.position, Color.White);
            }

            spriteBatch.End();
            
        }

        /// <summary>
        /// Обновление положения охранника
        /// </summary>
        public void Update(GameTime gameTime)
        {
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

                // смотрим, в каком направлении движемся 
                /*switch (this.direction)
                {
                    case 't':
                        newPosition.Offset(0, -offset);
                        break;

                    case 'l':
                        newPosition.Offset(-offset, 0);
                        break;

                    case 'r':
                        newPosition.Offset(offset, 0);
                        break;

                    case 'd':
                        newPosition.Offset(0, offset);
                        break;
                    
                    default: break;
                }*/
                newPosition.Offset(0, offset);
                //if (newPosition.Left > 0 && newPosition.Right < game.Width && !game.CollidesWithLevel(newPosition))
                    this.position = newPosition;
            }
        }







        /************************************************************************/
        /* Алгоритм поиска минимального пути                                    */
        /************************************************************************/
        public List<List<int>> PaveWay(char[,] arr, int N, int M, int x_f, int y_f)
        {
            int[,] workarr = new int[N, M];
            int k;
            int max_k = N * M - (N > M ? N : M); // максимальное число итераций
            int i, j;
            int twice;
            // заполняем рабочий массив
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    if (arr[i, j] == '0' || arr[i, j] == 'r' || arr[i, j] == 's' || arr[i, j] == 't' || arr[i, j] == 'u')
                        workarr[i, j] = (int)Propety.FreeWay; // проходимо
                    else
                        workarr[i, j] = (int)Propety.Wall; // не проходимо
                    if (i == x && j == y)
                        workarr[i, j] = (int)Propety.Start; // старт
                    if (i == x_f && j == y_f)
                        workarr[i, j] = (int)Propety.Finish; // финиш                        
                }
            }

            // Распространение волны 
            k = 0;
            // начинаем поиск.. сначала ищем финиш
            while (k < max_k)
            {
                for (i = 1; i < N - 1; i++)
                {
                    for (j = 1; j < M - 1; j++)
                    {
                        if (workarr[i, j] == k)
                        {
                            twice = 0;
                            do
                            {
                                for (int p = -1; p < 2; p = p + 2)
                                {
                                    if (workarr[i + p * (-twice + 1), j + p * twice] == (int)Propety.Start) // дошли до старта => выходим из поиска
                                        goto ex;
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
        ex:
            for (i = 0; i < N; i++)
            {
                for (j = 0; j < M; j++)
                {
                    Console.Write("{0,-4:d2}", workarr[i, j] + " ");
                }
                Console.WriteLine();
            }
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
