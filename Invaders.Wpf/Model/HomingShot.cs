using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public class HomingShot : Shot
    {
        private short _lastMovedUpDownTime;

        public HomingShot(Point location, Direction direction, Ship target = null) : base(location, direction)
        {
            ShotSize = new Size(2, 10);
            ShotPixelsPerSecond = 60;
            ShotManeuveringPixelsPerSecond = 80;
            TargetShip = target;
            Life = 5;
            Score = 15;
        }

        public int ShotManeuveringPixelsPerSecond { get; protected set; }

        public Ship TargetShip { get; protected set; }

        public override void Move()
        {
            if (TargetShip == null)
            {
                var timeSinceLastMoved = DateTime.Now - lastMoved;
                var distance = timeSinceLastMoved.Milliseconds * ShotPixelsPerSecond / 1000;
                if (Direction == Direction.Up) distance *= -1;
                Location = new Point(Location.X, Location.Y + distance);
                lastMoved = DateTime.Now;
                return;
            }

            if (_lastMovedUpDownTime > 2)
            {
                var timeSinceLastMoved = DateTime.Now - lastMoved;
                var distance = timeSinceLastMoved.Milliseconds * ShotManeuveringPixelsPerSecond / 1000;
                if (TargetShip.Location.X < Location.X) distance *= -1;
                Location = new Point(Location.X + distance, Location.Y);
                lastMoved = DateTime.Now;
                _lastMovedUpDownTime = 0;
            }
            else
            {
                var timeSinceLastMoved = DateTime.Now - lastMoved;
                var distance = timeSinceLastMoved.Milliseconds * ShotPixelsPerSecond / 1000;
                if (Direction == Direction.Up) distance *= -1;
                Location = new Point(Location.X, Location.Y + distance);
                lastMoved = DateTime.Now;
                _lastMovedUpDownTime++;
            }
        }
    }
}