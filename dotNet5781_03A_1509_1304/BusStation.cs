using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_02_1509_1304
{
    /// <summary>
    /// The base class of Line and BusStationLine that need access for some variables.
    /// all private variables has getters.
    /// ToString funcions is override as we asked in the exercise.
    /// The ctor get the values and match them to the private variables.
    /// </summary>
    class BusStation
    {
        private string stationCode;
        private string stationAddress;
        private double latitude;
        private double longitude;

        public string StationCode { get => stationCode; }
        public string StationAddress { get => stationAddress; }
        public double Latitude { get => latitude; }
        public double Longitude { get => longitude; }

        protected BusStation()
        {
            stationCode = "000000";
            stationAddress = "";
            latitude = 32.0;
            longitude = 34.5;
        }
        public BusStation(string StationCode, double Latitude, double Longtitude, string address = "")
        {
            stationCode = StationCode;
            latitude = Latitude;
            longitude = Longtitude;
            stationAddress = address;
        }

        public override string ToString()
        {
            return $"Bus Station Code: {stationCode}, {latitude}°N {longitude}°E";
        }
    }
}