using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BL
{
    static class TravelOperator
    {
        static Thread operatorThread = null;

        private static TimeSpan CalculateSleepTime(TimeSpan from, TimeSpan to, int rate)
        {
            if (from == to) return TimeSpan.Zero;
            TimeSpan mainTime = new TimeSpan(12, 0, 0);
            TimeSpan sleepTime = mainTime - from + to + mainTime;
            sleepTime = sleepTime.Subtract(TimeSpan.FromDays(sleepTime.Days));
            sleepTime = TimeSpan.FromSeconds(sleepTime.TotalSeconds / rate);
            return sleepTime;
        }

        public static void Start()
        {
            operatorThread = new Thread(() =>
            {
                List<BO.LineTrip> lineTrips = new List<BO.LineTrip>();
                foreach (var trip in BL.dal.GetLineTrips())
                {
                    if (Simulator.Cancel) break;
                    if (trip.Frequency <= TimeSpan.Zero)
                        lineTrips.Add(new BO.LineTrip { ID = trip.ID, StartTime = trip.StartTime });
                    else
                        for (TimeSpan i = trip.StartTime; i < trip.EndTime; i = i.Add(trip.Frequency))
                        {
                            if (Simulator.Cancel) break;
                            lineTrips.Add(new BO.LineTrip { ID = trip.ID, StartTime = i });
                        }
                }

                lineTrips = lineTrips.OrderBy(trip => trip.StartTime).ToList();

                int index = 0, count = lineTrips.Count;

                var startTime = Simulator.Time;
                var rate = Simulator.Rate;
                TimeSpan minTime = CalculateSleepTime(lineTrips[0].StartTime,
                    startTime, rate);
                for (int i = 1; i < count; i++)
                {
                    var sleepTime = CalculateSleepTime(lineTrips[i].StartTime,
                        startTime, rate);
                    if (sleepTime > minTime)
                    {
                        minTime = sleepTime;
                        index = i;
                    }
                }

                bool first = true;
                while (!Simulator.Cancel)
                {
                    TimeSpan sleepTime = new TimeSpan();
                    if (first)
                        sleepTime = CalculateSleepTime(
                                Simulator.Time, lineTrips[index % count].StartTime,
                                rate);
                    else
                        sleepTime = CalculateSleepTime(
                                lineTrips[index % count].StartTime,
                                lineTrips[(index + 1) % count].StartTime, rate);

                    try { Thread.Sleep(sleepTime); }
                    catch (ThreadInterruptedException) { break; }
                    first = false;

                    Trips.StartTrip(lineTrips[index % count]);

                    index++;
                }
            });
            operatorThread.Start();
        }

        public static void Stop()
        {
            operatorThread?.Interrupt();
            operatorThread = null;
        }
    }
}
