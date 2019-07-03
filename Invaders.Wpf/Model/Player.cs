using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public class Player : Ship
    {
        public const double PlayerPixelsPerSecond = 8;
        public static readonly Size PlayerSize = new Size(25, 15);

        public Player()
            : base(new Point(PlayerSize.Width,
                    InvadersModel.PlayAreaSize.Height - InvadersModel.PlayAreaSize.Height * 3),
                PlayerSize)
        {
            Location = new Point(Location.X, InvadersModel.PlayAreaSize.Height - PlayerSize.Height * 3);
        }

        public Ship TargetShip { get; private set; }

        public override void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    if (Location.X > PlayerSize.Width)
                        Location = new Point(Location.X - PlayerPixelsPerSecond, Location.Y);
                    break;
                case Direction.Right:
                    if (Location.X < InvadersModel.PlayAreaSize.Width - PlayerSize.Width * 2)
                        Location = new Point(Location.X + PlayerPixelsPerSecond, Location.Y);
                    break;
                default:
                    throw new ArgumentException(nameof(direction));
            }
        }
    }
}