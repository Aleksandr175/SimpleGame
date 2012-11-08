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
    enum DoorOrientation { Horiz, Vert };

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

        Guard = 30,

        Key = 40,
        Card = 41,

        Gold = 50, 

        // стол управления камерами
        SpLU = 60,
        SpUR = 61,
        SpRD = 62,
        SpDL = 63,

        // стол с компьютером
        TableU = 70,
        TableR = 71,
        TableD = 72,
        TableL = 73,

        //все виды лазеров
        LaserHoriz = 80,
        LaserVertic = 81,
        LaserHorizL = 82,
        LaserHorizR = 83,
        LaserVerticU = 84,
        LaserVerticD = 85,
        LaserHorizMiddle = 86,
        LaserVerticMiddle = 87,

        

        Floor = 90,

        Player = 100
    }
}