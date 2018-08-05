using Invaders.Uwp.Commons;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace Invaders.Uwp.Model
{
    class HistoryDataManager
    {
        public static string HistoryDataFilename = "historydata.json";

        private IStorageFolder _currentFolder;
        private IStorageFile _historyFile;
        private DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(HistoryData));
        private static object _thisLock = new object();

        public HistoryData HistoryData { get; private set; }

        public async Task WriteHistoryDataAsync()
        {
            _historyFile = await _currentFolder.CreateFileAsync(
                HistoryDataFilename, 
                CreationCollisionOption.ReplaceExisting);

            using (var stream = await _historyFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                lock (_thisLock)
                {
                    using (var writer = stream.AsStreamForWrite())
                    {
                        _serializer.WriteObject(writer, HistoryData);
                    }
                }
            }
        }

        public async void WriteHistoryData()
        {
            _historyFile = await _currentFolder.CreateFileAsync(
                HistoryDataFilename,
                CreationCollisionOption.ReplaceExisting);

            using (var stream = await _historyFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                lock (_thisLock)
                {
                    using (var writer = stream.AsStreamForWrite())
                    {
                        _serializer.WriteObject(writer, HistoryData);
                    }
                }
            }
        }

        public async Task ReadHistoryDataAsync()
        {
            try
            {
                _historyFile = await _currentFolder.GetFileAsync(HistoryDataFilename);
            }
            catch (FileNotFoundException e)
            {
                Log.E(e.Message);
                Log.D(e.StackTrace);
                Log.D(e.Source);
                HistoryData = new HistoryData();
                return;
            }

            using (var stream = await _historyFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                lock (_thisLock)
                {
                    using (var reader = stream.AsStreamForRead())
                    {
                        var obj = _serializer.ReadObject(reader);
                        if (obj is HistoryData)
                        {
                            HistoryData = obj as HistoryData;
                        }
                        else
                        {
                            throw new SerializationException(nameof(obj) + " is not " + nameof(HistoryData));
                        }
                    }
                }
            }
        }

        public async void ReadHistoryData()
        {
            try
            {
                _historyFile = await _currentFolder.GetFileAsync(HistoryDataFilename);
            }
            catch (FileNotFoundException e)
            {
                Log.E(e.Message);
                Log.D(e.StackTrace);
                Log.D(e.Source);
                HistoryData = new HistoryData();
                return;
            }

            using (var stream = await _historyFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                lock (_thisLock)
                {
                    using (var reader = stream.AsStreamForRead())
                    {
                        var obj = _serializer.ReadObject(reader);
                        if (obj is HistoryData)
                        {
                            HistoryData = obj as HistoryData;
                        }
                        else
                        {
                            throw new SerializationException(nameof(obj) + " is not " + nameof(HistoryData));
                        }
                    }
                }
            }
        }

        public HistoryDataManager(IStorageFolder currentFolder)
        {
            _currentFolder = currentFolder;
        }
    }
}
