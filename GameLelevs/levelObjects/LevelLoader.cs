﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Enumerations;
using GameLevels.levelObjects;
using GameLevels.levelObjects.door;
using GameLevels.levelObjects.money;

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
        public List<Cameras> cameras; // список охранников
        public List<SysControl> sysControls; // список столов управления камерами

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

        // количество прежметов, необходимых для перехода на новый уровень
        public int NumberOfJewelry { get; set; }
        
        public LevelLoader(Game1 game, Player player, Storage storage, Camera camera)
        {
            this.game = game;
            this.player = player;
            this.camera = camera;
            this.storage = storage;
            storage = new Storage();

            NumberOfJewelry = 0;
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
            cameras = new List<Cameras>();
            interactionSubjects = new List<BaseObject>();
            doors = new List<BaseObject>();
            sysControls = new List<SysControl>();
            NumberOfJewelry = 0;

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
            if (lvl >= 1)
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

                            // деревянные двери
                            case LevelObject.DoorWoodHoriz:
                                levelMap[indexI, indexJ] = LevelObject.DoorWoodHoriz;
                                levelDoors[indexI, indexJ] = LevelObject.DoorWoodHoriz;
                                break;
                            case LevelObject.DoorWoodVertic:
                                levelMap[indexI, indexJ] = LevelObject.DoorWoodVertic;
                                levelDoors[indexI, indexJ] = LevelObject.DoorWoodVertic;
                                break;
                            case LevelObject.DoorWoodHorizOpen:
                                levelMap[indexI, indexJ] = LevelObject.DoorWoodHorizOpen;
                                levelDoors[indexI, indexJ] = LevelObject.DoorWoodHorizOpen;
                                break;
                            case LevelObject.DoorWoodVerticOpen:
                                levelMap[indexI, indexJ] = LevelObject.DoorWoodVerticOpen;
                                levelDoors[indexI, indexJ] = LevelObject.DoorWoodVerticOpen;
                                break;

                            case LevelObject.Guard:
                                levelMap[indexI, indexJ] = LevelObject.Guard;
                                break;

                                // кресла
                            case LevelObject.Chairs_U:
                                levelMap[indexI, indexJ] = LevelObject.Chairs_U;
                                break;

                            case LevelObject.Chairs_R:
                                levelMap[indexI, indexJ] = LevelObject.Chairs_R;
                                break;

                            case LevelObject.Chairs_D:
                                levelMap[indexI, indexJ] = LevelObject.Chairs_D;
                                break;

                            case LevelObject.Chairs_L:
                                levelMap[indexI, indexJ] = LevelObject.Chairs_L;
                                break;

                                // диваны
                            case LevelObject.Sofa_U:
                                levelMap[indexI, indexJ] = LevelObject.Sofa_U;
                                break;
                            case LevelObject.Sofa_R:
                                levelMap[indexI, indexJ] = LevelObject.Sofa_R;
                                break;
                            case LevelObject.Sofa_D:
                                levelMap[indexI, indexJ] = LevelObject.Sofa_D;
                                break;
                            case LevelObject.Sofa_L:
                                levelMap[indexI, indexJ] = LevelObject.Sofa_L;
                                break;


                                

                            case LevelObject.Key:
                                levelMap[indexI, indexJ] = LevelObject.Key;
                                break;

                            // пластиковая карта
                            case LevelObject.Card:
                                levelMap[indexI, indexJ] = LevelObject.Card;
                                break;

                                // драгоценности для кражи
                            case LevelObject.Gold:
                                levelMap[indexI, indexJ] = LevelObject.Gold;
                                break;

                            case LevelObject.Rubin:
                                levelMap[indexI, indexJ] = LevelObject.Rubin;
                                break;
                            case LevelObject.Brilliant:
                                levelMap[indexI, indexJ] = LevelObject.Brilliant;
                                break;

                                // картины для кражи
                            case LevelObject.Picture1:
                                levelMap[indexI, indexJ] = LevelObject.Picture1;
                                break;
                            case LevelObject.Picture2:
                                levelMap[indexI, indexJ] = LevelObject.Picture2;
                                break;
                            case LevelObject.Picture3:
                                levelMap[indexI, indexJ] = LevelObject.Picture3;
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

                            // камеры
                            case LevelObject.CameraUL:
                                levelMap[indexI, indexJ] = LevelObject.CameraUL;
                                break;
                            case LevelObject.CameraUR:
                                levelMap[indexI, indexJ] = LevelObject.CameraUR;
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


                            // куст
                            case LevelObject.Plant:
                                levelMap[indexI, indexJ] = LevelObject.Plant;
                                break;


                            // лазеры
                            case LevelObject.LaserHoriz:
                                levelMap[indexI, indexJ] = LevelObject.LaserHoriz;
                                break;
                            case LevelObject.LaserVertic:
                                levelMap[indexI, indexJ] = LevelObject.LaserVertic;
                                break;
                            // движущиеся лазеры
                            case LevelObject.LaserHorizMoving:
                                levelMap[indexI, indexJ] = LevelObject.LaserHorizMoving;
                                break;
                            case LevelObject.LaserVerticMoving:
                                levelMap[indexI, indexJ] = LevelObject.LaserVerticMoving;
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

                
                // выделим память для карты уровня
                levelMapFloor = new LevelObject[sizeFile[1] + 1, sizeFile[0] + 1];
                // выделим память для комнат уровня
                levelMapRooms = new int[sizeFile[1] + 1, sizeFile[0] + 1];

                //считывание пола с комнатами в игре
                for (int i = sizeFile[0] + 1; i <= 2 * sizeFile[0]; i++)
                {
                    str = lines[i].Split(' ');
                    foreach (string s in str)
                    {

                        levelMapFloor[indexI, indexJ] = LevelObject.Empty;
                        // пол в игре
                        if (s.Equals("1", StringComparison.OrdinalIgnoreCase))
                        {
                            levelMapFloor[indexI, indexJ] = LevelObject.Floor; // пол
                            levelMapRooms[indexI, indexJ] = Convert.ToInt32(s); // номер комнаты
                        }
                        indexI++;
                    }
                    indexI = 0;
                    indexJ++;
                }
                



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
                if (lvl >=1)// || lvl == 8)
                {
                    for (int i = 0; i < sizeFile[1]; i++)
                    {
                        for (int j = sizeFile[0]; j <= 2 * sizeFile[0] - 1; j++)
                        {
                            Rectangle Rect = new Rectangle(i * LevelLoader.Size - LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                            //Rectangle Rect2 = new Rectangle((i + 1) * LevelLoader.Size - LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                            //Rectangle Rect3 = new Rectangle(i * LevelLoader.Size - LevelLoader.Size / 2, ((j - sizeFile[0]) + 1) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);


                            if (isFloor(levelMapFloor, i, j - sizeFile[0], sizeFile[1], sizeFile[0])) // i - колонки, j - строки
                            {
                                if (levelMapFloor[i, j - sizeFile[0]] == LevelObject.Floor)
                                {

                                    Block block = new Block(Rect, storage.Pull2DTexture("floor"), game, this.camera);
                                    blocks.Add(block);

                                    Rectangle Rect2 = new Rectangle(i * LevelLoader.Size - LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size + LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                                    Block block4 = new Block(Rect2, storage.Pull2DTexture("floor"), game, this.camera);
                                    blocks.Add(block4);
                                    
                                    Rect2 = new Rectangle(i * LevelLoader.Size + LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size - LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                                    Block block2 = new Block(Rect2, storage.Pull2DTexture("floor"), game, this.camera);
                                    blocks.Add(block2);

                                    Rect2 = new Rectangle(i * LevelLoader.Size + LevelLoader.Size / 2, (j - sizeFile[0]) * LevelLoader.Size + LevelLoader.Size / 2, LevelLoader.Size, LevelLoader.Size);
                                    Block block3 = new Block(Rect2, storage.Pull2DTexture("floor"), game, this.camera);
                                    blocks.Add(block3);
                                }


                            }

                        }
                    }
                }
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


                        //двери железные
                        if (levelMap[i, j] == LevelObject.DoorHoriz)
                        {
                            Door door = new Door(Rect, storage.Pull2DTexture("door_horiz"), game, this.camera, EColor.Blue, DoorOrientation.Horiz, true, i, j);
                            doors.Add(door);
                            door.posY = j;
                            door.posX = i;
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.DoorVertic)
                        {
                            Door door = new Door(Rect, storage.Pull2DTexture("door_vertic"), game, this.camera, EColor.Blue, DoorOrientation.Vert, true, i, j);
                            doors.Add(door);
                            door.posY = j;
                            door.posX = i;
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.DoorHorizOpen)
                        {
                            Door door = new Door(Rect, storage.Pull2DTexture("door_horiz_open"), game, this.camera, EColor.Blue, DoorOrientation.Horiz);
                            doors.Add(door);
                            door.posY = j;
                            door.posX = i;
                            levelMap[i, j] = LevelObject.Empty;
                        }
                        if (levelMap[i, j] == LevelObject.DoorVerticOpen)
                        {
                            Door door = new Door(Rect, storage.Pull2DTexture("door_vertic_open"), game, this.camera, EColor.Blue, DoorOrientation.Vert);
                            doors.Add(door);
                            door.posY = j;
                            door.posX = i;
                            levelMap[i, j] = LevelObject.Empty;
                        }



                        //двери деревянные
                        if (levelMap[i, j] == LevelObject.DoorWoodHoriz)
                        {
                            Door door = new Door(Rect, storage.Pull2DTexture("wood_door_horiz"), game, this.camera, EColor.Blue, DoorOrientation.HorizWood, true, i, j);
                            doors.Add(door);
                            door.posY = j;
                            door.posX = i;
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.DoorWoodVertic)
                        {
                            Door door = new Door(Rect, storage.Pull2DTexture("wood_door_vertic"), game, this.camera, EColor.Blue, DoorOrientation.VertWood, true, i, j);
                            doors.Add(door);
                            door.posY = j;
                            door.posX = i;
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.DoorWoodHorizOpen)
                        {
                            Door door = new Door(Rect, storage.Pull2DTexture("wood_door_horiz_open"), game, this.camera, EColor.Blue, DoorOrientation.HorizWood);
                            doors.Add(door);
                            door.posY = j;
                            door.posX = i;
                            levelMap[i, j] = LevelObject.Empty;
                        }
                        if (levelMap[i, j] == LevelObject.DoorWoodVerticOpen)
                        {
                            Door door = new Door(Rect, storage.Pull2DTexture("wood_door_vertic_open"), game, this.camera, EColor.Blue, DoorOrientation.VertWood);
                            doors.Add(door);
                            door.posY = j;
                            door.posX = i;
                            levelMap[i, j] = LevelObject.Empty;
                        }





                        if (levelMap[i, j] == LevelObject.Guard)
                        {
                            //пол
                            Block block = new Block(Rect, storage.Pull2DTexture("empty"), game, this.camera);
                            blocks.Add(block);

                            // инициализируем нового охранника
                            Rectangle RectGuard = new Rectangle(x + LevelLoader.SizePeople / 4, y + LevelLoader.SizePeople / 4, LevelLoader.SizePeople, LevelLoader.SizePeople);
                            Guards guard = new Guards(storage.Pull2DTexture("player"), storage.Pull2DTexture("player_run"), storage.Pull2DTexture("player_run_goriz"), storage.Pull2DTexture("guard_eye"), storage.Pull2DTexture("guard_eye_right"), RectGuard, game, player, this.camera, this);
                            guards.Add(guard);
                            guard.Run(PlayerMove.Left);
                        }

                        
                        // кресла
                        if (levelMap[i, j] == LevelObject.Chairs_U)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("chairs_U"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.Chairs_R)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("chairs_R"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.Chairs_D)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("chairs_D"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.Chairs_L)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("chairs_L"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }

                        // диваны
                        if (levelMap[i, j] == LevelObject.Sofa_U)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("sofa_U"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.Sofa_R)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("sofa_R"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.Sofa_D)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("sofa_D"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.Sofa_L)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("sofa_L"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }


                        // ключ и пластиковая карта
                        if (levelMap[i, j] == LevelObject.Key)
                        {
                            Key key = new Key(Rect, storage.Pull2DTexture("key"), game, this.camera, EColor.Blue);
                            key.posY = j;
                            key.posX = i;
                            interactionSubjects.Add(key);
                            levelMap[i, j] = LevelObject.Empty;
                        }
                        if (levelMap[i, j] == LevelObject.Card)
                        {
                            Card card = new Card(Rect, storage.Pull2DTexture("card"), game, this.camera);
                            card.posY = j;
                            card.posX = i;
                            interactionSubjects.Add(card);
                            levelMap[i, j] = LevelObject.Empty;
                        }

                        // золото
                        if (levelMap[i, j] == LevelObject.Gold)
                        {
                            Money money = new Money(Rect, storage.Pull2DTexture("money"), game, this.camera, 10);
                            interactionSubjects.Add(money); 
                            
                            levelMap[i, j] = LevelObject.Empty;
                        }

                        if (levelMap[i, j] == LevelObject.Rubin)
                        {
                            Rubin rubin = new Rubin(Rect, storage.Pull2DTexture("rubin"), game, this.camera, 20);
                            interactionSubjects.Add(rubin);

                            levelMap[i, j] = LevelObject.Empty;
                            NumberOfJewelry++;
                        }

                        if (levelMap[i, j] == LevelObject.Brilliant)
                        {
                            Brilliant brilliant = new Brilliant(Rect, storage.Pull2DTexture("brilliant"), game, this.camera, 50);
                            interactionSubjects.Add(brilliant);
                            
                            levelMap[i, j] = LevelObject.Empty;
                            NumberOfJewelry++;
                        }

                        if (levelMap[i, j] == LevelObject.Picture1)
                        {
                            Picture picture1 = new Picture(Rect, storage.Pull2DTexture("picture1"), game, this.camera, 30);
                            interactionSubjects.Add(picture1);
                            
                            levelMap[i, j] = LevelObject.Empty;
                            NumberOfJewelry++;
                        }

                        if (levelMap[i, j] == LevelObject.Picture2)
                        {
                            Picture picture2 = new Picture(Rect, storage.Pull2DTexture("picture2"), game, this.camera, 30);
                            interactionSubjects.Add(picture2);
                            
                            levelMap[i, j] = LevelObject.Empty;
                            NumberOfJewelry++;
                        }

                        if (levelMap[i, j] == LevelObject.Picture3)
                        {
                            Picture picture3 = new Picture(Rect, storage.Pull2DTexture("picture3"), game, this.camera, 30);
                            interactionSubjects.Add(picture3);
                            
                            levelMap[i, j] = LevelObject.Empty;
                            NumberOfJewelry++;
                        }

                        // стол системы управления камерами
                        if (levelMap[i, j] == LevelObject.SpLU)
                        {
                            SysControl sysControl = new SysControl(Rect, storage.Pull2DTexture("spLU"), game, this.camera);
                            sysControls.Add(sysControl);
                            sysControl.posY = j;
                            sysControl.posX = i;
                            sysControl.generateMathEmample();
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.SpUR)
                        {
                            SysControl sysControl = new SysControl(Rect, storage.Pull2DTexture("spUR"), game, this.camera);
                            sysControls.Add(sysControl);
                            sysControl.posY = j;
                            sysControl.posX = i;
                            sysControl.generateMathEmample();
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.SpRD)
                        {
                            SysControl sysControl = new SysControl(Rect, storage.Pull2DTexture("spRD"), game, this.camera);
                            sysControls.Add(sysControl);
                            sysControl.posY = j;
                            sysControl.posX = i;
                            sysControl.generateMathEmample();
                            levelMap[i, j] = LevelObject.Wall;
                        }
                        if (levelMap[i, j] == LevelObject.SpDL)
                        {
                            SysControl sysControl = new SysControl(Rect, storage.Pull2DTexture("spDL"), game, this.camera);
                            sysControls.Add(sysControl);
                            sysControl.posY = j;
                            sysControl.posX = i;
                            sysControl.generateMathEmample();
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

                        // куст
                        if (levelMap[i, j] == LevelObject.Plant)
                        {
                            Object obj = new Object(Rect, storage.Pull2DTexture("plant"), game, this.camera);
                            objs.Add(obj);
                            levelMap[i, j] = LevelObject.Wall;
                        }


                        if (levelMap[i, j] == LevelObject.CameraUL)
                        {
                            Rectangle Rect2 = new Rectangle(i * LevelLoader.Size - size / 3, j * LevelLoader.Size - size / 3, Convert.ToInt32(size * 2.25), Convert.ToInt32(size * 2.25));
                            Cameras newCamera = new Cameras(Rect2, storage.Pull2DTexture("camera_UL_active"), storage.Pull2DTexture("camera_UL"), LevelObject.CameraUL, game, this.camera);
                            cameras.Add(newCamera);
                            newCamera.posY = j;
                            newCamera.posX = i;
                            levelMap[i, j] = LevelObject.Empty;
                        }
                        if (levelMap[i, j] == LevelObject.CameraUR)
                        {
                            Rectangle Rect2 = new Rectangle(i * LevelLoader.Size - size / 3 * 2, j * LevelLoader.Size - size / 3, Convert.ToInt32(size * 2.25), Convert.ToInt32(size * 2.25));
                            Cameras newCamera = new Cameras(Rect2, storage.Pull2DTexture("camera_UR_active"), storage.Pull2DTexture("camera_UR"), LevelObject.CameraUR, game, this.camera);
                            newCamera.posY = j;
                            newCamera.posX = i;
                            cameras.Add(newCamera);
                            
                            levelMap[i, j] = LevelObject.Empty;
                        }



                        // лазеры
                        if (levelMap[i, j] == LevelObject.LaserHoriz) 
                        {
                            int size2 = (int)(size * 1.5);
                            Rectangle Rect2 = new Rectangle(i * LevelLoader.Size - size / 4, j * LevelLoader.Size, size2, size);
                            Laser laser = new Laser(Rect2, storage.Pull2DTexture("laser2_horiz"), storage.Pull2DTexture("laser2_horiz_inactive"), LevelObject.LaserHoriz, game, this.camera);
                            laser.typeLaser = LevelObject.LaserHoriz;
                            lasers.Add(laser);
                            levelMap[i, j] = LevelObject.Empty;
                        }
                        if (levelMap[i, j] == LevelObject.LaserVertic) 
                        {
                            int size2 = (int)(size * 1.5);
                            Rectangle Rect2 = new Rectangle(i * LevelLoader.Size, j * LevelLoader.Size - size / 4, size, size2);
                            Laser laser = new Laser(Rect2, storage.Pull2DTexture("laser2_vert"), storage.Pull2DTexture("laser2_vert_inactive"), LevelObject.LaserVertic, game, this.camera);
                            laser.typeLaser = LevelObject.LaserVertic;
                            lasers.Add(laser);
                            levelMap[i, j] = LevelObject.Empty;
                        }

                        if (levelMap[i, j] == LevelObject.LaserHorizMoving) 
                        {
                            int size2 = (int)(size * 1.5);
                            Rectangle Rect2 = new Rectangle(i * LevelLoader.Size - size / 4, j * LevelLoader.Size, size2, size);
                            Laser laser = new Laser(Rect2, storage.Pull2DTexture("laser2_horiz"), storage.Pull2DTexture("laser2_horiz_inactive"), LevelObject.LaserHorizMoving, game, this.camera);
                            laser.typeLaser = LevelObject.LaserHorizMoving;
                            lasers.Add(laser);
                            levelMap[i, j] = LevelObject.Empty;
                        }
                        if (levelMap[i, j] == LevelObject.LaserVerticMoving) 
                        {
                            int size2 = (int)(size * 1.5);
                            Rectangle Rect2 = new Rectangle(i * LevelLoader.Size, j * LevelLoader.Size - size / 4, size, size2);
                            Laser laser = new Laser(Rect2, storage.Pull2DTexture("laser2_vert"), storage.Pull2DTexture("laser2_vert_inactive"), LevelObject.LaserVerticMoving, game, this.camera);
                            laser.typeLaser = LevelObject.LaserVerticMoving;
                            lasers.Add(laser);
                            levelMap[i, j] = LevelObject.Empty;
                        }

                        


                        y += LevelLoader.Size;

                    }

                    y = 0;
                    x += LevelLoader.Size;

                }
                // конец создание объектов на уровне

                lenghtX = LevelLoader.Size * sizeFile[1]; // длина уровня в пикселях
                lenghtY = LevelLoader.Size * sizeFile[0];




                //считывание пути траекторий и связки
                lines = File.ReadAllLines(lvl_name); //получили массив строк                

                //считываем размеры массива с уровнем (sizeFile[0] значение - строки, sizeFile[1] - колонки)
                for (int i = sizeFile[0]; i < lines.Length; i++)
                {
                    int step = 0;
                    int numberGuard = 0;
                    int nextX = 0;
                    int nextY = 0;

                    str = lines[i].Split(' ');
                    LevelObject obj = (LevelObject)Int32.Parse(str[0]);

                    // считываем траектории охранников
                    // тип связки (Охранник Номер Шаг СледУ СледХ)
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


                    // считываем траектории лазеров
                    // тип связки (Лазер Номер Шаг СледУ СледХ)
                    if (obj == LevelObject.LaserVerticMoving)
                    {
                        step = Convert.ToInt32(str[2]);
                        numberGuard = Convert.ToInt32(str[1]);
                        nextX = Convert.ToInt32(str[4]);
                        nextY = Convert.ToInt32(str[3]);

                        // добавляем новую строку под координату
                        lasers[numberGuard].wayToPatrol.Add(new List<int>());

                        //задание начальной точки следования для охранника
                        if (step == 0)
                        {
                            lasers[numberGuard].TargetX = nextX;
                            lasers[numberGuard].TargetY = nextY;
                        }

                        // задание точки патрилрования
                        lasers[numberGuard].wayToPatrol[step].Add(nextX);
                        lasers[numberGuard].wayToPatrol[step].Add(nextY);
                        //максимальное кол-во шагов.
                        lasers[numberGuard].MaxStepToPatrol++;
                        
                        lasers[numberGuard].Patrol(); // сразу задаем первую точку, куда двигаться лазеру
                    }

                    // считываем связки (пункт управления - камера)
                    // связка (ПунктУправления КоордХПункта КоордУПунта КоордХКамеры КоордУКамеры)
                    if (obj == LevelObject.SpDL || obj == LevelObject.SpLU || obj == LevelObject.SpRD || obj == LevelObject.SpUR)
                    {
                        int posX = Convert.ToInt32(str[1]);
                        int posY = Convert.ToInt32(str[2]);
                        int posXCam = Convert.ToInt32(str[3]);
                        int posYCam = Convert.ToInt32(str[4]);
                        
                        foreach (SysControl sysControl in sysControls)
                        {
                            if (sysControl.posX == posX && sysControl.posY == posY)
                            {
                                sysControl.targetCameraX = posXCam;
                                sysControl.targetCameraY = posYCam;
                            }
                        }
                        
                    }



                    // считываем связки (ключ - дверь)
                    // связка (КлючИлиКарта КоордХКлюча КоордУКлюча КоордХДвери КоордУДвери)
                    if (obj == LevelObject.Key || obj == LevelObject.Card)
                    {
                        int posX = Convert.ToInt32(str[1]);
                        int posY = Convert.ToInt32(str[2]);
                        int posXDoor = Convert.ToInt32(str[3]);
                        int posYDoor = Convert.ToInt32(str[4]);

                        foreach (BaseObject interactionSubject in interactionSubjects)
                        {
                            if (interactionSubject.posX == posX && interactionSubject.posY == posY)
                            {
                                interactionSubject.targetDoorX = posXDoor;
                                interactionSubject.targetDoorY = posYDoor;
                            }
                        }

                    }




                }

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
        LevelObject isWallType(LevelObject[,] array, int i, int j, int iEnd, int jEnd)
        {

            // проверка краевых точек массива

            /*
             * *----
             * -----
             * -----
             * -----
             */
            if (i == 0 && j == 0)
            {
                return LevelObject.WallDownRight;
            }

            /*
             * -----
             * -----
             * -----
             * *----
             */
            if (i == 0 && j == jEnd)
            {
                return LevelObject.WallUpRight;
            }

            /*
             * ----*
             * -----
             * -----
             * -----
             */
            if (i == iEnd && j == 0)
            {
                return LevelObject.WallLeftDown;
            }

            /*
             * -----
             * -----
             * -----
             * ----*
             */
            if (i == iEnd && j == jEnd)
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
        /// Ищет стены в каждую сторону от клетки, если хотя бы 1 стены нет, то клетка - пустота
        /// </summary>
        /// <param name="floorPositionX">Номер колонки</param>
        /// <param name="floorPositionY">Номер строки</param>
        /// <returns></returns>
        private bool isFloor(LevelObject[,] levelFloor, int floorPositionX, int floorPositionY, int sizeLvlCols, int sizeLvlRows)
        {
            //floorPositionY -= sizeLvlRows;
            int qWalls = 0;
            for (int i = floorPositionX; i >= 0; i--)
            {
                if (levelFloor[i, floorPositionY] == LevelObject.Empty)
                {
                    qWalls++;
                    break;
                }
                   
            }

            for (int i = floorPositionX; i <= sizeLvlCols; i++)
            {
                if (levelFloor[i, floorPositionY] == LevelObject.Empty)
                {
                    qWalls++;
                    break;
                }

            }


            for (int j = floorPositionY; j >= 0; j--)
            {
                if (levelFloor[floorPositionX, j] == LevelObject.Empty)
                {
                    qWalls++;
                    break;
                }

            }

            for (int j = floorPositionY; j <= sizeLvlRows; j++)
            {
                if (levelFloor[floorPositionX, j] == LevelObject.Empty)
                {
                    qWalls++;
                    break;
                }

            }



            if (qWalls == 4)
            {
                return true;
            }
            else
            {
                return false;
            }
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
