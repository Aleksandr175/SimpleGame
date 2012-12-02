using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLevels.coreMission
{
    // базовый интерфейс для цели
    public interface CoreGoal { }

    // посчитать выражение
    public interface Expression : CoreGoal { }

    // пройти уровень за определенное количество времени
    public interface Time : CoreGoal { }

    // собрать определенное количество чего-либо, пока монеток
    public interface Collect : CoreGoal { }

    /// <summary>
    /// Класс описывает цель - посчитай выражение
    /// </summary>
    public class ExpressionTarger : BaseTarget, Expression
    {

        // само выражение, что нужно посчитать
        protected String expression;

        // правильный ответ
        protected double rightValue;

        public ExpressionTarger(String name, String description, bool isMain)
            : base(name, description, isMain)
        {
        }

        public ExpressionTarger(String name, String description, bool isMain, String expression, double rightValue)
            : base(name, description, isMain)
        {
            this.expression = expression;
            this.rightValue = rightValue;
        }

        public String Expression
        {
            get { return expression; }
            set { this.expression = value; }
        }

        public double RightValue
        {
            get { return rightValue; }
            set { this.rightValue = value; }
        }

        // установка ответа
        public String SetAnswer(double answer)
        {
            if (answer != rightValue)
                return base.ShowDone();

            isDone = true;
            return base.ShowVotedDone();
        }
    }

    /// <summary>
    /// Класс описывает цель - дойди до точки за определенное количество секунд
    /// </summary>
    public class TimeTarget : BaseTarget, Time
    {

        // время окончания задания
        protected double stopTime;

        public TimeTarget(String name, String description, bool isMain)
            : base(name, description, isMain)
        { }

        public TimeTarget(String name, String description, bool isMain, double runtime)
            : base(name, description, isMain)
        {
            this.stopTime = runtime;
        }

        /// <summary>
        /// Задание начала отсчета
        /// </summary>
        /// <param name="gametime">Игровое время</param>
        public void Start(GameTime gametime)
        {
            this.stopTime = gametime.ElapsedGameTime.TotalSeconds + this.stopTime;
        }

        /// <summary>
        /// Проверка на успешное прохождение
        /// </summary>
        /// <param name="gametime">Игровое время</param>
        /// <returns>Строка состояния</returns>
        public String IsTimeOver(GameTime gametime)
        {
            if (gametime.ElapsedGameTime.TotalSeconds > this.stopTime)
                return base.ShowVotedDone();

            isDone = true;
            return base.ShowDone();
        }
    }


    public class CollectTarget : BaseTarget, Collect
    {

        // количество собираемых предметов
        protected int count;

        public CollectTarget(String name, String description, bool isMain)
            : base(name, description, isMain)
        {
        }

        public CollectTarget(String name, String description, bool isMain, int count)
            : base(name, description, isMain)
        {
            this.count = count;
        }

        public int Count
        {
            get { return count; }
            set { this.count = value; }
        }

        public String IsCollect(int count)
        {
            if (count < this.count)
                return base.ShowVotedDone();

            isDone = true;
            return base.ShowDone();
        }
    }



    /// <summary>
    /// Класс описывает цель
    /// </summary>
    public class BaseTarget
    {
        // выполнено ли задание
        protected bool isDone;
        // название цели
        protected String name;
        // описание цели
        protected String description;
        // главная ли цель
        protected bool isMain;

        // предопределенные константы
        public const String VOTED_DONE = "ЗАДАНИЕ ПРОВАЛЕНО: ";
        public const String DONE = "ЗАДАНИЕ ВЫПОЛЕНО: ";
        public const String MISSION = "ЦЕЛЬ: ";

        public BaseTarget(String name, String description, bool isMain)
        {
            this.isDone = false;
            this.isMain = isMain;
            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Показывает, если сделано задание
        /// </summary>
        public String ShowDone()
        {
            // показать строку эффект
            // мб сделать коллекцию эффетов заранее определенных?
            return DONE + this.name;
        }

        /// <summary>
        /// Показывается, если задание провалено
        /// </summary>
        public String ShowVotedDone()
        {
            return VOTED_DONE + this.name;
        }

        /// <summary>
        /// Возвращает описание цели
        /// </summary>
        /// <returns>Описание цели</returns>
        public String GetInfo()
        {
            return MISSION + this.description;
        }

        /// <summary>
        /// Свойство, для просмотра / установки цели
        /// </summary>
        public bool IsDone
        {
            get { return isDone; }
            set { isDone = value; }
        }

        public bool IsMain
        {
            get { return isMain; }
            private set { }
        }
    }

}
