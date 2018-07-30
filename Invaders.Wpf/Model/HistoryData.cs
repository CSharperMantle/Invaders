using System;
using System.Runtime.Serialization;

namespace Invaders.Wpf.Model
{
    /// <summary>
    ///     Represents an object that records history game data.
    ///     It can be serialized using <see cref="System.Runtime.Serialization.DataContractSerializer" />
    ///     and <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter" />
    /// </summary>
    [Serializable]
    [DataContract]
    public class HistoryData : ICloneable<HistoryData>
    {
        public HistoryData()
        {
            HighestScore = 0;
            PlayedTime = new TimeSpan();
            PlayedGames = 0;
            KilledInvaders = 0;
            DiedTime = 0;
        }

        [DataMember] public int HighestScore { get; private set; }

        [DataMember] public TimeSpan? PlayedTime { get; private set; }

        [DataMember] public int PlayedGames { get; private set; }

        [DataMember] public int KilledInvaders { get; private set; }

        [DataMember] public int DiedTime { get; private set; }

        public HistoryData Clone()
        {
            return new HistoryData
            {
                HighestScore = HighestScore,
                PlayedGames = PlayedGames,
                PlayedTime = PlayedTime,
                KilledInvaders = KilledInvaders,
                DiedTime = DiedTime
            };
        }

        public void UpdateHighestScore(int newScore)
        {
            if (newScore > HighestScore) HighestScore = newScore;
        }

        public void IncreasePlayedGames()
        {
            PlayedGames++;
        }

        public void IncreaseKilledInvaders()
        {
            KilledInvaders++;
        }

        public void IncreaseDiedTime()
        {
            DiedTime++;
        }

        public void IncreasePlayedTime(TimeSpan timePlayed)
        {
            if (PlayedTime.HasValue)
            {
                PlayedTime += timePlayed;
            }
            else
            {
                PlayedTime = timePlayed;
            }
        }
    }
}