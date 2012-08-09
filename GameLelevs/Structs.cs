// <summary>
// Объявление структур для кпрощения работы
// </summary>

using Microsoft.Xna.Framework;
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
        public bool isRunning;
        public short speed;
        public Rectangle position;
    }
}