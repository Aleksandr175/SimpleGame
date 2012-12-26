// <summary>
// Объявление перечислений для упрощения работы
// </summary>

namespace Enumerations 
{
    // перечисление для определения положения движения игрока
    enum PlayerMove { Up, Down, Left, Right };

    // сложность уровня
    enum Complexity { Low, Medium, High };

    // ориентация дверей
    enum DoorOrientation { Horiz, Vert, HorizWood, VertWood };

    enum LevelObject { 
        Empty = 0, 
        
        Wall = 1,
        WallVertic = 2,

        WallDownRight = 3,
        WallUpRight = 4,
        WallLeftDown = 5,
        WallLeftUp = 6,

        Wall4Sides = 7,

        WallURD = 8,
        WallRDL = 9,
        WallDLU = 10,
        WallLUR = 11,

        DoorHoriz = 20, 
        DoorVertic = 21, 
        DoorHorizOpen = 22, 
        DoorVerticOpen = 23,

        DoorWoodHoriz = 24,
        DoorWoodVertic = 25,
        DoorWoodHorizOpen = 26,
        DoorWoodVerticOpen = 27,


        Guard = 30,

        // кресла
        Chairs_U = 32,
        Chairs_R = 33,
        Chairs_D = 34,
        Chairs_L = 35,

        // диван
        Sofa_U = 36,
        Sofa_R = 37,
        Sofa_D = 38,
        Sofa_L = 39,

        // ключ, карта
        Key = 40,
        Card = 41,

        // золото и др. для кражи
        Gold = 50,
        Rubin = 51,
        Brilliant = 52,
        Picture1 = 53,
        Picture2 = 54,
        Picture3 = 55,



        // стол управления камерами
        SpLU = 60,
        SpUR = 61,
        SpRD = 62,
        SpDL = 63,

        CameraUL = 65,
        CameraUR = 66,
        CameraULActive = 67,
        CameraURActive = 68,


        // стол с компьютером
        TableU = 70,
        TableR = 71,
        TableD = 72,
        TableL = 73,

        // куст
        Plant = 75,

        //все виды лазеров
        LaserHoriz = 80,
        LaserVertic = 81,
        LaserHorizMoving = 82,
        LaserVerticMoving = 83,
        

        Floor = 90,

        Player = 100
    }

    //состояние игры
    enum GameState
    {
        Game,
        Menu,
        LvlMenu,
        Advice
    }
}