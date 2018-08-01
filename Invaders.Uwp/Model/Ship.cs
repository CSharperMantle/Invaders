using Windows.Foundation;

namespace Invaders.Uwp.Model
{
    public abstract class Ship
    {
        public Ship(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        public Point Location { get; protected set; }

        public Size Size { get; }

        public Rect Area => new Rect(Location, Size);

        public abstract void Move(Direction direction);
    }
}