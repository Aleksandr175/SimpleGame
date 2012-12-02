using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XMLContent
{
    /// <summary>
    /// Базовый класс для загрузки из XML файла
    /// </summary>
    public class XMLTarget
    {
        // ключевая точка
        public Vector2 point;
        // название цели
        public string name;
        // описание цели
        public string description;
        // главная ли цель
        public bool isMain;
    }

    /// <summary>
    /// Класс дял загрузки целей - посчитай выражение
    /// </summary>
    public class XMLExpressionTarger : XMLTarget
    {
        // само выражение
        public string expression;
        // правильное значение
        public double rightValue;
    }

    /// <summary>
    /// Класс для загрузки целей - пройди на время
    /// </summary>
    public class XMLTimeTarget : XMLTarget
    {
        // за сколько нужно пройти
        public double runtime;
    }

    /// <summary>
    /// Класс для загрузки целей - собери в количесте не менее N штук
    /// </summary>
    public class XMLCollectTarget : XMLTarget
    {
        // количество
        public int count;
    }
}
