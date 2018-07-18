using System;
using System.IO;
using System.Runtime.Serialization;

namespace Invaders.Wpf.Commons
{
    public class AppLifeCycleManager
    {
        private HistoryData _historyData;

        public void OnAppActivated(object sender, EventArgs e)
        {
            Log.Info("Loading HistoryData.xml");
            if (!File.Exists("./Config/HistoryData.xml"))
            {
                Log.Warning("HistoryData.xml not exists!");
                _historyData = new HistoryData { 
                    HighestScore = 0,
                    PlayedTime = 0,
                    KilledInvaders = 0,
                    WinTime = 0
                };
                return;
            }
            using (Stream readerStream = File.OpenRead("./Config/HistoryData.xml"))
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(HistoryData));
                var obj = dcSerializer.ReadObject(readerStream);
                if (obj is HistoryData)
                {
                    _historyData = obj as HistoryData;
                } else
                {
                    throw new SerializationException(nameof(obj) + " is not " + nameof(HistoryData));
                }
            }
        }

        public void OnAppDeactivated(object sender, EventArgs e)
        {
            Log.Info("Writing HistoryData.xml");
            if (_historyData == null){ return; }

            if (!File.Exists("./config/HistoryData.xml"))
            {
                using (Stream writerStream = File.Create("./Config/HistoryData.xml"))
                {
                    DataContractSerializer dcSerializer = new DataContractSerializer(typeof(HistoryData));
                    dcSerializer.WriteObject(writerStream, _historyData);
                }

                return;
            }
            using (Stream writerStream = File.OpenWrite("./Config/HistoryData.xml"))
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(HistoryData));
                dcSerializer.WriteObject(writerStream, _historyData);
            }
        }
    }
}