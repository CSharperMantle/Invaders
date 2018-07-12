using System;
using System.Windows;

namespace Invaders.Wpf.Model
{
    public class StarChangedEventArgs : EventArgs
    {
        public StarChangedEventArgs(Point point, bool disappeared)
        {
            Disappeared = disappeared;
            Point = point;
        }

        public Point Point { get; }
        public bool Disappeared { get; }
    }
}