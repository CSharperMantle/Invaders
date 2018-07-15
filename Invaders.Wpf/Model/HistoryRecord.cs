using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
class HistoryRecord
{
    public int HighestScore { get; set; }

    public int InvadersKilled { get; set; }

    public int KilledTime { get; set; }

}