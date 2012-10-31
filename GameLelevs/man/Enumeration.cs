// <summary>
// Объявление перечислений для упрощения работы
// </summary>

namespace Enumerations 
{
    // перечисление для определения положения движения игрока
    enum PlayerMove { Up, Down, Left, Right };

    // сложность уровня
    enum Complexity { Low, Medium, High };

    enum LevelObject { 
        Empty = 0, 
        Wall = 1, 
        
        DoorHoriz = 20, 
        DoorVertic = 21, 
        DoorHorizOpen = 22, 
        DoorVerticOpen = 23,

        Guard = 30,

        Key = 40,
        Card = 41,

        Gold = 50, 


        Player = 100
    }
}