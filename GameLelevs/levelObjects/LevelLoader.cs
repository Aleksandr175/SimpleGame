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

        
        
        public LevelLoader(Game1 game, Player player, Storage storage)
        {
            this.game = game;
            this.player = player;
            this.camera = new Camera();
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




                int[,] tempArray = new int[sizeFile[0] + 1, sizeFile[1] + 1]; //временный массив, используется для преобразований
                int[,] map = new int[sizeFile[0] + 1, sizeFile[1] + 1]; // карта уровня после преобразования

                // преобразование значений объектов (стен) на уровне
                for (int i = 0; i <= sizeFile[0]; i++)
                {
                    for (int j = 0; j <= sizeFile[1]; j++)
                    {


                        map[i, j] = 0;


                        if (i > 0 && i < sizeFile[0] && j > 0 && j < sizeFile[1])
                        {
                            if (isWallAround(levelMap, i, j, sizeFile[0], sizeFile[1]))
                            {
                                map[i, j] = 7; // wall4sides
                                break;
                            }






                        }


                        if (isWallGoriz(levelMap, i, j, sizeFile[0], sizeFile[1]))
                        {
                            map[i, j] = 2; // почему-то надо наоборот с вертикальной стеной
                        }
                        if (isWallVertic(levelMap, i, j, sizeFile[0], sizeFile[1]))
                        {
                            map[i, j] = 1; // почему-то надо наоборот с горизонтальной стеной
                        }

                        //isWallTurn();


                    }
                }



                // создание объектов на уровне
                for (int i = 0; i <= sizeFile[0]; i++)
                {
                    for (int j = 0; j <= sizeFile[1]; j++)
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
                        if (levelMap[i, j] == 3)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_down_right"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 4)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_up_right"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 5)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_down"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 6)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_up"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == 7)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_4sides"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 8)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_urd"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 9)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_rdl"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 10)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_dlu"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 11)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_lur"), game, this.camera);
                            blocks.Add(block);
                        }


                        //двери
                        if (levelMap[i, j] == 20)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 21)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 22)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_horiz_open"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (levelMap[i, j] == 23)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("door_vertic_open"), game, this.camera);
                            blocks.Add(block);
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
                        }
                        if (levelMap[i, j] == 41)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("card"), game, this.camera);
                            objs.Add(obj);
                        }

                        // золото
                        if (levelMap[i, j] == 50)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("money"), game, this.camera);
                            objs.Add(obj);
                        }

                        // стол системы управления камерами
                        if (levelMap[i, j] == 60)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spLU"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 61)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spUR"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 62)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spRD"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 63)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spDL"), game, this.camera);
                            objs.Add(obj);
                        }

                        // стол с компьютером
                        if (levelMap[i, j] == 70)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableU"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 71)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableR"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 72)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableD"), game, this.camera);
                            objs.Add(obj);
                        }
                        if (levelMap[i, j] == 73)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableL"), game, this.camera);
                            objs.Add(obj);
                        }
                    }
                }
                // конец создание объектов на уровне

                lenghtX = LevelLoader.Size * sizeFile[1]; // длина уровня в пикселях
                lenghtY = LevelLoader.Size * sizeFile[0];


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
        /// 
        /// </summary>
        /// <param name="array">Карта уровня</param>
        /// <param name="i">Номер текущей строки</param>
        /// <param name="j">Номер текущей колонки</param>
        /// <param name="iEnd">Всего строк</param>
        /// <param name="jEnd">Всего колонок</param>
        /// <returns></returns>
        bool isWallGoriz(byte[,] array, int i, int j, int iEnd, int jEnd)
        {

            if (i == 0)
            {
                if (array[i, j] == 1 && array[i + 1, j] == 0)
                {
                    return true;
                }
            }
            else
            {
                if (i == iEnd)
                {
                    if (array[i, j] == 1 && array[i - 1, j] == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (array[i, j] == 1 && array[i - 1, j] == 0 && array[i + 1, j] == 0)
                    {
                        return true;
                    }
                    return false;
                }
            }

            return false;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="iEnd"></param>
        /// <param name="jEnd"></param>
        /// <returns></returns>
        bool isWallVertic(byte[,] array, int i, int j, int iEnd, int jEnd)
        {

            if (j == 0)
            {
                if (array[i, j] == 1 && array[i, j + 1] == 0)
                {
                    return true;
                }
            }
            else
            {
                if (j == jEnd)
                {
                    if (array[i, j] == 1 && array[i, j - 1] == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (array[i, j] == 1 && array[i, j - 1] == 0 && array[i, j + 1] == 0)
                    {
                        return true;
                    }
                    return false;
                }
            }

            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="iEnd"></param>
        /// <param name="jEnd"></param>
        /// <returns></returns>
        bool isWallAround(byte[,] array, int i, int j, int iEnd, int jEnd)
        {
            if (array[i, j] == 1 && array[i + 1, j] == 1 && array[i - 1, j] == 1 && array[i, j + 1] == 1 && array[i, j - 1] == 1)
                return true;
            else
                return false;
        }













    }
}
