using System.Windows;

namespace Invaders.Wpf.Model
{
    /// <summary>
    /// Abstract base class for all ship-like objects.
    /// </summary>
    public abstract class Ship
    {
        protected Ship(Point location, Size size)
        {
            Location = location;
            Size = size;
        }

        /// <summary>
        /// The location of a <see cref="Ship"/>.
        /// </summary>
        public Point Location { get; protected set; }

        /// <summary>
        /// The size of a <see cref="Ship"/>.
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// The hit box of a <see cref="Ship"/>.
        /// </summary>
        public Rect Area => new Rect(Location, Size);

        /// <summary>
        /// Abstract method of moving a <see cref="Ship"/>.
        /// </summary>
        public abstract void Move(Direction direction);
    }
}