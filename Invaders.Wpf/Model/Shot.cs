using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public abstract class Shot
    {
        protected DateTime lastMoved;

        protected Shot(Point location, Direction direction)
        {
            Location = location;
            Direction = direction;
            lastMoved = DateTime.Now;
        }

        public double ShotPixelsPerSecond { get; protected set; }
        public Size ShotSize { get; protected set; }

        public Point Location { get; set; }

        public int Life { get; protected set; }

        public int Score { get; protected set; }

        public Direction Direction { get; }

        public bool IsUsedUp()
        {
            return Life <= 0;
        }

        public void DecreaseLife(int value)
        {
            Life -= value;
        }

        public abstract void Move();
    }
}