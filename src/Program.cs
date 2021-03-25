using System;
using System.Threading;
using static HearthStoneWatcher.Config;

namespace HearthStoneWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var watcher = new LogWatcher(PATH, FILE);

            watcher.Watch();

            while(true)
            {
                if(watcher.LogLineQueue.TryDequeue(out LogLine line) && line.Method == OPTION_METHOD)
                    Console.WriteLine(line.ToString());

                if(Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.S)
                {
                    watcher.Stop();
                    break;
                }

                Thread.Sleep(100);
            }
        }
    }
}
