﻿using System;

namespace Invaders.Uwp.Model
{
    public class ShipChangedEventArgs : EventArgs
    {
        public ShipChangedEventArgs(Ship shipUpdated, bool killed)
        {
            ShipUpdated = shipUpdated;
            Killed = killed;
        }

        public Ship ShipUpdated { get; }
        public bool Killed { get; }
    }
}