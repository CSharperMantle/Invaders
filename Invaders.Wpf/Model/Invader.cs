using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public class Invader : Ship
    {
        /// <summary>
        ///     Horizontal gap between two <see cref="Invader" />s.
        /// </summary>
        public const int HorizontalInterval = 5;

        /// <summary>
        ///     Vertical gap between two <see cref="Invader" />s.
        /// </summary>
        public const int VerticalInterval = 15;

        /// <summary>
        ///     Size of the <see cref="Invader" />'s hit box.
        /// </summary>
        public static Size InvaderSize = new Size(15, 15);

        public Invader(Point location, InvaderType invaderType) : base(location, InvaderSize)
        {
            InvaderType = invaderType;
            switch (InvaderType)
            {
                case InvaderType.Spaceship:
                    Score = 50;
                    break;
                case InvaderType.Bug:
                    Score = 40;
                    break;
                case InvaderType.Saucer:
                    Score = 30;
                    break;
                case InvaderType.Satellite:
                    Score = 20;
                    break;
                case InvaderType.Star:
                    Score = 10;
                    break;
                default:
                    throw new ArgumentException("Invalid InvaderType! ");
            }
        }

        /// <summary>
        ///     The type of the <see cref="Invader" />
        /// </summary>
        public InvaderType InvaderType { get; }

        /// <summary>
        ///     The score the player gets from killing an <see cref="Invader" />.
        /// </summary>
        public int Score { get; }

        /// <summary>
        ///     The life of the <see cref="Invader" />.
        /// </summary>
        public int Hitpoint { get; }

        /// <summary>
        ///     Move a <see cref="Invader" />.
        /// </summary>
        /// <param name="direction">Moving direction</param>
        /// <exception cref="ArgumentException">Invalid direction received, for example <see cref="Direction.Up" />.</exception>
        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    Location = new Point(Location.X + HorizontalInterval, Location.Y);
                    break;
                case Direction.Left:
                    Location = new Point(Location.X - HorizontalInterval, Location.Y);
                    break;
                case Direction.Down:
                    Location = new Point(Location.X, Location.Y + VerticalInterval);
                    break;
                default:
                    throw new ArgumentException(nameof(direction));
            }
        }
    }
}