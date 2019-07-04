using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    /// <summary>
    /// Abstract base class for all the shots.
    /// </summary>
    public abstract class Shot
    {
        protected DateTime LastMoved;

        protected Shot(Point location, Direction direction, int hitpoint, int score)
        {
            Location = location;
            Direction = direction;
            Hitpoint = hitpoint;
            Score = score;
            LastMoved = DateTime.Now;
        }

        /// <summary>
        /// Shot speed.
        /// </summary>
        public double ShotPixelsPerSecond { get; protected set; }
        
        /// <summary>
        /// Shot size.
        /// </summary>
        public Size ShotSize { get; protected set; }

        /// <summary>
        /// Shot location.
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// The life of a shot.
        /// A <see cref="Shot"/> with a sub-zero <see cref="Shot.Hitpoint"/> will be deleted from the gamefield.
        /// </summary>
        public int Hitpoint { get; protected set; }

        /// <summary>
        /// The score player can ge from colliding two <see cref="Shot"/> objects.
        /// Only one shot's <see cref="Shot.Score"/> will be added to the player's score.
        /// </summary>
        public int Score { get; protected set; }

        public Direction Direction { get; }

        public bool IsUsedUp()
        {
            return Hitpoint <= 0;
        }

        public void DecreaseHitpoint(int value)
        {
            //TODO: Add value range check
            Hitpoint -= value;
        }

        public abstract void Move();
    }
}