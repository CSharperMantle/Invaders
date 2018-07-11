using System.Windows;

namespace Invaders.Wpf.Model
{
    public abstract class Ship
    {
        public Point Location { get; protected set; }
        
        public Size Size { get; private set; }
        
        public Rect Area => new Rect(Location, Size);

        public Ship(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        public abstract void Move(Direction direction);
    }
}