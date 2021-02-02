using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class DisplayCounts
    {
        public int UsersCount { get; set; }
        public int StationsCount { get; set; }
        public int LinesCount { get; set; }
        public int StationLinesCount { get; set; }
        public int AdjStationsCount { get; set; }
        public int LineTripsCount { get; set; }
        public string TotalSpace { get; set; }
    }
}
