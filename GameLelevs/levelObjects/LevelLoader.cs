using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Enumerations;
using GameLevels.levelObjects;

namespace GameLevels
{
    class LevelLoader
    {

        Game1 game;
        Camera camera;
        Player player;
        Storage storage;

        public List<Block> blocks; // объекты стен и дверей
        public List<Object> objs; // объекты на уровне
        public List<Guards> guards; // список охранников

        // сложность уровня
        public Complexity complexity;

        // карта уровня
        // так же используется для алгоритма поиска пути к игроку
        public byte[,] levelMap;

        
        
        public LevelLoader(Game1 game, Player player, Storage storage, Camera camera)
        {
            this.game = game;
            this.player = player;
            this.camera = camera;
            this.storage = storage;
            storage = new Storage();
        }



        static int sizePeople = 20; //размер изображения игрока и охранников
        public static int SizePeople
        {
            get { return sizePeople; }
        }

        //размер 1 ячейки уровня
        static int size = 30;
        public static int Size
        {
            get { return size; }
        }

        //длина уровня в пикселях
        static int lenghtX;
        static int lenghtY;

        //получаем двину уровня в пикселях
        public static int GetLenghtX
        {
            get { return lenghtX; }
            set { lenghtX = value; }
        }
        public static int GetLenghtY
        {
            get { return lenghtY; }
            set { lenghtY = value; }
        }

        /// <summary>
        /// Ф-ция создания уровня.
        /// Считывает данные из файла.
        /// Файл д.б. с названием "lvlX.txt";
        /// </summary>
        /// <param name="lvl">номер уровня</param>
        public void CreateLevel(int lvl)
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

            string[] str = { };

            int tempIndex = 0;
            int[] sizeFile = new int[2];



            // тестовый режим...
            // ЕСЛИ УРОВЕНЬ ИЗ РЕДАКТОРА, ТО...
            //приспособим загрузку уровней, сделанных в редакторе
            if (lvl >= 5)
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
                levelMap = new byte[sizeFile[1] + 1, sizeFile[0] + 1];


                tempIndex = 0;
                //считывание уровня из файла
                foreach (string line in lines)
                {

                    if (tempIndex == 0) { tempIndex++; continue; } // пропускаем первую строку с данными размера уровня

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




                int[,] map = new int[sizeFile[1] + 1, sizeFile[0] + 1]; // карта уровня после преобразования

                // преобразование значений объектов (стен) на уровне
                for (int i = 0; i < sizeFile[1]; i++)
                {
                    for (int j = 0; j < sizeFile[0]; j++)
                    {


                        map[i, j] = 0;
                        
                        if (levelMap[i, j] == 1)
                        {
                            map[i, j] = isWallType(levelMap, i, j, sizeFile[0] - 1, sizeFile[1] - 1);
                        }
                        

                    }
                }



                // создание объектов на уровне
                for (int i = 0; i < sizeFile[1]; i++)
                {
                    for (int j = 0; j < sizeFile[0]; j++)
                    {
                        Rectangle Rect = new Rectangle(i * LevelLoader.Size, j * LevelLoader.Size, LevelLoader.Size, LevelLoader.Size);
                        if (map[i, j] == 0)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 1)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_goriz"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 2)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_vert"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 3)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_down_right"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 4)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_up_right"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 5)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_down"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 6)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_up"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 7)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_4sides"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 8)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_urd"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 9)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_rdl"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 10)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_dlu"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 11)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_lur"), game, this.camera);
                            blocks.Add(block);
                        }


                        //двери
                        if (levelMap[i, j] == 20)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz"), game, this.camera);
                            blocks.Add(block);
                            levelMap[i, j] = 1;
                        }
                        if (levelMap[i, j] == 21)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic"), game, this.camera);
                            blocks.Add(block);
                            levelMap[i, j] = 1;
                        }
                        if (levelMap[i, j] == 22)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz_open"), game, this.camera);
                            blocks.Add(block);
                            levelMap[i, j] = 0;
                        }
                        if (levelMap[i, j] == 23)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic_open"), game, this.camera);
                            blocks.Add(block);
                            levelMap[i, j] = 0;
                        }

                        if (levelMap[i, j] == 30)
                        { //буква "о"
                            //пол
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), game, this.camera);
                            blocks.Add(block);

                            // инициализируем нового охранника
                            Rectangle RectGuard = new Rectangle(x + LevelLoader.SizePeople / 4, y + LevelLoader.SizePeople / 4, LevelLoader.SizePeople, LevelLoader.SizePeople);
                            Guards guard = new Guards(storage.Pull2DTexture("player"), storage.Pull2DTexture("player_run"), storage.Pull2DTexture("player_run_goriz"), RectGuard, game, player, this.camera, this);
                            guards.Add(guard);
                            guard.Run(PlayerMove.Left);
                        }

                        // ключ и пластиковая карта
                        if (levelMap[i, j] == 40)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("key"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 0;
                        }
                        if (levelMap[i, j] == 41)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("card"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 0;
                        }

                        // золото
                        if (levelMap[i, j] == 50)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("money"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 0;
                        }

                        // стол системы управления камерами
                        if (levelMap[i, j] == 60)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spLU"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 1;
                        }
                        if (levelMap[i, j] == 61)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spUR"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 1;
                        }
                        if (levelMap[i, j] == 62)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spRD"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 1;
                        }
                        if (levelMap[i, j] == 63)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spDL"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 1;
                        }

                        // стол с компьютером
                        if (levelMap[i, j] == 70)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableU"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 1;
                        }
                        if (levelMap[i, j] == 71)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableR"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 1;
                        }
                        if (levelMap[i, j] == 72)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableD"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 1;
                        }
                        if (levelMap[i, j] == 73)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableL"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = 1;
                        }

                        x += LevelLoader.Size;

                    }

                    x = 0;
                    y += LevelLoader.Size;

                }
                // конец создание объектов на уровне

                lenghtX = LevelLoader.Size * sizeFile[1]; // длина уровня в пикселях
                lenghtY = LevelLoader.Size * sizeFile[0];

                Guards.SetLevelMap(levelMap, lenghtX / LevelLoader.Size, lenghtY / LevelLoader.Size);
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
                        Rectangle Rect = new Rectangle(x, y, LevelLoader.Size, LevelLoader.Size);
                        if (s.Equals("0", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (s.Equals("1", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_goriz"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("2", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_vert"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("3", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_down_right"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("4", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_up_right"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("5", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_down"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("6", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_up"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("7", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_4sides"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("8", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_urd"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("9", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_rdl"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("10", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_dlu"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("11", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_lur"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }


                        //двери
                        if (s.Equals("20", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("21", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = 1;
                        }
                        if (s.Equals("22", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz_open"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (s.Equals("23", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic_open"), game, this.camera);
                            blocks.Add(block);
                        }

                        if (s.Equals("30", StringComparison.OrdinalIgnoreCase))
                        { //буква "о"
                            //пол
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), game, this.camera);
                            blocks.Add(block);

                            // инициализируем нового охранника
                            Rectangle RectGuard = new Rectangle(x + LevelLoader.SizePeople / 4, y + LevelLoader.SizePeople / 4, LevelLoader.SizePeople, LevelLoader.SizePeople);
                            Guards guard = new Guards(storage.Pull2DTexture("player"), storage.Pull2DTexture("player_run"), storage.Pull2DTexture("player_run_goriz"), RectGuard, game, player, this.camera, this);
                            guards.Add(guard);
                            guard.Run(PlayerMove.Left);
                        }

                        // ключ и пластиковая карта
                        if (s.Equals("40", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("key"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("41", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("card"), game, this.camera);
                            objs.Add(obj);
                        }

                        // золото
                        if (s.Equals("50", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("money"), game, this.camera);
                            objs.Add(obj);
                        }

                        // стол системы управления камерами
                        if (s.Equals("60", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spLU"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("61", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spUR"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("62", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spRD"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("63", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spDL"), game, this.camera);
                            objs.Add(obj);
                        }

                        // стол с компьютером
                        if (s.Equals("70", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableU"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("71", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableR"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("72", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableD"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (s.Equals("73", StringComparison.OrdinalIgnoreCase))
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableL"), game, this.camera);
                            objs.Add(obj);
                        }



                        x += LevelLoader.Size;

                        indexI++;
                    }

                    x = 0;
                    y += LevelLoader.Size;

                    indexI = 0;
                    indexJ++;

                } //end foreach

                lenghtX = LevelLoader.Size * str.Length; // длина уровня в пикселях
                lenghtY = y;

                Guards.SetLevelMap(levelMap, lenghtX / LevelLoader.Size, lenghtY / LevelLoader.Size);
            }


        }





        /// <summary>
        /// Фунция возвращает тип стены.
        /// Анализирует соседние клетки и делает вывод.
        /// </summary>
        /// <param name="array">Весь массив уровня</param>
        /// <param name="i">Текущая строка</param>
        /// <param name="j">Текущая колонка</param>
        /// <param name="iEnd">Максимальная строка</param>
        /// <param name="jEnd">Максимальная колонка</param>
        /// <returns>Тип стены в веди int</returns>
        int isWallType(byte[,] array, int i, int j, int iEnd, int jEnd)
        {

            // проверка краевых точек массива

            /*
             * *----
             * -----
             * -----
             * -----
             */
            if (i == 0 && j == 0 && isWall(array[i, j + 1]) && isWall(array[i + 1, j]))
            {
                return 3;
            }

            /*
             * -----
             * -----
             * -----
             * *----
             */
            if (i == 0 && j == jEnd && isWall(array[i, j - 1]) && isWall(array[i + 1, j]))
            {
                return 4;
            }

            /*
             * ----*
             * -----
             * -----
             * -----
             */
            if (i == iEnd && j == 0 && isWall(array[i - 1, j]) && isWall(array[i, j + 1]))
            {
                return 5;
            }

            /*
             * -----
             * -----
             * -----
             * ----*
             */
            if (i == iEnd && j == jEnd && isWall(array[i, j - 1]) && isWall(array[i - 1, j]))
            {
                return 6;
            }


            // проверяем крайнии линии (краевые точки уже проверили и сделали return)
            /*
             * -----
             * *----
             * *----
             * -----
             */
            if (i == 0) {

                //  *
                //  **
                //  *
                if (isWall(array[i, j - 1]) && isWall(array[i, j + 1]) && isWall(array[i + 1, j]))
                {
                    return 8;
                }

                // *
                // *
                if (isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
                {
                    return 2;
                }

                //  **
                //  *
                if (isWall(array[i, j + 1]) && isWall(array[i + 1, j]))
                {
                    return 3;
                }

                // *
                // **
                if (isWall(array[i + 1, j]) && isWall(array[i, j - 1]))
                {
                    return 4;
                }
                

            }

            /*
             * -----
             * ----*
             * ----*
             * -----
             */
            if (i == iEnd)
            {
                //  *
                // **
                //  *
                if (isWall(array[i - 1, j]) && isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
                {
                    return 10;
                }

                // *
                // *
                if (isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
                {
                    return 2;
                }

                // **
                //  *
                if (isWall(array[i, j + 1]) && isWall(array[i - 1, j]))
                {
                    return 5;
                }

                //  *
                // **
                if (isWall(array[i - 1, j]) && isWall(array[i, j - 1]))
                {
                    return 6;
                }
            }

            /*
             * -----
             * *----
             * *----
             * -----
             */
            if (j == 0) 
            {
                // ***
                //  *
                if (isWall(array[i, j + 1]) && isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
                {
                    return 9;
                }
                
                // **
                if (isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
                {
                    return 1;
                }
                // **
                // *
                if (isWall(array[i + 1, j]) && isWall(array[i, j + 1]))
                {
                    return 3;
                }
                // **
                //  *
                if (isWall(array[i - 1, j]) && isWall(array[i, j + 1]))
                {
                    return 5;
                }
            }

            /*
             * -----
             * ----*
             * ----*
             * -----
             */
            if (j == jEnd)
            {
                //  *
                // ***
                if (isWall(array[i, j - 1]) && isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
                {
                    return 11;
                }

                // **
                if (isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
                {
                    return 1;
                }
                //  *
                // **
                if (isWall(array[i, j - 1]) && isWall(array[i - 1, j]))
                {
                    return 6;
                }
                // *
                // **
                if (isWall(array[i + 1, j]) && isWall(array[i, j - 1]))
                {
                    return 4;
                }
            }


            // проверяем центральную часть уровня
            /*
             * -----
             * -***-
             * -***-
             * -----
             */

            //  *
            // ***
            //  *
            if (isWall(array[i - 1, j]) && isWall(array[i + 1, j]) && isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
            {
                return 7;
            }

            // *
            // **
            // *
            if (isWall(array[i, j - 1]) && isWall(array[i, j + 1]) && isWall(array[i + 1, j]))
            {
                return 8;
            }

            //  *
            // **
            //  *
            if (isWall(array[i - 1, j]) && isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
            {
                return 10;
            }

            //  *
            // ***
            if (isWall(array[i, j - 1]) && isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
            {
                return 11;
            }

            // ***
            //  *
            if (isWall(array[i, j + 1]) && isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
            {
                return 9;
            }


            // **
            if (isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
            {
                return 2;
            }

            // *
            // *
            if (isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
            {
                return 1;
            }

            // *
            // **
            if (isWall(array[i + 1, j]) && isWall(array[i, j - 1]))
            {
                return 4;
            }

            //  *
            // **
            if (isWall(array[i, j - 1]) && isWall(array[i - 1, j]))
            {
                return 6;
            }

            // **
            //  *
            if (isWall(array[i - 1, j]) && isWall(array[i, j + 1]))
            {
                return 5;
            }

            // **
            // *
            if (isWall(array[i + 1, j]) && isWall(array[i, j + 1]))
            {
                return 3;
            }


            // если попали сюда - то неизвестная стена - возвращаем пустое место
            return 0;








        }

        /// <summary>
        /// Проверяет, является ли текущая клетка стеной
        /// </summary>
        /// <param name="element">Значение ячейки карты</param>
        /// <returns>true - стена, false - нет</returns>
        bool isWall(byte element)
        {
            if (element > 0 && element <= 29) {  // стены 1-11 и двери 20-23
                return true;
            }
            return false;
        }


    }
}
