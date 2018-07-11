﻿using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public class Player : Ship
    {
        public static readonly Size PlayerSize = new Size(25, 15);

        public const double Speed = 10;
        
        public Player()
            : base(new Point(PlayerSize.Width, 
                InvadersModel.PlayAreaSize.Height - InvadersModel.PlayAreaSize.Height * 3), 
                PlayerSize) 
        {
            Location = new Point(Location.X, InvadersModel.PlayAreaSize.Height - PlayerSize.Height * 3);
        }

        public override void Move(Direction direction)
        {
            throw new NotImplementedException();
        }
    }
}