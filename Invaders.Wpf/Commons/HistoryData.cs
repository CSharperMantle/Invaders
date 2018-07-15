using System;
using System.Runtime.Serialization;

namespace Invaders.Wpf.Commons
{
    [Serializable]
    [DataContract]
    internal class HistoryData
    {
        [DataMember]
        public int HighestScore { get; set; }

        [DataMember]
        public int PlayedTime { get; set; }

        [DataMember]
        public int KilledInvaders { get; set; }

        [DataMember]
        public int WinTime { get; set; }
    }
}