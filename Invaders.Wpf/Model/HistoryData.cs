using System;
using System.Runtime.Serialization;

namespace Invaders.Wpf.Model
{
    ///<summary>
    ///Represents an object that records history game data.
    ///It can be serialized using <see cref="System.Runtime.Serialization.DataContractSerializer" />
    /// and <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter" />
    ///</summary>
    [Serializable]
    [DataContract]
    internal class HistoryData
    {
        [DataMember]
        public int HighestScore { get; private set; }

        [DataMember]
        public TimeSpan? PlayedTime { get; private set; }

        [DataMember]
        public int PlayedGames { get; private set; }

        [DataMember]
        public int KilledInvaders { get; private set; }

        [DataMember]
        public int DiedTime { get; private set; }

        public void UpdateHighestScore(int newScore)
        {
            if (newScore > HighestScore)
            {
                HighestScore = newScore;
            }
        }

        public void IncreasePlayedGames() => PlayedGames++;

        public void IncreaseKilledInvaders() => KilledInvaders++;

        public void IncreaseDiedTime() => DiedTime++;

        public HistoryData()
        {
            HighestScore = 0;
            PlayedTime = new TimeSpan();
            PlayedGames = 0;
            KilledInvaders = 0;
            DiedTime = 0;
        }
    }
}