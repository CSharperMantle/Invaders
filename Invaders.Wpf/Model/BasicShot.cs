using System.Windows;

namespace Invaders.Wpf.Model
{
    public class BasicShot : BasicMovingShot
    {
        public BasicShot(Point location, Direction direction)
            : base(location, direction, new Size(2, 10), 80, 20, 5)
        {
        }
    }
}