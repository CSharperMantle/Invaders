using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public class LazerShot : Shot
    {
        public LazerShot(Point location, Direction direction) : base(location, direction)
        {
            ShotSize = new Size(2, 10);
            ShotPixelsPerSecond = 200;
            Life = 50;
            Score = 10;
        }

        public override void Move()
        {
            var timeSinceLastMoved = DateTime.Now - lastMoved;
            var distance = timeSinceLastMoved.Milliseconds * ShotPixelsPerSecond / 1000;
            if (Direction == Direction.Up) distance *= -1;
            Location = new Point(Location.X, Location.Y + distance);
            lastMoved = DateTime.Now;
        }
    }
}
