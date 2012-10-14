/// <summary>
/// Объявление структур для упрощения работы
/// </summary>

using Microsoft.Xna.Framework;
using Enumerations;
namespace Structs
{

    /// <summary>
    /// Информация о фрейме
    /// </summary>
    struct FrameInfo
    {
        public int width;
        public int height;
        public int count;
        public int current;
        public int timeElapsed;
        public int timeForFrame;
    }

    /// <summary>
    /// Информация об игроке
    /// </summary>
    struct PlayerInfo
    {
        // двигается ли игрок
        public bool isRunning;
        // скорость игрока
        public short speed;
        // текущее положение
        public Rectangle position;
        // направление движение
        public PlayerMove direction;
    }

}