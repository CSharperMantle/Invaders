using System;
using Windows.Foundation;

namespace Invaders.Uwp.Model
{
    public class Shot
    {
        public const double ShotPixelsPerSecond = 80;
        public static Size ShotSize = new Size(2, 10);

        private DateTime _lastMoved;

        public Shot(Point location, Direction direction)
        {
            Location = location;
            Direction = direction;
            _lastMoved = DateTime.Now;
        }

        public Point Location { get; private set; }

        public Direction Direction { get; }

        public void Move()
        {
            var timeSinceLastMoved = DateTime.Now - _lastMoved;
            var distance = timeSinceLastMoved.Milliseconds * ShotPixelsPerSecond / 1000;
            if (Direction == Direction.Up) distance *= -1;
            Location = new Point(Location.X, Location.Y + distance);
            _lastMoved = DateTime.Now;
        }
    }
}