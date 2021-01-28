using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class AdjStations
    {
        public int StationCode1 { get; set; }
        public int StationCode2 { get; set; }
        public string Station1Name { get; set; }
        public string Station2Name { get; set; }
        public double Distance { get; set; }
        public TimeSpan Time { get; set; }
    }
}