using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public partial class Bus
    {
        public enum Status { Driving, DuringTest, DuringRefuel, ReadyToGo }

        public Status BusStatus { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime LastTestDate { get; set; }

        public int Fuel { get; set; }

        public int Mileage { get; set; }

        public int KM { get; set; }
    }
}
