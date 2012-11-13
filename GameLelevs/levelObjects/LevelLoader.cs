using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Enumerations;
using GameLevels.levelObjects;
using GameLevels.levelObjects.door;

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
        public List<Laser> lasers; // список охранников

        // список "интерактивных" объектов
        public List<BaseObject> interactionSubjects;

        // список дверей
        public List<BaseObject> doors;

        // сложность уровня
        public Complexity complexity;

        // карта уровня
        // так же используется для алгоритма поиска пути к игроку
        public LevelObject[,] levelMap; // карта уровня в "названиях"
        public LevelObject[,] levelMapFloor; // карта пола
        public LevelObject[,] levelDoors; // карта дверей
        public static int[,] levelMapRooms; // карта комнат
        

        
        public LevelLoader(Game1 game, Player player, Storage storage, Camera camera)
        {
            this.game = game;
            this.player = player;
            this.camera = camera;
            this.storage = storage;
            storage = new Storage();
        }



        static int sizePeople = 25; //размер изображения игрока и охранников
        public static int SizePeople
        {
            get { return sizePeople; }
        }

        //размер 1 ячейки уровня
        static int size = 40;
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
        /// Файл должен быть с названием "lvlX.txt";
        /// </summary>
        /// <param name="lvl">номер уровня</param>
        public void CreateLevel(int lvl)
        {
            blocks = new List<Block>();
            objs = new List<Object>();
            guards = new List<Guards>();
            lasers = new List<Laser>();
            interactionSubjects = new List<BaseObject>();
            doors = new List<BaseObject>();

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
                
                //считываем размеры массива с уровнем (sizeFile[0] значение - строки, sizeFile[1] - колонки)
                str = lines[0].Split(' ');
                foreach (string s in str)
                {
                    sizeFile[tempIndex] = Convert.ToInt32(s);
                    tempIndex++;
                    if (tempIndex == 2) { break; }
                }
                    

                // выделим память для карты уровня
                levelMap = new LevelObject[sizeFile[1] + 1, sizeFile[0] + 1];
                //выделим память для дверей уровня
                levelDoors = new LevelObject[sizeFile[1] + 1, sizeFile[0] + 1];


                tempIndex = 0;
                //считывание уровня из файла
                for (int i = 0; i <= sizeFile[0]; i++) {

                    if (tempIndex == 0) { tempIndex++; continue; } // пропускаем первую строку с данными размера уровня

                    str = lines[i].Split(' ');
                    foreach (string s in str)
                    {
                        levelMap[indexI, indexJ] = LevelObject.Empty;
                        LevelObject obj = (LevelObject)Int32.Parse(s);
                        
                        switch(obj)
                        {
                            case LevelObject.Empty:
                                levelMap[indexI, indexJ] = LevelObject.Empty;
                                break;
                            case LevelObject.Wall:
                                levelMap[indexI, indexJ] = LevelObject.Wall;
                                break;

                            case LevelObject.DoorHoriz:
                                levelMap[indexI, indexJ] = LevelObject.DoorHoriz;
                                levelDoors[indexI, indexJ] = LevelObject.DoorHoriz;
                                break;
                            case LevelObject.DoorVertic:
                                levelMap[indexI, indexJ] = LevelObject.DoorVertic;
                                levelDoors[indexI, indexJ] = LevelObject.DoorVertic;
                                break;
                            case LevelObject.DoorHorizOpen:
                                levelMap[indexI, indexJ] = LevelObject.DoorHorizOpen;
                                levelDoors[indexI, indexJ] = LevelObject.DoorHorizOpen;
                                break;
                            case LevelObject.DoorVerticOpen:
                                levelMap[indexI, indexJ] = LevelObject.DoorVerticOpen;
                                levelDoors[indexI, indexJ] = LevelObject.DoorVerticOpen;
                                break;

                            case LevelObject.Guard:
                                levelMap[indexI, indexJ] = LevelObject.Guard;
                                break;

                            case LevelObject.Key:
                                levelMap[indexI, indexJ] = LevelObject.Key;
                                break;

                            // пластиковая карта
                            case LevelObject.Card:
                                levelMap[indexI, indexJ] = LevelObject.Card;
                                break;

                            case LevelObject.Gold:
                                levelMap[indexI, indexJ] = LevelObject.Gold;
                                break;

                            //стол управления камерами
                            case LevelObject.SpLU:
                                levelMap[indexI, indexJ] = LevelObject.SpLU;
                                break;
                            case LevelObject.SpUR:
                                levelMap[indexI, indexJ] = LevelObject.SpUR;
                                break;
                            case LevelObject.SpRD:
                                levelMap[indexI, indexJ] = LevelObject.SpRD;
                                break;
                            case LevelObject.SpDL:
                                levelMap[indexI, indexJ] = LevelObject.SpDL;
                                break;

                            //стол с компьютером
                            case LevelObject.TableU:
                                levelMap[indexI, indexJ] = LevelObject.TableU;
                                break;
                            case LevelObject.TableR:
                                levelMap[indexI, indexJ] = LevelObject.TableR;
                                break;
                            case LevelObject.TableD:
                                levelMap[indexI, indexJ] = LevelObject.TableD;
                                break;
                            case LevelObject.TableL:
                                levelMap[indexI, indexJ] = LevelObject.TableL;
                                break;

                            // лазеры
                            case LevelObject.LaserHoriz:
                                levelMap[indexI, indexJ] = LevelObject.LaserHoriz;
                                break;
                            case LevelObject.LaserVertic:
                                levelMap[indexI, indexJ] = LevelObject.LaserVertic;
                                break;
                            case LevelObject.LaserHorizL:
                                levelMap[indexI, indexJ] = LevelObject.LaserHoriz;
                                break;
                            case LevelObject.LaserHorizR:
                                levelMap[indexI, indexJ] = LevelObject.LaserHoriz;
                                break;
                            case LevelObject.LaserVerticU:
                                levelMap[indexI, indexJ] = LevelObject.LaserHoriz;
                                break;
                            case LevelObject.LaserVerticD:
                                levelMap[indexI, indexJ] = LevelObject.LaserHoriz;
                                break;
                            case LevelObject.LaserHorizMiddle:
                                levelMap[indexI, indexJ] = LevelObject.LaserHoriz;
                                break;
                            case LevelObject.LaserVerticMiddle:
                                levelMap[indexI, indexJ] = LevelObject.LaserHoriz;
                                break;




                            // устанавливаем игрока и камеру в нач. позицию.
                            case LevelObject.Player:
                                int playerPosX = indexJ * size;
                                int playerPosY = indexI * size;

                                //размещаем игрока в начальную позицию
                                player.Position = new Rectangle(playerPosY, playerPosX, LevelLoader.SizePeople, LevelLoader.SizePeople);
                                camera.ScrollX = playerPosY - game.GetScreenWidth / 2 + size / 2;
                                camera.ScrollY = playerPosX - game.GetScreenHeight / 2 + size / 2;

                                // установка камеры в начальную позицию
                                if (camera.ScrollX < 0)
                                {
                                    camera.ScrollX = 0;
                                }
                                if (camera.ScrollY < 0)
                                {
                                    camera.ScrollY = 0;
                                }
                                if (camera.ScrollX > game.GetScreenWidth)
                                {
                                    camera.ScrollX = game.GetScreenWidth;
                                }
                                if (camera.ScrollY > game.GetScreenHeight)
                                {
                                    camera.ScrollY = game.GetScreenHeight;
                                }
                                break;


                        }

                        indexI++;

                    }
                    indexI = 0;
                    indexJ++;

                }



                indexI = 0;
                indexJ = 0;

                //if (lvl == 7 || lvl == 8)
               // {
                    // выделим память для карты уровня
                    levelMapFloor = new LevelObject[sizeFile[1] + 1, sizeFile[0] + 1];

                    //считывание пола в игре
                    for (int i = sizeFile[0] + 1; i <= 2 * sizeFile[0]; i++)
                    {
                        str = lines[i].Split(' ');
                        foreach (string s in str)
                        {

                            levelMapFloor[indexI, indexJ] = LevelObject.Empty;
                            // пол в игре
                            if (s.Equals("90", StringComparison.OrdinalIgnoreCase))
                            {
                                levelMapFloor[indexI, indexJ] = LevelObject.Floor;
                            }
                            indexI++;
                        }
                        indexI = 0;
                        indexJ++;
                    }

                //}

                //if (lvl == 7 || lvl == 8)
                //{
                    indexI = 0;
                    indexJ = 0;

                    // выделим память для комнат уровня
                    levelMapRooms = new int[sizeFile[1] + 1, sizeFile[0] + 1];

                    //считывание комнат в игре из файла
                    for (int i = sizeFile[0] * 2 + 1; i <= 3 * sizeFile[0]; i++)
                    {
                        str = lines[i].Split(' ');
                        foreach (string s in str)
                        {
                            //заносим конаты в массив
                            levelMapRooms[indexI, indexJ] = Convert.ToInt32(s);
                            indexI++;
                        }
                        indexI = 0;
                        indexJ++;
                    }
                //}



                LevelObject[,] map = new LevelObject[sizeFile[1] + 1, sizeFile[0] + 1]; // карта уровня после преобразования

                // преобразование значений объектов (стен) на уровне
                for (int i = 0; i < sizeFile[1]; i++)
                {
                    for (int j = 0; j < sizeFile[0]; j++)
                    {


                        map[i, j] = 0;
                        
                        if (levelMap[i, j] == LevelObject.Wall)
                        {
                            map[i, j] = isWallType(levelMap, i, j, sizeFile[0] - 1, sizeFile[1] - 1);
                        }
                        

                    }
                }





                // создание пола на уровне
                // пол
                // берется из 2 массива уровня в файле.
                // 1 1 2 3 1 2
                // ...........
                // 1 2 3 4 1 2 - осн. массив уровня
                // 90 90 90 90 - далее идет массив пола
                // 0 90 90 0
                // ..........  
                //if (lvl == 7 || lvl == 8)
                //{
                    for (int i = 0; i < sizeFile[1]; i++)
                    {
                        for (int j = sizeFile[0]; j <= 2 * sizeFile[0]; j++)
                        {
                            Rectangle Rect = new Rectangle(i * LevelLoader.Size - LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                            //Rectangle Rect2 = new Rectangle((i + 1) * LevelLoader.Size - LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                            //Rectangle Rect3 = new Rectangle(i * LevelLoader.Size - LevelLoader.Size / 2, ((j - sizeFile[0]) + 1) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);

                            if (levelMapFloor[i, j - sizeFile[0]] == LevelObject.Floor)
                            {
                                Block block = new Block(Rect, storage.Pull2DTexture("floor"), game, this.camera);
                                blocks.Add(block);
                            }

                            // - пустое место
                            // * пол

                            // *-
                            // *-
                            if (j - sizeFile[0] - 1 > 0) // не выходим за границы массива
                            {
                                if (levelMapFloor[i, j - sizeFile[0] - 1] == LevelObject.Floor) // если клетка выше - пол, то и ниже тоже пол
                                {
                                    Rectangle Rect2 = new Rectangle(i * LevelLoader.Size - LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                                    Block block = new Block(Rect2, storage.Pull2DTexture("floor"), game, this.camera);
                                    blocks.Add(block);
                                }
                            }

                            // **
                            // --
                            if (i - 1 > 0) // не выходим за границы массива
                            {

                                if (levelMapFloor[i - 1, j - sizeFile[0]] == LevelObject.Floor) // если клетка выше - пол, то и ниже тоже пол
                                {
                                    Rectangle Rect2 = new Rectangle(i * LevelLoader.Size - LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                                    Block block = new Block(Rect2, storage.Pull2DTexture("floor"), game, this.camera);
                                    blocks.Add(block);
                                }

                            }

                            // *-
                            // -*
                            if (i - 1 > 0 && j - sizeFile[0] - 1 > 0) // не выходим за границы массива
                            {

                                if (levelMapFloor[i - 1, j - sizeFile[0] - 1] == LevelObject.Floor) // если клетка выше - пол, то и ниже тоже пол
                                {
                                    Rectangle Rect2 = new Rectangle(i * LevelLoader.Size - LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                                    Block block = new Block(Rect2, storage.Pull2DTexture("floor"), game, this.camera);
                                    blocks.Add(block);
                                }

                            }

                        }
                    }
                //}
                // конец создания пола


                // создание объектов на уровне
                for (int i = 0; i < sizeFile[1]; i++)
                {
                    for (int j = 0; j < sizeFile[0]; j++)
                    {
                        Rectangle Rect = new Rectangle(i * LevelLoader.Size, j * LevelLoader.Size, LevelLoader.Size, LevelLoader.Size);
                        if (map[i, j] == LevelObject.Empty)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.Wall)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_goriz"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.WallVertic)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_vert"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.WallDownRight)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_down_right"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.WallUpRight)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_up_right"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.WallLeftDown)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_down"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.WallLeftUp)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_up"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.Wall4Sides)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_4sides"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.WallURD)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_urd"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.WallRDL)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_rdl"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.WallDLU)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_dlu"), game, this.camera);
                            blocks.Add(block);
                        }
                        if (map[i, j] == LevelObject.WallLUR)
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_lur"), game, this.camera);
                            blocks.Add(block);
                        }


                        //двери
                        if (levelMap[i, j] == LevelObject.DoorHoriz)
                        {
                            // Block block = new Block(Rect, storage.Pull2DTexture("door_horiz"), game, this.camera);
                            // blocks.Add(block);

                            Door door = new Door(Rect, storage.Pull2DTexture("door_horiz"), game, this.camera, EColor.Blue, DoorOrientation.Horiz, true, i, j);
                            doors.Add(door);

                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.DoorVertic)
                        {
                            // Block block = new Block(Rect, storage.Pull2DTexture("door_vertic"), game, this.camera);
                            // blocks.Add(block);

                            Door door = new Door(Rect, storage.Pull2DTexture("door_vertic"), game, this.camera, EColor.Blue, DoorOrientation.Vert, true, i, j);
                            doors.Add(door);
                            
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.DoorHorizOpen)
                        {
                            // Block block = new Block(Rect, storage.Pull2DTexture("door_horiz_open"), game, this.camera);
                            // blocks.Add(block);

                            Door door = new Door(Rect, storage.Pull2DTexture("door_horiz_open"), game, this.camera, EColor.Blue, DoorOrientation.Horiz);
                            doors.Add(door);
                            
                            levelMap[i, j] = LevelObject.Empty;
                        }
                        if (levelMap[i, j] == LevelObject.DoorVerticOpen)
                        {
                            // Block block = new Block(Rect, storage.Pull2DTexture("door_vertic_open"), game, this.camera);
                            // blocks.Add(block);

                            Door door = new Door(Rect, storage.Pull2DTexture("door_vertic_open"), game, this.camera, EColor.Blue, DoorOrientation.Vert);
                            doors.Add(door);

                            levelMap[i, j] = LevelObject.Empty;
                        }

                        if (levelMap[i, j] == LevelObject.Guard)
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
                        if (levelMap[i, j] == LevelObject.Key)
                        {
                            // Object obj = new Object(Rect, storage.Pull2DTexture("key"), game, this.camera);
                            // objs.Add(obj);

                            Key key = new Key(Rect, storage.Pull2DTexture("key"), game, this.camera, EColor.Blue);
                            interactionSubjects.Add(key);

                            levelMap[i, j] = LevelObject.Empty;
                        }
                        if (levelMap[i, j] == LevelObject.Card)
                        {
                            // Object obj = new Object(Rect, storage.Pull2DTexture("card"), game, this.camera);
                            // objs.Add(obj);

                            Card card = new Card(Rect, storage.Pull2DTexture("card"), game, this.camera);
                            interactionSubjects.Add(card);

                            levelMap[i, j] = LevelObject.Empty;
                        }

                        // золото
                        if (levelMap[i, j] == LevelObject.Gold)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("money"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Empty;
                        }

                        // стол системы управления камерами
                        if (levelMap[i, j] == LevelObject.SpLU)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spLU"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.SpUR)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spUR"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.SpRD)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spRD"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.SpDL)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("spDL"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }

                        // стол с компьютером
                        if (levelMap[i, j] == LevelObject.TableU)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableU"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.TableR)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableR"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.TableD)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableD"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.TableL)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("tableL"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }



                        // лазеры
                        if (levelMap[i, j] == LevelObject.LaserHoriz) 
                        {
                            Laser laser = new Laser(Rect, storage.Pull2DTexture("laser2_horiz"), game, this.camera);
                            lasers.Add(laser);
                            //levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.LaserVertic) 
                        {
                            Laser laser = new Laser(Rect, storage.Pull2DTexture("laser2_vert"), game, this.camera);
                            lasers.Add(laser);
                            //levelMap[i, j] = LevelObject.Wall;
                        }

                        if (levelMap[i, j] == LevelObject.LaserHorizL) 
                        {
                            Laser laser = new Laser(Rect, storage.Pull2DTexture("laser3_L"), game, this.camera);
                            lasers.Add(laser);
                            //levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.LaserHorizR) 
                        {
                            Laser laser = new Laser(Rect, storage.Pull2DTexture("laser3_R"), game, this.camera);
                            lasers.Add(laser);
                            //levelMap[i, j] = LevelObject.Wall;
                        }

                        


                        y += LevelLoader.Size;

                    }

                    y = 0;
                    x += LevelLoader.Size;

                }
                // конец создание объектов на уровне

                lenghtX = LevelLoader.Size * sizeFile[1]; // длина уровня в пикселях
                lenghtY = LevelLoader.Size * sizeFile[0];




                //считывание пути траекторий охранников
                lines = File.ReadAllLines(lvl_name); //получили массив строк                

                //считываем размеры массива с уровнем (sizeFile[0] значение - строки, sizeFile[1] - колонки)
                for (int i = sizeFile[0] + 1; i < lines.Length; i++)
                {
                    int step = 0;
                    int numberGuard = 0;
                    int nextX = 0;
                    int nextY = 0;

                    str = lines[i].Split(' ');
                    LevelObject obj = (LevelObject)Int32.Parse(str[0]);

                    if (obj == LevelObject.Guard)
                    {
                        step = Convert.ToInt32(str[2]);
                        numberGuard = Convert.ToInt32(str[1]);
                        nextX = Convert.ToInt32(str[4]);
                        nextY = Convert.ToInt32(str[3]);

                        // добавляем новую строку под координату
                        guards[numberGuard].wayToPatrol.Add(new List<int>());

                        //задание начальной точки следования для охранника
                        if (step == 0)
                        {
                            guards[numberGuard].TargetX = nextX;
                            guards[numberGuard].TargetY = nextY;
                        }

                        // задание точки патрилрования
                        guards[numberGuard].wayToPatrol[step].Add(nextX);
                        guards[numberGuard].wayToPatrol[step].Add(nextY);
                        //максимальное кол-во шагов.
                        guards[numberGuard].MaxStepToPatrol++;


                    }
                }


                Guards.SetLevelMap(levelMap, lenghtX / LevelLoader.Size, lenghtY / LevelLoader.Size);

                
            }




            // ЕСЛИ УРОВЕНЬ НЕ ИЗ РЕДАКТОРА, ТО ОБЫЧНАЯ ЗАГРУЗКА
            else
            {
                // выделим память для карты уровня
                levelMap = new LevelObject[lines[0].Length, lines.Length];


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
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("2", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_vert"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("3", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_down_right"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("4", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_up_right"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("5", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_down"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("6", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_left_up"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("7", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_4sides"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("8", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_urd"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("9", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_rdl"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("10", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_dlu"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("11", StringComparison.OrdinalIgnoreCase))
                        {
                            Block block = new Block(Rect, storage.Pull2DTexture("wall_lur"), game, this.camera);
                            blocks.Add(block);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }


                        //двери
                        if (s.Equals("20", StringComparison.OrdinalIgnoreCase))
                        {
                            // Block block = new Block(Rect, storage.Pull2DTexture("door_horiz"), game, this.camera);
                            // blocks.Add(block);

                            Door door = new Door(Rect, storage.Pull2DTexture("door_horiz"), game, this.camera, EColor.Blue, DoorOrientation.Horiz, true, indexI, indexJ);
                            doors.Add(door);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("21", StringComparison.OrdinalIgnoreCase))
                        {
                            // Block block = new Block(Rect, storage.Pull2DTexture("door_vertic"), game, this.camera);
                            // blocks.Add(block);

                            Door door = new Door(Rect, storage.Pull2DTexture("door_vertic"), game, this.camera, EColor.Blue, DoorOrientation.Vert, true, indexI, indexJ);
                            doors.Add(door);

                            // добавим стену в карту
                            levelMap[indexI, indexJ] = LevelObject.Wall;
                        }
                        if (s.Equals("22", StringComparison.OrdinalIgnoreCase))
                        {
                            // Block block = new Block(Rect, storage.Pull2DTexture("door_horiz_open"), game, this.camera);
                            // blocks.Add(block);

                            Door door = new Door(Rect, storage.Pull2DTexture("door_horiz_open"), game, this.camera, EColor.Blue, DoorOrientation.Horiz);
                            doors.Add(door);
                        }
                        if (s.Equals("23", StringComparison.OrdinalIgnoreCase))
                        {
                            // Block block = new Block(Rect, storage.Pull2DTexture("door_vertic_open"), game, this.camera);
                            // blocks.Add(block);

                            Door door = new Door(Rect, storage.Pull2DTexture("door_vertic_open"), game, this.camera, EColor.Blue, DoorOrientation.Vert);
                            doors.Add(door);
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
                            // Object obj = new Object(Rect, storage.Pull2DTexture("key"), game, this.camera);
                            // objs.Add(obj);

                            Key key = new Key(Rect, storage.Pull2DTexture("key"), game, this.camera, EColor.Blue);
                            interactionSubjects.Add(key);
                        }
                        if (s.Equals("41", StringComparison.OrdinalIgnoreCase))
                        {
                            // Object obj = new Object(Rect, storage.Pull2DTexture("card"), game, this.camera);
                            // objs.Add(obj);

                            Card card = new Card(Rect, storage.Pull2DTexture("card"), game, this.camera);
                            interactionSubjects.Add(card);
                        }

                        // золото
                        if (s.Equals("50", StringComparison.OrdinalIgnoreCase))
                        {
                            // Object obj = new Object(Rect, storage.Pull2DTexture("money"), game, this.camera);
                            // objs.Add(obj);

                            Money money = new Money(Rect, storage.Pull2DTexture("money"), game, this.camera, 10);
                            interactionSubjects.Add(money);
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

                //Guards.SetLevelMap(levelMap, lenghtX / LevelLoader.Size, lenghtY / LevelLoader.Size);
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
        LevelObject isWallType(LevelObject[,] array, int i, int j, int iEnd, int jEnd)
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
                return LevelObject.WallDownRight;
            }

            /*
             * -----
             * -----
             * -----
             * *----
             */
            if (i == 0 && j == jEnd && isWall(array[i, j - 1]) && isWall(array[i + 1, j]))
            {
                return LevelObject.WallUpRight;
            }

            /*
             * ----*
             * -----
             * -----
             * -----
             */
            if (i == iEnd && j == 0 && isWall(array[i - 1, j]) && isWall(array[i, j + 1]))
            {
                return LevelObject.WallLeftDown;
            }

            /*
             * -----
             * -----
             * -----
             * ----*
             */
            if (i == iEnd && j == jEnd && isWall(array[i, j - 1]) && isWall(array[i - 1, j]))
            {
                return LevelObject.WallLeftUp;
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
                    return LevelObject.WallURD;
                }

                // *
                // *
                if (isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
                {
                    return LevelObject.WallVertic;
                }

                //  **
                //  *
                if (isWall(array[i, j + 1]) && isWall(array[i + 1, j]))
                {
                    return LevelObject.WallDownRight;
                }

                // *
                // **
                if (isWall(array[i + 1, j]) && isWall(array[i, j - 1]))
                {
                    return LevelObject.WallUpRight;
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
                    return LevelObject.WallDLU;
                }

                // *
                // *
                if (isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
                {
                    return LevelObject.WallVertic;
                }

                // **
                //  *
                if (isWall(array[i, j + 1]) && isWall(array[i - 1, j]))
                {
                    return LevelObject.WallLeftDown;
                }

                //  *
                // **
                if (isWall(array[i - 1, j]) && isWall(array[i, j - 1]))
                {
                    return LevelObject.WallLeftUp;
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
                    return LevelObject.WallRDL;
                }
                
                // **
                if (isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
                {
                    return LevelObject.Wall;
                }
                // **
                // *
                if (isWall(array[i + 1, j]) && isWall(array[i, j + 1]))
                {
                    return LevelObject.WallDownRight;
                }
                // **
                //  *
                if (isWall(array[i - 1, j]) && isWall(array[i, j + 1]))
                {
                    return LevelObject.WallLeftDown;
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
                    return LevelObject.WallLUR;
                }

                // **
                if (isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
                {
                    return LevelObject.Wall;
                }
                //  *
                // **
                if (isWall(array[i, j - 1]) && isWall(array[i - 1, j]))
                {
                    return LevelObject.WallLeftUp;
                }
                // *
                // **
                if (isWall(array[i + 1, j]) && isWall(array[i, j - 1]))
                {
                    return LevelObject.WallUpRight;
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
                return LevelObject.Wall4Sides;
            }

            // *
            // **
            // *
            if (isWall(array[i, j - 1]) && isWall(array[i, j + 1]) && isWall(array[i + 1, j]))
            {
                return LevelObject.WallURD;
            }

            //  *
            // **
            //  *
            if (isWall(array[i - 1, j]) && isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
            {
                return LevelObject.WallDLU;
            }

            //  *
            // ***
            if (isWall(array[i, j - 1]) && isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
            {
                return LevelObject.WallLUR;
            }

            // ***
            //  *
            if (isWall(array[i, j + 1]) && isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
            {
                return LevelObject.WallRDL;
            }


            // **
            if (isWall(array[i, j - 1]) && isWall(array[i, j + 1]))
            {
                return LevelObject.WallVertic;
            }

            // *
            // *
            if (isWall(array[i - 1, j]) && isWall(array[i + 1, j]))
            {
                return LevelObject.Wall;
            }

            // *
            // **
            if (isWall(array[i + 1, j]) && isWall(array[i, j - 1]))
            {
                return LevelObject.WallUpRight;
            }

            //  *
            // **
            if (isWall(array[i, j - 1]) && isWall(array[i - 1, j]))
            {
                return LevelObject.WallLeftUp;
            }

            // **
            //  *
            if (isWall(array[i - 1, j]) && isWall(array[i, j + 1]))
            {
                return LevelObject.WallLeftDown;
            }

            // **
            // *
            if (isWall(array[i + 1, j]) && isWall(array[i, j + 1]))
            {
                return LevelObject.WallDownRight;
            }


            // если попали сюда - то неизвестная стена - возвращаем пустое место
            return 0;








        }

        /// <summary>
        /// Проверяет, является ли текущая клетка стеной
        /// </summary>
        /// <param name="element">Значение ячейки карты</param>
        /// <returns>true - стена, false - нет</returns>
        bool isWall(LevelObject element)
        {
            if ((int)element > 0 && (int)element <= 29) {  // стены 1-11 и двери 20-23
                return true;
            }
            return false;
        }


    }
}
