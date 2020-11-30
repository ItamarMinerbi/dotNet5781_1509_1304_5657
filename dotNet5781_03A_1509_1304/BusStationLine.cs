using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_02_1509_1304
{
    /// <summary>
    /// The class describes a station in single line.
    /// Distance parameter describes the distance in KM from last BusStationLine in the line.
    /// Time parameter describes the time in minutes from last BusStationLine in the line.
    /// The ctor get values about a BusStation class and the values for distance and time
    /// parameters and match them.
    /// has the interface of IComparable, compare between two StationCode variable in the base class
    /// of BusStationLine
    /// </summary>
    class BusStationLine : BusStation, IComparable<BusStationLine>
    {
        private double distance;
        private int time;

        public int Time { get => time; }
        public double Distance { get => distance; }

        public BusStationLine(string StationCode, double Latitude, double Longitude,
            double Distance, int Time, string Address = "") 
            : base(StationCode, Latitude, Longitude, Address)
        {
            time = Time;
            distance = Distance;
        }

        public int CompareTo(BusStationLine obj)
        {
            return (this.StationCode.CompareTo(obj.StationCode));
        }
    }
}