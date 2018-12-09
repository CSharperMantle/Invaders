using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Invaders.Wpf.Model
{
    class HomingShot : Shot
    {
        public readonly Ship TargetShip;

        private short _lastMoveUpDownTime = 0;

        public HomingShot(Point location, Direction direction, Ship target) : base(location, direction)
        {
            ShotSize = new Size(2, 10);
            ShotPixelsPerSecond = 50;
            TargetShip = target;
        }

        public override void Move()
        {
            if (_lastMoveUpDownTime > 5)
            {
                var timeSinceLastMoved = DateTime.Now - lastMoved;
                var distance = timeSinceLastMoved.Milliseconds * ShotPixelsPerSecond / 1000;
                if (TargetShip.Location.X < Location.X) distance *= -1;
                Location = new Point(Location.X + distance, Location.Y);
                lastMoved = DateTime.Now;
                _lastMoveUpDownTime = 0;
            } else
            {
                var timeSinceLastMoved = DateTime.Now - lastMoved;
                var distance = timeSinceLastMoved.Milliseconds * ShotPixelsPerSecond / 1000;
                if (Direction == Direction.Up) distance *= -1;
                Location = new Point(Location.X, Location.Y + distance);
                lastMoved = DateTime.Now;
                _lastMoveUpDownTime ++;
            }
        }
    }
}
