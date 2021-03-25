using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using static HearthStoneWatcher.Config;

namespace HearthStoneWatcher
{
    public class LogWatcher
    {
        private readonly LogParser _logParser;
        private readonly FileSystemWatcher _watcher;
        private bool _isBusy;
        private long _currentSize;
        private string _buffer;

        public LogWatcher(string path, string filter)
        {
            _logParser = new LogParser();
            LogLineQueue = new ConcurrentQueue<LogLine>();
            _watcher = new FileSystemWatcher(path, filter)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            _watcher.Changed += async (s, e) => await OnChanged(s, e);
            _buffer = string.Empty;
        }

        public ConcurrentQueue<LogLine> LogLineQueue { get; }

        protected Task OnChanged(object sender, FileSystemEventArgs e)
        {
            if(_isBusy) return Task.CompletedTask;

            Stop();

            var newSize = new FileInfo(e.FullPath).Length;

            if(_currentSize >= newSize) return Task.CompletedTask;

            using (var stream = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(stream))
            {
                sr.BaseStream.Seek(_currentSize, SeekOrigin.Begin);         
                var newData = _buffer + sr.ReadToEnd();         
                if (!newData.EndsWith(DELIMITER))
                {
                    if (newData.IndexOf(DELIMITER) == -1)
                    {
                        _buffer += newData;
                        newData = string.Empty;
                    }
                    else
                    {
                        var pos = newData.LastIndexOf(DELIMITER) + DELIMITER.Length;
                        _buffer = newData.Substring(pos);
                        newData = newData.Substring(0, pos);
                    }
                }

                var lines = _logParser.Parse(newData);
                foreach (var line in lines)
                {
                    LogLineQueue.Enqueue(line);
                }         
            }           
            _currentSize = newSize;
            
            Watch();

            return Task.CompletedTask;
        }

        public void Stop()
        {
            _isBusy = true;
            _watcher.EnableRaisingEvents = false;
        }

        public void Watch()
        {
            _isBusy = false;
            _watcher.EnableRaisingEvents = true;
        }


    }
}