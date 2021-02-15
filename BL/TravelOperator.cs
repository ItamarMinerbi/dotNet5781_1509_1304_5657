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
        static volatile bool Cancel;
        static Thread operatorThread = null;
        static event Action<BO.LineTiming> updateEvent = null;
        public static event Action<BO.LineTiming> UpdateEvent
        {
            add => updateEvent = value;
            remove => updateEvent -= value;
        }
        public static int StationCode;

        private static TimeSpan CalculateSleepTime(TimeSpan from, TimeSpan to, int rate)
        {
            TimeSpan mainTime = new TimeSpan(12, 0, 0);
            TimeSpan sleepTime = mainTime - from + to + mainTime;
            sleepTime = sleepTime.Subtract(TimeSpan.FromDays(sleepTime.Days));
            sleepTime = TimeSpan.FromSeconds(sleepTime.TotalSeconds / rate);
            return sleepTime;
        }

        public static void Start()
        {
            Cancel = false;
            operatorThread = new Thread(() =>
            {
                List<BO.LineTrip> lineTrips = new List<BO.LineTrip>();
                foreach (var line in BL.dal.GetStationLinesInStation(StationCode))
                {
                    if (Cancel) break;
                    foreach (var trip in BL.dal.GetLineTripsByPredicate(x => x.ID == line.ID))
                    {
                        if (Cancel) break;
                        for (TimeSpan i = trip.StartTime; i < trip.EndTime; i = i.Add(trip.Frequency))
                        {
                            if (Cancel) break;
                            lineTrips.Add(new BO.LineTrip { ID = trip.ID, StartTime = i });
                        }
                    }
                }

                lineTrips = lineTrips.OrderBy(trip => trip.StartTime).ToList();

                int index = 0, count = lineTrips.Count;

                var startTime = Simulator.Time;
                var rate = Simulator.Rate;
                TimeSpan time = CalculateSleepTime(lineTrips[0].StartTime, startTime, rate);
                for (int i = 1; i < count; i++)
                {
                    var sleepTime = CalculateSleepTime(lineTrips[i].StartTime, startTime, rate);
                    if (sleepTime > time)
                    {
                        time = sleepTime;
                        index = i;
                    }
                }

                while (!Cancel)
                {
                    TimeSpan sleepTime = CalculateSleepTime(
                        lineTrips[index % count].StartTime,
                        lineTrips[(index + 1) % count].StartTime, rate);

                    try { Thread.Sleep(sleepTime); }
                    catch (ThreadInterruptedException) { break; }

                    Thread trip = new Thread(() =>
                    {
                        try
                        {
                            Random rnd = new Random();
                            
                            var line = BL.Instance.BuildLine(lineTrips[index % count].ID);
                            
                            int lastStationCode = BL.dal.RequestLine(lineTrips[index % count].ID).LastStation;
                            string lastStationName = BL.dal.RequestStation(lastStationCode).Name;
                            
                            BO.LineTiming lineTiming = new BO.LineTiming
                            {
                                ID = line.ID,
                                StartTime = lineTrips[index % count].StartTime,
                                LineNumber = line.LineNumber,
                                LastStationName = lastStationName
                            };
                           
                            var lineStationsList = line.Stations.ToList();
                            
                            List<TimeSpan> avgTimesBetweenStations = new List<TimeSpan>();

                            for (int i = 0; i < lineStationsList.Count; i++)
                            {
                                TimeSpan totalTime = new TimeSpan(0, 0, 0);
                                for (int j = 0; j < i; j++)
                                    totalTime += lineStationsList[j].Time;
                                avgTimesBetweenStations.Add(totalTime);
                            }

                            int Index = lineStationsList.FindIndex(x => x.StationCode == StationCode);
                            if (Index < 0) Index = lineStationsList.Count - 1;

                            for (int i = 0; i < lineStationsList.Count; i++)
                            {
                                var station = lineStationsList[i];

                                for (int j = i; j < lineStationsList.Count; j++)
                                {
                                    avgTimesBetweenStations[j] -= station.Time;
                                    if(j > Index) updateEvent?.Invoke(new BO.LineTiming { ID = 0, ArrivalTime = avgTimesBetweenStations[Index] });
                                }
                                try { Thread.Sleep(1000); }
                                catch (ThreadInterruptedException) { }
                            }
                        }
                        catch (ThreadInterruptedException) { }
                    });
                    trip.Name = "";
                    trip.Start();
                    index++;
                }
            });
            operatorThread.Start();
        }

        public static void Stop()
        {
            Cancel = true;
            operatorThread?.Interrupt();
            operatorThread = null;
            updateEvent = null;
        }
    }
}
