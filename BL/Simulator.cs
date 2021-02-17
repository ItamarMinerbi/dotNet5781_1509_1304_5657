using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BL
{
    static class Simulator
    {
        static volatile bool cancel;
        public static bool Cancel { get => cancel; }

        static Thread clockThread = null;
        static event Action<TimeSpan> tickEvent = null;
        public static event Action<TimeSpan> TickEvent
        {
            add => tickEvent = value;
            remove => tickEvent -= value;
        }
        public static TimeSpan Time { get; private set; }
        public static int Rate { get; private set; }

        public static void Start(TimeSpan startTime, int rate)
        {
            cancel = false;
            clockThread = new Thread(() =>
            {
                Rate = rate;
                Time = startTime;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                while (!cancel)
                {
                    Time = startTime + TimeSpan.FromTicks(stopwatch.ElapsedTicks * rate);
                    tickEvent?.Invoke(startTime + TimeSpan.FromTicks(stopwatch.ElapsedTicks * rate));
                    try { Thread.Sleep(1000 / rate); }
                    catch (ThreadInterruptedException) { }
                }
                stopwatch.Stop();
            });
            clockThread.Start();
        }

        public static void Stop()
        {
            cancel = true;
            clockThread?.Interrupt();
            clockThread = null;
            tickEvent = null;
        }
    }
}