using System;
using System.Threading;

namespace Server
{
    class Program
    {

        public const int TICKS_PER_SEC = 30;
        public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;

        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(4, 26950);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {TICKS_PER_SEC} ricks per second");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while(_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(MS_PER_TICK);

                    if(_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
