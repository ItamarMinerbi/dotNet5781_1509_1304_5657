using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class LineTiming
    {
        public int ID { get; set; }
        public int License { get; set; }
        public int LineID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan ActualStartTime { get; set; }
        public int LastStation { get; set; }
        public TimeSpan LastStationTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
    }
}
