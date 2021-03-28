using System;
using System.IO;
using System.Threading;
using Xunit;

namespace HearthStoneWatcher.Tests
{
    public class LogWatcherTests : IDisposable
    {
        const string INITIAL = "Data/watch_initial.log";
        const string INPUT = "Data/watch_input.log";
        const string TEST_FILE = "Test.log";
        private readonly LogWatcher _watcher;
        private readonly LogParser _parser;

        public LogWatcherTests()
        {
            File.Copy(INITIAL, TEST_FILE, true);
            _watcher = new LogWatcher(TEST_FILE);
            _parser = new LogParser();
        }

        public void Dispose()
        {
            _watcher.Stop();
            if(File.Exists(TEST_FILE)) File.Delete(TEST_FILE);
        }

        [Fact]
        public void Watch_ExpectedTextAppeared()
        {
            var timeout = 250;
            _watcher.Watch(timeout);
            var inputText = File.ReadAllText(INPUT);
            File.AppendAllText(TEST_FILE, inputText);
            var expectedLogLines = _parser.Parse(inputText);

            Thread.Sleep(2 * timeout);
            Assert.True(_watcher.LogLineQueue.Count == expectedLogLines.Length, $"Log line count differs: expected - {expectedLogLines.Length}, observed - {_watcher.LogLineQueue.Count}");
        }
    }
}