using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_02_1509_1304
{
    /// <summary>
    /// This class describes a line. The busLine parameter contains the number of the line
    /// the stations list contains all BusStationLine in the line, has a getter.
    /// The class has IComparable interface that compere between two TotalTime function of two
    /// Line classes.
    /// </summary>
    class Line : BusStation, IComparable<Line>
    {
        public Line(string BusLine, BusStationLine FirstStation, 
            BusStationLine LastStation, Areas area)
        {
            Area = area;
            busLine = BusLine;
            firstStation = FirstStation;
            lastStation = LastStation;
        }
        public Line(string BusLine, List<BusStationLine> busStations, Areas area)
        {
            busLine = BusLine;
            stations = busStations;
            Area = area;
            firstStation = busStations[0];
            lastStation = busStations[busStations.Count - 1];
        }

        public enum Areas { General, North, South, Center, Jerusalem }
        private Areas Area;
        public string TheArea
        {
            get => Area.ToString();
        }

        private string busLine;
        public string BusLine { get => busLine; }

        private BusStationLine firstStation, lastStation;

        private List<BusStationLine> stations = new List<BusStationLine>();
        public List<BusStationLine> Stations { get => stations; }

        public void AddStation(BusStationLine station) 
        {
            if (!IsStationExist(station.StationCode))
            {
                stations.Add(station);
                firstStation = stations[0];
                lastStation = stations[stations.Count - 1];
            }
            else
                throw new ArgumentException("Station already exist!");
        }
        public void RemoveStation(string stationCode) 
        {
            BusStationLine station = stations.Find(i => i.StationCode == stationCode);
            if (IsStationExist(stationCode))
            {
                stations.Remove(station);
                firstStation = stations[0];
                lastStation = stations[stations.Count - 1];
            }
            else
                throw new ArgumentException("Station was not found!");
        }
        public bool IsStationExist(string stationCode) 
        {
            return stations.Exists(i => i.StationCode == stationCode);
        }
        public double Distance(string stationCode1, string stationCode2) 
        {
            BusStationLine station1 = stations.Find(i => i.StationCode == stationCode1);
            BusStationLine station2 = stations.Find(i => i.StationCode == stationCode2);
            double totalDistance = 0.0;
            if (IsStationExist(stationCode1) && IsStationExist(stationCode2))
            {
                int Index1 = stations.FindIndex(i => i.StationCode == station1.StationCode);
                int Index2 = stations.FindIndex(i => i.StationCode == station1.StationCode);
                for (int i = Math.Min(Index1, Index2); i <= Math.Max(Index1, Index2); i++)
                {
                    totalDistance += stations[i].Distance;
                }
            }
            else
                throw new ArgumentException("One of the stations was not found!");
            return totalDistance;
        }
        public int TotalTime(string stationCode1, string stationCode2) 
        {
            BusStationLine station1 = stations.Find(i => i.StationCode == stationCode1);
            BusStationLine station2 = stations.Find(i => i.StationCode == stationCode2);
            int totalTime = 0;
            if (IsStationExist(stationCode1) && IsStationExist(stationCode2))
            {
                int Index1 = stations.FindIndex(i => i.StationCode == station1.StationCode);
                int Index2 = stations.FindIndex(i => i.StationCode == station1.StationCode);
                for (int i = Math.Min(Index1, Index2); i <= Math.Max(Index1, Index2); i++)
                {
                    totalTime += stations[i].Time;
                }
            }
            else
                throw new ArgumentException("One of the stations was not found!");
            return totalTime;
        }
        public Line SubLine(string stationCode1, string stationCode2) 
        {
            BusStationLine station1 = stations.Find(i => i.StationCode == stationCode1);
            BusStationLine station2 = stations.Find(i => i.StationCode == stationCode2);
            Line result = null;
            if (IsStationExist(stationCode1) && IsStationExist(stationCode2))
            {
                int Index1 = stations.FindIndex(i => i.StationCode == station1.StationCode);
                int Index2 = stations.FindIndex(i => i.StationCode == station1.StationCode);
                result = new Line(busLine, stations[Math.Min(Index1, Index2)], stations[Math.Max(Index1, Index2)], Area);
                for (int i = Math.Min(Index1, Index2); i <= Math.Max(Index1, Index2); i++)
                {
                    result.AddStation(stations[i]);
                }
            }
            else
                throw new ArgumentException("One of the stations was not found!");
            return result;
        }
        public int CompareTo(Line other)
        {
            int time1 = TotalTime(firstStation.StationCode, lastStation.StationCode), 
                time2 = TotalTime(other.firstStation.StationCode, other.lastStation.StationCode);
            if (time1 > time2)
                return 1;
            else if (time1 < time2)
                return -1;
            else
                return 0;
        }
        public override string ToString()
        {
            string stations1 = "", stations2 = "";
            for (int i = 0, j = stations.Count - 1; i < stations.Count; i++, j--)
            {
                stations1 += stations[i].StationCode + ((j != 0) ? ", " : "");
                stations2 += stations[j].StationCode + ((j != 0) ? ", " : "");
            }
            return $"   Bus line: {busLine},\n   {Area},\n   List to: {stations1}.\n   List from: {stations2}";
        }
    }
}