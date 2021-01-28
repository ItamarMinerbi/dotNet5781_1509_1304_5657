using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Line
    {
        public int ID { get; set; }
        public int LineNumber { get; set; }
        public IEnumerable<DisplayStationLine> Stations { get; set; }
        public TimeSpan TotalTime { get; set; }
        public enum Areas { General, Center, Jerusalem, North }
        public Areas Area { get; set; }
        public IEnumerable<LineTrip> Trips { get; set; }
    }
}
