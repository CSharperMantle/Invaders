using System;
using Windows.Foundation;

namespace Invaders.Uwp.Model
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