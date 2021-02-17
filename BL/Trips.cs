using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BL
{
    static class Trips
    {
        private class StationPanel
        {
            public int StationCode { get; private set; }
            private event Action<BO.LineTiming> updateEvent = null;
            public event Action<BO.LineTiming> UpdateEvent
            {
                add => updateEvent = value;
                remove => updateEvent -= value;
            }

            public StationPanel(int stationCode) => StationCode = stationCode;

            public void InvokeEvent(BO.LineTiming lineTiming)
                => updateEvent?.Invoke(lineTiming);

            public void ClearEvent() => updateEvent = null;
        }

        static List<Thread> trips = new List<Thread>();
        static List<StationPanel> panels = new List<StationPanel>();

        public static void StartTrip(BO.LineTrip lineTrip)
        {
            Thread trip = new Thread(() =>
            {
                try
                {
                    Random rnd = new Random();
                    #region Get Data
                    var line = BL.Instance.BuildLine(lineTrip.ID);

                    var lineStationsList = line.Stations.ToList();

                    int lastIndex = lineStationsList.Count - 1;
                    string lastStationName = lineStationsList[lastIndex].Name;

                    BO.LineTiming lineTiming = new BO.LineTiming
                    {
                        ID = line.ID,
                        StartTime = lineTrip.StartTime,
                        LineNumber = line.LineNumber,
                        LastStationName = lastStationName
                    };
                    #endregion

                    #region Calculate Average Times
                    List<TimeSpan> avgTimesBetweenStations = new List<TimeSpan>();
                    for (int i = 0; i < lineStationsList.Count && !Simulator.Cancel; i++)
                    {
                        TimeSpan totalTime = new TimeSpan(0, 0, 0);
                        for (int j = 0; j <= i; j++)
                            totalTime += lineStationsList[j].Time;
                        avgTimesBetweenStations.Add(totalTime);
                    }
                    #endregion

                    for (int i = 0; i < lineStationsList.Count && !Simulator.Cancel; i++)
                    {
                        var station = lineStationsList[i];

                        for (int j = i; j < lineStationsList.Count && !Simulator.Cancel; j++)
                            avgTimesBetweenStations[j] -= station.Time;

                        #region Update Relevant Stations
                        int count = panels.Count;
                        for (int j = 0; j < count && !Simulator.Cancel; j++)
                        {
                            StationPanel panel = panels[j];
                            int stationIndex = lineStationsList.FindIndex(x => x.StationCode == panel.StationCode);
                            if (stationIndex != -1)
                            {
                                lineTiming.ArrivalTime = avgTimesBetweenStations[stationIndex];
                                if (stationIndex > i)
                                    panel?.InvokeEvent(lineTiming.Convert<BO.LineTiming, BO.LineTiming>());
                                if (stationIndex == i)
                                {
                                    panel?.InvokeEvent(lineTiming.Convert<BO.LineTiming, BO.LineTiming>());
                                    new Thread(() =>
                                    {
                                        panel?.InvokeEvent(new BO.LineTiming { ID = -1, LineNumber = line.LineNumber });
                                        try { Thread.Sleep(5000); }
                                        catch (ThreadInterruptedException) { }
                                        panel?.InvokeEvent(new BO.LineTiming { ID = -2 });
                                    }).Start();
                                }
                            }
                        }
                        #endregion

                        #region Sleep With Late
                        double withExtraTime = station.Time.TotalSeconds;
                        if (rnd.Next(0, 2) == 1) /* late */ withExtraTime *= rnd.Next(1, 3) + rnd.NextDouble();
                        else withExtraTime *= 0.9 + rnd.NextDouble() / 10;

                        try { Thread.Sleep(TimeSpan.FromSeconds(withExtraTime / Simulator.Rate)); }
                        catch (ThreadInterruptedException) { break; }
                        #endregion
                    }
                }
                catch (Exception) { }
            });
            trip.Start();
            trips.Add(trip);
        }

        public static void StopAllTrips()
        {
            for (int i = 0; i < trips.Count; i++)
            {
                trips[i]?.Interrupt();
                trips[i] = null;
            }
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].ClearEvent();
                panels[i] = null;
            }
            trips = new List<Thread>();
            panels = new List<StationPanel>();
        }

        public static void AddStationPanel(int stationCode, Action<BO.LineTiming> action)
        {
            int index = panels.FindIndex(x => x.StationCode == stationCode);
            if (index != -1) panels[index].UpdateEvent += action;
            else
            {
                StationPanel panel = new StationPanel(stationCode);
                panel.UpdateEvent += action;
                panels.Add(panel);
            }
        }

        public static void RemoveStationPanel(int stationCode)
        {
            int index = panels.FindIndex(x => x.StationCode == stationCode);
            if (index != -1) panels.RemoveAt(index);
        }
    }
}