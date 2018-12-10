using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public abstract class Shot
    {
        public double ShotPixelsPerSecond { get; protected set; }
        public Size ShotSize { get; protected set; }

        protected DateTime lastMoved;

        public Shot(Point location, Direction direction)
        {
            Location = location;
            Direction = direction;
            lastMoved = DateTime.Now;
        }

        public bool IsUsedUp() => Life <= 0;

        public void DecreaseLife(int value) => Life -= value;

        public Point Location { get; set; }

        public int Life { get; protected set; }

        public int Score { get; protected set; }

        public Direction Direction { get; }

        public abstract void Move();
    }
}