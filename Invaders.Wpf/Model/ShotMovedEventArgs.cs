using System;

namespace Invaders.Wpf.Model
{
    public class ShotMovedEventArgs : EventArgs
    {
        public ShotMovedEventArgs(Shot shot, bool disappeared)
        {
            Shot = shot;
            Disappeared = disappeared;
        }

        public Shot Shot { get; }
        public bool Disappeared { get; }
    }
}