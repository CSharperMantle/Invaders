using System;
using Windows.Foundation;

namespace Invaders.Uwp.Model
{
    public class Invader : Ship
    {
        public const int HorizontalInterval = 5;

        public const int VerticalInterval = 15;
        public static Size InvaderSize = new Size(15, 15);

        public Invader(Point location, InvaderType invaderType) : base(location, InvaderSize)
        {
            InvaderType = invaderType;
            switch (InvaderType)
            {
                case InvaderType.Bug:
                    Score = 40;
                    break;
                case InvaderType.Satellite:
                    Score = 20;
                    break;
                case InvaderType.Saucer:
                    Score = 30;
                    break;
                case InvaderType.Spaceship:
                    Score = 50;
                    break;
                case InvaderType.Star:
                    Score = 10;
                    break;
                default:
                    throw new ArgumentException("Invalid InvaderType! ");
            }
        }

        public InvaderType InvaderType { get; }

        public int Score { get; }

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