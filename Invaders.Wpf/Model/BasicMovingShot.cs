using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public abstract class BasicMovingShot : Shot
    {
        protected BasicMovingShot(Point location, Direction direction, Size shotSize, int shotSpeed, int hitpoint,
            int score)
            : base(location, direction, hitpoint, score)
        {
            ShotSize = shotSize;
            ShotPixelsPerSecond = shotSpeed;
        }

        public override void Move()
        {
            var timeSinceLastMoved = DateTime.Now - LastMoved;
            var distance = timeSinceLastMoved.Milliseconds * ShotPixelsPerSecond / 1000;
            if (Direction == Direction.Up) distance *= -1;
            Location = new Point(Location.X, Location.Y + distance);
            LastMoved = DateTime.Now;
        }
    }
}