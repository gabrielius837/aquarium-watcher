using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static HearthStoneWatcher.Config;

namespace HearthStoneWatcher
{
    public class LogWatcher
    {
        private readonly LogParser _logParser;
        private readonly string _path;
        private long _currentSize;
        private Task _watchTask;
        private bool _run;

        public LogWatcher(string path)
        {
            _logParser = new LogParser();
            _path = path;
            LogLineQueue = new ConcurrentQueue<LogLine>();
        }

        public ConcurrentQueue<LogLine> LogLineQueue { get; }

        protected async Task WatchTask(int timeout)
        {
            while(_run)
            {
                await Task.Delay(timeout);

                if (!File.Exists(_path)) continue;

                var newSize = new FileInfo(_path).Length;

                if(_currentSize >= newSize) continue;

                using (var stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(stream))
                {
                    sr.BaseStream.Seek(_currentSize, SeekOrigin.Begin);         
                    var newData = sr.ReadToEnd();         

                    var lines = _logParser.Parse(newData);
                    foreach (var line in lines)
                    {
                        LogLineQueue.Enqueue(line);
                    }         
                }           
                _currentSize = newSize;
            }
            return;
        }

        public Task Watch(int timeout = 250)
        {
            if(_watchTask is not null && (_watchTask.Status is TaskStatus.Running || _watchTask.Status is TaskStatus.WaitingToRun))
                return _watchTask;

            _run = true;
            if (File.Exists(_path)) _currentSize = new FileInfo(_path).Length;
            return _watchTask = WatchTask(timeout);
        }

        public void Stop()
        {
            if(_watchTask == null) return;

            _run = false;
            _watchTask.Wait();
            _watchTask.Dispose();
            _watchTask = null;
            return;
        }
    }
}