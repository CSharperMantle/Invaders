using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public class LazerShot : BasicMovingShot
    {
        public LazerShot(Point location, Direction direction)
            : base(location, direction, new Size(2, 10), 160, 5, 10)
        {
        }
    }
}