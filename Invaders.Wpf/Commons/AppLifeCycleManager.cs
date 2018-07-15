using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Invaders.Wpf.Commons
{
    class AppLifeCycleManager
    {
        private HistoryData _historyData;

        public void OnAppActivated(object sender, EventArgs e)
        {
            using (Stream readerStream = File.OpenRead("./Config/HistoryData.dat"))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                var obj = binaryFormatter.Deserialize(readerStream);
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
            
        }
    }
}