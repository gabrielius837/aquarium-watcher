using System;
using System.Threading;
using System.Threading.Tasks;
using static HearthStoneWatcher.Config;

namespace HearthStoneWatcher
{
    class Program
    {
        static int Main(string[] args)
        {
            var watcher = new LogWatcher(PATH);

            var task = watcher.Watch();

            while(task.Status is not TaskStatus.RanToCompletion)
            {
                if(!watcher.LogLineQueue.IsEmpty)
                {
                    while(watcher.LogLineQueue.TryDequeue(out LogLine line))
                    {
                        if(line.Method == OPTION_METHOD)
                            Console.WriteLine(line.ToString());
                    }
                }

                if(Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.S)
                    watcher.Stop();

                Thread.Sleep(250);
            }
            
            return 0;
        }
    }
}
