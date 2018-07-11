using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public class StarChangedEventArgs : EventArgs
    {
        public Point Point { get; private set; }
        public bool Disappeared { get; private set; }

        public StarChangedEventArgs(Point point, bool disappeared)
        {
            Disappeared = disappeared;
            Point = point;
        }
    }
}